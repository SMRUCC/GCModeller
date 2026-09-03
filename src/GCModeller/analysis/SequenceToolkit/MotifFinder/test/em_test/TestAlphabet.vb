' ============================================================================
' TestAlphabet.vb — 字母表：编码 / 歧义字符 / 反向互补 / 序列类型识别
' ----------------------------------------------------------------------------
' 对应 [em.md §1] 字母表与背景模型、[em.md §9] −revcomp 双链扫描。
' 每条断言标注其针对的 [缺陷 #n]。
' ============================================================================

Option Strict On

Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.EmMotif.Core
Imports SMRUCC.genomics.SequenceModel

Namespace EmMotif

    Public Module TestAlphabet

        Public Sub RunAll()
            TestAssert.Section("字母表：编码与歧义字符 [em.md §1]")
            TestEncodeDecode()
            TestUracilMapping()
            TestAmbiguous()
            TestComplement()
            TestRevcomp()
            TestProteinAlphabet()
            TestSequenceTypeBranch()
            TestEmptyAndEdge()
        End Sub

        ''' <summary>编解码往返</summary>
        Private Sub TestEncodeDecode()
            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim seq = "ACGTTGCA"
            Dim enc = alpha.Encode(seq)

            TestAssert.CheckEqual(enc.Length, seq.Length, "编码长度 = 序列长度")
            Dim roundTrip As Boolean = True
            For i = 0 To seq.Length - 1
                If alpha.Decode(enc(i)) <> seq(i).ToString() Then roundTrip = False
            Next
            TestAssert.Check(roundTrip, "DNA 编解码往返一致")
            TestAssert.CheckEqual(alpha.Size, 4, "DNA 字母表大小 = 4")

            ' 大小写不敏感
            Dim lower = alpha.Encode("acgttgca")
            Dim same As Boolean = True
            For i = 0 To lower.Length - 1
                If lower(i) <> enc(i) Then same = False
            Next
            TestAssert.Check(same, "编码对小写字母不敏感")
        End Sub

        ''' <summary>[缺陷 #5] U 必须并入 T（Letters="ACGT" 中 T 的索引是 3）</summary>
        Private Sub TestUracilMapping()
            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim idxT = alpha.EncodeChar("T"c)
            Dim idxU = alpha.EncodeChar("U"c)

            TestAssert.CheckEqual(idxT, 3, "T 的索引 = 3（Letters=""ACGT""）")
            TestAssert.CheckEqual(idxU, idxT, "U 并入 T，两者索引相同 [缺陷 #5]")
            TestAssert.Check(idxU <> alpha.EncodeChar("C"c), "U 不得被编码成 C（索引 1）[缺陷 #5]")

            ' 端到端：一条 RNA 序列与对应 DNA 序列编码结果一致
            Dim rna = alpha.Encode("ACGUUGCA")
            Dim dna = alpha.Encode("ACGTTGCA")
            Dim same As Boolean = True
            For i = 0 To rna.Length - 1
                If rna(i) <> dna(i) Then same = False
            Next
            TestAssert.Check(same, "RNA 序列（含 U）与对应 DNA 序列编码一致 [缺陷 #5]")
        End Sub

        ''' <summary>歧义字符 → −1（不参与 E 步，与 MEME 行为一致）</summary>
        Private Sub TestAmbiguous()
            Dim alpha As New Alphabet(SeqTypes.DNA)
            For Each c In "NRYSWKMBDHV"
                TestAssert.CheckEqual(alpha.EncodeChar(c), -1, $"核酸歧义字符 {c} → −1")
            Next

            Dim enc = alpha.Encode("ACGTNACGT")
            TestAssert.CheckEqual(enc(4), -1, "序列中的 N 编码为 −1")
            Dim validCount = 0
            For Each a In enc
                If a >= 0 Then validCount += 1
            Next
            TestAssert.CheckEqual(validCount, 8, "有效字母计数正确（9 − 1 个 N）")

            Dim pa As New Alphabet(SeqTypes.Protein)
            For Each c In "BXZUO"
                TestAssert.CheckEqual(pa.EncodeChar(c), -1, $"蛋白歧义字符 {c} → −1")
            Next
        End Sub

        ''' <summary>[缺陷 #6] 互补映射与 −1 的安全性</summary>
        Private Sub TestComplement()
            Dim alpha As New Alphabet(SeqTypes.DNA)
            TestAssert.CheckEqual(alpha.Complement(alpha.EncodeChar("A"c)), alpha.EncodeChar("T"c), "A ↔ T")
            TestAssert.CheckEqual(alpha.Complement(alpha.EncodeChar("T"c)), alpha.EncodeChar("A"c), "T ↔ A")
            TestAssert.CheckEqual(alpha.Complement(alpha.EncodeChar("C"c)), alpha.EncodeChar("G"c), "C ↔ G")
            TestAssert.CheckEqual(alpha.Complement(alpha.EncodeChar("G"c)), alpha.EncodeChar("C"c), "G ↔ C")

            ' 歧义字符的互补必须返回 −1 而不是越界崩溃：
            ' 多 motif 屏蔽会把已发现位点置为 −1，配合 −−revcomp 必然走到这里 [缺陷 #6]
            Dim noThrow As Boolean = True
            Dim value As Integer = 0
            Try
                value = alpha.Complement(-1)
            Catch ex As Exception
                noThrow = False
                Console.WriteLine($"         Complement(−1) 抛出 {ex.GetType().Name}: {ex.Message}")
            End Try
            TestAssert.Check(noThrow, "Complement(−1) 不抛异常 [缺陷 #6]")
            TestAssert.CheckEqual(value, -1, "Complement(−1) = −1（歧义仍是歧义）[缺陷 #6]")
        End Sub

        ''' <summary>反向互补（字符串形式，用于输出）</summary>
        Private Sub TestRevcomp()
            Dim alpha As New Alphabet(SeqTypes.DNA)
            TestAssert.CheckEqual(alpha.Revcomp("ACGTTGCA"), "TGCAACGT", "Revcomp 基本用例")
            TestAssert.CheckEqual(alpha.Revcomp("A"), "T", "Revcomp 单字符")
            TestAssert.CheckEqual(alpha.Revcomp(""), "", "Revcomp 空串")
            TestAssert.Check(alpha.Revcomp(alpha.Revcomp("ACGTTGCA")) = "ACGTTGCA", "Revcomp 两次 = 原串")

            ' 与测试内独立实现交叉验证
            Dim rng = TestData.MakeRng(2024)
            Dim ok As Boolean = True
            For t = 0 To 49
                Dim sb As New System.Text.StringBuilder()
                For i = 0 To 19
                    sb.Append(TestData.DnaLetters(rng.Next(4)))
                Next
                Dim s = sb.ToString()
                If alpha.Revcomp(s) <> TestData.RevcompOf(s) Then ok = False
            Next
            TestAssert.Check(ok, "Revcomp 与独立实现在 50 条随机序列上一致")

            ' 蛋白字母表不支持双链
            Dim pa As New Alphabet(SeqTypes.Protein)
            TestAssert.Check(Not pa.SupportsRevcomp, "蛋白字母表不支持 revcomp [em.md §9]")
            TestAssert.CheckEqual(pa.Revcomp("ACDE"), "ACDE", "蛋白 Revcomp 返回原串")
        End Sub

        ''' <summary>蛋白字母表</summary>
        Private Sub TestProteinAlphabet()
            Dim pa As New Alphabet(SeqTypes.Protein)
            TestAssert.CheckEqual(pa.Size, 20, "蛋白字母表大小 = 20")
            TestAssert.CheckEqual(pa.Letters, TestData.ProteinLetters, "蛋白字母表内容 = 20 种标准氨基酸")

            Dim enc = pa.Encode("ACDEFGHIKLMNPQRSTVWY")
            Dim ok As Boolean = True
            For i = 0 To 19
                If enc(i) <> i Then ok = False
            Next
            TestAssert.Check(ok, "蛋白 20 个字母依次编码为 0..19")
        End Sub

        ''' <summary>[缺陷 #12] SeqTypes 分支：RNA 属核酸语义，Unknown 必须显式失败</summary>
        Private Sub TestSequenceTypeBranch()
            Dim rna As New Alphabet(SeqTypes.RNA)
            TestAssert.CheckEqual(rna.Size, 4, "RNA 走核酸字母表（大小 = 4）[缺陷 #12]")
            TestAssert.CheckEqual(rna.EncodeChar("U"c), rna.EncodeChar("T"c), "RNA 的 U 与 T 同索引 [缺陷 #12]")
            TestAssert.Check(rna.SupportsRevcomp, "RNA 支持 revcomp [缺陷 #12]")

            ' 无法识别的序列类型不应被静默当成蛋白质
            Dim threw As Boolean = False
            Try
                Dim bad As New Alphabet(SeqTypes.Unknown)
                Console.WriteLine($"         Unknown 未抛异常，得到字母表大小 {bad.Size}")
            Catch ex As ArgumentException
                threw = True
            End Try
            TestAssert.Check(threw, "SeqTypes.Unknown 构造时抛出 ArgumentException [缺陷 #12]")
        End Sub

        ''' <summary>[缺陷 #13] 空序列与边界输入</summary>
        Private Sub TestEmptyAndEdge()
            Dim alpha As New Alphabet(SeqTypes.DNA)

            Dim emptyOk As Boolean = True
            Dim emptyLen As Integer = -1
            Try
                emptyLen = alpha.Encode("").Length
            Catch ex As Exception
                emptyOk = False
                Console.WriteLine($"         Encode("""") 抛出 {ex.GetType().Name}: {ex.Message}")
            End Try
            TestAssert.Check(emptyOk, "空序列 Encode 不抛异常 [缺陷 #13]")
            TestAssert.CheckEqual(emptyLen, 0, "空序列编码结果长度 = 0 [缺陷 #13]")

            ' 全歧义序列：全部 −1
            Dim allN = alpha.Encode("NNNN")
            Dim allNeg As Boolean = True
            For Each a In allN
                If a >= 0 Then allNeg = False
            Next
            TestAssert.Check(allNeg, "全歧义序列全部编码为 −1")

            ' 歧义索引的解码不应崩溃
            Dim decodeOk As Boolean = True
            Try
                Dim s = alpha.Decode(-1)
            Catch ex As Exception
                decodeOk = False
                Console.WriteLine($"         Decode(−1) 抛出 {ex.GetType().Name}: {ex.Message}")
            End Try
            TestAssert.Check(decodeOk, "Decode(−1) 不抛异常 [缺陷 #13]")
        End Sub

    End Module

End Namespace
