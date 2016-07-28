---
title: Taxonomy
---

# Taxonomy
_namespace: [SMRUCC.genomics.Assembly.NCBI](N-SMRUCC.genomics.Assembly.NCBI.html)_

Build Taxonomy tree from NCBI genbank data.



### Methods

#### AcquireAuto
```csharp
SMRUCC.genomics.Assembly.NCBI.Taxonomy.AcquireAuto(System.String)
```
根绝文件的拓展名来识别

|Parameter Name|Remarks|
|--------------|-------|
|dmp|-|


#### Archive
```csharp
SMRUCC.genomics.Assembly.NCBI.Taxonomy.Archive(System.String,System.String)
```
将文本数据库转换为二进制数据库已减少文件体积和加快文件的加载速度

|Parameter Name|Remarks|
|--------------|-------|
|dmp|-|
|bin|-|


#### Hash_gi2Taxi
```csharp
SMRUCC.genomics.Assembly.NCBI.Taxonomy.Hash_gi2Taxi(System.String)
```
Probably you should do the match in the bash first by using script"
 
 ```bash
 grep ">" nt-18S.fasta | cut -f2 -d'|' | sort | uniq >gi.txt
 tabtk_subset /biostack/database/taxonomy/gi_taxid_nucl.dmp gi.txt 1 0 >gi_match.txt 
 ```
 
 Then using the generated ``gi_match.txt`` as the inputs for parameter **dmp**, 
 this operation will save your time, no needs to load the entire database.

|Parameter Name|Remarks|
|--------------|-------|
|dmp|-|



