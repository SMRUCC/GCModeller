---
title: Reaction
---

# Reaction
_namespace: [SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism](N-SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.html)_





### Methods

#### GetStoichiometry
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.GetStoichiometry(System.String)
```
返回目标代谢物在本反应对象之中的化学计量数，不存在的时候返回0，
 当目标代谢物存在于Reactants列表之中的时候，返回的化学计量数小于0，即目标对象在本方程中是被消耗的对象；
 反之当目标对象存在于Products列表中的时候，返回的化学计量数大于零，即目标对象在本方程之中是合成的目标对象

|Parameter Name|Remarks|
|--------------|-------|
|Metabolite|目标代谢物对象的UniqueID属性值|

_returns: 目标代谢物在本反应对象之中的化学计量数_


### Properties

#### DynamicsRegulators
对反应过程起到调控作用的，而非对酶分子的活性起到调控作用的调控分子
#### Enzymes
UniqueId of the Enzymes.
#### Identifier

#### Metabolites
获取本反应中所使用的所有的反应代谢物
#### Name
Reaction Displaying title.(本生化反应对象的显示标题)
#### p_Dynamics_K_1
正向反应的反应常数
#### p_Dynamics_K_2
该反应的反方向的常数
#### Reversible
本生化反应过程对象是否为可逆的反应对象
