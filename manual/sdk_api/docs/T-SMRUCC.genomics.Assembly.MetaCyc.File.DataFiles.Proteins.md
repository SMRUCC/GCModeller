---
title: Proteins
---

# Proteins
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.html)_

本文件中列举出了所有的蛋白质复合物(CPLX)以及单体蛋白(MONOMER)

> 
>  对于某一种蛋白质而言，其以单体形式存在的时候，没有催化能力，但是在形成了蛋白质复合物之后，具备了催化能力
>  


### Methods

#### GetProteinComplexByComponent
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Proteins.GetProteinComplexByComponent(System.String)
```
递归的使用一个组分对象的UniqueId属性值查询出包含其所有的蛋白质复合物

|Parameter Name|Remarks|
|--------------|-------|
|Component|Component列表中的一个元素|



