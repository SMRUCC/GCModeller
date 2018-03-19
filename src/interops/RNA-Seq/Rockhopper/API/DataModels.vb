#Region "Microsoft.VisualBasic::debaa0852b1ebd4f8997d33ccd5df1c3, RNA-Seq\Rockhopper\API\DataModels.vb"

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

    '     Class Operon
    ' 
    '         Properties: [Stop], Genes, NumberOfGenes, Start, Strand
    ' 
    '         Function: ToString
    ' 
    '     Class Transcripts
    ' 
    '         Properties: ATG, Expression, Is_sRNA, IsPredictedRNA, IsRNA
    '                     Leaderless, Minus35BoxLoci, Name, Product, Strand
    '                     Synonym, TGA, TranscriptLength, TSSs, TTSs
    ' 
    '         Function: FromReadsMap, GenerateTranscripts, GetTULoci, InterGenicTranscript
    '         Enum Categories
    ' 
    '             asTSS, lmTSS, pmTSS, seTSS, sTSS
    '             ULmTSS
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: Get5UTRLeader, GetLociStrand, GetPromoterBoxLoci, GetTSSLoci, GetTTSsLoci
    '               ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports LANS.SystemsBiology.ComponentModel.Loci

Namespace AnalysisAPI

    Public Class Operon
        Public Property Start As Long
        Public Property [Stop] As Long
        Public Property Strand As String
        Public ReadOnly Property NumberOfGenes As Integer
            Get
                Return Genes.Count
            End Get
        End Property
        <CollectionAttribute("Genes", ", ")> Public Property Genes As String()

        Public Overrides Function ToString() As String
            Return $"[{Strand}]{Start},{[Stop]};    { String.Join(", ", Genes)}"
        End Function
    End Class

    ''' <summary>
    ''' 从RNA-seq数据之中分析出来的基因结构
    ''' </summary>
    Public Class Transcripts

        Public Shared Function FromReadsMap(Map As TSSAR.Reads.GeneAssociationView(),
                                            PTT As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT) As Transcripts()

            Dim LQuery = (From item In Map Select item, GeneID = item.AssociatedGene Group By GeneID Into Group).ToArray
            Dim Transcripts = (From Gene In LQuery
                               Where Not String.IsNullOrEmpty(Gene.GeneID)  '空的基因号表示可能在基因间隔区，则需要单独拿出来处理
                               Select GenerateTranscripts((From item In Gene.Group Select item.item).ToArray, PTT)).ToArray.MatrixToList
            Call Transcripts.AddRange(InterGenicTranscript((From Gene In LQuery Where String.IsNullOrEmpty(Gene.GeneID) Select (From item In Gene.Group Select item.item).ToArray).ToArray.MatrixToVector))
            Return Transcripts.ToArray
        End Function

        Private Shared Function InterGenicTranscript(Map As TSSAR.Reads.GeneAssociationView()) As Transcripts()
            Return (From item In Map Select New Transcripts With {.Strand = If(item.POS < item.PNEXT, "+", "-"), .TSSs = item.POS, .Expression = item.NumberOfReads}).ToArray
        End Function

        ''' <summary>
        ''' 都是同一个相同的基因号的
        ''' </summary>
        ''' <param name="Map"></param>
        ''' <returns></returns>
        Private Shared Function GenerateTranscripts(Map As TSSAR.Reads.GeneAssociationView(),
                                                    PTT As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT) As Transcripts()
            Dim GenePTT = PTT.GeneObject(Map.First.AssociatedGene)
            '上游或者上游重叠为TSSs
            '下游重叠为TTS
            '其他的关系抛弃掉
            Dim TSSs = (From item In Map
                        Where item.Position = SegmentRelationships.UpStream OrElse item.Position = SegmentRelationships.UpStreamOverlap
                        Select item).ToArray
            Dim TTS = (From item In Map
                       Where item.Position = SegmentRelationships.DownStreamOverlap
                       Select item).ToArray
            Dim LQuery = (From item In TSSs Select New Transcripts With {.ATG = GenePTT.ATG, .TGA = GenePTT.TGA, .Name = GenePTT.Gene, .Product = GenePTT.Product, .Strand = If(GenePTT.Location.Strand = Strands.Forward, "+", "-"), .Synonym = GenePTT.Synonym, .TSSs = If(GenePTT.Location.Strand = Strands.Forward, item.POS, item.PNEXT), .TTSs = GenePTT.TGA, .Expression = item.NumberOfReads}).ToList
            Call LQuery.AddRange((From item In TTS Select New Transcripts With {.ATG = GenePTT.ATG, .TGA = GenePTT.TGA, .Name = GenePTT.Gene, .Product = GenePTT.Product, .Strand = If(GenePTT.Location.Strand = Strands.Forward, "+", "-"), .Synonym = GenePTT.Synonym, .TSSs = 0, .TTSs = If(GenePTT.Location.Strand = Strands.Forward, item.PNEXT, item.POS), .Expression = item.NumberOfReads}).ToList)
            Call LQuery.AddRange((From item In (From r In Map Where r.Position = SegmentRelationships.DownStream OrElse r.Position = SegmentRelationships.Inside Select r).ToArray Select New Transcripts With {.ATG = 0, .Strand = GenePTT.Location.Strand,
                                                                                                                                                                                                                                                                     .TSSs = item.POS, .Expression = item.NumberOfReads}).ToArray)
            Return LQuery.ToArray
        End Function

        ''' <summary>
        ''' Transcription Start Sites
        ''' </summary>
        ''' <returns></returns>
        <Column("Transcription Start")> Public Property TSSs As Long
        <Column("Translation Start")> Public Property ATG As Long
        <Column("Translation Stop")> Public Property TGA As Long
        ''' <summary>
        ''' Transcription Stop Sites
        ''' </summary>
        ''' <returns></returns>
        <Column("Transcription Stop")> Public Property TTSs As Long
        ''' <summary>
        ''' Strand of the Gene <see cref="Synonym"/> on the double strand DNA
        ''' </summary>
        ''' <returns></returns>
        Public Property Strand As String
        Public Property Name As String
        ''' <summary>
        ''' GeneID
        ''' </summary>
        ''' <returns></returns>
        Public Property Synonym As String
        Public Property Product As String
        ''' <summary>
        ''' Gene RPKM
        ''' </summary>
        ''' <returns></returns>
        Public Property Expression As Long

        ''' <summary>
        ''' ATG为0就不会是ORF了
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsRNA As Boolean
            Get
                Return ATG <= 0
            End Get
        End Property

        ''' <summary>
        ''' TSS和ATG重叠在一起
        ''' </summary>
        ''' <returns>是绝对的要重叠在一起还是可以有1两个碱基的偏差呢？</returns>
        Public ReadOnly Property Leaderless As Boolean
            Get
                '有一些预测不出来的位置的值是0，这些也可能会相等的，是错误的数据
                Return (TSSs > 0 AndAlso ATG > 0) AndAlso TSSs = ATG  'lmTSS， 两个位点绝对的重叠在一起
                'Return Math.Abs(TSSs - ATG) < 2 '允许有一些碱基的偏差
            End Get
        End Property

        Public ReadOnly Property Minus35BoxLoci As Integer
            Get
                If TSSs <= 0 Then '预测不出来，则没有这个-35区的位点
                    Return -1
                End If

                If GetStrand(Strand) = Strands.Forward Then
                    Return TSSs - 60
                Else
                    Return TSSs + 60
                End If
            End Get
        End Property

        Public ReadOnly Property IsPredictedRNA As Boolean
            Get
                Return String.IsNullOrEmpty(Synonym) OrElse String.Equals("predicted RNA", Synonym)
            End Get
        End Property

        Public ReadOnly Property TranscriptLength As Integer
            Get
                Return GetTULoci.FragmentSize
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        Const SRNA_LEN As Integer = 150

        ''' <summary>
        ''' 长度小于<see cref="SRNA_LEN"/>的RNA分子
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Is_sRNA As Boolean
            Get
                If ATG = 0 Then
                    Return TranscriptLength <= SRNA_LEN
                Else
                    Return False
                End If
            End Get
        End Property

        ''' <summary>
        ''' 获取这个Transcript的基因组之上的位置
        ''' </summary>
        ''' <returns></returns>
        Public Function GetTULoci() As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation
            Dim Start As Integer = If(TSSs = 0, ATG, TSSs) '有些TSS位点是预测不出来的，则使用ATG位点来替代
            Dim [Stop] As Integer = If(TTSs = 0, TGA, TTSs)
            Dim Loci = New LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation(Start, [Stop], GetLociStrand)
            Return Loci
        End Function

        ''' <summary>
        ''' Figure 1 Identification of transcription start sites (TSS) in the S. meliloti genome. Schematic showing 
        ''' parameters of TSS types based on the minimal transcription unit model described In Methods. 
        ''' (i)   TSS Of an mRNA. 
        ''' (ii)  TSS Of a leaderless transcript. 
        ''' (iii) TSS Of a putative mRNA.
        ''' (iv)  TSS of a sense transcript. 
        ''' (v)   TSS of a cis-encoded antisense RNA. 
        ''' (vi)  TSS of a trans-encoded sRNA.
        ''' </summary>
        ''' <remarks>
        ''' Grouped into six categories based on their genomic context with respect to a minimal transcription unit (MTU) model. 
        ''' (根据他们的基因组之中的最小转录单元模型（MTU）的内容，可以将预测得到的结果分为6种类型)
        ''' 
        ''' TSS representing the prominent 50 End Of a sequence contig overlapping In sense orientation a region of 54 nt 
        ''' upstream of the start codon of proteincoding genes were classified As (i) TSS Of mRNAs (mTSS). 54 nt were defined as 
        ''' the minimal region upstream of the ATG to cover promoter motifs And the ribosome binding site, which are more likely 
        ''' To be associated With an mRNA than To a trans-encoded sRNA.
        ''' (mTSS是ATG上游的越54bp处的TSS位点,54bp的长度是ATG上游之中能够包含有启动子和RBS的最小长度，相较于小RNA其更可能与mRNA转录相关，请注意54bp只是最小的区域，也就是说这个数值还可以再增大)
        ''' 
        ''' (ii) TSS were assigned to leaderless transcripts (lmTSS), If matching the first nucleotide Of the translation initiation
        ''' codon. 
        ''' （lmTSS是指TSS位点和ATG位点重叠在一起的片段）
        ''' 
        ''' The class of (iii) putative TSS of mRNAs (pmTSS) comprises TSS that are difficult to distinguish For any given pmTSS, 
        ''' it Is uncertain if the TSS represents an mRNA Of a protein-coding gene possessing a Long 50-UTR Or a TSS of a trans-encoded sRNA. 
        ''' (pmTSS假定TSS指的是无法区分的：这个TSS位点是mRNA还是小RNA的TSS)
        ''' 
        ''' (iv) Sense TSS (seTSS) represent internal transcripts in the same orientation as, And located within, protein-coding genes. 
        ''' (seTSS，是orf内部的与orf相同方向的转录的TSS)
        ''' 
        ''' (v) TSS of cis-encoded antisense RNAs (asTSS) are orientated in antisense to protein-coding target genes
        ''' (asTSS则是在反方向的)
        ''' 
        ''' (vi) TSS of trans-encoded sRNAs (sTSS) are located in intergenic regions(IGR) And within a defined distance from neighboring genes.
        ''' (反式编码的小RNA分子的sTSS是在基因间隔区的，与相邻的基因有一个特定的设定值的距离)
        ''' 
        ''' </remarks>
        Public Enum Categories As Integer

            ''' <summary>
            ''' 无法进行TSS的分类定义的
            ''' </summary>
            UnClassified = -100

            ''' <summary>
            ''' (i)   TSS Of an mRNA. 
            ''' </summary>
            ''' <remarks>OK</remarks>>
            mTSS = 1
            ''' <summary>
            ''' (ii)  TSS Of a leaderless transcript.(ATG和TSS这两个位点都重叠在一起的) 
            ''' </summary>
            ''' <remarks>OK</remarks>
            lmTSS
            ''' <summary>
            ''' Ultra Long mRNA TSSs.(被注释为属于mRNA的但是TSS位点的距离大于300但是小于500的)
            ''' </summary>
            ULmTSS
            ''' <summary>
            ''' (iii) TSS Of a putative mRNA.
            ''' </summary>
            pmTSS
            ''' <summary>
            ''' (v)   TSS of a cis-encoded antisense RNA.(在基因内部的，但是方向和ORF的转录方向相反的RNA分子) 
            ''' </summary>
            ''' 
            ''' <remarks>OK</remarks>
            asTSS
            ''' <summary>
            ''' (iv)  TSS of a sense transcript. 
            ''' </summary>
            ''' 
            ''' <remarks>OK</remarks>
            seTSS
            ''' <summary>
            ''' (vi)  TSS of a trans-encoded sRNA.
            ''' </summary>
            ''' <remarks>OK</remarks>
            sTSS
        End Enum

        Public Function GetLociStrand() As LANS.SystemsBiology.ComponentModel.Loci.Strands
            Return GetStrand(Me.Strand)
        End Function

#Region "Sequence Parser Data Source"

        ''' <summary>
        ''' TSSs到ATG之间的5‘UTR
        ''' </summary>
        ''' <param name="Parser"></param>
        ''' <returns></returns>
        Public Function Get5UTRLeader(Parser As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader) As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
            Dim Loci As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation

            If TSSs <= 0 OrElse Me.Leaderless OrElse ATG <= 0 Then
                Return Nothing
            End If

            If GetStrand(Strand) = Strands.Forward Then
                Loci = New LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation(TSSs, ATG)
            Else
                Loci = New LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation(ATG, TSSs, True) '在正向链之上，则是相反的位置
            End If

            Dim SequenceData As String = Parser.TryParse(Loci).SequenceData
            Dim Fasta = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {
                             .SequenceData = SequenceData,
                             .Attributes = {Synonym, Me.GetTULoci.ToString, "ATG=" & ATG, "TSSs=" & TSSs}}
            Return Fasta
        End Function

        Public Function GetTSSLoci(Parser As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader) As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
            If TSSs <= 0 Then
                Return Nothing
            End If

            Dim Loci As NucleotideLocation =
                New NucleotideLocation(
                    TSSs - 5,'TSS上下游各5bp
                    TSSs + 5,
                    GetStrand(Strand) = Strands.Reverse)
            Dim SequenceData As String = Parser.TryParse(Loci).SequenceData
            Dim Fasta As New SequenceModel.FASTA.FastaToken With {
                .SequenceData = SequenceData,
                .Attributes = {Synonym, Me.GetTULoci.ToString, "TSSs=" & TSSs}
            }
            Return Fasta
        End Function

        ''' <summary>
        ''' 解析出TGA到TTS之间的序列片段   3'UTR
        ''' </summary>
        ''' <returns></returns>
        Public Function GetTTSsLoci(Parser As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader) As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
            Dim Loci As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation

            If TTSs <= 0 OrElse TGA <= 0 Then
                Return Nothing
            End If

            If GetStrand(Strand) = Strands.Forward Then
                Loci = New LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation(TGA, TTSs) '假若是在正向链之上，则不做任何处理，首先是TGA，再到TTS
            Else
                Loci = New LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation(TTSs, TGA, True) '在正向链之上，则是相反的位置
            End If

            Dim SequenceData As String = Parser.TryParse(Loci).SequenceData
            Dim Fasta As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken =
                New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {
                    .SequenceData = SequenceData,
                    .Attributes =
                    {
                        Synonym,
                        Me.GetTULoci.ToString,
                        "TGA=" & TGA,
                        "TTS=" & TTSs
                }
            }
            Return Fasta
        End Function

        ''' <summary>
        ''' -35区到-10区的序列片段
        ''' </summary>
        ''' <returns></returns>
        Public Function GetPromoterBoxLoci(Reader As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader) As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
            Dim Loci As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation

            If TSSs <= 0 Then
                Return Nothing
            End If

            If GetStrand(Strand) = Strands.Forward Then
                Loci = New LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation(Minus35BoxLoci, TSSs, False) '假若是在正向链之上，则不做任何处理
            Else
                Loci = New LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation(TSSs, Minus35BoxLoci, True) '在正向链之上，-35区的位置是在TSS的下游的
            End If

            Dim SequenceData As String = Reader.TryParse(Loci).SequenceData
            Dim Fasta As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken =
                New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {
                    .SequenceData = SequenceData,
                    .Attributes =
                    {
                        Synonym,
                        Me.GetTULoci.ToString,
                        "TSSs=" & TSSs,
                        "-35=" & Minus35BoxLoci
                }
            }
            Return Fasta
        End Function
#End Region

        Public Overrides Function ToString() As String
            Return Me.Synonym
        End Function
    End Class
End Namespace
