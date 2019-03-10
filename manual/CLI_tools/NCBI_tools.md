---
title: NCBI_tools
tags: [maunal, tools]
date: 5/28/2018 9:30:26 PM
---
# GCModeller [version 1.0.0.0]
> Tools collection for handling NCBI data, includes: nt/nr database, NCBI taxonomy analysis, OTU taxonomy analysis, genbank database, and sequence query tools.

<!--more-->

**NCBI_tools for processing NCBI bioinformatics data**<br/>
_NCBI_tools for processing NCBI bioinformatics data_<br/>
Copyright © GPL3 2016

**Module AssemblyName**: NCBI_tools<br/>
**Root namespace**: ``NCBI_tools.CLI``<br/>

------------------------------------------------------------
If you are having trouble debugging this Error, first read the best practices tutorial for helpful tips that address many common problems:
> http://docs.gcmodeller.org


The debugging facility Is helpful To figure out what's happening under the hood:
> https://github.com/SMRUCC/GCModeller/wiki


If you're still stumped, you can try get help from author directly from E-mail:
> xie.guigang@gcmodeller.org



All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Assign.Taxonomy.From.Ref](#/Assign.Taxonomy.From.Ref)||
|[/Assign.Taxonomy.SSU](#/Assign.Taxonomy.SSU)||
|[/Associates.Brief](#/Associates.Brief)||
|[/gpff.fasta](#/gpff.fasta)||
|[/MapHits.list](#/MapHits.list)||
|[/OTU.Taxonomy.Replace](#/OTU.Taxonomy.Replace)|Using ``MapHits`` property|


##### 1. NCBI ``nt`` database tools


|Function API|Info|
|------------|----|
|[/nt.matches.key](#/nt.matches.key)||
|[/nt.matches.name](#/nt.matches.name)||
|[/word.tokens](#/word.tokens)||


##### 2. NCBI data export tools


|Function API|Info|
|------------|----|
|[/Filter.Exports](#/Filter.Exports)|String similarity match of the fasta title with given terms for search and export by taxonomy.|


##### 3. NCBI GI tools(Obsolete from NCBI, 2016-10-20)

> https://www.ncbi.nlm.nih.gov/news/03-02-2016-phase-out-of-GI-numbers/

###### NCBI is phasing out sequence GIs - use Accession.Version instead!

As of September 2016, the integer sequence identifiers known as "GIs" will no longer be included in the GenBank, GenPept, and FASTA formats supported by NCBI for sequence records. The FASTA header will be further simplified to report only the sequence accession.version and record title for accessions managed by the International Sequence Database Collaboration (INSDC) and NCBI’s Reference Sequence (RefSeq) project. As NCBI makes this transition, we encourage any users who have workflows that depend on GI's to begin planning to use accession.version identifiers instead. After September 2016, any processes solely dependent on GIs will no longer function as expected.


|Function API|Info|
|------------|----|
|[/Assign.Taxonomy](#/Assign.Taxonomy)||
|[/Associate.Taxonomy](#/Associate.Taxonomy)||
|[/Build_gi2taxi](#/Build_gi2taxi)||
|[/Export.GI](#/Export.GI)||
|[/Filter.Exports](#/Filter.Exports)|String similarity match of the fasta title with given terms for search and export by taxonomy.|
|[/gi.Match](#/gi.Match)||
|[/gi.Matchs](#/gi.Matchs)||
|[/Nt.Taxonomy](#/Nt.Taxonomy)||
|[/Split.By.Taxid](#/Split.By.Taxid)|Split the input fasta file by taxid grouping.|
|[/Taxonomy.Data](#/Taxonomy.Data)||


##### 4. NCBI taxonomy tools


|Function API|Info|
|------------|----|
|[/accid2taxid.Match](#/accid2taxid.Match)|Creates the subset of the ultra-large accession to ncbi taxonomy id database.|
|[/OTU.associated](#/OTU.associated)||
|[/OTU.diff](#/OTU.diff)||
|[/OTU.Taxonomy](#/OTU.Taxonomy)||
|[/Search.Taxonomy](#/Search.Taxonomy)||
|[/Split.By.Taxid](#/Split.By.Taxid)|Split the input fasta file by taxid grouping.|
|[/Split.By.Taxid.Batch](#/Split.By.Taxid.Batch)||
|[/Taxonomy.Data](#/Taxonomy.Data)||
|[/Taxonomy.Maphits.Overview](#/Taxonomy.Maphits.Overview)||
|[/Taxonomy.Tree](#/Taxonomy.Tree)|Output taxonomy query info by a given NCBI taxid list.|

## CLI API list
--------------------------
<h3 id="/accid2taxid.Match"> 1. /accid2taxid.Match</h3>

Creates the subset of the ultra-large accession to ncbi taxonomy id database.

**Prototype**: ``NCBI_tools.CLI::Int32 accidMatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /accid2taxid.Match /in <nt.parts.fasta/list.txt> /acc2taxid <acc2taxid.dmp/DIR> [/gb_priority /out <acc2taxid_match.txt>]
```
<h3 id="/Assign.Taxonomy"> 2. /Assign.Taxonomy</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 AssignTaxonomy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Assign.Taxonomy /in <in.DIR> /gi <regexp> /index <fieldName> /tax <NCBI nodes/names.dmp> /gi2taxi <gi2taxi.txt/bin> [/out <out.DIR>]
```
<h3 id="/Assign.Taxonomy.From.Ref"> 3. /Assign.Taxonomy.From.Ref</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 AssignTaxonomyFromRef(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Assign.Taxonomy.From.Ref /in <in.DIR> /ref <nt.taxonomy.fasta> [/index <Name> /non-BIOM /out <out.DIR>]
```
<h3 id="/Assign.Taxonomy.SSU"> 4. /Assign.Taxonomy.SSU</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 AssignTaxonomy2(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Assign.Taxonomy.SSU /in <in.DIR> /index <fieldName> /ref <SSU-ref.fasta> [/out <out.DIR>]
```
<h3 id="/Associate.Taxonomy"> 5. /Associate.Taxonomy</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 AssociateTaxonomy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Associate.Taxonomy /in <in.DIR> /tax <ncbi_taxonomy:names,nodes> /gi2taxi <gi2taxi.bin> [/gi <nt.gi.csv> /out <out.DIR>]
```
<h3 id="/Associates.Brief"> 6. /Associates.Brief</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 Associates(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Associates.Brief /in <in.DIR> /ls <ls.txt> [/index <Name> /out <out.tsv>]
```
<h3 id="/Build_gi2taxi"> 7. /Build_gi2taxi</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 Build_gi2taxi(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Build_gi2taxi /in <gi2taxi.dmp> [/out <out.dat>]
```
<h3 id="/Export.GI"> 8. /Export.GI</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 ExportGI(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Export.GI /in <ncbi:nt.fasta> [/out <out.csv>]
```
<h3 id="/Filter.Exports"> 9. /Filter.Exports</h3>

String similarity match of the fasta title with given terms for search and export by taxonomy.

**Prototype**: ``NCBI_tools.CLI::Int32 FilterExports(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Filter.Exports /in <nt.fasta> /tax <taxonomy_DIR> /gi2taxid <gi2taxid.txt> /words <list.txt> [/out <out.DIR>]
```
<h3 id="/gi.Match"> 10. /gi.Match</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 giMatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /gi.Match /in <nt.parts.fasta/list.txt> /gi2taxid <gi2taxid.dmp> [/out <gi_match.txt>]
```
<h3 id="/gi.Matchs"> 11. /gi.Matchs</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 giMatchs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /gi.Matchs /in <nt.parts.fasta.DIR> /gi2taxid <gi2taxid.dmp> [/out <gi_match.txt.DIR> /num_threads <-1>]
```
<h3 id="/gpff.fasta"> 12. /gpff.fasta</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 gpff2Fasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /gpff.fasta /in <gpff.txt> [/out <out.fasta>]
```
<h3 id="/MapHits.list"> 13. /MapHits.list</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 GetMapHitsList(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /MapHits.list /in <in.csv> [/out <out.txt>]
```


#### Arguments
##### /in

###### Example
```bash
/in <term_string>
```
##### Accepted Types
###### /in
**Decalre**:  _NCBI_tools.MapHits_

Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "MapHits": [
        "System.String"
    ],
    "taxid": 0
}
```

**Decalre**:  _NCBI_tools.CLI+MapHit_

Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "MapHits": [
        "System.String"
    ],
    "taxid": 0,
    "Name": "System.String",
    "Taxonomy": "System.String",
    "TaxonomyName": "System.String"
}
```

<h3 id="/nt.matches.key"> 14. /nt.matches.key</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 NtKeyMatches(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /nt.matches.key /in <nt.fasta> /list <words.txt> [/out <out.fasta>]
```
<h3 id="/nt.matches.name"> 15. /nt.matches.name</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 NtNameMatches(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /nt.matches.name /in <nt.fasta> /list <names.csv> [/out <out.fasta>]
```


#### Arguments
##### /list

###### Example
```bash
/list <term_string>
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

<h3 id="/Nt.Taxonomy"> 16. /Nt.Taxonomy</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 NtTaxonomy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Nt.Taxonomy /in <nt.fasta> /gi2taxi <gi2taxi.bin> /tax <ncbi_taxonomy:names,nodes> [/out <out.fasta>]
```
<h3 id="/OTU.associated"> 17. /OTU.associated</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 OTUAssociated(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /OTU.associated /in <OTU.Data> /maps <mapsHit.csv> [/RawMap <data_mapping.csv> /OTU_Field <"#OTU_NUM"> /out <out.csv>]
```
<h3 id="/OTU.diff"> 18. /OTU.diff</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 OTUDiff(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /OTU.diff /ref <OTU.Data1.csv> /parts <OTU.Data2.csv> [/out <out.csv>]
```
<h3 id="/OTU.Taxonomy"> 19. /OTU.Taxonomy</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 OTU_Taxonomy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /OTU.Taxonomy /in <OTU.Data> /maps <mapsHit.csv> /tax <taxonomy:nodes/names> [/out <out.csv>]
```
<h3 id="/OTU.Taxonomy.Replace"> 20. /OTU.Taxonomy.Replace</h3>

Using ``MapHits`` property

**Prototype**: ``NCBI_tools.CLI::Int32 OTUTaxonomyReplace(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /OTU.Taxonomy.Replace /in <otu.table.csv> /maps <maphits.csv> [/out <out.csv>]
```
<h3 id="/Search.Taxonomy"> 21. /Search.Taxonomy</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 SearchTaxonomy(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Search.Taxonomy /in <list.txt/expression.csv> /ncbi_taxonomy <taxnonmy:name/nodes.dmp> [/top 10 /expression /cut 0.65 /out <out.csv>]
```


#### Arguments
##### /in

###### Example
```bash
/in <term_string>
```
##### [/expression]
Search the taxonomy text by using query expression? If this set true, then the input should be a expression csv file.

###### Example
```bash
/expression <term_string>
```
##### [/cut]
This parameter will be disabled when ``/expression`` is presents.

###### Example
```bash
/cut <term_string>
```
##### Accepted Types
###### /in
**Decalre**:  _<a href="https://msdn.microsoft.com/en-us/library/system.string[](v=vs.110).aspx?cs-save-lang=1&cs-lang=vb#code-snippet-1">System.String[]</a>_

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
        "System.String": "System.String"
    },
    "Expression": "System.String",
    "Name": "System.String"
}
```

<h3 id="/Split.By.Taxid"> 22. /Split.By.Taxid</h3>

Split the input fasta file by taxid grouping.

**Prototype**: ``NCBI_tools.CLI::Int32 SplitByTaxid(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Split.By.Taxid /in <nt.fasta> [/gi2taxid <gi2taxid.txt> /out <outDIR>]
```
<h3 id="/Split.By.Taxid.Batch"> 23. /Split.By.Taxid.Batch</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 SplitByTaxidBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Split.By.Taxid.Batch /in <nt.fasta.DIR> [/num_threads <-1> /out <outDIR>]
```
<h3 id="/Taxonomy.Data"> 24. /Taxonomy.Data</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 TaxonomyTreeData(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Taxonomy.Data /data <data.csv> /field.gi <GI> /gi2taxid <gi2taxid.list.txt> /tax <ncbi_taxonomy:nodes/names> [/out <out.csv>]
```
<h3 id="/Taxonomy.Maphits.Overview"> 25. /Taxonomy.Maphits.Overview</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 TaxidMapHitViews(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Taxonomy.Maphits.Overview /in <in.DIR> [/out <out.csv>]
```
<h3 id="/Taxonomy.Tree"> 26. /Taxonomy.Tree</h3>

Output taxonomy query info by a given NCBI taxid list.

**Prototype**: ``NCBI_tools.CLI::Int32 TaxonomyTree(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /Taxonomy.Tree /taxid <taxid.list.txt> /tax <ncbi_taxonomy:nodes/names> [/out <out.csv>]
```
<h3 id="/word.tokens"> 27. /word.tokens</h3>



**Prototype**: ``NCBI_tools.CLI::Int32 GetWordTokens(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage

```bash
NCBI_tools /word.tokens /in <list.txt> [/out <out.csv>]
```
