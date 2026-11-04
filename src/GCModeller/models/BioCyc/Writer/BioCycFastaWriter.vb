Imports System.IO
Imports System.Text

Namespace Writer

    ''' <summary>
    ''' BioCyc 数据库之中的序列文件(``protseq.fsa``/``dnaseq.fsa``)写入器。
    ''' 
    ''' 标题行的格式与 Pathway Tools 导出的 dump 保持一致：
    ''' 
    ''' ```
    ''' &gt;gnl|ECOLI|1-PFK-MONOMER gn=fruK 1-phosphofructokinase (2261427..2262365) Escherichia coli K-12
    ''' ```
    ''' 
    ''' 即 ``gnl|ORGID|UNIQUE-ID`` 之后依次是可选的 ``gn=`` 基因名称、描述信息、位置信息与物种名称。
    ''' 读取端(<see cref="Assembly.MetaCyc.File.FileSystem.FastaObjects.ProteinSeq"/>)依赖
    ''' ``|`` 分隔之后的第三个字段来提取对象的 ``UNIQUE-ID``。
    ''' </summary>
    Public Class BioCycFastaWriter

        ''' <summary>
        ''' 序列折行的字符数量，BioCyc 官方 dump 为 60
        ''' </summary>
        ''' <returns></returns>
        Public Property LineWidth As Integer = 60

        Public Property orgId As String
        Public Property organism As String

        ReadOnly buffer As New StringBuilder
        Dim n As Integer

        Sub New()
        End Sub

        Sub New(orgId As String, organism As String)
            Me.orgId = orgId
            Me.organism = organism
        End Sub

        ''' <summary>
        ''' 添加一条序列记录
        ''' </summary>
        ''' <param name="uniqueId">BioCyc 对象的 UNIQUE-ID</param>
        ''' <param name="gene">基因名称，输出为 ``gn=`` 标签</param>
        ''' <param name="description">对象描述</param>
        ''' <param name="location">位置描述，例如 ``(3162744..3163481)``</param>
        ''' <param name="seq">序列字符串</param>
        ''' <returns>当前写入器，便于链式调用</returns>
        Public Function Add(uniqueId As String, seq As String, Optional gene As String = Nothing, Optional description As String = Nothing, Optional location As String = Nothing) As BioCycFastaWriter
            If String.IsNullOrEmpty(uniqueId) OrElse String.IsNullOrEmpty(seq) Then
                Return Me
            End If

            Dim title As New StringBuilder($">gnl|{If(orgId, "UNKNOWN")}|{uniqueId}")

            If Not String.IsNullOrEmpty(gene) Then
                Call title.Append($" gn={gene}")
            End If

            If Not String.IsNullOrEmpty(description) Then
                Call title.Append(" " & flatten(description))
            End If

            If Not String.IsNullOrEmpty(location) Then
                Call title.Append(" " & flatten(location))
            End If

            If Not String.IsNullOrEmpty(organism) Then
                Call title.Append(" " & organism)
            End If

            Call buffer.AppendLine(title.ToString)

            Dim text As String = seq.Trim.ToUpper

            For i As Integer = 0 To text.Length - 1 Step LineWidth
                Call buffer.AppendLine(text.Substring(i, Math.Min(LineWidth, text.Length - i)))
            Next

            n += 1

            Return Me
        End Function

        ''' <summary>
        ''' 当前已经缓存的记录数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer
            Get
                Return n
            End Get
        End Property

        Public Sub Save(path As String)
            Dim dir As String = IO.Path.GetDirectoryName(path)

            If Not String.IsNullOrEmpty(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            Call File.WriteAllText(path, buffer.ToString, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))
        End Sub

        ''' <summary>
        ''' 标题行不允许出现换行符号
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>
        Private Shared Function flatten(text As String) As String
            Return text.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim
        End Function
    End Class

End Namespace
