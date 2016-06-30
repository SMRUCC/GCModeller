Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Cytoscape.Visualization

Namespace Visualization

    Public Class VizMap

        <XmlAttribute("documentVersion")> Public Property documentVersion As String = "3.0"
        <XmlAttribute("id")> Public Property id As String '= "VizMap-2015_07_25-05_45"
        <XmlElement("visualStyle")> Public Property visualStyle As visualStyle()

    End Class
End Namespace