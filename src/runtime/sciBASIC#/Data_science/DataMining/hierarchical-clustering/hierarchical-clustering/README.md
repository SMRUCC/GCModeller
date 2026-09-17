# Hierarchical Clustering with BIRCH and Linkage Strategies

Agglomerative hierarchical clustering engine that turns a distance matrix or a stream of vectors into a reusable `Cluster` dendrogram.

## Overview
- JBIRCH streaming clustering: a balanced CF-tree (`CFTree`, `CFNode`, `CFEntry`) with configurable node capacity, distance threshold, distance function (D0-D4) and optional merging refinement.
- Agglomerative clustering through `ClusteringAlgorithm` / `DefaultClusteringAlgorithm`, supporting standard, weighted and flat (threshold cut) clustering.
- Pluggable linkage criteria: single, complete, average (UPGMA) and weighted linkage strategies.
- Reusable hierarchy construction from pre-computed linkages via `DistanceMap`, `Distance`, `HierarchyLink` and `HierarchyBuilder`, plus a parallel Euclidean distance matrix helper in `DoCluster`.

## Key Types
- `Microsoft.VisualBasic.DataMining.HierarchicalClustering.Cluster` — a node of the dendrogram: leaf or merged cluster with children, distance, weight and leaf names.
- `Microsoft.VisualBasic.DataMining.HierarchicalClustering.ClusteringAlgorithm` — interface for performing standard, weighted and flat clustering from a distance matrix.
- `Microsoft.VisualBasic.DataMining.HierarchicalClustering.DefaultClusteringAlgorithm` — default agglomerative implementation driven by a `LinkageStrategy`.
- `Microsoft.VisualBasic.DataMining.HierarchicalClustering.LinkageStrategy` — abstraction for cluster-to-cluster distance, with `SingleLinkageStrategy`, `CompleteLinkageStrategy`, `AverageLinkageStrategy` and `WeightedLinkageStrategy`.
- `Microsoft.VisualBasic.DataMining.HierarchicalClustering.CFTree` — BIRCH clustering feature tree for incremental/streaming inserts.
- `Microsoft.VisualBasic.DataMining.HierarchicalClustering.HierarchyBuilder` — builds a `HierarchyTreeNode` tree from a `DistanceMap`.
- `Microsoft.VisualBasic.DataMining.HierarchicalClustering.DoCluster` — convenience extensions that build the Euclidean distance matrix and run clustering over named vectors.

## Quick Start
```vbnet
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering

' agglomerative clustering from a pre-computed distance matrix
Dim alg As ClusteringAlgorithm = New DefaultClusteringAlgorithm
Dim tree As Cluster = alg.performClustering(distances, names, New AverageLinkageStrategy)

' streaming BIRCH clustering
Dim cf As New CFTree(maxNodeEntries:=50, distThreshold:=0.5)
Call cf.insertEntry(New Double() {1.0, 2.0, 3.0})
```

## NumericTable entry points
The unified 2D-table facade (in `NumericTableExtensions`) provides two channels:

| Channel | Input | Entry points | Notes |
|---|---|---|---|
| Exact | **distance matrix** (square `NumericTable`) | `distanceMatrix()`, then `hca()` / `hcut(k)` / `hcut(threshold)` | Builds the full `n x n` distance matrix and `O(n^2)` linkages; suitable for small/medium `n`. |
| Approximate (BIRCH) | **feature table** | `hcaApprox()` / `hcutApprox(k)` / `hcutApprox(threshold)` | Compresses `n` samples into `m` sub-clusters first; **never materialises the `n x n` distance matrix**. Intended for `n > 20000`. |

```vbnet
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering.BIRCH

' exact channel (feature table -> distance matrix -> dendrogram)
Dim dist = x.distanceMatrix()
Dim tree = dist.hca()
Dim flat = dist.hcut(k:=3)

' approximate channel for large data sets (feature table -> BIRCH -> dendrogram)
Dim options As New BirchOptions With {
    .targetSubclusters = 2000,   ' m, controls the compression ratio
    .maxNodeEntries = 50,        ' BIRCH parameter B
    .silent = True
}
Dim approxTree = x.hcaApprox(options)
Dim approxFlat = x.hcutApprox(k:=6, options:=options)
```

`BirchOptions.threshold` may be set explicitly; when left at `<= 0` the radius is derived
automatically (exponential growth + binary refinement) so that the number of sub-clusters
stays near `targetSubclusters`. Because the second stage only ever sees `m` centroids,
memory and time scale with `m` rather than with `n`.

## Performance notes
- `DistanceMap` is an indexed binary min-heap with lazy deletion and periodic compaction:
  link removal is `O(1)`, insertion/minimum extraction is `O(log m)`. The previous
  sorted-list implementation performed a linear scan on every removal and re-sorted the
  whole link table on every merge, which made the agglomerative stage roughly `O(n^4)`.
- `HierarchyBuilder.Agglomerate` updates the link table in place (no per-iteration PLINQ,
  no per-cluster temporary collections, no per-iteration sort).
- Link hash keys are derived from a monotonic integer `Cluster.Id` (bit-packed, collision
  free) with an avalanche-mixing comparer, instead of hashing/compare cluster names.
- `Cluster.Leafs` is cached incrementally during merges, and `Cluster.LeafNames` is computed
  lazily on first access (previously it was copied eagerly on every merge, `O(n^2)`).
- The BIRCH pre-clustering path disables the periodic memory-limit rebuild by default and no
  longer forces blocking `GC.Collect()` calls during root splits / tree rebuilds.

## Package
- Assembly: `Microsoft.VisualBasic.DataMining.HierarchicalClustering`
- TargetFramework: `net10.0`
- Tags: `scibasic;hierarchical-clustering;birch;linkage;clustering`

## License
GPL-3.0-or-later
