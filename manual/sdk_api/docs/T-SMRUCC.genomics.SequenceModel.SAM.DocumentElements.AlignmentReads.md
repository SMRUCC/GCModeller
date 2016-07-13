---
title: AlignmentReads
---

# AlignmentReads
_namespace: [SMRUCC.genomics.SequenceModel.SAM.DocumentElements](N-SMRUCC.genomics.SequenceModel.SAM.DocumentElements.html)_

比对区域
 
 Each alignment line has 11 mandatory fields. These fields always appear in the same order and must
 be present, but their values can be '0' or '*' (depending on the field) if the corresponding information
 is unavailable. 
 
 The following table gives an overview of the mandatory fields in the SAM format:
 
 Col Field Type Regexp/Range Brief description
 1 QNAME String [!-?A-~]{1,255} Query template NAME
 2 FLAG Int [0,216-1] bitwise FLAG
 3 RNAME String \*|[!-()+-<>-~][!-~]* Reference sequence NAME
 4 POS Int [0,229-1] 1-based leftmost mapping POSition
 5 MAPQ Int [0,28-1] MAPping Quality
 6 CIGAR String \*|([0-9]+[MIDNSHPX=])+ CIGAR string
 7 RNEXT String \*|=|[!-()+-<>-~][!-~]* Ref. name of the mate/next segment
 8 PNEXT Int [0,229-1] Position of the mate/next segment
 9 TLEN Int [-229+1,229-1] observed Template LENgth
 10 SEQ String \*|[A-Za-z=.]+ segment SEQuence
 11 QUAL String [!-~]+ ASCII of Phred-scaled base QUALity+33



### Methods

#### #ctor
```csharp
SMRUCC.genomics.SequenceModel.SAM.DocumentElements.AlignmentReads.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|str|-|

> 
>  不可用的片段信息会使用0或者*来表示
>  

#### RangeAt
```csharp
SMRUCC.genomics.SequenceModel.SAM.DocumentElements.AlignmentReads.RangeAt(System.Int64,System.Int64)
```
查看这个Read是否在该范围之内，由于只是对片段区域内的每一个碱基上面的Reads频数进行计数，所以这里只需要二者只要有重叠就可以了

|Parameter Name|Remarks|
|--------------|-------|
|RangeStart|-|
|RangeEnds|-|



### Properties

#### CIGAR
CIGAR string
 
 CIGAR: CIGAR string. The CIGAR operations are given in the following table (set `*' if unavailable):
 
 Op BAM Description
 M 0 alignment match (can be a sequence match Or mismatch)
 I 1 insertion to the reference
 D 2 deletion from the reference
 N 3 skipped region from the reference
 S 4 soft clipping (clipped sequences present in SEQ)
 H 5 hard clipping (clipped sequences Not present in SEQ)
 P 6 padding (silent deletion from padded reference)
 = 7 sequence match
 X 8 sequence mismatch
 
 H can only be present As the first And/Or last operation.
 S may only have H operations between them And the ends Of the CIGAR String.
 For mRNA -To -genome alignment, an N operation represents an intron. For other types Of alignments, the interpretation of N Is Not defined.
 Sum of lengths of the M/I/S/=/X operations shall equal the length of SEQ.
#### FLAG
bitwise FLAG
 
 bitwise FLAG. Each bit is explained in the following table:
 
 Bit Description
 0x1 template having multiple segments in sequencing
 0x2 each segment properly aligned according to the aligner
 0x4 segment unmapped
 0x8 next segment in the template unmapped
 0x10 SEQ being reverse complemented
 0x20 SEQ of the next segment in the template being reversed
 0x40 the first segment in the template
 0x80 the last segment in the template
 0x100 secondary alignment
 0x200 Not passing quality controls
 0x400 PCR Or optical duplicate
 
 Bit 0x4 Is the only reliable place to tell whether the segment Is unmapped. If 0x4 Is set, 
 no assumptions can be made about RNAME, POS, CIGAR, MAPQ, bits 0x2, 0x10 And 0x100 And the 
 bit 0x20 of the next segment in the template.
 
 If 0x40 And 0x80 are both set, the segment Is part of a linear template, but it Is neither 
 the first nor the last segment. If both 0x40 And 0x80 are unset, the index of the segment 
 in the template Is unknown. This may happen For a non-linear template Or the index Is lost 
 in data processing.
 
 Bit 0x100 marks the alignment Not to be used in certain analyses when the tools in use are aware of this bit.
 If 0x1 Is unset, no assumptions can be made about 0x2, 0x8, 0x20, 0x40 And 0x80.
