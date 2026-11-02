Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics
Imports SMRUCC.genomics.Analysis.SequenceAlignment.Clustal
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Bayesian.Alignment

    ''' <summary>
    ''' 由「两个蛋白家族各自的多重比对」构建配对联合比对。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这是论文数据结构的第一种输入方式(<see cref="JointAlignment"/> 则对应第二种：
    ''' 直接读入已经配对好的联合比对)。构建流程是：
    ''' </para>
    ''' <list type="number">
    ''' <item><description>读入家族 A 与家族 B 各自的多重比对；</description></item>
    ''' <item><description>按照基因组编号把同一基因组之内的 A 成员与 B 成员配对；</description></item>
    ''' <item><description>去掉冗余序列(可选)以及空位过多的比对列；</description></item>
    ''' <item><description>输出联合比对与一份配对审计报告。</description></item>
    ''' </list>
    ''' <para>
    ''' 论文强调「配对 MSA 的质量决定方法的上限」，其中最关键的一点就是
    ''' <b>两个家族的同源序列必须来自同一个物种</b>。因此本类把「按基因组配对」当作硬性要求：
    ''' 任何一侧缺失的基因组都会被记录下来并且排除在联合比对之外，
    ''' 而不是像旧实现那祥把两条毫不相关的序列直接做字符串拼接。
    ''' </para>
    ''' </remarks>
    Public Class PairedMsaBuilder

        ''' <summary>
        ''' 一条基因组的配对信息
        ''' </summary>
        Public Structure Pairing
            ''' <summary>
            ''' 基因组(物种)编号
            ''' </summary>
            Public Genome As String
            ''' <summary>
            ''' 家族 A 一侧的基因编号
            ''' </summary>
            Public GeneA As String
            ''' <summary>
            ''' 家族 B 一侧的基因编号
            ''' </summary>
            Public GeneB As String

            Public Sub New(genome As String, geneA As String, geneB As String)
                Me.Genome = genome
                Me.GeneA = geneA
                Me.GeneB = geneB
            End Sub

            Public Overrides Function ToString() As String
                Return $"{Genome}: {GeneA} -- {GeneB}"
            End Function
        End Structure

        ''' <summary>
        ''' 配对过程的审计报告
        ''' </summary>
        Public Class BuildReport

            ''' <summary>
            ''' 成功配对的行数
            ''' </summary>
            Public Property Paired As Integer
            ''' <summary>
            ''' 有 A 成员但缺少 B 成员的基因组
            ''' </summary>
            Public Property MissingB As String()
            ''' <summary>
            ''' 有 B 成员但缺少 A 成员的基因组
            ''' </summary>
            Public Property MissingA As String()
            ''' <summary>
            ''' 因为基因组之内成员不唯一(多于一个 A 或者多于一个 B)而被跳过的基因组
            ''' </summary>
            Public Property Ambiguous As String()
            ''' <summary>
            ''' 家族 A 的原始列数
            ''' </summary>
            Public Property ColumnsABefore As Integer
            ''' <summary>
            ''' 家族 B 的原始列数
            ''' </summary>
            Public Property ColumnsBBefore As Integer
            ''' <summary>
            ''' 过滤之后家族 A 的列数
            ''' </summary>
            Public Property ColumnsAAfter As Integer
            ''' <summary>
            ''' 过滤之后家族 B 的列数
            ''' </summary>
            Public Property ColumnsBAfter As Integer

            Public Overrides Function ToString() As String
                Return $"paired={Paired}, missingA={If(MissingA Is Nothing, 0, MissingA.Length)}, " &
                    $"missingB={If(MissingB Is Nothing, 0, MissingB.Length)}, ambiguous={If(Ambiguous Is Nothing, 0, Ambiguous.Length)}, " &
                    $"columns=({ColumnsABefore}+{ColumnsBBefore}) -> ({ColumnsAAfter}+{ColumnsBAfter})"
            End Function
        End Class

        ''' <summary>
        ''' 残基字母表
        ''' </summary>
        Public Property Alphabet As ResidueAlphabet = Nothing
        ''' <summary>
        ''' 允许的最大空位比例，超过该比例的列会被删除
        ''' </summary>
        Public Property MaxGapFraction As Double = 0.75
        ''' <summary>
        ''' 序列标题之中「基因组」与「基因」之间的分隔符，默认使用竖线
        ''' </summary>
        Public Property HeaderSeparator As Char = "|"c
        ''' <summary>
        ''' 是否在构建之后做一次冗余序列去除
        ''' </summary>
        Public Property RemoveRedundant As Boolean = False
        ''' <summary>
        ''' 冗余判定的序列一致性阈值
        ''' </summary>
        Public Property RedundancyIdentity As Double = 0.95

        ''' <summary>
        ''' 最近一次构建的审计报告
        ''' </summary>
        Public ReadOnly Property LastReport As BuildReport
            Get
                Return _report
            End Get
        End Property

        Private _report As BuildReport = New BuildReport()

        ''' <summary>
        ''' 从两个家族各自的比对 FASTA 文件与配对表构建联合比对
        ''' </summary>
        ''' <param name="msaA">家族 A 的多重比对 FASTA</param>
        ''' <param name="msaB">家族 B 的多重比对 FASTA</param>
        ''' <param name="pairings">
        ''' 配对表：每一条记录给出基因组编号以及该基因组之内两个成员的基因编号。
        ''' 为 <c>Nothing</c> 的时候尝试从序列标题自动推导(要求每个基因组恰好各有一个成员)。
        ''' </param>
        Public Function Build(msaA As FastaFile, msaB As FastaFile, Optional pairings As IEnumerable(Of Pairing) = Nothing) As JointAlignment
            If msaA Is Nothing OrElse msaB Is Nothing Then
                Throw New ArgumentNullException(NameOf(msaA), "both family alignments are required")
            End If

            Dim indexA As Dictionary(Of String, FastaSeq) = IndexByName(msaA)
            Dim indexB As Dictionary(Of String, FastaSeq) = IndexByName(msaB)
            Dim mapping As List(Of Pairing) = If(pairings IsNot Nothing, pairings.ToList(), Derive(msaA, msaB))

            Dim report As New BuildReport With {
                .ColumnsABefore = msaA.First.SequenceData.Length,
                .ColumnsBBefore = msaB.First.SequenceData.Length
            }

            If _report IsNot Nothing AndAlso _report.Ambiguous IsNot Nothing Then
                report.Ambiguous = _report.Ambiguous
            End If

            Dim missingA As New List(Of String)
            Dim missingB As New List(Of String)
            Dim selected As New List(Of (pair As Pairing, seqA As FastaSeq, seqB As FastaSeq))

            For Each pair As Pairing In mapping
                Dim seqA As FastaSeq = Nothing
                Dim seqB As FastaSeq = Nothing

                If String.IsNullOrEmpty(pair.GeneA) OrElse Not indexA.TryGetValue(pair.GeneA, seqA) Then
                    Call missingA.Add(pair.Genome)
                    Continue For
                End If
                If String.IsNullOrEmpty(pair.GeneB) OrElse Not indexB.TryGetValue(pair.GeneB, seqB) Then
                    Call missingB.Add(pair.Genome)
                    Continue For
                End If

                Call selected.Add((pair, seqA, seqB))
            Next

            report.MissingA = missingA.Distinct().ToArray()
            report.MissingB = missingB.Distinct().ToArray()
            report.Paired = selected.Count

            If selected.Count = 0 Then
                Throw New ArgumentException(
                    "no genome has both family members in the two alignments; the pairing table does not match the sequence headers")
            End If

            Dim lengthA As Integer = selected(0).seqA.SequenceData.Length
            Dim lengthB As Integer = selected(0).seqB.SequenceData.Length

            For Each item In selected
                DebugAudit.Check(item.seqA.SequenceData.Length = lengthA,
                                 $"family A alignment has inconsistent length on '{item.seqA.Headers.First}'")
                DebugAudit.Check(item.seqB.SequenceData.Length = lengthB,
                                 $"family B alignment has inconsistent length on '{item.seqB.Headers.First}'")
            Next

            Dim alphabetUsed As ResidueAlphabet = If(Alphabet, ResidueAlphabet.Standard21)
            Dim data As Integer(,) = New Integer(selected.Count - 1, lengthA + lengthB - 1) {}
            Dim names As String() = New String(selected.Count - 1) {}

            For r As Integer = 0 To selected.Count - 1
                names(r) = selected(r).pair.Genome

                Dim codesA As Integer() = alphabetUsed.Encode(selected(r).seqA.SequenceData, selected(r).seqA.Headers.First)
                Dim codesB As Integer() = alphabetUsed.Encode(selected(r).seqB.SequenceData, selected(r).seqB.Headers.First)

                For c As Integer = 0 To lengthA - 1
                    data(r, c) = codesA(c)
                Next

                For c As Integer = 0 To lengthB - 1
                    data(r, lengthA + c) = codesB(c)
                Next
            Next

            Dim membersA As String() = New String(selected.Count - 1) {}
            Dim membersB As String() = New String(selected.Count - 1) {}

            For r As Integer = 0 To selected.Count - 1
                membersA(r) = selected(r).pair.GeneA
                membersB(r) = selected(r).pair.GeneB
            Next

            Dim joint As New JointAlignment(data, names, lengthA, lengthB, alphabetUsed) With {
                .MemberA = membersA,
                .MemberB = membersB
            }

            If RemoveRedundant Then
                joint = RemoveRedundantRows(joint)
            End If

            Dim filtered As JointAlignment = joint.FilterColumns(MaxGapFraction)

            report.ColumnsAAfter = filtered.LengthA
            report.ColumnsBAfter = filtered.LengthB
            _report = report

            Return filtered
        End Function

        ''' <summary>
        ''' 从文件路径构建联合比对
        ''' </summary>
        Public Function Build(msaPathA As String, msaPathB As String, Optional pairingFile As String = Nothing) As JointAlignment
            Dim pairings As IEnumerable(Of Pairing) = Nothing

            If Not String.IsNullOrEmpty(pairingFile) Then
                pairings = LoadPairings(pairingFile)
            End If

            Return Build(FastaFile.Read(msaPathA), FastaFile.Read(msaPathB), pairings)
        End Function

        ''' <summary>
        ''' 从 Clustal Omega 的比对结果构建联合比对
        ''' </summary>
        Public Function Build(resultA As ClustalOmegaResult, resultB As ClustalOmegaResult, Optional pairings As IEnumerable(Of Pairing) = Nothing) As JointAlignment
            If resultA Is Nothing OrElse resultB Is Nothing Then
                Throw New ArgumentNullException(NameOf(resultA), "both family alignments are required")
            End If

            Return Build(ToFasta(resultA), ToFasta(resultB), pairings)
        End Function

        ''' <summary>
        ''' 直接对两组未比对的序列运行 Clustal Omega(等价于论文之中的 <c>hmmalign</c> 步骤)，
        ''' 然后再构建联合比对。
        ''' </summary>
        Public Function BuildFromSequences(sequencesA As IEnumerable(Of FastaSeq),
                                           sequencesB As IEnumerable(Of FastaSeq),
                                           pairings As IEnumerable(Of Pairing)) As JointAlignment

            Dim resultA As ClustalOmegaResult = ClustalOmega.MultipleAlignmentOmega(sequencesA)
            Dim resultB As ClustalOmegaResult = ClustalOmega.MultipleAlignmentOmega(sequencesB)

            Return Build(resultA, resultB, pairings)
        End Function

        ''' <summary>
        ''' 读取配对表(CSV，列为 genome,geneA,geneB)
        ''' </summary>
        Public Shared Function LoadPairings(path As String) As Pairing()
            Dim lines As String() = System.IO.File.ReadAllLines(path)
            Dim result As New List(Of Pairing)

            For Each line As String In lines
                If String.IsNullOrWhiteSpace(line) Then
                    Continue For
                End If

                Dim cells As String() = SplitCells(line)

                If cells.Length < 2 Then
                    Continue For
                End If

                If String.Equals(cells(0).Trim(), "genome", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                Dim genome As String = cells(0).Trim()
                Dim geneA As String = cells(1).Trim()
                Dim geneB As String = If(cells.Length > 2, cells(2).Trim(), geneA)

                Call result.Add(New Pairing(genome, geneA, geneB))
            Next

            Return result.ToArray()
        End Function

        Private Shared Function SplitCells(line As String) As String()
            If line.IndexOf(vbTab) >= 0 Then
                Return line.Split(vbTab)
            Else
                Return line.Split(","c)
            End If
        End Function

        Private Shared Function ToFasta(result As ClustalOmegaResult) As FastaFile
            Dim data As New List(Of FastaSeq)

            For i As Integer = 0 To result.names.Length - 1
                Call data.Add(New FastaSeq With {
                    .Headers = {result.names(i)},
                    .SequenceData = result.MSA(i)
                })
            Next

            Return New FastaFile(data)
        End Function

        Private Function IndexByName(msa As FastaFile) As Dictionary(Of String, FastaSeq)
            Dim index As New Dictionary(Of String, FastaSeq)(StringComparer.Ordinal)

            For Each seq As FastaSeq In msa
                Dim id As String = GeneId(seq.Headers.First)

                If Not index.ContainsKey(id) Then
                    index(id) = seq
                End If
            Next

            Return index
        End Function

        ''' <summary>
        ''' 从序列标题之中取出基因编号(标题形如 <c>genome|gene</c> 或者 <c>gene description</c>)
        ''' </summary>
        Private Function GeneId(header As String) As String
            Dim token As String = header
            Dim space As Integer = token.IndexOf(" "c)

            If space > 0 Then
                token = token.Substring(0, space)
            End If

            If HeaderSeparator <> ChrW(0) Then
                Dim parts As String() = token.Split(HeaderSeparator)

                If parts.Length > 1 Then
                    Return parts(parts.Length - 1)
                End If
            End If

            Return token
        End Function

        ''' <summary>
        ''' 从序列标题推导配对关系：要求每个基因组在两侧都恰好各有一个成员。
        ''' </summary>
        ''' <remarks>
        ''' 标题的格式约定为 <c>genomeId|geneId</c>。这里必须使用<b>原始标题</b>而不是已经
        ''' 去掉基因组前缀之后的基因编号，否则无法恢复出基因组这一层分组信息。
        ''' </remarks>
        Private Function Derive(msaA As FastaFile, msaB As FastaFile) As List(Of Pairing)
            Dim groupA As Dictionary(Of String, List(Of String)) = GroupByGenome(msaA)
            Dim groupB As Dictionary(Of String, List(Of String)) = GroupByGenome(msaB)
            Dim result As New List(Of Pairing)
            Dim ambiguous As New List(Of String)

            For Each genome As String In groupA.Keys
                Dim left As List(Of String) = groupA(genome)
                Dim right As List(Of String) = Nothing

                If Not groupB.TryGetValue(genome, right) Then
                    Continue For
                End If

                If left.Count <> 1 OrElse right.Count <> 1 Then
                    Call ambiguous.Add(genome)
                    Continue For
                End If

                Call result.Add(New Pairing(genome, left(0), right(0)))
            Next

            _report = New BuildReport With {.Ambiguous = ambiguous.ToArray()}

            Return result
        End Function

        Private Function GroupByGenome(msa As FastaFile) As Dictionary(Of String, List(Of String))
            Dim groups As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)

            For Each seq As FastaSeq In msa
                Dim header As String = seq.Headers.First
                Dim token As String = header
                Dim space As Integer = token.IndexOf(" "c)

                If space > 0 Then
                    token = token.Substring(0, space)
                End If

                Dim genome As String = GenomeId(token)
                Dim list As List(Of String) = Nothing

                If Not groups.TryGetValue(genome, list) Then
                    list = New List(Of String)
                    groups(genome) = list
                End If

                Call list.Add(GeneId(token))
            Next

            Return groups
        End Function

        Private Function GenomeId(geneId As String) As String
            Dim p As Integer = geneId.IndexOf(HeaderSeparator)

            Return If(p > 0, geneId.Substring(0, p), geneId)
        End Function

        Private Function RemoveRedundantRows(alignment As JointAlignment) As JointAlignment
            Dim keep As New List(Of Integer)

            For i As Integer = 0 To alignment.Rows - 1
                Dim duplicate As Boolean = False

                For Each j As Integer In keep
                    If Identity(alignment, i, j) >= RedundancyIdentity Then
                        duplicate = True
                        Exit For
                    End If
                Next

                If Not duplicate Then
                    Call keep.Add(i)
                End If
            Next

            If keep.Count = alignment.Rows Then
                Return alignment
            End If

            Return alignment.Subset(keep.ToArray())
        End Function

        Private Shared Function Identity(alignment As JointAlignment, i As Integer, j As Integer) As Double
            Dim same As Integer = 0
            Dim compared As Integer = 0

            For c As Integer = 0 To alignment.Length - 1
                Dim a As Integer = alignment(i, c)
                Dim b As Integer = alignment(j, c)

                If a = alignment.Alphabet.GapIndex OrElse b = alignment.Alphabet.GapIndex Then
                    Continue For
                End If

                compared += 1

                If a = b Then
                    same += 1
                End If
            Next

            If compared = 0 Then
                Return 0.0
            Else
                Return same / CDbl(compared)
            End If
        End Function
    End Class
End Namespace
