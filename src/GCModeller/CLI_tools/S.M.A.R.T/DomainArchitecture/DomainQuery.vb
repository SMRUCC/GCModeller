Imports SMRUCC.genomics.ProteinModel

Public Class DomainQuery

    Dim ListData As protein()

    Sub New(SMARTDB As SMARTDB)
        ListData = SMARTDB.Proteins
    End Sub

    Public Function Query(DomainId As String) As SequenceModel.FASTA.FastaFile
        Dim LQuery = From Protein In ListData Where Protein.ContainsDomain(DomainId) Select Protein.EXPORT '
        Dim File = CType(LQuery.ToArray, SequenceModel.FASTA.FastaFile)
        Call File.Save(Settings.TEMP & "/" & DomainId & ".fsa")
        Return File
    End Function
End Class
