#Region "Microsoft.VisualBasic::0bb16da2e87920cac85fa46d6242ca7f, CLI_tools\eggHTS\CLI\Enrichment\3. EnrichmentAnalysis.vb"

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
    '     Function: Backgrounds, Converts, EnrichmentTermFilter, KEGGEnrichmentPathwayMapLocal, KEGGPathwayEnrichmentProfile
    '               Term2Genes
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.Proteomics.Mappings
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.DAVID
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.Web.Retrieve_IDmapping
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Partial Module CLI

    <ExportAPI("/Term2genes")>
    <Usage("/Term2genes /in <uniprot.XML> [/term <GO> /id <ORF> /out <out.tsv>]")>
    <Group(CLIGroups.ClusterProfiler)>
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
    <ArgumentAttribute("/mapping", True, CLITypes.File,
              Description:="The id mapping file, each row in format like ``id<TAB>uniprotID``")>
    <ArgumentAttribute("/in", True, CLITypes.File,
              Description:="The uniprotKB XML database which can be download from http://uniprot.org")>
    <Group(CLIGroups.ClusterProfiler)>
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
            Dim go_ref = protein.xrefs.TryGetValue("GO")
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

            Dim KO_ref = protein.xrefs.TryGetValue("KO")

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

    <ExportAPI("/KEGG.Enrichment.PathwayMap.Render")>
    <Group(CLIGroups.Enrichment_CLI)>
    <Usage("/KEGG.Enrichment.PathwayMap.Render /in <enrichment.csv> [/repo <maps.directory> /DEPs <deps.csv> /colors <default=red,blue,green> /map <id2uniprotID.txt> /uniprot <uniprot.XML> /pvalue <default=0.05> /out <DIR>]")>
    <Description("KEGG pathway map enrichment analysis visual rendering locally. This function required a local kegg pathway repository.")>
    <ArgumentAttribute("/repo", True, CLITypes.File, PipelineTypes.undefined, AcceptTypes:={GetType(Map)},
              Description:="If this argument is omitted, then the default kegg pathway map repository will be used. But the default kegg pathway map repository only works for the KO numbers.")>
    Public Function KEGGEnrichmentPathwayMapLocal(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}-KEGG_enrichment_pathwayMaps/"
        Dim pvalue# = args.GetValue("/pvalue", 0.05)
        Dim data As EnrichmentTerm() = [in].LoadCsv(Of EnrichmentTerm)
        Dim DEPs$ = args <= "/DEPs"
        Dim repo$ = args("/repo") Or (GCModeller.FileSystem.FileSystem.RepositoryRoot & "/KEGG/pathwayMap/")
        Dim render As LocalRender = LocalRender.FromRepository(repo, True)

        If Not DEPs.FileExists(True) Then
            ' 不存在DEP的数据的时候，默认将所有的term都按照url的参数进行染色
            Call KEGGPathwayMap.LocalRendering(
                render,
                data,
                export:=out,
                pvalue:=pvalue
            )
        Else
            Dim DEPgenes = EntityObject.LoadDataSet(DEPs) _
                .SplitID _
                .UserCustomMaps(args <= "/map")

            ' 假设这里的编号都是uniprot编号，还需要转换为KEGG基因编号
            Dim uniprot = UniProtXML.LoadDictionary(args <= "/uniprot")
            Dim mapID = uniprot _
                .Where(Function(gene) gene.Value.xrefs.ContainsKey("KEGG")) _
                .ToDictionary(Function(gene) gene.Key,
                              Function(gene)
                                  Return gene.Value _
                                      .xrefs("KEGG") _
                                      .Select(Function(x) x.id) _
                                      .ToArray
                              End Function)
            Dim isDEP As Func(Of EntityObject, Boolean) =
                Function(gene)
                    If gene.Properties.ContainsKey("is.DEP") Then
                        Return True = gene("is.DEP").ParseBoolean
                    Else
                        Return True
                    End If
                End Function
            Dim colors = DEGProfiling.ColorsProfiling(DEPgenes, isDEP, "log2FC", mapID)
            Dim translateKO As New Dictionary(Of String, String)
            Dim KO = uniprot.Values _
                .Where(Function(gene)
                           Return gene.xrefs.ContainsKey("KO")
                       End Function) _
                .ToArray

            ' 如果是使用默认的repository的话，还需要通过uniprot注释转换为KO编号
            ' 因为默认的repository是参考的pathway图，基因都是使用KO来表示的
            For Each gene As entry In KO
                Dim KO_id As String = gene.xrefs("KO").First.id

                gene.accessions _
                    .DoEach(Sub(id)
                                translateKO(id) = KO_id
                            End Sub)

                If gene.xrefs.ContainsKey("KEGG") Then
                    gene.xrefs("KEGG") _
                        .DoEach(Sub(id)
                                    translateKO(id.id) = KO_id
                                End Sub)
                End If
            Next

            Call KEGGPathwayMap.LocalRendering(
                render,
                data,
                export:=out,
                pvalue:=pvalue,
                color:=Function(id)
                           Return colors.TryGetValue(id, default:="lightgreen")
                       End Function,
                translateKO:=Function(id)
                                 Return translateKO.TryGetValue(id, [default]:=id)
                             End Function
            )
        End If

        Return 0
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

    <ExportAPI("/Converts")>
    <Usage("/Converts /in <GSEA.terms.csv> [/DAVID2KOBAS /out <result.terms.csv>]")>
    <Description("Converts the GCModeller enrichment analysis output as the KOBAS enrichment analysis result output table.")>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(EnrichmentResult)},
              Extensions:="*.csv",
              Description:="The GCModeller enrichment analysis output table.")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function Converts(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim DAVID2KOBAS As Boolean = args("/DAVID2KOBAS")
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}_converts.csv"
        Dim result As EnrichmentTerm()

        If DAVID2KOBAS Then
            result = [in] _
                .LoadCsv(Of FunctionCluster) _
                .Converts _
                .ToArray
        Else
            result = [in] _
                .LoadCsv(Of EnrichmentResult) _
                .Converts() _
                .ToArray
        End If

        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/KEGG.enrichment.profile")>
    <Usage("/KEGG.enrichment.profile /in <enrichment.csv> [/out <profile.csv>]")>
    Public Function KEGGPathwayEnrichmentProfile(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.KEGG_enrichmentProfiles.csv"

        Return [in] _
            .LoadCsv(Of EnrichmentTerm) _
            .DoCall(AddressOf KEGGOrthology.KEGGPathwayEnrichmentProfile) _
            .ToArray _
            .SaveDataSet(path:=out) _
            .CLICode
    End Function
End Module
