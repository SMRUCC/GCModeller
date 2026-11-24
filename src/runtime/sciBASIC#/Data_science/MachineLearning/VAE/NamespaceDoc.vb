' ============================================================================
' NamespaceDoc.vb - 包级文档
'
' 这里同时记录「零破坏」约束与各模型的行为开关，便于后续维护者判断
' 哪些是新增能力、哪些只是修正了原先错误的数值行为。
' ============================================================================

''' <summary>
''' Variational autoencoders: the graph VAE model and its math utilities.
''' </summary>
''' <remarks>
''' <para>
''' **零破坏约束**：既有公开类型名、方法签名、属性名一律保持不变，旧调用代码无需
''' 修改即可编译运行。新增能力只以「新增类型 / 新增重载 / 新增只读属性 /
''' 显式接口实现」的形式提供，不修改任何既有签名。
''' </para>
''' <para>
''' **行为开关**：现有实现中被证明产出错误数值的部分按缺陷修复处理，
''' 同时保留开关以便复现旧行为做对比：
''' <list type="bullet">
''' <item><description>
''' <see cref="GMVAE"/>：<c>PriorMode</c>（Learned 复现旧先验）、
''' <c>Optimizer</c>（Sgd 复现旧优化器）、
''' <c>ReconstructionScale</c>（PerElement 复现旧的按元素归一）。
''' </description></item>
''' <item><description>
''' <see cref="GaussianMixtureModel"/>：协方差结构可通过新增的
''' <c>GmmOptions</c> 切换 Full / Diagonal / Tied / Spherical；
''' 旧的 <c>diagCovariance</c> 构造参数语义保持不变。
''' </description></item>
''' <item><description>
''' <see cref="GraphVAE"/>：<c>Evaluate</c> 改为确定性（原先会隐式采样），
''' <c>GCNLayer.AssumeSymmetricAdjacency</c> 允许显式声明有向图。
''' </description></item>
''' </list>
''' </para>
''' <para>
''' **命名空间**：
''' <list type="bullet">
''' <item><description><c>Core</c>：统一模型契约（生成 / 聚类 / 评分 / 持久化）、
''' 训练选项、训练观察者、模型选择。</description></item>
''' <item><description><c>Core.Numerics</c>：稳定的对数域运算、成对 Box-Muller、
''' 可复现随机数服务、线性代数门面、梯度校验。</description></item>
''' <item><description><c>Core.Layers</c>：全连接层与统一 Adam 优化器。</description></item>
''' <item><description><c>Cells</c>：条件变分自编码器、计数似然（NB/ZINB）、
''' 扰动实验数据集。</description></item>
''' <item><description><c>Imaging</c>：卷积变分自编码器。</description></item>
''' </list>
''' </para>
''' <para>
''' **审计**：<c>dotnet run -- --audit</c> 会在 bin 目录生成
''' <c>vae-audit-report.json</c> 与 <c>vae-audit-dump.txt</c>，进程退出码为失败项数量。
''' </para>
''' </remarks>
Friend Class NamespaceDoc
End Class
