// ---------------------------------------------------------------------------
// 设备端伪随机数（ILCuda）
//
// 输出函数采用 PCG64（XSL RR 128/64）：128 位线性同余状态 + 异或移位/旋转输出。
// 种子与子序列用 SplitMix64 展开，避免低熵种子导致的短周期与相邻相关。
//
// 绘制语义是"计数器式"（counter-based）：第 i 个随机数只由
// (seed, subsequence, offset + i) 决定，与线程划分、块大小、调度顺序无关，
// 因此同一组参数在任何设备与任何 block 配置下都产生逐位相同的序列。
// 主机侧 GpuRandom 里的 Pcg64 参考实现与之逐位一致（均匀分布通道）。
//
// 正态分布用 Box-Muller：第 i 个输出消费计数器 2i 与 2i+1，
// 因此"每元素两次抽取"的顺序显式且可复现；但 sqrt/log/cos 属于超越函数，
// 设备 libm 与 .NET 的实现在最后几个比特上可能不同，验收按 1e-12 判据。
//
// 宏前缀 ILCUDA_RNG_，符号前缀 ilcudaRng。
// ---------------------------------------------------------------------------

#define ILCUDA_RNG_BLOCK 256

// PCG64 的 128 位乘法常数（标准 pcg64 的 multiplier）
#define ILCUDA_RNG_MUL_LO 0x4385DF649FCCF645ull
#define ILCUDA_RNG_MUL_HI 0x2360ED051FC65DA4ull

#define ILCUDA_RNG_GOLDEN 0x9E3779B97F4A7C15ull
#define ILCUDA_RNG_TWO_PI 6.283185307179586476925286766559

// ---------------------------------------------------------------------------
// 基础装置
// ---------------------------------------------------------------------------

__device__ __forceinline__ unsigned long long ilcudaRngSplitMix64(unsigned long long x)
{
    x += ILCUDA_RNG_GOLDEN;

    unsigned long long z = x;

    z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9ull;
    z = (z ^ (z >> 27)) * 0x94D049BB133111EBull;

    return z ^ (z >> 31);
}

//! 128 位状态推进：state = state * MUL + inc（inc 只有低 64 位非零）
__device__ __forceinline__ void ilcudaRngStep(unsigned long long* hi,
                                              unsigned long long* lo,
                                              unsigned long long inc)
{
    const unsigned long long aLo = *lo;
    const unsigned long long aHi = *hi;

    const unsigned long long p0lo = aLo * ILCUDA_RNG_MUL_LO;
    const unsigned long long p0hi = __umul64hi(aLo, ILCUDA_RNG_MUL_LO);
    const unsigned long long p1lo = aLo * ILCUDA_RNG_MUL_HI;
    const unsigned long long p2lo = aHi * ILCUDA_RNG_MUL_LO;

    unsigned long long newHi = p0hi + p1lo + p2lo;
    unsigned long long newLo = p0lo + inc;

    if (newLo < p0lo) {
        newHi += 1ull;
    }

    *hi = newHi;
    *lo = newLo;
}

//! XSL RR 128/64 输出函数
__device__ __forceinline__ unsigned long long ilcudaRngOutput(unsigned long long hi,
                                                             unsigned long long lo)
{
    const unsigned int rot = (unsigned int)(hi >> 58);
    const unsigned long long x = hi ^ lo;

    return (x >> rot) | (x << ((64u - rot) & 63u));
}

//! 第 counter 个 64 位输出：由 (seed, subsequence, counter) 唯一确定
__device__ __forceinline__ unsigned long long ilcudaRngNext(unsigned long long seed,
                                                            unsigned long long subsequence,
                                                            unsigned long long counter)
{
    unsigned long long hi = ilcudaRngSplitMix64(seed ^ (subsequence * ILCUDA_RNG_GOLDEN));
    unsigned long long lo = ilcudaRngSplitMix64(hi ^ counter);
    const unsigned long long inc = (subsequence << 1) | 1ull;

    ilcudaRngStep(&hi, &lo, inc);

    return ilcudaRngOutput(hi, lo);
}

//! [0, 1) 的 53 位均匀 double
__device__ __forceinline__ double ilcudaRngUniform(unsigned long long seed,
                                                   unsigned long long subsequence,
                                                   unsigned long long counter)
{
    const unsigned long long x = ilcudaRngNext(seed, subsequence, counter);

    return (double)(x >> 11) * (1.0 / 9007199254740992.0);
}

// ---------------------------------------------------------------------------
// 均匀分布填充
//
// 启动形状：一维 grid-stride；第 i 个元素的计数器固定为 offset + i，
// 因此与 block 划分无关。
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaRngUniformF64Kernel(double* __restrict__ out,
                                                     int n,
                                                     unsigned long long seed,
                                                     unsigned long long subsequence,
                                                     unsigned long long offset)
{
    const int stride = gridDim.x * blockDim.x;
    int i = blockIdx.x * blockDim.x + threadIdx.x;

    for (; i < n; i += stride) {
        out[i] = ilcudaRngUniform(seed, subsequence, offset + (unsigned long long)i);
    }
}

