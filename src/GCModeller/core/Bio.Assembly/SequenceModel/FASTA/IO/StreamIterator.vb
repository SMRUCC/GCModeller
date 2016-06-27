Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic

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
    End Class
End Namespace