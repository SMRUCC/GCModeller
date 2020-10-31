Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\KEGG_tools.exe

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
'  KEGG web services API tools.
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /16S_rRNA:                               Download 16S rRNA data from KEGG.
'  /all.enzymes:                            
'  /blastn:                                 Blastn analysis of your DNA sequence on KEGG server for
'                                           the functional analysis.
'  /Compile.Model:                          KEGG pathway model compiler
'  /Compound.Brite.Table:                   
'  /Compound.Map.Render:                    Render draw of the KEGG pathway map by using a given KEGG
'                                           compound id list.
'  /compound.names:                         
'  /Cut_sequence.upstream:                  
'  /Download.Fasta:                         Download fasta sequence from KEGG database web api.
'  /Download.human.genes:                   
'  /Download.Mapped.Sequence:               
'  /Download.Ortholog:                      Downloads the KEGG gene ortholog annotation data from the
'                                           web server.
'  /Download.reaction_class:                
'  /Dump.sp:                                Dump all of the KEGG organism and write table data in csv
'                                           file format.
'  /Enrichment.Map.Render:                  Rendering kegg pathway map for enrichment analysis result
'                                           in local.
'  /Fasta.By.Sp:                            Picks the fasta sequence from the input sequence database
'                                           by a given species list.
'  /Get.prot_motif:                         
'  /Gets.prot_motif:                        
'  /Glycan.compoundId:                      
'  /Imports.KO:                             Imports the KEGG reference pathway map and KEGG orthology
'                                           data as mysql dumps.
'  /Imports.SSDB:                           
'  /ko.index.sub.match:                     
'  /KO.list:                                Export a KO functional id list which all of the gene in
'                                           this list is involved with the given pathway kgml data.
'  /Organism.Table:                         
'  /Pathway.geneIDs:                        Get a list of gene ids from the given kegg pathway model
'                                           xml file.
'  /Query.KO:                               
'  /reaction.geneNames:                     
'  /show.organism:                          Save the summary information about the specific given kegg
'                                           organism.
'  /Views.mod_stat:                         
'  -Build.KO:                               Download data from KEGG database to local server.
'  --Dump.Db:                               
'  -function.association.analysis:          
'  --Get.KO:                                
'  --part.from:                             source and ref should be in KEGG annotation format.
'  -query:                                  Query the KEGG database for nucleotide sequence and protein
'                                           sequence by using a keywork.
'  -query.orthology:                        
'  -query.ref.map:                          
'  -Table.Create:                           
' 
' 
' API list that with functional grouping
' 
' 1. KEGG dbget API tools
' 
' 
'    /Download.Compounds:                     Downloads the KEGG compounds data from KEGG web server using
'                                             dbget API. Apply this downloaded KEGG compounds data used
'                                             for metabolism annotation in LC-MS data analysis.
'    /download.kegg.maps:                     Dumping the blank reference KEGG maps database.
'    /Download.Pathway.Maps:                  Fetch all of the pathway map information for a specific
'                                             kegg organism by using a specifc kegg sp code.
'    /Download.Pathway.Maps.Bacteria.All:     
'    /Download.Pathway.Maps.Batch:            
'    /Download.Reaction:                      Downloads the KEGG enzyme reaction reference model data.
'                                             Usually use these reference reaction data applied for metabolism
'                                             network analysis.
'    /Pathways.Downloads.All:                 Download all of the blank KEGG reference pathway map data.
'                                             Apply for render KEGG pathway enrichment result or other
'                                             biological system modelling work.
'    -ref.map.download:                       
' 
' 
' 2. KEGG models repository cli tools
' 
' 
'    /Build.Compounds.Repository:             
'    /Build.Ko.repository:                    
'    /Build.Reactions.Repository:             Package all of the single reaction model file into one data
'                                             file for make improvements on the data loading.
'    /Maps.Repository.Build:                  Union the individual kegg reference pathway map file into
'                                             one integral database file, usually used for fast loading.
'    /Pathway.Modules.Build:                  
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' KEGG web services API tools.
''' </summary>
'''
Public Class KEGG_tools : Inherits InteropService

    Public Const App$ = "KEGG_tools.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As KEGG_tools
          Return New KEGG_tools(App:=directory & "/" & KEGG_tools.App)
     End Function

