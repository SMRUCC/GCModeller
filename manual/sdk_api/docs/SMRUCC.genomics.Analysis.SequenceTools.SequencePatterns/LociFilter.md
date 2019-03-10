# LociFilter
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns](./index.md)_





### Methods

#### __filtering``1
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.LociFilter.__filtering``1(System.Collections.Generic.IEnumerable{``0},System.Func{``0,System.Int32[]},System.Action{``0,System.Int32[]},System.Int32,SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.LociFilter.Compares,System.Boolean,System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|getLocis|-|
|setLocis|-|
|interval|-|
|compare|-|
|returnsAll|-|
|lengthMin|重复片段的最小长度|


#### Filtering``1
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.LociFilter.Filtering``1(System.Collections.Generic.IEnumerable{``0},System.Int32,SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.LociFilter.Compares,System.Boolean,System.Int32)
```
筛选有效的重复片段位点

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|interval|-|
|compare|-|
|returnsAll|-|


#### FilteringRev
```csharp
SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.LociFilter.FilteringRev(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RevRepeatsView},System.Int32,SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.LociFilter.Compares,System.Boolean,System.Int32)
```
根据@``P:SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RevRepeatsView.RevLocis``来进行筛选

|Parameter Name|Remarks|
|--------------|-------|
|data|-|
|interval|-|
|compare|-|
|returnsAll|-|



