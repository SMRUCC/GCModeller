---
title: AnnotationTool
---

# AnnotationTool
_namespace: [SMRUCC.genomics.Analysis.Annotations](N-SMRUCC.genomics.Analysis.Annotations.html)_

注释的流程为：
 以此比对数据库，通过BBH得到直系同源

> 
>  对于BBH获取直系同源的基因的过程一般是这样的：
>  当所比对的注释源较小的时候，可以直接使用BBH进行比对，之后再根据数据库之中的物种数据分组获取BBH结果即可，故而，对于较小的fasta数据库，注释工具的初始化参数new.db为fasta文件的路径
>  当所比对的注释源很大的时候，进行BBH会非常慢，故而这个时候需要将数据库分割为不同的模块进行BBH操作，当BBH操作结束的时候，在合并为一个数据源按照数据库之中的物种信息进行分组获取BBH数据
>  


### Methods

#### GetAnnotationSourceMeta
```csharp
SMRUCC.genomics.Analysis.Annotations.AnnotationTool.GetAnnotationSourceMeta
```
获取所比对的蛋白质的信息的物种来源的信息

#### InvokeAnnotation
```csharp
SMRUCC.genomics.Analysis.Annotations.AnnotationTool.InvokeAnnotation(System.String,System.String,System.Boolean,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Fasta|-|
|Export|-|
|Parallel|并行模型|



