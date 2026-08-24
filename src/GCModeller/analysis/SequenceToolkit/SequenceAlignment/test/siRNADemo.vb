Imports SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit
Imports SMRUCC.genomics.SequenceModel.FASTA

Module siRNADemo

    ' miRNA 序列（21 nt）
    Const MIR As String = "UGACGUGACUGACGUGACUGA"

    ' 候选 mRNA 靶标集（T1~T7）
    Shared ReadOnly targets As (id As String, seq As String)() = {
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

    Sub Main()
        Console.WriteLine("=== siRNA target prediction demo (miR-Demo1) ===")
        Console.WriteLine($"miRNA: {MIR}  (reverse-complement target site)")
        Console.WriteLine($"miRNA rev-comp: {MIR.ReverseComplementRNA()}")
        Console.WriteLine()

        Dim mirna As FastaSeq = FA("miR-Demo1", MIR)
        Dim db As FastaSeq() = targets _
            .Select(Function(t) FA(t.id, t.seq)) _
            .ToArray

        ' ---- 运行两款算法 ----
        Dim psrna As New psRNATarget() With {.Version = psRNATarget.Schema.V2_2017, .MaxExpectation = 5.0}
        Dim tf As New TargetFinder() With {.ScoreCutoff = 5.0}

        Dim psrnaHits As List(Of siRNAHit) = psrna.Run(mirna, db)
        Dim tfHits As List(Of siRNAHit) = tf.Run(mirna, db)

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
        Dim intersect As List(Of siRNAHit) = merger.Merge(psrnaHits, tfHits)

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

        ' T2~T5, T7 至少被一款算法命中（宽松验证）
        For Each id In {"T2_3prime_mismatch", "T3_core_GU_pair", "T4_core_mismatch", "T5_cleavage_mismatch", "T7_bulge"}
            Dim hitBy = psrnaHits.Any(Function(h) h.Target = id) OrElse tfHits.Any(Function(h) h.Target = id)
            ok = pass(hitBy, $"{id} should be detected by at least one algorithm") AndAlso ok
        Next

        ' T5 切割位点错配应标记为翻译抑制候选（任一算法）
        Dim t5ti = psrnaHits.Any(Function(h) h.Target = "T5_cleavage_mismatch" AndAlso h.TranslationInhibition) _
               OrElse tfHits.Any(Function(h) h.Target = "T5_cleavage_mismatch" AndAlso h.TranslationInhibition)
        ok = pass(t5ti, "T5_cleavage_mismatch should be flagged as translation inhibition candidate") AndAlso ok

        Console.WriteLine()
        Console.WriteLine(If(ok, "ALL ASSERTIONS PASSED", "SOME ASSERTIONS FAILED"))
    End Sub
End Module
