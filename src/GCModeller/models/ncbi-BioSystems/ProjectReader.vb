Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class ProjectReader

    Dim buffer As StreamPack
    Dim proteins As PtfReader

    Sub New(stream As Stream)
        buffer = New StreamPack(stream)
        proteins = New PtfReader(buffer)
    End Sub

    Public Iterator Function GetProteinFasta() As IEnumerable(Of FastaSeq)
        Using file As Stream = buffer.OpenBlock("/workspace/protein_set.fasta")
            Using reader As New StreamReader(file, Encodings.ASCII.CodePage)

            End Using
        End Using
    End Function
End Class
