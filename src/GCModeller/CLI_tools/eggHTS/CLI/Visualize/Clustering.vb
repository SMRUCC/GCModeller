Imports System.ComponentModel
Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.DataMining
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Partial Module CLI

    <ExportAPI("/DEP.kmeans.scatter2D")>
    <Usage("/DEP.kmeans.scatter2D /in <kmeans.csv> /sampleInfo <sampleInfo.csv> [/t.log <default=-1> /cluster.prefix <default=""cluster: #""> /size <1600,1400> /schema <default=clusters> /out <out.png>]")>
    <Group(CLIGroups.DataVisualize_cli)>
    Public Function DEPKmeansScatter2D(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim sampleInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim size$ = (args <= "/size") Or "1600,1400".AsDefault
        Dim schema$ = (args <= "/schema") Or "clusters".AsDefault
        Dim out$ = (args <= "/out") Or ([in].TrimSuffix & ".scatter2D.png").AsDefault
        Dim clusterData As EntityClusterModel() = DataSet.LoadDataSet(Of EntityClusterModel)([in]).ToArray
        Dim prefix$ = (args <= "/cluster.prefix") Or "Cluster:  #".AsDefault
        Dim tlog# = args.GetValue("/t.log", -1.0R)

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

        Return Kmeans.Scatter2D(clusterData, (A, B), size, schema:=schema) _
            .AsGDIImage _
            .CorpBlank(30, Color.White) _
            .SaveAs(out) _
            .CLICode
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
        Dim clusters = DataSet _
            .LoadDataSet(Of EntityClusterModel)([in]) _
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
            .SaveDataSet(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 进行差异表达蛋白的聚类结果的3D scatter散点图的绘制可视化
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/DEP.heatmap.scatter.3D")>
    <Description("Visualize the DEPs' kmeans cluster result by using 3D scatter plot.")>
    <Usage("/DEP.heatmap.scatter.3D /in <kmeans.csv> /sampleInfo <sampleInfo.csv> [/cluster.prefix <default=""cluster: #""> /size <default=1600,1400> /schema <default=clusters> /view.angle <default=30,60,-56.25> /view.distance <default=2500> /cluster.title <names.csv> /out <out.png>]")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(EntityClusterModel)},
              Extensions:="*.csv",
              Description:="The kmeans cluster result from ``/DEP.heatmap`` command.")>
    <Argument("/sampleInfo", False, CLITypes.File,
              AcceptTypes:={GetType(SampleInfo)},
              Extensions:="*.csv",
              Description:="Sample info fot grouping the matrix column data and generates the 3d plot ``<x,y,z>`` coordinations.")>
    <Argument("/cluster.prefix", True, CLITypes.String,
              Description:="The term prefix of the kmeans cluster name when display on the legend title.")>
    <Argument("/size", True,
              AcceptTypes:={GetType(Size)},
              Description:="The output 3D scatter plot image size.")>
    <Argument("/view.angle", True,
              Description:="The view angle of the 3D scatter plot objects, in 3D direction of ``<X>,<Y>,<Z>``")>
    <Argument("/view.distance", True, CLITypes.Integer,
              Description:="The view distance from the 3D camera screen to the 3D objects.")>
    <Argument("/out", True, CLITypes.File,
              Extensions:="*.png, *.svg",
              Description:="The file path of the output plot image.")>
    <Group(CLIGroups.DataVisualize_cli)>
    Public Function DEPHeatmap3D(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim sampleInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim size$ = (args <= "/size") Or "1600,1400".AsDefault
        Dim schema$ = (args <= "/schema") Or "clusters".AsDefault
        Dim out$ = (args <= "/out") Or ([in].TrimSuffix & ".scatter.png").AsDefault
        Dim clusterData As EntityClusterModel() = DataSet.LoadDataSet(Of EntityClusterModel)([in]).ToArray
        Dim viewAngle As Vector = (args <= "/view.angle") Or "30,60,-56.25".AsDefault
        Dim viewDistance# = args.GetValue("/view.distance", 2500)
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

        If Not prefix.StringEmpty Then
            For Each protein As EntityClusterModel In clusterData
                protein.Cluster = prefix & protein.Cluster
            Next
        End If

        Return clusterData _
            .Scatter3D(category, camera, size, schema:=schema) _
            .AsGDIImage _
            .CorpBlank(30, Color.White) _
            .SaveAs(path:=out) _
            .CLICode
    End Function
End Module