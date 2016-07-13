---
title: SignalTransductionNetwork
---

# SignalTransductionNetwork
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Components](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Components.html)_





### Methods

#### _compile_OCS_RULE
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Components.SignalTransductionNetwork._compile_OCS_RULE
```

> 效应物的编号在左端的第二个，在这里是将MetaCyc和KEGG的代谢物进行合并的，首先会查找出MetaCyc的编号，然后在Mapping之中查找，假若存在KEGGcompound，则使用UniqueId，否则只是用MetaCycID


### Properties

#### CheB
蛋白质的标识号
#### KEGG_Compounds
以KEGG编号为主键的代谢物字典
#### PFAM_CHEB
PF01339
#### PFAM_CHER
PF01739, PF03705
