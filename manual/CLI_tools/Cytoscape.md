---
title: Cytoscape
tags: [maunal, tools]
date: 11/24/2016 2:54:04 AM
---
# GCModeller [version 1.0.0.0]
> Cytoscape model generator and visualization tools utils for GCModeller

<!--more-->

**Cytoscape model generator and visualization tools utils**<br/>
_Cytoscape model generator and visualization tools utils_<br/>
Copyright © GPL3 2015

**Module AssemblyName**: file:///G:/GCModeller/GCModeller/bin/Cytoscape.exe<br/>
**Root namespace**: ``xCytoscape.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/associate](#/associate)||
|[/Build.Tree.NET](#/Build.Tree.NET)||
|[/Build.Tree.NET.COGs](#/Build.Tree.NET.COGs)||
|[/Build.Tree.NET.DEGs](#/Build.Tree.NET.DEGs)||
|[/Build.Tree.NET.KEGG_Modules](#/Build.Tree.NET.KEGG_Modules)||
|[/Build.Tree.NET.KEGG_Pathways](#/Build.Tree.NET.KEGG_Pathways)||
|[/Build.Tree.NET.Merged_Regulons](#/Build.Tree.NET.Merged_Regulons)||
|[/Build.Tree.NET.TF](#/Build.Tree.NET.TF)||
|[/Motif.Cluster](#/Motif.Cluster)||
|[/Motif.Cluster.Fast](#/Motif.Cluster.Fast)||
|[/Motif.Cluster.Fast.Sites](#/Motif.Cluster.Fast.Sites)||
|[/Motif.Cluster.MAT](#/Motif.Cluster.MAT)||
|[/replace](#/replace)||
|[/Tree.Cluster](#/Tree.Cluster)|This method is not recommended.|
|[/Tree.Cluster.rFBA](#/Tree.Cluster.rFBA)||
|[-Draw](#-Draw)|Drawing a network image visualization based on the generate network layout from the officials cytoscape software.|


##### 1. KEGG phenotype network analysis tools

Associates the KEGG pathway category information with the gene annotations.


|Function API|Info|
|------------|----|
|[/modNET.Simple](#/modNET.Simple)||
|[/net.model](#/net.model)||
|[/net.pathway](#/net.pathway)||
|[/Phenotypes.KEGG](#/Phenotypes.KEGG)|Regulator phenotype relationship cluster from virtual footprints.|


##### 2. Metagenomics tools


|Function API|Info|
|------------|----|
|[/BBH.Simple](#/BBH.Simple)||
|[/bbh.Trim.Indeitites](#/bbh.Trim.Indeitites)||
|[/BLAST.Network](#/BLAST.Network)||
|[/BLAST.Network.MetaBuild](#/BLAST.Network.MetaBuild)||
|[/MAT2NET](#/MAT2NET)||


##### 3. KEGG tools


|Function API|Info|
|------------|----|
|[/KEGG.Mods.NET](#/KEGG.Mods.NET)||
|[/reaction.NET](#/reaction.NET)||
|[--mod.regulations](#--mod.regulations)||


##### 4. MetaCyc pathway network tools


|Function API|Info|
|------------|----|
|[/Net.rFBA](#/Net.rFBA)||


##### 5. TF/Regulon network tools


|Function API|Info|
|------------|----|
|[/NetModel.TF_regulates](#/NetModel.TF_regulates)|Builds the regulation network between the TF.|
|[--graph.regulates](#--graph.regulates)||


##### 6. Bacterial TCS network tools


|Function API|Info|
|------------|----|
|[--TCS](#--TCS)||

## CLI API list
--------------------------
<h3 id="/associate"> 1. /associate</h3>


**Prototype**: ``xCytoscape.CLI::Int32 Assciates(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /associate /in <net.csv> /nodes <nodes.csv> [/out <out.net.DIR>]
```
<h3 id="/BBH.Simple"> 2. /BBH.Simple</h3>


**Prototype**: ``xCytoscape.CLI::Int32 SimpleBBH(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /BBH.Simple /in <sbh.csv> [/evalue <evalue: 1e-5> /out <out.bbh.csv>]
```
<h3 id="/bbh.Trim.Indeitites"> 3. /bbh.Trim.Indeitites</h3>


**Prototype**: ``xCytoscape.CLI::Int32 BBHTrimIdentities(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /bbh.Trim.Indeitites /in <bbh.csv> [/identities <0.3> /out <out.csv>]
```
<h3 id="/BLAST.Network"> 4. /BLAST.Network</h3>


**Prototype**: ``xCytoscape.CLI::Int32 GenerateBlastNetwork(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /BLAST.Network /in <inFile> [/out <outDIR> /type <default:blast_out; values: blast_out, sbh, bbh> /dict <dict.xml>]
```
<h3 id="/BLAST.Network.MetaBuild"> 5. /BLAST.Network.MetaBuild</h3>


**Prototype**: ``xCytoscape.CLI::Int32 MetaBuildBLAST(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /BLAST.Network.MetaBuild /in <inDIR> [/out <outDIR> /dict <dict.xml>]
```
<h3 id="/Build.Tree.NET"> 6. /Build.Tree.NET</h3>


**Prototype**: ``xCytoscape.CLI::Int32 BuildTreeNET(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Build.Tree.NET /in <cluster.csv> [/out <outDIR> /brief /FamilyInfo <regulons.DIR>]
```
<h3 id="/Build.Tree.NET.COGs"> 7. /Build.Tree.NET.COGs</h3>


**Prototype**: ``xCytoscape.CLI::Int32 BuildTreeNETCOGs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Build.Tree.NET.COGs /cluster <cluster.csv> /COGs <myvacog.csv> [/out <outDIR>]
```
<h3 id="/Build.Tree.NET.DEGs"> 8. /Build.Tree.NET.DEGs</h3>


**Prototype**: ``xCytoscape.CLI::Int32 BuildTreeNET_DEGs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Build.Tree.NET.DEGs /in <cluster.csv> /up <locus.txt> /down <locus.txt> [/out <outDIR> /brief]
```
<h3 id="/Build.Tree.NET.KEGG_Modules"> 9. /Build.Tree.NET.KEGG_Modules</h3>


**Prototype**: ``xCytoscape.CLI::Int32 BuildTreeNET_KEGGModules(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Build.Tree.NET.KEGG_Modules /in <cluster.csv> /mods <modules.XML.DIR> [/out <outDIR> /brief /trim]
```
<h3 id="/Build.Tree.NET.KEGG_Pathways"> 10. /Build.Tree.NET.KEGG_Pathways</h3>


**Prototype**: ``xCytoscape.CLI::Int32 BuildTreeNET_KEGGPathways(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Build.Tree.NET.KEGG_Pathways /in <cluster.csv> /mods <pathways.XML.DIR> [/out <outDIR> /brief /trim]
```
<h3 id="/Build.Tree.NET.Merged_Regulons"> 11. /Build.Tree.NET.Merged_Regulons</h3>


**Prototype**: ``xCytoscape.CLI::Int32 BuildTreeNET_MergeRegulons(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Build.Tree.NET.Merged_Regulons /in <cluster.csv> /family <family_Hits.Csv> [/out <outDIR> /brief]
```
<h3 id="/Build.Tree.NET.TF"> 12. /Build.Tree.NET.TF</h3>


**Prototype**: ``xCytoscape.CLI::Int32 BuildTreeNetTF(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Build.Tree.NET.TF /in <cluster.csv> /maps <TF.Regprecise.maps.Csv> /map <keyvaluepair.xml> /mods <kegg_modules.DIR> [/out <outDIR> /brief /cuts 0.8]
```
<h3 id="/KEGG.Mods.NET"> 13. /KEGG.Mods.NET</h3>


**Prototype**: ``xCytoscape.CLI::Int32 ModsNET(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /KEGG.Mods.NET /in <mods.xml.DIR> [/out <outDIR> /pathway /footprints <footprints.Csv> /brief /cut 0 /pcc 0]
```


#### Arguments
##### [/brief]
If this parameter is represented, then the program just outs the modules, all of the non-pathway genes wil be removes.

###### Example
```bash
/brief <term_string>
```
<h3 id="/MAT2NET"> 14. /MAT2NET</h3>


**Prototype**: ``xCytoscape.CLI::Int32 MatrixToNetwork(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /MAT2NET /in <mat.csv> [/out <net.csv> /cutoff 0]
```
<h3 id="/modNET.Simple"> 15. /modNET.Simple</h3>


**Prototype**: ``xCytoscape.CLI::Int32 SimpleModesNET(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /modNET.Simple /in <mods/pathway_DIR> [/out <outDIR> /pathway]
```
<h3 id="/Motif.Cluster"> 16. /Motif.Cluster</h3>


**Prototype**: ``xCytoscape.CLI::Int32 MotifCluster(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Motif.Cluster /query <meme.txt/MEME_OUT.DIR> /LDM <LDM-name/xml.path> [/clusters <3> /out <outCsv>]
```


#### Arguments
##### [/clusters]
If the expects clusters number is greater than the maps number, then the maps number divid 2 is used.

###### Example
```bash
/clusters <term_string>
```
<h3 id="/Motif.Cluster.Fast"> 17. /Motif.Cluster.Fast</h3>


**Prototype**: ``xCytoscape.CLI::Int32 FastCluster(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Motif.Cluster.Fast /query <meme_OUT.DIR> [/LDM <ldm-DIR> /out <outDIR> /map <gb.gbk> /maxw -1 /ldm_loads]
```


#### Arguments
##### [/maxw]
If this parameter value is not set, then no motif in the query will be filterd, or all of the width greater then the width value will be removed.
If a filterd is necessary, value of 52 nt is recommended as the max width of the motif in the RegPrecise database is 52.

###### Example
```bash
/maxw <term_string>
```
<h3 id="/Motif.Cluster.Fast.Sites"> 18. /Motif.Cluster.Fast.Sites</h3>


**Prototype**: ``xCytoscape.CLI::Int32 MotifClusterSites(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Motif.Cluster.Fast.Sites /in <meme.txt.DIR> [/out <outDIR> /LDM <ldm-DIR>]
```
<h3 id="/Motif.Cluster.MAT"> 19. /Motif.Cluster.MAT</h3>


**Prototype**: ``xCytoscape.CLI::Int32 ClusterMatrix(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Motif.Cluster.MAT /query <meme_OUT.DIR> [/LDM <ldm-DIR> /clusters 5 /out <outDIR>]
```
<h3 id="/net.model"> 20. /net.model</h3>


**Prototype**: ``xCytoscape.CLI::Int32 BuildModelNet(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /net.model /model <kegg.xmlModel.xml> [/out <outDIR> /not-trim]
```
<h3 id="/net.pathway"> 21. /net.pathway</h3>


**Prototype**: ``xCytoscape.CLI::Int32 PathwayNet(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /net.pathway /model <kegg.pathway.xml> [/out <outDIR> /trim]
```
<h3 id="/Net.rFBA"> 22. /Net.rFBA</h3>


**Prototype**: ``xCytoscape.CLI::Int32 net_rFBA(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Net.rFBA /in <metacyc.sbml> /fba.out <flux.Csv> [/out <outDIR>]
```
<h3 id="/NetModel.TF_regulates"> 23. /NetModel.TF_regulates</h3>

Builds the regulation network between the TF.
**Prototype**: ``xCytoscape.CLI::Int32 TFNet(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /NetModel.TF_regulates /in <footprints.csv> [/out <outDIR> /cut 0.45]
```
<h3 id="/Phenotypes.KEGG"> 24. /Phenotypes.KEGG</h3>

Regulator phenotype relationship cluster from virtual footprints.
**Prototype**: ``xCytoscape.CLI::Int32 KEGGModulesPhenotypeRegulates(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Phenotypes.KEGG /mods <KEGG_Modules/Pathways.DIR> /in <VirtualFootprints.csv> [/pathway /out <outCluster.csv>]
```
<h3 id="/reaction.NET"> 25. /reaction.NET</h3>


**Prototype**: ``xCytoscape.CLI::Int32 ReactionNET(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /reaction.NET [/model <xmlModel.xml> /source <rxn.DIR> /out <outDIR>]
```
<h3 id="/replace"> 26. /replace</h3>


**Prototype**: ``xCytoscape.CLI::Int32 replaceName(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /replace /in <net.csv> /nodes <nodes.Csv> /out <out.Csv>
```
<h3 id="/Tree.Cluster"> 27. /Tree.Cluster</h3>

This method is not recommended.
**Prototype**: ``xCytoscape.CLI::Int32 TreeCluster(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Tree.Cluster /in <in.MAT.csv> [/out <out.cluster.csv> /Locus.Map <Name>]
```
<h3 id="/Tree.Cluster.rFBA"> 28. /Tree.Cluster.rFBA</h3>


**Prototype**: ``xCytoscape.CLI::Int32 rFBATreeCluster(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape /Tree.Cluster.rFBA /in <in.flux.pheno_OUT.Csv> [/out <out.cluster.csv>]
```
<h3 id="-Draw"> 29. -Draw</h3>

Drawing a network image visualization based on the generate network layout from the officials cytoscape software.
**Prototype**: ``xCytoscape.CLI::Int32 DrawingInvoke(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape -draw /network <net_file> /parser <xgmml/cyjs> [-size <width,height> -out <out_image> /style <style_file> /style_parser <vizmap/json>]
```
<h3 id="--graph.regulates"> 30. --graph.regulates</h3>


**Prototype**: ``xCytoscape.CLI::Int32 SimpleRegulation(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape --graph.regulates /footprint <footprints.csv> [/trim]
```
<h3 id="--mod.regulations"> 31. --mod.regulations</h3>


**Prototype**: ``xCytoscape.CLI::Int32 ModuleRegulations(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape --mod.regulations /model <KEGG.xml> /footprints <footprints.csv> /out <outDIR> [/pathway /class /type]
```


#### Arguments
##### [/class]
This parameter can not be co-exists with ``/type`` parameter

###### Example
```bash
/class <term_string>
```
##### [/type]
This parameter can not be co-exists with ``/class`` parameter

###### Example
```bash
/type <term_string>
```
<h3 id="--TCS"> 32. --TCS</h3>


**Prototype**: ``xCytoscape.CLI::Int32 TCS(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
Cytoscape --TCS /in <TCS.csv.DIR> /regulations <TCS.virtualfootprints> /out <outForCytoscape.xml> [/Fill-pcc]
```


#### Arguments
##### [/Fill-pcc]
If the predicted regulation data did'nt contains pcc correlation value, then you can using this parameter to fill default value 0.6 or just left it default as ZERO

###### Example
```bash
/Fill-pcc <term_string>
```
