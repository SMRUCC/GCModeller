# CLI
_namespace: [NCBI.localblast](./index.md)_





### Methods

#### __exportBBH
```csharp
NCBI.localblast.CLI.__exportBBH(System.Collections.Generic.KeyValuePair{SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.AlignEntry,SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.AlignEntry}[],System.Boolean,System.Double,System.Double,System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|entries|-|
|isAll|只导出最好的，还是导出全部匹配上的记录的？|
|coverage|-|
|identities|-|
|singleQuery|-|
|outDIR|-|


#### __mergeFile
```csharp
NCBI.localblast.CLI.__mergeFile(System.Collections.Generic.List{Microsoft.VisualBasic.Data.csv.DocumentStream.File})
```
必须要确保每一个文件之中的列的位置要一致

|Parameter Name|Remarks|
|--------------|-------|
|CsvList|-|


#### __sbhHelper
```csharp
NCBI.localblast.CLI.__sbhHelper(System.String,System.Double,System.Double,System.Boolean)
```
主要是为了节省内存的需要

|Parameter Name|Remarks|
|--------------|-------|
|out|-|
|coverage|-|
|identities|-|


#### BlastpBBHQuery
```csharp
NCBI.localblast.CLI.BlastpBBHQuery(Microsoft.VisualBasic.CommandLine.CommandLine)
```


|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### bLogAnalysis
```csharp
NCBI.localblast.CLI.bLogAnalysis(Microsoft.VisualBasic.CommandLine.CommandLine)
```
分析BLAST程序所输出的日志文件，目标日志文件必须是经过Grep操作得到的
> 最后一个文件对中的File2位最后一个基因组的标号集合

#### ExportBBH
```csharp
NCBI.localblast.CLI.ExportBBH(Microsoft.VisualBasic.CommandLine.CommandLine)
```
从这里批量导出bbh数据

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### GetDiReBh
```csharp
NCBI.localblast.CLI.GetDiReBh(Microsoft.VisualBasic.Data.csv.DocumentStream.File)
```
获取双向的最佳匹配结果

#### Grep
```csharp
NCBI.localblast.CLI.Grep(Microsoft.VisualBasic.CommandLine.CommandLine)
```
解析BLAST日志文件中的标记号名称

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### Merge
```csharp
NCBI.localblast.CLI.Merge(Microsoft.VisualBasic.CommandLine.CommandLine)
```
将多个结果文件整合成一个结果文件，之后就可以调用venn命令生成文氏图了
> 
>  对于多个结果文件的要求：
>  每一个文件中的第一列为相同的物种
>  

#### MergeBestHits
```csharp
NCBI.localblast.CLI.MergeBestHits(Microsoft.VisualBasic.CommandLine.CommandLine)
```


|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### SBHThread
```csharp
NCBI.localblast.CLI.SBHThread(Microsoft.VisualBasic.CommandLine.CommandLine)
```
导出单项最佳比对数据的工作线程

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### Sort
```csharp
NCBI.localblast.CLI.Sort(Microsoft.VisualBasic.Data.csv.DocumentStream.File)
```
[Space][Column1][Column2]

|Parameter Name|Remarks|
|--------------|-------|
|Csv|-|


#### vennBlastAll
```csharp
NCBI.localblast.CLI.vennBlastAll(Microsoft.VisualBasic.CommandLine.CommandLine)
```
两两比对

#### VennCache
```csharp
NCBI.localblast.CLI.VennCache(Microsoft.VisualBasic.CommandLine.CommandLine)
```
1

|Parameter Name|Remarks|
|--------------|-------|
|args|-|



