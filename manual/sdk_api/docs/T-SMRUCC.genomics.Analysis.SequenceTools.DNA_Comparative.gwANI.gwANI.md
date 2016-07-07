---
title: gwANI
---

# gwANI
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.gwANI](N-SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.gwANI.html)_

### pANIto
 Given a multi-FASTA alignment, output the genome wide average nucleotide identity (gwANI) 
 For Each sample against all other samples. A matrix containing the percentages Is outputted. 
 This software loads the whole file into memory.

 #### Usage
 ```
 $ panito
 Usage: panito [-hV] <file>
 This program calculates the genome wide ANI for a multiFASTA alignment.
 -h this help message
 -V print version and exit
 <file> input alignment file which can optionally be gzipped
 ```

 #### Input format
 The input file must be a multi-FASTA file, where all sequences are the same length:

 ```
 >sample1
 AAAAAAAAAA
 >sample2
 AAAAAAAAAC
 >sample3
 AAAAAAAACC
 ```

 #### Output
 ```
 sample1 sample2 sample3
 sample1100.00000090.0000080.000000
 sample2-100.00000090.000000
 sample3--100.000000
 ```

 #### Etymology
 pANIto has 'ani' in the middle. In Spanish it means babylon.

> 
>  ```vbnet
>  
>  Public Sub print_version()
>      Call Console.Write("{0} {1}" & vbLf, DefineConstants.PROGRAM_NAME, PACKAGE_VERSION)
>  End Sub
> 
>  Public Sub Main(argc As Integer, args As String())
>      Dim multi_fasta_filename As String() = {""}
>      Dim output_filename As String() = {""}
> 
>      Dim c As Integer
>      Dim index As Integer
>      Dim output_multi_fasta_file As Integer = 0
> 
>      While (InlineAssignHelper(c, getopt(argc, args, "ho:V"))) <> -1
>          Select Case c
>              Case "V"c
>                  GlobalMembersMain.print_version()
>             
>              Case "o"c
>                  output_filename = optarg.Substring(0, FILENAME_MAX)
>                   
>              Case "h"c
>                  GlobalMembersMain.print_usage()
>                  
>              Case Else
>                  output_multi_fasta_file = 1
>                    
>          End Select
>      End While
> 
>      If optind < argc Then
>          multi_fasta_filename = Convert.ToString(args(optind)).Substring(0, FILENAME_MAX)
>          gwANI.calculate_and_output_gwani(multi_fasta_filename);
>          gwANI.fast_calculate_gwani(multi_fasta_filename)
>      Else
>          Call print_usage()
>      End If
>  End Sub
>  ```
>  


### Methods

#### __fast_calculate_gwani
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.gwANI.gwANI.__fast_calculate_gwani(System.String@)
```
执行入口点

|Parameter Name|Remarks|
|--------------|-------|
|filename|-|


#### calculate_and_output_gwani
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.gwANI.gwANI.calculate_and_output_gwani(System.String@,System.IO.TextWriter)
```
执行入口点

|Parameter Name|Remarks|
|--------------|-------|
|filename|-|
|out|默认是打印在终端之上|


#### fast_calculate_gwani
```csharp
SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.gwANI.gwANI.fast_calculate_gwani(System.String@,System.IO.TextWriter)
```
执行入口点

|Parameter Name|Remarks|
|--------------|-------|
|filename|-|
|out|默认是打印在终端之上|



### Properties

#### out
The result output stream
