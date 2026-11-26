// ---------------------------------------------------------------------------
// 排序与查找原语（ILCuda）
//
// 面向"每个基因一段"的对照细胞排序值：段长可达 18000（对照细胞数），
// 远超单个 block 的共享内存容量，因此不使用双调排序，而是分段 LSD 基数排序。
//
// 浮点排序走"位模式变换"：
//   * 正数：置位符号位（u | 0x80000000），使无符号比较等价于数值比较
//   * 负数：整体按位取反（~u）
//   * -0.0 归一到 +0.0，NaN 统一映射到全 1 键（沉到段尾）
// 这样基数排序的无符号桶序与数值升序严格一致，且不依赖任何浮点比较。
//
// 关键点：LSD 基数排序的每一趟必须是**稳定**的，否则上一趟建立起来的
// 低位顺序会被破坏。这里用"warp 投票 + 逐 warp 计数"实现稳定散播：
//   rank = 同 warp 内同桶且线程序号更小的个数（ballot + popcount）
//   偏移 = 桶基址 + 前面所有 warp 在该桶上的计数 + rank
// 逐 tile 推进，因此散播顺序就是原始下标顺序。
//
// 宏前缀 ILCUDA_SORT_，符号前缀 ilcudaSeg / ilcudaSort。
// ---------------------------------------------------------------------------

#define ILCUDA_SORT_BUCKETS 256
#define ILCUDA_SORT_BLOCK 256
#define ILCUDA_SORT_WARPS (ILCUDA_SORT_BLOCK / 32)
// 最后一个槽位专门接收"本 tile 越界"的线程，避免污染真实桶的基址
#define ILCUDA_SORT_SLOTS (ILCUDA_SORT_BUCKETS + 1)

// ---------------------------------------------------------------------------
// 键变换：浮点 <-> 可无符号比较的 32 / 64 位键
// ---------------------------------------------------------------------------

__device__ __forceinline__ unsigned int ilcudaSegKeyF32(float v)
{
    // NaN 统一沉到段尾（放在键序的最后）
    if (!(v == v)) {
        return 0xFFFFFFFFu;
    }

    unsigned int u = __float_as_uint(v);

    // -0.0 与 +0.0 视为同一个值：统一成 +0.0 的位模式
    if (u == 0x80000000u) {
        u = 0u;
    }

    return (u & 0x80000000u) ? (~u) : (u | 0x80000000u);
}

__device__ __forceinline__ float ilcudaSegValueF32(unsigned int k)
{
    // 逆变换：高位为 1 说明原值是正数（取回符号位清零），否则原值是负数（取反）
    unsigned int u = (k & 0x80000000u) ? (k & 0x7FFFFFFFu) : (~k);

    return __uint_as_float(u);
}

__device__ __forceinline__ unsigned long long ilcudaSegKeyF64(double v)
{
    if (!(v == v)) {
        return 0xFFFFFFFFFFFFFFFFull;
    }

    unsigned long long u = __double_as_longlong(v);

    if (u == 0x8000000000000000ull) {
        u = 0ull;
    }

    return (u & 0x8000000000000000ull) ? (~u) : (u | 0x8000000000000000ull);
}

__device__ __forceinline__ double ilcudaSegValueF64(unsigned long long k)
{
    unsigned long long u = (k & 0x8000000000000000ull) ? (k & 0x7FFFFFFFFFFFFFFFull) : (~k);

    return __longlong_as_double(u);
}

