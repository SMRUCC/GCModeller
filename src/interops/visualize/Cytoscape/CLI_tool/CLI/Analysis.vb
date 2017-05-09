Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Mathematical.Correlations
Imports SMRUCC.genomics.Visualize
Imports NetGraph = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Network

Partial Module CLI

    ''' <summary>
    ''' 生成相似度矩阵，寻找相似的节点
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Analysis.Node.Clusters",
               Usage:="/Analysis.Node.Clusters /in <network.DIR> [/spcc /size ""10000,10000"" /schema <YlGn:c8> /out <DIR>]")>
    Public Function NodeCluster(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in])
        Dim net As NetGraph = NetGraph.Load([in])
        Dim nodes$() = net.Nodes.Keys
        Dim from = net.SearchIndex(from:=True)
        Dim [to] = net.SearchIndex(from:=False)
        Dim objects As New List(Of DataSet)
        Dim spcc As Boolean = args.GetBoolean("/spcc")
        Dim size$ = args.GetValue("/size", "10000,10000")
        Dim colors$ = args.GetValue("/schema", "YlGn:c8")

        For Each node$ In nodes$
            Dim data As New Dictionary(Of String, Double)
            Dim lapply =
                Sub(index As IndexOf(Of String))
                    If Not index Is Nothing Then
                        For Each x$ In nodes$
                            If index.IndexOf(x) > -1 Then
                                If data.ContainsKey(x) Then
                                    data(x) = 2
                                Else
                                    data(x) = 1
                                End If
                            End If
                        Next
                    End If
                End Sub

            Call lapply(from.TryGetValue(node))
            Call lapply([to].TryGetValue(node))

            objects += New DataSet With {
                .ID = node,
                .Properties = data
            }
        Next

        ' 需要转换一次csv再转换回来，从而才能进行排序和填充零，进行相似度矩阵运算
        Dim csv = objects.ToCsvDoc(False, metaBlank:=0)
        Dim vectors As NamedValue(Of Double())() = csv _
            .AsDataSource(Of DataSet) _
            .NamedMatrix _
            .Select(Function(x)
                        Return New NamedValue(Of Double()) With {
                            .Name = x.Name,
                            .Value = x.Value.Values.ToArray
                        }
                    End Function) _
            .ToArray
        Dim matrix As NamedValue(Of Dictionary(Of String, Double))()

        If spcc Then
            matrix = vectors.CorrelationMatrix(compute:=AddressOf Spearman)
        Else
            matrix = vectors.CorrelationMatrix(compute:=AddressOf GetPearson)
        End If

        Call objects.SaveTo(out & "/links.csv")
        Call matrix.SaveTo(out & "/matrix.csv")
        Call HeatmapTable _
            .Plot(matrix, size:=size, mapName:=colors) _
            .Save(out & "/heatmap.png")

        Return 0
    End Function

    <ExportAPI("/Analysis.Graph.Properties",
               Usage:="/Analysis.Graph.Properties /in <net.DIR> [/colors <Paired:c12> /ignores <fields> /out <out.DIR>]")>
    Public Function AnalysisNetworkProperty(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in])
        Dim network As NetGraph = NetGraph.Load([in])
        Dim data As NamedValue(Of Integer)()
        Dim schema$ = args.GetValue("/colors", "Paired:12")
        Dim ignores As New IndexOf(Of String)((args <= "/ignores").StringSplit(",", True))
        Dim nodeTable = network _
            .Nodes _
            .Where(Function(n) ignores.IndexOf(n.NodeType) = -1) _
            .ToDictionary

        ' 画图
        ' degrees使用catagory profiling图
        ' groups_count使用饼图

        data = network _
            .GetDegrees _
            .NamedValues _
            .Where(Function(x) nodeTable.ContainsKey(x.Name)) _
            .ToArray

        Call data.SaveTo(out & "/degrees.csv")
        Call data.Select(Function(x) (group:=nodeTable(x.Name).NodeType, x)) _
            .GroupBy(Function(n) n.group) _
            .OrderByDescending(Function(g) g.Sum(Function(x) x.Item2.Value)) _
            .Take(6) _
            .ToDictionary(Function(k) k.Key,
                          Function(g) g _
                              .Select(Function(t) t.Item2) _
                              .OrderByDescending(Function(x) x.Value) _
                              .Take(7) _
                              .OrderBy(Function(o) o.Name) _
                              .Select(Function(x)
                                          Return New NamedValue(Of Double) With {
                                              .Name = x.Name,
                                              .Value = x.Value
                                          }
                                      End Function) _
                              .ToArray) _
            .ProfilesPlot(size:=New Size(2400, 1900), 
                          title:="Network Connection Degrees", 
                          axisTitle:="Node Degrees", 
                          tick:=5) _
            .Save(out & "/degrees.png")

        data = network _
            .NodesGroupCount _
            .NamedValues _
            .OrderByDescending(Function(x) x.Value) _
            .Where(Function(x) ignores.NotExists(x.Name)) _
            .ToArray

        Call data.SaveTo(out & "/group_counts.csv")
        Call PieChart.Plot(data.FromData(schema:=schema)).Save(out & "/group_counts.png")
        Call {
            ("nodes", network.Nodes.Length),
            ("edges", network.Edges.Length)
           }.Select(Function(t) New NamedValue(Of Integer)(t.Item1, t.Item2)) _
            .ToArray _
            .SaveTo(out & "/stat.csv")

        Return 0
    End Function
End Module