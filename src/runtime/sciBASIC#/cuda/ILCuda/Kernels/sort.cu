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
// 数值口径：段内"值相同"的元素在排序后不可区分，因此散播阶段的原子偏移
// 不会影响输出的有序数组（多重集合唯一）。需要下标的场合（TopK）另走
// 阈值选择 + 按下标稳定收集，结果确定。
//
// 宏前缀 ILCUDA_SORT_，符号前缀 ilcudaSeg / ilcudaSort。
// ---------------------------------------------------------------------------

#define ILCUDA_SORT_BUCKETS 256
#define ILCUDA_SORT_DEFAULT_BLOCK 256

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
// 分段基数排序（值）
//
// 启动形状：grid = (segmentCount, 1)，block = (256, 1)
// 每段一个 block：先统计 256 桶直方图，再转独占前缀和，最后按桶散播。
// 一趟只处理一个字节（8 bit），由主机侧循环 shift = 0, 8, 16, 24（double 到 56）。
// 主机侧用两个缓冲乒乓，因此相邻两趟之间不需要额外的拷贝。
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaSortSegmentsF32Kernel(const float* __restrict__ src,
                                                       float* __restrict__ dst,
                                                       int segLen,
                                                       int shift)
{
    __shared__ int hist[ILCUDA_SORT_BUCKETS];

    const int tid = threadIdx.x;
    const int nth = blockDim.x;
    const size_t base = (size_t)blockIdx.x * (size_t)segLen;

    for (int b = tid; b < ILCUDA_SORT_BUCKETS; b += nth) {
        hist[b] = 0;
    }

    __syncthreads();

    for (int i = tid; i < segLen; i += nth) {
        unsigned int k = ilcudaSegKeyF32(src[base + i]);

        atomicAdd(&hist[(k >> shift) & 0xFF], 1);
    }

    __syncthreads();

    // 独占前缀和：256 项，由单线程完成（远小于一次全局访存的开销）
    if (tid == 0) {
        int acc = 0;

        for (int b = 0; b < ILCUDA_SORT_BUCKETS; ++b) {
            int count = hist[b];

            hist[b] = acc;
            acc += count;
        }
    }

    __syncthreads();

    for (int i = tid; i < segLen; i += nth) {
        float v = src[base + i];
        unsigned int k = ilcudaSegKeyF32(v);
        int pos = atomicAdd(&hist[(k >> shift) & 0xFF], 1);

        dst[base + pos] = v;
    }
}

extern "C" __global__ void ilcudaSortSegmentsF64Kernel(const double* __restrict__ src,
                                                       double* __restrict__ dst,
                                                       int segLen,
                                                       int shift)
{
    __shared__ int hist[ILCUDA_SORT_BUCKETS];

    const int tid = threadIdx.x;
    const int nth = blockDim.x;
    const size_t base = (size_t)blockIdx.x * (size_t)segLen;

    for (int b = tid; b < ILCUDA_SORT_BUCKETS; b += nth) {
        hist[b] = 0;
    }

    __syncthreads();

    for (int i = tid; i < segLen; i += nth) {
        unsigned long long k = ilcudaSegKeyF64(src[base + i]);

        atomicAdd(&hist[(k >> shift) & 0xFF], 1);
    }

    __syncthreads();

    if (tid == 0) {
        int acc = 0;

        for (int b = 0; b < ILCUDA_SORT_BUCKETS; ++b) {
            int count = hist[b];

            hist[b] = acc;
            acc += count;
        }
    }

    __syncthreads();

    for (int i = tid; i < segLen; i += nth) {
        double v = src[base + i];
        unsigned long long k = ilcudaSegKeyF64(v);
        int pos = atomicAdd(&hist[(k >> shift) & 0xFF], 1);

        dst[base + pos] = v;
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

                atomicAdd(&hist[(key >> shift) & 0xFF], 1);
            }
        } else {
            const unsigned int known = prefix >> (shift + 8);
            const int knownBits = shift + 8;

            for (int i = tid; i < n; i += nth) {
                unsigned int key = ilcudaSegKeyF32(keys[i]);

                if ((key >> knownBits) == known) {
                    atomicAdd(&hist[(key >> shift) & 0xFF], 1);
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
// 按阈值收集下标（降序前 k 名，按下标稳定）
//
// 先收集严格大于阈值的下标，再用"等于阈值"的下标补足到 k。
// 单线程顺序扫描：段长在万级时约 2 万步，且结果完全确定。
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
