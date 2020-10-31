Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\Xfam.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   3.3277.7609.23259
'  // ASSEMBLY:  Settings, Version=3.3277.7609.23259, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/31/2020 12:55:18 PM
'  // 
' 
' 
'  Xfam Tools (Pfam, Rfam, iPfam)
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /Export.Blastn:              
'  /Export.Blastn.Batch:        
'  /Fasta2Table:                Parse and save the pfam sequence fasta database as csv table file. (Debug
'                               used only)
'  /Rfam.Align:                 
'  /Rfam.SeedsDb.Dump:          
'  --Install.Rfam:              
' 
' 
' API list that with functional grouping
' 
' 1. Pfam Annotations CLI tools
' 
' 
'    /Export.hmmscan:             Export result from HMM search based domain annotation result.
'    /Export.hmmsearch:           
'    /Export.Pfam.UltraLarge:     Export pfam annotation result from blastp based sequence alignment analysis.
'    /Export.PfamHits:            
'    /Pfam.Annotation:            Do pfam functional domain annotation based on the pfam hits result.
'    /pfam2go:                    Do go annotation based on the pfam mapping to go term.
' 
' 
' 2. Rfam Annotations CLI tools
' 
' 
'    /Load.cmscan:                
'    /Load.cmsearch:              
'    /Rfam:                       
'    /Rfam.GenomicsContext:       
'    /Rfam.Regulatory:            
'    /Rfam.Regulons:              
'    /Rfam.Sites.seq:             
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Xfam Tools (Pfam, Rfam, iPfam)
''' </summary>
'''
Public Class Xfam : Inherits InteropService

    Public Const App$ = "Xfam.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Xfam
          Return New Xfam(App:=directory & "/" & Xfam.App)
     End Function

''' <summary>
''' ```bash
''' /Export.Blastn /in &lt;blastout.txt&gt; [/out &lt;blastn.Csv&gt;]
''' ```
''' </summary>
'''

