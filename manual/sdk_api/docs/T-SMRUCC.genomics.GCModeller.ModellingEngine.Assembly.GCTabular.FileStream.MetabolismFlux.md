---
title: MetabolismFlux
---

# MetabolismFlux
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.html)_





### Methods

#### get_Coefficient
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.MetabolismFlux.get_Coefficient(System.String)
```
If the target metabolite is the reactant of this reaction object, then return -1, return 1 for the target 
 metabolite is the product of this reaction event, and return 0 for the metabolite is not exists in this 
 reaction.
 (底物端为-1，产物端为1，不存在为0)

|Parameter Name|Remarks|
|--------------|-------|
|UniqueId|-|



### Properties

#### _Internal_compilerLeft
编译器所使用的属性 底物端{计量数，UniqueId}
#### _Internal_compilerRight
产物端
#### Enzymes
催化本反应过程的基因或者调控因子(列)，请注意，由于在前半部分为代谢流对象，故而Key的值不是从零开始的
