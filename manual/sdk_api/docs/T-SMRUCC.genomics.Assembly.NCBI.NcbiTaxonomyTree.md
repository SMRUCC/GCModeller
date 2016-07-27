---
title: NcbiTaxonomyTree
---

# NcbiTaxonomyTree
_namespace: [SMRUCC.genomics.Assembly.NCBI](N-SMRUCC.genomics.Assembly.NCBI.html)_

Builds the following dictionnary from NCBI taxonomy ``nodes.dmp`` and 
 ``names.dmp`` files 
 
 ```json
 { Taxid : namedtuple('Node', ['name', 'rank', 'parent', 'children'] }
 ```
 
 + https://www.biostars.org/p/13452/
 + https://pythonhosted.org/ete2/tutorial/tutorial_ncbitaxonomy.html

> 
>  https://github.com/frallain/NCBI_taxonomy_tree
>  
>  #### NCBI_taxonomy_tree
> 
>  The NCBI Taxonomy database Is a curated Set Of names And classifications For all Of the organisms that are 
>  represented In GenBank (http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/). It can be accessed 
>  via http://www.ncbi.nlm.nih.gov/Taxonomy/Browser/wwwtax.cgi Or it can be downloaded from 
>  ftp://ftp.ncbi.nih.gov/pub/taxonomy/ in the form of 2 files : ``nodes.dmp`` for the structure of the tree 
>  And ``names.dmp`` for the names of the different nodes.
> 
>  Here I make available my In-memory mapping Of the NCBI taxonomy : a Python 2.7 Class that maps the ``names.dmp`` 
>  And ``nodes.dmp`` files In a Python dictionnary which can be used To retrieve lineages, descendants, etc ...
>  


### Methods

#### #ctor
```csharp
SMRUCC.genomics.Assembly.NCBI.NcbiTaxonomyTree.#ctor(System.String,System.String)
```
Builds the following dictionnary from NCBI taxonomy ``nodes.dmp`` and 
 ``names.dmp`` files 
 
 ```json
 { Taxid namedtuple('Node', ['name', 'rank', 'parent', 'children'] }
 ```
 
 + https://www.biostars.org/p/13452/
 + https://pythonhosted.org/ete2/tutorial/tutorial_ncbitaxonomy.html

|Parameter Name|Remarks|
|--------------|-------|
|nodes_filename|-|
|names_filename|-|


#### flatten
```csharp
SMRUCC.genomics.Assembly.NCBI.NcbiTaxonomyTree.flatten(System.Collections.IEnumerable)
```
```
 >>> flatten([1 , [2, 2], [2, [3, 3, 3]]]) 
 [1, 2, 2, 2, 3, 3, 3]
 ```

|Parameter Name|Remarks|
|--------------|-------|
|seq|-|


#### GetAscendantsWithRanksAndNames
```csharp
SMRUCC.genomics.Assembly.NCBI.NcbiTaxonomyTree.GetAscendantsWithRanksAndNames(System.Collections.Generic.IEnumerable{System.Int32},System.Boolean)
```
使用这个函数得到物种的具体分类，返回来的数据是从小到大排列的

|Parameter Name|Remarks|
|--------------|-------|
|taxids|-|
|only_std_ranks|-|


#### GetDescendants
```csharp
SMRUCC.genomics.Assembly.NCBI.NcbiTaxonomyTree.GetDescendants(System.Int32[])
```
Returns all the descendant taxids from a branch/clade 
 of a list of taxids : all nodes (leaves or not) of the 
 tree are returned including the original one.

|Parameter Name|Remarks|
|--------------|-------|
|taxids|-|


#### GetDescendantsWithRanksAndNames
```csharp
SMRUCC.genomics.Assembly.NCBI.NcbiTaxonomyTree.GetDescendantsWithRanksAndNames(System.Int32[])
```
Returns the ordered list of the descendants with their respective ranks and names for a LIST of taxids.

|Parameter Name|Remarks|
|--------------|-------|
|taxids|-|


#### GetLeaves
```csharp
SMRUCC.genomics.Assembly.NCBI.NcbiTaxonomyTree.GetLeaves(System.Int32)
```
Returns all the descendant taxids that are leaves of the tree from 
 a branch/clade determined by ONE taxid.

|Parameter Name|Remarks|
|--------------|-------|
|taxid|-|


#### GetLeavesWithRanksAndNames
```csharp
SMRUCC.genomics.Assembly.NCBI.NcbiTaxonomyTree.GetLeavesWithRanksAndNames(System.Int32)
```
Returns all the descendant taxids that are leaves of the tree from 
 a branch/clade determined by ONE taxid.

|Parameter Name|Remarks|
|--------------|-------|
|taxid|-|


#### GetParent
```csharp
SMRUCC.genomics.Assembly.NCBI.NcbiTaxonomyTree.GetParent(System.Int32[])
```
Returns parent id

|Parameter Name|Remarks|
|--------------|-------|
|taxids|-|


#### GetTaxidsAtRank
```csharp
SMRUCC.genomics.Assembly.NCBI.NcbiTaxonomyTree.GetTaxidsAtRank(System.String)
```
Returns all the taxids that are at a specified rank: 
 
 + standard ranks: 
 ``species, genus, family, order, class, phylum, superkingdom.``
 + non-standard ranks: 
 ``forma, varietas, subspecies, species group, subtribe, tribe, subclass, kingdom.``

|Parameter Name|Remarks|
|--------------|-------|
|rank|-|


#### preorderTraversal
```csharp
SMRUCC.genomics.Assembly.NCBI.NcbiTaxonomyTree.preorderTraversal(System.Int32,System.Boolean)
```
Prefix (Preorder) visit of the tree: https://en.wikipedia.org/wiki/Tree_traversal

|Parameter Name|Remarks|
|--------------|-------|
|taxid|-|
|only_leaves|-|



### Properties

#### stdranks
+ species
 + genus
 + family
 + order
 + class
 + phylum
 + superkingdom
