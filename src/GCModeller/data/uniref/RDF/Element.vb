Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

Public Class Element : Inherits Description

    <XmlElement> Public Property member As Resource()
    <XmlElement> Public Property someMembersClassifiedWith As Resource()
    <XmlElement> Public Property commonTaxon As Resource
    <XmlElement> Public Property sequenceFor As Resource()
    <XmlElement> Public Property memberOf As Resource()

    Public Property modified As DataValue
    Public Property identity As DataValue

    Public Property label As String
    Public Property type As Resource
    Public Property length As DataValue
    Public Property value As String

    Public Property seedFor As Resource
    Public Property representativeFor As Resource
    Public Property reviewed As DataValue
    Public Property mnemonic As String
    Public Property organism As Resource

End Class

Public Class UniRefRDF : Inherits RDF(Of Element)

    <XmlElement("Description", [Namespace]:=RDFEntity.XmlnsNamespace)>
    Public Property data As Element()
End Class