#Region "Microsoft.VisualBasic::8bc786eb98ff986e7e55bdaeec6aaf77, CLI_tools\eggHTS\CLI\Enrichment\KOBAS.vb"

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
    '     Function: GO_cellularLocationPlot, GO_enrichmentPlot, KEGG_enrichment, KEGGEnrichmentPathwayMap, KOBASaddORFsource
    '               KOBASSplit, RetriveEnrichmentGeneInfo
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Fractions
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.GO.PlantRegMap
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.CatalogProfiling

Partial Module CLI

    ''' <summary>
    ''' 绘制GO分析之中的亚细胞定位结果的饼图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/GO.cellular_location.Plot")>
    <Description("Visualize of the subcellular location result from the GO enrichment analysis.")>
    <Usage("/GO.cellular_location.Plot /in <KOBAS.GO.csv> [/GO <go.obo> /3D /colors <schemaName, default=Paired:c8> /out <out.png>]")>
    <Argument("/3D", True,
              Description:="3D style pie chart for the plot?")>
    <Argument("/colors", True,
              Description:="Color schema name, default using color brewer color schema.")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function GO_cellularLocationPlot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim colors$ = args.GetValue("/colors", "Paired:c8")
        Dim using3D As Boolean = args.GetBoolean("/3D")
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".GO_cellularLocationPlot.png")
        Dim data As EnrichmentTerm() = [in].LoadCsv(Of EnrichmentTerm)
        Dim go$ = args.GetValue("/go", GCModeller.FileSystem.GO & "/Go.obo")
        Dim CCterms As Dictionary(Of Term) = GO_OBO _
            .Open(go) _
            .Where(Function(t) t.namespace = "cellular_component") _
            .ToDictionary
        Dim profiles As List(Of NamedValue(Of Integer)) =
            data _
            .Where(Function(x) CCterms.ContainsKey(x.ID)) _
            .Select(Function(g)
                        Return New NamedValue(Of Integer) With {
                            .Name = g.ID,
                            .Value = g.Input.Split("|"c).Length,
                            .Description = CCterms(g.ID).name
                        }
                    End Function) _
            .AsList  ' 分组计数

        Call profiles _
            .OrderByDescending(Function(cc) cc.Value) _
            .ToArray _
            .SaveTo(out.TrimSuffix & ".csv")

        Dim q As QuantileEstimationGK = profiles.Select(Function(cc) CDbl(cc.Value)).GKQuantile
        Dim quantile# = q.Query(0.75)
        Dim others As New List(Of NamedValue(Of Integer))

        For Each cc In profiles.ToArray
            If cc.Value <= quantile Then
                others += cc
                profiles -= cc
            End If
        Next

        profiles += New NamedValue(Of Integer) With {
            .Name = "Others",
            .Value = others.Sum(Function(cc) cc.Value)
        }

        If using3D Then
            Call profiles _
                .Fractions(colors) _
                .Plot3D(
                    New Camera With {
                        .screen = New Size(3600, 2500),
                        .ViewDistance = -3.4,
                        .angleZ = 30,
                        .angleX = 30,
                        .angleY = -30,
                        .offset = New PointF(0, -100)
                }).Save(out)
        Else
            Call profiles.Fractions(colors).Plot.Save(out)
        End If

        Return 0
    End Function

    ''' <summary>
    ''' go enrichment 绘图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Go.enrichment.plot")>
    <Usage("/Go.enrichment.plot /in <enrichmentTerm.csv> [/bubble /r ""log(x,1.5)"" /Corrected /displays <default=10> /PlantRegMap /label.right /label.color.disable /label.maxlen <char_count, default=64> /colors <default=Set1:c6> /gray /pvalue <0.05> /size <2000,1600> /tick 1 /go <go.obo> /out <out.png>]")>
    <Description("Go enrichment plot base on the KOBAS enrichment analysis result.")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.csv",
              Description:="The KOBAS enrichment analysis output csv file.")>
    <Argument("/out", True, CLITypes.File, PipelineTypes.std_out,
              Out:=True,
              Extensions:="*.svg, *.png",
              Description:="The file path of the output plot image. If the graphics driver is using svg engine, then this result can be output to the standard output if this parameter is not presented in the CLI input.")>
    <Argument("/r", True, CLITypes.String,
              Description:="The bubble radius expression, when this enrichment plot is in ``/bubble`` mode.")>
    <Argument("/label.right", True, CLITypes.Boolean,
              Description:="Align the label to right if this argument presented.")>
    <Argument("/Corrected", True, CLITypes.Boolean,
              Description:="Using the corrected p.value instead of using the p.value as the term filter for this enrichment plot.")>
    <Argument("/pvalue", True, CLITypes.Double,
              Description:="The p.value threshold for choose the terms that will be plot on the image, default is plot all terms that their enrichment p.value is smaller than 0.05.")>
    <Argument("/size", True, CLITypes.String,
              AcceptTypes:={GetType(Size)},
              Description:="The output image size in pixel.")>
    <Argument("/tick", True, CLITypes.Double,
              Description:="The axis ticking interval value, using **-1** for generated this value automatically, or any other positive numeric value will setup this interval value manually.")>
    <Argument("/GO", True, CLITypes.File,
              Extensions:="*.obo",
              Description:="The GO database for category the enrichment term result into their corrisponding Go namespace. If this argument value is not presented in the CLI input, then program will using the GO database file from the GCModeller repository data system.")>
    <Argument("/bubble", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Visuallize the GO enrichment analysis result using bubble plot, not the bar plot.")>
    <Argument("/displays", True, CLITypes.Integer,
              AcceptTypes:={GetType(Integer)},
              Description:="If the ``/bubble`` argument is not presented, then this will means the top number of the enriched term will plot on the barplot, else it is the term label display number in the bubble plot mode. 
              Set this argument value to -1 for display all terms.")>
    <Argument("/gray", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Set the color of all of the labels, bars, class labels on this chart plot output to color gray? If this presented, then color schema will not working. Otherwise if this parameter argument is not presented in the CLI input, then the labels and bars will render color based on their corresponding GO namespace.")>
    <Argument("/colors", True, CLITypes.String, PipelineTypes.undefined,
              AcceptTypes:={GetType(String), GetType(String())},
              Description:="Change the default color profiles of the categories plots. Value can be a color profile name term or color name list that join by delimiter comma symbol:
              
              + <profile name term>: Set1:c6 
              Full list of the profile names: https://github.com/xieguigang/sciBASIC/blob/master/gr/Colors/colorbrewer/colorbrewer.json
              + <color name list>: black,green,blue 
              Full list of the color names: https://github.com/xieguigang/sciBASIC/blob/master/etc/VB.NET_Colors.html")>
    <Group(CLIGroups.Enrichment_CLI)>
    <Note(DesignerTerms.TermHelpInfo)>
    Public Function GO_enrichmentPlot(args As CommandLine) As Integer
        Dim goDB As String = args("/go") Or (GCModeller.FileSystem.GO & "/go.obo")
        Dim terms = GO_OBO.Open(goDB).ToDictionary
        Dim [in] As String = args("/in")
        Dim PlantRegMap As Boolean = args.GetBoolean("/PlantRegMap")
        Dim pvalue As Double = args.GetValue("/pvalue", 0.05)
        Dim size As String = args.GetValue("/size", "2000,1600")
        Dim plot As GraphicsData
        Dim bubbleStyle As Boolean = args("/bubble")
        Dim tick# = args.GetValue("/tick", 1.0R)
        Dim gray As Boolean = args.GetBoolean("/gray")
        Dim labelRight As Boolean = args.GetBoolean("/label.right")
        Dim usingCorrected As Boolean = args.GetBoolean("/Corrected")
        Dim labelColorDisable As Boolean = args("/label.color.disable")
        Dim labelMaxlen As Integer = args("/label.maxlen") Or 64
        Dim out As String = args("/out") Or ([in].TrimSuffix & $".GO_enrichment.pvalue={pvalue}_{If(bubbleStyle, "bubbles", "barplot")}.png")

        If PlantRegMap Then
            Dim enrichments As IEnumerable(Of PlantRegMap_GoTermEnrichment) =
                [in].LoadTsv(Of PlantRegMap_GoTermEnrichment)
            plot = enrichments.PlantEnrichmentPlot(terms, pvalue, size, tick)
            enrichments.ToArray.SaveTo([in].TrimSuffix & ".csv")
        Else
            Dim enrichments As IEnumerable(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)
            ' The term/label display number
            Dim displays% = args("/displays") Or 10

            If bubbleStyle Then
                ' 获取半径的计算公式     
                Dim R$ = args("/r") Or "log(x,1.5)"

                plot = enrichments.BubblePlot(GO_terms:=terms,
                                              pvalue:=pvalue,
                                              R:=R,
                                              size:=size,
                                              displays:=displays,
                                              padding:="padding:200px 900px 250px 300px;",
                                              correlatedPvalue:=usingCorrected
                )
            Else
                plot = enrichments.EnrichmentPlot(
                    terms, pvalue, size,
                    tick,
                    gray, labelRight,
                    top:=displays,
                    colorSchema:=args("/colors") Or DefaultColorSchema,
                    disableLabelColor:=labelColorDisable,
                    labelMaxLen:=labelMaxlen
                )
            End If
        End If

        Return plot.Save(out).CLICode
    End Function

    <ExportAPI("/KEGG.enrichment.plot")>
    <Description("Bar plots of the KEGG enrichment analysis result.")>
    <Usage("/KEGG.enrichment.plot /in <enrichmentTerm.csv> [/gray /colors <default=Set1:c6> /top <default=13> /label.right /pvalue <0.05> /tick 1 /size <2000,1600> /out <out.png>]")>
    <Argument("/colors", True, CLITypes.String, PipelineTypes.undefined,
              AcceptTypes:={GetType(String), GetType(String())},
              Description:="Change the default color profiles of the categories plots. Value can be a color profile name term or color name list that join by delimiter comma symbol:
              
              + <profile name term>: Set1:c6 
              Full list of the profile names: https://github.com/xieguigang/sciBASIC/blob/master/gr/Colors/colorbrewer/colorbrewer.json
              + <color name list>: black,green,blue 
              Full list of the color names: https://github.com/xieguigang/sciBASIC/blob/master/etc/VB.NET_Colors.html,
              + <scale by value>: scale(color_set_name)
              This will create color profiles based on the result value dataset.")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function KEGG_enrichment(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim enrichments As IEnumerable(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)
        Dim pvalue As Double = args("/pvalue") Or 0.05
        Dim out As String = args("/out") Or ([in].TrimSuffix & $".KEGG_enrichment.pvalue={pvalue}.png")
        Dim size As String = args("/size") Or "2000,1600"
        Dim gray As Boolean = args.GetBoolean("/gray")
        Dim labelRight As Boolean = args.GetBoolean("/label.right")
        Dim tick As Double = args("/tick") Or 1.0
        Dim topN As Integer = args("/top") Or 13
        Dim plot As GraphicsData = enrichments.KEGGEnrichmentPlot(
            size, pvalue,
            gray:=gray,
            labelRightAlignment:=labelRight,
            tick:=tick,
            colorSchema:=args("/colors") Or DefaultColorSchema,
            topN:=topN
        )

        Return plot.Save(out).CLICode
    End Function

    ''' <summary>
    ''' ``/ORF``参数是表示使用uniprot注释数据库之中的ORF值来作为查找的键名
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Enrichments.ORF.info")>
    <Description("Retrive KEGG/GO info for the genes in the enrichment result.")>
    <Usage("/Enrichments.ORF.info /in <enrichment.csv> /proteins <uniprot-genome.XML> [/nocut /ORF /out <out.csv>]")>
    <Argument("/ORF", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this argument presented, then the program will using the ORF value in ``uniprot.xml`` as the record identifier, 
              default is using uniprotID in the accessions fields of the uniprot.XML records.")>
    <Argument("/nocut", True,
              Description:="Default is using pvalue < 0.05 as term cutoff, if this argument presented, then will no pavlue cutoff for the terms input.")>
    <Argument("/in",
              AcceptTypes:={GetType(EnrichmentTerm)},
              Description:="KOBAS analysis result output.")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function RetriveEnrichmentGeneInfo(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim proteins$ = args <= "/proteins"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "__Enrichments.ORF.Info.csv")
        Dim csv As New File
        Dim getORF As Boolean = args.GetBoolean("/ORF")
        Dim uniprot As Dictionary(Of String, entry)
        Dim pvalue# = If(args.GetBoolean("/nocut"), 100.0#, 0.05)

        If getORF Then
            uniprot = UniProtXML _
                .Load(proteins) _
                .entries _
                .Where(Function(x) Not x.ORF.StringEmpty) _
                .ToDictionary(Function(x) x.ORF)
        Else
            uniprot = UniProtXML _
                .LoadDictionary(proteins) _
                .Values _
                .ToDictionary(Function(x) DirectCast(x, INamedValue).Key)
        End If

        For Each term As EnrichmentTerm In [in].LoadCsv(Of EnrichmentTerm).Where(Function(t) t.Pvalue <= pvalue)
            csv += New RowObject From {
                "#term-ID=" & term.ID,
                "term=" & term.Term,
                "pvalue=" & term.Pvalue
            }

            If Not getORF OrElse term.ORF.IsNullOrEmpty Then
                term.ORF = term.Input.Split("|"c)
            End If

            csv += New RowObject From {"uniprot", "ORF", "EC", "geneNames", "fullName"}

            For Each ORF As String In term.ORF
                Dim protein As entry = uniprot.TryGetValue(ORF)
                Dim EC$ = protein?.ECNumberList.JoinBy("; ")
                Dim name$ = protein? _
                    .gene? _
                    .names? _
                    .SafeQuery _
                    .Select(Function(x) x.value) _
                    .JoinBy("; ")

                csv += New RowObject From {
                    ORF, protein.ORF, EC, name, protein.proteinFullName
                }
            Next

            Call csv.AppendLine()
        Next

        Return csv.Save(out).CLICode
    End Function

    ''' <summary>
    ''' 利用KOBAS的KEGG富集结果从KEGG服务器下载代谢物的显示图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/KEGG.Enrichment.PathwayMap")>
    <Description("Show the KEGG pathway map image by using KOBAS KEGG pathway enrichment result.")>
    <Usage("/KEGG.Enrichment.PathwayMap /in <kobas.csv> [/DEPs <deps.csv> /colors <default=red,blue,green> /map <id2uniprotID.txt> /uniprot <uniprot.XML> /pvalue <default=0.05> /out <DIR>]")>
    <Argument("/colors", AcceptTypes:={GetType(String())},
              Description:="A string vector that setups the DEPs' color profiles, if the argument ``/DEPs`` is presented. value format is ``up,down,present``")>
    <Argument("/DEPs", True, CLITypes.File, AcceptTypes:={GetType(DEP_iTraq)},
              Description:="Using for rendering color of the KEGG pathway map. The ``/colors`` argument only works when this argument is presented.")>
    <Argument("/map", True, CLITypes.File, Description:="Maps user custom ID to uniprot ID. A tsv file with format: ``<customID><TAB><uniprotID>``")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function KEGGEnrichmentPathwayMap(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}-KEGG_enrichment_pathwayMaps/"
        Dim pvalue# = args.GetValue("/pvalue", 0.05)
        Dim data As EnrichmentTerm() = [in].LoadCsv(Of EnrichmentTerm)
        Dim DEPs$ = args <= "/DEPs"

        If Not DEPs.FileExists(True) Then
            Call KEGGPathwayMap.KOBAS_visualize(
                data,
                EXPORT:=out,
                pvalue:=pvalue)
        Else
            Dim DEPgenes = EntityObject.LoadDataSet(DEPs) _
                .SplitID _
                .UserCustomMaps(args <= "/map")

            ' 假设这里的编号都是uniprot编号，还需要转换为KEGG基因编号
            Dim uniprot = UniProtXML.LoadDictionary(args <= "/uniprot")
            Dim mapID = uniprot _
                .Where(Function(gene)
                           Return gene.Value.xrefs.ContainsKey("KEGG")
                       End Function) _
                .ToDictionary(Function(gene) gene.Key,
                              Function(gene)
                                  Return gene.Value _
                                      .xrefs("KEGG") _
                                      .Select(Function(x) x.id) _
                                      .ToArray
                              End Function)
            Dim isDEP As Func(Of EntityObject, Boolean) =
                Function(gene)
                    Return True = gene("is.DEP").ParseBoolean
                End Function
            Dim colors = DEGProfiling.ColorsProfiling(DEPgenes, isDEP, "log2FC", mapID)

            Call data.KOBAS_DEPs(colors, EXPORT:=out, pvalue:=pvalue)
        End If

        Return 0
    End Function

    ''' <summary>
    ''' 4. 导出KOBAS结果
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/KOBAS.split")>
    <Usage("/KOBAS.split /in <kobas.out_run.txt> [/out <DIR>]")>
    <Description("Split the KOBAS run output result text file as seperated csv file.")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function KOBASSplit(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix)

        Call SMRUCC.genomics.Analysis.Microarray.KOBAS.SplitData([in], out)

        Return 0
    End Function

    ''' <summary>
    ''' Sample文件之中必须有一列ORF列，有一列为uniprot编号为主键
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/KOBAS.add.ORF")>
    <Usage("/KOBAS.add.ORF /in <table.csv> /sample <sample.csv> [/out <out.csv>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    <Argument("/in",
              AcceptTypes:={GetType(EnrichmentTerm)},
              Description:="The KOBAS enrichment result.")>
    <Argument("/sample",
              AcceptTypes:={GetType(EntityObject)},
              Description:="The uniprotID -> ORF annotation data. this table file should have a field named ""ORF"".")>
    Public Function KOBASaddORFsource(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim sample As String = args("/sample")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-ORF.csv")
        Dim table As List(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)
        Dim sampleData As Dictionary(Of String, String()) =
            EntityObject _
            .LoadDataSet(sample).GroupBy(Function(p) p.ID) _
            .ToDictionary(Function(u) u.Key,
                          Function(g) g.Select(Function(u) u("ORF")).Distinct.ToArray)

        For Each term As EnrichmentTerm In table
            Dim uniprotIDs = term.Input.Split("|"c)
            Dim ORFs As String() = sampleData _
                .Selects(uniprotIDs, True) _
                .IteratesALL _
                .ToArray

            term.ORF = ORFs
        Next

        Return table.SaveTo(out).CLICode
    End Function
End Module
