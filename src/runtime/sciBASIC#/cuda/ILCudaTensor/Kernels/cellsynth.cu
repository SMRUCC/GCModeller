// ---------------------------------------------------------------------------
// 单细胞计数合成融合核（ILCudaTensor / CellularAI）
//
// 每个基因一个 block，逐细胞状态放在共享内存里：
//   * tcSynthCopulaQuantile：Gaussian copula 秩混合 → (i+0.5)/n 分位与降序名次
//   * tcSynthQuantileMass  ：分位锚点取值 + 整数质量守恒
//   * tcSynthWilcoxonZ     ：每细胞 CPM（float32 栅格）、段内排序、tie 项、
//                            合并方差、秩和 z
//   * tcSynthShiftBisect   ：逐基因平移二分（核内顺序迭代）
//   * tcSynthBurst         ：低表达基因按目标库大小随机散布
//
// 段内排序用共享内存双调排序（n_cells = 400 → 512 槽位），
// 因此本文件自带排序与随机数装置，不依赖 ILCuda 的 sort.cu / rng.cu
// （两个程序集各自编译，跨文件 device 函数依赖拼接顺序，容易失效）。
//
// 秩和统计严格按"游程"累计：同一个取值只贡献一次
// U += runLength * (lo + 0.5 * (hi - lo))，与上游的并列口径一致。
//
// 宏前缀 TCSYNTH_，符号前缀 tcSynth。
// ---------------------------------------------------------------------------

#define TCSYNTH_BLOCK 256
#define TCSYNTH_MAX_N 1024          // 单基因细胞数上限（超出请由主机侧分批）
#define TCSYNTH_PAD 1.0E+300        // 排序填充值（远大于任何 CPM）
#define TCSYNTH_GOLDEN 0x9E3779B97F4A7C15ull

// ---------------------------------------------------------------------------
// 基础装置：双调排序（按 (key, index) 升序，确定性）
// ---------------------------------------------------------------------------

__device__ __forceinline__ void tcsCompareSwap(double* keys, int* idx, int i, int j, int dir)
{
    const double a = keys[i];
    const double b = keys[j];
    bool take;   // 是否交换

    if (a > b) {
        take = true;
    } else if (a < b) {
        take = false;
    } else {
        take = idx[i] > idx[j];
    }

    // dir = 1 升序，dir = 0 降序
    const bool swap = dir ? take : (!take);

    if (swap && !(a == b && idx[i] == idx[j])) {
        keys[i] = b;
        keys[j] = a;

        const int ti = idx[i];

        idx[i] = idx[j];
        idx[j] = ti;
    }
}

//! 共享内存双调排序：keys / idx 长度必须是 ncap（2 的幂）
__device__ __forceinline__ void tcsBitonicSort(double* keys, int* idx, int ncap, int nth, int tid)
{
    for (int k = 2; k <= ncap; k <<= 1) {
        for (int j = k >> 1; j > 0; j >>= 1) {
            for (int i = tid; i < ncap; i += nth) {
                const int partner = i ^ j;

                if (partner > i) {
                    const int dir = ((i & k) == 0) ? 1 : 0;

                    tcsCompareSwap(keys, idx, i, partner, dir);
                }
            }

            __syncthreads();
        }
    }
}

__device__ __forceinline__ int tcsCapacity(int n)
{
    int ncap = 1;

    while (ncap < n) {
        ncap <<= 1;
    }

    return ncap;
}

// ---------------------------------------------------------------------------
// 随机数（与 ILCuda 的 rng.cu 同一套 SplitMix64 + PCG64 输出函数，
// 这里为了不跨程序集依赖而自带一份）
// ---------------------------------------------------------------------------

__device__ __forceinline__ unsigned long long tcsSplitMix64(unsigned long long x)
{
    x += TCSYNTH_GOLDEN;

    unsigned long long z = x;

    z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9ull;
    z = (z ^ (z >> 27)) * 0x94D049BB133111EBull;

    return z ^ (z >> 31);
}

