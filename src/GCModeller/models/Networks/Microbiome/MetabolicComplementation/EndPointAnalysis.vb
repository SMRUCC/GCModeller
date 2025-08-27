#Region "Microsoft.VisualBasic::c1d95fb462026038a8191e947cf61309, models\Networks\Microbiome\MetabolicComplementation\EndPointAnalysis.vb"

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

    '   Total Lines: 95
    '    Code Lines: 70 (73.68%)
    ' Comment Lines: 11 (11.58%)
    '    - Xml Docs: 54.55%
    ' 
    '   Blank Lines: 14 (14.74%)
    '     File Size: 4.10 KB


    ' Module EndPointAnalysis
    ' 
    '     Function: BuildInternalNetwork, DoMetabolicEndPointsAnalysis, getNodes
    ' 
    '     Sub: buildGraph
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.GraphTheory.Network.Extensions
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data

Public Module EndPointAnalysis

    <Extension>
    Private Function getNodes(graph As NetworkGraph, compounds As IEnumerable(Of CompoundSpecieReference)) As List(Of Node)
        Dim nodes As New List(Of Node)

        For Each compound As CompoundSpecieReference In compounds
            With graph.GetElementByID(compound.ID)
                If .IsNothing Then
                    nodes += graph.CreateNode(compound.ID)
                Else
                    nodes += .ByRef
                End If
            End With
        Next

        Return nodes
    End Function

    <Extension>
    Private Sub buildGraph(graph As NetworkGraph, reactions As IEnumerable(Of Reaction))
        ' 对当前的这个细菌的基因组内的代谢网络进行装配
        ' 使用已经注释出来的KO编号列表，从参考反应模型库之中查询出相应的模型数据
        For Each link As Reaction In reactions
            Dim flux = link.ReactionModel

            ' 节点为kegg compounds
            ' 链接的边为kegg reaction
            Dim reactant = graph.getNodes(compounds:=flux.Reactants)
            Dim products = graph.getNodes(compounds:=flux.Products)

            ' 对代谢物之间创建代谢反应连接边
            For Each r As Node In reactant
                For Each p As Node In products
                    With graph.GetEdges(r, p).SafeQuery.ToArray
                        If .IsNullOrEmpty Then
                            Call graph.CreateEdge(r, p)
                        End If
                    End With
                Next
            Next
        Next
    End Sub

    ''' <summary>
    ''' 通过细菌的基因组内的KO编号列表查询出相对应的代谢反应过程模型，然后将这些代谢反应过程通过代谢物交点组装出代谢网络
    ''' </summary>
    ''' <param name="KO$">某一个细菌物种的基因组内的KO编号列表可以批量的从Uniprot数据库获取得到</param>
    ''' <param name="reactions">KEGG数据库之中的参考代谢反应列表</param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildInternalNetwork(KO$(), reactions As ReactionRepository, Optional nonEnzymetic As Reaction() = Nothing) As NetworkGraph
        Dim graph As New NetworkGraph

        Call graph.buildGraph(reactions.GetByKOMatch(KO))
        Call graph.buildGraph(nonEnzymetic.SafeQuery)

        Return graph
    End Function

    <Extension>
    Public Function DoMetabolicEndPointsAnalysis(taxon As TaxonomyRef, reactions As ReactionRepository, Optional nonEnzymetic As Reaction() = Nothing) As MetabolicEndPoints
        Dim endPoints As (input As Node(), output As Node())
        Dim metabolicGraph As NetworkGraph

        Call $"[{taxon.TaxonomyString.ToString(True)}] Assembling metabolic network.".debug
        metabolicGraph = taxon.genome _
            .Terms _
            .Select(Function(t) t.name) _
            .ToArray _
            .BuildInternalNetwork(reactions, nonEnzymetic)

        Call "Do endpoint analysis".debug
        endPoints = metabolicGraph.EndPoints

        Call $"[{taxon.TaxonomyString.ToString(True)}] {endPoints.input.Length} inputs / {endPoints.output.Length} outputs".info
        Return New MetabolicEndPoints With {
            .secrete = endPoints.output.Keys,
            .uptakes = endPoints.input.Keys,
            .taxonomy = taxon.TaxonomyString.ToString
        }
    End Function
End Module
