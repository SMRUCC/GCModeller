#Region "Microsoft.VisualBasic::293382cb2d197606c02af791b3541119, CLI_tools\eggHTS\CLI\2. DEP.vb"

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
    '     Function: DEPs_heatmapKmeans, DEPsCloudPlot, DEPStatics, DEPsUnion, DEPUniprotIDlist
    '               DEPUniprotIDs2, edgeRDesigner, iTraqInvert, logFCHistogram, logFCVolcano
    '               MergeDEPs, PairedSampleDesigner, TakeDEPsValues, TtestDesigner, TtestDesignerLFQ
    '               Union, unionDATA, VennData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics.Heatmap
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
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
Imports ColorDesigner = Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Designer

Partial Module CLI

#Region "DEG data experiment designer"

    <ExportAPI("/paired.sample.designer")>
    <Usage("/paired.sample.designer /sampleinfo <sampleInfo.csv> /designer <analysisDesigner.csv> /tuple <sampleTuple.csv> [/out <designer.out.csv.Directory>]")>
    Public Function PairedSampleDesigner(args As CommandLine) As Integer
        Dim in$ = args <= "/sampleInfo"
        Dim designer$ = args <= "/designer"
        Dim out$ = (args <= "/out") Or $"{[in].TrimSuffix}_{designer.BaseName}_sampleTuple".AsDefault

        For Each group As NamedCollection(Of SampleTuple) In [in] _
            .LoadCsv(Of SampleInfo) _
            .PairedAnalysisSamples(
                designer.LoadCsv(Of AnalysisDesigner),
                (args <= "/tuple").LoadCsv(Of SampleTuple))

            With group
                Call .value.SaveTo($"{out}/{ .name.NormalizePathString}.csv")
            End With
        Next

        Return 0
    End Function

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
    ''' 最后的所有的剩余的列数据都是FC值
    ''' 
    ''' ``Accession	T1.C1	T1.C2	T2.C1	T2.C2``
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/FoldChange.Matrix.Invert")>
    <Description("Reverse the FoldChange value from the source result matrix.")>
    <Usage("/FoldChange.Matrix.Invert /in <data.csv> [/log2FC /out <invert.csv>]")>
    <Argument("/log2FC", True, CLITypes.Boolean,
              Description:="This boolean flag indicated that the fold change value is log2FC, which required of power 2 and then invert by divided by 1.")>
    <Argument("/out", True, CLITypes.File, PipelineTypes.std_out,
              Extensions:="*.csv",
              Description:="This function will output a FoldChange matrix.")>
    <Group(CLIGroups.SamplesExpressions_CLI)>
    Public Function iTraqInvert(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".invert.csv")
        Dim data As DataSet() = DataSet.LoadDataSet([in]).ToArray
        Dim isLog2FC As Boolean = args.IsTrue("/log2FC")
        Dim invert As New List(Of DataSet)

        For Each protein As DataSet In data
            invert += New DataSet With {
                .ID = protein.ID,
                .Properties = protein _
                    .Properties _
                    .ToDictionary(Function(map)
                                      ' 假设列的标题是A/B，则颠倒过来之后应该是B/A
                                      With map.Key.Split("/"c)
                                          Return $"{ .Last}/{ .First}"
                                      End With
                                  End Function,
                                  Function(map)
                                      Dim foldChange# = map.Value

                                      If isLog2FC Then
                                          foldChange = 2 ^ foldChange
                                      End If

                                      Return 1 / foldChange
                                  End Function)
            }
        Next

        Return invert _
            .SaveTo(out) _
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
                          Function(g) g.Select(
                          Function(p) p.ID).ToArray)
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
        Dim in$ = args("/in")
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
        Dim out As String = args("/out") Or ([in].TrimSuffix & $"DEPs={DEPs.Length}.uniprotIDs.txt")

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
    <Argument("/data", False, CLITypes.File, PipelineTypes.std_in, Description:="A directory path which it contains the DEPs matrix csv files from the sample groups's analysis result.")>
    <Argument("/out", True, CLITypes.File, Description:="A directory path which it will contains the venn data result, includes venn matrix, venn plot tiff image, etc.")>
    <Argument("/title", True, CLITypes.String, Description:="The main title of the venn plot.")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function VennData(args As CommandLine) As Integer
        Dim DIR$ = args("/data")
        Dim out As String = args.GetValue("/out", DIR.TrimDIR & ".venn/")
        Dim dataOUT = out & "/DEP.venn.csv"
        Dim title$ = args.GetValue("/title", "VennDiagram title")

        Call Union(DIR, True, "", nonDEP_blank:=True, outGroup:=True, isLabelFree:=False) _
            .SaveDataSet(dataOUT)
        Call Apps.VennDiagram.VennDiagramA(dataOUT, title, o:=out & "/venn.tiff", first_id_skip:=True)

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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="DIR$"></param>
    ''' <param name="tlog2"></param>
    ''' <param name="ZERO$"></param>
    ''' <param name="nonDEP_blank"></param>
    ''' <param name="outGroup">如果这个参数为真，说明是生成的文氏图的数据矩阵</param>
    ''' <param name="isLabelFree">
    ''' 如果是LabelFree的结果，则列标题很有可能有重复的值出现，在这里需要额外的处理
    ''' </param>
    ''' <returns></returns>
    Public Function Union(DIR$, tlog2 As Boolean, ZERO$, nonDEP_blank As Boolean, outGroup As Boolean, isLabelFree As Boolean) As List(Of EntityObject)
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
                        With .ByRef(id)
                            If .isDEP Then
                                If outGroup Then

                                    If isLabelFree Then
                                        FClog2(group.Key) = .log2FC
                                    Else
                                        FClog2.Add(group.Key, .log2FC)
                                    End If

                                Else

                                    For Each prop In .Properties
                                        If prop.Value.TextEquals("NA") Then
                                            If isLabelFree Then
                                                FClog2(group.Key) = ZERO
                                            Else
                                                FClog2.Add(prop.Key, ZERO)
                                            End If
                                        Else
                                            If tlog2 Then
                                                If isLabelFree Then
                                                    FClog2(prop.Key) = Math.Log(Val(prop.Value), 2)
                                                Else
                                                    FClog2.Add(prop.Key, Math.Log(Val(prop.Value), 2))
                                                End If
                                            Else
                                                If isLabelFree Then
                                                    FClog2(prop.Key) = Val(prop.Value)
                                                Else
                                                    FClog2.Add(prop.Key, Val(prop.Value))
                                                End If
                                            End If
                                        End If
                                    Next

                                End If
                            Else
                                If nonDEP_blank Then

                                    If outGroup Then
                                        If isLabelFree Then
                                            FClog2(group.Key) = ZERO
                                        Else
                                            FClog2.Add(group.Key, ZERO)
                                        End If
                                    Else

                                        If isLabelFree Then
                                            For Each prop In .Properties
                                                FClog2(prop.Key) = ZERO
                                            Next
                                        Else
                                            For Each prop In .Properties
                                                FClog2.Add(prop.Key, ZERO) ' log2(1) = 0
                                            Next
                                        End If
                                    End If

                                Else

                                    If outGroup Then
                                        If isLabelFree Then
                                            FClog2(group.Key) = .log2FC
                                        Else
                                            FClog2.Add(group.Key, .log2FC)
                                        End If
                                    Else

                                        For Each prop In .Properties
                                            If prop.Value.TextEquals("NA") Then
                                                If isLabelFree Then
                                                    FClog2(prop.Key) = ZERO
                                                Else
                                                    FClog2.Add(prop.Key, ZERO)
                                                End If
                                            Else
                                                If tlog2 Then
                                                    If isLabelFree Then
                                                        FClog2(prop.Key) = Math.Log(Val(prop.Value), 2)
                                                    Else
                                                        FClog2.Add(prop.Key, Math.Log(Val(prop.Value), 2))
                                                    End If
                                                Else
                                                    If isLabelFree Then
                                                        FClog2(prop.Key) = Val(prop.Value)
                                                    Else
                                                        FClog2.Add(prop.Key, Val(prop.Value))
                                                    End If
                                                End If
                                            End If
                                        Next

                                    End If


                                End If
                            End If
                        End With
                    Else

                        If outGroup Then
                            If isLabelFree Then
                                FClog2(group.Key) = ZERO
                            Else
                                FClog2.Add(group.Key, ZERO)
                            End If
                        Else
                            If isLabelFree Then
                                For Each key In data(group.Key).Values.First.Properties.Keys
                                    FClog2(key) = ZERO
                                Next
                            Else
                                For Each key In data(group.Key).Values.First.Properties.Keys
                                    FClog2.Add(key, ZERO)
                                Next
                            End If
                        End If
                    End If
                End With
            Next

            matrix += New EntityObject With {
                .ID = id,
                .Properties = FClog2
            }
        Next

        If outGroup Then
            matrix = matrix _
                .Select(Function(d)
                            Return New EntityObject With {
                                .ID = d.ID,
                                .Properties = d _
                                    .Properties _
                                    .ToDictionary(Function(map)
                                                      Return map.Key.Split("."c).First
                                                  End Function,
                                                  Function(map) map.Value)
                            }
                        End Function) _
                .AsList
        End If

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
    <ExportAPI("/DEPs.heatmap")>
    <Description("Generates the heatmap plot input data. The default label profile is using for the iTraq result.")>
    <Usage("/DEPs.heatmap /data <Directory/csv_file> [/labelFree /schema <color_schema, default=RdYlGn:c11> /no-clrev /KO.class /annotation <annotation.csv> /row.labels.geneName /hide.labels /is.matrix /cluster.n <default=6> /sampleInfo <sampleinfo.csv> /non_DEP.blank /title ""Heatmap of DEPs log2FC"" /t.log2 /tick <-1> /size <size, default=2000,3000> /legend.size <size, default=600,100> /out <out.DIR>]")>
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
    <Argument("/is.matrix", True,
              CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="The input data is a data matrix, can be using for heatmap drawing directly.")>
    <Argument("/row.labels.geneName", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="This option will use the ``geneName``(from the annotation data) as the row display label instead of using uniprotID or geneID. This option required of the ``/annotation`` presented.")>
    <Argument("/annotation", True, CLITypes.File,
              AcceptTypes:={GetType(EntityObject)},
              Extensions:="*.csv",
              Description:="The protein annotation data that extract from the uniprot database. Some advanced heatmap plot feature required of this annotation data presented.")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPs_heatmapKmeans(args As CommandLine) As Integer
        Dim input$ = args <= "/data"
        Dim out As String = args.GetValue("/out", input.TrimDIR & ".heatmap/")
        Dim dataOUT = out & "/DEP.heatmap.csv"
        Dim size$ = args.GetValue("/size", "2000,3000")
        Dim title$ = args.GetValue("/title", "Heatmap of DEPs log2FC")
        Dim tlog2 As Boolean = args.IsTrue("/t.log2")
        Dim matrix As List(Of DataSet)
        Dim annotations As EntityObject() = EntityObject _
            .LoadDataSet(args <= "/annotation") _
            .ToArray
        Dim isLabelFree As Boolean = args("/labelFree")

        If args.IsTrue("/is.matrix") Then
            matrix = DataSet _
                .LoadDataSet(input) _
                .AsList

            If tlog2 Then
                matrix = matrix.Log2.AsList
            End If
        Else
            matrix = Union(input, tlog2, 0, args.GetBoolean("/non_DEP.blank"), False, isLabelFree) _
                .AsDataSet _
                .AsList
        End If

        If args.IsTrue("/row.labels.geneName") Then
            ' 将matrix之中的编号替换为geneName
            Dim ID2names = annotations.ToDictionary(Function(x) x.ID,
                                                    Function(x) x!geneName)
            For Each protein As DataSet In matrix
                If ID2names.ContainsKey(protein.ID) Then
                    ' 可能有些蛋白是还没有基因名的，则这个时候会是空的字符串
                    ' 这些空字符串还可能会出现多个蛋白上面，从而导致后面的程序崩溃
                    ' 所以在这里需要跳过那些空的基因名的蛋白质

                    If Not Strings.Trim(ID2names(protein.ID)).StringEmpty Then
                        protein.ID = ID2names(protein.ID).Trim
                    End If
                End If
            Next
        End If

        matrix = matrix _
            .Where(Function(d)
                       Return d.Properties.Values.Any(Function(n) n <> 0R)
                   End Function) _
            .AsList

        With matrix _
            .ToKMeansModels _
            .Kmeans(expected:=args.GetValue("/cluster.n", 6))

            ' 保存用于绘制3D/2D聚类图的数据集
            Call .ToEntityObjects _
                 .ToArray _
                 .SaveDataSet(dataOUT, Encodings.UTF8)

            ' 保存能够应用于R脚本进行热图绘制的矩阵数据
            Call .Select(Function(d)
                             Return New DataSet With {
                                 .ID = d.ID,
                                 .Properties = d.Properties
                             }
                         End Function) _
                 .AsCharacter _
                 .ToArray _
                 .SaveDataSet(dataOUT.TrimSuffix & ".heampa_Matrix.csv", Encodings.UTF8)
        End With

        Dim schema$ = args.GetValue("/schema", Colors.ColorBrewer.DivergingSchemes.RdYlGn11)
        Dim revColorSequence As Boolean = Not args.IsTrue("/no-clrev")
        Dim tick# = args.GetValue("/tick", -1.0#)
        Dim legendSize$ = (args <= "/legend.size") Or "600,100".AsDefault
        Dim min# = matrix _
            .Select(Function(d) d.Properties.Values) _
            .IteratesALL _
            .Min

        If min >= 0 Then
            min = 0
        End If

        If args.IsTrue("/KO.class") Then
            Dim groupInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
            Dim KOinfo As Dictionary(Of String, String) = matrix _
                .Keys _
                .GetKOTable(MySQLExtensions.GetMySQLClient(DBName:=UniprotKBEngine.DbName))
            Dim colors As Color() = ColorDesigner.GetColors("scibasic.category31()")

            Call DEPsKOHeatmap _
                .Plot(matrix, groupInfo.SampleGroupInfo, groupInfo.SampleGroupColor(colors), KOInfo:=KOinfo, schema:=schema) _
                .Save(out & "/plot.png")
        Else
            ' 绘制普通的热图
            Call Heatmap.Plot(
                matrix,
                size:=size,
                drawScaleMethod:=DrawElements.Rows,
                mainTitle:=title,
                rowLabelfontStyle:=CSSFont.Win7LargerNormal,
                colLabelFontStyle:=CSSFont.Win7LittleLarge,
                mapName:=schema,
                reverseClrSeq:=revColorSequence,
                legendSize:=legendSize,
                min:=min,
                tick:=tick).AsGDIImage _
                         .CorpBlank(30, Color.White) _
                         .SaveAs(out & "/plot.png")
        End If

        Return 0
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
                    .LoadDataSet(.ByRef) _
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
        Dim cut# = (New ExpressionEngine).Evaluate(threshold)

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
    <Usage("/DEP.logFC.hist /in <log2test.csv> [/step <0.25> /type <default=log2fc> /legend.title <Frequency(log2FC)> /x.axis ""(min,max),tick=0.25"" /color <lightblue> /size <1400,900> /out <out.png>]")>
    <Argument("/type", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="Which field in the input dataframe should be using as the data source for the histogram plot? Default field(column) name is ""log2FC"".")>
    <Argument("/step", True, CLITypes.Double,
              AcceptTypes:={GetType(Single)},
              Description:="The steps for generates the histogram test data.")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function logFCHistogram(args As CommandLine) As Integer
        Dim [in] = args("/in")
        Dim out As String = (args <= "/out") Or $"{[in].TrimSuffix}.log2FC.histogram.png".AsDefault
        Dim data = [in].LoadCsv(Of DEP_iTraq)
        Dim type$ = (args <= "/type") Or NameOf(DEP_iTraq.log2FC).AsDefault
        Dim xAxis As String = args("/x.axis")
        Dim step! = args.GetFloat("/step") Or 0.25!.AsDefault(Function(x) DirectCast(x, Single) = 0!)
        Dim lTitle$ = args.GetValue("/legend.title", "Frequency(log2FC)")
        Dim color$ = args.GetValue("/color", "darkblue")
        Dim size$ = (args <= "/size") Or "1440,900".AsDefault

        Return data _
            .logFCHistogram(size:=size,
                            [step]:=[step],
                            xAxis:=xAxis,
                            serialTitle:=lTitle,
                            color:=color,
                            type:=type) _
            .Save(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 绘制差异蛋白的火山图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/DEP.logFC.Volcano")>
    <Usage("/DEP.logFC.Volcano /in <DEP-log2FC.t.test-table.csv> [/title <title> /p.value <default=0.05> /level <default=1.5> /colors <up=red;down=green;other=black> /label.p <default=-1> /size <1400,1400> /display.count /out <plot.csv>]")>
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
    <Argument("/label.p", True, CLITypes.Boolean, Description:="Display the DEP protein name on the plot? by default -1 means not display. using this parameter for set the P value cutoff of the DEP for display labels.")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function logFCVolcano(args As CommandLine) As Integer
        Dim out$ = args.GetValue("/out", (args <= "/in").TrimSuffix & ".DEPs.vocano.plot.png")
        Dim sample = EntityObject.LoadDataSet(Of DEP_iTraq)(args <= "/in")
        Dim size$ = args.GetValue("/size", "1400,1400")
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
        Dim labelP As Double = args("/label.p") Or -1.0
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

        ' 如果是使用系统生成默认的名称的话，则文件名的模式为： groupName.log2FC.t.test.csv
        ' 使用split取第一个字符串即可得到groupName
        Dim title$ = (args <= "/title") Or ("Volcano plot of " & (args <= "/in").BaseName.Split("."c).First).AsDefault

        If log2FCLevel = 0R Then
            Call "log2FC level can not be ZERO! please check for the /level parameter!".Warning
            Throw New ArgumentOutOfRangeException("/level")
        End If

        Dim labelDisplay As LabelTypes = LabelTypes.None

        If labelP > 0 Then
            labelDisplay = LabelTypes.Custom
        End If

        Return Volcano.Plot(sample,
                            colors:=colors,
                            factors:=toFactor,
                            padding:="padding: 50 50 150 150",
                            displayLabel:=labelDisplay,
                            size:=size,
                            log2Threshold:=log2FCLevel,
                            pvalueThreshold:=pvalue,
                            title:=title,
                            displayCount:=displayCount,
                            labelP:=labelP) _
            .AsGDIImage _
            .CorpBlank(30, Color.White) _
            .SaveAs(out) _
            .CLICode
    End Function

    <ExportAPI("/DEPs.stat",
               Info:="https://github.com/xieguigang/GCModeller.cli2R/blob/master/GCModeller.cli2R/R/log2FC_t-test.R",
               Usage:="/DEPs.stat /in <log2.test.csv> [/log2FC <default=log2FC> /out <out.stat.csv>]")>
    <Argument("/log2FC", True, CLITypes.String, Description:="The field name that stores the log2FC value of the average FoldChange")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in, Extensions:="*.csv", Description:="The DEPs' t.test result in csv file format.")>
    <Argument("/out", True, CLITypes.File, PipelineTypes.std_out, Description:="The stat count output file path.")>
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

        result += {"组别", "上调", "下调", "总数"}
        result += {
            [in].BaseName.Split("."c).First,
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
