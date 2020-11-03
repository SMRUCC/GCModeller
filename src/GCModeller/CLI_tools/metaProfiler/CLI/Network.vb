#Region "Microsoft.VisualBasic::66f80a7252773b93101925a899abbe53, CLI_tools\metaProfiler\CLI\Network.vb"

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
    '     Function: BuildUniProtReference, Membrane_transportNetwork, MetabolicComplementationNetwork, MetabolicEndPointProfilesBackground, PathwayProfiles
    '               (+2 Overloads) RunProfile, ScreenModels
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.foundation.BIOM.v10.components
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.Model.Network.Microbiome
Imports SMRUCC.genomics.Model.Network.Microbiome.PathwayProfile

Partial Module CLI

    ''' <summary>
    ''' 这个命令行函数包含有创建profile矩阵以及计算显著性的功能
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/microbiome.pathway.profile")>
    <Usage("/microbiome.pathway.profile /in <gastout.csv> /ref <UniProt.ref.index.json> /maps <kegg.maps.ref.XML> [/sampleName <default=NULL> /just.profiles /rank <default=family> /p.value <default=0.05> /out <out.directory>]")>
    <Description("Generates the pathway network profile for the microbiome OTU result based on the KEGG and UniProt reference.")>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(gast.gastOUT), GetType(OTUTable)},
              Extensions:="*.csv",
              Description:="The OTU sample counting result.")>
    <ArgumentAttribute("/ref", False, CLITypes.File,
              Extensions:="*.json",
              AcceptTypes:={GetType(TaxonomyRepository)},
              Description:="The bacteria genome annotation data repository index file.")>
    <ArgumentAttribute("/just.profiles", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="This option will makes this cli command only creates a pathway profile matrix. For enrichment command debug used only.")>
    <ArgumentAttribute("/rank", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="The enrichment profile will be statistics at this level")>
    <ArgumentAttribute("/sampleName", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="This argument is only works when the input table file is a OTU result data table.")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    <Output(GetType(NetworkTables), "*.csv")>
    Public Function PathwayProfiles(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim ref$ = args <= "/ref"
        Dim out$
        Dim gast As gast.gastOUT()

        If MappingsHelper.Typeof([in], GetType(gast.gastOUT), GetType(OTUTable)) Is GetType(OTUTable) Then
            Dim sampleName$ = args("/sampleName")

            If sampleName.StringEmpty Then
                Throw New Exception("No sample name provides for your OTU table!")
            End If

            gast = OTUTable.LoadSample([in]).CreateGastCountTabel(sampleName).ToArray
            out = args("/out") Or $"{[in].TrimSuffix}_sample({sampleName}).pathway.profiles/"
        Else
            gast = [in].LoadCsv(Of gast.gastOUT)
            out = args("/out") Or $"{[in].TrimSuffix}.pathway.profiles/"
        End If

        Dim UniProt As TaxonomyRepository = Nothing
        Dim maps As MapRepository = args("/maps").LoadXml(Of MapRepository)
        Dim pvalue# = args.GetValue("/p.value", 0.05)
        Dim rank As TaxonomyRanks = args.GetValue("/rank", TaxonomyRanks.Family, AddressOf ParseRank)

        Call $"Read {gast.Length} OTU data...".__DEBUG_ECHO

        ' 合并OTU
        gast = gast _
            .GroupBy(Function(tax) tax.taxonomy) _
            .Select(Function(g)
                        Dim first = g.First
                        first.counts = g.Sum(Function(s) s.counts)
                        Return first
                    End Function) _
            .ToArray

        Call $"Lefts {gast.Length} OTU data after union operation".__INFO_ECHO

        Call "Load UniProt reference genome model....".__INFO_ECHO
        Call VBDebugger.BENCHMARK(Sub() UniProt = TaxonomyRepository.LoadRepository(ref))

        If args.IsTrue("/just.profiles") Then
            Return gast _
                .CreateProfile(UniProt, ref:=maps) _
                .Values _
                .IteratesALL _
                .SaveTo(out & "/taxonomy.maps.csv") _
                .CLICode
        Else
            Return gast _
                .PathwayProfiles(UniProt, ref:=maps) _
                .RunProfile(maps, out, pvalue)
        End If
    End Function

    ''' <summary>
    ''' 可以使用这个命令来处理<see cref="PathwayProfiles"/>命令所生成的profile矩阵的富集计算分析
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/microbiome.pathway.run.profile")>
    <Usage("/microbiome.pathway.run.profile /in <profile.csv> /maps <kegg.maps.ref.Xml> [/rank <default=family> /colors <default=Set1:c6> /tick 1 /size <2000,1600> /p.value <default=0.05> /out <out.directory>]")>
    <Description("Build pathway interaction network based on the microbiome profile result.")>
    <ArgumentAttribute("/p.value", True, CLITypes.Double,
              Description:="The pvalue cutoff of the profile mapID, selects as the network node if the mapID its pvalue is smaller than this cutoff value. 
              By default is 0.05. If no cutoff, please set this value to 1.")>
    <ArgumentAttribute("/maps", False, CLITypes.File,
              Extensions:="*.Xml",
              Description:="The kegg reference map repository database file.")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    Public Function RunProfile(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim maps As MapRepository = (args <= "/maps").LoadXml(Of MapRepository)
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.pathway.profiles/"
        Dim pvalue# = args("/p.value") Or 0.05
        Dim colors$ = args("/colors") Or "Set1:c6"
        Dim tick# = args("/tick") Or 1.0
        Dim size$ = args("/size") Or "2000,1600"
        Dim rank As TaxonomyRanks = Metagenomics.ParseRank(args("/rank") Or "family")

        If rank = TaxonomyRanks.NA Then
            Throw New InvalidExpressionException($"Invalid input rank level string: {args("/rank")}")
        End If

        Dim profiles = [in].LoadCsv(Of Profile) _
            .Where(Function(tax) tax.Taxonomy.lowestLevel > TaxonomyRanks.Phylum) _
            .GroupBy(Function(tax) tax.Taxonomy.ToString(rank)) _
            .Select(Function(tax)
                        Return tax.ProfileEnrichment _
                            .Select(Function(pathway As KeyValuePair(Of String, (profile#, pvalue#)))
                                        Return New EnrichmentProfiles With {
                                            .pathway = pathway.Key,
                                            .RankGroup = tax.Key,
                                            .profile = pathway.Value.profile,
                                            .pvalue = pathway.Value.pvalue
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray

        Return profiles.RunProfile(maps, out, pvalue, colors, size, tick)
    End Function

    <Extension>
    Public Function RunProfile(profiles As EnrichmentProfiles(), maps As MapRepository, out$,
                               Optional pvalue# = 0.05,
                               Optional color$ = "Set1:c6",
                               Optional size$ = "2000,1600",
                               Optional tick# = 1) As Integer

        Dim KO = Pathway.LoadFromResource.ToDictionary(Function(map) "map" & map.EntryId)
        Dim enrichmentTerms = profiles _
            .GroupBy(Function(map) map.pathway) _
            .Select(Function(pathway)
                        Dim profileVec = pathway.Select(Function(map) map.profile).ToArray
                        Dim maxprofile = pathway.OrderByDescending(Function(p) p.profile).First

                        Return New EnrichmentTerm With {
                            .Backgrounds = pathway.Count,
                            .CorrectedPvalue = 10 ^ (-profileVec.Average),
                            .Database = "KEGG",
                            .ID = pathway.Key,
                            .Input = maxprofile.profile,
                            .link = 1,
                            .number = 1,
                            .ORF = {maxprofile.RankGroup},
                            .Pvalue = 10 ^ (-profileVec.Max / 2),
                            .Term = KO(maxprofile.pathway).entry.text
                        }
                    End Function) _
            .OrderBy(Function(t) t.Pvalue) _
            .ToArray

        ' 进行绘图
        ' 绘制profile
        ' 绘制enrichment
        Dim plot As GraphicsData = enrichmentTerms _
            .KEGGEnrichmentPlot(
                size, pvalue,
                gray:=False,
                labelRightAlignment:=False,
                tick:=tick,
                colorSchema:=color
            )

        Call enrichmentTerms.SaveTo($"{out}/terms.csv")
        Call plot.Save($"{out}/pathway_enrichment.png")
        Call profiles _
            .Select(Function(profile)
                        Dim info As Pathway = KO(profile.pathway)

                        Return New EntityObject With {
                            .ID = profile.pathway,
                            .Properties = New Dictionary(Of String, String) From {
                                {"pvalue", profile.pvalue},
                                {"profile", profile.profile},
                                {"name", info.entry.text},
                                {"category", info.category},
                                {"taxonomy", profile.RankGroup}
                            }
                        }
                    End Function) _
            .ToArray _
            .SaveDataSet(out & "/profiles.csv")

        ' 生成网络模型
        Return profiles _
            .MicrobiomePathwayNetwork(KEGG:=maps, cutoff:=pvalue) _
            .Tabular(properties:={"pvalue", "profile"}) _
            .Save(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 这个只是生成了数据模型,并没有作图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Membrane_transport.network")>
    <Description("Construct a relationship network based on the Membrane transportor in bacteria genome")>
    <Usage("/Membrane_transport.network /metagenome <list.txt/OTU.tab/biom> /ref <reaction.repository.XML> /uniprot <repository.json> [/out <network.directory>]")>
    Public Function Membrane_transportNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/metagenome"
        Dim ref As ReactionRepository = ReactionRepository.LoadAuto(args("/ref")).Enzymetic
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.network/"
        Dim list$()
        Dim taxonomy As Taxonomy()
        ' Dim Membrane_transport = EntityObject.LoadDataSet(args <= "/Membrane_transport").ToArray
        'Dim enzymies As Dictionary(Of String, Enzyme()) = Membrane_transport _
        '    .Select(Function(e)
        '                Return New Enzyme(e.ID, e!fullName, e!EC_number)
        '            End Function) _
        '    .Where(Function(e) Not e.EC Is Nothing) _
        '    .GroupBy(Function(e) e.KO) _
        '    .ToDictionary(Function(e) e.Key,
        '                  Function(g)
        '                      Return g.ToArray
        '                  End Function)

        If [in].ExtensionSuffix.ToLower Like biomSuffix Then
            taxonomy = SMRUCC.genomics.foundation.BIOM _
                .ReadAuto([in]) _
                .rows _
                .Select(Function(r As row)
                            Return r.metadata?.lineage
                        End Function) _
                .ToArray
        Else
            If [in].ExtensionSuffix.TextEquals("csv") Then
                list = EntityObject.LoadDataSet([in]).Keys
            Else
                list = [in].ReadAllLines
            End If

            taxonomy = list _
                .Distinct _
                .Select(Function(tax) New Taxonomy(BIOMTaxonomy.TaxonomyParser(tax))) _
                .ToArray
        End If

        Dim network As NetworkGraph = TaxonomyRepository _
            .LoadRepository(args("/uniprot")) _
            .PopulateModels(taxonomy, distinct:=True) _
            .Where(Function(g)
                       ' 有些基因组的数据是空的？？
                       Return Not g.genome.Terms.IsNullOrEmpty
                   End Function) _
            .BuildTransferNetwork(repo:=ref)

        Return network _
            .Tabular(properties:={"color", "taxonomy", "reaction", "title"}) _
            .Save(out) _
            .CLICode
    End Function

    ReadOnly biomSuffix As Index(Of String) = {"json", "biom"}

    <ExportAPI("/microbiome.metabolic.network")>
    <Usage("/microbiome.metabolic.network /metagenome <list.txt/OTU.tab/biom> /ref <reaction.repository.XML> /uniprot <repository.json> /Membrane_transport <Membrane_transport.csv> [/out <network.directory>]")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    <Description("Construct a metabolic complementation network between the bacterial genomes from a given taxonomy list.")>
    <ArgumentAttribute("/uniprot", False, CLITypes.File,
              Extensions:="*.json",
              Description:="A reference model which is generated from ``/Metagenome.UniProt.Ref`` command.")>
    Public Function MetabolicComplementationNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/metagenome"
        Dim ref As ReactionRepository = ReactionRepository.LoadAuto(args("/ref")) '.Enzymetic
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.network/"
        Dim list$()
        Dim taxonomy As Taxonomy()

        If [in].ExtensionSuffix.ToLower Like biomSuffix Then
            taxonomy = SMRUCC.genomics.foundation.BIOM _
                .ReadAuto([in]) _
                .rows _
                .Select(Function(r As row)
                            Return r.metadata?.lineage
                        End Function) _
                .ToArray
        Else
            If [in].ExtensionSuffix.TextEquals("csv") Then
                list = EntityObject.LoadDataSet([in]).Keys
            Else
                list = [in].ReadAllLines
            End If

            taxonomy = list _
                .Distinct _
                .Select(Function(tax) New Taxonomy(BIOMTaxonomy.TaxonomyParser(tax))) _
                .ToArray
        End If

        Dim network As NetworkGraph = TaxonomyRepository _
            .LoadRepository(args("/uniprot")) _
            .PopulateModels(taxonomy, distinct:=True) _
            .Where(Function(g)
                       ' 有些基因组的数据是空的？？
                       Return Not g.genome.Terms.IsNullOrEmpty
                   End Function) _
            .BuildMicrobiomeMetabolicNetwork(reactions:=ref)

        Return network _
            .Tabular(properties:={"Color", "Family"}) _
            .Save(out) _
            .CLICode
    End Function

    <ExportAPI("/Metabolic.EndPoint.Profiles.background")>
    <Usage("/Metabolic.EndPoint.Profiles.Background /ref <reaction.repository.XML> /uniprot <repository.json> [/out <background.XML>]")>
    <Description("Create Metabolic EndPoint Profiles Background Model")>
    Public Function MetabolicEndPointProfilesBackground(args As CommandLine) As Integer
        Dim ref As ReactionRepository = ReactionRepository.LoadAuto(args("/ref"))
        Dim nonEnzymetic As bGetObject.Reaction() = ref.NonEnzymetic.ToArray
        Dim uniprot$ = args <= "/uniprot"
        Dim out$ = args("/out") Or $"{uniprot.TrimSuffix}.endpoints.Xml"
        Dim background = TaxonomyRepository.LoadRepository(uniprot) _
            .GetAll _
            .Values _
            .Where(Function(tax)
                       ' 有些基因组的数据是空的？？
                       Return Not tax.genome.Terms.IsNullOrEmpty
                   End Function) _
            .DoCall(Function(list)
                        Return MetabolicEndPointProfiles.CreateProfiles(list, ref, nonEnzymetic)
                    End Function)

        Return background.GetXml _
            .SaveTo(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 创建微生物组代谢途径富集计算所需要的背景参考库
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Metagenome.UniProt.Ref")>
    <Usage("/Metagenome.UniProt.Ref /in <uniprot.ultralarge.xml/cache.directory> [/cache /all /out <out.json>]")>
    <Description("Create background model for apply pathway enrichment analysis of the Metagenome data.")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.Xml",
              Description:="This argument should be the uniprot database file, multiple file is supported, which the multiple xml file path can be contract by ``|`` as delimiter.")>
    <ArgumentAttribute("/cache", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Debug used only.")>
    <ArgumentAttribute("/all", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this argument is presented, then all of the genome data will be saved, 
              includes all of the genome data that have ZERO coverage.")>
    Public Function BuildUniProtReference(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$
        Dim ref As TaxonomyRepository
        Dim all As Boolean = args("/all")

        If [in].Contains("|") OrElse [in].FileExists Then
            Dim cache As CacheGenerator = Nothing
            Dim files = [in].Split("|"c)

            out = args("/out") Or (files(Scan0).TrimSuffix & ".taxonomy_ref.json")
            ' 输入时由多个uniprot的XML数据库的Xml文件所构成的
            ' 这个通常用于组合uniprot_sprot以及uniprot_trembl这两个数据库文件的内容
            ref = UniProtXML _
                .EnumerateEntries(files) _
                .ScanUniProt(out.ParentPath & "/taxonomy_ref", all:=all, cache:=cache)

            If args.IsTrue("/cache") Then
                Call cache.CopyTo(destination:=out.TrimSuffix)
            End If
        Else
            out = args("/out") Or ([in].TrimDIR & ".taxonomy_ref.json")
            ref = UniProtBuild.ScanModels(
                cache:=New CacheGenerator([in]),
                export:=out.ParentPath & "/taxonomy_ref",
                all:=all
            )
        End If

        Return ref _
            .GetJson _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/UniProt.screen.model")>
    <Usage("/UniProt.screen.model /in <model.Xml> [/coverage <default=0.6> /terms <default=1000> /out <subset.xml>]")>
    <ArgumentAttribute("/in", Description:="The metagenome network UniProt reference database that build from ``/Metagenome.UniProt.Ref`` command.")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    Public Function ScreenModels(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim coverage# = args.GetValue("/coverage", 0.6)
        Dim terms% = args.GetValue("/terms", 1000)
        Dim out$ = args("/out") Or $"{[in].TrimSuffix},coverage={coverage},terms={terms}.Xml"
        Dim repository As TaxonomyRepository = [in].LoadXml(Of TaxonomyRepository)
        Dim idlist As String() = repository.taxonomy _
            .Keys _
            .Where(Function(taxid)
                       Dim genome As TaxonomyRef = repository.LoadByTaxonomyId(taxid)

                       Return Not genome.organism Is Nothing AndAlso
                              Not genome.genome.Terms.IsNullOrEmpty AndAlso
                                  genome.coverage >= coverage AndAlso
                                  genome.genome.Terms.Length >= terms
                   End Function) _
            .ToArray

        repository.taxonomy = repository.taxonomy.Subset(idlist)

        Return repository _
            .GetXml _
            .SaveTo(out) _
            .CLICode
    End Function
End Module
