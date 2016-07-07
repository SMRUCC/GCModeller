---
title: BBHIndex
---

# BBHIndex
_namespace: [SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH](N-SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.html)_

可以使用这个对象来表述@"T:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.I_BlastQueryHit"的所有派生类



### Methods

#### BuildHitsHash
```csharp
SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BBHIndex.BuildHitsHash(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.BBHIndex},System.Boolean,System.Boolean)
```
从bbh结果里面构建出比对信息的哈希表

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|hitsHash|Using @"P:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.I_BlastQueryHit.HitName" as hash key? Default is using @"P:SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.I_BlastQueryHit.QueryName"|
|trim|这个函数里面默认是消除了KEGG的物种简写代码的|



### Properties

#### Properties
动态属性
#### Property
请注意这个属性进行字典的读取的时候，假若不存在，则会返回空字符串，不会报错
