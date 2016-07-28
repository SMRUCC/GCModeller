---
title: MEME
tags: [maunal, tools]
date: 7/27/2016 6:40:19 PM
---
# GCModeller [version 1.34.0.2]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/MEME.exe
**Root namespace**: MEME.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/BBH.Select.Regulators|Select bbh result for the regulators in RegPrecise database from the regulon bbh data.|
|/Build.FamilyDb||
|/Copys||
|/Copys.DIR||
|/CORN||
|/EXPORT.MotifDraws||
|/Export.MotifSites|Motif iteration step 1|
|/Export.Regprecise.Motifs||
|/Export.Similarity.Hits|Motif iteration step 2|
|/Footprints|3 - Generates the regulation footprints.|
|/Hits.Context|2|
|/LDM.Compares||
|/LDM.MaxW||
|/LDM.Selects||
|/MAST.MotifMatches||
|/MAST.MotifMatchs.Family|1|
|/mast.Regulations||
|/MAST_LDM.Build||
|/MEME.Batch|Batch meme task by using tmod toolbox.|
|/MEME.LDMs||
|/Motif.BuildRegulons||
|/Motif.Info|Assign the phenotype information And genomic context Info for the motif sites. [SimpleSegment] -> [MotifLog]|
|/Motif.Info.Batch|[SimpleSegment] -> [MotifLog]|
|/Motif.Similarity|Export of the calculation result from the tomtom program.|
|/MotifHits.Regulation||
|/Parser.DEGs||
|/Parser.Locus||
|/Parser.Log2||
|/Parser.MAST||
|/Parser.Modules||
|/Parser.Operon||
|/Parser.Pathway||
|/Parser.RegPrecise.Operons||
|/Parser.Regulon||
|/Parser.Regulon.gb||
|/Parser.Regulon.Merged||
|/Regulator.Motifs||
|/Regulator.Motifs.Test||
|/regulon.export||
|/Regulon.Reconstruct||
|/Regulon.Reconstruct2||
|/Regulon.Reconstructs|Doing the regulon reconstruction job in batch mode.|
|/Regulon.Test||
|/RfamSites||
|/seq.logo||
|/Similarity.Union|Motif iteration step 3|
|/Site.MAST_Scan|[MAST.Xml] -> [SimpleSegment]|
|/Site.MAST_Scan.Batch|[MAST.Xml] -> [SimpleSegment]|
|/Site.RegexScan||
|/site.scan||
|/SiteHits.Footprints|Generates the regulation information.|
|/SWTOM.Compares||
|/SWTOM.Compares.Batch||
|/SWTOM.LDM||
|/SWTOM.Query||
|/SWTOM.Query.Batch||
|/Tom.Query||
|/Tom.Query.Batch||
|/TomTOM||
|/TomTom.LDM||
|/TomTOM.Similarity||
|/TOMTOM.Similarity.Batch||
|/TomTom.Sites.Groups||
|/Trim.MastSite||
|--build.Regulations|Genome wide step 2|
|--build.Regulations.From.Motifs||
|--CExpr.WGCNA||
|Download.Regprecise|Download Regprecise database from Web API|
|--Dump.KEGG.Family||
|--family.statics||
|--Get.Intergenic||
|--GetFasta||
|--hits.diff||
|--Intersect.Max||
|--logo.Batch||
|--mapped-Back||
|mast.compile||
|mast.compile.bulk|Genome wide step 1|
|--modules.regulates|Exports the Venn diagram model for the module regulations.|
|Motif.Locates||
|MotifScan|Scan for the motif site by using fragment similarity.|
|--pathway.regulates|Associates of the pathway regulation information for the predicted virtual footprint information.|
|Regprecise.Compile|The repository parameter is a directory path which is the regprecise database root directory in the GCModeller directory, if you didn't know how to set this value, please leave it blank.|
|regulators.bbh|Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.|
|regulators.compile|Regprecise regulators data source compiler.|
|--site.Match||
|--site.Matches||
|--site.Matches.text|Using this function for processing the meme text output from the tmod toolbox.|
|--site.stat|Statics of the PCC correlation distribution of the regulation|
|--TCS.Module.Regulations||
|--TCS.Regulations||
|VirtualFootprint.DIP|Associate the dip information with the Sigma 70 virtual footprints.|
|wGet.Regprecise|Download Regprecise database from REST API|

## Commands
--------------------------
##### Help for command '/BBH.Select.Regulators':

