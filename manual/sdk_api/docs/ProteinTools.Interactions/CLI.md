# CLI
_namespace: [ProteinTools.Interactions](./index.md)_





### Methods

#### AlignLDM
```csharp
ProteinTools.Interactions.CLI.AlignLDM(System.String,System.String,System.String[],SMRUCC.genomics.Interops.ClustalOrg.Clustal,System.String,Microsoft.VisualBasic.ComponentModel.KeyValuePair[]@)
```


|Parameter Name|Remarks|
|--------------|-------|
|HisK|-|
|RR|-|


#### CrossTalksCal
```csharp
ProteinTools.Interactions.CLI.CrossTalksCal(Microsoft.VisualBasic.CommandLine.CommandLine)
```
做MP比对，得到的结果得分和贝叶斯的计算得分相乘，之后求总和再取平均即为互作的可能性

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### DbMergeFromExists
```csharp
ProteinTools.Interactions.CLI.DbMergeFromExists(Microsoft.VisualBasic.CommandLine.CommandLine)
```
从已经存在的多重比对的数据之中建立数据库

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### MergePfam
```csharp
ProteinTools.Interactions.CLI.MergePfam(Microsoft.VisualBasic.CommandLine.CommandLine)
```
将可用的序列数据合并在一起然后做Pfam分析导出Pfam-String数据

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### Predicts
```csharp
ProteinTools.Interactions.CLI.Predicts(Microsoft.VisualBasic.CommandLine.CommandLine)
```
会将该分类之下的序列倒出来做一次多序列比对
 然后将分别进行特征比较，得分最高的特征为其最终的得分

|Parameter Name|Remarks|
|--------------|-------|
|args|-|



