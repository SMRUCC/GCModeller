---
title: PhenoTree
tags: [maunal, tools]
date: 5/28/2018 8:37:23 PM
---
# GCModeller [version 1.0.0.0]
> Cellular phenotype analysis tools.

<!--more-->

**PhenoTree**<br/>
__<br/>
Copyright ©  2016

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/PhenoTree.exe<br/>
**Root namespace**: ``PhenoTree.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Cluster.Enrichment](#/Cluster.Enrichment)||
|[/Cluster.Genes.Phenotype](#/Cluster.Genes.Phenotype)||
|[/locis.clustering](#/locis.clustering)||
|[/Parts.COGs](#/Parts.COGs)||
|[/Tree.Partitions](#/Tree.Partitions)||
|[/venn.Matrix](#/venn.Matrix)||

## CLI API list
--------------------------
<h3 id="/Cluster.Enrichment"> 1. /Cluster.Enrichment</h3>


**Prototype**: ``PhenoTree.CLI::Int32 ClusterEnrichment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
PhenoTree /Cluster.Enrichment /in <partitions.json> /go.anno <proteins.go.annos.csv> [/go.brief <go_brief.csv> /out <out.DIR>]
```
<h3 id="/Cluster.Genes.Phenotype"> 2. /Cluster.Genes.Phenotype</h3>


**Prototype**: ``PhenoTree.CLI::Int32 GenePhenoClusters(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
PhenoTree /Cluster.Genes.Phenotype /sites <motifSites.csv> [/out <out.tree_cluster.csv> /parallel]
```
<h3 id="/locis.clustering"> 3. /locis.clustering</h3>


**Prototype**: ``PhenoTree.CLI::Int32 LociClustering(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
PhenoTree /locis.clustering /in <locis.fasta> [/cut <0> /first.ID /method <NeedlemanWunsch> /colors <clusters> /clusters <20> /out <out.DIR>]
```
<h3 id="/Parts.COGs"> 4. /Parts.COGs</h3>


**Prototype**: ``PhenoTree.CLI::Int32 PartitioningCOGs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
PhenoTree /Parts.COGs /cluster <btree.clusters.csv> /myva <COGs.csv> [/depth <-1> /out <EXPORT_DIR>]
```
<h3 id="/Tree.Partitions"> 5. /Tree.Partitions</h3>


**Prototype**: ``PhenoTree.CLI::Int32 TreePartitions(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
PhenoTree /Tree.Partitions /in <btree.network.DIR> [/quantile <0.99> /out <out.DIR>]
```
<h3 id="/venn.Matrix"> 6. /venn.Matrix</h3>


**Prototype**: ``PhenoTree.CLI::Int32 VennMatrix(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
PhenoTree /venn.Matrix /besthits <besthits.xml.DIR> [/query <sp.name> /limits -1 /out <out.txt>]
```
