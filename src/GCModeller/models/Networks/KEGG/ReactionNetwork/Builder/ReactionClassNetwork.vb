#Region "Microsoft.VisualBasic::0dddcb7c817a6490fb1bd114b892bb47, GCModeller\models\Networks\KEGG\ReactionNetwork\Builder\ReactionClassNetwork.vb"

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

    '   Total Lines: 60
    '    Code Lines: 38
    ' Comment Lines: 12
    '   Blank Lines: 10
    '     File Size: 2.52 KB


    '     Class ReactionClassNetwork
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: createEdges
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ReactionNetwork

    Public Class ReactionClassNetwork : Inherits BuilderBase

        ReadOnly classIndex As Index(Of String)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="br08901">代谢反应数据</param>
        ''' <param name="compounds">KEGG化合物编号，``{kegg_id => compound name}``</param>
        Sub New(br08901 As IEnumerable(Of ReactionTable),
                compounds As IEnumerable(Of NamedValue(Of String)),
                reactionClass As IEnumerable(Of ReactionClassTable),
                Optional ignoresCommonList As Boolean = True,
                Optional edgeFilter As EdgeFilterEngine = EdgeFilterEngine.ReactionLinkFilter,
                Optional randomLayout As Boolean = True)

            Call MyBase.New(br08901, compounds, blue, ignoresCommonList, edgeFilter, randomLayout)

            classIndex = ReactionClassTable.IndexTable(reactionClass)
        End Sub

        ''' <summary>
        ''' 两个代谢物必须要存在reaction class信息才会添加一条边
        ''' </summary>
        ''' <param name="commonsReactionId"></param>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        Protected Overrides Sub createEdges(commonsReactionId() As String, a As Node, b As Node)
            Dim key As String = ReactionClassTable.CreateIndexKey(a.label, b.label)

            ' 没有边连接
            If Not key Like classIndex Then
                Return
            End If

            Dim rid = commonsReactionId.Select(Function(id) networkBase(id)).GetGeneSymbols

            Call New Edge With {
               .U = a,
               .V = b,
               .data = New EdgeData With {
                   .Properties = New Dictionary(Of String, String) From {
                       {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, rid.EC.Distinct.JoinBy(", ")},
                       {"kegg", commonsReactionId.GetJson}
                   }
               },
               .weight = rid.geneSymbols.TryCount
           }.DoCall(AddressOf addNewEdge)
        End Sub
    End Class
End Namespace
