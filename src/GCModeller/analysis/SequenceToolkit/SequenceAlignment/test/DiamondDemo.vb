Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.Analysis.SequenceAlignment.DIAMOND
Imports SMRUCC.genomics.SequenceModel.FASTA

Module DiamondDemo

    ' 一组用于验证的蛋白序列(简化、无生物学来源,仅供算法自检)
    ' Q 与 R1 完全相同(高相似),R2 是 I/L/V 保守替换(远源同源应被缩减字母表命中),
    ' R3 为不相关随机序列(应不命中或低分)。
    Const Q As String = "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQAPILSRVGDGTQDNLSGAEKAVQVKVKALPDAQFEVVHSLAKWKRQTLGQHDFSAGEGLYTHMKALRPDEDRLSPLHSVYVDQWDWE"
    Const R1 As String = "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQAPILSRVGDGTQDNLSGAEKAVQVKVKALPDAQFEVVHSLAKWKRQTLGQHDFSAGEGLYTHMKALRPDEDRLSPLHSVYVDQWDWE"
    Const R2 As String = "MKTAYIAKQRQISFVKSHFSRQLEERLGLIEVQAPILSRVGDGTQDNLSGAEKAIQVKVKALPDAQFEVVHSLAKWKRQTLGQHDFSAGEGLYTHMKALRPDEDRLSPLHSVYVDQWDWE" ' L->I
    Const R3 As String = "WYTQKPLVGMNHCFDRSBEUIDLKJMNCQWPVNHTGRKSLJASHDFGQWIEURTYCXMVBNLKJHASDFGQWEPRTYUIOASDLKFJGHZ"

    Function FA(header As String, seq As String) As FastaSeq
        Return New FastaSeq(New String() {">" & header}, seq)
    End Function

    Sub Main()
        Dim query As FastaSeq = FA("query", Q)
        Dim db As FastaSeq() = {FA("R1_exact", R1), FA("R2_conserved", R2), FA("R3_random", R3)}

        For Each mode In New SensitivityMode() {SensitivityMode.Fast, SensitivityMode.Sensitive, SensitivityMode.VerySensitive, SensitivityMode.UltraSensitive}
            Console.WriteLine($"===== Sensitivity Mode: {mode} (seeds={SpacedSeeds.GetSeeds(mode).Length}) =====")
            Dim diamond As New DiamondBlastp(mode)
            Dim hits = diamond.Search(query, db).ToArray

            For Each h In hits
                Console.WriteLine($"  {h.SubjectTitle,-14} id={h.PercentIdentity:F1}% len={h.AlignmentLength} score={h.RawScore:F0} q[{h.QueryStart}-{h.QueryEnd}] s[{h.SubjectStart}-{h.SubjectEnd}]")
            Next

            If hits.Length = 0 Then
                Console.WriteLine("  (no hits)")
            End If
        Next

        ' 一致性核对:对 R1 运行朴素全 SW,确认 Diamond 能检出且 subject 为 R1
        Console.WriteLine(vbCrLf & "===== Consistency check vs naive SW (R1) =====")
        Dim naive As GSW(Of Char) = New SmithWaterman(Q, R1).BuildMatrix()
        Dim naiveMatches = naive.Matches(0).ToList
        Console.WriteLine($"Naive SW R1 best score = {naiveMatches(0).score:F0}, coords q[{naiveMatches(0).fromA}-{naiveMatches(0).toA}] s[{naiveMatches(0).fromB}-{naiveMatches(0).toB}]")

        Dim d As New DiamondBlastp(SensitivityMode.UltraSensitive)
        Dim dh = d.Search(query, db).ToArray
        Dim top = dh.FirstOrDefault(Function(x) x.SubjectTitle.Contains("R1_exact"))

        If top Is Nothing Then
            Console.WriteLine("FAIL: Diamond did not report R1 as a hit.")
        Else
            Console.WriteLine($"Diamond R1 score = {top.RawScore:F0}, coords q[{top.QueryStart}-{top.QueryEnd}] s[{top.SubjectStart}-{top.SubjectEnd}]")
            Console.WriteLine($"PASS: R1 detected, identity={top.PercentIdentity:F1}%")
        End If

        Console.WriteLine(vbCrLf & "DONE.")
    End Sub
End Module
