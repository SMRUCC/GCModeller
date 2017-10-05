#Region "Microsoft.VisualBasic::11f4c70f3bf162d002cd62b42381fa3c, ..\CLI_tools\eggHTS\CLI\2. DEP.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics.Heatmap
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.DataMining
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.Repository.kb_UniProtKB
Imports SMRUCC.genomics.Data.Repository.kb_UniProtKB.UniprotKBEngine
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Visualize

Partial Module CLI

#Region "DEG data experiment designer"

    <ExportAPI("/edgeR.Designer")>
    <Description("Generates the edgeR inputs table")>
    <Usage("/edgeR.Designer /in <proteinGroups.csv> /designer <designer.csv> [/label <default is empty> /deli <default=-> /out <out.DIR>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function edgeRDesigner(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim designer = args <= "/designer"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & $"-{designer.BaseName}-edgeR/")
        Dim designers As Designer() = designer.LoadCsv(Of Designer)
        Dim label$ = args.GetValue("/label", "")
        Dim deli$ = args.GetValue("/deli", "-")

        Call EdgeR_rawDesigner([in], designers, (label, deli), workDIR:=out)

        Return 0
    End Function

    <ExportAPI("/T.test.Designer.iTraq")>
    <Description("Generates the iTraq data t.test DEP method inputs table")>
    <Usage("/T.test.Designer.iTraq /in <proteinGroups.csv> /designer <designer.csv> [/label <default is empty> /deli <default=-> /out <out.DIR>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function TtestDesigner(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim designer = args <= "/designer"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & $"-{designer.BaseName}-iTraq_Ttest/")
        Dim designers As Designer() = designer.LoadCsv(Of Designer)
        Dim label$ = args.GetValue("/label", "")
        Dim deli$ = args.GetValue("/deli", "-")

        Call DEGDesigner.TtestDesigner([in], designers, (label, deli), workDIR:=out)

        Return 0
    End Function

    ''' <summary>
    ''' 独立实验变量的t检验构建工具
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/T.test.Designer.LFQ")>
    <Description("Generates the LFQ data t.test DEP method inputs table")>
    <Usage("/T.test.Designer.LFQ /in <proteinGroups.csv> /designer <designer.csv> [/label <default is empty> /deli <default=-> /out <out.DIR>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function TtestDesignerLFQ(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim designer = args <= "/designer"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & $"-{designer.BaseName}-LFQ_Ttest/")
        Dim designers As Designer() = designer.LoadCsv(Of Designer)
        Dim label$ = args.GetValue("/label", "")
        Dim deli$ = args.GetValue("/deli", "-")

        Call DEGDesigner.TtestDesignerIndependent([in], designers, (label, deli), workDIR:=out)

        Return 0
    End Function

#End Region

    ''' <summary>
    ''' 因为绘制云图需要表达量的实际值，对于iTraq结果而言是没有表达量的，所以这里就仅限于iBAQ结果了
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/iBAQ.Cloud")>
    <Usage("/iBAQ.Cloud /in <expression.csv> /annotations <annotations.csv> /DEPs <DEPs.csv> /tag <expression> [/out <out.png>]")>
    <Description("Cloud plot of the iBAQ DEPs result.")>
    <Argument("/tag", Description:="The field name in the ``/in`` matrix that using as the expression value.")>
    Public Function DEPsCloudPlot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim annotations$ = args <= "/annotations"
        Dim DEPs$ = args <= "/DEPs"
        Dim tag$ = args <= "/tag"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "-" & tag.NormalizePathString & ".png")
        Dim expressions = EntityObject.LoadDataSet([in]).ToArray
        Dim annotationData = annotations.LoadCsv(Of UniprotAnnotations)
        Dim DEPsResult = EntityObject.LoadDataSet(DEPs).ToArray
        Dim plot As GraphicsData = CloudPlot.Plot(expressions, annotationData, DEPsResult, tag)
        Return plot.Save(out).CLICode
    End Function

    ''' <summary>
    ''' 使用这个函数来处理iTraq实验结果之中与分析需求单的FC比对方式颠倒的情况
    ''' 
    ''' 假设所输入的文件的第一列为标识符
    ''' 最后的三列为结果数据
    ''' 则中间的剩余的列数据都是FC值
    ''' 
    ''' ``Accession	T1.C1	T1.C2	T2.C1	T2.C2	FC.avg	p.value	is.DEP``
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/iTraq.Reverse.FC",
               Info:="Reverse the FC value from the source result.",
               Usage:="/iTraq.Reverse.FC /in <data.csv> [/out <Reverse.csv>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function iTraqInvert(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".reverse.csv")
        Dim data As File = File.Load([in])
        Dim start = 1
        Dim ends = (data.Width - 1) - 3

        For Each row As RowObject In data.Skip(1)
            For i As Integer = start To ends
                Dim s$ = row(i)

                If s = "NA" OrElse s.TextEquals("NaN") Then
                    Continue For
                Else
                    row(i) = 1 / Val(s)
                End If
            Next
        Next

        Return data _
            .Save(out, Encodings.ASCII) _
            .CLICode
    End Function

    <ExportAPI("/DEP.uniprot.list",
               Usage:="/DEP.uniprot.list /DEP <log2-test.DEP.csv> /sample <sample.csv> [/out <out.txt>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPUniprotIDlist(args As CommandLine) As Integer
        Dim DEP As String = args("/DEP")
        Dim sample As String = args("/sample")
        Dim out As String = args.GetValue("/out", DEP.TrimSuffix & "-uniprot.ID.list.txt")
        Dim DEP_data As IEnumerable(Of DEP_iTraq) = EntityObject _
            .LoadDataSet(Of DEP_iTraq)(path:=DEP) _
            .Where(Function(d) d.isDEP) _
            .ToArray
        Dim sampleData As Dictionary(Of String, String()) =
            sample _
            .LoadCsv(Of UniprotAnnotations) _
            .GroupBy(Function(p) p.ORF) _
            .ToDictionary(Function(p) p.Key,
                          Function(g) g.ToArray(
                          Function(p) p.ID))
        Dim list$() = DEP_data _
            .Select(Function(d) sampleData(d.ID)) _
            .IteratesALL _
            .Distinct _
            .ToArray

        Return list.SaveTo(out).CLICode
    End Function

    <ExportAPI("/DEP.uniprot.list2",
               Usage:="/DEP.uniprot.list2 /in <log2.test.csv> [/DEP.Flag <is.DEP?> /uniprot.Flag <uniprot> /species <scientifcName> /uniprot <uniprotXML> /out <out.txt>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPUniprotIDs2(args As CommandLine) As Integer
        Dim [in] = args("/in")
        Dim DEPFlag As String = args.GetValue("/DEP.flag", "is.DEP?")
        Dim uniprot As String = args.GetValue("/uniprot.Flag", "uniprot")
        Dim data = EntityObject.LoadDataSet([in])
        Dim DEPs = data.Where(Function(prot) prot(DEPFlag).ParseBoolean).ToArray
        Dim uniprotIDs$() = DEPs _
            .Select(Function(prot) prot(uniprot).Split(";"c)) _
            .Unlist _
            .Distinct _
            .Select(AddressOf Trim) _
            .ToArray
        Dim sciName$ = args("/species")
        Dim out As String = args.GetValue(
            "/out",
            [in].TrimSuffix & $"DEPs={DEPs.Length}.uniprotIDs.txt")

        uniprot$ = args("/uniprot")

        If Not sciName.StringEmpty AndAlso uniprot.FileExists(True) Then
            ' 将结果过滤为指定的物种的编号
            Dim table As Dictionary(Of entry) = UniProtXML.LoadDictionary(uniprot)
            uniprotIDs = uniprotIDs _
                .Where(Function(ID) table.ContainsKey(ID) AndAlso
                                    table(ID).organism.scientificName = sciName) _
                .ToArray
        End If

        Return uniprotIDs.SaveTo(out).CLICode
    End Function

    <ExportAPI("/DEP.venn",
               Info:="Generate the VennDiagram plot data and the venn plot tiff. The default parameter profile is using for the iTraq data.",
               Usage:="/DEP.venn /data <Directory> [/title <VennDiagram title> /out <out.DIR>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function VennData(args As CommandLine) As Integer
        Dim DIR$ = args("/data")
        Dim out As String = args.GetValue("/out", DIR.TrimDIR & ".venn/")
        Dim dataOUT = out & "/DEP.venn.csv"
        Dim title$ = args.GetValue("/title", "VennDiagram title")

        Call Union(DIR, True, "", nonDEP_blank:=True, outGroup:=True) _
            .SaveDataSet(dataOUT)
        Call Apps.VennDiagram.Draw(dataOUT, title, out:=out & "/venn.tiff")

        Return 0
    End Function

    <Extension>
    Private Function unionDATA(handle$) As Dictionary(Of String, Dictionary(Of DEP_iTraq))
        Dim files As IEnumerable(Of String)

        If handle.FileLength > 0 Then
            files = {handle}
        Else
            files = ls - l - r - "*.csv" <= handle
        End If

        Return files.ToDictionary(
            Function(path) path.BaseName,
            Function(path)
                Return EntityObject _
                    .LoadDataSet(Of DEP_iTraq)(path) _
                    .ToDictionary
            End Function)
    End Function

    Public Function Union(DIR$, tlog2 As Boolean, ZERO$, nonDEP_blank As Boolean, outGroup As Boolean) As List(Of EntityObject)
        Dim data As Dictionary(Of String, Dictionary(Of DEP_iTraq)) = DIR.unionDATA
        Dim allDEPs = data.Values _
            .IteratesALL _
            .Where(Function(x) x.Value.isDEP) _
            .Keys _
            .Distinct _
            .ToArray
        Dim matrix As New List(Of EntityObject)

        Call $"Input data have {allDEPs.Length} Union DEPs".__INFO_ECHO

        For Each id As String In allDEPs
            Dim FClog2 As New Dictionary(Of String, String)

            ' 将当前的这个DEP标记的基因的数据从所有的分组之中拿出来
            For Each group In data
                With group.Value
                    If .ContainsKey(id) Then
                        With .ref(id)
                            If .ref.isDEP Then
                                If outGroup Then

                                    FClog2.Add(group.Key, .ref.log2FC)

                                Else

                                    For Each prop In .ref.Properties
                                        If prop.Value.TextEquals("NA") Then
                                            FClog2.Add(prop.Key, ZERO)
                                        Else
                                            If tlog2 Then
                                                Call FClog2.Add(prop.Key, Math.Log(Val(prop.Value), 2))
                                            Else
                                                Call FClog2.Add(prop.Key, Val(prop.Value))
                                            End If
                                        End If
                                    Next

                                End If
                            Else
                                If nonDEP_blank Then

                                    If outGroup Then

                                        FClog2.Add(group.Key, ZERO)

                                    Else

                                        For Each prop In .ref.Properties
                                            FClog2.Add(prop.Key, ZERO) ' log2(1) = 0
                                        Next

                                    End If

                                Else

                                    If outGroup Then

                                        FClog2.Add(group.Key, .ref.log2FC)

                                    Else

                                        For Each prop In .ref.Properties
                                            If prop.Value.TextEquals("NA") Then
                                                FClog2.Add(prop.Key, ZERO)
                                            Else
                                                If tlog2 Then
                                                    Call FClog2.Add(prop.Key, Math.Log(Val(prop.Value), 2))
                                                Else
                                                    Call FClog2.Add(prop.Key, Val(prop.Value))
                                                End If
                                            End If
                                        Next

                                    End If


                                End If
                            End If
                        End With
                    Else

                        If outGroup Then

                            FClog2.Add(group.Key, ZERO)

                        Else

                            For Each key In data(group.Key).Values.First.Properties.Keys
                                FClog2.Add(key, ZERO)
                            Next

                        End If
                    End If
                End With
            Next

            matrix += New EntityObject With {
                .ID = id,
                .Properties = FClog2
            }
        Next

        Return matrix
    End Function

    <ExportAPI("/DEPs.union", Usage:="/DEPs.union /in <csv.DIR> [/FC <default=logFC> /out <out.csv>]")>
    Public Function DEPsUnion(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim FC$ = args.GetValue("/FC", "logFC")
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "-" & FC.NormalizePathString & ".union.csv")
        Dim datas = (ls - l - r - "*.csv" <= [in]) _
            .ToDictionary(Function(csv) csv.BaseName,
                          Function(csv)
                              Return EntityObject _
                                  .LoadDataSet(csv) _
                                  .ToDictionary
                          End Function)
        Dim allIDs = datas.Values _
            .Select(Function(sample) sample.Values) _
            .IteratesALL _
            .Select(Function(x) x.ID) _
            .Distinct _
            .ToArray
        Dim union As New List(Of EntityObject)

        For Each id As String In allIDs
            Dim protein As New EntityObject(id)

            For Each sample In datas
                Dim value = sample.Value.TryGetValue(id)
                If value Is Nothing Then
                    protein.Properties.Add(sample.Key, 1)
                Else
                    protein.Properties.Add(sample.Key, value(FC))
                End If
            Next

            union += protein
        Next

        Return union _
            .SaveTo(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' ``/iTraq``开关表示是够是iTraq的分析输出？
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/DEP.heatmap")>
    <Description("Generates the heatmap plot input data. The default label profile is using for the iTraq result.")>
    <Usage("/DEP.heatmap /data <Directory/csv_file> [/schema <color_schema, default=RdYlGn:c11> /no-clrev /KO.class /annotation <annotation.csv> /hide.labels /cluster.n <default=6> /sampleInfo <sampleinfo.csv> /non_DEP.blank /title ""Heatmap of DEPs log2FC"" /t.log2 /tick <-1> /size <size, default=2000,3000> /out <out.DIR>]")>
    <Argument("/non_DEP.blank", True, CLITypes.Boolean,
              Description:="If this parameter present, then all of the non-DEP that bring by the DEP set union, will strip as blank on its foldchange value, and set to 1 at finally. Default is reserve this non-DEP foldchange value.")>
    <Argument("/KO.class", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this argument was set, then the KO class information for uniprotID will be draw on the output heatmap.")>
    <Argument("/sampleInfo", True, CLITypes.File,
              Extensions:="*.csv",
              AcceptTypes:={GetType(SampleInfo)},
              Description:="Describ the experimental group information")>
    <Argument("/data", False, CLITypes.File, PipelineTypes.std_in,
              Description:="This file path parameter can be both a directory which contains a set of DEPs result or a single csv file path.")>
    <Argument("/hide.labels", True, CLITypes.Boolean,
              Description:="Hide the row labels?")>
    <Argument("/cluster.n", True, CLITypes.Integer,
              Description:="Expects the kmeans cluster result number, default is output 6 kmeans clusters.")>
    <Argument("/schema", True, CLITypes.String,
              Description:="The color patterns of the heatmap visualize, by default is using ``ColorBrewer`` colors.")>
    <Argument("/out", True, CLITypes.File,
              Extensions:="*.csv, *.svg, *.png",
              Description:="A directory path where will save the output heatmap plot image and the kmeans cluster details info.")>
    <Argument("/title", True,
              Description:="The main title of this chart plot.")>
    <Argument("/t.log2", True, CLITypes.Boolean, Description:="If this parameter is presented, then it will means apply the log2 transform on the matrix cell value before the heatmap plot.")>
    <Argument("/tick", True, CLITypes.Double, Description:="The ticks value of the color legend, by default value -1 means generates ticks automatically.")>
    <Argument("/no-clrev", True, CLITypes.Boolean, Description:="Do not reverse the color sequence.")>
    <Argument("/size", True, CLITypes.String, AcceptTypes:={GetType(Size)}, Description:="The canvas size.")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function Heatmap_DEPs(args As CommandLine) As Integer
        Dim DIR$ = args <= "/data"
        Dim out As String = args.GetValue("/out", DIR.TrimDIR & ".heatmap/")
        Dim dataOUT = out & "/DEP.heatmap.csv"
        Dim size$ = args.GetValue("/size", "2000,3000")
        Dim title$ = args.GetValue("/title", "Heatmap of DEPs log2FC")
        Dim tlog2 As Boolean = args.IsTrue("/t.log2")
        Dim matrix As List(Of DataSet) = Union(DIR, tlog2, 0, args.GetBoolean("/non_DEP.blank"), False) _
            .AsDataSet _
            .AsList

        Call matrix _
            .ToKMeansModels _
            .Kmeans(expected:=args.GetValue("/cluster.n", 6)) _
            .SaveTo(dataOUT)

        Dim min# = matrix.Select(Function(d) d.Properties.Values).IteratesALL.Min
        Dim schema$ = args.GetValue("/schema", Colors.ColorBrewer.DivergingSchemes.RdYlGn11)
        Dim revColorSequence As Boolean = Not args.IsTrue("/no-clrev")
        Dim tick# = args.GetValue("/tick", -1.0#)

        If min >= 0 Then
            min = 0
        End If

        If args.IsTrue("/KO.class") Then
            Dim groupInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
            Dim KOinfo As Dictionary(Of String, String) = matrix _
                .Keys _
                .GetKOTable(MySQLExtensions.GetMySQLClient(DBName:=UniprotKBEngine.DbName))

            Call DEPsKOHeatmap _
                .Plot(matrix, groupInfo.SampleGroupInfo, groupInfo.SampleGroupColor, KOInfo:=KOinfo, schema:=schema) _
                .Save(out & "/plot.png")
        Else
            ' 绘制普通的热图
            Call Heatmap.Plot(
                matrix,
                size:=size,
                drawScaleMethod:=DrawElements.Rows,
                mainTitle:=title, rowLabelfontStyle:=CSSFont.Win7Small,
                colLabelFontStyle:=CSSFont.Win7Large,
                mapName:=schema,
                reverseClrSeq:=revColorSequence,
                min:=min,
                tick:=tick).AsGDIImage _
                         .CorpBlank(30, Color.White) _
                         .SaveAs(out & "/plot.png")
        End If

        Return 0
    End Function

    <ExportAPI("/DEP.kmeans.scatter2D")>
    <Usage("/DEP.kmeans.scatter2D /in <kmeans.csv> /sampleInfo <sampleInfo.csv> [/t.log <default=-1> /cluster.prefix <default=""cluster: #""> /size <1600,1400> /schema <default=clusters> /out <out.png>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPKmeansScatter2D(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim sampleInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim size$ = (args <= "/size") Or "1600,1400".AsDefault
        Dim schema$ = (args <= "/schema") Or "clusters".AsDefault
        Dim out$ = (args <= "/out") Or ([in].TrimSuffix & ".scatter2D.png").AsDefault
        Dim clusterData As EntityLDM() = DataSet.LoadDataSet(Of EntityLDM)([in]).ToArray
        Dim prefix$ = (args <= "/cluster.prefix") Or "Cluster:  #".AsDefault
        Dim tlog# = args.GetValue("/t.log", -1.0R)

        If Not prefix.StringEmpty Then
            For Each protein As EntityLDM In clusterData
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

    ''' <summary>
    ''' 进行差异表达蛋白的聚类结果的3D scatter散点图的绘制可视化
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/DEP.heatmap.scatter.3D")>
    <Description("Visualize the DEPs' kmeans cluster result by using 3D scatter plot.")>
    <Usage("/DEP.heatmap.scatter.3D /in <kmeans.csv> /sampleInfo <sampleInfo.csv> [/cluster.prefix <default=""cluster: #""> /size <default=1600,1400> /schema <default=clusters> /view.angle <default=30,60,-56.25> /view.distance <default=2500> /out <out.png>]")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(EntityLDM)},
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
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPHeatmap3D(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim sampleInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim size$ = (args <= "/size") Or "1600,1400".AsDefault
        Dim schema$ = (args <= "/schema") Or "clusters".AsDefault
        Dim out$ = (args <= "/out") Or ([in].TrimSuffix & ".scatter.png").AsDefault
        Dim clusterData As EntityLDM() = DataSet.LoadDataSet(Of EntityLDM)([in]).ToArray
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
            For Each protein As EntityLDM In clusterData
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

    ''' <summary>
    ''' 获取DEPs的原始数据的热图数据
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/DEP.heatmap.raw")>
    <Description("All of the NA value was replaced by value ``1``, as the FC value when it equals 1, then ``log2(1) = 0``, which means it has no changes.")>
    <Usage("/DEP.heatmap.raw /DEPs <DEPs.csv.folder> [/DEP.tag <default=is.DEP> /out <out.csv>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPsHeatmapRaw(args As CommandLine) As Integer
        Dim in$ = args <= "/DEPs"
        Dim raw$ = args <= "/raw"
        Dim DEPTag$ = args.GetValue("/DEP.tag", "is.DEP")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ".heatmap.raw/")
        Dim dataOUT = out & "/DEP.heatmap.raw.csv"

        Return DEGDesigner _
            .GetDEPsRawValues([in], DEPTag) _
            .SaveDataSet(dataOUT) _
            .CLICode
    End Function

    ''' <summary>
    ''' 如果data参数不存在则默认只取出DEP的输入数据之中的is.DEP为真的部分
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/DEPs.takes.values")>
    <Description("")>
    <Usage("/DEPs.takes.values /in <DEPs.csv> [/boolean.tag <default=is.DEP> /by.FC <tag=value, default=logFC=log2(1.5)> /by.p.value <tag=value, p.value=0.05> /data <data.csv> /out <out.csv>]")>
    Public Function TakeDEPsValues(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim isDEP$ = args.GetValue("/boolean.tag", "is.DEP")
        Dim FC$ = args <= "/by.FC"
        Dim pvalue = args <= "/by.p.value"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "-DEPs.values.csv")
        Dim data As Dictionary(Of EntityObject)
        Dim source = EntityObject.LoadDataSet([in])
        Dim DEPs = source _
            .Where(Function(protein)
                       Return protein(isDEP).TextEquals("True")
                   End Function) _
            .ToArray

        With args <= "/data"
            If .FileLength > 0 Then
                data = EntityObject _
                    .LoadDataSet(.ref) _
                    .ToDictionary
            Else
                data = source.ToDictionary
            End If
        End With

        Dim values = DEPs _
            .Where(Function(protein)
                       Return data.ContainsKey(protein.ID)
                   End Function) _
            .Select(Function(protein) data(protein.ID)) _
            .ToArray
        Return values _
            .SaveTo(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 这个函数要求的列名称能够和raw之中的列名称可以一一对应，假若raw参数存在的话
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Merge.DEPs",
               Info:="Usually using for generates the heatmap plot matrix of the DEPs. This function call will generates two dataset, one is using for the heatmap plot and another is using for the venn diagram plot.",
               Usage:="/Merge.DEPs /in <*.csv,DIR> [/log2 /threshold ""log(1.5,2)"" /raw <sample.csv> /out <out.csv>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function MergeDEPs(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim log2 As Boolean = args.GetBoolean("/log2")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & $".data{If(log2, "-log2", "")}.csv")
        Dim raw As String = args("/raw")
        Dim threshold As String = args.GetValue("/threshold", "log(1.5,2)")
        Dim data = (ls - l - r - "*.csv" <= [in]) _
            .Select(Function(x) DataSet.LoadDataSet(path:=x)) _
            .IteratesALL _
            .GroupBy(Function(gene) gene.ID) _
            .ToArray
        Dim allKeys$() = data _
            .Select(Function(g) g.Select(Function(o) o.Properties.Keys)) _
            .IteratesALL _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim t = Function(n#)
                    If log2 Then
                        Return System.Math.Log(n, newBase:=2)
                    Else
                        Return n
                    End If
                End Function
        Dim output As New File

        If raw.FileExists(True) Then
            Dim sample As Dictionary(Of DataSet) = DataSet.LoadDataSet(path:=raw).ToDictionary
            Dim row As New RowObject({"ID"}.JoinIterates(allKeys))

            output += row

            For Each gene In data
                Dim gData = sample(gene.Key)

                row = New RowObject()
                row += gene.Key
                row += allKeys.Select(Function(k) t(n:=gData(k)).ToString)
                output += row
            Next

            Call output.Save(out.TrimSuffix & "-heatmapData.csv")
        Else

        End If

        ' 在下面生成文氏图的数据
        Dim cut# = (New Expression).Evaluation(threshold)

        For Each row In output.Skip(1)
            For i As Integer = 1 To row.Width - 1
                row(i) = If(Math.Abs(Val(row(i))) >= cut, row(i), "")
            Next
        Next

        Call output.Save(out.TrimSuffix & "-vennData.csv")

        Return 0
    End Function

    ''' <summary>
    ''' 当没有任何生物学重复的时候，就只能够使用这个函数进行FoldChange的直方图的绘制了
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/DEP.logFC.hist")>
    <Description("Using for plots the FC histogram when the experiment have no biological replicates.")>
    <Usage("/DEP.logFC.hist /in <log2test.csv> [/step <0.5> /tag <logFC> /legend.title <Frequency(logFC)> /x.axis ""(min,max),tick=0.25"" /color <lightblue> /size <1600,1200> /out <out.png>]")>
    <Argument("/tag", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="Which field in the input dataframe should be using as the data source for the histogram plot? Default field(column) name is ""logFC"".")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function logFCHistogram(args As CommandLine) As Integer
        Dim [in] = args("/in")
        Dim tag As String = args.GetValue("/tag", "logFC")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".{tag.NormalizePathString}.histogram.png")
        Dim data = EntityObject.LoadDataSet([in])
        Dim xAxis As String = args("/x.axis")
        Dim step! = args.GetValue("/step", 0.5!)
        Dim lTitle$ = args.GetValue("/legend.title", "Frequency(logFC)")
        Dim color$ = args.GetValue("/color", "lightblue")

        Return data _
            .logFCHistogram(tag,
                            size:=(args <= "/size") Or "1600,1200".AsDefault,
                            [step]:=[step],
                            xAxis:=xAxis,
                            serialTitle:=lTitle,
                            color:=color) _
            .Save(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 绘制差异蛋白的火山图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/DEP.logFC.Volcano")>
    <Usage("/DEP.logFC.Volcano /in <DEP-log2FC.t.test-table.csv> [/title <title> /p.value <default=0.05> /level <default=1.5> /colors <up=red;down=green;other=black> /size <1400,1400> /display.count /out <plot.csv>]")>
    <Description("Volcano plot of the DEPs' analysis result.")>
    <Argument("/size", True, CLITypes.String,
              Description:="The canvas size of the output image.")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(DEP_iTraq)},
              Description:="The input DEPs t.test result, should contains at least 3 columns which are names: ``ID``, ``log2FC`` and ``p.value``")>
    <Argument("/colors", True, CLITypes.String,
              Description:="The color profile for the DEPs and proteins that no-changes, value string in format like: key=value, and seperated by ``;`` symbol.")>
    <Argument("/title", True, CLITypes.String, Description:="The plot main title.")>
    <Argument("/p.value", True, CLITypes.Double, Description:="The p.value cutoff threshold, default is 0.05.")>
    <Argument("/level", True, CLITypes.Double, Description:="The log2FC value cutoff threshold, default is ``log2(1.5)``.")>
    <Argument("/display.count", True, CLITypes.Boolean, Description:="Display the protein counts in the legend label? by default is not.")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function logFCVolcano(args As CommandLine) As Integer
        Dim out$ = args.GetValue("/out", (args <= "/in").TrimSuffix & ".DEPs.vocano.plot.png")
        Dim sample = EntityObject.LoadDataSet(Of DEP_iTraq)(args <= "/in")
        Dim size$ = args.GetValue("/size", "1400,1400")
        Dim title$ = (args <= "/title") Or ("Volcano plot of " & (args <= "/in").BaseName).AsDefault
        Dim colors As Dictionary(Of Integer, Color) = args _
            .GetDictionary("/colors", [default]:="up=red;down=green;other=black") _
            .ToDictionary(Function(type)
                              Return CInt(DEGDesigner.ParseDEGTypes(type.Key))
                          End Function,
                          Function(color)
                              Return color.Value.TranslateColor
                          End Function)
        Dim log2FCLevel# = args.GetValue("/level", 1.5)
        Dim pvalue# = args.GetValue("/p.value", 0.05)
        Dim P = -Math.Log10(pvalue)
        Dim displayCount As Boolean = args.IsTrue("/display.count")
        Dim toFactor = Function(x As DEGModel)
                           If x.pvalue < P Then
                               Return 0
                           ElseIf Math.Abs(x.logFC) < Math.Log(log2FCLevel, 2) Then
                               Return 0
                           End If

                           If x.logFC > 0 Then
                               Return 1
                           Else
                               Return -1
                           End If
                       End Function

        Return Volcano.Plot(sample,
                            colors:=colors,
                            factors:=toFactor,
                            padding:="padding: 50 50 150 150",
                            displayLabel:=LabelTypes.None,
                            size:=size,
                            log2Threshold:=log2FCLevel,
                            pvalueThreshold:=pvalue,
                            title:=title,
                            displayCount:=displayCount) _
            .AsGDIImage _
            .CorpBlank(30, Color.White) _
            .SaveAs(out) _
            .CLICode
    End Function

    <ExportAPI("/DEPs.stat",
               Info:="https://github.com/xieguigang/GCModeller.cli2R/blob/master/GCModeller.cli2R/R/log2FC_t-test.R",
               Usage:="/DEPs.stat /in <log2.test.csv> [/log2FC <default=log2FC> /out <out.stat.csv>]")>
    <Argument("/log2FC", True, CLITypes.String, Description:="The field name that stores the log2FC value of the average FoldChange")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPStatics(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".DEPs.stat.csv")
        Dim log2FC$ = args.GetValue("/log2FC", "log2FC")
        Dim DEPs As EntityObject() = EntityObject _
           .LoadDataSet(path:=in$) _
           .Where(Function(d) d("is.DEP").TextEquals("TRUE")) _
           .ToArray
        Dim result As New File

        result += {"上调", "下调", "总数"}
        result += {
            DEPs _
                .Where(Function(prot) Val(prot(log2FC)) > 0) _
                .Count _
                .ToString,
            DEPs _
                .Where(Function(prot) Val(prot(log2FC)) < 0) _
                .Count _
                .ToString,
            CStr(DEPs.Length)
        }

        Return result.Save(out).CLICode
    End Function
End Module
