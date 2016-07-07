---
title: Function
---

# Function
_namespace: [SMRUCC.genomics.Assembly.NCBI.COG](N-SMRUCC.genomics.Assembly.NCBI.COG.html)_



> 
>  INFORMATION STORAGE AND PROCESSING
>  [J] Translation, ribosomal structure and biogenesis
>  [A] RNA processing and modification
>  [K] Transcription
>  [L] Replication, recombination and repair
>  [B] Chromatin structure and dynamics
> 
>  CELLULAR PROCESSES AND SIGNALING
>  [D] Cell cycle control, cell division, chromosome partitioning
>  [Y] Nuclear structure
>  [V] Defense mechanisms
>  [T] Signal transduction mechanisms
>  [M] Cell wall/membrane/envelope biogenesis
>  [N] Cell motility
>  [Z] Cytoskeleton
>  [W] Extracellular structures
>  [U] Intracellular trafficking, secretion, and vesicular transport
>  [O] Posttranslational modification, protein turnover, chaperones
> 
>  METABOLISM
>  [C] Energy production and conversion
>  [G] Carbohydrate transport and metabolism
>  [E] Amino acid transport and metabolism
>  [F] Nucleotide transport and metabolism
>  [H] Coenzyme transport and metabolism
>  [I] Lipid transport and metabolism
>  [P] Inorganic ion transport and metabolism
>  [Q] Secondary metabolites biosynthesis, transport and catabolism
> 
>  POORLY CHARACTERIZED
>  [R] General function prediction only
>  [S] Function unknown
> 
>  


### Methods

#### __getCategory
```csharp
SMRUCC.genomics.Assembly.NCBI.COG.Function.__getCategory(System.Char)
```
得到COG分类

|Parameter Name|Remarks|
|--------------|-------|
|cogChar|-|


#### __trimCOGs
```csharp
SMRUCC.genomics.Assembly.NCBI.COG.Function.__trimCOGs(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|value|COGxxxZZZ|


#### CanbeCategoryAs
```csharp
SMRUCC.genomics.Assembly.NCBI.COG.Function.CanbeCategoryAs(System.String,SMRUCC.genomics.Assembly.NCBI.COG.COGCategories)
```
当具有多个COG分类的时候，可以用这个来判断该基因是否可以被分类为指定的大分类

|Parameter Name|Remarks|
|--------------|-------|
|COG|必须是经过@"M:SMRUCC.genomics.BioAssemblyExtensions.GetCOGCategory(System.String)"修剪的字符串|


#### GetCategories
```csharp
SMRUCC.genomics.Assembly.NCBI.COG.Function.GetCategories(System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|category|相加得到的值，可以在这里使用这个函数进行分解|


#### GetCategory
```csharp
SMRUCC.genomics.Assembly.NCBI.COG.Function.GetCategory(System.String)
```
请注意，这个函数只会返回一个COG编号

|Parameter Name|Remarks|
|--------------|-------|
|COG|已经自动处理好所有事情了|


#### Statistics
```csharp
SMRUCC.genomics.Assembly.NCBI.COG.Function.Statistics(System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|lstId|List COG id|



