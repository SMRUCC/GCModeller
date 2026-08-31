#Region "Microsoft.VisualBasic::1dee7d92cb05052454408d85ecb4e7b4, analysis\SequenceToolkit\SequenceAlignment\test\siRNADemo.vb"

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

    '   Total Lines: 97
    '    Code Lines: 68 (70.10%)
    ' Comment Lines: 10 (10.31%)
    '    - Xml Docs: 10.00%
    ' 
    '   Blank Lines: 19 (19.59%)
    '     File Size: 4.71 KB


    ' Module siRNADemo
    ' 
    '     Function: FA, pass
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Diagnostics
Imports SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit
Imports SMRUCC.genomics.SequenceModel.FASTA

Module siRNADemo

    ' miRNA 序列（21 nt）
    Const MIR As String = "UGACGUGACUGACGUGACUGA"

    ' 候选 mRNA 靶标集（T1~T7）
    ReadOnly targets As (id As String, seq As String)() = {
        ("T1_perfect_match", "AUGGCAUCGGAUCCUCAGUCACGUCAGUCACGUCAAGGCUUAAGCCAU"),
        ("T2_3prime_mismatch", "AUGGCAUCGGAUCCUCAAUCACGUCAGUCACGUCAAGGCUUAAGCCAU"),
        ("T3_core_GU_pair", "AUGGCAUCGGAUCCUCAGUCACGUCAGUCACGUUAAGGCUUAAGCCAU"),
        ("T4_core_mismatch", "AUGGCAUCGGAUCCUCAGUCACGUCAGUCAAGUCAAGGCUUAAGCCAU"),
        ("T5_cleavage_mismatch", "AUGGCAUCGGAUCCUCAGUCACGUAAGUCACGUCAAGGCUUAAGCCAU"),
        ("T6_multiple_mismatch", "AUGGCAUCGGAUCCUCCGUCCCGUCAGUAACGCCAAGGCUUAAGCCAU"),
        ("T7_bulge", "AUGGCAUCGGAUCCUCAGUCACGUCAGUCAUCGUCAAGGCUUAAGCCAU")
    }

    ''' <summary>构造 FastaSeq 对象（复用 DiamondDemo 中的范式）。</summary>
    Function FA(header As String, seq As String) As FastaSeq
        Return New FastaSeq(New String() {">" & header}, seq)
    End Function

    Function pass(cond As Boolean, msg As String) As Boolean
        Console.WriteLine($"  [{(If(cond, "PASS", "FAIL"))}] {msg}")
        Return cond
    End Function

    Const BLASTN_OUT As String = "N:\project\20260831-miRNA\RM271-miRNA-blastn.txt"

    ''' <summary>
    ''' 第一轮 blastn 预筛 + 第二轮 psRNATarget 风格打分过滤的完整链路验证。
    ''' </summary>
    Sub blastnFilterTest()
        Console.WriteLine("=== blastn pre-filter -> psRNATarget scoring ===")

        If Not IO.File.Exists(BLASTN_OUT) Then
            Console.WriteLine($"  [SKIP] blastn output not found: {BLASTN_OUT}")
            Return
        End If

        Dim blastn = BlastnMapTable.Parse(BLASTN_OUT.Open(IO.FileMode.Open, doClear:=False, [readOnly]:=True)).ToArray
        Dim sw As Stopwatch = Stopwatch.StartNew
        Dim result = BlastFilterMiRNATargets.BlastnFilter(blastn).ToArray

        sw.Stop()

        Console.WriteLine($"parsed HSP      : {blastn.Length}")
        Console.WriteLine($"target hits     : {result.Length}")
        Console.WriteLine($"elapsed         : {sw.ElapsedMilliseconds} ms")
        Console.WriteLine($"distinct miRNA  : {result.Select(Function(h) h.miRNA).Distinct.Count}")
        Console.WriteLine($"distinct targets: {result.Select(Function(h) h.Target).Distinct.Count}")
        Console.WriteLine()

        Console.WriteLine("--- Top 10 hits (sorted by expectation, lower is better) ---")

        For Each h In result.OrderBy(Function(x) x.Expectation).ThenBy(Function(x) x.Target).Take(10)
            Console.WriteLine($"  {h.ToString()}  TI={h.TranslationInhibition}")
        Next

        Console.WriteLine()

        Dim ok As Boolean = True

        Console.WriteLine("=== Assertions ===")
        ok = pass(result.Length > 0, "blastn filter should produce non-empty hits") AndAlso ok
        ok = pass(result.All(Function(h) h.Expectation <= 5.0), "every hit expectation should be <= 5.0") AndAlso ok
        ok = pass(result.All(Function(h) h.EndSite >= h.StartSite), "site coords should be normalized to mRNA sense strand") AndAlso ok
        ok = pass(result.All(Function(h) h.Length > 0), "site length should be positive") AndAlso ok
        ok = pass(result.Any(Function(h) h.Expectation = 0.0), "should exist at least one perfect-match target (expectation = 0)") AndAlso ok

        Console.WriteLine()
        Console.WriteLine(If(ok, "ALL ASSERTIONS PASSED", "SOME ASSERTIONS FAILED"))
        Console.WriteLine()
    End Sub

    Sub ssearchTest()
        Console.WriteLine("=== siRNA target prediction demo (miR-Demo1) ===")
        Console.WriteLine($"miRNA: {MIR}  (reverse-complement target site)")
        Console.WriteLine($"miRNA rev-comp: {MIR.ReverseComplementRNA()}")
        Console.WriteLine()

        Dim mirna As FastaSeq = FA("miR-Demo1", MIR)
        Dim db As FastaSeq() = targets _
            .Select(Function(t) FA(t.id, t.seq)) _
            .ToArray

        Console.WriteLine($"miRNA id = {mirna.Title}, first target id = {db(0).Title}")

        ' ---- 运行两款算法 ----
        Dim psrna As New psRNATarget() With {.Version = psRNATarget.Schema.V2_2017, .MaxExpectation = 5.0}
        Dim tf As New TargetFinder() With {.ScoreCutoff = 5.0}

        ' Run 返回 IEnumerable（Iterator），必须显式物化成 List
        Dim psrnaHits As List(Of siRNAHit) = psrna.Run(mirna, db).ToList
        Dim tfHits As List(Of siRNAHit) = tf.Run(mirna, db).ToList

        Console.WriteLine("--- psRNATarget hits ---")
        For Each h In psrnaHits
            Console.WriteLine($"  {h.ToString()}  TI={h.TranslationInhibition}")
        Next
        Console.WriteLine("--- TargetFinder hits ---")
        For Each h In tfHits
            Console.WriteLine($"  {h.ToString()}  TI={h.TranslationInhibition}")
        Next
        Console.WriteLine()

        ' ---- 交集（高置信靶标）----
        Dim merger As New Intersection() With {.SiteTolerance = 3}
        Dim intersect As List(Of siRNAHit) = merger.Merge(psrnaHits, tfHits).ToList

        Console.WriteLine("--- High-confidence intersection (psRNATarget ∩ TargetFinder) ---")
        For Each h In intersect
            Console.WriteLine($"  {h.ToString()}  TI={h.TranslationInhibition}")
        Next
        Console.WriteLine()

        ' ---- 断言 ----
        Dim ok As Boolean = True

        Console.WriteLine("=== Assertions ===")
        ' T1 必须在交集中
        Dim t1inIntersect = intersect.Any(Function(h) h.Target = "T1_perfect_match")
        ok = pass(t1inIntersect, "T1_perfect_match should be in intersection") AndAlso ok

        ' T6 必须被过滤（不在任一算法的命中集中）
        Dim t6inPs = psrnaHits.Any(Function(h) h.Target = "T6_multiple_mismatch")
        Dim t6inTf = tfHits.Any(Function(h) h.Target = "T6_multiple_mismatch")
        ok = pass(Not t6inPs AndAlso Not t6inTf, "T6_multiple_mismatch should be filtered out by both algorithms") AndAlso ok

        ' T2~T5 至少被一款算法命中（宽松验证）
        For Each id In {"T2_3prime_mismatch", "T3_core_GU_pair", "T4_core_mismatch", "T5_cleavage_mismatch"}
            Dim hitBy = psrnaHits.Any(Function(h) h.Target = id) OrElse tfHits.Any(Function(h) h.Target = id)
            ok = pass(hitBy, $"{id} should be detected by at least one algorithm") AndAlso ok
        Next

        ' T7_bulge 是靶位点侧的 1 nt 插入，只有带 gap 的比对才能召回。
        ' 而 RNASeqHelper.BestLocalHit 走的是 LocalHSPMatch，其 seq1/seq2 都是 query/subject
        ' 的**原始切片**（见 LocalHSPMatch.New 中的 seq1.Skip(fromA).Take(toA-fromA)），
        ' 不携带 gap 字符，因此 bulge 会退化成向右移位的连续错配而被过滤掉。
        ' 这是既有 SW 封装的已知限制（与本次 blastn 流程修复无关），此处断言其当前行为，
        ' 以便日后引入带 gap 比对时该断言会失败并提醒同步更新。
        Dim t7detected = psrnaHits.Any(Function(h) h.Target = "T7_bulge") OrElse tfHits.Any(Function(h) h.Target = "T7_bulge")
        ok = pass(Not t7detected, "T7_bulge (target-side insertion) is NOT recallable under the gapless Smith-Waterman HSP - known limitation") AndAlso ok

        ' T5 切割位点错配应标记为翻译抑制候选（任一算法）
        Dim t5ti = psrnaHits.Any(Function(h) h.Target = "T5_cleavage_mismatch" AndAlso h.TranslationInhibition) _
               OrElse tfHits.Any(Function(h) h.Target = "T5_cleavage_mismatch" AndAlso h.TranslationInhibition)
        ok = pass(t5ti, "T5_cleavage_mismatch should be flagged as translation inhibition candidate") AndAlso ok

        Console.WriteLine()
        Console.WriteLine(If(ok, "ALL ASSERTIONS PASSED", "SOME ASSERTIONS FAILED"))
    End Sub

    Sub Main()
        Call blastnFilterTest()
        ' Call ssearchTest()
    End Sub
End Module

