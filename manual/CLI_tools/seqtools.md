---
title: seqtools
tags: [maunal, tools]
date: 5/28/2018 8:37:25 PM
---
# GCModeller [version 3.0.2456.4506]
> Sequence operation utilities

<!--more-->

**Sequence search tools and sequence operation tools**<br/>
_Sequence search tools and sequence operation tools_<br/>
Copyright © xie.guigang@gcmodeller.org 2018

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/seqtools.exe<br/>
**Root namespace**: ``seqtools.Utilities``<br/>

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
|[/Count](#/Count)||
|[/Fasta.Subset.Large](#/Fasta.Subset.Large)||
|[/Genotype](#/Genotype)||
|[/Genotype.Statics](#/Genotype.Statics)||
|[/Loci.describ](#/Loci.describ)|Testing|
|[/logo](#/logo)|* Drawing the sequence logo from the clustal alignment result.|
|[/motifs](#/motifs)|Populate possible motifs from a give nt fasta sequence dataset.|
|[/NeedlemanWunsch.NT](#/NeedlemanWunsch.NT)||
|[/Promoter.Palindrome.Fasta](#/Promoter.Palindrome.Fasta)||
|[/Promoter.Regions.Palindrome](#/Promoter.Regions.Palindrome)||
|[/Promoter.Regions.Parser.gb](#/Promoter.Regions.Parser.gb)||
|[/Rule.dnaA_gyrB](#/Rule.dnaA_gyrB)||
|[/Rule.dnaA_gyrB.Matrix](#/Rule.dnaA_gyrB.Matrix)||
|[/Screen.sites](#/Screen.sites)||
|[/Sites2Fasta](#/Sites2Fasta)|Converts the simple segment object collection as fasta file.|
|[/SSR](#/SSR)|Search for SSR on a nt sequence.|
|[-321](#-321)|Polypeptide sequence 3 letters to 1 lettes sequence.|
|[-complement](#-complement)||
|[--Drawing.ClustalW](#--Drawing.ClustalW)||
|[-pattern_search](#-pattern_search)|Parsing the sequence segment from the sequence source using regular expression.|
|[-reverse](#-reverse)||
|[--translates](#--translates)|Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.|


##### 1. DNA_Comparative tools


|Function API|Info|
|------------|----|
|[/CAI](#/CAI)||
|[/gwANI](#/gwANI)||
|[/Sigma](#/Sigma)||


##### 2. Fasta Sequence Tools

Tools command that works around the fasta format data.


|Function API|Info|
|------------|----|
|[/Compare.By.Locis](#/Compare.By.Locis)||
|[/Distinct](#/Distinct)|Distinct fasta sequence by sequence content.|
|[/Excel.2Fasta](#/Excel.2Fasta)|Convert the sequence data in a excel annotation file into a fasta sequence file.|
|[/Get.Locis](#/Get.Locis)||
|[/Gff.Sites](#/Gff.Sites)||
|[/Merge](#/Merge)|Only search for 1 level folder, dit not search receve.|
|[/Merge.Simple](#/Merge.Simple)|This tools just merge the fasta sequence into one larger file.|
|[/Select.By_Locus](#/Select.By_Locus)|Select fasta sequence by local_tag.|
|[/Split](#/Split)||
|[/subset](#/subset)||
|[-segment](#-segment)||
|[--segments](#--segments)||
|[--Trim](#--Trim)||


##### 3. Nucleotide Sequence Property Calculation tools


|Function API|Info|
|------------|----|
|[/Mirrors.Context](#/Mirrors.Context)|This function will convert the mirror data to the simple segment object data|
|[/Mirrors.Context.Batch](#/Mirrors.Context.Batch)|This function will convert the mirror data to the simple segment object data|
|[/Mirrors.Group](#/Mirrors.Group)||
|[/Mirrors.Group.Batch](#/Mirrors.Group.Batch)||
|[/SimpleSegment.AutoBuild](#/SimpleSegment.AutoBuild)||
|[/SimpleSegment.Mirrors](#/SimpleSegment.Mirrors)||
|[/SimpleSegment.Mirrors.Batch](#/SimpleSegment.Mirrors.Batch)||


##### 4. Palindrome batch task tools


|Function API|Info|
|------------|----|
|[/check.attrs](#/check.attrs)||
|[/Palindrome.BatchTask](#/Palindrome.BatchTask)||
|[/Palindrome.Workflow](#/Palindrome.Workflow)||


##### 5. Sequence Aligner


|Function API|Info|
|------------|----|
|[/align.SmithWaterman](#/align.SmithWaterman)||
|[/Clustal.Cut](#/Clustal.Cut)||
|[/nw](#/nw)|RunNeedlemanWunsch|
|[--align](#--align)||
|[--align.Self](#--align.Self)||


##### 6. Sequence Palindrome Features Analysis

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
|[--Palindrome.Imperfects](#--Palindrome.Imperfects)|Gets all partly matched palindrome sites.|
|[--PerfectPalindrome.Filtering](#--PerfectPalindrome.Filtering)||
|[--ToVector](#--ToVector)||


##### 7. Sequence Repeats Loci Search


|Function API|Info|
|------------|----|
|[/Write.Seeds](#/Write.Seeds)||
|[Repeats.Density](#Repeats.Density)||
|[rev-Repeats.Density](#rev-Repeats.Density)||
|[Search.Batch](#Search.Batch)|Batch search for repeats.|


##### 8. SNP search tools


|Function API|Info|
|------------|----|
|[/SNP](#/SNP)||
|[/Time.Mutation](#/Time.Mutation)|The ongoing time mutation of the genome sequence.|

## CLI API list
--------------------------
<h3 id="/align.SmithWaterman"> 1. /align.SmithWaterman</h3>


**Prototype**: ``seqtools.Utilities::Int32 Align2(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /align.SmithWaterman /query <query.fasta> /subject <subject.fasta> [/blosum <matrix.txt> /out <out.xml>]
```
<h3 id="/CAI"> 2. /CAI</h3>


**Prototype**: ``seqtools.Utilities::Int32 CAI(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /CAI /ORF <orf_nt.fasta> [/out <out.XML>]
```


#### Arguments
##### /ORF
If the target fasta file contains multiple sequence, then the CAI table xml will output to a folder or just output to a xml file if only one sequence in thye fasta file.

###### Example
```bash
/ORF <file/directory>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### Accepted Types
###### /ORF
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaSeq_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

<h3 id="/check.attrs"> 3. /check.attrs</h3>


**Prototype**: ``seqtools.Utilities::Int32 CheckHeaders(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /check.attrs /in <in.fasta> /n <attrs.count> [/all]
```
<h3 id="/Clustal.Cut"> 4. /Clustal.Cut</h3>


**Prototype**: ``seqtools.Utilities::Int32 CutMlAlignment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Clustal.Cut /in <in.fasta> [/left 0.1 /right 0.1 /out <out.fasta>]
```
<h3 id="/Compare.By.Locis"> 5. /Compare.By.Locis</h3>


**Prototype**: ``seqtools.Utilities::Int32 CompareFile(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Compare.By.Locis /file1 <file1.fasta> /file2 </file2.fasta>
```
<h3 id="/Count"> 6. /Count</h3>


**Prototype**: ``seqtools.Utilities::Int32 Count(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Count /in <data.fasta>
```
<h3 id="/Distinct"> 7. /Distinct</h3>

Distinct fasta sequence by sequence content.
**Prototype**: ``seqtools.Utilities::Int32 Distinct(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Distinct /in <in.fasta> [/out <out.fasta> /by_Uid <uid_regexp>]
```
<h3 id="/Excel.2Fasta"> 8. /Excel.2Fasta</h3>

Convert the sequence data in a excel annotation file into a fasta sequence file.
**Prototype**: ``seqtools.Utilities::Int32 ToFasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Excel.2Fasta /in <anno.csv> [/out <out.fasta> /attrs <gene;locus_tag;gi;location,...> /seq <Sequence>]
```


#### Arguments
##### /in
Excel csv table file.

###### Example
```bash
/in <term_string>
```
##### /attrs
Excel header fields name as the fasta sequence header.

###### Example
```bash
/attrs <term_string>
```
##### /seq
Excel header field name for reading the sequence data.

###### Example
```bash
/seq <term_string>
```
<h3 id="/Fasta.Subset.Large"> 9. /Fasta.Subset.Large</h3>


**Prototype**: ``seqtools.Utilities::Int32 SubsetFastaDb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Fasta.Subset.Large /in <locus.txt> /db <large_db.fasta> [/keyword.map.multiple /out <out.fasta>]
```
<h3 id="/Genotype"> 10. /Genotype</h3>


**Prototype**: ``seqtools.Utilities::Int32 Genotype(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Genotype /in <raw.csv> [/out <out.Csv>]
```
<h3 id="/Genotype.Statics"> 11. /Genotype.Statics</h3>


**Prototype**: ``seqtools.Utilities::Int32 GenotypeStatics(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Genotype.Statics /in <in.DIR> [/out <EXPORT>]
```
<h3 id="/Get.Locis"> 12. /Get.Locis</h3>


**Prototype**: ``seqtools.Utilities::Int32 GetSimpleSegments(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Get.Locis /in <locis.csv> /nt <genome.nt.fasta> [/out <outDIR>]
```
<h3 id="/Gff.Sites"> 13. /Gff.Sites</h3>


**Prototype**: ``seqtools.Utilities::Int32 GffSites(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Gff.Sites /fna <genomic.fna> /gff <genome.gff> [/out <out.fasta>]
```
<h3 id="/gwANI"> 14. /gwANI</h3>


**Prototype**: ``seqtools.Utilities::Int32 gwANI(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /gwANI /in <in.fasta> [/fast /out <out.Csv>]
```
<h3 id="/Loci.describ"> 15. /Loci.describ</h3>

Testing
**Prototype**: ``seqtools.Utilities::Int32 LociDescript(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Loci.describ /ptt <genome-context.ptt> [/test <loci:randomize> /complement /unstrand]
```
<h3 id="/logo"> 16. /logo</h3>

* Drawing the sequence logo from the clustal alignment result.
**Prototype**: ``seqtools.Utilities::Int32 SequenceLogo(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /logo /in <clustal.fasta> [/out <out.png> /title ""]
```


#### Arguments
##### /in
The file path of the clustal output fasta file.

###### Example
```bash
/in <term_string>
```
##### [/out]
The output sequence logo image file path. default is the same name as the input fasta sequence file.

###### Example
```bash
/out <term_string>
```
##### [/title]
The display title on the sequence logo, default is using the fasta file name.

###### Example
```bash
/title <term_string>
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

<h3 id="/Merge"> 17. /Merge</h3>

Only search for 1 level folder, dit not search receve.
**Prototype**: ``seqtools.Utilities::Int32 Merge(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Merge /in <fasta.DIR> [/out <out.fasta> /trim /unique /ext <*.fasta> /brief]
```
<h3 id="/Merge.Simple"> 18. /Merge.Simple</h3>

This tools just merge the fasta sequence into one larger file.
**Prototype**: ``seqtools.Utilities::Int32 SimpleMerge(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Merge.Simple /in <DIR> [/exts <default:*.fasta,*.fa> /line.break 120 /out <out.fasta>]
```
<h3 id="/Mirror.Batch"> 19. /Mirror.Batch</h3>


**Prototype**: ``seqtools.Utilities::Int32 MirrorBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirror.Batch /nt <nt.fasta> [/out <out.csv> /mp /min <3> /max <20> /num_threads <-1>]
```


#### Arguments
##### /nt

###### Example
```bash
/nt <term_string>
```
##### [/mp]
Calculation in the multiple process mode?

###### Example
```bash
/mp <term_string>
```
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /nt
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
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
    "Data": {
        "System.String": "System.String"
    },
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "SequenceData": "System.String",
    "Start": 0
}
```

<h3 id="/Mirror.Fuzzy"> 20. /Mirror.Fuzzy</h3>


**Prototype**: ``seqtools.Utilities::Int32 FuzzyMirrors(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirror.Fuzzy /in <in.fasta> [/out <out.csv> /cut 0.6 /max-dist 6 /min 3 /max 20]
```


#### Arguments
##### /in

###### Example
```bash
/in <term_string>
```
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaSeq_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "SequenceData": "System.String",
    "Start": 0
}
```

<h3 id="/Mirror.Fuzzy.Batch"> 21. /Mirror.Fuzzy.Batch</h3>


**Prototype**: ``seqtools.Utilities::Int32 FuzzyMirrorsBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirror.Fuzzy.Batch /in <in.fasta/DIR> [/out <out.DIR> /cut 0.6 /max-dist 6 /min 3 /max 20 /num_threads <-1>]
```


#### Arguments
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "SequenceData": "System.String",
    "Start": 0
}
```

<h3 id="/Mirror.Vector"> 22. /Mirror.Vector</h3>


**Prototype**: ``seqtools.Utilities::Int32 MirrorsVector(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirror.Vector /in <inDIR> /size <genome.size> [/out out.txt]
```
<h3 id="/Mirrors.Context"> 23. /Mirrors.Context</h3>

This function will convert the mirror data to the simple segment object data
**Prototype**: ``seqtools.Utilities::Int32 MirrorContext(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirrors.Context /in <mirrors.csv> /PTT <genome.ptt> [/trans /strand <+/-> /out <out.csv> /stranded /dist <500bp>]
```


#### Arguments
##### [/trans]
Enable this option will using genome_size minus loci location for the location correction, only works in reversed strand.

###### Example
```bash
/trans <term_string>
```
<h3 id="/Mirrors.Context.Batch"> 24. /Mirrors.Context.Batch</h3>

This function will convert the mirror data to the simple segment object data
**Prototype**: ``seqtools.Utilities::Int32 MirrorContextBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirrors.Context.Batch /in <mirrors.csv.DIR> /PTT <genome.ptt.DIR> [/trans /strand <+/-> /out <out.csv> /stranded /dist <500bp> /num_threads -1]
```


#### Arguments
##### [/trans]
Enable this option will using genome_size minus loci location for the location correction, only works in reversed strand.

###### Example
```bash
/trans <term_string>
```
<h3 id="/Mirrors.Group"> 25. /Mirrors.Group</h3>


**Prototype**: ``seqtools.Utilities::Int32 MirrorGroups(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirrors.Group /in <mirrors.Csv> [/batch /fuzzy <-1> /out <out.DIR>]
```


#### Arguments
##### [/fuzzy]
-1 means group sequence by string equals compared, and value of 0-1 means using string fuzzy compare.

###### Example
```bash
/fuzzy <term_string>
```
<h3 id="/Mirrors.Group.Batch"> 26. /Mirrors.Group.Batch</h3>


**Prototype**: ``seqtools.Utilities::Int32 MirrorGroupsBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirrors.Group.Batch /in <mirrors.DIR> [/fuzzy <-1> /out <out.DIR> /num_threads <-1>]
```
<h3 id="/Mirrors.Nt.Trim"> 27. /Mirrors.Nt.Trim</h3>


**Prototype**: ``seqtools.Utilities::Int32 TrimNtMirrors(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Mirrors.Nt.Trim /in <mirrors.Csv> [/out <out.Csv>]
```


#### Arguments
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "SequenceData": "System.String",
    "Start": 0
}
```

<h3 id="/motifs"> 28. /motifs</h3>

Populate possible motifs from a give nt fasta sequence dataset.
**Prototype**: ``seqtools.Utilities::Int32 FindMotifs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /motifs /in <data.fasta> [/min.w <default=6> /max.w <default=20> /n.motifs <default=25> /n.occurs <default=6> /out <out.directory>]
```
<h3 id="/NeedlemanWunsch.NT"> 29. /NeedlemanWunsch.NT</h3>


**Prototype**: ``seqtools.Utilities::Int32 NWNT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /NeedlemanWunsch.NT /query <nt> /subject <nt>
```
<h3 id="/nw"> 30. /nw</h3>

RunNeedlemanWunsch
**Prototype**: ``seqtools.Utilities::Int32 NW(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /nw /query <query.fasta> /subject <subject.fasta> [/out <out.txt>]
```


#### Arguments
##### /query

###### Example
```bash
/query <term_string>
```
##### /subject

###### Example
```bash
/subject <term_string>
```
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /query
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaSeq_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

###### /subject
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaSeq_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

###### /out
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

<h3 id="/Palindrome.BatchTask"> 31. /Palindrome.BatchTask</h3>


**Prototype**: ``seqtools.Utilities::Int32 PalindromeBatchTask(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Palindrome.BatchTask /in <in.DIR> [/num_threads 4 /min 3 /max 20 /min-appears 2 /cutoff <0.6> /Palindrome /max-dist <1000 (bp)> /partitions <-1> /out <out.DIR>]
```


#### Arguments
##### [/Palindrome]
Only search for Palindrome, not includes the repeats data.

###### Example
```bash
/Palindrome <term_string>
```
<h3 id="/Palindrome.Screen.MaxMatches"> 32. /Palindrome.Screen.MaxMatches</h3>


**Prototype**: ``seqtools.Utilities::Int32 FilteringMatches(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Palindrome.Screen.MaxMatches /in <in.csv> /min <min.max-matches> [/out <out.csv>]
```
<h3 id="/Palindrome.Screen.MaxMatches.Batch"> 33. /Palindrome.Screen.MaxMatches.Batch</h3>


**Prototype**: ``seqtools.Utilities::Int32 FilteringMatchesBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Palindrome.Screen.MaxMatches.Batch /in <inDIR> /min <min.max-matches> [/out <out.DIR> /num_threads <-1>]
```
<h3 id="/Palindrome.Workflow"> 34. /Palindrome.Workflow</h3>


**Prototype**: ``seqtools.Utilities::Int32 PalindromeWorkflow(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Palindrome.Workflow /in <in.fasta> [/batch /min-appears 2 /min 3 /max 20 /cutoff <0.6> /max-dist <1000 (bp)> /Palindrome /partitions <-1> /out <out.DIR>]
```


#### Arguments
##### /in
This is a single sequence fasta file.

###### Example
```bash
/in <term_string>
```
##### [/Palindrome]
Only search for Palindrome, not includes the repeats data.

###### Example
```bash
/Palindrome <term_string>
```
<h3 id="/Promoter.Palindrome.Fasta"> 35. /Promoter.Palindrome.Fasta</h3>


**Prototype**: ``seqtools.Utilities::Int32 PromoterPalindrome2Fasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Promoter.Palindrome.Fasta /in <palindrome.csv> [/out <out.fasta>]
```
<h3 id="/Promoter.Regions.Palindrome"> 36. /Promoter.Regions.Palindrome</h3>


**Prototype**: ``seqtools.Utilities::Int32 PromoterRegionPalindrome(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Promoter.Regions.Palindrome /in <genbank.gb> [/min <3> /max <20> /len <100,150,200,250,300,400,500, default:=250> /mirror /out <out.csv>]
```


#### Arguments
##### [/mirror]
Search for the mirror palindrome loci sites.

###### Example
```bash
/mirror
#(boolean flag does not require of argument value)
```
##### Accepted Types
###### /mirror
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

<h3 id="/Promoter.Regions.Parser.gb"> 37. /Promoter.Regions.Parser.gb</h3>


**Prototype**: ``seqtools.Utilities::Int32 PromoterRegionParser_gb(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Promoter.Regions.Parser.gb /gb <genbank.gb> [/out <out.DIR>]
```
<h3 id="/Rule.dnaA_gyrB"> 38. /Rule.dnaA_gyrB</h3>


**Prototype**: ``seqtools.Utilities::Int32 dnaA_gyrB_rule(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Rule.dnaA_gyrB /genome <genbank.gb> [/out <out.fasta>]
```
<h3 id="/Rule.dnaA_gyrB.Matrix"> 39. /Rule.dnaA_gyrB.Matrix</h3>


**Prototype**: ``seqtools.Utilities::Int32 RuleMatrix(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Rule.dnaA_gyrB.Matrix /genomes <genomes.gb.DIR> [/out <out.csv>]
```
<h3 id="/Screen.sites"> 40. /Screen.sites</h3>


**Prototype**: ``seqtools.Utilities::Int32 ScreenRepeats(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Screen.sites /in <DIR/sites.csv> /range <min_bp>,<max_bp> [/type <type,default:=RepeatsView,alt:RepeatsView,RevRepeatsView,PalindromeLoci,ImperfectPalindrome> /out <out.csv>]
```


#### Arguments
##### /in

###### Example
```bash
/in <term_string>
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsView_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
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
    "Data": {
        "System.String": "System.String"
    },
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

**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "SequenceData": "System.String",
    "Start": 0
}
```

**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.ImperfectPalindrome_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Distance": 0,
    "Evolr": "System.String",
    "Left": 0,
    "Matches": "System.String",
    "MaxMatch": 0,
    "Palindrome": "System.String",
    "Paloci": 0,
    "Score": 0,
    "SequenceData": "System.String",
    "Site": "System.String"
}
```

<h3 id="/Select.By_Locus"> 41. /Select.By_Locus</h3>

Select fasta sequence by local_tag.
**Prototype**: ``seqtools.Utilities::Int32 SelectByLocus(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Select.By_Locus /in <locus.txt/csv> /fa <fasta/.inDIR> [/field <columnName> /reverse /out <out.fasta>]
```


#### Arguments
##### /reverse
If this option is enable, then all of the sequence that not appeared in the list will be output.

###### Example
```bash
/reverse <term_string>
```
##### /fa
Both a fasta file or a directory that contains the fasta files are valid value.

###### Example
```bash
/fa <term_string>
```
##### [/field]
If this parameter was specified, then the input locus_tag data will comes from a csv file,
this parameter indicates that which column will be used for gets the locus_tag data.

###### Example
```bash
/field <term_string>
```
##### Accepted Types
###### /reverse
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

###### /field
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

<h3 id="/Sigma"> 42. /Sigma</h3>


**Prototype**: ``seqtools.Utilities::Int32 Sigma(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Sigma /in <in.fasta> [/out <out.Csv> /simple /round <-1>]
```
<h3 id="/SimpleSegment.AutoBuild"> 43. /SimpleSegment.AutoBuild</h3>


**Prototype**: ``seqtools.Utilities::Int32 ConvertsAuto(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /SimpleSegment.AutoBuild /in <locis.csv> [/out <out.csv>]
```


#### Arguments
##### /in

###### Example
```bash
/in <term_string>
```
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.ImperfectPalindrome_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Distance": 0,
    "Evolr": "System.String",
    "Left": 0,
    "Matches": "System.String",
    "MaxMatch": 0,
    "Palindrome": "System.String",
    "Paloci": 0,
    "Score": 0,
    "SequenceData": "System.String",
    "Site": "System.String"
}
```

**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RevRepeats_
Example: 
```json
{
    "Locations": [
        0
    ],
    "SequenceData": "System.String",
    "RepeatLoci": [
        0
    ],
    "RevSegment": "System.String"
}
```

**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Repeats_
Example: 
```json
{
    "Locations": [
        0
    ],
    "SequenceData": "System.String"
}
```

**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "SequenceData": "System.String",
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

<h3 id="/SimpleSegment.Mirrors"> 44. /SimpleSegment.Mirrors</h3>


**Prototype**: ``seqtools.Utilities::Int32 ConvertMirrors(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /SimpleSegment.Mirrors /in <in.csv> [/out <out.csv>]
```


#### Arguments
##### /in

###### Example
```bash
/in <term_string>
```
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "SequenceData": "System.String",
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

<h3 id="/SimpleSegment.Mirrors.Batch"> 45. /SimpleSegment.Mirrors.Batch</h3>


**Prototype**: ``seqtools.Utilities::Int32 ConvertMirrorsBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /SimpleSegment.Mirrors.Batch /in <in.DIR> [/out <out.DIR>]
```
<h3 id="/Sites2Fasta"> 46. /Sites2Fasta</h3>

Converts the simple segment object collection as fasta file.
**Prototype**: ``seqtools.Utilities::Int32 Sites2Fasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Sites2Fasta /in <segments.csv> [/assemble /out <out.fasta>]
```


#### Arguments
##### /in

###### Example
```bash
/in <term_string>
```
##### /out

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /in
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

###### /out
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

<h3 id="/SNP"> 47. /SNP</h3>


**Prototype**: ``seqtools.Utilities::Int32 SNP(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /SNP /in <nt.fasta> [/ref <int_index/title, default:0> /pure /monomorphic /high <0.65>]
```


#### Arguments
##### /in


###### Example
```bash
/in <term_string>
```
##### [/ref]

###### Example
```bash
/ref <term_string>
```
##### [/pure]

###### Example
```bash
/pure <term_string>
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
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

<h3 id="/Split"> 48. /Split</h3>


**Prototype**: ``seqtools.Utilities::Int32 Split(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Split /in <in.fasta> [/n <4096> /out <outDIR>]
```
<h3 id="/SSR"> 49. /SSR</h3>

Search for SSR on a nt sequence.
**Prototype**: ``seqtools.Utilities::Int32 SSRFinder(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /SSR /in <nt.fasta> [/range <default=2,6> /parallel /out <out.csv/DIR>]
```
<h3 id="/subset"> 50. /subset</h3>


**Prototype**: ``seqtools.Utilities::Int32 SubSet(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /subset /lstID <lstID.txt> /fa <source.fasta>
```
<h3 id="/Time.Mutation"> 51. /Time.Mutation</h3>

The ongoing time mutation of the genome sequence.
**Prototype**: ``seqtools.Utilities::Int32 TimeDiffs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Time.Mutation /in <aln.fasta> [/ref <default:first,other:title/index> /cumulative /out <out.csv>]
```
<h3 id="/Write.Seeds"> 52. /Write.Seeds</h3>


**Prototype**: ``seqtools.Utilities::Int32 WriteSeeds(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools /Write.Seeds /out <out.dat> [/prot /max <20>]
```
<h3 id="-321"> 53. -321</h3>

Polypeptide sequence 3 letters to 1 lettes sequence.
**Prototype**: ``seqtools.Utilities::Int32 PolypeptideBriefs(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools -321 /in <sequence.txt> [/out <out.fasta>]
```
<h3 id="--align"> 54. --align</h3>


**Prototype**: ``seqtools.Utilities::Int32 Align(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --align /query <query.fasta> /subject <subject.fasta> [/out <out.DIR> /cost <0.7>]
```
<h3 id="--align.Self"> 55. --align.Self</h3>


**Prototype**: ``seqtools.Utilities::Int32 AlignSelf(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --align.Self /query <query.fasta> /out <out.DIR> [/cost 0.75]
```
<h3 id="-complement"> 56. -complement</h3>


**Prototype**: ``seqtools.Utilities::Int32 Complement(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools -complement -i <input_fasta> [-o <output_fasta>]
```
<h3 id="--Drawing.ClustalW"> 57. --Drawing.ClustalW</h3>


**Prototype**: ``seqtools.Utilities::Int32 DrawClustalW(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Drawing.ClustalW /in <align.fasta> [/out <out.png> /dot.Size 10]
```
<h3 id="--Hairpinks"> 58. --Hairpinks</h3>


**Prototype**: ``seqtools.Utilities::Int32 Hairpinks(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Hairpinks /in <in.fasta> [/out <out.csv> /min <6> /max <7> /cutoff 3 /max-dist <35 (bp)>]
```


#### Arguments
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.ImperfectPalindrome_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Distance": 0,
    "Evolr": "System.String",
    "Left": 0,
    "Matches": "System.String",
    "MaxMatch": 0,
    "Palindrome": "System.String",
    "Paloci": 0,
    "Score": 0,
    "SequenceData": "System.String",
    "Site": "System.String"
}
```

<h3 id="--Hairpinks.batch.task"> 59. --Hairpinks.batch.task</h3>


**Prototype**: ``seqtools.Utilities::Int32 HairpinksBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Hairpinks.batch.task /in <in.fasta> [/out <outDIR> /min <6> /max <7> /cutoff <0.6> /max-dist <35 (bp)> /num_threads <-1>]
```
<h3 id="--ImperfectsPalindrome.batch.Task"> 60. --ImperfectsPalindrome.batch.Task</h3>


**Prototype**: ``seqtools.Utilities::Int32 BatchSearchImperfectsPalindrome(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --ImperfectsPalindrome.batch.Task /in <in.fasta> /out <outDir> [/min <3> /max <20> /cutoff <0.6> /max-dist <1000 (bp)> /num_threads <-1>]
```
<h3 id="--Mirror.From.Fasta"> 61. --Mirror.From.Fasta</h3>

Mirror Palindrome, search from a fasta file.
**Prototype**: ``seqtools.Utilities::Int32 SearchMirrotFasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Mirror.From.Fasta /nt <nt-sequence.fasta> [/out <out.csv> /min <3> /max <20>]
```


#### Arguments
##### /nt
This fasta file should contains only just one sequence.

###### Example
```bash
/nt <term_string>
```
##### Accepted Types
###### /nt
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaSeq_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

<h3 id="--Mirror.From.NT"> 62. --Mirror.From.NT</h3>

Mirror Palindrome, and this function is for the debugging test
**Prototype**: ``seqtools.Utilities::Int32 SearchMirrotNT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Mirror.From.NT /nt <nt-sequence> /out <out.csv> [/min <3> /max <20>]
```


#### Arguments
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "SequenceData": "System.String",
    "Start": 0
}
```

<h3 id="--Palindrome.batch.Task"> 63. --Palindrome.batch.Task</h3>


**Prototype**: ``seqtools.Utilities::Int32 BatchSearchPalindrome(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Palindrome.batch.Task /in <in.fasta> /out <outDir> [/min <3> /max <20> /num_threads <-1>]
```
<h3 id="--Palindrome.From.FASTA"> 64. --Palindrome.From.FASTA</h3>


**Prototype**: ``seqtools.Utilities::Int32 SearchPalindromeFasta(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Palindrome.From.Fasta /nt <nt-sequence.fasta> [/out <out.csv> /min <3> /max <20>]
```


#### Arguments
##### /nt
Fasta sequence file, and this file should just contains only one sequence.

###### Example
```bash
/nt <term_string>
```
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /nt
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaSeq_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "SequenceData": "System.String",
    "Start": 0
}
```

<h3 id="--Palindrome.From.NT"> 65. --Palindrome.From.NT</h3>

This function is just for debugger test, /nt parameter is the nucleotide sequence data as ATGCCCC
**Prototype**: ``seqtools.Utilities::Int32 SearchPalindromeNT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Palindrome.From.NT /nt <nt-sequence> /out <out.csv> [/min <3> /max <20>]
```


#### Arguments
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.PalindromeLoci_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Loci": "System.String",
    "MirrorSite": "System.String",
    "PalEnd": 0,
    "Palindrome": "System.String",
    "SequenceData": "System.String",
    "Start": 0
}
```

<h3 id="--Palindrome.Imperfects"> 66. --Palindrome.Imperfects</h3>

Gets all partly matched palindrome sites.
**Prototype**: ``seqtools.Utilities::Int32 ImperfectPalindrome(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Palindrome.Imperfects /in <in.fasta> [/out <out.csv> /min <3> /max <20> /cutoff <0.6> /max-dist <1000 (bp)> /partitions <-1>]
```


#### Arguments
##### /in
This parameter is a file path of a nt sequence in fasta format, or you can directly input the sequence data from commandline ``std_in``.

###### Example
```bash
/in <file, *.fasta, *.fsa, *.fa>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### Accepted Types
###### /in
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaSeq_
Example: 
```bash
>LexA
AAGCGAACAAATGTTCTATA
```

<h3 id="-pattern_search"> 67. -pattern_search</h3>

Parsing the sequence segment from the sequence source using regular expression.
**Prototype**: ``seqtools.Utilities::Int32 PatternSearchA(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools -pattern_search -i <file_name> -p <regex_pattern>[ -o <output_directory> -f <format:fsa/gbk>]
```
###### Example
```bash
seqtools -pattern_search -i ~/xcc8004.txt -p TTA{3}N{1,2} -f fsa
```


#### Arguments
##### -i
The sequence input data source file, it can be a fasta or genbank file.

###### Example
```bash
-i <term_string>
```
##### -p
This switch specific the regular expression pattern for search the sequence segment,
for more detail information about the regular expression please read the user manual.

###### Example
```bash
-p <term_string>
```
##### [-o]
Optional, this switch value specific the output directory for the result data, default is user Desktop folder.

###### Example
```bash
-o <term_string>
```
##### [-f]
Optional, specific the input file format for the sequence reader, default value is FASTA sequence file.
fsa - The input sequence data file is a FASTA format file;
gbk - The input sequence data file is a NCBI genbank flat file.

###### Example
```bash
-f <term_string>
```
<h3 id="--PerfectPalindrome.Filtering"> 68. --PerfectPalindrome.Filtering</h3>


**Prototype**: ``seqtools.Utilities::Int32 FilterPerfectPalindrome(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --PerfectPalindrome.Filtering /in <inDIR> [/min <8> /out <outDIR>]
```


#### Arguments
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.ImperfectPalindrome_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
    "Distance": 0,
    "Evolr": "System.String",
    "Left": 0,
    "Matches": "System.String",
    "MaxMatch": 0,
    "Palindrome": "System.String",
    "Paloci": 0,
    "Score": 0,
    "SequenceData": "System.String",
    "Site": "System.String"
}
```

<h3 id="Repeats.Density"> 69. Repeats.Density</h3>


**Prototype**: ``seqtools.Utilities::Int32 RepeatsDensity(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools Repeats.Density /dir <dir> /size <size> /ref <refName> [/out <out.csv> /cutoff <default:=0>]
```
<h3 id="-reverse"> 70. -reverse</h3>


**Prototype**: ``seqtools.Utilities::Int32 Reverse(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools -reverse -i <input_fasta> [-o <output_fasta>]
```
<h3 id="rev-Repeats.Density"> 71. rev-Repeats.Density</h3>


**Prototype**: ``seqtools.Utilities::Int32 revRepeatsDensity(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools rev-Repeats.Density /dir <dir> /size <size> /ref <refName> [/out <out.csv> /cutoff <default:=0>]
```
<h3 id="Search.Batch"> 72. Search.Batch</h3>

Batch search for repeats.
**Prototype**: ``seqtools.Utilities::Int32 BatchSearch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools Search.Batch /aln <alignment.fasta> [/min 3 /max 20 /min-rep 2 /out <./>]
```


#### Arguments
##### /aln
The input fasta file should be the output of the clustal multiple alignment fasta output.

###### Example
```bash
/aln <term_string>
```
##### [/out]

###### Example
```bash
/out <term_string>
```
##### Accepted Types
###### /out
**Decalre**:  _SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.RepeatsView_
Example: 
```json
{
    "Data": {
        "System.String": "System.String"
    },
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
    "Data": {
        "System.String": "System.String"
    },
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

<h3 id="-segment"> 73. -segment</h3>


**Prototype**: ``seqtools.Utilities::Int32 GetSegment(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools -segment /fasta <Fasta_Token> [-loci <loci>] [/left <left> /length <length> /right <right> [/reverse]] [/ptt <ptt> /geneID <gene_id> /dist <distance> /downstream] -o <saved> [-line.break 100]
```
<h3 id="--segments"> 74. --segments</h3>


**Prototype**: ``seqtools.Utilities::Int32 GetSegments(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --segments /regions <regions.csv> /fasta <nt.fasta> [/complement /reversed /brief-dump]
```


#### Arguments
##### [/reversed]
If the sequence is on the complement strand, reversed it after complement operation?

###### Example
```bash
/reversed <term_string>
```
##### [/complement]
If this Boolean switch is set on, then all of the reversed strand segment will be complemenet and reversed.

###### Example
```bash
/complement <term_string>
```
##### [/brief-dump]
If this parameter is set up true, then only the locus_tag of the ORF gene will be dump to the fasta sequence.

###### Example
```bash
/brief-dump <term_string>
```
<h3 id="--ToVector"> 75. --ToVector</h3>


**Prototype**: ``seqtools.Utilities::Int32 ToVector(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --ToVector /in <in.DIR> /min <4> /max <8> /out <out.txt> /size <genome.size>
```
<h3 id="--translates"> 76. --translates</h3>

Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.
**Prototype**: ``seqtools.Utilities::Int32 Translates(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --translates /orf <orf.fasta> [/transl_table 1 /force]
```


#### Arguments
##### /orf
ORF gene nt sequence should be completely complement and reversed as forwards strand if it is complement strand.

###### Example
```bash
/orf <file/directory>
# (This argument can accept the ``std_out`` from upstream app as input)
```
##### [/transl_table]
Available index value was described at
http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25

###### Example
```bash
/transl_table <term_string>
```
##### [/force]
This force parameter will force the translation program ignore of the stop code and continute sequence translation.

###### Example
```bash
/force
#(boolean flag does not require of argument value)
```
<h3 id="--Trim"> 77. --Trim</h3>


**Prototype**: ``seqtools.Utilities::Int32 Trim(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
seqtools --Trim /in <in.fasta> [/case <u/l> /break <-1/int> /out <out.fasta> /brief]
```


#### Arguments
##### [/case]
Adjust the letter case of your sequence, l for lower case and u for upper case. Default value is upper case.

###### Example
```bash
/case <term_string>
```
##### [/break]
Adjust the sequence break when this program write the fasta sequence, default is -1 which means no break, write all sequence in one line.

###### Example
```bash
/break <term_string>
```
