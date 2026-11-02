Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace test.Tests

    ''' <summary>
    ''' 联合比对结构、列计数与配对 MSA 构建的测试。
    ''' </summary>
    Module Alignment

        Public Sub Run()
            Call JointAlignmentChecks()
            Call ColumnCountChecks()
            Call SwapInvarianceChecks()
            Call PairedBuilderChecks()
            Call AlphabetChecks()
        End Sub

        Private Sub JointAlignmentChecks()
            Dim alignment As JointAlignment = Fixtures.FromRows({"ACD-E", "ACD-E", "ACDGE"}, 3)

            Call Support.Same(3, alignment.Rows, "the joint alignment must keep every row")
            Call Support.Same(3, alignment.LengthA, "the family A block length")
            Call Support.Same(2, alignment.LengthB, "the family B block length")
            Call Support.True(alignment.IsBlockA(2), "column 2 must belong to family A")
            Call Support.True(Not alignment.IsBlockA(3), "column 3 must belong to family B")

            Dim crossPairs As Integer = 0

            For Each pair In alignment.CrossBlockPairs()
                Call Support.True(alignment.IsBlockA(pair.i) AndAlso Not alignment.IsBlockA(pair.j),
                                  "every cross-block pair must join one A column with one B column")
                crossPairs += 1
            Next

            Call Support.Same(6, crossPairs, "3 x 2 cross-block column pairs")

            ' FASTA 往返
            Dim text As String = alignment.ToFasta()
            Dim round As JointAlignment = JointAlignment.FromFasta(ParseFasta(text))

            Call Support.Same(alignment.Rows, round.Rows, "the round-tripped alignment must keep the row count")
            Call Support.Same(alignment.LengthA, round.LengthA, "the round-tripped alignment must keep length A")
            Call Support.Same(alignment.LengthB, round.LengthB, "the round-tripped alignment must keep length B")

            For r As Integer = 0 To alignment.Rows - 1
                For c As Integer = 0 To alignment.Length - 1
                    Call Support.Same(alignment(r, c), round(r, c), $"residue ({r},{c}) must survive the FASTA round trip")
                Next
            Next

            ' 列过滤：删除空位比例超过阈值的列(第 1 列在 3 行之中有 2 行为空位)
            Dim gappy As JointAlignment = Fixtures.FromRows({"A-CD", "A--D", "AGCD"}, 3)
            Dim filtered As JointAlignment = gappy.FilterColumns(0.5)

            Call Support.Same(3, filtered.LengthA + filtered.LengthB, "the gappy column must be dropped")
            Call Support.Same(2, filtered.LengthA, "one family A column must be removed")
            Call Support.Same(3, filtered.Rows, "filtering columns must not change the number of rows")
        End Sub

        Private Function ParseFasta(text As String) As List(Of FastaSeq)
            Dim result As New List(Of FastaSeq)
            Dim lines As String() = text.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)
            Dim current As FastaSeq = Nothing

            For Each line As String In lines
                If line.StartsWith(">"c) Then
                    If current IsNot Nothing Then
                        Call result.Add(current)
                    End If

                    current = New FastaSeq With {.Headers = New String() {line.Substring(1)}, .SequenceData = ""}
                ElseIf current IsNot Nothing Then
                    current.SequenceData &= line.Trim()
                End If
            Next

            If current IsNot Nothing Then
                Call result.Add(current)
            End If

            Return result
        End Function

        Private Sub ColumnCountChecks()
            Dim alignment As JointAlignment = Fixtures.FromRows({"AA", "AC", "AC", "GG"}, 1)
            Dim counts As New ColumnCounts(alignment)

            Call Support.Same(4, counts.SampleSize, "the sample size")
            Call Support.Same(2, counts.Length, "the column count")

            Dim total As Integer = 0

            For Each n As Integer In counts.SingleCounts(0)
                total += n
            Next

            Call Support.Same(4, total, "the single column counts must sum to the sample size")

            Dim joint As Dictionary(Of Integer, Integer) = counts.PairCounts(0, 1)
            Dim jointTotal As Integer = 0

            For Each entry As KeyValuePair(Of Integer, Integer) In joint
                jointTotal += entry.Value
            Next

            Call Support.Same(4, jointTotal, "the joint counts must sum to the sample size")
            Call Support.Same(4, counts.PairTotal(0, 1), "the pair total must equal the sample size")

            ' (A,A) 出现 1 次、(A,C) 出现 2 次、(G,G) 出现 1 次
            Dim k As Integer = counts.AlphabetSize
            Dim codeA As Integer = alignment.Alphabet.IndexOf("A"c)
            Dim codeC As Integer = alignment.Alphabet.IndexOf("C"c)
            Dim codeG As Integer = alignment.Alphabet.IndexOf("G"c)

            Call Support.Same(1, joint(codeA * k + codeA), "count of the (A,A) residue pair")
            Call Support.Same(2, joint(codeA * k + codeC), "count of the (A,C) residue pair")
            Call Support.Same(1, joint(codeG * k + codeG), "count of the (G,G) residue pair")
        End Sub

        Private Sub SwapInvarianceChecks()
            Dim alignment As JointAlignment = Fixtures.RandomAlignment(24, 3, 3, 424242UL)
            Dim counts As New ColumnCounts(alignment)
            Dim singles As Integer()() = New Integer(5)() {}

            For c As Integer = 0 To 5
                singles(c) = DirectCast(counts.SingleCounts(c).Clone(), Integer())
            Next

            Dim jointBefore As Integer = 0

            For Each entry As KeyValuePair(Of Integer, Integer) In counts.PairCounts(0, 3)
                jointBefore += entry.Value
            Next

            Dim swapped As JointAlignment = counts.ApplySwap(0, 1)

            ' 单列计数在行置换之下是不变量
            For c As Integer = 0 To 5
                For v As Integer = 0 To counts.AlphabetSize - 1
                    Call Support.Same(singles(c)(v), counts.SingleCounts(c)(v),
                                      $"the single column count ({c},{v}) must be invariant under a partner swap")
                Next
            Next

            ' 跨家族列对的增量更新必须与全量重新统计一致
            Dim reference As New ColumnCounts(swapped)

            For i As Integer = 0 To swapped.LengthA - 1
                For j As Integer = swapped.LengthA To swapped.Length - 1
                    Dim incremental As Dictionary(Of Integer, Integer) = counts.PairCounts(i, j)
                    Dim expected As Dictionary(Of Integer, Integer) = reference.PairCounts(i, j)

                    Call Support.Same(expected.Count, incremental.Count,
                                      $"the incremental joint count size of the column pair ({i},{j}) must match a full recount")

                    For Each entry As KeyValuePair(Of Integer, Integer) In expected
                        Dim actual As Integer = 0

                        Call incremental.TryGetValue(entry.Key, actual)
                        Call Support.Same(entry.Value, actual, $"the incremental joint count of ({i},{j}) code {entry.Key}")
                    Next
                Next
            Next

            ' 交换两次应当回到原始状态
            Dim restored As JointAlignment = counts.ApplySwap(0, 1)

            For r As Integer = 0 To alignment.Rows - 1
                For c As Integer = 0 To alignment.Length - 1
                    Call Support.Same(alignment(r, c), restored(r, c), $"swapping twice must restore the residue ({r},{c})")
                Next
            Next
        End Sub

        Private Sub PairedBuilderChecks()
            Dim msaA As New FastaFile({
                New FastaSeq With {.Headers = New String() {"g1|a1"}, .SequenceData = "ACDE"},
                New FastaSeq With {.Headers = New String() {"g2|a2"}, .SequenceData = "ACDF"},
                New FastaSeq With {.Headers = New String() {"g3|a3"}, .SequenceData = "ACDG"}
            })
            Dim msaB As New FastaFile({
                New FastaSeq With {.Headers = New String() {"g1|b1"}, .SequenceData = "WY-"},
                New FastaSeq With {.Headers = New String() {"g2|b2"}, .SequenceData = "WY-"},
                New FastaSeq With {.Headers = New String() {"g3|b3"}, .SequenceData = "WY-"}
            })
            Dim pairings As PairedMsaBuilder.Pairing() = {
                New PairedMsaBuilder.Pairing("g1", "a1", "b1"),
                New PairedMsaBuilder.Pairing("g2", "a2", "b2"),
                New PairedMsaBuilder.Pairing("g3", "a3", Nothing),
                New PairedMsaBuilder.Pairing("g4", "a4", "b4")
            }
            Dim builder As New PairedMsaBuilder With {.MaxGapFraction = 1.0}
            Dim joint As JointAlignment = builder.Build(msaA, msaB, pairings)

            Call Support.Same(2, joint.Rows, "only the genomes that have both family members can be paired")
            Call Support.Same(4, joint.LengthA, "the family A block length")
            Call Support.Same(3, joint.LengthB, "the family B block length")
            Call Support.Same(1, builder.LastReport.MissingA.Length, "g3 has no family B member")
            Call Support.Same(1, builder.LastReport.MissingB.Length, "g4 has no family A member")
            Call Support.True(String.Equals(joint.NameOfA(0), "a1"), "the family A member id must be recorded")
            Call Support.True(String.Equals(joint.NameOfB(0), "b1"), "the family B member id must be recorded")

            ' 完全没有可配对的行时必须报错，而不是静默返回空比对
            Call Support.Throws(Of ArgumentException)(
                Sub() builder.Build(msaA, msaB, {New PairedMsaBuilder.Pairing("gx", "ax", "bx")}),
                "an empty pairing must raise an error")

            ' 自动推导：每个基因组恰好各有一个成员
            Dim automatic As JointAlignment = builder.Build(msaA, msaB)
            Call Support.Same(3, automatic.Rows, "the pairing can be derived from the sequence headers")
        End Sub

        Private Sub AlphabetChecks()
            Dim alphabet As ResidueAlphabet = ResidueAlphabet.Standard21

            Call Support.Same(21, alphabet.Size, "the paper uses a 21 symbol alphabet")
            Call Support.Same(alphabet.GapIndex, alphabet.IndexOf("-"c), "the gap index must be consistent")

            For i As Integer = 0 To alphabet.Size - 1
                Call Support.Same(i, alphabet.IndexOf(alphabet.Symbol(i)), $"the symbol {alphabet.Symbol(i)} must round trip")
            Next

            ' 非标准残基的映射
            Call Support.Same(alphabet.IndexOf("K"c), alphabet.IndexOf("O"c), "O must be mapped to K")
            Call Support.Same(alphabet.IndexOf("C"c), alphabet.IndexOf("U"c), "U must be mapped to C")

            ' 非标准残基在关闭映射之后必须被拒绝
            Dim strictAlphabet As New ResidueAlphabet(ResidueAlphabet.StandardSymbols, includeNonStandard:=False)
            Call Support.Throws(Of ArgumentException)(
                Sub() strictAlphabet.Encode("AO"),
                "an unknown residue must be rejected when the non standard mapping is disabled")

            Call Support.Same(4, alphabet.Encode("ACDE").Length, "the encoded length must equal the sequence length")
        End Sub
    End Module
End Namespace
