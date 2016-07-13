---
title: Fastaq
---

# Fastaq
_namespace: [SMRUCC.genomics.SequenceModel.Fastaq](N-SMRUCC.genomics.SequenceModel.Fastaq.html)_

FASTQ format is a text-based format for storing both a biological sequence (usually nucleotide sequence) and 
 its corresponding quality scores. Both the sequence letter and quality score are each encoded with a single 
 ASCII character for brevity. It was originally developed at the Wellcome Trust Sanger Institute to bundle a 
 FASTA sequence and its quality data, but has recently become the de facto standard for storing the output of 
 high-throughput sequencing instruments such as the Illumina Genome Analyzer.
 
 There is no standard file extension for a FASTQ file, but .fq and .fastq, are commonly used.

> 
>  A FASTQ file normally uses four lines per sequence.
>  
>  Line 1 begins with a '@' character and is followed by a sequence identifier and an optional description (like a FASTA title line).
>  Line 2 is the raw sequence letters.
>  Line 3 begins with a '+' character and is optionally followed by the same sequence identifier (and any description) again.
>  Line 4 encodes the quality values for the sequence in Line 2, and must contain the same number of symbols as letters in the sequence.
>  
>  一条Fastaq序列文件通常使用4行代表一条序列数据：
>  第一行： 起始于@字符，后面跟随着序列的标识符，以及一段可选的摘要描述信息
>  第二行： 原始的序列
>  第三行： 起始于+符号，与第一行的作用类似
>  第四行： 编码了第二行的序列数据的质量高低，长度与第二行相同
>  


### Methods

#### FastaqParser
```csharp
SMRUCC.genomics.SequenceModel.Fastaq.Fastaq.FastaqParser(System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|str|-|

> 
>  The original Sanger FASTQ files also allowed the sequence and quality strings to be wrapped (split over multiple lines), 
>  but this is generally discouraged as it can make parsing complicated due to the unfortunate choice of "@" and "+" as 
>  markers (these characters can also occur in the quality string).[2] An example of a tools that break the 4 line convention 
>  is vcfutils.pl from samtools.[3]
>  


### Properties

#### QUANTITY_ORDERS
The character '!' represents the lowest quality while '~' is the highest. Here are the quality value characters in left-to-right increasing order of quality (ASCII):
#### SEQ_ID
第一行的序列标识符
#### Title
第一行的摘要描述信息
