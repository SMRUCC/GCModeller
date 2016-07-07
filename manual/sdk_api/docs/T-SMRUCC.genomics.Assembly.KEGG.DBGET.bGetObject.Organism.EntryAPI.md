---
title: EntryAPI
---

# EntryAPI
_namespace: [SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism](N-SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism.html)_





### Methods

#### GetKEGGSpeciesCode
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism.EntryAPI.GetKEGGSpeciesCode(System.String)
```
通过本地资源从基因组全名之中得到KEGG之中的三字母的简写代码

|Parameter Name|Remarks|
|--------------|-------|
|Name|-|


#### GetOrganismList
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism.EntryAPI.GetOrganismList
```
Gets the latest KEGG organism list from query the KEGG database.

#### GetOrganismListFromResource
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism.EntryAPI.GetOrganismListFromResource
```
Load KEGG organism list from the internal resource.

#### GetValue
```csharp
SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism.EntryAPI.GetValue(System.String)
```
Gets the organism value from the KEGG database through the brief code, 
 if the data is not exists in the database, Nothing will be returns.

|Parameter Name|Remarks|
|--------------|-------|
|sp|The organism brief code in the KEGG database|



### Properties

#### __spHash
{brief_sp, @"T:SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism.Organism"}
