
Imports System.Xml.Serialization

Namespace GPML

    Public Enum DataNodeTypes
        GeneProduct
        Metabolite
        Protein
        Pathway
    End Enum

    Public Class DataNode

        <XmlAttribute> Public Property TextLabel As String
        <XmlAttribute> Public Property GraphId As String
        <XmlAttribute> Public Property Type As DataNodeTypes
        <XmlAttribute> Public Property GroupRef As String

        Public Property Comment As Comment
        Public Property Graphics As Graphics
        Public Property Xref As Xref

        Public Overrides Function ToString() As String
            Return $"[{GraphId}] {TextLabel} As {Type}"
        End Function

    End Class

    Public Class Interaction

        <XmlAttribute>
        Public Property GraphId As String
        Public Property BiopaxRef As String
        Public Property Graphics As Graphics
        Public Property Xref As Xref

        Public Overrides Function ToString() As String
            Return $"[{Graphics.Points(0).GraphRef} - {Graphics.Points(1).GraphRef}]"
        End Function

    End Class
End Namespace