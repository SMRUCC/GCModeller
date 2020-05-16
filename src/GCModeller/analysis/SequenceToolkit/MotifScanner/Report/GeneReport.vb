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
    Public Property components As promoterPos()
    Public Property tfBindingSites As TFBindingSite()

End Class

Public Class TFBindingSite
    Public Property regulator As String
    Public Property oligonucleotides As String
    Public Property position As Integer
    Public Property score As Double
End Class

Public Class promoterPos

    Public Property type As String
    Public Property pos As Integer
    Public Property oligonucleotides As String
    Public Property score As Double

End Class