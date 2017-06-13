Imports System.Drawing
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
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
Imports Microsoft.VisualBasic.Mathematical.Quantile
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.GO.PlantRegMap
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Partial Module CLI

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
                .FromData(colors) _
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
            Call profiles.FromData(colors).Plot.Save(out)
        End If

        Return 0
    End Function

    ''' <summary>
    ''' go enrichment 绘图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Go.enrichment.plot",
               Usage:="/Go.enrichment.plot /in <enrichmentTerm.csv> [/bubble /r ""log(x,1.5)"" /Corrected /displays 10 /PlantRegMap /label.right /gray /pvalue <0.05> /size <2000,1600> /tick 1 /go <go.obo> /out <out.png>]")>
    <Argument("/bubble", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Visuallize the GO enrichment analysis result using bubble plot, not the bar plot.")>
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
            plot = enrichments.PlantEnrichmentPlot(terms, pvalue, size.SizeParser, tick)
            enrichments.ToArray.SaveTo([in].TrimSuffix & ".csv")
        Else
            Dim enrichments As IEnumerable(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)

            If bubbleStyle Then
                Dim R$ = args.GetValue("/r", "log(x,1.5)")
                Dim displays% = args.GetValue("/displays", 10)

                plot = enrichments.BubblePlot(GO_terms:=terms,
                                              pvalue:=pvalue,
                                              R:=R,
                                              size:=size,
                                              displays:=displays)
            Else
                plot = enrichments.EnrichmentPlot(
                    terms, pvalue, size.SizeParser,
                    tick,
                    gray, labelRight)
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
            size.SizeParser, pvalue,
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
            uniprot = UniprotXML _
                .Load(proteins) _
                .entries _
                .Where(Function(x) Not x.ORF.StringEmpty) _
                .ToDictionary(Function(x) x.ORF)
        Else
            uniprot = UniprotXML _
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
    <ExportAPI("/KEGG.Enrichment.PathwayMap",
               Info:="Show the KEGG pathway map image by using KOBAS KEGG pathway enrichment result.",
               Usage:="/KEGG.Enrichment.PathwayMap /in <kobas.csv> [/out <DIR>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function KEGGEnrichmentPathwayMap(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "-KEGG_enrichment_pathwayMaps/")
        Dim data As EnrichmentTerm() = [in].LoadCsv(Of EnrichmentTerm)
        For Each term As EnrichmentTerm In data
            Dim path$ = out & "/" & term.ID & "-" & term.Term.NormalizePathString & $"-pvalue={term.Pvalue}" & ".png"
            Call PathwayMapping.ShowEnrichmentPathway(term.link, save:=path)
            Call Thread.Sleep(2000)
        Next
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
    Public Function Term2Genes(args As CommandLine) As Integer
        Dim [in] = args <= "/in"
        Dim term As String = args.GetValue("/term", "GO")
        Dim idType$ = args.GetValue("/id", "ORF")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $"-type={term},{idType}.term2genes.tsv")
        Dim uniprot As UniprotXML = UniprotXML.Load([in])
        Dim tsv As IDMap() = uniprot.Term2Gene(type:=term, idType:=GetIDs.ParseType(idType))
        Return tsv.SaveTSV(out).CLICode
    End Function

    <ExportAPI("/enricher.background",
               Usage:="/enricher.background /in <genbank.gb> [/out <universe.txt>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function Backgrounds(args As CommandLine) As Integer
        Dim [in] = args <= "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".backgrounds.txt")
        Dim gb As GBFF.File = GBFF.File.Load([in])
        Dim PTT As PTT = gb.GbffToORF_PTT
        Dim genes$() = PTT.GeneIDList.ToArray

        Return genes.SaveTo(out, Encodings.ASCII.CodePage).CLICode
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
