﻿#Region "Microsoft.VisualBasic::a62c0d8fa71ea8309fd5aa72a59675d8, CLI_tools\meta-assmebly\CLI\Network.vb"

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
'     Function: BuildUniProtReference, MetabolicComplementationNetwork, PathwayProfiles, (+2 Overloads) RunProfile, ScreenModels
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.Model.Network.Microbiome

Partial Module CLI

    <ExportAPI("/microbiome.pathway.profile")>
    <Usage("/microbiome.pathway.profile /in <gastout.csv> /ref <UniProt.ref.XML> /maps <kegg.maps.ref.XML> [/just.profiles /rank <default=family> /p.value <default=0.05> /out <out.directory>]")>
    <Description("Generates the pathway network profile for the microbiome OTU result based on the KEGG and UniProt reference.")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    Public Function PathwayProfiles(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim ref$ = args <= "/ref"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.pathway.profiles/"
        Dim gast As gast.gastOUT() = [in].LoadCsv(Of gast.gastOUT)
        Dim UniProt As TaxonomyRepository = Nothing
        Dim maps As MapRepository = args("/maps").LoadXml(Of MapRepository)
        Dim pvalue# = args.GetValue("/p.value", 0.05)
        Dim rank As TaxonomyRanks = args.GetValue("/rank", TaxonomyRanks.Family, AddressOf ParseRank)

        Call "Load UniProt reference genome model....".__INFO_ECHO
        Call VBDebugger.BENCHMARK(Sub() UniProt = ref.LoadXml(Of TaxonomyRepository))

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

    <ExportAPI("/microbiome.pathway.run.profile")>
    <Usage("/microbiome.pathway.run.profile /in <profile.csv> /maps <kegg.maps.ref.Xml> [/p.value <default=0.05> /out <out.directory>]")>
    <Description("Build pathway interaction network based on the microbiome profile result.")>
    <Argument("/p.value", True, CLITypes.Double,
              Description:="The pvalue cutoff of the profile mapID, selects as the network node if the mapID its pvalue is smaller than this cutoff value. By default is 0.05. If no cutoff, please set this value to 1.")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    Public Function RunProfile(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim maps As MapRepository = (args <= "/maps").LoadXml(Of MapRepository)
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.pathway.profiles/"
        Dim pvalue# = args.GetValue("/p.value", 0.05)
        Dim profiles = [in].LoadCsv(Of Profile) _
            .GroupBy(Function(tax) tax.RankGroup) _
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

        Return profiles.RunProfile(maps, out, pvalue)
    End Function

    <Extension>
    Public Function RunProfile(profiles As EnrichmentProfiles(), maps As MapRepository, out$, Optional pvalue# = 0.05) As Integer
        Dim KO = Pathway.LoadFromResource.ToDictionary(Function(map) "map" & map.EntryId)

        ' 进行绘图
        ' 绘制profile


        ' 绘制enrichment

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

    <ExportAPI("/microbiome.metabolic.network")>
    <Usage("/microbiome.metabolic.network /metagenome <list.txt/OTU.tab> /ref <reaction.repository.XML> /uniprot <repository.XML> [/out <network.directory>]")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    Public Function MetabolicComplementationNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/metagenome"
        Dim UniProt As TaxonomyRepository = Nothing
        Dim ref As ReactionRepository = args("/ref") _
            .LoadXml(Of ReactionRepository) _
            .Enzymetic
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.network/"
        Dim list$()

        Call "Load UniProt reference genome model....".__INFO_ECHO
        Call VBDebugger.BENCHMARK(Sub() UniProt = args("/uniprot").LoadXml(Of TaxonomyRepository))

        If [in].ExtensionSuffix.TextEquals("csv") Then
            list = EntityObject.LoadDataSet([in]).Keys
        Else
            list = [in].ReadAllLines
        End If

        Dim taxonomy As Taxonomy() = list _
            .Distinct _
            .Select(Function(tax) New Taxonomy(BIOMTaxonomy.TaxonomyParser(tax))) _
            .ToArray
        Dim network As NetworkGraph = UniProt _
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

    <ExportAPI("/Metagenome.UniProt.Ref")>
    <Usage("/Metagenome.UniProt.Ref /in <uniprot.ultralarge.xml/cache.directory> [/cache /out <out.XML>]")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    Public Function BuildUniProtReference(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$
        Dim ref As TaxonomyRepository

        If [in].FileExists Then
            Dim cache As (String, String, String) = Nothing

            out = args("/out") Or ([in].TrimSuffix & ".taxonomy_ref.Xml")
            ref = UniProtXML.EnumerateEntries([in]).ScanUniProt(cache)

            If args.IsTrue("/cache") Then
                Call cache.CopyTo(destination:=out.TrimSuffix)
            End If
        Else
            out = args("/out") Or ([in].TrimDIR & ".taxonomy_ref.Xml")
            ref = UniProtBuild.ScanModels(cache:=[in])
        End If

        Return ref _
            .GetXml _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/UniProt.screen.model")>
    <Usage("/UniProt.screen.model /in <model.Xml> [/coverage <default=0.6> /terms <default=1000> /out <subset.xml>]")>
    <Argument("/in", Description:="The metagenome network UniProt reference database that build from ``/Metagenome.UniProt.Ref`` command.")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    Public Function ScreenModels(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim coverage# = args.GetValue("/coverage", 0.6)
        Dim terms% = args.GetValue("/terms", 1000)
        Dim out$ = args("/out") Or $"{[in].TrimSuffix},coverage={coverage},terms={terms}.Xml"
        Dim models As TaxonomyRepository = [in].LoadXml(Of TaxonomyRepository)

        models.Taxonomy = models.Taxonomy _
            .Where(Function(genome)
                       Return Not genome.organism Is Nothing AndAlso
                              Not genome.genome.Terms.IsNullOrEmpty AndAlso
                                  genome.Coverage >= coverage AndAlso
                                  genome.genome.Terms.Length >= terms
                   End Function) _
            .ToArray

        Return models _
            .GetXml _
            .SaveTo(out) _
            .CLICode
    End Function
End Module
