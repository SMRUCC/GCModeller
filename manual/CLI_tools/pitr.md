---
title: pitr
tags: [maunal, tools]
date: 11/24/2016 2:54:15 AM
---
# ProteinInteraction [version 1.0.0.0]
> Tools for analysis the protein interaction relationship.

<!--more-->

**ProteinInteraction**<br/>
__<br/>
Copyright © ???????????? 2014

**Module AssemblyName**: file:///G:/GCModeller/GCModeller/bin/pitr.exe<br/>
**Root namespace**: ``ProteinTools.Interactions.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/BioGRID.Id.types](#/BioGRID.Id.types)||
|[/BioGRID.selects](#/BioGRID.selects)||
|[/STRING.selects](#/STRING.selects)||
|[--align.LDM](#--align.LDM)||
|[--Contacts](#--Contacts)||
|[--CrossTalks.Probability](#--CrossTalks.Probability)||
|[--Db.From.Exists](#--Db.From.Exists)||
|[--domain.Interactions](#--domain.Interactions)||
|[--interact.TCS](#--interact.TCS)||
|[--Merge.Pfam](#--Merge.Pfam)||
|[--predicts.TCS](#--predicts.TCS)||
|[--Profiles.Create](#--Profiles.Create)||
|[--ProtFasta.Downloads](#--ProtFasta.Downloads)||
|[--ProtFasta.Downloads.Batch](#--ProtFasta.Downloads.Batch)||
|[--signature](#--signature)||
|[--SwissTCS.Downloads](#--SwissTCS.Downloads)||

## CLI API list
--------------------------
<h3 id="/BioGRID.Id.types"> 1. /BioGRID.Id.types</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 BioGridIdTypes(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr /BioGRID.Id.types /in <BIOGRID-IDENTIFIERS.tsv> [/out <out.txt>]
```
<h3 id="/BioGRID.selects"> 2. /BioGRID.selects</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 BioGRIDSelects(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr /BioGRID.selects /in <in.DIR/*.Csv> /key <GeneId> /links <BioGRID-links.mitab.txt> [/out <out.DIR/*.Csv>]
```
<h3 id="/STRING.selects"> 3. /STRING.selects</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 STRINGSelects(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr /STRING.selects /in <in.DIR/*.Csv> /key <GeneId> /links <links.txt> /maps <maps_id.tsv> [/out <out.DIR/*.Csv>]
```
<h3 id="--align.LDM"> 4. --align.LDM</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 GenerateModel(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --align.LDM /in <source.fasta>
```
<h3 id="--Contacts"> 5. --Contacts</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 Contacts(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --Contacts /in <in.DIR>
```
<h3 id="--CrossTalks.Probability"> 6. --CrossTalks.Probability</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 CrossTalksCal(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --CrossTalks.Probability /query <pfam-string.csv> /swiss <swissTCS_pfam-string.csv> [/out <out.CrossTalks.csv> /test <queryName>]
```
<h3 id="--Db.From.Exists"> 7. --Db.From.Exists</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 DbMergeFromExists(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --Db.From.Exists /aln <clustal-aln.DIR> /pfam <pfam-string.csv>
```
<h3 id="--domain.Interactions"> 8. --domain.Interactions</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 DomainInteractions(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --domain.Interactions /pfam <pfam-string.csv> /swissTCS <swissTCS.DIR>
```
<h3 id="--interact.TCS"> 9. --interact.TCS</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 TCSParser(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --interact.TCS /door <door.opr> /MiST2 <mist2.xml> /swiss <tcs.csv.DIR> /out <out.DIR>
```
<h3 id="--Merge.Pfam"> 10. --Merge.Pfam</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 MergePfam(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --Merge.Pfam /in <in.DIR>
```
<h3 id="--predicts.TCS"> 11. --predicts.TCS</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 Predicts(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --predicts.TCS /pfam <pfam-string.csv> /prot <prot.fasta> /Db <interaction.xml>
```
<h3 id="--Profiles.Create"> 12. --Profiles.Create</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 CreateProfiles(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --Profiles.Create /MiST2 <MiST2.xml> /pfam <pfam-string.csv> [/out <out.csv>]
```
<h3 id="--ProtFasta.Downloads"> 13. --ProtFasta.Downloads</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 ProtFastaDownloads(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --ProtFasta.Downloads /in <sp.DIR>
```
<h3 id="--ProtFasta.Downloads.Batch"> 14. --ProtFasta.Downloads.Batch</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 ProtFastaDownloadsBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --ProtFasta.Downloads.Batch /in <sp.DIR.Source>
```
<h3 id="--signature"> 15. --signature</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 SignatureGenerates(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --signature /in <aln.fasta> [/p-cut <0.95>]
```
<h3 id="--SwissTCS.Downloads"> 16. --SwissTCS.Downloads</h3>


**Prototype**: ``ProteinTools.Interactions.CLI::Int32 DownloadEntireDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
pitr --SwissTCS.Downloads /out <out.DIR>
```