//! 与 ILCuda 的 Pcg64.Uniform(seed, subsequence: 0, counter) 逐位一致
__device__ __forceinline__ double tcsUniform(unsigned long long seed, unsigned long long counter)
{
    const unsigned long long h = tcsSplitMix64(seed);
    unsigned long long l = tcsSplitMix64(h ^ counter);
    const unsigned long long inc = 1ull;

    const unsigned long long p0lo = l * 0x4385DF649FCCF645ull;
    const unsigned long long p0hi = __umul64hi(l, 0x4385DF649FCCF645ull);
    const unsigned long long p1lo = l * 0x2360ED051FC65DA4ull;
    const unsigned long long p2lo = h * 0x4385DF649FCCF645ull;

    unsigned long long hi = p0hi + p1lo + p2lo;
    unsigned long long lo = p0lo + inc;

    if (lo < p0lo) {
        hi += 1ull;
    }

    const unsigned int rot = (unsigned int)(hi >> 58);
    const unsigned long long x = hi ^ lo;
    const unsigned long long out = (x >> rot) | (x << ((64u - rot) & 63u));

    return (double)(out >> 11) * (1.0 / 9007199254740992.0);
}

// ---------------------------------------------------------------------------
// 秩和 z 的核心（共享内存里的 CPM 已按升序排好）
//
// 返回块内归约后的 (U, 合并 tie 校正量)，通过共享缓冲区交换。
// ---------------------------------------------------------------------------

__device__ void tcsRankSum(const double* sortedCpm,
                           int n,
                           const float* ctrlRow,
                           int m,
                           double* partial,
                           int nth,
                           int tid,
                           double* uOut,
                           double* tieOut)
{
    double u = 0.0;
    double adjust = 0.0;

    for (int c = tid; c < n; c += nth) {
        const float v = (float)sortedCpm[c];

        // 只处理游程起点，保证同一个取值只累计一次
        if (c > 0 && (float)sortedCpm[c - 1] == v) {
            continue;
        }

        int lo = 0;
        int hi = m;

        while (lo < hi) {
            const int mid = (lo + hi) >> 1;

            if (ctrlRow[mid] < v) {
                lo = mid + 1;
            } else {
                hi = mid;
            }
        }

        int up = lo;
        int upEnd = m;

        while (up < upEnd) {
            const int mid = (up + upEnd) >> 1;

            if (ctrlRow[mid] <= v) {
                up = mid + 1;
            } else {
                upEnd = mid;
            }
        }

        int end = c + 1;

        while (end < n && (float)sortedCpm[end] == v) {
            ++end;
        }

        const double run = (double)(end - c);
        const double cControl = (double)(up - lo);
        const double merged = cControl + run;

        u += run * ((double)lo + 0.5 * cControl);
        adjust += (merged * merged * merged - merged) -
                  (cControl * cControl * cControl - cControl);
    }

    partial[tid] = u;
    __syncthreads();

    for (int s = nth >> 1; s > 0; s >>= 1) {
        if (tid < s) partial[tid] += partial[tid + s];
        __syncthreads();
    }

    const double uTotal = partial[0];

    partial[tid] = adjust;
    __syncthreads();

    for (int s = nth >> 1; s > 0; s >>= 1) {
        if (tid < s) partial[tid] += partial[tid + s];
        __syncthreads();
    }

    *uOut = uTotal;
    *tieOut = partial[0];
}

__device__ __forceinline__ double tcsZValue(double u,
                                            double tieAdjust,
                                            double tieSum,
                                            int n,
                                            int m)
{
    const double nf = (double)n;
    const double mf = (double)m;
    const double total = nf + mf;
    const double tc = 1.0 - (tieSum + tieAdjust) / (total * total * total - total);
    const double variance = tc * nf * mf * (total + 1.0) / 12.0;

    if (variance <= 1.0E-24) {
        return 0.0;
    }

    return (u - nf * mf / 2.0) / sqrt(variance);
}

// ---------------------------------------------------------------------------
// 质量守恒的分位取值
//
//   idx   = floor((shift + q - 0.5) * m)     （<= 0 取 0，>= m 取 m-1）
//   x[c]  = round(s[idx] * Lm[c] * 1e-6)
//   质量不足：按 ordq 顺序均摊 D = T - mass
//   质量超出：按 x 降序水位削平到 T
// ---------------------------------------------------------------------------

