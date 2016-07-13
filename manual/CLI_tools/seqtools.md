---
title: seqtools
tags: [maunal, tools]
date: 7/7/2016 6:51:59 PM
---
# GCModeller [version 3.0.2456.4506]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/seqtools.exe
**Root namespace**: seqtools.Utilities


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/align||
|/Clustal.Cut||
|/Distinct||
|/Get.Locis||
|/Gff.Sites||
|/gwANI||
|/Loci.describ|Testing|
|/logo|* Drawing the sequence logo from the clustal alignment result.|
|/Merge|Only search for 1 level folder, dit not search receve.|
|/Mirror.Batch||
|/Mirror.Fuzzy||
|/Mirror.Fuzzy.Batch||
|/Mirror.Vector||
|/Mirrors.Context|This function will convert the mirror data to the simple segment object data|
|/Mirrors.Group||
|/Mirrors.Group.Batch||
|/nw|RunNeedlemanWunsch|
|/Palindrome.BatchTask||
|/Palindrome.Screen.MaxMatches||
|/Palindrome.Screen.MaxMatches.Batch||
|/Palindrome.Workflow||
|/Select.By_Locus||
|/Sigma||
|/SimpleSegment.AutoBuild||
|/SimpleSegment.Mirrors||
|/SimpleSegment.Mirrors.Batch||
|/SNP||
|/Split||
|/subset||
|/To_Fasta|Convert the sequence data in a excel annotation file into a fasta sequence file.|
|/Write.Seeds||
|-321|Polypeptide sequence 3 letters to 1 lettes sequence.|
|--align||
|--align.Self||
|-complement||
|--Drawing.ClustalW||
|--Hairpinks||
|--Hairpinks.batch.task||
|--ImperfectsPalindrome.batch.Task||
|--Mirror.From.Fasta|Mirror Palindrome, search from a fasta file.|
|--Mirror.From.NT|Mirror Palindrome, and this function is for the debugging test|
|--Palindrome.batch.Task||
|--Palindrome.From.FASTA||
|--Palindrome.From.NT|This function is just for debugger test, /nt parameter is the nucleotide sequence data as ATGCCCC|
|--Palindrome.Imperfects||
|-pattern_search|Parsing the sequence segment from the sequence source using regular expression.|
|--PerfectPalindrome.Filtering||
|Repeats.Density||
|-reverse||
|rev-Repeats.Density||
|Search.Batch|Batch search for repeats.|
|-segment||
|--segments||
|--ToVector||
|--translates|Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.|
|--Trim||

## Commands
--------------------------
##### Help for command '/align':

