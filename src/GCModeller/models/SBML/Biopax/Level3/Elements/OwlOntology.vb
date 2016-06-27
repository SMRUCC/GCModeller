Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DocumentFormat.RDF
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MetaCyc.Biopax.Level3.Elements

    <XmlType("Ontology")> Public Class owlOntology : Inherits RDFEntity
        <XmlElement("imports")> Public Property owlImports As owlImports

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' owl:imports
    ''' </summary>
    <XmlType("imports")> Public Class owlImports : Inherits EntityProperty

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace