Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace RegulatesGraph

    Public Class Entity : Inherits FileStream.Node

        ''' <summary>
        ''' 代谢途径  => 有多少个基因
        ''' 调控位点家族 => 调控多少个基因
        ''' </summary>
        ''' <returns></returns>
        Public Property Size As Integer
    End Class

    Public Class PathwayRegulates : Inherits FileStream.NetworkEdge

        ''' <summary>
        ''' 位点是通过这个基因列表来调控代谢途径的
        ''' </summary>
        ''' <returns></returns>
        Public Property Regulates As String

        Sub New()
            Me.InteractionType = "regulates"
        End Sub
    End Class
End Namespace