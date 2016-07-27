---
title: CIGAR_OPERATIONS
---

# CIGAR_OPERATIONS
_namespace: [SMRUCC.genomics.SequenceModel.SAM](N-SMRUCC.genomics.SequenceModel.SAM.html)_






### Properties

#### D
deletion from the reference
#### EQ
sequence match
#### H
hard clipping (clipped sequences Not present In SEQ)
 
 H can only be present As the first And/Or last operation.
#### I
insertion To the reference
#### M
alignment match (can be a sequence match Or mismatch)
#### N
skipped region from the reference
 
 For mRNA -To -genome alignment, an N operation represents an intron. For other types Of alignments, the interpretation of N Is Not defined.
#### P
padding (silent deletion from padded reference)
#### S
soft clipping (clipped sequences present In SEQ)
 
 S may only have H operations between them And the ends Of the CIGAR String.
#### X
sequence mismatch
