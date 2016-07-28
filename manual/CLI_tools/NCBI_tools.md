---
title: NCBI_tools
tags: [maunal, tools]
date: 7/27/2016 6:40:21 PM
---
# GCModeller [version 1.0.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/NCBI_tools.exe
**Root namespace**: NCBI_tools.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/Assign.Taxonomy||
|/Assign.Taxonomy.SSU||
|/Associate.Taxonomy||
|/Build_gi2taxi||
|/Export.GI||
|/gi.Match||
|/Nt.Taxonomy||

## Commands
--------------------------
##### Help for command '/Assign.Taxonomy':

**Prototype**: NCBI_tools.CLI::Int32 AssignTaxonomy(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\NCBI_tools.exe /Assign.Taxonomy /in <in.DIR> /gi <regexp> /index <fieldName> /tax <NCBI nodes/names.dmp> /gi2taxi <gi2taxi.txt/bin> [/out <out.DIR>]
  Example:      NCBI_tools /Assign.Taxonomy 
```

##### Help for command '/Assign.Taxonomy.SSU':

**Prototype**: NCBI_tools.CLI::Int32 AssignTaxonomy2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\NCBI_tools.exe /Assign.Taxonomy.SSU /in <in.DIR> /index <fieldName> /ref <SSU-ref.fasta> [/out <out.DIR>]
  Example:      NCBI_tools /Assign.Taxonomy.SSU 
```

##### Help for command '/Associate.Taxonomy':

**Prototype**: NCBI_tools.CLI::Int32 AssociateTaxonomy(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\NCBI_tools.exe /Associate.Taxonomy /in <in.DIR> /tax <ncbi_taxonomy:names,nodes> /gi2taxi <gi2taxi.bin> [/gi <nt.gi.csv> /out <out.DIR>]
  Example:      NCBI_tools /Associate.Taxonomy 
```

##### Help for command '/Build_gi2taxi':

**Prototype**: NCBI_tools.CLI::Int32 Build_gi2taxi(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\NCBI_tools.exe /Build_gi2taxi /in <gi2taxi.dmp> [/out <out.dat>]
  Example:      NCBI_tools /Build_gi2taxi 
```

##### Help for command '/Export.GI':

**Prototype**: NCBI_tools.CLI::Int32 ExportGI(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\NCBI_tools.exe /Export.GI /in <ncbi:nt.fasta> [/out <out.csv>]
  Example:      NCBI_tools /Export.GI 
```

##### Help for command '/gi.Match':

**Prototype**: NCBI_tools.CLI::Int32 giMatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\NCBI_tools.exe /gi.Match /in <nt.parts.fasta> /gi2taxid <gi2taxid.dmp> [/out <gi_match.txt>]
  Example:      NCBI_tools /gi.Match 
```

##### Help for command '/Nt.Taxonomy':

**Prototype**: NCBI_tools.CLI::Int32 NtTaxonomy(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\NCBI_tools.exe /Nt.Taxonomy /in <nt.fasta> /gi2taxi <gi2taxi.bin> /tax <ncbi_taxonomy:names,nodes> [/out <out.fasta>]
  Example:      NCBI_tools /Nt.Taxonomy 
```

