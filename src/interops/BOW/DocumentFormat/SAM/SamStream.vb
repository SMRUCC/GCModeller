Imports System.IO
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.BOW.DocumentFormat.SAM.DocumentElements
Imports Microsoft.VisualBasic

Namespace DocumentFormat.SAM

    Public Class SamStream

        Public ReadOnly Property FileName As String
        ''' <summary>
        ''' If present, the header must be prior to the alignments. Header lines start With `@', while alignment lines do not.
        ''' (文件的可选头部区域必须要在比对数据区域的前面并且每一行以@符号开始)
        ''' </summary>
        Public ReadOnly Property Head As SAMHeader()

        ReadOnly _encoding As System.Text.Encoding
        ReadOnly Fs As FileStream

        Dim Tokens As String() = Nothing
        Dim LastLine As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="handle">The file path of the *.sam file.</param>
        Sub New(handle As String, Optional encoding As System.Text.Encoding = Nothing)
            _FileName = handle
            _encoding = If(encoding Is Nothing, System.Text.Encoding.UTF8, encoding)
            Fs = New FileStream(handle, FileMode.Open, FileAccess.Read)

            Call $"Open file stream from handle {FileName.ToFileURL}...".__DEBUG_ECHO
            Call $"File stream encoding is specific as {_encoding.ToString}".__DEBUG_ECHO

            Head = ReadHeaders(Fs, _encoding, Tokens, SAM.CHUNK_SIZE)
            LastLine = Tokens.Last
        End Sub

        ''' <summary>
        ''' 调用的时候请不要使用并行化拓展
        ''' </summary>
        ''' <param name="chunkSize"></param>
        ''' <returns></returns>
        Public Iterator Function ReadBlock(Optional chunkSize As Integer = SAM.CHUNK_SIZE) As IEnumerable(Of AlignmentReads)
            Dim TokenCopy As String() = New String(Tokens.Length - 2) {}

            Call Array.ConstrainedCopy(Tokens, 0, TokenCopy, 0, TokenCopy.Length)

            Dim readsBuffer As AlignmentReads() = __parsingSAMReads(TokenCopy)

            For Each reads As AlignmentReads In readsBuffer
                Yield reads
            Next

            Do While Fs.Position < Fs.Length
                readsBuffer = __parserInner(chunkSize)

                For Each reads As AlignmentReads In readsBuffer
                    Yield reads
                Next
#If DEBUG Then
                Call $"*{Fs.Position} --> {Fs.Length}   ({Math.Round(100 * Fs.Position / Fs.Length, 2)}%)".__DEBUG_ECHO
#End If
            Loop

            If Not String.IsNullOrEmpty(LastLine) Then
                Yield New AlignmentReads(LastLine)
            End If

            Call "File load job done!".__DEBUG_ECHO
        End Function

        Private Function __parserInner(chunkSize As Integer) As AlignmentReads()
            Dim ChunkBuffer As Byte()

            If Fs.Length - Fs.Position > chunkSize Then
                ChunkBuffer = New Byte(chunkSize - 1) {}
                Call Fs.Read(ChunkBuffer, 0, chunkSize)
            Else
                ChunkBuffer = New Byte(Fs.Length - Fs.Position - 1) {}
                Call Fs.Read(ChunkBuffer, 0, Fs.Length - Fs.Position)
            End If

            Dim s_Data As String =
                LastLine & _encoding.GetString(ChunkBuffer).Replace(vbLf, "")

            Tokens = s_Data.Split(CChar(vbCr))
            LastLine = Tokens.Last

            Dim TokenCopy As String() = New String(Tokens.Length - 2) {}

            Call Array.ConstrainedCopy(Tokens, 0, TokenCopy, 0, TokenCopy.Length)
            Call Console.Write(".")

            Dim readsBuffer As AlignmentReads() = __parsingSAMReads(TokenCopy)

            Return readsBuffer
        End Function

        ''' <summary>
        ''' 由于这个函数是在后台背景线程之中被调用的，所以这里不再使用并行化了，以提高计算效率
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        Private Shared Function __parsingSAMReads(source As String()) As AlignmentReads()
            Dim ChunkBuffer As AlignmentReads() = (From s As String
                                                   In source.AsParallel
                                                   Where Not String.IsNullOrEmpty(s)
                                                   Select New AlignmentReads(s)).ToArray
            Call Console.Write(".")
            Return ChunkBuffer
        End Function

        Private Shared Function ReadHeaders(ByRef Fs As FileStream, encoding As System.Text.Encoding, ByRef LeftArray As String(), CHUNK_SIZE As Integer) As SAMHeader()
            Dim ChunkBuffer As Byte()

            If Fs.Length - Fs.Position > CHUNK_SIZE Then
                ChunkBuffer = New Byte(CHUNK_SIZE - 1) {}
                Call Fs.Read(ChunkBuffer, 0, CHUNK_SIZE)
            Else
                ChunkBuffer = New Byte(Fs.Length - Fs.Position - 1) {}
                Call Fs.Read(ChunkBuffer, 0, Fs.Length - Fs.Position)
            End If

            Dim s_Data As String = encoding.GetString(ChunkBuffer).Replace(vbLf, "")
            Dim Tokens As String() = s_Data.Split(CChar(vbCr))
            Dim i As Integer
            Dim s As String = Tokens(i)
            Dim Headers As New List(Of String)

            Do While True

                If String.IsNullOrEmpty(s) Then
                    Continue Do
                End If

                If s(0) = "@"c Then
                    Call Headers.Add(s)
                Else
                    Exit Do
                End If

                i += 1
                s = Tokens(i)
            Loop

            LeftArray = Tokens.Skip(i).ToArray

            Dim LQuery = (From str As String In Headers Select New SAMHeader(str)).ToArray
            Return LQuery
        End Function

        Public Overrides Function ToString() As String
            Return FileName
        End Function
    End Class
End Namespace