Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DocumentFormat.RDF
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace CytoscapeGraphView.XGMML

    Public Class InnerRDF

        <XmlElement("rdf-Description")> Public Property meta As NetworkMetadata

        Public Overrides Function ToString() As String
            Return meta.GetJson
        End Function
    End Class

    Public Class NetworkMetadata : Inherits RDFEntity

        ''' <summary>
        ''' 节点之间互作的类型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("dc-type")> Public Property InteractionType As String = "Protein-Protein Interaction"
        <XmlElement("dc-description")> Public Property Description As String = "N/A"
        <XmlElement("dc-identifier")> Public Property Identifer As String = "N/A"
        <XmlElement("dc-date")> Public Property [Date] As String

        ''' <summary>
        ''' 网络模型的名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("dc-title")> Public Property Title As String = "Default Network Title"
        <XmlElement("dc-source")> Public Property Source As String = "http://GCModeller.org/"
        <XmlElement("dc-format")> Public Property Format As String = "Cytoscape-XGMML"

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace