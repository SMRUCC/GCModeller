Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.GEARS.Graph
Imports SMRUCC.genomics.Analysis.GEARS.Training

Namespace IO

    ''' <summary>
    ''' 真实 Perturb-seq / CROP-seq 数据加载工具
    ''' </summary>
    ''' <remarks>
    ''' GEARS 在有实测扰动数据时应当优先使用实测标签训练。这里提供两种文件的读取：
    ''' <list type="bullet">
    ''' <item><description><strong>扰动表达矩阵</strong>（宽表 CSV）：行为基因、列为扰动样本，
    ''' 列名即被扰动基因的组合，多个基因用 <c>+</c> 连接，例如 <c>codY</c>、<c>codY+luxR</c>；
    ''' 列名可选带模式后缀，例如 <c>codY_Knockout</c>、<c>codY+luxR_Knockdown</c>。</description></item>
    ''' <item><description><strong>control 表达谱</strong>（两列 CSV/TSV）：列为 <c>gene,expression</c>，
    ''' 给出野生型（未扰动）基线表达。</description></item>
    ''' </list>
    ''' </remarks>
    Public Module PerturbSeqIO

        ''' <summary>CSV/TSV 支持的分隔符</summary>
        ReadOnly delimiters As Char() = {","c, ControlChars.Tab, ";"c}

        ''' <summary>
        ''' 读取 control 表达谱
        ''' </summary>
        ''' <param name="path">两列文件（gene, expression）路径，首行可为表头</param>
        ''' <param name="geneNames">表达矩阵的基因名列表，决定输出向量的顺序</param>
        ''' <returns>control 表达向量 [numGenes]；文件中缺失的基因回退为 0</returns>
        Public Function LoadControlProfile(path As String, geneNames As String()) As Double()
            Dim map As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
            Dim lines As String() = File.ReadAllLines(path)

            For Each line As String In lines
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If

                Dim tokens As String() = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)

                If tokens.Length < 2 Then
                    Continue For
                End If

                Dim gene As String = tokens(0).Trim()
                Dim value As Double

                If Not Double.TryParse(tokens(1).Trim(), value) Then
                    Continue For
                End If

                map(gene) = value
            Next

            Dim result As Double() = New Double(geneNames.Length - 1) {}

            For i As Integer = 0 To geneNames.Length - 1
                Dim v As Double = 0

                If map.TryGetValue(geneNames(i), v) Then
                    result(i) = v
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 读取真实 Perturb-seq 宽表矩阵并转换为训练样本集合
        ''' </summary>
        ''' <param name="matrixFile">
        ''' 扰动表达矩阵 CSV；行为基因、列为扰动样本，列名形如 <c>codY</c> 或 <c>codY+luxR_Knockout</c>
        ''' </param>
        ''' <param name="controlExpression">control 基线表达向量 [numGenes]</param>
        ''' <param name="graph">基因调控图，用于把基因名映射为节点索引</param>
        ''' <param name="defaultMode">列名未指定干预模式时使用的默认模式</param>
        ''' <param name="combinationSeparator">组合扰动列名中的基因分隔符，默认为 <c>+</c></param>
        ''' <returns>训练样本列表</returns>
        Public Function LoadPerturbSeq(matrixFile As String,
                                        controlExpression As Double(),
                                        graph As GeneRegulatoryGraph,
                                        Optional defaultMode As InterventionMode = InterventionMode.Knockout,
                                        Optional combinationSeparator As String = "+") As List(Of PerturbSeqSample)

            Dim samples As New List(Of PerturbSeqSample)()
            Dim lines As String() = File.ReadAllLines(matrixFile)

            If lines.Length < 2 Then
                Return samples
            End If

            Dim header As String() = SplitLine(lines(0))
            Dim nCol As Integer = header.Length
            Dim geneRows As New List(Of String())()
            Dim rowNames As New List(Of String)()

            For i As Integer = 1 To lines.Length - 1
                If String.IsNullOrWhiteSpace(lines(i)) Then
                    Continue For
                End If

                Dim tokens As String() = SplitLine(lines(i))

                If tokens.Length < nCol Then
                    Continue For
                End If

                rowNames.Add(tokens(0).Trim())
                geneRows.Add(tokens)
            Next

            Dim nGene As Integer = graph.NumGenes
            Dim rowIndex As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To rowNames.Count - 1
                rowIndex(rowNames(i)) = i
            Next

            For c As Integer = 1 To nCol - 1
                Dim colName As String = header(c).Trim()

                If String.IsNullOrEmpty(colName) Then
                    Continue For
                End If

                Dim mode As InterventionMode = defaultMode
                Dim genePart As String = ParseMode(colName, mode)

                If String.IsNullOrEmpty(genePart) Then
                    Continue For
                End If

                Dim pertGenes As String() = genePart.Split(New String() {combinationSeparator}, StringSplitOptions.RemoveEmptyEntries)
                Dim indices As New List(Of Integer)()
                Dim names As New List(Of String)()

                For Each g As String In pertGenes
                    Dim idx As Integer = -1

                    If graph.TryGetGeneIndex(g.Trim(), idx) Then
                        indices.Add(idx)
                        names.Add(graph.GeneNames(idx))
                    End If
                Next

                If indices.Count = 0 Then
                    Continue For
                End If

                ' 组装输入侧表达谱：control 基线 + 被扰动基因改写为干预值
                Dim inputExpr As Double() = CType(controlExpression.Clone(), Double())
                Dim perturbedExpr As Double() = New Double(nGene - 1) {}

                For i As Integer = 0 To nGene - 1
                    Dim v As Double = 0
                    Dim row As Integer = -1

                    If rowIndex.TryGetValue(graph.GeneNames(i), row) Then
                        Double.TryParse(geneRows(row)(c).Trim(), v)
                    End If

                    perturbedExpr(i) = v
                Next

                For Each idx As Integer In indices
                    inputExpr(idx) = perturbedExpr(idx)
                Next

                samples.Add(New PerturbSeqSample With {
                    .PerturbedGeneIndices = indices.ToArray(),
                    .PerturbedGeneNames = names.ToArray(),
                    .ControlExpression = inputExpr,
                    .PerturbedExpression = perturbedExpr,
                    .Label = String.Join(combinationSeparator, names) & "_" & mode.ToString(),
                    .Mode = mode
                })
            Next

            Return samples
        End Function

        ''' <summary>
        ''' 按分隔符切分一行文本
        ''' </summary>
        ''' <param name="line">原始文本行</param>
        ''' <returns>切分后的字段数组</returns>
        Private Function SplitLine(line As String) As String()
            Return line.Split(delimiters, StringSplitOptions.None)
        End Function

        ''' <summary>
        ''' 从列名中解析干预模式后缀
        ''' </summary>
        ''' <param name="columnName">列名，例如 <c>codY+luxR_Knockout</c></param>
        ''' <param name="mode">解析得到的干预模式；未指定时保持调用方传入的默认值</param>
        ''' <returns>去掉模式后缀之后的基因名部分</returns>
        Private Function ParseMode(columnName As String, ByRef mode As InterventionMode) As String
            Dim pos As Integer = columnName.LastIndexOf("_"c)

            If pos <= 0 Then
                Return columnName
            End If

            Dim suffix As String = columnName.Substring(pos + 1).Trim()

            For Each candidate As InterventionMode In New InterventionMode() {
                InterventionMode.Knockout,
                InterventionMode.Knockdown,
                InterventionMode.Overexpression,
                InterventionMode.Custom
            }
                If String.Equals(suffix, candidate.ToString(), StringComparison.OrdinalIgnoreCase) Then
                    mode = candidate

                    Return columnName.Substring(0, pos)
                End If
            Next

            Return columnName
        End Function
    End Module
End Namespace
