Imports SMRUCC.genomics.SequenceModel.Polypeptides

Namespace ProteinModel.ChouFasmanRules

    Public Class StructuralAnnotation

        Public Property polyseq As AminoAcid()

        Public ReadOnly Property prot As String
            Get
                Return Polypeptide.ToString(From aa As AminoAcid In polyseq Select aa.AminoAcid)
            End Get
        End Property

        Public ReadOnly Property struct As String
            Get
                Return (From aa As AminoAcid In polyseq Select aa.StructureChar).CharString
            End Get
        End Property

    End Class
End Namespace