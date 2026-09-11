' /********************************************************************************/
'
'  Rockhopper —— 比对覆盖度存储
'
'  原始 Rockhopper 在比对阶段把每个重复实验的逐碱基覆盖度压缩为 WIG 格式文件
'  （由 FileOps.java 读写），Replicate 再据此重建表达谱。
'  重写后使用一个简单、可读、可流式处理的文本格式保存 AlignmentCoverage，
'  以彻底摆脱 Oracle.Java 文件层：
'
'      #ROCKHOPPER-COVERAGE	1
'      #NAME	<replicate name>
'      #AVG_LENGTH	<avg read length>
'      #FILE	<original reads file>
'      #SIZE	<n>
'      #PLUS	<c1>	<c2>	...	<cn>
'      #MINUS	<c1>	<c2>	...	<cn>
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Namespace FileIO

    ''' <summary>
    ''' <see cref="Core.AlignmentCoverage"/> 的读写器。
    ''' </summary>
    Public Module CoverageStore

        Private Const HEADER As String = "#ROCKHOPPER-COVERAGE"

        ''' <summary>
        ''' 将一组覆盖度数据写入指定文件（每个复制子一段）。
        ''' </summary>
        Public Sub Save(path As String, coverages As IEnumerable(Of Core.AlignmentCoverage), Optional encoding As Text.Encoding = Nothing)
            If encoding Is Nothing Then encoding = Text.Encoding.UTF8
            Using writer As New StreamWriter(path, False, encoding)
                writer.WriteLine($"{HEADER}{vbTab}1")
                For Each coverage As Core.AlignmentCoverage In coverages
                    If coverage Is Nothing Then
                        writer.WriteLine("#NULL")
                        Continue For
                    End If
                    writer.WriteLine($"#NAME{vbTab}{coverage.Name}")
                    writer.WriteLine($"#FILE{vbTab}{coverage.ReadFileName}")
                    writer.WriteLine($"#AVG_LENGTH{vbTab}{coverage.AvgLengthReads}")
                    writer.WriteLine($"#SIZE{vbTab}{If(coverage.PlusReads Is Nothing, 0, coverage.PlusReads.Length - 1)}")
                    writer.WriteLine($"#PLUS{vbTab}{joinCounts(coverage.PlusReads)}")
                    writer.WriteLine($"#MINUS{vbTab}{joinCounts(coverage.MinusReads)}")
                Next
            End Using
        End Sub

        ''' <summary>
        ''' 从文件读取一组覆盖度数据。
        ''' </summary>
        Public Function Load(path As String, Optional encoding As Text.Encoding = Nothing) As Core.AlignmentCoverage()
            If encoding Is Nothing Then encoding = Text.Encoding.UTF8
            Dim coverages As New List(Of Core.AlignmentCoverage)()
            Dim current As Core.AlignmentCoverage = Nothing

            For Each line As String In File.ReadLines(path, encoding)
                If String.IsNullOrWhiteSpace(line) Then Continue For
                If line.StartsWith("#NULL") Then
                    coverages.Add(Nothing)
                    Continue For
                End If
                If line.StartsWith(HEADER) Then Continue For
                If Not line.StartsWith("#") Then Continue For

                Dim tokens As String() = line.Split(ControlChars.Tab)
                Select Case tokens(0)
                    Case "#NAME"
                        current = New Core.AlignmentCoverage With {.Name = tokenAt(tokens, 1)}
                        coverages.Add(current)
                    Case "#FILE"
                        If current IsNot Nothing Then current.ReadFileName = tokenAt(tokens, 1)
                    Case "#AVG_LENGTH"
                        If current IsNot Nothing Then current.AvgLengthReads = CLng(Val(tokenAt(tokens, 1)))
                    Case "#SIZE"
                        If current IsNot Nothing Then
                            Dim size As Integer = CInt(Val(tokenAt(tokens, 1)))
                            current.PlusReads = New Integer(size) {}
                            current.MinusReads = New Integer(size) {}
                        End If
                    Case "#PLUS"
                        If current IsNot Nothing Then current.PlusReads = parseCounts(tokens, If(current.PlusReads Is Nothing, 0, current.PlusReads.Length - 1))
                    Case "#MINUS"
                        If current IsNot Nothing Then current.MinusReads = parseCounts(tokens, If(current.MinusReads Is Nothing, 0, current.MinusReads.Length - 1))
                End Select
            Next

            Return coverages.ToArray
        End Function

        Private Function joinCounts(counts As Integer()) As String
            If counts Is Nothing Then Return ""
            Return String.Join(vbTab, counts.Skip(1))
        End Function

        Private Function parseCounts(tokens As String(), size As Integer) As Integer()
            Dim counts As Integer() = New Integer(size) {}
            Dim n As Integer = System.Math.Min(size, tokens.Length - 1)
            For i As Integer = 1 To n
                counts(i) = CInt(Val(tokens(i)))
            Next
            Return counts
        End Function

        Private Function tokenAt(tokens As String(), index As Integer) As String
            If index < tokens.Length Then Return tokens(index)
            Return ""
        End Function

    End Module

End Namespace
