---
title: RNA-seq
tags: [maunal, tools]
date: 2016/10/22 12:30:17
---
# GCModeller [version 1.0.0.0]
> 

<!--more-->

**RNA-seq data analysis**
__
Copyright ?  2016

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/RNA-seq.exe
**Root namespace**: ``RNA_seq.CLI``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Associate.GI](#/Associate.GI)||
|[/calcFst](#/calcFst)||
|[/chisq.test](#/chisq.test)||
|[/Clustering](#/Clustering)||
|[/Co.Vector](#/Co.Vector)||
|[/Contacts.NNN](#/Contacts.NNN)|Using for contacts the reference sequence for the metagenome analysis. reference sequence was contact in one sequence by a interval ``NNNNNNNNNNNNNNNNNN``|
|[/Contacts.Ref](#/Contacts.Ref)|This tools using for the furthering analysis when finish the first mapping.|
|[/Data.Frame](#/Data.Frame)|Generates the data input for the DESeq2 R package.|
|[/DataFrame.RPKMs](#/DataFrame.RPKMs)|Merges the RPKM csv data files.|
|[/DEGs](#/DEGs)||
|[/DEGs.UpDown](#/DEGs.UpDown)||
|[/DESeq2](#/DESeq2)||
|[/DOOR.Corrects](#/DOOR.Corrects)||
|[/Export.Megan.BIOM](#/Export.Megan.BIOM)||
|[/Export.SAM.Maps](#/Export.SAM.Maps)||
|[/Export.SAM.Maps.By_Samples](#/Export.SAM.Maps.By_Samples)||
|[/Export.SSU.Refs](#/Export.SSU.Refs)||
|[/Export.SSU.Refs.Batch](#/Export.SSU.Refs.Batch)||
|[/Format.GI](#/Format.GI)||
|[/fq2fa](#/fq2fa)||
|[/gast](#/gast)||
|[/gast.stat.names](#/gast.stat.names)||
|[/Genotype](#/Genotype)||
|[/Genotype.Statics](#/Genotype.Statics)||
|[/Group.n](#/Group.n)||
|[/HT-seq](#/HT-seq)|Count raw reads for DESeq2 analysis.|
|[/Imports.gast.Refs.NCBI_nt](#/Imports.gast.Refs.NCBI_nt)||
|[/log2.selects](#/log2.selects)||
|[/Merge.FastQ](#/Merge.FastQ)||
|[/PCC](#/PCC)||
|[/Rank.Statics](#/Rank.Statics)||
|[/RPKM](#/RPKM)||
|[/RPKM.Log2](#/RPKM.Log2)||
|[/Sampling.stats](#/Sampling.stats)||
|[/Select.Subs](#/Select.Subs)||
|[/sid.map](#/sid.map)||
|[/SPCC](#/SPCC)||
|[/Stat.Changes](#/Stat.Changes)||
|[/Statics.Labels](#/Statics.Labels)||
|[/WGCNA](#/WGCNA)|Generates the cytoscape network model from WGCNA analysis.|

## CLI API list
--------------------------
<h3 id="/Associate.GI"> 1. /Associate.GI</h3>


**Prototype**: ``RNA_seq.CLI::Int32 AssociateGI(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Associate.GI /in <list.Csv.DIR> /gi <nt.gi.csv> [/out <out.DIR>]
```
<h3 id="/calcFst"> 2. /calcFst</h3>


**Prototype**: ``RNA_seq.CLI::Int32 calcFst(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /calcFst /in <in.csv> [/out <out.Csv>]
```
<h3 id="/chisq.test"> 3. /chisq.test</h3>


**Prototype**: ``RNA_seq.CLI::Int32 chisqTest(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /chisq.test /in <in.DIR> [/out <out.DIR>]
```
<h3 id="/Clustering"> 4. /Clustering</h3>


**Prototype**: ``RNA_seq.CLI::Int32 Clustering(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Clustering /in <fq> /kmax <int> [/out <out.Csv>]
```
<h3 id="/Co.Vector"> 5. /Co.Vector</h3>


**Prototype**: ``RNA_seq.CLI::Int32 CorrelatesVector(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Co.Vector /in <co.Csv/DIR> [/min 0.01 /max 0.05 /out <out.csv>]
```
<h3 id="/Contacts.NNN"> 6. /Contacts.NNN</h3>

Using for contacts the reference sequence for the metagenome analysis. reference sequence was contact in one sequence by a interval ``NNNNNNNNNNNNNNNNNN``
**Prototype**: ``RNA_seq.CLI::Int32 ContactsNNN(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Contacts /in <in.fasta/DIR> [/out <out.DIR>]
```
<h3 id="/Contacts.Ref"> 7. /Contacts.Ref</h3>

This tools using for the furthering analysis when finish the first mapping.
**Prototype**: ``RNA_seq.CLI::Int32 ContactsRef(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Contacts.Ref /in <in.fasta> /maps <maps.sam> [/out <out.DIR>]
```
<h3 id="/Data.Frame"> 8. /Data.Frame</h3>

Generates the data input for the DESeq2 R package.
**Prototype**: ``RNA_seq.CLI::Int32 Df(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Data.Frame /in <in.DIR> /ptt <genome.ptt> [/out out.csv]
```


#### Arguments
##### /in
A directory location which it contains the Ht-Seq raw count text files.

###### Example
```bash
/in <term_string>
```
<h3 id="/DataFrame.RPKMs"> 9. /DataFrame.RPKMs</h3>

Merges the RPKM csv data files.
**Prototype**: ``RNA_seq.CLI::Int32 MergeRPKMs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /DataFrame.RPKMs /in <in.DIR> [/trim /out <out.csv>]
```
<h3 id="/DEGs"> 10. /DEGs</h3>


**Prototype**: ``RNA_seq.CLI::Int32 DEGs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /DEGs /in <diff.csv> [/out <degs.csv> /log_fold 2]
```
<h3 id="/DEGs.UpDown"> 11. /DEGs.UpDown</h3>


**Prototype**: ``RNA_seq.CLI::Int32 DEGsUpDown(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /DEGs.UpDown /in <diff.csv> /sample.table <sampleTable.Csv> [/out <outDIR>]
```
<h3 id="/DESeq2"> 12. /DESeq2</h3>


**Prototype**: ``RNA_seq.CLI::Int32 DESeq2(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /DESeq2 /sample.table <sampleTable.csv> /raw <raw.txt.DIR> /ptt <genome.ptt> [/design <design, default: ~condition>]
```
<h3 id="/DOOR.Corrects"> 13. /DOOR.Corrects</h3>


**Prototype**: ``RNA_seq.CLI::Int32 DOORCorrects(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /DOOR.Corrects /DOOR <genome.opr> /pcc <pcc.dat> [/out <out.opr> /pcc-cut <0.45>]
```
<h3 id="/Export.Megan.BIOM"> 14. /Export.Megan.BIOM</h3>


**Prototype**: ``RNA_seq.CLI::Int32 ExportToMegan(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Export.Megan.BIOM /in <relative.table.csv> [/out <out.json.biom>]
```
<h3 id="/Export.SAM.Maps"> 15. /Export.SAM.Maps</h3>


**Prototype**: ``RNA_seq.CLI::Int32 ExportSAMMaps(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Export.SAM.Maps /in <in.sam> [/large /contigs <NNNN.contig.Csv> /raw <ref.fasta> /out <out.Csv> /debug]
```


#### Arguments
##### [/raw]
When this command is processing the NNNNN contact data, just input the contigs csv file, this raw reference is not required for the contig information.

###### Example
```bash
/raw <term_string>
```
##### Accepted Types
###### /raw
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaToken_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

<h3 id="/Export.SAM.Maps.By_Samples"> 16. /Export.SAM.Maps.By_Samples</h3>


**Prototype**: ``RNA_seq.CLI::Int32 ExportSAMMapsBySamples(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Export.SAM.Maps.By_Samples /in <in.sam> /tag <sampleTag_regex> [/ref <ref.fasta> /out <out.Csv>]
```
<h3 id="/Export.SSU.Refs"> 17. /Export.SSU.Refs</h3>


**Prototype**: ``RNA_seq.CLI::Int32 ExportSSURefs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Export.SSU.Refs /in <ssu.fasta> [/out <out.DIR> /no-suffix]
```
<h3 id="/Export.SSU.Refs.Batch"> 18. /Export.SSU.Refs.Batch</h3>


**Prototype**: ``RNA_seq.CLI::Int32 ExportSSUBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Export.SSU.Refs /in <ssu.fasta.DIR> [/out <out.DIR>]
```
<h3 id="/Format.GI"> 19. /Format.GI</h3>


**Prototype**: ``RNA_seq.CLI::Int32 FormatGI(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Format.GI /in <txt> /gi <regex> /format <gi|{gi}> /out <out.txt>
```
<h3 id="/fq2fa"> 20. /fq2fa</h3>


**Prototype**: ``RNA_seq.CLI::Int32 Fq2fa(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /fq2fa /in <fastaq> [/out <fasta>]
```
<h3 id="/gast"> 21. /gast</h3>


**Prototype**: ``RNA_seq.CLI::Int32 gastInvoke(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq
```
<h3 id="/gast.stat.names"> 22. /gast.stat.names</h3>


**Prototype**: ``RNA_seq.CLI::Int32 StateNames(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /gast.stat.names /in <*.names> /gast <gast.out> [/out <out.Csv>]
```
<h3 id="/Genotype"> 23. /Genotype</h3>


**Prototype**: ``RNA_seq.CLI::Int32 Genotype(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Genotype /in <raw.csv> [/out <out.Csv>]
```
<h3 id="/Genotype.Statics"> 24. /Genotype.Statics</h3>


**Prototype**: ``RNA_seq.CLI::Int32 GenotypeStatics(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Genotype.Statics /in <in.DIR> [/out <EXPORT>]
```
<h3 id="/Group.n"> 25. /Group.n</h3>


**Prototype**: ``RNA_seq.CLI::Int32 GroupN(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Group.n /in <dataset.csv> [/locus_map <locus> /out <out.csv>]
```
<h3 id="/HT-seq"> 26. /HT-seq</h3>

Count raw reads for DESeq2 analysis.
**Prototype**: ``RNA_seq.CLI::Int32 HTSeqCount(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Ht-seq /in <in.sam> /gff <genome.gff> [/out <out.txt> /mode <union, intersection_strict, intersection_nonempty; default:intersection_nonempty> /rpkm /feature <CDS>]
```


#### Arguments
##### [/Mode]
The value of this parameter specific the counter of the function will be used, the available counter values are: union, intersection_strict and intersection_nonempty

###### Example
```bash
/Mode <term_string>
```
##### [/feature]
[NOTE: value is case sensitive!!!] Value of the gff features can be one of the: tRNA, CDS, exon, gene, tmRNA, rRNA, region

###### Example
```bash
/feature <term_string>
```
<h3 id="/Imports.gast.Refs.NCBI_nt"> 27. /Imports.gast.Refs.NCBI_nt</h3>


**Prototype**: ``RNA_seq.CLI::Int32 ImportsRefFromNt(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Imports.gast.Refs.NCBI_nt /in <in.nt> /gi2taxid <dmp/txt/bin> /taxonomy <nodes/names.dmp_DIR> [/out <out.fasta>]
```
<h3 id="/log2.selects"> 28. /log2.selects</h3>


**Prototype**: ``RNA_seq.CLI::Int32 Log2Selects(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /log2.selects /log2 <rpkm.log2.csv> /data <dataset.csv> [/locus_map <locus> /factor 1 /out <out.dataset.csv>]
```
<h3 id="/Merge.FastQ"> 29. /Merge.FastQ</h3>


**Prototype**: ``RNA_seq.CLI::Int32 MergeFastQ(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Merge.FastQ /in <in.DIR> [/out <out.fq>]
```
<h3 id="/PCC"> 30. /PCC</h3>


**Prototype**: ``RNA_seq.CLI::Int32 PCC(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /PCC /expr <expr.matrix.csv> [/out <out.dat>]
```
<h3 id="/Rank.Statics"> 31. /Rank.Statics</h3>


**Prototype**: ``RNA_seq.CLI::Int32 RankStatics(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Rank.Statics /in <relative.table.csv> [/out <EXPORT_DIR>]
```
<h3 id="/RPKM"> 32. /RPKM</h3>


**Prototype**: ``RNA_seq.CLI::Int32 RPKM(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /RPKM /raw <raw_count.txt> /gff <genome.gff> [/out <expr.out.csv>]
```
<h3 id="/RPKM.Log2"> 33. /RPKM.Log2</h3>


**Prototype**: ``RNA_seq.CLI::Int32 Log2(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /RPKM.Log2 /in <RPKM.csv> /cond <conditions> [/out <out.csv>]
```


#### Arguments
##### /cond
Syntax format as:  <experiment1>/<experiment2>|<experiment3>/<experiment4>|.....

###### Example
```bash
/cond colR1/xcb1|colR2/xcb2
```
<h3 id="/Sampling.stats"> 34. /Sampling.stats</h3>


**Prototype**: ``RNA_seq.CLI::Int32 SamplingStats(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Sampling.stats /in <expression.csv> /samples <stats.csv.DIR> [/out <out.csv>]
```
<h3 id="/Select.Subs"> 35. /Select.Subs</h3>


**Prototype**: ``RNA_seq.CLI::Int32 SelectSubs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Select.Subs /in <in.DIR> /cols <list','> [/out <out.DIR>]
```
<h3 id="/sid.map"> 36. /sid.map</h3>


**Prototype**: ``RNA_seq.CLI::Int32 sIdMapping(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /sid.map /gff <genome.gff> /raw <htseq-count.txt> [/out <out.txt>]
```
<h3 id="/SPCC"> 37. /SPCC</h3>


**Prototype**: ``RNA_seq.CLI::Int32 SPCC(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /SPCC /expr <expr.matrix.csv> [/out <out.dat>]
```
<h3 id="/Stat.Changes"> 38. /Stat.Changes</h3>


**Prototype**: ``RNA_seq.CLI::Int32 StatChanges(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Stat.Changes /deseq <deseq.result.csv> /sample <sampletable.csv> [/out <out.csv> /levels <1000> /diff <0.5>]
```
<h3 id="/Statics.Labels"> 39. /Statics.Labels</h3>


**Prototype**: ``RNA_seq.CLI::Int32 MergeLabels(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /Statics.Labels /in <in.csv> [/label <Label> /name <Name> /value <value> /out <out.csv>]
```
<h3 id="/WGCNA"> 40. /WGCNA</h3>

Generates the cytoscape network model from WGCNA analysis.
**Prototype**: ``RNA_seq.CLI::Int32 FromWGCNA(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RNA-seq /WGCNA /data <dataExpr.csv> /anno <annotations.csv> [/out <DIR.Out> /mods <color.list> /from.DESeq /id.map <GeneId>]
```


#### Arguments
##### /data
A sets of RNA-seq RPKM expression data sets, the first row in the csv table should be the experiments or conditions, and first column in the table should be the id of the genes and each cell in the table should be the RPKM expression value of each gene in each condition.
The data format of the table it would be like:
GeneId, condi1, cond12, condi3, ....
locus1, x, xx, x,
locus2, y, yy, yyy,
locus3, ,z, zz, zzz,
......

xyz Is the RPKM of the genes

###### Example
```bash
/data <term_string>
```
##### /anno
A table of the gene name annotation, the table should be in formats of
Id, gene_symbol
locus1, geneName
locus2, geneName
....

###### Example
```bash
/anno <term_string>
```
##### [/mods]
Each color in this parameter value is stands for a co expression module, and this parameter controls of the module output filtering, using | character as the seperator for each module color.

###### Example
```bash
/mods <term_string>
```
##### [/out]
Export directory of the WGCNA data, if this parameter value is not presents in the arguments, then the current work directory will be used.

###### Example
```bash
/out <term_string>
```
##### [/From.Deseq]
Is the /data matrix if comes from the DESeq analysis result output?
If is true, then the expression value will be extract from the original matrix file and save a new file named DESeq.dataExpr0.Csv in the out directory,
and last using this extracted data as the source of the WGCNA R script.

###### Example
```bash
/From.Deseq <term_string>
```
