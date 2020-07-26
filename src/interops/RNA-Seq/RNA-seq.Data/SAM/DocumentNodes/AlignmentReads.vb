#Region "Microsoft.VisualBasic::b4d584c5828264395ff50d640d4eead1, RNA-Seq\RNA-seq.Data\SAM\DocumentNodes\AlignmentReads.vb"

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

    '     Class AlignmentReads
    ' 
    '         Properties: CIGAR, FLAG, IsUnmappedReads, LowQuality, MAPQ
    '                     OptionalTable, PNEXT, POS, QNAME, QUAL
    '                     RNAME, RNEXT, Strand, TLEN
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: GenerateDocumentLine, GetBitFLAGSDescriptions, GetLocation, HaveFLAG, (+2 Overloads) RangeAt
    '                   ToString
    ' 
    '         Sub: CIGARParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace SAM

    ''' <summary>
    ''' 比对区域
    ''' 
    ''' Each alignment line has 11 mandatory fields. These fields always appear in the same order and must
    ''' be present, but their values can be '0' or '*' (depending on the field) if the corresponding information
    ''' is unavailable. 
    ''' 
    ''' The following table gives an overview of the mandatory fields in the SAM format:
    ''' 
    ''' Col  Field  Type    Regexp/Range                Brief description
    '''   1  QNAME  String  [!-?A-~]{1,255}             Query template NAME
    '''   2  FLAG   Int     [0,216-1]                   bitwise FLAG
    '''   3  RNAME  String  \*|[!-()+-&lt;>-~][!-~]*    Reference sequence NAME
    '''   4  POS    Int     [0,229-1]                   1-based leftmost mapping POSition
    '''   5  MAPQ   Int     [0,28-1]                    MAPping Quality
    '''   6  CIGAR  String  \*|([0-9]+[MIDNSHPX=])+     CIGAR string
    '''   7  RNEXT  String  \*|=|[!-()+-&lt;>-~][!-~]*  Ref. name of the mate/next segment
    '''   8  PNEXT  Int     [0,229-1]                   Position of the mate/next segment
    '''   9  TLEN   Int     [-229+1,229-1]              observed Template LENgth
    '''  10  SEQ    String  \*|[A-Za-z=.]+              segment SEQuence
    '''  11  QUAL   String  [!-~]+                      ASCII of Phred-scaled base QUALity+33
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AlignmentReads : Inherits ISequenceModel

#Region "Mandatory Fields.(有11个必填的字段)"

        ''' <summary>
        ''' Query template NAME.(Query template NAME. Reads/segments having identical QNAME are regarded to
        ''' come from the same template. A QNAME '*' indicates the information is unavailable.)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property QNAME As String

