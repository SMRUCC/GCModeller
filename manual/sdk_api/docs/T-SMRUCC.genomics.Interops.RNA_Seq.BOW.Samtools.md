---
title: Samtools
---

# Samtools
_namespace: [SMRUCC.genomics.Interops.RNA_Seq.BOW](N-SMRUCC.genomics.Interops.RNA_Seq.BOW.html)_

SAMtools is a set of utilities for interacting with and post-processing short DNA sequence read alignments 
 in the SAM, BAM and CRAM formats, written by Heng Li. These files are generated as output by short read 
 aligners like BWA. Both simple and advanced tools are provided, supporting complex tasks like variant calling 
 and alignment viewing as well as sorting, indexing, data extraction and format conversion.[2] SAM files can 
 be very large (10s of Gigabytes is common), so compression is used to save space. SAM files are human-readable 
 text files, and BAM files are simply their binary equivalent, whilst CRAM files are a restructured column-oriented 
 binary container format. BAM files are typically compressed and more efficient for software to work with than SAM. 
 
 SAMtools makes it possible to work directly with a compressed BAM file, without having to uncompress the whole file. 
 Additionally, since the format for a SAM/BAM file is somewhat complex - containing reads, references, alignments, 
 quality information, and user-specified annotations - SAMtools reduces the effort needed to use SAM/BAM files by 
 hiding low-level details.



### Methods

#### Assembly
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.Samtools.Assembly(SMRUCC.genomics.SequenceModel.SAM.SAM,System.Boolean,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|SAM|-|
|TrimError|-|
|EXPORT|The data export directory path|


#### Bam2Sam
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.Samtools.Bam2Sam(System.String,System.String)
```
Convert the binary format Sam mapping file into the text format Sam mapping file.
 The view command filters SAM or BAM formatted data. Using options and arguments it understands what data to select 
 (possibly all of it) and passes only that data through. Input is usually a sam or bam file specified as an argument, 
 but could be sam or bam data piped from any other command. Possible uses include extracting a subset of data into a 
 new file, converting between BAM and SAM formats, and just looking at the raw file contents. 
 The order of extracted reads is preserved.

|Parameter Name|Remarks|
|--------------|-------|
|Bam|-|
|Sam|-|


#### Import
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.Samtools.Import(System.String,System.String,System.String)
```
Next, we need to convert the SAM file into a BAM file. (A BAM file is just a binary version of a SAM file.)

#### Index
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.Samtools.Index(System.String,System.String)
```
And last, we need Samtools to index the BAM file. The index command creates a new index file that allows fast look-up of data in a 
 (sorted) SAM or BAM. Like an index on a database, the generated *.sam.sai or *.bam.bai file allows programs that can read it to 
 more efficiently work with the data in the associated files.

|Parameter Name|Remarks|
|--------------|-------|
|Bam|-|


#### Indexing
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.Samtools.Indexing(System.String)
```
Like bwa, Samtools also requires us to go through several steps before we have our data in usable form. 
 First, we need to have Samtools generate its own index of the reference genome

|Parameter Name|Remarks|
|--------------|-------|
|Fasta|-|


#### Sam2Bam
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.Samtools.Sam2Bam(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Sam|The input source file path of the sam file.|
|Bam|The save file path of the converted bam file.|


#### Sort
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.Samtools.Sort(System.String,System.String,System.Boolean)
```
Now, we need to sort the BAM file. The sort command sorts a BAM file based on its position in the reference, 
 as determined by its alignment. The element + coordinate in the reference that the first matched base in the 
 read aligns to is used as the key to order it by. [TODO: verify]. The sorted output is dumped to a new file 
 by default, although it can be directed to stdout (using the -o option). As sorting is memory intensive and 
 BAM files can be large, this command supports a sectioning mode (with the -m options) to use at most a given 
 amount of memory and generate multiple output file. These files can then be merged to produce a complete 
 sorted BAM file [TODO - investigate the details of this more carefully].
 (请注意，本函数只能够对bam文件进行排序，假若需要对sam文件进行排序，请先转换为bam文件)

|Parameter Name|Remarks|
|--------------|-------|
|Bam|-|
|Out|-|


#### Viewing
```csharp
SMRUCC.genomics.Interops.RNA_Seq.BOW.Samtools.Viewing(System.String,System.String)
```
Viewing the output with TView. Now that we've generated the files, we can view the output with TView.
 The tview command starts an interactive ascii-based viewer that can be used to visualize how reads are aligned to specified 
 small regions of the reference genome. Compared to a graphics based viewer like IGV,[3] it has few features. Within the view, 
 it is possible to jumping to different positions along reference elements (using 'g') and display help information ('?').

|Parameter Name|Remarks|
|--------------|-------|
|Bam|-|
|Fasta|-|



