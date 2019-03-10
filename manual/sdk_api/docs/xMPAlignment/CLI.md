# CLI
_namespace: [xMPAlignment](./index.md)_





### Methods

#### __getSBHhash
```csharp
xMPAlignment.CLI.__getSBHhash(System.String,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Xfam.Pfam.PfamString.PfamString},System.Boolean)
```
Query, Subject()

|Parameter Name|Remarks|
|--------------|-------|
|path|-|


#### AlignFunction
```csharp
xMPAlignment.CLI.AlignFunction(Microsoft.VisualBasic.CommandLine.CommandLine)
```
这个是和KEGG标准数据库来做比较的

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### BuildOrthologDb
```csharp
xMPAlignment.CLI.BuildOrthologDb(Microsoft.VisualBasic.CommandLine.CommandLine)
```
构建功能注释的KEGG标准库

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### DumpPfamString
```csharp
xMPAlignment.CLI.DumpPfamString(Microsoft.VisualBasic.CommandLine.CommandLine)
```
从blastp结果之中解析出结构域数据

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### ExportAppSBH
```csharp
xMPAlignment.CLI.ExportAppSBH(Microsoft.VisualBasic.CommandLine.CommandLine)
```
导出所有的单向比对的结果
 宽松 0.5 coverage / 0.15 identities
 严格 0.8 coverage / 0.5 identities

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### FamilyAlignmentTest
```csharp
xMPAlignment.CLI.FamilyAlignmentTest(Microsoft.VisualBasic.CommandLine.CommandLine)
```
测试用

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### ManualBuild
```csharp
xMPAlignment.CLI.ManualBuild(Microsoft.VisualBasic.CommandLine.CommandLine)
```
手工建立家族数据库

#### MPAlignment
```csharp
xMPAlignment.CLI.MPAlignment(Microsoft.VisualBasic.CommandLine.CommandLine)
```
使用自定义数据库来比对

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### MPAlignment2
```csharp
xMPAlignment.CLI.MPAlignment2(Microsoft.VisualBasic.CommandLine.CommandLine)
```
测试用

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### StructureAlign
```csharp
xMPAlignment.CLI.StructureAlign(Microsoft.VisualBasic.CommandLine.CommandLine)
```
这个操作是ppi比对操作的基础

|Parameter Name|Remarks|
|--------------|-------|
|args|-|



