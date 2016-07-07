---
title: make_lattice
---

# make_lattice
_namespace: [RDotNet.Extensions.Bioinformatics.igraph](N-RDotNet.Extensions.Bioinformatics.igraph.html)_

make_lattice is a flexible function, it can create lattices of arbitrary dimensions, periodic or unperiodic ones. It has two forms. 
 In the first form you only supply dimvector, but not length and dim. In the second form you omit dimvector and supply length and dim.




### Properties

#### circular
Logical, if TRUE the lattice or ring will be circular.
#### dim
Integer constant, the dimension of the lattice.
#### dimvector
A vector giving the size of the lattice in each dimension.
#### directed
Whether to create a directed lattice.
#### length
Integer constant, for regular lattices, the size of the lattice in each dimension.
#### mutual
Logical, if TRUE directed lattices will be mutually connected.
#### nei
The distance within which (inclusive) the neighbors on the lattice will be connected. This parameter is not used right now.
