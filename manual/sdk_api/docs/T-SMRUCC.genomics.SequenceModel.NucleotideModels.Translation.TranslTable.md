---
title: TranslTable
---

# TranslTable
_namespace: [SMRUCC.genomics.SequenceModel.NucleotideModels.Translation](N-SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.html)_

Compiled by Andrzej (Anjay) Elzanowski and Jim Ostell at National Center for Biotechnology Information (NCBI), Bethesda, Maryland, U.S.A.
 
 NCBI takes great care To ensure that the translation For Each coding sequence (CDS) present In GenBank records Is correct. 
 Central To this effort Is careful checking On the taxonomy Of Each record And assignment Of the correct genetic code 
 (shown As a /transl_table qualifier On the CDS In the flat files) For Each organism And record. This page summarizes And references this work.
 
 The synopsis presented below Is based primarily On the reviews by Osawa et al. (1992) And Jukes And Osawa (1993). 
 Listed In square brackets [] (under Systematic Range) are tentative assignments Of a particular code based On 
 sequence homology And/Or phylogenetic relationships.
 
 The print-form ASN.1 version Of this document, which includes all the genetic codes outlined below, Is also available here. 
 Detailed information On codon usage can be found at the Codon Usage Database.
 
 GenBank format by historical convention displays mRNA sequences Using the DNA alphabet. 
 Thus, For the convenience Of people reading GenBank records, the genetic code tables shown here use T instead Of U.
 
 The following genetic codes are described here:
 
 •1. The Standard Code
 •2. The Vertebrate Mitochondrial Code
 •3. The Yeast Mitochondrial Code
 •4. The Mold, Protozoan, And Coelenterate Mitochondrial Code And the Mycoplasma/Spiroplasma Code
 •5. The Invertebrate Mitochondrial Code
 •6. The Ciliate, Dasycladacean And Hexamita Nuclear Code
 •9. The Echinoderm And Flatworm Mitochondrial Code
 •10. The Euplotid Nuclear Code
 •11. The Bacterial, Archaeal And Plant Plastid Code
 •12. The Alternative Yeast Nuclear Code
 •13. The Ascidian Mitochondrial Code
 •14. The Alternative Flatworm Mitochondrial Code
 •16. Chlorophycean Mitochondrial Code
 •21. Trematode Mitochondrial Code
 •22. Scenedesmus obliquus Mitochondrial Code
 •23. Thraustochytrium Mitochondrial Code
 •24. Pterobranchia Mitochondrial Code
 •25. Candidate Division SR1 And Gracilibacteria Code
 
 http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25



### Methods

#### #ctor
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.TranslTable.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Table|资源文件里面的字典数据或者读取自外部文件的数据|


#### __initProfiles
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.TranslTable.__initProfiles(System.Collections.Generic.Dictionary{SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.Codon,SMRUCC.genomics.SequenceModel.Polypeptides.Polypeptides.AminoAcid})
```
生成起始密码子和终止密码子

#### GetTable
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.TranslTable.GetTable(System.Int32)
```
Available index value was described at http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25

|Parameter Name|Remarks|
|--------------|-------|
|index|-|


#### IsInitCoden
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.TranslTable.IsInitCoden(System.String)
```
三个字母所表示的三联体密码子

|Parameter Name|Remarks|
|--------------|-------|
|coden|-|


#### IsStopCoden
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.TranslTable.IsStopCoden(System.Int32)
```
判断某一个密码子是否为终止密码子

|Parameter Name|Remarks|
|--------------|-------|
|hash|该密码子的哈希值|

_returns: 这个密码子是否为一个终止密码_

#### IsStopCodon
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.TranslTable.IsStopCodon(System.String)
```
三个字母所表示的三联体密码子

|Parameter Name|Remarks|
|--------------|-------|
|coden|-|


#### ToCodonCollection
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.TranslTable.ToCodonCollection(SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid)
```
没有终止密码子，非翻译用途的

|Parameter Name|Remarks|
|--------------|-------|
|SequenceData|-|


#### Translate
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.TranslTable.Translate(System.String,System.Boolean)
```
将一条核酸链翻译为蛋白质序列

|Parameter Name|Remarks|
|--------------|-------|
|NucleicAcid|-|
|force|强制程序跳过终止密码子|



### Properties

#### CodenTable
遗传密码子表（哈希表）
#### TranslTable
transl_table=@"P:SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.TranslTable.TranslTable"