extern "C" __global__ void ilcudaRngUniformF32Kernel(float* __restrict__ out,
                                                     int n,
                                                     unsigned long long seed,
                                                     unsigned long long subsequence,
                                                     unsigned long long offset)
{
    const int stride = gridDim.x * blockDim.x;
    int i = blockIdx.x * blockDim.x + threadIdx.x;

    for (; i < n; i += stride) {
        out[i] = (float)ilcudaRngUniform(seed, subsequence, offset + (unsigned long long)i);
    }
}

// ---------------------------------------------------------------------------
// 标准正态填充（Box-Muller，每元素消费两个计数器）
// ---------------------------------------------------------------------------

__device__ __forceinline__ double ilcudaRngNormal(unsigned long long seed,
                                                  unsigned long long subsequence,
                                                  unsigned long long index)
{
    const unsigned long long base = index * 2ull;
    double u1 = ilcudaRngUniform(seed, subsequence, base);
    const double u2 = ilcudaRngUniform(seed, subsequence, base + 1ull);

    // u1 取到 0 会让 log 发散；53 位分辨率下最小正值为 2^-53
    if (u1 <= 0.0) {
        u1 = 1.0 / 9007199254740992.0;
    }

    const double radius = sqrt(-2.0 * log(u1));

    return radius * cos(ILCUDA_RNG_TWO_PI * u2);
}

extern "C" __global__ void ilcudaRngNormalF64Kernel(double* __restrict__ out,
                                                    int n,
                                                    unsigned long long seed,
                                                    unsigned long long subsequence,
                                                    unsigned long long offset)
{
    const int stride = gridDim.x * blockDim.x;
    int i = blockIdx.x * blockDim.x + threadIdx.x;

    for (; i < n; i += stride) {
        out[i] = ilcudaRngNormal(seed, subsequence, offset + (unsigned long long)i);
    }
}

extern "C" __global__ void ilcudaRngNormalF32Kernel(float* __restrict__ out,
                                                    int n,
                                                    unsigned long long seed,
                                                    unsigned long long subsequence,
                                                    unsigned long long offset)
{
    const int stride = gridDim.x * blockDim.x;
    int i = blockIdx.x * blockDim.x + threadIdx.x;

    for (; i < n; i += stride) {
        out[i] = (float)ilcudaRngNormal(seed, subsequence, offset + (unsigned long long)i);
    }
}

// ---------------------------------------------------------------------------
// 置换（Fisher-Yates，单线程顺序执行）
//
// 顺序语义必须与主机参考实现完全一致：先写 0..n-1，再从 n-1 往下交换。
// n 在千级（细胞数）时单线程足够；这不是热路径。
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaRngPermutationKernel(int* __restrict__ indices,
                                                      int n,
                                                      unsigned long long seed,
                                                      unsigned long long subsequence,
                                                      unsigned long long offset)
{
    if (blockIdx.x != 0 || threadIdx.x != 0) {
        return;
    }

    for (int i = 0; i < n; ++i) {
        indices[i] = i;
    }

    for (int i = n - 1; i > 0; --i) {
        const double u = ilcudaRngUniform(seed, subsequence, offset + (unsigned long long)i);
        const int j = (int)(u * (double)(i + 1));

        if (j >= 0 && j <= i && j != i) {
            const int swap = indices[i];

            indices[i] = indices[j];
            indices[j] = swap;
        }
    }
}

// ---------------------------------------------------------------------------
// 按累积分布加权抽样
//
// cdf 是单调不减的累计概率，最后一个元素应当是 1（更大也可以）。
// 第 k 次抽样的计数器为 offset + k，因此抽样序列可复现。
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaRngChoiceF64Kernel(const double* __restrict__ cdf,
                                                    int n,
                                                    int* __restrict__ out,
                                                    int draws,
                                                    unsigned long long seed,
                                                    unsigned long long subsequence,
                                                    unsigned long long offset)
{
    const int stride = gridDim.x * blockDim.x;
    int k = blockIdx.x * blockDim.x + threadIdx.x;

    for (; k < draws; k += stride) {
        const double u = ilcudaRngUniform(seed, subsequence, offset + (unsigned long long)k);

        int lo = 0;
        int hi = n;

        // 第一个满足 cdf[i] > u 的位置（与主机侧参考实现同口径）
        while (lo < hi) {
            const int mid = (lo + hi) >> 1;

            if (cdf[mid] > u) {
                hi = mid;
            } else {
                lo = mid + 1;
            }
        }

        out[k] = (lo < n) ? lo : (n - 1);
    }
}