''' <summary>
''' ```bash
''' /16s_rna [/out &lt;outDIR&gt;]
''' ```
''' Download 16S rRNA data from KEGG.
''' </summary>
'''

Public Function Download16SRNA(Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/16s_rna")
    Call CLI.Append(" ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /all.enzymes /code &lt;kegg_organism_code&gt; [/out &lt;enzymes.csv&gt;]
''' ```
''' </summary>
'''

Public Function GetAllEnzymes(code As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/all.enzymes")
    Call CLI.Append(" ")
    Call CLI.Append("/code " & """" & code & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /blastn /query &lt;query.fasta&gt; [/out &lt;outDIR&gt;]
''' ```
''' Blastn analysis of your DNA sequence on KEGG server for the functional analysis.
''' </summary>
'''

Public Function Blastn(query As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/blastn")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Build.Compounds.Repository /in &lt;directory&gt; [/glycan.ignore /out &lt;repository.XML&gt;]
''' ```
''' </summary>
'''

Public Function BuildCompoundsRepository([in] As String, Optional out As String = "", Optional glycan_ignore As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Build.Compounds.Repository")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If glycan_ignore Then
        Call CLI.Append("/glycan.ignore ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Build.Ko.repository /DIR &lt;DIR&gt; /repo &lt;root&gt;
''' ```
''' </summary>
'''

