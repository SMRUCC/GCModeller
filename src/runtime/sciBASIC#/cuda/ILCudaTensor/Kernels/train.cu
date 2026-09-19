// ---------------------------------------------------------------------------
// 手写训练内核（单精度 FP32）
//
// 为什么必须手写、而不能走 IL2Cuda 自动生成：
//   IL2Cuda 把内核函数的数组形参一律发射成 `const T* __restrict__`
//   （见 IL2Cuda\IlCudaKernel.vb 的 BuildSource），且每个内核只允许一个返回值
//   （唯一的 il_out，每线程只写一个元素）。于是：
//     * AdamW 需要"一个线程同时更新 param / m / v 并清零 grad"  -> 无法表达；
//     * 掩码交叉熵需要块内共享内存协作求行的 max / sum     -> 无共享内存支持。
//   这与 gemm.cu 头部的说明一致，因此同样以内嵌资源形式手写。
//
// 全部内核用 grid-stride 循环，block 数不必覆盖全部元素，超大张量也能一趟跑完。
//
// 命名约定：符号一律带 Fp32 后缀，宏一律带 TAW_ / TCE_ 前缀。
//   —— 所有 .cu 会被 KernelSources.CombinedSource() 拼进**同一个 NVRTC 编译单元**，
//      因此函数名与宏名都不能与 tensor.cu / gemm.cu / elementwise.cu 冲突。
//
// 索引一律用 long long 计算：语言模型的 LM head 是 [N, d_model] × [d_model, vocab]，
// 在 12.8 万词表下行偏移轻松超过 Int32 上限（约 21.5 亿）。
// ---------------------------------------------------------------------------

#define TAW_STRIDE_LOOP for (int i = blockDim.x * blockIdx.x + threadIdx.x; i < n; i += blockDim.x * gridDim.x)

// ---------------------------------------------------------------------------
// AdamW：原地更新参数与一阶/二阶矩，并在更新后清零梯度累加器
//
//   m <- b1*m + (1-b1)*g
//   v <- b2*v + (1-b2)*g*g
//   theta <- theta - lr * (m/bc1) / (sqrt(v/bc2) + eps) - lr*wd*theta
//
// 与主机端 AdamW.MakeTrainingStep 逐项对应（beta1=0.9 / beta2=0.999 / eps=1e-8 由调用方传入，
// 偏差校正项 bc1 = 1-b1^step、bc2 = 1-b2^step 也在主机侧算好后传入）。
//
// 启动：grid = (min(ceil(n/256), 4096), 1)，block = (256, 1)
// ---------------------------------------------------------------------------
extern "C" __global__ void tensorAdamWFp32Kernel(float* __restrict__ p,
                                                 float* __restrict__ g,
                                                 float* __restrict__ m,
                                                 float* __restrict__ v,
                                                 int n,
                                                 float lr, float b1, float b2, float eps,
                                                 float bc1, float bc2, float wd) {
    TAW_STRIDE_LOOP {
        const float gi = g[i];
        const float mi = b1 * m[i] + (1.0f - b1) * gi;
        const float vi = b2 * v[i] + (1.0f - b2) * gi * gi;

        m[i] = mi;
        v[i] = vi;

        const float mHat = mi / bc1;
        const float vHat = vi / bc2;

        // Adam 自适应项
        float delta = lr * mHat / (sqrtf(vHat) + eps);

        // 解耦权重衰减：与梯度统计完全无关的一项
        if (wd > 0.0f) delta += lr * wd * p[i];

        p[i] -= delta;

        // 更新即清零：这一步省掉一次全量显存往返
        g[i] = 0.0f;
    }
}

// ---------------------------------------------------------------------------
// 梯度累加：accum += alpha * src
//
// 独立内核而不是复用 ewAxpyKernel —— 后者把 a 与 out 都标了 __restrict__，
// 在同一段缓冲上做累加会违反 restrict 契约（未定义行为）。
//
// 启动：grid = (min(ceil(n/256), 4096), 1)，block = (256, 1)
// ---------------------------------------------------------------------------
extern "C" __global__ void tensorAccumulateFp32Kernel(float* __restrict__ accum,
                                                      const float* __restrict__ src,
                                                      float alpha, int n) {
    TAW_STRIDE_LOOP {
        accum[i] += alpha * src[i];
    }
}

