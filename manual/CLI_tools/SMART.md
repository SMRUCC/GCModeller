---
title: SMART
tags: [maunal, tools]
date: 2016/10/22 12:30:18
---
# GCModeller [version 1.123.0.0]
> SMART protein domain structure tools CLI interface.

<!--more-->

**(SMART) Simple Protein Modular Architecture Research Analysis Tool**
_(SMART) Simple Protein Modular Architecture Research Analysis Tool_
Copyright ? LANS Corp. 2014

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/SMART.exe
**Root namespace**: ``ProteinTools.SMART.CLI``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[--align](#--align)||
|[--align.family](#--align.family)||
|[-build_cache](#-build_cache)||
|[-buildsmart](#-buildsmart)||
|[convert](#convert)||
|[export](#export)||
|[--Export.Domains](#--Export.Domains)||
|[--Export.Pfam-String](#--Export.Pfam-String)||
|[--Family.Align](#--Family.Align)|Family Annotation by MPAlignment|
|[--Family.Domains](#--Family.Domains)|Build the Family database for the protein family annotation by MPAlignment.|
|[--Family.Stat](#--Family.Stat)||
|[grep](#grep)|The gene id in the blast output log file are not well format for reading and program processing, so before you generate the venn diagram you should call this command to parse the gene id from the log file. You can also done this id parsing job using other tools.|
|[--manual-Build](#--manual-Build)||
|[--MPAlignment](#--MPAlignment)||
|[pure_domain](#pure_domain)||
|[--SelfAlign](#--SelfAlign)||

## CLI API list
--------------------------
<h3 id="--align"> 1. --align</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 Align(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --align /query <query.csv> /subject <subject.csv> [/out <out.DIR> /inst]
```
<h3 id="--align.family"> 2. --align.family</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 FamilyAlign(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --align.family /In <In.bbh.csv> /query <query-pfam.csv> /subject <subject-pfam.csv> [/out <out.DIR> /mp <mp-align:0.65> /lev <lev-align:0.65>]
```
<h3 id="-build_cache"> 3. -build_cache</h3>


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
<h3 id="-buildsmart"> 4. -buildsmart</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 BuildSmart(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART
```
<h3 id="convert"> 5. convert</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 Convert(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART convert -i <input_file> [-o <xml_file>]
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
-keyword HTH,GGDEF,Clp,REC
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
<h3 id="grep"> 12. grep</h3>

The gene id in the blast output log file are not well format for reading and program processing, so before you generate the venn diagram you should call this command to parse the gene id from the log file. You can also done this id parsing job using other tools.
**Prototype**: ``ProteinTools.SMART.CLI::Int32 Grep(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART grep -i <xml_log_file> -q <script_statements> -h <script_statements>
```
###### Example
```bash
SMART grep -i C:\Users\WORKGROUP\Desktop\blast_xml_logs\1__8004_ecoli_prot.log.xml -q "tokens | 4" -h "'tokens | 2';'tokens ' ' 0'"
```


#### Arguments
##### -q
The parsing script for parsing the gene_id from the blast log file, this switch value is consist of sevral operation tokens, and each token is separate by the ';' character and the token unit in each script token should seperate by the ' character.
There are two basic operation in this parsing script:
tokens - Split the query or hit name string into sevral piece of string by the specific delimiter character and           get the specifc location unit in the return string array.
Usage:   tokens <delimiter> <position>
Example: tokens | 3 match - match a gene id using a specific pattern regular expression.
usage:   match <regular_expression>
Example: match .+[-]\d{5}

###### Example
```bash
-q "'tokens | 5';'match .+[-].+'"
```
##### -h
The parsing script for parsing the gene_id from the blast log file, this switch value is consist of sevral operation tokens, and each token is separate by the ';' character and the token unit in each script token should seperate by the ' character.
There are two basic operation in this parsing script:
tokens - Split the query or hit name string into sevral piece of string by the specific delimiter character and           get the specifc location unit in the return string array.
Usage:   tokens <delimiter> <position>
Example: tokens | 3 match - match a gene id using a specific pattern regular expression.
usage:   match <regular_expression>
Example: match .+[-]\d{5}

###### Example
```bash
-h "'tokens | 5';'match .+[-].+'"
```
<h3 id="--manual-Build"> 13. --manual-Build</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 ManualBuild(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --manual-Build /pfam-string <pfam-string.csv> /name <familyName>
```
<h3 id="--MPAlignment"> 14. --MPAlignment</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 SBHAlignment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART --MPAlignment /sbh <sbh.csv> /query <pfam-string.csv> /subject <pfam-string.csv> [/mp <0.65> /out <out.csv>]
```
<h3 id="pure_domain"> 15. pure_domain</h3>


**Prototype**: ``ProteinTools.SMART.CLI::Int32 FiltePureDomain(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
SMART pure_domain -i <input_smart_log> -o <output_file>
```
<h3 id="--SelfAlign"> 16. --SelfAlign</h3>


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
