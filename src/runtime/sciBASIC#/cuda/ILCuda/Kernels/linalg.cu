// ---------------------------------------------------------------------------
// 双精度线性代数原语（ILCuda）
//
// gemmF64：行主序分块矩阵乘，transA / transB 用 int 标志传递（0 = 不转置），
//          因为 Launch 参数无法携带模板实参。
//          转置语义遵循 BLAS：transA=1 表示 A 以 k x m 存放，取用时按 Aᵀ 索引。
// gemvF64：矩阵-向量乘，每 block 一行，共享内存树形归约。
// qrF64  ：economy QR（Householder），面向 m x n 且 m >= n 的高瘦矩阵
//          （随机化 SVD 的 A·Ω、AᵀQ 与 Q 组装都属于这一形状）。
//          Q 显式构造为 m x n，R 为 n x n。
//          整个分解在单个 block 内顺序推进列，列内用块内归约并行。
//
// 取舍：l x l 的小规模对称特征分解（l <= 168）留在主机，
//       单 block 的 Jacobi 扫掠在 GPU 上比 CPU 更慢，且不需要大矩阵带宽。
//
// 宏前缀 ILCUDA_GEMM_ / ILCUDA_QR_，符号前缀 ilcudaGemm / ilcudaGemv / ilcudaQr。
// ---------------------------------------------------------------------------

#define ILCUDA_GEMM_TILE 16
#define ILCUDA_QR_BLOCK 512

// ---------------------------------------------------------------------------
// 分块矩阵乘（双精度）：C(m x n) = op(A) * op(B)
//
// 启动形状：grid = (ceil(n/16), ceil(m/16))，block = (16, 16)
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaGemmF64Kernel(const double* __restrict__ A,
                                               const double* __restrict__ B,
                                               double* __restrict__ C,
                                               int m, int n, int k,
                                               int transA, int transB)
{
    __shared__ double aTile[ILCUDA_GEMM_TILE][ILCUDA_GEMM_TILE];
    __shared__ double bTile[ILCUDA_GEMM_TILE][ILCUDA_GEMM_TILE];

    const int row = blockIdx.y * ILCUDA_GEMM_TILE + threadIdx.y;
    const int col = blockIdx.x * ILCUDA_GEMM_TILE + threadIdx.x;

    double acc = 0.0;
    const int nTiles = (k + ILCUDA_GEMM_TILE - 1) / ILCUDA_GEMM_TILE;

    for (int t = 0; t < nTiles; ++t) {
        const int pa = t * ILCUDA_GEMM_TILE + threadIdx.x;   // 归约维下标（A 的列）
        const int pb = t * ILCUDA_GEMM_TILE + threadIdx.y;   // 归约维下标（B 的行）

        double av = 0.0;
        double bv = 0.0;

        if (row < m && pa < k) {
            av = transA ? A[(size_t)pa * (size_t)m + (size_t)row]
                        : A[(size_t)row * (size_t)k + (size_t)pa];
        }

        if (pb < k && col < n) {
            bv = transB ? B[(size_t)col * (size_t)k + (size_t)pb]
                        : B[(size_t)pb * (size_t)n + (size_t)col];
        }

        aTile[threadIdx.y][threadIdx.x] = av;
        bTile[threadIdx.y][threadIdx.x] = bv;

        __syncthreads();

#pragma unroll
        for (int q = 0; q < ILCUDA_GEMM_TILE; ++q) {
            acc += aTile[threadIdx.y][q] * bTile[q][threadIdx.x];
        }

        __syncthreads();
    }

    if (row < m && col < n) {
        C[(size_t)row * (size_t)n + (size_t)col] = acc;
    }
}

// ---------------------------------------------------------------------------
// 矩阵-向量乘（双精度）：y(m) = A(m x k) * x(k)
//
// 启动形状：grid = (m, 1)，block = (blockSize, 1)；动态共享内存 = blockSize * 8
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaGemvF64Kernel(const double* __restrict__ A,
                                               const double* __restrict__ x,
                                               double* __restrict__ y,
                                               int m, int k)
{
    extern __shared__ double cache[];

    const int row = blockIdx.x;
    const int tid = threadIdx.x;
    const int nth = blockDim.x;

    double acc = 0.0;

    for (int p = tid; p < k; p += nth) {
        acc += A[(size_t)row * (size_t)k + (size_t)p] * x[p];
    }

    cache[tid] = acc;
    __syncthreads();

    for (int s = nth >> 1; s > 0; s >>= 1) {
        if (tid < s) {
            cache[tid] += cache[tid + s];
        }

        __syncthreads();
    }

    if (tid == 0) {
        y[row] = cache[0];
    }
}

