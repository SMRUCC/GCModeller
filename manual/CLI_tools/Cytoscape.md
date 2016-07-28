---
title: Cytoscape
tags: [maunal, tools]
date: 7/27/2016 6:40:16 PM
---
# GCModeller [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/Cytoscape.exe
**Root namespace**: xCytoscape.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/BBH.Simple||
|/bbh.Trim.Indeitites||
|/BLAST.Network||
|/BLAST.Network.MetaBuild||
|/Build.Tree.NET||
|/Build.Tree.NET.COGs||
|/Build.Tree.NET.DEGs||
|/Build.Tree.NET.KEGG_Modules||
|/Build.Tree.NET.KEGG_Pathways||
|/Build.Tree.NET.Merged_Regulons||
|/Build.Tree.NET.TF||
|/KEGG.Mods.NET||
|/MAT2NET||
|/modNET.Simple||
|/Motif.Cluster||
|/Motif.Cluster.Fast||
|/Motif.Cluster.Fast.Sites||
|/Motif.Cluster.MAT||
|/net.model||
|/net.pathway||
|/Net.rFBA||
|/NetModel.TF_regulates|Builds the regulation network between the TF.|
|/Phenotypes.KEGG|Regulator phenotype relationship cluster from virtual footprints.|
|/reaction.NET||
|/Tree.Cluster|This method is not recommended.|
|/Tree.Cluster.rFBA||
|-Draw|Drawing a network image visualization based on the generate network layout from the officials cytoscape software.|
|--graph.regulates||
|--mod.regulations||
|--TCS||

## Commands
--------------------------
##### Help for command '/BBH.Simple':

