Imports System.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.DataVisualization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.DataVisualization.Network.Dijkstra
Imports Microsoft.VisualBasic.Linq

Namespace PathRoutes

    <[PackageNamespace]("SpiderMan.PQDijkstraProvider", Category:=APICategories.UtilityTools, Description:="Tools for finding path in a network.", Publisher:="xie.guigang@gmail.com", Url:="")>
    Public Class PQDijkstraProvider : Inherits PQDijkstra.PQDijkstraProvider

        Dim OriginalNodes As NodeAttributes()
        Dim _nodeHash As Dictionary(Of String, SeqValue(Of NodeAttributes))
        Dim _nodes As SeqValue(Of NodeAttributes)()
        Dim NetworkInteractions As Interactions()
        Dim _IgnoreNodes As String()

        Sub New(Network As Interactions(), Nodes As NodeAttributes())
            Call MyBase.New(Nodes.Count)

            Me._nodes = Nodes.SeqIterator.ToArray
            Me._nodeHash = _nodes.ToDictionary(Function(x) x.obj.Identifier)
            Me.OriginalNodes = Nodes
            Me.NetworkInteractions = Network
        End Sub

        <ExportAPI("Session.New")>
        Public Shared Function CreateSession(Network As KeyValuePair(Of Interactions(), DataVisualization.NodeAttributes())) As PQDijkstraProvider
            Return New PQDijkstraProvider(Network.Key, Network.Value)
        End Function

        <ExportAPI("PathRoutes.Compute")>
        Public Shared Function ComputePath(provider As PQDijkstraProvider, start As String, ends As String, Optional ignores As Generic.IEnumerable(Of Object) = Nothing) As DataVisualization.NodeAttributes()
            If ignores.IsNullOrEmpty Then
                Return provider.Compute(start, ends)
            Else
                Return provider.Compute(start, ends, (From obj In ignores Let strValue As String = obj.ToString Select strValue).ToArray)
            End If
        End Function

        <ExportAPI("Save.Routes")>
        Public Shared Function SavePathRoutes(routes As DataVisualization.NodeAttributes(), saveto As String) As Boolean
            Call routes.SaveTo(saveto, False)
            Return True
        End Function

        <ExportAPI("Print.Routes")>
        Public Shared Function PrintRoutes(provider As PQDijkstraProvider, routes As DataVisualization.NodeAttributes()) As String
            Dim Path As List(Of DataVisualization.Interactions) = New List(Of DataVisualization.Interactions)
            For i As Integer = 0 To routes.Count - 2
                Dim NodeA = routes(i), NodeB = routes(i + 1)
                Dim Interaction = (From itr In provider.NetworkInteractions Where itr.Equals(NodeA.Identifier, NodeB.Identifier) Select itr).First
                Call Path.Add(Interaction)
            Next

            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            For Each node In Path
                Call sBuilder.AppendLine(String.Format("{0}  {1} --> {2}", String.Format("[{0}  ""{1}""]", node.UniqueId, node.InteractionType.ToString), node.FromNode, node.ToNode))
            Next

            Call Console.WriteLine(sBuilder.ToString)
            Return sBuilder.ToString
        End Function

        Private Function __getIndex(UniqueId As String) As Integer
            If _nodeHash.ContainsKey(UniqueId) Then
                Return _nodeHash(UniqueId).i
            Else
                Return -1
            End If
        End Function

        Public Overloads Function Compute(start As String, ends As String, Optional IgnoreNodes As String() = Nothing) As DataVisualization.NodeAttributes()
            If IgnoreNodes.IsNullOrEmpty Then
                _IgnoreNodes = New String() {}
            Else
                _IgnoreNodes = (From strValue In IgnoreNodes Select strValue.ToUpper).ToArray
            End If

            Dim AdjacencyLQuery = (From itr In NetworkInteractions.AsParallel Where itr.Equals(start, ends) Select itr).ToArray
            If Not AdjacencyLQuery.IsNullOrEmpty Then  '是直接相邻的两个节点
                Dim AdjacencyPath = New DataVisualization.NodeAttributes() {
                    OriginalNodes.GetItem(uniqueId:=start),
                    OriginalNodes.GetItem(uniqueId:=AdjacencyLQuery.First.UniqueId),
                    OriginalNodes.GetItem(uniqueId:=ends)}
                Return AdjacencyPath
            End If

            Dim idx_start = __getIndex(start), idx_ends = __getIndex(ends)
            If idx_ends < 0 OrElse idx_start < 0 Then
                Return New NodeAttributes() {}
            End If

            Dim path = Compute(idx_start, idx_ends)
            Dim nodes = (From x In Me._nodeHash.Values Select x Order By x.i Ascending).ToArray
            Dim LQuery = (From idx As Integer In path Select nodes(idx).obj).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 获取指定的两个节点之间的权重，不相邻返回-1
        ''' </summary>
        ''' <param name="start"></param>
        ''' <param name="finish"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function getInternodeTraversalCost(start As Integer, finish As Integer) As Single
            Dim NodeA As String = _nodes(start).obj.Identifier
            Dim NodeB As String = _nodes(finish).obj.Identifier

            Dim LQuery = (From itr In NetworkInteractions.AsParallel Where itr.Equals(NodeA, NodeB) Select itr).ToArray
            If LQuery.IsNullOrEmpty Then
                Return Single.MaxValue
            Else
                Return 1
            End If
        End Function

        Protected Overrides Function GetNearbyNodes(startingNode As Integer) As IEnumerable(Of Integer)
            Dim Node As String = _nodes(startingNode).obj.Identifier
            Dim LQuery = (From Interaction As Interactions
                          In NetworkInteractions
                          Let strId As String = __getNearbyNodeId(Node, Interaction)
                          Where Not String.IsNullOrEmpty(strId)
                          Select strId Distinct).ToArray
            Dim GetIndexLQuery = (From strId As String
                                  In LQuery.AsParallel
                                  Let Index As Integer = __getIndex(UniqueId:=strId)
                                  Select Index).ToArray
            Return GetIndexLQuery
        End Function

        Private Function __getNearbyNodeId(node As String, interaction As Interactions) As String '节点之间的相互作用是具有方向性的
            If String.Equals(node, interaction.FromNode) Then
                If Array.IndexOf(_IgnoreNodes, interaction.ToNode) > -1 Then  '该节点被忽略
                    Return ""
                Else
                    Return interaction.ToNode
                End If
            Else
                Return ""
            End If
        End Function
    End Class
End Namespace