**Prototype**: MEME.CLI::Int32 SelectRegulatorsBBH(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Select bbh result for the regulators in RegPrecise database from the regulon bbh data.
  Usage:        G:\GCModeller\manual\bin\MEME.exe /BBH.Select.Regulators /in <bbh.csv> /db <regprecise_downloads.DIR> [/out <out.csv>]
  Example:      MEME /BBH.Select.Regulators 
```

##### Help for command '/Build.FamilyDb':

**Prototype**: MEME.CLI::Int32 BuildFamilyDb(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Build.FamilyDb /prot <RegPrecise.prot.fasta> /pfam <pfam-string.csv> [/out <out.Xml>]
  Example:      MEME /Build.FamilyDb 
```

##### Help for command '/Copys':

**Prototype**: MEME.CLI::Int32 BatchCopy(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Copys /in <inDIR> [/out <outDIR> /file <meme.txt>]
  Example:      MEME /Copys 
```

##### Help for command '/Copys.DIR':

**Prototype**: MEME.CLI::Int32 BatchCopyDIR(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Copys.DIR /in <inDIR> /out <outDIR>
  Example:      MEME /Copys.DIR 
```

##### Help for command '/CORN':

**Prototype**: MEME.CLI::Int32 CORN(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /CORN /in <operons.csv> /mast <mastDIR> /PTT <genome.ptt> [/out <out.csv>]
  Example:      MEME /CORN 
```

##### Help for command '/EXPORT.MotifDraws':

**Prototype**: MEME.CLI::Int32 ExportMotifDraw(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /EXPORT.MotifDraws /in <virtualFootprints.csv> /MEME <meme.txt.DIR> /KEGG <KEGG_Modules/Pathways.DIR> [/pathway /out <outDIR>]
  Example:      MEME /EXPORT.MotifDraws 
```

##### Help for command '/Export.MotifSites':

**Prototype**: MEME.CLI::Int32 ExportTestMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Motif iteration step 1
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Export.MotifSites /in <meme.txt> [/out <outDIR> /batch]
  Example:      MEME /Export.MotifSites 
```

##### Help for command '/Export.Regprecise.Motifs':

**Prototype**: MEME.CLI::Int32 ExportRegpreciseMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe 
  Example:      MEME /Export.Regprecise.Motifs 
```

##### Help for command '/Export.Similarity.Hits':

**Prototype**: MEME.CLI::Int32 LoadSimilarityHits(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Motif iteration step 2
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Export.Similarity.Hits /in <inDIR> [/out <out.Csv>]
  Example:      MEME /Export.Similarity.Hits 
```

##### Help for command '/Footprints':

**Prototype**: MEME.CLI::Int32 ToFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  3 - Generates the regulation footprints.
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Footprints /footprints <footprints.xml> /coor <name/DIR> /DOOR <genome.opr> /maps <bbhMappings.Csv> [/out <out.csv> /cuts <0.65> /extract]
  Example:      MEME /Footprints 
```



  Parameters information:
```
       [/extract]
    Description:  Extract the DOOR operon when the regulated gene is the first gene of the operon.

    Example:      /extract ""


```

#### Accepted Types
##### /extract
##### Help for command '/Hits.Context':

**Prototype**: MEME.CLI::Int32 HitContext(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  2
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Hits.Context /footprints <footprints.Xml> /PTT <genome.PTT> [/out <out.Xml> /RegPrecise <RegPrecise.Regulations.Xml>]
  Example:      MEME /Hits.Context 
```

##### Help for command '/LDM.Compares':

**Prototype**: MEME.CLI::Int32 CompareMotif(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /LDM.Compares /query <query.LDM.Xml> /sub <subject.LDM.Xml> [/out <outDIR>]
  Example:      MEME /LDM.Compares 
```

##### Help for command '/LDM.MaxW':

**Prototype**: MEME.CLI::Int32 LDMMaxLen(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /LDM.MaxW [/in <sourceDIR>]
  Example:      MEME /LDM.MaxW 
```

##### Help for command '/LDM.Selects':

**Prototype**: MEME.CLI::Int32 Selectes(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /LDM.Selects /trace <footprints.xml> /meme <memeDIR> [/out <outDIR> /named]
  Example:      MEME /LDM.Selects 
```

##### Help for command '/MAST.MotifMatches':

**Prototype**: MEME.CLI::Int32 MotifMatch2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /MAST.MotifMatches /meme <meme.txt.DIR> /mast <MAST_OUT.DIR> [/out <out.csv>]
  Example:      MEME /MAST.MotifMatches 
```

##### Help for command '/MAST.MotifMatchs.Family':

**Prototype**: MEME.CLI::Int32 MotifMatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  1
  Usage:        G:\GCModeller\manual\bin\MEME.exe /MAST.MotifMatchs.Family /meme <meme.txt.DIR> /mast <MAST_OUT.DIR> [/out <out.Xml>]
  Example:      MEME /MAST.MotifMatchs.Family 
```

##### Help for command '/mast.Regulations':

**Prototype**: MEME.CLI::Int32 MastRegulations(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /mast.Regulations /in <mastSites.Csv> /correlation <sp_name/DIR> /DOOR <DOOR.opr> [/out <footprint.csv> /cut <0.65>]
  Example:      MEME /mast.Regulations 
```

##### Help for command '/MAST_LDM.Build':

**Prototype**: MEME.CLI::Int32 BuildPWMDb(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /MAST_LDM.Build /source <sourceDIR> [/out <exportDIR:=./> /evalue <1e-3>]
  Example:      MEME /MAST_LDM.Build 
```

##### Help for command '/MEME.Batch':

**Prototype**: MEME.CLI::Int32 MEMEBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Batch meme task by using tmod toolbox.
  Usage:        G:\GCModeller\manual\bin\MEME.exe /MEME.Batch /in <inDIR> [/out <outDIR> /evalue <1> /nmotifs <30> /mod <zoops> /maxw <100>]
  Example:      MEME /MEME.Batch 
```



  Parameters information:
```
    /in
    Description:  A directory path which contains the fasta sequence for the meme motifs analysis.

    Example:      /in ""

   [/out]
    Description:  A directory path which outputs the meme.txt data to that directory.

    Example:      /out ""


```

#### Accepted Types
##### /in
##### /out
##### Help for command '/MEME.LDMs':

**Prototype**: MEME.CLI::Int32 MEME2LDM(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /MEME.LDMs /in <meme.txt> [/out <outDIR>]
  Example:      MEME /MEME.LDMs 
```

##### Help for command '/Motif.BuildRegulons':

**Prototype**: MEME.CLI::Int32 BuildRegulons(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Motif.BuildRegulons /meme <meme.txt.DIR> /model <FootprintTrace.xml> /DOOR <DOOR.opr> /maps <bbhmappings.csv> /corrs <name/DIR> [/cut <0.65> /out <outDIR>]
  Example:      MEME /Motif.BuildRegulons 
```

##### Help for command '/Motif.Info':

**Prototype**: MEME.CLI::Int32 MotifInfo(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Assign the phenotype information And genomic context Info for the motif sites. [SimpleSegment] -> [MotifLog]
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Motif.Info /loci <loci.csv> [/motifs <motifs.DIR> /gff <genome.gff> /atg-dist 250 /out <out.csv>]
  Example:      MEME /Motif.Info 
```



  Parameters information:
```
    /loci
    Description:  The motif site info data set, type Is simple segment.

    Example:      /loci ""

/motifs
    Description:  A directory which contains the motifsitelog data in the xml file format. Regulogs.Xml source directory

    Example:      /motifs ""


```

#### Accepted Types
##### /loci
##### /motifs
##### Help for command '/Motif.Info.Batch':

**Prototype**: MEME.CLI::Int32 MotifInfoBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  [SimpleSegment] -> [MotifLog]
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Motif.Info.Batch /in <sites.csv.inDIR> /gffs <gff.DIR> [/motifs <regulogs.motiflogs.MEME.DIR> /num_threads -1 /atg-dist 350 /out <out.DIR>]
  Example:      MEME /Motif.Info.Batch 
```



  Parameters information:
```
    /motifs
    Description:  Regulogs.Xml source directory

    Example:      /motifs ""

   [/num_threads]
    Description:  Default Is -1, means auto config of the threads number.

    Example:      /num_threads ""


```

#### Accepted Types
##### /motifs
##### /num_threads
##### Help for command '/Motif.Similarity':

**Prototype**: MEME.CLI::Int32 MEMETOM_MotifSimilarity(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Export of the calculation result from the tomtom program.
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Motif.Similarity /in <tomtom.DIR> /motifs <MEME_OUT.DIR> [/out <out.csv> /bp.var]
  Example:      MEME /Motif.Similarity 
```

##### Help for command '/MotifHits.Regulation':

**Prototype**: MEME.CLI::Int32 HitsRegulation(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /MotifHits.Regulation /hits <motifHits.Csv> /source <meme.txt.DIR> /PTT <genome.PTT> /correlates <sp/DIR> /bbh <bbhh.csv> [/out <out.footprints.Csv>]
  Example:      MEME /MotifHits.Regulation 
```

##### Help for command '/Parser.DEGs':

**Prototype**: MEME.CLI::Int32 ParserDEGs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Parser.DEGs /degs <deseq2.csv> /PTT <genomePTT.DIR> /door <genome.opr> /out <out.DIR> [/log-fold 2]
  Example:      MEME /Parser.DEGs 
```

##### Help for command '/Parser.Locus':

**Prototype**: MEME.CLI::Int32 ParserLocus(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Parser.Locus /locus <locus.txt> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/out <out.DIR>]
  Example:      MEME /Parser.Locus 
```

##### Help for command '/Parser.Log2':

**Prototype**: MEME.CLI::Int32 ParserLog2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Parser.Log2 /in <log2.csv> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/factor 1 /out <outDIR>]
  Example:      MEME /Parser.Log2 
```

##### Help for command '/Parser.MAST':

**Prototype**: MEME.CLI::Int32 ParserMAST(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Parser.MAST /sites <mastsites.csv> /ptt <genome-context.pttDIR> /door <genome.opr> [/out <outDIR>]
  Example:      MEME /Parser.MAST 
```

##### Help for command '/Parser.Modules':

**Prototype**: MEME.CLI::Int32 ModuleParser(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Parser.Modules /KEGG.Modules <KEGG.modules.DIR> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/locus <union/initx/locus, default:=union> /out <fasta.outDIR>]
  Example:      MEME /Parser.Modules 
```

##### Help for command '/Parser.Operon':

**Prototype**: MEME.CLI::Int32 ParserNextIterator(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Parser.Operon /in <footprint.csv> /PTT <PTTDIR> [/out <outDIR> /family /offset <50> /all]
  Example:      MEME /Parser.Operon 
```



  Parameters information:
```
       [/family]
    Description:  Group the source by family? Or output the source in one fasta set

    Example:      /family ""


```

#### Accepted Types
##### /family
##### Help for command '/Parser.Pathway':

**Prototype**: MEME.CLI::Int32 PathwayParser(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Parser.Pathway /KEGG.Pathways <KEGG.pathways.DIR> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/locus <union/initx/locus, default:=union> /out <fasta.outDIR>]
  Example:      MEME /Parser.Pathway 
```

##### Help for command '/Parser.RegPrecise.Operons':

**Prototype**: MEME.CLI::Int32 ParserRegPreciseOperon(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Parser.RegPrecise.Operons /operon <operons.Csv> /PTT <PTT_DIR> [/corn /DOOR <genome.opr> /id <null> /locus <union/initx/locus, default:=union> /out <outDIR>]
  Example:      MEME /Parser.RegPrecise.Operons 
```

##### Help for command '/Parser.Regulon':

**Prototype**: MEME.CLI::Int32 RegulonParser(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Parser.Regulon /inDIR <regulons.inDIR> /out <fasta.outDIR> /PTT <genomePTT.DIR> [/door <genome.opr>]
  Example:      MEME /Parser.Regulon 
```

##### Help for command '/Parser.Regulon.gb':

**Prototype**: MEME.CLI::Int32 RegulonParser2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Parser.Regulon.gb /inDIR <regulons.inDIR> /out <fasta.outDIR> /gb <genbank.gbk> [/door <genome.opr>]
  Example:      MEME /Parser.Regulon.gb 
```

##### Help for command '/Parser.Regulon.Merged':

**Prototype**: MEME.CLI::Int32 RegulonParser3(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Parser.Regulon.Merged /in <merged.Csv> /out <fasta.outDIR> /PTT <genomePTT.DIR> [/DOOR <genome.opr>]
  Example:      MEME /Parser.Regulon.Merged 
```

##### Help for command '/Regulator.Motifs':

**Prototype**: MEME.CLI::Int32 RegulatorMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Regulator.Motifs /bbh <bbh.csv> /regprecise <genome.DIR> [/out <outDIR>]
  Example:      MEME /Regulator.Motifs 
```

##### Help for command '/Regulator.Motifs.Test':

**Prototype**: MEME.CLI::Int32 TestRegulatorMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Regulator.Motifs.Test /hits <familyHits.Csv> /motifs <motifHits.Csv> [/out <out.csv>]
  Example:      MEME /Regulator.Motifs.Test 
```

##### Help for command '/regulon.export':

**Prototype**: MEME.CLI::Int32 ExportRegulon(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /regulon.export /in <sw-tom_out.DIR> /ref <regulon.bbh.xml.DIR> [/out <out.csv>]
  Example:      MEME /regulon.export 
```

##### Help for command '/Regulon.Reconstruct':

**Prototype**: MEME.CLI::Int32 RegulonReconstruct(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Regulon.Reconstruct /bbh <bbh.csv> /genome <RegPrecise.genome.xml> /door <operon.door> [/out <outfile.csv>]
  Example:      MEME /Regulon.Reconstruct 
```

##### Help for command '/Regulon.Reconstruct2':

**Prototype**: MEME.CLI::Int32 RegulonReconstructs2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Regulon.Reconstruct2 /bbh <bbh.csv> /genome <RegPrecise.genome.DIR> /door <operons.opr> [/out <outDIR>]
  Example:      MEME /Regulon.Reconstruct2 
```

##### Help for command '/Regulon.Reconstructs':

**Prototype**: MEME.CLI::Int32 RegulonReconstructs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Doing the regulon reconstruction job in batch mode.
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Regulon.Reconstructs /bbh <bbh_EXPORT_csv.DIR> /genome <RegPrecise.genome.DIR> [/door <operon.door> /out <outDIR>]
  Example:      MEME /Regulon.Reconstructs 
```



  Parameters information:
```
    /bbh
    Description:  A directory which contains the bbh export csv data from the localblast tool.

    Example:      /bbh ""

/genome
    Description:  The directory which contains the RegPrecise bacterial genome downloads data from the RegPrecise web server.

    Example:      /genome ""

/door
    Description:  Door file which is the prediction data of the bacterial operon.

    Example:      /door ""


```

#### Accepted Types
##### /bbh
##### /genome
##### /door
##### Help for command '/Regulon.Test':

**Prototype**: MEME.CLI::Int32 RegulonTest(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Regulon.Test /in <meme.txt> /reg <genome.bbh.regulon.xml> /bbh <maps.bbh.Csv>
  Example:      MEME /Regulon.Test 
```

##### Help for command '/RfamSites':

**Prototype**: MEME.CLI::Int32 RfamSites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /RfamSites /source <sourceDIR> [/out <out.fastaDIR>]
  Example:      MEME /RfamSites 
```

##### Help for command '/seq.logo':

**Prototype**: MEME.CLI::Int32 SequenceLogoTask(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /seq.logo /in <meme.txt> [/out <outDIR>]
  Example:      MEME /seq.logo 
```

##### Help for command '/Similarity.Union':

**Prototype**: MEME.CLI::Int32 UnionSimilarity(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Motif iteration step 3
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Similarity.Union /in <preSource.fasta.DIR> /meme <meme.txt.DIR> /hits <similarity_hist.Csv> [/out <out.DIR>]
  Example:      MEME /Similarity.Union 
```

##### Help for command '/Site.MAST_Scan':

**Prototype**: MEME.CLI::Int32 SiteMASTScan(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  [MAST.Xml] -> [SimpleSegment]
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Site.MAST_Scan /mast <mast.xml/DIR> [/batch /out <out.csv>]
  Example:      MEME /Site.MAST_Scan 
```



  Parameters information:
```
       [/batch]
    Description:  If this parameter presented in the CLI, then the parameter /mast will be used as a DIR.

    Example:      /batch ""


```

#### Accepted Types
##### /batch
##### Help for command '/Site.MAST_Scan.Batch':

**Prototype**: MEME.CLI::Int32 SiteMASTScanBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  [MAST.Xml] -> [SimpleSegment]
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Site.MAST_Scan /mast <mast.xml.DIR> [/out <out.csv.DIR> /num_threads <-1>]
  Example:      MEME /Site.MAST_Scan.Batch 
```

##### Help for command '/Site.RegexScan':

**Prototype**: MEME.CLI::Int32 SiteRegexScan(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Site.RegexScan /meme <meme.txt> /nt <nt.fasta> [/batch /out <out.csv>]
  Example:      MEME /Site.RegexScan 
```

##### Help for command '/site.scan':

**Prototype**: MEME.CLI::Int32 SiteScan(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /site.scan /query <LDM.xml> /subject <subject.fasta> [/out <outDIR>]
  Example:      MEME /site.scan 
```

##### Help for command '/SiteHits.Footprints':

**Prototype**: MEME.CLI::Int32 SiteHitsToFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Generates the regulation information.
  Usage:        G:\GCModeller\manual\bin\MEME.exe /SiteHits.Footprints /sites <MotifSiteHits.Csv> /bbh <bbh.Csv> /meme <meme.txt_DIR> /PTT <genome.PTT> /DOOR <DOOR.opr> [/queryHash /out <out.csv>]
  Example:      MEME /SiteHits.Footprints 
```

##### Help for command '/SWTOM.Compares':

**Prototype**: MEME.CLI::Int32 SWTomCompares(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /SWTOM.Compares /query <query.meme.txt> /subject <subject.meme.txt> [/out <outDIR> /no-HTML]
  Example:      MEME /SWTOM.Compares 
```

##### Help for command '/SWTOM.Compares.Batch':

**Prototype**: MEME.CLI::Int32 SWTomComparesBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /SWTOM.Compares.Batch /query <query.meme.DIR> /subject <subject.meme.DIR> [/out <outDIR> /no-HTML]
  Example:      MEME /SWTOM.Compares.Batch 
```

##### Help for command '/SWTOM.LDM':

**Prototype**: MEME.CLI::Int32 SWTomLDM(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /SWTOM.LDM /query <ldm.xml> /subject <ldm.xml> [/out <outDIR> /method <pcc/ed/sw; default:=pcc>]
  Example:      MEME /SWTOM.LDM 
```

##### Help for command '/SWTOM.Query':

**Prototype**: MEME.CLI::Int32 SWTomQuery(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /SWTOM.Query /query <meme.txt> [/out <outDIR> /method <pcc> /bits.level 1.6 /minW 6 /no-HTML]
  Example:      MEME /SWTOM.Query 
```



  Parameters information:
```
       [/no-HTML]
    Description:  If this parameter is true, then only the XML result will be export.

    Example:      /no-HTML ""


```

#### Accepted Types
##### /no-HTML
##### Help for command '/SWTOM.Query.Batch':

**Prototype**: MEME.CLI::Int32 SWTomQueryBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /SWTOM.Query.Batch /query <meme.txt.DIR> [/out <outDIR> /SW-offset 0.6 /method <pcc> /bits.level 1.5 /minW 4 /SW-threshold 0.75 /tom-threshold 0.75 /no-HTML]
  Example:      MEME /SWTOM.Query.Batch 
```



  Parameters information:
```
       [/no-HTML]
    Description:  If this parameter is true, then only the XML result will be export.

    Example:      /no-HTML ""


```

#### Accepted Types
##### /no-HTML
##### Help for command '/Tom.Query':

**Prototype**: MEME.CLI::Int32 TomQuery(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Tom.Query /query <ldm.xml/meme.txt> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost <0.7> /threshold <0.65> /meme]
  Example:      MEME /Tom.Query 
```

##### Help for command '/Tom.Query.Batch':

**Prototype**: MEME.CLI::Int32 TomQueryBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Tom.Query.Batch /query <inDIR> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost 0.7 /threshold <0.65>]
  Example:      MEME /Tom.Query.Batch 
```

##### Help for command '/TomTOM':

**Prototype**: MEME.CLI::Int32 TomTOMMethod(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /TomTOM /query <meme.txt> /subject <LDM.xml> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost <0.7> /threshold <0.3>]
  Example:      MEME /TomTOM 
```

##### Help for command '/TomTom.LDM':

**Prototype**: MEME.CLI::Int32 LDMTomTom(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /TomTom.LDM /query <ldm.xml> /subject <ldm.xml> [/out <outDIR> /method <pcc/ed/sw; default:=sw> /cost <0.7> /threshold <0.65>]
  Example:      MEME /TomTom.LDM 
```

##### Help for command '/TomTOM.Similarity':

**Prototype**: MEME.CLI::Int32 MEMEPlantSimilarity(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /TomTOM.Similarity /in <TOM_OUT.DIR> [/out <out.Csv>]
  Example:      MEME /TomTOM.Similarity 
```

##### Help for command '/TOMTOM.Similarity.Batch':

**Prototype**: MEME.CLI::Int32 MEMEPlantSimilarityBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /TOMTOM.Similarity.Batch /in <inDIR> [/out <out.csv>]
  Example:      MEME /TOMTOM.Similarity.Batch 
```

##### Help for command '/TomTom.Sites.Groups':

**Prototype**: MEME.CLI::Int32 ExportTOMSites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /TomTom.Sites.Groups /in <similarity.csv> /meme <meme.DIR> [/grep <regex> /out <out.DIR>]
  Example:      MEME /TomTom.Sites.Groups 
```

##### Help for command '/Trim.MastSite':

**Prototype**: MEME.CLI::Int32 Trim(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe /Trim.MastSite /in <mastSite.Csv> /locus <locus_tag> /correlations <DIR/name> [/out <out.csv> /cut <0.65>]
  Example:      MEME /Trim.MastSite 
```

##### Help for command '--build.Regulations':

**Prototype**: MEME.CLI::Int32 Build(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Genome wide step 2
  Usage:        G:\GCModeller\manual\bin\MEME.exe --build.Regulations /bbh <regprecise.bbhMapped.csv> /mast <mastSites.csv> [/cutoff <0.6> /out <out.csv> /sp <spName> /DOOR <genome.opr> /DOOR.extract]
  Example:      MEME --build.Regulations 
```



  Parameters information:
```
       [/DOOR.extract]
    Description:  Extract the operon structure genes after assign the operon information.

    Example:      /DOOR.extract ""


```

#### Accepted Types
##### /DOOR.extract
##### Help for command '--build.Regulations.From.Motifs':

**Prototype**: MEME.CLI::Int32 BuildFromMotifSites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --build.Regulations.From.Motifs /bbh <regprecise.bbhMapped.csv> /motifs <motifSites.csv> [/cutoff <0.6> /sp <spName> /out <out.csv>]
  Example:      MEME --build.Regulations.From.Motifs 
```

##### Help for command '--CExpr.WGCNA':

**Prototype**: MEME.CLI::Int32 WGCNAModsCExpr(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --CExpr.WGCNA /mods <CytoscapeNodes.txt> /genome <genome.DIR|*.PTT;*.fna> /out <DIR.out>
  Example:      MEME --CExpr.WGCNA 
```

##### Help for command 'Download.Regprecise':

**Prototype**: MEME.CLI::Int32 DownloadRegprecise2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Download Regprecise database from Web API
  Usage:        G:\GCModeller\manual\bin\MEME.exe Download.Regprecise [/work ./ /save <saveXml>]
  Example:      MEME Download.Regprecise 
```

##### Help for command '--Dump.KEGG.Family':

**Prototype**: MEME.CLI::Int32 KEGGFamilyDump(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --Dump.KEGG.Family /in <in.fasta> [/out <out.csv>]
  Example:      MEME --Dump.KEGG.Family 
```



  Parameters information:
```
    /in
    Description:  The RegPrecise formated title fasta file.

    Example:      /in ""


```

#### Accepted Types
##### /in
##### Help for command '--family.statics':

**Prototype**: MEME.CLI::Int32 FamilyStatics(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --family.statics /sites <motifSites.csv> /mods <directory.kegg_modules>
  Example:      MEME --family.statics 
```

##### Help for command '--Get.Intergenic':

**Prototype**: MEME.CLI::Int32 GetIntergenic(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --Get.Intergenic /PTT <genome.ptt> /nt <genome.fasta> [/o <out.fasta> /len 100 /strict]
  Example:      MEME --Get.Intergenic 
```

##### Help for command '--GetFasta':

**Prototype**: MEME.CLI::Int32 GetFasta(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --GetFasta /bbh <bbhh.csv> /id <subject_id> /regprecise <regprecise.fasta>
  Example:      MEME --GetFasta 
```

##### Help for command '--hits.diff':

**Prototype**: MEME.CLI::Int32 DiffHits(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --hits.diff /query <bbhh.csv> /subject <bbhh.csv> [/reverse]
  Example:      MEME --hits.diff 
```

##### Help for command '--Intersect.Max':

**Prototype**: MEME.CLI::Int32 MaxIntersection(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --Intersect.Max /query <bbhh.csv> /subject <bbhh.csv>
  Example:      MEME --Intersect.Max 
```

##### Help for command '--logo.Batch':

**Prototype**: MEME.CLI::Int32 LogoBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --logo.Batch -in <inDIR> [/out <outDIR>]
  Example:      MEME --logo.Batch 
```

##### Help for command '--mapped-Back':

**Prototype**: MEME.CLI::Int32 SiteMappedBack(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --mapped-Back /meme <meme.text> /mast <mast.xml> /ptt <genome.ptt> [/out <out.csv> /offset <10> /atg-dist <250>]
  Example:      MEME --mapped-Back 
```

##### Help for command 'mast.compile':

**Prototype**: MEME.CLI::Int32 CompileMast(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe mast.compile /mast <mast.xml> /ptt <genome.ptt> [/no-meme /no-regInfo /p-value 1e-3 /mast-ldm <DIR default:=GCModeller/Regprecise/MEME/MAST_LDM> /atg-dist 250]
  Example:      MEME mast.compile 
```

##### Help for command 'mast.compile.bulk':

**Prototype**: MEME.CLI::Int32 CompileMastBuck(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Genome wide step 1
  Usage:        G:\GCModeller\manual\bin\MEME.exe mast.compile.bulk /source <source_dir> [/ptt <genome.ptt> /atg-dist <500> /no-meme /no-regInfo /p-value 1e-3 /mast-ldm <DIR default:=GCModeller/Regprecise/MEME/MAST_LDM> /related.all]
  Example:      MEME mast.compile.bulk 
```



  Parameters information:
```
       [/no-meme]
    Description:  Specific that the mast site construction will without and meme pwm MAST_LDM model.

    Example:      /no-meme ""


```

#### Accepted Types
##### /no-meme
##### Help for command '--modules.regulates':

**Prototype**: MEME.CLI::Int32 ModuleRegulates(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Exports the Venn diagram model for the module regulations.
  Usage:        G:\GCModeller\manual\bin\MEME.exe --modules.regulates /in <virtualfootprints.csv> [/out <out.DIR> /mods <KEGG_modules.DIR>]
  Example:      MEME --modules.regulates 
```



  Parameters information:
```
    /in
    Description:  The footprints data required of fill out the pathway Class, category and type information before you call this function.
                   If the fields is blank, then your should specify the /mods parameter.

    Example:      /in ""


```

#### Accepted Types
##### /in
##### Help for command 'Motif.Locates':

**Prototype**: MEME.CLI::Int32 MotifLocites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe Motif.Locates -ptt <bacterial_genome.ptt> -meme <meme.txt> [/out <out.csv>]
  Example:      MEME Motif.Locates 
```

##### Help for command 'MotifScan':

**Prototype**: MEME.CLI::Int32 MotifScan(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Scan for the motif site by using fragment similarity.
  Usage:        G:\GCModeller\manual\bin\MEME.exe MotifScan -nt <nt.fasta> /motif <motifLDM.xml/LDM_Name/FamilyName> [/delta <default:80> /delta2 <default:70> /offSet <default:5> /out <saved.csv>]
  Example:      MEME MotifScan 
```

##### Help for command '--pathway.regulates':

**Prototype**: MEME.CLI::Int32 PathwayRegulations(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Associates of the pathway regulation information for the predicted virtual footprint information.
  Usage:        G:\GCModeller\manual\bin\MEME.exe --pathway.regulates -footprints <virtualfootprint.csv> /pathway <DIR.KEGG.Pathways> [/out <./PathwayRegulations/>]
  Example:      MEME --pathway.regulates 
```

##### Help for command 'Regprecise.Compile':

**Prototype**: MEME.CLI::Int32 CompileRegprecise(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  The repository parameter is a directory path which is the regprecise database root directory in the GCModeller directory, if you didn't know how to set this value, please leave it blank.
  Usage:        G:\GCModeller\manual\bin\MEME.exe Regprecise.Compile [<repository>]
  Example:      MEME Regprecise.Compile 
```

##### Help for command 'regulators.bbh':

**Prototype**: MEME.CLI::Int32 RegulatorsBBh(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.
  Usage:        G:\GCModeller\manual\bin\MEME.exe regulators.bbh /bbh <bbhDIR/bbh.index.Csv> [/save <save.csv> /direct /regulons /maps <genome.gb>]
  Example:      MEME regulators.bbh 
```



  Parameters information:
```
       [/regulons]
    Description:  The data source of the /bbh parameter is comes from the regulons bbh data.

    Example:      /regulons ""


```

#### Accepted Types
##### /regulons
##### Help for command 'regulators.compile':

**Prototype**: MEME.CLI::Int32 RegulatorsCompile()

```
  Information:  Regprecise regulators data source compiler.
  Usage:        G:\GCModeller\manual\bin\MEME.exe 
  Example:      MEME regulators.compile 
```

##### Help for command '--site.Match':

**Prototype**: MEME.CLI::Int32 SiteMatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --site.Match /meme <meme.text> /mast <mast.xml> /out <out.csv> [/ptt <genome.ptt> /len <150,200,250,300,350,400,450,500>]
  Example:      MEME --site.Match 
```



  Parameters information:
```
       [/len]
    Description:  If not specific this parameter, then the function will trying to parsing the length value from the meme text automatically.

    Example:      /len ""


```

#### Accepted Types
##### /len
##### Help for command '--site.Matches':

**Prototype**: MEME.CLI::Int32 SiteMatches(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --site.Matches /meme <DIR.meme.text> /mast <DIR.mast.xml> /out <out.csv> [/ptt <genome.ptt>]
  Example:      MEME --site.Matches 
```

##### Help for command '--site.Matches.text':

**Prototype**: MEME.CLI::Int32 SiteMatchesText(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Using this function for processing the meme text output from the tmod toolbox.
  Usage:        G:\GCModeller\manual\bin\MEME.exe --site.Matches.text /meme <DIR.meme.text> /mast <DIR.mast.xml> /out <out.csv> [/ptt <genome.ptt> /fasta <original.fasta.DIR>]
  Example:      MEME --site.Matches.text 
```

##### Help for command '--site.stat':

**Prototype**: MEME.CLI::Int32 SiteStat(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Statics of the PCC correlation distribution of the regulation
  Usage:        G:\GCModeller\manual\bin\MEME.exe --site.stat /in <footprints.csv> [/out <out.csv>]
  Example:      MEME --site.stat 
```

##### Help for command '--TCS.Module.Regulations':

**Prototype**: MEME.CLI::Int32 TCSRegulateModule(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --TCS.Module.Regulations /MiST2 <MiST2.xml> /footprint <footprints.csv> /Pathways <KEGG_Pathways.DIR>
  Example:      MEME --TCS.Module.Regulations 
```

##### Help for command '--TCS.Regulations':

**Prototype**: MEME.CLI::Int32 TCSRegulations(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\MEME.exe --TCS.Regulations /TCS <DIR.TCS.csv> /modules <DIR.mod.xml> /regulations <virtualfootprint.csv>
  Example:      MEME --TCS.Regulations 
```

##### Help for command 'VirtualFootprint.DIP':

**Prototype**: MEME.CLI::Int32 VirtualFootprintDIP(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Associate the dip information with the Sigma 70 virtual footprints.
  Usage:        G:\GCModeller\manual\bin\MEME.exe VirtualFootprint.DIP vf.csv <csv> dip.csv <csv>
  Example:      MEME VirtualFootprint.DIP 
```

##### Help for command 'wGet.Regprecise':

**Prototype**: MEME.CLI::Int32 DownloadRegprecise(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Download Regprecise database from REST API
  Usage:        G:\GCModeller\manual\bin\MEME.exe wGet.Regprecise [/repository-export <dir.export, default: ./> /updates]
  Example:      MEME wGet.Regprecise 
```

