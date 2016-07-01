Imports System.Xml.Serialization
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Cytoscape.Visualization.visualProperty

Namespace Visualization

    Public Class network : Inherits visualNode
    End Class

    Public MustInherit Class visualNode
        <XmlElement("visualProperty")> Public Property visualPropertys As visualProperty.visualProperty()
        <XmlElement("dependency")> Public Property dependency As dependency()
    End Class
End Namespace