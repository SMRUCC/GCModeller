Imports System.Xml.Serialization

''' <summary>
''' the mesh Descriptor Record Set xml file
''' </summary>
''' <remarks>
''' which could be download from the ncbi ftp website: 
''' 
''' https://nlmpubs.nlm.nih.gov/projects/mesh/MESH_FILES/xmlmesh/?_gl=1*jikpoo*_ga*MTQ4NzExODI0OS4xNjg3NDAyOTQ4*_ga_7147EPK006*MTcxMTE2MDE3Ny4xLjEuMTcxMTE2MDQzNC4wLjAuMA..*_ga_P1FPTH9PL4*MTcxMTE2MDE3Ny4xLjEuMTcxMTE2MDQzNC4wLjAuMA..
''' </remarks>
Public Class DescriptorRecordSet

    <XmlAttribute> Public Property LanguageCode As String

    <XmlElement>
    Public Property DescriptorRecord As DescriptorRecord()

End Class

Public Class DescriptorRecord

    <XmlAttribute>
    Public Property DescriptorClass As Integer
    Public Property DescriptorUI As String
    Public Property DescriptorName As xmlstring

End Class