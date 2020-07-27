#Region "Microsoft.VisualBasic::9de589a4975eee4a69bf12d50408c65f, visualize\Circos\CLI\CLI\CLI.vb"

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
    '     Function: AlignmentTableDump, ATContent, GCSkew, MGA2Myva, NTVariation
    '               propertyVector, vector
    '     Class tRNA
    ' 
    '         Properties: [end], AntiCodon, seqName, start, strand
    '                     tRNAType
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty
Imports SMRUCC.genomics.SequenceModel.Patterns

<Package("Circos.CLI",
                  Category:=APICategories.CLI_MAN,
                  Description:="Tools for generates the circos drawing model file for the circos perl script.")>
Public Module CLI

    <ExportAPI("/NT.Variation",
               Usage:="/NT.Variation /mla <fasta.fa> [/ref <index/fasta.fa, 0> /out <out.txt> /cut 0.75]")>
    Public Function NTVariation(args As CommandLine) As Integer
        Dim mla As String = args("/mla")
        Dim ref As String = args.GetValue("/ref", "0")
        Dim cut As Double = args.GetValue("/cut", 0.75)
        Dim source As New FastaFile(mla)

        If ref.FileExists Then
            Dim refFa As FastaSeq = New FastaSeq(ref)
            Dim out As String = args.GetValue("/out", mla.TrimSuffix & "-" & ref.BaseName & ".NTVariations.txt")
            Dim vec = refFa.NTVariations(source, cut)
            Return vec.FlushAllLines(out, Encodings.ASCII).CLICode
        Else
            Dim idx As Integer = source.Index(ref)
            Dim out As String = args.GetValue("/out", mla.TrimSuffix & "." & idx & ".NTVariations.txt")
            Dim vec = NTVariations(source, idx, cut)
            Return vec.FlushAllLines(out, Encodings.ASCII).CLICode
        End If
    End Function

    <ExportAPI("--AT.Percent", Usage:="--AT.Percent /in <in.fasta> [/win_size <200> /step <25> /out <out.txt>]")>
    Public Function ATContent(args As CommandLine) As Integer
        Dim inFasta As String = args("/in")
        Dim out As String = args.GetValue("/out", inFasta.TrimSuffix & ".ATPercent.txt")
        Dim winSize As Integer = args.GetValue("/win_size", 200)
        Dim steps As Integer = args.GetValue("/step", 25)
        Dim lst As FastaFile = FastaFile.Read(inFasta)

        If lst.Count = 1 Then
            Return propertyVector(AddressOf NucleicAcidStaticsProperty.ATPercent, lst.First, out, winSize, steps)
        End If

        Dim LQuery = (From genome As Integer
                      In lst.Sequence.AsParallel
                      Select genome,
                          percent = NucleicAcidStaticsProperty.ATPercent(lst(genome), winSize, steps, True)
                      Order By genome Ascending).ToArray
        Dim vector As Double() = LQuery.Select(Function(genome) genome.percent).vector
        Return vector.Select(Function(n) CStr(n)).FlushAllLines(out, Encodings.ASCII).CLICode
    End Function

    Private Function propertyVector(method As NtProperty, inFasta As FastaSeq, out As String, winSize As Integer, steps As Integer) As Integer
        Dim vector As Double() = method(inFasta, winSize, steps, True)
        Return vector.Select(Function(n) CStr(n)).FlushAllLines(out, Encodings.ASCII).CLICode
    End Function

    ''' <summary>
    ''' 如果只有一条序列，则只做一条序列的GCSkew，反之做所有序列的GCSkew的平均值，要求都是经过多序列比对对齐了的，默认第一条序列为参考序列
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--GC.Skew", Usage:="--GC.Skew /in <in.fasta> [/win_size <200> /step <25> /out <out.txt>]")>
    Public Function GCSkew(args As CommandLine) As Integer
        Dim inFasta = FastaFile.Read(args("/in"))
        Dim winSize As Integer = args.GetValue("/win_size", 200)
        Dim steps As Integer = args.GetValue("/step", 25)
        Dim out As String = args.GetValue("/out", inFasta.FilePath.TrimSuffix & ".GCSkew.txt")

        If inFasta.Count = 1 Then
            Return propertyVector(AddressOf NucleicAcidStaticsProperty.GCSkew, inFasta.First, out, winSize, steps)
        End If

        Dim LQuery = (From genome As Integer
                      In inFasta.Sequence.AsParallel
                      Select genome,
                          skew = SMRUCC.genomics.SequenceModel.NucleotideModels.GCSkew(inFasta(genome), winSize, steps, True)
                      Order By genome Ascending).ToArray  ' 排序是因为可能没有做多序列比对对齐，在这里需要使用第一条序列的长度作为参考
        Dim vector As Double() = LQuery.Select(Function(genome) genome.skew).vector
        Return vector.Select(Function(n) CStr(n)).FlushAllLines(out, Encodings.ASCII).CLICode
    End Function

    <Extension>
    Private Function vector(vectors As IEnumerable(Of Double())) As Double()
        Dim LQuery As Double() = vectors(Scan0).Select(Function(null, idx) vectors.Select(Function(genome) genome(idx)).Average)
        Return LQuery
    End Function

    <ExportAPI("/MGA2Myva", Usage:="/MGA2Myva /in <mga_cog.csv> [/out <myva_cog.csv> /map <genome.gb>]")>
    Public Function MGA2Myva(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".MyvaCOG.csv")
        Dim doc As MGACOG() = MGACOG.LoadDoc(inFile)
        Dim setName = New SetValue(Of MGACOG) <= NameOf(MGACOG.QueryName)
        doc = (From x In doc Let n As String = x.QueryName.Split("|"c)(6) Select setName(x, n)).ToArray
        Dim myva = MGACOG.ToMyvaCOG(doc)

        If Not String.IsNullOrEmpty(args("/map")) Then
            Dim gb = GBFF.File.Load(args("/map"))
            Dim maps As Dictionary(Of String, String) = gb.LocusMaps

            For Each x As MyvaCOG In myva
                If Not maps.ContainsKey(x.QueryName) Then
                    Continue For
                End If
                x.QueryName = maps(x.QueryName)
            Next
        End If

        Return myva.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Alignment.Dumps", Usage:="/Alignment.Dumps /in <inDIR> [/out <out.Xml>]")>
    Public Function AlignmentTableDump(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & ".Xml")
        Dim tbl As AlignmentTable = AlignmentTableParser.CreateFromBlastn(inDIR)
        Return tbl.Save(out).CLICode
    End Function

    'Public Function bg() As Integer
    '    Dim doc = CircosAPI.CreateDataModel
    '    doc.chromosomes_units = "10000"
    '    Dim fa = New FastaToken("F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\1329830.5.ED.fna")
    '    Call CircosAPI.SetBasicProperty(doc, fa)

    '    Dim ptt = LoadPTT("F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\1329830.5.ED.ptt")

    '    Dim regulons = "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\circos\Regulators.csv".LoadCsv(Of Name).Select(Function(x) x.ToMeta)
    '    Dim resistss = "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\circos\resistance.csv".LoadCsv(Of Name)
    '    regulons.Add(resistss.Select(Function(x) x.ToMeta))
    '    regulons = TrackDatas.Distinct(regulons)

    '    '   Dim labels = New HighlightLabel(regulons)

    '    ' Call Circos.CircosAPI.AddPlotElement(doc, New Plots.TextLabel(labels))

    '    Dim regulations = "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\MAST\Regulations.csv".LoadCsv(Of SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints.PredictedRegulationFootprint)
    '    Dim connector = FromVirtualFootprint(regulations, ptt, resistss)

    '    Call Circos.CircosAPI.AddPlotTrack(doc, New Connector(connector))



    '    Dim cog = "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\circos\output.MyvaCOG.csv".LoadCsv(Of MyvaCOG)
    '    Dim gb = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File.Load("F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\1329830.5.ED.gb")
    '    doc = Circos.CircosAPI.AddGeneInfoTrack(doc, gb, cog, splitOverlaps:=False)
    '    Dim tbl = "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\circos\1329830.5.ED.Blastn.Xml".LoadXml(Of AlignmentTable)
    '    Dim iddd = (From x In tbl.Hits Select x.Identity).ToArray
    '    Dim tblColor = CircosAPI.IdentityColors(iddd.Min, iddd.Max, 512)
    '    doc = CircosAPI.GenerateBlastnAlignment(doc, tbl, 1, 0.2, tblColor)

    '    Dim at = IO.File.ReadAllLines("F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\circos\1329830.5.2.ATPercent.txt").Select(Function(x) Val(x))
    '    Dim gc = IO.File.ReadAllLines("F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\circos\1329830.5.2.GCSkew.txt").Select(Function(x) Val(x))

    '    Dim repeats = "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\circos\repeat.csv".LoadCsv(Of Circos.TrackDatas.NtProps.Repeat)

    '    Dim nnnt = "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\circos\1329830.5.ED.Full_AT.txt".LoadDblArray

    '    Dim rMaps = New Circos.TrackDatas.Highlights.Repeat(repeats, nnnt)

    '    Call Circos.CircosAPI.AddPlotTrack(doc, New Plots.HighLight(rMaps))
    '    Call Circos.CircosAPI.AddPlotTrack(doc, New Plots.Histogram(New TrackDatas.NtProps.GCSkew(fa, 25, 250, True)))
    '    Call Circos.CircosAPI.AddPlotTrack(doc, New Plots.Histogram(New TrackDatas.NtProps.GCSkew(fa, 25, 250, True)))

    '    Dim ideo = doc.GetIdeogram

    '    Call Circos.CircosAPI.SetIdeogramWidth(ideo, 1)
    '    Call Circos.CircosAPI.SetIdeogramRadius(ideo, 0.25)

    '    Call CircosAPI.WriteData(doc, "F:\2015.12.26.vir_genome_sequencing\genome_annotations\1329830.5.ED\circos", False)
    'End Function

    Public Class tRNA
        <Column("#seq-name")> Public Property seqName As String
        Public Property start As Integer
        Public Property [end] As Integer
        Public Property strand As String
        <Column("tRNA-type")> Public Property tRNAType As String
        <Column("Anti-Codon")> Public Property AntiCodon As String
    End Class

End Module
