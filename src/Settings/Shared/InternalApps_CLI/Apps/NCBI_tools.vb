#Region "Microsoft.VisualBasic::41846eca9a83840db85f78d0cbd4d23b, Shared\InternalApps_CLI\Apps\NCBI_tools.vb"

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

    ' Class NCBI_tools
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
' assembly: ..\bin\NCBI_tools.exe

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
'  Tools collection for handling NCBI data, includes: nt/nr database, NCBI taxonomy analysis, OTU taxonomy analysis,
'  genbank database, and sequence query tools.
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /assign.fasta.taxonomy:         
'  /Assign.Taxonomy.From.Ref:      
'  /Assign.Taxonomy.SSU:           
'  /Associates.Brief:              
'  /gbff.union:                    
'  /gpff.fasta:                    
'  /MapHits.list:                  
'  /OTU.Taxonomy.Replace:          Using ``MapHits`` property
' 
' 
' API list that with functional grouping
' 
' 1. NCBI ``nt`` database tools
' 
' 
'    /nt.matches.accession:          Create subset of the nt database by a given list of Accession ID.
'    /nt.matches.key:                
'    /nt.matches.name:               
'    /word.tokens:                   
' 
' 
' 2. NCBI data export tools
' 
' 
'    /Filter.Exports:                String similarity match of the fasta title with given terms for search
'                                    and export by taxonomy.
' 
' 
' 3. NCBI GI tools(Obsolete from NCBI, 2016-10-20)
' 
'    > https://www.ncbi.nlm.nih.gov/news/03-02-2016-phase-out-of-GI-numbers/
'    
' 
'    ###### NCBI is phasing out sequence GIs - use Accession.Version instead!
'    
' 
'    As of September 2016, the integer sequence identifiers known as "GIs" will no longer be included in the GenBank,
'    GenPept, and FASTA formats supported by NCBI for sequence records. The FASTA header will be further simplified
'    to report only the sequence accession.version and record title for accessions managed by the International Sequence
'    Database Collaboration (INSDC) and NCBI’s Reference Sequence (RefSeq) project. As NCBI makes this transition, we
'    encourage any users who have workflows that depend on GI's to begin planning to use accession.version identifiers
'    instead. After September 2016, any processes solely dependent on GIs will no longer function as expected.
' 
' 
'    /Assign.Taxonomy:               
'    /Associate.Taxonomy:            
'    /Build_gi2taxi:                 
'    /Export.GI:                     
'    /Filter.Exports:                String similarity match of the fasta title with given terms for search
'                                    and export by taxonomy.
'    /gi.Match:                      
'    /gi.Matchs:                     
'    /Nt.Taxonomy:                   
'    /Split.By.Taxid:                Split the input fasta file by taxid grouping.
'    /Taxonomy.Data:                 
' 
' 
' 4. NCBI taxonomy tools
' 
' 
'    /accid2taxid.Match:             Creates the subset of the ultra-large accession to ncbi taxonomy
'                                    id database.
'    /OTU.associated:                
'    /OTU.diff:                      
'    /OTU.Taxonomy:                  
'    /Search.Taxonomy:               
'    /Split.By.Taxid:                Split the input fasta file by taxid grouping.
'    /Split.By.Taxid.Batch:          
'    /Taxonomy.Data:                 
'    /Taxonomy.Maphits.Overview:     
'    /Taxonomy.Tree:                 Output taxonomy query info by a given NCBI taxid list.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Tools collection for handling NCBI data, includes: nt/nr database, NCBI taxonomy analysis, OTU taxonomy analysis, genbank database, and sequence query tools.
''' </summary>
'''
Public Class NCBI_tools : Inherits InteropService

    Public Const App$ = "NCBI_tools.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As NCBI_tools
          Return New NCBI_tools(App:=directory & "/" & NCBI_tools.App)
     End Function

