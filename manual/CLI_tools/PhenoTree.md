---
title: PhenoTree
tags: [maunal, tools]
date: 11/24/2016 2:54:14 AM
---
# GCModeller [version 1.0.0.0]
> Cellular phenotype analysis tools.

<!--more-->

**PhenoTree**<br/>
__<br/>
Copyright ©  2016

**Module AssemblyName**: file:///G:/GCModeller/GCModeller/bin/PhenoTree.exe<br/>
**Root namespace**: ``PhenoTree.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Cluster.Genes.Phenotype](#/Cluster.Genes.Phenotype)||
|[/Parts.COGs](#/Parts.COGs)||
|[/venn.Matrix](#/venn.Matrix)||

## CLI API list
--------------------------
<h3 id="/Cluster.Genes.Phenotype"> 1. /Cluster.Genes.Phenotype</h3>


**Prototype**: ``PhenoTree.CLI::Int32 GenePhenoClusters(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
PhenoTree /Cluster.Genes.Phenotype /sites <motifSites.csv> [/out <out.tree_cluster.csv> /parallel]
```
<h3 id="/Parts.COGs"> 2. /Parts.COGs</h3>


**Prototype**: ``PhenoTree.CLI::Int32 PartitioningCOGs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
PhenoTree /Parts.COGs /cluster <btree.clusters.csv> /myva <COGs.csv> [/depth <-1> /out <EXPORT_DIR>]
```
<h3 id="/venn.Matrix"> 3. /venn.Matrix</h3>


**Prototype**: ``PhenoTree.CLI::Int32 VennMatrix(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
PhenoTree /venn.Matrix /besthits <besthits.xml.DIR> [/query <sp.name> /limits -1 /out <out.txt>]
```
