# TaxonomyNode
_namespace: [SMRUCC.genomics.Assembly.NCBI.Taxonomy](./index.md)_

The tree node calculation model for @``T:SMRUCC.genomics.Assembly.NCBI.Taxonomy.NcbiTaxonomyTree``



### Methods

#### BuildBIOM
```csharp
SMRUCC.genomics.Assembly.NCBI.Taxonomy.TaxonomyNode.BuildBIOM(System.Collections.Generic.IEnumerable{SMRUCC.genomics.Assembly.NCBI.Taxonomy.TaxonomyNode})
```
@``P:SMRUCC.genomics.Metagenomics.BIOMTaxonomy.BIOMPrefix``

|Parameter Name|Remarks|
|--------------|-------|
|nodes|-|


#### Taxonomy
```csharp
SMRUCC.genomics.Assembly.NCBI.Taxonomy.TaxonomyNode.Taxonomy(SMRUCC.genomics.Assembly.NCBI.Taxonomy.TaxonomyNode[],System.String)
```
直接处理@``M:SMRUCC.genomics.Assembly.NCBI.Taxonomy.NcbiTaxonomyTree.GetAscendantsWithRanksAndNames(System.Int32,System.Boolean)``的输出数据，不需要进行额外的排序操作

|Parameter Name|Remarks|
|--------------|-------|
|tree|-|
|delimiter|-|



### Properties

#### parent
当前的节点的父节点的编号: ``@``P:SMRUCC.genomics.Assembly.NCBI.Taxonomy.TaxonomyNode.taxid````
