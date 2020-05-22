Imports System.Xml.Serialization

Public Class GeneReport

    ''' <summary>
    ''' the promoter region associated gene id
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property locus_tag As String
    ''' <summary>
    ''' the length of the input promoter region
    ''' </summary>
    ''' <returns></returns>
    Public Property length As Integer
    Public Property threshold As Double
    <XmlAttribute> Public Property numOfPromoters As Integer
    Public Property promoterPos As Integer
    Public Property promoterPosLDF As Double
    <XmlElement> Public Property components As PromoterComponent()
    <XmlElement> Public Property tfBindingSites As TFBindingSite()

End Class

Public Class TFBindingSite
    <XmlAttribute> Public Property regulator As String
    <XmlText> Public Property oligonucleotides As String
    <XmlAttribute> Public Property position As Integer
    <XmlAttribute> Public Property score As Double

    Public Overrides Function ToString() As String
        Return $"       {regulator}:  {oligonucleotides} at position      {position} Score -  {score}"
    End Function
End Class

Public Class PromoterComponent

    <XmlAttribute> Public Property type As String
    <XmlAttribute> Public Property pos As Integer
    <XmlText> Public Property oligonucleotides As String
    <XmlAttribute> Public Property score As Double

    Public Overrides Function ToString() As String
        Return $" {type} at pos.     {pos} {oligonucleotides} Score    {score}"
    End Function

End Class