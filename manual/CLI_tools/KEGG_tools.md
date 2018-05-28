---
title: KEGG_tools
tags: [maunal, tools]
date: 5/28/2018 9:05:06 PM
---
# GCModeller [version 3.0.854.0]
> KEGG web services API tools.

<!--more-->

**CLI tools utilis for KEGG database DBGET API**<br/>
_CLI tools utilis for KEGG database DBGET API_<br/>
Copyright © xie.guigang@gmail.com 2014

**Module AssemblyName**: KEGG_tools<br/>
**Root namespace**: ``KEGG_tools.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/16S_rRNA](#/16S_rRNA)|Download 16S rRNA data from KEGG.|
|[/blastn](#/blastn)|Blastn analysis of your DNA sequence on KEGG server for the functional analysis.|
|[/Compile.Model](#/Compile.Model)|KEGG pathway model compiler|
|[/Compound.Map.Render](#/Compound.Map.Render)|Render draw of the KEGG pathway map by using a given KEGG compound id list.|
|[/Cut_sequence.upstream](#/Cut_sequence.upstream)||
|[/Download.human.genes](#/Download.human.genes)||
|[/Download.Module.Maps](#/Download.Module.Maps)|Download the KEGG reference modules map data.|
|[/Download.Ortholog](#/Download.Ortholog)|Downloads the KEGG gene ortholog annotation data from the web server.|
|[/Dump.sp](#/Dump.sp)||
|[/Fasta.By.Sp](#/Fasta.By.Sp)||
|[/Get.prot_motif](#/Get.prot_motif)||
|[/Gets.prot_motif](#/Gets.prot_motif)||
|[/Imports.KO](#/Imports.KO)|Imports the KEGG reference pathway map and KEGG orthology data as mysql dumps.|
|[/Imports.SSDB](#/Imports.SSDB)||
|[/ko.index.sub.match](#/ko.index.sub.match)||
|[/Organism.Table](#/Organism.Table)||
|[/Pathway.geneIDs](#/Pathway.geneIDs)||
|[/Query.KO](#/Query.KO)||
|[/show.organism](#/show.organism)||
|[/Views.mod_stat](#/Views.mod_stat)||
|[-Build.KO](#-Build.KO)|Download data from KEGG database to local server.|
|[Download.Sequence](#Download.Sequence)||
|[--Dump.Db](#--Dump.Db)||
|[-function.association.analysis](#-function.association.analysis)||
|[--Get.KO](#--Get.KO)||
|[--part.from](#--part.from)|source and ref should be in KEGG annotation format.|
|[-query](#-query)|Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.|
|[-query.orthology](#-query.orthology)||
|[-query.ref.map](#-query.ref.map)||
|[-Table.Create](#-Table.Create)||


##### 1. KEGG dbget API tools


|Function API|Info|
|------------|----|
|[/Download.Compounds](#/Download.Compounds)|Downloads the KEGG compounds data from KEGG web server using dbget API|
|[/Download.Pathway.Maps](#/Download.Pathway.Maps)|Fetch all of the pathway map information for a specific kegg organism by using a specifc kegg sp code.|
|[/Download.Pathway.Maps.Bacteria.All](#/Download.Pathway.Maps.Bacteria.All)||
|[/Download.Pathway.Maps.Batch](#/Download.Pathway.Maps.Batch)||
|[/Download.Reaction](#/Download.Reaction)|Downloads the KEGG enzyme reaction reference model data.|
|[/dump.kegg.maps](#/dump.kegg.maps)|Dumping the KEGG maps database for human species.|
|[/Pathways.Downloads.All](#/Pathways.Downloads.All)|Download all of the KEGG reference pathway map data.|
|[-ref.map.download](#-ref.map.download)||


##### 2. KEGG models repository cli tools


|Function API|Info|
|------------|----|
|[/Build.Compounds.Repository](#/Build.Compounds.Repository)||
|[/Build.Ko.repository](#/Build.Ko.repository)||
|[/Build.Reactions.Repository](#/Build.Reactions.Repository)||
|[/Maps.Repository.Build](#/Maps.Repository.Build)||
|[/Pathway.Modules.Build](#/Pathway.Modules.Build)||

## CLI API list
--------------------------
<h3 id="/16S_rRNA"> 1. /16S_rRNA</h3>

Download 16S rRNA data from KEGG.
**Prototype**: ``KEGG_tools.CLI::Int32 Download16SRNA(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /16s_rna [/out <outDIR>]
```
<h3 id="/blastn"> 2. /blastn</h3>

Blastn analysis of your DNA sequence on KEGG server for the functional analysis.
**Prototype**: ``KEGG_tools.CLI::Int32 Blastn(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /blastn /query <query.fasta> [/out <outDIR>]
```
<h3 id="/Build.Compounds.Repository"> 3. /Build.Compounds.Repository</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 BuildCompoundsRepository(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Build.Compounds.Repository /in <directory> [/glycan.ignore /out <repository.XML>]
```
<h3 id="/Build.Ko.repository"> 4. /Build.Ko.repository</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 BuildKORepository(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Build.Ko.repository /DIR <DIR> /repo <root>
```
<h3 id="/Build.Reactions.Repository"> 5. /Build.Reactions.Repository</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 BuildReactionsRepository(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Build.Reactions.Repository /in <directory> [/out <repository.XML>]
```
<h3 id="/Compile.Model"> 6. /Compile.Model</h3>

KEGG pathway model compiler
**Prototype**: ``KEGG_tools.CLI::Int32 Compile(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Compile.Model /pathway <pathwayDIR> /mods <modulesDIR> /sp <sp_code> [/out <out.Xml>]
```
<h3 id="/Compound.Map.Render"> 7. /Compound.Map.Render</h3>

Render draw of the KEGG pathway map by using a given KEGG compound id list.
**Prototype**: ``KEGG_tools.CLI::Int32 CompoundMapRender(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Compound.Map.Render /list <csv/txt> [/repo <pathwayMap.repository> /scale <default=1> /color <default=red> /out <out.DIR>]
```


#### Arguments
##### /list
A KEGG compound id list that provides the KEGG pathway map rendering source.

###### Example
```bash
/list <file, *.txt, *.csv>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/repo]
A directory path that contains the KEGG reference pathway map XML model. If this argument value is not presented in the commandline, then the default installed GCModeller KEGG compound repository will be used.

###### Example
```bash
/repo <file/directory>
```
##### [/out]
A directory output path that will be using for contains the rendered pathway map image and the summary table file.

###### Example
```bash
/out <file/directory>
```
##### [/color]
The node color that the KEGG compound rendering on the pathway map.

###### Example
```bash
/color <term_string>
```
##### [/scale]
The circle radius size of the KEGG compound that rendering on the output pathway map image. By default is no scale.

###### Example
```bash
/scale <float>
```
##### Accepted Types
###### /list
**Decalre**:  _System.String[]_
Example: 
```json
[
    "System.String"
]
```

<h3 id="/Cut_sequence.upstream"> 8. /Cut_sequence.upstream</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 CutSequence_Upstream(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Cut_sequence.upstream /in <list.txt> /PTT <genome.ptt> /org <kegg_sp> [/len <100bp> /overrides /out <outDIR>]
```
<h3 id="/Download.Compounds"> 9. /Download.Compounds</h3>

Downloads the KEGG compounds data from KEGG web server using dbget API
**Prototype**: ``KEGG_tools.CLI::Int32 DownloadCompounds(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Download.Compounds [/chebi <accessions.tsv> /flat /updates /save <DIR>]
```


#### Arguments
##### [/chebi]
Some compound metabolite in the KEGG database have no brite catalog info, then using the brite database for the compounds downloads will missing some compounds,
then you can using this option for downloads the complete compounds data in the KEGG database.

###### Example
```bash
/chebi <file/directory>
```
##### Accepted Types
###### /chebi
**Decalre**:  _SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables.Accession_
Example: 
```json
{
    "COMPOUND_ID": "System.String",
    "ID": "System.String",
    "SOURCE": "System.String",
    "TYPE": "System.String",
    "ACCESSION_NUMBER": "System.String"
}
```

<h3 id="/Download.human.genes"> 10. /Download.human.genes</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DownloadHumanGenes(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Download.human.genes /in <geneID.list/DIR> [/batch /out <save.DIR>]
```
<h3 id="/Download.Module.Maps"> 11. /Download.Module.Maps</h3>

Download the KEGG reference modules map data.
**Prototype**: ``KEGG_tools.CLI::Int32 DownloadReferenceModule(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Download.Module.Maps [/out <EXPORT_DIR, default="./">]
```
<h3 id="/Download.Ortholog"> 12. /Download.Ortholog</h3>

Downloads the KEGG gene ortholog annotation data from the web server.
**Prototype**: ``KEGG_tools.CLI::Int32 DownloadOrthologs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Download.Ortholog -i <gene_list_file.txt/gbk> -export <exportedDIR> [/gbk /sp <KEGG.sp>]
```
<h3 id="/Download.Pathway.Maps"> 13. /Download.Pathway.Maps</h3>

Fetch all of the pathway map information for a specific kegg organism by using a specifc kegg sp code.
**Prototype**: ``KEGG_tools.CLI::Int32 DownloadPathwayMaps(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Download.Pathway.Maps /sp <kegg.sp_code> [/KGML /out <EXPORT_DIR> /@set <progress_bar=disabled>]
```


#### Arguments
##### /sp
The 3 characters kegg organism code, example as: "xcb" is stands for organism "Xanthomonas campestris pv. campestris 8004 (Beijing)"

###### Example
```bash
/sp <term_string>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### Accepted Types
###### /sp
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

<h3 id="/Download.Pathway.Maps.Bacteria.All"> 14. /Download.Pathway.Maps.Bacteria.All</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DownloadsBacteriasRefMaps(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Download.Pathway.Maps.Bacteria.All [/in <brite.keg> /KGML /out <out.directory>]
```
<h3 id="/Download.Pathway.Maps.Batch"> 15. /Download.Pathway.Maps.Batch</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DownloadPathwayMapsBatchTask(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Download.Pathway.Maps.Batch /sp <kegg.sp_code.list> [/KGML /out <EXPORT_DIR>]
```
<h3 id="/Download.Reaction"> 16. /Download.Reaction</h3>

Downloads the KEGG enzyme reaction reference model data.
**Prototype**: ``KEGG_tools.CLI::Int32 DownloadKEGGReaction(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Download.Reaction [/save <DIR> /@set sleep=2000]
```
<h3 id="/dump.kegg.maps"> 17. /dump.kegg.maps</h3>

Dumping the KEGG maps database for human species.
**Prototype**: ``KEGG_tools.CLI::Int32 DumpKEGGMaps(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /dump.kegg.maps [/htext <htext.txt> /out <save_dir>]
```


#### Arguments
##### /htext
The KEGG category term provider

###### Example
```bash
/htext <file, *.txt>
```
##### [/out]
A directory path that contains the download KEGG reference pathway map model data, this output can be using as the KEGG pathway map rendering repository source.

###### Example
```bash
/out <file/directory>
```
<h3 id="/Dump.sp"> 18. /Dump.sp</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DumpOrganisms(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Dump.sp [/res sp.html /out <out.csv>]
```
<h3 id="/Fasta.By.Sp"> 19. /Fasta.By.Sp</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 GetFastaBySp(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Fasta.By.Sp /in <KEGG.fasta> /sp <sp.list> [/out <out.fasta>]
```
<h3 id="/Get.prot_motif"> 20. /Get.prot_motif</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 ProteinMotifs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Get.prot_motif /query <sp:locus> [/out out.json]
```
<h3 id="/Gets.prot_motif"> 21. /Gets.prot_motif</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 GetsProteinMotifs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Gets.prot_motif /query <query.txt/genome.PTT> [/PTT /sp <kegg-sp> /out <out.json> /update]
```
<h3 id="/Imports.KO"> 22. /Imports.KO</h3>

Imports the KEGG reference pathway map and KEGG orthology data as mysql dumps.
**Prototype**: ``KEGG_tools.CLI::Int32 ImportsKODatabase(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Imports.KO /pathways <DIR> /KO <DIR> [/save <DIR>]
```
<h3 id="/Imports.SSDB"> 23. /Imports.SSDB</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 ImportsDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Imports.SSDB /in <source.DIR> [/out <ssdb.csv>]
```
<h3 id="/ko.index.sub.match"> 24. /ko.index.sub.match</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 IndexSubMatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /ko.index.sub.match /index <index.csv> /maps <maps.csv> /key <key> /map <mapTo> [/out <out.csv>]
```
<h3 id="/Maps.Repository.Build"> 25. /Maps.Repository.Build</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 BuildPathwayMapsRepository(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Maps.Repository.Build /imports <directory> [/out <repository.XML>]
```
<h3 id="/Organism.Table"> 26. /Organism.Table</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 KEGGOrganismTable(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Organism.Table [/in <br08601-htext.keg> /Bacteria /out <out.csv>]
```


#### Arguments
##### [/in]
If this kegg brite file is not presented in the cli arguments, the internal kegg resource will be used.

###### Example
```bash
/in <file, *.keg, *.txt>
# (This argument can accept the ``std_out`` from upstream app as input)
```
<h3 id="/Pathway.geneIDs"> 27. /Pathway.geneIDs</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 PathwayGeneList(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Pathway.geneIDs /in <pathway.XML> [/out <out.list.txt>]
```
<h3 id="/Pathway.Modules.Build"> 28. /Pathway.Modules.Build</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 CompileGenomePathwayModule(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Pathway.Modules.Build /in <directory> [/batch /out <out.Xml>]
```


#### Arguments
##### /in
A directory that created by ``/Download.Pathway.Maps`` command.

###### Example
```bash
/in <file/directory>
```
<h3 id="/Pathways.Downloads.All"> 29. /Pathways.Downloads.All</h3>

Download all of the KEGG reference pathway map data.
**Prototype**: ``KEGG_tools.CLI::Int32 DownloadsAllPathways(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Pathways.Downloads.All [/out <outDIR>]
```
<h3 id="/Query.KO"> 30. /Query.KO</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 QueryKOAnno(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Query.KO /in <blastnhits.csv> [/out <out.csv> /evalue 1e-5 /batch]
```
<h3 id="/show.organism"> 31. /show.organism</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 ShowOrganism(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /show.organism /code <kegg_sp> [/out <out.json>]
```
<h3 id="/Views.mod_stat"> 32. /Views.mod_stat</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 Stats(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools /Views.mod_stat /in <KEGG_Modules/Pathways_DIR> /locus <in.csv> [/locus_map Gene /pathway /out <out.csv>]
```
<h3 id="-Build.KO"> 33. -Build.KO</h3>

Download data from KEGG database to local server.
**Prototype**: ``KEGG_tools.CLI::Int32 BuildKEGGOrthology(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -Build.KO [/fill-missing]
```
<h3 id="Download.Sequence"> 34. Download.Sequence</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DownloadSequence(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools Download.Sequence /query <querySource.txt> [/out <outDIR> /source <existsDIR>]
```
<h3 id="--Dump.Db"> 35. --Dump.Db</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DumpDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools --Dump.Db /KEGG.Pathways <DIR> /KEGG.Modules <DIR> /KEGG.Reactions <DIR> /sp <sp.Code> /out <out.Xml>
```
<h3 id="-function.association.analysis"> 36. -function.association.analysis</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 FunctionAnalysis(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -function.association.analysis -i <matrix_csv>
```
<h3 id="--Get.KO"> 37. --Get.KO</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 GetKOAnnotation(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools --Get.KO /in <KASS-query.txt>
```
<h3 id="--part.from"> 38. --part.from</h3>

source and ref should be in KEGG annotation format.
**Prototype**: ``KEGG_tools.CLI::Int32 GetSource(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools --part.from /source <source.fasta> /ref <referenceFrom.fasta> [/out <out.fasta> /brief]
```
<h3 id="-query"> 39. -query</h3>

Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.
**Prototype**: ``KEGG_tools.CLI::Int32 QueryGenes(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -query -keyword <keyword> -o <out_dir>
```
<h3 id="-query.orthology"> 40. -query.orthology</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 QueryOrthology(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -query.orthology -keyword <gene_name> -o <output_csv>
```
<h3 id="-query.ref.map"> 41. -query.ref.map</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DownloadReferenceMap(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -query.ref.map -id <id> -o <out_dir>
```
<h3 id="-ref.map.download"> 42. -ref.map.download</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 DownloadReferenceMapDatabase(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -ref.map.download -o <out_dir>
```
<h3 id="-Table.Create"> 43. -Table.Create</h3>


**Prototype**: ``KEGG_tools.CLI::Int32 CreateTABLE(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
KEGG_tools -table.create -i <input_dir> -o <out_csv>
```


#### Arguments
##### -i
This parameter specific the source directory input of the download data.

###### Example
```bash
-i <term_string>
```
