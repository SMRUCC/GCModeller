---
title: Correlations
---

# Correlations
_namespace: [SMRUCC.genomics.Analysis.Annotations](N-SMRUCC.genomics.Analysis.Annotations.html)_

基因表达相关性的数据库服务



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.Annotations.Correlations.#ctor(Oracle.LinuxCompatibility.MySQL.ConnectionUri)
```


|Parameter Name|Remarks|
|--------------|-------|
|uri|一般情况下这个参数为空，程序会自动根据配置文件来查找数据源|


#### GetPcc
```csharp
SMRUCC.genomics.Analysis.Annotations.Correlations.GetPcc(System.String,System.String)
```
无方向性的

|Parameter Name|Remarks|
|--------------|-------|
|id1|-|
|id2|-|


#### GetPccSignificantThan
```csharp
SMRUCC.genomics.Analysis.Annotations.Correlations.GetPccSignificantThan(System.String,System.Double)
```
@"M:SMRUCC.genomics.Analysis.Annotations.Correlations.GetPccGreaterThan(System.String,System.Double)"不取绝对值，这个函数是取绝对值的

|Parameter Name|Remarks|
|--------------|-------|
|id|-|
|cutoff|-|


#### GetSPcc
```csharp
SMRUCC.genomics.Analysis.Annotations.Correlations.GetSPcc(System.String,System.String)
```
无方向性的

|Parameter Name|Remarks|
|--------------|-------|
|id1|-|
|id2|-|


#### GetWGCNAWeight
```csharp
SMRUCC.genomics.Analysis.Annotations.Correlations.GetWGCNAWeight(System.String,System.String)
```
无方向性的

|Parameter Name|Remarks|
|--------------|-------|
|id1|-|
|id2|-|



