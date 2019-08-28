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
'  // VERSION:   1.0.0.0
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
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
'  /blastn:                                 Blastn analysis of your DNA sequence on KEGG server for
'                                           the functional analysis.
'  /Compile.Model:                          KEGG pathway model compiler
'  /Compound.Map.Render:                    Render draw of the KEGG pathway map by using a given KEGG
'                                           compound id list.
'  /Cut_sequence.upstream:                  
'  /Download.Fasta:                         Download fasta sequence from KEGG database web api.
'  /Download.human.genes:                   
'  /Download.Mapped.Sequence:               
'  /Download.Module.Maps:                   Download the KEGG reference modules map data.
'  /Download.Ortholog:                      Downloads the KEGG gene ortholog annotation data from the
'                                           web server.
'  /Dump.sp:                                /Dumping KEGG organism table in csv file format.
'  /Enrichment.Map.Render:                  Rendering kegg pathway map for enrichment analysis result
'                                           in local.
'  /Fasta.By.Sp:                            Picks the fasta sequence from the input sequence database
'                                           by a given species list.
'  /Get.prot_motif:                         
'  /Gets.prot_motif:                        
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
'                                             dbget API
'    /Download.Pathway.Maps:                  Fetch all of the pathway map information for a specific
'                                             kegg organism by using a specifc kegg sp code.
'    /Download.Pathway.Maps.Bacteria.All:     
'    /Download.Pathway.Maps.Batch:            
'    /Download.Reaction:                      Downloads the KEGG enzyme reaction reference model data.
'    /dump.kegg.maps:                         Dumping the KEGG maps database for human species.
'    /Pathways.Downloads.All:                 Download all of the KEGG reference pathway map data.
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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromEnvironment(directory As String) As KEGG_tools
            Return New KEGG_tools(App:=directory & "/" & KEGG_tools.App)
        End Function

        ''' <summary>
        ''' ```
        ''' /16s_rna [/out &lt;outDIR>]
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
        ''' ```
        ''' /blastn /query &lt;query.fasta> [/out &lt;outDIR>]
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
        ''' ```
        ''' /Build.Compounds.Repository /in &lt;directory> [/glycan.ignore /out &lt;repository.XML>]
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
        ''' ```
        ''' /Build.Ko.repository /DIR &lt;DIR> /repo &lt;root>
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
        ''' ```
        ''' /Build.Reactions.Repository /in &lt;directory> [/out &lt;repository.XML>]
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
        ''' ```
        ''' /Compile.Model /pathway &lt;pathwayDIR> /mods &lt;modulesDIR> /sp &lt;sp_code> [/out &lt;out.Xml>]
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
        ''' ```
        ''' /Compound.Map.Render /list &lt;csv/txt> [/repo &lt;pathwayMap.repository> /scale &lt;default=1> /color &lt;default=red> /out &lt;out.DIR>]
        ''' ```
        ''' Render draw of the KEGG pathway map by using a given KEGG compound id list.
        ''' </summary>
        '''
        Public Function CompoundMapRender(list As String, Optional repo As String = "", Optional scale As String = "1", Optional color As String = "red", Optional out As String = "") As Integer
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
        ''' ```
        ''' /Cut_sequence.upstream /in &lt;list.txt> /PTT &lt;genome.ptt> /org &lt;kegg_sp> [/len &lt;100bp> /overrides /out &lt;outDIR>]
        ''' ```
        ''' </summary>
        '''
        Public Function CutSequence_Upstream([in] As String, PTT As String, org As String, Optional len As String = "", Optional out As String = "", Optional [overrides] As Boolean = False) As Integer
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
        ''' ```
        ''' /Download.Compounds [/chebi &lt;accessions.tsv> /flat /updates /save &lt;DIR>]
        ''' ```
        ''' Downloads the KEGG compounds data from KEGG web server using dbget API
        ''' </summary>
        '''
        Public Function DownloadCompounds(Optional chebi As String = "", Optional save As String = "", Optional flat As Boolean = False, Optional updates As Boolean = False) As Integer
            Dim CLI As New StringBuilder("/Download.Compounds")
            Call CLI.Append(" ")
            If Not chebi.StringEmpty Then
                Call CLI.Append("/chebi " & """" & chebi & """ ")
            End If
            If Not save.StringEmpty Then
                Call CLI.Append("/save " & """" & save & """ ")
            End If
            If flat Then
                Call CLI.Append("/flat ")
            End If
            If updates Then
                Call CLI.Append("/updates ")
            End If
            Call CLI.Append("/@set --internal_pipeline=TRUE ")


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Download.Fasta /query &lt;querySource.txt> [/out &lt;outDIR> /source &lt;existsDIR>]
        ''' ```
        ''' Download fasta sequence from KEGG database web api.
        ''' </summary>
        '''
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
        ''' ```
        ''' /Download.human.genes /in &lt;geneID.list/DIR> [/batch /out &lt;save.DIR>]
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
        ''' ```
        ''' /Download.Mapped.Sequence /map &lt;map.list> [/nucl /out &lt;seq.fasta>]
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
        ''' ```
        ''' /Download.Module.Maps [/out &lt;EXPORT_DIR, default="./">]
        ''' ```
        ''' Download the KEGG reference modules map data.
        ''' </summary>
        '''
        Public Function DownloadReferenceModule(Optional out As String = "./") As Integer
            Dim CLI As New StringBuilder("/Download.Module.Maps")
            Call CLI.Append(" ")
            If Not out.StringEmpty Then
                Call CLI.Append("/out " & """" & out & """ ")
            End If
            Call CLI.Append("/@set --internal_pipeline=TRUE ")


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Download.Ortholog -i &lt;gene_list_file.txt/gbk> -export &lt;exportedDIR> [/gbk /sp &lt;KEGG.sp>]
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
        ''' ```
        ''' /Download.Pathway.Maps /sp &lt;kegg.sp_code> [/KGML /out &lt;EXPORT_DIR> /@set &lt;progress_bar=disabled>]
        ''' ```
        ''' Fetch all of the pathway map information for a specific kegg organism by using a specifc kegg sp code.
        ''' </summary>
        '''
        Public Function DownloadPathwayMaps(sp As String, Optional out As String = "", Optional _set As String = "", Optional kgml As Boolean = False) As Integer
            Dim CLI As New StringBuilder("/Download.Pathway.Maps")
            Call CLI.Append(" ")
            Call CLI.Append("/sp " & """" & sp & """ ")
            If Not out.StringEmpty Then
                Call CLI.Append("/out " & """" & out & """ ")
            End If
            If Not _set.StringEmpty Then
                Call CLI.Append("/@set " & """" & _set & """ ")
            End If
            If kgml Then
                Call CLI.Append("/kgml ")
            End If
            Call CLI.Append("/@set --internal_pipeline=TRUE ")


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Download.Pathway.Maps.Bacteria.All [/in &lt;brite.keg> /KGML /out &lt;out.directory>]
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
        ''' ```
        ''' /Download.Pathway.Maps.Batch /sp &lt;kegg.sp_code.list> [/KGML /out &lt;EXPORT_DIR>]
        ''' ```
        ''' </summary>
        '''
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
        ''' ```
        ''' /Download.Reaction [/try_all /compounds &lt;compounds.directory> /save &lt;DIR> /@set sleep=2000]
        ''' ```
        ''' Downloads the KEGG enzyme reaction reference model data.
        ''' </summary>
        '''
        Public Function DownloadKEGGReaction(Optional compounds As String = "", Optional save As String = "", Optional _set As String = "", Optional try_all As Boolean = False) As Integer
            Dim CLI As New StringBuilder("/Download.Reaction")
            Call CLI.Append(" ")
            If Not compounds.StringEmpty Then
                Call CLI.Append("/compounds " & """" & compounds & """ ")
            End If
            If Not save.StringEmpty Then
                Call CLI.Append("/save " & """" & save & """ ")
            End If
            If Not _set.StringEmpty Then
                Call CLI.Append("/@set " & """" & _set & """ ")
            End If
            If try_all Then
                Call CLI.Append("/try_all ")
            End If
            Call CLI.Append("/@set --internal_pipeline=TRUE ")


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /dump.kegg.maps [/htext &lt;htext.txt> /out &lt;save_dir>]
        ''' ```
        ''' Dumping the KEGG maps database for human species.
        ''' </summary>
        '''
        Public Function DumpKEGGMaps(Optional htext As String = "", Optional out As String = "") As Integer
            Dim CLI As New StringBuilder("/dump.kegg.maps")
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
        ''' ```
        ''' /Dump.sp [/res &lt;sp.html, default=weburl.html> /out &lt;out.csv>]
        ''' ```
        ''' /Dumping KEGG organism table in csv file format.
        ''' </summary>
        '''
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
        ''' ```
        ''' /Enrichment.Map.Render /url &lt;url> [/repo &lt;pathwayMap.repository> /out &lt;out.png>]
        ''' ```
        ''' Rendering kegg pathway map for enrichment analysis result in local.
        ''' </summary>
        '''
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
        ''' ```
        ''' /Fasta.By.Sp /in &lt;KEGG.fasta> /sp &lt;sp.list> [/out &lt;out.fasta>]
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
        ''' ```
        ''' /Get.prot_motif /query &lt;sp:locus> [/out out.json]
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
        ''' ```
        ''' /Gets.prot_motif /query &lt;query.txt/genome.PTT> [/PTT /sp &lt;kegg-sp> /out &lt;out.json> /update]
        ''' ```
        ''' </summary>
        '''
        Public Function GetsProteinMotifs(query As String, Optional sp As String = "", Optional out As String = "", Optional ptt As Boolean = False, Optional update As Boolean = False) As Integer
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
        ''' ```
        ''' /Imports.KO /pathways &lt;DIR> /KO &lt;DIR> [/save &lt;DIR>]
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
        ''' ```
        ''' /Imports.SSDB /in &lt;source.DIR> [/out &lt;ssdb.csv>]
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
        ''' ```
        ''' /ko.index.sub.match /index &lt;index.csv> /maps &lt;maps.csv> /key &lt;key> /map &lt;mapTo> [/out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function IndexSubMatch(index As String, maps As String, key As String, map As String, Optional out As String = "") As Integer
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
        ''' ```
        ''' /KO.list /kgml &lt;pathway.kgml> [/skip.empty /out &lt;list.csv>]
        ''' ```
        ''' Export a KO functional id list which all of the gene in this list is involved with the given pathway kgml data.
        ''' </summary>
        '''
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
        ''' ```
        ''' /Maps.Repository.Build /imports &lt;directory> [/out &lt;repository.XML>]
        ''' ```
        ''' Union the individual kegg reference pathway map file into one integral database file, usually used for fast loading.
        ''' </summary>
        '''
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
        ''' ```
        ''' /Organism.Table [/in &lt;br08601-htext.keg> /Bacteria /out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
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
        ''' ```
        ''' /Pathway.geneIDs /in &lt;pathway.XML> [/out &lt;out.list.txt>]
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
        ''' ```
        ''' /Pathway.Modules.Build /in &lt;directory> [/batch /out &lt;out.Xml>]
        ''' ```
        ''' </summary>
        '''
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
        ''' ```
        ''' /Pathways.Downloads.All [/out &lt;outDIR>]
        ''' ```
        ''' Download all of the KEGG reference pathway map data.
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
        ''' ```
        ''' /Query.KO /in &lt;blastnhits.csv> [/out &lt;out.csv> /evalue 1e-5 /batch]
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
        ''' ```
        ''' /show.organism /code &lt;kegg_sp> [/out &lt;out.json>]
        ''' ```
        ''' Save the summary information about the specific given kegg organism.
        ''' </summary>
        '''
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
        ''' ```
        ''' /Views.mod_stat /in &lt;KEGG_Modules/Pathways_DIR> /locus &lt;in.csv> [/locus_map Gene /pathway /out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function Stats([in] As String, locus As String, Optional locus_map As String = "", Optional out As String = "", Optional pathway As Boolean = False) As Integer
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
        ''' ```
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
        ''' ```
        ''' --Dump.Db /KEGG.Pathways &lt;DIR> /KEGG.Modules &lt;DIR> /KEGG.Reactions &lt;DIR> /sp &lt;sp.Code> /out &lt;out.Xml>
        ''' ```
        ''' </summary>
        '''
        Public Function DumpDb(KEGG_Pathways As String, KEGG_Modules As String, KEGG_Reactions As String, sp As String, out As String) As Integer
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
        ''' ```
        ''' -function.association.analysis -i &lt;matrix_csv>
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
        ''' ```
        ''' --Get.KO /in &lt;KASS-query.txt>
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
        ''' ```
        ''' --part.from /source &lt;source.fasta> /ref &lt;referenceFrom.fasta> [/out &lt;out.fasta> /brief]
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
        ''' ```
        ''' -query -keyword &lt;keyword> -o &lt;out_dir>
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
        ''' ```
        ''' -query.orthology -keyword &lt;gene_name> -o &lt;output_csv>
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
        ''' ```
        ''' -query.ref.map -id &lt;id> -o &lt;out_dir>
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
        ''' ```
        ''' -ref.map.download -o &lt;out_dir>
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
        ''' ```
        ''' -table.create -i &lt;input_dir> -o &lt;out_csv>
        ''' ```
        ''' </summary>
        '''
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
