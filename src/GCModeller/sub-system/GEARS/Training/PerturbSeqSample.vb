Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention

Namespace Training

    ''' <summary>
    ''' 一次（虚拟）扰动实验样本，等价于 Perturb-seq 中的一条记录
    ''' </summary>
    ''' <remarks>
    ''' 训练样本由三部分组成：
    ''' <list type="bullet">
    ''' <item><description>输入侧的 control 表达谱 —— 注意被扰动基因自身的表达值已被改写为干预值
    ''' （readme §4.2 的关键设计：扰动基因自身的表达改变会作为信息在网络中传播）；</description></item>
    ''' <item><description>输入侧的扰动基因集合（单基因或组合扰动）；</description></item>
    ''' <item><description>标签侧的扰动后真实表达谱。</description></item>
    ''' </list>
    '''
    ''' 模型实际学习的是 Δ = 扰动后表达 − 输入表达，而不是绝对表达值，
    ''' 这样标签分布近似零均值、训练更稳定（readme §6.3）。
    ''' </remarks>
    Public Class PerturbSeqSample

        ''' <summary>被扰动基因的节点索引集合</summary>
        ''' <returns>节点索引数组</returns>
        Public Property PerturbedGeneIndices As Integer()

        ''' <summary>被扰动基因的基因名集合，顺序与 <see cref="PerturbedGeneIndices"/> 一致</summary>
        ''' <returns>基因名数组</returns>
        Public Property PerturbedGeneNames As String()

        ''' <summary>输入侧表达谱 [numGenes]，被扰动基因位置已被改写为干预值</summary>
        ''' <returns>表达向量</returns>
        Public Property ControlExpression As Double()

        ''' <summary>标签侧表达谱 [numGenes]，即扰动之后的真实表达</summary>
        ''' <returns>表达向量</returns>
        Public Property PerturbedExpression As Double()

        ''' <summary>样本描述标签，例如 "codY_Knockout" 或 "codY+luxR_Knockout"</summary>
        ''' <returns>样本标签字符串</returns>
        Public Property Label As String

        ''' <summary>本次扰动所使用的干预模式（组合扰动取第一个基因的模式）</summary>
        ''' <returns><see cref="InterventionMode"/> 枚举值</returns>
        Public Property Mode As InterventionMode = InterventionMode.Knockout

        ''' <summary>
        ''' 计算标签 Δ = 扰动后表达 − 输入表达
        ''' </summary>
        ''' <returns>每个基因的表达变化量</returns>
        Public Function Delta() As Double()
            Dim n As Integer = ControlExpression.Length
            Dim result As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                result(i) = PerturbedExpression(i) - ControlExpression(i)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 构建扰动 multi-hot 标记向量
        ''' </summary>
        ''' <param name="numGenes">基因总数</param>
        ''' <returns>长度为 <paramref name="numGenes"/> 的标记向量，被扰动位置为 1</returns>
        Public Function PerturbationFlag(numGenes As Integer) As Double()
            Dim flag As Double() = New Double(numGenes - 1) {}

            For Each idx As Integer In PerturbedGeneIndices
                If idx >= 0 AndAlso idx < numGenes Then
                    flag(idx) = 1.0
                End If
            Next

            Return flag
        End Function

        ''' <summary>
        ''' 判断是否为组合扰动（同时扰动两个及以上基因）
        ''' </summary>
        ''' <returns>组合扰动返回 True</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IsCombination() As Boolean
            Return PerturbedGeneIndices IsNot Nothing AndAlso PerturbedGeneIndices.Length > 1
        End Function

        ''' <summary>
        ''' 输出样本摘要
        ''' </summary>
        ''' <returns>描述字符串</returns>
        Public Overrides Function ToString() As String
            Dim genes As String = If(PerturbedGeneNames Is Nothing, "", String.Join("+", PerturbedGeneNames))
            Dim tag As String = If(String.IsNullOrEmpty(Label), genes, Label)

            Return $"{tag} ({Mode})"
        End Function
    End Class
End Namespace
