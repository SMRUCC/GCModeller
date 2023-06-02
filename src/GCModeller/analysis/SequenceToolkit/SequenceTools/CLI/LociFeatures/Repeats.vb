#Region "Microsoft.VisualBasic::34fbce7e36348a9da72df2465a6aaab8, analysis\SequenceToolkit\SequenceTools\CLI\LociFeatures\Repeats.vb"

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

' Module Utilities
' 
'     Function: BatchSearch, RepeatsDensity, ScreenRepeats, SearchRepeats, SSRFinder
'               WriteSeeds
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Data
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Math.Distributions.BinBox
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Partial Module Utilities

    <ExportAPI("/Search.Repeats")>
    <Description("Search for repeats sequence loci sites.")>
    <Usage("/Search.Repeats /in <nt.fasta> [/min <default=3> /max <default=20> /minOccurs <default=3> /reverse /out <result.csv>]")>
    <Group(CLIGrouping.RepeatsTools)>
    Public Function SearchRepeats(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim min% = args("/min") Or 3
        Dim max% = args("/max") Or 20
        Dim minAp% = args("/minOccurs") Or 3
        Dim isReversed As Boolean = args("/reverse")
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.repeats{If(isReversed, "(reverse)", "")}.[{min},{max}],minOccurs={minAp}.csv"
        Dim nt As FastaSeq = FastaSeq.Load([in])

        If isReversed Then
            ' 反向重复
            Dim reversed As ReverseRepeats() = RepeatsSearchAPI.SearchReversedRepeats(nt, min, max, minAp)
            ' 反向重复
            Dim views = ReverseRepeatsView.TrimView(reversed).Trim(min, max, minAp)

            Return views.SaveTo(out).CLICode
        Else
            ' 简单重复
            Dim repeats As Topologically.Repeats() = RepeatsSearchAPI.SearchRepeats(nt, min, max, minAp)
            ' 简单重复
            Dim views = RepeatsView.TrimView(Topologically.Repeats.CreateDocument(repeats)).Trim(min, max, minAp)

            Return views.SaveTo(out).CLICode
        End If
    End Function

    ''' <summary>
    ''' 這個函數會將文件夾之中的文件都合并到一個文件之中
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Screen.sites",
               Usage:="/Screen.sites /in <DIR/sites.csv> /range <min_bp>,<max_bp> [/type <type,default:=RepeatsView,alt:RepeatsView,RevRepeatsView,PalindromeLoci,ImperfectPalindrome> /out <out.csv>]")>
    <ArgumentAttribute("/in", AcceptTypes:={
        GetType(RepeatsView),
        GetType(ReverseRepeatsView),
        GetType(PalindromeLoci),
        GetType(ImperfectPalindrome)
    })>
    <Group(CLIGrouping.RepeatsTools)>
    Public Function ScreenRepeats(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim range As String() = args("/range").Split(","c)
        Dim loci As New IntRange(Val(range(Scan0)), Val(range(1)))
        Dim type As String = args.GetValue("/type", "RepeatsView")
        Dim out As String = If(
            [in].FileExists,
            [in].TrimSuffix,
            [in].TrimDIR) & $"-range={args("/range")}.csv"

        out = args.GetValue("/out", out)

        If type.TextEquals(NameOf(RepeatsView)) Then
            Dim result As New List(Of RepeatsView)

            For Each part In loci.RangeSelects([in].RequestFiles(Of RepeatsView))
                For Each x In part.Value
                    x.Data.Add("seq", part.Name)
                Next

                result += part.Value
            Next

            Return result.SaveTo(out).CLICode

        ElseIf type.TextEquals(NameOf(ReverseRepeatsView)) Then
            Dim result As New List(Of ReverseRepeatsView)

            For Each part In loci.RangeSelects([in].RequestFiles(Of ReverseRepeatsView))
                For Each x In part.Value
                    x.Data.Add("seq", part.Name)
                Next

                result += part.Value
            Next

            Return result.SaveTo(out).CLICode

        ElseIf type.TextEquals(NameOf(PalindromeLoci)) Then
            Dim result As New List(Of PalindromeLoci)

            For Each part In loci.RangeSelects([in].RequestFiles(Of PalindromeLoci))
                For Each x In part.Value
                    x.Data.Add("seq", part.Name)
                Next

                result += part.Value
            Next

            Return result.SaveTo(out).CLICode

        ElseIf type.TextEquals(NameOf(ImperfectPalindrome)) Then
            Dim result As New List(Of ImperfectPalindrome)

            For Each part In loci.RangeSelects([in].RequestFiles(Of ImperfectPalindrome))
                For Each x In part.Value
                    x.Data.Add("seq", part.Name)
                Next

                result += part.Value
            Next

            Return result.SaveTo(out).CLICode

        Else
            Throw New Exception("Type is invalid: " & type)
        End If
    End Function

    <ExportAPI("Search.Batch",
               Info:="Batch search for repeats.",
               Usage:="Search.Batch /aln <alignment.fasta> [/min 3 /max 20 /min-rep 2 /out <./>]")>
    <ArgumentAttribute("/aln", False,
                   Description:="The input fasta file should be the output of the clustal multiple alignment fasta output.")>
    <ArgumentAttribute("/out", True, AcceptTypes:={GetType(RepeatsView), GetType(ReverseRepeatsView)})>
    <Group(CLIGrouping.RepeatsTools)>
    Public Function BatchSearch(args As CommandLine) As Integer
        Dim Mla As FastaFile = args.GetObject(Of FastaFile)("/aln", AddressOf FastaFile.Read)
        Dim Min As Integer = args.GetValue("/min", 3)
        Dim Max As Integer = args.GetValue("/max", 20)
        Dim MinAppeared As Integer = args.GetValue("/min-rep", 2)
        Dim EXPORT As String = args.GetValue("/out", "./")

        Call Topologically.BatchSearch(Mla, Min, Max, MinAppeared, EXPORT)

        Return 0
    End Function

    ''' <summary>
    ''' 统计位点在目标序列上面的分布密度
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/loci.Density")>
    <Description("Do statistics of the loci density on a specific sequence.")>
    <Usage("/loci.Density /locis <data.csv> [/left <default=Start> /size <size> /win_size <default=100> /offset <default=1000> /out <result.txt>]")>
    <Group(CLIGrouping.RepeatsTools)>
    Public Function RepeatsDensity(args As CommandLine) As Integer
        Dim in$ = args <= "/locis"
        Dim leftFieldName$ = args("/left") Or "Start"
        Dim data = EntityObject.LoadDataSet([in]) _
            .Select(Function(d) d(leftFieldName).ParseInteger) _
            .OrderBy(Function(x) x) _
            .ToArray
        Dim size As Integer = args("/size") Or data.Max
        Dim winSize = args("/win_size") Or 100
        Dim offset = args("/offset") Or 1000
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.density.txt"
        Dim density As Double() = CutBins _
            .FixedWidthBins(Of Integer)(data, winSize, Function(x) x, 1, size, offset) _
            .Select(Function(bin) CDbl(bin.Count)) _
            .ToArray

        Return density.FlushAllLines(out).CLICode
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

    <ExportAPI("/SSR")>
    <Description("Search for SSR on a nt sequence.")>
    <Usage("/SSR /in <nt.fasta> [/range <default=2,6> /parallel /out <out.csv/DIR>]")>
    <Group(CLIGrouping.RepeatsTools)>
    Public Function SSRFinder(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim range$ = args("/range") Or "2,6"
        Dim out$ = args("/out") Or ([in].TrimSuffix & ".SSR.csv")

        SSRSearch.Parallel = args.IsTrue("/parallel")

        If FastaFile.SingleSequence([in]) Then
            Dim nt As FastaSeq = FastaSeq.LoadNucleotideData([in])

            If nt.IsProtSource Then
                Throw New InvalidExpressionException([in])
            End If

            Return nt _
                .SSR(range) _
                .ToArray _
                .SaveTo(out) _
                .CLICode
        Else
            For Each nt As FastaSeq In New StreamIterator([in]).ReadStream
                Dim path$ = out.TrimSuffix & $"/{nt.Title.NormalizePathString}.csv"

                If path.FileLength > 0 Then
                    Console.Write(".")
                    Continue For
                Else
                    Call nt.Title.__INFO_ECHO
                End If

                Try
                    Dim SSR As SSR() = nt _
                        .SSR(range) _
                        .ToArray

                    Call SSR.SaveTo(path)
                Catch ex As Exception
                    Call ex.PrintException
                End Try
            Next

            Return 0
        End If
    End Function
End Module
