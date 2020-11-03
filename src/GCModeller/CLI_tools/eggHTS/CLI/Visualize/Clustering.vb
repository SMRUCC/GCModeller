#Region "Microsoft.VisualBasic::270ffd62c3669a5bbaf6263807da9201, CLI_tools\eggHTS\CLI\Visualize\Clustering.vb"

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
    '     Function: DEPHeatmapScatter3D, DEPKmeansScatter2D, MatrixClustering, PfamStringEnrichment
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize
Imports Microsoft.VisualBasic.Data.visualize.KMeans
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Partial Module CLI

    <ExportAPI("/DEP.kmeans.scatter2D")>
    <Usage("/DEP.kmeans.scatter2D /in <kmeans.csv> /sampleInfo <sampleInfo.csv> [/t.log <default=-1> /cluster.prefix <default=""cluster: #""> /size <2500,2200> /pt.size <radius pixels, default=15> /schema <default=clusters> /out <out.png>]")>
    <Group(CLIGroups.DataVisualize_cli)>
    <ArgumentAttribute("/sampleinfo", False, CLITypes.File, PipelineTypes.undefined,
              AcceptTypes:={GetType(SampleInfo)},
              Extensions:="*.csv",
              Description:="This file describ how to assign the axis data. The ``sample_group`` in this file defines the X or Y axis label, 
                            and the corresponding ``sample_name`` data is the data for plot on the X or Y axis.")>
    Public Function DEPKmeansScatter2D(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim sampleInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim size$ = (args <= "/size") Or "2500,2200".AsDefault
        Dim schema$ = (args <= "/schema") Or "clusters".AsDefault
        Dim out$ = (args <= "/out") Or ([in].TrimSuffix & ".scatter2D.png").AsDefault
        Dim clusterData As EntityClusterModel() = [in].LoadCsv(Of EntityClusterModel).ToArray
        Dim prefix$ = (args <= "/cluster.prefix") Or "Cluster:  #".AsDefault
        Dim tlog# = args.GetValue("/t.log", -1.0R)
        Dim ptSize! = args("/pt.size") Or 15.0!

        If Not prefix.StringEmpty Then
            For Each protein As EntityClusterModel In clusterData
                protein.Cluster = prefix & protein.Cluster
            Next
        End If

        If tlog > 0 Then
            For Each protein In clusterData
                For Each key In protein.Properties.Keys.ToArray
                    ' +1S 防止log(0)出现
                    protein.Properties(key) = Math.Log(protein.Properties(key) + +1S, newBase:=tlog)
                Next
            Next
        End If

        Dim category As Dictionary(Of NamedCollection(Of String)) = sampleInfo.ToCategory
        Dim keys = category.Keys.ToArray
        Dim A As New NamedCollection(Of String) With {.Name = keys(0), .Value = category(.Name).Value}
        Dim B As New NamedCollection(Of String) With {.Name = keys(1), .Value = category(.Name).Value}

        Return KmeansExtensions.Scatter2D(clusterData, (A, B), size,
                                schema:=schema,
                                pointSize:=ptSize,
                                padding:=g.DefaultUltraLargePadding
            ) _
            .AsGDIImage _
            .CorpBlank(30, Color.White) _
            .SaveAs(out) _
            .CLICode
    End Function

    <ExportAPI("/matrix.clustering")>
    <Usage("/matrix.clustering /in <matrix.csv> [/cluster.n <default:=10> /out <EntityClusterModel.csv>]")>
    <Group(CLIGroups.DataVisualize_cli)>
    Public Function MatrixClustering(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.kmeans_clusters.csv"
        Dim matrix = DataSet.LoadDataSet([in])
        Dim n% = args.GetValue("/cluster.n", 10)

        matrix = matrix _
           .Where(Function(d)
                      Return d.Properties.Values.Any(Function(x) x <> 0R)
                  End Function) _
           .AsList

        With matrix _
            .ToKMeansModels _
            .Kmeans(expected:=n)

            ' 保存用于绘制3D/2D聚类图的数据集
            Return .ToEntityObjects _
                .ToArray _
                .SaveDataSet(out, Encodings.UTF8) _
                .CLICode
        End With
    End Function

    <ExportAPI("/pfamstring.enrichment")>
    <Usage("/pfamstring.enrichment /in <EntityClusterModel.csv> /pfamstring <pfamstring.csv> [/out <out.directory>]")>
    Public Function PfamStringEnrichment(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim pfamstring = (args <= "/pfamstring") _
            .LoadCsv(Of PfamString) _
            .ToDictionary(Function(prot)
                              Return prot.ProteinId
                          End Function)
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.pfamstring.enrichment.csv"
        Dim clusters = [in].LoadCsv(Of EntityClusterModel) _
            .GroupBy(Function(c) c.Cluster) _
            .ToArray

        Dim profile As New List(Of EntityObject)

        For Each cluster In clusters
            Dim clusterName$ = cluster.Key
            Dim n% = cluster.Count
            Dim members = cluster _
                .Where(Function(prot)
                           Return pfamstring.ContainsKey(prot.ID) AndAlso
                              Not pfamstring(prot.ID) _
                                 .Domains _
                                 .IsNullOrEmpty
                       End Function) _
                .Select(Function(prot) pfamstring(prot.ID)) _
                .ToArray
            Dim ALL = members _
                .Select(Function(prot) prot.Domains) _
                .IteratesALL _
                .Distinct _
                .OrderBy(Function(d) d) _
                .ToArray

            profile += New EntityObject With {
                .ID = clusterName,
                .Properties = ALL _
                    .ToDictionary(Function(d) d,
                                  Function(d)
                                      Dim hits = members _
                                          .Where(Function(prot) prot.Domains.IndexOf(d) > -1) _
                                          .Count
                                      Dim pct# = Math.Round(100% * (hits / n), 2)

                                      Return pct
                                  End Function) _
                    .AsCharacter
            }
        Next

        Return profile _
            .SaveDataSet(out, blank:="0", transpose:=True) _
            .CLICode
    End Function

    ''' <summary>
    ''' 进行差异表达蛋白的聚类结果的3D scatter散点图的绘制可视化
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/DEP.heatmap.scatter.3D")>
    <Description("Visualize the DEPs' kmeans cluster result by using 3D scatter plot.")>
    <Usage("/DEP.heatmap.scatter.3D /in <kmeans.csv> /sampleInfo <sampleInfo.csv> [/display.labels <default=-1> /cluster.prefix <default=""cluster: #""> /size <default=1600,1400> /schema <default=clusters> /view.angle <default=30,60,-56.25> /view.distance <default=2500> /arrow.factor <default=1,2> /cluster.title <names.csv> /out <out.png>]")>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(EntityClusterModel)},
              Extensions:="*.csv",
              Description:="The kmeans cluster result from ``/DEP.heatmap`` command.")>
    <ArgumentAttribute("/sampleInfo", False, CLITypes.File,
              AcceptTypes:={GetType(SampleInfo)},
              Extensions:="*.csv",
              Description:="Sample info fot grouping the matrix column data and generates the 3d plot ``<x,y,z>`` coordinations.")>
    <ArgumentAttribute("/cluster.prefix", True, CLITypes.String,
              Description:="The term prefix of the kmeans cluster name when display on the legend title.")>
    <ArgumentAttribute("/size", True,
              AcceptTypes:={GetType(Size)},
              Description:="The output 3D scatter plot image size.")>
    <ArgumentAttribute("/view.angle", True,
              Description:="The view angle of the 3D scatter plot objects, in 3D direction of ``<X>,<Y>,<Z>``")>
    <ArgumentAttribute("/view.distance", True, CLITypes.Integer,
              Description:="The view distance from the 3D camera screen to the 3D objects.")>
    <ArgumentAttribute("/out", True, CLITypes.File,
              Extensions:="*.png, *.svg",
              Description:="The file path of the output plot image.")>
    <ArgumentAttribute("/display.labels", True, CLITypes.Double,
              AcceptTypes:={GetType(Double)},
              Description:="If this parameter is positive and then all of the value greater than this quantile threshold its labels will be display on the plot.")>
    <Group(CLIGroups.DataVisualize_cli)>
    Public Function DEPHeatmapScatter3D(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim sampleInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim size$ = (args <= "/size") Or "1600,1400".AsDefault
        Dim schema$ = (args <= "/schema") Or "clusters".AsDefault
        Dim out$ = (args <= "/out") Or ([in].TrimSuffix & ".scatter.png").AsDefault
        Dim clusterData As EntityClusterModel() = [in].LoadCsv(Of EntityClusterModel).ToArray
        Dim viewAngle As Vector = (args <= "/view.angle") Or "30,60,-56.25".AsDefault
        Dim viewDistance# = args.GetValue("/view.distance", 2500)
        Dim qDisplay# = args("/display.labels") Or -1.0
        Dim camera As New Camera With {
            .fov = 500000,
            .screen = size.SizeParser,
            .ViewDistance = viewDistance,
            .angleX = viewAngle(0),
            .angleY = viewAngle(1),
            .angleZ = viewAngle(2)
        }
        Dim category As Dictionary(Of NamedCollection(Of String)) = sampleInfo.ToCategory
        Dim prefix$ = (args <= "/cluster.prefix") Or "Cluster:  #".AsDefault
        Dim arrowFactor$ = args("/arrow.factor") Or "1,2"

        If Not prefix.StringEmpty Then
            For Each protein As EntityClusterModel In clusterData
                protein.Cluster = prefix & protein.Cluster
            Next
        End If

        Return clusterData _
            .Scatter3D(category, camera, size, schema:=schema, arrowFactor:=arrowFactor, labelsQuantile:=qDisplay) _
            .AsGDIImage _
            .CorpBlank(30, Color.White) _
            .SaveAs(path:=out) _
            .CLICode
    End Function
End Module
