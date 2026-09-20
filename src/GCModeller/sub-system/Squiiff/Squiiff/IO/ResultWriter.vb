Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace IO

    ''' <summary>
    ''' 统一的结果导出工具（分隔符默认逗号，可由表格软件或绘图工具直接读取）。
    '''
    ''' 约定（与同目录 SpikingLoop / GEARS 的做法一致）：
    ''' <list type="bullet">
    ''' <item>数值一律用 <c>G8</c> + 不变文化格式化，避免区域设置导致的小数点差异；</item>
    ''' <item>自动创建目标目录；</item>
    ''' <item>文件编码为 UTF-8（带 BOM，便于 Excel 正确识别中文表头）。</item>
    ''' </list>
    ''' </summary>
    Public Module ResultWriter

        ''' <summary>默认分隔符。</summary>
        Public Const DefaultDelimiter As String = ","

        ''' <summary>写出数值表（可选行标签列）。</summary>
        Public Sub WriteNumericTable(path As String, header As String(), rows As IEnumerable(Of Double()),
                                     Optional rowLabels As String() = Nothing,
                                     Optional delimiter As String = DefaultDelimiter)
            Dim lines As New List(Of String)
            lines.Add(ComposeHeader(header, rowLabels IsNot Nothing, delimiter))

            Dim index As Integer = 0
            For Each row In rows
                Dim cells As New List(Of String)
                If rowLabels IsNot Nothing Then
                    cells.Add(Escape(If(index < rowLabels.Length, rowLabels(index), String.Empty), delimiter))
                End If

                For Each value In row
                    cells.Add(Format(value))
                Next

                lines.Add(String.Join(delimiter, cells))
                index += 1
            Next

            Call WriteLines(path, lines)
        End Sub

        ''' <summary>写出文本表（可选行标签列）。</summary>
        Public Sub WriteTextTable(path As String, header As String(), rows As IEnumerable(Of String()),
                                  Optional rowLabels As String() = Nothing,
                                  Optional delimiter As String = DefaultDelimiter)
            Dim lines As New List(Of String)
            lines.Add(ComposeHeader(header, rowLabels IsNot Nothing, delimiter))

            Dim index As Integer = 0
            For Each row In rows
                Dim cells As New List(Of String)
                If rowLabels IsNot Nothing Then
                    cells.Add(Escape(If(index < rowLabels.Length, rowLabels(index), String.Empty), delimiter))
                End If

                For Each value In row
                    cells.Add(Escape(value, delimiter))
                Next

                lines.Add(String.Join(delimiter, cells))
                index += 1
            Next

            Call WriteLines(path, lines)
        End Sub

        ''' <summary>写出矩阵（行 = 第 0 维，列 = 第 1 维）。</summary>
        Public Sub WriteMatrix(path As String, matrix As Tensor, rowLabels As String(), columnLabels As String(),
                               Optional rowNameHeader As String = "gene",
                               Optional delimiter As String = DefaultDelimiter)
            Dim data = matrix.Data
            Dim rows = matrix.Shape(0)
            Dim cols = matrix.Shape(1)

            Dim lines As New List(Of String)
            lines.Add(ComposeHeader(columnLabels, True, delimiter, rowNameHeader))

            For r As Integer = 0 To rows - 1
                Dim cells As New List(Of String)
                cells.Add(Escape(If(rowLabels IsNot Nothing AndAlso r < rowLabels.Length, rowLabels(r), $"row{r + 1}"), delimiter))

                Dim offset = r * cols
                For c As Integer = 0 To cols - 1
                    cells.Add(Format(data(offset + c)))
                Next

                lines.Add(String.Join(delimiter, cells))
            Next

            Call WriteLines(path, lines)
        End Sub

        ''' <summary>写出"名称 / 数值"两列清单。</summary>
        Public Sub WriteKeyValues(path As String, items As IEnumerable(Of KeyValuePair(Of String, Double)),
                                  Optional delimiter As String = DefaultDelimiter)
            Dim lines As New List(Of String)
            lines.Add($"name{delimiter}value")

            For Each item In items
                lines.Add($"{Escape(item.Key, delimiter)}{delimiter}{Format(item.Value)}")
            Next

            Call WriteLines(path, lines)
        End Sub

        ''' <summary>写出自由文本行（每行原样输出）。</summary>
        Public Sub WriteText(path As String, lines As IEnumerable(Of String))
            Call WriteLines(path, lines)
        End Sub

        ''' <summary>列出目录下的全部导出文件（文件名 + 大小），用于报告末尾罗列结果。</summary>
        Public Function ListFiles(outputDirectory As String) As String()
            If Not System.IO.Directory.Exists(outputDirectory) Then Return New String() {}

            Dim result As New List(Of String)
            For Each file In System.IO.Directory.GetFiles(outputDirectory)
                Dim info As New FileInfo(file)
                result.Add($"{info.Name,-46} {info.Length,10:N0} bytes")
            Next

            result.Sort()
            Return result.ToArray()
        End Function

        ''' <summary>把 <see cref="Double"/> 格式化为 <c>G8</c> 不变文化字符串。</summary>
        Public Function Format(value As Double) As String
            If Double.IsNaN(value) Then Return "NaN"
            If Double.IsPositiveInfinity(value) Then Return "Inf"
            If Double.IsNegativeInfinity(value) Then Return "-Inf"

            Return value.ToString("G8", Globalization.CultureInfo.InvariantCulture)
        End Function

        Private Function ComposeHeader(header As String(), includeRowLabel As Boolean,
                                       delimiter As String,
                                       Optional rowLabelHeader As String = "label") As String
            Dim cells As New List(Of String)
            If includeRowLabel Then cells.Add(Escape(rowLabelHeader, delimiter))

            If header IsNot Nothing Then
                For Each item In header
                    cells.Add(Escape(item, delimiter))
                Next
            End If

            Return String.Join(delimiter, cells)
        End Function

        Private Function Escape(value As String, delimiter As String) As String
            If String.IsNullOrEmpty(value) Then Return String.Empty

            If value.IndexOfAny({","c, ControlChars.Tab, """"c, ControlChars.Cr, ControlChars.Lf}) >= 0 Then
                Return """" & value.Replace("""", """""") & """"
            End If

            Return value
        End Function

        Private Sub WriteLines(path As String, lines As IEnumerable(Of String))
            Dim parentDirectory = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path))
            If Not String.IsNullOrEmpty(parentDirectory) AndAlso Not System.IO.Directory.Exists(parentDirectory) Then
                Call System.IO.Directory.CreateDirectory(parentDirectory)
            End If

            File.WriteAllLines(path, lines, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=True))
        End Sub
    End Module
End Namespace
