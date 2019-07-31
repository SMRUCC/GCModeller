#Region "Microsoft.VisualBasic::c7486e198ab3ada804e8a52200c276d2, Shared\InternalApps_CLI\Apps\seqtools.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class seqtools
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: FromEnvironment
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\seqtools.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   1.0.0.0
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // 
' 
' 
'  Sequence operation utilities
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /Count:                                  
'  /Fasta.Subset.Large:                     
'  /Genotype:                               
'  /Genotype.Statics:                       
'  /Loci.describ:                           Testing
'  /logo:                                   * Drawing the sequence logo from the clustal alignment result.
'  /motifs:                                 Populate possible motifs from a give nt fasta sequence dataset.
'  /NeedlemanWunsch.NT:                     
'  /Promoter.Palindrome.Fasta:              
'  /Promoter.Regions.Palindrome:            
'  /Promoter.Regions.Parser.gb:             
'  /Rule.dnaA_gyrB:                         Create a ruler fasta sequence for DNA sequence distance
'                                           computing.
'  /Rule.dnaA_gyrB.Matrix:                  
'  /ruler.dist.calc:                        
'  /Screen.sites:                           
'  /Sites2Fasta:                            Converts the simple segment object collection as fasta file.
'  /SSR:                                    Search for SSR on a nt sequence.
'  -321:                                    Polypeptide sequence 3 letters to 1 lettes sequence.
'  -complement:                             
'  --Drawing.ClustalW:                      
'  -pattern_search:                         Parsing the sequence segment from the sequence source using
'                                           regular expression.
'  -reverse:                                
'  --translates:                            Translates the ORF gene as protein sequence. If any error
'                                           was output from the console, please using > operator dump
'                                           the output to a log file for the analysis.
' 
' 
' API list that with functional grouping
' 
' 1. DNA_Comparative tools
' 
' 
'    /CAI:                                    
'    /gwANI:                                  Given a multi-FASTA alignment, output the genome wide average
'                                             nucleotide identity (gwANI) for Each sample against all
'                                             other samples. A matrix containing the percentages Is outputted.
'    /Sigma:                                  Create a distance similarity matrix for the input sequence.
' 
' 
' 2. Fasta Sequence Tools
' 
'    Tools command that works around the fasta format data.
' 
' 
'    /Compare.By.Locis:                       
'    /Distinct:                               Distinct fasta sequence by sequence content.
'    /Excel.2Fasta:                           Convert the sequence data in a excel annotation file into
'                                             a fasta sequence file.
'    /Get.Locis:                              
'    /Gff.Sites:                              
'    /Merge:                                  Only search for 1 level folder, dit not search receve.
'    /Merge.Simple:                           This tools just merge the fasta sequence into one larger
'                                             file.
'    /Select.By_Locus:                        Select fasta sequence by local_tag.
'    /Split:                                  
'    /subset:                                 
'    -segment:                                
'    --segments:                              
'    --Trim:                                  
' 
' 
' 3. Nucleotide Sequence Property Calculation tools
' 
' 
'    /Mirrors.Context:                        This function will convert the mirror data to the simple
'                                             segment object data
'    /Mirrors.Context.Batch:                  This function will convert the mirror data to the simple
'                                             segment object data
'    /Mirrors.Group:                          
'    /Mirrors.Group.Batch:                    
'    /SimpleSegment.AutoBuild:                
'    /SimpleSegment.Mirrors:                  
'    /SimpleSegment.Mirrors.Batch:            
' 
' 
' 4. Palindrome batch task tools
' 
' 
'    /check.attrs:                            
'    /Palindrome.BatchTask:                   
'    /Palindrome.Workflow:                    
' 
' 
' 5. Sequence Aligner
' 
' 
'    /align.SmithWaterman:                    
'    /Clustal.Cut:                            
'    /nw:                                     RunNeedlemanWunsch
'    --align:                                 
'    --align.Self:                            
' 
' 
' 6. Sequence Palindrome Features Analysis
' 
'    Tools command that using for finding Palindrome sites.
' 
' 
'    /Mirror.Batch:                           
'    /Mirror.Fuzzy:                           
'    /Mirror.Fuzzy.Batch:                     
'    /Mirror.Vector:                          
'    /Mirrors.Nt.Trim:                        
'    /Palindrome.Screen.MaxMatches:           
'    /Palindrome.Screen.MaxMatches.Batch:     
'    --Hairpinks:                             
'    --Hairpinks.batch.task:                  
'    --ImperfectsPalindrome.batch.Task:       
'    --Mirror.From.Fasta:                     Mirror Palindrome, search from a fasta file.
'    --Mirror.From.NT:                        Mirror Palindrome, and this function is for the debugging
'                                             test
'    --Palindrome.batch.Task:                 
'    --Palindrome.From.FASTA:                 
'    --Palindrome.From.NT:                    This function is just for debugger test, /nt parameter is
'                                             the nucleotide sequence data as ATGCCCC
'    --Palindrome.Imperfects:                 Gets all partly matched palindrome sites.
'    --PerfectPalindrome.Filtering:           
'    --ToVector:                              
' 
' 
' 7. Sequence Repeats Loci Search
' 
' 
'    /Write.Seeds:                            
'    Repeats.Density:                         
'    rev-Repeats.Density:                     
'    Search.Batch:                            Batch search for repeats.
' 
' 
' 8. SNP search tools
' 
' 
'    /SNP:                                    
'    /Time.Mutation:                          The ongoing time mutation of the genome sequence.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Sequence operation utilities
''' </summary>
'''
Public Class seqtools : Inherits InteropService

    Public Const App$ = "seqtools.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As seqtools
          Return New seqtools(App:=directory & "/" & seqtools.App)
     End Function

