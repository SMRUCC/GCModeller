---
title: RNA-seq
tags: [maunal, tools]
date: 7/7/2016 6:51:55 PM
---
# GCModeller [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/RNA-seq.exe
**Root namespace**: RNA_seq.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/Clustering||
|/Data.Frame|Generates the data input for the DESeq2 R package.|
|/DataFrame.RPKMs|Merges the RPKM csv data files.|
|/DEGs||
|/DEGs.UpDown||
|/DESeq2||
|/DOOR.Corrects||
|/fq2fa||
|/Group.n||
|/HT-seq|Count raw reads for DESeq2 analysis.|
|/log2.selects||
|/PCC||
|/RPKM||
|/RPKM.Log2||
|/sid.map||
|/SPCC||
|/Stat.Changes||
|/WGCNA|Generates the cytoscape network model from WGCNA analysis.|

## Commands
--------------------------
##### Help for command '/Clustering':

**Prototype**: RNA_seq.CLI::Int32 Clustering(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /Clustering /in <fq> /kmax <int> [/out <out.Csv>]
  Example:      RNA-seq /Clustering 
```

##### Help for command '/Data.Frame':

**Prototype**: RNA_seq.CLI::Int32 Df(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Generates the data input for the DESeq2 R package.
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /Data.Frame /in <in.DIR> /ptt <genome.ptt> [/out out.csv]
  Example:      RNA-seq /Data.Frame 
```



  Parameters information:
```
    /in
    Description:  A directory location which it contains the Ht-Seq raw count text files.

    Example:      /in ""


```

#### Accepted Types
##### /in
##### Help for command '/DataFrame.RPKMs':

**Prototype**: RNA_seq.CLI::Int32 MergeRPKMs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Merges the RPKM csv data files.
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /DataFrame.RPKMs /in <in.DIR> [/trim /out <out.csv>]
  Example:      RNA-seq /DataFrame.RPKMs 
```

##### Help for command '/DEGs':

**Prototype**: RNA_seq.CLI::Int32 DEGs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /DEGs /in <diff.csv> [/out <degs.csv> /log_fold 2]
  Example:      RNA-seq /DEGs 
```

##### Help for command '/DEGs.UpDown':

**Prototype**: RNA_seq.CLI::Int32 DEGsUpDown(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /DEGs.UpDown /in <diff.csv> /sample.table <sampleTable.Csv> [/out <outDIR>]
  Example:      RNA-seq /DEGs.UpDown 
```

##### Help for command '/DESeq2':

**Prototype**: RNA_seq.CLI::Int32 DESeq2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /DESeq2 /sample.table <sampleTable.csv> /raw <raw.txt.DIR> /ptt <genome.ptt> [/design <design, default: ~condition>]
  Example:      RNA-seq /DESeq2 
```

##### Help for command '/DOOR.Corrects':

**Prototype**: RNA_seq.CLI::Int32 DOORCorrects(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /DOOR.Corrects /DOOR <genome.opr> /pcc <pcc.dat> [/out <out.opr> /pcc-cut <0.45>]
  Example:      RNA-seq /DOOR.Corrects 
```

##### Help for command '/fq2fa':

**Prototype**: RNA_seq.CLI::Int32 Fq2fa(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /fq2fa /in <fastaq> [/out <fasta>]
  Example:      RNA-seq /fq2fa 
```

##### Help for command '/Group.n':

**Prototype**: RNA_seq.CLI::Int32 GroupN(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /Group.n /in <dataset.csv> [/locus_map <locus> /out <out.csv>]
  Example:      RNA-seq /Group.n 
```

##### Help for command '/HT-seq':

**Prototype**: RNA_seq.CLI::Int32 HTSeqCount(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Count raw reads for DESeq2 analysis.
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /Ht-seq /in <in.sam> /gff <genome.gff> [/out <out.txt> /mode <union, intersection_strict, intersection_nonempty; default:intersection_nonempty> /rpkm /feature <CDS>]
  Example:      RNA-seq /HT-seq 
```



  Parameters information:
```
       [/Mode]
    Description:  The value of this parameter specific the counter of the function will be used, the available counter values are: union, intersection_strict and intersection_nonempty

    Example:      /Mode ""

   [/feature]
    Description:  [NOTE: value is case sensitive!!!] Value of the gff features can be one of the: tRNA, CDS, exon, gene, tmRNA, rRNA, region

    Example:      /feature ""


```

#### Accepted Types
##### /Mode
##### /feature
##### Help for command '/log2.selects':

**Prototype**: RNA_seq.CLI::Int32 Log2Selects(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /log2.selects /log2 <rpkm.log2.csv> /data <dataset.csv> [/locus_map <locus> /factor 1 /out <out.dataset.csv>]
  Example:      RNA-seq /log2.selects 
```

##### Help for command '/PCC':

**Prototype**: RNA_seq.CLI::Int32 PCC(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /PCC /expr <expr.matrix.csv> [/out <out.dat>]
  Example:      RNA-seq /PCC 
```

##### Help for command '/RPKM':

**Prototype**: RNA_seq.CLI::Int32 RPKM(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /RPKM /raw <raw_count.txt> /gff <genome.gff> [/out <expr.out.csv>]
  Example:      RNA-seq /RPKM 
```

##### Help for command '/RPKM.Log2':

**Prototype**: RNA_seq.CLI::Int32 Log2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /RPKM.Log2 /in <RPKM.csv> /cond <conditions> [/out <out.csv>]
  Example:      RNA-seq /RPKM.Log2 
```



  Parameters information:
```
    /cond
    Description:  Syntax format as:  <experiment1>/<experiment2>|<experiment3>/<experiment4>|.....

    Example:      /cond "colR1/xcb1|colR2/xcb2"


```

#### Accepted Types
##### /cond
##### Help for command '/sid.map':

**Prototype**: RNA_seq.CLI::Int32 sIdMapping(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /sid.map /gff <genome.gff> /raw <htseq-count.txt> [/out <out.txt>]
  Example:      RNA-seq /sid.map 
```

##### Help for command '/SPCC':

**Prototype**: RNA_seq.CLI::Int32 SPCC(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /SPCC /expr <expr.matrix.csv> [/out <out.dat>]
  Example:      RNA-seq /SPCC 
```

##### Help for command '/Stat.Changes':

**Prototype**: RNA_seq.CLI::Int32 StatChanges(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /Stat.Changes /deseq <deseq.result.csv> /sample <sampletable.csv> [/out <out.csv> /levels <1000> /diff <0.5>]
  Example:      RNA-seq /Stat.Changes 
```

##### Help for command '/WGCNA':

**Prototype**: RNA_seq.CLI::Int32 FromWGCNA(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Generates the cytoscape network model from WGCNA analysis.
  Usage:        G:\GCModeller\manual\bin\RNA-seq.exe /WGCNA /data <dataExpr.csv> /anno <annotations.csv> [/out <DIR.Out> /mods <color.list> /from.DESeq /id.map <GeneId>]
  Example:      RNA-seq /WGCNA 
```



  Parameters information:
```
    /data
    Description:  A sets of RNA-seq RPKM expression data sets, the first row in the csv table should be the experiments or conditions, and first column in the table should be the id of the genes and each cell in the table should be the RPKM expression value of each gene in each condition.
                   The data format of the table it would be like:
                   GeneId, condi1, cond12, condi3, ....
                   locus1, x, xx, x,
                   locus2, y, yy, yyy,
                   locus3, ,z, zz, zzz,
                   ......

    xyz Is the RPKM of the genes

    Example:      /data ""

/anno
    Description:  A table of the gene name annotation, the table should be in formats of
    Id, gene_symbol
    locus1, geneName
    locus2, geneName
    ....

    Example:      /anno ""

   [/mods]
    Description:  Each color in this parameter value is stands for a co expression module, and this parameter controls of the module output filtering, using | character as the seperator for each module color.

    Example:      /mods ""

   [/out]
    Description:  Export directory of the WGCNA data, if this parameter value is not presents in the arguments, then the current work directory will be used.

    Example:      /out ""

   [/From.Deseq]
    Description:  Is the /data matrix if comes from the DESeq analysis result output?
                   If is true, then the expression value will be extract from the original matrix file and save a new file named DESeq.dataExpr0.Csv in the out directory,
                   and last using this extracted data as the source of the WGCNA R script.

    Example:      /From.Deseq ""


```

#### Accepted Types
##### /data
##### /anno
##### /mods
##### /out
##### /From.Deseq