// ---------------------------------------------------------------------------
// 分段基数排序（值，稳定）
//
// 启动形状：grid = (segmentCount, 1)，block = (256, 1) —— block 必须固定 256，
// 因为共享内存里按 ILCUDA_SORT_WARPS = 8 个 warp 分配了计数表。
//
// 一趟只处理一个字节（8 bit），由主机侧循环 shift = 0, 8, 16, 24（double 到 56）。
// 主机侧用两个缓冲乒乓，因此相邻两趟之间不需要额外的拷贝。
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaSortSegmentsF32Kernel(const float* __restrict__ src,
                                                       float* __restrict__ dst,
                                                       int segLen,
                                                       int shift)
{
    __shared__ int hist[ILCUDA_SORT_BUCKETS];
    __shared__ int base[ILCUDA_SORT_BUCKETS];
    __shared__ int warpCnt[ILCUDA_SORT_WARPS][ILCUDA_SORT_SLOTS];

    const int tid = threadIdx.x;
    const int lane = tid & 31;
    const int warp = tid >> 5;
    const size_t segBase = (size_t)blockIdx.x * (size_t)segLen;
    const unsigned int lowerLanes = (1u << lane) - 1u;

    // 1) 直方图
    for (int b = tid; b < ILCUDA_SORT_BUCKETS; b += blockDim.x) {
        hist[b] = 0;
    }

    __syncthreads();

    for (int i = tid; i < segLen; i += blockDim.x) {
        unsigned int k = ilcudaSegKeyF32(src[segBase + i]);

        atomicAdd(&hist[(k >> shift) & 0xFFu], 1);
    }

    __syncthreads();

    // 2) 桶基址（独占前缀和）+ 清空逐 warp 计数
    if (tid == 0) {
        int acc = 0;

        for (int b = 0; b < ILCUDA_SORT_BUCKETS; ++b) {
            base[b] = acc;
            acc += hist[b];
        }
    }

    for (int s = tid; s < ILCUDA_SORT_WARPS * ILCUDA_SORT_SLOTS; s += blockDim.x) {
        (&warpCnt[0][0])[s] = 0;
    }

    __syncthreads();

    // 3) 逐 tile 稳定散播
    for (int tile = 0; tile < segLen; tile += blockDim.x) {
        const int i = tile + tid;
        const bool valid = i < segLen;
        const float v = valid ? src[segBase + i] : 0.0f;
        const int slot = valid ? (int)((ilcudaSegKeyF32(v) >> shift) & 0xFFu) : ILCUDA_SORT_BUCKETS;

        // warp 内的同槽位名次：本 lane 之前有多少 lane 落在同一个槽
        // __match_any_sync 返回"与本 lane 取值相同的 lane 集合"，一次指令完成
        const unsigned int poll = __match_any_sync(0xFFFFFFFFu, slot);
        const int rank = __popc(poll & lowerLanes);

        if (rank == 0) {
            warpCnt[warp][slot] = __popc(poll);
        }

        __syncthreads();

        if (valid) {
            int offset = base[slot];

            for (int w = 0; w < warp; ++w) {
                offset += warpCnt[w][slot];
            }

            dst[segBase + offset + rank] = v;
        }

        __syncthreads();

        // 4) 推进桶基址，并清空本 tile 的 warp 计数
        for (int b = tid; b < ILCUDA_SORT_BUCKETS; b += blockDim.x) {
            int add = 0;

            for (int w = 0; w < ILCUDA_SORT_WARPS; ++w) {
                add += warpCnt[w][b];
                warpCnt[w][b] = 0;
            }

            base[b] += add;
        }

        if (tid < ILCUDA_SORT_WARPS) {
            warpCnt[tid][ILCUDA_SORT_BUCKETS] = 0;
        }

        __syncthreads();
    }
}

extern "C" __global__ void ilcudaSortSegmentsF64Kernel(const double* __restrict__ src,
                                                       double* __restrict__ dst,
                                                       int segLen,
                                                       int shift)
{
    __shared__ int hist[ILCUDA_SORT_BUCKETS];
    __shared__ int base[ILCUDA_SORT_BUCKETS];
    __shared__ int warpCnt[ILCUDA_SORT_WARPS][ILCUDA_SORT_SLOTS];

    const int tid = threadIdx.x;
    const int lane = tid & 31;
    const int warp = tid >> 5;
    const size_t segBase = (size_t)blockIdx.x * (size_t)segLen;
    const unsigned int lowerLanes = (1u << lane) - 1u;

    for (int b = tid; b < ILCUDA_SORT_BUCKETS; b += blockDim.x) {
        hist[b] = 0;
    }

    __syncthreads();

    for (int i = tid; i < segLen; i += blockDim.x) {
        unsigned long long k = ilcudaSegKeyF64(src[segBase + i]);

        atomicAdd(&hist[(k >> shift) & 0xFFu], 1);
    }

    __syncthreads();

    if (tid == 0) {
        int acc = 0;

        for (int b = 0; b < ILCUDA_SORT_BUCKETS; ++b) {
            base[b] = acc;
            acc += hist[b];
        }
    }

    for (int s = tid; s < ILCUDA_SORT_WARPS * ILCUDA_SORT_SLOTS; s += blockDim.x) {
        (&warpCnt[0][0])[s] = 0;
    }

    __syncthreads();

    for (int tile = 0; tile < segLen; tile += blockDim.x) {
        const int i = tile + tid;
        const bool valid = i < segLen;
        const double v = valid ? src[segBase + i] : 0.0;
        const int slot = valid ? (int)((ilcudaSegKeyF64(v) >> shift) & 0xFFu) : ILCUDA_SORT_BUCKETS;

        const unsigned int poll = __match_any_sync(0xFFFFFFFFu, slot);
        const int rank = __popc(poll & lowerLanes);

        if (rank == 0) {
            warpCnt[warp][slot] = __popc(poll);
        }

        __syncthreads();

        if (valid) {
            int offset = base[slot];

            for (int w = 0; w < warp; ++w) {
                offset += warpCnt[w][slot];
            }

            dst[segBase + offset + rank] = v;
        }

        __syncthreads();

        for (int b = tid; b < ILCUDA_SORT_BUCKETS; b += blockDim.x) {
            int add = 0;

            for (int w = 0; w < ILCUDA_SORT_WARPS; ++w) {
                add += warpCnt[w][b];
                warpCnt[w][b] = 0;
            }

            base[b] += add;
        }

        if (tid < ILCUDA_SORT_WARPS) {
            warpCnt[tid][ILCUDA_SORT_BUCKETS] = 0;
        }

        __syncthreads();
    }
}

