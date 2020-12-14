Imports System.Xml.Serialization

Namespace GPML

    <XmlRoot("Pathway", [Namespace]:="http://pathvisio.org/GPML/2013a")>
    <XmlType("Pathway", [Namespace]:="http://pathvisio.org/GPML/2013a")>
    Public Class Pathway

        <XmlAttribute> Public Property Name As String
        <XmlAttribute> Public Property Version As String
        <XmlAttribute> Public Property Organism As String

        Public Property Comment As Comment

        <XmlElement>
        Public Property BiopaxRef As String()

        Public Property Graphics As Graphics

        <XmlElement> Public Property DataNode As DataNode()
        <XmlElement> Public Property Interaction As Interaction()
        <XmlElement> Public Property Label As Label()
        <XmlElement> Public Property Group As Group()
        Public Property InfoBox As InfoBox()
        ' Public Property Biopax As Biopax.Level3.File

    End Class

End Namespace