# NCBI_taxonomy_tree

The NCBI Taxonomy database is a curated set of names and classifications for all of the organisms that are represented in GenBank (http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/).
It can be accessed via http://www.ncbi.nlm.nih.gov/Taxonomy/Browser/wwwtax.cgi or it can be downloaded from ftp://ftp.ncbi.nih.gov/pub/taxonomy/ in the form of 2 files : nodes.dmp for the structure of the tree and names.dmp for the names of the different nodes.

Here I make available my in-memory mapping of the NCBI taxonomy : a Python 2.7 class that maps the names.dmp and nodes.dmp files in a Python dictionnary which can be used to retrieve lineages, descendants, etc ...

The object is built this way :

```python
tree = NcbiTaxonomyTree(nodes_filename="./nodes.dmp", names_filename="./names.dmp")
```

As of July 2015 (the files nodes.dmp and names.dmp are respectively 85 mb and 108 mb), this object takes about 14 seconds to be built and takes 480 mb in RAM.
Then we can access to the names, ranks, parents and children of any nodes :

```python
>>> tree.getName([28384, 131567])
{28384: 'other sequences', 131567: 'cellular organisms'}

>>> tree.getRank([28384, 131567])
{28384: 'no rank', 131567: 'no rank'}

>>> tree.getParent([28384, 131567])
{28384: 1, 131567: 1}

>>> tree.getChildren([28384, 131567])
{28384: [2387, 2673, 31896, 36549, 81077], 131567: [2, 2157, 2759]}
```

to the lineages of the nodes, with or without non-standard taxonomical ranks (i.e. 'species','genus','family','order','class','phylum','superkingdom'):

```python
>>> tree.getAscendantsWithRanksAndNames([1,562])
{1: [Node(taxid=1, rank='no rank', name='root')],
 562: [Node(taxid=562, rank='species', name='Escherichia coli'),
  Node(taxid=561, rank='genus', name='Escherichia'),
  Node(taxid=543, rank='family', name='Enterobacteriaceae'),
  Node(taxid=91347, rank='order', name='Enterobacteriales'),
  Node(taxid=1236, rank='class', name='Gammaproteobacteria'),
  Node(taxid=1224, rank='phylum', name='Proteobacteria'),
  Node(taxid=2, rank='superkingdom', name='Bacteria'),
  Node(taxid=131567, rank='no rank', name='cellular organisms'),
  Node(taxid=1, rank='no rank', name='root')]}
  
>>> tree.getAscendantsWithRanksAndNames([562], only_std_ranks=True) # doctest: +NORMALIZE_WHITESPACE
{562: [Node(taxid=562, rank='species', name='Escherichia coli'),
  Node(taxid=561, rank='genus', name='Escherichia'),
  Node(taxid=543, rank='family', name='Enterobacteriaceae'),
  Node(taxid=91347, rank='order', name='Enterobacteriales'),
  Node(taxid=1236, rank='class', name='Gammaproteobacteria'),
  Node(taxid=1224, rank='phylum', name='Proteobacteria'),
  Node(taxid=2, rank='superkingdom', name='Bacteria')]}
```

to the descendants of some nodes:

```python
>>> taxid2descendants = tree.getDescendantsWithRanksAndNames([566])
>>> taxid2descendants[566][1]
Node(taxid=1115515, rank='no rank', name='Escherichia vulneris NBRC 102420')]}
```

or to all the leaves of the tree :

```python
>>> taxids_leaves_entire_tree = tree.getLeaves(1)
>>> len(taxids_leaves_entire_tree)
1184218
```

or only the leaves of a clade :

```python
>>> taxids_leaves_escherichia_genus = tree.getLeaves(561)
>>> len(taxids_leaves_escherichia_genus)
3382
```

the leaves can be returned with their rank and name :

```python
>>> taxids_leaves_entire_tree = tree.getLeavesWithRanksAndNames(561)
>>> taxids_leaves_entire_tree[0]
Node(taxid=1266749, rank='no rank', name='Escherichia coli B1C1')
```

and finally, all nodes at a certain rank can be retrieved :

```python
>>> tree.getTaxidsAtRank('superkingdom')
[2, 2157, 2759, 10239, 12884]
```

