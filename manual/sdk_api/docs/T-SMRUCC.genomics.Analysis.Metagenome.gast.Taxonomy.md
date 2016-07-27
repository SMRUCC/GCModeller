---
title: Taxonomy
---

# Taxonomy
_namespace: [SMRUCC.genomics.Analysis.Metagenome.gast](N-SMRUCC.genomics.Analysis.Metagenome.gast.html)_

Create taxonomic objects,
 Return classes Or full text Of a taxonoDim Object,
 Calculate consensus Of an array Of taxonomic objects.



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.Metagenome.gast.Taxonomy.#ctor(System.String)
```
Create a new taxonomy object

|Parameter Name|Remarks|
|--------------|-------|
|line|-|


#### __data
```csharp
SMRUCC.genomics.Analysis.Metagenome.gast.Taxonomy.__data(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|line|-|

> 
>  $newString =~ s/;$//;
>  这个语法应该是正则表达式替换匹配字符串为空白字符串
>  

#### consensus
```csharp
SMRUCC.genomics.Analysis.Metagenome.gast.Taxonomy.consensus(SMRUCC.genomics.Analysis.Metagenome.gast.Taxonomy[],System.Double)
```
For an array of tax objects and a majority required, calculate a consensus taxonomy
 Return the consensus tax Object, As well As stats On the agreement

#### taxstring
```csharp
SMRUCC.genomics.Analysis.Metagenome.gast.Taxonomy.taxstring
```
Return the object as a ";" delimited string


### Properties

#### class
Return the Class Of an Object
#### depth
Return the depth of an object - last rank with valid taxonomy
#### DepthLevel
{domain, phylum, [class], order, family, genus, species, strain}
#### domain
Return the domain Of an Object
#### family
Return the family Of an Object
#### genus
Return the genus Of an Object
#### order
Return the order Of an Object
#### phylum
Return the phylum Of an Object
#### species
Return the species Of an Object
#### strain
Return the strain Of an Object
