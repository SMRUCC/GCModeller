---
title: mpl
tags: [maunal, tools]
date: 11/24/2016 2:54:12 AM
---
# GCModeller [version 1.0.0.0]
> 

<!--more-->

**Motif Parallel Alignment Tools**<br/>
_Motif Parallel Alignment Tools_<br/>
Copyright © gcmodeller.org 2015

**Module AssemblyName**: file:///G:/GCModeller/GCModeller/bin/mpl.exe<br/>
**Root namespace**: ``xMPAlignment.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Build.Db.CDD](#/Build.Db.CDD)|Install NCBI CDD database into the GCModeller repository database for the MPAlignment analysis.|
|[/Build.Db.Family](#/Build.Db.Family)|Build protein family database from KEGG database dump data and using for the protein family annotation by MPAlignment.|
|[/Build.Db.Family.Manual-Build](#/Build.Db.Family.Manual-Build)||
|[/Build.Db.Ortholog](#/Build.Db.Ortholog)|Build protein functional orthology database from KEGG orthology or NCBI COG database.|
|[/Build.Db.PPI](#/Build.Db.PPI)|Build protein interaction seeds database from string-db.|
|[/Build.PPI.Signature](#/Build.PPI.Signature)||
|[/KEGG.Family](#/KEGG.Family)||
|[/Motif.Density](#/Motif.Density)||
|[/Pfam.Align](#/Pfam.Align)|Align your proteins with selected protein domain structure database by using blast+ program.|
|[/Pfam.Sub](#/Pfam.Sub)||
|[/Pfam-String.Dump](#/Pfam-String.Dump)|Dump the pfam-String domain Structure composition information from the blastp alignment result.|
|[/Select.Pfam-String](#/Select.Pfam-String)||
|[--align](#--align)|MPAlignment on your own dataset.|
|[--align.Family](#--align.Family)|Protein family annotation by using MPAlignment algorithm.|
|[--align.Family_test](#--align.Family_test)||
|[--align.Function](#--align.Function)|Protein function annotation by using MPAlignment algorithm.|
|[--align.PPI](#--align.PPI)|Protein-Protein interaction network annotation by using MPAlignment algorithm.|
|[--align.PPI_test](#--align.PPI_test)||
|[--align.String](#--align.String)|MPAlignment test, pfam-string value must be in format as  <locusId>:<length>:<pfam-string>|
|[--blast.allhits](#--blast.allhits)||
|[--View.Alignment](#--View.Alignment)||

## CLI API list
--------------------------
<h3 id="/Build.Db.CDD"> 1. /Build.Db.CDD</h3>

Install NCBI CDD database into the GCModeller repository database for the MPAlignment analysis.
**Prototype**: ``xMPAlignment.CLI::Int32 InstallCDD(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl /Build.Db.CDD /source <source.DIR>
```
<h3 id="/Build.Db.Family"> 2. /Build.Db.Family</h3>

Build protein family database from KEGG database dump data and using for the protein family annotation by MPAlignment.
**Prototype**: ``xMPAlignment.CLI::Int32 BuildFamily(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl /Build.Db.Family /source <source.KEGG.fasta> /pfam <pfam-string.csv>
```
<h3 id="/Build.Db.Family.Manual-Build"> 3. /Build.Db.Family.Manual-Build</h3>


**Prototype**: ``xMPAlignment.CLI::Int32 ManualBuild(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl /Build.Db.Family.Manual-Build /pfam-string <pfam-string.csv> /name <familyName>
```
<h3 id="/Build.Db.Ortholog"> 4. /Build.Db.Ortholog</h3>

Build protein functional orthology database from KEGG orthology or NCBI COG database.
**Prototype**: ``xMPAlignment.CLI::Int32 BuildOrthologDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl /Build.Db.Ortholog [/COG <cogDIR> /KO]
```
<h3 id="/Build.Db.PPI"> 5. /Build.Db.PPI</h3>

Build protein interaction seeds database from string-db.
**Prototype**: ``xMPAlignment.CLI::Int32 BuildPPIDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl
```
<h3 id="/Build.PPI.Signature"> 6. /Build.PPI.Signature</h3>


**Prototype**: ``xMPAlignment.CLI::Int32 BuildSignature(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl /Build.PPI.Signature /in <clustalW.fasta> [/level <5> /out <out.xml>]
```


#### Arguments
##### [/level]
It is not recommended to modify this value. The greater of this value, the more strict of the interaction scoring. level 5 is enough.

###### Example
```bash
/level <term_string>
```
<h3 id="/KEGG.Family"> 7. /KEGG.Family</h3>


**Prototype**: ``xMPAlignment.CLI::Int32 KEGGFamilys(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl /KEGG.Family /in <inDIR> /pfam <pfam-string.csv> [/out <out.csv>]
```
<h3 id="/Motif.Density"> 8. /Motif.Density</h3>


**Prototype**: ``xMPAlignment.CLI::Int32 MotifDensity(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl /Motif.Density /in <pfam-string.csv> [/out <out.csv>]
```
<h3 id="/Pfam.Align"> 9. /Pfam.Align</h3>

Align your proteins with selected protein domain structure database by using blast+ program.
**Prototype**: ``xMPAlignment.CLI::Int32 AlignPfam(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl /Pfam.Align /query <query.fasta> [/db <name/path> /out <blastOut.txt>]
```
<h3 id="/Pfam.Sub"> 10. /Pfam.Sub</h3>


**Prototype**: ``xMPAlignment.CLI::Int32 SubPfam(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl /Pfam.Sub /index <bbh_index.csv> /pfam <pfam-string> [/out <sub-out.csv>]
```
<h3 id="/Pfam-String.Dump"> 11. /Pfam-String.Dump</h3>

Dump the pfam-String domain Structure composition information from the blastp alignment result.
**Prototype**: ``xMPAlignment.CLI::Int32 DumpPfamString(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl /Pfam-String.Dump /In <blastp_out.txt> [/out <pfam-String.csv> /evalue <0.001> /identities <0.2> /coverage <0.85>]
```


#### Arguments
##### /In
The blastp output For the protein alignment, which the aligned database can be selected from Pfam-A Or NCBI CDD. And blast+ program Is recommended For used For the domain alignment.

###### Example
```bash
/In <term_string>
```
##### [/out]
The output Excel .csv data file path For the dumped pfam-String data Of your annotated protein. If this parameter Is empty, Then the file will saved On the same location With your blastp input file.

###### Example
```bash
/out <term_string>
```
<h3 id="/Select.Pfam-String"> 12. /Select.Pfam-String</h3>


**Prototype**: ``xMPAlignment.CLI::Int32 SelectPfams(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl /Select.Pfam-String /in <pfam-string.csv> /hits <bbh/sbh.csv> [/hit_name /out <out.csv>]
```
<h3 id="--align"> 13. --align</h3>

MPAlignment on your own dataset.
**Prototype**: ``xMPAlignment.CLI::Int32 MPAlignment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl --align /query <pfam-string.csv> /subject <pfam-string.csv> [/hits <query_vs_sbj.blastp.csv> /flip-bbh /out <alignment_out.csv> /mp <cutoff:=0.65> /swap /parts]
```


#### Arguments
##### [/swap]
Swap the location of query and subject in the output result set.

###### Example
```bash
/swap <term_string>
```
##### [/parts]
Does the domain motif equals function determine the domain positioning equals just if one side in the high scoring then thoese two domain its position is equals?
Default is not, default checks right side and left side.

###### Example
```bash
/parts <term_string>
```
##### [/flip-bbh]
Swap the direction of the query_name/hit_name in the hits?

###### Example
```bash
/flip-bbh <term_string>
```
<h3 id="--align.Family"> 14. --align.Family</h3>

Protein family annotation by using MPAlignment algorithm.
**Prototype**: ``xMPAlignment.CLI::Int32 FamilyClassified(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl --align.Family /query <pfam-string.csv> [/out <out.csv> /threshold 0.5 /mp 0.6 /Name <null>]
```


#### Arguments
##### [/Name]
The database name of the aligned subject, if this value is empty or not exists in the source, then the entired Family database will be used.

###### Example
```bash
/Name <term_string>
```
<h3 id="--align.Family_test"> 15. --align.Family_test</h3>


**Prototype**: ``xMPAlignment.CLI::Int32 FamilyAlignmentTest(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl --align.Family_test /query <pfam-string> /name <dbName/Path> [/threshold <0.65> /mpCut <0.65> /accept <10>]
```
<h3 id="--align.Function"> 16. --align.Function</h3>

Protein function annotation by using MPAlignment algorithm.
**Prototype**: ``xMPAlignment.CLI::Int32 AlignFunction(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl
```
<h3 id="--align.PPI"> 17. --align.PPI</h3>

Protein-Protein interaction network annotation by using MPAlignment algorithm.
**Prototype**: ``xMPAlignment.CLI::Int32 MplPPI(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl
```
<h3 id="--align.PPI_test"> 18. --align.PPI_test</h3>


**Prototype**: ``xMPAlignment.CLI::Int32 StructureAlign(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl --align.PPI_test /query <contacts.fasta> /db <ppi_signature.Xml> [/mp <cutoff:=0.9> /out <outDIR>]
```
<h3 id="--align.String"> 19. --align.String</h3>

MPAlignment test, pfam-string value must be in format as  <locusId>:<length>:<pfam-string>
**Prototype**: ``xMPAlignment.CLI::Int32 MPAlignment2(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl --align.String /query <pfam-string> /subject <pfam-string> [/mp <cutoff:=0.65> /out <outDIR> /parts]
```
<h3 id="--blast.allhits"> 20. --blast.allhits</h3>


**Prototype**: ``xMPAlignment.CLI::Int32 ExportAppSBH(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl --blast.allhits /blast <blastp.txt> [/out <sbh.csv> /coverage <0.5> /identities 0.15]
```
<h3 id="--View.Alignment"> 21. --View.Alignment</h3>


**Prototype**: ``xMPAlignment.CLI::Int32 ViewAlignment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
mpl --View.Alignment /blast <blastp.txt> /name <queryName> [/out <out.png>]
```
