#Region "Microsoft.VisualBasic::f39b1bf760a437d63e32afe42598c913, ..\GCModeller\core\Bio.Assembly\SequenceModel\FASTA\IO\StreamIterator.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language.UnixBash

Namespace SequenceModel.FASTA

    Public Class StreamIterator : Inherits BufferedStream

        Sub New(path As String)
            Call MyBase.New(path, maxBufferSize:=1024 * 1024 * 10)
        End Sub

        ''' <summary>
        ''' Read all sequence from the fasta file.
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function ReadStream() As IEnumerable(Of FastaToken)
            Dim temp As New List(Of String)

            Do While Not Me.EndRead
                Dim block As String() = MyBase.BufferProvider
                temp += block(Scan0)

                For Each line As String In block.Skip(1)
                    If line.IsBlank Then
                        Continue For
                    End If

                    If line.First = ">"c Then
                        Dim fa As FastaToken = FastaToken.ParseFromStream(temp)
                        Yield fa

                        temp.Clear()
                    End If

                    temp += line
                Next
            Loop

            If Not temp.Count = 0 Then
                Yield FastaToken.ParseFromStream(temp)
            End If
        End Function

        ''' <summary>
        ''' 子集里面的序列元素的数目
        ''' </summary>
        ''' <param name="size"></param>
        ''' <returns></returns>
        Public Iterator Function Split(size As Integer) As IEnumerable(Of FastaFile)
            Dim temp As New List(Of FastaToken)
            Dim i As Integer

            Call MyBase.Reset()

            For Each fa As FastaToken In Me.ReadStream
                If i < size Then
                    Call temp.Add(fa)
                    i += 1
                Else
                    i = 0
                    Yield New FastaFile(temp)
                    Call temp.Clear()
                End If
            Next

            If Not temp.Count = 0 Then
                Yield New FastaFile(temp)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="handle">File path or directory.</param>
        ''' <returns></returns>
        Public Shared Iterator Function SeqSource(handle As String, ParamArray ext As String()) As IEnumerable(Of FastaToken)
            If handle.FileExists Then
                For Each fa As FastaToken In New StreamIterator(handle).ReadStream
                    Yield fa
                Next
            Else
                For Each file As String In ls - l - r - wildcards(ext) <= handle
                    For Each nt As FastaToken In New StreamIterator(file).ReadStream
                        Yield nt
                    Next
                Next
            End If
        End Function
    End Class
End Namespace
