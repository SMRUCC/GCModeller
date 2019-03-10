# CLI
_namespace: [xVirtualFootprint](./index.md)_





### Methods

#### __copy
```csharp
xVirtualFootprint.CLI.__copy(SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifLog,System.String,System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|x|-|
|TF|RegulatorTrace|
|locus|Regulator|
|family|-|


#### alloacte
```csharp
xVirtualFootprint.CLI.alloacte(SMRUCC.genomics.SequenceModel.FASTA.FastaFile,System.Nullable{System.Boolean})
```
并行算法似乎會因爲内存資源的讀取問題而在linux平臺上面出現較高的系統CPU時間
 在這裏創建新對象來解決這個問題

|Parameter Name|Remarks|
|--------------|-------|
|seq|-|


#### BuildFootprints
```csharp
xVirtualFootprint.CLI.BuildFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)
```
调控位点分析

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### CastLogAsFootprints
```csharp
xVirtualFootprint.CLI.CastLogAsFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)
```
假若tag数据里面是调控因子的话，可以用这个函数进行转换

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### ContextMappings
```csharp
xVirtualFootprint.CLI.ContextMappings(Microsoft.VisualBasic.CommandLine.CommandLine)
```
将相对丰度映射为逻辑位置

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### MergeRegulonsExport
```csharp
xVirtualFootprint.CLI.MergeRegulonsExport(Microsoft.VisualBasic.CommandLine.CommandLine)
```
合并bbh得到的regulon，得到可能的完整的regulon

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### Scanner
```csharp
xVirtualFootprint.CLI.Scanner(Microsoft.VisualBasic.CommandLine.CommandLine)
```
使用一个特定的motif信息来扫描指定的基因组的序列

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### SiteScreens
```csharp
xVirtualFootprint.CLI.SiteScreens(Microsoft.VisualBasic.CommandLine.CommandLine)
```
位点数太少了

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### TestFootprints
```csharp
xVirtualFootprint.CLI.TestFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)
```
Footprint文件可能会比较大

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### TFDensity
```csharp
xVirtualFootprint.CLI.TFDensity(Microsoft.VisualBasic.CommandLine.CommandLine)
```
计算出调控因子在基因组上面的分布相对丰度

|Parameter Name|Remarks|
|--------------|-------|
|args|-|



