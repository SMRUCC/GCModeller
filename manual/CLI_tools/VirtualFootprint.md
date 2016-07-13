---
title: VirtualFootprint
tags: [maunal, tools]
date: 7/7/2016 6:52:18 PM
---
# GCModeller [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/VirtualFootprint.exe
**Root namespace**: xVirtualFootprint.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/Build.Footprints|Build regulations from motif log site.|
|/Density.Mappings||
|/Export.Footprints.Sites|Exports the motif sites from the virtual footprints sites.|
|/Export.Primer|[SSR name], [Forward primer], [Reverse primer]|
|/Filter.Promoter.Sites||
|/Filter.PromoterSites.Batch||
|/Intersect||
|/KEGG.Regulons||
|/Logs.Cast.Footprints||
|/MAST_Sites.Screen||
|/MAST_Sites.Screen2||
|/Merge.Footprints||
|/Merge.Regulons||
|/Merge.Sites|Merge the segment loci sites within the specific length offset ranges.|
|/Motif.From.MAL||
|/restrict_enzyme.builds||
|/scan|Sanning genome sequence with a specific motif meme model.|
|/Sites.Pathways|[Type 1] Grouping sites loci by pathway|
|/Sites.Regulons|[Type 2]|
|/Test.Footprints||
|/Test.Footprints.2||
|/TF.Density||
|/TF.Density.Batch||
|/TF.Regulons||
|/TF.Sites||
|/Trim.Regulates||
|/Trim.Regulons||
|/Trim.Strand|Removes all of the sites which is on the different strand with the tag gene.|
|/Write.Network||

## Commands
--------------------------
##### Help for command '/Build.Footprints':

**Prototype**: xVirtualFootprint.CLI::Int32 BuildFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Build regulations from motif log site.
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Build.Footprints /motifs <motifLogs.csv> /bbh <queryHits.csv> [/hitshash /sites <motifLogSites.Xml.DIR> /out <out.csv>]
  Example:      VirtualFootprint /Build.Footprints 
```



  Parameters information:
```
    /bbh
    Description:  The bbh hit result between the RegPrecise database and annotated genome proteins. query should be the RegPrecise TF and hits should be the annotated proteins.

    Example:      /bbh ""

   [/sites]
    Description:  If this parameter not presented, then using GCModeller repository data as default.

    Example:      /sites ""

   [/hitshash]
    Description:  Using hit name as the bbh hash index key? default is using query name.

    Example:      /hitshash ""


```

#### Accepted Types
##### /bbh
##### /sites
##### /hitshash
##### Help for command '/Density.Mappings':

**Prototype**: xVirtualFootprint.CLI::Int32 ContextMappings(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Density.Mappings /in <density.Csv> [/scale 100 /out <out.PTT>]
  Example:      VirtualFootprint /Density.Mappings 
```

##### Help for command '/Export.Footprints.Sites':

