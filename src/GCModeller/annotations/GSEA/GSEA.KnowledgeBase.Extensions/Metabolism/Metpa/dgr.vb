#Region "Microsoft.VisualBasic::61ced995b7c339c9e9aa347c74692c3d, G:/GCModeller/src/GCModeller/annotations/GSEA/GSEA.KnowledgeBase.Extensions//Metabolism/Metpa/dgr.vb"

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

    '   Total Lines: 70
    '    Code Lines: 57
    ' Comment Lines: 0
    '   Blank Lines: 13
    '     File Size: 2.82 KB


    '     Class dgr
    ' 
    '         Properties: dgr, kegg_id, network_id
    ' 
    '         Function: GetImpacts
    ' 
    '     Class dgrList
    ' 
    '         Properties: pathways
    ' 
    '         Function: (+2 Overloads) calcDgr, GetScoreImpacts
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

Namespace Metabolism.Metpa

    Public Class dgr

        Public Property kegg_id As String()
        Public Property dgr As Double()
        Public Property network_id As String

        Public Function GetImpacts() As Dictionary(Of String, Double)
            Dim impact As New Dictionary(Of String, Double)

            For i As Integer = 0 To kegg_id.Length - 1
                impact.Add(kegg_id(i), dgr(i))
            Next

            Return impact
        End Function

    End Class

    Public Class dgrList : Implements TopologyScoreProvider

        Public Property pathways As Dictionary(Of String, dgr)

        Public Shared Function calcDgr(graphs As NamedValue(Of NetworkGraph)(), multipleOmics As Boolean) As Dictionary(Of String, dgr)
            Return graphs _
                .Select(Function(g) dgrList.calcDgr(g, multipleOmics)) _
                .ToDictionary(Function(map) map.Name,
                              Function(map)
                                  Return map.Value
                              End Function)
        End Function

        Public Shared Function calcDgr(a As NamedValue(Of NetworkGraph), multipleOmics As Boolean) As NamedValue(Of dgr)
            Dim vlist As Node() = a.Value.vertex _
                .Where(Function(c)
                           If multipleOmics Then
                               Return True
                           Else
                               Return c.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = CompoundNodeTable.KEGGCompoundNodeType
                           End If
                       End Function) _
                .ToArray
            Dim cid = vlist.Select(Function(c) c.label).ToArray
            Dim dgrVal As Vector = vlist _
                .Select(Function(c)
                            Return c.data(NamesOf.REFLECTION_ID_MAPPING_RELATIVE_DEGREE_CENTRALITY)
                        End Function) _
                .Select(AddressOf Val) _
                .ToArray
            Dim dgr As New dgr With {
                .dgr = dgrVal / dgrVal.Sum,
                .kegg_id = cid,
                .network_id = a.Name
            }

            Return New NamedValue(Of dgr)(a.Name, dgr)
        End Function

        Public Function GetScoreImpacts(mapid As String) As Dictionary(Of String, Double) Implements TopologyScoreProvider.GetScoreImpacts
            Return pathways(mapid).GetImpacts
        End Function
    End Class
End Namespace
