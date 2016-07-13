---
title: LocalBLAST
---

# LocalBLAST
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.html)_





### Methods

#### #ctor
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.LocalBLAST.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|BlastBin|Blast+ bin dir|


#### Blastp
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.LocalBLAST.Blastp(System.String,System.String,System.String,System.String)
```
Generate the command line arguments of the program blastp.
 (生成blastp程序的命令行参数)

|Parameter Name|Remarks|
|--------------|-------|
|Input|The target sequence FASTA file.(包含有目标待比对序列的FASTA文件)|
|TargetDb|The selected database that to aligned.(将要进行比对的目标数据库)|
|Output|-|
|e|-|


#### FormatDb
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.LocalBLAST.FormatDb(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Db|-|
|dbType|"T" for protein, "F" for nucleotide.|


#### GetLastLogFile
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.LocalBLAST.GetLastLogFile
```
返回BLAST程序的日志文件，本函数必须在执行完BLASTP或者BLASTN操作之后才可以调用，否则返回空对象


### Properties

#### AlignmentView
Alignment view options, Range from 0 to 11:
 0 = pairwise,
 1 = query-anchored showing identities,
 2 = query-anchored no identities,
 3 = flat query-anchored, show identities,
 4 = flat query-anchored, no identities,
 5 = query-anchored no identities and blunt ends,
 6 = flat query-anchored, no identities and blunt ends,
 7 = XML Blast output,
 8 = tabular, 
 9 tabular with comment lines
 10 ASN, text
 11 ASN, binary [Integer]
 Default = 0
#### ARGUMS_BLASTN
The command line arguments of the blastn program.
#### ARGUMS_BLASTP
The command line arguments of the blastp program.
 (blastp程序的命令行参数)
#### BestHitsKeepsNumber
Number of best hits from a region to keep. Off by default.
 If used a value of 100 is recommended. Very high values of -v or -b is also suggested [Integer]
 default = 0
#### BLASTALLAssembly
The file path of the blastall program the in the BLAST+ program groups.
 (在BLAST+程序组之中的BLASTALL程序的文件路径)
#### CheckpointFile
PSI-TBLASTN checkpoint file [File In] Optional
#### CompositionBasedScoreAdjustments
Use composition-based score adjustments for blastp or tblastn:
 
 As first character:
 D or d: default (equivalent to T)
 0 or F or f: no composition-based statistics
 2 or T or t: Composition-based score adjustments as in Bioinformatics 21:902-911,
 1: Composition-based statistics as in NAR 29:2994-3005, 2001
 2005, conditioned on sequence properties
 3: Composition-based score adjustment as in Bioinformatics 21:902-911,
 2005, unconditionally
 For programs other than tblastn, must either be absent or be D, F or 0.
 
 As second character, if first character is equivalent to 1, 2, or 3:
 U or u: unified p-value combining alignment p-value and compositional p-value in round 1 only
 
 default = D
#### ConcatenatedQueries
Number of concatenated queries, for blastn and tblastn [Integer] Optional
 default = 0
#### ConcatenatedQueriesNumber
Number of database sequence to show alignments for (B) [Integer]
 default = 250
#### DBEffectiveLength
Effective length of the database (use zero for the real size) [Real]
 default = 0
#### DBGeneticCode
DB Genetic code (for tblast[nx] only) [Integer]
 default = 1
#### DropoffValue
X dropoff value for gapped alignment (in bits) (zero invokes default behavior)
 blastn 30, megablast 20, tblastx 0, all others 15 [Integer]
 default = 0
#### EValue
Expectation value (E) [Real] 
 Default = 10.0
#### Filter
Filter query sequence (DUST with blastn, SEG with others) [String]
 Default = T
#### ForceLegacy
Force use of the legacy BLAST engine [T/F] Optional
 default = F
#### ForceLegacyNumber
Number of database sequences to show one-line descriptions for (V) [Integer]
 default = 500
#### FormatDbAssembly
formatdb程序的文件名
#### FrameShiftPenalty
Frame shift penalty (OOF algorithm for blastx) [Integer]
 default = 0
#### GapExtend
Cost to extend a gap (-1 invokes default behavior) [Integer]
 default = -1
#### GapOpen
Cost to open a gap (-1 invokes default behavior) [Integer]
 Default = -1
#### GappedAlignment
Perform gapped alignment (not available with tblastx) [T/F]
 default = T
#### GI
Show GI's in deflines [T/F]
 default = F
#### Hits
0 for multiple hit, 1 for single hit (does not apply to blastn) [Integer]
 default = 0
#### HitsWindowSize
Multiple Hits window size, default if zero (blastn/megablast 0, all others 40 [Integer]
 default = 0
#### HTML
Produce HTML output [T/F]
 default = F
#### IntronLength
Length of the largest intron allowed in a translated nucleotide sequence when linking multiple distinct alignments. (0 invokes default behavior; a negative value disables linking.) [Integer]
 default = 0
#### LowerCaseFiltering
Use lower case filtering of FASTA sequence [T/F] Optional
#### MatchReward
Reward for a nucleotide match (blastn only) [Integer]
 default = 1
#### Matrix
Matrix [String]
 default = BLOSUM62
#### MegaBlast
MegaBlast search [T/F]
 default = F
#### MismatchPenalty
Penalty for a nucleotide mismatch (blastn only) [Integer]
 default = -3
#### Processors
Number of processors to use [Integer]
 default = 1
#### QueryDeflineBelieve
Believe the query defline [T/F]
 default = F
#### QueryGeneticCode
Query Genetic code to use [Integer]
 default = 1
#### QueryLocation
Location on query sequence [String] Optional
#### RestrictList
Restrict search of database to list of GI's [String] Optional
#### SearchSpace
Effective length of the search space (use zero for the real size) [Real]
 default = 0
#### SeqAlign
SeqAlign file [File Out] Optional
#### SmithWatermanAlignments
Compute locally optimal Smith-Waterman alignments (This option is only available for gapped tblastn.) [T/F]
 default = F
#### Strand
Query strands to search against database (for blast[nx], and tblastx)
 3 is both, 1 is top, 2 is bottom [Integer]
 default = 3
#### Threshold
Threshold for extending hits, default if zero
 blastp 11, blastn 0, blastx 12, tblastn 13
 tblastx 13, megablast 0 [Real]
 default = 0
#### UngappedExtensionsXDropOff
X dropoff value for ungapped extensions in bits (0.0 invokes default behavior)
 blastn 20, megablast 10, all others 7 [Real]
 default = 0.0
#### WordSize
Word size, default if zero (blastn 11, megablast 28, all others 3) [Integer]
 default = 0
#### XDropoffValue
X dropoff value for final gapped alignment in bits (0.0 invokes default behavior)
 blastn/megablast 100, tblastx 0, all others 25 [Integer]
 default = 0
