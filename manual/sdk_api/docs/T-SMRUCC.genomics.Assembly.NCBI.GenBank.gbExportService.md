---
title: gbExportService
---

# gbExportService
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank](N-SMRUCC.genomics.Assembly.NCBI.GenBank.html)_

Genbank export methods collection.



### Methods

#### BatchExport
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService.BatchExport(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File},SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo[]@,SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.gbEntryBrief[]@,System.String,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|list|-|
|GeneList|-|
|GBK|-|
|FastaExport|Fasta序列文件的导出文件夹|


#### BatchExportPlasmid
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService.BatchExportPlasmid(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File},SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.GeneDumpInfo[]@,SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.Plasmid[]@,System.String,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|list|-|
|GeneList|-|
|GBK|-|
|FastaExport|-|
|FastaWithAnnotation|是否将序列的注释信息一同导出来，@"F:Microsoft.VisualBasic.Constants.vbTrue"会将功能注释信息和菌株信息一同导出，@"F:Microsoft.VisualBasic.Constants.vbFalse"则仅仅会导出基因号，假若没有基因号，则会导出蛋白质编号|


#### CopyGenomeSequence
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService.CopyGenomeSequence(System.String,System.String)
```
将PTT文件夹之中的基因组序列数据复制到目标文件夹之中

|Parameter Name|Remarks|
|--------------|-------|
|source|-|


#### Distinct
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService.Distinct(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports.Plasmid})
```
返回去除掉重复的数据之后的AccessionId编号

|Parameter Name|Remarks|
|--------------|-------|
|data|-|


#### ExportGeneAnno
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService.ExportGeneAnno(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File)
```
Exports CDS feature

|Parameter Name|Remarks|
|--------------|-------|
|gbk|-|


#### ExportGeneNtFasta
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService.ExportGeneNtFasta(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File,System.Boolean)
```
{locus_tag, gene.Location.ToString, products.SafeGetValue(locus_tag)?.Function}.
 (导出每一个基因的核酸序列)

|Parameter Name|Remarks|
|--------------|-------|
|gb|Genbank数据库文件|


#### GbffToORF_PTT
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService.GbffToORF_PTT(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File)
```


|Parameter Name|Remarks|
|--------------|-------|
|gb|只导出CDS部分的数据|


#### GbkffExportToPTT
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService.GbkffExportToPTT(SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.File)
```
将GBK文件之中的基因的位置数据导出为PTT格式的数据

|Parameter Name|Remarks|
|--------------|-------|
|Genbank|导出gene和RNA部分的数据|


#### LoadGbkSource
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.gbExportService.LoadGbkSource(System.String,System.String[])
```
假若目标GBK是使用本模块之中的方法保存或者导出来的，则可以使用本方法生成Entry列表；（在返回的结果之中，KEY为文件名，没有拓展名，VALUE为文件的路径）

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|ext|文件类型的拓展名称|