**Prototype**: xCytoscape.CLI::Int32 SimpleBBH(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /BBH.Simple /in <sbh.csv> [/evalue <evalue: 1e-5> /out <out.bbh.csv>]
  Example:      Cytoscape /BBH.Simple 
```

##### Help for command '/bbh.Trim.Indeitites':

**Prototype**: xCytoscape.CLI::Int32 BBHTrimIdentities(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /bbh.Trim.Indeitites /in <bbh.csv> [/identities <0.3> /out <out.csv>]
  Example:      Cytoscape /bbh.Trim.Indeitites 
```

##### Help for command '/BLAST.Network':

**Prototype**: xCytoscape.CLI::Int32 GenerateBlastNetwork(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /BLAST.Network /in <inFile> [/out <outDIR> /type <default:blast_out; values: blast_out, sbh, bbh> /dict <dict.xml>]
  Example:      Cytoscape /BLAST.Network 
```

##### Help for command '/BLAST.Network.MetaBuild':

**Prototype**: xCytoscape.CLI::Int32 MetaBuildBLAST(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /BLAST.Network.MetaBuild /in <inDIR> [/out <outDIR> /dict <dict.xml>]
  Example:      Cytoscape /BLAST.Network.MetaBuild 
```

##### Help for command '/Build.Tree.NET':

**Prototype**: xCytoscape.CLI::Int32 BuildTreeNET(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Build.Tree.NET /in <cluster.csv> [/out <outDIR> /brief /FamilyInfo <regulons.DIR>]
  Example:      Cytoscape /Build.Tree.NET 
```

##### Help for command '/Build.Tree.NET.COGs':

**Prototype**: xCytoscape.CLI::Int32 BuildTreeNETCOGs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Build.Tree.NET.COGs /cluster <cluster.csv> /COGs <myvacog.csv> [/out <outDIR>]
  Example:      Cytoscape /Build.Tree.NET.COGs 
```

##### Help for command '/Build.Tree.NET.DEGs':

**Prototype**: xCytoscape.CLI::Int32 BuildTreeNET_DEGs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Build.Tree.NET.DEGs /in <cluster.csv> /up <locus.txt> /down <locus.txt> [/out <outDIR> /brief]
  Example:      Cytoscape /Build.Tree.NET.DEGs 
```

##### Help for command '/Build.Tree.NET.KEGG_Modules':

**Prototype**: xCytoscape.CLI::Int32 BuildTreeNET_KEGGModules(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Build.Tree.NET.KEGG_Modules /in <cluster.csv> /mods <modules.XML.DIR> [/out <outDIR> /brief /trim]
  Example:      Cytoscape /Build.Tree.NET.KEGG_Modules 
```

##### Help for command '/Build.Tree.NET.KEGG_Pathways':

**Prototype**: xCytoscape.CLI::Int32 BuildTreeNET_KEGGPathways(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Build.Tree.NET.KEGG_Pathways /in <cluster.csv> /mods <pathways.XML.DIR> [/out <outDIR> /brief /trim]
  Example:      Cytoscape /Build.Tree.NET.KEGG_Pathways 
```

##### Help for command '/Build.Tree.NET.Merged_Regulons':

**Prototype**: xCytoscape.CLI::Int32 BuildTreeNET_MergeRegulons(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Build.Tree.NET.Merged_Regulons /in <cluster.csv> /family <family_Hits.Csv> [/out <outDIR> /brief]
  Example:      Cytoscape /Build.Tree.NET.Merged_Regulons 
```

##### Help for command '/Build.Tree.NET.TF':

**Prototype**: xCytoscape.CLI::Int32 BuildTreeNetTF(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Build.Tree.NET.TF /in <cluster.csv> /maps <TF.Regprecise.maps.Csv> /map <keyvaluepair.xml> /mods <kegg_modules.DIR> [/out <outDIR> /brief /cuts 0.8]
  Example:      Cytoscape /Build.Tree.NET.TF 
```

##### Help for command '/KEGG.Mods.NET':

**Prototype**: xCytoscape.CLI::Int32 ModsNET(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /KEGG.Mods.NET /in <mods.xml.DIR> [/out <outDIR> /pathway /footprints <footprints.Csv> /brief /cut 0 /pcc 0]
  Example:      Cytoscape /KEGG.Mods.NET 
```



  Parameters information:
```
       [/brief]
    Description:  If this parameter is represented, then the program just outs the modules, all of the non-pathway genes wil be removes.

    Example:      /brief ""


```

#### Accepted Types
##### /brief
##### Help for command '/MAT2NET':

**Prototype**: xCytoscape.CLI::Int32 MatrixToNetwork(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /MAT2NET /in <mat.csv> [/out <net.csv> /cutoff 0]
  Example:      Cytoscape /MAT2NET 
```

##### Help for command '/modNET.Simple':

**Prototype**: xCytoscape.CLI::Int32 SimpleModesNET(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /modNET.Simple /in <mods/pathway_DIR> [/out <outDIR> /pathway]
  Example:      Cytoscape /modNET.Simple 
```

##### Help for command '/Motif.Cluster':

**Prototype**: xCytoscape.CLI::Int32 MotifCluster(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Motif.Cluster /query <meme.txt/MEME_OUT.DIR> /LDM <LDM-name/xml.path> [/clusters <3> /out <outCsv>]
  Example:      Cytoscape /Motif.Cluster 
```



  Parameters information:
```
       [/clusters]
    Description:  If the expects clusters number is greater than the maps number, then the maps number divid 2 is used.

    Example:      /clusters ""


```

#### Accepted Types
##### /clusters
##### Help for command '/Motif.Cluster.Fast':

**Prototype**: xCytoscape.CLI::Int32 FastCluster(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Motif.Cluster.Fast /query <meme_OUT.DIR> [/LDM <ldm-DIR> /out <outDIR> /map <gb.gbk> /maxw -1 /ldm_loads]
  Example:      Cytoscape /Motif.Cluster.Fast 
```



  Parameters information:
```
       [/maxw]
    Description:  If this parameter value is not set, then no motif in the query will be filterd, or all of the width greater then the width value will be removed.
                   If a filterd is necessary, value of 52 nt is recommended as the max width of the motif in the RegPrecise database is 52.

    Example:      /maxw ""


```

#### Accepted Types
##### /maxw
##### Help for command '/Motif.Cluster.Fast.Sites':

**Prototype**: xCytoscape.CLI::Int32 MotifClusterSites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Motif.Cluster.Fast.Sites /in <meme.txt.DIR> [/out <outDIR> /LDM <ldm-DIR>]
  Example:      Cytoscape /Motif.Cluster.Fast.Sites 
```

##### Help for command '/Motif.Cluster.MAT':

**Prototype**: xCytoscape.CLI::Int32 ClusterMatrix(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Motif.Cluster.MAT /query <meme_OUT.DIR> [/LDM <ldm-DIR> /clusters 5 /out <outDIR>]
  Example:      Cytoscape /Motif.Cluster.MAT 
```

##### Help for command '/net.model':

**Prototype**: xCytoscape.CLI::Int32 BuildModelNet(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /net.model /model <kegg.xmlModel.xml> [/out <outDIR> /not-trim]
  Example:      Cytoscape /net.model 
```

##### Help for command '/net.pathway':

**Prototype**: xCytoscape.CLI::Int32 PathwayNet(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /net.pathway /model <kegg.pathway.xml> [/out <outDIR> /trim]
  Example:      Cytoscape /net.pathway 
```

##### Help for command '/Net.rFBA':

**Prototype**: xCytoscape.CLI::Int32 net_rFBA(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Net.rFBA /in <metacyc.sbml> /fba.out <flux.Csv> [/out <outDIR>]
  Example:      Cytoscape /Net.rFBA 
```

##### Help for command '/NetModel.TF_regulates':

**Prototype**: xCytoscape.CLI::Int32 TFNet(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Builds the regulation network between the TF.
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /NetModel.TF_regulates /in <footprints.csv> [/out <outDIR> /cut 0.45]
  Example:      Cytoscape /NetModel.TF_regulates 
```

##### Help for command '/Phenotypes.KEGG':

**Prototype**: xCytoscape.CLI::Int32 KEGGModulesPhenotypeRegulates(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Regulator phenotype relationship cluster from virtual footprints.
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Phenotypes.KEGG /mods <KEGG_Modules/Pathways.DIR> /in <VirtualFootprints.csv> [/pathway /out <outCluster.csv>]
  Example:      Cytoscape /Phenotypes.KEGG 
```

##### Help for command '/reaction.NET':

**Prototype**: xCytoscape.CLI::Int32 ReactionNET(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /reaction.NET [/model <xmlModel.xml> /source <rxn.DIR> /out <outDIR>]
  Example:      Cytoscape /reaction.NET 
```

##### Help for command '/Tree.Cluster':

**Prototype**: xCytoscape.CLI::Int32 TreeCluster(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  This method is not recommended.
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Tree.Cluster /in <in.MAT.csv> [/out <out.cluster.csv> /Locus.Map <Name>]
  Example:      Cytoscape /Tree.Cluster 
```

##### Help for command '/Tree.Cluster.rFBA':

**Prototype**: xCytoscape.CLI::Int32 rFBATreeCluster(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe /Tree.Cluster.rFBA /in <in.flux.pheno_OUT.Csv> [/out <out.cluster.csv>]
  Example:      Cytoscape /Tree.Cluster.rFBA 
```

##### Help for command '-Draw':

**Prototype**: xCytoscape.CLI::Int32 DrawingInvoke(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Drawing a network image visualization based on the generate network layout from the officials cytoscape software.
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe -draw /network <net_file> /parser <xgmml/cyjs> [-size <width,height> -out <out_image> /style <style_file> /style_parser <vizmap/json>]
  Example:      Cytoscape -Draw 
```

##### Help for command '--graph.regulates':

**Prototype**: xCytoscape.CLI::Int32 SimpleRegulation(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe --graph.regulates /footprint <footprints.csv> [/trim]
  Example:      Cytoscape --graph.regulates 
```

##### Help for command '--mod.regulations':

**Prototype**: xCytoscape.CLI::Int32 ModuleRegulations(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe --mod.regulations /model <KEGG.xml> /footprints <footprints.csv> /out <outDIR> [/pathway /class /type]
  Example:      Cytoscape --mod.regulations 
```



  Parameters information:
```
       [/class]
    Description:  This parameter can not be co-exists with /type parameter

    Example:      /class ""

   [/type]
    Description:  This parameter can not be co-exists with /class parameter

    Example:      /type ""


```

#### Accepted Types
##### /class
##### /type
##### Help for command '--TCS':

**Prototype**: xCytoscape.CLI::Int32 TCS(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\Cytoscape.exe --TCS /in <TCS.csv.DIR> /regulations <TCS.virtualfootprints> /out <outForCytoscape.xml> [/Fill-pcc]
  Example:      Cytoscape --TCS 
```



  Parameters information:
```
       [/Fill-pcc]
    Description:  If the predicted regulation data did'nt contains pcc correlation value, then you can using this parameter to fill default value 0.6 or just left it default as ZERO

    Example:      /Fill-pcc ""


```

#### Accepted Types
##### /Fill-pcc
