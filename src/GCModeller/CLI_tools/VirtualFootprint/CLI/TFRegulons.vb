#Region "Microsoft.VisualBasic::4bcf67cfc1be07e438c0e0631ce4a83b, CLI_tools\VirtualFootprint\CLI\TFRegulons.vb"

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
'     Function: ContextMappings, PathwaySites, RegulonSites, TFDensity, TFDensityBatch
'               TFRegulons
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Text
Imports Parallel.ThreadTask
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Partial Module CLI

    <ExportAPI("/TF.Regulons",
               Usage:="/TF.Regulons /bbh <tf.bbh.csv> /footprints <regulations.csv> [/out <out.csv>]")>
    <Group(CLIGrouping.TFRegulonTools)>
    Public Function TFRegulons(args As CommandLine) As Integer
        Dim [in] As String = args("/bbh")
        Dim footprints As String = args("/footprints")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $"-{footprints.BaseName}.Csv")
        Dim bbh As IEnumerable(Of String) =
            [in].LoadCsv(Of BBHIndex).Select(Function(x) x.QueryName).Distinct
        Dim regs As Dictionary(Of String, RegulatesFootprints()) = (From x As RegulatesFootprints
                                                                    In footprints.LoadCsv(Of RegulatesFootprints)
                                                                    Where Not String.IsNullOrEmpty(x.ORF) AndAlso
                                                                        Not String.IsNullOrEmpty(x.Regulator)
                                                                    Select x
                                                                    Group x By x.ORF Into Group) _
                                                                         .ToDictionary(Function(x) x.ORF,
                                                                                       Function(x) x.Group.ToArray)
        Dim result As New List(Of RegulatesFootprints)

        For Each ORF As String In bbh
            If regs.ContainsKey(ORF) Then
                result += regs(ORF)
            End If
        Next

        Return result.SaveTo(out)
    End Function

    <ExportAPI("/TF.Density.Batch",
               Usage:="/TF.Density.Batch /TF <TF-list.txt> /PTT <genome.PTT.DIR> [/ranges 5000 /out <out.DIR> /cis /un-strand]")>
    <Group(CLIGrouping.TFRegulonTools)>
    Public Function TFDensityBatch(args As CommandLine) As Integer
        Dim TFs As String = args("/TF")
        Dim PTT As String = args("/PTT")
        Dim ranges As Integer = args.GetValue("/ranges", 5000)
        Dim out As String = args.GetValue("/out", TFs.TrimSuffix & "/")
        Dim cis As String = args.Assert("/cis")
        Dim unstrand As String = args.Assert("/un-strand")
        Dim genomes As IEnumerable(Of String) = ls - l - r - wildcards("*.PTT") <= PTT
        Dim task As Func(Of String, String) =
            Function(genome) _
                $"{GetType(CLI).API(NameOf(CLI.TFDensity))} /TF {TFs.CLIPath} /PTT {genome.CLIPath} /ranges {ranges} /out {out & $"/{genome.BaseName}.Csv"} {cis} {unstrand} /batch"

        Dim CLIs As String() = genomes.Select(task).ToArray

        Return BatchTasks.SelfFolks(CLIs, LQuerySchedule.CPU_NUMBER) ' 使用Linq线程模块配置计算的并发数
    End Function

    ''' <summary>
    ''' 计算出调控因子在基因组上面的分布相对丰度
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/TF.Density",
               Usage:="/TF.Density /TF <TF-list.txt> /PTT <genome.PTT> [/ranges 5000 /out <out.csv> /cis /un-strand /batch]")>
    <ArgumentAttribute("/TF", False,
                   Description:="A plant text file with the TF locus_tag list.")>
    <ArgumentAttribute("/batch", True,
                   Description:="This function is works in batch mode.")>
    <Group(CLIGrouping.TFRegulonTools)>
    Public Function TFDensity(args As CommandLine) As Integer
        Dim TFs As String = args - "/TF"
        Dim PTT As String = args - "/PTT"
        Dim ranges As Integer = args.GetValue("/ranges", 5000)
        Dim locus As String() = TFs.ReadAllLines
        Dim genome As PTT = TabularFormat.PTT.Load(PTT)
        Dim cis As Boolean = args.GetBoolean("/cis")
        Dim result As Density()

        If args.GetBoolean("/batch") Then
            locus = LinqAPI.Exec(Of String) <= From sId As String
                                               In locus
                                               Where genome.ExistsLocusId(sId)
                                               Select sId
                                               Order By sId Ascending
        End If

        If cis Then
            result = ContextModel.TFDensity.DensityCis(genome, locus, ranges)
        Else
            result = ContextModel.TFDensity.Density(genome, locus, ranges, Not args.GetBoolean("/un-strand"))
        End If

        For Each x As Density In result
            x.locus_tag = genome(x.locus_tag).Gene
        Next

        Dim out As String =
            args.GetValue("/out", PTT.TrimSuffix & $"-{If(cis, "cis-", "")}TF.Density.Csv")

        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Sites.Pathways",
               Info:="[Type 1] Grouping sites loci by pathway",
               Usage:="/Sites.Pathways /pathway <KEGG.DIR> /sites <simple_segment.Csv.DIR> [/out <out.DIR>]")>
    <Group(CLIGrouping.TFRegulonTools)>
    Public Function PathwaySites(args As CommandLine) As Integer
        Dim [in] As String = args("/pathway")
        Dim sites As String = args("/sites")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & "-" & sites.BaseName & "/")
        Dim pathways As bGetObject.Pathway() =
            (ls - l - r - wildcards("*.Xml") <= [in]) _
            .Select(AddressOf LoadXml(Of bGetObject.Pathway))
        Dim siteHash = (From x As SimpleSegment
                        In (ls - l - r - wildcards("*.Csv") <= sites) _
                           .Select(AddressOf csv.Extensions.LoadCsv(Of SimpleSegment)) _
                           .IteratesALL
                        Let sId As String = x.ID.Split(":"c).First
                        Select sId,
                            x
                        Group By sId Into Group) _
                           .ToDictionary(Function(x) x.sId,
                                         Function(x) x.Group.Select(Function(o) o.x))

        For Each pathway As bGetObject.Pathway In pathways
            Dim locis As SimpleSegment() =
                LinqAPI.Exec(Of SimpleSegment) <= From x As String
                                                  In pathway.GetPathwayGenes
                                                  Where siteHash.ContainsKey(x)
                                                  Select siteHash(x)

            Dim base As String = out & "/" & pathway.briteID

            locis = (From x As SimpleSegment
                     In locis
                     Where x.SequenceData.Length >= 8  ' MEME要求序列的长度至少8个字符
                     Select x
                     Group x By x.ID Into Group) _
                          .Select(Function(x) (From site As SimpleSegment
                                                In x.Group
                                               Select site
                                               Order By site.SequenceData.Length Descending).First)

            Call locis.SaveTo(base & ".Csv")
            Call New FastaFile(
                 locis.Select(Function(x) New FastaSeq({x.ID}, x.SequenceData))) _
                .Save(base & ".fasta", Encodings.ASCII)
        Next

        Return 0
    End Function

    <ExportAPI("/Sites.Regulons",
               Info:="[Type 2]",
               Usage:="/Sites.Regulons /regulon <RegPrecise.Regulon.Csv> /sites <simple_segment.Csv.DIR> [/map <genome.PTT> /out <out.DIR>]")>
    <Group(CLIGrouping.TFRegulonTools)>
    Public Function RegulonSites(args As CommandLine) As Integer
        Dim regulon As String = args("/regulon")
        Dim sites As String = args("/sites")
        Dim out As String = args.GetValue("/out", regulon.TrimSuffix & "-" & sites.BaseName & "/")
        Dim regulons As RegPreciseOperon() = regulon.LoadCsv(Of RegPreciseOperon)
        Dim siteHash = (From x As SimpleSegment
                        In (ls - l - r - wildcards("*.Csv") <= sites) _
                        .Select(AddressOf csv.Extensions.LoadCsv(Of SimpleSegment)) _
                        .IteratesALL
                        Where x.SequenceData.Length >= 8
                        Let sId As String = x.ID.Split(":"c).First
                        Select sId,
                            x
                        Group By sId Into Group) _
                        .ToDictionary(Function(x) x.sId,
                                      Function(x) x.Group.Select(Function(o) o.x))
        Dim PTT As String = args("/map")
        Dim maps As Func(Of String, String)

        If PTT.FileExists Then
            Dim genome As PTT = TabularFormat.PTT.Load(PTT)
            maps = Function(id) genome(id)?.Gene
        Else
            maps = Function(id) id
        End If

        For Each operon As RegPreciseOperon In regulons
            Dim path As String =
                out & "/" & (operon.Pathway & "-" & operon.source).NormalizePathString(True).Replace(" ", "_") & ".fasta"
            Dim members As String() = operon.Operon.Select(maps).ToArray
            Dim sitesLoci As SimpleSegment() =
                LinqAPI.Exec(Of SimpleSegment) <= From sId As String
                                                  In members
                                                  Where siteHash.ContainsKey(sId)
                                                  Select siteHash(sId)
            sitesLoci = (From x As SimpleSegment
                         In sitesLoci
                         Select x
                         Group x By x.ID.Split(":"c).First Into Group) _
                              .Select(Function(x) (From site As SimpleSegment
                                                    In x.Group
                                                   Select site
                                                   Order By site.SequenceData.Length Descending).First)
            Call New FastaFile(
              sitesLoci.Select(Function(x) New FastaSeq({x.ID}, x.SequenceData))) _
             .Save(path, Encodings.ASCII)
        Next

        Return 0
    End Function

    ''' <summary>
    ''' 将相对丰度映射为逻辑位置
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Density.Mappings",
               Usage:="/Density.Mappings /in <density.Csv> [/scale 100 /out <out.PTT>]")>
    <Group(CLIGrouping.TFRegulonTools)>
    Public Function ContextMappings(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Maps.PTT")
        Dim density As IEnumerable(Of Density) =
            LinqAPI.Exec(Of Density) <= From x As Density
                                        In [in].LoadCsv(Of Density)
                                        Select x
                                        Order By x.Abundance Ascending

        Dim min As Double = density.Min(Function(x) x.Abundance)
        Dim max As Double = density.Max(Function(x) x.Abundance)
        Dim len As Double = max - min
        Dim totalLen As Integer = density.Sum(Function(x) x.loci.FragmentSize)
        Dim list As New List(Of GeneBrief)
        Dim scale As Integer = args.GetValue("/scale", 100)
        Dim plus As Integer
        Dim minus As Integer
        Dim pre As Double = min

        For Each gene As Density In density  ' 在这里主要是根据相对长度以及丰度来计算出映射的位置，原来的实际的物理位置已经不重要了
            Dim left As Double = If(gene.loci.Strand = Strands.Forward, plus, minus)
            Dim gLen As Double =
                scale *
                gene.loci.FragmentSize *
                gene.Abundance
            Dim right As Double = left + gLen
            Dim loci As New NucleotideLocation(CInt(left), CInt(right), gene.loci.Strand)
            Dim d As Integer = ((gene.Abundance - pre) + 1) * scale

            pre = gene.Abundance

            If gene.loci.Strand = Strands.Forward Then
                plus = right + d
            Else
                minus = right + d
            End If

            list += New GeneBrief With {
                .Code = "-",
                .COG = "-"c,
                .Gene = gene.locus_tag,
                .Length = gLen,
                .Location = loci,
                .PID = "",
                .Product = gene.product,
                .Synonym = gene.locus_tag
            }
        Next

        totalLen = list _
            .Select(Function(x) {x.Location.left, x.Location.right}) _
            .IteratesALL _
            .Max

        Dim PTT As New PTT(list, [in].BaseName & " #" & NameOf(ContextMappings), totalLen)
        Return PTT.Save(out, Encoding.ASCII).CLICode
    End Function
End Module
