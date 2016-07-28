---
title: SMART
tags: [maunal, tools]
date: 7/27/2016 6:40:27 PM
---
# GCModeller [version 1.123.0.0]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/SMART.exe
**Root namespace**: ProteinTools.SMART.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|--align||
|--align.family||
|-build_cache||
|-buildsmart||
|convert||
|export||
|--Export.Domains||
|--Export.Pfam-String||
|--Family.Align|Family Annotation by MPAlignment|
|--Family.Domains|Build the Family database for the protein family annotation by MPAlignment.|
|--Family.Stat||
|grep|The gene id in the blast output log file are not well format for reading and program processing, so before you generate the venn diagram you should call this command to parse the gene id from the log file. You can also done this id parsing job using other tools.|
|--manual-Build||
|--MPAlignment||
|pure_domain||
|--SelfAlign||

## Commands
--------------------------
##### Help for command '--align':

**Prototype**: ProteinTools.SMART.CLI::Int32 Align(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe --align /query <query.csv> /subject <subject.csv> [/out <out.DIR> /inst]
  Example:      SMART --align 
```

##### Help for command '--align.family':

**Prototype**: ProteinTools.SMART.CLI::Int32 FamilyAlign(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe --align.family /In <In.bbh.csv> /query <query-pfam.csv> /subject <subject-pfam.csv> [/out <out.DIR> /mp <mp-align:0.65> /lev <lev-align:0.65>]
  Example:      SMART --align.family 
```

##### Help for command '-build_cache':

**Prototype**: ProteinTools.SMART.CLI::Int32 BuildCache(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe -build_cache -i <fsa_file> [-o <export_file> -db <cdd_db_name> -cdd <cdd_db_path> -grep_script <script>]
  Example:      SMART -build_cache 
```



  Parameters information:
```
       [-cdd]
    Description:  The cdd database directory, if this switch value is null then system will using the default position in the profile file.

    Example:      -cdd ""


```

#### Accepted Types
##### -cdd
##### Help for command '-buildsmart':

**Prototype**: ProteinTools.SMART.CLI::Int32 BuildSmart(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe 
  Example:      SMART -buildsmart 
```

##### Help for command 'convert':

**Prototype**: ProteinTools.SMART.CLI::Int32 Convert(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe convert -i <input_file> [-o <xml_file>]
  Example:      SMART convert 
```

##### Help for command 'export':

**Prototype**: ProteinTools.SMART.CLI::Int32 Export(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe export -keyword <keyword_list> [-m <any/all>] -o <export_file> [-d <db_name> -casesense <T/F>]
  Example:      SMART export 
```



  Parameters information:
```
    -d
    Description:  This switch value can be both a domain database name or a fasta file path.

    Example:      -d ""

-keyword
    Description:  The keyword list will be use for the sequence record search, each keyword should seperated by comma character.

    Example:      -keyword "HTH,GGDEF,Clp,REC"


```

#### Accepted Types
##### -d
##### -keyword
##### Help for command '--Export.Domains':

**Prototype**: ProteinTools.SMART.CLI::Int32 ExportRegpreciseDomains(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe --Export.Domains /in <pfam-string.csv>
  Example:      SMART --Export.Domains 
```

##### Help for command '--Export.Pfam-String':

**Prototype**: ProteinTools.SMART.CLI::Int32 ExportPfamString(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe --Export.Pfam-String /in <blast_out.txt>
  Example:      SMART --Export.Pfam-String 
```

##### Help for command '--Family.Align':

**Prototype**: ProteinTools.SMART.CLI::Int32 FamilyClassify(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Family Annotation by MPAlignment
  Usage:        G:\GCModeller\manual\bin\SMART.exe --Family.Align /query <pfam-string.csv> [/threshold 0.65 /mp 0.65 /Name <null>]
  Example:      SMART --Family.Align 
```



  Parameters information:
```
       [/Name]
    Description:  The database name of the aligned subject, if this value is empty or not exists in the source, then the entired Family database will be used.

    Example:      /Name ""


```

#### Accepted Types
##### /Name
##### Help for command '--Family.Domains':

**Prototype**: ProteinTools.SMART.CLI::Int32 FamilyDomains(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Build the Family database for the protein family annotation by MPAlignment.
  Usage:        G:\GCModeller\manual\bin\SMART.exe --Family.Domains /regprecise <regulators.fasta> /pfam <pfam-string.csv>
  Example:      SMART --Family.Domains 
```

##### Help for command '--Family.Stat':

**Prototype**: ProteinTools.SMART.CLI::Int32 FamilyStat(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe --Family.Stat /in <anno_out.csv>
  Example:      SMART --Family.Stat 
```

##### Help for command 'grep':

**Prototype**: ProteinTools.SMART.CLI::Int32 Grep(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  The gene id in the blast output log file are not well format for reading and program processing, so before you generate the venn diagram you should call this command to parse the gene id from the log file. You can also done this id parsing job using other tools.
  Usage:        G:\GCModeller\manual\bin\SMART.exe grep -i <xml_log_file> -q <script_statements> -h <script_statements>
  Example:      SMART grep grep -i C:\Users\WORKGROUP\Desktop\blast_xml_logs\1__8004_ecoli_prot.log.xml -q "tokens | 4" -h "'tokens | 2';'tokens ' ' 0'"
```



  Parameters information:
```
    -q
    Description:  The parsing script for parsing the gene_id from the blast log file, this switch value is consist of sevral operation tokens, and each token is separate by the ';' character and the token unit in each script token should seperate by the ' character.
              There are two basic operation in this parsing script:
               tokens - Split the query or hit name string into sevral piece of string by the specific delimiter character and           get the specifc location unit in the return string array.
                 Usage:   tokens <delimiter> <position>
                 Example: tokens | 3 match - match a gene id using a specific pattern regular expression.
                 usage:   match <regular_expression>
                 Example: match .+[-]\d{5}

    Example:      -q "'tokens | 5';'match .+[-].+'"

-h
    Description:  The parsing script for parsing the gene_id from the blast log file, this switch value is consist of sevral operation tokens, and each token is separate by the ';' character and the token unit in each script token should seperate by the ' character.
              There are two basic operation in this parsing script:
               tokens - Split the query or hit name string into sevral piece of string by the specific delimiter character and           get the specifc location unit in the return string array.
                 Usage:   tokens <delimiter> <position>
                 Example: tokens | 3 match - match a gene id using a specific pattern regular expression.
                 usage:   match <regular_expression>
                 Example: match .+[-]\d{5}

    Example:      -h "'tokens | 5';'match .+[-].+'"


```

#### Accepted Types
##### -q
##### -h
##### Help for command '--manual-Build':

**Prototype**: ProteinTools.SMART.CLI::Int32 ManualBuild(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe --manual-Build /pfam-string <pfam-string.csv> /name <familyName>
  Example:      SMART --manual-Build 
```

##### Help for command '--MPAlignment':

**Prototype**: ProteinTools.SMART.CLI::Int32 SBHAlignment(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe --MPAlignment /sbh <sbh.csv> /query <pfam-string.csv> /subject <pfam-string.csv> [/mp <0.65> /out <out.csv>]
  Example:      SMART --MPAlignment 
```

##### Help for command 'pure_domain':

**Prototype**: ProteinTools.SMART.CLI::Int32 FiltePureDomain(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe pure_domain -i <input_smart_log> -o <output_file>
  Example:      SMART pure_domain 
```

##### Help for command '--SelfAlign':

**Prototype**: ProteinTools.SMART.CLI::Int32 SelfAlign(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\SMART.exe --SelfAlign /query <pfam-string.csv> /subject <subject.csv> /aln <mpalignment.csv> [/lstID <lstID.txt> /mp <0.65> /id <id>]
  Example:      SMART --SelfAlign 
```



  Parameters information:
```
       [/lstID]
    Description:  If this parameter is not empty, then the /aln parameter will be disable

    Example:      /lstID ""

   [/id]
    Description:  If this parameter is not null, then the record of this query or hits will be used to subset the alignment set.

    Example:      /id ""


```

#### Accepted Types
##### /lstID
##### /id
