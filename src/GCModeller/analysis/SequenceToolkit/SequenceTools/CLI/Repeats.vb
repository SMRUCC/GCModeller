#Region "Microsoft.VisualBasic::1747a6a4009e8ab4f489c3b379d89664, ..\GCModeller\analysis\SequenceToolkit\SequenceTools\CLI\Repeats.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Partial Module Utilities

    <ExportAPI("Search.Batch",
               Info:="Batch search for repeats.",
               Usage:="Search.Batch /aln <alignment.fasta> [/min 3 /max 20 /min-rep 2 /out <./>]")>
    <Argument("/aln", False,
                   Description:="The input fasta file should be the output of the clustal multiple alignment fasta output.")>
    <Argument("/out", True, AcceptTypes:={GetType(RepeatsView), GetType(RevRepeatsView)})>
    <Group(CLIGrouping.RepeatsTools)>
    Public Function BatchSearch(args As CommandLine) As Integer
        Dim Mla As FastaFile = args.GetObject("/aln", AddressOf FastaFile.Read)
        Dim Min As Integer = args.GetValue("/min", 3)
        Dim Max As Integer = args.GetValue("/max", 20)
        Dim MinAppeared As Integer = args.GetValue("/min-rep", 2)
        Dim EXPORT As String = args.GetValue("/out", "./")

        Call Topologically.BatchSearch(Mla, Min, Max, MinAppeared, EXPORT)

        Return 0
    End Function

    <ExportAPI("Repeats.Density",
               Usage:="Repeats.Density /dir <dir> /size <size> /ref <refName> [/out <out.csv> /cutoff <default:=0>]")>
    <Group(CLIGrouping.RepeatsTools)>
    Public Function RepeatsDensity(args As CommandLine) As Integer
        Dim DIR As String = args("/dir")
        Dim size As Integer = args.GetInt32("/size")
        Dim out As String = args.GetValue("/out", DIR & "/Repeats.Density.vector.txt")
        Dim vector As Double() = Topologically.RepeatsDensity(DIR, size, ref:=args("/ref"), cutoff:=args.GetValue("/cutoff", 0R))
        Return vector.FlushAllLines(out).CLICode
    End Function

    <ExportAPI("rev-Repeats.Density", Usage:="rev-Repeats.Density /dir <dir> /size <size> /ref <refName> [/out <out.csv> /cutoff <default:=0>]")>
    <Group(CLIGrouping.RepeatsTools)>
    Public Function revRepeatsDensity(args As CommandLine) As Integer
        Dim DIR As String = args("/dir")
        Dim size As Integer = args.GetInt32("/size")
        Dim out As String = args.GetValue("/out", DIR & "/rev-Repeats.Density.vector.txt")
        Dim vector As Double() = Topologically.RevRepeatsDensity(DIR, size, ref:=args("/ref"), cutoff:=args.GetValue("/cutoff", 0R))
        Return vector.FlushAllLines(out).CLICode
    End Function

    <ExportAPI("/Write.Seeds",
               Usage:="/Write.Seeds /out <out.dat> [/prot /max <20>]")>
    <Group(CLIGrouping.RepeatsTools)>
    Public Function WriteSeeds(args As CommandLine) As Integer
        Dim isProt As Boolean = args.GetBoolean("/prot")
        Dim out As String = args("/out")
        Dim max As Integer = args.GetValue("/max", 20)
        Dim chars As Char() = If(isProt, ToChar.Values.Distinct.ToArray, {"A"c, "T"c, "G"c, "C"c})
        Dim seeds As SeedData = SeedData.Initialize(chars, max)

        Return seeds.Save(out)
    End Function
End Module