#Region "bitwise FLAG"

        ''' <summary>
        ''' bitwise FLAG
        ''' 
        ''' bitwise FLAG. Each bit is explained in the following table:
        ''' 
        '''   Bit    Description
        '''   0x1    template having multiple segments in sequencing
        '''   0x2    each segment properly aligned according to the aligner
        '''   0x4    segment unmapped
        '''   0x8    next segment in the template unmapped
        '''  0x10    SEQ being reverse complemented
        '''  0x20    SEQ of the next segment in the template being reversed
        '''  0x40    the first segment in the template
        '''  0x80    the last segment in the template
        ''' 0x100    secondary alignment
        ''' 0x200    Not passing quality controls
        ''' 0x400    PCR Or optical duplicate
        ''' 
        ''' Bit 0x4 Is the only reliable place to tell whether the segment Is unmapped. If 0x4 Is set, 
        ''' no assumptions can be made about RNAME, POS, CIGAR, MAPQ, bits 0x2, 0x10 And 0x100 And the 
        ''' bit 0x20 of the next segment in the template.
        ''' 
        ''' If 0x40 And 0x80 are both set, the segment Is part of a linear template, but it Is neither 
        ''' the first nor the last segment. If both 0x40 And 0x80 are unset, the index of the segment 
        ''' in the template Is unknown. This may happen For a non-linear template Or the index Is lost 
        ''' in data processing.
        ''' 
        ''' Bit 0x100 marks the alignment Not to be used in certain analyses when the tools in use are aware of this bit.
        ''' If 0x1 Is unset, no assumptions can be made about 0x2, 0x8, 0x20, 0x40 And 0x80.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' 可以从这个参数之中了解Reads的方向
        ''' 
        ''' Sequence reads are always from 5' to 3', but the forward and reversed reactions are running along the opposite strands. 
        ''' The 2 complementary DNA strands are oriented in opposite orientation, and sequence reads from either end are generating 
        ''' results of those 2 different strands. See this page here - it shows DNA replication, but that follows the same logic 
        ''' as DNA sequencing
        '''
        ''' 
        ''' Question: In Sam Format, Clarify The Meaning Of The "0" Flag.
        ''' 
        ''' Hello, I have 40,637 short sequences (probes) in a fastq file named "seq.fq".
        ''' 
        ''' • First, I mapped them against a reference genome (hg19) using BWA ("bwa aln ...").
        ''' • Then, I converted the alignments from suffix-array coordinates into chromosomal coordinates ("bwa samse ..."), 
        '''   And obtained the results into a SAM file named "seq_aln.sam".
        ''' • Finally, I counted the number of occurrences for each flag
        ''' 
        ''' $ grep -v "@" seq_aln.sam | awk -F"\t" 'BEGIN{print "flag\toccurrences"} {a[$2]++} END{for(i in a)print i"\t"a[i]}'
        ''' 
        '''   flag......occurrences
        ''' 
        '''   4.........3083
        '''   0.........19039
        '''  16.........18515
        ''' 
        ''' According to this page, the "4" flag means that the short sequence doesn't map onto the reference genome, 
        ''' and the "16" flag means that the short sequence does map on the reverse strand of the reference genome.
        ''' 
        ''' But, what does the "0" flag mean? According to this forum page, it means "the read is not paired and mapped, 
        ''' forward strand", which Is unclear to me... Does it mean "it is not paired but it maps on forward strand"? 
        ''' Or "it is neither paired nor maps on forward strand"? Or "it is neither paired nor maps on any strand"?
        ''' 
        ''' At the End, does all this mean that I can work With only 18,515 Short sequences out Of 40,637?
        ''' 
        ''' Thanks for your help!
        ''' 
        ''' 
        ''' Anwser Comments:
        ''' 
        ''' When the flag field Is 0, it means none of the bitwise flags specified in the SAM spec (on page 4) are set. 
        ''' That means that your reads with flag 0 are unpaired (because the first flag, 0x1, Is Not set), 
        ''' successfully mapped to the reference (because 0x4 Is Not set) And mapped to the forward strand (because 0x10 Is Not set).
        ''' 
        ''' Summarizing your data, the reads With flag 4 are unmapped, the reads With flag 0 are mapped To the forward strand 
        ''' And the reads With flag 16 are mapped To the reverse strand.
        ''' 
        ''' </remarks>
        Public Property FLAG As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return _Flag
            End Get
            Set(value As Integer)
                _Flag = value
                _Flags = ComputeBitFLAGS(value)
            End Set
        End Property

        Public ReadOnly Property IsUnmappedReads As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return HaveFLAG(BitFlags.Bit0x4)
            End Get
        End Property

        Public ReadOnly Property LowQuality As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return HaveFLAG(BitFlags.Bit0x200)
            End Get
        End Property

        Dim _Flags As BitFlags(), _Flag As Integer

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function HaveFLAG(FLAG As BitFlags) As Boolean
            Return Array.IndexOf(_Flags, FLAG) > -1
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetBitFLAGSDescriptions() As String
            Return GetBitFLAGDescriptions(Me._Flags)
        End Function

        ''' <summary>
        ''' 获取当前的这条reads在基因组之上的方向
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Strand As Strands
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                If Array.IndexOf(Me._Flags, BitFlags.Bit0x10) > -1 Then
                    Return Strands.Reverse  'Reads序列已经被反向互补了
                ElseIf Array.IndexOf(Me._Flags, BitFlags.Bit0x4) > -1 Then   '没有被Mapping到，则无法判断
                    Return Strands.Unknown
                Else  '剩余的情况都是正向的了
                    Return Strands.Forward
                End If
            End Get
        End Property
#End Region

        ''' <summary>
        ''' Reference sequence NAME
        ''' 
        ''' RNAME: Reference sequence NAME of the alignment. If @SQ header lines are present, RNAME
        ''' (if Not `*') must be present in one of the SQ-SN tag. An unmapped segment without coordinate
        ''' has a `*' at this field. However, an unmapped segment may also have an ordinary coordinate
        ''' such that it can be placed at a desired position after sorting. If RNAME Is `*', no assumptions
        ''' can be made about POS And CIGAR.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RNAME As String
        ''' <summary>
        ''' 1-based leftmost mapping POSition.(本Mapping在参考基因组之中的最左端的位置)
        ''' 
        ''' POS: 1-based leftmost mapping POSition of the first matching base. The first base in a reference
        ''' sequence has coordinate 1. POS Is Set As 0 For an unmapped read without coordinate. If POS Is
        ''' 0, no assumptions can be made about RNAME And CIGAR.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property POS As Integer
        ''' <summary>
        ''' MAPping Quality
        ''' 
        ''' MAPQ: MAPping Quality. It equals -10 log10 Prfmapping position is wrongg, rounded to the
        ''' nearest integer. A value 255 indicates that the mapping quality Is Not available.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MAPQ As Integer
        ''' <summary>
        ''' CIGAR string
        ''' 
        ''' CIGAR: CIGAR string. The CIGAR operations are given in the following table (set `*' if unavailable):
        ''' 
        '''  Op  BAM  Description
        '''   M   0   alignment match (can be a sequence match Or mismatch)
        '''   I   1   insertion to the reference
        '''   D   2   deletion from the reference
        '''   N   3   skipped region from the reference
        '''   S   4   soft clipping (clipped sequences present in SEQ)
        '''   H   5   hard clipping (clipped sequences Not present in SEQ)
        '''   P   6   padding (silent deletion from padded reference)
        '''   =   7   sequence match
        '''   X   8   sequence mismatch
        ''' 
        ''' H can only be present As the first And/Or last operation.
        ''' S may only have H operations between them And the ends Of the CIGAR String.
        ''' For mRNA -To -genome alignment, an N operation represents an intron. For other types Of alignments, the interpretation of N Is Not defined.
        ''' Sum of lengths of the M/I/S/=/X operations shall equal the length of SEQ.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CIGAR As String

        Public Sub CIGARParser(ByRef paraValue As Integer, ByRef opr As CIGAROperations)

        End Sub

        ''' <summary>
        ''' Ref. name of the mate/next segment.
        ''' 
        ''' RNEXT: Reference sequence name of the NEXT segment in the template. For the last segment,
        ''' the next segment Is the first segment in the template. If @SQ header lines are present, RNEXT
        ''' (if Not `*' or `=') must be present in one of the SQ-SN tag. This field is set as `*' when the
        ''' information Is unavailable, And set as `=' if RNEXT is identical RNAME. If not `=' and the next
        ''' segment in the template has one primary mapping (see also bit <see cref="BitFlags.Bit0x100">0x100</see> in FLAG), this field Is
        ''' identical to RNAME of the next segment. If the next segment has multiple primary mappings,
        ''' no assumptions can be made about RNEXT And PNEXT. If RNEXT Is `*', no assumptions can
        ''' be made On PNEXT And bit <see cref="BitFlags.Bit0x20">0x20</see>.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RNEXT As String
        ''' <summary>
        ''' Position of the mate/next segment
        ''' 
        ''' PNEXT: Position of the NEXT segment in the template. Set as 0 when the information is
        ''' unavailable. This field equals POS of the next segment. If PNEXT Is 0, no assumptions can be
        ''' made on RNEXT And bit 0x20.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PNEXT As Integer
        ''' <summary>
        ''' observed Template LENgth
        ''' 
        ''' TLEN: signed observed Template LENgth. If all segments are mapped to the same reference, the
        ''' unsigned observed template length equals the number Of bases from the leftmost mapped base
        ''' to the rightmost mapped base. The leftmost segment has a plus sign And the rightmost has a
        ''' minus sign. The sign Of segments In the middle Is undefined. It Is Set As 0 For Single-segment
        ''' template Or when the information Is unavailable.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TLEN As Integer
        ''' <summary>
        ''' ASCII of Phred-scaled base QUALity+33
        ''' 
        ''' QUAL: ASCII of base QUALity plus 33 (same as the quality string in the Sanger FASTQ format).
        ''' A base quality Is the phred-scaled base Error probability which equals -10 log10 Prfbase Is wrongg.
        ''' This field can be a `*' when quality is not stored. If not a `*', SEQ must not be a `*' and the
        ''' length of the quality string ought to equal the length of SEQ.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property QUAL As String

#End Region

        ''' <summary>
        ''' 最后面的可选的字段数据
        ''' 
        ''' 可选字段（optional fields)，格式如：TAG:TYPE:VALUE，其中TAG有两个大写字母组成，每个TAG代表一类信息，
        ''' 每一行一个TAG只能出现一次，TYPE表示TAG对应值的类型，可以是字符串、整数、字节、数组等。
        '''
        ''' 要注意的几个概念，以及与之对应的模型
        ''' 
        '''  ◾reference
        '''  ◾read
        '''  ◾segment
        '''  ◾template（参考序列和比对上的序列共同组成的序列为template）
        '''  ◾alignment
        '''  ◾seq
        '''
        ''' </summary>
        ''' <returns></returns>
        Public Property OptionalTable As Dictionary(Of String, KeyValuePair)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="str"></param>
        ''' <remarks>
        ''' 不可用的片段信息会使用0或者*来表示
        ''' </remarks>
        Sub New(str As String)
            Dim arr$() = str.Split(ASCII.TAB)
            Dim n = arr.Length
            Dim i As i32 = 0

            QNAME = arr(++i)
            FLAG = CInt(Val(arr(++i)))
            RNAME = arr(++i)
            POS = CInt(Val(arr(++i)))
            MAPQ = CInt(Val(arr(++i)))
            CIGAR = arr(++i)
            RNEXT = arr(++i)
            PNEXT = CInt(Val(arr(++i)))
            TLEN = CInt(Val(arr(++i)))
            SequenceData = arr(++i)
            QUAL = arr(++i)

            If arr.Length > 11 Then
                With From s As String
                     In arr.Skip(11)
                     Let t = s.Split(":"c)
                     Where t.Length >= 3
                     Select tag = t(0), type = t(1), value = t(2) '

                    OptionalTable = .ToDictionary(
                        Function(t) t.tag,
                        Function(attr)
                            Return New KeyValuePair With {
                                .Key = attr.type,
                                .Value = attr.value
                            }
                        End Function)
                End With
            Else
                ' table is nothing
            End If
        End Sub

        Sub New()
        End Sub

        Public Function GenerateDocumentLine() As String
            Dim array$() = New String() {QNAME, FLAG, RNAME, POS, MAPQ, CIGAR, RNEXT, PNEXT, TLEN, SequenceData, QUAL}
            Dim attrs = OptionalTable _
                .SafeQuery _
                .Select(Function(attr)
                            Return $"{attr.Key}:{attr.Value.Key}:{attr.Value.Value}"
                        End Function) _
                .AsList

            Return (array + attrs).JoinBy(ASCII.TAB)
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} ({1} ~)   {2}", Me.QNAME, Me.POS, SequenceData) & "   //BitFLAGS=" & Me.GetBitFLAGSDescriptions
        End Function

        ''' <summary>
        ''' 查看这个Read是否在该范围之内，由于只是对片段区域内的每一个碱基上面的Reads频数进行计数，所以这里只需要二者只要有重叠就可以了
        ''' </summary>
        ''' <param name="RangeStart"></param>
        ''' <param name="RangeEnds"></param>
        ''' <returns></returns>
        Public Function RangeAt(RangeStart As Long, RangeEnds As Long) As Boolean
            Dim Ranges As New NucleotideLocation(RangeStart, RangeEnds, False)
            Return RangeAt(Ranges)
        End Function

        Public Function RangeAt(Ranges As NucleotideLocation) As Boolean
            Dim MyLoc = NucleotideLocation.CreateObject(POS, Length, Strand)
            Dim r As SegmentRelationships = Ranges.GetRelationship(MyLoc)

            Return r = SegmentRelationships.Equals OrElse
                r = SegmentRelationships.Inside OrElse
                r = SegmentRelationships.DownStreamOverlap OrElse
                r = SegmentRelationships.UpStreamOverlap
        End Function

        Public Function GetLocation() As NucleotideLocation
            With {PNEXT, POS}
                Return New NucleotideLocation(.Min, .Max, Strand)
            End With
        End Function
    End Class
End Namespace
