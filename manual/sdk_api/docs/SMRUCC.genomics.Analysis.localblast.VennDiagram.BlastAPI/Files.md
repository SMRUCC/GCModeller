# Files
_namespace: [SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI](./index.md)_





### Methods

#### SelectCopy
```csharp
SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI.Files.SelectCopy(System.String,System.String,System.String,System.Double,System.Double)
```
There are some times the bbh data source size is too large for the bi-directionary best hit blastp, 
 so that you needs to select the genome source first, using this method by the besthit method to 
 filtering the raw data.
 (可能有时候需要进行两两双向比对的数据太多了，故而需要先进行单向比对，在使用这个函数将原数据拷贝出来之后，
 再进行单向必对)

|Parameter Name|Remarks|
|--------------|-------|
|BlastoutputSource|-|
|EXPORT|-|
|trimValue|默认是匹配上60%个Query基因组的基因数目|



