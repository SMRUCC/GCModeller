# CLI
_namespace: [RegPrecise](./index.md)_





### Methods

#### __CORNsiteThread
```csharp
RegPrecise.CLI.__CORNsiteThread(SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifLog,System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.Data.Regprecise.RegPreciseOperon[]},System.Collections.Generic.Dictionary{System.String,System.Collections.Generic.Dictionary{System.String,System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifLog[]}}},System.Collections.Generic.Dictionary{System.String,System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.Data.Regprecise.RegPreciseOperon[]}})
```
成立，会返回位点，不成立，则会返回空值

|Parameter Name|Remarks|
|--------------|-------|
|site|-|


#### __operon
```csharp
RegPrecise.CLI.__operon(SMRUCC.genomics.Data.Regprecise.Regulator,System.Collections.Generic.Dictionary{System.String,System.String[]},System.String[],Microsoft.VisualBasic.ComponentModel.DefaultHashHandle{SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief},Microsoft.VisualBasic.ComponentModel.DefaultHashHandle{SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief})
```


|Parameter Name|Remarks|
|--------------|-------|
|regulon|-|
|members|{RegPrecise -> bbh}|
|TF|-|
|plus|-|
|minus|-|


#### __scanOperon
```csharp
RegPrecise.CLI.__scanOperon(SMRUCC.genomics.Data.Regprecise.Regulator,Microsoft.VisualBasic.Language.List{Microsoft.VisualBasic.ComponentModel.LinkNode{Microsoft.VisualBasic.ComponentModel.IHashValue{SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief}}},System.Int32,System.String,Microsoft.VisualBasic.Language.List{System.String},Microsoft.VisualBasic.Language.List{SMRUCC.genomics.Data.Regprecise.RegPreciseOperon}@,System.String[],System.String[])
```
由于反向链上面的基因是反向排序的，在基因组上面的扫描构建的过程已经变得和正向链的基因一样了

|Parameter Name|Remarks|
|--------------|-------|
|source|基因组上下文|
|n|这个保守的操纵子之中的结构基因的数量|
|strand|链的方向|
|locus|bbh数据库比对结果之中的得到的在目标基因组之中的匹配记录|
|result|所构建的操纵子列表|


#### CORN
```csharp
RegPrecise.CLI.CORN(Microsoft.VisualBasic.CommandLine.CommandLine)
```
Cluster of co-regulated orthologous operons.(假若服务器的内存和性能足够强大，可以直接使用这个函数进行比较，这个函数可能会准确性比较好些)

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### CORNSingleThread
```csharp
RegPrecise.CLI.CORNSingleThread(Microsoft.VisualBasic.CommandLine.CommandLine)
```
单条基因组上下文模型的比对线程

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### DownloadFasta
```csharp
RegPrecise.CLI.DownloadFasta(Microsoft.VisualBasic.CommandLine.CommandLine)
```
下载在Regprecise数据库之中的调控和被调控的基因的蛋白质序列，以方便进行regulon的推测和构建

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### ExportRegulators
```csharp
RegPrecise.CLI.ExportRegulators(Microsoft.VisualBasic.CommandLine.CommandLine)
```
从下载得到的FASTA数据库之中导出所有的调控因子的序列数据

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### FamilyHits
```csharp
RegPrecise.CLI.FamilyHits(Microsoft.VisualBasic.CommandLine.CommandLine)
```
计数调控因子所比对上的家族，这个函数输出两个文件，一个是原始的文件，一个是只取最多的hit的家族的文件

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### MergeDownload
```csharp
RegPrecise.CLI.MergeDownload(Microsoft.VisualBasic.CommandLine.CommandLine)
```
将下载得到的fasta序列文件进行合并

#### OperonBuilder
```csharp
RegPrecise.CLI.OperonBuilder(Microsoft.VisualBasic.CommandLine.CommandLine)
```
得到的只能是一个受共同调控因子调控的基因簇，可能和操纵子不太一样，所以还是使用DOOR的结果

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### SelectTFBBH
```csharp
RegPrecise.CLI.SelectTFBBH(Microsoft.VisualBasic.CommandLine.CommandLine)
```
这个函数会默认将KEGG里面的物种简写代码进行移除

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### Supports
```csharp
RegPrecise.CLI.Supports(Microsoft.VisualBasic.CommandLine.CommandLine)
```
假设文件名都是基因组的名称

|Parameter Name|Remarks|
|--------------|-------|
|args|-|