// ---------------------------------------------------------------------------
// 融合掩码交叉熵：softmax + NLL + d(logits) = (softmax - onehot) / count
//
// 等价于主机端 LLMTensorOps.MaskedCrossEntropy 的三段循环
//   （求行最大值 -> 求 exp 和 -> 归一化）加一次散射减一，
// 但把 [rows, vocab] 级别的逐元素 exp 从主机搬到 GPU。
// 12.8 万词表下这是整个训练步里最重的一处主机循环。
//
// 语义与主机实现严格对齐：
//   * 只有满足「掩码为真」且「target 落在 [0, vocab)」的行才计入；
//   * 未计入的行梯度保持 0（dLogits 由调用方预清零），损失记 0；
//   * 损失与梯度都乘以 inv = 1 / count（count 由主机统计后传入）；
//   * 单行损失 = -log(max(p_target, 1e-12))。
//
// 每行一个 block，块内共享内存树形归约求 max / sum。
// 启动：grid = (rows, 1)，block = (TCE_BLOCK, 1)，无动态共享内存。
// ---------------------------------------------------------------------------

#define TCE_BLOCK 256
// 单精度最大值（不用 INFINITY 宏，与 tensor.cu 保持同一风格约定）
#define TCE_NEG_BIG (-3.402823466e+38f)

extern "C" __global__ void tensorMaskedCrossEntropyFp32Kernel(const float* __restrict__ logits,
                                                              const int* __restrict__ targets,
                                                              const int* __restrict__ mask,
                                                              float* __restrict__ dLogits,
                                                              float* __restrict__ rowLoss,
                                                              int rows, int vocab,
                                                              int useMask, float inv) {
    __shared__ float cache[TCE_BLOCK];

    const int row = blockIdx.x;
    if (row >= rows) return;

    const int tid = threadIdx.x;

    // ---- 判定该行是否计入损失 ----
    int supervised = 1;

    if (useMask != 0 && mask != 0 && mask[row] == 0) supervised = 0;

    int target = 0;

    if (supervised != 0) {
        target = targets[row];
        if (target < 0 || target >= vocab) supervised = 0;
    }

    if (supervised == 0) {
        // 梯度缓冲已由调用方清零，这里只需给出损失占位
        if (tid == 0) rowLoss[row] = 0.0f;
        return;
    }

    const float* p = logits + (long long)row * vocab;
    float* q = dLogits + (long long)row * vocab;

    // ---- 阶段 1：行内最大值（数值稳定）----
    float mx = TCE_NEG_BIG;

    for (int k = tid; k < vocab; k += blockDim.x) mx = fmaxf(mx, p[k]);

    cache[tid] = mx;
    __syncthreads();

    for (int s = blockDim.x >> 1; s > 0; s >>= 1) {
        if (tid < s) cache[tid] = fmaxf(cache[tid], cache[tid + s]);
        __syncthreads();
    }

    mx = cache[0];
    __syncthreads();

    // ---- 阶段 2：sum(exp(x - mx)) ----
    float sum = 0.0f;

    for (int k = tid; k < vocab; k += blockDim.x) sum += expf(p[k] - mx);

    cache[tid] = sum;
    __syncthreads();

    for (int s = blockDim.x >> 1; s > 0; s >>= 1) {
        if (tid < s) cache[tid] += cache[tid + s];
        __syncthreads();
    }

    sum = cache[0];
    if (sum <= 0.0f) sum = 1.0f;

    // ---- 阶段 3：梯度 = softmax * inv ----
    for (int k = tid; k < vocab; k += blockDim.x) {
        q[k] = (expf(p[k] - mx) / sum) * inv;
    }

    // 必须等全部 q[k] 落盘后才能在同一段缓冲上做散射，否则与阶段 3 的写入竞争
    __syncthreads();

    // ---- 阶段 4：target 位置减去 inv（onehot 项）并回写该行损失 ----
    if (tid == 0) {
        q[target] -= inv;

        float pt = expf(p[target] - mx) / sum;
        if (pt < 1.0e-12f) pt = 1.0e-12f;

        rowLoss[row] = -logf(pt) * inv;
    }
}
