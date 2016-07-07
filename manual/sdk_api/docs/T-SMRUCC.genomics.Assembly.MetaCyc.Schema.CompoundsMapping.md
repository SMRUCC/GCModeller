---
title: CompoundsMapping
---

# CompoundsMapping
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.Schema](N-SMRUCC.genomics.Assembly.MetaCyc.Schema.html)_

查询通用标准名称在MetaCyc数据库Compound之间的等价



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.CompoundsMapping.#ctor(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.MetaCyc.Schema.ICompoundObject})
```


|Parameter Name|Remarks|
|--------------|-------|
|RightSideCompounds|该类型对象是出现在EffectorMap等式的@"P:SMRUCC.genomics.Assembly.MetaCyc.Schema.EffectorMap.MetaCycId"[右边的UniqueId]|


#### EffectorMapping
```csharp
SMRUCC.genomics.Assembly.MetaCyc.Schema.CompoundsMapping.EffectorMapping(SMRUCC.genomics.Assembly.MetaCyc.Schema.ICompoundObject[])
```
主要的算法思路就是将名称与MetaCyc Compound中的通用名称和同义名进行匹配

|Parameter Name|Remarks|
|--------------|-------|
|CompoundSpecies|-|



### Properties

#### Compounds
用于做等价Mapping的目标数据源
