---
title: VirtualFootprint
tags: [maunal, tools]
date: 2016/10/19 16:38:39
---
# GCModeller [version 1.0.0.0]
> 

<!--more-->

**VirtualFootprint gene expression regulation network reconstruct tools**
_VirtualFootprint gene expression regulation network reconstruct tools_
Copyright ? http://services.gcmodeller.org 2016

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/VirtualFootprint.exe
**Root namespace**: ``xVirtualFootprint.CLI``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
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
|[/scan](#/scan)|Sanning genome sequence with a specific motif meme model.|
|[/Test.Footprints](#/Test.Footprints)||
|[/Test.Footprints.2](#/Test.Footprints.2)||
|[/TF.Sites](#/TF.Sites)||
|[/Trim.Regulates](#/Trim.Regulates)||
|[/Trim.Regulons](#/Trim.Regulons)||
|[/Trim.Strand](#/Trim.Strand)|Removes all of the sites which is on the different strand with the tag gene.|
|[/Write.Network](#/Write.Network)||


##### 1. TF/Regulon tools


|Function API|Info|
|------------|----|
|[/Density.Mappings](#/Density.Mappings)||
|[/Sites.Pathways](#/Sites.Pathways)|[Type 1] Grouping sites loci by pathway|
|[/Sites.Regulons](#/Sites.Regulons)|[Type 2]|
|[/TF.Density](#/TF.Density)||
|[/TF.Density.Batch](#/TF.Density.Batch)||
|[/TF.Regulons](#/TF.Regulons)||


##### 2. Primer Designer


|Function API|Info|
|------------|----|
|[/Export.Primer](#/Export.Primer)|[SSR name], [Forward primer], [Reverse primer]|
|[/restrict_enzyme.builds](#/restrict_enzyme.builds)||




## CLI API list
--------------------------
<h3 id="/Build.Footprints"> 1. /Build.Footprints</h3>

Build regulations from motif log site.
**Prototype**: ``xVirtualFootprint.CLI::Int32 BuildFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Build.Footprints /motifs <motifLogs.csv> /bbh <queryHits.csv> [/hitshash /sites <motifLogSites.Xml.DIR> /out <out.csv>]
```
###### Example
```bash
VirtualFootprint
```



#### Parameters information:
##### /bbh
The bbh hit result between the RegPrecise database and annotated genome proteins. query should be the RegPrecise TF and hits should be the annotated proteins.

###### Example
```bash

```
##### [/sites]
If this parameter not presented, then using GCModeller repository data as default.

###### Example
```bash

```
##### [/hitshash]
Using hit name as the bbh hash index key? default is using query name.

###### Example
```bash

```
##### Accepted Types
###### /bbh
###### /sites
###### /hitshash
<h3 id="/Density.Mappings"> 2. /Density.Mappings</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 ContextMappings(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Density.Mappings /in <density.Csv> [/scale 100 /out <out.PTT>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Export.Footprints.Sites"> 3. /Export.Footprints.Sites</h3>

Exports the motif sites from the virtual footprints sites.
**Prototype**: ``xVirtualFootprint.CLI::Int32 ExportFasta(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Export.Footprints.Sites /in <virtualfootprints> [/TF <locus_tag> /offset <group-offset> /out <outDIR/fasta>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Export.Primer"> 4. /Export.Primer</h3>

[SSR name], [Forward primer], [Reverse primer]
**Prototype**: ``xVirtualFootprint.CLI::Int32 ExportPrimer(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Export.Primer /in <primer.csv/DIR> [/out <out.DIR> /batch]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Filter.Promoter.Sites"> 5. /Filter.Promoter.Sites</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 PromoterSites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Filter.Promoter.Sites /in <motifLog.Csv> [/out <out.csv>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Filter.PromoterSites.Batch"> 6. /Filter.PromoterSites.Batch</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 PromoterSitesBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Filter.PromoterSites.Batch /in <motifLogs.DIR> [/num_threads <-1> /out <out.DIR>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/gc.outliers"> 7. /gc.outliers</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 Outliers(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /gc.outliers /mal <mal.fasta> [/q <quantiles:0.95,0.99,1> /method <gcskew/gccontent,default:gccontent> /out <out.csv> /win 250 /steps 50 /slides 5]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Intersect"> 8. /Intersect</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 Intersection(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Intersect /s1 <footprints.csv> /s2 <footprints.csv> [/out <out.csv> /strict]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/KEGG.Regulons"> 9. /KEGG.Regulons</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 KEGGRegulons(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /KEGG.Regulons /in <footprints.csv> /mods <KEGG.mods.DIR> [/pathway /out <out.csv>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Logs.Cast.Footprints"> 10. /Logs.Cast.Footprints</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 CastLogAsFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Logs.Cast.Footprints /in <motifLogs.Csv> [/out <out.csv>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/MAST_Sites.Screen"> 11. /MAST_Sites.Screen</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 SiteScreens(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /MAST_Sites.Screen /in <mast_sites.csv> /operons <regprecise.operons.csv> [/out <out.csv>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/MAST_Sites.Screen2"> 12. /MAST_Sites.Screen2</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 SiteScreens2(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /MAST_Sites.Screen2 /in <mast_sites.csv> [/n <2> /offset <30> /out <out.csv>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Merge.Footprints"> 13. /Merge.Footprints</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 MergeFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Merge.Footprints /in <inDIR> [/out <out.csv> /trim]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Merge.Regulons"> 14. /Merge.Regulons</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 MergeRegulonsExport(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Merge.Regulons /in <regulons.bbh.inDIR> [/out <out.csv>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Merge.Sites"> 15. /Merge.Sites</h3>

Merge the segment loci sites within the specific length offset ranges.
**Prototype**: ``xVirtualFootprint.CLI::Int32 MergeSites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Merge.Sites /in <segments.Csv> [/nt <nt.fasta> /out <out.csv> /offset <10>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Motif.From.MAL"> 16. /Motif.From.MAL</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 MotifFromMAL(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Motif.From.MAL /in <clustal.fasta> /out <outDIR>
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/restrict_enzyme.builds"> 17. /restrict_enzyme.builds</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 BuildEnzymeDb(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /restrict_enzyme.builds [/source <html_DIR> /out <out.json>]
```
###### Example
```bash
VirtualFootprint
```



#### Parameters information:
##### [/source]
Default using the data source from Wikipedia.

###### Example
```bash

```
##### [/out]
Enzyme database was writing to the GCModeller repository by default.

###### Example
```bash

```
##### Accepted Types
###### /source
###### /out
<h3 id="/scan"> 18. /scan</h3>

Sanning genome sequence with a specific motif meme model.
**Prototype**: ``xVirtualFootprint.CLI::Int32 Scanner(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /scan /motif <meme.txt> /nt <genome.fasta> [/PTT <genome.ptt> /atg-dist <250> /out <out.csv>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Sites.Pathways"> 19. /Sites.Pathways</h3>

[Type 1] Grouping sites loci by pathway
**Prototype**: ``xVirtualFootprint.CLI::Int32 PathwaySites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Sites.Pathways /pathway <KEGG.DIR> /sites <simple_segment.Csv.DIR> [/out <out.DIR>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Sites.Regulons"> 20. /Sites.Regulons</h3>

[Type 2]
**Prototype**: ``xVirtualFootprint.CLI::Int32 RegulonSites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Sites.Regulons /regulon <RegPrecise.Regulon.Csv> /sites <simple_segment.Csv.DIR> [/map <genome.PTT> /out <out.DIR>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Test.Footprints"> 21. /Test.Footprints</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 TestFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Test.Footprints /in <virtualfootprints.csv> /opr <regulon-operons.csv> [/out <out.csv>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Test.Footprints.2"> 22. /Test.Footprints.2</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 TestFootprints2(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Test.Footprints.2 /in <virtualfootprints.csv> [/out <out.csv> /n 2]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/TF.Density"> 23. /TF.Density</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 TFDensity(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /TF.Density /TF <TF-list.txt> /PTT <genome.PTT> [/ranges 5000 /out <out.csv> /cis /un-strand /batch]
```
###### Example
```bash
VirtualFootprint
```



#### Parameters information:
##### /TF
A plant text file with the TF locus_tag list.

###### Example
```bash

```
##### [/batch]
This function is works in batch mode.

###### Example
```bash

```
##### Accepted Types
###### /TF
###### /batch
<h3 id="/TF.Density.Batch"> 24. /TF.Density.Batch</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 TFDensityBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /TF.Density.Batch /TF <TF-list.txt> /PTT <genome.PTT.DIR> [/ranges 5000 /out <out.DIR> /cis /un-strand]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/TF.Regulons"> 25. /TF.Regulons</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 TFRegulons(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /TF.Regulons /bbh <tf.bbh.csv> /footprints <regulations.csv> [/out <out.csv>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/TF.Sites"> 26. /TF.Sites</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 TFMotifSites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /TF.Sites /bbh <bbh.Csv> /RegPrecise <RegPrecise.Xmls.DIR> [/hitHash /out <outDIR>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Trim.Regulates"> 27. /Trim.Regulates</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 TrimRegulates(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Trim.Regulates /in <virtualfootprint.csv> [/out <out.csv> /cut 0.65]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Trim.Regulons"> 28. /Trim.Regulons</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 TrimRegulon(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Trim.Regulons /in <regulons.csv> /pcc <pccDIR/sp_code> [/out <out.csv> /cut 0.65]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Trim.Strand"> 29. /Trim.Strand</h3>

Removes all of the sites which is on the different strand with the tag gene.
**Prototype**: ``xVirtualFootprint.CLI::Int32 TrimStrand(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Trim.Strand /in <segments.Csv> /PTT <genome.ptt> [/out <out.csv>]
```
###### Example
```bash
VirtualFootprint
```
<h3 id="/Write.Network"> 30. /Write.Network</h3>


**Prototype**: ``xVirtualFootprint.CLI::Int32 SaveNetwork(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
VirtualFootprint /Write.Network /in <regulons.csv> [/out <netDIR>]
```
###### Example
```bash
VirtualFootprint
```