Public Function BuildKORepository(DIR As String, repo As String) As Integer
    Dim CLI As New StringBuilder("/Build.Ko.repository")
    Call CLI.Append(" ")
    Call CLI.Append("/DIR " & """" & DIR & """ ")
    Call CLI.Append("/repo " & """" & repo & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Build.Reactions.Repository /in &lt;directory&gt; [/out &lt;repository.XML&gt;]
''' ```
''' Package all of the single reaction model file into one data file for make improvements on the data loading.
''' </summary>
'''

Public Function BuildReactionsRepository([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Build.Reactions.Repository")
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
''' /Compile.Model /pathway &lt;pathwayDIR&gt; /mods &lt;modulesDIR&gt; /sp &lt;sp_code&gt; [/out &lt;out.Xml&gt;]
''' ```
''' KEGG pathway model compiler
''' </summary>
'''

Public Function Compile(pathway As String, mods As String, sp As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Compile.Model")
    Call CLI.Append(" ")
    Call CLI.Append("/pathway " & """" & pathway & """ ")
    Call CLI.Append("/mods " & """" & mods & """ ")
    Call CLI.Append("/sp " & """" & sp & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Compound.Brite.Table [/save &lt;table.csv&gt;]
''' ```
''' </summary>
'''

Public Function CompoundBriteTable(Optional save As String = "") As Integer
    Dim CLI As New StringBuilder("/Compound.Brite.Table")
    Call CLI.Append(" ")
    If Not save.StringEmpty Then
            Call CLI.Append("/save " & """" & save & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Compound.Map.Render /list &lt;csv/txt&gt; [/repo &lt;pathwayMap.repository&gt; /scale &lt;default=1&gt; /color &lt;default=red&gt; /out &lt;out.DIR&gt;]
''' ```
''' Render draw of the KEGG pathway map by using a given KEGG compound id list.
''' </summary>
'''
''' <param name="list"> A KEGG compound id list that provides the KEGG pathway map rendering source.
''' </param>
''' <param name="repo"> A directory path that contains the KEGG reference pathway map XML model. If this argument value is not presented in the commandline, then the default installed GCModeller KEGG compound repository will be used.
''' </param>
''' <param name="scale"> The circle radius size of the KEGG compound that rendering on the output pathway map image. By default is no scale.
''' </param>
''' <param name="color"> The node color that the KEGG compound rendering on the pathway map.
''' </param>
''' <param name="out"> A directory output path that will be using for contains the rendered pathway map image and the summary table file.
''' </param>
Public Function CompoundMapRender(list As String, 
                                     Optional repo As String = "", 
                                     Optional scale As String = "1", 
                                     Optional color As String = "red", 
                                     Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Compound.Map.Render")
    Call CLI.Append(" ")
    Call CLI.Append("/list " & """" & list & """ ")
    If Not repo.StringEmpty Then
            Call CLI.Append("/repo " & """" & repo & """ ")
    End If
    If Not scale.StringEmpty Then
            Call CLI.Append("/scale " & """" & scale & """ ")
    End If
    If Not color.StringEmpty Then
            Call CLI.Append("/color " & """" & color & """ ")
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
''' /compound.names /repo &lt;kegg_compounds.directory&gt; [/out &lt;names.json&gt;]
''' ```
''' </summary>
'''

Public Function CompoundNames(repo As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/compound.names")
    Call CLI.Append(" ")
    Call CLI.Append("/repo " & """" & repo & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Cut_sequence.upstream /in &lt;list.txt&gt; /PTT &lt;genome.ptt&gt; /org &lt;kegg_sp&gt; [/len &lt;100bp&gt; /overrides /out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function CutSequence_Upstream([in] As String, 
                                        PTT As String, 
                                        org As String, 
                                        Optional len As String = "", 
                                        Optional out As String = "", 
                                        Optional [overrides] As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Cut_sequence.upstream")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/PTT " & """" & PTT & """ ")
    Call CLI.Append("/org " & """" & org & """ ")
    If Not len.StringEmpty Then
            Call CLI.Append("/len " & """" & len & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If [overrides] Then
        Call CLI.Append("/overrides ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Download.Compounds [/list &lt;idlist.txt&gt; /chebi &lt;accessions.tsv&gt; /reactions &lt;kegg.reactions.repository&gt; /flat /skip.compoundbrite /updates /save &lt;DIR&gt;]
''' ```
''' Downloads the KEGG compounds data from KEGG web server using dbget API. Apply this downloaded KEGG compounds data used for metabolism annotation in LC-MS data analysis.
''' </summary>
'''
''' <param name="chebi"> Some compound metabolite in the KEGG database have no brite catalog info, then using the brite database for the compounds downloads will missing some compounds, 
'''               then you can using this option for downloads the complete compounds data in the KEGG database.
''' </param>
Public Function DownloadCompounds(Optional list As String = "", 
                                     Optional chebi As String = "", 
                                     Optional reactions As String = "", 
                                     Optional save As String = "", 
                                     Optional flat As Boolean = False, 
                                     Optional skip_compoundbrite As Boolean = False, 
                                     Optional updates As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Download.Compounds")
    Call CLI.Append(" ")
    If Not list.StringEmpty Then
            Call CLI.Append("/list " & """" & list & """ ")
    End If
    If Not chebi.StringEmpty Then
            Call CLI.Append("/chebi " & """" & chebi & """ ")
    End If
    If Not reactions.StringEmpty Then
            Call CLI.Append("/reactions " & """" & reactions & """ ")
    End If
    If Not save.StringEmpty Then
            Call CLI.Append("/save " & """" & save & """ ")
    End If
    If flat Then
        Call CLI.Append("/flat ")
    End If
    If skip_compoundbrite Then
        Call CLI.Append("/skip.compoundbrite ")
    End If
    If updates Then
        Call CLI.Append("/updates ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Download.Fasta /query &lt;querySource.txt&gt; [/out &lt;outDIR&gt; /source &lt;existsDIR&gt;]
''' ```
''' Download fasta sequence from KEGG database web api.
''' </summary>
'''
''' <param name="query"> This file should contains the locus_tag id list for download sequence.
''' </param>
Public Function DownloadSequence(query As String, Optional out As String = "", Optional source As String = "") As Integer
    Dim CLI As New StringBuilder("/Download.Fasta")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not source.StringEmpty Then
            Call CLI.Append("/source " & """" & source & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Download.human.genes /in &lt;geneID.list/DIR&gt; [/batch /out &lt;save.DIR&gt;]
''' ```
''' </summary>
'''

Public Function DownloadHumanGenes([in] As String, Optional out As String = "", Optional batch As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Download.human.genes")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
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
''' ```bash
''' /download.kegg.maps [/htext &lt;htext.txt&gt; /out &lt;save_dir&gt;]
''' ```
''' Dumping the blank reference KEGG maps database.
''' </summary>
'''
''' <param name="htext"> The KEGG category term provider
''' </param>
''' <param name="out"> A directory path that contains the download KEGG reference pathway map model data, this output can be using as the KEGG pathway map rendering repository source.
''' </param>
Public Function HumanKEGGMaps(Optional htext As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/download.kegg.maps")
    Call CLI.Append(" ")
    If Not htext.StringEmpty Then
            Call CLI.Append("/htext " & """" & htext & """ ")
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
''' /Download.Mapped.Sequence /map &lt;map.list&gt; [/nucl /out &lt;seq.fasta&gt;]
''' ```
''' </summary>
'''

Public Function DownloadMappedSequence(map As String, Optional out As String = "", Optional nucl As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Download.Mapped.Sequence")
    Call CLI.Append(" ")
    Call CLI.Append("/map " & """" & map & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If nucl Then
        Call CLI.Append("/nucl ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Download.Ortholog -i &lt;gene_list_file.txt/gbk&gt; -export &lt;exportedDIR&gt; [/gbk /sp &lt;KEGG.sp&gt;]
''' ```
''' Downloads the KEGG gene ortholog annotation data from the web server.
''' </summary>
'''

Public Function DownloadOrthologs(i As String, export As String, Optional sp As String = "", Optional gbk As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Download.Ortholog")
    Call CLI.Append(" ")
    Call CLI.Append("-i " & """" & i & """ ")
    Call CLI.Append("-export " & """" & export & """ ")
    If Not sp.StringEmpty Then
            Call CLI.Append("/sp " & """" & sp & """ ")
    End If
    If gbk Then
        Call CLI.Append("/gbk ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Download.Pathway.Maps /sp &lt;kegg.sp_code&gt; [/KGML /out &lt;EXPORT_DIR&gt; /debug /@set &lt;progress_bar=disabled&gt;]
''' ```
''' Fetch all of the pathway map information for a specific kegg organism by using a specifc kegg sp code.
''' </summary>
'''
''' <param name="sp"> The 3 characters kegg organism code, example as: &quot;xcb&quot; Is stands for organism &quot;Xanthomonas campestris pv. campestris 8004 (Beijing)&quot;
''' </param>
Public Function DownloadPathwayMaps(sp As String, 
                                       Optional out As String = "", 
                                       Optional _set As String = "", 
                                       Optional kgml As Boolean = False, 
                                       Optional debug As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Download.Pathway.Maps")
    Call CLI.Append(" ")
    Call CLI.Append("/sp " & """" & sp & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If kgml Then
        Call CLI.Append("/kgml ")
    End If
    If debug Then
        Call CLI.Append("/debug ")
    End If
    If Not _set.StringEmpty Then
     Call CLI.Append($"/@set """"--internal_pipeline=TRUE;'{_set}'"""" ")
Else
     Call CLI.Append("/@set --internal_pipeline=TRUE ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Download.Pathway.Maps.Bacteria.All [/in &lt;brite.keg&gt; /KGML /out &lt;out.directory&gt;]
''' ```
''' </summary>
'''

Public Function DownloadsBacteriasRefMaps(Optional [in] As String = "", Optional out As String = "", Optional kgml As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Download.Pathway.Maps.Bacteria.All")
    Call CLI.Append(" ")
    If Not [in].StringEmpty Then
            Call CLI.Append("/in " & """" & [in] & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If kgml Then
        Call CLI.Append("/kgml ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Download.Pathway.Maps.Batch /sp &lt;kegg.sp_code.list&gt; [/KGML /out &lt;EXPORT_DIR&gt;]
''' ```
''' </summary>
'''
''' <param name="sp"> A list of kegg species code. If this parameter Is a text file, 
'''               then each line should be start with the kegg organism code in three Or four letters, 
'''               else if this parameter is a csv table file, then it must in format of kegg organism data model.
''' </param>
Public Function DownloadPathwayMapsBatchTask(sp As String, Optional out As String = "", Optional kgml As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Download.Pathway.Maps.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/sp " & """" & sp & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If kgml Then
        Call CLI.Append("/kgml ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Download.Reaction [/try_all /compounds &lt;compounds.directory&gt; /save &lt;DIR&gt; /@set sleep=2000]
''' ```
''' Downloads the KEGG enzyme reaction reference model data. Usually use these reference reaction data applied for metabolism network analysis.
''' </summary>
'''
''' <param name="compounds"> If this argument Is present in the commandline, then it means only this collection of compounds related reactions will be download.
''' </param>
Public Function DownloadKEGGReaction(Optional compounds As String = "", Optional save As String = "", Optional _set As String = "", Optional try_all As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Download.Reaction")
    Call CLI.Append(" ")
    If Not compounds.StringEmpty Then
            Call CLI.Append("/compounds " & """" & compounds & """ ")
    End If
    If Not save.StringEmpty Then
            Call CLI.Append("/save " & """" & save & """ ")
    End If
    If try_all Then
        Call CLI.Append("/try_all ")
    End If
    If Not _set.StringEmpty Then
     Call CLI.Append($"/@set """"--internal_pipeline=TRUE;'{_set}'"""" ")
Else
     Call CLI.Append("/@set --internal_pipeline=TRUE ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Download.reaction_class [/save &lt;dir, default=./&gt;]
''' ```
''' </summary>
'''

Public Function DownloadReactionClass(Optional save As String = "./") As Integer
    Dim CLI As New StringBuilder("/Download.reaction_class")
    Call CLI.Append(" ")
    If Not save.StringEmpty Then
            Call CLI.Append("/save " & """" & save & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Dump.sp [/res &lt;sp.html, default=weburl.html&gt; /out &lt;out.csv&gt;]
''' ```
''' Dump all of the KEGG organism and write table data in csv file format.
''' </summary>
'''
''' <param name="res"> By default is fetch table resource from web url: &apos;http://www.kegg.jp/kegg/catalog/org_list.html&apos;.
''' </param>
Public Function DumpOrganisms(Optional res As String = "weburl.html", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Dump.sp")
    Call CLI.Append(" ")
    If Not res.StringEmpty Then
            Call CLI.Append("/res " & """" & res & """ ")
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
''' /Enrichment.Map.Render /url &lt;url&gt; [/repo &lt;pathwayMap.repository&gt; /out &lt;out.png&gt;]
''' ```
''' Rendering kegg pathway map for enrichment analysis result in local.
''' </summary>
'''
''' <param name="repo"> A directory path that contains the KEGG reference pathway map XML model. 
'''               If this argument value is not presented in the commandline, then the default installed 
'''               GCModeller KEGG compound repository will be used.
''' </param>
Public Function EnrichmentMapRender(url As String, Optional repo As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Enrichment.Map.Render")
    Call CLI.Append(" ")
    Call CLI.Append("/url " & """" & url & """ ")
    If Not repo.StringEmpty Then
            Call CLI.Append("/repo " & """" & repo & """ ")
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
''' /Fasta.By.Sp /in &lt;KEGG.fasta&gt; /sp &lt;sp.list&gt; [/out &lt;out.fasta&gt;]
''' ```
''' Picks the fasta sequence from the input sequence database by a given species list.
''' </summary>
'''

Public Function GetFastaBySp([in] As String, sp As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Fasta.By.Sp")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/sp " & """" & sp & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Get.prot_motif /query &lt;sp:locus&gt; [/out out.json]
''' ```
''' </summary>
'''

Public Function ProteinMotifs(query As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Get.prot_motif")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Gets.prot_motif /query &lt;query.txt/genome.PTT&gt; [/PTT /sp &lt;kegg-sp&gt; /out &lt;out.json&gt; /update]
''' ```
''' </summary>
'''

Public Function GetsProteinMotifs(query As String, 
                                     Optional sp As String = "", 
                                     Optional out As String = "", 
                                     Optional ptt As Boolean = False, 
                                     Optional update As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Gets.prot_motif")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not sp.StringEmpty Then
            Call CLI.Append("/sp " & """" & sp & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If ptt Then
        Call CLI.Append("/ptt ")
    End If
    If update Then
        Call CLI.Append("/update ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Glycan.compoundId /repo &lt;kegg_compounds.directory&gt; [/out &lt;id_mapping.json&gt;]
''' ```
''' </summary>
'''

Public Function Glycan2CompoundId(repo As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Glycan.compoundId")
    Call CLI.Append(" ")
    Call CLI.Append("/repo " & """" & repo & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Imports.KO /pathways &lt;DIR&gt; /KO &lt;DIR&gt; [/save &lt;DIR&gt;]
''' ```
''' Imports the KEGG reference pathway map and KEGG orthology data as mysql dumps.
''' </summary>
'''

Public Function ImportsKODatabase(pathways As String, KO As String, Optional save As String = "") As Integer
    Dim CLI As New StringBuilder("/Imports.KO")
    Call CLI.Append(" ")
    Call CLI.Append("/pathways " & """" & pathways & """ ")
    Call CLI.Append("/KO " & """" & KO & """ ")
    If Not save.StringEmpty Then
            Call CLI.Append("/save " & """" & save & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Imports.SSDB /in &lt;source.DIR&gt; [/out &lt;ssdb.csv&gt;]
''' ```
''' </summary>
'''

Public Function ImportsDb([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Imports.SSDB")
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
''' /ko.index.sub.match /index &lt;index.csv&gt; /maps &lt;maps.csv&gt; /key &lt;key&gt; /map &lt;mapTo&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function IndexSubMatch(index As String, 
                                 maps As String, 
                                 key As String, 
                                 map As String, 
                                 Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/ko.index.sub.match")
    Call CLI.Append(" ")
    Call CLI.Append("/index " & """" & index & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    Call CLI.Append("/key " & """" & key & """ ")
    Call CLI.Append("/map " & """" & map & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KO.list /kgml &lt;pathway.kgml&gt; [/skip.empty /out &lt;list.csv&gt;]
''' ```
''' Export a KO functional id list which all of the gene in this list is involved with the given pathway kgml data.
''' </summary>
'''
''' <param name="out"> If this argument is not presented in the commandline input, then result list will print on the console in tsv format.
''' </param>
Public Function TransmembraneKOlist(kgml As String, Optional out As String = "", Optional skip_empty As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/KO.list")
    Call CLI.Append(" ")
    Call CLI.Append("/kgml " & """" & kgml & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If skip_empty Then
        Call CLI.Append("/skip.empty ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Maps.Repository.Build /imports &lt;directory&gt; [/out &lt;repository.XML&gt;]
''' ```
''' Union the individual kegg reference pathway map file into one integral database file, usually used for fast loading.
''' </summary>
'''
''' <param name="[imports]"> A directory folder path which contains multiple KEGG reference pathway map xml files.
''' </param>
''' <param name="out"> An integral database xml file.
''' </param>
Public Function BuildPathwayMapsRepository([imports] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Maps.Repository.Build")
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
''' /Organism.Table [/in &lt;br08601-htext.keg&gt; /Bacteria /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="[in]"> If this kegg brite file is not presented in the cli arguments, the internal kegg resource will be used.
''' </param>
Public Function KEGGOrganismTable(Optional [in] As String = "", Optional out As String = "", Optional bacteria As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Organism.Table")
    Call CLI.Append(" ")
    If Not [in].StringEmpty Then
            Call CLI.Append("/in " & """" & [in] & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If bacteria Then
        Call CLI.Append("/bacteria ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Pathway.geneIDs /in &lt;pathway.XML&gt; [/out &lt;out.list.txt&gt;]
''' ```
''' Get a list of gene ids from the given kegg pathway model xml file.
''' </summary>
'''

Public Function PathwayGeneList([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Pathway.geneIDs")
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
''' /Pathway.Modules.Build /in &lt;directory&gt; [/batch /out &lt;out.Xml&gt;]
''' ```
''' </summary>
'''
''' <param name="[in]"> A directory that created by ``/Download.Pathway.Maps`` command.
''' </param>
Public Function CompileGenomePathwayModule([in] As String, Optional out As String = "", Optional batch As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Pathway.Modules.Build")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
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
''' ```bash
''' /Pathways.Downloads.All [/out &lt;outDIR&gt;]
''' ```
''' Download all of the blank KEGG reference pathway map data. Apply for render KEGG pathway enrichment result or other biological system modelling work.
''' </summary>
'''

Public Function DownloadsAllPathways(Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Pathways.Downloads.All")
    Call CLI.Append(" ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Query.KO /in &lt;blastnhits.csv&gt; [/out &lt;out.csv&gt; /evalue 1e-5 /batch]
''' ```
''' </summary>
'''

Public Function QueryKOAnno([in] As String, Optional out As String = "", Optional evalue As String = "", Optional batch As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Query.KO")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
    End If
    If batch Then
        Call CLI.Append("/batch ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /reaction.geneNames /repo &lt;kegg_reaction.directory&gt; [/out &lt;names.json&gt;]
''' ```
''' </summary>
'''

Public Function ReactionToGeneNames(repo As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/reaction.geneNames")
    Call CLI.Append(" ")
    Call CLI.Append("/repo " & """" & repo & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /show.organism /code &lt;kegg_sp&gt; [/out &lt;out.json&gt;]
''' ```
''' Save the summary information about the specific given kegg organism.
''' </summary>
'''
''' <param name="code"> The kegg organism brief code.
''' </param>
Public Function ShowOrganism(code As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/show.organism")
    Call CLI.Append(" ")
    Call CLI.Append("/code " & """" & code & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Views.mod_stat /in &lt;KEGG_Modules/Pathways_DIR&gt; /locus &lt;in.csv&gt; [/locus_map Gene /pathway /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function Stats([in] As String, 
                         locus As String, 
                         Optional locus_map As String = "", 
                         Optional out As String = "", 
                         Optional pathway As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Views.mod_stat")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/locus " & """" & locus & """ ")
    If Not locus_map.StringEmpty Then
            Call CLI.Append("/locus_map " & """" & locus_map & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If pathway Then
        Call CLI.Append("/pathway ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' -Build.KO [/fill-missing]
''' ```
''' Download data from KEGG database to local server.
''' </summary>
'''

Public Function BuildKEGGOrthology(Optional fill_missing As Boolean = False) As Integer
    Dim CLI As New StringBuilder("-Build.KO")
    Call CLI.Append(" ")
    If fill_missing Then
        Call CLI.Append("/fill-missing ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' --Dump.Db /KEGG.Pathways &lt;DIR&gt; /KEGG.Modules &lt;DIR&gt; /KEGG.Reactions &lt;DIR&gt; /sp &lt;sp.Code&gt; /out &lt;out.Xml&gt;
''' ```
''' </summary>
'''

Public Function DumpDb(KEGG_Pathways As String, 
                          KEGG_Modules As String, 
                          KEGG_Reactions As String, 
                          sp As String, 
                          out As String) As Integer
    Dim CLI As New StringBuilder("--Dump.Db")
    Call CLI.Append(" ")
    Call CLI.Append("/KEGG.Pathways " & """" & KEGG_Pathways & """ ")
    Call CLI.Append("/KEGG.Modules " & """" & KEGG_Modules & """ ")
    Call CLI.Append("/KEGG.Reactions " & """" & KEGG_Reactions & """ ")
    Call CLI.Append("/sp " & """" & sp & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' -function.association.analysis -i &lt;matrix_csv&gt;
''' ```
''' </summary>
'''

Public Function FunctionAnalysis(i As String) As Integer
    Dim CLI As New StringBuilder("-function.association.analysis")
    Call CLI.Append(" ")
    Call CLI.Append("-i " & """" & i & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' --Get.KO /in &lt;KASS-query.txt&gt;
''' ```
''' </summary>
'''

Public Function GetKOAnnotation([in] As String) As Integer
    Dim CLI As New StringBuilder("--Get.KO")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' --part.from /source &lt;source.fasta&gt; /ref &lt;referenceFrom.fasta&gt; [/out &lt;out.fasta&gt; /brief]
''' ```
''' source and ref should be in KEGG annotation format.
''' </summary>
'''

Public Function GetSource(source As String, ref As String, Optional out As String = "", Optional brief As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--part.from")
    Call CLI.Append(" ")
    Call CLI.Append("/source " & """" & source & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
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

''' <summary>
''' ```bash
''' -query -keyword &lt;keyword&gt; -o &lt;out_dir&gt;
''' ```
''' Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.
''' </summary>
'''

Public Function QueryGenes(keyword As String, o As String) As Integer
    Dim CLI As New StringBuilder("-query")
    Call CLI.Append(" ")
    Call CLI.Append("-keyword " & """" & keyword & """ ")
    Call CLI.Append("-o " & """" & o & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' -query.orthology -keyword &lt;gene_name&gt; -o &lt;output_csv&gt;
''' ```
''' </summary>
'''

Public Function QueryOrthology(keyword As String, o As String) As Integer
    Dim CLI As New StringBuilder("-query.orthology")
    Call CLI.Append(" ")
    Call CLI.Append("-keyword " & """" & keyword & """ ")
    Call CLI.Append("-o " & """" & o & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' -query.ref.map -id &lt;id&gt; -o &lt;out_dir&gt;
''' ```
''' </summary>
'''

Public Function DownloadReferenceMap(id As String, o As String) As Integer
    Dim CLI As New StringBuilder("-query.ref.map")
    Call CLI.Append(" ")
    Call CLI.Append("-id " & """" & id & """ ")
    Call CLI.Append("-o " & """" & o & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' -ref.map.download -o &lt;out_dir&gt;
''' ```
''' </summary>
'''

Public Function DownloadReferenceMapDatabase(o As String) As Integer
    Dim CLI As New StringBuilder("-ref.map.download")
    Call CLI.Append(" ")
    Call CLI.Append("-o " & """" & o & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' -table.create -i &lt;input_dir&gt; -o &lt;out_csv&gt;
''' ```
''' </summary>
'''
''' <param name="i"> This parameter specific the source directory input of the download data.
''' </param>
Public Function CreateTABLE(i As String, o As String) As Integer
    Dim CLI As New StringBuilder("-table.create")
    Call CLI.Append(" ")
    Call CLI.Append("-i " & """" & i & """ ")
    Call CLI.Append("-o " & """" & o & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
