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
End Module