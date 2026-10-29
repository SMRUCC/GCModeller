Imports System.Globalization
Imports System.IO
Imports Microsoft.VisualBasic.Text

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

    End Class

    ''' <summary>
    ''' 上下文表读取：csv / tsv，自动识别分隔符、表头与数值列。
    ''' </summary>
    Public Module GeneContextReader

        ''' <summary>
        ''' 读取上下文表。
        ''' </summary>
        ''' <param name="path">表格文件路径（csv 或 tsv）</param>
        ''' <param name="valueColumn">
        ''' 数值列的列名；为空时取最后一列
        ''' </param>
        ''' <returns>解析结果（<see cref="ContextTable.Columns"/> 为全部非数值列）</returns>
        ''' <exception cref="FileNotFoundException">文件不存在</exception>
        ''' <exception cref="InvalidDataException">文件为空或没有任何有效数据行</exception>
        Public Function ReadTable(path As String, Optional valueColumn As String = Nothing) As ContextTable
            If String.IsNullOrWhiteSpace(path) Then
                Throw New ArgumentException("the context file path is empty", NameOf(path))
            End If
            If Not File.Exists(path) Then
                Throw New FileNotFoundException($"the context file does not exist: {path}", path)
            End If

            Using reader As New StreamReader(path, Encodings.UTF8.CodePage, detectEncodingFromByteOrderMarks:=True, bufferSize:=1 << 16)
                Dim first As String = ReadNonEmptyLine(reader)

                If first Is Nothing Then
                    Throw New InvalidDataException($"the context file is empty: {path}")
                End If

                Dim delimiter As Char = DetectDelimiter(first)
                Dim header As String() = SplitLine(first, delimiter)
                Dim valueIndex As Integer = header.Length - 1
                Dim hasHeader As Boolean = Not IsNumericField(header(valueIndex))

                If Not String.IsNullOrWhiteSpace(valueColumn) Then
                    If Not hasHeader Then
                        Throw New InvalidDataException(
                            $"the context file has no header line, so the value column '{valueColumn}' cannot be located")
                    End If

                    valueIndex = -1

                    For i As Integer = 0 To header.Length - 1
                        If String.Equals(TrimToken(header(i)), valueColumn.Trim(), StringComparison.OrdinalIgnoreCase) Then
                            valueIndex = i
                            Exit For
                        End If
                    Next

                    If valueIndex < 0 Then
                        Throw New InvalidDataException(
                            $"the value column '{valueColumn}' was not found in the header of {path}: {String.Join(", ", header)}")
                    End If
                End If

                Dim geneIndices As New List(Of Integer)

                For i As Integer = 0 To header.Length - 1
                    If i <> valueIndex Then
                        Call geneIndices.Add(i)
                    End If
                Next

                If geneIndices.Count = 0 Then
                    Throw New InvalidDataException($"the context file does not carry any gene name column: {path}")
                End If

                Dim headers(geneIndices.Count - 1) As String

                For c As Integer = 0 To geneIndices.Count - 1
                    headers(c) = If(hasHeader, TrimToken(header(geneIndices(c))), $"column_{geneIndices(c) + 1}")
                Next

                Dim geneValues As New List(Of String())(1024)
                Dim numeric As New List(Of Double)(1024)
                Dim invalid As Integer = 0

                If Not hasHeader Then
                    Call ParseRow(header, geneIndices, valueIndex, geneValues, numeric, invalid)
                End If

                Dim line As String

                Do
                    line = reader.ReadLine()

                    If line Is Nothing Then
                        Exit Do
                    End If
                    If line.Length = 0 Then
                        Continue Do
                    End If

                    Call ParseRow(SplitLine(line, delimiter), geneIndices, valueIndex, geneValues, numeric, invalid)
                Loop

                If geneValues.Count = 0 Then
                    Throw New InvalidDataException($"no valid expression value was found in the context file: {path}")
                End If

                Dim columns(headers.Length - 1)() As String
                Dim rows As Integer = geneValues.Count

                For c As Integer = 0 To headers.Length - 1
                    Dim column(rows - 1) As String

                    For r As Integer = 0 To rows - 1
                        column(r) = geneValues(r)(c)
                    Next

                    columns(c) = column
                Next

                Return New ContextTable With {
                    .Headers = headers,
                    .Columns = columns,
                    .Values = numeric.ToArray(),
                    .Invalid = invalid
                }
            End Using
        End Function

        Private Sub ParseRow(tokens As String(),
                             geneIndices As List(Of Integer),
                             valueIndex As Integer,
                             ByRef genes As List(Of String()),
                             ByRef values As List(Of Double),
                             ByRef invalid As Integer)

            If tokens Is Nothing OrElse tokens.Length <= valueIndex Then
                invalid += 1
                Return
            End If

            Dim text As String = TrimToken(tokens(valueIndex))
            Dim value As Double

            ' 只接受有限数：NA / NaN / 空值一律跳过，宁可少覆盖也不要悄悄变成 0
            If String.IsNullOrEmpty(text) OrElse
               Not Double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, value) OrElse
               Double.IsNaN(value) OrElse Double.IsInfinity(value) Then

                invalid += 1
                Return
            End If

            Dim names(geneIndices.Count - 1) As String

            For c As Integer = 0 To geneIndices.Count - 1
                names(c) = TrimToken(tokens(geneIndices(c)))
            Next

            Call genes.Add(names)
            Call values.Add(value)
        End Sub

        Private Function ReadNonEmptyLine(reader As StreamReader) As String
            Dim line As String

            Do
                line = reader.ReadLine()

                If line Is Nothing Then
                    Return Nothing
                End If

                If line.Length > 0 Then
                    Return line
                End If
            Loop
        End Function

        ''' <summary>按第一行的制表符/逗号数量判断分隔符。</summary>
        Private Function DetectDelimiter(line As String) As Char
            Dim tabs As Integer = 0
            Dim commas As Integer = 0

            For Each c As Char In line
                If c = ASCII.TAB Then
                    tabs += 1
                ElseIf c = ","c Then
                    commas += 1
                End If
            Next

            Return If(tabs > commas, ASCII.TAB, ","c)
        End Function

        ''' <summary>拆分一行，并去掉字段两侧的空白与引号。</summary>
        Private Function SplitLine(line As String, delimiter As Char) As String()
            Dim tokens As String() = line.Split(delimiter)

            For i As Integer = 0 To tokens.Length - 1
                Dim token As String = tokens(i)

                If token.Length > 0 Then
                    Dim start As Integer = 0
                    Dim [end] As Integer = token.Length - 1

                    While start <= [end] AndAlso (Char.IsWhiteSpace(token(start)) OrElse token(start) = """"c)
                        start += 1
                    End While
                    While [end] >= start AndAlso (Char.IsWhiteSpace(token([end])) OrElse token([end]) = """"c)
                        [end] -= 1
                    End While

                    tokens(i) = If([end] >= start, token.Substring(start, [end] - start + 1), "")
                End If
            Next

            Return tokens
        End Function

        Private Function IsNumericField(text As String) As Boolean
            Dim value As Double
            Dim clean As String = TrimToken(text)

            If clean.Length = 0 Then
                Return False
            End If

            Return Double.TryParse(clean, NumberStyles.Float, CultureInfo.InvariantCulture, value)
        End Function

        Private Function TrimToken(text As String) As String
            If text Is Nothing Then
                Return ""
            End If

            Return text.Trim().Trim(""""c).Trim()
        End Function

    End Module

End Namespace
