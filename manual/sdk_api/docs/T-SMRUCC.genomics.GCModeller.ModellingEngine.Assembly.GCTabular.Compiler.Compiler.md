---
title: Compiler
---

# Compiler
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.html)_





### Methods

#### __Initialize_MetaCyc
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Compiler.__Initialize_MetaCyc(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder)
```
所有与MetaCyc数据库相关的模块变量请在这里初始化

#### _createProteinAssembly
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Compiler._createProteinAssembly(Microsoft.VisualBasic.List{SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.Regulator}@,Microsoft.VisualBasic.Dictionary{SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.Metabolite}@)
```
这个方法主要是用于对于有Effector的TF生成TF的复合物，从而产生代谢组对基因表达调控的约束

#### _createTranscripts
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Compiler._createTranscripts(System.Collections.Generic.KeyValuePair{System.String,System.String}[],System.Collections.Generic.KeyValuePair{System.String,System.String}[])
```
{UniqueId, SequenceData.ToUpper}

|Parameter Name|Remarks|
|--------------|-------|
|GeneSequence|-|
|ProteinSequence|-|


#### AnalysisTransmenbraneFlux
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Compiler.AnalysisTransmenbraneFlux
```
本函数会将@"P:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader.TransmembraneTransportation"解析完毕，并使用@"P:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.MetabolismFlux.Identifier"属性
 从@"P:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader.MetabolismModel"列表之中移除相对应的过程

#### Compile
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Compiler.Compile(Microsoft.VisualBasic.CommandLine.CommandLine)
```


|Parameter Name|Remarks|
|--------------|-------|
|ModelProperty|本参数里面除了模型属性的参数外，还有基因组的注释数据|


#### GetGeneId
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Compiler.GetGeneId(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder)
```
将MetaCyc的基因号映射为NCBI上面的基因号

|Parameter Name|Remarks|
|--------------|-------|
|MetaCyc|-|


#### Link
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Compiler.Link
```
执行连接操作并将临时数据保存至Exported文件夹
> 请注意每一步的函数调用之间是有顺序

#### LinkEffectors
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Compiler.LinkEffectors(Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File)
```
连接效应物到调控因子之上

|Parameter Name|Remarks|
|--------------|-------|
|CrossTalks|-|


#### PreCompile
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Compiler.PreCompile(Microsoft.VisualBasic.CommandLine.CommandLine)
```


|Parameter Name|Remarks|
|--------------|-------|
|argvs|[@"T:Microsoft.VisualBasic.CommandLine.CommandLine"[MetaCyc database data directory|Export directory|RegpreciseRegulators]] -
 -precompile -metacyc "@"F:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Compiler._MetaCyc"" -regprecise_regulator "@"F:SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.Compiler.Compiler._RegpreciseRegulators"" -export "ModelParentDir"
 假若-transcript_regulation参数为空的话，则使用MetaCyc数据库中的Regulation关系数据表|



