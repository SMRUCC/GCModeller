// ---------------------------------------------------------------------------
// 批量小线性求解（ILCuda）
//
// 面向"每个基因一组小方程"的场景：
//   * 小档 n <= 32 ：一个 block 一个矩阵，Cholesky 因子放共享内存
//                   （残差岭回归的特征维 f = 6）
//   * 中档 n <= 256：一个 block 一个矩阵，因子上移到全局暂存
//                   （响应维 k = 160 的投影 ridge）
//   * 融合核：带行掩码的 Gram 矩阵 + 岭项，一次产出 XᵀX + λI 与 XᵀY，
//             避免为每个基因重复搬运设计矩阵
//
// 数值口径：Cholesky 的下三角因子在分解过程中就地覆盖输入的工作副本；
// 主元 <= 0（数值上非正定）时该列的因子置 0，求解退化为 0 而不是产生 NaN。
// 布尔量一律用 int 传递（CUDA 内核参数不支持 Boolean）。
//
// 宏前缀 ILCUDA_CHOL_，符号前缀 ilcudaChol / ilcudaGram。
// ---------------------------------------------------------------------------

#define ILCUDA_CHOL_SMALL_N 32
#define ILCUDA_CHOL_MEDIUM_N 256

// ---------------------------------------------------------------------------
// 小档：共享内存版（n <= 32）
//
// 启动形状：grid = (batch, 1)，block = (128, 1)；动态共享内存 = n * n * 8
// A[batch, n, n] 为对称正定矩阵（对角上加 lambda），B[batch, n, nrhs] 为右端项。
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaCholSolveSmallF64Kernel(const double* __restrict__ A,
                                                         const double* __restrict__ B,
                                                         double* __restrict__ X,
                                                         int n, int nrhs,
                                                         double lambda)
{
    extern __shared__ double L[];

    const int tid = threadIdx.x;
    const int nth = blockDim.x;
    const size_t abase = (size_t)blockIdx.x * (size_t)n * (size_t)n;
    const size_t bbase = (size_t)blockIdx.x * (size_t)n * (size_t)nrhs;

    // 载入下三角（对角加岭项）
    for (int idx = tid; idx < n * n; idx += nth) {
        const int i = idx / n;
        const int j = idx - i * n;

        L[idx] = (j <= i) ? (A[abase + idx] + ((i == j) ? lambda : 0.0)) : 0.0;
    }

    __syncthreads();

    // 就地 Cholesky
    for (int k = 0; k < n; ++k) {
        if (tid == 0) {
            double s = L[k * n + k];

            for (int j = 0; j < k; ++j) {
                s -= L[k * n + j] * L[k * n + j];
            }

            L[k * n + k] = (s > 0.0) ? sqrt(s) : 0.0;
        }

        __syncthreads();

        const double pivot = L[k * n + k];

        for (int i = k + 1 + tid; i < n; i += nth) {
            double s = L[i * n + k];

            for (int j = 0; j < k; ++j) {
                s -= L[i * n + j] * L[k * n + j];
            }

            L[i * n + k] = (pivot > 0.0) ? (s / pivot) : 0.0;
        }

        __syncthreads();
    }

    // 前代 / 回代（每个右端项一个线程，就地写回 X）
    for (int c = tid; c < nrhs; c += nth) {
        for (int i = 0; i < n; ++i) {
            double s = B[bbase + (size_t)i * nrhs + c];

            for (int j = 0; j < i; ++j) {
                s -= L[i * n + j] * X[bbase + (size_t)j * nrhs + c];
            }

            X[bbase + (size_t)i * nrhs + c] = (L[i * n + i] > 0.0) ? (s / L[i * n + i]) : 0.0;
        }

        for (int i = n - 1; i >= 0; --i) {
            double s = X[bbase + (size_t)i * nrhs + c];

            for (int j = i + 1; j < n; ++j) {
                s -= L[j * n + i] * X[bbase + (size_t)j * nrhs + c];
            }

            X[bbase + (size_t)i * nrhs + c] = (L[i * n + i] > 0.0) ? (s / L[i * n + i]) : 0.0;
        }
    }
}

