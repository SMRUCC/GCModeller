---
title: metaProfiler
tags: [maunal, tools]
date: 5/28/2018 9:30:25 PM
---
# GCModeller [version 1.0.0.0]
> 

<!--more-->

**Metagenome/Microbiome assembly analysis CLI tool**<br/>
_Metagenome/Microbiome assembly analysis CLI tool_<br/>
Copyright © SMRUCC 2017

**Module AssemblyName**: metaProfiler<br/>
**Root namespace**: ``metaProfiler.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/box.plot](#/box.plot)||
|[/heatmap.plot](#/heatmap.plot)||
|[/LefSe.Matrix](#/LefSe.Matrix)|Processing the relative aboundance matrix to the input format file as it describ: http://huttenhower.sph.harvard.edu/galaxy/root?tool_id=lefse_upload|
|[/OTU.cluster](#/OTU.cluster)||
|[/Relative_abundance.barplot](#/Relative_abundance.barplot)||
|[/Relative_abundance.stacked.barplot](#/Relative_abundance.stacked.barplot)||
|[/significant.difference](#/significant.difference)||
|[/SILVA.bacteria](#/SILVA.bacteria)||
|[/UPGMA.Tree](#/UPGMA.Tree)||


##### 1. 02. Alpha diversity analysis tools


|Function API|Info|
|------------|----|
|[/Rank_Abundance](#/Rank_Abundance)|https://en.wikipedia.org/wiki/Rank_abundance_curve|


##### 2. 03. Human Microbiome Project cli tool


|Function API|Info|
|------------|----|
|[/handle.hmp.manifest](#/handle.hmp.manifest)||
|[/hmp.manifest.files](#/hmp.manifest.files)||


##### 3. Microbiome antibiotic resistance composition analysis tools


|Function API|Info|
|------------|----|
|[/ARO.fasta.header.table](#/ARO.fasta.header.table)||


##### 4. Microbiome network cli tools


|Function API|Info|
|------------|----|
|[/Metagenome.UniProt.Ref](#/Metagenome.UniProt.Ref)||
|[/microbiome.metabolic.network](#/microbiome.metabolic.network)||
|[/microbiome.pathway.profile](#/microbiome.pathway.profile)|Generates the pathway network profile for the microbiome OTU result based on the KEGG and UniProt reference.|
|[/microbiome.pathway.run.profile](#/microbiome.pathway.run.profile)|Build pathway interaction network based on the microbiome profile result.|
|[/UniProt.screen.model](#/UniProt.screen.model)||


##### 5. SILVA database cli tools


|Function API|Info|
|------------|----|
|[/SILVA.headers](#/SILVA.headers)||


##### 6. Taxonomy assign cli tools


|Function API|Info|
|------------|----|
|[/gast.Taxonomy.greengenes](#/gast.Taxonomy.greengenes)|OTU taxonomy assign by apply gast method on the result of OTU rep sequence alignment against the greengenes.|

## CLI API list
--------------------------
<h3 id="/ARO.fasta.header.table"> 1. /ARO.fasta.header.table</h3>



**Prototype**: ``metaProfiler.CLI::Int32 AROSeqTable(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /ARO.fasta.header.table /in <directory> [/out <out.csv>]
```
<h3 id="/box.plot"> 2. /box.plot</h3>



**Prototype**: ``metaProfiler.CLI::Int32 Boxplot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /box.plot /in <data.csv> /groups <sampleInfo.csv> [/out <out.DIR>]
```
<h3 id="/gast.Taxonomy.greengenes"> 3. /gast.Taxonomy.greengenes</h3>

OTU taxonomy assign by apply gast method on the result of OTU rep sequence alignment against the greengenes.

**Prototype**: ``metaProfiler.CLI::Int32 gastTaxonomy_greengenes(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /gast.Taxonomy.greengenes /in <blastn.txt> /query <OTU.rep.fasta> /taxonomy <97_otu_taxonomy.txt> [/removes.lt <default=0.0001> /gast.consensus /min.pct <default=0.6> /out <gastOut.csv>]
```


#### Arguments
##### [/removes.lt]
OTU contains members number less than the percentage value of this argument value(low abundance) will be removes from the result.

###### Example
```bash
/removes.lt <float>
```
##### [/min.pct]
The required minium vote percentage of the taxonomy assigned from a OTU reference alignment by using gast method, default is required level 60% agreement.

###### Example
```bash
/min.pct <float>
```
<h3 id="/handle.hmp.manifest"> 4. /handle.hmp.manifest</h3>



**Prototype**: ``metaProfiler.CLI::Int32 Download16sSeq(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /handle.hmp.manifest /in <manifest.tsv> [/out <save.directory>]
```
<h3 id="/heatmap.plot"> 5. /heatmap.plot</h3>



**Prototype**: ``metaProfiler.CLI::Int32 HeatmapPlot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /heatmap.plot /in <data.csv> /groups <sampleInfo.csv> [/schema <default=YlGnBu:c9> /tsv /group /title <title> /size <2700,3000> /out <out.DIR>]
```
<h3 id="/hmp.manifest.files"> 6. /hmp.manifest.files</h3>



**Prototype**: ``metaProfiler.CLI::Int32 ExportFileList(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /hmp.manifest.files /in <manifest.tsv> [/out <list.txt>]
```
<h3 id="/LefSe.Matrix"> 7. /LefSe.Matrix</h3>

Processing the relative aboundance matrix to the input format file as it describ: http://huttenhower.sph.harvard.edu/galaxy/root?tool_id=lefse_upload

**Prototype**: ``metaProfiler.CLI::Int32 LefSeMatrix(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /LefSe.Matrix /in <Species_abundance.csv> /ncbi_taxonomy <NCBI_taxonomy> [/all_rank /out <out.tsv>]
```
<h3 id="/Metagenome.UniProt.Ref"> 8. /Metagenome.UniProt.Ref</h3>



**Prototype**: ``metaProfiler.CLI::Int32 BuildUniProtReference(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /Metagenome.UniProt.Ref /in <uniprot.ultralarge.xml/cache.directory> [/cache /out <out.XML>]
```
<h3 id="/microbiome.metabolic.network"> 9. /microbiome.metabolic.network</h3>



**Prototype**: ``metaProfiler.CLI::Int32 MetabolicComplementationNetwork(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /microbiome.metabolic.network /metagenome <list.txt/OTU.tab> /ref <reaction.repository.XML> /uniprot <repository.XML> [/out <network.directory>]
```
<h3 id="/microbiome.pathway.profile"> 10. /microbiome.pathway.profile</h3>

Generates the pathway network profile for the microbiome OTU result based on the KEGG and UniProt reference.

**Prototype**: ``metaProfiler.CLI::Int32 PathwayProfiles(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /microbiome.pathway.profile /in <gastout.csv> /ref <UniProt.ref.XML> /maps <kegg.maps.ref.XML> [/just.profiles /rank <default=family> /p.value <default=0.05> /out <out.directory>]
```
<h3 id="/microbiome.pathway.run.profile"> 11. /microbiome.pathway.run.profile</h3>

Build pathway interaction network based on the microbiome profile result.

**Prototype**: ``metaProfiler.CLI::Int32 RunProfile(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /microbiome.pathway.run.profile /in <profile.csv> /maps <kegg.maps.ref.Xml> [/p.value <default=0.05> /out <out.directory>]
```


#### Arguments
##### [/p.value]
The pvalue cutoff of the profile mapID, selects as the network node if the mapID its pvalue is smaller than this cutoff value. By default is 0.05. If no cutoff, please set this value to 1.

###### Example
```bash
/p.value <float>
```
<h3 id="/OTU.cluster"> 12. /OTU.cluster</h3>



**Prototype**: ``metaProfiler.CLI::Int32 ClusterOTU(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /OTU.cluster /left <left.fq> /right <right.fq> /silva <silva.bacteria.fasta> [/out <out.directory> /processors <default=2> /@set mothur=path]
```
<h3 id="/Rank_Abundance"> 13. /Rank_Abundance</h3>

https://en.wikipedia.org/wiki/Rank_abundance_curve

**Prototype**: ``metaProfiler.CLI::Int32 Rank_Abundance(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /Rank_Abundance /in <OTU.table.csv> [/schema <color schema, default=Rainbow> /out <out.DIR>]
```
<h3 id="/Relative_abundance.barplot"> 14. /Relative_abundance.barplot</h3>



**Prototype**: ``metaProfiler.CLI::Int32 Relative_abundance_barplot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /Relative_abundance.barplot /in <dataset.csv> [/group <sample_group.csv> /desc /asc /take <-1> /size <3000,2700> /column.n <default=9> /interval <10px> /out <out.png>]
```


#### Arguments
##### [/take]


###### Example
```bash
/take <int32>
```
##### [/desc]


###### Example
```bash
/desc
#(boolean flag does not require of argument value)
```
##### [/asc]


###### Example
```bash
/asc
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /take
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.int32(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Int32</a>_

Example: 
```json
0
```

<h3 id="/Relative_abundance.stacked.barplot"> 15. /Relative_abundance.stacked.barplot</h3>



**Prototype**: ``metaProfiler.CLI::Int32 Relative_abundance_stackedbarplot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /Relative_abundance.stacked.barplot /in <dataset.csv> [/group <sample_group.csv> /out <out.png>]
```
<h3 id="/significant.difference"> 16. /significant.difference</h3>



**Prototype**: ``metaProfiler.CLI::Int32 SignificantDifference(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /significant.difference /in <data.csv> /groups <sampleInfo.csv> [/out <out.csv.DIR>]
```


#### Arguments
##### /in
A matrix file that contains the sample data.

###### Example
```bash
/in <file, *.csv>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### /groups
Grouping info of the samples.

###### Example
```bash
/groups <file, *.csv>
```
##### Accepted Types
###### /in
**Decalre**:  _Microsoft.VisualBasic.Data.csv.IO.DataSet_

Example: 
```json
{
    "Properties": {
        "System.String": 0
    },
    "ID": "System.String"
}
```

###### /groups
**Decalre**:  _SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo_

Example: 
```json
{
    "sample_group": "System.String",
    "sample_name": "System.String",
    "ID": "System.String",
    "Order": 0,
    "color": "System.String",
    "shapetype": "System.String"
}
```

<h3 id="/SILVA.bacteria"> 17. /SILVA.bacteria</h3>



**Prototype**: ``metaProfiler.CLI::Int32 SILVABacterial(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /SILVA.bacteria /in <silva.fasta> [/out <silva.bacteria.fasta>]
```
<h3 id="/SILVA.headers"> 18. /SILVA.headers</h3>



**Prototype**: ``metaProfiler.CLI::Int32 SILVA_headers(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /SILVA.headers /in <silva.fasta> /out <headers.tsv>
```
<h3 id="/UniProt.screen.model"> 19. /UniProt.screen.model</h3>



**Prototype**: ``metaProfiler.CLI::Int32 ScreenModels(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /UniProt.screen.model /in <model.Xml> [/coverage <default=0.6> /terms <default=1000> /out <subset.xml>]
```


#### Arguments
##### /in
The metagenome network UniProt reference database that build from ``/Metagenome.UniProt.Ref`` command.

###### Example
```bash
/in <term_string>
```
<h3 id="/UPGMA.Tree"> 20. /UPGMA.Tree</h3>



**Prototype**: ``metaProfiler.CLI::Int32 UPGMATree(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
metaProfiler /UPGMA.Tree /in <in.csv> [/out <>]
```


#### Arguments
##### /in
The input matrix in csv table format for build and visualize as a UPGMA Tree.

###### Example
```bash
/in <file/directory>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### Accepted Types
###### /in
**Decalre**:  _Microsoft.VisualBasic.Data.csv.IO.DataSet_

Example: 
```json
{
    "Properties": {
        "System.String": 0
    },
    "ID": "System.String"
}
```

