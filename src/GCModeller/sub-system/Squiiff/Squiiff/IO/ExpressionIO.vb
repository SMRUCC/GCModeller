Imports System.IO
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports std = System.Math

Namespace IO

    ''' <summary>
    ''' 表达矩阵的读写与预处理。
    '''
    ''' **数据加载复用仓库通用链路**（与 GEARS / SpikingLoop 一致，避免重复实现 CSV 解析）：
    ''' <code>
    ''' Dim matrix As Matrix = Matrix.LoadData(csvPath)              ' 首列基因名、首行样本名
    ''' Dim expr = BnIO.ReadGeneExpressionMatrix(matrix)             ' 转置为 [gene, sample]
    ''' </code>
    ''' 随后再转置为 SquiDiff 使用的 <c>[cell, gene]</c> 布局。
    ''' </summary>
    Public Module ExpressionIO

#Region "加载"

        ''' <summary>
        ''' 走通用加载链读取宽表表达矩阵（行 = 基因、列 = 样本/细胞），返回 <c>[cell, gene]</c>。
        ''' </summary>
        Public Function LoadGeneSampleMatrix(csvPath As String) As CellExpressionMatrix
            If Not File.Exists(csvPath) Then Throw New FileNotFoundException($"表达矩阵文件不存在: {csvPath}", csvPath)

            Dim matrix As Matrix = Matrix.LoadData(csvPath)
            Dim expr = BnIO.ReadGeneExpressionMatrix(matrix)

            Return Transpose(expr.GeneNames, expr.SampleNames, expr.Matrix)
        End Function

        ''' <summary>
        ''' 直接读取 CSV/TSV（首列为行名，首行为列名），不经 BNLearn 生态。
        ''' 用于 demo 自包含数据或外部矩阵。
        ''' </summary>
        Public Function LoadCsv(path As String, Optional delimiter As Char = ","c) As CellExpressionMatrix
            If Not File.Exists(path) Then Throw New FileNotFoundException($"矩阵文件不存在: {path}", path)

            Dim lines = File.ReadAllLines(path)
            If lines.Length < 2 Then Throw New InvalidDataException($"矩阵文件至少需要表头与一行数据: {path}")

            Dim header = SplitLine(lines(0), delimiter)
            Dim geneCount = header.Length - 1
            Dim geneNames(geneCount - 1) As String
            For j As Integer = 1 To header.Length - 1
                geneNames(j - 1) = header(j)
            Next

            Dim cellNames As New List(Of String)
            Dim rows As New List(Of Double())

            For i As Integer = 1 To lines.Length - 1
                If String.IsNullOrWhiteSpace(lines(i)) Then Continue For

                Dim cells = SplitLine(lines(i), delimiter)
                If cells.Length < 2 Then Continue For

                cellNames.Add(cells(0))

                Dim values(geneCount - 1) As Double
                For j As Integer = 1 To std.Min(cells.Length - 1, geneCount)
                    values(j - 1) = ParseDouble(cells(j))
                Next
                rows.Add(values)
            Next

            Return Pack(cellNames.ToArray(), geneNames, rows)
        End Function

        ''' <summary>按细胞名清单筛选列（细胞）与基因子集，返回 <c>[cell, gene]</c>。</summary>
        Public Shared Function Transpose(geneNames As String(), cellNames As String(), geneBySample As Double(,)) As CellExpressionMatrix
            Dim rows = cellNames.Length
            Dim columns = geneNames.Length
            Dim values(rows - 1, columns - 1) As Double

            For i As Integer = 0 To rows - 1
                For j As Integer = 0 To columns - 1
                    values(i, j) = geneBySample(j, i)
                Next
            Next

            Return New CellExpressionMatrix(cellNames, geneNames, values)
        End Function

#End Region

