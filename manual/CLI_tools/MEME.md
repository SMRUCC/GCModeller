---
title: MEME
tags: [maunal, tools]
date: 2016/10/19 16:38:32
---
# GCModeller [version 1.34.0.2]
> A wrapper tools for the NCBR meme tools, this is a powerfull tools for reconstruct the regulation in the bacterial genome.

<!--more-->

**MEME wrapper tools for reconstruct the regulation network in the bacterial genome.**
_MEME wrapper tools for reconstruct the regulation network in the bacterial genome._
Copyright ? SMRUCC 2015. All rights reserved.

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/MEME.exe
**Root namespace**: ``MEME.CLI``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Copys](#/Copys)||
|[/Copys.DIR](#/Copys.DIR)||
|[/EXPORT.MotifDraws](#/EXPORT.MotifDraws)||
|[/Footprints](#/Footprints)|3 - Generates the regulation footprints.|
|[/Hits.Context](#/Hits.Context)|2|
|[/LDM.Compares](#/LDM.Compares)||
|[/LDM.Selects](#/LDM.Selects)||
|[/MAST.MotifMatches](#/MAST.MotifMatches)||
|[/MAST.MotifMatchs.Family](#/MAST.MotifMatchs.Family)|1|
|[/mast.Regulations](#/mast.Regulations)||
|[/MEME.Batch](#/MEME.Batch)|Batch meme task by using tmod toolbox.|
|[/MEME.LDMs](#/MEME.LDMs)||
|[/Motif.BuildRegulons](#/Motif.BuildRegulons)||
|[/Motif.Info](#/Motif.Info)|Assign the phenotype information And genomic context Info for the motif sites. [SimpleSegment] -> [MotifLog]|
|[/Motif.Info.Batch](#/Motif.Info.Batch)|[SimpleSegment] -> [MotifLog]|
|[/Motif.Similarity](#/Motif.Similarity)|Export of the calculation result from the tomtom program.|
|[/MotifHits.Regulation](#/MotifHits.Regulation)||
|[/Regulator.Motifs](#/Regulator.Motifs)||
|[/Regulator.Motifs.Test](#/Regulator.Motifs.Test)||
|[/RfamSites](#/RfamSites)||
|[/seq.logo](#/seq.logo)||
|[/Site.MAST_Scan](#/Site.MAST_Scan)|[MAST.Xml] -> [SimpleSegment]|
|[/Site.MAST_Scan.Batch](#/Site.MAST_Scan.Batch)|[MAST.Xml] -> [SimpleSegment]|
|[/Site.RegexScan](#/Site.RegexScan)||
|[/site.scan](#/site.scan)||
|[/SiteHits.Footprints](#/SiteHits.Footprints)|Generates the regulation information.|
|[/SWTOM.Compares](#/SWTOM.Compares)||
|[/SWTOM.Compares.Batch](#/SWTOM.Compares.Batch)||
|[/SWTOM.LDM](#/SWTOM.LDM)||
|[/SWTOM.Query](#/SWTOM.Query)||
|[/SWTOM.Query.Batch](#/SWTOM.Query.Batch)||
|[/Tom.Query](#/Tom.Query)||
|[/Tom.Query.Batch](#/Tom.Query.Batch)||
|[/TomTOM](#/TomTOM)||
|[/TomTom.LDM](#/TomTom.LDM)||
|[/TomTOM.Similarity](#/TomTOM.Similarity)||
|[/TOMTOM.Similarity.Batch](#/TOMTOM.Similarity.Batch)||
|[/TomTom.Sites.Groups](#/TomTom.Sites.Groups)||
|[/Trim.MastSite](#/Trim.MastSite)||
|[--CExpr.WGCNA](#--CExpr.WGCNA)||
|[--family.statics](#--family.statics)||
|[--GetFasta](#--GetFasta)||
|[--hits.diff](#--hits.diff)||
|[--Intersect.Max](#--Intersect.Max)||
|[--logo.Batch](#--logo.Batch)||
|[--modules.regulates](#--modules.regulates)|Exports the Venn diagram model for the module regulations.|
|[Motif.Locates](#Motif.Locates)||
|[MotifScan](#MotifScan)|Scan for the motif site by using fragment similarity.|
|[--pathway.regulates](#--pathway.regulates)|Associates of the pathway regulation information for the predicted virtual footprint information.|
|[--site.Match](#--site.Match)||
|[--site.Matches](#--site.Matches)||
|[--site.Matches.text](#--site.Matches.text)|Using this function for processing the meme text output from the tmod toolbox.|
|[--site.stat](#--site.stat)|Statics of the PCC correlation distribution of the regulation|
|[VirtualFootprint.DIP](#VirtualFootprint.DIP)|Associate the dip information with the Sigma 70 virtual footprints.|


##### 1. RegPrecise Analysis Tools


|Function API|Info|
|------------|----|
|[/BBH.Select.Regulators](#/BBH.Select.Regulators)|Select bbh result for the regulators in RegPrecise database from the regulon bbh data.|
|[/Build.FamilyDb](#/Build.FamilyDb)||
|[/CORN](#/CORN)||
|[/LDM.MaxW](#/LDM.MaxW)||
|[--build.Regulations](#--build.Regulations)|Genome wide step 2|
|[--build.Regulations.From.Motifs](#--build.Regulations.From.Motifs)||
|[Download.Regprecise](#Download.Regprecise)|Download Regprecise database from Web API|
|[--Dump.KEGG.Family](#--Dump.KEGG.Family)||
|[--mapped-Back](#--mapped-Back)||
|[mast.compile](#mast.compile)||
|[mast.compile.bulk](#mast.compile.bulk)|Genome wide step 1|
|[Regprecise.Compile](#Regprecise.Compile)|The repository parameter is a directory path which is the regprecise database root directory in the GCModeller directory, if you didn't know how to set this value, please leave it blank.|
|[regulators.bbh](#regulators.bbh)|Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.|
|[regulators.compile](#regulators.compile)|Regprecise regulators data source compiler.|
|[--TCS.Module.Regulations](#--TCS.Module.Regulations)||
|[--TCS.Regulations](#--TCS.Regulations)||
|[wGet.Regprecise](#wGet.Regprecise)|Download Regprecise database from REST API|


##### 2. Motif Sites Analysis Tools


|Function API|Info|
|------------|----|
|[/Export.MotifSites](#/Export.MotifSites)|Motif iteration step 1|
|[/Export.Similarity.Hits](#/Export.Similarity.Hits)|Motif iteration step 2|
|[/Similarity.Union](#/Similarity.Union)|Motif iteration step 3|


##### 3. MEME tools database utilities


|Function API|Info|
|------------|----|
|[/Export.Regprecise.Motifs](#/Export.Regprecise.Motifs)||
|[/MAST_LDM.Build](#/MAST_LDM.Build)||
|[--Get.Intergenic](#--Get.Intergenic)||


##### 4. MEME analysis sequence parser


|Function API|Info|
|------------|----|
|[/Parser.DEGs](#/Parser.DEGs)||
|[/Parser.Locus](#/Parser.Locus)||
|[/Parser.Log2](#/Parser.Log2)||
|[/Parser.MAST](#/Parser.MAST)||
|[/Parser.Modules](#/Parser.Modules)||
|[/Parser.Operon](#/Parser.Operon)||
|[/Parser.Pathway](#/Parser.Pathway)||
|[/Parser.RegPrecise.Operons](#/Parser.RegPrecise.Operons)||
|[/Parser.Regulon](#/Parser.Regulon)||
|[/Parser.Regulon.gb](#/Parser.Regulon.gb)||
|[/Parser.Regulon.Merged](#/Parser.Regulon.Merged)||


##### 5. Regulon tools


|Function API|Info|
|------------|----|
|[/regulon.export](#/regulon.export)||
|[/Regulon.Reconstruct](#/Regulon.Reconstruct)||
|[/Regulon.Reconstruct2](#/Regulon.Reconstruct2)||
|[/Regulon.Reconstructs](#/Regulon.Reconstructs)|Doing the regulon reconstruction job in batch mode.|
|[/Regulon.Test](#/Regulon.Test)||




## CLI API list
--------------------------
<h3 id="/BBH.Select.Regulators"> 1. /BBH.Select.Regulators</h3>

Select bbh result for the regulators in RegPrecise database from the regulon bbh data.
**Prototype**: ``MEME.CLI::Int32 SelectRegulatorsBBH(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /BBH.Select.Regulators /in <bbh.csv> /db <regprecise_downloads.DIR> [/out <out.csv>]
```
###### Example
```bash
MEME
```
<h3 id="/Build.FamilyDb"> 2. /Build.FamilyDb</h3>


**Prototype**: ``MEME.CLI::Int32 BuildFamilyDb(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Build.FamilyDb /prot <RegPrecise.prot.fasta> /pfam <pfam-string.csv> [/out <out.Xml>]
```
###### Example
```bash
MEME
```
<h3 id="/Copys"> 3. /Copys</h3>


**Prototype**: ``MEME.CLI::Int32 BatchCopy(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Copys /in <inDIR> [/out <outDIR> /file <meme.txt>]
```
###### Example
```bash
MEME
```
<h3 id="/Copys.DIR"> 4. /Copys.DIR</h3>


**Prototype**: ``MEME.CLI::Int32 BatchCopyDIR(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Copys.DIR /in <inDIR> /out <outDIR>
```
###### Example
```bash
MEME
```
<h3 id="/CORN"> 5. /CORN</h3>


**Prototype**: ``MEME.CLI::Int32 CORN(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /CORN /in <operons.csv> /mast <mastDIR> /PTT <genome.ptt> [/out <out.csv>]
```
###### Example
```bash
MEME
```
<h3 id="/EXPORT.MotifDraws"> 6. /EXPORT.MotifDraws</h3>


**Prototype**: ``MEME.CLI::Int32 ExportMotifDraw(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /EXPORT.MotifDraws /in <virtualFootprints.csv> /MEME <meme.txt.DIR> /KEGG <KEGG_Modules/Pathways.DIR> [/pathway /out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Export.MotifSites"> 7. /Export.MotifSites</h3>

Motif iteration step 1
**Prototype**: ``MEME.CLI::Int32 ExportTestMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Export.MotifSites /in <meme.txt> [/out <outDIR> /batch]
```
###### Example
```bash
MEME
```
<h3 id="/Export.Regprecise.Motifs"> 8. /Export.Regprecise.Motifs</h3>


**Prototype**: ``MEME.CLI::Int32 ExportRegpreciseMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME
```
###### Example
```bash
MEME
```
<h3 id="/Export.Similarity.Hits"> 9. /Export.Similarity.Hits</h3>

Motif iteration step 2
**Prototype**: ``MEME.CLI::Int32 LoadSimilarityHits(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Export.Similarity.Hits /in <inDIR> [/out <out.Csv>]
```
###### Example
```bash
MEME
```
<h3 id="/Footprints"> 10. /Footprints</h3>

3 - Generates the regulation footprints.
**Prototype**: ``MEME.CLI::Int32 ToFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Footprints /footprints <footprints.xml> /coor <name/DIR> /DOOR <genome.opr> /maps <bbhMappings.Csv> [/out <out.csv> /cuts <0.65> /extract]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### [/extract]
Extract the DOOR operon when the regulated gene is the first gene of the operon.

###### Example
```bash

```
##### Accepted Types
###### /extract
<h3 id="/Hits.Context"> 11. /Hits.Context</h3>

2
**Prototype**: ``MEME.CLI::Int32 HitContext(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Hits.Context /footprints <footprints.Xml> /PTT <genome.PTT> [/out <out.Xml> /RegPrecise <RegPrecise.Regulations.Xml>]
```
###### Example
```bash
MEME
```
<h3 id="/LDM.Compares"> 12. /LDM.Compares</h3>


**Prototype**: ``MEME.CLI::Int32 CompareMotif(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /LDM.Compares /query <query.LDM.Xml> /sub <subject.LDM.Xml> [/out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/LDM.MaxW"> 13. /LDM.MaxW</h3>


**Prototype**: ``MEME.CLI::Int32 LDMMaxLen(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /LDM.MaxW [/in <sourceDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/LDM.Selects"> 14. /LDM.Selects</h3>


**Prototype**: ``MEME.CLI::Int32 Selectes(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /LDM.Selects /trace <footprints.xml> /meme <memeDIR> [/out <outDIR> /named]
```
###### Example
```bash
MEME
```
<h3 id="/MAST.MotifMatches"> 15. /MAST.MotifMatches</h3>


**Prototype**: ``MEME.CLI::Int32 MotifMatch2(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /MAST.MotifMatches /meme <meme.txt.DIR> /mast <MAST_OUT.DIR> [/out <out.csv>]
```
###### Example
```bash
MEME
```
<h3 id="/MAST.MotifMatchs.Family"> 16. /MAST.MotifMatchs.Family</h3>

1
**Prototype**: ``MEME.CLI::Int32 MotifMatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /MAST.MotifMatchs.Family /meme <meme.txt.DIR> /mast <MAST_OUT.DIR> [/out <out.Xml>]
```
###### Example
```bash
MEME
```
<h3 id="/mast.Regulations"> 17. /mast.Regulations</h3>


**Prototype**: ``MEME.CLI::Int32 MastRegulations(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /mast.Regulations /in <mastSites.Csv> /correlation <sp_name/DIR> /DOOR <DOOR.opr> [/out <footprint.csv> /cut <0.65>]
```
###### Example
```bash
MEME
```
<h3 id="/MAST_LDM.Build"> 18. /MAST_LDM.Build</h3>


**Prototype**: ``MEME.CLI::Int32 BuildPWMDb(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /MAST_LDM.Build /source <sourceDIR> [/out <exportDIR:=./> /evalue <1e-3>]
```
###### Example
```bash
MEME
```
<h3 id="/MEME.Batch"> 19. /MEME.Batch</h3>

Batch meme task by using tmod toolbox.
**Prototype**: ``MEME.CLI::Int32 MEMEBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /MEME.Batch /in <inDIR> [/out <outDIR> /evalue <1> /nmotifs <30> /mod <zoops> /maxw <100>]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### /in
A directory path which contains the fasta sequence for the meme motifs analysis.

###### Example
```bash

```
##### [/out]
A directory path which outputs the meme.txt data to that directory.

###### Example
```bash

```
##### Accepted Types
###### /in
###### /out
<h3 id="/MEME.LDMs"> 20. /MEME.LDMs</h3>


**Prototype**: ``MEME.CLI::Int32 MEME2LDM(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /MEME.LDMs /in <meme.txt> [/out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Motif.BuildRegulons"> 21. /Motif.BuildRegulons</h3>


**Prototype**: ``MEME.CLI::Int32 BuildRegulons(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Motif.BuildRegulons /meme <meme.txt.DIR> /model <FootprintTrace.xml> /DOOR <DOOR.opr> /maps <bbhmappings.csv> /corrs <name/DIR> [/cut <0.65> /out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Motif.Info"> 22. /Motif.Info</h3>

Assign the phenotype information And genomic context Info for the motif sites. [SimpleSegment] -> [MotifLog]
**Prototype**: ``MEME.CLI::Int32 MotifInfo(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Motif.Info /loci <loci.csv> [/motifs <motifs.DIR> /gff <genome.gff> /atg-dist 250 /out <out.csv>]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### /loci
The motif site info data set, type Is simple segment.

###### Example
```bash

```
##### /motifs
A directory which contains the motifsitelog data in the xml file format. Regulogs.Xml source directory

###### Example
```bash

```
##### Accepted Types
###### /loci
###### /motifs
<h3 id="/Motif.Info.Batch"> 23. /Motif.Info.Batch</h3>

[SimpleSegment] -> [MotifLog]
**Prototype**: ``MEME.CLI::Int32 MotifInfoBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Motif.Info.Batch /in <sites.csv.inDIR> /gffs <gff.DIR> [/motifs <regulogs.motiflogs.MEME.DIR> /num_threads -1 /atg-dist 350 /out <out.DIR>]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### /motifs
Regulogs.Xml source directory

###### Example
```bash

```
##### [/num_threads]
Default Is -1, means auto config of the threads number.

###### Example
```bash

```
##### Accepted Types
###### /motifs
###### /num_threads
<h3 id="/Motif.Similarity"> 24. /Motif.Similarity</h3>

Export of the calculation result from the tomtom program.
**Prototype**: ``MEME.CLI::Int32 MEMETOM_MotifSimilarity(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Motif.Similarity /in <tomtom.DIR> /motifs <MEME_OUT.DIR> [/out <out.csv> /bp.var]
```
###### Example
```bash
MEME
```
<h3 id="/MotifHits.Regulation"> 25. /MotifHits.Regulation</h3>


**Prototype**: ``MEME.CLI::Int32 HitsRegulation(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /MotifHits.Regulation /hits <motifHits.Csv> /source <meme.txt.DIR> /PTT <genome.PTT> /correlates <sp/DIR> /bbh <bbhh.csv> [/out <out.footprints.Csv>]
```
###### Example
```bash
MEME
```
<h3 id="/Parser.DEGs"> 26. /Parser.DEGs</h3>


**Prototype**: ``MEME.CLI::Int32 ParserDEGs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Parser.DEGs /degs <deseq2.csv> /PTT <genomePTT.DIR> /door <genome.opr> /out <out.DIR> [/log-fold 2]
```
###### Example
```bash
MEME
```
<h3 id="/Parser.Locus"> 27. /Parser.Locus</h3>


**Prototype**: ``MEME.CLI::Int32 ParserLocus(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Parser.Locus /locus <locus.txt> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/out <out.DIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Parser.Log2"> 28. /Parser.Log2</h3>


**Prototype**: ``MEME.CLI::Int32 ParserLog2(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Parser.Log2 /in <log2.csv> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/factor 1 /out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Parser.MAST"> 29. /Parser.MAST</h3>


**Prototype**: ``MEME.CLI::Int32 ParserMAST(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Parser.MAST /sites <mastsites.csv> /ptt <genome-context.pttDIR> /door <genome.opr> [/out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Parser.Modules"> 30. /Parser.Modules</h3>


**Prototype**: ``MEME.CLI::Int32 ModuleParser(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Parser.Modules /KEGG.Modules <KEGG.modules.DIR> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/locus <union/initx/locus, default:=union> /out <fasta.outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Parser.Operon"> 31. /Parser.Operon</h3>


**Prototype**: ``MEME.CLI::Int32 ParserNextIterator(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Parser.Operon /in <footprint.csv> /PTT <PTTDIR> [/out <outDIR> /family /offset <50> /all]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### [/family]
Group the source by family? Or output the source in one fasta set

###### Example
```bash

```
##### Accepted Types
###### /family
<h3 id="/Parser.Pathway"> 32. /Parser.Pathway</h3>


**Prototype**: ``MEME.CLI::Int32 PathwayParser(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Parser.Pathway /KEGG.Pathways <KEGG.pathways.DIR> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/locus <union/initx/locus, default:=union> /out <fasta.outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Parser.RegPrecise.Operons"> 33. /Parser.RegPrecise.Operons</h3>


**Prototype**: ``MEME.CLI::Int32 ParserRegPreciseOperon(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Parser.RegPrecise.Operons /operon <operons.Csv> /PTT <PTT_DIR> [/corn /DOOR <genome.opr> /id <null> /locus <union/initx/locus, default:=union> /out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Parser.Regulon"> 34. /Parser.Regulon</h3>


**Prototype**: ``MEME.CLI::Int32 RegulonParser(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Parser.Regulon /inDIR <regulons.inDIR> /out <fasta.outDIR> /PTT <genomePTT.DIR> [/door <genome.opr>]
```
###### Example
```bash
MEME
```
<h3 id="/Parser.Regulon.gb"> 35. /Parser.Regulon.gb</h3>


**Prototype**: ``MEME.CLI::Int32 RegulonParser2(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Parser.Regulon.gb /inDIR <regulons.inDIR> /out <fasta.outDIR> /gb <genbank.gbk> [/door <genome.opr>]
```
###### Example
```bash
MEME
```
<h3 id="/Parser.Regulon.Merged"> 36. /Parser.Regulon.Merged</h3>


**Prototype**: ``MEME.CLI::Int32 RegulonParser3(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Parser.Regulon.Merged /in <merged.Csv> /out <fasta.outDIR> /PTT <genomePTT.DIR> [/DOOR <genome.opr>]
```
###### Example
```bash
MEME
```
<h3 id="/Regulator.Motifs"> 37. /Regulator.Motifs</h3>


**Prototype**: ``MEME.CLI::Int32 RegulatorMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Regulator.Motifs /bbh <bbh.csv> /regprecise <genome.DIR> [/out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Regulator.Motifs.Test"> 38. /Regulator.Motifs.Test</h3>


**Prototype**: ``MEME.CLI::Int32 TestRegulatorMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Regulator.Motifs.Test /hits <familyHits.Csv> /motifs <motifHits.Csv> [/out <out.csv>]
```
###### Example
```bash
MEME
```
<h3 id="/regulon.export"> 39. /regulon.export</h3>


**Prototype**: ``MEME.CLI::Int32 ExportRegulon(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /regulon.export /in <sw-tom_out.DIR> /ref <regulon.bbh.xml.DIR> [/out <out.csv>]
```
###### Example
```bash
MEME
```
<h3 id="/Regulon.Reconstruct"> 40. /Regulon.Reconstruct</h3>


**Prototype**: ``MEME.CLI::Int32 RegulonReconstruct(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Regulon.Reconstruct /bbh <bbh.csv> /genome <RegPrecise.genome.xml> /door <operon.door> [/out <outfile.csv>]
```
###### Example
```bash
MEME
```
<h3 id="/Regulon.Reconstruct2"> 41. /Regulon.Reconstruct2</h3>


**Prototype**: ``MEME.CLI::Int32 RegulonReconstructs2(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Regulon.Reconstruct2 /bbh <bbh.csv> /genome <RegPrecise.genome.DIR> /door <operons.opr> [/out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Regulon.Reconstructs"> 42. /Regulon.Reconstructs</h3>

Doing the regulon reconstruction job in batch mode.
**Prototype**: ``MEME.CLI::Int32 RegulonReconstructs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Regulon.Reconstructs /bbh <bbh_EXPORT_csv.DIR> /genome <RegPrecise.genome.DIR> [/door <operon.door> /out <outDIR>]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### /bbh
A directory which contains the bbh export csv data from the localblast tool.

###### Example
```bash

```
##### /genome
The directory which contains the RegPrecise bacterial genome downloads data from the RegPrecise web server.

###### Example
```bash

```
##### /door
Door file which is the prediction data of the bacterial operon.

###### Example
```bash

```
##### Accepted Types
###### /bbh
###### /genome
###### /door
<h3 id="/Regulon.Test"> 43. /Regulon.Test</h3>


**Prototype**: ``MEME.CLI::Int32 RegulonTest(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Regulon.Test /in <meme.txt> /reg <genome.bbh.regulon.xml> /bbh <maps.bbh.Csv>
```
###### Example
```bash
MEME
```
<h3 id="/RfamSites"> 44. /RfamSites</h3>


**Prototype**: ``MEME.CLI::Int32 RfamSites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /RfamSites /source <sourceDIR> [/out <out.fastaDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/seq.logo"> 45. /seq.logo</h3>


**Prototype**: ``MEME.CLI::Int32 SequenceLogoTask(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /seq.logo /in <meme.txt> [/out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Similarity.Union"> 46. /Similarity.Union</h3>

Motif iteration step 3
**Prototype**: ``MEME.CLI::Int32 UnionSimilarity(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Similarity.Union /in <preSource.fasta.DIR> /meme <meme.txt.DIR> /hits <similarity_hist.Csv> [/out <out.DIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Site.MAST_Scan"> 47. /Site.MAST_Scan</h3>

[MAST.Xml] -> [SimpleSegment]
**Prototype**: ``MEME.CLI::Int32 SiteMASTScan(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Site.MAST_Scan /mast <mast.xml/DIR> [/batch /out <out.csv>]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### [/batch]
If this parameter presented in the CLI, then the parameter /mast will be used as a DIR.

###### Example
```bash

```
##### Accepted Types
###### /batch
<h3 id="/Site.MAST_Scan.Batch"> 48. /Site.MAST_Scan.Batch</h3>

[MAST.Xml] -> [SimpleSegment]
**Prototype**: ``MEME.CLI::Int32 SiteMASTScanBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Site.MAST_Scan /mast <mast.xml.DIR> [/out <out.csv.DIR> /num_threads <-1>]
```
###### Example
```bash
MEME
```
<h3 id="/Site.RegexScan"> 49. /Site.RegexScan</h3>


**Prototype**: ``MEME.CLI::Int32 SiteRegexScan(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Site.RegexScan /meme <meme.txt> /nt <nt.fasta> [/batch /out <out.csv>]
```
###### Example
```bash
MEME
```
<h3 id="/site.scan"> 50. /site.scan</h3>


**Prototype**: ``MEME.CLI::Int32 SiteScan(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /site.scan /query <LDM.xml> /subject <subject.fasta> [/out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="/SiteHits.Footprints"> 51. /SiteHits.Footprints</h3>

Generates the regulation information.
**Prototype**: ``MEME.CLI::Int32 SiteHitsToFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /SiteHits.Footprints /sites <MotifSiteHits.Csv> /bbh <bbh.Csv> /meme <meme.txt_DIR> /PTT <genome.PTT> /DOOR <DOOR.opr> [/queryHash /out <out.csv>]
```
###### Example
```bash
MEME
```
<h3 id="/SWTOM.Compares"> 52. /SWTOM.Compares</h3>


**Prototype**: ``MEME.CLI::Int32 SWTomCompares(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /SWTOM.Compares /query <query.meme.txt> /subject <subject.meme.txt> [/out <outDIR> /no-HTML]
```
###### Example
```bash
MEME
```
<h3 id="/SWTOM.Compares.Batch"> 53. /SWTOM.Compares.Batch</h3>


**Prototype**: ``MEME.CLI::Int32 SWTomComparesBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /SWTOM.Compares.Batch /query <query.meme.DIR> /subject <subject.meme.DIR> [/out <outDIR> /no-HTML]
```
###### Example
```bash
MEME
```
<h3 id="/SWTOM.LDM"> 54. /SWTOM.LDM</h3>


**Prototype**: ``MEME.CLI::Int32 SWTomLDM(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /SWTOM.LDM /query <ldm.xml> /subject <ldm.xml> [/out <outDIR> /method <pcc/ed/sw; default:=pcc>]
```
###### Example
```bash
MEME
```
<h3 id="/SWTOM.Query"> 55. /SWTOM.Query</h3>


**Prototype**: ``MEME.CLI::Int32 SWTomQuery(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /SWTOM.Query /query <meme.txt> [/out <outDIR> /method <pcc> /bits.level 1.6 /minW 6 /no-HTML]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### [/no-HTML]
If this parameter is true, then only the XML result will be export.

###### Example
```bash

```
##### Accepted Types
###### /no-HTML
<h3 id="/SWTOM.Query.Batch"> 56. /SWTOM.Query.Batch</h3>


**Prototype**: ``MEME.CLI::Int32 SWTomQueryBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /SWTOM.Query.Batch /query <meme.txt.DIR> [/out <outDIR> /SW-offset 0.6 /method <pcc> /bits.level 1.5 /minW 4 /SW-threshold 0.75 /tom-threshold 0.75 /no-HTML]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### [/no-HTML]
If this parameter is true, then only the XML result will be export.

###### Example
```bash

```
##### Accepted Types
###### /no-HTML
<h3 id="/Tom.Query"> 57. /Tom.Query</h3>


**Prototype**: ``MEME.CLI::Int32 TomQuery(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Tom.Query /query <ldm.xml/meme.txt> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost <0.7> /threshold <0.65> /meme]
```
###### Example
```bash
MEME
```
<h3 id="/Tom.Query.Batch"> 58. /Tom.Query.Batch</h3>


**Prototype**: ``MEME.CLI::Int32 TomQueryBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Tom.Query.Batch /query <inDIR> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost 0.7 /threshold <0.65>]
```
###### Example
```bash
MEME
```
<h3 id="/TomTOM"> 59. /TomTOM</h3>


**Prototype**: ``MEME.CLI::Int32 TomTOMMethod(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /TomTOM /query <meme.txt> /subject <LDM.xml> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost <0.7> /threshold <0.3>]
```
###### Example
```bash
MEME
```
<h3 id="/TomTom.LDM"> 60. /TomTom.LDM</h3>


**Prototype**: ``MEME.CLI::Int32 LDMTomTom(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /TomTom.LDM /query <ldm.xml> /subject <ldm.xml> [/out <outDIR> /method <pcc/ed/sw; default:=sw> /cost <0.7> /threshold <0.65>]
```
###### Example
```bash
MEME
```
<h3 id="/TomTOM.Similarity"> 61. /TomTOM.Similarity</h3>


**Prototype**: ``MEME.CLI::Int32 MEMEPlantSimilarity(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /TomTOM.Similarity /in <TOM_OUT.DIR> [/out <out.Csv>]
```
###### Example
```bash
MEME
```
<h3 id="/TOMTOM.Similarity.Batch"> 62. /TOMTOM.Similarity.Batch</h3>


**Prototype**: ``MEME.CLI::Int32 MEMEPlantSimilarityBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /TOMTOM.Similarity.Batch /in <inDIR> [/out <out.csv>]
```
###### Example
```bash
MEME
```
<h3 id="/TomTom.Sites.Groups"> 63. /TomTom.Sites.Groups</h3>


**Prototype**: ``MEME.CLI::Int32 ExportTOMSites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /TomTom.Sites.Groups /in <similarity.csv> /meme <meme.DIR> [/grep <regex> /out <out.DIR>]
```
###### Example
```bash
MEME
```
<h3 id="/Trim.MastSite"> 64. /Trim.MastSite</h3>


**Prototype**: ``MEME.CLI::Int32 Trim(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME /Trim.MastSite /in <mastSite.Csv> /locus <locus_tag> /correlations <DIR/name> [/out <out.csv> /cut <0.65>]
```
###### Example
```bash
MEME
```
<h3 id="--build.Regulations"> 65. --build.Regulations</h3>

Genome wide step 2
**Prototype**: ``MEME.CLI::Int32 Build(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --build.Regulations /bbh <regprecise.bbhMapped.csv> /mast <mastSites.csv> [/cutoff <0.6> /out <out.csv> /sp <spName> /DOOR <genome.opr> /DOOR.extract]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### [/DOOR.extract]
Extract the operon structure genes after assign the operon information.

###### Example
```bash

```
##### Accepted Types
###### /DOOR.extract
<h3 id="--build.Regulations.From.Motifs"> 66. --build.Regulations.From.Motifs</h3>


**Prototype**: ``MEME.CLI::Int32 BuildFromMotifSites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --build.Regulations.From.Motifs /bbh <regprecise.bbhMapped.csv> /motifs <motifSites.csv> [/cutoff <0.6> /sp <spName> /out <out.csv>]
```
###### Example
```bash
MEME
```
<h3 id="--CExpr.WGCNA"> 67. --CExpr.WGCNA</h3>


**Prototype**: ``MEME.CLI::Int32 WGCNAModsCExpr(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --CExpr.WGCNA /mods <CytoscapeNodes.txt> /genome <genome.DIR|*.PTT;*.fna> /out <DIR.out>
```
###### Example
```bash
MEME
```
<h3 id="Download.Regprecise"> 68. Download.Regprecise</h3>

Download Regprecise database from Web API
**Prototype**: ``MEME.CLI::Int32 DownloadRegprecise2(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME Download.Regprecise [/work ./ /save <saveXml>]
```
###### Example
```bash
MEME
```
<h3 id="--Dump.KEGG.Family"> 69. --Dump.KEGG.Family</h3>


**Prototype**: ``MEME.CLI::Int32 KEGGFamilyDump(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --Dump.KEGG.Family /in <in.fasta> [/out <out.csv>]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### /in
The RegPrecise formated title fasta file.

###### Example
```bash

```
##### Accepted Types
###### /in
<h3 id="--family.statics"> 70. --family.statics</h3>


**Prototype**: ``MEME.CLI::Int32 FamilyStatics(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --family.statics /sites <motifSites.csv> /mods <directory.kegg_modules>
```
###### Example
```bash
MEME
```
<h3 id="--Get.Intergenic"> 71. --Get.Intergenic</h3>


**Prototype**: ``MEME.CLI::Int32 GetIntergenic(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --Get.Intergenic /PTT <genome.ptt> /nt <genome.fasta> [/o <out.fasta> /len 100 /strict]
```
###### Example
```bash
MEME
```
<h3 id="--GetFasta"> 72. --GetFasta</h3>


**Prototype**: ``MEME.CLI::Int32 GetFasta(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --GetFasta /bbh <bbhh.csv> /id <subject_id> /regprecise <regprecise.fasta>
```
###### Example
```bash
MEME
```
<h3 id="--hits.diff"> 73. --hits.diff</h3>


**Prototype**: ``MEME.CLI::Int32 DiffHits(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --hits.diff /query <bbhh.csv> /subject <bbhh.csv> [/reverse]
```
###### Example
```bash
MEME
```
<h3 id="--Intersect.Max"> 74. --Intersect.Max</h3>


**Prototype**: ``MEME.CLI::Int32 MaxIntersection(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --Intersect.Max /query <bbhh.csv> /subject <bbhh.csv>
```
###### Example
```bash
MEME
```
<h3 id="--logo.Batch"> 75. --logo.Batch</h3>


**Prototype**: ``MEME.CLI::Int32 LogoBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --logo.Batch -in <inDIR> [/out <outDIR>]
```
###### Example
```bash
MEME
```
<h3 id="--mapped-Back"> 76. --mapped-Back</h3>


**Prototype**: ``MEME.CLI::Int32 SiteMappedBack(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --mapped-Back /meme <meme.text> /mast <mast.xml> /ptt <genome.ptt> [/out <out.csv> /offset <10> /atg-dist <250>]
```
###### Example
```bash
MEME
```
<h3 id="mast.compile"> 77. mast.compile</h3>


**Prototype**: ``MEME.CLI::Int32 CompileMast(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME mast.compile /mast <mast.xml> /ptt <genome.ptt> [/no-meme /no-regInfo /p-value 1e-3 /mast-ldm <DIR default:=GCModeller/Regprecise/MEME/MAST_LDM> /atg-dist 250]
```
###### Example
```bash
MEME
```
<h3 id="mast.compile.bulk"> 78. mast.compile.bulk</h3>

Genome wide step 1
**Prototype**: ``MEME.CLI::Int32 CompileMastBuck(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME mast.compile.bulk /source <source_dir> [/ptt <genome.ptt> /atg-dist <500> /no-meme /no-regInfo /p-value 1e-3 /mast-ldm <DIR default:=GCModeller/Regprecise/MEME/MAST_LDM> /related.all]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### [/no-meme]
Specific that the mast site construction will without and meme pwm MAST_LDM model.

###### Example
```bash

```
##### Accepted Types
###### /no-meme
<h3 id="--modules.regulates"> 79. --modules.regulates</h3>

Exports the Venn diagram model for the module regulations.
**Prototype**: ``MEME.CLI::Int32 ModuleRegulates(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --modules.regulates /in <virtualfootprints.csv> [/out <out.DIR> /mods <KEGG_modules.DIR>]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### /in
The footprints data required of fill out the pathway Class, category and type information before you call this function.
If the fields is blank, then your should specify the /mods parameter.

###### Example
```bash

```
##### Accepted Types
###### /in
<h3 id="Motif.Locates"> 80. Motif.Locates</h3>


**Prototype**: ``MEME.CLI::Int32 MotifLocites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME Motif.Locates -ptt <bacterial_genome.ptt> -meme <meme.txt> [/out <out.csv>]
```
###### Example
```bash
MEME
```
<h3 id="MotifScan"> 81. MotifScan</h3>

Scan for the motif site by using fragment similarity.
**Prototype**: ``MEME.CLI::Int32 MotifScan(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME MotifScan -nt <nt.fasta> /motif <motifLDM.xml/LDM_Name/FamilyName> [/delta <default:80> /delta2 <default:70> /offSet <default:5> /out <saved.csv>]
```
###### Example
```bash
MEME
```
<h3 id="--pathway.regulates"> 82. --pathway.regulates</h3>

Associates of the pathway regulation information for the predicted virtual footprint information.
**Prototype**: ``MEME.CLI::Int32 PathwayRegulations(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --pathway.regulates -footprints <virtualfootprint.csv> /pathway <DIR.KEGG.Pathways> [/out <./PathwayRegulations/>]
```
###### Example
```bash
MEME
```
<h3 id="Regprecise.Compile"> 83. Regprecise.Compile</h3>

The repository parameter is a directory path which is the regprecise database root directory in the GCModeller directory, if you didn't know how to set this value, please leave it blank.
**Prototype**: ``MEME.CLI::Int32 CompileRegprecise(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME Regprecise.Compile [<repository>]
```
###### Example
```bash
MEME
```
<h3 id="regulators.bbh"> 84. regulators.bbh</h3>

Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.
**Prototype**: ``MEME.CLI::Int32 RegulatorsBBh(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME regulators.bbh /bbh <bbhDIR/bbh.index.Csv> [/save <save.csv> /direct /regulons /maps <genome.gb>]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### [/regulons]
The data source of the /bbh parameter is comes from the regulons bbh data.

###### Example
```bash

```
##### Accepted Types
###### /regulons
<h3 id="regulators.compile"> 85. regulators.compile</h3>

Regprecise regulators data source compiler.
**Prototype**: ``MEME.CLI::Int32 RegulatorsCompile()``

###### Usage
```bash
MEME
```
###### Example
```bash
MEME
```
<h3 id="--site.Match"> 86. --site.Match</h3>


**Prototype**: ``MEME.CLI::Int32 SiteMatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --site.Match /meme <meme.text> /mast <mast.xml> /out <out.csv> [/ptt <genome.ptt> /len <150,200,250,300,350,400,450,500>]
```
###### Example
```bash
MEME
```



#### Parameters information:
##### [/len]
If not specific this parameter, then the function will trying to parsing the length value from the meme text automatically.

###### Example
```bash

```
##### Accepted Types
###### /len
<h3 id="--site.Matches"> 87. --site.Matches</h3>


**Prototype**: ``MEME.CLI::Int32 SiteMatches(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --site.Matches /meme <DIR.meme.text> /mast <DIR.mast.xml> /out <out.csv> [/ptt <genome.ptt>]
```
###### Example
```bash
MEME
```
<h3 id="--site.Matches.text"> 88. --site.Matches.text</h3>

Using this function for processing the meme text output from the tmod toolbox.
**Prototype**: ``MEME.CLI::Int32 SiteMatchesText(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --site.Matches.text /meme <DIR.meme.text> /mast <DIR.mast.xml> /out <out.csv> [/ptt <genome.ptt> /fasta <original.fasta.DIR>]
```
###### Example
```bash
MEME
```
<h3 id="--site.stat"> 89. --site.stat</h3>

Statics of the PCC correlation distribution of the regulation
**Prototype**: ``MEME.CLI::Int32 SiteStat(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --site.stat /in <footprints.csv> [/out <out.csv>]
```
###### Example
```bash
MEME
```
<h3 id="--TCS.Module.Regulations"> 90. --TCS.Module.Regulations</h3>


**Prototype**: ``MEME.CLI::Int32 TCSRegulateModule(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --TCS.Module.Regulations /MiST2 <MiST2.xml> /footprint <footprints.csv> /Pathways <KEGG_Pathways.DIR>
```
###### Example
```bash
MEME
```
<h3 id="--TCS.Regulations"> 91. --TCS.Regulations</h3>


**Prototype**: ``MEME.CLI::Int32 TCSRegulations(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME --TCS.Regulations /TCS <DIR.TCS.csv> /modules <DIR.mod.xml> /regulations <virtualfootprint.csv>
```
###### Example
```bash
MEME
```
<h3 id="VirtualFootprint.DIP"> 92. VirtualFootprint.DIP</h3>

Associate the dip information with the Sigma 70 virtual footprints.
**Prototype**: ``MEME.CLI::Int32 VirtualFootprintDIP(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME VirtualFootprint.DIP vf.csv <csv> dip.csv <csv>
```
###### Example
```bash
MEME
```
<h3 id="wGet.Regprecise"> 93. wGet.Regprecise</h3>

Download Regprecise database from REST API
**Prototype**: ``MEME.CLI::Int32 DownloadRegprecise(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
MEME wGet.Regprecise [/repository-export <dir.export, default: ./> /updates]
```
###### Example
```bash
MEME
```