// ---------------------------------------------------------------------------
// 批量二分查找：每个查询值给出它在排序段内的 lower / upper bound
//
// 启动形状：grid = (segmentCount, 1)，block = (256, 1)
// queries / lower / upper 都按 [segmentCount, queryPerSegment] 连续存放。
// 由于数组已升序且 NaN 在段尾，这里直接用浮点比较即可。
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaSegBoundsF32Kernel(const float* __restrict__ sorted,
                                                    const float* __restrict__ queries,
                                                    int* __restrict__ lower,
                                                    int* __restrict__ upper,
                                                    int segLen,
                                                    int queryPerSegment)
{
    const int seg = blockIdx.x;
    const float* s = sorted + (size_t)seg * (size_t)segLen;
    const size_t qbase = (size_t)seg * (size_t)queryPerSegment;

    for (int q = threadIdx.x; q < queryPerSegment; q += blockDim.x) {
        float v = queries[qbase + q];

        int lo = 0;
        int hi = segLen;

        while (lo < hi) {
            int mid = (lo + hi) >> 1;

            if (s[mid] < v) {
                lo = mid + 1;
            } else {
                hi = mid;
            }
        }

        lower[qbase + q] = lo;

        lo = 0;
        hi = segLen;

        while (lo < hi) {
            int mid = (lo + hi) >> 1;

            if (s[mid] <= v) {
                lo = mid + 1;
            } else {
                hi = mid;
            }
        }

        upper[qbase + q] = lo;
    }
}

extern "C" __global__ void ilcudaSegBoundsF64Kernel(const double* __restrict__ sorted,
                                                    const double* __restrict__ queries,
                                                    int* __restrict__ lower,
                                                    int* __restrict__ upper,
                                                    int segLen,
                                                    int queryPerSegment)
{
    const int seg = blockIdx.x;
    const double* s = sorted + (size_t)seg * (size_t)segLen;
    const size_t qbase = (size_t)seg * (size_t)queryPerSegment;

    for (int q = threadIdx.x; q < queryPerSegment; q += blockDim.x) {
        double v = queries[qbase + q];

        int lo = 0;
        int hi = segLen;

        while (lo < hi) {
            int mid = (lo + hi) >> 1;

            if (s[mid] < v) {
                lo = mid + 1;
            } else {
                hi = mid;
            }
        }

        lower[qbase + q] = lo;

        lo = 0;
        hi = segLen;

        while (lo < hi) {
            int mid = (lo + hi) >> 1;

            if (s[mid] <= v) {
                lo = mid + 1;
            } else {
                hi = mid;
            }
        }

        upper[qbase + q] = lo;
    }
}