// ---------------------------------------------------------------------------
// 中档：全局暂存版（n <= 256）
//
// 启动形状：grid = (batch, 1)，block = (256, 1)
// L 为 [batch, n, n] 的全局暂存（调用方分配）。
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaCholSolveMediumF64Kernel(const double* __restrict__ A,
                                                          const double* __restrict__ B,
                                                          double* __restrict__ X,
                                                          double* __restrict__ L,
                                                          int n, int nrhs,
                                                          double lambda)
{
    const int tid = threadIdx.x;
    const int nth = blockDim.x;
    const size_t abase = (size_t)blockIdx.x * (size_t)n * (size_t)n;
    const size_t bbase = (size_t)blockIdx.x * (size_t)n * (size_t)nrhs;

    for (int idx = tid; idx < n * n; idx += nth) {
        const int i = idx / n;
        const int j = idx - i * n;

        L[abase + idx] = (j <= i) ? (A[abase + idx] + ((i == j) ? lambda : 0.0)) : 0.0;
    }

    __syncthreads();

    for (int k = 0; k < n; ++k) {
        if (tid == 0) {
            double s = L[abase + (size_t)k * n + k];

            for (int j = 0; j < k; ++j) {
                const double v = L[abase + (size_t)k * n + j];

                s -= v * v;
            }

            L[abase + (size_t)k * n + k] = (s > 0.0) ? sqrt(s) : 0.0;
        }

        __syncthreads();

        const double pivot = L[abase + (size_t)k * n + k];

        for (int i = k + 1 + tid; i < n; i += nth) {
            double s = L[abase + (size_t)i * n + k];

            for (int j = 0; j < k; ++j) {
                s -= L[abase + (size_t)i * n + j] * L[abase + (size_t)k * n + j];
            }

            L[abase + (size_t)i * n + k] = (pivot > 0.0) ? (s / pivot) : 0.0;
        }

        __syncthreads();
    }

    for (int c = tid; c < nrhs; c += nth) {
        for (int i = 0; i < n; ++i) {
            double s = B[bbase + (size_t)i * nrhs + c];

            for (int j = 0; j < i; ++j) {
                s -= L[abase + (size_t)i * n + j] * X[bbase + (size_t)j * nrhs + c];
            }

            const double pivot = L[abase + (size_t)i * n + i];

            X[bbase + (size_t)i * nrhs + c] = (pivot > 0.0) ? (s / pivot) : 0.0;
        }

        for (int i = n - 1; i >= 0; --i) {
            double s = X[bbase + (size_t)i * nrhs + c];

            for (int j = i + 1; j < n; ++j) {
                s -= L[abase + (size_t)j * n + i] * X[bbase + (size_t)j * nrhs + c];
            }

            const double pivot = L[abase + (size_t)i * n + i];

            X[bbase + (size_t)i * nrhs + c] = (pivot > 0.0) ? (s / pivot) : 0.0;
        }
    }
}

// ---------------------------------------------------------------------------
// 带行掩码的 Gram 矩阵 + 岭项
//
// 启动形状：grid = (batch, 1)，block = (256, 1)
//   X   [batch, rows, f]
//   Y   [batch, rows, k]
//   mask[batch, rows]  （0 表示该行不参与）
// 输出：
//   XX  [batch, f, f] = Σ_{mask} Xᵀ X + λI
//   XY  [batch, f, k] = Σ_{mask} Xᵀ Y
// ---------------------------------------------------------------------------

extern "C" __global__ void ilcudaGramMaskedF64Kernel(const double* __restrict__ X,
                                                     const double* __restrict__ Y,
                                                     const int* __restrict__ mask,
                                                     double* __restrict__ XX,
                                                     double* __restrict__ XY,
                                                     int rows, int f, int k,
                                                     double lambda)
{
    const size_t xbase = (size_t)blockIdx.x * (size_t)rows * (size_t)f;
    const size_t ybase = (size_t)blockIdx.x * (size_t)rows * (size_t)k;
    const int* maskBase = mask + (size_t)blockIdx.x * (size_t)rows;
    double* xx = XX + (size_t)blockIdx.x * (size_t)f * (size_t)f;
    double* xy = XY + (size_t)blockIdx.x * (size_t)f * (size_t)k;

    for (int idx = threadIdx.x; idx < f * f; idx += blockDim.x) {
        const int a = idx / f;
        const int b = idx - a * f;

        double s = 0.0;

        for (int r = 0; r < rows; ++r) {
            if (maskBase[r]) {
                s += X[xbase + (size_t)r * f + a] * X[xbase + (size_t)r * f + b];
            }
        }

        if (a == b) {
            s += lambda;
        }

        xx[idx] = s;
    }

    for (int idx = threadIdx.x; idx < f * k; idx += blockDim.x) {
        const int a = idx / k;
        const int c = idx - a * k;

        double s = 0.0;

        for (int r = 0; r < rows; ++r) {
            if (maskBase[r]) {
                s += X[xbase + (size_t)r * f + a] * Y[ybase + (size_t)r * k + c];
            }
        }

        xy[idx] = s;
    }
}
