Imports SMRUCC.genomics.Analysis.FuzzyGene.Data
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace IO

    ''' <summary>
    ''' 调控权重矩阵的行列布局。
    ''' </summary>
    Public Enum RegulationMatrixLayout
        ''' <summary>
        ''' 严格 <c>N×N</c>：行与列都是网络的**全部基因**（顺序与 <see cref="FuzzyGeneNetwork.GeneNames"/> 一致）。
        ''' </summary>
        ''' <remarks>
        ''' 绝大多数列会是全 0（只有真正当过调控因子的基因才有非零列）。
        ''' 3 万基因规模的网络请考虑 <see cref="RegulatorsOnly"/> 或 <see cref="Compact"/>，
        ''' 否则 <c>18533²</c> 个双精度就是 2.7 GB。
        ''' </remarks>
        Square = 0

        ''' <summary>
        ''' 列只保留**真正出现在调控因子位置**的基因（行仍是全部基因）。
        ''' </summary>
        RegulatorsOnly = 1

        ''' <summary>
        ''' 行列都只保留参与调控的基因：行 = 有入边的靶基因，列 = 有出边的调控因子。
        ''' </summary>
        ''' <remarks>适合直接丢给网络可视化工具（无空行空列）。</remarks>
        Compact = 2
    End Enum

    ''' <summary>
    ''' 把带符号的调控边导出成 <c>基因 × 调控因子</c> 的权重矩阵。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 矩阵元素 <c>W(i, j)</c> = 调控因子 <c>j</c> 对靶基因 <c>i</c> 的**带符号调控强度**
    ''' （即 <see cref="RegulationEdgeModel.Weight"/>，等于靶基因模型对第 j 个输入的
    ''' 平均局部灵敏度 <c>∂x̂_i / ∂x_j</c>）：正数激活、负数抑制、0 表示两者之间没有建模出来的边。
    ''' </para>
    ''' <para>
    ''' 返回标准 <see cref="Matrix"/> 对象，因此可以直接用
    ''' <c>geneExpression::write.expr_matrix()</c> 写成 csv，或作为后续网络分析的输入。
    ''' </para>
    ''' </remarks>
    Public Module RegulationMatrixExporter

        ''' <summary>
        ''' 构建调控权重矩阵。
        ''' </summary>
        ''' <param name="network">已训练的调控网络</param>
        ''' <param name="layout">行列布局，默认严格 <c>N×N</c></param>
        ''' <param name="minWeight">
        ''' 绝对值低于该值的边按 0 处理（只影响矩阵内容，不改动网络本身）；0 表示保留全部边
        ''' </param>
        ''' <param name="genes">
        ''' 只导出这些靶基因（行）；<c>Nothing</c> 表示全部。
        ''' 列不受该参数影响，仍按 <paramref name="layout"/> 决定——这样可以保证不同子集导出的矩阵列一致。
        ''' </param>
        ''' <returns>行 = 靶基因、列 = 调控因子的权重矩阵</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="network"/> 为空</exception>
        ''' <exception cref="ArgumentException"><paramref name="genes"/> 与网络基因完全不相交</exception>
        Public Function ToMatrix(network As FuzzyGeneNetwork,
                                 Optional layout As RegulationMatrixLayout = RegulationMatrixLayout.Square,
                                 Optional minWeight As Double = 0.0,
                                 Optional genes As String() = Nothing) As Matrix

            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If

            Dim n As Integer = network.NGene
            Dim edges As RegulationEdgeModel() = If(network.Edges, New RegulationEdgeModel() {})

            ' ---- 1. 决定列（调控因子） ----
            Dim columnGenes As String()

            Select Case layout
                Case RegulationMatrixLayout.Square
                    columnGenes = network.GeneNames
                Case Else
                    Dim sources As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                    For Each edge In edges
                        If edge Is Nothing OrElse String.IsNullOrEmpty(edge.Source) Then
                            Continue For
                        End If

                        Call sources.Add(edge.Source)
                    Next

                    Dim ordered As New List(Of String)

                    For i As Integer = 0 To n - 1
                        If sources.Contains(network.GeneNames(i)) Then
                            Call ordered.Add(network.GeneNames(i))
                        End If
                    Next

                    columnGenes = ordered.ToArray()
            End Select

            ' ---- 2. 决定行（靶基因） ----
            Dim rowGenes As New List(Of String)
            Dim rowFilter As HashSet(Of String) = Nothing

            If genes IsNot Nothing AndAlso genes.Length > 0 Then
                rowFilter = New HashSet(Of String)(genes.Where(Function(g) Not String.IsNullOrEmpty(g)), StringComparer.OrdinalIgnoreCase)
            End If

            If layout = RegulationMatrixLayout.Compact Then
                Dim targets As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                For Each edge In edges
                    If edge Is Nothing OrElse String.IsNullOrEmpty(edge.Target) Then
                        Continue For
                    End If

                    Call targets.Add(edge.Target)
                Next

                For i As Integer = 0 To n - 1
                    Dim name As String = network.GeneNames(i)

                    If targets.Contains(name) AndAlso (rowFilter Is Nothing OrElse rowFilter.Contains(name)) Then
                        Call rowGenes.Add(name)
                    End If
                Next
            Else
                For i As Integer = 0 To n - 1
                    Dim name As String = network.GeneNames(i)

                    If rowFilter Is Nothing OrElse rowFilter.Contains(name) Then
                        Call rowGenes.Add(name)
                    End If
                Next
            End If

            If rowFilter IsNot Nothing AndAlso rowGenes.Count = 0 Then
                Throw New ArgumentException("none of the given genes is present in the network", NameOf(genes))
            End If

            ' ---- 3. 行列索引 ----
            Dim columnIndex As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For j As Integer = 0 To columnGenes.Length - 1
                If Not columnIndex.ContainsKey(columnGenes(j)) Then
                    columnIndex(columnGenes(j)) = j
                End If
            Next

            Dim rows(rowGenes.Count - 1) As DataFrameRow
            Dim rowIndex As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To rowGenes.Count - 1
                rowIndex(rowGenes(i)) = i
                rows(i) = New DataFrameRow With {
                    .geneID = rowGenes(i),
                    .experiments = New Double(columnGenes.Length - 1) {}
                }
            Next

            ' ---- 4. 填权重 ----
            For Each edge As RegulationEdgeModel In edges
                If edge Is Nothing Then
                    Continue For
                End If

                Dim value As Double = edge.Weight

                If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                    Continue For
                End If
                If std.Abs(value) < minWeight Then
                    Continue For
                End If

                Dim row As Integer
                Dim col As Integer

                If Not rowIndex.TryGetValue(If(edge.Target, ""), row) Then
                    Continue For
                End If
                If Not columnIndex.TryGetValue(If(edge.Source, ""), col) Then
                    Continue For
                End If

                rows(row).experiments(col) = value
            Next

            Return New Matrix With {
                .tag = $"fuzzygene regulation weights ({layout})",
                .sampleID = columnGenes,
                .expression = rows
            }
        End Function

    End Module

End Namespace
