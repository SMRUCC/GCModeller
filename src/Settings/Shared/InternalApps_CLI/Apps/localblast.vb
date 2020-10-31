Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\localblast.exe

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
'  Wrapper tools for the ncbi blast+ program and the blast output data analysis program.
'  For running a large scale parallel alignment task, using ``/venn.BlastAll`` command for ``blastp``
'  and ``/blastn.Query.All`` command for ``blastn``.
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /align.union:                      
'  /Bash.Venn:                        
'  /bbh.topbest:                      
'  /COG.myva:                         COG myva annotation using blastp raw output or exports sbh/bbh
'                                     table result.
'  /COG2014.result:                   
'  /Copy.Fasta:                       Copy target type files from different sub directory into a directory.
'  /hits.ID.list:                     
'  /Identities.Matrix:                
'  /MAT.evalue:                       
'  /SBH.tophits:                      Filtering the sbh result with top SBH Score
'  /to.kobas:                         
'  /UniProt.GO.faa:                   Export all of the protein sequence from the Uniprot database which
'                                     have GO term id been assigned.
'  /UniProt.KO.assign:                Assign KO number to query from Uniprot reference sequence database
'                                     alignment result.
'  /Whog.XML:                         Converts the whog text file into a XML data file.
'  --bbh.export:                      Batch export bbh result data from a directory.
'  --blast.self:                      Query fasta query against itself for paralogs.
'  --Export.Fasta:                    
'  --Export.Overviews:                
'  --Export.SBH:                      
'  --Xml2Excel:                       
'  --Xml2Excel.Batch:                 
' 
' 
' API list that with functional grouping
' 
' 1. Blastn alignment tools
' 
' 
'    /Blastn.Maps.Taxid:                
'    /blastn.Query:                     Using target fasta sequence query against all of the fasta sequence
'                                       in target direcotry. This function is single thread.
'    /blastn.Query.All:                 Using the fasta sequence in a directory query against all of the
'                                       sequence in another directory.
'    /BlastnMaps.Match.Taxid:           
'    /BlastnMaps.Select:                
'    /BlastnMaps.Select.Top:            
'    /BlastnMaps.Summery:               
'    /Chromosomes.Export:               
'    /Export.Blastn:                    
'    /Export.blastnMaps:                
'    /Export.blastnMaps.Batch:          Multiple processor task.
'    /Export.blastnMaps.littles:        
'    /Export.blastnMaps.Write:          Exports large amount of blastn output files and write all data
'                                       into a specific csv file.
' 
' 
' 2. Blastp BBH tools
' 
' 
'    /bbh.EXPORT:                       Export bbh mapping result from the blastp raw output.
'    /BBH.Merge:                        
'    /Blastp.BBH.Query:                 Using query fasta invoke blastp against the fasta files in a directory.
'                                       
' 
'                                       * This command tools required of NCBI blast+ suite,
'                                       you must config the blast bin path by using ``settings.exe`` before
'                                       running this command.
'    /Export.Locus:                     
'    /Fasta.Filters:                    Filter the fasta sequence subset from a larger fasta database
'                                       by using the regexp for match on the fasta title.
'    /locus.Selects:                    
'    /SBH.BBH.Batch:                    
'    /SBH.Export.Large:                 Using this command for export the sbh result of your blastp raw
'                                       data.
'    /SBH.Trim:                         
'    /sbh2bbh:                          Export bbh result from the sbh pairs.
'    /Select.Meta:                      
'    /venn.BBH:                         2. Build venn table And bbh data from the blastp result out Or
'                                       sbh data cache.
'    /venn.BlastAll:                    Completely paired combos blastp bbh operations for the venn diagram
'                                       Or network builder.
'    /venn.cache:                       1. [SBH_Batch] Creates the sbh cache data for the downstream bbh
'                                       analysis.
'                                       And this batch function is suitable with any scale of the
'                                       blastp sbh data output.
'    /venn.sbh.thread:                  
' 
' 
' 3. COG annotation tools
' 
' 
'    /COG.Statics:                      Statics the COG profiling in your analysised genome.
'    /EXPORT.COGs.from.DOOR:            
'    /install.cog2003-2014:             Config the ``prot2003-2014.fasta`` database for GCModeller localblast
'                                       tools. This database will be using for the COG annotation.
'                                       This command required of the blast+ install first.
'    /query.cog2003-2014:               Protein COG annotation by using NCBI cog2003-2014.fasta database.
' 
' 
' 4. NCBI genbank tools
' 
' 
'    /add.locus_tag:                    Add locus_tag qualifier into the feature slot.
'    /add.names:                        
'    /Copy.PTT:                         
'    /Copys:                            
'    /Export.BlastX:                    Export the blastx alignment result into a csv table.
'    /Export.gb:                        Export the *.fna, *.faa, *.ptt file from the gbk file.
'    /Export.gb.genes:                  
'    /Export.gpff:                      
'    /Export.gpffs:                     
'    /Export.Protein:                   Export all of the protein sequence from the genbank database file.
'    /Merge.faa:                        
'    /Print:                            
' 
' 
' 5. NCBI taxonomy tools
' 
' 
'    /Reads.OTU.Taxonomy:               If the blastnmapping data have the duplicated OTU tags, then this
'                                       function will makes a copy of the duplicated OTU tag data. top-best
'                                       data will not.
'    /ref.acc.list:                     
'    /ref.gi.list:                      
' 
' 
' 6. NCBI Web Blast Tools
' 
' 
'    /AlignmentTable.TopBest:           Export the top best hit result from the input web alignment table
'                                       output.
'    /Export.AlignmentTable:            Export the web alignment result file as csv table.
'    /Export.AlignmentTable.giList:     
'    /Taxonomy.efetch:                  Fetch the taxonomy information of the fasta sequence from NCBI
'                                       web server.
'    /Taxonomy.efetch.Merge:            
' 
' 
' 7. UniProt tools
' 
' 
'    /protein.EXPORT:                   Export the protein sequence And save as fasta format from the
'                                       uniprot database dump XML.
'    /UniProt.bbh.mappings:             
'    /UniProt.KO.faa:                   Export all of the protein sequence from the Uniprot database which
'                                       have KO number been assigned.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Wrapper tools for the ncbi blast+ program and the blast output data analysis program.
''' For running a large scale parallel alignment task, using ``/venn.BlastAll`` command for ``blastp`` and ``/blastn.Query.All`` command for ``blastn``.
''' </summary>
'''
Public Class localblast : Inherits InteropService

    Public Const App$ = "localblast.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As localblast
          Return New localblast(App:=directory & "/" & localblast.App)
     End Function

''' <summary>
''' ```bash
''' /add.locus_tag /gb &lt;gb.gbk&gt; /prefix &lt;prefix&gt; [/add.gene /out &lt;out.gb&gt;]
''' ```
''' Add locus_tag qualifier into the feature slot.
''' </summary>
'''
''' <param name="add_gene"> Add gene features?
''' </param>
Public Function AddLocusTag(gb As String, prefix As String, Optional out As String = "", Optional add_gene As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/add.locus_tag")
    Call CLI.Append(" ")
    Call CLI.Append("/gb " & """" & gb & """ ")
    Call CLI.Append("/prefix " & """" & prefix & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If add_gene Then
        Call CLI.Append("/add.gene ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /add.names /anno &lt;anno.csv&gt; /gb &lt;genbank.gbk&gt; [/out &lt;out.gbk&gt; /tag &lt;overrides_name&gt;]
''' ```
''' </summary>
'''

Public Function AddNames(anno As String, gb As String, Optional out As String = "", Optional tag As String = "") As Integer
    Dim CLI As New StringBuilder("/add.names")
    Call CLI.Append(" ")
    Call CLI.Append("/anno " & """" & anno & """ ")
    Call CLI.Append("/gb " & """" & gb & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not tag.StringEmpty Then
            Call CLI.Append("/tag " & """" & tag & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /align.union /query &lt;input.fasta&gt; /ref &lt;input.fasta&gt; /besthit &lt;besthit.csv&gt; [/out &lt;union_hits.csv&gt;]
''' ```
''' </summary>
'''

Public Function AlignUnion(query As String, ref As String, besthit As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/align.union")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    Call CLI.Append("/besthit " & """" & besthit & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /AlignmentTable.TopBest /in &lt;table.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' Export the top best hit result from the input web alignment table output.
''' </summary>
'''

Public Function AlignmentTableTopBest([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/AlignmentTable.TopBest")
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
''' /Bash.Venn /blast &lt;blastDIR&gt; /inDIR &lt;fasta.DIR&gt; /inRef &lt;inRefAs.DIR&gt; [/out &lt;outDIR&gt; /evalue &lt;evalue:10&gt;]
''' ```
''' </summary>
'''

Public Function BashShell(blast As String, 
                             inDIR As String, 
                             inRef As String, 
                             Optional out As String = "", 
                             Optional evalue As String = "") As Integer
    Dim CLI As New StringBuilder("/Bash.Venn")
    Call CLI.Append(" ")
    Call CLI.Append("/blast " & """" & blast & """ ")
    Call CLI.Append("/inDIR " & """" & inDIR & """ ")
    Call CLI.Append("/inRef " & """" & inRef & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /bbh.EXPORT /query &lt;query.blastp_out&gt; /subject &lt;subject.blast_out&gt; [/trim /out &lt;bbh.csv&gt; /evalue 1e-3 /coverage 0.85 /identities 0.3]
''' ```
''' Export bbh mapping result from the blastp raw output.
''' </summary>
'''

Public Function BBHExportFile(query As String, 
                                 subject As String, 
                                 Optional out As String = "", 
                                 Optional evalue As String = "", 
                                 Optional coverage As String = "", 
                                 Optional identities As String = "", 
                                 Optional trim As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/bbh.EXPORT")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
    End If
    If trim Then
        Call CLI.Append("/trim ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /BBH.Merge /in &lt;inDIR&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function MergeBBH([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/BBH.Merge")
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
''' /bbh.topbest /in &lt;bbh.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function BBHTopBest([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/bbh.topbest")
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
''' /Blastn.Maps.Taxid /in &lt;blastnMapping.csv&gt; /2taxid &lt;acc2taxid.tsv/gi2taxid.dmp&gt; [/gi2taxid /trim /tax &lt;NCBI_taxonomy:nodes/names&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="gi2taxid"> The 2taxid data source is comes from gi2taxid, by default is acc2taxid.
''' </param>
Public Function BlastnMapsTaxonomy([in] As String, 
                                      _2taxid As String, 
                                      Optional tax As String = "", 
                                      Optional out As String = "", 
                                      Optional gi2taxid As Boolean = False, 
                                      Optional trim As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Blastn.Maps.Taxid")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/2taxid " & """" & _2taxid & """ ")
    If Not tax.StringEmpty Then
            Call CLI.Append("/tax " & """" & tax & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If gi2taxid Then
        Call CLI.Append("/gi2taxid ")
    End If
    If trim Then
        Call CLI.Append("/trim ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /blastn.Query /query &lt;query.fna/faa&gt; /db &lt;db.DIR&gt; [/thread /evalue 1e-5 /word_size &lt;-1&gt; /out &lt;out.DIR&gt;]
''' ```
''' Using target fasta sequence query against all of the fasta sequence in target direcotry. This function is single thread.
''' </summary>
'''
''' <param name="thread"> Is this CLI api running in one of the processor in thread mode for a caller API ``/blastn.Query.All``
''' </param>
Public Function BlastnQuery(query As String, 
                               db As String, 
                               Optional evalue As String = "", 
                               Optional word_size As String = "", 
                               Optional out As String = "", 
                               Optional thread As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/blastn.Query")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/db " & """" & db & """ ")
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
    End If
    If Not word_size.StringEmpty Then
            Call CLI.Append("/word_size " & """" & word_size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If thread Then
        Call CLI.Append("/thread ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /blastn.Query.All /query &lt;query.fasta.DIR&gt; /db &lt;db.DIR&gt; [/skip-format /evalue 10 /word_size &lt;-1&gt; /out &lt;out.DIR&gt; /parallel /penalty &lt;penalty&gt; /reward &lt;reward&gt;]
''' ```
''' Using the fasta sequence in a directory query against all of the sequence in another directory.
''' </summary>
'''

Public Function BlastnQueryAll(query As String, 
                                  db As String, 
                                  Optional evalue As String = "", 
                                  Optional word_size As String = "", 
                                  Optional out As String = "", 
                                  Optional penalty As String = "", 
                                  Optional reward As String = "", 
                                  Optional skip_format As Boolean = False, 
                                  Optional parallel As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/blastn.Query.All")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/db " & """" & db & """ ")
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
    End If
    If Not word_size.StringEmpty Then
            Call CLI.Append("/word_size " & """" & word_size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not penalty.StringEmpty Then
            Call CLI.Append("/penalty " & """" & penalty & """ ")
    End If
    If Not reward.StringEmpty Then
            Call CLI.Append("/reward " & """" & reward & """ ")
    End If
    If skip_format Then
        Call CLI.Append("/skip-format ")
    End If
    If parallel Then
        Call CLI.Append("/parallel ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /BlastnMaps.Match.Taxid /in &lt;maps.csv&gt; /acc2taxid &lt;acc2taxid.DIR&gt; [/out &lt;out.tsv&gt;]
''' ```
''' </summary>
'''

Public Function MatchTaxid([in] As String, acc2taxid As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/BlastnMaps.Match.Taxid")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/acc2taxid " & """" & acc2taxid & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /BlastnMaps.Select /in &lt;reads.id.list.txt&gt; /data &lt;blastn.maps.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function SelectMaps([in] As String, data As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/BlastnMaps.Select")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/data " & """" & data & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /BlastnMaps.Select.Top /in &lt;maps.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function TopBlastnMapReads([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/BlastnMaps.Select.Top")
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
''' /BlastnMaps.Summery /in &lt;in.DIR&gt; [/split &quot;-&quot; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function BlastnMapsSummery([in] As String, Optional split As String = "-", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/BlastnMaps.Summery")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not split.StringEmpty Then
            Call CLI.Append("/split " & """" & split & """ ")
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
''' /Blastp.BBH.Query /query &lt;query.fasta&gt; /hit &lt;hit.source&gt; [/out &lt;outDIR&gt; /overrides /num_threads &lt;-1&gt;]
''' ```
''' Using query fasta invoke blastp against the fasta files in a directory.
''' * This command tools required of NCBI blast+ suite, you must config the blast bin path by using ``settings.exe`` before running this command.
''' </summary>
'''
''' <param name="query"> The protein query fasta file.
''' </param>
''' <param name="hit"> A directory contains the protein sequence fasta files which will be using for bbh search.
''' </param>
Public Function BlastpBBHQuery(query As String, 
                                  hit As String, 
                                  Optional out As String = "", 
                                  Optional num_threads As String = "", 
                                  Optional [overrides] As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Blastp.BBH.Query")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/hit " & """" & hit & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
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
''' /Chromosomes.Export /reads &lt;reads.fasta/DIR&gt; /maps &lt;blastnMappings.Csv/DIR&gt; [/out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function ChromosomesBlastnResult(reads As String, maps As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Chromosomes.Export")
    Call CLI.Append(" ")
    Call CLI.Append("/reads " & """" & reads & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /COG.myva /blastp &lt;blastp.myva.txt/sbh.csv&gt; /whog &lt;whog.XML&gt; [/top.best /grep &lt;donothing&gt; /simple /out &lt;out.csv/txt&gt;]
''' ```
''' COG myva annotation using blastp raw output or exports sbh/bbh table result.
''' </summary>
'''
''' <param name="simple"> This flag will change the output file format. 
'''                   If this parameter value is presented, then the tool will outoput a simple tsv file;
'''                   Otherwise output a csv file with complete COG assign result records.
''' </param>
Public Function COG_myva(blastp As String, 
                            whog As String, 
                            Optional grep As String = "", 
                            Optional out As String = "", 
                            Optional top_best As Boolean = False, 
                            Optional simple As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/COG.myva")
    Call CLI.Append(" ")
    Call CLI.Append("/blastp " & """" & blastp & """ ")
    Call CLI.Append("/whog " & """" & whog & """ ")
    If Not grep.StringEmpty Then
            Call CLI.Append("/grep " & """" & grep & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If top_best Then
        Call CLI.Append("/top.best ")
    End If
    If simple Then
        Call CLI.Append("/simple ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /COG.Statics /in &lt;myva_cogs.csv&gt; [/locus &lt;locus.txt/csv&gt; /locuMap &lt;Gene&gt; /out &lt;out.csv&gt;]
''' ```
''' Statics the COG profiling in your analysised genome.
''' </summary>
'''

Public Function COGStatics([in] As String, Optional locus As String = "", Optional locumap As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/COG.Statics")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not locus.StringEmpty Then
            Call CLI.Append("/locus " & """" & locus & """ ")
    End If
    If Not locumap.StringEmpty Then
            Call CLI.Append("/locumap " & """" & locumap & """ ")
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
''' /COG2014.result /sbh &lt;query-vs-COG2003-2014.fasta&gt; /cog &lt;cog2003-2014.csv&gt; [/cog.names &lt;cognames2003-2014.tab&gt; /out &lt;out.myva_cog.csv&gt;]
''' ```
''' </summary>
'''

Public Function COG2014_result(sbh As String, cog As String, Optional cog_names As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/COG2014.result")
    Call CLI.Append(" ")
    Call CLI.Append("/sbh " & """" & sbh & """ ")
    Call CLI.Append("/cog " & """" & cog & """ ")
    If Not cog_names.StringEmpty Then
            Call CLI.Append("/cog.names " & """" & cog_names & """ ")
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
''' /Copy.Fasta /imports &lt;DIR&gt; [/type &lt;faa,fna,ffn,fasta,...., default:=faa&gt; /out &lt;DIR&gt;]
''' ```
''' Copy target type files from different sub directory into a directory.
''' </summary>
'''

Public Function CopyFasta([imports] As String, Optional type As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Copy.Fasta")
    Call CLI.Append(" ")
    Call CLI.Append("/imports " & """" & [imports] & """ ")
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
''' ```bash
''' /Copy.PTT /in &lt;inDIR&gt; [/out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function CopyPTT([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Copy.PTT")
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
''' /Copys /imports &lt;DIR&gt; [/out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function Copys([imports] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Copys")
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
''' /Export.AlignmentTable /in &lt;alignment.txt&gt; [/split /header.split /out &lt;outDIR/file&gt;]
''' ```
''' Export the web alignment result file as csv table.
''' </summary>
'''

Public Function ExportWebAlignmentTable([in] As String, Optional out As String = "", Optional split As Boolean = False, Optional header_split As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.AlignmentTable")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If split Then
        Call CLI.Append("/split ")
    End If
    If header_split Then
        Call CLI.Append("/header.split ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.AlignmentTable.giList /in &lt;table.csv&gt; [/out &lt;gi.txt&gt;]
''' ```
''' </summary>
'''

Public Function ParseAlignmentTableGIlist([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.AlignmentTable.giList")
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
''' /Export.Blastn /in &lt;in.txt&gt; [/out &lt;out.csv&gt;]
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
''' /Export.blastnMaps /in &lt;blastn.txt&gt; [/best /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="best"> Only output the first hit result for each query as best?
''' </param>
Public Function ExportBlastnMaps([in] As String, Optional out As String = "", Optional best As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.blastnMaps")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If best Then
        Call CLI.Append("/best ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.blastnMaps.Batch /in &lt;blastn_out.DIR&gt; [/best /out &lt;out.DIR&gt; /num_threads &lt;-1&gt;]
''' ```
''' Multiple processor task.
''' </summary>
'''

Public Function ExportBlastnMapsBatch([in] As String, Optional out As String = "", Optional num_threads As String = "", Optional best As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.blastnMaps.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
    If best Then
        Call CLI.Append("/best ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.blastnMaps.littles /in &lt;blastn.txt.DIR&gt; [/out &lt;out.csv.DIR&gt;]
''' ```
''' </summary>
'''

Public Function ExportBlastnMapsSmall([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.blastnMaps.littles")
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
''' /Export.blastnMaps.Write /in &lt;blastn_out.DIR&gt; [/best /out &lt;write.csv&gt;]
''' ```
''' Exports large amount of blastn output files and write all data into a specific csv file.
''' </summary>
'''
''' <param name="best"> Only export the top best blastn alignment hit?
''' </param>
''' <param name="out"> Blastn alignment maps data.
''' </param>
''' <param name="[in]"> The directory path that contains the blastn output data.
''' </param>
Public Function ExportBlastnMapsBatchWrite([in] As String, Optional out As String = "", Optional best As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.blastnMaps.Write")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If best Then
        Call CLI.Append("/best ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.BlastX /in &lt;blastx.txt&gt; [/top /Uncharacterized.exclude /out &lt;out.csv&gt;]
''' ```
''' Export the blastx alignment result into a csv table.
''' </summary>
'''
''' <param name="top"> Only output the top first alignment result? Default is not.
''' </param>
''' <param name="[in]"> The text file content output from the blastx command in NCBI blast+ suite.
''' </param>
Public Function ExportBlastX([in] As String, Optional out As String = "", Optional top As Boolean = False, Optional uncharacterized_exclude As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.BlastX")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If top Then
        Call CLI.Append("/top ")
    End If
    If uncharacterized_exclude Then
        Call CLI.Append("/uncharacterized.exclude ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /EXPORT.COGs.from.DOOR /in &lt;DOOR.opr&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function ExportDOORCogs([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/EXPORT.COGs.from.DOOR")
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
''' /Export.gb /gb &lt;genbank.gb/DIR&gt; [/flat /out &lt;outDIR&gt; /simple /batch]
''' ```
''' Export the *.fna, *.faa, *.ptt file from the gbk file.
''' </summary>
'''
''' <param name="simple"> Fasta sequence short title, which is just only contains locus_tag
''' </param>
''' <param name="flat"> If the argument is presented in your commandline input, then all of the files 
'''               will be saved in one directory, otherwise will group by genome locus_tag in seperated folders.
''' </param>
Public Function ExportGenbank(gb As String, 
                                 Optional out As String = "", 
                                 Optional flat As Boolean = False, 
                                 Optional simple As Boolean = False, 
                                 Optional batch As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.gb")
    Call CLI.Append(" ")
    Call CLI.Append("/gb " & """" & gb & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If flat Then
        Call CLI.Append("/flat ")
    End If
    If simple Then
        Call CLI.Append("/simple ")
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
''' /Export.gb.genes /gb &lt;genbank.gb&gt; [/locus_tag /geneName /out &lt;out.fasta&gt;]
''' ```
''' </summary>
'''
''' <param name="geneName"> If this parameter is specific as True, then this function will try using geneName as the fasta sequence title, or using locus_tag value as default.
''' </param>
Public Function ExportGenesFasta(gb As String, Optional out As String = "", Optional locus_tag As Boolean = False, Optional genename As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.gb.genes")
    Call CLI.Append(" ")
    Call CLI.Append("/gb " & """" & gb & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If locus_tag Then
        Call CLI.Append("/locus_tag ")
    End If
    If genename Then
        Call CLI.Append("/genename ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.gpff /in &lt;genome.gpff&gt; /gff &lt;genome.gff&gt; [/out &lt;out.PTT&gt;]
''' ```
''' </summary>
'''

Public Function EXPORTgpff([in] As String, gff As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.gpff")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/gff " & """" & gff & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.gpffs [/in &lt;inDIR&gt;]
''' ```
''' </summary>
'''

Public Function EXPORTgpffs(Optional [in] As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.gpffs")
    Call CLI.Append(" ")
    If Not [in].StringEmpty Then
            Call CLI.Append("/in " & """" & [in] & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.Locus /in &lt;sbh/bbh_DIR&gt; [/hit /out &lt;out.txt&gt;]
''' ```
''' </summary>
'''

Public Function ExportLocus([in] As String, Optional out As String = "", Optional hit As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.Locus")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If hit Then
        Call CLI.Append("/hit ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Export.Protein /gb &lt;genome.gb&gt; [/out &lt;out.fasta&gt;]
''' ```
''' Export all of the protein sequence from the genbank database file.
''' </summary>
'''

Public Function ExportProt(gb As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.Protein")
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
''' ```bash
''' /Fasta.Filters /in &lt;nt.fasta&gt; /key &lt;regex/list.txt&gt; [/tokens /out &lt;out.fasta&gt; /p]
''' ```
''' Filter the fasta sequence subset from a larger fasta database by using the regexp for match on the fasta title.
''' </summary>
'''
''' <param name="key"> A regexp string term that will be using for title search or file path of a text file contains lines of regexp.
''' </param>
''' <param name="p"> Using the parallel edition?? If GCModeller running in a 32bit environment, do not use this option. This option only works in single key mode.
''' </param>
Public Function Filter([in] As String, 
                          key As String, 
                          Optional out As String = "", 
                          Optional tokens As Boolean = False, 
                          Optional p As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Fasta.Filters")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/key " & """" & key & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If tokens Then
        Call CLI.Append("/tokens ")
    End If
    If p Then
        Call CLI.Append("/p ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /hits.ID.list /in &lt;bbhindex.csv&gt; [/out &lt;out.txt&gt;]
''' ```
''' </summary>
'''

Public Function HitsIDList([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/hits.ID.list")
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
''' /Identities.Matrix /hit &lt;sbh/bbh.csv&gt; [/out &lt;out.csv&gt; /cut 0.65]
''' ```
''' </summary>
'''

Public Function IdentitiesMAT(hit As String, Optional out As String = "", Optional cut As String = "") As Integer
    Dim CLI As New StringBuilder("/Identities.Matrix")
    Call CLI.Append(" ")
    Call CLI.Append("/hit " & """" & hit & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not cut.StringEmpty Then
            Call CLI.Append("/cut " & """" & cut & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /install.cog2003-2014 /db &lt;prot2003-2014.fasta&gt;
''' ```
''' Config the ``prot2003-2014.fasta`` database for GCModeller localblast tools. This database will be using for the COG annotation.
''' This command required of the blast+ install first.
''' </summary>
'''
''' <param name="db"> The fasta database using for COG annotation, which can be download from NCBI ftp: 
'''               &gt; ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/prot2003-2014.fa.gz
''' </param>
Public Function InstallCOGDatabase(db As String) As Integer
    Dim CLI As New StringBuilder("/install.cog2003-2014")
    Call CLI.Append(" ")
    Call CLI.Append("/db " & """" & db & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /locus.Selects /locus &lt;locus.txt&gt; /bh &lt;bbhindex.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function LocusSelects(locus As String, bh As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/locus.Selects")
    Call CLI.Append(" ")
    Call CLI.Append("/locus " & """" & locus & """ ")
    Call CLI.Append("/bh " & """" & bh & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /MAT.evalue /in &lt;sbh.csv&gt; [/out &lt;mat.csv&gt; /flip]
''' ```
''' </summary>
'''

Public Function EvalueMatrix([in] As String, Optional out As String = "", Optional flip As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/MAT.evalue")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If flip Then
        Call CLI.Append("/flip ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Merge.faa /in &lt;DIR&gt; /out &lt;out.fasta&gt;
''' ```
''' </summary>
'''

Public Function MergeFaa([in] As String, out As String) As Integer
    Dim CLI As New StringBuilder("/Merge.faa")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Print /in &lt;inDIR&gt; [/ext &lt;ext&gt; /out &lt;out.Csv&gt;]
''' ```
''' </summary>
'''

Public Function Print([in] As String, Optional ext As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Print")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not ext.StringEmpty Then
            Call CLI.Append("/ext " & """" & ext & """ ")
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
''' /protein.EXPORT /in &lt;uniprot.xml&gt; [/sp &lt;name&gt; /exclude /out &lt;out.fasta&gt;]
''' ```
''' Export the protein sequence And save as fasta format from the uniprot database dump XML.
''' </summary>
'''
''' <param name="sp"> The organism scientific name.
''' </param>
''' <param name="uniprot"> The Uniprot protein database in XML file format.
''' </param>
''' <param name="exclude"> Exclude the specific organism by ``/sp`` scientific name instead of only include it?
''' </param>
''' <param name="out"> The saved file path for output protein sequence fasta file. The title format of this command output Is ``uniprot_id fullName``
''' </param>
Public Function proteinEXPORT([in] As String, Optional sp As String = "", Optional out As String = "", Optional exclude As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/protein.EXPORT")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not sp.StringEmpty Then
            Call CLI.Append("/sp " & """" & sp & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If exclude Then
        Call CLI.Append("/exclude ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /query.cog2003-2014 /query &lt;query.fasta&gt; [/evalue 1e-5 /coverage 0.65 /identities 0.85 /all /out &lt;out.DIR&gt; /db &lt;cog2003-2014.fasta&gt; /blast+ &lt;blast+/bin&gt;]
''' ```
''' Protein COG annotation by using NCBI cog2003-2014.fasta database.
''' </summary>
'''
''' <param name="db"> The file path to the database fasta file.
'''               If you have config the cog2003-2014 database previously, then this argument can be omitted.
''' </param>
''' <param name="blast_"> The directory to the NCBI blast+ suite ``bin`` directory. If you have config this path before, then this argument can be omitted.
''' </param>
''' <param name="all"> For export the bbh result, export all match or only the top best? default is only top best.
''' </param>
''' <param name="evalue"> blastp e-value cutoff.
''' </param>
''' <param name="out"> The output directory for the work files.
''' </param>
Public Function COG2003_2014(query As String, 
                                Optional evalue As String = "", 
                                Optional coverage As String = "", 
                                Optional identities As String = "", 
                                Optional out As String = "", 
                                Optional db As String = "", 
                                Optional blast_ As String = "", 
                                Optional all As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/query.cog2003-2014")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
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
    If Not db.StringEmpty Then
            Call CLI.Append("/db " & """" & db & """ ")
    End If
    If Not blast_.StringEmpty Then
            Call CLI.Append("/blast+ " & """" & blast_ & """ ")
    End If
    If all Then
        Call CLI.Append("/all ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Reads.OTU.Taxonomy /in &lt;blastnMaps.csv&gt; /OTU &lt;OTU_data.csv&gt; /tax &lt;taxonomy:nodes/names&gt; [/fill.empty /out &lt;out.csv&gt;]
''' ```
''' If the blastnmapping data have the duplicated OTU tags, then this function will makes a copy of the duplicated OTU tag data. top-best data will not.
''' </summary>
'''
''' <param name="[in]"> This input data should have a column named ``taxid`` for the taxonomy information.
''' </param>
''' <param name="fill_empty"> If this options is true, then this function will only fill the rows which have an empty ``Taxonomy`` field column.
''' </param>
''' <param name="OTU">
''' </param>
Public Function ReadsOTU_Taxonomy([in] As String, 
                                     OTU As String, 
                                     tax As String, 
                                     Optional out As String = "", 
                                     Optional fill_empty As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Reads.OTU.Taxonomy")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/OTU " & """" & OTU & """ ")
    Call CLI.Append("/tax " & """" & tax & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If fill_empty Then
        Call CLI.Append("/fill.empty ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /ref.acc.list /in &lt;blastnMaps.csv/DIR&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function AccessionList([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/ref.acc.list")
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
''' /ref.gi.list /in &lt;blastnMaps.csv/DIR&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function GiList([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/ref.gi.list")
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
''' /SBH.BBH.Batch /in &lt;sbh.DIR&gt; [/identities &lt;-1&gt; /coverage &lt;-1&gt; /all /out &lt;bbh.DIR&gt; /num_threads &lt;-1&gt;]
''' ```
''' </summary>
'''

Public Function SBH_BBH_Batch([in] As String, 
                                 Optional identities As String = "", 
                                 Optional coverage As String = "", 
                                 Optional out As String = "", 
                                 Optional num_threads As String = "", 
                                 Optional all As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/SBH.BBH.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
    If all Then
        Call CLI.Append("/all ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /SBH.Export.Large /in &lt;blastp_out.txt/directory&gt; [/top.best /trim-kegg /s.pattern &lt;default=-&gt; /q.pattern &lt;default=-&gt; /keeps_raw.queryName /identities 0.15 /coverage 0.5 /split /out &lt;sbh.csv&gt;]
''' ```
''' Using this command for export the sbh result of your blastp raw data.
''' </summary>
'''
''' <param name="trim_KEGG"> If the fasta sequence source is comes from the KEGG database, and you want to removes the kegg species brief code for the locus_tag, then enable this option.
''' </param>
''' <param name="out"> The sbh result output csv file location.
''' </param>
''' <param name="[in]"> The blastp raw result input file path.
''' </param>
Public Function ExportSBHLargeSize([in] As String, 
                                      Optional s_pattern As String = "-", 
                                      Optional q_pattern As String = "-", 
                                      Optional identities As String = "", 
                                      Optional coverage As String = "", 
                                      Optional out As String = "", 
                                      Optional top_best As Boolean = False, 
                                      Optional trim_kegg As Boolean = False, 
                                      Optional keeps_raw_queryname As Boolean = False, 
                                      Optional split As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/SBH.Export.Large")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not s_pattern.StringEmpty Then
            Call CLI.Append("/s.pattern " & """" & s_pattern & """ ")
    End If
    If Not q_pattern.StringEmpty Then
            Call CLI.Append("/q.pattern " & """" & q_pattern & """ ")
    End If
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If top_best Then
        Call CLI.Append("/top.best ")
    End If
    If trim_kegg Then
        Call CLI.Append("/trim-kegg ")
    End If
    If keeps_raw_queryname Then
        Call CLI.Append("/keeps_raw.queryname ")
    End If
    If split Then
        Call CLI.Append("/split ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /SBH.tophits /in &lt;sbh.csv&gt; [/uniprotKB /out &lt;out.csv&gt;]
''' ```
''' Filtering the sbh result with top SBH Score
''' </summary>
'''

Public Function SBH_topHits([in] As String, Optional out As String = "", Optional uniprotkb As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/SBH.tophits")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If uniprotkb Then
        Call CLI.Append("/uniprotkb ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /SBH.Trim /in &lt;sbh.csv&gt; /evalue &lt;evalue&gt; [/identities 0.15 /coverage 0.5 /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function SBHTrim([in] As String, 
                           evalue As String, 
                           Optional identities As String = "", 
                           Optional coverage As String = "", 
                           Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/SBH.Trim")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/evalue " & """" & evalue & """ ")
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
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
''' /sbh2bbh /qvs &lt;qvs.sbh.csv&gt; /svq &lt;svq.sbh.csv&gt; [/trim /query.pattern &lt;default=&quot;-&quot;&gt; /hit.pattern &lt;default=&quot;-&quot;&gt; /identities &lt;-1&gt; /coverage &lt;-1&gt; /all /out &lt;bbh.csv&gt;]
''' ```
''' Export bbh result from the sbh pairs.
''' </summary>
'''
''' <param name="identities"> Makes a further filtering on the bbh by using this option, default value is -1, so that this means no filter.
''' </param>
''' <param name="coverage"> Makes a further filtering on the bbh by using this option, default value is -1, so that this means no filter.
''' </param>
''' <param name="trim"> If this option was enabled, then the queryName and hitname will be trimed by using space and the first token was taken as the name ID.
''' </param>
Public Function BBHExport2(qvs As String, 
                              svq As String, 
                              Optional query_pattern As String = "-", 
                              Optional hit_pattern As String = "-", 
                              Optional identities As String = "", 
                              Optional coverage As String = "", 
                              Optional out As String = "", 
                              Optional trim As Boolean = False, 
                              Optional all As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/sbh2bbh")
    Call CLI.Append(" ")
    Call CLI.Append("/qvs " & """" & qvs & """ ")
    Call CLI.Append("/svq " & """" & svq & """ ")
    If Not query_pattern.StringEmpty Then
            Call CLI.Append("/query.pattern " & """" & query_pattern & """ ")
    End If
    If Not hit_pattern.StringEmpty Then
            Call CLI.Append("/hit.pattern " & """" & hit_pattern & """ ")
    End If
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If trim Then
        Call CLI.Append("/trim ")
    End If
    If all Then
        Call CLI.Append("/all ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Select.Meta /in &lt;meta.Xml&gt; /bbh &lt;bbh.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function SelectsMeta([in] As String, bbh As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Select.Meta")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Taxonomy.efetch /in &lt;nt.fasta&gt; [/out &lt;out.DIR&gt;]
''' ```
''' Fetch the taxonomy information of the fasta sequence from NCBI web server.
''' </summary>
'''

Public Function FetchTaxnData([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Taxonomy.efetch")
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
''' /Taxonomy.efetch.Merge /in &lt;in.DIR&gt; [/out &lt;out.Csv&gt;]
''' ```
''' </summary>
'''

Public Function MergeFetchTaxonData([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Taxonomy.efetch.Merge")
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
''' /to.kobas /in &lt;sbh.csv&gt; [/out &lt;kobas.tsv&gt;]
''' ```
''' </summary>
'''

Public Function _2_KOBASOutput([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/to.kobas")
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
''' /UniProt.bbh.mappings /in &lt;bbh.csv&gt; [/reverse /out &lt;mappings.txt&gt;]
''' ```
''' </summary>
'''

Public Function UniProtBBHMapTable([in] As String, Optional out As String = "", Optional reverse As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/UniProt.bbh.mappings")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
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
''' ```bash
''' /UniProt.GO.faa /in &lt;uniprot.xml&gt; [/lineBreak &lt;default=120&gt; /out &lt;proteins.faa&gt;]
''' ```
''' Export all of the protein sequence from the Uniprot database which have GO term id been assigned.
''' </summary>
'''

Public Function ExportGOFromUniprot([in] As String, Optional linebreak As String = "120", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/UniProt.GO.faa")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not linebreak.StringEmpty Then
            Call CLI.Append("/linebreak " & """" & linebreak & """ ")
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
''' /UniProt.KO.assign /in &lt;query_vs_uniprot.KO.besthit&gt; [/bbh &lt;uniprot_vs_query.KO.besthit&gt; /out &lt;out.KO.csv&gt;]
''' ```
''' Assign KO number to query from Uniprot reference sequence database alignment result.
''' </summary>
'''
''' <param name="[in]"> The sbh result of the alignment: query vs uniprot.KO.
''' </param>
''' <param name="bbh"> If this argument is presents in the cli input, then it means we use the bbh method for assign the KO number to query. 
'''               Both ``/in`` and ``/bbh`` is not top best selection output. The input file for this argument should be the result of ``/SBH.Export.Large``
'''               command, and ``/keeps_raw.queryName`` option should be enabled for keeps the taxonomy information.
''' </param>
''' <param name="out"> Use the eggHTS command ``/proteins.KEGG.plot`` for export the final KO number assignment result table.
''' </param>
Public Function UniProtKOAssign([in] As String, Optional bbh As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/UniProt.KO.assign")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not bbh.StringEmpty Then
            Call CLI.Append("/bbh " & """" & bbh & """ ")
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
''' /UniProt.KO.faa /in &lt;uniprot.xml&gt; [/lineBreak &lt;default=120&gt; /out &lt;proteins.faa&gt;]
''' ```
''' Export all of the protein sequence from the Uniprot database which have KO number been assigned.
''' </summary>
'''
''' <param name="[in]"> The Uniprot database which is downloaded from the Uniprot website or ftp site. 
'''               NOTE: this argument could be a file name list for export multiple database file, 
'''               each file should located in current directory and all of the sequence in given 
'''               file names will export into one fasta sequence file. 
'''               File names should be seperated by comma symbol as delimiter.
''' </param>
''' <param name="out"> The file path of the export protein sequence, title of each sequence consist with these fields: ``KO|uniprot_id fullName|scientificName``
''' </param>
Public Function ExportKOFromUniprot([in] As String, Optional linebreak As String = "120", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/UniProt.KO.faa")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not linebreak.StringEmpty Then
            Call CLI.Append("/linebreak " & """" & linebreak & """ ")
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
''' /venn.BBH /imports &lt;blastp_out.DIR&gt; [/skip-load /query &lt;queryName&gt; /all /coverage &lt;0.6&gt; /identities &lt;0.3&gt; /out &lt;outDIR&gt;]
''' ```
''' 2. Build venn table And bbh data from the blastp result out Or sbh data cache.
''' </summary>
'''
''' <param name="skip_load"> If the data source in the imports directory Is already the sbh data source, then using this parameter to skip the blastp file parsing.
''' </param>
Public Function VennBBH([imports] As String, 
                           Optional query As String = "", 
                           Optional coverage As String = "", 
                           Optional identities As String = "", 
                           Optional out As String = "", 
                           Optional skip_load As Boolean = False, 
                           Optional all As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/venn.BBH")
    Call CLI.Append(" ")
    Call CLI.Append("/imports " & """" & [imports] & """ ")
    If Not query.StringEmpty Then
            Call CLI.Append("/query " & """" & query & """ ")
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
    If skip_load Then
        Call CLI.Append("/skip-load ")
    End If
    If all Then
        Call CLI.Append("/all ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /venn.BlastAll /query &lt;queryDIR&gt; [/out &lt;outDIR&gt; /num_threads &lt;-1&gt; /evalue 10 /overrides /all /coverage &lt;0.8&gt; /identities &lt;0.3&gt;]
''' ```
''' Completely paired combos blastp bbh operations for the venn diagram Or network builder.
''' </summary>
'''
''' <param name="num_threads"> The number of the parallel blast task in this command, set this argument ZERO for single thread. default value Is -1 which means the number of the blast threads Is determined by system automatically.
''' </param>
''' <param name="all"> If this parameter Is represent, then all of the paired best hit will be export, otherwise only the top best will be export.
''' </param>
''' <param name="query"> Recommended format of the fasta title Is that the fasta title only contains gene locus_tag.
''' </param>
Public Function vennBlastAll(query As String, 
                                Optional out As String = "", 
                                Optional num_threads As String = "", 
                                Optional evalue As String = "", 
                                Optional coverage As String = "", 
                                Optional identities As String = "", 
                                Optional [overrides] As Boolean = False, 
                                Optional all As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/venn.BlastAll")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
    End If
    If Not evalue.StringEmpty Then
            Call CLI.Append("/evalue " & """" & evalue & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
    End If
    If [overrides] Then
        Call CLI.Append("/overrides ")
    End If
    If all Then
        Call CLI.Append("/all ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /venn.cache /imports &lt;blastp.DIR&gt; [/out &lt;sbh.out.DIR&gt; /coverage &lt;0.6&gt; /identities &lt;0.3&gt; /num_threads &lt;-1&gt; /overrides]
''' ```
''' 1. [SBH_Batch] Creates the sbh cache data for the downstream bbh analysis.
''' And this batch function is suitable with any scale of the blastp sbh data output.
''' </summary>
'''
''' <param name="num_threads"> The number of the sub process thread. -1 value is stands for auto config by the system.
''' </param>
Public Function VennCache([imports] As String, 
                             Optional out As String = "", 
                             Optional coverage As String = "", 
                             Optional identities As String = "", 
                             Optional num_threads As String = "", 
                             Optional [overrides] As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/venn.cache")
    Call CLI.Append(" ")
    Call CLI.Append("/imports " & """" & [imports] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
    End If
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
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
''' /venn.sbh.thread /in &lt;blastp.txt&gt; [/out &lt;out.sbh.csv&gt; /coverage &lt;0.6&gt; /identities &lt;0.3&gt; /overrides]
''' ```
''' </summary>
'''

Public Function SBHThread([in] As String, 
                             Optional out As String = "", 
                             Optional coverage As String = "", 
                             Optional identities As String = "", 
                             Optional [overrides] As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/venn.sbh.thread")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
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
''' /Whog.XML /in &lt;whog&gt; [/out &lt;whog.XML&gt;]
''' ```
''' Converts the whog text file into a XML data file.
''' </summary>
'''

Public Function WhogXML([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Whog.XML")
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
''' --bbh.export /in &lt;blast_out.DIR&gt; [/all /out &lt;out.DIR&gt; /single-query &lt;queryName&gt; /coverage &lt;0.5&gt; /identities 0.15]
''' ```
''' Batch export bbh result data from a directory.
''' </summary>
'''
''' <param name="all"> If this all Boolean value is specific, then the program will export all hits for the bbh not the top 1 best.
''' </param>
Public Function ExportBBH([in] As String, 
                             Optional out As String = "", 
                             Optional single_query As String = "", 
                             Optional coverage As String = "", 
                             Optional identities As String = "", 
                             Optional all As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--bbh.export")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not single_query.StringEmpty Then
            Call CLI.Append("/single-query " & """" & single_query & """ ")
    End If
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
    End If
    If all Then
        Call CLI.Append("/all ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' --blast.self /query &lt;query.fasta&gt; [/blast &lt;blast_HOME&gt; /out &lt;out.csv&gt;]
''' ```
''' Query fasta query against itself for paralogs.
''' </summary>
'''

Public Function SelfBlast(query As String, Optional blast As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("--blast.self")
    Call CLI.Append(" ")
    Call CLI.Append("/query " & """" & query & """ ")
    If Not blast.StringEmpty Then
            Call CLI.Append("/blast " & """" & blast & """ ")
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
''' --Export.Fasta /hits &lt;query-hits.csv&gt; /query &lt;query.fasta&gt; /subject &lt;subject.fasta&gt;
''' ```
''' </summary>
'''

Public Function ExportFasta(hits As String, query As String, subject As String) As Integer
    Dim CLI As New StringBuilder("--Export.Fasta")
    Call CLI.Append(" ")
    Call CLI.Append("/hits " & """" & hits & """ ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/subject " & """" & subject & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' --Export.Overviews /blast &lt;blastout.txt&gt; [/out &lt;overview.csv&gt;]
''' ```
''' </summary>
'''

Public Function ExportOverviews(blast As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("--Export.Overviews")
    Call CLI.Append(" ")
    Call CLI.Append("/blast " & """" & blast & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' --Export.SBH /in &lt;in.DIR&gt; /prefix &lt;queryName&gt; /out &lt;out.csv&gt; [/txt]
''' ```
''' </summary>
'''

Public Function ExportSBH([in] As String, prefix As String, out As String, Optional txt As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--Export.SBH")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/prefix " & """" & prefix & """ ")
    Call CLI.Append("/out " & """" & out & """ ")
    If txt Then
        Call CLI.Append("/txt ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' --Xml2Excel /in &lt;in.xml&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function XmlToExcel([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("--Xml2Excel")
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
''' --Xml2Excel.Batch /in &lt;inDIR&gt; [/out &lt;outDIR&gt; /Merge]
''' ```
''' </summary>
'''

Public Function XmlToExcelBatch([in] As String, Optional out As String = "", Optional merge As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--Xml2Excel.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If merge Then
        Call CLI.Append("/merge ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
