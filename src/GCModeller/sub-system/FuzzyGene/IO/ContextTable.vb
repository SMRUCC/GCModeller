Imports SMRUCC.genomics.BioSystem.Intervention

Namespace IO

    ''' <summary>
    ''' 外部上下文表（例如 pseudobulk 的 <c>gene_name,gene_id,mean_counts</c> 三列）的解析结果。
    ''' </summary>
    ''' <remarks>
    ''' 一张表里往往同时有基因符号与数据库 ID 两列，究竟用哪一列去匹配模型基因，
    ''' 只能由模型自己决定，所以这里把**所有非数值列**都原样保留成候选，
    ''' 由 <see cref="FuzzyGeneModel.LoadContext"/> 挑命中数最多的那一列。
    ''' </remarks>
    Public Class ContextTable

        ''' <summary>数值列之外的各列表头（自动识别表头时；无表头时为空字符串）。</summary>
        Public Property Headers As String()

        ''' <summary>候选基因名列：<c>Columns(c)(row)</c>。</summary>
        Public Property Columns As String()()

        ''' <summary>与行一一对应的表达值。</summary>
        Public Property Values As Double()

        ''' <summary>因为取值无法解析为有限数而整行跳过的行数。</summary>
        Public Property Invalid As Integer

        ''' <summary>
        ''' 把**上游扰动实验的结果**转换成上下文表，用于迭代扰动（本轮突变体稳态 → 下一轮的对照基线）。
        ''' </summary>
        ''' <param name="result">上游扰动结果（<c>knockout</c>/<c>overexpress</c>/… 的输出）</param>
        ''' <param name="useMutant">
        ''' <c>True</c>（默认）取 <see cref="InterventionResult.MutantMeans"/>：把「扰动之后」的表达谱
        ''' 当作下一轮的初始条件；
        ''' <c>False</c> 取 <see cref="InterventionResult.WildtypeMeans"/>：把上游的对照态当作基线，
        ''' 相当于回到上游扰动之前的参照系。
        ''' </param>
        ''' <returns>单列（<c>gene_name</c>）+ 表达值的上下文表，行序与 <see cref="InterventionResult.GeneNames"/> 一致</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="result"/> 为空</exception>
        ''' <exception cref="InvalidOperationException">
        ''' 上游结果被标记为 <see cref="InterventionResult.Undefined"/>（扰动没有真正发生，例如靶基因不在模型里），
        ''' 或者基因名与表达值数量不一致
        ''' </exception>
        ''' <remarks>
        ''' <para>
        ''' 迭代扰动的语义：第 1 轮在基线 <c>B0</c> 上敲 A 得到 <c>M1</c>；把 <c>M1</c> 注入为上下文之后，
        ''' 第 2 轮敲 B 的 <c>foldchange</c> 就是「在 A 已失活的前提下再敲 B」的效应，
        ''' 而不再是「从原始基线出发」的效应。
        ''' </para>
        ''' <para>
        ''' 这里**不**做任何数值修正：上游结果里的 <c>NaN</c>/<c>Inf</c> 原样带过来，
        ''' 由 <see cref="FuzzyGeneModel.SetContext"/> 统一跳过并计入
        ''' <c>ContextAlignmentDiagnostics.InvalidValues</c>，避免两处各算一遍。
        ''' </para>
        ''' </remarks>
        Public Shared Function FromUpstreamIntervention(result As InterventionResult,
                                                        Optional useMutant As Boolean = True) As ContextTable
            If result Is Nothing Then
                Throw New ArgumentNullException(NameOf(result))
            End If
            If result.Undefined Then
                Throw New InvalidOperationException(
                    "the upstream perturbation is undefined (the target gene was not modelled), so it cannot be used as the context")
            End If

            Dim genes As String() = result.GeneNames
            Dim values As Double() = If(useMutant, result.MutantMeans, result.WildtypeMeans)

            If genes Is Nothing OrElse genes.Length = 0 Then
                Throw New InvalidOperationException("the upstream perturbation result does not carry any gene name")
            End If
            If values Is Nothing Then
                Throw New InvalidOperationException(
                    $"the upstream perturbation result has no {(If(useMutant, "mutant", "wildtype"))} expression vector")
            End If
            If genes.Length <> values.Length Then
                Throw New InvalidOperationException(
                    $"the upstream result has {genes.Length} gene names but {values.Length} expression values")
            End If

            Return New ContextTable With {
                .Headers = New String() {"gene_name"},
                .Columns = New String()() {CType(genes.Clone(), String())},
                .Values = CType(values.Clone(), Double()),
                .Invalid = 0
            }
        End Function

    End Class
End Namespace