---
title: TaxonomyWebAPI
---

# TaxonomyWebAPI
_namespace: [SMRUCC.genomics.Assembly.NCBI.Entrez](N-SMRUCC.genomics.Assembly.NCBI.Entrez.html)_

##### Automatically Getting The Ncbi Taxonomy Id From The Genbank Identifier
 
 The question is whether, given a (long) list of Genbank identifiers, is possible to 
 get the ncbi taxonomy identifier for each one. I know it may seem very easy, but I 
 have not found any web service which makes this, and I wouldn't like to do this 
 manually.



### Methods

#### efetch
```csharp
SMRUCC.genomics.Assembly.NCBI.Entrez.TaxonomyWebAPI.efetch(System.String,System.String)
```
NCBI efetch can use an accession number instead of a gi. and the XML/Fasta returned by efetch contains the taxonomy-ID:

|Parameter Name|Remarks|
|--------------|-------|
|gi|-|



