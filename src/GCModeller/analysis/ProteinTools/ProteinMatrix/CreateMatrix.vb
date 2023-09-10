Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Public Class CreateMatrix

    Dim sgt As SequenceGraphTransform

    Sub New()
        Dim allChars As String() = AminoAcidObjUtility _
            .AminoAcidLetters _
            .JoinIterates("-") _
            .Select(Function(c) c.ToString) _
            .ToArray

        sgt = New SequenceGraphTransform
        sgt.set_alphabets(allChars)
    End Sub

    Public Function ToMatrix(prot As FastaSeq) As Double()()
        Dim v = sgt.fit(prot.SequenceData)
        Dim m = sgt.TranslateMatrix(v)

        Return m
    End Function

End Class
