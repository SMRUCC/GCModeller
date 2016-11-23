# RegpreciseSummary
_namespace: [MEME.Analysis](./index.md)_





### Methods

#### __correlationFilter
```csharp
MEME.Analysis.RegpreciseSummary.__correlationFilter(System.String,System.String[],SMRUCC.genomics.Analysis.RNA_Seq.ICorrelations,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|ORF|-|
|regulators|-|
|correlations|-|
|cutOff|一般是0.6|


#### __createSites
```csharp
MEME.Analysis.RegpreciseSummary.__createSites(SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel.MotifSite,System.Collections.Generic.Dictionary{System.Int32,SMRUCC.genomics.Data.Regprecise.bbhMappings[]},SMRUCC.genomics.Data.Regprecise.WebServices.Regulations,System.Collections.Generic.Dictionary{System.String,SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Module},SMRUCC.genomics.Analysis.RNA_Seq.ICorrelations,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|site|没有调控因子的记录，需要从数据库之中读取数据|
|regulators|需要进行注释的基因组和Regprecise数据库Mapping所得到的结果|


#### __getMapped
```csharp
MEME.Analysis.RegpreciseSummary.__getMapped(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Data.Regprecise.bbhMappings})
```
得到基因组之中被mapped到的调控因子

#### GenerateRegulations
```csharp
MEME.Analysis.RegpreciseSummary.GenerateRegulations(System.Collections.Generic.Dictionary{System.Int32,SMRUCC.genomics.Data.Regprecise.bbhMappings[]},System.Collections.Generic.IEnumerable{SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel.MotifSite},System.String,System.Double)
```
以site位点为基准：从site找调控因子

|Parameter Name|Remarks|
|--------------|-------|
|regulators|-|
|sites|-|


#### LoadRegpreciseBBH
```csharp
MEME.Analysis.RegpreciseSummary.LoadRegpreciseBBH(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|csv|-|


#### SiteToRegulation
```csharp
MEME.Analysis.RegpreciseSummary.SiteToRegulation(SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans.MastSites,SMRUCC.genomics.Analysis.RNA_Seq.ICorrelations,SMRUCC.genomics.Assembly.DOOR.DOOR)
```
直接将MastSite里面的Trace作为调控因子

|Parameter Name|Remarks|
|--------------|-------|
|site|-|
|correlations|-|



