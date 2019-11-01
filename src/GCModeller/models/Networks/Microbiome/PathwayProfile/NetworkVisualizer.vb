Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG
Imports numeric = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

Namespace PathwayProfile

    Public Module ProfileNetworkVisualizer

        <Extension>
        Public Function MicrobiomePathwayNetwork(profiles As EnrichmentProfiles(), KEGG As MapRepository, Optional cutoff# = 0.05) As NetworkGraph
            Dim idlist = profiles _
                .Where(Function(map) map.pvalue <= cutoff) _
                .Select(Function(map) map.pathway) _
                .ToArray
            Dim mapGroup = profiles _
                .GroupBy(Function(map) map.pathway) _
                .ToDictionary(Function(g) g.Key,
                              Function(mapProfiles)
                                  Return mapProfiles.ToArray
                              End Function)
            Dim maps = idlist _
                .Select(Function(mapID) KEGG.GetByKey(mapID)) _
                .ToArray
            Dim network As NetworkGraph = maps.BuildNetwork(
                Sub(mapNode)
                    With mapGroup(mapNode.label).Shadows
                        mapNode.data!pvalue = (-numeric.Log10(!pvalue)).Average
                        mapNode.data!profile = !profile.Average
                    End With
                End Sub)

            Return network
        End Function
    End Module
End Namespace