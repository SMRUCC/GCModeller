---
title: DisposableCompound`1
---

# DisposableCompound`1
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.html)_





### Methods

#### CreatePeptideDisposalObject
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.DisposableCompound`1.CreatePeptideDisposalObject(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.IDisposableCompound,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.MetabolismCompartment,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Compound[],System.String[])
```
[3.4.11.1-RXN] 1 PEPTIDES + 1 WATER --> 1 AMINO-ACIDS-20

|Parameter Name|Remarks|
|--------------|-------|
|Compound|-|
|Metabolism|-|


#### CreateTranscriptDisposalObject
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.DisposableCompound`1.CreateTranscriptDisposalObject(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.IDisposableCompound,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.MetabolismCompartment,SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.DispositionReactant,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Compound[],System.String[])
```
[RXN0-7023] 1 RNASE-R-DEGRADATION-SUBSTRATE-RNA + 1 WATER --> 1 NUCLEOSIDE-MONOPHOSPHATES + 1 DIRIBONUCLEOTIDE

|Parameter Name|Remarks|
|--------------|-------|
|Compound|-|
|Metabolism|-|



### Properties

#### Lambda
介于0-1之间的数，值越大表示越不容易被降解
#### PretendedSubstrate
将要被降解的目标代谢物的假定产物
