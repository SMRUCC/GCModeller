#Region "Microsoft.VisualBasic::6f1ec21a1ab6e9e445549a03fe88ddb1, GSEA\Profiler\CLI\CLI.vb"

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
    '     Function: CreateGOClusters, CreateKOCluster, CreateKOClusterFromBBH, EnrichmentTest, GOCluster_blastp
    '               GSEA_GO, IDconverts
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KnowledgeBase
Imports SMRUCC.genomics.Analysis.Microarray.GSEA
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

<CLI>
Public Module CLI

    <ExportAPI("/KO.clusters")>
    <Usage("/KO.clusters /background <KO.txt/uniprot.XML> /maps <kegg_maps.XML/directory> [/generic /out <clusters.XML>]")>
    <Description("Create KEGG pathway map background for a given genome data or a reference KO list.")>
    <ArgumentAttribute("/background", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(UniProtXML)},
              Extensions:="*.xml",
              Description:="the KO annotation background data, it can be a ``UniProt`` database that contains the uniprot_id to KO_id mapping. or just
              a plain text file that contains a list of KO terms as background, each line in this text file should be one KO term word.")>
    <ArgumentAttribute("/maps", False, CLITypes.File,
              AcceptTypes:={GetType(Map)},
              Description:="This argument should be a directory path which this folder contains multiple 
              KEGG reference pathway map xml files. A xml file path of the kegg pathway map database 
              is also accepted!")>
    Public Function CreateKOCluster(args As CommandLine) As Integer
        Dim background$ = args <= "/background"
        Dim maps$ = args <= "/maps"
        Dim isGeneric As Boolean = args("/generic")
        Dim out$ = args("/out") Or $"{background.TrimSuffix}_KO{If(isGeneric, ".generic", "")}.XML"
        Dim kegg As IEnumerable(Of Map) = MapRepository.GetMapsAuto(maps).ToArray
        Dim model As Background

        If background.ExtensionSuffix("txt") Then
            ' create the KO generic background
            Dim KO_terms As String() = background _
                .ReadAllLines _
                .Where(Function(line) line.IsPattern("K\d+")) _
                .Distinct _
                .ToArray

            model = KO_terms.CreateKOGeneric(kegg, nsize:=background.ReadAllLines.Length)
        ElseIf isGeneric Then
            Dim proteins = UniProtXML.EnumerateEntries(background).ToArray
            Dim KO_terms As String() = proteins _
                .Select(Function(a) a.xrefs.TryGetValue("KO")) _
                .IteratesALL _
                .Select(Function(a) a.id) _
                .Distinct _
                .ToArray

            model = KO_terms.CreateKOGeneric(kegg, nsize:=proteins.Length)
        Else
            model = GSEA.ImportsUniProt(
                db:=UniProtXML.EnumerateEntries(background),
                getTerm:=GSEA.UniProtGetKOTerms,
                define:=GSEA.KEGGClusters(kegg)
            )
        End If

        Return model.GetXml.SaveTo(out).CLICode
    End Function

    <ExportAPI("/KO.clusters.By_bbh")>
    <Usage("/KO.clusters.By_bbh /in <KO.bbh.csv> /maps <kegg_maps.XML/directory> [/size <backgroundSize, default=-1> /genome <genomeName/taxonomy> /out <clusters.XML>]")>
    Public Function CreateKOClusterFromBBH(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim maps$ = args <= "/maps"
        Dim size% = args("/size") Or -1
        Dim genomeName$ = args("/genome") Or "Unknown"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.{genomeName.NormalizePathString(True)},size={size}.KOclusters.Xml"
        Dim kegg As IEnumerable(Of Map) = MapRepository.GetMapsAuto(maps)
        Dim define = GSEA.KEGGClusters(kegg)
        Dim model As Background = BBHLibrary.CreateBackground(
            annotations:=[in].LoadCsv(Of BiDirectionalBesthit),
            define:=define,
            backgroundSize:=size,
            outputAll:=True,
            genomeName:=genomeName
        )

        Return model.GetXml _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/GO.clusters")>
    <Usage("/GO.clusters /uniprot <uniprot.XML> /go <go.obo> [/generic /out <clusters.XML>]")>
    <Description("Create GO enrichment background model from uniprot database.")>
    <ArgumentAttribute("/uniprot", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.Xml",
              Description:="The uniprot database.")>
    Public Function CreateGOClusters(args As CommandLine) As Integer
        Dim uniprot$ = args <= "/uniprot"
        Dim obo$ = args <= "/go"
        Dim isGeneric As Boolean = args("/generic")
        Dim out$ = args("/out") Or $"{uniprot.TrimSuffix}_GO{If(isGeneric, ".generic", "")}.XML"
        Dim go As GetClusterTerms = GSEA.Imports.GOClusters(GO_OBO.Open(obo))
        Dim entries As entry() = UniProtXML.EnumerateEntries(uniprot).ToArray
        Dim model As Background

        If isGeneric Then
            model = entries _
                .Select(Function(a) a.xrefs.TryGetValue("GO")) _
                .IteratesALL _
                .Select(Function(a) a.id) _
                .Distinct _
                .CreateGOGeneric(go, nsize:=entries.Length)
        Else
            model = GSEA.Imports.ImportsUniProt(
                entries,
                getTerm:=GSEA.UniProtGetGOTerms,
                define:=go
            )
        End If

        Return model.GetXml.SaveTo(out).CLICode
    End Function

    <ExportAPI("/GO.clusters.blastp")>
    <Description("Create GO clusters from the blastp besthit dataset.")>
    <Usage("/GO.clusters.blastp /in <besthit.csv> /go <go.obo> [/size <default=-1> /out <clusters.XML>]")>
    Public Function GOCluster_blastp(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim obo$ = args <= "/go"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}_GO.XML"
        Dim queryVsUniprot = [in] _
            .LoadCsv(Of BestHit)(skipWhile:=Pipeline.SkipHitNotFound) _
            .GroupBy(Function(q) q.QueryName) _
            .ToArray
        Dim bgSize As Integer = args("/size") Or queryVsUniprot.Length
        Dim go = GSEA.Imports.GOClusters(GO_OBO.Open(obo))
        Dim model As Background = GSEA.Imports _
            .CreateBackground(
                db:=queryVsUniprot,
                createGene:=Function(g, terms)
                                Return New BackgroundGene With {
                                    .accessionID = g.Key,
                                    .[alias] = {g.Key},
                                    .locus_tag = New NamedValue With {
                                        .name = g.Key,
                                        .text = g.Key
                                    },
                                    .name = g.Key,
                                    .term_id = terms
                                }
                            End Function,
                getTerms:=Function(g)
                              Return g _
                                 .Select(Function(hit)
                                             Return hit.HitName.Split("|"c).First.Split(","c)
                                         End Function) _
                                 .IteratesALL _
                                 .Distinct _
                                 .ToArray
                          End Function,
                define:=go
            )

        model.size = bgSize

        Return model.GetXml.SaveTo(out).CLICode
    End Function

    <ExportAPI("/id.converts")>
    <Usage("/id.converts /uniprot <uniprot.XML> /geneSet <geneSet.txt> [/out <converts.txt>]")>
    Public Function IDconverts(args As CommandLine) As Integer
        Dim uniprot$ = args <= "/uniprot"
        Dim list$ = args("/geneset")
        Dim out$ = args("/out") Or $"{list.TrimSuffix}_uniprot.txt"
        Dim geneSet$() = list _
            .IterateAllLines _
            .Select(Function(l)
                        Return Strings.Trim(l).Split.First
                    End Function) _
            .ToArray

        ' 先推测一下是否是uniprot编号？
        ' 如果不是，则会需要进行编号转换操作
        Dim convertor As New IDConvertor(UniProtXML.EnumerateEntries(uniprot))
        Dim type As IDTypes = convertor.GetType(geneSet)

        If type = IDTypes.NA Then
            Throw New NotSupportedException(geneSet.GetJson)
        End If

        Dim converts As NamedVector(Of String)() = convertor _
            .Converts(geneSet, type) _
            .ToArray
        Dim convertGeneSet$() = converts _
            .Select(Function(c) c.vector) _
            .IteratesALL _
            .Distinct _
            .ToArray

        Call converts _
            .Select(Function(c)
                        Return $"{c.name}{ASCII.TAB}{c.vector.JoinBy(",")}"
                    End Function) _
            .FlushAllLines(out.TrimSuffix & ".converts.txt")
        Call convertGeneSet.FlushAllLines(out)

        Return 0
    End Function

    ''' <summary>
    ''' GO富集比较特殊一些，所以单独一个工具函数来进行计算分析
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/GSEA.GO")>
    <Usage("/GSEA.GO /background <clusters.XML> /geneSet <geneSet.txt> /go <go.obo> [/hide.progress /locus_tag /cluster_id <null, debug_used> /format <default=GCModeller> /out <out.csv>]")>
    Public Function GSEA_GO(args As CommandLine) As Integer
        Dim backgroundXML$ = args("/background")
        Dim background = backgroundXML.LoadXml(Of Background)
        Dim debugIdlist As Index(Of String) = (args("/cluster_id") Or "").Split(","c)
        Dim list$ = args("/geneset")
        Dim geneSet$() = list _
            .IterateAllLines _
            .Select(Function(l)
                        Return Strings.Trim(l).Split.First
                    End Function) _
            .ToArray
        Dim out$ = args("/out") Or $"{list.TrimSuffix}_{backgroundXML.BaseName}_enrichment.csv"
        Dim isLocusTag As Boolean = args("/locus_tag")
        Dim format$ = args("/format") Or "GCModeller"
        Dim go As GO_OBO = GO_OBO.LoadDocument(args <= "/go")

        ' 在这里还需要将列表约束在背景模型的范围内
        ' 这一步操作在LC-MS的代谢物富集分析中尤其重要
        geneSet = background.clusters _
            .Select(Function(c) c.Intersect(geneSet, isLocusTag)) _
            .IteratesALL _
            .Distinct _
            .ToArray

        ' for debug test
        If debugIdlist Then
            background = background _
                .SubsetOf(Function(cluster)
                              Return cluster.ID Like debugIdlist
                          End Function)
        End If

        Dim result As EnrichmentResult() = background _
            .Enrichment(
                list:=geneSet,
                go:=go,
                isLocustag:=isLocusTag,
                showProgress:=Not args.IsTrue("/hide.progress")
            ) _
            .FDRCorrection _
            .OrderBy(Function(term) term.pvalue) _
            .ToArray

        If format.TextEquals("KOBAS") Then
            ' convert to KOBAS table
            Return result.Converts.SaveTo(out).CLICode
        Else
            Return result.SaveTo(out).CLICode
        End If
    End Function

    <ExportAPI("/GSEA")>
    <Usage("/GSEA /background <clusters.XML> /geneSet <geneSet.txt> [/hide.progress /locus_tag /cluster_id <null, debug_used> /format <default=GCModeller> /out <out.csv>]")>
    <Description("Do gene set enrichment analysis.")>
    <ArgumentAttribute("/background", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.Xml",
              Description:="A genome background data file which is created by ``/KO.clusters`` or ``/GO.clusters`` tools.")>
    <ArgumentAttribute("/cluster_id", True, CLITypes.String,
              AcceptTypes:={GetType(String())},
              Description:="A list of specific cluster id that used for program debug use only.")>
    <ArgumentAttribute("/format", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="apply this argument to specify the output table format, by default is in GCModeller table format, or you can assign the ``KOBAS`` format value at this parameter.")>
    <ArgumentAttribute("/out", True, CLITypes.File,
              AcceptTypes:={GetType(EnrichmentResult), GetType(EnrichmentTerm)},
              Extensions:="*.csv",
              Description:="The file path of the result output, the output result table format is affects by the ``/format`` argument.")>
    <ArgumentAttribute("/geneSet", False, CLITypes.File,
              AcceptTypes:={GetType(String())},
              Extensions:="*.txt",
              Description:="A text file that contains the gene id list that will be apply the GSEA analysis.")>
    <ArgumentAttribute("/hide.progress", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="A logical flag argument that controls the console screen display the progress bar or not.")>
    Public Function EnrichmentTest(args As CommandLine) As Integer
        Dim backgroundXML$ = args("/background")
        Dim background = backgroundXML.LoadXml(Of Background)
        Dim debugIdlist As Index(Of String) = (args("/cluster_id") Or "").Split(","c)
        Dim list$ = args("/geneset")
        Dim geneSet$() = list _
            .IterateAllLines _
            .Select(Function(l)
                        Return Strings.Trim(l).Split.First
                    End Function) _
            .ToArray
        Dim out$ = args("/out") Or $"{list.TrimSuffix}_{backgroundXML.BaseName}_enrichment.csv"
        Dim isLocusTag As Boolean = args("/locus_tag")
        Dim format$ = args("/format") Or "GCModeller"

        ' 在这里还需要将列表约束在背景模型的范围内
        ' 这一步操作在LC-MS的代谢物富集分析中尤其重要
        geneSet = background.clusters _
            .Select(Function(c) c.Intersect(geneSet, isLocusTag)) _
            .IteratesALL _
            .Distinct _
            .ToArray

        ' for debug test
        If debugIdlist Then
            background = background _
                .SubsetOf(Function(cluster)
                              Return cluster.ID Like debugIdlist
                          End Function)
        End If

        Dim result As EnrichmentResult() = background _
            .Enrichment(
                list:=geneSet,
                isLocustag:=isLocusTag,
                showProgress:=Not args.IsTrue("/hide.progress")
            ) _
            .FDRCorrection _
            .OrderBy(Function(term) term.pvalue) _
            .ToArray

        If format.TextEquals("KOBAS") Then
            ' convert to KOBAS table
            Return result.Converts.SaveTo(out).CLICode
        Else
            Return result.SaveTo(out).CLICode
        End If
    End Function
End Module
