Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq

Namespace Writer

    ''' <summary>
    ''' BioCyc 属性-值平面文件(``*.dat``)的写入器。
    ''' 
    ''' ### 转义规则
    ''' 
    ''' 写出端必须与 <see cref="FeatureElement"/> 的解析规则严格对称，否则写出之后的数据无法被
    ''' <see cref="AttrDataCollection(Of T).LoadFile(Stream)"/> 正确读回：
    ''' 
    ''' 1. 值之中的换行符号转换为**续行**：第一行写在 ``NAME - value`` 之中，后续的每一行
    '''    以一个 ``/`` 开头；
    ''' 2. 续行内容本身以 ``/`` 开头的时候，需要再补一个 ``/`` 作为转义(即 ``//xxx``)；
    ''' 3. 行首的 ``#`` 表示注释，出现在续行首字符的时候会造成内容丢失，因此续行统一以 ``/``
    '''    作为前缀，从而 ``#`` 不再位于行首；
    ''' 4. 对象之间使用单独一行的 ``//`` 分隔。
    ''' 
    ''' ### 注解规则
    ''' 
    ''' ``^LABEL - value`` 必须紧跟在它所注解的那个属性值之后，由 <see cref="AttrSlot.Annotations"/>
    ''' 的顺序保证。
    ''' </summary>
    Public Class AttrFileWriter : Implements IDisposable

        ''' <summary>
        ''' 换行符号，默认与 BioCyc 官方 dump 一致使用 ``CRLF``
        ''' </summary>
        ''' <returns></returns>
        Public Property NewLine As String = vbCrLf

        ReadOnly writer As TextWriter
        Dim disposed As Boolean

        Public Sub New(path As String)
            Dim dir As String = IO.Path.GetDirectoryName(path)

            If Not String.IsNullOrEmpty(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            writer = New StreamWriter(path, append:=False, encoding:=New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))
        End Sub

        Public Sub New(stream As Stream)
            writer = New StreamWriter(stream, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))
        End Sub

        ''' <summary>
        ''' 写出 ``#`` 注释块形式的文件头。解析端的 <see cref="FileMeta.readMeta"/> 依赖这些行
        ''' 来恢复 organism/database/version 等元数据信息。
        ''' </summary>
        ''' <param name="meta">文件元数据，未被赋值的字段会使用默认值</param>
        ''' <param name="attributes">当前文件之中可能出现的属性名称列表</param>
        ''' <param name="orthologs">是否包含同源数据</param>
        Public Sub WriteHeader(meta As FileMeta, attributes As IEnumerable(Of String), Optional orthologs As Boolean = False)
            If meta Is Nothing Then
                Throw New ArgumentNullException(NameOf(meta))
            End If

            Dim copyright As String = meta.copyright

            If String.IsNullOrEmpty(copyright) Then
                copyright = "Copyright SMRUCC genomics institute. All Rights Reserved."
            End If

            Call WriteComment(copyright)
            Call WriteComment("")

            If Not meta.authors.IsNullOrEmpty Then
                Call WriteComment("Authors:")

                For Each author As String In meta.authors
                    Call WriteComment("   " & author)
                Next

                Call WriteComment("")
            End If

            Call WriteComment("Please see the license agreement regarding the use of and distribution of this file.")
            Call WriteComment("The format of this file is defined at http://bioinformatics.ai.sri.com/ptools/flatfile-format.html .")
            Call WriteComment("")
            Call WriteComment("Filename: " & If(meta.fileName, "unknown"))
            Call WriteComment("")
            Call WriteComment("Organism: " & If(meta.organism, "unknown"))
            Call WriteComment("Database: " & If(meta.database, "unknown"))
            Call WriteComment("Version: " & If(meta.version?.ToString, "1.0"))
            Call WriteComment("Orthologs: " & orthologs.ToString.ToLower)
            Call WriteComment("Date and time generated: " & If(meta.create_time = Nothing, Date.Now, meta.create_time).ToString("MMMM d, yyyy, HH:mm:ss", CultureInfo.InvariantCulture))
            Call WriteComment("")

            Dim names As String() = attributes.SafeQuery.Distinct.ToArray

            If names.Length > 0 Then
                Call WriteComment("Attributes:")

                For Each name As String In names
                    Call WriteComment("   " & name)
                Next

                Call WriteComment("")
            End If
        End Sub

        ''' <summary>
        ''' 写出一个对象记录以及结尾的 ``//`` 分隔行
        ''' </summary>
        ''' <param name="record"></param>
        Public Sub Write(record As AttrRecord)
            For Each line As String In FormatRecord(record)
                Call writer.Write(line)
                Call writer.Write(NewLine)
            Next

            Call writer.Write("//")
            Call writer.Write(NewLine)

            Call writer.Flush()
        End Sub

        ''' <summary>
        ''' 写出任意文本行(调用者自行保证格式)
        ''' </summary>
        ''' <param name="line"></param>
        Public Sub WriteLine(line As String)
            Call writer.Write(line)
            Call writer.Write(NewLine)
        End Sub

        Public Sub Flush()
            Call writer.Flush()
        End Sub

        ''' <summary>
        ''' 将一个记录格式化为文本行(<see cref="AttrRecord.uniqueId"/> 会作为第一个槽位写出)
        ''' </summary>
        ''' <param name="record"></param>
        ''' <returns></returns>
        Public Shared Iterator Function FormatRecord(record As AttrRecord) As IEnumerable(Of String)
            Yield "UNIQUE-ID - " & sanitize(record.uniqueId).Trim

            For Each slot As AttrSlot In record.slots
                Dim name As String = sanitize(slot.Name).Trim
                Dim lines As String() = SplitValue(slot.Value)

                If lines.Length = 0 Then
                    Continue For
                End If

                Yield $"{name} - {lines(0)}"

                For i As Integer = 1 To lines.Length - 1
                    Yield formatContinuationLine(lines(i))
                Next

                For Each annotation In slot.Annotations.SafeQuery
                    Yield $"^{sanitize(annotation.Name).Trim} - {singleLine(annotation.Value)}"
                Next
            Next
        End Function

        ''' <summary>
        ''' 将一组记录格式化为完整的文本块(包含文件头与对象分隔符号)，用于调试或者内存输出
        ''' </summary>
        ''' <param name="records"></param>
        ''' <returns></returns>
        Public Shared Function Format(records As IEnumerable(Of AttrRecord)) As String
            Dim sb As New StringBuilder

            For Each record As AttrRecord In records.SafeQuery
                For Each line As String In FormatRecord(record)
                    Call sb.AppendLine(line)
                Next

                Call sb.AppendLine("//")
            Next

            Return sb.ToString
        End Function

        ''' <summary>
        ''' 将属性值按照 BioCyc 的续行规则拆分为多行，空值返回空数组
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Shared Function SplitValue(value As String) As String()
            If value Is Nothing Then
                Return New String() {}
            End If

            Dim text As String = sanitize(value) _
                .Replace(vbCrLf, vbLf) _
                .Replace(vbCr, vbLf)

            If text.Trim.Length = 0 Then
                Return New String() {}
            End If

            Return text.Split(ControlChars.Lf)
        End Function

        ''' <summary>
        ''' 续行：以 ``/`` 开头；若内容本身以 ``/`` 开头则再补一个 ``/`` 进行转义
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Private Shared Function formatContinuationLine(line As String) As String
            Dim text As String = line.TrimEnd()

            If text.StartsWith("/") Then
                Return "//" & text.Substring(1)
            Else
                Return "/" & text
            End If
        End Function

        Private Shared Function singleLine(value As String) As String
            Return sanitize(value).Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim
        End Function

        ''' <summary>
        ''' 移除 BioCyc 平面文件不接受的不可打印控制字符(保留 TAB)
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Friend Shared Function sanitize(value As String) As String
            If String.IsNullOrEmpty(value) Then
                Return value
            End If

            Dim sb As New StringBuilder(value.Length)

            For Each ch As Char In value
                Dim code As Integer = AscW(ch)

                If ch = ControlChars.Tab OrElse ch = ControlChars.LF OrElse ch = ControlChars.CR Then
                    Call sb.Append(ch)
                ElseIf code < &H20 OrElse code = &HFFFE OrElse code = &HFFFF Then
                    Continue For
                Else
                    Call sb.Append(ch)
                End If
            Next

            Return sb.ToString
        End Function

        Private Sub WriteComment(text As String)
            If String.IsNullOrEmpty(text) Then
                Call writer.Write("#")
            Else
                Call writer.Write("# " & text)
            End If

            Call writer.Write(NewLine)
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If disposed Then
                Return
            End If

            disposed = True

            Call writer.Flush()
            Call writer.Dispose()
        End Sub
    End Class

End Namespace
