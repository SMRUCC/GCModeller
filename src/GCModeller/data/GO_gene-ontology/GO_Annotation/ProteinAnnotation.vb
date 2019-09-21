Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports Microsoft.VisualBasic.Linq

Public Class ProteinAnnotation

    <XmlAttribute("id")>
    Public Property proteinID As String
    Public Property description As String
    Public Property GO_terms As XmlList(Of NamedValue)

    Public Overrides Function ToString() As String
        Return $"Dim {proteinID} As [{description}] = {GO_terms.AsEnumerable.Select(Function(term) term.name).GetJson}"
    End Function
End Class

Public Class AnnotationClusters : Inherits ListOf(Of ProteinAnnotation)

    <XmlElement>
    Public Property proteins As ProteinAnnotation()

    Protected Overrides Function getSize() As Integer
        Return proteins.Length
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of ProteinAnnotation)
        Return proteins
    End Function
End Class