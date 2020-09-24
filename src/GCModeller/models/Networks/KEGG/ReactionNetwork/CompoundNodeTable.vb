#Region "Microsoft.VisualBasic::073bb54ad1a56b4f6799c1cb36a0c245, models\Networks\KEGG\ReactionNetwork\CompoundNodeTable.vb"

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

    '     Class CompoundNodeTable
    ' 
    '         Properties: values
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: add, containsKey, createCompoundNode
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace ReactionNetwork

    Public Class CompoundNodeTable

        ReadOnly nodes As Dictionary(Of Node)
        ReadOnly g As NetworkGraph

        Friend ReadOnly ignores As Index(Of String)

        Public ReadOnly Property values As IEnumerable(Of Node)
            Get
                Return nodes.Values
            End Get
        End Property

        Default Public ReadOnly Property getNode(id As String) As Node
            Get
                Return nodes(id)
            End Get
        End Property

        Sub New(compounds As IEnumerable(Of NamedValue(Of String)), cpdGroups As Dictionary(Of String, String()), ignores As Index(Of String), g As NetworkGraph, color As Brush)
            nodes = compounds _
                .Where(Function(cpd) Not cpd.Name Like ignores) _
                .GroupBy(Function(a) a.Name) _
                .Select(Function(a) a.First) _
                .Select(Function(cpd As NamedValue(Of String))
                            Return createCompoundNode(cpd, cpdGroups, color)
                        End Function) _
                .ToDictionary
            nodes.Values _
                .DoEach(AddressOf g.AddNode)

            Me.ignores = ignores
            Me.g = g
        End Sub

        Public Function containsKey(nodeLabelId As String) As Boolean
            Return nodes.ContainsKey(nodeLabelId)
        End Function

        Private Shared Function createCompoundNode(cpd As NamedValue(Of String), cpdGroups As Dictionary(Of String, String()), color As Brush) As Node
            Dim type$ = "n/a"

            If cpdGroups.ContainsKey(cpd.Name) Then
                type = cpdGroups(cpd.Name).JoinBy("|")
            End If

            Return New Node With {
                .label = cpd.Name,
                .data = New NodeData With {
                    .label = cpd.Name,
                    .origID = cpd.Value,
                    .color = color,
                    .Properties = New Dictionary(Of String, String) From {
                        {"common_name", cpd.Value},
                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "KEGG Compound"},
                        {"related", type},
                        {"kegg", cpd.Name}
                    }
                }
            }
        End Function

        Public Function add(compoundNode As Node) As Node
            Call nodes.Add(compoundNode)
            Call g.AddNode(compoundNode)

            Return compoundNode
        End Function
    End Class
End Namespace
