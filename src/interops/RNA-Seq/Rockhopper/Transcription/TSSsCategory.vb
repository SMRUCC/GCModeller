' /********************************************************************************/
'
'  Rockhopper —— TSS 六分类
'
'  复刻自原始 Rockhopper 的 TSSsCategory 模块（API/TSSsCategory.vb），
'  分类依据论文 Figure 1 的"最小转录单元(MTU)模型"：
'    (i)   mTSS   —— 位于 ATG 上游 0–300 bp 的 mRNA 的 TSS
'    (ii)  lmTSS  —— leaderless：TSS 与 ATG 完全重合
'    (iii) ULmTSS —— 超长 5'UTR：TSS 位于 ATG 上游 300–500 bp
'    (iv)  seTSS  —— 与蛋白编码基因同向且位于其内部的内部转录本 TSS
'    (v)   asTSS  —— 位于蛋白编码基因内部但方向相反（顺式反义 RNA）
'    (vi)  sTSS   —— 基因间区(IGR)内、与相邻基因有确定距离的 trans-encoded sRNA
'    (vii) pmTSS  —— 假定的 mRNA（无法在注释中定位的长 5'UTR / sRNA）
'
'  迁移要点：
'    * 原实现依赖 `LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader`，
'      该类型在当前框架已不存在。这里把"序列"改为可选的普通字符串参数，
'      并内置等价的 ORF 查找，从而彻底去掉该依赖。
'    * CSV / LANS 等失效符号全部移除。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Transcription

    ''' <summary>
    ''' TSS 的六分类（外加"未分类"）。
    ''' </summary>
    Public Enum TssCategory As Integer
        ''' <summary>无法进行 TSS 分类定义。</summary>
        UnClassified = -100
        ''' <summary>(i) TSS Of an mRNA.</summary>
        mTSS = 1
        ''' <summary>(ii) TSS Of a leaderless transcript.（TSS 与 ATG 重合）</summary>
        lmTSS = 2
        ''' <summary>Ultra Long mRNA TSSs（TSS 距 ATG 300–500 bp）。</summary>
        ULmTSS = 3
        ''' <summary>(iii) TSS Of a putative mRNA.</summary>
        pmTSS = 4
        ''' <summary>(v) TSS of a cis-encoded antisense RNA.</summary>
        asTSS = 5
        ''' <summary>(iv) TSS of a sense transcript.</summary>
        seTSS = 6
        ''' <summary>(vi) TSS of a trans-encoded sRNA.</summary>
        sTSS = 7
    End Enum

    ''' <summary>
    ''' 基因类型（与原始实现的 GeneTypes 保持一致）。
    ''' </summary>
    Public Enum GeneTypes
        misc_RNA
        CDS
    End Enum

    ''' <summary>
    ''' TSS 位点分类器。
    ''' </summary>
    Public Module TSSsCategory

        ''' <summary>
        ''' 对单个 TSS 位点进行分类。
        ''' </summary>
        ''' <param name="tss">转录起始位点。</param>
        ''' <param name="atg">翻译起始位点（0 表示未知）。</param>
        ''' <param name="tga">翻译终止位点（0 表示未知）。</param>
        ''' <param name="transcriptLoci">该转录本的基因组位点（用于与注释基因比较关系）。</param>
        ''' <param name="isRNA">该转录本是否为 RNA 类型（而非已注释的蛋白编码基因）。</param>
        ''' <param name="isSRNA">是否为小 RNA。</param>
        ''' <param name="synonym">基因号 / locus_tag（可为 "putative mRNA"）。</param>
        ''' <param name="ptt">注释表。</param>
        ''' <param name="putativeMRNA">是否已由外部 ORF 查找判定为假定 mRNA。</param>
        ''' <param name="relatedGene">输出：与之相关的注释基因。</param>
        Public Function Category(tss As Integer, atg As Integer, tga As Integer,
                                 transcriptLoci As NucleotideLocation,
                                 isRNA As Boolean, isSRNA As Boolean,
                                 synonym As String,
                                 ptt As PTT,
                                 Optional putativeMRNA As Boolean = False,
                                 Optional ByRef relatedGene As GeneBrief = Nothing) As TssCategory

            Dim pttGenes As IEnumerable(Of GeneBrief) = If(ptt Is Nothing, Enumerable.Empty(Of GeneBrief)(), ptt.GeneObjects)

            ' (ii) leaderless：TSS 与 ATG 完全重合
            If tss > 0 AndAlso atg > 0 AndAlso tss = atg Then
                relatedGene = lookupGene(ptt, synonym)
                Return TssCategory.lmTSS
            End If

            ' (i) / (iii) mRNA 与超长 5'UTR
            If tss > 0 AndAlso atg > 0 Then
                Dim d As Integer = System.Math.Abs(tss - atg)
                If d >= 0 AndAlso d < 300 Then
                    relatedGene = lookupGene(ptt, synonym)
                    Return TssCategory.mTSS
                ElseIf d >= 300 AndAlso d < 500 Then
                    relatedGene = lookupGene(ptt, synonym)
                    Return TssCategory.ULmTSS
                End If
            End If

            ' (vii) 假定 mRNA：从头装配得到的 RNA，在 PTT 中找不到对应注释
            If isRNA AndAlso Not isSRNA AndAlso (putativeMRNA OrElse String.Equals(synonym, "putative mRNA")) Then
                Dim pmRNALoci As NucleotideLocation = NucleotideLocation.CreateObject(atg, tga)
                Dim orf As GeneBrief() = (From g As GeneBrief In pttGenes
                                          Where g.Location.Equals(pmRNALoci)
                                          Select g).ToArray()

                If orf.Length = 0 Then
                    relatedGene = New GeneBrief With {
                        .Synonym = "Putative mRNA",
                        .Location = New NucleotideLocation With {
                            .left = atg,
                            .right = tga,
                            .Strand = transcriptLoci.Strand
                        }
                    }
                Else
                    relatedGene = orf.First
                End If
                Return TssCategory.pmTSS
            End If

            Dim tuloci As NucleotideLocation = transcriptLoci

            ' (iv) sense TSS：与蛋白编码基因同向，且位点位于其内部或与其重叠
            If isRNA Then
                Dim senseHits As GeneBrief() = (From g As GeneBrief In pttGenes
                                                Let relationship As SegmentRelationships = g.Location.GetRelationship(tuloci)
                                                Where (relationship = SegmentRelationships.Inside OrElse
                                                       relationship = SegmentRelationships.DownStreamOverlap OrElse
                                                       relationship = SegmentRelationships.UpStreamOverlap) AndAlso
                                                      g.Location.Strand = tuloci.Strand
                                                Select g).ToArray()
                If senseHits.Length > 0 Then
                    relatedGene = senseHits.First
                    Return TssCategory.seTSS
                End If
            End If

            ' (v) antisense TSS：位点位于基因内部，但方向相反
            If isRNA Then
                Dim antisenseHits As GeneBrief() = (From g As GeneBrief In pttGenes
                                                    Let relationship As SegmentRelationships = g.Location.GetRelationship(tuloci)
                                                    Where relationship = SegmentRelationships.Inside AndAlso
                                                          tuloci.Strand <> g.Location.Strand
                                                    Select g).ToArray()
                If antisenseHits.Length > 0 Then
                    relatedGene = antisenseHits.First
                    Return TssCategory.asTSS
                End If
            End If

            ' 未找到任何相关基因
            relatedGene = New GeneBrief With {
                .Synonym = "null",
                .Location = New NucleotideLocation With {
                    .left = -1,
                    .right = -1,
                    .Strand = Strands.Unknown
                }
            }

            If isRNA Then
                Return TssCategory.seTSS
            End If
            Return TssCategory.UnClassified
        End Function

        ''' <summary>
        ''' 在给定核酸序列中查找最长的开放阅读框（ATG...终止密码子）。
        ''' 等价于原实现中的 Putative_mRNA(Nt, ATG, TGA, ORF)。
        ''' </summary>
        ''' <param name="sequence">大写核酸序列。</param>
        ''' <param name="atg">输出：ORF 的起始偏移（0-based，找不到为 -1）。</param>
        ''' <param name="tga">输出：ORF 的终止偏移。</param>
        Public Function Putative_mRNA(sequence As String, ByRef atg As Integer, ByRef tga As Integer) As Boolean
            atg = -1
            tga = -1
            If String.IsNullOrEmpty(sequence) Then Return False

            Dim bestLen As Integer = -1
            Dim bestAtg As Integer = -1
            Dim bestStop As Integer = -1

            For i As Integer = 0 To sequence.Length - 3
                If Not isStartCodon(sequence, i) Then Continue For
                For j As Integer = i + 3 To sequence.Length - 3 Step 3
                    If isStopCodon(sequence, j) Then
                        Dim len As Integer = j - i
                        If len > bestLen Then
                            bestLen = len
                            bestAtg = i
                            bestStop = j
                        End If
                        Exit For
                    End If
                Next
            Next

            If bestAtg < 0 Then Return False

            atg = bestAtg
            tga = bestStop
            Return True
        End Function

        Private Function isStartCodon(sequence As String, i As Integer) As Boolean
            Return sequence(i) = "A"c AndAlso sequence(i + 1) = "T"c AndAlso sequence(i + 2) = "G"c
        End Function

        Private Function isStopCodon(sequence As String, i As Integer) As Boolean
            Dim codon As String = sequence.Substring(i, 3)
            Return codon = "TAA" OrElse codon = "TAG" OrElse codon = "TGA"
        End Function

        Private Function lookupGene(ptt As PTT, synonym As String) As GeneBrief
            If ptt Is Nothing OrElse String.IsNullOrEmpty(synonym) Then Return Nothing
            Return ptt.GeneObject(synonym)
        End Function

        ''' <summary>
        ''' 把字符串解析为 <see cref="TssCategory"/>（用于回读结果文件）。
        ''' </summary>
        Public Function GetCType(str As String) As TssCategory
            Select Case str
                Case "sTSS" : Return TssCategory.sTSS
                Case "seTSS" : Return TssCategory.seTSS
                Case "asTSS" : Return TssCategory.asTSS
                Case "mTSS" : Return TssCategory.mTSS
                Case "pmTSS" : Return TssCategory.pmTSS
                Case "lmTSS" : Return TssCategory.lmTSS
                Case "ULmTSS" : Return TssCategory.ULmTSS
                Case Else : Return TssCategory.UnClassified
            End Select
        End Function

        ''' <summary>
        ''' 把字符串解析为 <see cref="GeneTypes"/>。
        ''' </summary>
        Public Function ParseGeneType(str As String) As GeneTypes
            If String.Equals(str, GeneTypes.CDS.ToString, System.StringComparison.OrdinalIgnoreCase) Then
                Return GeneTypes.CDS
            End If
            Return GeneTypes.misc_RNA
        End Function

    End Module

End Namespace
