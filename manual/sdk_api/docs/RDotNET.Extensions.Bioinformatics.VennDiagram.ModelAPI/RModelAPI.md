﻿# RModelAPI
_namespace: [RDotNET.Extensions.Bioinformatics.VennDiagram.ModelAPI](./index.md)_





### Methods

#### Generate
```csharp
RDotNET.Extensions.Bioinformatics.VennDiagram.ModelAPI.RModelAPI.Generate(Microsoft.VisualBasic.Data.csv.DocumentStream.File)
```
从一个Excel逗号分割符文件之中生成一个文氏图的数据模型

|Parameter Name|Remarks|
|--------------|-------|
|source|-|


#### VectorMapper
```csharp
RDotNET.Extensions.Bioinformatics.VennDiagram.ModelAPI.RModelAPI.VectorMapper(System.Collections.Generic.IEnumerable{System.String},System.Func{System.String,System.String[]})
```
从实际的对象映射到venn图里面的实体标记

_returns: 为了保证一一对应的映射关系，这个函数里面不再使用并行化拓展_

#### VectorMapper``1
```csharp
RDotNET.Extensions.Bioinformatics.VennDiagram.ModelAPI.RModelAPI.VectorMapper``1(``0)
```
从实际的对象映射到venn图里面的实体标记

|Parameter Name|Remarks|
|--------------|-------|
|entities|字符串矩阵|


_returns: 为了保证一一对应的映射关系，这个函数里面不再使用并行化拓展_