#### MAPQ
MAPping Quality
 
 MAPQ: MAPping Quality. It equals -10 log10 Prfmapping position is wrongg, rounded to the
 nearest integer. A value 255 indicates that the mapping quality Is Not available.
#### OptionalHash
最后面的可选的字段数据
 
 可选字段（optional fields)，格式如：TAG:TYPE:VALUE，其中TAG有两个大写字母组成，每个TAG代表一类信息，
 每一行一个TAG只能出现一次，TYPE表示TAG对应值的类型，可以是字符串、整数、字节、数组等。

 要注意的几个概念，以及与之对应的模型
 
 ◾reference
 ◾read
 ◾segment
 ◾template（参考序列和比对上的序列共同组成的序列为template）
 ◾alignment
 ◾seq
#### PNEXT
Position of the mate/next segment
 
 PNEXT: Position of the NEXT segment in the template. Set as 0 when the information is
 unavailable. This field equals POS of the next segment. If PNEXT Is 0, no assumptions can be
 made on RNEXT And bit 0x20.
#### POS
1-based leftmost mapping POSition.(本Mapping在参考基因组之中的最左端的位置)
 
 POS: 1-based leftmost mapping POSition of the first matching base. The first base in a reference
 sequence has coordinate 1. POS Is Set As 0 For an unmapped read without coordinate. If POS Is
 0, no assumptions can be made about RNAME And CIGAR.
#### QNAME
Query template NAME.(Query template NAME. Reads/segments having identical QNAME are regarded to
 come from the same template. A QNAME '*' indicates the information is unavailable.)
#### QUAL
ASCII of Phred-scaled base QUALity+33
 
 QUAL: ASCII of base QUALity plus 33 (same as the quality string in the Sanger FASTQ format).
 A base quality Is the phred-scaled base Error probability which equals -10 log10 Prfbase Is wrongg.
 This field can be a `*' when quality is not stored. If not a `*', SEQ must not be a `*' and the
 length of the quality string ought to equal the length of SEQ.
#### RNAME
Reference sequence NAME
 
 RNAME: Reference sequence NAME of the alignment. If @SQ header lines are present, RNAME
 (if Not `*') must be present in one of the SQ-SN tag. An unmapped segment without coordinate
 has a `*' at this field. However, an unmapped segment may also have an ordinary coordinate
 such that it can be placed at a desired position after sorting. If RNAME Is `*', no assumptions
 can be made about POS And CIGAR.
#### RNEXT
Ref. name of the mate/next segment.
 
 RNEXT: Reference sequence name of the NEXT segment in the template. For the last segment,
 the next segment Is the first segment in the template. If @SQ header lines are present, RNEXT
 (if Not `*' or `=') must be present in one of the SQ-SN tag. This field is set as `*' when the
 information Is unavailable, And set as `=' if RNEXT is identical RNAME. If not `=' and the next
 segment in the template has one primary mapping (see also bit @"F:SMRUCC.genomics.SequenceModel.SAM.DocumentElements.BitFLAGS.Bit0x100"[0x100] in FLAG), this field Is
 identical to RNAME of the next segment. If the next segment has multiple primary mappings,
 no assumptions can be made about RNEXT And PNEXT. If RNEXT Is `*', no assumptions can
 be made On PNEXT And bit @"F:SMRUCC.genomics.SequenceModel.SAM.DocumentElements.BitFLAGS.Bit0x20"[0x20].
#### Strand
获取当前的这条reads在基因组之上的方向
#### TLEN
observed Template LENgth
 
 TLEN: signed observed Template LENgth. If all segments are mapped to the same reference, the
 unsigned observed template length equals the number Of bases from the leftmost mapped base
 to the rightmost mapped base. The leftmost segment has a plus sign And the rightmost has a
 minus sign. The sign Of segments In the middle Is undefined. It Is Set As 0 For Single-segment
 template Or when the information Is unavailable.