__device__ void tcsBuildCounts(const float* ctrlRow,
                               int m,
                               const double* q,
                               const int* ordq,
                               const double* libs,
                               int n,
                               long long total,
                               double shift,
                               int ntop,
                               double* keys,
                               int* idx,
                               int* counts,
                               int tid,
                               int nth)
{
    for (int c = tid; c < n; c += nth) {
        const double position = shift + q[c] - 0.5;
        double v = 0.0;

        if (position > 0.0) {
            int at = (int)(position * (double)m);

            if (at >= m) {
                at = m - 1;
            }

            v = (double)ctrlRow[at];
        }

        counts[c] = (int)(v * libs[c] * 1e-6 + 0.5);
    }

    __syncthreads();

    long long mass = 0;

    for (int c = 0; c < n; ++c) {
        mass += (long long)counts[c];
    }

    if (mass < total) {
        long long deficit = total - mass;
        int nd = (int)((deficit < (long long)ntop) ? deficit : (long long)ntop);

        if (nd > n) nd = n;
        if (nd < 1) nd = 1;

        const long long add = deficit / nd;
        const long long rest = deficit - add * nd;

        for (int j = tid; j < nd; j += nth) {
            counts[ordq[j]] += (int)(add + ((j < rest) ? 1 : 0));
        }

        __syncthreads();

        return;
    }

    if (mass == total) {
        return;
    }

    // 质量超出：按降序水位削平
    const long long excess = mass - total;
    const int ncap = tcsCapacity(n);

    for (int i = tid; i < ncap; i += nth) {
        keys[i] = (i < n) ? (double)counts[i] : TCSYNTH_PAD;
        idx[i] = i;
    }

    __syncthreads();

    tcsBitonicSort(keys, idx, ncap, nth, tid);

    if (tid == 0) {
        long long cumulative = 0;
        bool solved = false;

        for (int i = n - 1; i >= 0; --i) {
            cumulative += (long long)keys[i];

            const long long next = (i > 0) ? (long long)keys[i - 1] : 0;
            const int width = n - i;

            if (cumulative - (long long)width * next >= excess) {
                const long long keep = cumulative - excess;
                const long long level = keep / (long long)width;
                const long long rest = keep - level * (long long)width;

                for (int j = 0; j < width; ++j) {
                    counts[idx[n - 1 - j]] = (int)(level + ((j < rest) ? 1 : 0));
                }

                solved = true;

                break;
            }
        }

        if (!solved) {
            for (int c = 0; c < n; ++c) {
                counts[c] = 0;
            }
        }
    }

    __syncthreads();
}

// ---------------------------------------------------------------------------
// Gaussian copula：e = r·zl + sqrt(1-r²)·e0 → (i+0.5)/n 分位与降序名次
//
// 启动形状：grid = (G, 1)，block = (256, 1)
// ---------------------------------------------------------------------------

extern "C" __global__ void tcSynthCopulaQuantileF64Kernel(const double* __restrict__ e0,
                                                          const double* __restrict__ zl,
                                                          double correlation,
                                                          int n,
                                                          double* __restrict__ quantiles,
                                                          int* __restrict__ ordering)
{
    __shared__ double keys[TCSYNTH_MAX_N];
    __shared__ int idx[TCSYNTH_MAX_N];

    const int tid = threadIdx.x;
    const int nth = blockDim.x;
    const int g = blockIdx.x;
    const double complement = sqrt(fmax(1.0 - correlation * correlation, 0.0));
    const int ncap = tcsCapacity(n);

    if (ncap > TCSYNTH_MAX_N) {
        return;   // n 超过容量：由主机侧分批
    }

    for (int i = tid; i < ncap; i += nth) {
        keys[i] = (i < n) ? (correlation * zl[i] + complement * e0[(size_t)g * n + i]) : TCSYNTH_PAD;
        idx[i] = i;
    }

    __syncthreads();

    tcsBitonicSort(keys, idx, ncap, nth, tid);

    for (int i = tid; i < n; i += nth) {
        quantiles[(size_t)g * n + idx[i]] = ((double)i + 0.5) / (double)n;
        ordering[(size_t)g * n + i] = idx[n - 1 - i];
    }
}

// ---------------------------------------------------------------------------
// 分位取值 + 质量守恒
//
// 启动形状：grid = (G, 1)，block = (256, 1)
// ---------------------------------------------------------------------------

