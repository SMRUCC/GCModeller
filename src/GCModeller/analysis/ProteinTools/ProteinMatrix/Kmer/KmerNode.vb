
Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.MorganFingerprint

Namespace Kmer

    Public Class KmerNode : Implements IMorganAtom

        Public Property Index As Integer Implements IMorganAtom.Index
        Public Property Code As ULong Implements IMorganAtom.Code
        Public Property Type As String Implements IMorganAtom.Type

        Public Overrides Function ToString() As String
            Return $"[{Index}] {Type} = {Code}"
        End Function

    End Class
End Namespace