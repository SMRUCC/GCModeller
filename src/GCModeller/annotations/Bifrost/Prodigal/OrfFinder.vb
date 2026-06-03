
Imports SMRUCC.genomics.SequenceModel.FASTA

' ========================================================================
' ORF查找器 - 六框翻译找所有候选ORF
' ========================================================================

''' <summary>
''' 在六框翻译中查找所有候选ORF
''' 核心思路：对每个阅读框，先找所有终止密码子位置，再在每个终止-终止区间内
''' 查找所有可能的起始密码子，生成多个候选ORF（同一终止密码子，不同起始密码子）
''' </summary>
Public Class OrfFinder

    ''' <summary>最小ORF长度（核苷酸数），默认90nt=30aa</summary>
    Private _minOrfLength As Integer

    ''' <summary>允许的起始密码子列表</summary>
    Private _startCodons As String()

    Public Sub New(Optional minOrfLength As Integer = 90)
        _minOrfLength = minOrfLength
        _startCodons = {"ATG", "GTG", "TTG"}
    End Sub

    ''' <summary>
    ''' 在一条序列上查找所有候选ORF
    ''' </summary>
    Public Function FindOrfs(fastaSeq As FastaSeq) As List(Of CandidateOrf)
        Dim orfs As New List(Of CandidateOrf)()
        Dim seq = fastaSeq.SequenceData
        Dim seqId = fastaSeq.locus_tag

        If seq.Length < 6 Then Return orfs

        ' 正向链（阅读框0, 1, 2）
        FindOrfsOnStrand(seq, seqId, "+"c, orfs)

        ' 反向链
        Dim rcSeq = SequenceUtils.ReverseComplement(seq)
        FindOrfsOnStrand(rcSeq, seqId, "-"c, orfs)

        ' 转换反向链ORF的坐标到原始序列坐标系
        For Each orf In orfs
            If orf.Strand = "-"c Then
                ConvertReverseStrandCoords(orf, seq.Length)
            End If
        Next

        Return orfs
    End Function

    ''' <summary>
    ''' 在单条链上查找ORF
    ''' 算法：对每个阅读框，先定位所有终止密码子，再在每个终止-终止区间内
    ''' 查找所有起始密码子，为每个起始-终止对生成一个候选ORF
    ''' </summary>
    Private Sub FindOrfsOnStrand(seq As String, seqId As String, strand As Char, orfs As List(Of CandidateOrf))
        For frame As Integer = 0 To 2
            ' 第一步：收集该阅读框内所有终止密码子的位置
            Dim stopPositions As New List(Of Integer)
            For i As Integer = frame To seq.Length - 3 Step 3
                Dim codon = seq.Substring(i, 3)
                If SequenceUtils.IsStopCodon(codon) Then
                    stopPositions.Add(i)
                End If
            Next

            ' 第二步：在每个终止-终止区间内查找起始密码子
            Dim regionStart As Integer = frame  ' 区间起始（上一个终止密码子之后，或序列开头）

            For Each stopPos In stopPositions
                ' 在 [regionStart, stopPos) 范围内查找所有起始密码子
                Dim foundStart As Boolean = False
                For i As Integer = regionStart To stopPos - 3 Step 3
                    Dim codon = seq.Substring(i, 3)
                    If Array.IndexOf(_startCodons, codon) >= 0 Then
                        ' 找到一个起始密码子，生成候选ORF
                        Dim orfLen = stopPos + 3 - i
                        If orfLen >= _minOrfLength Then
                            Dim orfSeq = seq.Substring(i, stopPos + 3 - i)
                            Dim aaSeq = SequenceUtils.Translate(orfSeq)
                            ' 去除末尾的终止密码子对应的 '*'
                            If aaSeq.Length > 0 AndAlso aaSeq(aaSeq.Length - 1) = "*"c Then
                                aaSeq = aaSeq.Substring(0, aaSeq.Length - 1)
                            End If

                            Dim orf As New CandidateOrf With {
                                .SeqId = seqId,
                                .RawStart = i,
                                .RawEnd = stopPos + 2,
                                .Start = i + 1,  ' 暂用0-based+1，反向链后面会转换
                                .[End] = stopPos + 3,
                                .Strand = strand,
                                .Frame = frame,
                                .StartCodon = codon,
                                .StopCodon = seq.Substring(stopPos, 3),
                                .Length = orfLen,
                                .NtSequence = orfSeq,
                                .AaSequence = aaSeq
                            }
                            orfs.Add(orf)
                            foundStart = True
                        End If
                    End If
                Next

                ' 如果该区间没有起始密码子，跳过（非编码区间）
                regionStart = stopPos + 3
            Next

            ' 第三步：处理序列末尾的边缘ORF（无终止密码子）
            If regionStart < seq.Length - 2 Then
                For i As Integer = regionStart To seq.Length - 3 Step 3
                    If i + 2 >= seq.Length Then Exit For
                    Dim codon = seq.Substring(i, 3)
                    If Array.IndexOf(_startCodons, codon) >= 0 Then
                        Dim orfLen = seq.Length - i
                        If orfLen >= _minOrfLength Then
                            Dim orfSeq = seq.Substring(i)
                            Dim aaSeq = SequenceUtils.Translate(orfSeq)

                            Dim orf As New PredictedGene With {
                                .SeqId = seqId,
                                .RawStart = i,
                                .RawEnd = seq.Length - 1,
                                .Start = i + 1,
                                .[End] = seq.Length,
                                .Strand = strand,
                                .Frame = frame,
                                .StartCodon = codon,
                                .StopCodon = "---",
                                .Length = orfLen,
                                .NtSequence = orfSeq,
                                .AaSequence = aaSeq,
                                .PartialType = "3'"  ' 3'端截断
                            }
                            orfs.Add(orf)
                        End If
                    End If
                Next
            End If

            ' 第四步：处理序列开头的边缘ORF（5'端截断，无起始密码子）
            ' 在第一个终止密码子之前，可能存在没有起始密码子的部分ORF
            If stopPositions.Count > 0 Then
                Dim firstStop = stopPositions(0)
                If firstStop >= _minOrfLength Then
                    ' 检查从序列开头到第一个终止密码子之间是否有in-frame的编码区
                    Dim partialOrfSeq = seq.Substring(frame, firstStop + 3 - frame)
                    Dim aaSeq = SequenceUtils.Translate(partialOrfSeq)
                    If aaSeq.Length > 10 Then  ' 至少10个氨基酸
                        Dim orf As New PredictedGene With {
                            .SeqId = seqId,
                            .RawStart = frame,
                            .RawEnd = firstStop + 2,
                            .Start = frame + 1,
                            .[End] = firstStop + 3,
                            .Strand = strand,
                            .Frame = frame,
                            .StartCodon = ">>>",  ' 边缘标记
                            .StopCodon = seq.Substring(firstStop, 3),
                            .Length = firstStop + 3 - frame,
                            .NtSequence = partialOrfSeq,
                            .AaSequence = aaSeq,
                            .PartialType = "5'"  ' 5'端截断
                        }
                        orfs.Add(orf)
                    End If
                End If
            End If
        Next
    End Sub

    ''' <summary>
    ''' 将反向链ORF的坐标从反向互补链坐标转换为原始序列坐标
    ''' </summary>
    Private Sub ConvertReverseStrandCoords(orf As CandidateOrf, seqLen As Integer)
        ' 反向链上：rawStart在RC序列中的位置 → 原始序列中的位置
        ' 原始序列位置 = seqLen - rcPos - 1
        Dim origStart = seqLen - orf.RawEnd
        Dim origEnd = seqLen - orf.RawStart
        orf.Start = origStart + 1  ' 1-based
        orf.[End] = origEnd
    End Sub

End Class
