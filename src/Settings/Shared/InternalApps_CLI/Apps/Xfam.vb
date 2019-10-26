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
'  // VERSION:   3.3277.7238.20186
'  // ASSEMBLY:  Settings, Version=3.3277.7238.20186, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/26/2019 11:12:52 AM
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

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Xfam
          Return New Xfam(App:=directory & "/" & Xfam.App)
     End Function

''' <summary>
''' ```
''' /Export.Blastn /in &lt;blastout.txt> [/out &lt;blastn.Csv>]
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
''' ```
''' /Export.Blastn.Batch /in &lt;blastout.DIR> [/out outDIR /large /num_threads &lt;-1> /no_parallel]
''' ```
''' </summary>
'''
Public Function ExportBlastns([in] As String, Optional out As String = "", Optional num_threads As String = "", Optional large As Boolean = False, Optional no_parallel As Boolean = False) As Integer
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
''' ```
''' /Export.hmmscan /in &lt;input_hmmscan.txt> [/evalue 1e-5 /out &lt;pfam.csv>]
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
''' ```
''' /Export.hmmsearch /in &lt;input_hmmsearch.txt> [/prot &lt;query.fasta> /out &lt;pfam.csv>]
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
''' ```
''' /Export.Pfam.UltraLarge /in &lt;blastOUT.txt> [/out &lt;out.csv> /evalue &lt;0.00001> /coverage &lt;0.85> /offset &lt;0.1>]
''' ```
''' Export pfam annotation result from blastp based sequence alignment analysis.
''' </summary>
'''
Public Function ExportUltraLarge([in] As String, Optional out As String = "", Optional evalue As String = "", Optional coverage As String = "", Optional offset As String = "") As Integer
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
''' ```
''' /Export.PfamHits /in &lt;blastp_vs_pfamA.txt> [/alt.direction /evalue &lt;1e-5> /coverage &lt;0.8> /identities &lt;0.7> /out &lt;pfamhits.csv>]
''' ```
''' </summary>
'''
Public Function ExportPfamHits([in] As String, Optional evalue As String = "", Optional coverage As String = "", Optional identities As String = "", Optional out As String = "", Optional alt_direction As Boolean = False) As Integer
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
''' ```
''' /Fasta2Table /in &lt;database.fasta> [/out &lt;table.csv>]
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
''' ```
''' /Load.cmscan /in &lt;stdout.txt> [/out &lt;out.Xml>]
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
''' ```
''' /Load.cmsearch /in &lt;stdio.txt> /out &lt;out.Xml>
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
''' ```
''' /Pfam.Annotation /in &lt;pfamhits.csv> [/out &lt;out.pfamstring.csv>]
''' ```
''' Do pfam functional domain annotation based on the pfam hits result.
''' </summary>
'''
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
''' ```
''' /pfam2go /in &lt;pfamhits.csv> /togo &lt;pfam2go.txt> [/out &lt;annotations.xml>]
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
''' ```
''' /Rfam /in &lt;blastMappings.Csv.DIR> /PTT &lt;pttDIR> [/prefix &lt;sp_prefix> /out &lt;out.Rfam.csv> /offset 10 /non-directed]
''' ```
''' </summary>
'''
Public Function RfamAnalysis([in] As String, PTT As String, Optional prefix As String = "", Optional out As String = "", Optional offset As String = "", Optional non_directed As Boolean = False) As Integer
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
''' ```
''' /Rfam.Align /query &lt;sequence.fasta> [/rfam &lt;DIR> /out &lt;outDIR> /num_threads -1 /ticks 1000]
''' ```
''' </summary>
'''
Public Function RfamAlignment(query As String, Optional rfam As String = "", Optional out As String = "", Optional num_threads As String = "", Optional ticks As String = "") As Integer
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
''' ```
''' /Rfam.GenomicsContext /in &lt;scan_sites.Csv> /PTT &lt;genome.PTT> [/dist 500 /out &lt;out.csv>]
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
''' ```
''' /Rfam.Regulatory /query &lt;RfamilyMappings.csv> /mast &lt;mastsites.csv> [/out &lt;out.csv>]
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
''' ```
''' /Rfam.Regulons /in &lt;cmsearch.hits.csv> /regulons &lt;regprecise.regulons.hits.csv> [/out &lt;out.csv>]
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
''' ```
''' /Rfam.SeedsDb.Dump /in &lt;rfam.seed> [/out &lt;rfam.csv>]
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
''' ```
''' /Rfam.Sites.Seq /nt &lt;nt.fasta> /sites &lt;sites.csv> [/out out.fasta]
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
''' ```
''' --Install.Rfam /seed &lt;rfam.seed>
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