''' <summary>
''' ```bash
''' /accid2taxid.Match /in &lt;nt.parts.fasta/list.txt&gt; /acc2taxid &lt;acc2taxid.dmp/DIR&gt; [/gb_priority /append.src /accid_grep &lt;default=-&gt; /out &lt;acc2taxid_match.txt&gt;]
''' ```
''' Creates the subset of the ultra-large accession to ncbi taxonomy id database.
''' </summary>
'''
''' <param name="accid_grep"> When the fasta title or the text line in the list is not an NCBI accession id, 
'''               then you needs this script for accession id grep operation.
''' </param>
Public Function accidMatch([in] As String, 
                              acc2taxid As String, 
                              Optional accid_grep As String = "-", 
                              Optional out As String = "", 
                              Optional gb_priority As Boolean = False, 
                              Optional append_src As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/accid2taxid.Match")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/acc2taxid " & """" & acc2taxid & """ ")
    If Not accid_grep.StringEmpty Then
            Call CLI.Append("/accid_grep " & """" & accid_grep & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If gb_priority Then
        Call CLI.Append("/gb_priority ")
    End If
    If append_src Then
        Call CLI.Append("/append.src ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /assign.fasta.taxonomy /in &lt;database.fasta&gt; /accession2taxid &lt;accession2taxid.txt&gt; /taxonomy &lt;names.dmp/nodes.dmp&gt; [/accid_grep &lt;default=-&gt; /append &lt;data.csv&gt; /summary.tsv /out &lt;out.directory&gt;]
''' ```
''' </summary>
'''
''' <param name="accession2taxid"> This mapping data file is usually a subset of the accession2taxid file, and comes from the ``/accid2taxid.Match`` command.
''' </param>
''' <param name="append"> If this parameter was presented, then additional data will append to the fasta title and the csv summary file. 
'''               This file should have a column named ``ID`` correspond to the ``accession_id``, 
'''               or a column named ``Species`` correspond to the ``species`` from NCBI taxonomy.
''' </param>
''' <param name="summary_tsv"> The output summary table file saved in tsv format.
''' </param>
Public Function AssignFastaTaxonomy([in] As String, 
                                       accession2taxid As String, 
                                       taxonomy As String, 
                                       Optional accid_grep As String = "-", 
                                       Optional append As String = "", 
                                       Optional out As String = "", 
                                       Optional summary_tsv As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/assign.fasta.taxonomy")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/accession2taxid " & """" & accession2taxid & """ ")
    Call CLI.Append("/taxonomy " & """" & taxonomy & """ ")
    If Not accid_grep.StringEmpty Then
            Call CLI.Append("/accid_grep " & """" & accid_grep & """ ")
    End If
    If Not append.StringEmpty Then
            Call CLI.Append("/append " & """" & append & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If summary_tsv Then
        Call CLI.Append("/summary.tsv ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Assign.Taxonomy /in &lt;in.DIR&gt; /gi &lt;regexp&gt; /index &lt;fieldName&gt; /tax &lt;NCBI nodes/names.dmp&gt; /gi2taxi &lt;gi2taxi.txt/bin&gt; [/out &lt;out.DIR&gt;]
''' ```
''' </summary>
'''

Public Function AssignTaxonomy([in] As String, 
                                  gi As String, 
                                  index As String, 
                                  tax As String, 
                                  gi2taxi As String, 
                                  Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Assign.Taxonomy")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/gi " & """" & gi & """ ")
    Call CLI.Append("/index " & """" & index & """ ")
    Call CLI.Append("/tax " & """" & tax & """ ")
    Call CLI.Append("/gi2taxi " & """" & gi2taxi & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Assign.Taxonomy.From.Ref /in &lt;in.DIR&gt; /ref &lt;nt.taxonomy.fasta&gt; [/index &lt;Name&gt; /non-BIOM /out &lt;out.DIR&gt;]
''' ```
''' </summary>
'''

Public Function AssignTaxonomyFromRef([in] As String, 
                                         ref As String, 
                                         Optional index As String = "", 
                                         Optional out As String = "", 
                                         Optional non_biom As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Assign.Taxonomy.From.Ref")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    If Not index.StringEmpty Then
            Call CLI.Append("/index " & """" & index & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If non_biom Then
        Call CLI.Append("/non-biom ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Assign.Taxonomy.SSU /in &lt;in.DIR&gt; /index &lt;fieldName&gt; /ref &lt;SSU-ref.fasta&gt; [/out &lt;out.DIR&gt;]
''' ```
''' </summary>
'''

Public Function AssignTaxonomy2([in] As String, index As String, ref As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Assign.Taxonomy.SSU")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/index " & """" & index & """ ")
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
''' /Associate.Taxonomy /in &lt;in.DIR&gt; /tax &lt;ncbi_taxonomy:names,nodes&gt; /gi2taxi &lt;gi2taxi.bin&gt; [/gi &lt;nt.gi.csv&gt; /out &lt;out.DIR&gt;]
''' ```
''' </summary>
'''

Public Function AssociateTaxonomy([in] As String, 
                                     tax As String, 
                                     gi2taxi As String, 
                                     Optional gi As String = "", 
                                     Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Associate.Taxonomy")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/tax " & """" & tax & """ ")
    Call CLI.Append("/gi2taxi " & """" & gi2taxi & """ ")
    If Not gi.StringEmpty Then
            Call CLI.Append("/gi " & """" & gi & """ ")
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
''' /Associates.Brief /in &lt;in.DIR&gt; /ls &lt;ls.txt&gt; [/index &lt;Name&gt; /out &lt;out.tsv&gt;]
''' ```
''' </summary>
'''

Public Function Associates([in] As String, ls As String, Optional index As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Associates.Brief")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/ls " & """" & ls & """ ")
    If Not index.StringEmpty Then
            Call CLI.Append("/index " & """" & index & """ ")
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
''' /Build_gi2taxi /in &lt;gi2taxi.dmp&gt; [/out &lt;out.dat&gt;]
''' ```
''' </summary>
'''

Public Function Build_gi2taxi([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Build_gi2taxi")
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
''' /Export.GI /in &lt;ncbi:nt.fasta&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function ExportGI([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Export.GI")
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
''' /Filter.Exports /in &lt;nt.fasta&gt; /tax &lt;taxonomy_DIR&gt; /gi2taxid &lt;gi2taxid.txt&gt; /words &lt;list.txt&gt; [/out &lt;out.DIR&gt;]
''' ```
''' String similarity match of the fasta title with given terms for search and export by taxonomy.
''' </summary>
'''

Public Function FilterExports([in] As String, 
                                 tax As String, 
                                 gi2taxid As String, 
                                 words As String, 
                                 Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Filter.Exports")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/tax " & """" & tax & """ ")
    Call CLI.Append("/gi2taxid " & """" & gi2taxid & """ ")
    Call CLI.Append("/words " & """" & words & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /gbff.union /in &lt;data.directory&gt; [/out &lt;output.union.gb&gt;]
''' ```
''' </summary>
'''

Public Function UnionGBK([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/gbff.union")
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
''' /gi.Match /in &lt;nt.parts.fasta/list.txt&gt; /gi2taxid &lt;gi2taxid.dmp&gt; [/out &lt;gi_match.txt&gt;]
''' ```
''' </summary>
'''

Public Function giMatch([in] As String, gi2taxid As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/gi.Match")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/gi2taxid " & """" & gi2taxid & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /gi.Matchs /in &lt;nt.parts.fasta.DIR&gt; /gi2taxid &lt;gi2taxid.dmp&gt; [/out &lt;gi_match.txt.DIR&gt; /num_threads &lt;-1&gt;]
''' ```
''' </summary>
'''

Public Function giMatchs([in] As String, gi2taxid As String, Optional out As String = "", Optional num_threads As String = "") As Integer
    Dim CLI As New StringBuilder("/gi.Matchs")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/gi2taxid " & """" & gi2taxid & """ ")
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
''' ```bash
''' /gpff.fasta /in &lt;gpff.txt&gt; [/out &lt;out.fasta&gt;]
''' ```
''' </summary>
'''

Public Function gpff2Fasta([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/gpff.fasta")
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
''' /MapHits.list /in &lt;in.csv&gt; [/out &lt;out.txt&gt;]
''' ```
''' </summary>
'''
''' <param name="[in]">
''' </param>
Public Function GetMapHitsList([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/MapHits.list")
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
''' /nt.matches.accession /in &lt;nt.fasta&gt; /list &lt;accession.list&gt; [/accid &lt;default=&quot;tokens &apos;.&apos; first&quot;&gt; /out &lt;subset.fasta&gt;]
''' ```
''' Create subset of the nt database by a given list of Accession ID.
''' </summary>
'''

Public Function NtAccessionMatches([in] As String, list As String, Optional accid As String = "tokens '.' first", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/nt.matches.accession")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/list " & """" & list & """ ")
    If Not accid.StringEmpty Then
            Call CLI.Append("/accid " & """" & accid & """ ")
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
''' /nt.matches.key /in &lt;nt.fasta&gt; /list &lt;words.txt&gt; [/out &lt;out.fasta&gt;]
''' ```
''' </summary>
'''

Public Function NtKeyMatches([in] As String, list As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/nt.matches.key")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/list " & """" & list & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /nt.matches.name /in &lt;nt.fasta&gt; /list &lt;names.csv&gt; [/out &lt;out.fasta&gt;]
''' ```
''' </summary>
'''
''' <param name="list">
''' </param>
Public Function NtNameMatches([in] As String, list As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/nt.matches.name")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/list " & """" & list & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Nt.Taxonomy /in &lt;nt.fasta&gt; /gi2taxi &lt;gi2taxi.bin&gt; /tax &lt;ncbi_taxonomy:names,nodes&gt; [/out &lt;out.fasta&gt;]
''' ```
''' </summary>
'''

Public Function NtTaxonomy([in] As String, gi2taxi As String, tax As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Nt.Taxonomy")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/gi2taxi " & """" & gi2taxi & """ ")
    Call CLI.Append("/tax " & """" & tax & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /OTU.associated /in &lt;OTU.Data&gt; /maps &lt;mapsHit.csv&gt; [/RawMap &lt;data_mapping.csv&gt; /OTU_Field &lt;&quot;#OTU_NUM&quot;&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function OTUAssociated([in] As String, 
                                 maps As String, 
                                 Optional rawmap As String = "", 
                                 Optional otu_field As String = "", 
                                 Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/OTU.associated")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    If Not rawmap.StringEmpty Then
            Call CLI.Append("/rawmap " & """" & rawmap & """ ")
    End If
    If Not otu_field.StringEmpty Then
            Call CLI.Append("/otu_field " & """" & otu_field & """ ")
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
''' /OTU.diff /ref &lt;OTU.Data1.csv&gt; /parts &lt;OTU.Data2.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function OTUDiff(ref As String, parts As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/OTU.diff")
    Call CLI.Append(" ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    Call CLI.Append("/parts " & """" & parts & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /OTU.Taxonomy /in &lt;OTU.Data&gt; /maps &lt;mapsHit.csv&gt; /tax &lt;taxonomy:nodes/names&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function OTU_Taxonomy([in] As String, maps As String, tax As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/OTU.Taxonomy")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    Call CLI.Append("/tax " & """" & tax & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /OTU.Taxonomy.Replace /in &lt;otu.table.csv&gt; /maps &lt;maphits.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' Using ``MapHits`` property
''' </summary>
'''

Public Function OTUTaxonomyReplace([in] As String, maps As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/OTU.Taxonomy.Replace")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
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
''' /Search.Taxonomy /in &lt;list.txt/expression.csv&gt; /ncbi_taxonomy &lt;taxnonmy:name/nodes.dmp&gt; [/top 10 /expression /cut 0.65 /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="expression"> Search the taxonomy text by using query expression? If this set true, then the input should be a expression csv file.
''' </param>
''' <param name="cut"> This parameter will be disabled when ``/expression`` is presents.
''' </param>
''' <param name="[in]">
''' </param>
Public Function SearchTaxonomy([in] As String, 
                                  ncbi_taxonomy As String, 
                                  Optional top As String = "", 
                                  Optional cut As String = "", 
                                  Optional out As String = "", 
                                  Optional expression As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Search.Taxonomy")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/ncbi_taxonomy " & """" & ncbi_taxonomy & """ ")
    If Not top.StringEmpty Then
            Call CLI.Append("/top " & """" & top & """ ")
    End If
    If Not cut.StringEmpty Then
            Call CLI.Append("/cut " & """" & cut & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If expression Then
        Call CLI.Append("/expression ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Split.By.Taxid /in &lt;nt.fasta&gt; [/gi2taxid &lt;gi2taxid.txt&gt; /out &lt;outDIR&gt;]
''' ```
''' Split the input fasta file by taxid grouping.
''' </summary>
'''

Public Function SplitByTaxid([in] As String, Optional gi2taxid As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Split.By.Taxid")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not gi2taxid.StringEmpty Then
            Call CLI.Append("/gi2taxid " & """" & gi2taxid & """ ")
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
''' /Split.By.Taxid.Batch /in &lt;nt.fasta.DIR&gt; [/num_threads &lt;-1&gt; /out &lt;outDIR&gt;]
''' ```
''' </summary>
'''

Public Function SplitByTaxidBatch([in] As String, Optional num_threads As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Split.By.Taxid.Batch")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not num_threads.StringEmpty Then
            Call CLI.Append("/num_threads " & """" & num_threads & """ ")
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
''' /Taxonomy.Data /data &lt;data.csv&gt; /field.gi &lt;GI&gt; /gi2taxid &lt;gi2taxid.list.txt&gt; /tax &lt;ncbi_taxonomy:nodes/names&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function TaxonomyTreeData(data As String, 
                                    field_gi As String, 
                                    gi2taxid As String, 
                                    tax As String, 
                                    Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Taxonomy.Data")
    Call CLI.Append(" ")
    Call CLI.Append("/data " & """" & data & """ ")
    Call CLI.Append("/field.gi " & """" & field_gi & """ ")
    Call CLI.Append("/gi2taxid " & """" & gi2taxid & """ ")
    Call CLI.Append("/tax " & """" & tax & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Taxonomy.Maphits.Overview /in &lt;in.DIR&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function TaxidMapHitViews([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Taxonomy.Maphits.Overview")
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
''' /Taxonomy.Tree /taxid &lt;taxid.list.txt&gt; /tax &lt;ncbi_taxonomy:nodes/names&gt; [/out &lt;out.csv&gt;]
''' ```
''' Output taxonomy query info by a given NCBI taxid list.
''' </summary>
'''

Public Function TaxonomyTree(taxid As String, tax As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Taxonomy.Tree")
    Call CLI.Append(" ")
    Call CLI.Append("/taxid " & """" & taxid & """ ")
    Call CLI.Append("/tax " & """" & tax & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /word.tokens /in &lt;list.txt&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function GetWordTokens([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/word.tokens")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace

