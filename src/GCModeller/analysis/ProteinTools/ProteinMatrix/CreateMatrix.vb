Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Public Class CreateMatrix

    Dim sgt As SequenceGraphTransform

    Sub New()
        sgt = New SequenceGraphTransform
        sgt.set_alphabets(AminoAcidObjUtility.AminoAcidLetters.JoinIterates("-").Select(Function(c) c.ToString).ToArray)
    End Sub

    Public Function ToMatrix(prot As FastaSeq) As Double()()

    End Function

End Class
