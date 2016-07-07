---
title: PccMatrix
---

# PccMatrix
_namespace: [SMRUCC.genomics.Analysis.RNA_Seq](N-SMRUCC.genomics.Analysis.RNA_Seq.html)_

Pearson correlation coefficient calculator.
 (因为为了查找字典方便，所以里面的所有的编号都已经被转换为大写形式了，在查找的时候应该要注意)



### Methods

#### GetValue
```csharp
SMRUCC.genomics.Analysis.RNA_Seq.PccMatrix.GetValue(System.String,System.String,System.Boolean)
```
Get the pcc value between the specified two gene object.(获取任意两个基因之间的Pcc系数，请注意，所有的编号应该是大写的)

|Parameter Name|Remarks|
|--------------|-------|
|Id1|-|
|Id2|-|
|Parallel|本参数无任何用处，仅仅是为了保持接口的统一性而设置的|



### Properties

#### _pccValues

#### lstGenes
顺序是与@"F:SMRUCC.genomics.Analysis.RNA_Seq.PccMatrix._pccValues"之中的对象是一一对应的
#### PCC_SPCC_MixedType
当前的这个矩阵对象是否为皮尔森系数和斯皮尔曼相关性系数的混合矩阵？
