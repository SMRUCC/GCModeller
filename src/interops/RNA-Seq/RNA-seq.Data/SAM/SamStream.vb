#Region "Microsoft.VisualBasic::fb90e08d2bbd7d21292708cb7a45e25f, RNA-Seq\RNA-seq.Data\SAM\SamStream.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class SAMStream
    ' 
    '         Properties: Encoding, FileName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: IteratesAllHeaders, IteratesAllReads, SkipSAMHeaders, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace SAM

    ''' <summary>
    ''' 这个对象可以同时兼容小文件以及非常大的测序文件
    ''' </summary>
    Public Class SAMStream

        Public ReadOnly Property FileName As String
        Public ReadOnly Property Encoding As Encodings

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="handle">The file path of the *.sam file.</param>
        Sub New(handle$, Optional encoding As Encodings = Encodings.UTF8)
            _FileName = handle
            _Encoding = encoding
        End Sub

        Public Iterator Function IteratesAllHeaders() As IEnumerable(Of SAMHeader)
            For Each line As String In FileName.IterateAllLines
                If line(Scan0) = "@"c Then
                    Yield New SAMHeader(line)
                Else
                    Exit For
                End If
            Next
        End Function

        Private Shared Function SkipSAMHeaders(ByRef reader As StreamReader) As String
            Dim current As Value(Of String) = ""

            Do While Not reader.EndOfStream
                If (current = reader.ReadLine).First = "@"c Then
                    ' continute move to next line
                Else
                    Return current
                End If
            Loop

            Throw New InvalidDataException("No mapped reads data was found!")
        End Function

        ''' <summary>
        ''' 调用的时候请不要使用并行化拓展
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function IteratesAllReads() As IEnumerable(Of AlignmentReads)
            Using reader As StreamReader = FileName.OpenReader(Encoding.CodePage)

                ' Skips all of the sam headers and returns the first line of the reads data.
                Dim line$ = SkipSAMHeaders(reader)
                Dim reads As AlignmentReads

                Yield New AlignmentReads(line)

                Do While Not reader.EndOfStream
                    line = reader.ReadLine
                    reads = New AlignmentReads(line)

                    Yield reads
                Loop
            End Using
        End Function

        Public Overrides Function ToString() As String
            Return FileName
        End Function
    End Class
End Namespace
