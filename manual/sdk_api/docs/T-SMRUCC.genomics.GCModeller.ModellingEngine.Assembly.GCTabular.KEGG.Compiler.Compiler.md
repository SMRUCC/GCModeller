---
title: Compiler
---

# Compiler
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler.html)_





### Methods

#### Link
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler.Compiler.Link
```
将没有涉及任何反应过程的代谢物移除,并将重复的代谢反应去除

#### PreCompile
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler.Compiler.PreCompile(Microsoft.VisualBasic.CommandLine.CommandLine)
```
预先将整个KEGG数据库之中的数据读取进入内存之中

|Parameter Name|Remarks|
|--------------|-------|
|argvs|-|



### Properties

#### KEGGCompounds
由于可能需要下载一些残缺的数据，故而KEY的作用是指明原始文件夹以存放所下载的数据
#### KEGGReactions
由于可能需要下载一些残缺的数据，故而KEY的作用是指明原始文件夹以存放所下载的数据
