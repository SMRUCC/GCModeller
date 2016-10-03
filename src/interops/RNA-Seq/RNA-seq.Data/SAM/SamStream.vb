#Region "Microsoft.VisualBasic::535095c8f1ddc17f8c4a3d763a2407c6, ..\interops\RNA-Seq\RNA-seq.Data\SAM\SamStream.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.IO
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel.SAM

Namespace SAM

    Public Class SamStream

        Public ReadOnly Property FileName As String
        ''' <summary>
        ''' If present, the header must be prior to the alignments. Header lines start With `@', while alignment lines do not.
        ''' (文件的可选头部区域必须要在比对数据区域的前面并且每一行以@符号开始)
        ''' </summary>
        Public ReadOnly Property Head As SAMHeader()

        ReadOnly _encoding As System.Text.Encoding
        ReadOnly fs As FileStream

        Dim Tokens As String() = Nothing
        Dim LastLine As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="handle">The file path of the *.sam file.</param>
        Sub New(handle As String, Optional encoding As System.Text.Encoding = Nothing)
            _FileName = handle
            _encoding = If(encoding Is Nothing, System.Text.Encoding.UTF8, encoding)
            fs = New FileStream(handle, FileMode.Open, FileAccess.Read)

            Call $"Open file stream from handle {FileName.ToFileURL}...".__DEBUG_ECHO
            Call $"File stream encoding is specific as {_encoding.ToString}".__DEBUG_ECHO

            Head = ReadHeaders(fs, _encoding, Tokens, SAM.CHUNK_SIZE)
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

            Do While fs.Position < fs.Length
                readsBuffer = __parserInner(chunkSize)

                For Each reads As AlignmentReads In readsBuffer
                    Yield reads
                Next
#If DEBUG Then
                Call $"*{fs.Position} --> {fs.Length}   ({Math.Round(100 * fs.Position / fs.Length, 2)}%)".__DEBUG_ECHO
#Else
              
#End If
            Loop

            If Not String.IsNullOrEmpty(LastLine) Then
                Yield New AlignmentReads(LastLine)
            End If

            Call "File load job done!".__DEBUG_ECHO
        End Function

        Private Function __parserInner(chunkSize As Integer) As AlignmentReads()
            Dim bytBuffer As Byte()

            If fs.Length - fs.Position > chunkSize Then
                bytBuffer = New Byte(chunkSize - 1) {}
                Call fs.Read(bytBuffer, 0, chunkSize)
            Else
                bytBuffer = New Byte(fs.Length - fs.Position - 1) {}
                Call fs.Read(bytBuffer, 0, fs.Length - fs.Position)
            End If

            Dim s_Data As String = LastLine & _encoding.GetString(bytBuffer)

            Tokens = s_Data.lTokens
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
            Dim bufs As AlignmentReads() =
                LinqAPI.Exec(Of AlignmentReads) <=
 _
                From s As String
                In source.AsParallel
                Where Not String.IsNullOrEmpty(s)
                Select New AlignmentReads(s)

            Call Console.Write(".")

            Return bufs
        End Function

        Private Shared Function ReadHeaders(ByRef Fs As FileStream, encoding As System.Text.Encoding, ByRef LeftArray As String(), CHUNK_SIZE As Integer) As SAMHeader()
            Dim bytsBuf As Byte()

            If Fs.Length < 1024 * 1024 * 128 Then
                bytsBuf = New Byte(Fs.Length - 1) {}
                Call Fs.Read(bytsBuf, 0, Fs.Length)
            Else
                If Fs.Length - Fs.Position > CHUNK_SIZE Then
                    bytsBuf = New Byte(CHUNK_SIZE - 1) {}
                    Call Fs.Read(bytsBuf, 0, CHUNK_SIZE)
                Else
                    bytsBuf = New Byte(Fs.Length - Fs.Position - 1) {}
                    Call Fs.Read(bytsBuf, 0, Fs.Length - Fs.Position)
                End If
            End If

            Dim s_Data As String = encoding.GetString(bytsBuf)
            Dim Tokens As String() = s_Data.lTokens
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
