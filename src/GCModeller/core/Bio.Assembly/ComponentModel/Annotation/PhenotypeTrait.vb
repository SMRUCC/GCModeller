Imports System.Xml.Serialization

Namespace ComponentModel.Annotation

    ''' <summary>
    ''' the metaTraits prediciton result
    ''' </summary>
    Public Class PhenotypeTrait

        <XmlAttribute> Public Property category As String
        <XmlAttribute> Public Property accession As String
        <XmlAttribute> Public Property data_type As String
        <XmlAttribute> Public Property result As String

        Public Property unit As String
        Public Property confidence As Double
        Public Property score As Double
        Public Property cvScore As Double

    End Class
End Namespace