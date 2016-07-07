---
title: Codon
---

# Codon
_namespace: [SMRUCC.genomics.SequenceModel.NucleotideModels.Translation](N-SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.html)_

密码子对象



### Methods

#### #ctor
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.Codon.#ctor(System.String[])
```
翻译用途的

|Parameter Name|Remarks|
|--------------|-------|
|Tokens|-|


#### CreateHashTable
```csharp
SMRUCC.genomics.SequenceModel.NucleotideModels.Translation.Codon.CreateHashTable
```
生成非翻译用途的密码子列表


### Properties

#### CodonValue
返回三联体密码子的核酸片段
#### IsStopCodon
是否为终止密码子
#### TranslHash
第一个碱基*1000+第二个碱基*100+第三个碱基
#### X
密码子中的第一个碱基
#### Y
密码子中的第二个碱基
#### Z
密码子中的第三个碱基
