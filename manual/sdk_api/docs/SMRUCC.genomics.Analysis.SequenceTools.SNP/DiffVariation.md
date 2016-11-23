# DiffVariation
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SNP](./index.md)_





### Methods

#### GetSeqs
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SNP.DiffVariation.GetSeqs(System.Collections.Generic.IEnumerable{SMRUCC.genomics.SequenceModel.FASTA.FastaToken},System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|aln|必须是经过对齐了的，第一条序列为参考序列，假若参考可选参数是缺失的话|
|refIndex|默认是第一条序列，如果index参数是缺失的话|


#### GroupByDate
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SNP.DiffVariation.GroupByDate(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.SequenceTools.SNP.KSeq},System.Nullable{System.Boolean},Microsoft.VisualBasic.Language.List{Microsoft.VisualBasic.Data.csv.DocumentStream.EntityObject}@)
```


|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|cumulative|数据是否是按照日期的累计性的|
|raw|原始的分数数据|



