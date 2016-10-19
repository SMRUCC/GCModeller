---
title: seqtools
tags: [maunal, tools]
date: 2016/10/19 16:38:36
---
# GCModeller [version 3.0.2456.4506]
> Sequence operation utilities

<!--more-->

**Sequence search tools and sequence operation tools**
_Sequence search tools and sequence operation tools_
Copyright ? xie.guigang@gcmodeller.org 2014

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/seqtools.exe
**Root namespace**: ``seqtools.Utilities``

------------------------------------------------------------
If you are having trouble debugging this Error, first read the best practices tutorial for helpful tips that address many common problems:
> https://github.com/smrucc/gcmodeller/src/analysis/


The debugging facility Is helpful To figure out what's happening under the hood:
> http://gcmodeller.org


If you're still stumped, you can try get help from author directly from E-mail:
> xie.guigang@gcmodeller.org



All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Loci.describ](#/Loci.describ)|Testing|
|[/logo](#/logo)|* Drawing the sequence logo from the clustal alignment result.|
|[-321](#-321)|Polypeptide sequence 3 letters to 1 lettes sequence.|
|[-complement](#-complement)||
|[--Drawing.ClustalW](#--Drawing.ClustalW)||
|[-pattern_search](#-pattern_search)|Parsing the sequence segment from the sequence source using regular expression.|
|[-reverse](#-reverse)||
|[--translates](#--translates)|Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.|


##### 1. Fasta Sequence Tools
Tools command that works around the fasta format data.

|Function API|Info|
|------------|----|
|[/Compare.By.Locis](#/Compare.By.Locis)||
|[/Distinct](#/Distinct)|Distinct fasta sequence by sequence content.|
|[/Get.Locis](#/Get.Locis)||
|[/Gff.Sites](#/Gff.Sites)||
|[/Merge](#/Merge)|Only search for 1 level folder, dit not search receve.|
|[/Merge.Simple](#/Merge.Simple)|This tools just merge the fasta sequence into one larger file.|
|[/Select.By_Locus](#/Select.By_Locus)||
|[/Split](#/Split)||
|[/subset](#/subset)||
|[/To_Fasta](#/To_Fasta)|Convert the sequence data in a excel annotation file into a fasta sequence file.|
|[-segment](#-segment)||
|[--segments](#--segments)||
|[--Trim](#--Trim)||


##### 2. Sequence Palindrome Features Analysis
Tools command that using for finding Palindrome sites.

|Function API|Info|
|------------|----|
|[/Mirror.Batch](#/Mirror.Batch)||
|[/Mirror.Fuzzy](#/Mirror.Fuzzy)||
|[/Mirror.Fuzzy.Batch](#/Mirror.Fuzzy.Batch)||
|[/Mirror.Vector](#/Mirror.Vector)||
|[/Mirrors.Nt.Trim](#/Mirrors.Nt.Trim)||
|[/Palindrome.Screen.MaxMatches](#/Palindrome.Screen.MaxMatches)||
|[/Palindrome.Screen.MaxMatches.Batch](#/Palindrome.Screen.MaxMatches.Batch)||
|[--Hairpinks](#--Hairpinks)||
|[--Hairpinks.batch.task](#--Hairpinks.batch.task)||
|[--ImperfectsPalindrome.batch.Task](#--ImperfectsPalindrome.batch.Task)||
|[--Mirror.From.Fasta](#--Mirror.From.Fasta)|Mirror Palindrome, search from a fasta file.|
|[--Mirror.From.NT](#--Mirror.From.NT)|Mirror Palindrome, and this function is for the debugging test|
|[--Palindrome.batch.Task](#--Palindrome.batch.Task)||
|[--Palindrome.From.FASTA](#--Palindrome.From.FASTA)||
|[--Palindrome.From.NT](#--Palindrome.From.NT)|This function is just for debugger test, /nt parameter is the nucleotide sequence data as ATGCCCC|
|[--Palindrome.Imperfects](#--Palindrome.Imperfects)||
|[--PerfectPalindrome.Filtering](#--PerfectPalindrome.Filtering)||
|[--ToVector](#--ToVector)||


##### 3. Sequence Aligner


|Function API|Info|
|------------|----|
|[/align](#/align)||
|[/Clustal.Cut](#/Clustal.Cut)||
|[/gwANI](#/gwANI)||
|[/nw](#/nw)|RunNeedlemanWunsch|
|[/Sigma](#/Sigma)||
|[--align](#--align)||
|[--align.Self](#--align.Self)||


##### 4. Palindrome batch task tools


|Function API|Info|
|------------|----|
|[/check.attrs](#/check.attrs)||
|[/Palindrome.BatchTask](#/Palindrome.BatchTask)||
|[/Palindrome.Workflow](#/Palindrome.Workflow)||


##### 5. Nucleotide Sequence Property Calculation tools


|Function API|Info|
|------------|----|
|[/Mirrors.Context](#/Mirrors.Context)|This function will convert the mirror data to the simple segment object data|
|[/Mirrors.Context.Batch](#/Mirrors.Context.Batch)|This function will convert the mirror data to the simple segment object data|
|[/Mirrors.Group](#/Mirrors.Group)||
|[/Mirrors.Group.Batch](#/Mirrors.Group.Batch)||
|[/SimpleSegment.AutoBuild](#/SimpleSegment.AutoBuild)||
|[/SimpleSegment.Mirrors](#/SimpleSegment.Mirrors)||
|[/SimpleSegment.Mirrors.Batch](#/SimpleSegment.Mirrors.Batch)||


##### 6. SNP search tools


|Function API|Info|
|------------|----|
|[/SNP](#/SNP)||
|[/Time.Diffs](#/Time.Diffs)||


##### 7. Sequence Repeats Loci Search


|Function API|Info|
|------------|----|
|[/Write.Seeds](#/Write.Seeds)||
|[Repeats.Density](#Repeats.Density)||
|[rev-Repeats.Density](#rev-Repeats.Density)||
|[Search.Batch](#Search.Batch)|Batch search for repeats.|




## CLI API list
--------------------------
<h3 id="/align"> 1. /align</h3>


**Prototype**: ``seqtools.Utilities::Int32 Align2(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /align /query <query.fasta> /subject <subject.fasta> [/blosum <matrix.txt> /out <out.xml>]
```
###### Example
```bash
seqtools
```
<h3 id="/check.attrs"> 2. /check.attrs</h3>


**Prototype**: ``seqtools.Utilities::Int32 CheckHeaders(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /check.attrs /in <in.fasta> /n <attrs.count> [/all]
```
###### Example
```bash
seqtools
```
<h3 id="/Clustal.Cut"> 3. /Clustal.Cut</h3>


**Prototype**: ``seqtools.Utilities::Int32 CutMlAlignment(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Clustal.Cut /in <in.fasta> [/left 0.1 /right 0.1 /out <out.fasta>]
```
###### Example
```bash
seqtools
```
<h3 id="/Compare.By.Locis"> 4. /Compare.By.Locis</h3>


**Prototype**: ``seqtools.Utilities::Int32 CompareFile(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Compare.By.Locis /file1 <file1.fasta> /file2 </file2.fasta>
```
###### Example
```bash
seqtools
```
<h3 id="/Distinct"> 5. /Distinct</h3>

Distinct fasta sequence by sequence content.
**Prototype**: ``seqtools.Utilities::Int32 Distinct(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Distinct /in <in.fasta> [/out <out.fasta> /by_Uid <uid_regexp>]
```
###### Example
```bash
seqtools
```
<h3 id="/Get.Locis"> 6. /Get.Locis</h3>


**Prototype**: ``seqtools.Utilities::Int32 GetSimpleSegments(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Get.Locis /in <locis.csv> /nt <genome.nt.fasta> [/out <outDIR>]
```
###### Example
```bash
seqtools
```
<h3 id="/Gff.Sites"> 7. /Gff.Sites</h3>


**Prototype**: ``seqtools.Utilities::Int32 GffSites(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Gff.Sites /fna <genomic.fna> /gff <genome.gff> [/out <out.fasta>]
```
###### Example
```bash
seqtools
```
<h3 id="/gwANI"> 8. /gwANI</h3>


**Prototype**: ``seqtools.Utilities::Int32 gwANI(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /gwANI /in <in.fasta> [/fast /out <out.Csv>]
```
###### Example
```bash
seqtools
```
<h3 id="/Loci.describ"> 9. /Loci.describ</h3>

Testing
**Prototype**: ``seqtools.Utilities::Int32 LociDescript(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Loci.describ /ptt <genome-context.ptt> [/test <loci:randomize> /complement /unstrand]
```
###### Example
```bash
seqtools
```
<h3 id="/logo"> 10. /logo</h3>

* Drawing the sequence logo from the clustal alignment result.
**Prototype**: ``seqtools.Utilities::Int32 SequenceLogo(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /logo /in <clustal.fasta> [/out <out.png> /title ""]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /in
The file path of the clustal output fasta file.

###### Example
```bash

```
##### [/out]
The output sequence logo image file path. default is the same name as the input fasta sequence file.

###### Example
```bash

```
##### [/title]
The display title on the sequence logo, default is using the fasta file name.

###### Example
```bash

```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```json
[
    
]
```

###### /out
###### /title
<h3 id="/Merge"> 11. /Merge</h3>

Only search for 1 level folder, dit not search receve.
**Prototype**: ``seqtools.Utilities::Int32 Merge(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Merge /in <fasta.DIR> [/out <out.fasta> /trim /unique /ext <*.fasta> /brief]
```
###### Example
```bash
seqtools
```
<h3 id="/Merge.Simple"> 12. /Merge.Simple</h3>

This tools just merge the fasta sequence into one larger file.
**Prototype**: ``seqtools.Utilities::Int32 SimpleMerge(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Merge.Simple /in <DIR> [/exts <default:*.fasta,*.fa> /line.break 120 /out <out.fasta>]
```
###### Example
```bash
seqtools
```
<h3 id="/Mirror.Batch"> 13. /Mirror.Batch</h3>


**Prototype**: ``seqtools.Utilities::Int32 MirrorBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirror.Batch /nt <nt.fasta> [/out <out.csv> /mp /min <3> /max <20> /num_threads <-1>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /nt

###### Example
```bash

```
##### [/mp]
Calculation in the multiple process mode?

###### Example
```bash

```
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /nt
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```json
[
    
]
```

###### /mp
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "Start": 0
}
```

<h3 id="/Mirror.Fuzzy"> 14. /Mirror.Fuzzy</h3>


**Prototype**: ``seqtools.Utilities::Int32 FuzzyMirrors(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirror.Fuzzy /in <in.fasta> [/out <out.csv> /cut 0.6 /max-dist 6 /min 3 /max 20]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /in

###### Example
```bash

```
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaToken_
Example: 
```json
{
    "Extension": {
        "DynamicHash": {
            "Properties": {
                
            },
            "source": [
                
            ]
        }
    },
    "SequenceData": "System.String",
    "Attributes": [
        "System.String"
    ]
}
```

###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "Start": 0
}
```

<h3 id="/Mirror.Fuzzy.Batch"> 15. /Mirror.Fuzzy.Batch</h3>


**Prototype**: ``seqtools.Utilities::Int32 FuzzyMirrorsBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirror.Fuzzy.Batch /in <in.fasta/DIR> [/out <out.DIR> /cut 0.6 /max-dist 6 /min 3 /max 20 /num_threads <-1>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "Start": 0
}
```

<h3 id="/Mirror.Vector"> 16. /Mirror.Vector</h3>


**Prototype**: ``seqtools.Utilities::Int32 MirrorsVector(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirror.Vector /in <inDIR> /size <genome.size> [/out out.txt]
```
###### Example
```bash
seqtools
```
<h3 id="/Mirrors.Context"> 17. /Mirrors.Context</h3>

This function will convert the mirror data to the simple segment object data
**Prototype**: ``seqtools.Utilities::Int32 MirrorContext(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirrors.Context /in <mirrors.csv> /PTT <genome.ptt> [/trans /strand <+/-> /out <out.csv> /stranded /dist <500bp>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/trans]
Enable this option will using genome_size minus loci location for the location correction, only works in reversed strand.

###### Example
```bash

```
##### Accepted Types
###### /trans
<h3 id="/Mirrors.Context.Batch"> 18. /Mirrors.Context.Batch</h3>

This function will convert the mirror data to the simple segment object data
**Prototype**: ``seqtools.Utilities::Int32 MirrorContextBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirrors.Context.Batch /in <mirrors.csv.DIR> /PTT <genome.ptt.DIR> [/trans /strand <+/-> /out <out.csv> /stranded /dist <500bp> /num_threads -1]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/trans]
Enable this option will using genome_size minus loci location for the location correction, only works in reversed strand.

###### Example
```bash

```
##### Accepted Types
###### /trans
<h3 id="/Mirrors.Group"> 19. /Mirrors.Group</h3>


**Prototype**: ``seqtools.Utilities::Int32 MirrorGroups(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirrors.Group /in <mirrors.Csv> [/batch /fuzzy <-1> /out <out.DIR>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/fuzzy]
-1 means group sequence by string equals compared, and value of 0-1 means using string fuzzy compare.

###### Example
```bash

```
##### Accepted Types
###### /fuzzy
<h3 id="/Mirrors.Group.Batch"> 20. /Mirrors.Group.Batch</h3>


**Prototype**: ``seqtools.Utilities::Int32 MirrorGroupsBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirrors.Group.Batch /in <mirrors.DIR> [/fuzzy <-1> /out <out.DIR> /num_threads <-1>]
```
###### Example
```bash
seqtools
```
<h3 id="/Mirrors.Nt.Trim"> 21. /Mirrors.Nt.Trim</h3>


**Prototype**: ``seqtools.Utilities::Int32 TrimNtMirrors(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirrors.Nt.Trim /in <mirrors.Csv> [/out <out.Csv>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "Start": 0
}
```

<h3 id="/nw"> 22. /nw</h3>

RunNeedlemanWunsch
**Prototype**: ``seqtools.Utilities::Int32 NW(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /nw /query <query.fasta> /subject <subject.fasta> [/out <out.txt>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /query

###### Example
```bash

```
##### /subject

###### Example
```bash

```
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /query
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaToken_
Example: 
```json
{
    "Extension": {
        "DynamicHash": {
            "Properties": {
                
            },
            "source": [
                
            ]
        }
    },
    "SequenceData": "System.String",
    "Attributes": [
        "System.String"
    ]
}
```

###### /subject
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaToken_
Example: 
```json
{
    "Extension": {
        "DynamicHash": {
            "Properties": {
                
            },
            "source": [
                
            ]
        }
    },
    "SequenceData": "System.String",
    "Attributes": [
        "System.String"
    ]
}
```

###### /out
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

<h3 id="/Palindrome.BatchTask"> 23. /Palindrome.BatchTask</h3>


**Prototype**: ``seqtools.Utilities::Int32 PalindromeBatchTask(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Palindrome.BatchTask /in <in.DIR> [/num_threads 4 /min 3 /max 20 /min-appears 2 /cutoff <0.6> /Palindrome /max-dist <1000 (bp)> /partitions <-1> /out <out.DIR>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/Palindrome]
Only search for Palindrome, not includes the repeats data.

###### Example
```bash

```
##### Accepted Types
###### /Palindrome
<h3 id="/Palindrome.Screen.MaxMatches"> 24. /Palindrome.Screen.MaxMatches</h3>


**Prototype**: ``seqtools.Utilities::Int32 FilteringMatches(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Palindrome.Screen.MaxMatches /in <in.csv> /min <min.max-matches> [/out <out.csv>]
```
###### Example
```bash
seqtools
```
<h3 id="/Palindrome.Screen.MaxMatches.Batch"> 25. /Palindrome.Screen.MaxMatches.Batch</h3>


**Prototype**: ``seqtools.Utilities::Int32 FilteringMatchesBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Palindrome.Screen.MaxMatches.Batch /in <inDIR> /min <min.max-matches> [/out <out.DIR> /num_threads <-1>]
```
###### Example
```bash
seqtools
```
<h3 id="/Palindrome.Workflow"> 26. /Palindrome.Workflow</h3>


**Prototype**: ``seqtools.Utilities::Int32 PalindromeWorkflow(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Palindrome.Workflow /in <in.fasta> [/batch /min-appears 2 /min 3 /max 20 /cutoff <0.6> /max-dist <1000 (bp)> /Palindrome /partitions <-1> /out <out.DIR>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /in
This is a single sequence fasta file.

###### Example
```bash

```
##### [/Palindrome]
Only search for Palindrome, not includes the repeats data.

###### Example
```bash

```
##### Accepted Types
###### /in
###### /Palindrome
<h3 id="/Select.By_Locus"> 27. /Select.By_Locus</h3>


**Prototype**: ``seqtools.Utilities::Int32 SelectByLocus(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Select.By_Locus /in <locus.txt> /fa <fasta/.inDIR> [/out <out.fasta>]
```
###### Example
```bash
seqtools
```
<h3 id="/Sigma"> 28. /Sigma</h3>


**Prototype**: ``seqtools.Utilities::Int32 Sigma(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Sigma /in <in.fasta> [/out <out.Csv> /simple /round <-1>]
```
###### Example
```bash
seqtools
```
<h3 id="/SimpleSegment.AutoBuild"> 29. /SimpleSegment.AutoBuild</h3>


**Prototype**: ``seqtools.Utilities::Int32 ConvertsAuto(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /SimpleSegment.AutoBuild /in <locis.csv> [/out <out.csv>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /in

###### Example
```bash

```
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /in
**Decalre**:  _Microsoft.VisualBasic.Data.csv.DocumentStream.File_
Example: 
```json
[
    
]
```

###### /out
**Decalre**:  _SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment_
Example: 
```json
{
    "Complement": "System.String",
    "Ends": 0,
    "ID": "System.String",
    "SequenceData": "System.String",
    "Start": 0,
    "Strand": "System.String"
}
```

<h3 id="/SimpleSegment.Mirrors"> 30. /SimpleSegment.Mirrors</h3>


**Prototype**: ``seqtools.Utilities::Int32 ConvertMirrors(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /SimpleSegment.Mirrors /in <in.csv> [/out <out.csv>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /in

###### Example
```bash

```
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "Start": 0
}
```

###### /out
**Decalre**:  _SMRUCC.genomics.SequenceModel.NucleotideModels.SimpleSegment_
Example: 
```json
{
    "Complement": "System.String",
    "Ends": 0,
    "ID": "System.String",
    "SequenceData": "System.String",
    "Start": 0,
    "Strand": "System.String"
}
```

<h3 id="/SimpleSegment.Mirrors.Batch"> 31. /SimpleSegment.Mirrors.Batch</h3>


**Prototype**: ``seqtools.Utilities::Int32 ConvertMirrorsBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /SimpleSegment.Mirrors.Batch /in <in.DIR> [/out <out.DIR>]
```
###### Example
```bash
seqtools
```
<h3 id="/SNP"> 32. /SNP</h3>


**Prototype**: ``seqtools.Utilities::Int32 SNP(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /SNP /in <nt.fasta> [/ref 0 /pure /monomorphic]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /in


###### Example
```bash

```
##### [/ref]

###### Example
```bash

```
##### [/pure]

###### Example
```bash

```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```json
[
    
]
```

###### /ref
**Decalre**:  _System.Int32_
Example: 
```json
0
```

###### /pure
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/Split"> 33. /Split</h3>


**Prototype**: ``seqtools.Utilities::Int32 Split(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Split /in <in.fasta> [/n <4096> /out <outDIR>]
```
###### Example
```bash
seqtools
```
<h3 id="/subset"> 34. /subset</h3>


**Prototype**: ``seqtools.Utilities::Int32 SubSet(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /subset /lstID <lstID.txt> /fa <source.fasta>
```
###### Example
```bash
seqtools
```
<h3 id="/Time.Diffs"> 35. /Time.Diffs</h3>


**Prototype**: ``seqtools.Utilities::Int32 TimeDiffs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Time.Diffs /in <aln.fasta> [/out <out.csv>]
```
###### Example
```bash
seqtools
```
<h3 id="/To_Fasta"> 36. /To_Fasta</h3>

Convert the sequence data in a excel annotation file into a fasta sequence file.
**Prototype**: ``seqtools.Utilities::Int32 ToFasta(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /To_Fasta /in <anno.csv> [/out <out.fasta> /attrs <gene;locus_tag;gi;location,...> /seq <Sequence>]
```
###### Example
```bash
seqtools
```
<h3 id="/Write.Seeds"> 37. /Write.Seeds</h3>


**Prototype**: ``seqtools.Utilities::Int32 WriteSeeds(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Write.Seeds /out <out.dat> [/prot /max <20>]
```
###### Example
```bash
seqtools
```
<h3 id="-321"> 38. -321</h3>

Polypeptide sequence 3 letters to 1 lettes sequence.
**Prototype**: ``seqtools.Utilities::Int32 PolypeptideBriefs(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools -321 /in <sequence.txt> [/out <out.fasta>]
```
###### Example
```bash
seqtools
```
<h3 id="--align"> 39. --align</h3>


**Prototype**: ``seqtools.Utilities::Int32 Align(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --align /query <query.fasta> /subject <subject.fasta> [/out <out.DIR> /cost <0.7>]
```
###### Example
```bash
seqtools
```
<h3 id="--align.Self"> 40. --align.Self</h3>


**Prototype**: ``seqtools.Utilities::Int32 AlignSelf(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --align.Self /query <query.fasta> /out <out.DIR> [/cost 0.75]
```
###### Example
```bash
seqtools
```
<h3 id="-complement"> 41. -complement</h3>


**Prototype**: ``seqtools.Utilities::Int32 Complement(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools -complement -i <input_fasta> [-o <output_fasta>]
```
###### Example
```bash
seqtools
```
<h3 id="--Drawing.ClustalW"> 42. --Drawing.ClustalW</h3>


**Prototype**: ``seqtools.Utilities::Int32 DrawClustalW(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Drawing.ClustalW /in <align.fasta> [/out <out.png> /dot.Size 10]
```
###### Example
```bash
seqtools
```
<h3 id="--Hairpinks"> 43. --Hairpinks</h3>


**Prototype**: ``seqtools.Utilities::Int32 Hairpinks(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Hairpinks /in <in.fasta> [/out <out.csv> /min <6> /max <7> /cutoff 3 /max-dist <35 (bp)>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.ImperfectPalindrome_
Example: 
```json
{
    "Distance": 0,
    "Evolr": "System.String",
    "Left": 0,
    "Matches": "System.String",
    "MaxMatch": 0,
    "Palindrome": "System.String",
    "Paloci": 0,
    "Score": 0,
    "Site": "System.String"
}
```

<h3 id="--Hairpinks.batch.task"> 44. --Hairpinks.batch.task</h3>


**Prototype**: ``seqtools.Utilities::Int32 HairpinksBatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Hairpinks.batch.task /in <in.fasta> [/out <outDIR> /min <6> /max <7> /cutoff <0.6> /max-dist <35 (bp)> /num_threads <-1>]
```
###### Example
```bash
seqtools
```
<h3 id="--ImperfectsPalindrome.batch.Task"> 45. --ImperfectsPalindrome.batch.Task</h3>


**Prototype**: ``seqtools.Utilities::Int32 BatchSearchImperfectsPalindrome(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --ImperfectsPalindrome.batch.Task /in <in.fasta> /out <outDir> [/min <3> /max <20> /cutoff <0.6> /max-dist <1000 (bp)> /num_threads <-1>]
```
###### Example
```bash
seqtools
```
<h3 id="--Mirror.From.Fasta"> 46. --Mirror.From.Fasta</h3>

Mirror Palindrome, search from a fasta file.
**Prototype**: ``seqtools.Utilities::Int32 SearchMirrotFasta(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Mirror.From.Fasta /nt <nt-sequence.fasta> [/out <out.csv> /min <3> /max <20>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /nt
This fasta file should contains only just one sequence.

###### Example
```bash

```
##### Accepted Types
###### /nt
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaToken_
Example: 
```json
{
    "Extension": {
        "DynamicHash": {
            "Properties": {
                
            },
            "source": [
                
            ]
        }
    },
    "SequenceData": "System.String",
    "Attributes": [
        "System.String"
    ]
}
```

<h3 id="--Mirror.From.NT"> 47. --Mirror.From.NT</h3>

Mirror Palindrome, and this function is for the debugging test
**Prototype**: ``seqtools.Utilities::Int32 SearchMirrotNT(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Mirror.From.NT /nt <nt-sequence> /out <out.csv> [/min <3> /max <20>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "Start": 0
}
```

<h3 id="--Palindrome.batch.Task"> 48. --Palindrome.batch.Task</h3>


**Prototype**: ``seqtools.Utilities::Int32 BatchSearchPalindrome(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Palindrome.batch.Task /in <in.fasta> /out <outDir> [/min <3> /max <20> /num_threads <-1>]
```
###### Example
```bash
seqtools
```
<h3 id="--Palindrome.From.FASTA"> 49. --Palindrome.From.FASTA</h3>


**Prototype**: ``seqtools.Utilities::Int32 SearchPalindromeFasta(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Palindrome.From.Fasta /nt <nt-sequence.fasta> [/out <out.csv> /min <3> /max <20>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /nt
Fasta sequence file, and this file should just contains only one sequence.

###### Example
```bash

```
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /nt
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaToken_
Example: 
```json
{
    "Extension": {
        "DynamicHash": {
            "Properties": {
                
            },
            "source": [
                
            ]
        }
    },
    "SequenceData": "System.String",
    "Attributes": [
        "System.String"
    ]
}
```

###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "Start": 0
}
```

<h3 id="--Palindrome.From.NT"> 50. --Palindrome.From.NT</h3>

This function is just for debugger test, /nt parameter is the nucleotide sequence data as ATGCCCC
**Prototype**: ``seqtools.Utilities::Int32 SearchPalindromeNT(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Palindrome.From.NT /nt <nt-sequence> /out <out.csv> [/min <3> /max <20>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "Start": 0
}
```

<h3 id="--Palindrome.Imperfects"> 51. --Palindrome.Imperfects</h3>


**Prototype**: ``seqtools.Utilities::Int32 ImperfectPalindrome(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Palindrome.Imperfects /in <in.fasta> [/out <out.csv> /min <3> /max <20> /cutoff <0.6> /max-dist <1000 (bp)> /partitions <-1>]
```
###### Example
```bash
seqtools
```
<h3 id="-pattern_search"> 52. -pattern_search</h3>

Parsing the sequence segment from the sequence source using regular expression.
**Prototype**: ``seqtools.Utilities::Int32 PatternSearchA(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools -pattern_search -i <file_name> -p <regex_pattern>[ -o <output_directory> -f <format:fsa/gbk>]
```
###### Example
```bash
seqtools -pattern_search -i ~/xcc8004.txt -p TTA{3}N{1,2} -f fsa
```



#### Parameters information:
##### -i
The sequence input data source file, it can be a fasta or genbank file.

###### Example
```bash
~/Desktop/xcc8004.txt
```
##### -p
This switch specific the regular expression pattern for search the sequence segment,
for more detail information about the regular expression please read the user manual.

###### Example
```bash
N{1,5}TA
```
##### [-o]
Optional, this switch value specific the output directory for the result data, default is user Desktop folder.

###### Example
```bash
~/Documents/
```
##### [-f]
Optional, specific the input file format for the sequence reader, default value is FASTA sequence file.
fsa - The input sequence data file is a FASTA format file;
gbk - The input sequence data file is a NCBI genbank flat file.

###### Example
```bash
fsa
```
##### Accepted Types
###### -i
###### -p
###### -o
###### -f
<h3 id="--PerfectPalindrome.Filtering"> 53. --PerfectPalindrome.Filtering</h3>


**Prototype**: ``seqtools.Utilities::Int32 FilterPerfectPalindrome(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --PerfectPalindrome.Filtering /in <inDIR> [/min <8> /out <outDIR>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.ImperfectPalindrome_
Example: 
```json
{
    "Distance": 0,
    "Evolr": "System.String",
    "Left": 0,
    "Matches": "System.String",
    "MaxMatch": 0,
    "Palindrome": "System.String",
    "Paloci": 0,
    "Score": 0,
    "Site": "System.String"
}
```

<h3 id="Repeats.Density"> 54. Repeats.Density</h3>


**Prototype**: ``seqtools.Utilities::Int32 RepeatsDensity(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools Repeats.Density /dir <dir> /size <size> /ref <refName> [/out <out.csv> /cutoff <default:=0>]
```
###### Example
```bash
seqtools
```
<h3 id="-reverse"> 55. -reverse</h3>


**Prototype**: ``seqtools.Utilities::Int32 Reverse(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools -reverse -i <input_fasta> [-o <output_fasta>]
```
###### Example
```bash
seqtools
```
<h3 id="rev-Repeats.Density"> 56. rev-Repeats.Density</h3>


**Prototype**: ``seqtools.Utilities::Int32 revRepeatsDensity(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools rev-Repeats.Density /dir <dir> /size <size> /ref <refName> [/out <out.csv> /cutoff <default:=0>]
```
###### Example
```bash
seqtools
```
<h3 id="Search.Batch"> 57. Search.Batch</h3>

Batch search for repeats.
**Prototype**: ``seqtools.Utilities::Int32 BatchSearch(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools Search.Batch /aln <alignment.fasta> [/min 3 /max 20 /min-rep 2 /out <./>]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /aln
The input fasta file should be the output of the clustal multiple alignment fasta output.

###### Example
```bash

```
##### [/out]

###### Example
```bash

```
##### Accepted Types
###### /aln
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsView_
Example: 
```json
{
    "Left": 0,
    "Locis": [
        0
    ],
    "SequenceData": "System.String"
}
```

**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RevRepeatsView_
Example: 
```json
{
    "Left": 0,
    "Locis": [
        0
    ],
    "SequenceData": "System.String",
    "RevLocis": [
        0
    ],
    "RevSegment": "System.String"
}
```

<h3 id="-segment"> 58. -segment</h3>


**Prototype**: ``seqtools.Utilities::Int32 GetSegment(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools -segment /fasta <Fasta_Token> [-loci <loci>] [/left <left> /length <length> /right <right> [/reverse]] [/ptt <ptt> /geneID <gene_id> /dist <distance> /downstream] -o <saved> [-line.break 100]
```
###### Example
```bash
seqtools
```
<h3 id="--segments"> 59. --segments</h3>


**Prototype**: ``seqtools.Utilities::Int32 GetSegments(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --segments /regions <regions.csv> /fasta <nt.fasta> [/complement /reversed /brief-dump]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/reversed]
If the sequence is on the complement strand, reversed it after complement operation?

###### Example
```bash

```
##### [/complement]
If this Boolean switch is set on, then all of the reversed strand segment will be complemenet and reversed.

###### Example
```bash

```
##### [/brief-dump]
If this parameter is set up true, then only the locus_tag of the ORF gene will be dump to the fasta sequence.

###### Example
```bash

```
##### Accepted Types
###### /reversed
###### /complement
###### /brief-dump
<h3 id="--ToVector"> 60. --ToVector</h3>


**Prototype**: ``seqtools.Utilities::Int32 ToVector(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --ToVector /in <in.DIR> /min <4> /max <8> /out <out.txt> /size <genome.size>
```
###### Example
```bash
seqtools
```
<h3 id="--translates"> 61. --translates</h3>

Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.
**Prototype**: ``seqtools.Utilities::Int32 Translates(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --translates /orf <orf.fasta> [/transl_table 1 /force]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### /orf
ORF gene nt sequence should be completely complement and reversed as forwards strand if it is complement strand.

###### Example
```bash

```
##### [/force]
This force parameter will force the translation program ignore of the stop code and continute sequence translation.

###### Example
```bash

```
##### [/transl_table]
Available index value was described at http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25

###### Example
```bash

```
##### Accepted Types
###### /orf
###### /force
###### /transl_table
<h3 id="--Trim"> 62. --Trim</h3>


**Prototype**: ``seqtools.Utilities::Int32 Trim(Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Trim /in <in.fasta> [/case <u/l> /break <-1/int> /out <out.fasta> /brief]
```
###### Example
```bash
seqtools
```



#### Parameters information:
##### [/case]
Adjust the letter case of your sequence, l for lower case and u for upper case. Default value is upper case.

###### Example
```bash

```
##### [/break]
Adjust the sequence break when this program write the fasta sequence, default is -1 which means no break, write all sequence in one line.

###### Example
```bash

```
##### Accepted Types
###### /case
###### /break
