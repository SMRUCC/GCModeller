# CLI
_namespace: [RNA_seq](./index.md)_





### Methods

#### __export
```csharp
RNA_seq.CLI.__export(SMRUCC.genomics.SequenceModel.SAM.SamStream,SMRUCC.genomics.ContextModel.GenomeContextProvider{SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief},System.Boolean)
```
导出来的结果是依靠@``P:SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment.ID``属性来进行结果统计的

|Parameter Name|Remarks|
|--------------|-------|
|reader|-|
|genome|-|
|showDebug|-|


#### ContactsRef
```csharp
RNA_seq.CLI.ContactsRef(Microsoft.VisualBasic.CommandLine.CommandLine)
```
这个函数是在做完了一次mapping之后，进行更近一步分析用的

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### ExportSAMMaps
```csharp
RNA_seq.CLI.ExportSAMMaps(Microsoft.VisualBasic.CommandLine.CommandLine)
```
小文件适用

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### sIdMapping
```csharp
RNA_seq.CLI.sIdMapping(Microsoft.VisualBasic.CommandLine.CommandLine)
```
如果是CDS feature的话，由于并没有直接的locus_tag，分析起来会不太直观，所以可以使用这个方法将id映射回locus_tag

|Parameter Name|Remarks|
|--------------|-------|
|args|-|