// ---------------------------------------------------------------------------
// economy QR（Householder）
//
// 启动形状：grid = (1, 1)，block = (512, 1)（必须是 2 的幂且 <= ILCUDA_QR_BLOCK）
//
// A 被就地改写：
//   * 上三角（含对角）= R
//   * 下三角 = 第 j 个反射向量 v 的尾部（v[j] 的首元素另存于 v0[j]）
// 参数：
//   v0    [n]：每个反射向量的首元素
//   betas [n]：每个反射的 2/(v·v)
//   Q     [m x n]：显式左奇异向量（H_0 H_1 ... H_{n-1} 作用于 I）
//   R     [n x n]：上三角
//
// 退化列（整列为 0）会得到 beta = 0，反射退化为恒等，不会破坏同步。
// ---------------------------------------------------------------------------

__device__ __forceinline__ double ilcudaQrBlockSum(double* red, double value, int nth)
{
    // 先同步一次：保证上一次调用留下的 red[0] 已经被所有线程读走
    __syncthreads();

    red[threadIdx.x] = value;
    __syncthreads();

    for (int s = nth >> 1; s > 0; s >>= 1) {
        if (threadIdx.x < s) {
            red[threadIdx.x] += red[threadIdx.x + s];
        }

        __syncthreads();
    }

    return red[0];
}

__device__ __forceinline__ double ilcudaQrReflector(const double* A,
                                                   int n,
                                                   const double* v0,
                                                   int j,
                                                   int i)
{
    return (i == j) ? v0[j] : A[(size_t)i * (size_t)n + (size_t)j];
}

extern "C" __global__ void ilcudaQrHouseholderF64Kernel(double* __restrict__ A,
                                                        int m, int n,
                                                        double* __restrict__ v0,
                                                        double* __restrict__ betas,
                                                        double* __restrict__ Q,
                                                        double* __restrict__ R)
{
    __shared__ double red[ILCUDA_QR_BLOCK];

    const int tid = threadIdx.x;
    const int nth = blockDim.x;

    // ---------------- 1) 逐列消元 ----------------
    for (int j = 0; j < n; ++j) {
        double square = 0.0;

        for (int i = j + tid; i < m; i += nth) {
            const double v = A[(size_t)i * (size_t)n + (size_t)j];

            square += v * v;
        }

        const double norm = sqrt(ilcudaQrBlockSum(red, square, nth));

        if (tid == 0) {
            const double head = A[(size_t)j * (size_t)n + (size_t)j];
            const double alpha = (head >= 0.0) ? -norm : norm;

            v0[j] = head - alpha;
            betas[j] = v0[j] * v0[j];
            A[(size_t)j * (size_t)n + (size_t)j] = alpha;   // R 的对角
        }

        __syncthreads();

        // v·v 的尾部（i > j）
        double tail = 0.0;

        for (int i = j + 1 + tid; i < m; i += nth) {
            const double v = A[(size_t)i * (size_t)n + (size_t)j];

            tail += v * v;
        }

        tail = ilcudaQrBlockSum(red, tail, nth);

        if (tid == 0) {
            const double vv = betas[j] + tail;

            betas[j] = (vv > 0.0) ? (2.0 / vv) : 0.0;
        }

        __syncthreads();

        // 从左应用反射到后面的列
        for (int c = j + 1; c < n; ++c) {
            double dot = 0.0;

            for (int i = j + tid; i < m; i += nth) {
                const double v = ilcudaQrReflector(A, n, v0, j, i);

                dot += v * A[(size_t)i * (size_t)n + (size_t)c];
            }

            dot = ilcudaQrBlockSum(red, dot, nth) * betas[j];

            for (int i = j + tid; i < m; i += nth) {
                const double v = ilcudaQrReflector(A, n, v0, j, i);

                A[(size_t)i * (size_t)n + (size_t)c] -= dot * v;
            }

            __syncthreads();
        }
    }

    // ---------------- 2) R 取上三角 ----------------
    for (int idx = tid; idx < n * n; idx += nth) {
        const int i = idx / n;
        const int c = idx - i * n;

        R[idx] = (c >= i) ? A[(size_t)i * (size_t)n + (size_t)c] : 0.0;
    }

    // ---------------- 3) Q = H_0 H_1 ... H_{n-1} · I ----------------
    for (int idx = tid; idx < m * n; idx += nth) {
        const int i = idx / n;
        const int c = idx - i * n;

        Q[idx] = (i == c) ? 1.0 : 0.0;
    }

    __syncthreads();

    for (int j = n - 1; j >= 0; --j) {
        for (int c = 0; c < n; ++c) {
            double dot = 0.0;

            for (int i = j + tid; i < m; i += nth) {
                const double v = ilcudaQrReflector(A, n, v0, j, i);

                dot += v * Q[(size_t)i * (size_t)n + (size_t)c];
            }

            dot = ilcudaQrBlockSum(red, dot, nth) * betas[j];

            for (int i = j + tid; i < m; i += nth) {
                const double v = ilcudaQrReflector(A, n, v0, j, i);

                Q[(size_t)i * (size_t)n + (size_t)c] -= dot * v;
            }

            __syncthreads();
        }
    }
}
