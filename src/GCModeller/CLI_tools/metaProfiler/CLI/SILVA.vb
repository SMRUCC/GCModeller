#Region "Microsoft.VisualBasic::1bdaeb075db3118c64bfc8bd9a2d8e61, CLI_tools\metaProfiler\CLI\SILVA.vb"

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
    '     Function: ClusterOTU, gastTaxonomy_greengenes, SILVA_headers, SILVABacterial
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Analysis.Metagenome.greengenes
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    ''' <summary>
    ''' 如果不需要序列，而只是需要根据编号来获取物种的分类信息的话，可以先使用这个命令建立SILVA物种数据库，直接从这个建立好的库之中获取物种分类信息即可
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/SILVA.headers")>
    <Usage("/SILVA.headers /in <silva.fasta> /out <headers.tsv>")>
    <Group(CLIGroups.SILVA_cli)>
    Public Function SILVA_headers(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.headers.tsv"

        Using writer As StreamWriter = out.OpenWriter
            For Each SSU As FastaSeq In StreamIterator.SeqSource(handle:=[in], debug:=True)
                Dim headers = SSU.Headers.JoinBy("|").GetTagValue(" ", trim:=True)
                Call writer.WriteLine(headers.Name & vbTab & headers.Value)
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/SILVA.bacteria")>
    <Usage("/SILVA.bacteria /in <silva.fasta> [/out <silva.bacteria.fasta>]")>
    Public Function SILVABacterial(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].ParentPath}/SILVA.bacterial_ssuref.fasta"

        Using writer As StreamWriter = out.OpenWriter
            For Each SSU As FastaSeq In StreamIterator.SeqSource(handle:=[in]).SILVABacteria
                Call writer.WriteLine(SSU.GenerateDocument(lineBreak:=120))
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/OTU.cluster")>
    <Usage("/OTU.cluster /left <left.fq> /right <right.fq> /silva <silva.bacteria.fasta> [/out <out.directory> /processors <default=2> /@set mothur=path]")>
    Public Function ClusterOTU(args As CommandLine) As Integer
        Dim left$ = args <= "/left"
        Dim right$ = args <= "/right"
        Dim out$ = args("/out") Or "./"
        Dim silva$ = args("/silva")
        Dim num_threads% = args.GetValue("/processors", 2)

        Call MothurContigsOTU.ClusterOTUByMothur(
            left, right,
            silva:=silva,
            workspace:=out,
            processor:=num_threads
        )

        Return 0
    End Function

    <ExportAPI("/gast.Taxonomy.greengenes")>
    <Usage("/gast.Taxonomy.greengenes /in <blastn.txt> /query <OTU.rep.fasta> /taxonomy <97_otu_taxonomy.txt> [/removes.lt <default=0.0001> /gast.consensus /min.pct <default=0.6> /out <gastOut.csv>]")>
    <Description("OTU taxonomy assign by apply gast method on the result of OTU rep sequence alignment against the greengenes.")>
    <ArgumentAttribute("/removes.lt", True, CLITypes.Double,
              Description:="OTU contains members number less than the percentage value of this argument value(low abundance) will be removes from the result.")>
    <ArgumentAttribute("/min.pct", True, CLITypes.Double,
              Description:="The required minium vote percentage of the taxonomy assigned from a OTU reference alignment by using gast method, default is required level 60% agreement.")>
    <Group(CLIGroups.Taxonomy_cli)>
    Public Function gastTaxonomy_greengenes(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim query$ = args <= "/query"
        Dim taxonomy$ = args <= "/taxonomy"
        Dim minPct# = args.GetValue("/min.pct", 0.6)
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.{taxonomy.BaseName}.gast_min.pct={minPct}.csv"
        Dim lt# = args.GetValue("/removes.lt", 0.0001)
        Dim OTUs As Dictionary(Of String, NamedValue(Of Integer)) =
            StreamIterator _
            .SeqSource(query) _
            .ParseOTUrep() _
            .RemovesOTUlt(cutoff:=lt)
        Dim otu_taxonomy = greengenes.otu_taxonomy _
            .Load(taxonomy) _
            .ToDictionary(Function(t) t.ID)
        Dim blastn As IEnumerable(Of Query) = BlastnOutputReader.RunParser([in])
        Dim gast As gastOUT()

        If args.IsTrue("/gast.consensus") Then
            gast = blastn _
                .OTUgreengenesTaxonomy(OTUs, otu_taxonomy, min_pct:=minPct) _
                .ToArray
        Else
            gast = blastn _
                .OTUgreengenesTaxonomyTreeAssign(OTUs, otu_taxonomy, min_pct:=minPct) _
                .ToArray
        End If

        Return gast.SaveTo(out).CLICode
    End Function
End Module
