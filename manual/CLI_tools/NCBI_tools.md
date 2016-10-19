---
title: NCBI_tools
tags: [maunal, tools]
date: 2016/10/19 16:38:33
---
# GCModeller [version 1.0.0.0]
> 

<!--more-->

**NCBI_tools for processing NCBI bioinformatics data**
_NCBI_tools for processing NCBI bioinformatics data_
Copyright ? GPL3 2016

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/NCBI_tools.exe
**Root namespace**: ``NCBI_tools.CLI``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Assign.Taxonomy](#/Assign.Taxonomy)||
|[/Assign.Taxonomy.From.Ref](#/Assign.Taxonomy.From.Ref)||
|[/Assign.Taxonomy.SSU](#/Assign.Taxonomy.SSU)||
|[/Associate.Taxonomy](#/Associate.Taxonomy)||
|[/Associates.Brief](#/Associates.Brief)||
|[/Build_gi2taxi](#/Build_gi2taxi)||
|[/Export.GI](#/Export.GI)||
|[/gi.Match](#/gi.Match)||
|[/gi.Matchs](#/gi.Matchs)||
|[/Nt.Taxonomy](#/Nt.Taxonomy)||


##### 1. NCBI data export tools


|Function API|Info|
|------------|----|
|[/Filter.Exports](#/Filter.Exports)||


##### 2. NCBI ``nt`` database tools


|Function API|Info|
|------------|----|
|[/nt.matches.key](#/nt.matches.key)||
|[/nt.matches.name](#/nt.matches.name)||
|[/word.tokens](#/word.tokens)||


##### 3. NCBI taxonomy tools


|Function API|Info|
|------------|----|
|[/OTU.associated](#/OTU.associated)||
|[/OTU.diff](#/OTU.diff)||
|[/OTU.Taxonomy](#/OTU.Taxonomy)||
|[/Search.Taxonomy](#/Search.Taxonomy)||
|[/Split.By.Taxid](#/Split.By.Taxid)||
|[/Split.By.Taxid.Batch](#/Split.By.Taxid.Batch)||
|[/Taxonomy.Data](#/Taxonomy.Data)||
|[/Taxonomy.Tree](#/Taxonomy.Tree)||




## CLI API list
--------------------------
<h3 id="/Assign.Taxonomy"> 1. /Assign.Taxonomy</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 AssignTaxonomy(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Assign.Taxonomy /in <in.DIR> /gi <regexp> /index <fieldName> /tax <NCBI nodes/names.dmp> /gi2taxi <gi2taxi.txt/bin> [/out <out.DIR>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/Assign.Taxonomy.From.Ref"> 2. /Assign.Taxonomy.From.Ref</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 AssignTaxonomyFromRef(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Assign.Taxonomy.From.Ref /in <in.DIR> /ref <nt.taxonomy.fasta> [/index <Name> /non-BIOM /out <out.DIR>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/Assign.Taxonomy.SSU"> 3. /Assign.Taxonomy.SSU</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 AssignTaxonomy2(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Assign.Taxonomy.SSU /in <in.DIR> /index <fieldName> /ref <SSU-ref.fasta> [/out <out.DIR>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/Associate.Taxonomy"> 4. /Associate.Taxonomy</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 AssociateTaxonomy(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Associate.Taxonomy /in <in.DIR> /tax <ncbi_taxonomy:names,nodes> /gi2taxi <gi2taxi.bin> [/gi <nt.gi.csv> /out <out.DIR>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/Associates.Brief"> 5. /Associates.Brief</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 Associates(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Associates.Brief /in <in.DIR> /ls <ls.txt> [/index <Name> /out <out.tsv>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/Build_gi2taxi"> 6. /Build_gi2taxi</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 Build_gi2taxi(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Build_gi2taxi /in <gi2taxi.dmp> [/out <out.dat>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/Export.GI"> 7. /Export.GI</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 ExportGI(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Export.GI /in <ncbi:nt.fasta> [/out <out.csv>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/Filter.Exports"> 8. /Filter.Exports</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 FilterExports(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Filter.Exports /in <nt.fasta> /tax <taxonomy_DIR> /gi2taxid <gi2taxid.txt> /words <list.txt> [/out <out.DIR>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/gi.Match"> 9. /gi.Match</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 giMatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /gi.Match /in <nt.parts.fasta/list.txt> /gi2taxid <gi2taxid.dmp> [/out <gi_match.txt>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/gi.Matchs"> 10. /gi.Matchs</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 giMatchs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /gi.Matchs /in <nt.parts.fasta.DIR> /gi2taxid <gi2taxid.dmp> [/out <gi_match.txt.DIR> /num_threads <-1>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/nt.matches.key"> 11. /nt.matches.key</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 NtKeyMatches(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /nt.matches.key /in <nt.fasta> /list <words.txt> [/out <out.fasta>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/nt.matches.name"> 12. /nt.matches.name</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 NtNameMatches(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /nt.matches.name /in <nt.fasta> /list <names.csv> [/out <out.fasta>]
```
###### Example
```bash
NCBI_tools
```



#### Parameters information:
##### /list

###### Example
```bash

```
##### Accepted Types
###### /list
**Decalre**:  _NCBI_tools.WordTokens_
Example: 
```json
{
    "name": "System.String",
    "tokens": [
        "System.String"
    ]
}
```

<h3 id="/Nt.Taxonomy"> 13. /Nt.Taxonomy</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 NtTaxonomy(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Nt.Taxonomy /in <nt.fasta> /gi2taxi <gi2taxi.bin> /tax <ncbi_taxonomy:names,nodes> [/out <out.fasta>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/OTU.associated"> 14. /OTU.associated</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 OTUAssociated(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /OTU.associated /in <OTU.Data> /maps <mapsHit.csv> [/RawMap <data_mapping.csv> /OTU_Field <"#OTU_NUM"> /out <out.csv>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/OTU.diff"> 15. /OTU.diff</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 OTUDiff(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /OTU.diff /ref <OTU.Data1.csv> /parts <OTU.Data2.csv> [/out <out.csv>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/OTU.Taxonomy"> 16. /OTU.Taxonomy</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 OTU_Taxonomy(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /OTU.Taxonomy /in <OTU.Data> /maps <mapsHit.csv> /tax <taxonomy:nodes/names> [/out <out.csv>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/Search.Taxonomy"> 17. /Search.Taxonomy</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 SearchTaxonomy(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Search.Taxonomy /in <list.txt/expression.csv> /ncbi_taxonomy <taxnonmy:name/nodes.dmp> [/top 10 /expression /cut 0.65 /out <out.csv>]
```
###### Example
```bash
NCBI_tools
```



#### Parameters information:
##### /in

###### Example
```bash

```
##### [/expression]
Search the taxonomy text by using query expression? If this set true, then the input should be a expression csv file.

###### Example
```bash

```
##### [/cut]
This parameter will be disabled when ``/expression`` is presents.

###### Example
```bash

```
##### Accepted Types
###### /in
**Decalre**:  _System.String[]_
Example: 
```json
[
    "System.String"
]
```

**Decalre**:  _Microsoft.VisualBasic.Data.IO.SearchEngine.QueryArgument_
Example: 
```json
{
    "Data": {
        
    },
    "Expression": "System.String",
    "Name": "System.String"
}
```

###### /expression
###### /cut
<h3 id="/Split.By.Taxid"> 18. /Split.By.Taxid</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 SplitByTaxid(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Split.By.Taxid /in <nt.fasta> [/gi2taxid <gi2taxid.txt> /out <outDIR>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/Split.By.Taxid.Batch"> 19. /Split.By.Taxid.Batch</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 SplitByTaxidBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Split.By.Taxid.Batch /in <nt.fasta.DIR> [/num_threads <-1> /out <outDIR>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/Taxonomy.Data"> 20. /Taxonomy.Data</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 TaxonomyTreeData(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Taxonomy.Data /data <data.csv> /field.gi <GI> /gi2taxid <gi2taxid.list.txt> /tax <ncbi_taxonomy:nodes/names> [/out <out.csv>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/Taxonomy.Tree"> 21. /Taxonomy.Tree</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 TaxonomyTree(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /Taxonomy.Tree /taxid <taxid.list.txt> /tax <ncbi_taxonomy:nodes/names> [/out <out.csv>]
```
###### Example
```bash
NCBI_tools
```
<h3 id="/word.tokens"> 22. /word.tokens</h3>


**Prototype**: ``NCBI_tools.CLI::Int32 GetWordTokens(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
NCBI_tools /word.tokens /in <list.txt> [/out <out.csv>]
```
###### Example
```bash
NCBI_tools
```