**Prototype**: seqtools.Utilities::Int32 Align2(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /align /query <query.fasta> /subject <subject.fasta> [/blosum <matrix.txt> /out <out.xml>]
  Example:      seqtools /align 
```

##### Help for command '/Clustal.Cut':

**Prototype**: seqtools.Utilities::Int32 CutMlAlignment(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Clustal.Cut /in <in.fasta> [/left 0.1 /right 0.1 /out <out.fasta>]
  Example:      seqtools /Clustal.Cut 
```

##### Help for command '/Distinct':

**Prototype**: seqtools.Utilities::Int32 Distinct(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Distinct /in <in.fasta> [/out <out.fasta>]
  Example:      seqtools /Distinct 
```

##### Help for command '/Get.Locis':

**Prototype**: seqtools.Utilities::Int32 GetSimpleSegments(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Get.Locis /in <locis.csv> /nt <genome.nt.fasta> [/out <outDIR>]
  Example:      seqtools /Get.Locis 
```

##### Help for command '/Gff.Sites':

**Prototype**: seqtools.Utilities::Int32 GffSites(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Gff.Sites /fna <genomic.fna> /gff <genome.gff> [/out <out.fasta>]
  Example:      seqtools /Gff.Sites 
```

##### Help for command '/gwANI':

**Prototype**: seqtools.Utilities::Int32 gwANI(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /gwANI /in <in.fasta> [/fast /out <out.Csv>]
  Example:      seqtools /gwANI 
```

##### Help for command '/Loci.describ':

**Prototype**: seqtools.Utilities::Int32 LociDescript(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Testing
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Loci.describ /ptt <genome-context.ptt> [/test <loci:randomize> /complement /unstrand]
  Example:      seqtools /Loci.describ 
```

##### Help for command '/logo':

**Prototype**: seqtools.Utilities::Int32 SequenceLogo(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  * Drawing the sequence logo from the clustal alignment result.
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /logo /in <clustal.fasta> [/out <out.png> /title ""]
  Example:      seqtools /logo 
```



  Parameters information:
```
    /in
    Description:  The file path of the clustal output fasta file.

    Example:      /in ""

   [/out]
    Description:  The output sequence logo image file path. default is the same name as the input fasta sequence file.

    Example:      /out ""

   [/title]
    Description:  The display title on the sequence logo, default is using the fasta file name.

    Example:      /title ""


```

#### Accepted Types
##### /in
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```json
[
    
]
```

##### /out
##### /title
##### Help for command '/Merge':

**Prototype**: seqtools.Utilities::Int32 Merge(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Only search for 1 level folder, dit not search receve.
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Merge /in <fasta.DIR> [/out <out.fasta> /trim /ext <*.fasta> /brief]
  Example:      seqtools /Merge 
```

##### Help for command '/Mirror.Batch':

**Prototype**: seqtools.Utilities::Int32 MirrorBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Mirror.Batch /nt <nt.fasta> [/out <out.csv> /mp /min <3> /max <20> /num_threads <-1>]
  Example:      seqtools /Mirror.Batch 
```



  Parameters information:
```
    /nt
    Description:  
    Example:      /nt ""

   [/mp]
    Description:  Calculation in the multiple process mode?

    Example:      /mp ""


```

#### Accepted Types
##### /nt
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```json
[
    
]
```

##### /mp
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

##### Help for command '/Mirror.Fuzzy':

**Prototype**: seqtools.Utilities::Int32 FuzzyMirrors(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Mirror.Fuzzy /in <in.fasta> [/out <out.csv> /cut 0.6 /max-dist 6 /min 3 /max 20]
  Example:      seqtools /Mirror.Fuzzy 
```



  Parameters information:
```
    /in
    Description:  
    Example:      /in ""

   [/out]
    Description:  
    Example:      /out ""


```

#### Accepted Types
##### /in
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaToken_
Example: 
```json
{
    "Extension": {
        "DynamicHash": {
            "Properties": [
                
            ],
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

##### /out
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

##### Help for command '/Mirror.Fuzzy.Batch':

**Prototype**: seqtools.Utilities::Int32 FuzzyMirrorsBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Mirror.Fuzzy.Batch /in <in.fasta> [/out <out.DIR> /cut 0.6 /max-dist 6 /min 3 /max 20 /num_threads <-1>]
  Example:      seqtools /Mirror.Fuzzy.Batch 
```

##### Help for command '/Mirror.Vector':

**Prototype**: seqtools.Utilities::Int32 MirrorsVector(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Mirror.Vector /in <inDIR> /size <genome.size> [/out out.txt]
  Example:      seqtools /Mirror.Vector 
```

##### Help for command '/Mirrors.Context':

**Prototype**: seqtools.Utilities::Int32 MirrorContext(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  This function will convert the mirror data to the simple segment object data
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Mirrors.Context /in <mirrors.csv> /PTT <genome.ptt> [/trans /strand <+/-> /out <out.csv> /stranded /dist <500bp>]
  Example:      seqtools /Mirrors.Context 
```



  Parameters information:
```
       [/trans]
    Description:  Enable this option will using genome_size minus loci location for the location correction, only works in reversed strand.

    Example:      /trans ""


```

#### Accepted Types
##### /trans
##### Help for command '/Mirrors.Group':

**Prototype**: seqtools.Utilities::Int32 MirrorGroups(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Mirrors.Group /in <mirrors.Csv> [/batch /fuzzy <-1> /out <out.DIR>]
  Example:      seqtools /Mirrors.Group 
```



  Parameters information:
```
       [/fuzzy]
    Description:  -1 means group sequence by string equals compared, and value of 0-1 means using string fuzzy compare.

    Example:      /fuzzy ""


```

#### Accepted Types
##### /fuzzy
##### Help for command '/Mirrors.Group.Batch':

**Prototype**: seqtools.Utilities::Int32 MirrorGroupsBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Mirrors.Group.Batch /in <mirrors.DIR> [/fuzzy <-1> /out <out.DIR> /num_threads <-1>]
  Example:      seqtools /Mirrors.Group.Batch 
```

##### Help for command '/nw':

**Prototype**: seqtools.Utilities::Int32 NW(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  RunNeedlemanWunsch
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /nw /query <query.fasta> /subject <subject.fasta> [/out <out.txt>]
  Example:      seqtools /nw 
```



  Parameters information:
```
    /query
    Description:  
    Example:      /query ""

/subject
    Description:  
    Example:      /subject ""

   [/out]
    Description:  
    Example:      /out ""


```

#### Accepted Types
##### /query
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaToken_
Example: 
```json
{
    "Extension": {
        "DynamicHash": {
            "Properties": [
                
            ],
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

##### /subject
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaToken_
Example: 
```json
{
    "Extension": {
        "DynamicHash": {
            "Properties": [
                
            ],
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

##### /out
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

##### Help for command '/Palindrome.BatchTask':

**Prototype**: seqtools.Utilities::Int32 PalindromeBatchTask(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Palindrome.BatchTask /in <in.DIR> [/num_threads 4 /min 3 /max 20 /min-appears 2 /cutoff <0.6> /Palindrome /max-dist <1000 (bp)> /partitions <-1> /out <out.DIR>]
  Example:      seqtools /Palindrome.BatchTask 
```



  Parameters information:
```
       [/Palindrome]
    Description:  Only search for Palindrome, not includes the repeats data.

    Example:      /Palindrome ""


```

#### Accepted Types
##### /Palindrome
##### Help for command '/Palindrome.Screen.MaxMatches':

**Prototype**: seqtools.Utilities::Int32 FilteringMatches(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Palindrome.Screen.MaxMatches /in <in.csv> /min <min.max-matches> [/out <out.csv>]
  Example:      seqtools /Palindrome.Screen.MaxMatches 
```

##### Help for command '/Palindrome.Screen.MaxMatches.Batch':

**Prototype**: seqtools.Utilities::Int32 FilteringMatchesBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Palindrome.Screen.MaxMatches.Batch /in <inDIR> /min <min.max-matches> [/out <out.DIR> /num_threads <-1>]
  Example:      seqtools /Palindrome.Screen.MaxMatches.Batch 
```

##### Help for command '/Palindrome.Workflow':

**Prototype**: seqtools.Utilities::Int32 PalindromeWorkflow(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Palindrome.Workflow /in <in.fasta> [/batch /min-appears 2 /min 3 /max 20 /cutoff <0.6> /max-dist <1000 (bp)> /Palindrome /partitions <-1> /out <out.DIR>]
  Example:      seqtools /Palindrome.Workflow 
```



  Parameters information:
```
    /in
    Description:  This is a single sequence fasta file.

    Example:      /in ""

   [/Palindrome]
    Description:  Only search for Palindrome, not includes the repeats data.

    Example:      /Palindrome ""


```

#### Accepted Types
##### /in
##### /Palindrome
##### Help for command '/Select.By_Locus':

**Prototype**: seqtools.Utilities::Int32 SelectByLocus(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Select.By_Locus /in <locus.txt> /fa <fasta.inDIR> [/out <out.fasta>]
  Example:      seqtools /Select.By_Locus 
```

##### Help for command '/Sigma':

**Prototype**: seqtools.Utilities::Int32 Sigma(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Sigma /in <in.fasta> [/out <out.Csv> /simple /round <-1>]
  Example:      seqtools /Sigma 
```

##### Help for command '/SimpleSegment.AutoBuild':

**Prototype**: seqtools.Utilities::Int32 ConvertsAuto(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /SimpleSegment.AutoBuild /in <locis.csv> [/out <out.csv>]
  Example:      seqtools /SimpleSegment.AutoBuild 
```



  Parameters information:
```
    /in
    Description:  
    Example:      /in ""

   [/out]
    Description:  
    Example:      /out ""


```

#### Accepted Types
##### /in
**Decalre**:  _Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File_
Example: 
```json
[
    
]
```

##### /out
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

##### Help for command '/SimpleSegment.Mirrors':

**Prototype**: seqtools.Utilities::Int32 ConvertMirrors(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /SimpleSegment.Mirrors /in <in.csv> [/out <out.csv>]
  Example:      seqtools /SimpleSegment.Mirrors 
```



  Parameters information:
```
    /in
    Description:  
    Example:      /in ""

   [/out]
    Description:  
    Example:      /out ""


```

#### Accepted Types
##### /in
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

##### /out
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

##### Help for command '/SimpleSegment.Mirrors.Batch':

**Prototype**: seqtools.Utilities::Int32 ConvertMirrorsBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /SimpleSegment.Mirrors.Batch /in <in.DIR> [/out <out.DIR>]
  Example:      seqtools /SimpleSegment.Mirrors.Batch 
```

##### Help for command '/SNP':

**Prototype**: seqtools.Utilities::Int32 SNP(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /SNP /in <nt.fasta> [/ref 0 /pure /monomorphic]
  Example:      seqtools /SNP 
```



  Parameters information:
```
    /in
    Description:  

    Example:      /in ""

   [/ref]
    Description:  
    Example:      /ref ""

   [/pure]
    Description:  
    Example:      /pure ""


```

#### Accepted Types
##### /in
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaFile_
Example: 
```json
[
    
]
```

##### /ref
**Decalre**:  _System.Int32_
Example: 
```json
0
```

##### /pure
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

##### Help for command '/Split':

**Prototype**: seqtools.Utilities::Int32 Split(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Split /in <in.fasta> [/n <4096> /out <outDIR>]
  Example:      seqtools /Split 
```

##### Help for command '/subset':

**Prototype**: seqtools.Utilities::Int32 SubSet(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /subset /lstID <lstID.txt> /fa <source.fasta>
  Example:      seqtools /subset 
```

##### Help for command '/To_Fasta':

**Prototype**: seqtools.Utilities::Int32 ToFasta(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Convert the sequence data in a excel annotation file into a fasta sequence file.
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /To_Fasta /in <anno.csv> [/out <out.fasta> /attrs <gene;locus_tag;gi;location,...> /seq <Sequence>]
  Example:      seqtools /To_Fasta 
```

##### Help for command '/Write.Seeds':

**Prototype**: seqtools.Utilities::Int32 WriteSeeds(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe /Write.Seeds /out <out.dat> [/prot /max <20>]
  Example:      seqtools /Write.Seeds 
```

##### Help for command '-321':

**Prototype**: seqtools.Utilities::Int32 PolypeptideBriefs(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Polypeptide sequence 3 letters to 1 lettes sequence.
  Usage:        G:\GCModeller\manual\bin\seqtools.exe -321 /in <sequence.txt> [/out <out.fasta>]
  Example:      seqtools -321 
```

##### Help for command '--align':

**Prototype**: seqtools.Utilities::Int32 Align(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --align /query <query.fasta> /subject <subject.fasta> [/out <out.DIR> /cost <0.7>]
  Example:      seqtools --align 
```

##### Help for command '--align.Self':

**Prototype**: seqtools.Utilities::Int32 AlignSelf(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --align.Self /query <query.fasta> /out <out.DIR> [/cost 0.75]
  Example:      seqtools --align.Self 
```

##### Help for command '-complement':

**Prototype**: seqtools.Utilities::Int32 Complement(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe -complement -i <input_fasta> [-o <output_fasta>]
  Example:      seqtools -complement 
```

##### Help for command '--Drawing.ClustalW':

**Prototype**: seqtools.Utilities::Int32 DrawClustalW(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --Drawing.ClustalW /in <align.fasta> [/out <out.png> /dot.Size 10]
  Example:      seqtools --Drawing.ClustalW 
```

##### Help for command '--Hairpinks':

**Prototype**: seqtools.Utilities::Int32 Hairpinks(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --Hairpinks /in <in.fasta> [/out <out.csv> /min <6> /max <7> /cutoff 3 /max-dist <35 (bp)>]
  Example:      seqtools --Hairpinks 
```

##### Help for command '--Hairpinks.batch.task':

**Prototype**: seqtools.Utilities::Int32 HairpinksBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --Hairpinks.batch.task /in <in.fasta> [/out <outDIR> /min <6> /max <7> /cutoff <0.6> /max-dist <35 (bp)> /num_threads <-1>]
  Example:      seqtools --Hairpinks.batch.task 
```

##### Help for command '--ImperfectsPalindrome.batch.Task':

**Prototype**: seqtools.Utilities::Int32 BatchSearchImperfectsPalindrome(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --ImperfectsPalindrome.batch.Task /in <in.fasta> /out <outDir> [/min <3> /max <20> /cutoff <0.6> /max-dist <1000 (bp)> /num_threads <-1>]
  Example:      seqtools --ImperfectsPalindrome.batch.Task 
```

##### Help for command '--Mirror.From.Fasta':

**Prototype**: seqtools.Utilities::Int32 SearchMirrotFasta(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Mirror Palindrome, search from a fasta file.
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --Mirror.From.Fasta /nt <nt-sequence.fasta> [/out <out.csv> /min <3> /max <20>]
  Example:      seqtools --Mirror.From.Fasta 
```



  Parameters information:
```
    /nt
    Description:  This fasta file should contains only just one sequence.

    Example:      /nt ""


```

#### Accepted Types
##### /nt
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaToken_
Example: 
```json
{
    "Extension": {
        "DynamicHash": {
            "Properties": [
                
            ],
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

##### Help for command '--Mirror.From.NT':

**Prototype**: seqtools.Utilities::Int32 SearchMirrotNT(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Mirror Palindrome, and this function is for the debugging test
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --Mirror.From.NT /nt <nt-sequence> /out <out.csv> [/min <3> /max <20>]
  Example:      seqtools --Mirror.From.NT 
```

##### Help for command '--Palindrome.batch.Task':

**Prototype**: seqtools.Utilities::Int32 BatchSearchPalindrome(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --Palindrome.batch.Task /in <in.fasta> /out <outDir> [/min <3> /max <20> /num_threads <-1>]
  Example:      seqtools --Palindrome.batch.Task 
```

##### Help for command '--Palindrome.From.FASTA':

**Prototype**: seqtools.Utilities::Int32 SearchPalindromeFasta(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --Palindrome.From.Fasta /nt <nt-sequence.fasta> [/out <out.csv> /min <3> /max <20>]
  Example:      seqtools --Palindrome.From.FASTA 
```



  Parameters information:
```
    /nt
    Description:  Fasta sequence file, and this file should just contains only one sequence.

    Example:      /nt ""

   [/out]
    Description:  
    Example:      /out ""


```

#### Accepted Types
##### /nt
**Decalre**:  _SMRUCC.genomics.SequenceModel.FASTA.FastaToken_
Example: 
```json
{
    "Extension": {
        "DynamicHash": {
            "Properties": [
                
            ],
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

##### /out
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

##### Help for command '--Palindrome.From.NT':

**Prototype**: seqtools.Utilities::Int32 SearchPalindromeNT(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  This function is just for debugger test, /nt parameter is the nucleotide sequence data as ATGCCCC
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --Palindrome.From.NT /nt <nt-sequence> /out <out.csv> [/min <3> /max <20>]
  Example:      seqtools --Palindrome.From.NT 
```

##### Help for command '--Palindrome.Imperfects':

**Prototype**: seqtools.Utilities::Int32 ImperfectPalindrome(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --Palindrome.Imperfects /in <in.fasta> [/out <out.csv> /min <3> /max <20> /cutoff <0.6> /max-dist <1000 (bp)> /partitions <-1>]
  Example:      seqtools --Palindrome.Imperfects 
```

##### Help for command '-pattern_search':

**Prototype**: seqtools.Utilities::Int32 PatternSearchA(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Parsing the sequence segment from the sequence source using regular expression.
  Usage:        G:\GCModeller\manual\bin\seqtools.exe -pattern_search -i <file_name> -p <regex_pattern>[ -o <output_directory> -f <format:fsa/gbk>]
  Example:      seqtools -pattern_search -pattern_search -i ~/xcc8004.txt -p TTA{3}N{1,2} -f fsa
```



  Parameters information:
```
    -i
    Description:  The sequence input data source file, it can be a fasta or genbank file.

    Example:      -i "~/Desktop/xcc8004.txt"

-p
    Description:  This switch specific the regular expression pattern for search the sequence segment,
              for more detail information about the regular expression please read the user manual.

    Example:      -p "N{1,5}TA"

   [-o]
    Description:  Optional, this switch value specific the output directory for the result data, default is user Desktop folder.

    Example:      -o "~/Documents/"

   [-f]
    Description:  Optional, specific the input file format for the sequence reader, default value is FASTA sequence file.
               fsa - The input sequence data file is a FASTA format file;
               gbk - The input sequence data file is a NCBI genbank flat file.

    Example:      -f "fsa"


```

#### Accepted Types
##### -i
##### -p
##### -o
##### -f
##### Help for command '--PerfectPalindrome.Filtering':

**Prototype**: seqtools.Utilities::Int32 FilterPerfectPalindrome(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --PerfectPalindrome.Filtering /in <inDIR> [/min <8> /out <outDIR>]
  Example:      seqtools --PerfectPalindrome.Filtering 
```

##### Help for command 'Repeats.Density':

**Prototype**: seqtools.Utilities::Int32 RepeatsDensity(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe Repeats.Density /dir <dir> /size <size> /ref <refName> [/out <out.csv> /cutoff <default:=0>]
  Example:      seqtools Repeats.Density 
```

##### Help for command '-reverse':

**Prototype**: seqtools.Utilities::Int32 Reverse(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe -reverse -i <input_fasta> [-o <output_fasta>]
  Example:      seqtools -reverse 
```

##### Help for command 'rev-Repeats.Density':

**Prototype**: seqtools.Utilities::Int32 revRepeatsDensity(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe rev-Repeats.Density /dir <dir> /size <size> /ref <refName> [/out <out.csv> /cutoff <default:=0>]
  Example:      seqtools rev-Repeats.Density 
```

##### Help for command 'Search.Batch':

**Prototype**: seqtools.Utilities::Int32 BatchSearch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Batch search for repeats.
  Usage:        G:\GCModeller\manual\bin\seqtools.exe Search.Batch /aln <alignment.fasta> [/min 3 /max 20 /min-rep 2 /out <./>]
  Example:      seqtools Search.Batch 
```



  Parameters information:
```
    /aln
    Description:  The input fasta file should be the output of the clustal multiple alignment fasta output.

    Example:      /aln ""


```

#### Accepted Types
##### /aln
##### Help for command '-segment':

**Prototype**: seqtools.Utilities::Int32 GetSegment(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe -segment /fasta <Fasta_Token> [-loci <loci>] [/left <left> /length <length> /right <right> [/reverse]] [/ptt <ptt> /geneID <gene_id> /dist <distance> /downstream] -o <saved> [-line.break 100]
  Example:      seqtools -segment 
```

##### Help for command '--segments':

**Prototype**: seqtools.Utilities::Int32 GetSegments(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --segments /regions <regions.csv> /fasta <nt.fasta> [/complement /reversed /brief-dump]
  Example:      seqtools --segments 
```



  Parameters information:
```
       [/reversed]
    Description:  If the sequence is on the complement strand, reversed it after complement operation?

    Example:      /reversed ""

   [/complement]
    Description:  If this Boolean switch is set on, then all of the reversed strand segment will be complemenet and reversed.

    Example:      /complement ""

   [/brief-dump]
    Description:  If this parameter is set up true, then only the locus_tag of the ORF gene will be dump to the fasta sequence.

    Example:      /brief-dump ""


```

#### Accepted Types
##### /reversed
##### /complement
##### /brief-dump
##### Help for command '--ToVector':

**Prototype**: seqtools.Utilities::Int32 ToVector(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --ToVector /in <in.DIR> /min <4> /max <8> /out <out.txt> /size <genome.size>
  Example:      seqtools --ToVector 
```

##### Help for command '--translates':

**Prototype**: seqtools.Utilities::Int32 Translates(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --translates /orf <orf.fasta> [/transl_table 1 /force]
  Example:      seqtools --translates 
```



  Parameters information:
```
    /orf
    Description:  ORF gene nt sequence should be completely complement and reversed as forwards strand if it is complement strand.

    Example:      /orf ""

   [/force]
    Description:  This force parameter will force the translation program ignore of the stop code and continute sequence translation.

    Example:      /force ""

   [/transl_table]
    Description:  Available index value was described at http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25

    Example:      /transl_table ""


```

#### Accepted Types
##### /orf
##### /force
##### /transl_table
##### Help for command '--Trim':

**Prototype**: seqtools.Utilities::Int32 Trim(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\seqtools.exe --Trim /in <in.fasta> [/case <u/l> /break <-1/int> /out <out.fasta> /brief]
  Example:      seqtools --Trim 
```



  Parameters information:
```
       [/case]
    Description:  Adjust the letter case of your sequence, l for lower case and u for upper case. Default value is upper case.

    Example:      /case ""

   [/break]
    Description:  Adjust the sequence break when this program write the fasta sequence, default is -1 which means no break, write all sequence in one line.

    Example:      /break ""


```

#### Accepted Types
##### /case
##### /break
