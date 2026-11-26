// ---------------------------------------------------------------------------
// 三维加权池化融合核（ILCudaTensor / CellularAI）
//
// 输入是"训练行 x 基因 x 扰动"的已中心化 log2FC 与有符号 z，
// 权重口径与上游一致：
//
//     w[t, g, p] = row[t, g] * col[t, p] * mat[t]
//
// 未测项（effect 为 NaN）按"值为 0、权重照算"计入分母：
// 这对应上游 random-effects 池化的 missingAsZero 语义，
// 与"缺失就剔除"是两种完全不同的口径，必须显式区分。
//
// 一次内核产出 DerSimonian-Laird 与逆方差合并所需的全部充分统计量：
//     S0 = Σw,  S1 = Σw·y,  S2 = Σw·y²,  SQ = Σw²,  K = 参与行数
// z 通道另有 Σw·z 与 Σw²（Stouffer 的分母）。
//
// 因子表达式（"sef*kd" 之类的算子串）在主机侧求值成稠密因子向量后传入，
// 内核只做纯乘加，不做表达式解析 —— 这样内核里没有分支爆炸，
// 也保证"乘法顺序固定"（浮点加法不满足结合律）。
//
// 宏前缀 TCPOOL_（TCX_ 已被 cellaccel.cu 的 TCX_ACT_* 占用，
// 同一 NVRTC 编译单元内不能复用），符号前缀 tcPool。
// ---------------------------------------------------------------------------

#define TCPOOL_BLOCK 256

// ---------------------------------------------------------------------------
// 充分统计量
//
// 启动形状：grid = (G, 1)，block = (256, 1)（线程铺在 P 轴上）
// 每个线程负责若干个 (g, p) 组合，沿 T 轴顺序累加 —— 顺序固定，
// 因此结果与线程调度无关。
// ---------------------------------------------------------------------------

extern "C" __global__ void tcPoolStatsF64Kernel(const double* __restrict__ effect,
                                                const double* __restrict__ rowFactor,
                                                const double* __restrict__ colFactor,
                                                const double* __restrict__ matFactor,
                                                int T, int G, int P,
                                                double* __restrict__ s0,
                                                double* __restrict__ s1,
                                                double* __restrict__ s2,
                                                double* __restrict__ sq,
                                                int* __restrict__ measured)
{
    const int g = blockIdx.x;
    const int stride = blockDim.x;

    for (int p = threadIdx.x; p < P; p += stride) {
        double a0 = 0.0;
        double a1 = 0.0;
        double a2 = 0.0;
        double aq = 0.0;
        int count = 0;

        for (int t = 0; t < T; ++t) {
            const double w = rowFactor[(size_t)t * G + g] *
                             colFactor[(size_t)t * P + p] *
                             matFactor[t];
            const double y = effect[((size_t)t * G + g) * P + p];
            const bool missing = !(y == y);
            const double value = missing ? 0.0 : y;

            a0 += w;
            a1 += w * value;
            a2 += w * value * value;
            aq += w * w;

            if (!missing) {
                ++count;
            }
        }

        const size_t out = (size_t)g * P + p;

        s0[out] = a0;
        s1[out] = a1;
        s2[out] = a2;
        sq[out] = aq;
        measured[out] = count;
    }
}

// ---------------------------------------------------------------------------
// 合并结果：log2FC 加权均值 + z 的 Stouffer
//
//   l2 = S1 / S0                              （S0 <= 0 时为 NaN）
//   z  = |Σ w_z·z| / sqrt(Σ w_z²)             （只累加实测项）
//
// 启动形状：grid = (G, 1)，block = (256, 1)
// ---------------------------------------------------------------------------

extern "C" __global__ void tcPoolEffectsF64Kernel(const double* __restrict__ z,
                                                  const double* __restrict__ s0,
                                                  const double* __restrict__ s1,
                                                  const double* __restrict__ rowFactor,
                                                  const double* __restrict__ colFactor,
                                                  const double* __restrict__ matFactor,
                                                  int T, int G, int P,
                                                  double* __restrict__ l2,
                                                  double* __restrict__ zOut,
                                                  double* __restrict__ numZ,
                                                  double* __restrict__ sqwZ)
{
    const int g = blockIdx.x;
    const int stride = blockDim.x;

    for (int p = threadIdx.x; p < P; p += stride) {
        const size_t out = (size_t)g * P + p;

        const double den = s0[out];

        l2[out] = (den > 0.0) ? (s1[out] / den) : (0.0 / 0.0);

        double nz = 0.0;
        double qz = 0.0;

        for (int t = 0; t < T; ++t) {
            const double value = z[((size_t)t * G + g) * P + p];

            if (value == value) {
                const double w = rowFactor[(size_t)t * G + g] *
                                 colFactor[(size_t)t * P + p] *
                                 matFactor[t];

                nz += w * value;
                qz += w * w;
            }
        }

        numZ[out] = nz;
        sqwZ[out] = qz;
        zOut[out] = (qz > 0.0) ? (fabs(nz) / sqrt(qz)) : 0.0;
    }
}

// ---------------------------------------------------------------------------
// 逐基因剖面标准化
//
//   out[i] = (input[i] - center[i]) / scale[i]   （scale <= 0 时取 0）
//
// 启动形状：一维 grid-stride
// ---------------------------------------------------------------------------

extern "C" __global__ void tcPoolProfileF64Kernel(const double* __restrict__ input,
                                                  const double* __restrict__ center,
                                                  const double* __restrict__ scale,
                                                  int n,
                                                  double* __restrict__ out)
{
    const int stride = gridDim.x * blockDim.x;
    int i = blockIdx.x * blockDim.x + threadIdx.x;

    for (; i < n; i += stride) {
        const double s = scale[i];

        out[i] = (s > 0.0) ? ((input[i] - center[i]) / s) : 0.0;
    }
}
