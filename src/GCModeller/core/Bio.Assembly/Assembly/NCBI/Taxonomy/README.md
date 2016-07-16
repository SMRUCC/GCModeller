# NCBI_taxonomy_tree

The NCBI Taxonomy database is a curated set of names and classifications for all of the organisms that are represented in GenBank (http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/).

+ It can be accessed via http://www.ncbi.nlm.nih.gov/Taxonomy/Browser/wwwtax.cgi 
+ or it can be downloaded from ftp://ftp.ncbi.nih.gov/pub/taxonomy/ in the form of 2 files : ``nodes.dmp`` for the structure of the tree and ``names.dmp`` for the names of the different nodes.

Here I make available my in-memory mapping of the NCBI taxonomy : a VisualBasic.NET class that maps the ``names.dmp`` and ``nodes.dmp`` files in a dictionnary which can be used to retrieve lineages, descendants, etc ...

> The source code was translated from the python script: https://github.com/frallain/NCBI_taxonomy_tree

### Usage

The object is built this way :

```vbnet
tree = New NcbiTaxonomyTree(nodes_filename:="./nodes.dmp", names_filename:="./names.dmp")
```

As of July 2015 (the files ``nodes.dmp`` and ``names.dmp`` are respectively 85 mb and 108 mb), this object takes about 14 seconds to be built and takes 480 mb in RAM.
Then we can access to the ``names``, ``ranks``, ``parents`` and ``hildren`` of any nodes :

```vbnet
tree.GetName(28384, 131567)
' {28384: 'other sequences', 131567: 'cellular organisms'}

tree.GetRank(28384, 131567)
' {28384: 'no rank', 131567: 'no rank'}

tree.GetParent(28384, 131567)
' {28384: 1, 131567: 1}

tree.GetChildren(28384, 131567)
' {28384: [2387, 2673, 31896, 36549, 81077], 131567: [2, 2157, 2759]}
```

to the lineages of the nodes, with or without non-standard taxonomical ranks (i.e. ``species``,``genus``,``family``,``order``,``class``,``phylum``,``superkingdom``):

```vbnet
tree.GetAscendantsWithRanksAndNames(1,562)
'{1: [Node(taxid=1, rank='no rank', name='root')],
' 562: [Node(taxid=562, rank='species', name='Escherichia coli'),
'  Node(taxid=561, rank='genus', name='Escherichia'),
'  Node(taxid=543, rank='family', name='Enterobacteriaceae'),
'  Node(taxid=91347, rank='order', name='Enterobacteriales'),
'  Node(taxid=1236, rank='class', name='Gammaproteobacteria'),
'  Node(taxid=1224, rank='phylum', name='Proteobacteria'),
'  Node(taxid=2, rank='superkingdom', name='Bacteria'),
'  Node(taxid=131567, rank='no rank', name='cellular organisms'),
'  Node(taxid=1, rank='no rank', name='root')]}
  
tree.GetAscendantsWithRanksAndNames({562}, only_std_ranks:=True) ' doctest: +NORMALIZE_WHITESPACE
'{562: [Node(taxid=562, rank='species', name='Escherichia coli'),
'  Node(taxid=561, rank='genus', name='Escherichia'),
'  Node(taxid=543, rank='family', name='Enterobacteriaceae'),
'  Node(taxid=91347, rank='order', name='Enterobacteriales'),
'  Node(taxid=1236, rank='class', name='Gammaproteobacteria'),
'  Node(taxid=1224, rank='phylum', name='Proteobacteria'),
'  Node(taxid=2, rank='superkingdom', name='Bacteria')]}
```

to the descendants of some nodes:

```vbnet
taxid2descendants = tree.GetDescendantsWithRanksAndNames(566)
taxid2descendants(566)(1)
' Node(taxid=1115515, rank='no rank', name='Escherichia vulneris NBRC 102420')]}
```

or to all the leaves of the tree :

```vbnet
taxids_leaves_entire_tree = tree.GetLeaves(1)
len(taxids_leaves_entire_tree)
' 1184218
```

or only the leaves of a clade :

```vbnet
taxids_leaves_escherichia_genus = tree.GetLeaves(561)
len(taxids_leaves_escherichia_genus)
' 3382
```

the leaves can be returned with their rank and name :

```vbnet
taxids_leaves_entire_tree = tree.GetLeavesWithRanksAndNames(561)
taxids_leaves_entire_tree(0)
' Node(taxid=1266749, rank='no rank', name='Escherichia coli B1C1')
```

and finally, all nodes at a certain rank can be retrieved :

```vbnet
tree.GetTaxidsAtRank("superkingdom")
' [2, 2157, 2759, 10239, 12884]
```
