---
title: BlastnOptionalArguments
---

# BlastnOptionalArguments
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.CLIArgumentsBuilder](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.CLIArgumentsBuilder.html)_






### Properties

#### BestHitOverhang
Best Hit algorithm overhang value ((0, 0.5), recommended value: 0.1)
 * Incompatible with: culling_limit
#### BestHitScoreEdge
Best Hit algorithm score edge value ((0, 0.5), recommended value: 0.1)
 * Incompatible with: culling_limit
#### CompBasedStats
Use composition-based statistics:
 D or d: default (equivalent to 2 )
 0 or F or f: No composition-based statistics
 1: Composition-based statistics as in NAR 29:2994-3005, 2001
 2 or T or t : Composition-based score adjustment as in Bioinformatics 21:902-911, 2005, conditioned on sequence properties
 3: Composition-based score adjustment as in Bioinformatics 21:902-911, 2005, unconditionally
 
 Default = `2'
#### CullingLimit
If the query range of a hit is enveloped by that of at least this many higher-scoring hits >=0, delete the hit
 * Incompatible with: best_hit_overhang, best_hit_score_edge
#### DbHardMask
Filtering algorithm ID to apply to the BLAST database as hard masking
 * Incompatible with: db_soft_mask, subject, subject_loc
#### DbSize
Effective length of the database
#### DbSoftMask
Filtering algorithm ID to apply to the BLAST database as soft masking
 * Incompatible with: db_hard_mask, subject, subject_loc
#### EntrezQuery
Restrict search with the given Entrez query
 * Requires: remote
#### ExportSearchStrategy
File name to record the search strategy used
 * Incompatible with: import_search_strategy
#### GapExtend
Cost to extend a gap
#### GapOpen
Cost to open a gap
#### GiList
Restrict search of database to list of GI's
 * Incompatible with: negative_gilist, seqidlist, remote, subject, subject_loc
#### Html
Produce HTML output?
#### ImportSearchStrategy
Search strategy to use
 * Incompatible with: export_search_strategy
#### LCaseMasking
Use lower case filtering in query and subject sequence(s)?
#### Matrix
Scoring matrix name (normally BLOSUM62)
#### MaxHspsPerSubject
Override maximum number >=0 of HSPs per subject to save for ungapped searches
 (0 means do not override)
 Default = `0'
#### MaxTargetSeqs
Maximum number >=1 of aligned sequences to keep 
 Not applicable for outfmt less than 4
 Default = `500'
 * Incompatible with: num_descriptions, num_alignments
#### NegativeGiList
Restrict search of database to everything except the listed GIs
 * Incompatible with: gilist, seqidlist, remote, subject, subject_loc
#### NumberAlignments
Number >=0 of database sequences to show alignments for
 Default = `250'
 * Incompatible with: max_target_seqs
#### NumberDescriptions
Number >=0 of database sequences to show one-line descriptions for Not applicable for outfmt > 4
 Default = `500'
 * Incompatible with: max_target_seqs
#### NumThreads
Number of threads >=1 (CPUs) to use in the BLAST search
 Default = `1'
 * Incompatible with: remote
#### OutFormat
Alignment view options:
 0 = pairwise,
 1 = query-anchored showing identities,
 2 = query-anchored no identities,
 3 = flat query-anchored, show identities,
 4 = flat query-anchored, no identities,
 5 = XML Blast output,
 6 = tabular,
 7 = tabular with comment lines,
 8 = Text ASN.1,
 9 = Binary ASN.1,
 10 = Comma-separated values,
 11 = BLAST archive format (ASN.1) 
 
 Options 6, 7, and 10 can be additionally configured to produce a custom format specified by space delimited 
 format specifiers.
 The supported format specifiers are:
 qseqid means Query Seq-id
 qgi means Query GI
 qacc means Query accesion
 qaccver means Query accesion.version
 qlen means Query sequence length
 sseqid means Subject Seq-id
 sallseqid means All subject Seq-id(s), separated by a ';'
 sgi means Subject GI
 sallgi means All subject GIs
 sacc means Subject accession
 saccver means Subject accession.version
 sallacc means All subject accessions
 slen means Subject sequence length
 qstart means Start of alignment in query
 qend means End of alignment in query
 sstart means Start of alignment in subject
 send means End of alignment in subject
 qseq means Aligned part of query sequence
 sseq means Aligned part of subject sequence
 evalue means Expect value
 bitscore means Bit score
 score means Raw score
 length means Alignment length
 pident means Percentage of identical matches
 nident means Number of identical matches
 mismatch means Number of mismatches
 positive means Number of positive-scoring matches
 gapopen means Number of gap openings
 gaps means Total number of gaps
 ppos means Percentage of positive-scoring matches
 frames means Query and subject frames separated by a '/'
 qframe means Query frame
 sframe means Subject frame
 btop means Blast traceback operations (BTOP)
 staxids means Subject Taxonomy ID(s), separated by a ';'
 sscinames means Subject Scientific Name(s), separated by a ';'
 scomnames means Subject Common Name(s), separated by a ';'
 sblastnames means Subject Blast Name(s), separated by a ';'
 (in alphabetical order)
 sskingdoms means Subject Super Kingdom(s), separated by a ';'
 (in alphabetical order) 
 stitle means Subject Title
 salltitles means All Subject Title(s), separated by a 
 sstrand means Subject Strand
 qcovs means Query Coverage Per Subject
 qcovhsp means Query Coverage Per HSP
 When not provided, the default value is:
 'qseqid sseqid pident length mismatch gapopen qstart qend sstart send evalue bitscore', which is equivalent to the keyword 'std'
 Default = `0'
#### ParseDeflines
Should the query and subject defline(s) be parsed?
#### Query
-query <File_In>
 
 Input file name
 Default = `-'
#### QueryLocation
Location on the query sequence in 1-based offsets (Format: start-stop)
#### Remote
Execute search remotely?
 * Incompatible with: gilist, seqidlist, negative_gilist, subject_loc, num_threads
#### SearcHsp
Effective length >=0 of the search space
#### Seg
Filter query sequence with SEG (Format: 'yes', 'window locut hicut', or 'no' to disable)
 Default = `no'
#### SeqIdList
Restrict search of database to list of SeqId's
 * Incompatible with: gilist, negative_gilist, remote, subject, subject_loc
#### ShowGis
Show NCBI GIs in deflines?
#### SoftMasking
Apply filtering locations as soft masks
 Default = `false'
#### SubjectLocation
Location on the subject sequence in 1-based offsets (Format: start-stop)
 * Incompatible with: db, gilist, seqidlist, negative_gilist, db_soft_mask, db_hard_mask, remote
#### Task
Task to execute, Permissible values: 'blastp' 'blastp-short' 'deltablast'
 Default = `blastp'
#### Threshold
Minimum word score such that the word is added to the BLAST lookup table, >=0
#### UnGapped
Perform ungapped alignment only?
#### UseSwTback
Compute locally optimal Smith-Waterman alignments?
#### WindowSize
Multiple hits window size >=0, use 0 to specify 1-hit algorithm
#### WordSize
Word size for wordfinder algorithm, >=2
#### XDropGap
X-dropoff value (in bits) for preliminary gapped extensions
#### XDropGapFinal
X-dropoff value (in bits) for final gapped alignment
#### XDropUngap
X-dropoff value (in bits) for ungapped extensions
