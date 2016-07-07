---
title: Citation
---

# Citation
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.html)_

Any of the above components may be omitted, but it is meaningless to supply a timestamp, 
 curator or probability if the evidence-code is omitted. Trailing colons should be 
 omitted, but if a value contains an evidence-code with no accompanying citation, the 
 leading colon must be present. The square brackets are optional.

> 
>  Examples:
>    [123456] -- a PubMed or MEDLINE reference
>    [SMITH95] -- a non-PubMed reference
>    [123456:EV-IDA] -- an evidence code with associated PubMed reference
>    [:EV-HINF] -- an evidence code with no associated reference
>    [123456:EV-IGI:9876543:paley] -- a time- and user-stamped evidence code with associated reference
>  


### Methods

#### ToString
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Citation.ToString
```

> CITATIONS - :EV-COMP-AINF:3567386625:kaipa


### Properties

#### Curator
curator is the username of the curator who assigned the evidence code.
#### EmptyFill
为了防止Citation初始化的时候，由于在列表中的元素的数目不够的时候，赋值出现错误
#### EvidenceCode
evidence-code is the object identifier of some class belonging to the Evidence class, e.g. EV-EXP.
#### Probability
probability is a number between 0 and 1 describing the probability 
 that the evidence is correct, where available.
#### ReferenceId
reference-ID is a PubMed unique identifier or the identifier of a Publications object 
 (without the leading "PUB-").
#### TimeStamp
timestamp is a lisp universal time (not human readable) corresponding to the time the 
 evidence code was assigned.
#### With
with is a free text string that modifies the evidence-code when the citation annotates a GO term. 
 This is the "with" field described in GO documentation.