Public Function ExportBlastn([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.Blastn")
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
''' ```bash
''' /Export.Blastn.Batch /in &lt;blastout.DIR&gt; [/out outDIR /large /num_threads &lt;-1&gt; /no_parallel]
''' ```
''' </summary>
'''

Public Function ExportBlastns([in] As String, 
                                 Optional out As String = "", 
                                 Optional num_threads As String = "", 
                                 Optional large As Boolean = False, 
                                 Optional no_parallel As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.Blastn.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
    If large Then
        Call CLI.Append("/large ")
    End If
    If no_parallel Then
        Call CLI.Append("/no_parallel ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.hmmscan /in &lt;input_hmmscan.txt&gt; [/evalue 1e-5 /out &lt;pfam.csv&gt;]
''' ```
''' Export result from HMM search based domain annotation result.
''' </summary>
'''

Public Function ExportHMMScan([in] As String, Optional evalue As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.hmmscan")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.hmmsearch /in &lt;input_hmmsearch.txt&gt; [/prot &lt;query.fasta&gt; /out &lt;pfam.csv&gt;]
''' ```
''' </summary>
'''

Public Function ExportHMMSearch([in] As String, Optional prot As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.hmmsearch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not prot.StringEmpty Then
            Call CLI.Append("/prot " & """" & prot & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.Pfam.UltraLarge /in &lt;blastOUT.txt&gt; [/out &lt;out.csv&gt; /evalue &lt;0.00001&gt; /coverage &lt;0.85&gt; /offset &lt;0.1&gt;]
''' ```
''' Export pfam annotation result from blastp based sequence alignment analysis.
''' </summary>
'''
''' <param name="[in]"> The blastp raw output file of alignment in direction protein query vs pfam database.
''' </param>
''' <param name="out"> The pfam annotation output.
''' </param>
''' <param name="offset"> The max allowed offset value of the length delta between ``length_query`` and ``length_hit``.
''' </param>
Public Function ExportUltraLarge([in] As String, 
                                    Optional out As String = "", 
                                    Optional evalue As String = "", 
                                    Optional coverage As String = "", 
                                    Optional offset As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.Pfam.UltraLarge")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not offset.StringEmpty Then
            Call CLI.Append("/offset " & """" & offset & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.PfamHits /in &lt;blastp_vs_pfamA.txt&gt; [/alt.direction /evalue &lt;1e-5&gt; /coverage &lt;0.8&gt; /identities &lt;0.7&gt; /out &lt;pfamhits.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="out"> The output pfam hits result which is parsed from the pfam_vs_protein blastp result.
''' </param>
''' <param name="[in]"> The blastp alignment output of pfamA align with query proteins.
''' </param>
''' <param name="alt_direction"> By default, this cli tools processing the blastp alignment result in direction ``protein_vs_pfam``, 
'''               apply this option argument in cli to switch the processor in direction ``pfam_vs_protein``.
''' </param>
''' <param name="evalue"> E-value cutoff of the blastp alignment result.
''' </param>
''' <param name="coverage"> The coverage cutoff of the pfam domain sequence. This argument is not the coverage threshold of your query protein.
''' </param>
Public Function ExportPfamHits([in] As String, 
                                  Optional evalue As String = "", 
                                  Optional coverage As String = "", 
                                  Optional identities As String = "", 
                                  Optional out As String = "", 
                                  Optional alt_direction As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.PfamHits")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If alt_direction Then
        Call CLI.Append("/alt.direction ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Fasta2Table /in &lt;database.fasta&gt; [/out &lt;table.csv&gt;]
''' ```
''' Parse and save the pfam sequence fasta database as csv table file. (Debug used only)
''' </summary>
'''

Public Function ParseFastaAsTable([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Fasta2Table")
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
''' ```bash
''' /Load.cmscan /in &lt;stdout.txt&gt; [/out &lt;out.Xml&gt;]
''' ```
''' </summary>
'''

Public Function LoadDoc([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Load.cmscan")
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
''' ```bash
''' /Load.cmsearch /in &lt;stdio.txt&gt; /out &lt;out.Xml&gt;
''' ```
''' </summary>
'''

Public Function LoadCMSearch([in] As String, out As String) As Integer
    Dim CLI As New StringBuilder("/Load.cmsearch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Pfam.Annotation /in &lt;pfamhits.csv&gt; [/out &lt;out.pfamstring.csv&gt;]
''' ```
''' Do pfam functional domain annotation based on the pfam hits result.
''' </summary>
'''
''' <param name="[in]"> The pfam hits result from the blastp query output or hmm search output.
''' </param>
''' <param name="out"> The annotation output.
''' </param>
Public Function PfamAnnotation([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Pfam.Annotation")
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
''' ```bash
''' /pfam2go /in &lt;pfamhits.csv&gt; /togo &lt;pfam2go.txt&gt; [/out &lt;annotations.xml&gt;]
''' ```
''' Do go annotation based on the pfam mapping to go term.
''' </summary>
'''

Public Function Pfam2Go([in] As String, togo As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/pfam2go")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/togo " & """" & togo & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Rfam /in &lt;blastMappings.Csv.DIR&gt; /PTT &lt;pttDIR&gt; [/prefix &lt;sp_prefix&gt; /out &lt;out.Rfam.csv&gt; /offset 10 /non-directed]
''' ```
''' </summary>
'''
''' <param name="prefix"> Optional for the custom RNA id, is this parameter value is nothing, then the id prefix will be parsed from the PTT file automaticslly.
''' </param>
Public Function RfamAnalysis([in] As String, 
                                PTT As String, 
                                Optional prefix As String = "", 
                                Optional out As String = "", 
                                Optional offset As String = "", 
                                Optional non_directed As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Rfam")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    If Not prefix.StringEmpty Then
            Call CLI.Append("/prefix " & """" & prefix & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not offset.StringEmpty Then
            Call CLI.Append("/offset " & """" & offset & """ ")
    End If
    If non_directed Then
        Call CLI.Append("/non-directed ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Rfam.Align /query &lt;sequence.fasta&gt; [/rfam &lt;DIR&gt; /out &lt;outDIR&gt; /num_threads -1 /ticks 1000]
''' ```
''' </summary>
'''
''' <param name="formatdb"> If the /rfam directory parameter is specific and the database is not formatted, then this value should be TRUE for local blast. 
'''                    If /rfam parameter is not specific, then the program will using the system database if it is exists, and the database is already be formatted as the installation of the database is includes this formation process.
''' </param>
Public Function RfamAlignment(query As String, 
                                 Optional rfam As String = "", 
                                 Optional out As String = "", 
                                 Optional num_threads As String = "", 
                                 Optional ticks As String = "") As Integer
    Dim CLI As New StringBuilder("/Rfam.Align")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not rfam.StringEmpty Then
            Call CLI.Append("/rfam " & """" & rfam & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
    If Not ticks.StringEmpty Then
            Call CLI.Append("/ticks " & """" & ticks & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Rfam.GenomicsContext /in &lt;scan_sites.Csv&gt; /PTT &lt;genome.PTT&gt; [/dist 500 /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function RfamGenomicsContext([in] As String, PTT As String, Optional dist As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Rfam.GenomicsContext")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    If Not dist.StringEmpty Then
            Call CLI.Append("/dist " & """" & dist & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Rfam.Regulatory /query &lt;RfamilyMappings.csv&gt; /mast &lt;mastsites.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function RfamRegulatory(query As String, mast As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Rfam.Regulatory")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/mast " & """" & mast & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Rfam.Regulons /in &lt;cmsearch.hits.csv&gt; /regulons &lt;regprecise.regulons.hits.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function RFamRegulons([in] As String, regulons As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Rfam.Regulons")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/regulons " & """" & regulons & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Rfam.SeedsDb.Dump /in &lt;rfam.seed&gt; [/out &lt;rfam.csv&gt;]
''' ```
''' </summary>
'''

Public Function DumpSeedsDb([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Rfam.SeedsDb.Dump")
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
''' ```bash
''' /Rfam.Sites.Seq /nt &lt;nt.fasta&gt; /sites &lt;sites.csv&gt; [/out out.fasta]
''' ```
''' </summary>
'''

Public Function RfamSites(nt As String, sites As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Rfam.Sites.Seq")
    Call CLI.Append(" ")
    Call CLI.Append("/nt " & """" & nt & """ ")
    Call CLI.Append("/sites " & """" & sites & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' --Install.Rfam /seed &lt;rfam.seed&gt;
''' ```
''' </summary>
'''

Public Function InstallRfam(seed As String) As Integer
    Dim CLI As New StringBuilder("--Install.Rfam")
    Call CLI.Append(" ")
    Call CLI.Append("/seed " & """" & seed & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
