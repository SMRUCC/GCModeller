---
title: PfamString
---

# PfamString
_namespace: [SMRUCC.genomics.Data.Xfam.Pfam.PfamString](N-SMRUCC.genomics.Data.Xfam.Pfam.PfamString.html)_

This data object specific for a protein function protein domain structure.



### Methods

#### get_ChouFasmanData
```csharp
SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString.get_ChouFasmanData
```
这个方法仅返回ChouFasman的计算结果，每一个计算结果都会被当作为一个独立的蛋白质对象进行MP的计算

#### GetDomainData
```csharp
SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString.GetDomainData(System.Boolean)
```
从@"P:SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString.PfamString"属性之中返回Pfam的比对数据，请注意，这个函数不会返回ChouFasman的计算数据；
 返回的数据可能是经过排序操作的


### Properties

#### HasChouFasmanData
是否有@"T:SMRUCC.genomics.SequenceModel.Polypeptides.SecondaryStructure.ChouFasman"
 的蛋白质二级结构计算数据
#### Length
The protein sequence length
#### PfamString
在Pfam结构域的数据之间可能会有ChouFasman方法所计算出来的二级结构的数据
