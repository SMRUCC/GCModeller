Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Class ProteinAnnotation

    Public Property proteinID As String
    Public Property description As String
    Public Property GO As String()

    Public Overrides Function ToString() As String
        Return $"Dim {proteinID} As [{description}] = {GO.GetJson}"
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