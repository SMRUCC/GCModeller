Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
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

    Public Iterator Function GetReactions() As IEnumerable(Of Reaction)
        Dim groups = description _
            .SafeQuery _
            .GroupBy(Function(res) res.GetClassType) _
            .ToDictionary(Function(res) res.Key,
                          Function(res)
                              Return res.ToArray
                          End Function)

    End Function

    Public Shared Function Load(doc As String) As RheaRDF
        Return doc.SolveStream.LoadFromXml(Of RheaRDF)
    End Function

End Class

<XmlType("Description", [Namespace]:=RheaRDF.rh)>
Public Class RheaDescription : Inherits Description

    <XmlElement("subClassOf", [Namespace]:=RDFEntity.rdfs)>
    Public Property subClassOf As Resource()
    <XmlElement("id", [Namespace]:=RheaRDF.rh)> Public Property id As RDFProperty
    <XmlElement("accession", [Namespace]:=RheaRDF.rh)> Public Property accession As String
    <XmlElement("name", [Namespace]:=RheaRDF.rh)> Public Property name As String
    <XmlElement("formula", [Namespace]:=RheaRDF.rh)> Public Property formula As String
    <XmlElement("chebi", [Namespace]:=RheaRDF.rh)> Public Property chebi As String
    <XmlElement("equation", [Namespace]:=RheaRDF.rh)> Public Property equation As String
    <XmlElement("status", [Namespace]:=RheaRDF.rh)> Public Property status As Resource
    <XmlElement("ec", [Namespace]:=RheaRDF.rh)> Public Property ec As Resource

    <XmlElement("directionalReaction", [Namespace]:=RheaRDF.rh)>
    Public Property directionalReaction As Resource()

    <XmlElement("compound", [Namespace]:=RheaRDF.rh)>
    Public Property compound As Resource

    <XmlElement("substrates", [Namespace]:=RheaRDF.rh)> Public Property substrates As Resource()
    <XmlElement("products", [Namespace]:=RheaRDF.rh)> Public Property products As Resource()

    <XmlElement("substratesOrProducts", [Namespace]:=RheaRDF.rh)>
    Public Property substratesOrProducts As Resource()

    Sub New()
        Call MyBase.New()
    End Sub

    Public Function GetClassType() As String
        For Each subclassOf As Resource In Me.subClassOf.SafeQuery
            If subclassOf.resource.StartsWith("http://rdf.rhea-db.org/") Then
                Return subclassOf.resource.Replace("http://rdf.rhea-db.org/", "").Trim(" "c, "/"c)
            End If
        Next

        Return ""
    End Function

End Class