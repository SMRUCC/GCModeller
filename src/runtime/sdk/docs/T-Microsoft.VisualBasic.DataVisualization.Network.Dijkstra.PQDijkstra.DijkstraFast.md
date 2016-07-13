---
title: DijkstraFast
---

# DijkstraFast
_namespace: [Microsoft.VisualBasic.DataVisualization.Network.Dijkstra.PQDijkstra](N-Microsoft.VisualBasic.DataVisualization.Network.Dijkstra.PQDijkstra.html)_

Implements a generalized Dijkstra's algorithm to calculate 
 both minimum distance and minimum path.

>  
>  For this algorithm, all nodes should be provided, and handled 
>  in the delegate methods, including the start and finish nodes. 
>  


### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.DataVisualization.Network.Dijkstra.PQDijkstra.DijkstraFast.#ctor(System.Int32,Microsoft.VisualBasic.DataVisualization.Network.Dijkstra.PQDijkstra.DijkstraFast.InternodeTraversalCost,Microsoft.VisualBasic.DataVisualization.Network.Dijkstra.PQDijkstra.DijkstraFast.NearbyNodesHint)
```
Creates an instance of the @"N:Microsoft.VisualBasic.DataVisualization.Network.Dijkstra" class.

|Parameter Name|Remarks|
|--------------|-------|
|totalNodeCount__1| 
 The total number of nodes in the graph. 
 |
|traversalCost__2| 
 The delegate that can provide the cost of a transition between 
 any two nodes. 
 |
|hint__3| 
 An optional delegate that can provide a small subset of nodes 
 that a given node may be connected to. 
 |



