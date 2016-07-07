---
title: CLIBuilder
---

# CLIBuilder
_namespace: [SMRUCC.genomics.Interops.ClustalOrg](N-SMRUCC.genomics.Interops.ClustalOrg.html)_

Clustal Omega - 1.2.0 (AndreaGiacomo)

 If you Like Clustal - Omega please cite:
 Sievers F, Wilm A, Dineen D, Gibson TJ, Karplus K, Li W, Lopez R, McWilliam H, Remmert M, S├╢ding J, Thompson JD, Higgins DG.
 Fast, scalable generation of high-quality protein multiple sequence alignments using Clustal Omega.
 Mol Syst Biol. 2011 Oct 11;7:539. doi: 10.1038/msb.2011.75. PMID: 21988835.
 If you don't like Clustal-Omega, please let us know why (and cite us anyway).

 Check http : //www.clustal.org for more information And updates.

 Usage: clustalo [-hv] [-i {<file>,-}] [--hmm-In=<file>]... [--dealign] [--profile1=<file>] [--profile2=<file>] 
 [--Is-profile] [-t {Protein, RNA, DNA}] [--infmt={a2m=fa[sta],clu[stal],msf,phy[lip],selex,st[ockholm],vie[nna]}] 
 [--distmat-In=<file>] [--distmat-out=<file>] [--guidetree-In=<file>] [--guidetree-out=<file>] [--full] [--full-iter] 
 [--cluster-size=<n>] [--clustering-out=<file>] [--use-kimura] [--percent-id] [-o {file,-}] 
 [--outfmt={a2m=fa[sta],clu[stal],msf,phy[lip],selex,st[ockholm],vie[nna]}] [--residuenumber] [--wrap=<n>] 
 [--output-order={input-order,tree-order}] [--iterations=<n>] [--max-guidetree-iterations=<n>] [--max-hmm-iterations=<n>] 
 [--maxnumseq=<n>] [--maxseqlen=<l>] [--auto] [--threads=<n>] [-l <file>] [--version] [--Long-version] [--force] 
 [--MAC-RAM=<n>]

 A typical invocation would be: clustalo -i my-in-seqs.fa -o my-out-seqs.fa -v




### Properties

#### Auto
--auto 
 Set options automatically (might overwrite some of your options)
#### ClusteringOut
--clustering-out=<file> 
 Clustering output file
#### ClusterSize
--cluster-size=<n> 
 soft maximum of sequences in sub-clusters
#### Dealign
--dealign 
 Dealign input sequences
#### DistmatIn
--distmat-in=<file> 
 Pairwise distance matrix input file (skips distance computation)
#### DistmatOut
--distmat-out=<file> 
 Pairwise distance matrix output file
#### Force
--force 
 Force file overwriting
#### Full
--full 
 Use full distance matrix for guide-tree calculation (might be slow; mBed Is default)
#### FullIter
--full-iter 
 Use full distance matrix for guide-tree calculation during iteration (might be slowish; mBed Is default)
#### GuidetreeIn
--guidetree-in=<file> 
 Guide tree input file (skips distance computation And guide-tree clustering step)
#### GuidetreeOut
--guidetree-out=<file> 
 Guide tree output file
#### Help
-h, --help 
 Print this help And exit
#### HMMIn
--hmm-in=<file> 
 HMM input files
#### InFile
-i, --in, --infile={<file>,-} 
 Multiple sequence input file (- for stdin)
#### InFmt
--infmt={a2m=fa[sta],clu[stal],msf,phy[lip],selex,st[ockholm],vie[nna]} 
 Forced sequence input file format (default: auto)
#### IsProfile
--Is-profile 
 disable check if profile, force profile (default no)
#### Iterations
--iterations, --iter=<n> 
 Number of (combined guide-tree/HMM) iterations
#### Log
-l, --log=<file> 
 Log all non-essential output to this file
#### LongVersion
--long-version 
 Print long version information And exit
#### MaxGuidetreeIterations
--max-guidetree-iterations=<n> 
 Maximum number guidetree iterations
#### MaxHMMIterations
--max-hmm-iterations=<n> 
 Maximum number of HMM iterations
#### MaxNumSeq
--maxnumseq=<n> 
 Maximum allowed number of sequences
#### MaxSeqLen
--maxseqlen=<l> 
 Maximum allowed sequence length
#### Out
-o, --out, --outfile={file,-} 
 Multiple sequence alignment output file (default stdout)
#### OutFmt
--outfmt={a2m=fa[sta],clu[stal],msf,phy[lip],selex,st[ockholm],vie[nna]} 
 MSA output file format (default: fasta)
#### OutputOrder
--output-order={input-order,tree-order} 
 MSA output orderlike in input/guide-tree
#### p1
--profile1, --p1=<file> 
 Pre-aligned multiple sequence file (aligned columns will be kept fix)
#### p2
--profile2, --p2=<file> 
 Pre-aligned multiple sequence file (aligned columns will be kept fix)
#### PercentId
--percent-id 
 convert distances into percent identities (default no)
#### ResidueNumber
--residuenumber, --resno 
 in Clustal format print residue numbers (default no)
#### SeqType
-t, --seqtype={Protein, RNA, DNA} 
 Force a sequence type (default: auto)
#### Threads
--threads=<n> 
 Number of processors to use
#### UseKimura
--use-kimura 
 use Kimura distance correction for aligned sequences (default no)
#### Verbose
-v, --verbose 
 Verbose output (increases if given multiple times)
#### Version
--version 
 Print version information And exit
#### Wrap
--wrap=<n> 
 number of residues before line-wrap in output
