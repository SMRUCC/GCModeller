
Imports LANS.SystemsBiology.SequenceModel.FASTA

Namespace Assembly.MetaCyc.File.FileSystem.FastaObjects

    Public Class Proteins : Inherits FastaToken
        Dim Description As String

        Public ReadOnly Property UniqueId As String
            Get
                Return Me.Attributes.Last.Split.First
            End Get
        End Property

        Public Shared Shadows Sub Save(Data As Proteins(), FilePath As String)
            Dim FASTA As FastaFile = New FastaFile
            Call FASTA.AddRange(Data)
            Call FASTA.Save(FilePath)
        End Sub

        Sub New()
        End Sub

        Sub New(fa As FastaToken)
            Attributes = fa.Attributes
            SequenceData = fa.SequenceData
            Description = fa.Attributes.Last
        End Sub
    End Class
End Namespace