// ---------------------------------------------------------------------------
// 单段第 k 名阈值（基数选择）
//
// 启动形状：grid = (1, 1)，block = (256, 1)（可用到 1024）
// k 是"降序名次"（1 基）：k = 1 表示最大值。等价于升序名次 rank = n - k。
// 从最高字节开始逐字节收窄候选集合，最后一趟结束得到精确的键。
//
// 选择只需要计数，不需要稳定，因此这里用普通的原子直方图。
// 只用单段：HVG / 计划层的前 K 名筛选都作用在"基因轴"这一个段上；
// 多段场景由主机侧循环调用。
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaSegSelectF32Kernel(const float* __restrict__ keys,
                                                    int n,
                                                    int k,
                                                    float* __restrict__ threshold)
{
    __shared__ int hist[ILCUDA_SORT_BUCKETS];
    __shared__ int chosen;

    const int tid = threadIdx.x;
    const int nth = blockDim.x;

    if (n <= 0) {
        if (tid == 0) {
            threshold[0] = 0.0f;
        }

        return;
    }

    if (k < 1) {
        k = 1;
    }

    if (k > n) {
        k = n;
    }

    int rank = n - k;
    unsigned int prefix = 0u;

    for (int pass = 3; pass >= 0; --pass) {
        const int shift = pass << 3;

        for (int b = tid; b < ILCUDA_SORT_BUCKETS; b += nth) {
            hist[b] = 0;
        }

        __syncthreads();

        if (pass == 3) {
            for (int i = tid; i < n; i += nth) {
                unsigned int key = ilcudaSegKeyF32(keys[i]);

                atomicAdd(&hist[(key >> shift) & 0xFFu], 1);
            }
        } else {
            const int knownBits = shift + 8;
            const unsigned int known = prefix >> knownBits;

            for (int i = tid; i < n; i += nth) {
                unsigned int key = ilcudaSegKeyF32(keys[i]);

                if ((key >> knownBits) == known) {
                    atomicAdd(&hist[(key >> shift) & 0xFFu], 1);
                }
            }
        }

        __syncthreads();

        if (tid == 0) {
            int acc = 0;
            int sel = ILCUDA_SORT_BUCKETS - 1;

            for (int b = 0; b < ILCUDA_SORT_BUCKETS; ++b) {
                if (rank < acc + hist[b]) {
                    sel = b;
                    rank -= acc;

                    break;
                }

                acc += hist[b];
            }

            chosen = sel;
        }

        __syncthreads();

        prefix |= ((unsigned int)chosen) << shift;
    }

    if (tid == 0) {
        threshold[0] = ilcudaSegValueF32(prefix);
    }
}

// ---------------------------------------------------------------------------
// 按阈值收集下标
//
// 先收集严格大于阈值的下标，再用"等于阈值"的下标补足到 k；
// 两趟都按下标升序扫描，因此结果是确定的。
// 单线程顺序扫描：段长在万级时约 2 万步，代价可接受且完全可复现。
//
// 注：这里只保证"集合"正确；最终展示顺序由主机侧的排序统一收口
// （值降序、值相同按下标升序）。
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaSegCollectF32Kernel(const float* __restrict__ keys,
                                                     int n,
                                                     float threshold,
                                                     int k,
                                                     int* __restrict__ indices,
                                                     int* __restrict__ count)
{
    if (blockIdx.x != 0 || threadIdx.x != 0) {
        return;
    }

    int c = 0;

    for (int i = 0; i < n && c < k; ++i) {
        if (keys[i] > threshold) {
            indices[c++] = i;
        }
    }

    for (int i = 0; i < n && c < k; ++i) {
        if (keys[i] == threshold) {
            indices[c++] = i;
        }
    }

    count[0] = c;
}

// ---------------------------------------------------------------------------
// 逐段并列和 Σ(t³ - t)
//
// 启动形状：grid = (segmentCount, 1)，block = (1, 1)（只用到线程 0）
// 排序后单趟游程扫描；NaN 与任何值都不相等，因此每个 NaN 自成一段（按单元素处理）。
// 段与段之间天然并行：每段耗时约 2 * segLen 次连续访存。
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaSegTieSumF32Kernel(const float* __restrict__ sorted,
                                                    double* __restrict__ out,
                                                    int segLen)
{
    if (threadIdx.x != 0) {
        return;
    }

    const size_t base = (size_t)blockIdx.x * (size_t)segLen;
    double acc = 0.0;
    int i = 0;

    while (i < segLen) {
        float v = sorted[base + i];
        int j = i + 1;

        while (j < segLen && sorted[base + j] == v) {
            ++j;
        }

        double t = (double)(j - i);

        acc += t * t * t - t;
        i = j;
    }

    out[blockIdx.x] = acc;
}

extern "C" __global__ void ilcudaSegTieSumF64Kernel(const double* __restrict__ sorted,
                                                    double* __restrict__ out,
                                                    int segLen)
{
    if (threadIdx.x != 0) {
        return;
    }

    const size_t base = (size_t)blockIdx.x * (size_t)segLen;
    double acc = 0.0;
    int i = 0;

    while (i < segLen) {
        double v = sorted[base + i];
        int j = i + 1;

        while (j < segLen && sorted[base + j] == v) {
            ++j;
        }

        double t = (double)(j - i);

        acc += t * t * t - t;
        i = j;
    }

    out[blockIdx.x] = acc;
}
