---
title: PhenotypeRegulations
---

# PhenotypeRegulations
_namespace: [SMRUCC.genomics.Analysis.CellPhenotype](N-SMRUCC.genomics.Analysis.CellPhenotype.html)_

将MEME所分析出来的调控信息附加到代谢途径的网络图之中



### Methods

#### __levelMapping
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PhenotypeRegulations.__levelMapping(SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel.DataSerials{System.Double}[],System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|DataChunk|都是对同一个基因的分组数据|


#### __ranking
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PhenotypeRegulations.__ranking(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|Matrix|-|
|Level|映射的等级数目|


#### AssignPhenotype
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PhenotypeRegulations.AssignPhenotype(System.Collections.Generic.IEnumerable{SMRUCC.genomics.InteractionModel.Regulon.IRegulatorRegulation},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.KEGG.Archives.Csv.Pathway})
```
仅仅只是根据调控数据将表型调控赋值给指定的调控因子,没有计算得分的过程

|Parameter Name|Remarks|
|--------------|-------|
|Regulations|-|
|Pathways|-|


#### AssignPhenotype2
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PhenotypeRegulations.AssignPhenotype2(System.Collections.Generic.IEnumerable{SMRUCC.genomics.InteractionModel.Regulon.IRegulatorRegulation},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.KEGG.Archives.Csv.Pathway},System.String)
```
仅仅只是根据调控数据将表型调控赋值给指定的调控因子,没有计算得分的过程

|Parameter Name|Remarks|
|--------------|-------|
|Regulations|-|
|Pathways|-|


#### CommandLineTools
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PhenotypeRegulations.CommandLineTools(System.String,System.String,System.String,System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|dir|计算数据的Export文件夹|


#### CreateDynamicNetwork
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PhenotypeRegulations.CreateDynamicNetwork(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.RegulatesFootprints},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.NetworkInput})
```
简要版本是没有长度映射的

|Parameter Name|Remarks|
|--------------|-------|
|footprints|-|
|Inits|-|


#### CreateEmptyInput
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PhenotypeRegulations.CreateEmptyInput(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat.RegulatesFootprints},System.String)
```
生成网络计算模型的状态输入数据

|Parameter Name|Remarks|
|--------------|-------|
|NetworkModel|-|
|SaveToPath|-|


#### CreateExpressionMatrix
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PhenotypeRegulations.CreateExpressionMatrix(System.String,System.Double)
```
从蒙特卡洛实验的计算数据之中生成实验样本，即将多个计算样本合并为一个矩阵（数据预处理阶段），最后生成的数据都是没有时间标签的

|Parameter Name|Remarks|
|--------------|-------|
|dir|-|


#### CreateMutationInit
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PhenotypeRegulations.CreateMutationInit(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.NetworkInput},Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.Int32)
```
将野生型的蒙特卡洛实验数据之中的后半部分的稳定状态的数据转换为网络输入

|Parameter Name|Remarks|
|--------------|-------|
|inits|在本参数之中，仅含有**MAT**之中的一部分数据|
|MAT|-|


#### ExportTCSCrossTalksCytoscape
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PhenotypeRegulations.ExportTCSCrossTalksCytoscape(SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader,Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,System.String)
```
将计算得到的矩阵数据之中导出TCS的CrossTalk的Cytoscape的网络XML文件，并分别保存至**Export_to**所指定的文件夹之下

|Parameter Name|Remarks|
|--------------|-------|
|Model|-|
|MAT|-|
|Export_to|-|


#### MonteCarloExperiment
```csharp
SMRUCC.genomics.Analysis.CellPhenotype.PhenotypeRegulations.MonteCarloExperiment(SMRUCC.genomics.Analysis.CellPhenotype.Simulation.ExpressionRegulationNetwork.BinaryNetwork,System.Int32,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|network|-|
|counts|实验的重复次数，建议至少1000次以上|
|export|实验数据的导出位置|



