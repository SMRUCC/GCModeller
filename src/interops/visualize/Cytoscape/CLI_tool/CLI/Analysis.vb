#Region "Microsoft.VisualBasic::06e730c99ebbcde875e673764cfe0b3f, visualize\Cytoscape\CLI_tool\CLI\Analysis.vb"

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

' Module CLI
' 
'     Function: AnalysisNetworkProperty, NodeCluster
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Fractions
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics.Heatmap
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Visualize.CatalogProfiling
Imports NetGraph = Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkTables

Partial Module CLI

    ''' <summary>
    ''' 生成相似度矩阵，寻找相似的节点
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Analysis.Node.Clusters")>
    <Usage("/Analysis.Node.Clusters /in <network.DIR> [/spcc /size ""10000,10000"" /schema <YlGn:c8> /out <DIR>]")>
    Public Function NodeCluster(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in])
        Dim net As NetGraph = NetworkFileIO.Load([in])
        Dim nodes$() = net.nodes.Keys.ToArray
        Dim from = net.SearchIndex(from:=True)
        Dim [to] = net.SearchIndex(from:=False)
        Dim objects As New List(Of DataSet)
        Dim spcc As Boolean = args.GetBoolean("/spcc")
        Dim size$ = args.GetValue("/size", "10000,10000")
        Dim colors$ = args.GetValue("/schema", "YlGn:c8")

        For Each node$ In nodes$
            Dim data As New Dictionary(Of String, Double)
            Dim lapply =
                Sub(index As Index(Of String))
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
        Dim csv = objects.ToCsvDoc(False, metaBlank:="0")
        Dim matrix = csv.AsDataSource(Of DataSet).Correlation

        Call objects.SaveTo(out & "/links.csv")
        Call matrix.PopulateRowObjects(Of DataSet).SaveTo(out & "/matrix.csv")
        Call CorrelationTriangle _
            .Plot(matrix, size:=size, mapName:=colors) _
            .Save(out & "/heatmap.png")

        Return 0
    End Function

    <ExportAPI("/Analysis.Graph.Properties")>
    <Usage("/Analysis.Graph.Properties /in <net.DIR> [/colors <Paired:c12> /ignores <fields> /tick 5 /out <out.DIR>]")>
    Public Function AnalysisNetworkProperty(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in])
        Dim network As NetGraph = NetworkFileIO.Load([in])
        Dim data As NamedValue(Of Integer)()
        Dim schema$ = args.GetValue("/colors", "Paired:12")
        Dim ignores As New Index(Of String)((args <= "/ignores").StringSplit(",", True))
        Dim nodeTable = network _
            .nodes _
            .Where(Function(n) ignores.IndexOf(n.NodeType) = -1) _
            .ToDictionary
        Dim tick% = args.GetValue("/tick", 5)

        ' 画图
        ' degrees使用catagory profiling图
        ' groups_count使用饼图

        data = network _
            .GetDegrees _
            .NamedValues _
            .Where(Function(x) nodeTable.ContainsKey(x.Name)) _
            .ToArray

        Call data.SaveTo(out & "/degrees.csv")
        Call data.Select(Function(x)
                             Return (group:=nodeTable(x.Name).NodeType, x)
                         End Function) _
            .GroupBy(Function(n) n.group) _
            .OrderByDescending(Function(g)
                                   Return g.Sum(Function(x) x.Item2.Value)
                               End Function) _
            .Take(6) _
            .ToDictionary(Function(k) k.Key,
                          Function(g)
                              Return g _
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
                                  .ToArray
                          End Function) _
            .DoCall(Function(table) New CatalogProfiles(table)) _
            .ProfilesPlot(size:="2400,1900",
                          title:="Network Connection Degrees",
                          axisTitle:="Node Degrees",
                          tick:=tick
            ) _
            .Save(out & "/degrees.png")

        data = network _
            .NodesGroupCount _
            .NamedValues _
            .OrderByDescending(Function(x) x.Value) _
            .Where(Function(x) ignores.NotExists(x.Name)) _
            .ToArray

        Call data.SaveTo(out & "/group_counts.csv")
        Call PieChart.Plot(
                data.Fractions(schema:=schema),
                size:="2600,2000",
                valueLabelStyle:=CSSFont.Win7Large) _
            .Save(out & "/group_counts.png")
        Call {
                 ("nodes", network.nodes.Length),
                 ("edges", network.edges.Length)
             } _
              .Select(Function(t) New NamedValue(Of Integer)(t.Item1, t.Item2)) _
              .ToArray _
              .SaveTo(out & "/stat.csv")

        Return 0
    End Function
End Module