**Prototype**: xVirtualFootprint.CLI::Int32 ExportFasta(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Exports the motif sites from the virtual footprints sites.
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Export.Footprints.Sites /in <virtualfootprints> [/TF <locus_tag> /offset <group-offset> /out <outDIR/fasta>]
  Example:      VirtualFootprint /Export.Footprints.Sites 
```

##### Help for command '/Export.Primer':

**Prototype**: xVirtualFootprint.CLI::Int32 ExportPrimer(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  [SSR name], [Forward primer], [Reverse primer]
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Export.Primer /in <primer.csv/DIR> [/out <out.DIR> /batch]
  Example:      VirtualFootprint /Export.Primer 
```

##### Help for command '/Filter.Promoter.Sites':

**Prototype**: xVirtualFootprint.CLI::Int32 PromoterSites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Filter.Promoter.Sites /in <motifLog.Csv> [/out <out.csv>]
  Example:      VirtualFootprint /Filter.Promoter.Sites 
```

##### Help for command '/Filter.PromoterSites.Batch':

**Prototype**: xVirtualFootprint.CLI::Int32 PromoterSitesBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Filter.PromoterSites.Batch /in <motifLogs.DIR> [/num_threads <-1> /out <out.DIR>]
  Example:      VirtualFootprint /Filter.PromoterSites.Batch 
```

##### Help for command '/Intersect':

**Prototype**: xVirtualFootprint.CLI::Int32 Intersection(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Intersect /s1 <footprints.csv> /s2 <footprints.csv> [/out <out.csv> /strict]
  Example:      VirtualFootprint /Intersect 
```

##### Help for command '/KEGG.Regulons':

**Prototype**: xVirtualFootprint.CLI::Int32 KEGGRegulons(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /KEGG.Regulons /in <footprints.csv> /mods <KEGG.mods.DIR> [/pathway /out <out.csv>]
  Example:      VirtualFootprint /KEGG.Regulons 
```

##### Help for command '/Logs.Cast.Footprints':

**Prototype**: xVirtualFootprint.CLI::Int32 CastLogAsFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Logs.Cast.Footprints /in <motifLogs.Csv> [/out <out.csv>]
  Example:      VirtualFootprint /Logs.Cast.Footprints 
```

##### Help for command '/MAST_Sites.Screen':

**Prototype**: xVirtualFootprint.CLI::Int32 SiteScreens(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /MAST_Sites.Screen /in <mast_sites.csv> /operons <regprecise.operons.csv> [/out <out.csv>]
  Example:      VirtualFootprint /MAST_Sites.Screen 
```

##### Help for command '/MAST_Sites.Screen2':

**Prototype**: xVirtualFootprint.CLI::Int32 SiteScreens2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /MAST_Sites.Screen2 /in <mast_sites.csv> [/n <2> /offset <30> /out <out.csv>]
  Example:      VirtualFootprint /MAST_Sites.Screen2 
```

##### Help for command '/Merge.Footprints':

**Prototype**: xVirtualFootprint.CLI::Int32 MergeFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Merge.Footprints /in <inDIR> [/out <out.csv> /trim]
  Example:      VirtualFootprint /Merge.Footprints 
```

##### Help for command '/Merge.Regulons':

**Prototype**: xVirtualFootprint.CLI::Int32 MergeRegulonsExport(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Merge.Regulons /in <regulons.bbh.inDIR> [/out <out.csv>]
  Example:      VirtualFootprint /Merge.Regulons 
```

##### Help for command '/Merge.Sites':

**Prototype**: xVirtualFootprint.CLI::Int32 MergeSites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Merge the segment loci sites within the specific length offset ranges.
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Merge.Sites /in <segments.Csv> [/nt <nt.fasta> /out <out.csv> /offset <10>]
  Example:      VirtualFootprint /Merge.Sites 
```

##### Help for command '/Motif.From.MAL':

**Prototype**: xVirtualFootprint.CLI::Int32 MotifFromMAL(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Motif.From.MAL /in <clustal.fasta> /out <outDIR>
  Example:      VirtualFootprint /Motif.From.MAL 
```

##### Help for command '/restrict_enzyme.builds':

**Prototype**: xVirtualFootprint.CLI::Int32 BuildEnzymeDb(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /restrict_enzyme.builds [/source <html_DIR> /out <out.json>]
  Example:      VirtualFootprint /restrict_enzyme.builds 
```



  Parameters information:
```
       [/source]
    Description:  Default using the data source from Wikipedia.

    Example:      /source ""

   [/out]
    Description:  Enzyme database was writing to the GCModeller repository by default.

    Example:      /out ""


```

#### Accepted Types
##### /source
##### /out
##### Help for command '/scan':

**Prototype**: xVirtualFootprint.CLI::Int32 Scanner(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Sanning genome sequence with a specific motif meme model.
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /scan /motif <meme.txt> /nt <genome.fasta> [/PTT <genome.ptt> /atg-dist <250> /out <out.csv>]
  Example:      VirtualFootprint /scan 
```

##### Help for command '/Sites.Pathways':

**Prototype**: xVirtualFootprint.CLI::Int32 PathwaySites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  [Type 1] Grouping sites loci by pathway
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Sites.Pathways /pathway <KEGG.DIR> /sites <simple_segment.Csv.DIR> [/out <out.DIR>]
  Example:      VirtualFootprint /Sites.Pathways 
```

##### Help for command '/Sites.Regulons':

**Prototype**: xVirtualFootprint.CLI::Int32 RegulonSites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  [Type 2]
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Sites.Regulons /regulon <RegPrecise.Regulon.Csv> /sites <simple_segment.Csv.DIR> [/map <genome.PTT> /out <out.DIR>]
  Example:      VirtualFootprint /Sites.Regulons 
```

##### Help for command '/Test.Footprints':

**Prototype**: xVirtualFootprint.CLI::Int32 TestFootprints(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Test.Footprints /in <virtualfootprints.csv> /opr <regulon-operons.csv> [/out <out.csv>]
  Example:      VirtualFootprint /Test.Footprints 
```

##### Help for command '/Test.Footprints.2':

**Prototype**: xVirtualFootprint.CLI::Int32 TestFootprints2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Test.Footprints.2 /in <virtualfootprints.csv> [/out <out.csv> /n 2]
  Example:      VirtualFootprint /Test.Footprints.2 
```

##### Help for command '/TF.Density':

**Prototype**: xVirtualFootprint.CLI::Int32 TFDensity(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /TF.Density /TF <TF-list.txt> /PTT <genome.PTT> [/ranges 5000 /out <out.csv> /cis /un-strand /batch]
  Example:      VirtualFootprint /TF.Density 
```



  Parameters information:
```
    /TF
    Description:  A plant text file with the TF locus_tag list.

    Example:      /TF ""

   [/batch]
    Description:  This function is works in batch mode.

    Example:      /batch ""


```

#### Accepted Types
##### /TF
##### /batch
##### Help for command '/TF.Density.Batch':

**Prototype**: xVirtualFootprint.CLI::Int32 TFDensityBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /TF.Density.Batch /TF <TF-list.txt> /PTT <genome.PTT.DIR> [/ranges 5000 /out <out.DIR> /cis /un-strand]
  Example:      VirtualFootprint /TF.Density.Batch 
```

##### Help for command '/TF.Regulons':

**Prototype**: xVirtualFootprint.CLI::Int32 TFRegulons(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /TF.Regulons /bbh <tf.bbh.csv> /footprints <regulations.csv> [/out <out.csv>]
  Example:      VirtualFootprint /TF.Regulons 
```

##### Help for command '/TF.Sites':

**Prototype**: xVirtualFootprint.CLI::Int32 TFMotifSites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /TF.Sites /bbh <bbh.Csv> /RegPrecise <RegPrecise.Xmls.DIR> [/hitHash /out <outDIR>]
  Example:      VirtualFootprint /TF.Sites 
```

##### Help for command '/Trim.Regulates':

**Prototype**: xVirtualFootprint.CLI::Int32 TrimRegulates(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Trim.Regulates /in <virtualfootprint.csv> [/out <out.csv> /cut 0.65]
  Example:      VirtualFootprint /Trim.Regulates 
```

##### Help for command '/Trim.Regulons':

**Prototype**: xVirtualFootprint.CLI::Int32 TrimRegulon(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Trim.Regulons /in <regulons.csv> /pcc <pccDIR/sp_code> [/out <out.csv> /cut 0.65]
  Example:      VirtualFootprint /Trim.Regulons 
```

##### Help for command '/Trim.Strand':

**Prototype**: xVirtualFootprint.CLI::Int32 TrimStrand(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Removes all of the sites which is on the different strand with the tag gene.
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Trim.Strand /in <segments.Csv> /PTT <genome.ptt> [/out <out.csv>]
  Example:      VirtualFootprint /Trim.Strand 
```

##### Help for command '/Write.Network':

**Prototype**: xVirtualFootprint.CLI::Int32 SaveNetwork(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\VirtualFootprint.exe /Write.Network /in <regulons.csv> [/out <netDIR>]
  Example:      VirtualFootprint /Write.Network 
```

