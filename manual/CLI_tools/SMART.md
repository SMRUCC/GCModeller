---
title: SMART
tags: [maunal, tools]
date: 5/28/2018 8:37:26 PM
---
# GCModeller [version 1.123.0.0]
> SMART protein domain structure tools CLI interface.

<!--more-->

**(SMART) Simple Protein Modular Architecture Research Analysis Tool**<br/>
_(SMART) Simple Protein Modular Architecture Research Analysis Tool_<br/>
Copyright © LANS Corp. 2014

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/SMART.exe<br/>
**Root namespace**: ``ProteinTools.SMART.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/UniProt.domains](#/UniProt.domains)|Export the protein structure domain annotation table from UniProt database dump.|
|[--align](#--align)||
|[--align.family](#--align.family)||
|[-build_cache](#-build_cache)||
|[-buildsmart](#-buildsmart)||
|[export](#export)||
|[--Export.Domains](#--Export.Domains)||
|[--Export.Pfam-String](#--Export.Pfam-String)||
|[--Family.Align](#--Family.Align)|Family Annotation by MPAlignment|
|[--Family.Domains](#--Family.Domains)|Build the Family database for the protein family annotation by MPAlignment.|
|[--Family.Stat](#--Family.Stat)||
|[--manual-Build](#--manual-Build)||
|[--MPAlignment](#--MPAlignment)||
|[pure_domain](#pure_domain)||
|[--SelfAlign](#--SelfAlign)||

## CLI API list
--------------------------
<h3 id="/UniProt.domains"> 1. /UniProt.domains</h3>

Export the protein structure domain annotation table from UniProt database dump.
**Prototype**: ``ProteinTools.SMART.CLI::Int32 UniProtXmlDomains(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART /UniProt.domains /in <uniprot.Xml> [/map <maps.tab/tsv> /out <proteins.csv>]
```
<h3 id="--align"> 2. --align</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 Align(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --align /query <query.csv> /subject <subject.csv> [/out <out.DIR> /inst]
```
<h3 id="--align.family"> 3. --align.family</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 FamilyAlign(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --align.family /In <In.bbh.csv> /query <query-pfam.csv> /subject <subject-pfam.csv> [/out <out.DIR> /mp <mp-align:0.65> /lev <lev-align:0.65>]
```
<h3 id="-build_cache"> 4. -build_cache</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 BuildCache(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART -build_cache -i <fsa_file> [-o <export_file> -db <cdd_db_name> -cdd <cdd_db_path> -grep_script <script>]
```


#### Arguments
##### [-cdd]
The cdd database directory, if this switch value is null then system will using the default position in the profile file.

###### Example
```bash
-cdd <term_string>
```
<h3 id="-buildsmart"> 5. -buildsmart</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 BuildSmart(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART
```
<h3 id="export"> 6. export</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 Export(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART export -keyword <keyword_list> [-m <any/all>] -o <export_file> [-d <db_name> -casesense <T/F>]
```


#### Arguments
##### -d
This switch value can be both a domain database name or a fasta file path.

###### Example
```bash
-d <term_string>
```
##### -keyword
The keyword list will be use for the sequence record search, each keyword should seperated by comma character.

###### Example
```bash
-keyword <term_string>
```
<h3 id="--Export.Domains"> 7. --Export.Domains</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 ExportRegpreciseDomains(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --Export.Domains /in <pfam-string.csv>
```
<h3 id="--Export.Pfam-String"> 8. --Export.Pfam-String</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 ExportPfamString(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --Export.Pfam-String /in <blast_out.txt>
```
<h3 id="--Family.Align"> 9. --Family.Align</h3>

Family Annotation by MPAlignment
**Prototype**: ``ProteinTools.SMART.CLI::Int32 FamilyClassify(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --Family.Align /query <pfam-string.csv> [/threshold 0.65 /mp 0.65 /Name <null>]
```


#### Arguments
##### [/Name]
The database name of the aligned subject, if this value is empty or not exists in the source, then the entired Family database will be used.

###### Example
```bash
/Name <term_string>
```
<h3 id="--Family.Domains"> 10. --Family.Domains</h3>

Build the Family database for the protein family annotation by MPAlignment.
**Prototype**: ``ProteinTools.SMART.CLI::Int32 FamilyDomains(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --Family.Domains /regprecise <regulators.fasta> /pfam <pfam-string.csv>
```
<h3 id="--Family.Stat"> 11. --Family.Stat</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 FamilyStat(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --Family.Stat /in <anno_out.csv>
```
<h3 id="--manual-Build"> 12. --manual-Build</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 ManualBuild(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --manual-Build /pfam-string <pfam-string.csv> /name <familyName>
```
<h3 id="--MPAlignment"> 13. --MPAlignment</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 SBHAlignment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --MPAlignment /sbh <sbh.csv> /query <pfam-string.csv> /subject <pfam-string.csv> [/mp <0.65> /out <out.csv>]
```
<h3 id="pure_domain"> 14. pure_domain</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 FiltePureDomain(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART pure_domain -i <input_smart_log> -o <output_file>
```
<h3 id="--SelfAlign"> 15. --SelfAlign</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 SelfAlign(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --SelfAlign /query <pfam-string.csv> /subject <subject.csv> /aln <mpalignment.csv> [/lstID <lstID.txt> /mp <0.65> /id <id>]
```


#### Arguments
##### [/lstID]
If this parameter is not empty, then the /aln parameter will be disable

###### Example
```bash
/lstID <term_string>
```
##### [/id]
If this parameter is not null, then the record of this query or hits will be used to subset the alignment set.

###### Example
```bash
/id <term_string>
```
