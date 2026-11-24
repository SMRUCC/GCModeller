Imports System.Globalization
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.DataFrame.Formats

Namespace KOBAS

    ''' <summary>
    ''' KOBAS 兼容的表达矩阵读取入口。
    ''' </summary>
    ''' <remarks>
    ''' 对应 KOBAS 3.0 <c>src/kobas/gsea.py</c> 的 <c>read_gct_file</c> 与
    ''' <c>read_txt_file</c>：
    ''' <list type="bullet">
    ''' <item><c>*.gct</c> 直接复用 <see cref="Gct.LoadFile"/>（GCT 1.2/1.3 格式）；</item>
    ''' <item>其余文本文件按 KOBAS 的「制表符分隔、前两列为基因编号与描述」的布局读取。</item>
    ''' </list>
    ''' 两种格式都要求表达矩阵的列数（样本数）与 CLS 表型文件的样本数严格一致。
    ''' </remarks>
    Public Module ExpressionTable

        ''' <summary>
        ''' 读取表达矩阵文件。
        ''' </summary>
        ''' <param name="exprFile">表达矩阵文件路径，支持 <c>*.gct</c> 与 KOBAS 风格的 <c>*.txt</c>。</param>
        ''' <param name="pheno">
        ''' 可选的表型分组信息；给定时会校验样本数量与 <see cref="phenotype.sample_num"/> 是否一致。
        ''' </param>
        ''' <returns>行是基因、列是样本的表达矩阵。</returns>
        ''' <exception cref="NotSupportedException">文件扩展名不受支持时抛出。</exception>
        ''' <exception cref="FormatException">文件内容无法解析时抛出。</exception>
        ''' <exception cref="InvalidOperationException">样本数量与表型文件不一致时抛出。</exception>
        Public Function Load(exprFile As String, Optional pheno As phenotype = Nothing) As Matrix

            Dim expr As Matrix

            If String.Equals(Path.GetExtension(exprFile), ".gct", StringComparison.OrdinalIgnoreCase) Then
                expr = Gct.LoadFile(exprFile).AsMatrix()
            Else
                expr = LoadKobasTxt(exprFile)
            End If

            If expr Is Nothing OrElse expr.size = 0 Then
                Throw New FormatException($"The expression matrix '{exprFile}' contains no gene row.")
            End If

            If pheno IsNot Nothing Then
                Call Validate(expr, pheno)
            End If

            Return expr
        End Function

        ''' <summary>
        ''' 校验表达矩阵的样本数量与表型分组是否一致。
        ''' </summary>
        ''' <param name="expr">表达矩阵。</param>
        ''' <param name="pheno">表型分组。</param>
        ''' <returns>原样返回 <paramref name="expr"/>，便于链式调用。</returns>
        ''' <exception cref="InvalidOperationException">样本数量不一致时抛出。</exception>
        <Extension>
        Public Function Validate(expr As Matrix, pheno As phenotype) As Matrix
            If expr.sampleID.Length <> pheno.sample_num Then
                Throw New InvalidOperationException(
                    "The total of samples in the expression matrix does not match the total samples in the cls file." &
                    $"{vbCrLf}expression: {expr.sampleID.Length}, cls: {pheno.sample_num}")
            End If

            Return expr
        End Function

        ''' <summary>
        ''' 读取 KOBAS 风格的制表符分隔表达矩阵（<c>read_txt_file</c> 的等价实现）。
        ''' </summary>
        ''' <param name="txtFile">文本文件路径。</param>
        ''' <returns>表达矩阵。</returns>
        ''' <remarks>
        ''' 文件布局：第一行为表头（前两列为 <c>Name/Description</c>，其后每列一个样本），
        ''' 之后每行依次为基因编号、描述以及各样本的数值。
        ''' </remarks>
        Public Function LoadKobasTxt(txtFile As String) As Matrix
            Dim lines As String() = File.ReadAllLines(txtFile)

            If lines.Length < 2 Then
                Throw New FormatException($"Bad expression data format in '{txtFile}': at least a header and one data row are required.")
            End If

            Dim header As String() = lines(0).TrimEnd().Split(ControlChars.Tab)
            Dim samples As String() = header.Skip(2).ToArray

            If samples.Length = 0 Then
                Throw New FormatException($"Bad expression data format in '{txtFile}': no sample column was detected.")
            End If

            Dim genes As New List(Of DataFrameRow)

            For i As Integer = 1 To lines.Length - 1
                Dim text As String = lines(i).TrimEnd()

                If text.Length = 0 Then
                    Continue For
                End If

                Dim words As String() = text.Split(ControlChars.Tab)

                If words.Length < samples.Length + 2 Then
                    Throw New FormatException(
                        $"Bad expression data format in '{txtFile}' at line {i + 1}: " &
                        $"expect {samples.Length + 2} column(s) but gets {words.Length}.")
                End If

                Dim values As Double() = New Double(samples.Length - 1) {}

                For j As Integer = 0 To samples.Length - 1
                    Dim raw As String = words(j + 2).Trim()

                    If Not Double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, values(j)) Then
                        Throw New FormatException(
                            $"Bad expression value '{raw}' in '{txtFile}' at line {i + 1}, column {j + 3}.")
                    End If
                Next

                genes.Add(New DataFrameRow With {
                    .geneID = words(0).Trim(),
                    .experiments = values
                })
            Next

            Return New Matrix With {
                .tag = Path.GetFileName(txtFile),
                .sampleID = samples,
                .expression = genes.ToArray
            }
        End Function

        ''' <summary>
        ''' 取得表达矩阵之中与表型标签同序的样本下标。
        ''' </summary>
        ''' <param name="expr">表达矩阵。</param>
        ''' <param name="pheno">表型分组。</param>
        ''' <returns>长度为样本数的下标数组；由于 CLS 文件不包含样本名，此处按位置一一对应。</returns>
        <Extension>
        Public Function SampleOrder(expr As Matrix, pheno As phenotype) As Integer()
            Call Validate(expr, pheno)

            Return Enumerable.Range(0, expr.sampleID.Length).ToArray
        End Function
    End Module
End Namespace
