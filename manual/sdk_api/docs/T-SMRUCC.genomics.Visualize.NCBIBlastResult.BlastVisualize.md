---
title: BlastVisualize
---

# BlastVisualize
_namespace: [SMRUCC.genomics.Visualize.NCBIBlastResult](N-SMRUCC.genomics.Visualize.NCBIBlastResult.html)_





### Methods

#### __createHits
```csharp
SMRUCC.genomics.Visualize.NCBIBlastResult.BlastVisualize.__createHits(System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo},SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.v228)
```
只需要找出有hits的query，然后将位置列举出来即可

|Parameter Name|Remarks|
|--------------|-------|
|ORF|-|
|Blastoutput|-|


#### ApplyDescription
```csharp
SMRUCC.genomics.Visualize.NCBIBlastResult.BlastVisualize.ApplyDescription(SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.AlignmentTable,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief},System.Int32)
```
将编号信息转换为描述信息

|Parameter Name|Remarks|
|--------------|-------|
|table|-|
|info|-|


#### ApplyDescription2
```csharp
SMRUCC.genomics.Visualize.NCBIBlastResult.BlastVisualize.ApplyDescription2(SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.AlignmentTable,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief},System.Int32)
```
将编号信息转换为描述信息

|Parameter Name|Remarks|
|--------------|-------|
|table|-|
|info|-|


#### CreateTableFromBlastOutput
```csharp
SMRUCC.genomics.Visualize.NCBIBlastResult.BlastVisualize.CreateTableFromBlastOutput(System.String,System.String,System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo})
```
这个函数主要是针对blastp的结果的

|Parameter Name|Remarks|
|--------------|-------|
|source|blast输出日志文件夹|


#### InvokeDrawing
```csharp
SMRUCC.genomics.Visualize.NCBIBlastResult.BlastVisualize.InvokeDrawing(SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.AlignmentTable,SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT,System.String[],System.Int32,System.Boolean,System.String,SMRUCC.genomics.SequenceModel.FASTA.FastaToken,System.Boolean,System.Boolean,System.Boolean,System.Boolean,System.Boolean,System.String,System.Boolean,System.Double,SMRUCC.genomics.Visualize.ComparativeGenomics.ModelAPI.ICOGsBrush)
```
对blast结果进行可视化

|Parameter Name|Remarks|
|--------------|-------|
|alignment|-|
|refQuery|-|
|AlignmentColorSchema|bit_scores, identities|


#### ShortID
```csharp
SMRUCC.genomics.Visualize.NCBIBlastResult.BlastVisualize.ShortID(SMRUCC.genomics.SequenceModel.FASTA.FastaFile,System.String)
```
对Entry信息进行简化

|Parameter Name|Remarks|
|--------------|-------|
|data|-|



### Properties

#### ConvertFactor
一个碱基或者一个氨基酸所对应的像素
