Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Imaging

Public Module ReactionNetwork

    ''' <summary>
    ''' 将代谢物网络之中的reaction编号转换为pathway的名称
    ''' </summary>
    ''' <param name="net"></param>
    ''' <param name="ko0001"></param>
    <Extension>
    Public Sub AssignNodeClass(net As NetworkTables, ko0001 As KOLinks(), Optional delimiter$ = FunctionalNetwork.Delimiter)
        Dim index = ko0001 _
            .Where(Function(ko) Not ko.reactions.IsNullOrEmpty) _
            .Select(Function(ko) ko.reactions.Select(Function(rn) (rn, ko))) _
            .IteratesALL _
            .GroupBy(Function(id) id.Item1) _
            .ToDictionary(Function(id) id.Key,
                          Function(rn)
                              Return rn.Select(Function(x) x.Item2).ToArray
                          End Function)

        For Each node In net.Nodes
            Dim [class] As New List(Of String)
            Dim rn$() = Strings.Split(node.NodeType, delimiter)

            For Each id In rn
                If index.ContainsKey(id) Then
                    [class] += index(id) _
                        .Select(Function(ko) ko.pathways.Select(Function(x) x.text)) _
                        .IteratesALL _
                        .Distinct
                End If
            Next

            [class] = [class].Distinct.AsList

            If [class].IsNullOrEmpty Then
                node.NodeType = "KEGG Compound"
            Else
                node.NodeType = [class].JoinBy(delimiter)
            End If
        Next
    End Sub

    ''' <summary>
    ''' 利用代谢反应的摘要数据构建出代谢物的互作网络
    ''' </summary>
    ''' <param name="br08901">代谢反应数据</param>
    ''' <param name="compounds">KEGG化合物编号</param>
    ''' <param name="delimiter$"></param>
    ''' <param name="extended">是否对结果进行进一步的拓展，以获取得到一个连通性更加多的大网络？默认不进行拓展</param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildModel(br08901 As IEnumerable(Of ReactionTable),
                               compounds As IEnumerable(Of NamedValue(Of String)),
                               Optional delimiter$ = FunctionalNetwork.Delimiter,
                               Optional extended As Boolean = False) As NetworkTables

        Dim blue = Color.CornflowerBlue.RGBExpression
        Dim edges As New Dictionary(Of String, NetworkEdge)
        Dim cpdGroups = br08901 _
            .Select(Function(x)
                        Return x.substrates _
                            .JoinIterates(x.products) _
                            .Select(Function(id) (id, x))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(x) x.Item1) _
            .ToDictionary(Function(x) x.Key,
                          Function(reactions)
                              Return reactions _
                                  .Select(Function(x) x.Item2.entry) _
                                  .Distinct _
                                  .ToArray
                          End Function)
        Dim commons As Value(Of String()) = {}
        Dim nodes As Dictionary(Of Node) = compounds _
            .Select(Function(cpd)
                        Dim type$

                        If cpdGroups.ContainsKey(cpd.Name) Then
                            type = cpdGroups(cpd.Name) _
                                .JoinBy(delimiter)
                        Else
                            type = "KEGG Compound"
                        End If

                        Return New Node With {
                            .ID = cpd.Name,
                            .NodeType = type,
                            .Properties = New Dictionary(Of String, String) From {
                                {"name", cpd.Value},
                                {"color", blue}
                            }
                        }
                    End Function) _
            .ToDictionary

        For Each a As Node In nodes.Values
            Dim reactionA = cpdGroups.TryGetValue(a.ID)

            If reactionA.IsNullOrEmpty Then
                Continue For
            End If

            For Each b As Node In nodes.Values.Where(Function(x) x.ID <> a.ID)
                Dim rB = cpdGroups.TryGetValue(b.ID)

                If rB.IsNullOrEmpty Then
                    Continue For
                End If

                If Not (commons = reactionA.Intersect(rB).ToArray).IsNullOrEmpty Then
                    Dim edge As New NetworkEdge With {
                        .FromNode = a.ID,
                        .ToNode = b.ID,
                        .value = commons.Value.Length,
                        .Interaction = commons.Value.JoinBy("|")
                    }

                    With edge.GetNullDirectedGuid(True)
                        If Not edges.ContainsKey(.ref) Then
                            Call edges.Add(.ref, edge)
                        End If
                    End With
                End If
            Next
        Next

        Return New NetworkTables(nodes.Values, edges.Values)
    End Function
End Module
