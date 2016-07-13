---
title: Replicate
---

# Replicate
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs](N-SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.html)_

A Replicate object represents information about a single RNA-seq experiment, including information about all reads from the experiment.



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.Replicate.#ctor(System.Int64,System.Boolean,System.String)
```
Constructs a new Replicate object based on compressed sequencing reads files.

#### __readInAlignmentFile
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.Replicate.__readInAlignmentFile(System.String,System.Boolean,System.Int64)
```
Reads in a compressed alignment file for a genome with the specified index z. Stores the 
 RNA-seq data in two lists, one for the plus strand 
 and one for the minus strand. Also computes the 
 total reads in the file.

#### __transformation
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.Replicate.__transformation(System.Double)
```
Helper method for setting the minimum expression level for UTRs and ncRNAs.

#### getBackgroundProb
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.Replicate.getBackgroundProb(System.Int32)
```
Returns the probability that the given number of reads
 at some nucleotide corresponds to the background, i.e.,
 a non-transcript.

#### getMeanOfRange
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.Replicate.getMeanOfRange(System.Int32,System.Int32,System.Char)
```
Return the mean number of reads on the given strand mapping
 to the given range of genomic coordinates in the specified
 genome at index z.

#### getReads
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.Replicate.getReads(System.Int32,System.Char)
```
Return the number of reads mapping to the specified coordinate
 on the specified strand for the specified genome at index z.

#### getReadsInRange
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.Replicate.getReadsInRange(System.Int32,System.Int32,System.Char)
```
Return the number of reads on the given strand mapping
 to the given range of genomic coordinates for the specified
 genome at index z.

#### getStdevOfRange
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.Replicate.getStdevOfRange(System.Int32,System.Int32,System.Char,System.Double)
```
Return the standard deviation of reads on the given strand mapping
 to the given range of genomic coordinates in the specified genome
 at index z.

#### ToString
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.Transcriptome.UTRs.Replicate.ToString
```
Returns a String representation of this object.


### Properties

#### avgLengthReads
Return average length of sequencing reads in this Replicate.
#### avgReads
Return average number of reads in this Replicate.
#### background
Number of nucleotides with few or no reads
#### backgroundParameter
Parameter of geometric distribution of background reads
#### fileName
Path to sequencing reads file
#### minExpression
Sets the minimum level of expression (for a UTR region and
 ncRNA to be considered expressed) in this Replicate based on 
 the average number of reads per nucleotide in this Replicate
 and the specified transcript sensitivity between 0.0 and 1.0, 
 inclusive.
#### minExpressionRNA
Return the minimum level of expression for a ncRNA to be
 considered expressed in this Replicate.
#### minExpressionUTR
Return the minimum level of expression for a UTR region to be
 considered expressed in this Replicate.
#### minusReads
Reads on the minus strand for each genome
#### name
Return the name of this Replicate.
#### plusReads
Reads on the plus strand for each genome
#### totalReads
Return total number of reads in this Replicate.
#### upperQuartile
Return the upper quartile for this Replicate.
