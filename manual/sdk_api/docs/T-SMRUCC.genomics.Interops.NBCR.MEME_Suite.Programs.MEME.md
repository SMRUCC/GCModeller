---
title: MEME
---

# MEME
_namespace: [SMRUCC.genomics.Interops.NBCR.MEME_Suite.Programs](N-SMRUCC.genomics.Interops.NBCR.MEME_Suite.Programs.html)_





### Methods

#### Invoke
```csharp
SMRUCC.genomics.Interops.NBCR.MEME_Suite.Programs.MEME.Invoke(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|DataSet|file containing sequences in FASTA format|



### Properties

#### BackgroundFile
name of background Markov model file
#### BranchingFactor
branching factor for branching search
#### Cons
consensus sequence to start EM from
#### Distance
EM convergence criterion
#### Ev
stop if motif E-value greater than [evt]
#### h
Print this message
#### Hs
size of heaps for widths where substring search occurs
#### IsDNA
sequences use DNA alphabet
#### IsProtein
sequences use protein alphabet
#### IsTextOutput
output in text format (default is HTML)
#### MaxIter
maximum EM iterations to run
#### MaxSites
maximum number of sites for each motif
#### MaxSize
maximum dataset size in characters
#### MaxWidth
maximum motif width
#### MinSites
minimum number of sites for each motif
#### MinWidth
minumum motif width
#### Mode
oops|zoops|anr, distribution of motifs
#### NoEndGaps
do not count end gaps in multiple alignments
#### NomaTrim
do not adjust motif width using multiple alignments
#### NoStatus
do not print progress reports to terminal
#### Np
use parallel version with [np] processors
#### NumMotifs
maximum number of motifs to find
#### OutputDir
name of directory for output files will not replace existing directory
#### OutputDir2
name of directory for output files will replace existing directory
#### Palindromes
force palindromes (requires -dna)
#### PLib
name of Dirichlet prior file
#### PriorStrength
strength of the prior
#### PriorType
dirichlet|dmix|mega|megap|addone, type of prior to use
#### PspFile
name of positional priors file
#### RevComp
allow sites on + or - DNA strands
#### sf
print [sf] as name of sequence file
#### Sites
number of sites for each motif
#### SpFuzz
fuzziness of sequence to theta mapping
#### SpMap
uni|pam, starting point seq to theta mapping type
#### Time
quit before number of CPU seconds consumed
#### Verbose
verbose mode
#### WBranch
perform width branching
#### Wg
gap opening cost for multiple alignments
#### WnSites
weight on expected number of sites
#### Ws
gap extension cost for multiple alignments
#### XBranch
perform x-branching
