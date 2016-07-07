---
title: graph_from_edgelist
---

# graph_from_edgelist
_namespace: [RDotNet.Extensions.Bioinformatics.igraph](N-RDotNet.Extensions.Bioinformatics.igraph.html)_

graph_from_edgelist creates a graph from an edge list. Its argument is a two-column matrix, each row defines one edge.
 If it is a numeric matrix then its elements are interpreted as vertex ids.
 If it is a character matrix then it is interpreted as symbolic vertex names and a vertex id will be assigned to each name, and also a name vertex attribute will be added.




### Properties

#### directed
Whether to create a directed graph.
#### el
The edge list, a two column matrix, character or numeric.
