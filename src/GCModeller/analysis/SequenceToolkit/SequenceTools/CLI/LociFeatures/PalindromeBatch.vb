#Region "Microsoft.VisualBasic::dadfaba5e7034f6efabc3d3fd7c3c6ce, analysis\SequenceToolkit\SequenceTools\CLI\LociFeatures\PalindromeBatch.vb"

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


    ' Code Statistics:

    '   Total Lines: 151
    '    Code Lines: 119 (78.81%)
    ' Comment Lines: 6 (3.97%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 26 (17.22%)
    '     File Size: 7.93 KB


    ' Module Utilities
    ' 
    '     Function: CheckHeaders, PalindromeBatchTask, PalindromeWorkflow
    ' 
    ' /********************************************************************************/

#End Region

Imports Darwinism.HPC.Parallel.ThreadTask
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Partial Module Utilities

    <ExportAPI("/check.attrs", Usage:="/check.attrs /in <in.fasta> /n <attrs.count> [/all]")>
    <Group(CLIGrouping.PalindromeBatchTaskTools)>
    Public Function CheckHeaders(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim n As Integer = args("/n")
        Dim all As Boolean = args("/all")

        For Each fa As FastaSeq In New FastaFile([in])
            If fa.Headers.Length <> n OrElse fa.ToString.Length > 220 Then
                Call Console.WriteLine(fa.ToString)
                If Not all Then  ' 默认是停止于遇到的第一条序列
                    Exit For
                End If
            End If
        Next

        Call Console.WriteLine("DONE!")

        Return 0
    End Function

    <ExportAPI("/Palindrome.BatchTask",
               Usage:="/Palindrome.BatchTask /in <in.DIR> [/num_threads 4 /min 3 /max 20 /min-appears 2 /cutoff <0.6> /Palindrome /max-dist <1000 (bp)> /partitions <-1> /out <out.DIR>]")>
    <ArgumentAttribute("/Palindrome", True, Description:="Only search for Palindrome, not includes the repeats data.")>
    <Group(CLIGrouping.PalindromeBatchTaskTools)>
    Public Function PalindromeBatchTask(args As CommandLine) As Integer
        Dim inDIR As String = args - "/in"
        Dim min As Integer = args.GetValue("/min", 3)
        Dim max As Integer = args.GetValue("/max", 20)
        Dim out As String = args.GetValue("/out", inDIR.TrimDIR & $"-{min},{max}.Palindrome.Workflow/")
        Dim files As IEnumerable(Of String) = ls - l - r - wildcards("*.fasta", "*.fa", "*.fsa", "*.fna") <= inDIR
        Dim api As String = GetType(Utilities).API(NameOf(PalindromeWorkflow))
        Dim n As Integer = LQuerySchedule.AutoConfig(args.GetValue("/num_threads", 4))
        Dim cutoff As Double = args.GetValue("/cutoff", 0.6)
        Dim maxDist As Integer = args.GetValue("/max-dist", 1000)
        Dim parts As Integer = args.GetValue("/partitions", -1)
        Dim minAp As Integer = args.GetValue("/min-appears", 2)
        Dim onlyPalindrome As String = If(args("/Palindrome"), "/Palindrome", "")
        Dim task As Func(Of String, String) =
            Function(fa) _
                $"{api} /in {fa.CLIPath} /min {min} /max {max} /min-appears {minAp} /out {out.CLIPath} /cutoff {cutoff} /max-dist {maxDist} /partitions {parts} /batch {onlyPalindrome}"
        Dim CLI As String() = files.Select(task).ToArray

        Return BatchTasks.SelfFolks(CLI, parallel:=n)
    End Function

    ''' <summary>
    ''' 这个函数会同时保存Raw数据和经过了转换的<see cref="SimpleSegment"/>数据
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Palindrome.Workflow",
               Usage:="/Palindrome.Workflow /in <in.fasta> [/batch /min-appears 2 /min 3 /max 20 /cutoff <0.6> /max-dist <1000 (bp)> /Palindrome /partitions <-1> /out <out.DIR>]")>
    <ArgumentAttribute("/in", False,
                   Description:="This is a single sequence fasta file.")>
    <ArgumentAttribute("/Palindrome", True, Description:="Only search for Palindrome, not includes the repeats data.")>
    <Group(CLIGrouping.PalindromeBatchTaskTools)>
    Public Function PalindromeWorkflow(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim min As Integer = args.GetValue("/min", 3)
        Dim max As Integer = args.GetValue("/max", 20)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Palindrome.Workflow/")
        Dim isBatch As Boolean = args("/batch") ' 批量和单独的模式相比，差异只是在保存结果的时候的位置
        Dim nt As New FASTA.FastaSeq([in])
        Dim minAp As Integer = args.GetValue("/min-appears", 2)

        Dim mirrorPalindrome As PalindromeLoci() = Topologically.SearchMirrorPalindrome(nt, min, max)   ' 镜像回文

        Dim palindrome = Topologically.SearchPalindrome(nt, min, max)  ' 简单回文

        Dim cutoff As Double = args.GetValue("/cutoff", 0.6)
        Dim maxDist As Integer = args.GetValue("/max-dist", 1000)
        Dim parts As Integer = args.GetValue("/partitions", -1)
        Dim imPalSearch As New Topologically.Imperfect(nt, min, max, cutoff, maxDist, parts)
        Call imPalSearch.DoSearch()
        Dim imperfectPalindrome As Topologically.ImperfectPalindrome() = imPalSearch.ResultSet   ' 非完全回文

        Dim MirrorLocis = mirrorPalindrome.ToLocis
        Dim palindromeLocis = palindrome.ToLocis
        Dim imPalLocis = imperfectPalindrome.ToLocis
        Dim name As String = [in].BaseName

        If isBatch Then
            Call mirrorPalindrome.SaveTo(out & $"/MirrorPalindrome/{name}.Csv")

            Call palindrome.SaveTo(out & $"/Palindrome/{name}.Csv")
            Call imperfectPalindrome.SaveTo(out & $"/ImperfectPalindrome/{name}.csv")

            Call MirrorLocis.SaveTo(out & $"/Sites-MirrorPalindrome/{name}.csv")

            Call palindromeLocis.SaveTo(out & $"/Sites-Palindrome/{name}.Csv")
            Call imPalLocis.SaveTo(out & $"/Sites-ImperfectPalindrome/{name}.csv")
        Else
            ' 保存在同一个文件夹之中
            Dim prefix As String = out & "/" & name

            Call mirrorPalindrome.SaveTo(prefix & ".MirrorPalindrome.Csv")

            Call palindrome.SaveTo(prefix & ".Palindrome.Csv")
            Call imperfectPalindrome.SaveTo(prefix & ".ImperfectPalindrome.csv")

            Call MirrorLocis.SaveTo(prefix & ".Sites-MirrorPalindrome.csv")

            Call palindromeLocis.SaveTo(prefix & ".Sites-Palindrome.Csv")
            Call imPalLocis.SaveTo(prefix & ".Sites-ImperfectPalindrome.csv")
        End If

        If Not args("/Palindrome") Then
            Dim repeats As Topologically.Repeats() = RepeatsSearchAPI.SearchRepeats(nt, min, max, minAp) ' 简单重复
            Dim rev As ReverseRepeats() = RepeatsSearchAPI.SearchReversedRepeats(nt, min, max, minAp) ' 反向重复

            Dim repeatsViews = RepeatsView.TrimView(Topologically.Repeats.CreateDocument(repeats)).Trim(min, max, minAp)  ' 简单重复
            Dim revViews = ReverseRepeatsView.TrimView(rev).Trim(min, max, minAp)     ' 反向重复

            Dim RepeatLocis = repeats.ToLocis
            Dim revRepeatlocis = rev.ToLocis

            If isBatch Then
                Call repeatsViews.SaveTo(out & $"/SimpleRepeats/{name}.Csv")
                Call revViews.SaveTo(out & $"/ReversedRepeats/{name}.Csv")
                Call RepeatLocis.SaveTo(out & $"/Sites-SimpleRepeats/{name}.Csv")
                Call revRepeatlocis.SaveTo(out & $"/Sites-ReversedRepeats/{name}.Csv")
            Else
                Dim prefix As String = out & "/" & name

                Call repeatsViews.SaveTo(prefix & ".SimpleRepeats.Csv")
                Call revViews.SaveTo(prefix & ".ReversedRepeats.Csv")
                Call RepeatLocis.SaveTo(prefix & ".Sites-SimpleRepeats.Csv")
                Call revRepeatlocis.SaveTo(prefix & ".Sites-ReversedRepeats.Csv")
            End If
        End If

        Return 0
    End Function
End Module
