Imports SMRUCC.genomics.ComponentModel

Namespace GeneQuantification

    ''' <summary>
    ''' gene abundance result
    ''' </summary>
    Public Class GeneData : Implements IExpressionValue

        Public Property GeneID As String Implements IExpressionValue.Identity
        Public Property Length As Double
        Public Property RawCount As Double
        Public Property RPK As Double
        Public Property TPM As Double Implements IExpressionValue.ExpressionValue
        Public Property FPKM As Double

    End Class
End Namespace


