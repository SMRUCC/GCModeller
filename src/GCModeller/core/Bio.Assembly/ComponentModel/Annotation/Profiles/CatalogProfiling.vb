Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.Annotation

    Public Class CatalogProfiling
        Implements INamedValue

        ''' <summary>
        ''' COG/KO/GO, etc
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Overridable Property Catalog As String Implements INamedValue.Key
        Public Property Description As String
        Public Property SubCategory As Dictionary(Of String, CatalogList)

        Public Overrides Function ToString() As String
            Return $"{Catalog} contains {SubCategory.Count} subcategories... {Mid(SubCategory.Keys.GetJson, 1, 20)}..."
        End Function

    End Class
End Namespace