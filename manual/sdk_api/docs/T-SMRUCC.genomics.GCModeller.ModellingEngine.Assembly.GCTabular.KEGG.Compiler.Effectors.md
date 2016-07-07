---
title: Effectors
---

# Effectors
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler.html)_

本处理过程的目的是将模型之中的Regulation关系之中的Effector替换为KEGG的经过Normalization操作之后的UniqueId

> 
>  首先将KEGGCoumpound在MetaCyc_ALL之间映射
>  


### Methods

#### CreateDictionary
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler.Effectors.CreateDictionary(SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Compounds)
```
可能会有重复的记录，仅仅依靠拓展函数无法解决这个问题，故而专门编写本方法来解决这个问题

|Parameter Name|Remarks|
|--------------|-------|
|MetaCycCompounds|-|


#### MappingEffectors
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler.Effectors.MappingEffectors(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,Microsoft.VisualBasic.List{SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.Metabolite},SMRUCC.genomics.Data.Regprecise.TranscriptionFactors)
```
在当前的这个函数之中已经将MetaCyc的标识符赋值给KEGGCompound了

|Parameter Name|Remarks|
|--------------|-------|
|MetaCyc|-|
|KEGGCompounds|-|
|Regprecise|-|


#### MappingKEGGRegprecise
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler.Effectors.MappingKEGGRegprecise(System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.Metabolite},Microsoft.VisualBasic.List{SMRUCC.genomics.Assembly.MetaCyc.Schema.EffectorMap})
```


|Parameter Name|Remarks|
|--------------|-------|
|KEGGCompounds|{MetaCycId, Metabolite}|
|MetaCycRegpreciseMapping|-|



