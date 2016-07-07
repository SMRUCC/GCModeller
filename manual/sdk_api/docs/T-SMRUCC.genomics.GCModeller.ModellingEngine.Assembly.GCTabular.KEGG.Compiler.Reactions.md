---
title: Reactions
---

# Reactions
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler.html)_





### Methods

#### CompileCARMEN
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler.Reactions.CompileCARMEN(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction},System.String,SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader,System.String,System.String,Microsoft.VisualBasic.Logging.LogFile)
```


|Parameter Name|Remarks|
|--------------|-------|
|KEGGReactions|整个KEGG Reaction的数据库，酶促反映对象会使用CARMEN软件进行筛选|
|CARMEN_DIR|-|
|ModelLoader|包含有整个KEGG数据库之中的Compound|


#### CompileExpasy
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler.Reactions.CompileExpasy(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction},SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO.XmlresxLoader,SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,SMRUCC.genomics.Model.SBML.Level2.XmlFile,System.String,System.String,Microsoft.VisualBasic.Logging.LogFile)
```
在调用本方法之前，请确认已经将代谢物模型给编译好了

|Parameter Name|Remarks|
|--------------|-------|
|ECTable|-|
|KEGGReactions|-|
|ModelLoader|-|
|MetaCyc|-|
|Sbml|-|
|CompoundsDownloads|-|
|ReactionDownloads|-|
|Logging|-|


#### CompileSmallMoleculeReactions
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.KEGG.Compiler.Reactions.CompileSmallMoleculeReactions(SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.Model.SBML.Level2.Elements.Reaction},System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.Metabolite},System.String,Microsoft.VisualBasic.List{SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.Metabolite}@,Microsoft.VisualBasic.Logging.LogFile)
```
编译所有不受任何酶分子催化的小分子化合物代谢反应过程

|Parameter Name|Remarks|
|--------------|-------|
|MetaCyc|-|
|Sbml|-|



