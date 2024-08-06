Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

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
        Dim objs = description.GroupBy(Function(o) o.about) _
            .ToDictionary(Function(o) o.Key,
                          Function(o)
                              Return o.ToArray
                          End Function)

        For Each r As RheaDescription In groups!DirectionalReaction
            Dim left = r.substrates.Select(Function(c) GetCompounds(c, objs).Select(Function(ci) New SideCompound("substrate", ci))).IteratesALL.GroupBy(Function(c) c.compound.entry) _
                .Select(Function(c) c.First).ToArray
            Dim right = r.products.Select(Function(c) GetCompounds(c, objs).Select(Function(ci) New SideCompound("product", ci))).IteratesALL.GroupBy(Function(c) c.compound.entry) _
                .Select(Function(c) c.First).ToArray

            Yield New Reaction With {
                .definition = r.equation,
                .entry = r.accession,
                .enzyme = r.GetECNumber.ToArray,
                .equation = Equation.TryParse(.definition),
                .compounds = left.JoinIterates(right).ToArray
            }
        Next

        For Each r As RheaDescription In groups!BidirectionalReaction
            Dim compounds = r.substratesOrProducts _
                .Select(Function(c) GetCompounds(c, objs).Select(Function(ci) New SideCompound("*", ci))) _
                .IteratesALL _
                .GroupBy(Function(c) c.compound.entry) _
                .Select(Function(c) c.First) _
                .ToArray

            Yield New Reaction With {
                .definition = r.equation,
                .entry = r.accession,
                .enzyme = r.GetECNumber.ToArray,
                .equation = Equation.TryParse(.definition),
                .compounds = compounds
            }
        Next
    End Function

    Shared ReadOnly compoundType As Index(Of String) = {"Compound", "GenericCompound", "SmallMolecule",
            "GenericPolypeptide", "GenericPolynucleotide", "GenericHeteropolysaccharide",
            "GenericSmallMolecule", "Polymer"}

    Private Shared Iterator Function GetCompounds(ref As Resource, objs As Dictionary(Of String, RheaDescription())) As IEnumerable(Of Compound)
        Dim links = objs(ref.resource)
        Dim compound As Compound

        If links.Length = 1 AndAlso links(0).GetClassType Like compoundType Then
            Yield links(0).GetCompound
            Return
        End If

        For Each link As RheaDescription In links
            If link.contains IsNot Nothing Then
                compound = GetCompounds(link.contains, objs)

                If Not compound Is Nothing Then
                    Yield compound
                End If
            End If
            If link.contains1 IsNot Nothing Then
                compound = GetCompounds(link.contains1, objs)

                If Not compound Is Nothing Then
                    Yield compound
                End If
            End If

            If Not link.compound Is Nothing Then
                Yield GetCompounds(link.compound, objs)
            End If
        Next
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
    <XmlElement("ec", [Namespace]:=RheaRDF.rh)> Public Property ec As Resource()

    <XmlElement("directionalReaction", [Namespace]:=RheaRDF.rh)>
    Public Property directionalReaction As Resource()

    <XmlElement("compound", [Namespace]:=RheaRDF.rh)>
    Public Property compound As Resource

    <XmlElement("contains", [Namespace]:=RheaRDF.rh)> Public Property contains As Resource
    <XmlElement("contains1", [Namespace]:=RheaRDF.rh)> Public Property contains1 As Resource

    <XmlElement("substrates", [Namespace]:=RheaRDF.rh)> Public Property substrates As Resource()
    <XmlElement("products", [Namespace]:=RheaRDF.rh)> Public Property products As Resource()

    <XmlElement("substratesOrProducts", [Namespace]:=RheaRDF.rh)>
    Public Property substratesOrProducts As Resource()

    Sub New()
        Call MyBase.New()
    End Sub

    Public Function GetClassType() As String
        If subClassOf Is Nothing Then
            Return ""
        End If

        Static i32 As New Regex("\d+")

        For Each subclassOf As Resource In Me.subClassOf
            If subclassOf.resource.StartsWith("http://rdf.rhea-db.org/") Then
                Dim cls = subclassOf.resource.Replace("http://rdf.rhea-db.org/", "").Trim(" "c, "/"c)

                If Not i32.Match(cls).Success Then
                    Return cls
                End If
            End If
        Next

        Return ""
    End Function

    Public Iterator Function GetECNumber() As IEnumerable(Of String)
        If ec Is Nothing Then
            Return
        End If

        For Each num As Resource In ec
            Dim m As Match = ECNumber.r.Match(num.resource)

            If m.Success Then
                Yield m.Value
            End If
        Next
    End Function

    Public Function GetCompound() As Compound
        Return New Compound With {
            .entry = accession,
            .name = name,
            .formula = formula
        }
    End Function

End Class