#Region "Microsoft.VisualBasic::72159d86f15ee8833f7af0d3d8933adf, CLI_tools\Spiderman\PQDijkstraProvider.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::771065c560eabd2d144bbde8d942f720, CLI_tools\Spiderman\PQDijkstraProvider.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    '     Class PQDijkstraProvider
'    ' 
'    '         Constructor: (+1 Overloads) Sub New
'    '         Function: __getIndex, __getNearbyNodeId, Compute, ComputePath, CreateSession
'    '                   getInternodeTraversalCost, GetNearbyNodes, PrintRoutes, SavePathRoutes
'    ' 
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports System.Text
'Imports Microsoft.VisualBasic.CommandLine.Reflection
'Imports Microsoft.VisualBasic.Data.csv.Extensions
'Imports Microsoft.VisualBasic.Data.GraphTheory.Dijkstra
'Imports Microsoft.VisualBasic.Linq
'Imports Microsoft.VisualBasic.Scripting.MetaData
'Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.DataVisualization

'Namespace PathRoutes

'    <Package("SpiderMan.PQDijkstraProvider", Category:=APICategories.UtilityTools, Description:="Tools for finding path in a network.", Publisher:="xie.guigang@gmail.com", Url:="")>
'    Public Class PQDijkstraProvider : Inherits PQDijkstra.PQDijkstraProvider

'        Dim OriginalNodes As NodeAttributes()
'        Dim _nodeHash As Dictionary(Of String, SeqValue(Of NodeAttributes))
'        Dim _nodes As SeqValue(Of NodeAttributes)()
'        Dim NetworkInteractions As Interactions()
'        Dim _IgnoreNodes As String()

'        Sub New(Network As Interactions(), Nodes As NodeAttributes())
'            Call MyBase.New(Nodes.Count)

'            Me._nodes = Nodes.SeqIterator.ToArray
'            Me._nodeHash = _nodes.ToDictionary(Function(x) x.value.ID)
'            Me.OriginalNodes = Nodes
'            Me.NetworkInteractions = Network
'        End Sub

'        <ExportAPI("Session.New")>
'        Public Shared Function CreateSession(Network As KeyValuePair(Of Interactions(), NodeAttributes())) As PQDijkstraProvider
'            Return New PQDijkstraProvider(Network.Key, Network.Value)
'        End Function

'        <ExportAPI("PathRoutes.Compute")>
'        Public Shared Function ComputePath(provider As PQDijkstraProvider, start As String, ends As String, Optional ignores As Generic.IEnumerable(Of Object) = Nothing) As NodeAttributes()
'            If ignores Is Nothing Then
'                Return provider.Compute(start, ends)
'            Else
'                Return provider.Compute(start, ends, (From obj In ignores Let strValue As String = obj.ToString Select strValue).ToArray)
'            End If
'        End Function

'        <ExportAPI("Save.Routes")>
'        Public Shared Function SavePathRoutes(routes As NodeAttributes(), saveto As String) As Boolean
'            Return routes.SaveTo(saveto, False)
'        End Function

'        <ExportAPI("Print.Routes")>
'        Public Shared Function PrintRoutes(provider As PQDijkstraProvider, routes As NodeAttributes()) As String
'            Dim Path As New List(Of Interactions)
'            For i As Integer = 0 To routes.Count - 2
'                Dim NodeA = routes(i), NodeB = routes(i + 1)
'                Dim Interaction = (From itr In provider.NetworkInteractions Where itr.Equals(NodeA.ID, NodeB.ID) Select itr).First
'                Call Path.Add(Interaction)
'            Next

'            Dim sBuilder As StringBuilder = New StringBuilder(1024)
'            For Each node In Path
'                Call sBuilder.AppendLine(String.Format("{0}  {1} --> {2}", String.Format("[{0}  ""{1}""]", node.UniqueId, node.Interaction), node.FromNode, node.ToNode))
'            Next

'            Call Console.WriteLine(sBuilder.ToString)
'            Return sBuilder.ToString
'        End Function

'        Private Function __getIndex(UniqueId As String) As Integer
'            If _nodeHash.ContainsKey(UniqueId) Then
'                Return _nodeHash(UniqueId).i
'            Else
'                Return -1
'            End If
'        End Function

'        Public Overloads Function Compute(start As String, ends As String, Optional IgnoreNodes As String() = Nothing) As NodeAttributes()
'            If IgnoreNodes.IsNullOrEmpty Then
'                _IgnoreNodes = New String() {}
'            Else
'                _IgnoreNodes = (From strValue In IgnoreNodes Select strValue.ToUpper).ToArray
'            End If

'            Dim AdjacencyLQuery = (From itr In NetworkInteractions.AsParallel Where itr.Equals(start, ends) Select itr).ToArray
'            If Not AdjacencyLQuery.IsNullOrEmpty Then  '是直接相邻的两个节点
'                Dim AdjacencyPath = New NodeAttributes() {
'                    OriginalNodes.Take(uniqueId:=start),
'                    OriginalNodes.Take(uniqueId:=AdjacencyLQuery.First.UniqueId),
'                    OriginalNodes.Take(uniqueId:=ends)}
'                Return AdjacencyPath
'            End If

'            Dim idx_start = __getIndex(start), idx_ends = __getIndex(ends)
'            If idx_ends < 0 OrElse idx_start < 0 Then
'                Return New NodeAttributes() {}
'            End If

'            Dim path = Compute(idx_start, idx_ends)
'            Dim nodes = (From x In Me._nodeHash.Values Select x Order By x.i Ascending).ToArray
'            Dim LQuery = (From idx As Integer In path Select nodes(idx).value).ToArray
'            Return LQuery
'        End Function

'        ''' <summary>
'        ''' 获取指定的两个节点之间的权重，不相邻返回-1
'        ''' </summary>
'        ''' <param name="start"></param>
'        ''' <param name="finish"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Protected Overrides Function getInternodeTraversalCost(start As Integer, finish As Integer) As Single
'            Dim NodeA As String = _nodes(start).value.ID
'            Dim NodeB As String = _nodes(finish).value.ID

'            Dim LQuery = (From itr In NetworkInteractions.AsParallel Where itr.Equals(NodeA, NodeB) Select itr).ToArray
'            If LQuery.IsNullOrEmpty Then
'                Return Single.MaxValue
'            Else
'                Return 1
'            End If
'        End Function

'        Protected Overrides Function GetNearbyNodes(startingNode As Integer) As IEnumerable(Of Integer)
'            Dim Node As String = _nodes(startingNode).value.ID
'            Dim LQuery = (From Interaction As Interactions
'                          In NetworkInteractions
'                          Let strId As String = __getNearbyNodeId(Node, Interaction)
'                          Where Not String.IsNullOrEmpty(strId)
'                          Select strId Distinct).ToArray
'            Dim GetIndexLQuery = (From strId As String
'                                  In LQuery.AsParallel
'                                  Let Index As Integer = __getIndex(UniqueId:=strId)
'                                  Select Index).ToArray
'            Return GetIndexLQuery
'        End Function

'        Private Function __getNearbyNodeId(node As String, interaction As Interactions) As String '节点之间的相互作用是具有方向性的
'            If String.Equals(node, interaction.FromNode) Then
'                If Array.IndexOf(_IgnoreNodes, interaction.ToNode) > -1 Then  '该节点被忽略
'                    Return ""
'                Else
'                    Return interaction.ToNode
'                End If
'            Else
'                Return ""
'            End If
'        End Function
'    End Class
'End Namespace
