Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' 分块存储的FASTA序列对象，用于处理大型基因组序列，例如植物基因组，动物基因组这些序列长度超过了2GB的基因组序列
    ''' </summary>
    ''' <remarks>
    ''' all char in this sequence has already been converted to upper case
    ''' </remarks>
    Public Class ChunkedNtFasta

        ReadOnly seq As New BucketSet(Of DNA)()

        ''' <summary>
        ''' 序列的标题/描述
        ''' </summary>
        Public ReadOnly Property title As String

        ''' <summary>
        ''' 序列的总长度（碱基数）
        ''' </summary>
        Public ReadOnly Property Length As Long
            Get
                Return seq.Count
            End Get
        End Property

        Private Sub New(title As String)
            _title = title
        End Sub

        Public Shared Function ReadFromFile(file As String, Optional chunkSize As Integer = ByteSize.MB * 32) As IEnumerable(Of ChunkedNtFasta)
            Using s As Stream = file.OpenReadonly(, verbose:=True)
                Return LoadDocument(s, chunkSize)
            End Using
        End Function

        ''' <summary>
        ''' 从文件流中加载FASTA文档
        ''' </summary>
        ''' <param name="s">输入文件流</param>
        ''' <param name="chunkSize">分块大小，默认32MB</param>
        ''' <returns>FASTA序列对象的枚举</returns>
        Public Shared Iterator Function LoadDocument(s As Stream, Optional chunkSize As Integer = ByteSize.MB * 32) As IEnumerable(Of ChunkedNtFasta)
            Using reader As New StreamReader(s)
                Dim currentFasta As ChunkedNtFasta = Nothing
                Dim currentChunk As New List(Of DNA)()
                Dim line As String = Nothing

                While reader.Peek() >= 0
                    line = reader.ReadLine()

                    If String.IsNullOrWhiteSpace(line) Then
                        Continue While
                    End If

                    ' 检查是否是标题行（以'>'开头）
                    If line(0) = ">"c Then
                        ' 如果已经有正在处理的序列，先完成它
                        If currentFasta IsNot Nothing Then
                            ' 添加最后一个块
                            If currentChunk.Count > 0 Then
                                currentFasta.seq.Add(currentChunk)
                                currentChunk = New List(Of DNA)()
                            End If
                            Yield currentFasta
                        End If

                        ' 开始新的序列
                        ' 去掉'>'符号并去除两端空格
                        currentFasta = New ChunkedNtFasta(line.Substring(1).Trim())
                    Else
                        ' 序列数据行
                        If currentFasta Is Nothing Then
                            Throw New InvalidDataException("FASTA文件格式错误：在标题行之前出现序列数据")
                        End If

                        ' 处理当前行的每个字符
                        For Each c As Char In line
                            If Char.IsWhiteSpace(c) Then
                                Continue For ' 跳过空白字符
                            End If

                            Dim base As DNA = CharToDNA(Char.ToUpperInvariant(c))
                            currentChunk.Add(base)

                            ' 如果当前块达到指定大小，则添加到序列中并创建新块
                            If currentChunk.Count >= chunkSize Then
                                currentFasta.seq.Add(currentChunk)
                                currentChunk = New List(Of DNA)()
                            End If
                        Next
                    End If
                End While

                ' 处理最后一个序列
                If currentFasta IsNot Nothing Then
                    If currentChunk.Count > 0 Then
                        currentFasta.seq.Add(currentChunk)
                    End If
                    Yield currentFasta
                End If
            End Using
        End Function

        ''' <summary>
        ''' all char in this sequence has already been converted to upper case
        ''' </summary>
        ''' <param name="k"></param>
        ''' <returns></returns>
        Public Iterator Function Kmers(k As Integer) As IEnumerable(Of String)
            Dim length As UInteger = Me.Length

            For i As UInteger = 0 To length - k
                Yield GetRegion(i, i + k)
            Next
        End Function

        ''' <summary>
        ''' 获取指定区域的序列字符串
        ''' </summary>
        ''' <param name="left">起始位置（从1开始）</param>
        ''' <param name="right">结束位置（从1开始）</param>
        ''' <returns>指定区域的序列字符串</returns>
        ''' <remarks>
        ''' all char in this sequence has already been converted to upper case
        ''' </remarks>
        Public Function GetRegion(left As Long, right As Long) As String
            ' 将1-based索引转换为0-based索引
            Dim startIndex As Long = left - 1
            Dim endIndex As Long = right - 1

            ' 参数验证
            If startIndex < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(left), "起始位置不能小于1")
            End If
            If endIndex >= seq.Count Then
                Throw New ArgumentOutOfRangeException(NameOf(right), $"结束位置不能超过序列长度 {seq.Count}")
            End If
            If startIndex > endIndex Then
                Throw New ArgumentException("起始位置不能大于结束位置")
            End If

            Dim result As New StringBuilder(CInt(endIndex - startIndex + 1))

            ' 获取指定范围内的DNA碱基并转换为字符
            For Each base As DNA In seq.GetRange(startIndex, endIndex)
                result.Append(DNAToChar(base))
            Next

            Return result.ToString()
        End Function

        ''' <summary>
        ''' 将字符转换为DNA枚举值
        ''' </summary>
        ''' <param name="c">字符</param>
        ''' <returns>对应的DNA碱基</returns>
        Private Shared Function CharToDNA(c As Char) As DNA
            Select Case c
                Case "A"c : Return DNA.dAMP
                Case "G"c : Return DNA.dGMP
                Case "C"c : Return DNA.dCMP
                Case "T"c : Return DNA.dTMP
                Case "R"c : Return DNA.R
                Case "Y"c : Return DNA.Y
                Case "M"c : Return DNA.M
                Case "K"c : Return DNA.K
                Case "S"c : Return DNA.S
                Case "W"c : Return DNA.W
                Case "H"c : Return DNA.H
                Case "B"c : Return DNA.B
                Case "V"c : Return DNA.V
                Case "D"c : Return DNA.D
                Case "N"c : Return DNA.N
                Case Else : Return DNA.NA
            End Select
        End Function

        ''' <summary>
        ''' 将DNA枚举值转换为字符
        ''' </summary>
        ''' <param name="base">DNA碱基</param>
        ''' <returns>对应的字符</returns>
        Private Shared Function DNAToChar(base As DNA) As Char
            Select Case base
                Case DNA.dAMP : Return "A"c
                Case DNA.dGMP : Return "G"c
                Case DNA.dCMP : Return "C"c
                Case DNA.dTMP : Return "T"c
                Case DNA.R : Return "R"c
                Case DNA.Y : Return "Y"c
                Case DNA.M : Return "M"c
                Case DNA.K : Return "K"c
                Case DNA.S : Return "S"c
                Case DNA.W : Return "W"c
                Case DNA.H : Return "H"c
                Case DNA.B : Return "B"c
                Case DNA.V : Return "V"c
                Case DNA.D : Return "D"c
                Case DNA.N : Return "N"c
                Case Else : Return "-"c
            End Select
        End Function

        ''' <summary>
        ''' 获取序列的字符串表示（用于调试，可能不返回完整序列）
        ''' </summary>
        ''' <returns>序列的字符串表示</returns>
        Public Overrides Function ToString() As String
            Return $"{title} (size: {Length}bp, chunks: {seq.Chunks})"
        End Function
    End Class

End Namespace