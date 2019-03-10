﻿#Region "Microsoft.VisualBasic::96e3be24a684b26541a484c4d834ca74, CLI_tools\eggHTS\CLI\4. NetworkEnrichment.vb"

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
    '     Function: FunctionalNetworkEnrichment, GeneIDListFromKOBASResult, KOBASNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.Microarray.DEGProfiling
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Model.Network.STRING
Imports protein = Microsoft.VisualBasic.Data.csv.IO.EntityObject

Partial Module CLI

    ''' <summary>
    ''' 可视化string-db搜索结果，并使用KEGG pathway进行颜色分组
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/func.rich.string")>
    <Usage("/func.rich.string /in <string_interactions.tsv> /uniprot <uniprot.XML> /DEP <dep.t.test.csv> [/map <map.tsv> /r.range <default=12,30> /log2FC <default=log2FC> /layout <string_network_coordinates.txt> /out <out.network.DIR>]")>
    <Description("DEPs' functional enrichment network based on string-db exports, and color by KEGG pathway.")>
    <Group(CLIGroups.NetworkEnrichment_CLI)>
    <Argument("/map", True, CLITypes.File,
              Description:="A tsv file that using for map the user custom gene ID as the uniprotKB ID, in format like: ``UserID<TAB>UniprotID``")>
    <Argument("/DEP", False, CLITypes.File,
              AcceptTypes:={GetType(DEP_iTraq)},
              Description:="The DEPs t.test output result csv file.")>
    <Argument("/r.range", True, CLITypes.String,
              AcceptTypes:={GetType(DoubleRange)},
              Description:="The network node size radius range, input string in format like: ``min,max``")>
    <Argument("/log2FC", True, CLITypes.String,
              Description:="The csv field name for read the DEPs fold change value, default is ``log2FC`` as the field name.")>
    Public Function FunctionalNetworkEnrichment(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim uniprot$ = args <= "/uniprot"
        Dim DEP$ = args <= "/DEP"
        Dim log2FC$ = args.GetValue("/log2FC", NameOf(log2FC))
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & "-funrich_string/")
        Dim proteins As protein() = protein.LoadDataSet(DEP).UserCustomMaps(args <= "/map")
        Dim stringNetwork = [in].LoadTsv(Of InteractExports)
        Dim layouts As Coordinates() = (args <= "/layout").LoadTsv(Of Coordinates)
        Dim annotations = UniProtXML.Load(uniprot).StringUniprot ' STRING -> uniprot
        Dim DEGs = proteins.GetDEGs(
            Function(gene)
                Return gene("is.DEP").TextEquals("TRUE")
            End Function,
            log2FC)
        Dim Uniprot2STRING = annotations.Uniprot2STRING
        Dim radius = args.GetValue("/r.range", "12,30")

        With DEGs
            DEGs = (Uniprot2STRING(.UP), Uniprot2STRING(.DOWN))
        End With

        With stringNetwork.NetworkVisualize(
            annotations:=annotations,
            DEGs:=DEGs,
            layouts:=layouts,
            radius:=radius)

            Call .image _
                .SaveAs(out & "/network.png")

            Return .model _
                .Save(out) _
                .CLICode
        End With
    End Function

    ''' <summary>
    ''' 将KOBAS富集得到的基因的编号列表写入到一个文本文件之中
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/Gene.list.from.KOBAS")>
    <Usage("/Gene.list.from.KOBAS /in <KOBAS.csv> [/p.value <default=1> /out <out.txt>]")>
    <Description("Using this command for generates the gene id list input for the STRING-db search.")>
    <Argument("/p.value", True, AcceptTypes:={GetType(Double)},
              Description:="Using for enrichment term result filters, default is p.value less than or equals to 1, means no cutoff.")>
    <Group(CLIGroups.NetworkEnrichment_CLI)>
    Public Function GeneIDListFromKOBASResult(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim pvalue = args.GetValue("/p.value", 1.0R)
        Dim out$ = If(pvalue = 1.0R,
            [in].TrimSuffix & ".gene.list.txt",
            [in].TrimSuffix & $"_p.value={pvalue},gene.list.txt")

        out = args.GetValue("/out", out)

        Dim data As EnrichmentTerm() = [in].LoadCsv(Of EnrichmentTerm)
        data = data _
            .Where(Function(t) t.Pvalue <= pvalue) _
            .ToArray

        Return data _
            .Select(Function(t) t.ORF) _
            .IteratesALL _
            .Distinct _
            .ToArray _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/richfun.KOBAS")>
    <Usage("/richfun.KOBAS /in <string_interactions.tsv> /uniprot <uniprot.XML> /DEP <dep.t.test.csv> /KOBAS <enrichment.csv> [/r.range <default=5,20> /fold <1.5> /iTraq /logFC <logFC> /layout <string_network_coordinates.txt> /out <out.network.DIR>]")>
    <Argument("/KOBAS", Description:="The pvalue result in the enrichment term, will be using as the node radius size.")>
    <Group(CLIGroups.NetworkEnrichment_CLI)>
    Public Function KOBASNetwork(args As CommandLine) As Integer

    End Function
End Module