extern "C" __global__ void tcSynthQuantileMassF64Kernel(const float* __restrict__ ctrl,
                                                        const double* __restrict__ q,
                                                        const int* __restrict__ ordq,
                                                        const double* __restrict__ libs,
                                                        const long long* __restrict__ totals,
                                                        const double* __restrict__ shift,
                                                        int n,
                                                        int m,
                                                        int ntop,
                                                        int* __restrict__ counts)
{
    __shared__ double keys[TCSYNTH_MAX_N];
    __shared__ int idx[TCSYNTH_MAX_N];
    __shared__ int sharedCounts[TCSYNTH_MAX_N];

    const int g = blockIdx.x;
    const int tid = threadIdx.x;
    const int nth = blockDim.x;

    tcsBuildCounts(ctrl + (size_t)g * m, m,
                   q + (size_t)g * n, ordq + (size_t)g * n,
                   libs, n, totals[g], shift[g], ntop,
                   keys, idx, sharedCounts, tid, nth);

    for (int c = tid; c < n; c += nth) {
        counts[(size_t)g * n + c] = sharedCounts[c];
    }
}

// ---------------------------------------------------------------------------
// 精确秩和 z
//
// 启动形状：grid = (G, 1)，block = (256, 1)
// ---------------------------------------------------------------------------

extern "C" __global__ void tcSynthWilcoxonZF64Kernel(const int* __restrict__ counts,
                                                     const double* __restrict__ libs,
                                                     const float* __restrict__ ctrl,
                                                     const double* __restrict__ tieSum,
                                                     int n,
                                                     int m,
                                                     double* __restrict__ zOut)
{
    __shared__ double keys[TCSYNTH_MAX_N];
    __shared__ int idx[TCSYNTH_MAX_N];
    __shared__ double partial[TCSYNTH_BLOCK];

    const int g = blockIdx.x;
    const int tid = threadIdx.x;
    const int nth = blockDim.x;
    const int ncap = tcsCapacity(n);

    if (ncap > TCSYNTH_MAX_N) {
        if (tid == 0) zOut[g] = 0.0;

        return;
    }

    for (int i = tid; i < ncap; i += nth) {
        if (i < n) {
            const double library = fmax(libs[i], 1.0);

            keys[i] = (double)(float)(1e6 * (double)counts[(size_t)g * n + i] / library);
        } else {
            keys[i] = TCSYNTH_PAD;
        }

        idx[i] = i;
    }

    __syncthreads();

    tcsBitonicSort(keys, idx, ncap, nth, tid);

    double u = 0.0;
    double tie = 0.0;

    tcsRankSum(keys, n, ctrl + (size_t)g * m, m, partial, nth, tid, &u, &tie);

    if (tid == 0) {
        zOut[g] = tcsZValue(u, tie, tieSum[g], n, m);
    }
}

// ---------------------------------------------------------------------------
// 逐基因平移二分
//
// 启动形状：grid = (G, 1)，block = (256, 1)
// 迭代常量（上限 / 容差 / 窗口带）由主机侧按上游 config 传入，
// 保证与参考实现的迭代路径一致。
// ---------------------------------------------------------------------------

