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

    Public Function GetProteinFasta() As IEnumerable(Of FastaSeq)
        Using file As Stream = buffer.OpenBlock("/workspace/protein_set.fasta")
            Dim cacheLines As String() = New StreamReader(file, Encodings.ASCII.CodePage).ReadToEnd.LineTokens
            Dim load As IEnumerable(Of FastaSeq) = FastaFile.DocParser(cacheLines)

            Return load
        End Using
    End Function
End Class
