---
title: COGTable
---

# COGTable
_namespace: [SMRUCC.genomics.Assembly.NCBI.COG.COGs](N-SMRUCC.genomics.Assembly.NCBI.COG.COGs.html)_

cog2003-2014.csv
 CSV table row for COG, Contains list of orthology domains. Comma-delimited,



### Methods

#### LoadDocument
```csharp
SMRUCC.genomics.Assembly.NCBI.COG.COGs.COGTable.LoadDocument(System.String)
```
* Example:

 333894695,Alteromonas_SN2_uid67349,333894695,427,1,427,COG0001,0,

|Parameter Name|Remarks|
|--------------|-------|
|path|-|



### Properties

#### COGId
<COG-id>
#### DomainID
<domain-id>
 
 In this version the fields <domain-id> and <protein-id> are identical
 And both normally refer to GenBank GIs. Thus neither <domain-id> nor
 <protein-id> are necessarily unique in this file (this happens when a
 protein consists Of more than one orthology domains, e.g. 48478501).
#### Ends
<domain-End>
#### GenomeName
<genome-name>
#### Membership
<membership-Class>
 
 The <membership-class> field indicates the nature of the match
 between the sequence And the COG consensus
 
 0 - the domain matches the COG consensus;
 
 1 - the domain Is significantly longer than the COG consensus;
 
 2 - the domain Is significantly shorter than the COG consensus;
 
 3 - partial match between the domain And the COG consensus.
#### ProteinID
<protein-id>
#### ProteinLength
<protein-length>
#### Start
<domain-start>
