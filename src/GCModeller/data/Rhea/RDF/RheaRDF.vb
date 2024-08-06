Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

''' <summary>
''' 
''' </summary>
''' <remarks>
''' https://ftp.expasy.org/databases/rhea/rdf/rhea.rdf.gz
''' </remarks>
''' 
<XmlRoot("RDF", [Namespace]:=RDFEntity.XmlnsNamespace)>
Public Class RheaRDF : Inherits RDF(Of RheaDescription)

    Public Const rh As String = "http://rdf.rhea-db.org/"

    Sub New()
        Call MyBase.New()
        Call xmlns.Add("rh", rh)
    End Sub
End Class

<XmlType("Description", [Namespace]:=RDFEntity.XmlnsNamespace)>
Public Class RheaDescription : Inherits Description

    <XmlElement("subClassOf", [Namespace]:=RDFEntity.rdfs)>
    Public Property subClassOf As Resource
    <XmlElement("id", [Namespace]:=RheaRDF.rh)> Public Property id As RDFProperty
    <XmlElement("accession", [Namespace]:=RheaRDF.rh)> Public Property accession As String
    <XmlElement("equation", [Namespace]:=RheaRDF.rh)> Public Property equation As String
    <XmlElement("status", [Namespace]:=RheaRDF.rh)> Public Property status As Resource
    <XmlElement("ec", [Namespace]:=RheaRDF.rh)> Public Property ec As Resource

    Sub New()
        Call MyBase.New()
    End Sub

End Class