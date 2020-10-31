Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\RegPrecise.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   3.3277.7609.23646
'  // ASSEMBLY:  Settings, Version=3.3277.7609.23646, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/31/2020 1:08:12 PM
'  // 
' 
' 
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /Build.Operons:                     If the /regprecise parameter is not presented, then you should
'                                      install the regprecise in the GCModeller database repostiory
'                                      first.
'  /Build.Regulons.Batch:              
'  /CORN:                              Join two vertices by edge if the correspondent operons:
'                                      i) are orthologous;
'                                      ii) have cantiodate transcription factor binding
'                                      sites.
'                                      Collect all linked components. Two operons from
'                                      two different genomes are called orthologous if they share at
'                                      least one orthologous gene.
'  /CORN.Batch:                        
'  /CORN.thread:                       
'  /DOOR.Merge:                        
'  /Export.Regprecise.motifs:          Export Regprecise motif sites as a single fasta sequence file.
'  /Export.Regulators:                 Exports all of the fasta sequence of the TF regulator from the
'                                      download RegPrecsie FASTA database.
'  /Family.Hits:                       
'  /Fetches:                           
'  /Fetches.Thread:                    
'  /Gets.Sites.Genes:                  
'  /heap.Supports:                     
'  /install.motifs:                    
'  /Maps.Effector:                     
'  /Merge.CORN:                        
'  /Merge.RegPrecise.Fasta:            
'  /Prot_Motifs.EXPORT.pfamString:     
'  /Prot_Motifs.PfamString:            
'  /ProtMotifs.Downloads:              Download protein domain motifs structures from KEGG ssdb.
'  /Repository.Fetch:                  
'  /Rfam.Regulates:                    
'  /Select.TF.BBH:                     
'  /Select.TF.Pfam-String:             
'  /siRNA.Maps:                        
' 
' 
' API list that with functional grouping
' 
' 1. Regulon Builders
' 
' 
'    /regulators.bbh:                    Compiles for the regulators in the bacterial genome mapped on
'                                        the regprecise database using bbh method.
' 
' 
' 2. Web api
' 
' 
'    /Download.Motifs:                   
'    /Download.Regprecise:               Download Regprecise database from Web API
'    /Fasta.Downloads:                   Download protein fasta sequence from KEGG database.
'    Regprecise.Compile:                 The repository parameter is a directory path which is the regprecise
'                                        database root directory in the GCModeller directory, if you didn't
'                                        know how to set this value, please leave it blank.
'    wGet.Regprecise:                    Download Regprecise database from REST API
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
'''
''' </summary>
'''
Public Class RegPrecise : Inherits InteropService

    Public Const App$ = "RegPrecise.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As RegPrecise
          Return New RegPrecise(App:=directory & "/" & RegPrecise.App)
     End Function

''' <summary>
''' ```bash
''' /Build.Operons /bbh &lt;bbh.csv&gt; /PTT &lt;genome.PTT&gt; /TF-bbh &lt;bbh.csv&gt; [/tfHit_hash /out &lt;out.csv&gt; /regprecise &lt;regprecise.Xml&gt;]
''' ```
''' If the /regprecise parameter is not presented, then you should install the regprecise in the GCModeller database repostiory first.
''' </summary>
'''
''' <param name="bbh"> The bbh result between the annotated genome And RegPrecise database. 
'''                    This result was used for generates the operons, and query should be the genes in 
'''                    the RegPrecise database and the hits is the genes in your annotated genome.
''' </param>
Public Function OperonBuilder(bbh As String, 
                                 PTT As String, 
                                 TF_bbh As String, 
                                 Optional out As String = "", 
                                 Optional regprecise As String = "", 
                                 Optional tfhit_hash As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Build.Operons")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    Call CLI.Append("/TF-bbh " & """" & TF_bbh & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not regprecise.StringEmpty Then
            Call CLI.Append("/regprecise " & """" & regprecise & """ ")
    End If
    If tfhit_hash Then
        Call CLI.Append("/tfhit_hash ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Build.Regulons.Batch /bbh &lt;bbh.DIR&gt; /PTT &lt;PTT.DIR&gt; /tf-bbh &lt;tf-bbh.DIR&gt; /regprecise &lt;regprecise.Xml&gt; [/num_threads &lt;-1&gt; /hits_hash /out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function RegulonBatchBuilder(bbh As String, 
                                       PTT As String, 
                                       tf_bbh As String, 
                                       regprecise As String, 
                                       Optional num_threads As String = "", 
                                       Optional out As String = "", 
                                       Optional hits_hash As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Build.Regulons.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    Call CLI.Append("/tf-bbh " & """" & tf_bbh & """ ")
    Call CLI.Append("/regprecise " & """" & regprecise & """ ")
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If hits_hash Then
        Call CLI.Append("/hits_hash ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /CORN /in &lt;regulons.DIR&gt; /motif-sites &lt;motiflogs.csv.DIR&gt; /sites &lt;motiflogs.csv&gt; /ref &lt;regulons.Csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' Join two vertices by edge if the correspondent operons:
''' i) are orthologous;
''' ii) have cantiodate transcription factor binding sites.
''' Collect all linked components. Two operons from two different genomes are called orthologous if they share at least one orthologous gene.
''' </summary>
'''

Public Function CORN([in] As String, 
                        motif_sites As String, 
                        sites As String, 
                        ref As String, 
                        Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/CORN")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/motif-sites " & """" & motif_sites & """ ")
    Call CLI.Append("/sites " & """" & sites & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /CORN.Batch /sites &lt;motiflogs.gff.sites.Csv.DIR&gt; /regulons &lt;regprecise.regulons.csv.DIR&gt; [/name &lt;name&gt; /out &lt;outDIR&gt; /num_threads &lt;-1&gt; /null-regprecise]
''' ```
''' </summary>
'''
''' <param name="name"> 
''' </param>
''' <param name="sites">
''' </param>
''' <param name="regulons">
''' </param>
Public Function CORNBatch(sites As String, 
                             regulons As String, 
                             Optional name As String = "", 
                             Optional out As String = "", 
                             Optional num_threads As String = "", 
                             Optional null_regprecise As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/CORN.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/sites " & """" & sites & """ ")
    Call CLI.Append("/regulons " & """" & regulons & """ ")
    If Not name.StringEmpty Then
            Call CLI.Append("/name " & """" & name & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
    If null_regprecise Then
        Call CLI.Append("/null-regprecise ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /CORN.thread /hit &lt;regulons.Csv&gt; /hit-sites &lt;motiflogs.csv&gt; /sites &lt;query.motiflogs.csv&gt; /ref &lt;query.regulons.Csv&gt; [/null-regprecise /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="null_regprecise"> Does the motif log data have the RegPrecise database value? If this parameter is presented that which it means the site data have no RegPrecise data.
''' </param>
''' <param name="hit">
''' </param>
''' <param name="hit_sites">
''' </param>
''' <param name="sites">
''' </param>
''' <param name="ref">
''' </param>
''' <param name="out">
''' </param>
Public Function CORNSingleThread(hit As String, 
                                    hit_sites As String, 
                                    sites As String, 
                                    ref As String, 
                                    Optional out As String = "", 
                                    Optional null_regprecise As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/CORN.thread")
    Call CLI.Append(" ")
    Call CLI.Append("/hit " & """" & hit & """ ")
    Call CLI.Append("/hit-sites " & """" & hit_sites & """ ")
    Call CLI.Append("/sites " & """" & sites & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If null_regprecise Then
        Call CLI.Append("/null-regprecise ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DOOR.Merge /in &lt;operon.csv&gt; /DOOR &lt;genome.opr&gt; [/out &lt;out.opr&gt;]
''' ```
''' </summary>
'''

Public Function MergeDOOR([in] As String, DOOR As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/DOOR.Merge")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/DOOR " & """" & DOOR & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Download.Motifs /imports &lt;RegPrecise.DIR&gt; [/export &lt;EXPORT_DIR&gt;]
''' ```
''' </summary>
'''

Public Function DownloadMotifSites([imports] As String, Optional export As String = "") As Integer
    Dim CLI As New StringBuilder("/Download.Motifs")
    Call CLI.Append(" ")
    Call CLI.Append("/imports " & """" & [imports] & """ ")
    If Not export.StringEmpty Then
            Call CLI.Append("/export " & """" & export & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Download.Regprecise [/work ./ /save &lt;save.Xml&gt;]
''' ```
''' Download Regprecise database from Web API
''' </summary>
'''
''' <param name="work"> The temporary directory for save the xml data. Is a cache directory path, Value is current directory by default.
''' </param>
''' <param name="save"> The repository saved xml file path.
''' </param>
Public Function DownloadRegprecise2(Optional work As String = "", Optional save As String = "") As Integer
    Dim CLI As New StringBuilder("/Download.Regprecise")
    Call CLI.Append(" ")
    If Not work.StringEmpty Then
            Call CLI.Append("/work " & """" & work & """ ")
    End If
    If Not save.StringEmpty Then
            Call CLI.Append("/save " & """" & save & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.Regprecise.motifs /in &lt;dir=genome_regprecise.xml&gt; [/out &lt;motifs.fasta&gt;]
''' ```
''' Export Regprecise motif sites as a single fasta sequence file.
''' </summary>
'''

Public Function ExportRegpreciseMotifSites([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.Regprecise.motifs")
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
''' /Export.Regulators /imports &lt;regprecise.downloads.DIR&gt; /Fasta &lt;regprecise.fasta&gt; [/locus-out /out &lt;out.fasta&gt;]
''' ```
''' Exports all of the fasta sequence of the TF regulator from the download RegPrecsie FASTA database.
''' </summary>
'''
''' <param name="locus_out"> Does the program saves a copy of the TF locus_tag list at the mean time of the TF fasta sequence export.
''' </param>
Public Function ExportRegulators([imports] As String, Fasta As String, Optional out As String = "", Optional locus_out As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.Regulators")
    Call CLI.Append(" ")
    Call CLI.Append("/imports " & """" & [imports] & """ ")
    Call CLI.Append("/Fasta " & """" & Fasta & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If locus_out Then
        Call CLI.Append("/locus-out ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Family.Hits /bbh &lt;bbh.csv&gt; [/regprecise &lt;RegPrecise.Xml&gt; /pfamKey &lt;query.pfam-string&gt; /out &lt;out.DIR&gt;]
''' ```
''' </summary>
'''

Public Function FamilyHits(bbh As String, Optional regprecise As String = "", Optional pfamkey As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Family.Hits")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    If Not regprecise.StringEmpty Then
            Call CLI.Append("/regprecise " & """" & regprecise & """ ")
    End If
    If Not pfamkey.StringEmpty Then
            Call CLI.Append("/pfamkey " & """" & pfamkey & """ ")
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
''' /Fasta.Downloads /source &lt;sourceDIR&gt; [/out &lt;outDIR&gt;]
''' ```
''' Download protein fasta sequence from KEGG database.
''' </summary>
'''

Public Function DownloadFasta(source As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Fasta.Downloads")
    Call CLI.Append(" ")
    Call CLI.Append("/source " & """" & source & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Fetches /ncbi &lt;all_gbk.DIR&gt; /imports &lt;inDIR&gt; /out &lt;outDIR&gt;
''' ```
''' </summary>
'''

Public Function Fetch(ncbi As String, [imports] As String, out As String) As Integer
    Dim CLI As New StringBuilder("/Fetches")
    Call CLI.Append(" ")
    Call CLI.Append("/ncbi " & """" & ncbi & """ ")
    Call CLI.Append("/imports " & """" & [imports] & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Fetches.Thread /gbk &lt;gbkDIR&gt; /query &lt;query.txt&gt; /out &lt;outDIR&gt;
''' ```
''' </summary>
'''

Public Function FetchThread(gbk As String, query As String, out As String) As Integer
    Dim CLI As New StringBuilder("/Fetches.Thread")
    Call CLI.Append(" ")
    Call CLI.Append("/gbk " & """" & gbk & """ ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Gets.Sites.Genes /in &lt;tf.bbh.csv&gt; /sites &lt;motiflogs.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function GetSites([in] As String, sites As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Gets.Sites.Genes")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
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
''' /heap.supports /in &lt;inDIR&gt; [/out &lt;out.Csv&gt; /T /l]
''' ```
''' </summary>
'''

Public Function Supports([in] As String, Optional out As String = "", Optional t As Boolean = False, Optional l As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/heap.supports")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If t Then
        Call CLI.Append("/t ")
    End If
    If l Then
        Call CLI.Append("/l ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /install.motifs /imports &lt;motifs.DIR&gt;
''' ```
''' </summary>
'''

Public Function InstallRegPreciseMotifs([imports] As String) As Integer
    Dim CLI As New StringBuilder("/install.motifs")
    Call CLI.Append(" ")
    Call CLI.Append("/imports " & """" & [imports] & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Maps.Effector /imports &lt;RegPrecise.DIR&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function Effectors([imports] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Maps.Effector")
    Call CLI.Append(" ")
    Call CLI.Append("/imports " & """" & [imports] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Merge.CORN /in &lt;inDIR&gt; [/out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function MergeCORN([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Merge.CORN")
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
''' /Merge.RegPrecise.Fasta [/in &lt;inDIR&gt; /out outDIR /offline]
''' ```
''' </summary>
'''

Public Function MergeDownload(Optional [in] As String = "", Optional out As String = "", Optional offline As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Merge.RegPrecise.Fasta")
    Call CLI.Append(" ")
    If Not [in].StringEmpty Then
            Call CLI.Append("/in " & """" & [in] & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If offline Then
        Call CLI.Append("/offline ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Prot_Motifs.EXPORT.pfamString /in &lt;motifs.json&gt; /PTT &lt;genome.ptt&gt; [/out &lt;pfam-string.csv&gt;]
''' ```
''' </summary>
'''

Public Function ProteinMotifsEXPORT([in] As String, PTT As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Prot_Motifs.EXPORT.pfamString")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Prot_Motifs.PfamString /in &lt;RegPrecise.Download_DIR&gt; [/fasta &lt;RegPrecise.fasta&gt; /out &lt;pfam-string.csv&gt;]
''' ```
''' </summary>
'''

Public Function ProtMotifToPfamString([in] As String, Optional fasta As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Prot_Motifs.PfamString")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not fasta.StringEmpty Then
            Call CLI.Append("/fasta " & """" & fasta & """ ")
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
''' /ProtMotifs.Downloads /source &lt;source.DIR&gt; [/kegg.Tools &lt;./kegg.exe&gt;]
''' ```
''' Download protein domain motifs structures from KEGG ssdb.
''' </summary>
'''

Public Function DownloadProteinMotifs(source As String, Optional kegg_tools As String = "") As Integer
    Dim CLI As New StringBuilder("/ProtMotifs.Downloads")
    Call CLI.Append(" ")
    Call CLI.Append("/source " & """" & source & """ ")
    If Not kegg_tools.StringEmpty Then
            Call CLI.Append("/kegg.tools " & """" & kegg_tools & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /regulators.bbh /bbh &lt;bbh.index.Csv&gt; /regprecise &lt;repository.directory&gt; [/sbh /description &lt;KEGG_genomes.fasta&gt; /allow.multiple /out &lt;save.csv&gt;]
''' ```
''' Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.
''' </summary>
'''
''' <param name="allow_multiple"> Allow the regulator assign multiple family name? By default is not allow, which means one protein just have one TF family name.
''' </param>
Public Function RegulatorsBBh(bbh As String, 
                                 regprecise As String, 
                                 Optional description As String = "", 
                                 Optional out As String = "", 
                                 Optional sbh As Boolean = False, 
                                 Optional allow_multiple As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/regulators.bbh")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/regprecise " & """" & regprecise & """ ")
    If Not description.StringEmpty Then
            Call CLI.Append("/description " & """" & description & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If sbh Then
        Call CLI.Append("/sbh ")
    End If
    If allow_multiple Then
        Call CLI.Append("/allow.multiple ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Repository.Fetch /imports &lt;RegPrecise.Xml&gt; /genbank &lt;NCBI_Genbank_DIR&gt; [/full /out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function FetchRepostiory([imports] As String, genbank As String, Optional out As String = "", Optional full As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Repository.Fetch")
    Call CLI.Append(" ")
    Call CLI.Append("/imports " & """" & [imports] & """ ")
    Call CLI.Append("/genbank " & """" & genbank & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If full Then
        Call CLI.Append("/full ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Rfam.Regulates /in &lt;RegPrecise.regulons.csv&gt; /rfam &lt;rfam_search.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function RfamRegulates([in] As String, rfam As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Rfam.Regulates")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/rfam " & """" & rfam & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Select.TF.BBH /bbh &lt;bbh.csv&gt; /imports &lt;RegPrecise.downloads.DIR&gt; [/out &lt;out.bbh.csv&gt;]
''' ```
''' </summary>
'''

Public Function SelectTFBBH(bbh As String, [imports] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Select.TF.BBH")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/imports " & """" & [imports] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Select.TF.Pfam-String /pfam-string &lt;RegPrecise.pfam-string.csv&gt; /imports &lt;regprecise.downloads.DIR&gt; [/out &lt;TF.pfam-string.csv&gt;]
''' ```
''' </summary>
'''

Public Function SelectTFPfams(pfam_string As String, [imports] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Select.TF.Pfam-String")
    Call CLI.Append(" ")
    Call CLI.Append("/pfam-string " & """" & pfam_string & """ ")
    Call CLI.Append("/imports " & """" & [imports] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /siRNA.Maps /in &lt;siRNA.csv&gt; /hits &lt;blastn.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function siRNAMaps([in] As String, hits As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/siRNA.Maps")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/hits " & """" & hits & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' Regprecise.Compile [/src &lt;repository&gt;]
''' ```
''' The repository parameter is a directory path which is the regprecise database root directory in the GCModeller directory, if you didn&apos;t know how to set this value, please leave it blank.
''' </summary>
'''

Public Function CompileRegprecise(Optional src As String = "") As Integer
    Dim CLI As New StringBuilder("Regprecise.Compile")
    Call CLI.Append(" ")
    If Not src.StringEmpty Then
            Call CLI.Append("/src " & """" & src & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' wGet.Regprecise [/repository-export &lt;dir.export, default: ./&gt; /updates]
''' ```
''' Download Regprecise database from REST API
''' </summary>
'''

Public Function DownloadRegprecise(Optional repository_export As String = "", Optional updates As Boolean = False) As Integer
    Dim CLI As New StringBuilder("wGet.Regprecise")
    Call CLI.Append(" ")
    If Not repository_export.StringEmpty Then
            Call CLI.Append("/repository-export " & """" & repository_export & """ ")
    End If
    If updates Then
        Call CLI.Append("/updates ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
