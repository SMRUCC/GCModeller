---
title: link
tags: [maunal, tools]
date: 5/28/2018 9:30:24 PM
---
# ProteinInteraction [version 1.0.0.0]
> Tools for analysis the protein interaction relationship.

<!--more-->

**ProteinInteraction**<br/>
__<br/>
Copyright © 蓝思生物信息工程师工作站 2014

**Module AssemblyName**: link<br/>
**Root namespace**: ``ProteinTools.Interactions.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
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


##### 1. BioGrid CLI tools


|Function API|Info|
|------------|----|
|[/BioGRID.Id.types](#/BioGRID.Id.types)||
|[/BioGRID.selects](#/BioGRID.selects)||


##### 2. STRING-db CLI tools


|Function API|Info|
|------------|----|
|[/STRING.Network](#/STRING.Network)||
|[/STRING.selects](#/STRING.selects)||

## CLI API list
--------------------------
<h3 id="/BioGRID.Id.types"> 1. /BioGRID.Id.types</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 BioGridIdTypes(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link /BioGRID.Id.types /in <BIOGRID-IDENTIFIERS.tsv> [/out <out.txt>]
```
<h3 id="/BioGRID.selects"> 2. /BioGRID.selects</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 BioGRIDSelects(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link /BioGRID.selects /in <in.DIR/*.Csv> /key <GeneId> /links <BioGRID-links.mitab.txt> [/out <out.DIR/*.Csv>]
```
<h3 id="/STRING.Network"> 3. /STRING.Network</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 StringNetwork(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link /STRING.Network /id <uniprot_idMappings.tsv> /links <protein.actions-links.tsv> [/sub <idlist.txt> /attributes <dataset.csv> /id_field <ID> /all_links <protein.links.txt> /out <outDIR>]
```
<h3 id="/STRING.selects"> 4. /STRING.selects</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 STRINGSelects(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link /STRING.selects /in <in.DIR/*.Csv> /key <GeneId> /links <links.txt> /maps <maps_id.tsv> [/out <out.DIR/*.Csv>]
```
<h3 id="--align.LDM"> 5. --align.LDM</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 GenerateModel(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --align.LDM /in <source.fasta>
```
<h3 id="--Contacts"> 6. --Contacts</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 Contacts(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --Contacts /in <in.DIR>
```
<h3 id="--CrossTalks.Probability"> 7. --CrossTalks.Probability</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 CrossTalksCal(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --CrossTalks.Probability /query <pfam-string.csv> /swiss <swissTCS_pfam-string.csv> [/out <out.CrossTalks.csv> /test <queryName>]
```
<h3 id="--Db.From.Exists"> 8. --Db.From.Exists</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 DbMergeFromExists(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --Db.From.Exists /aln <clustal-aln.DIR> /pfam <pfam-string.csv>
```
<h3 id="--domain.Interactions"> 9. --domain.Interactions</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 DomainInteractions(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --domain.Interactions /pfam <pfam-string.csv> /swissTCS <swissTCS.DIR>
```
<h3 id="--interact.TCS"> 10. --interact.TCS</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 TCSParser(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --interact.TCS /door <door.opr> /MiST2 <mist2.xml> /swiss <tcs.csv.DIR> /out <out.DIR>
```
<h3 id="--Merge.Pfam"> 11. --Merge.Pfam</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 MergePfam(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --Merge.Pfam /in <in.DIR>
```
<h3 id="--predicts.TCS"> 12. --predicts.TCS</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 Predicts(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --predicts.TCS /pfam <pfam-string.csv> /prot <prot.fasta> /Db <interaction.xml>
```
<h3 id="--Profiles.Create"> 13. --Profiles.Create</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 CreateProfiles(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --Profiles.Create /MiST2 <MiST2.xml> /pfam <pfam-string.csv> [/out <out.csv>]
```
<h3 id="--ProtFasta.Downloads"> 14. --ProtFasta.Downloads</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 ProtFastaDownloads(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --ProtFasta.Downloads /in <sp.DIR>
```
<h3 id="--ProtFasta.Downloads.Batch"> 15. --ProtFasta.Downloads.Batch</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 ProtFastaDownloadsBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --ProtFasta.Downloads.Batch /in <sp.DIR.Source>
```
<h3 id="--signature"> 16. --signature</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 SignatureGenerates(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --signature /in <aln.fasta> [/p-cut <0.95>]
```
<h3 id="--SwissTCS.Downloads"> 17. --SwissTCS.Downloads</h3>



**Prototype**: ``ProteinTools.Interactions.CLI::Int32 DownloadEntireDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
link --SwissTCS.Downloads /out <out.DIR>
```
