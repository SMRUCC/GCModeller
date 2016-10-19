---
title: RegPrecise
tags: [maunal, tools]
date: 2016/10/19 16:38:35
---
# GCModeller [version 1.0.0.0]
> 

<!--more-->

**RegPrecise**
__
Copyright ?  2016

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/RegPrecise.exe
**Root namespace**: ``RegPrecise.CLI``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Build.Operons](#/Build.Operons)|If the /regprecise parameter is not presented, then you should install the regprecise in the GCModeller database repostiory first.|
|[/Build.Regulons.Batch](#/Build.Regulons.Batch)||
|[/CORN](#/CORN)|Join two vertices by edge if the correspondent operons: 
               i) are orthologous; 
               ii) have cantiodate transcription factor binding sites. 
               Collect all linked components. Two operons from two different genomes are called orthologous if they share at least one orthologous gene.|
|[/CORN.Batch](#/CORN.Batch)||
|[/CORN.thread](#/CORN.thread)||
|[/DOOR.Merge](#/DOOR.Merge)||
|[/Download.Motifs](#/Download.Motifs)||
|[/Download.Regprecise](#/Download.Regprecise)|Download Regprecise database from Web API|
|[/Effector.FillNames](#/Effector.FillNames)||
|[/Export.Regulators](#/Export.Regulators)|Exports all of the fasta sequence of the TF regulator from the download RegPrecsie FASTA database.|
|[/Family.Hits](#/Family.Hits)||
|[/Fasta.Downloads](#/Fasta.Downloads)|Download protein fasta sequence from KEGG database.|
|[/Fetches](#/Fetches)||
|[/Fetches.Thread](#/Fetches.Thread)||
|[/Gets.Sites.Genes](#/Gets.Sites.Genes)||
|[/heap.Supports](#/heap.Supports)||
|[/install.motifs](#/install.motifs)||
|[/Maps.Effector](#/Maps.Effector)||
|[/Merge.CORN](#/Merge.CORN)||
|[/Merge.RegPrecise.Fasta](#/Merge.RegPrecise.Fasta)||
|[/Prot_Motifs.EXPORT.pfamString](#/Prot_Motifs.EXPORT.pfamString)||
|[/Prot_Motifs.PfamString](#/Prot_Motifs.PfamString)||
|[/ProtMotifs.Downloads](#/ProtMotifs.Downloads)|Download protein domain motifs structures from KEGG ssdb.|
|[/Repository.Fetch](#/Repository.Fetch)||
|[/Rfam.Regulates](#/Rfam.Regulates)||
|[/Select.TF.BBH](#/Select.TF.BBH)||
|[/Select.TF.Pfam-String](#/Select.TF.Pfam-String)||
|[/siRNA.Maps](#/siRNA.Maps)||




## CLI API list
--------------------------
<h3 id="/Build.Operons"> 1. /Build.Operons</h3>

If the /regprecise parameter is not presented, then you should install the regprecise in the GCModeller database repostiory first.
**Prototype**: ``RegPrecise.CLI::Int32 OperonBuilder(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Build.Operons /bbh <bbh.csv> /PTT <genome.PTT> /TF-bbh <bbh.csv> [/tfHit_hash /out <out.csv> /regprecise <regprecise.Xml>]
```
###### Example
```bash
RegPrecise
```



#### Parameters information:
##### [/bbh]
The bbh result between the annotated genome And RegPrecise database.
This result was used for generates the operons, and query should be the genes in
the RegPrecise database and the hits is the genes in your annotated genome.

###### Example
```bash

```
##### Accepted Types
###### /bbh
<h3 id="/Build.Regulons.Batch"> 2. /Build.Regulons.Batch</h3>


**Prototype**: ``RegPrecise.CLI::Int32 RegulonBatchBuilder(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Build.Regulons.Batch /bbh <bbh.DIR> /PTT <PTT.DIR> /tf-bbh <tf-bbh.DIR> /regprecise <regprecise.Xml> [/num_threads <-1> /hits_hash /out <outDIR>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/CORN"> 3. /CORN</h3>

Join two vertices by edge if the correspondent operons:
i) are orthologous;
ii) have cantiodate transcription factor binding sites.
Collect all linked components. Two operons from two different genomes are called orthologous if they share at least one orthologous gene.
**Prototype**: ``RegPrecise.CLI::Int32 CORN(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /CORN /in <regulons.DIR> /motif-sites <motiflogs.csv.DIR> /sites <motiflogs.csv> /ref <regulons.Csv> [/out <out.csv>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/CORN.Batch"> 4. /CORN.Batch</h3>


**Prototype**: ``RegPrecise.CLI::Int32 CORNBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /CORN.Batch /sites <motiflogs.gff.sites.Csv.DIR> /regulons <regprecise.regulons.csv.DIR> [/name <name> /out <outDIR> /num_threads <-1> /null-regprecise]
```
###### Example
```bash
RegPrecise
```



#### Parameters information:
##### /sites

###### Example
```bash

```
##### /regulons

###### Example
```bash

```
##### [/name]


###### Example
```bash

```
##### Accepted Types
###### /sites
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifLog_
Example: 
```json
{
    "Complement": "System.String",
    "Ends": 0,
    "ID": "System.String",
    "SequenceData": "System.String",
    "Start": 0,
    "Strand": "System.String",
    "ATGDist": 0,
    "BiologicalProcess": "System.String",
    "Family": "System.String",
    "Location": "System.String",
    "Regulog": "System.String",
    "Taxonomy": "System.String",
    "tag": "System.String",
    "tags": {
        
    }
}
```

###### /regulons
**Decalre**:  _SMRUCC.genomics.Data.Regprecise.RegPreciseOperon_
Example: 
```json
{
    "BiologicalProcess": "System.String",
    "Effector": "System.String",
    "Operon": [
        "System.String"
    ],
    "Pathway": "System.String",
    "Regulators": [
        "System.String"
    ],
    "Strand": "System.String",
    "TF_trace": "System.String",
    "bbh": [
        "System.String"
    ],
    "source": "System.String"
}
```

###### /name
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

<h3 id="/CORN.thread"> 5. /CORN.thread</h3>


**Prototype**: ``RegPrecise.CLI::Int32 CORNSingleThread(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /CORN.thread /hit <regulons.Csv> /hit-sites <motiflogs.csv> /sites <query.motiflogs.csv> /ref <query.regulons.Csv> [/null-regprecise /out <out.csv>]
```
###### Example
```bash
RegPrecise
```



#### Parameters information:
##### /hit

###### Example
```bash

```
##### /hit-sites

###### Example
```bash

```
##### /sites

###### Example
```bash

```
##### /ref

###### Example
```bash

```
##### [/null-regprecise]
Does the motif log data have the RegPrecise database value? If this parameter is presented that which it means the site data have no RegPrecise data.

###### Example
```bash

```
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /hit
**Decalre**:  _SMRUCC.genomics.Data.Regprecise.RegPreciseOperon_
Example: 
```json
{
    "BiologicalProcess": "System.String",
    "Effector": "System.String",
    "Operon": [
        "System.String"
    ],
    "Pathway": "System.String",
    "Regulators": [
        "System.String"
    ],
    "Strand": "System.String",
    "TF_trace": "System.String",
    "bbh": [
        "System.String"
    ],
    "source": "System.String"
}
```

###### /hit-sites
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifLog_
Example: 
```json
{
    "Complement": "System.String",
    "Ends": 0,
    "ID": "System.String",
    "SequenceData": "System.String",
    "Start": 0,
    "Strand": "System.String",
    "ATGDist": 0,
    "BiologicalProcess": "System.String",
    "Family": "System.String",
    "Location": "System.String",
    "Regulog": "System.String",
    "Taxonomy": "System.String",
    "tag": "System.String",
    "tags": {
        
    }
}
```

###### /sites
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifLog_
Example: 
```json
{
    "Complement": "System.String",
    "Ends": 0,
    "ID": "System.String",
    "SequenceData": "System.String",
    "Start": 0,
    "Strand": "System.String",
    "ATGDist": 0,
    "BiologicalProcess": "System.String",
    "Family": "System.String",
    "Location": "System.String",
    "Regulog": "System.String",
    "Taxonomy": "System.String",
    "tag": "System.String",
    "tags": {
        
    }
}
```

###### /ref
**Decalre**:  _SMRUCC.genomics.Data.Regprecise.RegPreciseOperon_
Example: 
```json
{
    "BiologicalProcess": "System.String",
    "Effector": "System.String",
    "Operon": [
        "System.String"
    ],
    "Pathway": "System.String",
    "Regulators": [
        "System.String"
    ],
    "Strand": "System.String",
    "TF_trace": "System.String",
    "bbh": [
        "System.String"
    ],
    "source": "System.String"
}
```

###### /null-regprecise
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.MotifLog_
Example: 
```json
{
    "Complement": "System.String",
    "Ends": 0,
    "ID": "System.String",
    "SequenceData": "System.String",
    "Start": 0,
    "Strand": "System.String",
    "ATGDist": 0,
    "BiologicalProcess": "System.String",
    "Family": "System.String",
    "Location": "System.String",
    "Regulog": "System.String",
    "Taxonomy": "System.String",
    "tag": "System.String",
    "tags": {
        
    }
}
```

<h3 id="/DOOR.Merge"> 6. /DOOR.Merge</h3>


**Prototype**: ``RegPrecise.CLI::Int32 MergeDOOR(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /DOOR.Merge /in <operon.csv> /DOOR <genome.opr> [/out <out.opr>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Download.Motifs"> 7. /Download.Motifs</h3>


**Prototype**: ``RegPrecise.CLI::Int32 DownloadMotifSites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Download.Motifs /imports <RegPrecise.DIR> [/export <EXPORT_DIR>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Download.Regprecise"> 8. /Download.Regprecise</h3>

Download Regprecise database from Web API
**Prototype**: ``RegPrecise.CLI::Int32 DownloadRegprecise2(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise Download.Regprecise [/work ./ /save <saveXml>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Effector.FillNames"> 9. /Effector.FillNames</h3>


**Prototype**: ``RegPrecise.CLI::Int32 EffectorFillNames(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Effector.FillNames /in <effectors.csv> /compounds <metacyc.compounds> [/out <out.csv>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Export.Regulators"> 10. /Export.Regulators</h3>

Exports all of the fasta sequence of the TF regulator from the download RegPrecsie FASTA database.
**Prototype**: ``RegPrecise.CLI::Int32 ExportRegulators(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Export.Regulators /imports <regprecise.downloads.DIR> /Fasta <regprecise.fasta> [/locus-out /out <out.fasta>]
```
###### Example
```bash
RegPrecise
```



#### Parameters information:
##### [/locus-out]
Does the program saves a copy of the TF locus_tag list at the mean time of the TF fasta sequence export.

###### Example
```bash

```
##### Accepted Types
###### /locus-out
<h3 id="/Family.Hits"> 11. /Family.Hits</h3>


**Prototype**: ``RegPrecise.CLI::Int32 FamilyHits(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Family.Hits /bbh <bbh.csv> [/regprecise <RegPrecise.Xml> /pfamKey <query.pfam-string> /out <out.DIR>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Fasta.Downloads"> 12. /Fasta.Downloads</h3>

Download protein fasta sequence from KEGG database.
**Prototype**: ``RegPrecise.CLI::Int32 DownloadFasta(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise Fasta.Downloads /source <sourceDIR> [/out <outDIR> /keggTools <kegg.exe>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Fetches"> 13. /Fetches</h3>


**Prototype**: ``RegPrecise.CLI::Int32 Fetch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Fetches /ncbi <all_gbk.DIR> /imports <inDIR> /out <outDIR>
```
###### Example
```bash
RegPrecise
```
<h3 id="/Fetches.Thread"> 14. /Fetches.Thread</h3>


**Prototype**: ``RegPrecise.CLI::Int32 FetchThread(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Fetches.Thread /gbk <gbkDIR> /query <query.txt> /out <outDIR>
```
###### Example
```bash
RegPrecise
```
<h3 id="/Gets.Sites.Genes"> 15. /Gets.Sites.Genes</h3>


**Prototype**: ``RegPrecise.CLI::Int32 GetSites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Gets.Sites.Genes /in <tf.bbh.csv> /sites <motiflogs.csv> [/out <out.csv>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/heap.Supports"> 16. /heap.Supports</h3>


**Prototype**: ``RegPrecise.CLI::Int32 Supports(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /heap.supports /in <inDIR> [/out <out.Csv> /T /l]
```
###### Example
```bash
RegPrecise
```
<h3 id="/install.motifs"> 17. /install.motifs</h3>


**Prototype**: ``RegPrecise.CLI::Int32 InstallRegPreciseMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /install.motifs /imports <motifs.DIR>
```
###### Example
```bash
RegPrecise
```
<h3 id="/Maps.Effector"> 18. /Maps.Effector</h3>


**Prototype**: ``RegPrecise.CLI::Int32 Effectors(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Maps.Effector /imports <RegPrecise.DIR> [/out <out.csv>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Merge.CORN"> 19. /Merge.CORN</h3>


**Prototype**: ``RegPrecise.CLI::Int32 MergeCORN(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Merge.CORN /in <inDIR> [/out <outDIR>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Merge.RegPrecise.Fasta"> 20. /Merge.RegPrecise.Fasta</h3>


**Prototype**: ``RegPrecise.CLI::Int32 MergeDownload(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Merge.RegPrecise.Fasta [/in <inDIR> /out outDIR /offline]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Prot_Motifs.EXPORT.pfamString"> 21. /Prot_Motifs.EXPORT.pfamString</h3>


**Prototype**: ``RegPrecise.CLI::Int32 ProteinMotifsEXPORT(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Prot_Motifs.EXPORT.pfamString /in <motifs.json> /PTT <genome.ptt> [/out <pfam-string.csv>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Prot_Motifs.PfamString"> 22. /Prot_Motifs.PfamString</h3>


**Prototype**: ``RegPrecise.CLI::Int32 ProtMotifToPfamString(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Prot_Motifs.PfamString /in <RegPrecise.Download_DIR> [/fasta <RegPrecise.fasta> /out <pfam-string.csv>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/ProtMotifs.Downloads"> 23. /ProtMotifs.Downloads</h3>

Download protein domain motifs structures from KEGG ssdb.
**Prototype**: ``RegPrecise.CLI::Int32 DownloadProteinMotifs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /ProtMotifs.Downloads /source <source.DIR> [/kegg.Tools <./kegg.exe>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Repository.Fetch"> 24. /Repository.Fetch</h3>


**Prototype**: ``RegPrecise.CLI::Int32 FetchRepostiory(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Repository.Fetch /imports <RegPrecise.Xml> /genbank <NCBI_Genbank_DIR> [/full /out <outDIR>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Rfam.Regulates"> 25. /Rfam.Regulates</h3>


**Prototype**: ``RegPrecise.CLI::Int32 RfamRegulates(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Rfam.Regulates /in <RegPrecise.regulons.csv> /rfam <rfam_search.csv> [/out <out.csv>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Select.TF.BBH"> 26. /Select.TF.BBH</h3>


**Prototype**: ``RegPrecise.CLI::Int32 SelectTFBBH(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Select.TF.BBH /bbh <bbh.csv> /imports <RegPrecise.downloads.DIR> [/out <out.bbh.csv>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/Select.TF.Pfam-String"> 27. /Select.TF.Pfam-String</h3>


**Prototype**: ``RegPrecise.CLI::Int32 SelectTFPfams(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /Select.TF.Pfam-String /pfam-string <RegPrecise.pfam-string.csv> /imports <regprecise.downloads.DIR> [/out <TF.pfam-string.csv>]
```
###### Example
```bash
RegPrecise
```
<h3 id="/siRNA.Maps"> 28. /siRNA.Maps</h3>


**Prototype**: ``RegPrecise.CLI::Int32 siRNAMaps(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
RegPrecise /siRNA.Maps /in <siRNA.csv> /hits <blastn.csv> [/out <out.csv>]
```
###### Example
```bash
RegPrecise
```
