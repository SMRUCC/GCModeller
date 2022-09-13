#Region "Microsoft.VisualBasic::0c15a8c982fd59445bc6ed133cd5d02d, GCModeller\models\Networks\Microbiome\PathwayProfile\NetworkVisualizer.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 39
    '    Code Lines: 35
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 1.62 KB


    '     Module ProfileNetworkVisualizer
    ' 
    '         Function: MicrobiomePathwayNetwork
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
