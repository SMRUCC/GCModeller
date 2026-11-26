Namespace Hypothesis

    ''' <summary>
    ''' 逐基因的批量秩和检验结果（一个实验组 × 一组基因）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 数组下标与对照组的基因轴一致；不满足表达门槛的基因，其
    ''' <see cref="PValue"/> / <see cref="PAdjusted"/> 为 <see cref="Double.NaN"/>、
    ''' <see cref="Eligible"/> 为 <c>False</c>，这样调用方可以按"是否参与检验"与
    ''' "检验结果是否显著"分开判断，不会把"未检验"误认为"检验后不显著"。
    ''' </para>
    ''' <para>
    ''' 多重校正只在<b>参与检验的基因</b>上做（与单细胞差异表达的标准做法一致），
    ''' 校正后按 <see cref="RankSumOptions.Alpha"/> 标记显著性。
    ''' </para>
    ''' </remarks>
    Public Class RankSumTable

        Friend Sub New(geneCount As Integer, options As RankSumOptions)
            Me.GeneCount = geneCount
            Me.Options = options
            Me.U = New Double(geneCount - 1) {}
            Me.Z = New Double(geneCount - 1) {}
            Me.PValue = New Double(geneCount - 1) {}
            Me.PAdjusted = New Double(geneCount - 1) {}
            Me.Effect = New Double(geneCount - 1) {}
            Me.Significant = New Boolean(geneCount - 1) {}
            Me.Eligible = New Boolean(geneCount - 1) {}
            Me.Method = New RankSumMethod(geneCount - 1) {}
        End Sub

        ''' <summary>
        ''' 基因数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GeneCount As Integer

        ''' <summary>
        ''' 本次检验使用的参数
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Options As RankSumOptions

        ''' <summary>
        ''' Mann-Whitney U 统计量
        ''' </summary>
        ''' <returns></returns>
        Public Property U As Double()

        ''' <summary>
        ''' 正态化 z 值（精确法时同样给出，仅作参考）
        ''' </summary>
        ''' <returns></returns>
        Public Property Z As Double()

        ''' <summary>
        ''' 原始 P 值
        ''' </summary>
        ''' <returns></returns>
        Public Property PValue As Double()

        ''' <summary>
        ''' 多重校正后的 P 值（BH 等）
        ''' </summary>
        ''' <returns></returns>
        Public Property PAdjusted As Double()

        ''' <summary>
        ''' 效应量 <c>log2((mean_sample + eps)/(mean_control + eps))</c>
        ''' </summary>
        ''' <returns></returns>
        Public Property Effect As Double()

        ''' <summary>
        ''' 校正后是否显著（<c>PAdjusted &lt; Alpha</c>）
        ''' </summary>
        ''' <returns></returns>
        Public Property Significant As Boolean()

        ''' <summary>
        ''' 是否参与检验（对照组平均 CPM 超过门槛）
        ''' </summary>
        ''' <returns></returns>
        Public Property Eligible As Boolean()

        ''' <summary>
        ''' 每个基因使用的检验方法
        ''' </summary>
        ''' <returns></returns>
        Public Property Method As RankSumMethod()

        ''' <summary>
        ''' 参与检验的基因个数
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property EligibleCount As Integer
            Get
                Dim n As Integer = 0

                For i As Integer = 0 To Eligible.Length - 1
                    If Eligible(i) Then n += 1
                Next

                Return n
            End Get
        End Property

        ''' <summary>
        ''' 显著基因的下标
        ''' </summary>
        ''' <returns></returns>
        Public Function SignificantGenes() As Integer()
            Dim index As New List(Of Integer)

            For i As Integer = 0 To Significant.Length - 1
                If Significant(i) Then Call index.Add(i)
            Next

            Return index.ToArray
        End Function

        ''' <summary>
        ''' 取得单个基因的结果摘要
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <returns></returns>
        Public Function Row(gene As Integer) As WilcoxonResult
            Return New WilcoxonResult With {
                .U = U(gene),
                .Z = Z(gene),
                .PValue = PValue(gene),
                .PAdjusted = PAdjusted(gene),
                .Effect = Effect(gene),
                .Significant = Significant(gene),
                .Alternative = Options.Alternative,
                .Method = Method(gene),
                .N1 = 0,
                .N2 = 0
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"[{GeneCount} gene] rank sum table, eligible={EligibleCount}, significant={SignificantGenes().Length}"
        End Function

    End Class
End Namespace
