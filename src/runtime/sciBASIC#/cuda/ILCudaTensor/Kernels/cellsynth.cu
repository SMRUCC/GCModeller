// ---------------------------------------------------------------------------
// 单细胞计数合成融合核（ILCudaTensor / CellularAI）
//
// 每个基因一个 block：
//   * tcSynthQuantileMass：分位锚点取值 + 整数质量守恒
//   * tcSynthWilcoxonZ   ：每细胞 CPM（float32 栅格）、段内排序、tie 项、
//                          合并方差、秩和 z
//   * tcSynthShiftBisect ：逐基因平移二分（核内顺序迭代，常量与参考实现一致）
//   * tcSynthCopulaQ     ：Gaussian copula 秩混合后转 (i+0.5)/n 分位
//   * tcSynthBurst       ：低表达基因按目标库大小随机散布
//
// 段内排序用共享内存双调排序（n_cells = 400 -> 400*4B = 1.6 KB），
// 因此本文件自带排序与随机数装置，不依赖 ILCuda 的 sort.cu / rng.cu
// （两个程序集各自编译，跨文件 device 函数依赖拼接顺序，容易失效）。
//
// 宏前缀 TCSYNTH_，符号前缀 tcSynth。
// ---------------------------------------------------------------------------