#Region "保存"

        ''' <summary>把表达矩阵写为 CSV（行 = 细胞，列 = 基因）。</summary>
        Public Sub SaveCsv(path As String, matrix As CellExpressionMatrix)
            Dim rows As New List(Of Double())
            For i As Integer = 0 To matrix.NCell - 1
                Dim values(matrix.NGene - 1) As Double
                For j As Integer = 0 To matrix.NGene - 1
                    values(j) = matrix.Values(i, j)
                Next
                rows.Add(values)
            Next

            Call ResultWriter.WriteNumericTable(path, matrix.GeneNames, rows, matrix.CellNames, ",")
        End Sub

#End Region

#Region "预处理"

        ''' <summary>
        ''' 选择高变基因（按跨细胞的方差降序取前 <paramref name="topN"/> 个），返回基因名。
        ''' </summary>
        Public Function SelectHighlyVariableGenes(matrix As CellExpressionMatrix, topN As Integer) As String()
            Dim n = std.Min(topN, matrix.NGene)
            If n <= 0 Then Return New String() {}

            Dim variance(matrix.NGene - 1) As Double
            For j As Integer = 0 To matrix.NGene - 1
                Dim sum As Double = 0.0
                For i As Integer = 0 To matrix.NCell - 1
                    sum += matrix.Values(i, j)
                Next

                Dim mean = sum / std.Max(1, matrix.NCell)
                Dim ss As Double = 0.0
                For i As Integer = 0 To matrix.NCell - 1
                    Dim d = matrix.Values(i, j) - mean
                    ss += d * d
                Next

                variance(j) = ss / std.Max(1, matrix.NCell - 1)
            Next

            Dim order = Enumerable.Range(0, matrix.NGene).ToArray()
            Array.Sort(order, Function(a, b) variance(b).CompareTo(variance(a)))

            Dim selected(n - 1) As String
            For i As Integer = 0 To n - 1
                selected(i) = matrix.GeneNames(order(i))
            Next

            Return selected
        End Function

        ''' <summary>
        ''' 逐基因 z-score 标准化（跨细胞），等价于 <c>GeneExpressionData.Standardize</c> 的口径。
        ''' 方差为 0 的基因置为 0。
        ''' </summary>
        Public Function StandardizeGenes(matrix As CellExpressionMatrix) As CellExpressionMatrix
            Dim values(matrix.NCell - 1, matrix.NGene - 1) As Double

            For j As Integer = 0 To matrix.NGene - 1
                Dim sum As Double = 0.0
                For i As Integer = 0 To matrix.NCell - 1
                    sum += matrix.Values(i, j)
                Next

                Dim mean = sum / std.Max(1, matrix.NCell)
                Dim ss As Double = 0.0
                For i As Integer = 0 To matrix.NCell - 1
                    Dim d = matrix.Values(i, j) - mean
                    ss += d * d
                Next

                Dim sd = std.Sqrt(ss / std.Max(1, matrix.NCell - 1))
                If sd < 1.0E-12 Then sd = 1.0

                For i As Integer = 0 To matrix.NCell - 1
                    values(i, j) = (matrix.Values(i, j) - mean) / sd
                Next
            Next

            Return New CellExpressionMatrix(matrix.CellNames, matrix.GeneNames, values)
        End Function

        ''' <summary>逐细胞 log1p 归一化（每个细胞缩放到相同总计数后取 log1p）。</summary>
        Public Function LogNormalize(matrix As CellExpressionMatrix, Optional targetSum As Double = 10000.0) As CellExpressionMatrix
            Dim values(matrix.NCell - 1, matrix.NGene - 1) As Double

            For i As Integer = 0 To matrix.NCell - 1
                Dim total As Double = 0.0
                For j As Integer = 0 To matrix.NGene - 1
                    total += matrix.Values(i, j)
                Next

                Dim scaleFactor = If(total > 0.0, targetSum / total, 1.0)
                For j As Integer = 0 To matrix.NGene - 1
                    values(i, j) = std.Log(1.0 + matrix.Values(i, j) * scaleFactor)
                Next
            Next

            Return New CellExpressionMatrix(matrix.CellNames, matrix.GeneNames, values)
        End Function

#End Region

#Region "辅助"

        ''' <summary>把行数据打包为矩阵。</summary>
        Public Function Pack(cellNames As String(), geneNames As String(), rows As IEnumerable(Of Double())) As CellExpressionMatrix
            Dim list = rows.ToArray()
            Dim values(list.Length - 1, std.Max(0, geneNames.Length - 1)) As Double

            For i As Integer = 0 To list.Length - 1
                For j As Integer = 0 To std.Min(list(i).Length, geneNames.Length) - 1
                    values(i, j) = list(i)(j)
                Next
            Next

            Return New CellExpressionMatrix(cellNames, geneNames, values)
        End Function

        ''' <summary>把张量（<c>[cell, gene]</c>）包装成表达矩阵。</summary>
        Public Function Pack(cellNames As String(), geneNames As String(), tensor As Microsoft.VisualBasic.MachineLearning.TensorFlow.Tensor) As CellExpressionMatrix
            Return CellExpressionMatrix.FromTensor(tensor, cellNames, geneNames)
        End Function

        Private Function SplitLine(line As String, delimiter As Char) As String()
            If delimiter = ControlChars.Tab Then Return line.Split(delimiter)

            Return line.Split(delimiter)
        End Function

        Private Function ParseDouble(text As String) As Double
            Dim value As Double = 0.0
            If Double.TryParse(text.Trim(), Globalization.NumberStyles.Float,
                               Globalization.CultureInfo.InvariantCulture, value) Then
                Return value
            End If

            Return 0.0
        End Function

#End Region
    End Module
End Namespace
