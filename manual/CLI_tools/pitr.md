---
title: pitr
tags: [maunal, tools]
date: 7/7/2016 6:51:47 PM
---
# ProteinInteraction [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/pitr.exe
**Root namespace**: ProteinTools.Interactions.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|--align.LDM||
|--Contacts||
|--CrossTalks.Probability||
|--Db.From.Exists||
|--domain.Interactions||
|--interact.TCS||
|--Merge.Pfam||
|--predicts.TCS||
|--Profiles.Create||
|--ProtFasta.Downloads||
|--ProtFasta.Downloads.Batch||
|--signature||
|--SwissTCS.Downloads||

## Commands
--------------------------
##### Help for command '--align.LDM':

**Prototype**: ProteinTools.Interactions.CLI::Int32 GenerateModel(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --align.LDM /in <source.fasta>
  Example:      pitr --align.LDM 
```

##### Help for command '--Contacts':

**Prototype**: ProteinTools.Interactions.CLI::Int32 Contacts(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --Contacts /in <in.DIR>
  Example:      pitr --Contacts 
```

##### Help for command '--CrossTalks.Probability':

**Prototype**: ProteinTools.Interactions.CLI::Int32 CrossTalksCal(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --CrossTalks.Probability /query <pfam-string.csv> /swiss <swissTCS_pfam-string.csv> [/out <out.CrossTalks.csv> /test <queryName>]
  Example:      pitr --CrossTalks.Probability 
```

##### Help for command '--Db.From.Exists':

**Prototype**: ProteinTools.Interactions.CLI::Int32 DbMergeFromExists(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --Db.From.Exists /aln <clustal-aln.DIR> /pfam <pfam-string.csv>
  Example:      pitr --Db.From.Exists 
```

##### Help for command '--domain.Interactions':

**Prototype**: ProteinTools.Interactions.CLI::Int32 DomainInteractions(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --domain.Interactions /pfam <pfam-string.csv> /swissTCS <swissTCS.DIR>
  Example:      pitr --domain.Interactions 
```

##### Help for command '--interact.TCS':

**Prototype**: ProteinTools.Interactions.CLI::Int32 TCSParser(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --interact.TCS /door <door.opr> /MiST2 <mist2.xml> /swiss <tcs.csv.DIR> /out <out.DIR>
  Example:      pitr --interact.TCS 
```

##### Help for command '--Merge.Pfam':

**Prototype**: ProteinTools.Interactions.CLI::Int32 MergePfam(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --Merge.Pfam /in <in.DIR>
  Example:      pitr --Merge.Pfam 
```

##### Help for command '--predicts.TCS':

**Prototype**: ProteinTools.Interactions.CLI::Int32 Predicts(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --predicts.TCS /pfam <pfam-string.csv> /prot <prot.fasta> /Db <interaction.xml>
  Example:      pitr --predicts.TCS 
```

##### Help for command '--Profiles.Create':

**Prototype**: ProteinTools.Interactions.CLI::Int32 CreateProfiles(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --Profiles.Create /MiST2 <MiST2.xml> /pfam <pfam-string.csv> [/out <out.csv>]
  Example:      pitr --Profiles.Create 
```

##### Help for command '--ProtFasta.Downloads':

**Prototype**: ProteinTools.Interactions.CLI::Int32 ProtFastaDownloads(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --ProtFasta.Downloads /in <sp.DIR>
  Example:      pitr --ProtFasta.Downloads 
```

##### Help for command '--ProtFasta.Downloads.Batch':

**Prototype**: ProteinTools.Interactions.CLI::Int32 ProtFastaDownloadsBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --ProtFasta.Downloads.Batch /in <sp.DIR.Source>
  Example:      pitr --ProtFasta.Downloads.Batch 
```

##### Help for command '--signature':

**Prototype**: ProteinTools.Interactions.CLI::Int32 SignatureGenerates(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --signature /in <aln.fasta> [/p-cut <0.95>]
  Example:      pitr --signature 
```

##### Help for command '--SwissTCS.Downloads':

**Prototype**: ProteinTools.Interactions.CLI::Int32 DownloadEntireDb(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\pitr.exe --SwissTCS.Downloads /out <out.DIR>
  Example:      pitr --SwissTCS.Downloads 
```