extern "C" __global__ void tcSynthShiftBisectF64Kernel(const float* __restrict__ ctrl,
                                                       const double* __restrict__ tieSum,
                                                       const double* __restrict__ q,
                                                       const int* __restrict__ ordq,
                                                       const double* __restrict__ libs,
                                                       const long long* __restrict__ totals,
                                                       const double* __restrict__ target,
                                                       int n,
                                                       int m,
                                                       int ntop,
                                                       int maxIterations,
                                                       double tolerance,
                                                       double lower,
                                                       double upper,
                                                       int* __restrict__ counts,
                                                       double* __restrict__ shiftOut,
                                                       double* __restrict__ zOut)
{
    __shared__ double keys[TCSYNTH_MAX_N];
    __shared__ int idx[TCSYNTH_MAX_N];
    __shared__ int sharedCounts[TCSYNTH_MAX_N];
    __shared__ double partial[TCSYNTH_BLOCK];
    __shared__ double bounds[2];
    __shared__ double bestZ;
    __shared__ double bestShift;

    const int g = blockIdx.x;
    const int tid = threadIdx.x;
    const int nth = blockDim.x;
    const int ncap = tcsCapacity(n);
    const float* ctrlRow = ctrl + (size_t)g * m;

    if (ncap > TCSYNTH_MAX_N) {
        if (tid == 0) {
            zOut[g] = 0.0;
            shiftOut[g] = lower;
        }

        return;
    }

    if (tid == 0) {
        bounds[0] = lower;
        bounds[1] = upper;
        bestZ = 1.0E+300;
        bestShift = lower;

        for (int c = 0; c < n; ++c) {
            counts[(size_t)g * n + c] = 0;
        }
    }

    __syncthreads();

    for (int iteration = 0; iteration < maxIterations; ++iteration) {
        const double mid = 0.5 * (bounds[0] + bounds[1]);

        tcsBuildCounts(ctrlRow, m, q + (size_t)g * n, ordq + (size_t)g * n,
                       libs, n, totals[g], mid, ntop,
                       keys, idx, sharedCounts, tid, nth);

        __syncthreads();

        for (int i = tid; i < ncap; i += nth) {
            if (i < n) {
                const double library = fmax(libs[i], 1.0);

                keys[i] = (double)(float)(1e6 * (double)sharedCounts[i] / library);
            } else {
                keys[i] = TCSYNTH_PAD;
            }

            idx[i] = i;
        }

        __syncthreads();

        tcsBitonicSort(keys, idx, ncap, nth, tid);

        double u = 0.0;
        double tie = 0.0;

        tcsRankSum(keys, n, ctrlRow, m, partial, nth, tid, &u, &tie);

        if (tid == 0) {
            const double want = target[g];
            const double z = tcsZValue(u, tie, tieSum[g], n, m);

            if (fabs(z - want) < fabs(bestZ - want)) {
                bestZ = z;
                bestShift = mid;

                for (int c = 0; c < n; ++c) {
                    counts[(size_t)g * n + c] = sharedCounts[c];
                }
            }

            if (fabs(z - want) <= tolerance) {
                bounds[0] = mid;
                bounds[1] = mid;
            } else if (z < want) {
                bounds[0] = mid;
            } else {
                bounds[1] = mid;
            }
        }

        __syncthreads();

        if (bounds[1] - bounds[0] <= 1.0E-12) {
            break;
        }
    }

    if (tid == 0) {
        shiftOut[g] = bestShift;
        zOut[g] = bestZ;
    }
}

// ---------------------------------------------------------------------------
// 低表达基因的突发散布
//
// 把每个基因的整数总量按"小突发"分散到若干个细胞上：先按目标库大小
// 归一化成分布，再按均匀随机数挑细胞。
// 启动形状：grid = (G, 1)，block = (1, 1)（顺序执行，保证随机流可复现）
// ---------------------------------------------------------------------------

extern "C" __global__ void tcSynthBurstF64Kernel(const long long* __restrict__ totals,
                                                 const double* __restrict__ libs,
                                                 int n,
                                                 int burst,
                                                 unsigned long long seed,
                                                 int* __restrict__ counts)
{
    if (threadIdx.x != 0) {
        return;
    }

    const int g = blockIdx.x;
    const long long total = totals[g];

    for (int c = 0; c < n; ++c) {
        counts[(size_t)g * n + c] = 0;
    }

    if (total <= 0 || burst < 1) {
        return;
    }

    double sum = 0.0;

    for (int c = 0; c < n; ++c) {
        sum += fmax(libs[c], 0.0);
    }

    if (sum <= 0.0) {
        return;
    }

    long long remaining = total;
    int step = 0;

    while (remaining > 0) {
        const double u = tcsUniform(seed, (unsigned long long)(g * 7919 + step));
        double acc = 0.0;
        int hit = n - 1;

        for (int c = 0; c < n; ++c) {
            acc += fmax(libs[c], 0.0) / sum;

            if (u < acc) {
                hit = c;

                break;
            }
        }

        const int size = (remaining < (long long)burst) ? (int)remaining : burst;

        counts[(size_t)g * n + hit] += size;
        remaining -= (long long)size;
        step += 1;
    }
}
