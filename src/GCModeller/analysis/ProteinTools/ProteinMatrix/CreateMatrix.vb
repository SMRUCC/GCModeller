Imports System.Drawing
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Public Class CreateMatrix

    Dim sgt As SequenceGraphTransform

    ''' <summary>
    ''' get the dimension size of the generated protein matrix via the function <see cref="ToMatrix(FastaSeq)"/>
    ''' </summary>
    ''' <returns>The CNN input size</returns>
    Public ReadOnly Property dimension As Size
        Get
            Return New Size(sgt.alphabets.Length, sgt.alphabets.Length)
        End Get
    End Property

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
