Public Class GeneReport

    ''' <summary>
    ''' the promoter region associated gene id
    ''' </summary>
    ''' <returns></returns>
    Public Property locus_tag As String
    ''' <summary>
    ''' the length of the input promoter region
    ''' </summary>
    ''' <returns></returns>
    Public Property length As Integer
    Public Property threshold As Double
    Public Property numOfPromoters As Integer
    Public Property promoterPos As Integer
    Public Property promoterPosLDF As Double
    Public Property components As PromoterComponent()
    Public Property tfBindingSites As TFBindingSite()

End Class

Public Class TFBindingSite
    Public Property regulator As String
    Public Property oligonucleotides As String
    Public Property position As Integer
    Public Property score As Double

    Public Overrides Function ToString() As String
        Return $"       {regulator}:  {oligonucleotides} at position      {position} Score -  {score}"
    End Function
End Class

Public Class PromoterComponent

    Public Property type As String
    Public Property pos As Integer
    Public Property oligonucleotides As String
    Public Property score As Double

    Public Overrides Function ToString() As String
        Return $" {type} at pos.     {pos} {oligonucleotides} Score    {score}"
    End Function

End Class