''' <summary>
''' ```
''' /align.SmithWaterman /query &lt;query.fasta> /subject &lt;subject.fasta> [/blosum &lt;matrix.txt> /out &lt;out.xml>]
''' ```
''' </summary>
'''
Public Function Align2(query As String, subject As String, Optional blosum As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/align.SmithWaterman")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
    If Not blosum.StringEmpty Then
            Call CLI.Append("/blosum " & """" & blosum & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /CAI /ORF &lt;orf_nt.fasta> [/out &lt;out.XML>]
''' ```
''' </summary>
'''
Public Function CAI(ORF As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/CAI")
    Call CLI.Append(" ")
    Call CLI.Append("/ORF " & """" & ORF & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /check.attrs /in &lt;in.fasta> /n &lt;attrs.count> [/all]
''' ```
''' </summary>
'''
Public Function CheckHeaders([in] As String, n As String, Optional all As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/check.attrs")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/n " & """" & n & """ ")
    If all Then
        Call CLI.Append("/all ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Clustal.Cut /in &lt;in.fasta> [/left 0.1 /right 0.1 /out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function CutMlAlignment([in] As String, Optional left As String = "", Optional right As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Clustal.Cut")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not left.StringEmpty Then
            Call CLI.Append("/left " & """" & left & """ ")
    End If
    If Not right.StringEmpty Then
            Call CLI.Append("/right " & """" & right & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Compare.By.Locis /file1 &lt;file1.fasta> /file2 &lt;/file2.fasta>
''' ```
''' </summary>
'''
Public Function CompareFile(file1 As String, file2 As String) As Integer
    Dim CLI As New StringBuilder("/Compare.By.Locis")
    Call CLI.Append(" ")
    Call CLI.Append("/file1 " & """" & file1 & """ ")
    Call CLI.Append("/file2 " & """" & file2 & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Count /in &lt;data.fasta>
''' ```
''' </summary>
'''
Public Function Count([in] As String) As Integer
    Dim CLI As New StringBuilder("/Count")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Distinct /in &lt;in.fasta> [/out &lt;out.fasta> /by_Uid &lt;uid_regexp>]
''' ```
''' Distinct fasta sequence by sequence content.
''' </summary>
'''
Public Function Distinct([in] As String, Optional out As String = "", Optional by_uid As String = "") As Integer
    Dim CLI As New StringBuilder("/Distinct")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not by_uid.StringEmpty Then
            Call CLI.Append("/by_uid " & """" & by_uid & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Excel.2Fasta /in &lt;anno.csv> [/out &lt;out.fasta> /attrs &lt;gene;locus_tag;gi;location,...> /seq &lt;Sequence>]
''' ```
''' Convert the sequence data in a excel annotation file into a fasta sequence file.
''' </summary>
'''
Public Function ToFasta([in] As String, Optional out As String = "", Optional attrs As String = "", Optional seq As String = "") As Integer
    Dim CLI As New StringBuilder("/Excel.2Fasta")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not attrs.StringEmpty Then
            Call CLI.Append("/attrs " & """" & attrs & """ ")
    End If
    If Not seq.StringEmpty Then
            Call CLI.Append("/seq " & """" & seq & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Fasta.Subset.Large /in &lt;locus.txt> /db &lt;large_db.fasta> [/keyword.map.multiple /out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function SubsetFastaDb([in] As String, db As String, Optional out As String = "", Optional keyword_map_multiple As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Fasta.Subset.Large")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/db " & """" & db & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If keyword_map_multiple Then
        Call CLI.Append("/keyword.map.multiple ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Genotype /in &lt;raw.csv> [/out &lt;out.Csv>]
''' ```
''' </summary>
'''
Public Function Genotype([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Genotype")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Genotype.Statics /in &lt;in.DIR> [/out &lt;EXPORT>]
''' ```
''' </summary>
'''
Public Function GenotypeStatics([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Genotype.Statics")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Get.Locis /in &lt;locis.csv> /nt &lt;genome.nt.fasta> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function GetSimpleSegments([in] As String, nt As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Get.Locis")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/nt " & """" & nt & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Gff.Sites /fna &lt;genomic.fna> /gff &lt;genome.gff> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function GffSites(fna As String, gff As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Gff.Sites")
    Call CLI.Append(" ")
    Call CLI.Append("/fna " & """" & fna & """ ")
    Call CLI.Append("/gff " & """" & gff & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /gwANI /in &lt;in.fasta> [/fast /out &lt;out.Csv>]
''' ```
''' Given a multi-FASTA alignment, output the genome wide average nucleotide identity (gwANI) for Each sample against all other samples. A matrix containing the percentages Is outputted.
''' </summary>
'''
Public Function gwANIEvaluate([in] As String, Optional out As String = "", Optional fast As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/gwANI")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If fast Then
        Call CLI.Append("/fast ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Loci.describ /ptt &lt;genome-context.ptt> [/test &lt;loci:randomize> /complement /unstrand]
''' ```
''' Testing
''' </summary>
'''
Public Function LociDescript(ptt As String, Optional test As String = "", Optional complement As Boolean = False, Optional unstrand As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Loci.describ")
    Call CLI.Append(" ")
    Call CLI.Append("/ptt " & """" & ptt & """ ")
    If Not test.StringEmpty Then
            Call CLI.Append("/test " & """" & test & """ ")
    End If
    If complement Then
        Call CLI.Append("/complement ")
    End If
    If unstrand Then
        Call CLI.Append("/unstrand ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /logo /in &lt;clustal.fasta> [/out &lt;out.png> /title ""]
''' ```
''' * Drawing the sequence logo from the clustal alignment result.
''' </summary>
'''
Public Function SequenceLogo([in] As String, Optional out As String = "", Optional title As String = "") As Integer
    Dim CLI As New StringBuilder("/logo")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not title.StringEmpty Then
            Call CLI.Append("/title " & """" & title & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Merge /in &lt;fasta.DIR> [/out &lt;out.fasta> /trim /unique /ext &lt;*.fasta> /brief]
''' ```
''' Only search for 1 level folder, dit not search receve.
''' </summary>
'''
Public Function Merge([in] As String, Optional out As String = "", Optional ext As String = "", Optional trim As Boolean = False, Optional unique As Boolean = False, Optional brief As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Merge")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not ext.StringEmpty Then
            Call CLI.Append("/ext " & """" & ext & """ ")
    End If
    If trim Then
        Call CLI.Append("/trim ")
    End If
    If unique Then
        Call CLI.Append("/unique ")
    End If
    If brief Then
        Call CLI.Append("/brief ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Merge.Simple /in &lt;DIR> [/exts &lt;default:*.fasta,*.fa> /line.break 120 /out &lt;out.fasta>]
''' ```
''' This tools just merge the fasta sequence into one larger file.
''' </summary>
'''
Public Function SimpleMerge([in] As String, Optional exts As String = "", Optional line_break As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Merge.Simple")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not exts.StringEmpty Then
            Call CLI.Append("/exts " & """" & exts & """ ")
    End If
    If Not line_break.StringEmpty Then
            Call CLI.Append("/line.break " & """" & line_break & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Mirror.Batch /nt &lt;nt.fasta> [/out &lt;out.csv> /mp /min &lt;3> /max &lt;20> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function MirrorBatch(nt As String, Optional out As String = "", Optional min As String = "", Optional max As String = "", Optional num_threads As String = "", Optional mp As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Mirror.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/nt " & """" & nt & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
    If mp Then
        Call CLI.Append("/mp ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Mirror.Fuzzy /in &lt;in.fasta> [/out &lt;out.csv> /cut 0.6 /max-dist 6 /min 3 /max 20]
''' ```
''' </summary>
'''
Public Function FuzzyMirrors([in] As String, Optional out As String = "", Optional cut As String = "", Optional max_dist As String = "", Optional min As String = "", Optional max As String = "") As Integer
    Dim CLI As New StringBuilder("/Mirror.Fuzzy")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not cut.StringEmpty Then
            Call CLI.Append("/cut " & """" & cut & """ ")
    End If
    If Not max_dist.StringEmpty Then
            Call CLI.Append("/max-dist " & """" & max_dist & """ ")
    End If
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Mirror.Fuzzy.Batch /in &lt;in.fasta/DIR> [/out &lt;out.DIR> /cut 0.6 /max-dist 6 /min 3 /max 20 /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function FuzzyMirrorsBatch([in] As String, Optional out As String = "", Optional cut As String = "", Optional max_dist As String = "", Optional min As String = "", Optional max As String = "", Optional num_threads As String = "") As Integer
    Dim CLI As New StringBuilder("/Mirror.Fuzzy.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not cut.StringEmpty Then
            Call CLI.Append("/cut " & """" & cut & """ ")
    End If
    If Not max_dist.StringEmpty Then
            Call CLI.Append("/max-dist " & """" & max_dist & """ ")
    End If
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Mirror.Vector /in &lt;inDIR> /size &lt;genome.size> [/out out.txt]
''' ```
''' </summary>
'''
Public Function MirrorsVector([in] As String, size As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Mirror.Vector")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/size " & """" & size & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Mirrors.Context /in &lt;mirrors.csv> /PTT &lt;genome.ptt> [/trans /strand &lt;+/-> /out &lt;out.csv> /stranded /dist &lt;500bp>]
''' ```
''' This function will convert the mirror data to the simple segment object data
''' </summary>
'''
Public Function MirrorContext([in] As String, PTT As String, Optional strand As String = "", Optional out As String = "", Optional dist As String = "", Optional trans As Boolean = False, Optional stranded As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Mirrors.Context")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    If Not strand.StringEmpty Then
            Call CLI.Append("/strand " & """" & strand & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not dist.StringEmpty Then
            Call CLI.Append("/dist " & """" & dist & """ ")
    End If
    If trans Then
        Call CLI.Append("/trans ")
    End If
    If stranded Then
        Call CLI.Append("/stranded ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Mirrors.Context.Batch /in &lt;mirrors.csv.DIR> /PTT &lt;genome.ptt.DIR> [/trans /strand &lt;+/-> /out &lt;out.csv> /stranded /dist &lt;500bp> /num_threads -1]
''' ```
''' This function will convert the mirror data to the simple segment object data
''' </summary>
'''
Public Function MirrorContextBatch([in] As String, PTT As String, Optional strand As String = "", Optional out As String = "", Optional dist As String = "", Optional num_threads As String = "", Optional trans As Boolean = False, Optional stranded As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Mirrors.Context.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    If Not strand.StringEmpty Then
            Call CLI.Append("/strand " & """" & strand & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not dist.StringEmpty Then
            Call CLI.Append("/dist " & """" & dist & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
    If trans Then
        Call CLI.Append("/trans ")
    End If
    If stranded Then
        Call CLI.Append("/stranded ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Mirrors.Group /in &lt;mirrors.Csv> [/batch /fuzzy &lt;-1> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function MirrorGroups([in] As String, Optional fuzzy As String = "", Optional out As String = "", Optional batch As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Mirrors.Group")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not fuzzy.StringEmpty Then
            Call CLI.Append("/fuzzy " & """" & fuzzy & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If batch Then
        Call CLI.Append("/batch ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Mirrors.Group.Batch /in &lt;mirrors.DIR> [/fuzzy &lt;-1> /out &lt;out.DIR> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function MirrorGroupsBatch([in] As String, Optional fuzzy As String = "", Optional out As String = "", Optional num_threads As String = "") As Integer
    Dim CLI As New StringBuilder("/Mirrors.Group.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not fuzzy.StringEmpty Then
            Call CLI.Append("/fuzzy " & """" & fuzzy & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Mirrors.Nt.Trim /in &lt;mirrors.Csv> [/out &lt;out.Csv>]
''' ```
''' </summary>
'''
Public Function TrimNtMirrors([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Mirrors.Nt.Trim")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /motifs /in &lt;data.fasta> [/min.w &lt;default=6> /max.w &lt;default=20> /n.motifs &lt;default=25> /n.occurs &lt;default=6> /out &lt;out.directory>]
''' ```
''' Populate possible motifs from a give nt fasta sequence dataset.
''' </summary>
'''
Public Function FindMotifs([in] As String, Optional min_w As String = "6", Optional max_w As String = "20", Optional n_motifs As String = "25", Optional n_occurs As String = "6", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/motifs")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not min_w.StringEmpty Then
            Call CLI.Append("/min.w " & """" & min_w & """ ")
    End If
    If Not max_w.StringEmpty Then
            Call CLI.Append("/max.w " & """" & max_w & """ ")
    End If
    If Not n_motifs.StringEmpty Then
            Call CLI.Append("/n.motifs " & """" & n_motifs & """ ")
    End If
    If Not n_occurs.StringEmpty Then
            Call CLI.Append("/n.occurs " & """" & n_occurs & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /NeedlemanWunsch.NT /query &lt;nt> /subject &lt;nt>
''' ```
''' </summary>
'''
Public Function NWNT(query As String, subject As String) As Integer
    Dim CLI As New StringBuilder("/NeedlemanWunsch.NT")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /nw /query &lt;query.fasta> /subject &lt;subject.fasta> [/out &lt;out.txt>]
''' ```
''' RunNeedlemanWunsch
''' </summary>
'''
Public Function NW(query As String, subject As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/nw")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Palindrome.BatchTask /in &lt;in.DIR> [/num_threads 4 /min 3 /max 20 /min-appears 2 /cutoff &lt;0.6> /Palindrome /max-dist &lt;1000 (bp)> /partitions &lt;-1> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function PalindromeBatchTask([in] As String, Optional num_threads As String = "", Optional min As String = "", Optional max As String = "", Optional min_appears As String = "", Optional cutoff As String = "", Optional max_dist As String = "", Optional partitions As String = "", Optional out As String = "", Optional palindrome As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Palindrome.BatchTask")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If Not min_appears.StringEmpty Then
            Call CLI.Append("/min-appears " & """" & min_appears & """ ")
    End If
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If Not max_dist.StringEmpty Then
            Call CLI.Append("/max-dist " & """" & max_dist & """ ")
    End If
    If Not partitions.StringEmpty Then
            Call CLI.Append("/partitions " & """" & partitions & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If palindrome Then
        Call CLI.Append("/palindrome ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Palindrome.Screen.MaxMatches /in &lt;in.csv> /min &lt;min.max-matches> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function FilteringMatches([in] As String, min As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Palindrome.Screen.MaxMatches")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/min " & """" & min & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Palindrome.Screen.MaxMatches.Batch /in &lt;inDIR> /min &lt;min.max-matches> [/out &lt;out.DIR> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function FilteringMatchesBatch([in] As String, min As String, Optional out As String = "", Optional num_threads As String = "") As Integer
    Dim CLI As New StringBuilder("/Palindrome.Screen.MaxMatches.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/min " & """" & min & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Palindrome.Workflow /in &lt;in.fasta> [/batch /min-appears 2 /min 3 /max 20 /cutoff &lt;0.6> /max-dist &lt;1000 (bp)> /Palindrome /partitions &lt;-1> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function PalindromeWorkflow([in] As String, Optional min_appears As String = "", Optional min As String = "", Optional max As String = "", Optional cutoff As String = "", Optional max_dist As String = "", Optional partitions As String = "", Optional out As String = "", Optional batch As Boolean = False, Optional palindrome As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Palindrome.Workflow")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not min_appears.StringEmpty Then
            Call CLI.Append("/min-appears " & """" & min_appears & """ ")
    End If
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If Not max_dist.StringEmpty Then
            Call CLI.Append("/max-dist " & """" & max_dist & """ ")
    End If
    If Not partitions.StringEmpty Then
            Call CLI.Append("/partitions " & """" & partitions & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If batch Then
        Call CLI.Append("/batch ")
    End If
    If palindrome Then
        Call CLI.Append("/palindrome ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Promoter.Palindrome.Fasta /in &lt;palindrome.csv> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function PromoterPalindrome2Fasta([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Promoter.Palindrome.Fasta")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Promoter.Regions.Palindrome /in &lt;genbank.gb> [/min &lt;3> /max &lt;20> /len &lt;100,150,200,250,300,400,500, default:=250> /mirror /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function PromoterRegionPalindrome([in] As String, Optional min As String = "", Optional max As String = "", Optional len As String = "", Optional out As String = "", Optional mirror As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Promoter.Regions.Palindrome")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If Not len.StringEmpty Then
            Call CLI.Append("/len " & """" & len & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If mirror Then
        Call CLI.Append("/mirror ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Promoter.Regions.Parser.gb /gb &lt;genbank.gb> [/out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function PromoterRegionParser_gb(gb As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Promoter.Regions.Parser.gb")
    Call CLI.Append(" ")
    Call CLI.Append("/gb " & """" & gb & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Rule.dnaA_gyrB /genome &lt;genbank.gb> [/out &lt;out.fasta>]
''' ```
''' Create a ruler fasta sequence for DNA sequence distance computing.
''' </summary>
'''
Public Function dnaA_gyrB_rule(genome As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Rule.dnaA_gyrB")
    Call CLI.Append(" ")
    Call CLI.Append("/genome " & """" & genome & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Rule.dnaA_gyrB.Matrix /genomes &lt;genomes.gb.DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function RuleMatrix(genomes As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Rule.dnaA_gyrB.Matrix")
    Call CLI.Append(" ")
    Call CLI.Append("/genomes " & """" & genomes & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /ruler.dist.calc /in &lt;ruler.fasta> /genomes &lt;genome.gb.DIR> [/winSize &lt;default=1000> /step &lt;default=500> /out &lt;out.csv.dir>]
''' ```
''' </summary>
'''
Public Function RulerSlideWindowMatrix([in] As String, genomes As String, Optional winsize As String = "1000", Optional [step] As String = "500", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/ruler.dist.calc")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/genomes " & """" & genomes & """ ")
    If Not winsize.StringEmpty Then
            Call CLI.Append("/winsize " & """" & winsize & """ ")
    End If
    If Not [step].StringEmpty Then
            Call CLI.Append("/step " & """" & [step] & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Screen.sites /in &lt;DIR/sites.csv> /range &lt;min_bp>,&lt;max_bp> [/type &lt;type,default:=RepeatsView,alt:RepeatsView,RevRepeatsView,PalindromeLoci,ImperfectPalindrome> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ScreenRepeats([in] As String, range As String, Optional type As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Screen.sites")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/range " & """" & range & """ ")
    If Not type.StringEmpty Then
            Call CLI.Append("/type " & """" & type & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Select.By_Locus /in &lt;locus.txt/csv> /fa &lt;fasta/.inDIR> [/field &lt;columnName> /reverse /out &lt;out.fasta>]
''' ```
''' Select fasta sequence by local_tag.
''' </summary>
'''
Public Function SelectByLocus([in] As String, fa As String, Optional field As String = "", Optional out As String = "", Optional reverse As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Select.By_Locus")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/fa " & """" & fa & """ ")
    If Not field.StringEmpty Then
            Call CLI.Append("/field " & """" & field & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If reverse Then
        Call CLI.Append("/reverse ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Sigma /in &lt;in.fasta> [/out &lt;out.Csv> /simple /round &lt;-1>]
''' ```
''' Create a distance similarity matrix for the input sequence.
''' </summary>
'''
Public Function Sigma([in] As String, Optional out As String = "", Optional round As String = "", Optional simple As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Sigma")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not round.StringEmpty Then
            Call CLI.Append("/round " & """" & round & """ ")
    End If
    If simple Then
        Call CLI.Append("/simple ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SimpleSegment.AutoBuild /in &lt;locis.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ConvertsAuto([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/SimpleSegment.AutoBuild")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SimpleSegment.Mirrors /in &lt;in.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ConvertMirrors([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/SimpleSegment.Mirrors")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SimpleSegment.Mirrors.Batch /in &lt;in.DIR> [/out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function ConvertMirrorsBatch([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/SimpleSegment.Mirrors.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Sites2Fasta /in &lt;segments.csv> [/assemble /out &lt;out.fasta>]
''' ```
''' Converts the simple segment object collection as fasta file.
''' </summary>
'''
Public Function Sites2Fasta([in] As String, Optional out As String = "", Optional assemble As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Sites2Fasta")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If assemble Then
        Call CLI.Append("/assemble ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SNP /in &lt;nt.fasta> [/ref &lt;int_index/title, default:0> /pure /monomorphic /high &lt;0.65>]
''' ```
''' </summary>
'''
Public Function SNP([in] As String, Optional ref As String = "", Optional high As String = "", Optional pure As Boolean = False, Optional monomorphic As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/SNP")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not ref.StringEmpty Then
            Call CLI.Append("/ref " & """" & ref & """ ")
    End If
    If Not high.StringEmpty Then
            Call CLI.Append("/high " & """" & high & """ ")
    End If
    If pure Then
        Call CLI.Append("/pure ")
    End If
    If monomorphic Then
        Call CLI.Append("/monomorphic ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Split /in &lt;in.fasta> [/n &lt;4096> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function Split([in] As String, Optional n As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Split")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not n.StringEmpty Then
            Call CLI.Append("/n " & """" & n & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SSR /in &lt;nt.fasta> [/range &lt;default=2,6> /parallel /out &lt;out.csv/DIR>]
''' ```
''' Search for SSR on a nt sequence.
''' </summary>
'''
Public Function SSRFinder([in] As String, Optional range As String = "2,6", Optional out As String = "", Optional parallel As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/SSR")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not range.StringEmpty Then
            Call CLI.Append("/range " & """" & range & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If parallel Then
        Call CLI.Append("/parallel ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /subset /lstID &lt;lstID.txt> /fa &lt;source.fasta>
''' ```
''' </summary>
'''
Public Function SubSet(lstID As String, fa As String) As Integer
    Dim CLI As New StringBuilder("/subset")
    Call CLI.Append(" ")
    Call CLI.Append("/lstID " & """" & lstID & """ ")
    Call CLI.Append("/fa " & """" & fa & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Time.Mutation /in &lt;aln.fasta> [/ref &lt;default:first,other:title/index> /cumulative /out &lt;out.csv>]
''' ```
''' The ongoing time mutation of the genome sequence.
''' </summary>
'''
Public Function TimeDiffs([in] As String, Optional ref As String = "", Optional out As String = "", Optional cumulative As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Time.Mutation")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not ref.StringEmpty Then
            Call CLI.Append("/ref " & """" & ref & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If cumulative Then
        Call CLI.Append("/cumulative ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Write.Seeds /out &lt;out.dat> [/prot /max &lt;20>]
''' ```
''' </summary>
'''
Public Function WriteSeeds(out As String, Optional max As String = "", Optional prot As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Write.Seeds")
    Call CLI.Append(" ")
    Call CLI.Append("/out " & """" & out & """ ")
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If prot Then
        Call CLI.Append("/prot ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' -321 /in &lt;sequence.txt> [/out &lt;out.fasta>]
''' ```
''' Polypeptide sequence 3 letters to 1 lettes sequence.
''' </summary>
'''
Public Function PolypeptideBriefs([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("-321")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --align /query &lt;query.fasta> /subject &lt;subject.fasta> [/out &lt;out.DIR> /cost &lt;0.7>]
''' ```
''' </summary>
'''
Public Function Align(query As String, subject As String, Optional out As String = "", Optional cost As String = "") As Integer
    Dim CLI As New StringBuilder("--align")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not cost.StringEmpty Then
            Call CLI.Append("/cost " & """" & cost & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --align.Self /query &lt;query.fasta> /out &lt;out.DIR> [/cost 0.75]
''' ```
''' </summary>
'''
Public Function AlignSelf(query As String, out As String, Optional cost As String = "") As Integer
    Dim CLI As New StringBuilder("--align.Self")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If Not cost.StringEmpty Then
            Call CLI.Append("/cost " & """" & cost & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' -complement -i &lt;input_fasta> [-o &lt;output_fasta>]
''' ```
''' </summary>
'''
Public Function Complement(i As String, Optional o As String = "") As Integer
    Dim CLI As New StringBuilder("-complement")
    Call CLI.Append(" ")
    Call CLI.Append("-i " & """" & i & """ ")
    If Not o.StringEmpty Then
            Call CLI.Append("-o " & """" & o & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Drawing.ClustalW /in &lt;align.fasta> [/out &lt;out.png> /dot.Size 10]
''' ```
''' </summary>
'''
Public Function DrawClustalW([in] As String, Optional out As String = "", Optional dot_size As String = "") As Integer
    Dim CLI As New StringBuilder("--Drawing.ClustalW")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not dot_size.StringEmpty Then
            Call CLI.Append("/dot.size " & """" & dot_size & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Hairpinks /in &lt;in.fasta> [/out &lt;out.csv> /min &lt;6> /max &lt;7> /cutoff 3 /max-dist &lt;35 (bp)>]
''' ```
''' </summary>
'''
Public Function Hairpinks([in] As String, Optional out As String = "", Optional min As String = "", Optional max As String = "", Optional cutoff As String = "", Optional max_dist As String = "") As Integer
    Dim CLI As New StringBuilder("--Hairpinks")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If Not max_dist.StringEmpty Then
            Call CLI.Append("/max-dist " & """" & max_dist & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Hairpinks.batch.task /in &lt;in.fasta> [/out &lt;outDIR> /min &lt;6> /max &lt;7> /cutoff &lt;0.6> /max-dist &lt;35 (bp)> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function HairpinksBatch([in] As String, Optional out As String = "", Optional min As String = "", Optional max As String = "", Optional cutoff As String = "", Optional max_dist As String = "", Optional num_threads As String = "") As Integer
    Dim CLI As New StringBuilder("--Hairpinks.batch.task")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If Not max_dist.StringEmpty Then
            Call CLI.Append("/max-dist " & """" & max_dist & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --ImperfectsPalindrome.batch.Task /in &lt;in.fasta> /out &lt;outDir> [/min &lt;3> /max &lt;20> /cutoff &lt;0.6> /max-dist &lt;1000 (bp)> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function BatchSearchImperfectsPalindrome([in] As String, out As String, Optional min As String = "", Optional max As String = "", Optional cutoff As String = "", Optional max_dist As String = "", Optional num_threads As String = "") As Integer
    Dim CLI As New StringBuilder("--ImperfectsPalindrome.batch.Task")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If Not max_dist.StringEmpty Then
            Call CLI.Append("/max-dist " & """" & max_dist & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Mirror.From.Fasta /nt &lt;nt-sequence.fasta> [/out &lt;out.csv> /min &lt;3> /max &lt;20>]
''' ```
''' Mirror Palindrome, search from a fasta file.
''' </summary>
'''
Public Function SearchMirrotFasta(nt As String, Optional out As String = "", Optional min As String = "", Optional max As String = "") As Integer
    Dim CLI As New StringBuilder("--Mirror.From.Fasta")
    Call CLI.Append(" ")
    Call CLI.Append("/nt " & """" & nt & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Mirror.From.NT /nt &lt;nt-sequence> /out &lt;out.csv> [/min &lt;3> /max &lt;20>]
''' ```
''' Mirror Palindrome, and this function is for the debugging test
''' </summary>
'''
Public Function SearchMirrotNT(nt As String, out As String, Optional min As String = "", Optional max As String = "") As Integer
    Dim CLI As New StringBuilder("--Mirror.From.NT")
    Call CLI.Append(" ")
    Call CLI.Append("/nt " & """" & nt & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Palindrome.batch.Task /in &lt;in.fasta> /out &lt;outDir> [/min &lt;3> /max &lt;20> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function BatchSearchPalindrome([in] As String, out As String, Optional min As String = "", Optional max As String = "", Optional num_threads As String = "") As Integer
    Dim CLI As New StringBuilder("--Palindrome.batch.Task")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Palindrome.From.Fasta /nt &lt;nt-sequence.fasta> [/out &lt;out.csv> /min &lt;3> /max &lt;20>]
''' ```
''' </summary>
'''
Public Function SearchPalindromeFasta(nt As String, Optional out As String = "", Optional min As String = "", Optional max As String = "") As Integer
    Dim CLI As New StringBuilder("--Palindrome.From.Fasta")
    Call CLI.Append(" ")
    Call CLI.Append("/nt " & """" & nt & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Palindrome.From.NT /nt &lt;nt-sequence> /out &lt;out.csv> [/min &lt;3> /max &lt;20>]
''' ```
''' This function is just for debugger test, /nt parameter is the nucleotide sequence data as ATGCCCC
''' </summary>
'''
Public Function SearchPalindromeNT(nt As String, out As String, Optional min As String = "", Optional max As String = "") As Integer
    Dim CLI As New StringBuilder("--Palindrome.From.NT")
    Call CLI.Append(" ")
    Call CLI.Append("/nt " & """" & nt & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Palindrome.Imperfects /in &lt;in.fasta> [/out &lt;out.csv> /min &lt;3> /max &lt;20> /cutoff &lt;0.6> /max-dist &lt;1000 (bp)> /partitions &lt;-1>]
''' ```
''' Gets all partly matched palindrome sites.
''' </summary>
'''
Public Function ImperfectPalindrome([in] As String, Optional out As String = "", Optional min As String = "", Optional max As String = "", Optional cutoff As String = "", Optional max_dist As String = "", Optional partitions As String = "") As Integer
    Dim CLI As New StringBuilder("--Palindrome.Imperfects")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If Not max_dist.StringEmpty Then
            Call CLI.Append("/max-dist " & """" & max_dist & """ ")
    End If
    If Not partitions.StringEmpty Then
            Call CLI.Append("/partitions " & """" & partitions & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' -pattern_search -i &lt;file_name> -p &lt;regex_pattern> [-o &lt;output_directory> -f &lt;format:fsa/gbk>]
''' ```
''' Parsing the sequence segment from the sequence source using regular expression.
''' </summary>
'''
Public Function PatternSearchA(i As String, p As String, Optional o As String = "", Optional f As String = "") As Integer
    Dim CLI As New StringBuilder("-pattern_search")
    Call CLI.Append(" ")
    Call CLI.Append("-i " & """" & i & """ ")
    Call CLI.Append("-p " & """" & p & """ ")
    If Not o.StringEmpty Then
            Call CLI.Append("-o " & """" & o & """ ")
    End If
    If Not f.StringEmpty Then
            Call CLI.Append("-f " & """" & f & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --PerfectPalindrome.Filtering /in &lt;inDIR> [/min &lt;8> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function FilterPerfectPalindrome([in] As String, Optional min As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("--PerfectPalindrome.Filtering")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' Repeats.Density /dir &lt;dir> /size &lt;size> /ref &lt;refName> [/out &lt;out.csv> /cutoff &lt;default:=0>]
''' ```
''' </summary>
'''
Public Function RepeatsDensity(dir As String, size As String, ref As String, Optional out As String = "", Optional cutoff As String = "") As Integer
    Dim CLI As New StringBuilder("Repeats.Density")
    Call CLI.Append(" ")
    Call CLI.Append("/dir " & """" & dir & """ ")
    Call CLI.Append("/size " & """" & size & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' -reverse -i &lt;input_fasta> [-o &lt;output_fasta>]
''' ```
''' </summary>
'''
Public Function Reverse(i As String, Optional o As String = "") As Integer
    Dim CLI As New StringBuilder("-reverse")
    Call CLI.Append(" ")
    Call CLI.Append("-i " & """" & i & """ ")
    If Not o.StringEmpty Then
            Call CLI.Append("-o " & """" & o & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' rev-Repeats.Density /dir &lt;dir> /size &lt;size> /ref &lt;refName> [/out &lt;out.csv> /cutoff &lt;default:=0>]
''' ```
''' </summary>
'''
Public Function revRepeatsDensity(dir As String, size As String, ref As String, Optional out As String = "", Optional cutoff As String = "") As Integer
    Dim CLI As New StringBuilder("rev-Repeats.Density")
    Call CLI.Append(" ")
    Call CLI.Append("/dir " & """" & dir & """ ")
    Call CLI.Append("/size " & """" & size & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' Search.Batch /aln &lt;alignment.fasta> [/min 3 /max 20 /min-rep 2 /out &lt;./>]
''' ```
''' Batch search for repeats.
''' </summary>
'''
Public Function BatchSearch(aln As String, Optional min As String = "", Optional max As String = "", Optional min_rep As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("Search.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/aln " & """" & aln & """ ")
    If Not min.StringEmpty Then
            Call CLI.Append("/min " & """" & min & """ ")
    End If
    If Not max.StringEmpty Then
            Call CLI.Append("/max " & """" & max & """ ")
    End If
    If Not min_rep.StringEmpty Then
            Call CLI.Append("/min-rep " & """" & min_rep & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' -segment /fasta &lt;Fasta_Token> [-loci &lt;loci>] [/left &lt;left> /length &lt;length> /right &lt;right> [/reverse]] [/ptt &lt;ptt> /geneID &lt;gene_id> /dist &lt;distance> /downstream] -o &lt;saved> [-line.break 100]
''' ```
''' </summary>
'''
Public Function GetSegment(fasta As String, Optional loci As String = "", Optional length As String = "", Optional right As String = "", Optional __reverse__ As String = "", Optional geneid As String = "", Optional dist As String = "", Optional o As String = "", Optional __line_break As String = "", Optional downstream_ As Boolean = False) As Integer
    Dim CLI As New StringBuilder("-segment")
    Call CLI.Append(" ")
    Call CLI.Append("/fasta " & """" & fasta & """ ")
    If Not loci.StringEmpty Then
            Call CLI.Append("-loci " & """" & loci & """ ")
    End If
    If Not length.StringEmpty Then
            Call CLI.Append("/length " & """" & length & """ ")
    End If
    If Not right.StringEmpty Then
            Call CLI.Append("/right " & """" & right & """ ")
    End If
    If Not __reverse__.StringEmpty Then
            Call CLI.Append("[/reverse]] " & """" & __reverse__ & """ ")
    End If
    If Not geneid.StringEmpty Then
            Call CLI.Append("/geneid " & """" & geneid & """ ")
    End If
    If Not dist.StringEmpty Then
            Call CLI.Append("/dist " & """" & dist & """ ")
    End If
    If Not o.StringEmpty Then
            Call CLI.Append("-o " & """" & o & """ ")
    End If
    If Not __line_break.StringEmpty Then
            Call CLI.Append("[-line.break " & """" & __line_break & """ ")
    End If
    If downstream_ Then
        Call CLI.Append("/downstream] ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --segments /regions &lt;regions.csv> /fasta &lt;nt.fasta> [/complement /reversed /brief-dump]
''' ```
''' </summary>
'''
Public Function GetSegments(regions As String, fasta As String, Optional complement As Boolean = False, Optional reversed As Boolean = False, Optional brief_dump As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--segments")
    Call CLI.Append(" ")
    Call CLI.Append("/regions " & """" & regions & """ ")
    Call CLI.Append("/fasta " & """" & fasta & """ ")
    If complement Then
        Call CLI.Append("/complement ")
    End If
    If reversed Then
        Call CLI.Append("/reversed ")
    End If
    If brief_dump Then
        Call CLI.Append("/brief-dump ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --ToVector /in &lt;in.DIR> /min &lt;4> /max &lt;8> /out &lt;out.txt> /size &lt;genome.size>
''' ```
''' </summary>
'''
Public Function ToVector([in] As String, min As String, max As String, out As String, size As String) As Integer
    Dim CLI As New StringBuilder("--ToVector")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/min " & """" & min & """ ")
    Call CLI.Append("/max " & """" & max & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    Call CLI.Append("/size " & """" & size & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --translates /orf &lt;orf.fasta> [/transl_table 1 /force]
''' ```
''' Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.
''' </summary>
'''
Public Function Translates(orf As String, Optional transl_table As String = "", Optional force As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--translates")
    Call CLI.Append(" ")
    Call CLI.Append("/orf " & """" & orf & """ ")
    If Not transl_table.StringEmpty Then
            Call CLI.Append("/transl_table " & """" & transl_table & """ ")
    End If
    If force Then
        Call CLI.Append("/force ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --Trim /in &lt;in.fasta> [/case &lt;u/l> /break &lt;-1/int> /out &lt;out.fasta> /brief]
''' ```
''' </summary>
'''
Public Function Trim([in] As String, Optional [case] As String = "", Optional break As String = "", Optional out As String = "", Optional brief As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--Trim")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not [case].StringEmpty Then
            Call CLI.Append("/case " & """" & [case] & """ ")
    End If
    If Not break.StringEmpty Then
            Call CLI.Append("/break " & """" & break & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If brief Then
        Call CLI.Append("/brief ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace

