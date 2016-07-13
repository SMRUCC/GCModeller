---
title: GFF
---

# GFF
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat](N-SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.html)_

GFF (General Feature Format) specifications document



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF.#ctor(SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.FeatureKeys.Features)
```
Copy specific type

|Parameter Name|Remarks|
|--------------|-------|
|gff|-|
|type|-|


#### GetByName
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF.GetByName(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Name|@"P:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.Feature.attributes" -> name|


#### LoadDocument
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF.LoadDocument(System.String)
```
Load a GFF (General Feature Format) specifications document file from a plant text file.
 (从一个指定的文本文件之中加载基因组特性片段的数据)

|Parameter Name|Remarks|
|--------------|-------|
|Path|-|


#### TryGetValue
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.GFF.TryGetValue(System.Collections.Generic.Dictionary{System.String,System.String},System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|hash|-|
|Key|全部是小写字符|



### Properties

#### Date
date (##date <date>)
 
 The date the file was made, Or perhaps that the prediction programs were run. We suggest to use 
 astronomical format 1997-11-08 for 8th November 1997, first because these sort properly, And 
 second to avoid any US/European bias.
#### DNA
DNA 
 
 (##DNA <seqname>
 ##acggctcggattggcgctggatgatagatcagacgac
 ##...
 ##End-DNA)
 
 To give a DNA sequence. Several people have pointed out that it may be convenient to include the sequence in the file. 
 It should Not become mandatory to do so, And in our experience this has been very little used. Often the seqname will 
 be a well-known identifier, And the sequence can easily be retrieved from a database, Or an accompanying file.
#### Features
基因组上面的特性位点
#### GffVersion
gff-version (##gff-version 2)
 
 GFF version - In Case it Is a real success And we want To change it. The current Default version Is 2, 
 so If this line Is Not present version 2 Is assumed.
#### Protein
Protein
 
 (##Protein <seqname>

 ##MVLSPADKTNVKAAWGKVGAHAGEYGAEALERMFLSF
 ##...
 ##End-Protein)
 
 Similar to DNA. Creates an implicit ##Type Protein <seqname> directive.
#### RNA
RNA 
 
 (##RNA <seqname>
 ##acggcucggauuggcgcuggaugauagaucagacgac
 ##...
 ##End-RNA)
 
 Similar to DNA. Creates an implicit ##Type RNA <seqname> directive.
#### SeqRegion
sequence-region (##sequence-region <seqname> <start> <end>)
 
 To indicate that this file only contains entries for the specified subregion of a sequence.
#### Size
Genome size
#### SrcVersion
source-version (##source-version <source> <version text>)
 
 So that people can record what version Of a program Or package was used To make the data In this file. 
 I suggest the version Is text without whitespace. That allows things Like 1.3, 4a etc. There should be 
 at most one source-version line per source.
#### Type
type (##Type <type> [<seqname>])
 
 The type Of host sequence described by the features. Standard types are 'DNA', 'Protein' and 'RNA'. 
 The optional <seqname> allows multiple ##Type definitions describing multiple GFF sets in one file, 
 each of which have a distinct type. If the name is not provided, then all the features in the file 
 are of the given type. Thus, with this meta-comment, a single file could contain DNA, RNA and 
 Protein features, for example, representing a single genomic locus or 'gene', alongside type-specific 
 features of its transcribed mRNA and translated protein sequences. If no ##Type meta-comment is 
 provided for a given GFF file, then the type is assumed to be DNA.
