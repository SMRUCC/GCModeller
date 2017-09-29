#Region "Microsoft.VisualBasic::f9a73c91a9f6754ed46d0efe4d933c94, ..\CLI_tools\eggHTS\CLI\3. EnrichmentAnalysis.vb"

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
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.GO.PlantRegMap
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.DAVID
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.Web.Retrieve_IDmapping
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Partial Module CLI

    <ExportAPI("/KEGG.enrichment.DAVID")>
    <Usage("/KEGG.enrichment.DAVID /in <david.csv> [/tsv /custom <ko00001.keg> /size <default=1200,1000> /tick 1 /out <out.png>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function DAVID_KEGGplot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".DAVID_KEGG.plot.png")

        ' 处理DAVID数据
        Dim table As FunctionCluster() = If(
            args.GetBoolean("/tsv"),
            DAVID.Load([in]),
            [in].LoadCsv(Of FunctionCluster).ToArray)
        Dim KEGG = table.SelectKEGGPathway
        Dim size$ = args.GetValue("/size", "1200,1000")
        Dim KEGG_PATH As Dictionary(Of String, BriteHText) = Nothing

        With args <= "/custom"
            If .FileExists(True) Then
                KEGG_PATH = PathwayMapping.CustomPathwayTable(ko00001:= .ref)
            End If
        End With

        Return KEGG _
            .KEGGEnrichmentPlot(size:=size,
                                KEGG:=KEGG_PATH,
                                tick:=args.GetValue("/tick", 1.0R)) _
            .Save(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 因为富集分析的输出列表都是uniprotID，所以还需要uniprot注释数据转换为KEGG编号
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/KEGG.enrichment.DAVID.pathwaymap")>
    <Usage("/KEGG.enrichment.DAVID.pathwaymap /in <david.csv> /uniprot <uniprot.XML> [/tsv /DEPs <deps.csv> /colors <default=red,blue,green> /tag <default=log2FC> /pvalue <default=0.05> /out <out.DIR>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function DAVID_KEGGPathwayMap(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".DAVID_KEGG/")
        Dim uniprot$ = args <= "/uniprot"
        Dim uniprot2KEGG = UniProtXML.Load(uniprot) _
            .entries _
            .Where(Function(x) x.Xrefs.ContainsKey("KEGG")) _
            .Select(Function(protein)
                        Return protein.accessions.Select(Function(uniprotID)
                                                             Return (uniprotID, protein.Xrefs("KEGG").Select(Function(id) id.id).ToArray)
                                                         End Function)
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(x) x.Item1) _
            .ToDictionary(Function(id) id.Key,
                          Function(x)
                              Return x.Select(Function(o) o.Item2) _
                                  .IteratesALL _
                                  .Distinct _
                                  .ToArray
                          End Function)
        Dim pvalue# = args.GetValue("/pvalue", 0.05)

        ' 处理DAVID数据
        Dim table As FunctionCluster() = If(
            args.GetBoolean("/tsv"),
            DAVID.Load([in]),
            [in].LoadCsv(Of FunctionCluster).ToArray)
        Dim KEGG = table.SelectKEGGPathway(uniprot2KEGG)

        With args <= "/DEPs"
            If .FileLength > 0 Then
                Dim DEPgenes = EntityObject.LoadDataSet(.ref).ToArray
                Dim isDEP As Func(Of EntityObject, Boolean) =
                    Function(gene)
                        Return gene("is.DEP").TextEquals("TRUE")
                    End Function
                Dim readTag$ = args.GetValue("/tag", "log2FC")
                Dim colors = DEGProfiling.ColorsProfiling(DEPgenes, isDEP, readTag, uniprot2KEGG)

                Call KEGG.KOBAS_DEPs(colors, EXPORT:=out, pvalue:=pvalue)
            Else
                Call KEGG.KOBAS_visualize(EXPORT:=out, pvalue:=pvalue)
            End If
        End With

        Return 0
    End Function

    ''' <summary>
    ''' 绘制GO分析之中的亚细胞定位结果的饼图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/GO.cellular_location.Plot",
               Info:="Visualize of the subcellular location result from the GO enrichment analysis.",
               Usage:="/GO.cellular_location.Plot /in <KOBAS.GO.csv> [/GO <go.obo> /3D /colors <schemaName, default=Paired:c8> /out <out.png>]")>
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
                        .offset = New Point(0, -100)
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
    <ExportAPI("/Go.enrichment.plot",
               Usage:="/Go.enrichment.plot /in <enrichmentTerm.csv> [/bubble /r ""log(x,1.5)"" /Corrected /displays <default=10> /PlantRegMap /label.right /gray /pvalue <0.05> /size <2000,1600> /tick 1 /go <go.obo> /out <out.png>]")>
    <Description("Go enrichment plot base on the KOBAS enrichment analysis result.")>
    <Argument("/bubble", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Visuallize the GO enrichment analysis result using bubble plot, not the bar plot.")>
    <Argument("/displays", True, CLITypes.Integer,
              AcceptTypes:={GetType(Integer)},
              Description:="If the ``/bubble`` argument is not presented, then this will means the top number of the enriched term will plot on the barplot, else it is the term label display number in the bubble plot mode. 
              Set this argument value to -1 for display all terms.")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function GO_enrichmentPlot(args As CommandLine) As Integer
        Dim goDB As String = args.GetValue("/go", GCModeller.FileSystem.GO & "/go.obo")
        Dim terms = GO_OBO.Open(goDB).ToDictionary
        Dim [in] As String = args("/in")
        Dim PlantRegMap As Boolean = args.GetBoolean("/PlantRegMap")
        Dim pvalue As Double = args.GetValue("/pvalue", 0.05)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".GO_enrichment.pvalue={pvalue}.png")
        Dim size As String = args.GetValue("/size", "2000,1600")
        Dim plot As GraphicsData
        Dim bubbleStyle As Boolean = args.GetBoolean("/bubble")
        Dim tick# = args.GetValue("/tick", 1.0R)
        Dim gray As Boolean = args.GetBoolean("/gray")
        Dim labelRight As Boolean = args.GetBoolean("/label.right")
        Dim usingCorrected As Boolean = args.GetBoolean("/Corrected")

        If PlantRegMap Then
            Dim enrichments As IEnumerable(Of PlantRegMap_GoTermEnrichment) =
                [in].LoadTsv(Of PlantRegMap_GoTermEnrichment)
            plot = enrichments.PlantEnrichmentPlot(terms, pvalue, size, tick)
            enrichments.ToArray.SaveTo([in].TrimSuffix & ".csv")
        Else
            Dim enrichments As IEnumerable(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)
            Dim displays% = args.GetValue("/displays", 10)  ' The term/label display number

            If bubbleStyle Then
                Dim R$ = args.GetValue("/r", "log(x,1.5)")  ' 获取半径的计算公式              

                plot = enrichments.BubblePlot(GO_terms:=terms,
                                              pvalue:=pvalue,
                                              R:=R,
                                              size:=size,
                                              displays:=displays)
            Else
                plot = enrichments.EnrichmentPlot(
                    terms, pvalue, size,
                    tick,
                    gray, labelRight,
                    top:=displays)
            End If
        End If

        Return plot.Save(out).CLICode
    End Function

    <ExportAPI("/KEGG.enrichment.plot",
               Info:="Bar plots of the KEGG enrichment analysis result.",
               Usage:="/KEGG.enrichment.plot /in <enrichmentTerm.csv> [/gray /label.right /pvalue <0.05> /tick 1 /size <2000,1600> /out <out.png>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function KEGG_enrichment(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim enrichments As IEnumerable(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)
        Dim pvalue As Double = args.GetValue("/pvalue", 0.05)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".GO_enrichment.pvalue={pvalue}.png")
        Dim size As String = args.GetValue("/size", "2000,1600")
        Dim gray As Boolean = args.GetBoolean("/gray")
        Dim labelRight As Boolean = args.GetBoolean("/label.right")
        Dim tick As Double = args.GetValue("/tick", 1.0)
        Dim plot As GraphicsData = enrichments.KEGGEnrichmentPlot(
            size, pvalue,
            gray:=gray,
            labelRightAlignment:=labelRight,
            tick:=tick)

        Return plot.Save(out).CLICode
    End Function

    ''' <summary>
    ''' ``/ORF``参数是表示使用uniprot注释数据库之中的ORF值来作为查找的键名
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Enrichments.ORF.info",
               Info:="Retrive KEGG/GO info for the genes in the enrichment result.",
               Usage:="/Enrichments.ORF.info /in <enrichment.csv> /proteins <uniprot-genome.XML> [/nocut /ORF /out <out.csv>]")>
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
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "-KEGG_enrichment_pathwayMaps/")
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
                .Where(Function(gene) gene.Value.Xrefs.ContainsKey("KEGG")) _
                .ToDictionary(Function(gene) gene.Key,
                              Function(gene)
                                  Return gene.Value _
                                      .Xrefs("KEGG") _
                                      .Select(Function(x) x.id) _
                                      .ToArray
                              End Function)
            Dim isDEP As Func(Of EntityObject, Boolean) =
                Function(gene)
                    Return gene("is.DEP").TextEquals("TRUE")
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
    <ExportAPI("/KOBAS.split", Usage:="/KOBAS.split /in <kobas.out_run.txt> [/out <DIR>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function KOBASSplit(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix)

        Call KOBAS.SplitData([in], out)

        Return 0
    End Function

    ''' <summary>
    ''' Sample文件之中必须有一列ORF列，有一列为uniprot编号为主键
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/KOBAS.add.ORF", Usage:="/KOBAS.add.ORF /in <table.csv> /sample <sample.csv> [/out <out.csv>]")>
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

    <ExportAPI("/Term2genes",
               Usage:="/Term2genes /in <uniprot.XML> [/term <GO> /id <ORF> /out <out.tsv>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function Term2Genes(args As CommandLine) As Integer
        Dim [in] = args <= "/in"
        Dim term As String = args.GetValue("/term", "GO")
        Dim idType$ = args.GetValue("/id", "ORF")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $"-type={term},{idType}.term2genes.tsv")
        Dim uniprot As UniProtXML = UniProtXML.Load([in])
        Dim tsv As IDMap() = uniprot.Term2Gene(type:=term, idType:=GetIDs.ParseType(idType))
        Return tsv.SaveTSV(out).CLICode
    End Function

    ''' <summary>
    ''' 生成背景基因列表
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/enricher.background")>
    <Usage("/enricher.background /in <uniprot.XML> [/mapping <maps.tsv> /out <term2gene.txt.DIR>]")>
    <Description("Create enrichment analysis background based on the uniprot xml database.")>
    <Argument("/mapping", True, CLITypes.File,
              Description:="The id mapping file, each row in format like ``id<TAB>uniprotID``")>
    <Argument("/in", True, CLITypes.File,
              Description:="The uniprotKB XML database which can be download from http://uniprot.org")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function Backgrounds(args As CommandLine) As Integer
        Dim [in] = args <= "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".t2g_backgrounds/")
        Dim go As New List(Of (accessions As String(), GO As String()))
        Dim KEGG = go.AsList
        Dim maps = MappingReader(args <= "/mapping") _
            .Select(Function(id) id.Value.Select(Function(uniprotID) (uniprotID, id.Key))) _
            .IteratesALL _
            .GroupBy(Function(x) x.Item1) _
            .ToDictionary(Function(x) x.Key,
                          Function(g)
                              Return g _
                                  .Select(Function(x) x.Item2) _
                                  .Distinct _
                                  .ToArray
                          End Function)

        For Each protein As entry In in$.LoadXmlDataSet(Of entry)(xmlns:="http://uniprot.org/uniprot")
            Dim go_ref = protein.Xrefs.TryGetValue("GO")
            Dim ID$()

            If maps.IsNullOrEmpty Then
                ID = protein.accessions
            Else
                ID = protein.accessions _
                    .Where(Function(uniprotID) maps.ContainsKey(uniprotID)) _
                    .Select(Function(uniprotID) maps(uniprotID)) _
                    .IteratesALL _
                    .Distinct _
                    .ToArray
            End If

            If ID.IsNullOrEmpty Then
                Continue For
            End If

            If Not go_ref.IsNullOrEmpty Then
                go += (ID, go_ref.Select(Function(x) x.id).ToArray)
            End If

            Dim KO_ref = protein.Xrefs.TryGetValue("KO")

            If Not KO_ref.IsNullOrEmpty Then
                KEGG += (ID, KO_ref.Select(Function(x) x.id).ToArray)
            End If
        Next

        Dim createBackground =
            Function(data As (accessions As String(), GO As String())()) As String()
                Return data _
                    .Select(Function(protein)
                                Return Combination _
                                    .CreateCombos(protein.accessions, protein.GO) _
                                    .Select(Function(x)
                                                ' term gene
                                                Return {x.Item2, x.Item1}.JoinBy(ASCII.TAB)
                                            End Function)
                            End Function) _
                    .IteratesALL _
                    .Distinct _
                    .ToArray
            End Function

        Call createBackground(go).SaveTo(out & "/GO.txt")
        Call createBackground(KEGG).SaveTo(out & "/KO.txt")

        Return 0
    End Function

    <ExportAPI("/enrichment.go",
               Usage:="/enrichment.go /deg <deg.list> /backgrounds <genome_genes.list> /t2g <term2gene.csv> [/go <go_brief.csv> /out <enricher.result.csv>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function GoEnrichment(args As CommandLine) As Integer

    End Function

    <ExportAPI("/Enrichment.Term.Filter",
               Info:="Filter the specific term result from the analysis output by using pattern keyword",
               Usage:="/Enrichment.Term.Filter /in <enrichment.csv> /filter <key-string> [/out <out.csv>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function EnrichmentTermFilter(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim filter$ = args <= "/filter"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & filter.NormalizePathString & ".csv")
        Dim terms = [in].LoadCsv(Of EnrichmentTerm)
        Dim r As New Regex(filter, RegexICSng)
        Dim result = terms.Where(Function(t) r.Match(t.Term).Success).ToArray
        Return result.SaveTo(out).CLICode
    End Function
End Module

