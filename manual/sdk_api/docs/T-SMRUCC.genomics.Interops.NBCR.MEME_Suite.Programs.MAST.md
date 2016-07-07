---
title: MAST
---

# MAST
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Programs](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Programs.html)_

MAST: Motif Alignment and Search Tool



### Methods

#### Invoke
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Programs.MAST.Invoke(System.String,System.String)
```
mast motif_file sequence_file [options]

|Parameter Name|Remarks|
|--------------|-------|
|motifFile|file containing motifs to use; normally a MEME output file|
|SequenceFile|search sequences in FASTA-formatted database with motifs|



### Properties

#### BackgroundFile
read background frequencies from file
#### Best
include only the best motif hits in -hit_list diagrams
#### Composition
adjust p-values and E-values for sequence composition
#### Count
only use the first count motifs or all motifs when count is zero (default: 0)
#### DbList
read the sequence file as a list of FASTA-formatted databases
#### df
in results use df as database name (ignored when -dblist)
#### Diag
nominal order and spacing of motifs is specified by diag which is a block diagram
#### dl
in results use dl as link to search sequence names; token SEQUENCEID is replaced with the FASTA sequence ID; ignored when -dblist;
#### DNA
translate DNA sequences to protein; motifs must be protein; sequences must be DNA
#### EValue
print results for sequences with E-value small than ev (default: 10)
#### HitList
print a machine-readable list of all hits only; outputs to standard out and overrides -seqp
#### Mev
use only motifs with E-values less than mev
#### mf
in results use mf as motif file name
#### MotifNumber
use only motif number m (overrides -mev); this can be repeated to select multiple motifs
#### ms
lower bound on number of sequences in db
#### mt
show motif matches with p-value smaller than mt (default: 0.0001)
#### NoHtml
do not create html output
#### NoReverse
do not score reverse complement DNA strand
#### NoStatus
do not print progress report
#### NoText
do not create text output
#### OutputDir
directory to output mast results; directory must not exist
#### OutputOverwriting
directory to output mast results with overwriting allowed
#### RemoveMotifs
remove highly correlated motifs from query
#### Separate
score reverse complement DNA strand as a separate sequence
#### SeqP
use SEQUENCE p-values for motif thresholds (default: use POSITION p-values)
#### WeakMatches
show weak matches (mt small than p-value and small than mt*10) in angle brackets in the hit list or when the xml is converted to text
