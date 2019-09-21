Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Class ProteinAnnotation

    <XmlAttribute("id")>
    Public Property proteinID As String
    Public Property description As String
    Public Property GO_terms As XmlList(Of NamedValue)

    Public Overrides Function ToString() As String
        Return $"Dim {proteinID} As [{description}] = {GO_terms.AsEnumerable.Select(Function(term) term.name).GetJson}"
    End Function
End Class

Public Class AnnotationClusters : Inherits XmlDataModel
    Implements IList(Of ProteinAnnotation)

    <XmlElement>
    Public Property proteins As ProteinAnnotation()

    <XmlAttribute>
    Public ReadOnly Property size As Integer Implements IList(Of ProteinAnnotation).size
        Get
            Return proteins.Length
        End Get
    End Property

    Public Iterator Function GenericEnumerator() As IEnumerator(Of ProteinAnnotation) Implements Enumeration(Of ProteinAnnotation).GenericEnumerator
        For Each protein In proteins
            Yield protein
        Next
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of ProteinAnnotation).GetEnumerator
        Yield GenericEnumerator()
    End Function
End Class