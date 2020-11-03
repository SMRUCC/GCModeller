#Region "Microsoft.VisualBasic::d147130aa0cacac1225cd1e3759cb9e0, CLI_tools\VirtualFootprint\CLI\Sites.vb"

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
    '     Function: MatchSiteGenes, MergeSites, TrimStrand
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Model.Network.VirtualFootprint
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Partial Module CLI

    <ExportAPI("/Trim.Strand",
               Info:="Removes all of the sites which is on the different strand with the tag gene.",
               Usage:="/Trim.Strand /in <segments.Csv> /PTT <genome.ptt> [/out <out.csv>]")>
    Public Function TrimStrand(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim PTT As String = args("/PTT")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & PTT.BaseName & ".Csv")
        Dim sites As SimpleSegment() = [in].LoadCsv(Of SimpleSegment)
        Dim genome As PTT = TabularFormat.PTT.Load(PTT)

        sites = sites.TrimStranded(genome, Function(x) x.ID.Split(":"c).First).ToArray

        Return sites.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Merge.Sites",
               Info:="Merge the segment loci sites within the specific length offset ranges.",
               Usage:="/Merge.Sites /in <segments.Csv> [/nt <nt.fasta> /out <out.csv> /offset <10>]")>
    Public Function MergeSites(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim offset As Integer = args.GetValue("/offset", 10)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-offsets-" & offset & ".Csv")
        Dim sites As SimpleSegment() = [in].LoadCsv(Of SimpleSegment)
        Dim seq As String = args - "/nt"

        sites = sites.MergeLocis(offset,
                                 Function(x) CInt(x.ID.Split(":"c).Last),
                                 Function(x) x.ID.Split(":"c).First).ToArray

        If seq.FileExists Then
            Dim nt As FastaSeq = FastaSeq.LoadNucleotideData(seq)

            For Each site As SimpleSegment In sites
                site.SequenceData = nt.CutSequenceLinear(site.MappingLocation).SequenceData
            Next
        End If

        Return sites.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 获取得到给定位点相关的下游基因列表，以及该motif位点的序列片段
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/Site.match.genes")>
    <Usage("/Site.match.genes /in <sites.csv> /genome <genome.gb> [/replicon <default=accession> /skip.RNA /max.dist <default=500bp> /out <out.csv>]")>
    <Description("Match genome context for the sites model.")>
    <ArgumentAttribute("/replicon", True, CLITypes.String, AcceptTypes:={GetType(String)},
              Description:="This argument indicate the replicon name source of your export result. The option can be ``accession/locus``, by default is ``accession``.")>
    Public Function MatchSiteGenes(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim gb As GBFF.File = GBFF.File.Load(args <= "/genome")
        Dim maxDist% = args("/max.dist") Or 500
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.genome_context,max_dist={maxDist}.csv"
        Dim skipRNA As Boolean = args("/skip.RNA")
        Dim context As New GenomeContext(Of GeneBrief)(gb.GbffToPTT(ORF:=skipRNA), name:=gb.Source.SpeciesName)
        Dim nt As FastaSeq = gb.Origin.ToFasta
        Dim isPlasmid As Boolean = gb.isPlasmid
        Dim replicon$

        If (args("/replicon") Or "accession").TextEquals("accession") Then
            replicon = gb.Accession.AccessionId Or $"{gb.Accession.AccessionId}=plasmid".When(isPlasmid)
        Else
            replicon = gb.Locus.AccessionID Or $"{gb.Locus.AccessionID}=plasmid".When(isPlasmid)
        End If

        Using output As New WriteStream(Of FootprintSite)(out)
            For Each site As MotifSiteMatch In [in].LoadCsv(Of MotifSiteMatch)
                Dim strand As Strands = site.MappingLocation.Strand
                Dim downstream As GeneBrief()
                Dim footprint As FootprintSite
                Dim sequence As SimpleSegment = nt.CutSequenceLinear(site.MappingLocation)

                ' 挑选出下游的基因
                If strand = Strands.Forward Then
                    Dim min = site.MappingLocation.Right
                    Dim max = site.MappingLocation.Right + maxDist - 1
                    Dim left = site.MappingLocation.Left

                    downstream = context _
                        .SelectByRange(min, max, Strands.Forward) _
                        .Where(Function(gene)
                                   ' gene的left要大于位点的最左端
                                   ' 即取出下游
                                   Return gene.Location.Left > left
                               End Function) _
                        .ToArray

                    For Each gene As GeneBrief In downstream
                        footprint = New FootprintSite With {
                            .distance = gene.Location.Left - min, ' 起始位点减去区域的最右边
                            .left = site.MappingLocation.Left,
                            .right = site.MappingLocation.Right,
                            .strand = site.strand,
                            .gene = gene.Synonym,
                            .ID = site.ID,
                            .location = gene.Location,
                            .product = gene.Product,
                            .src = site.src,
                            .sequenceData = sequence,
                            .replicon = replicon
                        }
                        output.Flush(footprint)
                    Next
                Else
                    Dim max = site.MappingLocation.Left + 1
                    Dim min = max - maxDist + 1
                    Dim right = site.MappingLocation.Right

                    downstream = context _
                        .SelectByRange(min, max, Strands.Reverse) _
                        .Where(Function(gene)
                                   ' gene的right要小于位点的最右端
                                   ' 即取出下游
                                   Return gene.Location.Right < right
                               End Function) _
                        .ToArray

                    For Each gene As GeneBrief In downstream
                        footprint = New FootprintSite With {
                            .distance = max - gene.Location.Right, ' 区域的最左边减去基因的起始位点
                            .gene = gene.Synonym,
                            .ID = site.ID,
                            .left = site.left,
                            .right = site.right,
                            .location = gene.Location,
                            .product = gene.Product,
                            .src = site.src,
                            .strand = site.strand,
                            .sequenceData = sequence,
                            .replicon = replicon
                        }
                        output.Flush(footprint)
                    Next
                End If
            Next
        End Using

        Return 0
    End Function
End Module
