---
title: DbExtensions
---

# DbExtensions
_namespace: [SMRUCC.genomics.Data.Model_Repository.MySQL](N-SMRUCC.genomics.Data.Model_Repository.MySQL.html)_





### Methods

#### GetCorrelateScore
```csharp
SMRUCC.genomics.Data.Model_Repository.MySQL.DbExtensions.GetCorrelateScore(System.Double,System.Double)
```
一般认为WGCNA至少为0.1即可看作为共表达，而在筛选函数之中的过滤参数为0.6，故而WGCNA肯定会被过滤掉，在这里乘以6来避免被过滤

|Parameter Name|Remarks|
|--------------|-------|
|pcc|-|
|spcc|-|



