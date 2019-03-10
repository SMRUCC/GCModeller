---
title: VirtualFootprint
tags: [maunal, tools]
date: 5/28/2018 9:30:29 PM
---
# GCModeller [version 1.0.0.0]
> 

<!--more-->

**VirtualFootprint gene expression regulation network reconstruct tools**<br/>
_VirtualFootprint gene expression regulation network reconstruct tools_<br/>
Copyright © http://services.gcmodeller.org 2016

**Module AssemblyName**: VirtualFootprint<br/>
**Root namespace**: ``xVirtualFootprint.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Binary.KMeans.SW](#/Binary.KMeans.SW)||
|[/Build.Footprints](#/Build.Footprints)|Build regulations from motif log site.|
|[/Export.Footprints.Sites](#/Export.Footprints.Sites)|Exports the motif sites from the virtual footprints sites.|
|[/Filter.Promoter.Sites](#/Filter.Promoter.Sites)||
|[/Filter.PromoterSites.Batch](#/Filter.PromoterSites.Batch)||
|[/gc.outliers](#/gc.outliers)||
|[/Intersect](#/Intersect)||
|[/KEGG.Regulons](#/KEGG.Regulons)||
|[/Logs.Cast.Footprints](#/Logs.Cast.Footprints)||
|[/MAST_Sites.Screen](#/MAST_Sites.Screen)||
|[/MAST_Sites.Screen2](#/MAST_Sites.Screen2)||
|[/Merge.Footprints](#/Merge.Footprints)||
|[/Merge.Regulons](#/Merge.Regulons)||
|[/Merge.Sites](#/Merge.Sites)|Merge the segment loci sites within the specific length offset ranges.|
|[/Motif.From.MAL](#/Motif.From.MAL)||
|[/Promoter.Palindrome.loci.hist](#/Promoter.Palindrome.loci.hist)||
|[/scan](#/scan)|Sanning genome sequence with a specific motif meme model.|
|[/Test.Footprints](#/Test.Footprints)||
|[/Test.Footprints.2](#/Test.Footprints.2)||
|[/TF.Sites](#/TF.Sites)||
|[/Trim.Regulates](#/Trim.Regulates)||
|[/Trim.Regulons](#/Trim.Regulons)||
|[/Trim.Strand](#/Trim.Strand)|Removes all of the sites which is on the different strand with the tag gene.|
|[/Write.Network](#/Write.Network)||


##### 1. Primer Designer


|Function API|Info|
|------------|----|
|[/Export.Primer](#/Export.Primer)|[SSR name], [Forward primer], [Reverse primer]|
|[/restrict_enzyme.builds](#/restrict_enzyme.builds)||


##### 2. TF/Regulon tools


|Function API|Info|
|------------|----|
|[/Density.Mappings](#/Density.Mappings)||
|[/Sites.Pathways](#/Sites.Pathways)|[Type 1] Grouping sites loci by pathway|
|[/Sites.Regulons](#/Sites.Regulons)|[Type 2]|
|[/TF.Density](#/TF.Density)||
|[/TF.Density.Batch](#/TF.Density.Batch)||
|[/TF.Regulons](#/TF.Regulons)||

## CLI API list
--------------------------
<h3 id="/Binary.KMeans.SW"> 1. /Binary.KMeans.SW</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 BinaryKmeansSW(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Binary.KMeans.SW /in <dataset.fasta> [/cut 0.65 /minw 6 /first.ID /parallel.depth <-1> /out <out.DIR>]
```


#### Arguments
##### /first.ID
Using the first token in the fasta header as the output entity ID? Default is using the full title.

###### Example
```bash
/first.ID <term_string>
```
<h3 id="/Build.Footprints"> 2. /Build.Footprints</h3>

Build regulations from motif log site.

**Prototype**: ``xVirtualFootprint.CLI::Int32 BuildFootprints(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Build.Footprints /motifs <motifLogs.csv> /bbh <queryHits.csv> [/hitshash /sites <motifLogSites.Xml.DIR> /out <out.csv>]
```


#### Arguments
##### /bbh
The bbh hit result between the RegPrecise database and annotated genome proteins. query should be the RegPrecise TF and hits should be the annotated proteins.

###### Example
```bash
/bbh <term_string>
```
##### [/sites]
If this parameter not presented, then using GCModeller repository data as default.

###### Example
```bash
/sites <term_string>
```
##### [/hitshash]
Using hit name as the bbh hash index key? default is using query name.

###### Example
```bash
/hitshash <term_string>
```
<h3 id="/Density.Mappings"> 3. /Density.Mappings</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 ContextMappings(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Density.Mappings /in <density.Csv> [/scale 100 /out <out.PTT>]
```
<h3 id="/Export.Footprints.Sites"> 4. /Export.Footprints.Sites</h3>

Exports the motif sites from the virtual footprints sites.

**Prototype**: ``xVirtualFootprint.CLI::Int32 ExportFasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Export.Footprints.Sites /in <virtualfootprints> [/TF <locus_tag> /offset <group-offset> /out <outDIR/fasta>]
```
<h3 id="/Export.Primer"> 5. /Export.Primer</h3>

[SSR name], [Forward primer], [Reverse primer]

**Prototype**: ``xVirtualFootprint.CLI::Int32 ExportPrimer(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Export.Primer /in <primer.csv/DIR> [/out <out.DIR> /batch]
```
<h3 id="/Filter.Promoter.Sites"> 6. /Filter.Promoter.Sites</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 PromoterSites(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Filter.Promoter.Sites /in <motifLog.Csv> [/out <out.csv>]
```
<h3 id="/Filter.PromoterSites.Batch"> 7. /Filter.PromoterSites.Batch</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 PromoterSitesBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Filter.PromoterSites.Batch /in <motifLogs.DIR> [/num_threads <-1> /out <out.DIR>]
```
<h3 id="/gc.outliers"> 8. /gc.outliers</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 Outliers(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /gc.outliers /mal <mal.fasta> [/q <quantiles:0.95,0.99,1> /method <gcskew/gccontent,default:gccontent> /out <out.csv> /win 250 /steps 50 /slides 5]
```
<h3 id="/Intersect"> 9. /Intersect</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 Intersection(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Intersect /s1 <footprints.csv> /s2 <footprints.csv> [/out <out.csv> /strict]
```
<h3 id="/KEGG.Regulons"> 10. /KEGG.Regulons</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 KEGGRegulons(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /KEGG.Regulons /in <footprints.csv> /mods <KEGG.mods.DIR> [/pathway /out <out.csv>]
```
<h3 id="/Logs.Cast.Footprints"> 11. /Logs.Cast.Footprints</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 CastLogAsFootprints(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Logs.Cast.Footprints /in <motifLogs.Csv> [/out <out.csv>]
```
<h3 id="/MAST_Sites.Screen"> 12. /MAST_Sites.Screen</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 SiteScreens(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /MAST_Sites.Screen /in <mast_sites.csv> /operons <regprecise.operons.csv> [/out <out.csv>]
```
<h3 id="/MAST_Sites.Screen2"> 13. /MAST_Sites.Screen2</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 SiteScreens2(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /MAST_Sites.Screen2 /in <mast_sites.csv> [/n <2> /offset <30> /out <out.csv>]
```
<h3 id="/Merge.Footprints"> 14. /Merge.Footprints</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 MergeFootprints(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Merge.Footprints /in <inDIR> [/out <out.csv> /trim]
```
<h3 id="/Merge.Regulons"> 15. /Merge.Regulons</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 MergeRegulonsExport(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Merge.Regulons /in <regulons.bbh.inDIR> [/out <out.csv>]
```
<h3 id="/Merge.Sites"> 16. /Merge.Sites</h3>

Merge the segment loci sites within the specific length offset ranges.

**Prototype**: ``xVirtualFootprint.CLI::Int32 MergeSites(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Merge.Sites /in <segments.Csv> [/nt <nt.fasta> /out <out.csv> /offset <10>]
```
<h3 id="/Motif.From.MAL"> 17. /Motif.From.MAL</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 MotifFromMAL(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Motif.From.MAL /in <clustal.fasta> /out <outDIR>
```
<h3 id="/Promoter.Palindrome.loci.hist"> 18. /Promoter.Palindrome.loci.hist</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 PromoterPalindromeLociHist(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Promoter.Palindrome.loci.hist /in <palindrome.csv> /len <length-bp> [/step <10> /out <out.csv>]
```
<h3 id="/restrict_enzyme.builds"> 19. /restrict_enzyme.builds</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 BuildEnzymeDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /restrict_enzyme.builds [/source <html_DIR> /out <out.json>]
```


#### Arguments
##### [/source]
Default using the data source from Wikipedia.

###### Example
```bash
/source <term_string>
```
##### [/out]
Enzyme database was writing to the GCModeller repository by default.

###### Example
```bash
/out <term_string>
```
<h3 id="/scan"> 20. /scan</h3>

Sanning genome sequence with a specific motif meme model.

**Prototype**: ``xVirtualFootprint.CLI::Int32 Scanner(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /scan /motif <meme.txt> /nt <genome.fasta> [/PTT <genome.ptt> /atg-dist <250> /out <out.csv>]
```
<h3 id="/Sites.Pathways"> 21. /Sites.Pathways</h3>

[Type 1] Grouping sites loci by pathway

**Prototype**: ``xVirtualFootprint.CLI::Int32 PathwaySites(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Sites.Pathways /pathway <KEGG.DIR> /sites <simple_segment.Csv.DIR> [/out <out.DIR>]
```
<h3 id="/Sites.Regulons"> 22. /Sites.Regulons</h3>

[Type 2]

**Prototype**: ``xVirtualFootprint.CLI::Int32 RegulonSites(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Sites.Regulons /regulon <RegPrecise.Regulon.Csv> /sites <simple_segment.Csv.DIR> [/map <genome.PTT> /out <out.DIR>]
```
<h3 id="/Test.Footprints"> 23. /Test.Footprints</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 TestFootprints(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Test.Footprints /in <virtualfootprints.csv> /opr <regulon-operons.csv> [/out <out.csv>]
```
<h3 id="/Test.Footprints.2"> 24. /Test.Footprints.2</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 TestFootprints2(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Test.Footprints.2 /in <virtualfootprints.csv> [/out <out.csv> /n 2]
```
<h3 id="/TF.Density"> 25. /TF.Density</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 TFDensity(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /TF.Density /TF <TF-list.txt> /PTT <genome.PTT> [/ranges 5000 /out <out.csv> /cis /un-strand /batch]
```


#### Arguments
##### /TF
A plant text file with the TF locus_tag list.

###### Example
```bash
/TF <term_string>
```
##### [/batch]
This function is works in batch mode.

###### Example
```bash
/batch <term_string>
```
<h3 id="/TF.Density.Batch"> 26. /TF.Density.Batch</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 TFDensityBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /TF.Density.Batch /TF <TF-list.txt> /PTT <genome.PTT.DIR> [/ranges 5000 /out <out.DIR> /cis /un-strand]
```
<h3 id="/TF.Regulons"> 27. /TF.Regulons</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 TFRegulons(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /TF.Regulons /bbh <tf.bbh.csv> /footprints <regulations.csv> [/out <out.csv>]
```
<h3 id="/TF.Sites"> 28. /TF.Sites</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 TFMotifSites(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /TF.Sites /bbh <bbh.Csv> /RegPrecise <RegPrecise.Xmls.DIR> [/hitHash /out <outDIR>]
```
<h3 id="/Trim.Regulates"> 29. /Trim.Regulates</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 TrimRegulates(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Trim.Regulates /in <virtualfootprint.csv> [/out <out.csv> /cut 0.65]
```
<h3 id="/Trim.Regulons"> 30. /Trim.Regulons</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 TrimRegulon(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Trim.Regulons /in <regulons.csv> /pcc <pccDIR/sp_code> [/out <out.csv> /cut 0.65]
```
<h3 id="/Trim.Strand"> 31. /Trim.Strand</h3>

Removes all of the sites which is on the different strand with the tag gene.

**Prototype**: ``xVirtualFootprint.CLI::Int32 TrimStrand(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Trim.Strand /in <segments.Csv> /PTT <genome.ptt> [/out <out.csv>]
```
<h3 id="/Write.Network"> 32. /Write.Network</h3>



**Prototype**: ``xVirtualFootprint.CLI::Int32 SaveNetwork(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
VirtualFootprint /Write.Network /in <regulons.csv> [/out <netDIR>]
```
