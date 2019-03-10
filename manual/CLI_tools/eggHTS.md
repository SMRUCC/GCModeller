---
title: eggHTS
tags: [maunal, tools]
date: 5/28/2018 9:30:21 PM
---
# GCModeller [version 1.0.0.0]
> Custom KO classification set can be download from: http://www.kegg.jp/kegg-bin/get_htext?ko00001.keg. 
>               You can replace the %s mark using kegg organism code in url example as: http://www.kegg.jp/kegg-bin/download_htext?htext=%s00001&format=htext&filedir= for download the custom KO classification set.

<!--more-->

**eggHTS: Proteomics data analysis toolkit**<br/>
_eggHTS: Proteomics data analysis toolkit_<br/>
Copyright © I@xieguigang.me 2017

**Module AssemblyName**: eggHTS<br/>
**Root namespace**: ``eggHTS.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/COG.profiling.plot](#/COG.profiling.plot)|Plots the COGs category statics profiling of the target genome from the COG annotation file.|
|[/DEPs.takes.values](#/DEPs.takes.values)||
|[/DEPs.union](#/DEPs.union)||
|[/Exocarta.Hits](#/Exocarta.Hits)||
|[/Fasta.IDlist](#/Fasta.IDlist)||
|[/iBAQ.Cloud](#/iBAQ.Cloud)|Cloud plot of the iBAQ DEPs result.|
|[/KO.Catalogs](#/KO.Catalogs)|Display the barplot of the KEGG orthology match.|
|[/KOBAS.Sim.Heatmap](#/KOBAS.Sim.Heatmap)||
|[/KOBAS.similarity](#/KOBAS.similarity)||
|[/KOBAS.Term.Kmeans](#/KOBAS.Term.Kmeans)||
|[/labelFree.t.test](#/labelFree.t.test)||
|[/Network.PCC](#/Network.PCC)||
|[/paired.sample.designer](#/paired.sample.designer)||
|[/Perseus.MajorityProteinIDs](#/Perseus.MajorityProteinIDs)|Export the uniprot ID list from ``Majority Protein IDs`` row and generates a text file for batch search of the uniprot database.|
|[/Perseus.Table.annotations](#/Perseus.Table.annotations)||
|[/pfamstring.enrichment](#/pfamstring.enrichment)||
|[/protein.annotations.shotgun](#/protein.annotations.shotgun)||
|[/Samples.IDlist](#/Samples.IDlist)|Extracts the protein hits from the protomics sample data, and using this ID list for downlaods the uniprot annotation data.|
|[/UniProt.IDs](#/UniProt.IDs)||
|[/Uniprot.Mappings](#/Uniprot.Mappings)|Retrieve the uniprot annotation data by using ID mapping operations.|
|[/UniRef.map.organism](#/UniRef.map.organism)||


##### 1. 0. Samples CLI tools


|Function API|Info|
|------------|----|
|[/Data.Add.Mappings](#/Data.Add.Mappings)||
|[/Data.Add.ORF](#/Data.Add.ORF)||
|[/Data.Add.uniprotIDs](#/Data.Add.uniprotIDs)||
|[/Perseus.Stat](#/Perseus.Stat)||
|[/Perseus.Table](#/Perseus.Table)||
|[/plot.pimw](#/plot.pimw)|'calc. pI' - 'MW [kDa]' scatter plot of the protomics raw sample data.|
|[/Sample.Species.Normalization](#/Sample.Species.Normalization)||
|[/Shotgun.Data.Strip](#/Shotgun.Data.Strip)||


##### 2. 0. Samples expression analysis


|Function API|Info|
|------------|----|
|[/FoldChange.Matrix.Invert](#/FoldChange.Matrix.Invert)|Reverse the FoldChange value from the source result matrix.|
|[/proteinGroups.venn](#/proteinGroups.venn)||
|[/Relative.amount](#/Relative.amount)|Statistics of the relative expression value of the total proteins.|


##### 3. 1. uniprot annotation CLI tools


|Function API|Info|
|------------|----|
|[/blastX.fill.ORF](#/blastX.fill.ORF)||
|[/ID.Replace.bbh](#/ID.Replace.bbh)|Replace the source ID to a unify organism protein ID by using ``bbh`` method. <br />    This tools required the protein in ``datatset.csv`` associated with the alignment result in ``bbh.csv`` by using the ``query_name`` property.|
|[/KEGG.Color.Pathway](#/KEGG.Color.Pathway)||
|[/protein.annotations](#/protein.annotations)|Total proteins functional annotation by using uniprot database.|
|[/protein.EXPORT](#/protein.EXPORT)|Export the protein sequence and save as fasta format from the uniprot database dump XML.|
|[/proteins.Go.plot](#/proteins.Go.plot)|ProteinGroups sample data go profiling plot from the uniprot annotation data.|
|[/proteins.KEGG.plot](#/proteins.KEGG.plot)|KEGG function catalog profiling plot of the TP sample.|
|[/Species.Normalization](#/Species.Normalization)||
|[/UniRef.UniprotKB](#/UniRef.UniprotKB)||
|[/update.uniprot.mapped](#/update.uniprot.mapped)||


##### 4. 2. DEP analysis CLI tools


|Function API|Info|
|------------|----|
|[/DEP.logFC.hist](#/DEP.logFC.hist)|Using for plots the FC histogram when the experiment have no biological replicates.|
|[/DEP.logFC.Volcano](#/DEP.logFC.Volcano)|Volcano plot of the DEPs' analysis result.|
|[/DEP.uniprot.list](#/DEP.uniprot.list)||
|[/DEP.uniprot.list2](#/DEP.uniprot.list2)||
|[/DEP.venn](#/DEP.venn)|Generate the VennDiagram plot data and the venn plot tiff. The default parameter profile is using for the iTraq data.|
|[/DEPs.heatmap](#/DEPs.heatmap)|Generates the heatmap plot input data. The default label profile is using for the iTraq result.|
|[/DEPs.stat](#/DEPs.stat)|https://github.com/xieguigang/GCModeller.cli2R/blob/master/GCModeller.cli2R/R/log2FC_t-test.R|
|[/edgeR.Designer](#/edgeR.Designer)|Generates the edgeR inputs table|
|[/Merge.DEPs](#/Merge.DEPs)|Usually using for generates the heatmap plot matrix of the DEPs. This function call will generates two dataset, one is using for the heatmap plot and another is using for the venn diagram plot.|
|[/T.test.Designer.iTraq](#/T.test.Designer.iTraq)|Generates the iTraq data t.test DEP method inputs table|
|[/T.test.Designer.LFQ](#/T.test.Designer.LFQ)|Generates the LFQ data t.test DEP method inputs table|


##### 5. 3. Enrichment analysis tools


|Function API|Info|
|------------|----|
|[/Enrichment.Term.Filter](#/Enrichment.Term.Filter)|Filter the specific term result from the analysis output by using pattern keyword|
|[/Enrichments.ORF.info](#/Enrichments.ORF.info)|Retrive KEGG/GO info for the genes in the enrichment result.|
|[/GO.cellular_location.Plot](#/GO.cellular_location.Plot)|Visualize of the subcellular location result from the GO enrichment analysis.|
|[/Go.enrichment.plot](#/Go.enrichment.plot)|Go enrichment plot base on the KOBAS enrichment analysis result.|
|[/KEGG.Enrichment.PathwayMap](#/KEGG.Enrichment.PathwayMap)|Show the KEGG pathway map image by using KOBAS KEGG pathway enrichment result.|
|[/KEGG.enrichment.plot](#/KEGG.enrichment.plot)|Bar plots of the KEGG enrichment analysis result.|
|[/KOBAS.add.ORF](#/KOBAS.add.ORF)||
|[/KOBAS.split](#/KOBAS.split)|Split the KOBAS run output result text file as seperated csv file.|


##### 6. 3. Enrichment analysis tools: clusterProfiler


|Function API|Info|
|------------|----|
|[/enricher.background](#/enricher.background)|Create enrichment analysis background based on the uniprot xml database.|
|[/enrichment.go](#/enrichment.go)||
|[/Term2genes](#/Term2genes)||


##### 7. 3. Enrichment analysis tools: DAVID


|Function API|Info|
|------------|----|
|[/DAVID.Split](#/DAVID.Split)||
|[/GO.enrichment.DAVID](#/GO.enrichment.DAVID)||
|[/KEGG.enrichment.DAVID](#/KEGG.enrichment.DAVID)||
|[/KEGG.enrichment.DAVID.pathwaymap](#/KEGG.enrichment.DAVID.pathwaymap)||


##### 8. 4. Network enrichment visualize tools


|Function API|Info|
|------------|----|
|[/func.rich.string](#/func.rich.string)|DEPs' functional enrichment network based on string-db exports, and color by KEGG pathway.|
|[/Gene.list.from.KOBAS](#/Gene.list.from.KOBAS)|Using this command for generates the gene id list input for the STRING-db search.|
|[/richfun.KOBAS](#/richfun.KOBAS)||


##### 9. Data visualization tool


|Function API|Info|
|------------|----|
|[/DEP.heatmap.scatter.3D](#/DEP.heatmap.scatter.3D)|Visualize the DEPs' kmeans cluster result by using 3D scatter plot.|
|[/DEP.kmeans.scatter2D](#/DEP.kmeans.scatter2D)||
|[/matrix.clustering](#/matrix.clustering)||


##### 10. iTraq data analysis tool


|Function API|Info|
|------------|----|
|[/iTraq.Bridge.Matrix](#/iTraq.Bridge.Matrix)||
|[/iTraq.matrix.split](#/iTraq.matrix.split)|Split the raw matrix into different compare group based on the experimental designer information.|
|[/iTraq.RSD-P.Density](#/iTraq.RSD-P.Density)||
|[/iTraq.Symbol.Replacement](#/iTraq.Symbol.Replacement)|* Using this CLI tool for processing the tag header of iTraq result at first.|
|[/iTraq.t.test](#/iTraq.t.test)|Implements the screening for different expression proteins by using log2FC threshold and t.test pvalue threshold.|


##### 11. Repository data tools


|Function API|Info|
|------------|----|
|[/Imports.Go.obo.mysql](#/Imports.Go.obo.mysql)|Dumping GO obo database as mysql database files.|
|[/Imports.Uniprot.Xml](#/Imports.Uniprot.Xml)|Dumping the UniprotKB XML database as mysql database file.|

## CLI API list
--------------------------
<h3 id="/blastX.fill.ORF"> 1. /blastX.fill.ORF</h3>



**Prototype**: ``eggHTS.CLI::Int32 BlastXFillORF(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /blastX.fill.ORF /in <annotations.csv> /blastx <blastx.csv> [/out <out.csv>]
```
<h3 id="/COG.profiling.plot"> 2. /COG.profiling.plot</h3>

Plots the COGs category statics profiling of the target genome from the COG annotation file.

**Prototype**: ``eggHTS.CLI::Int32 COGCatalogProfilingPlot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /COG.profiling.plot /in <myvacog.csv> [/size <image_size, default=1800,1200> /out <out.png>]
```


#### Arguments
##### /in
The COG annotation result.

###### Example
```bash
/in <file, *.csv>
# (This argument can accept the ``std_out`` from upstream app as input)
```
<h3 id="/Data.Add.Mappings"> 3. /Data.Add.Mappings</h3>



**Prototype**: ``eggHTS.CLI::Int32 AddReMapping(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Data.Add.Mappings /in <data.csv> /bbh <bbh.csv> /ID.mappings <uniprot.ID.mappings.tsv> /uniprot <uniprot.XML> [/ID <fieldName> /out <out.csv>]
```
<h3 id="/Data.Add.ORF"> 4. /Data.Add.ORF</h3>



**Prototype**: ``eggHTS.CLI::Int32 DataAddORF(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Data.Add.ORF /in <data.csv> /uniprot <uniprot.XML> [/ID <fieldName> /out <out.csv>]
```
<h3 id="/Data.Add.uniprotIDs"> 5. /Data.Add.uniprotIDs</h3>



**Prototype**: ``eggHTS.CLI::Int32 DataAddUniprotIDs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Data.Add.uniprotIDs /in <annotations.csv> /data <data.csv> [/out <out.csv>]
```
<h3 id="/DAVID.Split"> 6. /DAVID.Split</h3>



**Prototype**: ``eggHTS.CLI::Int32 SplitDAVID(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DAVID.Split /in <DAVID.txt> [/out <out.DIR, default=./>]
```
<h3 id="/DEP.heatmap.scatter.3D"> 7. /DEP.heatmap.scatter.3D</h3>

Visualize the DEPs' kmeans cluster result by using 3D scatter plot.

**Prototype**: ``eggHTS.CLI::Int32 DEPHeatmap3D(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DEP.heatmap.scatter.3D /in <kmeans.csv> /sampleInfo <sampleInfo.csv> [/cluster.prefix <default="cluster: #"> /size <default=1600,1400> /schema <default=clusters> /view.angle <default=30,60,-56.25> /view.distance <default=2500> /arrow.factor <default=1,2> /cluster.title <names.csv> /out <out.png>]
```


#### Arguments
##### /in
The kmeans cluster result from ``/DEP.heatmap`` command.

###### Example
```bash
/in <file, *.csv>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### /sampleInfo
Sample info fot grouping the matrix column data and generates the 3d plot ``<x,y,z>`` coordinations.

###### Example
```bash
/sampleInfo <file, *.csv>
```
##### [/out]
The file path of the output plot image.

###### Example
```bash
/out <file, *.png, *.svg>
```
##### [/cluster.prefix]
The term prefix of the kmeans cluster name when display on the legend title.

###### Example
```bash
/cluster.prefix <term_string>
```
##### [/size]
The output 3D scatter plot image size.

###### Example
```bash
/size <term_string>
```
##### [/view.angle]
The view angle of the 3D scatter plot objects, in 3D direction of ``<X>,<Y>,<Z>``

###### Example
```bash
/view.angle <term_string>
```
##### [/view.distance]
The view distance from the 3D camera screen to the 3D objects.

###### Example
```bash
/view.distance <int32>
```
##### Accepted Types
###### /in
**Decalre**:  _Microsoft.VisualBasic.DataMining.KMeans.EntityClusterModel_

Example: 
```json
{
    "Properties": {
        "System.String": 0
    },
    "Cluster": "System.String",
    "ID": "System.String"
}
```

###### /sampleInfo
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

###### /size
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.drawing.size(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Drawing.Size</a>_

Example: 
```json
{
    "height": 0,
    "width": 0
}
```

<h3 id="/DEP.kmeans.scatter2D"> 8. /DEP.kmeans.scatter2D</h3>



**Prototype**: ``eggHTS.CLI::Int32 DEPKmeansScatter2D(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DEP.kmeans.scatter2D /in <kmeans.csv> /sampleInfo <sampleInfo.csv> [/t.log <default=-1> /cluster.prefix <default="cluster: #"> /size <1600,1400> /schema <default=clusters> /out <out.png>]
```
<h3 id="/DEP.logFC.hist"> 9. /DEP.logFC.hist</h3>

Using for plots the FC histogram when the experiment have no biological replicates.

**Prototype**: ``eggHTS.CLI::Int32 logFCHistogram(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DEP.logFC.hist /in <log2test.csv> [/step <0.25> /type <default=log2fc> /legend.title <Frequency(log2FC)> /x.axis "(min,max),tick=0.25" /color <lightblue> /size <1400,900> /out <out.png>]
```


#### Arguments
##### [/type]
Which field in the input dataframe should be using as the data source for the histogram plot? Default field(column) name is "log2FC".

###### Example
```bash
/type <term_string>
```
##### [/step]
The steps for generates the histogram test data.

###### Example
```bash
/step <float>
```
##### Accepted Types
###### /type
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.string(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.String</a>_

Example: 
```json
"System.String"
```

###### /step
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.single(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Single</a>_

Example: 
```json
0
```

<h3 id="/DEP.logFC.Volcano"> 10. /DEP.logFC.Volcano</h3>

Volcano plot of the DEPs' analysis result.

**Prototype**: ``eggHTS.CLI::Int32 logFCVolcano(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DEP.logFC.Volcano /in <DEP-log2FC.t.test-table.csv> [/title <title> /p.value <default=0.05> /level <default=1.5> /colors <up=red;down=green;other=black> /size <1400,1400> /display.count /out <plot.csv>]
```


#### Arguments
##### /in
The input DEPs t.test result, should contains at least 3 columns which are names: ``ID``, ``log2FC`` and ``p.value``

###### Example
```bash
/in <file/directory>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/size]
The canvas size of the output image.

###### Example
```bash
/size <term_string>
```
##### [/colors]
The color profile for the DEPs and proteins that no-changes, value string in format like: key=value, and seperated by ``;`` symbol.

###### Example
```bash
/colors <term_string>
```
##### [/title]
The plot main title.

###### Example
```bash
/title <term_string>
```
##### [/p.value]
The p.value cutoff threshold, default is 0.05.

###### Example
```bash
/p.value <float>
```
##### [/level]
The log2FC value cutoff threshold, default is ``log2(1.5)``.

###### Example
```bash
/level <float>
```
##### [/display.count]
Display the protein counts in the legend label? by default is not.

###### Example
```bash
/display.count
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.Analysis.HTS.Proteomics.DEP_iTraq_

Example: 
```json
{
    "Properties": {
        "System.String": "System.String"
    },
    "ID": "System.String",
    "FCavg": 0,
    "FDR": 0,
    "isDEP": true,
    "log2FC": 0,
    "pvalue": 0
}
```

<h3 id="/DEP.uniprot.list"> 11. /DEP.uniprot.list</h3>



**Prototype**: ``eggHTS.CLI::Int32 DEPUniprotIDlist(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DEP.uniprot.list /DEP <log2-test.DEP.csv> /sample <sample.csv> [/out <out.txt>]
```
<h3 id="/DEP.uniprot.list2"> 12. /DEP.uniprot.list2</h3>



**Prototype**: ``eggHTS.CLI::Int32 DEPUniprotIDs2(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DEP.uniprot.list2 /in <log2.test.csv> [/DEP.Flag <is.DEP?> /uniprot.Flag <uniprot> /species <scientifcName> /uniprot <uniprotXML> /out <out.txt>]
```
<h3 id="/DEP.venn"> 13. /DEP.venn</h3>

Generate the VennDiagram plot data and the venn plot tiff. The default parameter profile is using for the iTraq data.

**Prototype**: ``eggHTS.CLI::Int32 VennData(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DEP.venn /data <Directory> [/title <VennDiagram title> /out <out.DIR>]
```


#### Arguments
##### /data
A directory path which it contains the DEPs matrix csv files from the sample groups's analysis result.

###### Example
```bash
/data <file/directory>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/out]
A directory path which it will contains the venn data result, includes venn matrix, venn plot tiff image, etc.

###### Example
```bash
/out <file/directory>
```
##### [/title]
The main title of the venn plot.

###### Example
```bash
/title <term_string>
```
<h3 id="/DEPs.heatmap"> 14. /DEPs.heatmap</h3>

Generates the heatmap plot input data. The default label profile is using for the iTraq result.

**Prototype**: ``eggHTS.CLI::Int32 DEPs_heatmapKmeans(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DEPs.heatmap /data <Directory/csv_file> [/schema <color_schema, default=RdYlGn:c11> /no-clrev /KO.class /annotation <annotation.csv> /row.labels.geneName /hide.labels /is.matrix /cluster.n <default=6> /sampleInfo <sampleinfo.csv> /non_DEP.blank /title "Heatmap of DEPs log2FC" /t.log2 /tick <-1> /size <size, default=2000,3000> /legend.size <size, default=600,100> /out <out.DIR>]
```


#### Arguments
##### /data
This file path parameter can be both a directory which contains a set of DEPs result or a single csv file path.

###### Example
```bash
/data <file/directory>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/sampleInfo]
Describ the experimental group information

###### Example
```bash
/sampleInfo <file, *.csv>
```
##### [/out]
A directory path where will save the output heatmap plot image and the kmeans cluster details info.

###### Example
```bash
/out <file, *.csv, *.svg, *.png>
```
##### [/annotation]
The protein annotation data that extract from the uniprot database. Some advanced heatmap plot feature required of this annotation data presented.

###### Example
```bash
/annotation <file, *.csv>
```
##### [/schema]
The color patterns of the heatmap visualize, by default is using ``ColorBrewer`` colors.

###### Example
```bash
/schema <term_string>
```
##### [/title]
The main title of this chart plot.

###### Example
```bash
/title <term_string>
```
##### [/size]
The canvas size.

###### Example
```bash
/size <term_string>
```
##### [/cluster.n]
Expects the kmeans cluster result number, default is output 6 kmeans clusters.

###### Example
```bash
/cluster.n <int32>
```
##### [/tick]
The ticks value of the color legend, by default value -1 means generates ticks automatically.

###### Example
```bash
/tick <float>
```
##### [/non_DEP.blank]
If this parameter present, then all of the non-DEP that bring by the DEP set union, will strip as blank on its foldchange value, and set to 1 at finally. Default is reserve this non-DEP foldchange value.

###### Example
```bash
/non_DEP.blank
#(boolean flag does not require of argument value)
```
##### [/KO.class]
If this argument was set, then the KO class information for uniprotID will be draw on the output heatmap.

###### Example
```bash
/KO.class
#(boolean flag does not require of argument value)
```
##### [/hide.labels]
Hide the row labels?

###### Example
```bash
/hide.labels
#(boolean flag does not require of argument value)
```
##### [/t.log2]
If this parameter is presented, then it will means apply the log2 transform on the matrix cell value before the heatmap plot.

###### Example
```bash
/t.log2
#(boolean flag does not require of argument value)
```
##### [/no-clrev]
Do not reverse the color sequence.

###### Example
```bash
/no-clrev
#(boolean flag does not require of argument value)
```
##### [/is.matrix]
The input data is a data matrix, can be using for heatmap drawing directly.

###### Example
```bash
/is.matrix
#(boolean flag does not require of argument value)
```
##### [/row.labels.geneName]
This option will use the ``geneName``(from the annotation data) as the row display label instead of using uniprotID or geneID. This option required of the ``/annotation`` presented.

###### Example
```bash
/row.labels.geneName
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /sampleInfo
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

###### /annotation
**Decalre**:  _Microsoft.VisualBasic.Data.csv.IO.EntityObject_

Example: 
```json
{
    "Properties": {
        "System.String": "System.String"
    },
    "ID": "System.String"
}
```

###### /size
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.drawing.size(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Drawing.Size</a>_

Example: 
```json
{
    "height": 0,
    "width": 0
}
```

###### /KO.class
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.boolean(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Boolean</a>_

Example: 
```json
true
```

###### /is.matrix
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.boolean(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Boolean</a>_

Example: 
```json
true
```

###### /row.labels.geneName
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.boolean(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Boolean</a>_

Example: 
```json
true
```

<h3 id="/DEPs.stat"> 15. /DEPs.stat</h3>

https://github.com/xieguigang/GCModeller.cli2R/blob/master/GCModeller.cli2R/R/log2FC_t-test.R

**Prototype**: ``eggHTS.CLI::Int32 DEPStatics(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DEPs.stat /in <log2.test.csv> [/log2FC <default=log2FC> /out <out.stat.csv>]
```


#### Arguments
##### /in
The DEPs' t.test result in csv file format.

###### Example
```bash
/in <file, *.csv>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/out]
The stat count output file path.

###### Example
```bash
/out <file/directory>
# (This argument can output data to ``std_out``)
```
##### [/log2FC]
The field name that stores the log2FC value of the average FoldChange

###### Example
```bash
/log2FC <term_string>
```
<h3 id="/DEPs.takes.values"> 16. /DEPs.takes.values</h3>



**Prototype**: ``eggHTS.CLI::Int32 TakeDEPsValues(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DEPs.takes.values /in <DEPs.csv> [/boolean.tag <default=is.DEP> /by.FC <tag=value, default=logFC=log2(1.5)> /by.p.value <tag=value, p.value=0.05> /data <data.csv> /out <out.csv>]
```
<h3 id="/DEPs.union"> 17. /DEPs.union</h3>



**Prototype**: ``eggHTS.CLI::Int32 DEPsUnion(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /DEPs.union /in <csv.DIR> [/FC <default=logFC> /out <out.csv>]
```
<h3 id="/edgeR.Designer"> 18. /edgeR.Designer</h3>

Generates the edgeR inputs table

**Prototype**: ``eggHTS.CLI::Int32 edgeRDesigner(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /edgeR.Designer /in <proteinGroups.csv> /designer <designer.csv> [/label <default is empty> /deli <default=-> /out <out.DIR>]
```
<h3 id="/enricher.background"> 19. /enricher.background</h3>

Create enrichment analysis background based on the uniprot xml database.

**Prototype**: ``eggHTS.CLI::Int32 Backgrounds(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /enricher.background /in <uniprot.XML> [/mapping <maps.tsv> /out <term2gene.txt.DIR>]
```


#### Arguments
##### [/mapping]
The id mapping file, each row in format like ``id<TAB>uniprotID``

###### Example
```bash
/mapping <file/directory>
```
##### [/in]
The uniprotKB XML database which can be download from http://uniprot.org

###### Example
```bash
/in <file/directory>
```
<h3 id="/enrichment.go"> 20. /enrichment.go</h3>



**Prototype**: ``eggHTS.CLI::Int32 GoEnrichment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /enrichment.go /deg <deg.list> /backgrounds <genome_genes.list> /t2g <term2gene.csv> [/go <go_brief.csv> /out <enricher.result.csv>]
```
<h3 id="/Enrichment.Term.Filter"> 21. /Enrichment.Term.Filter</h3>

Filter the specific term result from the analysis output by using pattern keyword

**Prototype**: ``eggHTS.CLI::Int32 EnrichmentTermFilter(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Enrichment.Term.Filter /in <enrichment.csv> /filter <key-string> [/out <out.csv>]
```
<h3 id="/Enrichments.ORF.info"> 22. /Enrichments.ORF.info</h3>

Retrive KEGG/GO info for the genes in the enrichment result.

**Prototype**: ``eggHTS.CLI::Int32 RetriveEnrichmentGeneInfo(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Enrichments.ORF.info /in <enrichment.csv> /proteins <uniprot-genome.XML> [/nocut /ORF /out <out.csv>]
```


#### Arguments
##### /in
KOBAS analysis result output.

###### Example
```bash
/in <term_string>
```
##### [/nocut]
Default is using pvalue < 0.05 as term cutoff, if this argument presented, then will no pavlue cutoff for the terms input.

###### Example
```bash
/nocut <term_string>
```
##### [/ORF]
If this argument presented, then the program will using the ORF value in ``uniprot.xml`` as the record identifier,
default is using uniprotID in the accessions fields of the uniprot.XML records.

###### Example
```bash
/ORF
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.Analysis.Microarray.KOBAS.EnrichmentTerm_

Example: 
```json
{
    "Backgrounds": 0,
    "CorrectedPvalue": 0,
    "Database": "System.String",
    "ID": "System.String",
    "Input": "System.String",
    "ORF": [
        "System.String"
    ],
    "Pvalue": 0,
    "Term": "System.String",
    "link": "System.String",
    "number": 0
}
```

###### /ORF
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.boolean(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Boolean</a>_

Example: 
```json
true
```

<h3 id="/Exocarta.Hits"> 23. /Exocarta.Hits</h3>



**Prototype**: ``eggHTS.CLI::Int32 ExocartaHits(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Exocarta.Hits /in <list.txt> /annotation <annotations.csv> /exocarta <Exocarta.tsv> [/out <out.csv>]
```
<h3 id="/Fasta.IDlist"> 24. /Fasta.IDlist</h3>



**Prototype**: ``eggHTS.CLI::Int32 GetFastaIDlist(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Fasta.IDlist /in <prot.fasta> [/out <geneIDs.txt>]
```
<h3 id="/FoldChange.Matrix.Invert"> 25. /FoldChange.Matrix.Invert</h3>

Reverse the FoldChange value from the source result matrix.

**Prototype**: ``eggHTS.CLI::Int32 iTraqInvert(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /FoldChange.Matrix.Invert /in <data.csv> [/log2FC /out <invert.csv>]
```


#### Arguments
##### [/out]
This function will output a FoldChange matrix.

###### Example
```bash
/out <file, *.csv>
# (This argument can output data to ``std_out``)
```
##### [/log2FC]
This boolean flag indicated that the fold change value is log2FC, which required of power 2 and then invert by divided by 1.

###### Example
```bash
/log2FC
#(boolean flag does not require of argument value)
```
<h3 id="/func.rich.string"> 26. /func.rich.string</h3>

DEPs' functional enrichment network based on string-db exports, and color by KEGG pathway.

**Prototype**: ``eggHTS.CLI::Int32 FunctionalNetworkEnrichment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /func.rich.string /in <string_interactions.tsv> /uniprot <uniprot.XML> /DEP <dep.t.test.csv> [/map <map.tsv> /r.range <default=12,30> /log2FC <default=log2FC> /layout <string_network_coordinates.txt> /out <out.network.DIR>]
```


#### Arguments
##### /DEP
The DEPs t.test output result csv file.

###### Example
```bash
/DEP <file/directory>
```
##### [/map]
A tsv file that using for map the user custom gene ID as the uniprotKB ID, in format like: ``UserID<TAB>UniprotID``

###### Example
```bash
/map <file/directory>
```
##### [/r.range]
The network node size radius range, input string in format like: ``min,max``

###### Example
```bash
/r.range <term_string>
```
##### [/log2FC]
The csv field name for read the DEPs fold change value, default is ``log2FC`` as the field name.

###### Example
```bash
/log2FC <term_string>
```
##### Accepted Types
###### /DEP
**Decalre**:  _SMRUCC.genomics.Analysis.HTS.Proteomics.DEP_iTraq_

Example: 
```json
{
    "Properties": {
        "System.String": "System.String"
    },
    "ID": "System.String",
    "FCavg": 0,
    "FDR": 0,
    "isDEP": true,
    "log2FC": 0,
    "pvalue": 0
}
```

###### /r.range
**Decalre**:  _Microsoft.VisualBasic.ComponentModel.Ranges.Model.DoubleRange_

Example: 
```json
[
    
]
```

<h3 id="/Gene.list.from.KOBAS"> 27. /Gene.list.from.KOBAS</h3>

Using this command for generates the gene id list input for the STRING-db search.

**Prototype**: ``eggHTS.CLI::Int32 GeneIDListFromKOBASResult(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Gene.list.from.KOBAS /in <KOBAS.csv> [/p.value <default=1> /out <out.txt>]
```


#### Arguments
##### [/p.value]
Using for enrichment term result filters, default is p.value less than or equals to 1, means no cutoff.

###### Example
```bash
/p.value <term_string>
```
##### Accepted Types
###### /p.value
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.double(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Double</a>_

Example: 
```json
0
```

<h3 id="/GO.cellular_location.Plot"> 28. /GO.cellular_location.Plot</h3>

Visualize of the subcellular location result from the GO enrichment analysis.

**Prototype**: ``eggHTS.CLI::Int32 GO_cellularLocationPlot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /GO.cellular_location.Plot /in <KOBAS.GO.csv> [/GO <go.obo> /3D /colors <schemaName, default=Paired:c8> /out <out.png>]
```


#### Arguments
##### [/3D]
3D style pie chart for the plot?

###### Example
```bash
/3D <term_string>
```
##### [/colors]
Color schema name, default using color brewer color schema.

###### Example
```bash
/colors <term_string>
```
<h3 id="/GO.enrichment.DAVID"> 29. /GO.enrichment.DAVID</h3>



**Prototype**: ``eggHTS.CLI::Int32 DAVID_GOplot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /GO.enrichment.DAVID /in <DAVID.csv> [/tsv /go <go.obo> /size <default=1200,1000> /tick 1 /p.value <0.05> /out <out.png>]
```
<h3 id="/Go.enrichment.plot"> 30. /Go.enrichment.plot</h3>

Go enrichment plot base on the KOBAS enrichment analysis result.

**Prototype**: ``eggHTS.CLI::Int32 GO_enrichmentPlot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Go.enrichment.plot /in <enrichmentTerm.csv> [/bubble /r "log(x,1.5)" /Corrected /displays <default=10> /PlantRegMap /label.right /gray /pvalue <0.05> /size <2000,1600> /tick 1 /go <go.obo> /out <out.png>]
```


#### Arguments
##### /in
The KOBAS enrichment analysis output csv file.

###### Example
```bash
/in <file, *.csv>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/out]
The file path of the output plot image. If the graphics driver is using svg engine, then this result can be output to the standard output if this parameter is not presented in the CLI input.

###### Example
```bash
/out <file, *.svg, *.png>
# (This argument can output data to ``std_out``)
```
##### [/GO]
The GO database for category the enrichment term result into their corrisponding Go namespace. If this argument value is not presented in the CLI input, then program will using the GO database file from the GCModeller repository data system.

###### Example
```bash
/GO <file, *.obo>
```
##### [/r]
The bubble radius expression, when this enrichment plot is in ``/bubble`` mode.

###### Example
```bash
/r <term_string>
```
##### [/size]
The output image size in pixel.

###### Example
```bash
/size <term_string>
```
##### [/displays]
If the ``/bubble`` argument is not presented, then this will means the top number of the enriched term will plot on the barplot, else it is the term label display number in the bubble plot mode.
Set this argument value to -1 for display all terms.

###### Example
```bash
/displays <int32>
```
##### [/pvalue]
The p.value threshold for choose the terms that will be plot on the image, default is plot all terms that their enrichment p.value is smaller than 0.05.

###### Example
```bash
/pvalue <float>
```
##### [/tick]
The axis ticking interval value, using **-1** for generated this value automatically, or any other positive numeric value will setup this interval value manually.

###### Example
```bash
/tick <float>
```
##### [/label.right]
Align the label to right if this argument presented.

###### Example
```bash
/label.right
#(boolean flag does not require of argument value)
```
##### [/Corrected]
Using the corrected p.value instead of using the p.value as the term filter for this enrichment plot.

###### Example
```bash
/Corrected
#(boolean flag does not require of argument value)
```
##### [/bubble]
Visuallize the GO enrichment analysis result using bubble plot, not the bar plot.

###### Example
```bash
/bubble
#(boolean flag does not require of argument value)
```
##### [/gray]
Set the color of all of the labels, bars, class labels on this chart plot output to color gray? If this presented, then color schema will not working. Otherwise if this parameter argument is not presented in the CLI input, then the labels and bars will render color based on their corresponding GO namespace.

###### Example
```bash
/gray
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /size
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.drawing.size(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Drawing.Size</a>_

Example: 
```json
{
    "height": 0,
    "width": 0
}
```

###### /displays
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.int32(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Int32</a>_

Example: 
```json
0
```

###### /bubble
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.boolean(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Boolean</a>_

Example: 
```json
true
```

###### /gray
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.boolean(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Boolean</a>_

Example: 
```json
true
```

<h3 id="/iBAQ.Cloud"> 31. /iBAQ.Cloud</h3>

Cloud plot of the iBAQ DEPs result.

**Prototype**: ``eggHTS.CLI::Int32 DEPsCloudPlot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /iBAQ.Cloud /in <expression.csv> /annotations <annotations.csv> /DEPs <DEPs.csv> /tag <expression> [/out <out.png>]
```


#### Arguments
##### /tag
The field name in the ``/in`` matrix that using as the expression value.

###### Example
```bash
/tag <term_string>
```
<h3 id="/ID.Replace.bbh"> 32. /ID.Replace.bbh</h3>

Replace the source ID to a unify organism protein ID by using ``bbh`` method.
This tools required the protein in ``datatset.csv`` associated with the alignment result in ``bbh.csv`` by using the ``query_name`` property.

**Prototype**: ``eggHTS.CLI::Int32 BBHReplace(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /ID.Replace.bbh /in <dataset.csv> /bbh <bbh.csv> [/out <ID.replaced.csv>]
```
<h3 id="/Imports.Go.obo.mysql"> 33. /Imports.Go.obo.mysql</h3>

Dumping GO obo database as mysql database files.

**Prototype**: ``eggHTS.CLI::Int32 DumpGOAsMySQL(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Imports.Go.obo.mysql /in <go.obo> [/out <out.sql>]
```


#### Arguments
##### /in
The Go obo database file.

###### Example
```bash
/in <file, *.obo>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/out]
The output file path of the generated sql database file. If this argument is not presented in the CLI inputs, then all of the generated content will be output to the console.

###### Example
```bash
/out <file, *.sql>
# (This argument can output data to ``std_out``)
```
<h3 id="/Imports.Uniprot.Xml"> 34. /Imports.Uniprot.Xml</h3>

Dumping the UniprotKB XML database as mysql database file.

**Prototype**: ``eggHTS.CLI::Int32 DumpUniprot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Imports.Uniprot.Xml /in <uniprot.xml> [/out <out.sql>]
```


#### Arguments
##### /in
The uniprotKB XML database file.

###### Example
```bash
/in <file, *.xml>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/out]
The output file path of the generated sql database file. If this argument is not presented in the CLI inputs, then all of the generated content will be output to the console.

###### Example
```bash
/out <file, *.sql>
# (This argument can output data to ``std_out``)
```
<h3 id="/iTraq.Bridge.Matrix"> 35. /iTraq.Bridge.Matrix</h3>



**Prototype**: ``eggHTS.CLI::Int32 iTraqBridge(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /iTraq.Bridge.Matrix /A <A_iTraq.csv> /B <B_iTraq.csv> /C <bridge_symbol> [/symbols.A <symbols.csv> /symbols.B <symbols.csv> /out <matrix.csv>]
```
<h3 id="/iTraq.matrix.split"> 36. /iTraq.matrix.split</h3>

Split the raw matrix into different compare group based on the experimental designer information.

**Prototype**: ``eggHTS.CLI::Int32 iTraqAnalysisMatrixSplit(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /iTraq.matrix.split /in <matrix.csv> /sampleInfo <sampleInfo.csv> /designer <analysis.design.csv> [/allowed.swap /out <out.Dir>]
```


#### Arguments
##### /sampleInfo

###### Example
```bash
/sampleInfo <file/directory>
```
##### /designer
The analysis designer in csv file format for the DEPs calculation, should contains at least two column: ``<Controls>,<Experimental>``.
The analysis design: ``controls vs experimental`` means formula ``experimental/controls`` in the FoldChange calculation.

###### Example
```bash
/designer <file/directory>
```
##### Accepted Types
###### /sampleInfo
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

###### /designer
**Decalre**:  _SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.AnalysisDesigner_

Example: 
```json
{
    "Controls": "System.String",
    "Note": "System.String",
    "Treatment": "System.String"
}
```

<h3 id="/iTraq.RSD-P.Density"> 37. /iTraq.RSD-P.Density</h3>



**Prototype**: ``eggHTS.CLI::Int32 iTraqRSDPvalueDensityPlot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /iTraq.RSD-P.Density /in <matrix.csv> [/out <out.png>]
```


#### Arguments
##### /in
A data matrix which is comes from the ``/iTraq.matrix.split`` command.

###### Example
```bash
/in <file, *.csv>
# (This argument can accept the ``std_out`` from upstream app as input)
```
<h3 id="/iTraq.Symbol.Replacement"> 38. /iTraq.Symbol.Replacement</h3>

* Using this CLI tool for processing the tag header of iTraq result at first.

**Prototype**: ``eggHTS.CLI::Int32 iTraqSignReplacement(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /iTraq.Symbol.Replacement /in <iTraq.data.csv/xlsx> /symbols <symbols.csv> [/sheet.name <Sheet1> /out <out.DIR>]
```


#### Arguments
##### /in


###### Example
```bash
/in <file, *.csv, *.xlsx>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### /symbols
Using for replace the mass spectrum expeirment symbol to the user experiment tag.

###### Example
```bash
/symbols <file/directory>
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.Analysis.HTS.Proteomics.iTraqReader_

Example: 
```json
{
    "Properties": {
        "System.String": 0
    },
    "ID": "System.String",
    "AAs": "System.String",
    "Coverage": "System.String",
    "Description": "System.String",
    "MW": "System.String",
    "PSMs": "System.String",
    "Peptides": "System.String",
    "Proteins": "System.String",
    "Score": "System.String",
    "UniquePeptides": "System.String",
    "calcPI": "System.String"
}
```

###### /symbols
**Decalre**:  _SMRUCC.genomics.Analysis.HTS.Proteomics.iTraqSymbols_

Example: 
```json
{
    "AnalysisID": "System.String",
    "SampleGroup": "System.String",
    "SampleID": "System.String",
    "Symbol": "System.String"
}
```

<h3 id="/iTraq.t.test"> 39. /iTraq.t.test</h3>

Implements the screening for different expression proteins by using log2FC threshold and t.test pvalue threshold.

**Prototype**: ``eggHTS.CLI::Int32 iTraqTtest(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /iTraq.t.test /in <matrix.csv> [/level <default=1.5> /p.value <default=0.05> /FDR <default=0.05> /skip.significant.test /pairInfo <sampleTuple.csv> /out <out.csv>]
```


#### Arguments
##### [/FDR]
do FDR adjust on the p.value result? If this argument value is set to 1, means no adjustment.

###### Example
```bash
/FDR <float>
```
##### [/skip.significant.test]
If this option is presented in the CLI input, then the significant test from the p.value and FDR will be disabled.

###### Example
```bash
/skip.significant.test
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /skip.significant.test
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.boolean(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Boolean</a>_

Example: 
```json
true
```

<h3 id="/KEGG.Color.Pathway"> 40. /KEGG.Color.Pathway</h3>



**Prototype**: ``eggHTS.CLI::Int32 ColorKEGGPathwayMap(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /KEGG.Color.Pathway /in <protein.annotations.csv> /ref <KEGG.ref.pathwayMap.directory repository> [/out <out.directory>]
```
<h3 id="/KEGG.enrichment.DAVID"> 41. /KEGG.enrichment.DAVID</h3>



**Prototype**: ``eggHTS.CLI::Int32 DAVID_KEGGplot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /KEGG.enrichment.DAVID /in <david.csv> [/tsv /custom <ko00001.keg> /size <default=1200,1000> /p.value <default=0.05> /tick 1 /out <out.png>]
```
<h3 id="/KEGG.enrichment.DAVID.pathwaymap"> 42. /KEGG.enrichment.DAVID.pathwaymap</h3>



**Prototype**: ``eggHTS.CLI::Int32 DAVID_KEGGPathwayMap(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /KEGG.enrichment.DAVID.pathwaymap /in <david.csv> /uniprot <uniprot.XML> [/tsv /DEPs <deps.csv> /colors <default=red,blue,green> /tag <default=log2FC> /pvalue <default=0.05> /out <out.DIR>]
```
<h3 id="/KEGG.Enrichment.PathwayMap"> 43. /KEGG.Enrichment.PathwayMap</h3>

Show the KEGG pathway map image by using KOBAS KEGG pathway enrichment result.

**Prototype**: ``eggHTS.CLI::Int32 KEGGEnrichmentPathwayMap(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /KEGG.Enrichment.PathwayMap /in <kobas.csv> [/DEPs <deps.csv> /colors <default=red,blue,green> /map <id2uniprotID.txt> /uniprot <uniprot.XML> /pvalue <default=0.05> /out <DIR>]
```


#### Arguments
##### /colors
A string vector that setups the DEPs' color profiles, if the argument ``/DEPs`` is presented. value format is ``up,down,present``

###### Example
```bash
/colors <term_string>
```
##### [/DEPs]
Using for rendering color of the KEGG pathway map. The ``/colors`` argument only works when this argument is presented.

###### Example
```bash
/DEPs <file/directory>
```
##### [/map]
Maps user custom ID to uniprot ID. A tsv file with format: ``<customID><TAB><uniprotID>``

###### Example
```bash
/map <file/directory>
```
##### Accepted Types
###### /colors
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.string[](v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.String[]</a>_

Example: 
```json
[
    "System.String"
]
```

###### /DEPs
**Decalre**:  _SMRUCC.genomics.Analysis.HTS.Proteomics.DEP_iTraq_

Example: 
```json
{
    "Properties": {
        "System.String": "System.String"
    },
    "ID": "System.String",
    "FCavg": 0,
    "FDR": 0,
    "isDEP": true,
    "log2FC": 0,
    "pvalue": 0
}
```

<h3 id="/KEGG.enrichment.plot"> 44. /KEGG.enrichment.plot</h3>

Bar plots of the KEGG enrichment analysis result.

**Prototype**: ``eggHTS.CLI::Int32 KEGG_enrichment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /KEGG.enrichment.plot /in <enrichmentTerm.csv> [/gray /label.right /pvalue <0.05> /tick 1 /size <2000,1600> /out <out.png>]
```
<h3 id="/KO.Catalogs"> 45. /KO.Catalogs</h3>

Display the barplot of the KEGG orthology match.

**Prototype**: ``eggHTS.CLI::Int32 KOCatalogs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /KO.Catalogs /in <blast.mapping.csv> /ko <ko_genes.csv> [/key <Query_id> /mapTo <Subject_id> /out <outDIR>]
```
<h3 id="/KOBAS.add.ORF"> 46. /KOBAS.add.ORF</h3>



**Prototype**: ``eggHTS.CLI::Int32 KOBASaddORFsource(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /KOBAS.add.ORF /in <table.csv> /sample <sample.csv> [/out <out.csv>]
```


#### Arguments
##### /in
The KOBAS enrichment result.

###### Example
```bash
/in <term_string>
```
##### /sample
The uniprotID -> ORF annotation data. this table file should have a field named "ORF".

###### Example
```bash
/sample <term_string>
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.Analysis.Microarray.KOBAS.EnrichmentTerm_

Example: 
```json
{
    "Backgrounds": 0,
    "CorrectedPvalue": 0,
    "Database": "System.String",
    "ID": "System.String",
    "Input": "System.String",
    "ORF": [
        "System.String"
    ],
    "Pvalue": 0,
    "Term": "System.String",
    "link": "System.String",
    "number": 0
}
```

###### /sample
**Decalre**:  _Microsoft.VisualBasic.Data.csv.IO.EntityObject_

Example: 
```json
{
    "Properties": {
        "System.String": "System.String"
    },
    "ID": "System.String"
}
```

<h3 id="/KOBAS.Sim.Heatmap"> 47. /KOBAS.Sim.Heatmap</h3>



**Prototype**: ``eggHTS.CLI::Int32 SimHeatmap(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /KOBAS.Sim.Heatmap /in <sim.csv> [/size <1024,800> /colors <RdYlBu:8> /out <out.png>]
```
<h3 id="/KOBAS.similarity"> 48. /KOBAS.similarity</h3>



**Prototype**: ``eggHTS.CLI::Int32 KOBASSimilarity(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /KOBAS.Similarity /group1 <DIR> /group2 <DIR> [/fileName <default=output_run-Gene Ontology.csv> /out <out.DIR>]
```
<h3 id="/KOBAS.split"> 49. /KOBAS.split</h3>

Split the KOBAS run output result text file as seperated csv file.

**Prototype**: ``eggHTS.CLI::Int32 KOBASSplit(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /KOBAS.split /in <kobas.out_run.txt> [/out <DIR>]
```
<h3 id="/KOBAS.Term.Kmeans"> 50. /KOBAS.Term.Kmeans</h3>



**Prototype**: ``eggHTS.CLI::Int32 KOBASKMeans(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /KOBAS.Term.Kmeans /in <dir.input> [/n <default=3> /out <out.clusters.csv>]
```
<h3 id="/labelFree.t.test"> 51. /labelFree.t.test</h3>



**Prototype**: ``eggHTS.CLI::Int32 labelFreeTtest(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /labelFree.t.test /in <matrix.csv> /sampleInfo <sampleInfo.csv> /design <analysis_designer.csv> [/level <default=1.5> /p.value <default=0.05> /FDR <default=0.05> /out <out.csv>]
```
<h3 id="/matrix.clustering"> 52. /matrix.clustering</h3>



**Prototype**: ``eggHTS.CLI::Int32 MatrixClustering(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /matrix.clustering /in <matrix.csv> [/cluster.n <default:=10> /out <EntityClusterModel.csv>]
```
<h3 id="/Merge.DEPs"> 53. /Merge.DEPs</h3>

Usually using for generates the heatmap plot matrix of the DEPs. This function call will generates two dataset, one is using for the heatmap plot and another is using for the venn diagram plot.

**Prototype**: ``eggHTS.CLI::Int32 MergeDEPs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Merge.DEPs /in <*.csv,DIR> [/log2 /threshold "log(1.5,2)" /raw <sample.csv> /out <out.csv>]
```
<h3 id="/Network.PCC"> 54. /Network.PCC</h3>



**Prototype**: ``eggHTS.CLI::Int32 PccNetwork(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Network.PCC /in <matrix.csv> [/cut <default=0.45> /out <out.DIR>]
```
<h3 id="/paired.sample.designer"> 55. /paired.sample.designer</h3>



**Prototype**: ``eggHTS.CLI::Int32 PairedSampleDesigner(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /paired.sample.designer /sampleinfo <sampleInfo.csv> /designer <analysisDesigner.csv> /tuple <sampleTuple.csv> [/out <designer.out.csv.Directory>]
```
<h3 id="/Perseus.MajorityProteinIDs"> 56. /Perseus.MajorityProteinIDs</h3>

Export the uniprot ID list from ``Majority Protein IDs`` row and generates a text file for batch search of the uniprot database.

**Prototype**: ``eggHTS.CLI::Int32 MajorityProteinIDs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Perseus.MajorityProteinIDs /in <table.csv> [/out <out.txt>]
```
<h3 id="/Perseus.Stat"> 57. /Perseus.Stat</h3>



**Prototype**: ``eggHTS.CLI::Int32 PerseusStatics(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Perseus.Stat /in <proteinGroups.txt> [/out <out.csv>]
```
<h3 id="/Perseus.Table"> 58. /Perseus.Table</h3>



**Prototype**: ``eggHTS.CLI::Int32 PerseusTable(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Perseus.Table /in <proteinGroups.txt> [/out <out.csv>]
```
<h3 id="/Perseus.Table.annotations"> 59. /Perseus.Table.annotations</h3>



**Prototype**: ``eggHTS.CLI::Int32 PerseusTableAnnotations(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Perseus.Table.annotations /in <proteinGroups.csv> /uniprot <uniprot.XML> [/scientifcName <""> /out <out.csv>]
```
<h3 id="/pfamstring.enrichment"> 60. /pfamstring.enrichment</h3>



**Prototype**: ``eggHTS.CLI::Int32 PfamStringEnrichment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /pfamstring.enrichment /in <EntityClusterModel.csv> /pfamstring <pfamstring.csv> [/out <out.directory>]
```
<h3 id="/plot.pimw"> 61. /plot.pimw</h3>

'calc. pI' - 'MW [kDa]' scatter plot of the protomics raw sample data.

**Prototype**: ``eggHTS.CLI::Int32 pimwScatterPlot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /plot.pimw /in <samples.csv> [/field.pi <default="calc. pI"> /field.mw <default="MW [kDa]"> /legend.fontsize <20> /legend.size (100,30) /quantile.removes <default=1> /out <pimw.png> /size <1600,1200> /color <black> /ticks.Y <-1> /pt.size <8>]
```


#### Arguments
##### [/field.pi]
The field name that records the pI value of the protein samples, default is using iTraq result out format: ``calc. pI``

###### Example
```bash
/field.pi <term_string>
```
##### [/field.mw]
The field name that records the MW value of the protein samples, default is using iTraq result out format: ``MW [kDa]``

###### Example
```bash
/field.mw <term_string>
```
##### [/size]
The plot image its canvas size of this scatter plot.

###### Example
```bash
/size <term_string>
```
##### [/pt.size]
The radius size of the point in this scatter plot.

###### Example
```bash
/pt.size <float>
```
##### [/quantile.removes]
All of the Y sample value greater than this quantile value will be removed. By default is quantile 100%, means no cuts.

###### Example
```bash
/quantile.removes <float>
```
##### Accepted Types
###### /field.pi
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.string(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.String</a>_

Example: 
```json
"System.String"
```

###### /field.mw
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.string(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.String</a>_

Example: 
```json
"System.String"
```

###### /size
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.drawing.size(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Drawing.Size</a>_

Example: 
```json
{
    "height": 0,
    "width": 0
}
```

**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.drawing.sizef(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Drawing.SizeF</a>_

Example: 
```json
{
    "height": 0,
    "width": 0
}
```

###### /pt.size
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.double(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Double</a>_

Example: 
```json
0
```

###### /quantile.removes
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.double(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Double</a>_

Example: 
```json
0
```

<h3 id="/protein.annotations"> 62. /protein.annotations</h3>

Total proteins functional annotation by using uniprot database.

**Prototype**: ``eggHTS.CLI::Int32 SampleAnnotations(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /protein.annotations /uniprot <uniprot.XML> [/accession.ID /iTraq /list <uniprot.id.list.txt/rawtable.csv/Xlsx> /mapping <mappings.tab/tsv> /out <out.csv>]
```


#### Arguments
##### /uniprot
The Uniprot protein database in XML file format.

###### Example
```bash
/uniprot <file, *.xml>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/list]
Using for the iTraq method result.

###### Example
```bash
/list <file, *.txt, *.csv, *.xlsx>
```
##### [/mapping]
The id mapping table, only works when the argument ``/list`` is presented.

###### Example
```bash
/mapping <file, *.tsv, *.txt>
```
##### [/out]
The file path for output protein annotation table where to save.

###### Example
```bash
/out <file, *.csv>
```
##### [/iTraq]
* Using for the iTraq method result. If this option was enabled, then the protein ID in the output table using be using the value from the uniprot ID field.

###### Example
```bash
/iTraq
#(boolean flag does not require of argument value)
```
##### [/accession.ID]
Using the uniprot protein ID from the ``/uniprot`` input as the generated dataset's ID value, instead of using the numeric sequence as the ID value.

###### Example
```bash
/accession.ID
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /uniprot
**Decalre**:  _SMRUCC.genomics.Assembly.Uniprot.XML.UniProtXML_

Example: 
```json
{
    "copyright": "System.String",
    "entries": [
        {
            "CommentList": {
                "System.String": [
                    {
                        "event": {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        },
                        "evidence": "System.String",
                        "isoforms": [
                            {
                                "id": "System.String",
                                "name": "System.String",
                                "sequence": {
                                    "description": "System.String",
                                    "evidence": "System.String",
                                    "id": "System.String",
                                    "type": "System.String",
                                    "value": "System.String"
                                },
                                "text": {
                                    "description": "System.String",
                                    "evidence": "System.String",
                                    "id": "System.String",
                                    "type": "System.String",
                                    "value": "System.String"
                                }
                            }
                        ],
                        "subcellularLocations": [
                            {
                                "locations": [
                                    {
                                        "description": "System.String",
                                        "evidence": "System.String",
                                        "id": "System.String",
                                        "type": "System.String",
                                        "value": "System.String"
                                    }
                                ],
                                "topology": {
                                    "description": "System.String",
                                    "evidence": "System.String",
                                    "id": "System.String",
                                    "type": "System.String",
                                    "value": "System.String"
                                }
                            }
                        ],
                        "text": {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        },
                        "type": "System.String"
                    }
                ]
            },
            "Xrefs": {
                "System.String": [
                    {
                        "id": "System.String",
                        "molecule": {
                            "id": null
                        },
                        "properties": [
                            {
                                "type": "System.String",
                                "value": "System.String"
                            }
                        ],
                        "type": "System.String"
                    }
                ]
            },
            "accessions": [
                "System.String"
            ],
            "comments": [
                {
                    "event": {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    },
                    "evidence": "System.String",
                    "isoforms": [
                        {
                            "id": "System.String",
                            "name": "System.String",
                            "sequence": {
                                "description": "System.String",
                                "evidence": "System.String",
                                "id": "System.String",
                                "type": "System.String",
                                "value": "System.String"
                            },
                            "text": {
                                "description": "System.String",
                                "evidence": "System.String",
                                "id": "System.String",
                                "type": "System.String",
                                "value": "System.String"
                            }
                        }
                    ],
                    "subcellularLocations": [
                        {
                            "locations": [
                                {
                                    "description": "System.String",
                                    "evidence": "System.String",
                                    "id": "System.String",
                                    "type": "System.String",
                                    "value": "System.String"
                                }
                            ],
                            "topology": {
                                "description": "System.String",
                                "evidence": "System.String",
                                "id": "System.String",
                                "type": "System.String",
                                "value": "System.String"
                            }
                        }
                    ],
                    "text": {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    },
                    "type": "System.String"
                }
            ],
            "created": "System.String",
            "dataset": "System.String",
            "dbReferences": [
                {
                    "id": "System.String",
                    "molecule": {
                        "id": null
                    },
                    "properties": [
                        {
                            "type": "System.String",
                            "value": "System.String"
                        }
                    ],
                    "type": "System.String"
                }
            ],
            "features": [
                {
                    "description": "System.String",
                    "evidence": "System.String",
                    "location": {
                        "begin": {
                            "position": 0
                        },
                        "end": {
                            "position": 0
                        },
                        "position": {
                            "position": 0
                        }
                    },
                    "original": "System.String",
                    "type": "System.String",
                    "value": "System.String",
                    "variation": "System.String"
                }
            ],
            "gene": {
                "ORF": null,
                "Primary": null,
                "names": [
                    {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    }
                ]
            },
            "keywords": [
                {
                    "description": "System.String",
                    "evidence": "System.String",
                    "id": "System.String",
                    "type": "System.String",
                    "value": "System.String"
                }
            ],
            "modified": "System.String",
            "name": "System.String",
            "organism": {
                "dbReference": {
                    "description": "System.String",
                    "evidence": "System.String",
                    "id": "System.String",
                    "type": "System.String",
                    "value": "System.String"
                },
                "evidence": "System.String",
                "lineage": {
                    "taxonlist": [
                        "System.String"
                    ]
                },
                "names": [
                    {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    }
                ],
                "namesData": {
                    "System.String": {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    }
                }
            },
            "protein": {
                "alternativeNames": [
                    {
                        "ecNumber": [
                            {
                                "description": "System.String",
                                "evidence": "System.String",
                                "id": "System.String",
                                "type": "System.String",
                                "value": "System.String"
                            }
                        ],
                        "fullName": {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        },
                        "shortNames": [
                            {
                                "description": "System.String",
                                "evidence": "System.String",
                                "id": "System.String",
                                "type": "System.String",
                                "value": "System.String"
                            }
                        ]
                    }
                ],
                "recommendedName": {
                    "ecNumber": [
                        {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        }
                    ],
                    "fullName": {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    },
                    "shortNames": [
                        {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        }
                    ]
                },
                "submittedName": {
                    "ecNumber": [
                        {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        }
                    ],
                    "fullName": {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    },
                    "shortNames": [
                        {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        }
                    ]
                }
            },
            "proteinExistence": {
                "description": "System.String",
                "evidence": "System.String",
                "id": "System.String",
                "type": "System.String",
                "value": "System.String"
            },
            "references": [
                {
                    "citation": {
                        "authorList": [
                            {
                                "name": "System.String"
                            }
                        ],
                        "date": "System.String",
                        "db": "System.String",
                        "dbReferences": [
                            {
                                "id": "System.String",
                                "molecule": {
                                    "id": null
                                },
                                "properties": [
                                    {
                                        "type": "System.String",
                                        "value": "System.String"
                                    }
                                ],
                                "type": "System.String"
                            }
                        ],
                        "first": "System.String",
                        "last": "System.String",
                        "name": "System.String",
                        "title": "System.String",
                        "type": "System.String",
                        "volume": "System.String"
                    },
                    "key": "System.String",
                    "scope": [
                        "System.String"
                    ],
                    "source": {
                        "tissues": [
                            "System.String"
                        ]
                    }
                }
            ],
            "sequence": {
                "checksum": "System.String",
                "length": 0,
                "mass": "System.String",
                "modified": "System.String",
                "sequence": "System.String",
                "version": "System.String"
            },
            "version": "System.String"
        }
    ],
    "version": "System.String"
}
```

###### /list
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.string[](v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.String[]</a>_

Example: 
```json
[
    "System.String"
]
```

###### /iTraq
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.boolean(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Boolean</a>_

Example: 
```json
true
```

<h3 id="/protein.annotations.shotgun"> 63. /protein.annotations.shotgun</h3>



**Prototype**: ``eggHTS.CLI::Int32 SampleAnnotations2(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /protein.annotations.shotgun /p1 <data.csv> /p2 <data.csv> /uniprot <data.DIR/*.xml,*.tab> [/remapping /out <out.csv>]
```
<h3 id="/protein.EXPORT"> 64. /protein.EXPORT</h3>

Export the protein sequence and save as fasta format from the uniprot database dump XML.

**Prototype**: ``eggHTS.CLI::Int32 proteinEXPORT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /protein.EXPORT /in <uniprot.xml> [/sp <name> /exclude /out <out.fasta>]
```


#### Arguments
##### /uniprot
The Uniprot protein database in XML file format.

###### Example
```bash
/uniprot <file, *.xml>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/out]
The saved file path for output protein sequence fasta file.

###### Example
```bash
/out <file, *.fa, *.fasta, *.txt>
```
##### [/sp]
The organism scientific name.

###### Example
```bash
/sp <term_string>
```
##### [/exclude]
Exclude the specific organism by ``/sp`` scientific name instead of only include it?

###### Example
```bash
/exclude
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /uniprot
**Decalre**:  _SMRUCC.genomics.Assembly.Uniprot.XML.UniProtXML_

Example: 
```json
{
    "copyright": "System.String",
    "entries": [
        {
            "CommentList": {
                "System.String": [
                    {
                        "event": {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        },
                        "evidence": "System.String",
                        "isoforms": [
                            {
                                "id": "System.String",
                                "name": "System.String",
                                "sequence": {
                                    "description": "System.String",
                                    "evidence": "System.String",
                                    "id": "System.String",
                                    "type": "System.String",
                                    "value": "System.String"
                                },
                                "text": {
                                    "description": "System.String",
                                    "evidence": "System.String",
                                    "id": "System.String",
                                    "type": "System.String",
                                    "value": "System.String"
                                }
                            }
                        ],
                        "subcellularLocations": [
                            {
                                "locations": [
                                    {
                                        "description": "System.String",
                                        "evidence": "System.String",
                                        "id": "System.String",
                                        "type": "System.String",
                                        "value": "System.String"
                                    }
                                ],
                                "topology": {
                                    "description": "System.String",
                                    "evidence": "System.String",
                                    "id": "System.String",
                                    "type": "System.String",
                                    "value": "System.String"
                                }
                            }
                        ],
                        "text": {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        },
                        "type": "System.String"
                    }
                ]
            },
            "Xrefs": {
                "System.String": [
                    {
                        "id": "System.String",
                        "molecule": {
                            "id": null
                        },
                        "properties": [
                            {
                                "type": "System.String",
                                "value": "System.String"
                            }
                        ],
                        "type": "System.String"
                    }
                ]
            },
            "accessions": [
                "System.String"
            ],
            "comments": [
                {
                    "event": {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    },
                    "evidence": "System.String",
                    "isoforms": [
                        {
                            "id": "System.String",
                            "name": "System.String",
                            "sequence": {
                                "description": "System.String",
                                "evidence": "System.String",
                                "id": "System.String",
                                "type": "System.String",
                                "value": "System.String"
                            },
                            "text": {
                                "description": "System.String",
                                "evidence": "System.String",
                                "id": "System.String",
                                "type": "System.String",
                                "value": "System.String"
                            }
                        }
                    ],
                    "subcellularLocations": [
                        {
                            "locations": [
                                {
                                    "description": "System.String",
                                    "evidence": "System.String",
                                    "id": "System.String",
                                    "type": "System.String",
                                    "value": "System.String"
                                }
                            ],
                            "topology": {
                                "description": "System.String",
                                "evidence": "System.String",
                                "id": "System.String",
                                "type": "System.String",
                                "value": "System.String"
                            }
                        }
                    ],
                    "text": {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    },
                    "type": "System.String"
                }
            ],
            "created": "System.String",
            "dataset": "System.String",
            "dbReferences": [
                {
                    "id": "System.String",
                    "molecule": {
                        "id": null
                    },
                    "properties": [
                        {
                            "type": "System.String",
                            "value": "System.String"
                        }
                    ],
                    "type": "System.String"
                }
            ],
            "features": [
                {
                    "description": "System.String",
                    "evidence": "System.String",
                    "location": {
                        "begin": {
                            "position": 0
                        },
                        "end": {
                            "position": 0
                        },
                        "position": {
                            "position": 0
                        }
                    },
                    "original": "System.String",
                    "type": "System.String",
                    "value": "System.String",
                    "variation": "System.String"
                }
            ],
            "gene": {
                "ORF": null,
                "Primary": null,
                "names": [
                    {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    }
                ]
            },
            "keywords": [
                {
                    "description": "System.String",
                    "evidence": "System.String",
                    "id": "System.String",
                    "type": "System.String",
                    "value": "System.String"
                }
            ],
            "modified": "System.String",
            "name": "System.String",
            "organism": {
                "dbReference": {
                    "description": "System.String",
                    "evidence": "System.String",
                    "id": "System.String",
                    "type": "System.String",
                    "value": "System.String"
                },
                "evidence": "System.String",
                "lineage": {
                    "taxonlist": [
                        "System.String"
                    ]
                },
                "names": [
                    {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    }
                ],
                "namesData": {
                    "System.String": {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    }
                }
            },
            "protein": {
                "alternativeNames": [
                    {
                        "ecNumber": [
                            {
                                "description": "System.String",
                                "evidence": "System.String",
                                "id": "System.String",
                                "type": "System.String",
                                "value": "System.String"
                            }
                        ],
                        "fullName": {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        },
                        "shortNames": [
                            {
                                "description": "System.String",
                                "evidence": "System.String",
                                "id": "System.String",
                                "type": "System.String",
                                "value": "System.String"
                            }
                        ]
                    }
                ],
                "recommendedName": {
                    "ecNumber": [
                        {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        }
                    ],
                    "fullName": {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    },
                    "shortNames": [
                        {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        }
                    ]
                },
                "submittedName": {
                    "ecNumber": [
                        {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        }
                    ],
                    "fullName": {
                        "description": "System.String",
                        "evidence": "System.String",
                        "id": "System.String",
                        "type": "System.String",
                        "value": "System.String"
                    },
                    "shortNames": [
                        {
                            "description": "System.String",
                            "evidence": "System.String",
                            "id": "System.String",
                            "type": "System.String",
                            "value": "System.String"
                        }
                    ]
                }
            },
            "proteinExistence": {
                "description": "System.String",
                "evidence": "System.String",
                "id": "System.String",
                "type": "System.String",
                "value": "System.String"
            },
            "references": [
                {
                    "citation": {
                        "authorList": [
                            {
                                "name": "System.String"
                            }
                        ],
                        "date": "System.String",
                        "db": "System.String",
                        "dbReferences": [
                            {
                                "id": "System.String",
                                "molecule": {
                                    "id": null
                                },
                                "properties": [
                                    {
                                        "type": "System.String",
                                        "value": "System.String"
                                    }
                                ],
                                "type": "System.String"
                            }
                        ],
                        "first": "System.String",
                        "last": "System.String",
                        "name": "System.String",
                        "title": "System.String",
                        "type": "System.String",
                        "volume": "System.String"
                    },
                    "key": "System.String",
                    "scope": [
                        "System.String"
                    ],
                    "source": {
                        "tissues": [
                            "System.String"
                        ]
                    }
                }
            ],
            "sequence": {
                "checksum": "System.String",
                "length": 0,
                "mass": "System.String",
                "modified": "System.String",
                "sequence": "System.String",
                "version": "System.String"
            },
            "version": "System.String"
        }
    ],
    "version": "System.String"
}
```

###### /sp
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.string(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.String</a>_

Example: 
```json
"System.String"
```

<h3 id="/proteinGroups.venn"> 65. /proteinGroups.venn</h3>



**Prototype**: ``eggHTS.CLI::Int32 proteinGroupsVenn(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /proteinGroups.venn /in <proteinGroups.csv> /designer <designer.csv> [/label <tag label> /deli <delimiter, default=_> /out <out.venn.DIR>]
```
<h3 id="/proteins.Go.plot"> 66. /proteins.Go.plot</h3>

ProteinGroups sample data go profiling plot from the uniprot annotation data.

**Prototype**: ``eggHTS.CLI::Int32 ProteinsGoPlot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /proteins.Go.plot /in <proteins-uniprot-annotations.csv> [/GO <go.obo> /label.right /tick <default=-1> /level <default=2> /selects Q3 /size <2000,2200> /out <out.DIR>]
```


#### Arguments
##### /in
Uniprot XML database export result from ``/protein.annotations`` command.

###### Example
```bash
/in <file, *.csv>
```
##### [/GO]
The go database file path, if this argument is present in the CLI, then will using the GO.obo database file from GCModeller repository.

###### Example
```bash
/GO <file/directory>
```
##### [/out]
A directory path which will created for save the output result. The output result from this command contains a bar plot png image and a csv file for view the Go terms distribution in the sample uniprot annotation data.

###### Example
```bash
/out <file, *.csv, *.png>
```
##### [/size]
The size of the output plot image.

###### Example
```bash
/size <term_string>
```
##### [/selects]
The quantity selector for the bar plot content, by default is using quartile Q3 value, which means the term should have at least greater than Q3 quantitle then it will be draw on the bar plot.

###### Example
```bash
/selects <term_string>
```
##### [/level]
The GO annotation level from the DAG, default is level 2. Argument value -1 means no level.

###### Example
```bash
/level <int32>
```
##### [/tick]
The Axis ticking interval, if this argument is not present in the CLI, then program will create this interval value automatically.

###### Example
```bash
/tick <float>
```
##### [/label.right]
Plot GO term their label will be alignment on right. default is alignment left if this aegument is not present.

###### Example
```bash
/label.right
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /size
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.drawing.size(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Drawing.Size</a>_

Example: 
```json
{
    "height": 0,
    "width": 0
}
```

<h3 id="/proteins.KEGG.plot"> 67. /proteins.KEGG.plot</h3>

KEGG function catalog profiling plot of the TP sample.

**Prototype**: ``eggHTS.CLI::Int32 proteinsKEGGPlot(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /proteins.KEGG.plot /in <proteins-uniprot-annotations.csv> [/label.right /custom <sp00001.keg> /size <2200,2000> /tick 20 /out <out.DIR>]
```


#### Arguments
##### /in
Total protein annotation from UniProtKB database. Which is generated from the command ``/protein.annotations``.

###### Example
```bash
/in <file, *.Xlsx, *.csv>
```
##### /custom
Custom KO classification set can be download from: http://www.kegg.jp/kegg-bin/get_htext?ko00001.keg.
You can replace the %s mark using kegg organism code in url example as: http://www.kegg.jp/kegg-bin/download_htext?htext=%s00001&format=htext&filedir= for download the custom KO classification set.

###### Example
```bash
/custom <term_string>
```
##### [/size]
The canvas size value.

###### Example
```bash
/size <term_string>
```
##### [/label.right]
Align the label from right.

###### Example
```bash
/label.right
#(boolean flag does not require of argument value)
```
<h3 id="/Relative.amount"> 68. /Relative.amount</h3>

Statistics of the relative expression value of the total proteins.

**Prototype**: ``eggHTS.CLI::Int32 RelativeAmount(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Relative.amount /in <proteinGroups.csv> /designer <designer.csv> [/uniprot <annotations.csv> /label <tag label> /deli <delimiter, default=_> /out <out.csv>]
```
<h3 id="/richfun.KOBAS"> 69. /richfun.KOBAS</h3>



**Prototype**: ``eggHTS.CLI::Int32 KOBASNetwork(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /richfun.KOBAS /in <string_interactions.tsv> /uniprot <uniprot.XML> /DEP <dep.t.test.csv> /KOBAS <enrichment.csv> [/r.range <default=5,20> /fold <1.5> /iTraq /logFC <logFC> /layout <string_network_coordinates.txt> /out <out.network.DIR>]
```


#### Arguments
##### /KOBAS
The pvalue result in the enrichment term, will be using as the node radius size.

###### Example
```bash
/KOBAS <term_string>
```
<h3 id="/Sample.Species.Normalization"> 70. /Sample.Species.Normalization</h3>



**Prototype**: ``eggHTS.CLI::Int32 NormalizeSpecies_samples(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Sample.Species.Normalization /bbh <bbh.csv> /uniprot <uniprot.XML/DIR> /idMapping <refSeq2uniprotKB_mappings.tsv> /sample <sample.csv> [/Description <Description.FileName> /ID <columnName> /out <out.csv>]
```


#### Arguments
##### /bbh
The queryName should be the entry accession ID in the uniprot and the subject name is the refSeq proteinID in the NCBI database.

###### Example
```bash
/bbh <file/directory>
```
<h3 id="/Samples.IDlist"> 71. /Samples.IDlist</h3>

Extracts the protein hits from the protomics sample data, and using this ID list for downlaods the uniprot annotation data.

**Prototype**: ``eggHTS.CLI::Int32 GetIDlistFromSampleTable(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Samples.IDlist /in <samples.csv> [/Perseus /shotgun /pair <samples2.csv> /out <out.list.txt>]
```


#### Arguments
##### [/Perseus]
If this flag was presented, that means the input sample data is the Perseus analysis output file ``ProteinGroups.txt``, or the input sample data is the iTraq result.

###### Example
```bash
/Perseus
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /Perseus
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.boolean(v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.Boolean</a>_

Example: 
```json
true
```

<h3 id="/Shotgun.Data.Strip"> 72. /Shotgun.Data.Strip</h3>



**Prototype**: ``eggHTS.CLI::Int32 StripShotgunData(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Shotgun.Data.Strip /in <data.csv> [/out <output.csv>]
```
<h3 id="/Species.Normalization"> 73. /Species.Normalization</h3>



**Prototype**: ``eggHTS.CLI::Int32 NormalizeSpecies(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Species.Normalization /bbh <bbh.csv> /uniprot <uniprot.XML> /idMapping <refSeq2uniprotKB_mappings.tsv> /annotations <annotations.csv> [/out <out.csv>]
```


#### Arguments
##### /bbh
The queryName should be the entry accession ID in the uniprot and the subject name is the refSeq proteinID in the NCBI database.

###### Example
```bash
/bbh <file/directory>
```
<h3 id="/T.test.Designer.iTraq"> 74. /T.test.Designer.iTraq</h3>

Generates the iTraq data t.test DEP method inputs table

**Prototype**: ``eggHTS.CLI::Int32 TtestDesigner(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /T.test.Designer.iTraq /in <proteinGroups.csv> /designer <designer.csv> [/label <default is empty> /deli <default=-> /out <out.DIR>]
```
<h3 id="/T.test.Designer.LFQ"> 75. /T.test.Designer.LFQ</h3>

Generates the LFQ data t.test DEP method inputs table

**Prototype**: ``eggHTS.CLI::Int32 TtestDesignerLFQ(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /T.test.Designer.LFQ /in <proteinGroups.csv> /designer <designer.csv> [/label <default is empty> /deli <default=-> /out <out.DIR>]
```
<h3 id="/Term2genes"> 76. /Term2genes</h3>



**Prototype**: ``eggHTS.CLI::Int32 Term2Genes(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Term2genes /in <uniprot.XML> [/term <GO> /id <ORF> /out <out.tsv>]
```
<h3 id="/UniProt.IDs"> 77. /UniProt.IDs</h3>



**Prototype**: ``eggHTS.CLI::Int32 UniProtIDList(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /UniProt.IDs /in <list.csv/txt> [/out <list.txt>]
```
<h3 id="/Uniprot.Mappings"> 78. /Uniprot.Mappings</h3>

Retrieve the uniprot annotation data by using ID mapping operations.

**Prototype**: ``eggHTS.CLI::Int32 UniprotMappings(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /Uniprot.Mappings /in <id.list> [/type <P_REFSEQ_AC> /out <out.DIR>]
```
<h3 id="/UniRef.map.organism"> 79. /UniRef.map.organism</h3>



**Prototype**: ``eggHTS.CLI::Int32 UniRefMap2Organism(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /UniRef.map.organism /in <uniref.xml> [/org <organism_name> /out <out.csv>]
```


#### Arguments
##### /in
The uniRef XML cluster database its file path.

###### Example
```bash
/in <file/directory>
```
##### [/org]
The organism scientific name. If this argument is presented in the CLI input, then this program will output the top organism in this input data.

###### Example
```bash
/org <term_string>
```
<h3 id="/UniRef.UniprotKB"> 80. /UniRef.UniprotKB</h3>



**Prototype**: ``eggHTS.CLI::Int32 UniRef2UniprotKB(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /UniRef.UniprotKB /in <uniref.xml> [/out <maps.csv>]
```


#### Arguments
##### /in
The uniRef XML cluster database its file path.

###### Example
```bash
/in <file/directory>
```
<h3 id="/update.uniprot.mapped"> 81. /update.uniprot.mapped</h3>



**Prototype**: ``eggHTS.CLI::Int32 Update2UniprotMappedID(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
eggHTS /update.uniprot.mapped /in <table.csv> /mapping <mapping.tsv/tab> [/source /out <out.csv>]
```
