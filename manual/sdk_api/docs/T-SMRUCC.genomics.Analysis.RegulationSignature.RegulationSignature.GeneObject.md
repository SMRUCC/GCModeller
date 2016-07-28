---
title: GeneObject
---

# GeneObject
_namespace: [SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature](N-SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.html)_





### Methods

#### __buildRegulations
```csharp
SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.GeneObject.__buildRegulations(SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.SignatureBuilder,System.Text.StringBuilder@)
```
构建调控缩影

|Parameter Name|Remarks|
|--------------|-------|
|Builder|分别抽取三段数据构建调控缩影|
|sBuilder|-|

> 由于**builder**里面的字典中的各个元素都是已经排好序的，所以在这里不可以再使用并行拓展，以防止破环排好序的顺序

#### KOGenerateSequence
```csharp
SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.GeneObject.KOGenerateSequence(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|KO_ID|对于OtherHash，这个参数值使用-空值|


#### TFGenerateSequence
```csharp
SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.GeneObject.TFGenerateSequence(SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.SignatureBuilder)
```
使用这个属性获取得到特征序列

|Parameter Name|Remarks|
|--------------|-------|
|Builder|应用于调控因子上面|


#### ToString
```csharp
SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.GeneObject.ToString
```
调试的时候使用这个接口进行可视化


### Properties

#### GeneID
放在ATG的前面
#### KO
KEGG Pathway的EntryID列表
#### Regulations
受本调控因子所调控的下游基因的基因号列表，这个属性不是直接被序列化的，而是首先在字典之中根据这个值查找出@"P:SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.GeneObject.GeneID"然后在附加@"P:SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.GeneObject.GeneID"的序列化的值
#### TFFamily
假若这个基因还是调控因子的话，则这个属性值不会为空并且@"P:SMRUCC.genomics.Analysis.RegulationSignature.RegulationSignature.GeneObject.Regulations"也不为空
#### TFMotifs
{MotifFamilyName, ATG relative Distance}
