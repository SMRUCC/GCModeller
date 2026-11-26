Namespace RandomSources

    ''' <summary>
    ''' 可复现的随机数源抽象。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 统计重采样（bootstrap、jackknife 抽样、copula 的噪声项）必须能接受调用方提供的
    ''' 随机源，才能在并行、多进程与回归测试中复现同一组结果。因此库内所有用到随机性的
    ''' API 都接受一个可选的 <see cref="IRandomSource"/>，并且**不持有静态共享实例**。
    ''' </para>
    ''' <para>
    ''' 契约：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description><see cref="NextDouble"/> 返回 <c>[0, 1)</c>；</description></item>
    ''' <item><description><see cref="NextNormal"/> 返回标准正态分布样本（均值 0，方差 1）；</description></item>
    ''' <item><description><see cref="Permutation(Integer)"/> 返回 <c>0..n-1</c> 的一个排列；</description></item>
    ''' <item><description>同一实现 + 同一种子必须给出相同序列（<see cref="IsRepeatable"/>），
    ''' 但**不承诺**跨实现（例如与 NumPy 的 PCG64）逐位一致。</description></item>
    ''' </list>
    ''' </remarks>
    Public Interface IRandomSource

        ''' <summary>
        ''' 该实现是否在相同种子下产生相同的序列
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsRepeatable As Boolean

        ''' <summary>
        ''' 均匀分布样本，返回 <c>[0, 1)</c>
        ''' </summary>
        ''' <returns></returns>
        Function NextDouble() As Double

        ''' <summary>
        ''' 标准正态分布样本
        ''' </summary>
        ''' <returns></returns>
        Function NextNormal() As Double

        ''' <summary>
        ''' 均匀分布整数，返回 <c>[0, bound)</c>
        ''' </summary>
        ''' <param name="bound">上界（不含），必须为正</param>
        ''' <returns></returns>
        Function NextInt(bound As Integer) As Integer

        ''' <summary>
        ''' 按给定权重抽取一个下标
        ''' </summary>
        ''' <param name="n">候选个数</param>
        ''' <param name="p">权重向量（长度 = <paramref name="n"/>，可以未归一化）；为 Nothing 表示等概率</param>
        ''' <returns><c>[0, n)</c> 内的下标</returns>
        Function Choice(n As Integer, Optional p As Double() = Nothing) As Integer

        ''' <summary>
        ''' 按给定权重抽取 <paramref name="size"/> 个下标（有放回）
        ''' </summary>
        ''' <param name="n">候选个数</param>
        ''' <param name="size">抽样次数</param>
        ''' <param name="p">权重向量；为 Nothing 表示等概率</param>
        ''' <returns>长度为 <paramref name="size"/> 的下标数组</returns>
        Function Choices(n As Integer, size As Integer, Optional p As Double() = Nothing) As Integer()

        ''' <summary>
        ''' 抽样 <paramref name="k"/> 个下标（可选有放回）
        ''' </summary>
        ''' <param name="n">候选个数</param>
        ''' <param name="k">样本量</param>
        ''' <param name="replace">是否放回</param>
        ''' <returns>长度为 <paramref name="k"/> 的下标数组</returns>
        Function Sample(n As Integer, k As Integer, Optional replace As Boolean = False) As Integer()

        ''' <summary>
        ''' 生成 <c>0..n-1</c> 的随机排列
        ''' </summary>
        ''' <param name="n">元素个数</param>
        ''' <returns></returns>
        Function Permutation(n As Integer) As Integer()

        ''' <summary>
        ''' 生成输入序列的随机排列（不修改输入）
        ''' </summary>
        ''' <param name="values">输入序列</param>
        ''' <returns></returns>
        Function Permutation(values As Double()) As Double()

        ''' <summary>
        ''' 派生一个独立的子随机源。
        ''' </summary>
        ''' <param name="key">派生键（例如扰动名、基因名的哈希）</param>
        ''' <returns>
        ''' 与当前实例的种子确定关联、但状态彼此独立的新实例；
        ''' 用于"每个任务一个子流"，保证并行执行顺序不影响结果。
        ''' </returns>
        Function Derive(key As Long) As IRandomSource

    End Interface
End Namespace
