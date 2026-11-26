''' <summary>
''' CellularAI：在未知细胞类型中预测转录组对基因扰动的响应，并为每个扰动合成非负整数计数的单细胞。
''' </summary>
''' <remarks>
''' <para>
''' 本程序集是 CellularAI 算法的<b>纯 CPU、纯内存</b>实现：只接收上游装配好的内存对象
''' （<c>CsrMatrix</c> 控制细胞矩阵 + <see cref="Summary.SummaryBundle"/> 训练摘要 + <see cref="Model.CellularAiModel"/> 参数），
''' 返回预测计数矩阵与 <see cref="PredictionReport"/>。<b>不做任何文件 IO</b>——
''' parquet / h5ad / vcc 的读写由调用方负责。
''' </para>
''' <para>七个阶段与对应的命名空间：</para>
''' <list type="number">
''' <item><description><see cref="Summary"/> + <see cref="Evidence"/>：轴对齐、带符号 z、三维加权随机效应池化。</description></item>
''' <item><description><see cref="Planning"/>：相似度权重、计划显著性、调用尺寸与成员选择。</description></item>
''' <item><description><see cref="Residual"/>：低秩响应基 + 逐基因多响应岭回归 + 形状约束。</description></item>
''' <item><description><see cref="Design"/>：剖面离散度、边距求解、控制程序平滑、整数总量与固定点迭代。</description></item>
''' <item><description><see cref="Synthesis"/> + <see cref="Placement"/>：分位取值、精确秩和、copula 搜索与逐扰动并行合成。</description></item>
''' <item><description><see cref="Scoring"/>：DE 表与 VCC 六项指标。</description></item>
''' <item><description><see cref="ContextPipeline"/>：端到端装配与门控，唯一对外入口。</description></item>
''' </list>
''' <para>
''' 随机性统一由 <see cref="Randomizer.Pcg64"/>（<c>xoshiro256+</c> 内核）提供，扰动种子由
''' <see cref="Randomizer.PerturbationSeed"/> 从扰动名派生，因此同一 <c>seed</c> 重跑两次的
''' 计数矩阵逐元素相等，且与并行度无关。
''' </para>
''' </remarks>
Module NamespaceDoc
End Module
