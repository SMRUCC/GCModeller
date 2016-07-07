---
title: API
---

# API
_namespace: [SMRUCC.genomics.Data.Xfam.Rfam](N-SMRUCC.genomics.Data.Xfam.Rfam.html)_

Rfam is a database of structure-annotated multiple sequence alignments,
 convariance models And family annotation For a number Of non-coding RNA,
 cis-regulatory And self - splicing intron families. Rfam 12.0 contains 2450
 families.The seed alignments are hand curated And aligned using available
 sequence And Structure data, And covariance models are built from these
 alignments Using the INFERNAL 1.1 software suite (http://infernal.janelia.org).
 The full regions list Is created by searching the RFAMSEQ database (described
 below) Using the covariance model, And Then listing all hits above a family
 specific threshold To the model. Rfam 12.0 annotates 19,623,515 regions In the
 RFAMSEQ database.
 
 Rfam Is maintained by a consortium Of researchers at the EMBL European
 Bioinformatics Institute, Hinxton, UK And the Howard Huges Medical Institute,
 Janelia Farm Research Campus, Ashburn, Virginia, USA. We are very keen To hear
 any feedback, positive Or negative, that you may have On Rfam - please contact
 rfam-help@ebi.ac.uk.
 
 Rfam Is freely available And In the Public domain under the Creative Commons
 Zero licence. See ftp://ftp.ebi.ac.uk/pub/databases/Rfam/CURRENT/COPYING for
 more information.



### Methods

#### RfamAnalysisBatch
```csharp
SMRUCC.genomics.Data.Xfam.Rfam.API.RfamAnalysisBatch(System.String,System.String,System.String,System.String,System.Int32,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|blastn|-|
|Rfam|-|
|PTT_DIR|-|
|locusPrefix|-|
|offset|用于合并位点的偏移量|



