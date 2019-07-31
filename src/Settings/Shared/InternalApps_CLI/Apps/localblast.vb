#Region "Microsoft.VisualBasic::277478d7965958802a4492ed2db1ef70, Shared\InternalApps_CLI\Apps\localblast.vb"

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

    ' Class localblast
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
' assembly: ..\bin\localblast.exe

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
'  /Paralog:                          
'  /SBH.tophits:                      Filtering the sbh result with top SBH Score
'  /to.kobas:                         
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
'    /protein.EXPORT:                   Export the protein sequence and save as fasta format from the
'                                       uniprot database dump XML.
'    /UniProt.bbh.mappings:             
'    /UniProt.KO.faa:                   
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

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As localblast
          Return New localblast(App:=directory & "/" & localblast.App)
     End Function

''' <summary>
''' ```
''' /add.locus_tag /gb &lt;gb.gbk> /prefix &lt;prefix> [/add.gene /out &lt;out.gb>]
''' ```
''' Add locus_tag qualifier into the feature slot.
''' </summary>
'''
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
''' ```
''' /add.names /anno &lt;anno.csv> /gb &lt;genbank.gbk> [/out &lt;out.gbk> /tag &lt;overrides_name>]
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
''' ```
''' /align.union /query &lt;input.fasta> /ref &lt;input.fasta> /besthit &lt;besthit.csv> [/out &lt;union_hits.csv>]
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
''' ```
''' /AlignmentTable.TopBest /in &lt;table.csv> [/out &lt;out.csv>]
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
''' ```
''' /Bash.Venn /blast &lt;blastDIR> /inDIR &lt;fasta.DIR> /inRef &lt;inRefAs.DIR> [/out &lt;outDIR> /evalue &lt;evalue:10>]
''' ```
''' </summary>
'''
Public Function BashShell(blast As String, inDIR As String, inRef As String, Optional out As String = "", Optional evalue As String = "") As Integer
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
''' ```
''' /bbh.EXPORT /query &lt;query.blastp_out> /subject &lt;subject.blast_out> [/trim /out &lt;bbh.csv> /evalue 1e-3 /coverage 0.85 /identities 0.3]
''' ```
''' Export bbh mapping result from the blastp raw output.
''' </summary>
'''
Public Function BBHExportFile(query As String, subject As String, Optional out As String = "", Optional evalue As String = "", Optional coverage As String = "", Optional identities As String = "", Optional trim As Boolean = False) As Integer
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
''' ```
''' /BBH.Merge /in &lt;inDIR> [/out &lt;out.csv>]
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
''' ```
''' /bbh.topbest /in &lt;bbh.csv> [/out &lt;out.csv>]
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
''' ```
''' /Blastn.Maps.Taxid /in &lt;blastnMapping.csv> /2taxid &lt;acc2taxid.tsv/gi2taxid.dmp> [/gi2taxid /trim /tax &lt;NCBI_taxonomy:nodes/names> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function BlastnMapsTaxonomy([in] As String, _2taxid As String, Optional tax As String = "", Optional out As String = "", Optional gi2taxid As Boolean = False, Optional trim As Boolean = False) As Integer
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
''' ```
''' /blastn.Query /query &lt;query.fna/faa> /db &lt;db.DIR> [/thread /evalue 1e-5 /word_size &lt;-1> /out &lt;out.DIR>]
''' ```
''' Using target fasta sequence query against all of the fasta sequence in target direcotry. This function is single thread.
''' </summary>
'''
Public Function BlastnQuery(query As String, db As String, Optional evalue As String = "", Optional word_size As String = "", Optional out As String = "", Optional thread As Boolean = False) As Integer
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
''' ```
''' /blastn.Query.All /query &lt;query.fasta.DIR> /db &lt;db.DIR> [/skip-format /evalue 10 /word_size &lt;-1> /out &lt;out.DIR> /parallel /penalty &lt;penalty> /reward &lt;reward>]
''' ```
''' Using the fasta sequence in a directory query against all of the sequence in another directory.
''' </summary>
'''
Public Function BlastnQueryAll(query As String, db As String, Optional evalue As String = "", Optional word_size As String = "", Optional out As String = "", Optional penalty As String = "", Optional reward As String = "", Optional skip_format As Boolean = False, Optional parallel As Boolean = False) As Integer
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
''' ```
''' /BlastnMaps.Match.Taxid /in &lt;maps.csv> /acc2taxid &lt;acc2taxid.DIR> [/out &lt;out.tsv>]
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
''' ```
''' /BlastnMaps.Select /in &lt;reads.id.list.txt> /data &lt;blastn.maps.csv> [/out &lt;out.csv>]
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
''' ```
''' /BlastnMaps.Select.Top /in &lt;maps.csv> [/out &lt;out.csv>]
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
''' ```
''' /BlastnMaps.Summery /in &lt;in.DIR> [/split "-" /out &lt;out.csv>]
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
''' ```
''' /Blastp.BBH.Query /query &lt;query.fasta> /hit &lt;hit.source> [/out &lt;outDIR> /overrides /num_threads &lt;-1>]
''' ```
''' Using query fasta invoke blastp against the fasta files in a directory.
''' * This command tools required of NCBI blast+ suite, you must config the blast bin path by using ``settings.exe`` before running this command.
''' </summary>
'''
Public Function BlastpBBHQuery(query As String, hit As String, Optional out As String = "", Optional num_threads As String = "", Optional [overrides] As Boolean = False) As Integer
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
''' ```
''' /Chromosomes.Export /reads &lt;reads.fasta/DIR> /maps &lt;blastnMappings.Csv/DIR> [/out &lt;outDIR>]
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
''' ```
''' /COG.myva /blastp &lt;blastp.myva.txt/sbh.csv> /whog &lt;whog.XML> [/top.best /grep &lt;donothing> /simple /out &lt;out.csv/txt>]
''' ```
''' COG myva annotation using blastp raw output or exports sbh/bbh table result.
''' </summary>
'''
Public Function COG_myva(blastp As String, whog As String, Optional grep As String = "", Optional out As String = "", Optional top_best As Boolean = False, Optional simple As Boolean = False) As Integer
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
''' ```
''' /COG.Statics /in &lt;myva_cogs.csv> [/locus &lt;locus.txt/csv> /locuMap &lt;Gene> /out &lt;out.csv>]
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
''' ```
''' /COG2014.result /sbh &lt;query-vs-COG2003-2014.fasta> /cog &lt;cog2003-2014.csv> [/cog.names &lt;cognames2003-2014.tab> /out &lt;out.myva_cog.csv>]
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
''' ```
''' /Copy.Fasta /imports &lt;DIR> [/type &lt;faa,fna,ffn,fasta,...., default:=faa> /out &lt;DIR>]
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
''' ```
''' /Copy.PTT /in &lt;inDIR> [/out &lt;outDIR>]
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
''' ```
''' /Copys /imports &lt;DIR> [/out &lt;outDIR>]
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
''' ```
''' /Export.AlignmentTable /in &lt;alignment.txt> [/split /header.split /out &lt;outDIR/file>]
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
''' ```
''' /Export.AlignmentTable.giList /in &lt;table.csv> [/out &lt;gi.txt>]
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
''' ```
''' /Export.Blastn /in &lt;in.txt> [/out &lt;out.csv>]
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
''' /Export.blastnMaps /in &lt;blastn.txt> [/best /out &lt;out.csv>]
''' ```
''' </summary>
'''
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
''' ```
''' /Export.blastnMaps.Batch /in &lt;blastn_out.DIR> [/best /out &lt;out.DIR> /num_threads &lt;-1>]
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
''' ```
''' /Export.blastnMaps.littles /in &lt;blastn.txt.DIR> [/out &lt;out.csv.DIR>]
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
''' ```
''' /Export.blastnMaps.Write /in &lt;blastn_out.DIR> [/best /out &lt;write.csv>]
''' ```
''' Exports large amount of blastn output files and write all data into a specific csv file.
''' </summary>
'''
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
''' ```
''' /Export.BlastX /in &lt;blastx.txt> [/top /Uncharacterized.exclude /out &lt;out.csv>]
''' ```
''' Export the blastx alignment result into a csv table.
''' </summary>
'''
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
''' ```
''' /EXPORT.COGs.from.DOOR /in &lt;DOOR.opr> [/out &lt;out.csv>]
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
''' ```
''' /Export.gb /gb &lt;genbank.gb/DIR> [/flat /out &lt;outDIR> /simple /batch]
''' ```
''' Export the *.fna, *.faa, *.ptt file from the gbk file.
''' </summary>
'''
Public Function ExportGenbank(gb As String, Optional out As String = "", Optional flat As Boolean = False, Optional simple As Boolean = False, Optional batch As Boolean = False) As Integer
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
''' ```
''' /Export.gb.genes /gb &lt;genbank.gb> [/geneName /out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function ExportGenesFasta(gb As String, Optional out As String = "", Optional genename As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Export.gb.genes")
    Call CLI.Append(" ")
    Call CLI.Append("/gb " & """" & gb & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If genename Then
        Call CLI.Append("/genename ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.gpff /in &lt;genome.gpff> /gff &lt;genome.gff> [/out &lt;out.PTT>]
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
''' ```
''' /Export.gpffs [/in &lt;inDIR>]
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
''' ```
''' /Export.Locus /in &lt;sbh/bbh_DIR> [/hit /out &lt;out.txt>]
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
''' ```
''' /Export.Protein /gb &lt;genome.gb> [/out &lt;out.fasta>]
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
''' ```
''' /Fasta.Filters /in &lt;nt.fasta> /key &lt;regex/list.txt> [/tokens /out &lt;out.fasta> /p]
''' ```
''' Filter the fasta sequence subset from a larger fasta database by using the regexp for match on the fasta title.
''' </summary>
'''
Public Function Filter([in] As String, key As String, Optional out As String = "", Optional tokens As Boolean = False, Optional p As Boolean = False) As Integer
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
''' ```
''' /hits.ID.list /in &lt;bbhindex.csv> [/out &lt;out.txt>]
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
''' ```
''' /Identities.Matrix /hit &lt;sbh/bbh.csv> [/out &lt;out.csv> /cut 0.65]
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
''' ```
''' /install.cog2003-2014 /db &lt;prot2003-2014.fasta>
''' ```
''' Config the ``prot2003-2014.fasta`` database for GCModeller localblast tools. This database will be using for the COG annotation.
''' This command required of the blast+ install first.
''' </summary>
'''
Public Function InstallCOGDatabase(db As String) As Integer
    Dim CLI As New StringBuilder("/install.cog2003-2014")
    Call CLI.Append(" ")
    Call CLI.Append("/db " & """" & db & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /locus.Selects /locus &lt;locus.txt> /bh &lt;bbhindex.csv> [/out &lt;out.csv>]
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
''' ```
''' /MAT.evalue /in &lt;sbh.csv> [/out &lt;mat.csv> /flip]
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
''' ```
''' /Merge.faa /in &lt;DIR> /out &lt;out.fasta>
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
''' ```
''' /Paralog /blastp &lt;blastp.txt> [/coverage 0.5 /identities 0.3 /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ExportParalog(blastp As String, Optional coverage As String = "", Optional identities As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Paralog")
    Call CLI.Append(" ")
    Call CLI.Append("/blastp " & """" & blastp & """ ")
    If Not coverage.StringEmpty Then
            Call CLI.Append("/coverage " & """" & coverage & """ ")
    End If
    If Not identities.StringEmpty Then
            Call CLI.Append("/identities " & """" & identities & """ ")
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
''' /Print /in &lt;inDIR> [/ext &lt;ext> /out &lt;out.Csv>]
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
''' ```
''' /protein.EXPORT /in &lt;uniprot.xml> [/sp &lt;name> /exclude /out &lt;out.fasta>]
''' ```
''' Export the protein sequence and save as fasta format from the uniprot database dump XML.
''' </summary>
'''
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
''' ```
''' /query.cog2003-2014 /query &lt;query.fasta> [/evalue 1e-5 /coverage 0.65 /identities 0.85 /all /out &lt;out.DIR> /db &lt;cog2003-2014.fasta> /blast+ &lt;blast+/bin>]
''' ```
''' Protein COG annotation by using NCBI cog2003-2014.fasta database.
''' </summary>
'''
Public Function COG2003_2014(query As String, Optional evalue As String = "", Optional coverage As String = "", Optional identities As String = "", Optional out As String = "", Optional db As String = "", Optional blast_ As String = "", Optional all As Boolean = False) As Integer
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
''' ```
''' /Reads.OTU.Taxonomy /in &lt;blastnMaps.csv> /OTU &lt;OTU_data.csv> /tax &lt;taxonomy:nodes/names> [/fill.empty /out &lt;out.csv>]
''' ```
''' If the blastnmapping data have the duplicated OTU tags, then this function will makes a copy of the duplicated OTU tag data. top-best data will not.
''' </summary>
'''
Public Function ReadsOTU_Taxonomy([in] As String, OTU As String, tax As String, Optional out As String = "", Optional fill_empty As Boolean = False) As Integer
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
''' ```
''' /ref.acc.list /in &lt;blastnMaps.csv/DIR> [/out &lt;out.csv>]
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
''' ```
''' /ref.gi.list /in &lt;blastnMaps.csv/DIR> [/out &lt;out.csv>]
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
''' ```
''' /SBH.BBH.Batch /in &lt;sbh.DIR> [/identities &lt;-1> /coverage &lt;-1> /all /out &lt;bbh.DIR> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function SBH_BBH_Batch([in] As String, Optional identities As String = "", Optional coverage As String = "", Optional out As String = "", Optional num_threads As String = "", Optional all As Boolean = False) As Integer
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
''' ```
''' /SBH.Export.Large /in &lt;blastp_out.txt/directory> [/top.best /trim-kegg /out &lt;sbh.csv> /s.pattern &lt;default=-> /q.pattern &lt;default=-> /identities 0.15 /coverage 0.5 /split]
''' ```
''' Using this command for export the sbh result of your blastp raw data.
''' </summary>
'''
Public Function ExportBBHLarge([in] As String, Optional out As String = "", Optional s_pattern As String = "-", Optional q_pattern As String = "-", Optional identities As String = "", Optional coverage As String = "", Optional top_best As Boolean = False, Optional trim_kegg As Boolean = False, Optional split As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/SBH.Export.Large")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
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
    If top_best Then
        Call CLI.Append("/top.best ")
    End If
    If trim_kegg Then
        Call CLI.Append("/trim-kegg ")
    End If
    If split Then
        Call CLI.Append("/split ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /SBH.tophits /in &lt;sbh.csv> [/uniprotKB /out &lt;out.csv>]
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
''' ```
''' /SBH.Trim /in &lt;sbh.csv> /evalue &lt;evalue> [/identities 0.15 /coverage 0.5 /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function SBHTrim([in] As String, evalue As String, Optional identities As String = "", Optional coverage As String = "", Optional out As String = "") As Integer
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
''' ```
''' /sbh2bbh /qvs &lt;qvs.sbh.csv> /svq &lt;svq.sbh.csv> [/trim /query.pattern &lt;default="-"> /hit.pattern &lt;default="-"> /identities &lt;-1> /coverage &lt;-1> /all /out &lt;bbh.csv>]
''' ```
''' Export bbh result from the sbh pairs.
''' </summary>
'''
Public Function BBHExport2(qvs As String, svq As String, Optional query_pattern As String = "-", Optional hit_pattern As String = "-", Optional identities As String = "", Optional coverage As String = "", Optional out As String = "", Optional trim As Boolean = False, Optional all As Boolean = False) As Integer
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
''' ```
''' /Select.Meta /in &lt;meta.Xml> /bbh &lt;bbh.csv> [/out &lt;out.csv>]
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
''' ```
''' /Taxonomy.efetch /in &lt;nt.fasta> [/out &lt;out.DIR>]
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
''' ```
''' /Taxonomy.efetch.Merge /in &lt;in.DIR> [/out &lt;out.Csv>]
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
''' ```
''' /to.kobas /in &lt;sbh.csv> [/out &lt;kobas.tsv>]
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
''' ```
''' /UniProt.bbh.mappings /in &lt;bbh.csv> [/reverse /out &lt;mappings.txt>]
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
''' ```
''' /UniProt.KO.faa /in &lt;uniprot.xml> [/out &lt;proteins.faa>]
''' ```
''' </summary>
'''
Public Function ExportKOFromUniprot([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/UniProt.KO.faa")
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
''' /venn.BBH /imports &lt;blastp_out.DIR> [/skip-load /query &lt;queryName> /all /coverage &lt;0.6> /identities &lt;0.3> /out &lt;outDIR>]
''' ```
''' 2. Build venn table And bbh data from the blastp result out Or sbh data cache.
''' </summary>
'''
Public Function VennBBH([imports] As String, Optional query As String = "", Optional coverage As String = "", Optional identities As String = "", Optional out As String = "", Optional skip_load As Boolean = False, Optional all As Boolean = False) As Integer
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
''' ```
''' /venn.BlastAll /query &lt;queryDIR> [/out &lt;outDIR> /num_threads &lt;-1> /evalue 10 /overrides /all /coverage &lt;0.8> /identities &lt;0.3>]
''' ```
''' Completely paired combos blastp bbh operations for the venn diagram Or network builder.
''' </summary>
'''
Public Function vennBlastAll(query As String, Optional out As String = "", Optional num_threads As String = "", Optional evalue As String = "", Optional coverage As String = "", Optional identities As String = "", Optional [overrides] As Boolean = False, Optional all As Boolean = False) As Integer
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
''' ```
''' /venn.cache /imports &lt;blastp.DIR> [/out &lt;sbh.out.DIR> /coverage &lt;0.6> /identities &lt;0.3> /num_threads &lt;-1> /overrides]
''' ```
''' 1. [SBH_Batch] Creates the sbh cache data for the downstream bbh analysis.
''' And this batch function is suitable with any scale of the blastp sbh data output.
''' </summary>
'''
Public Function VennCache([imports] As String, Optional out As String = "", Optional coverage As String = "", Optional identities As String = "", Optional num_threads As String = "", Optional [overrides] As Boolean = False) As Integer
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
''' ```
''' /venn.sbh.thread /in &lt;blastp.txt> [/out &lt;out.sbh.csv> /coverage &lt;0.6> /identities &lt;0.3> /overrides]
''' ```
''' </summary>
'''
Public Function SBHThread([in] As String, Optional out As String = "", Optional coverage As String = "", Optional identities As String = "", Optional [overrides] As Boolean = False) As Integer
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
''' ```
''' /Whog.XML /in &lt;whog> [/out &lt;whog.XML>]
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
''' ```
''' --bbh.export /in &lt;blast_out.DIR> [/all /out &lt;out.DIR> /single-query &lt;queryName> /coverage &lt;0.5> /identities 0.15]
''' ```
''' Batch export bbh result data from a directory.
''' </summary>
'''
Public Function ExportBBH([in] As String, Optional out As String = "", Optional single_query As String = "", Optional coverage As String = "", Optional identities As String = "", Optional all As Boolean = False) As Integer
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
''' ```
''' --blast.self /query &lt;query.fasta> [/blast &lt;blast_HOME> /out &lt;out.csv>]
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
''' ```
''' --Export.Fasta /hits &lt;query-hits.csv> /query &lt;query.fasta> /subject &lt;subject.fasta>
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
''' ```
''' --Export.Overviews /blast &lt;blastout.txt> [/out &lt;overview.csv>]
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
''' ```
''' --Export.SBH /in &lt;in.DIR> /prefix &lt;queryName> /out &lt;out.csv> [/txt]
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
''' ```
''' --Xml2Excel /in &lt;in.xml> [/out &lt;out.csv>]
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
''' ```
''' --Xml2Excel.Batch /in &lt;inDIR> [/out &lt;outDIR> /Merge]
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

