#Region "Microsoft.VisualBasic::14d5b1207a47e770d80a4d83b4c2c366, G:/GCModeller/src/GCModeller/annotations/GSEA/GSEA.KnowledgeBase.Extensions//Metabolism/Metpa/rbc.vb"

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


    '     Class rbc
    ' 
    '         Properties: data, kegg_id, network_id
    ' 
    '         Function: GetImpacts
    ' 
    '     Class rbcList
    ' 
    '         Properties: list
    ' 
    '         Function: (+2 Overloads) calcRbc, GetScoreImpacts
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

    Public Class rbc

        Public Property data As Double()
        Public Property kegg_id As String()
        Public Property network_id As String

        Public Function GetImpacts() As Dictionary(Of String, Double)
            Dim impact As New Dictionary(Of String, Double)

            For i As Integer = 0 To kegg_id.Length - 1
                impact.Add(kegg_id(i), data(i))
            Next

            Return impact
        End Function

    End Class

    Public Class rbcList : Implements TopologyScoreProvider

        Public Property list As Dictionary(Of String, rbc)

        Public Shared Function calcRbc(graphs As NamedValue(Of NetworkGraph)(), multipleOmics As Boolean) As Dictionary(Of String, rbc)
            Return graphs _
                .Select(Function(g) rbcList.calcRbc(g, multipleOmics)) _
                .ToDictionary(Function(map) map.Name,
                              Function(map)
                                  Return map.Value
                              End Function)
        End Function

        Public Shared Function calcRbc(a As NamedValue(Of NetworkGraph), multipleOmics As Boolean) As NamedValue(Of rbc)
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
            Dim rbcVal As Vector = vlist _
                .Select(Function(c)
                            Return c.data(NamesOf.REFLECTION_ID_MAPPING_RELATIVE_BETWEENESS_CENTRALITY)
                        End Function) _
                .Select(AddressOf Val) _
                .ToArray
            Dim rbc As New rbc With {
                .data = rbcVal / rbcVal.Sum,
                .kegg_id = cid,
                .network_id = a.Name
            }

            Return New NamedValue(Of rbc)(a.Name, rbc)
        End Function

        Public Function GetScoreImpacts(mapid As String) As Dictionary(Of String, Double) Implements TopologyScoreProvider.GetScoreImpacts
            Return list(mapid).GetImpacts
        End Function
    End Class
End Namespace
