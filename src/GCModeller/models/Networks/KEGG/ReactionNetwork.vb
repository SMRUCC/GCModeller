Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.ComponentModel.Collection

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

        ' {KEGG_compound --> reaction ID()}
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

        ' 从输入的数据之中构建出网络的节点列表
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

        Dim extendes As New List(Of Node)

        ' 下面的这个for循环对所构建出来的节点列表进行边链接构建
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

                ' a 和 b 是直接相连的
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

                Else

                    ' 这两个节点之间可能存在一个空位，
                    ' 对所有的节点进行遍历，找出同时链接a和b的节点
                    If extended Then

                        If Not cpdGroups.ContainsKey(a.ID) OrElse Not cpdGroups.ContainsKey(b.ID) Then
                            Continue For
                        End If

                        Dim indexA = cpdGroups(a.ID).Indexing
                        Dim indexB = cpdGroups(b.ID).Indexing

                        For Each x In cpdGroups
                            Dim list = x.Value

                            If list.Any(Function(r) indexA(r) > -1) AndAlso list.Any(Function(r) indexB(r) > -1) Then
                                ' 这是一个间接的拓展链接，将其加入到边列表之中
                                ' X也添加进入拓展节点列表之中

                                extendes += New Node With {
                                    .ID = x.Key,
                                    .NodeType = list.JoinBy(delimiter),
                                    .Properties = New Dictionary(Of String, String) From {
                                        {"name", x.Key},
                                        {"color", blue}
                                    }
                                }

                                Dim populate = Iterator Function()
                                                   Yield (a.ID, indexA)
                                                   Yield (b.ID, indexB)
                                               End Function

                                For Each n As (ID$, list As Index(Of String)) In populate()
                                    Dim edge As New NetworkEdge With {
                                        .FromNode = n.ID,
                                        .ToNode = x.Key,
                                        .Interaction = n.list.Objects.Intersect(list).JoinBy("|"),
                                        .value = - .Interaction.Split("|"c).Length
                                    }

                                    With edge.GetNullDirectedGuid(True)
                                        If Not edges.ContainsKey(.ref) Then
                                            Call edges.Add(.ref, edge)
                                        End If
                                    End With
                                Next

                                Exit For
                            End If
                        Next
                    End If
                End If
            Next
        Next

        extendes = extendes _
            .GroupBy(Function(n) n.ID) _
            .Select(Function(x) x.First) _
            .AsList

        For Each x In extendes
            If Not nodes.ContainsKey(x.ID) Then
                nodes += x
            End If
        Next

        Return New NetworkTables(nodes.Values, edges.Values)
    End Function
End Module
