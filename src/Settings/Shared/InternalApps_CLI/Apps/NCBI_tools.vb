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
'  // VERSION:   3.3277.7238.20186
'  // ASSEMBLY:  Settings, Version=3.3277.7238.20186, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/26/2019 11:12:52 AM
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

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As NCBI_tools
          Return New NCBI_tools(App:=directory & "/" & NCBI_tools.App)
     End Function

''' <summary>
''' ```
''' /accid2taxid.Match /in &lt;nt.parts.fasta/list.txt> /acc2taxid &lt;acc2taxid.dmp/DIR> [/gb_priority /append.src /accid_grep &lt;default=-> /out &lt;acc2taxid_match.txt>]
''' ```
''' Creates the subset of the ultra-large accession to ncbi taxonomy id database.
''' </summary>
'''
Public Function accidMatch([in] As String, acc2taxid As String, Optional accid_grep As String = "-", Optional out As String = "", Optional gb_priority As Boolean = False, Optional append_src As Boolean = False) As Integer
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
''' ```
''' /assign.fasta.taxonomy /in &lt;database.fasta> /accession2taxid &lt;accession2taxid.txt> /taxonomy &lt;names.dmp/nodes.dmp> [/accid_grep &lt;default=-> /append &lt;data.csv> /summary.tsv /out &lt;out.directory>]
''' ```
''' </summary>
'''
Public Function AssignFastaTaxonomy([in] As String, accession2taxid As String, taxonomy As String, Optional accid_grep As String = "-", Optional append As String = "", Optional out As String = "", Optional summary_tsv As Boolean = False) As Integer
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
''' ```
''' /Assign.Taxonomy /in &lt;in.DIR> /gi &lt;regexp> /index &lt;fieldName> /tax &lt;NCBI nodes/names.dmp> /gi2taxi &lt;gi2taxi.txt/bin> [/out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function AssignTaxonomy([in] As String, gi As String, index As String, tax As String, gi2taxi As String, Optional out As String = "") As Integer
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
''' ```
''' /Assign.Taxonomy.From.Ref /in &lt;in.DIR> /ref &lt;nt.taxonomy.fasta> [/index &lt;Name> /non-BIOM /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function AssignTaxonomyFromRef([in] As String, ref As String, Optional index As String = "", Optional out As String = "", Optional non_biom As Boolean = False) As Integer
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
''' ```
''' /Assign.Taxonomy.SSU /in &lt;in.DIR> /index &lt;fieldName> /ref &lt;SSU-ref.fasta> [/out &lt;out.DIR>]
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
''' ```
''' /Associate.Taxonomy /in &lt;in.DIR> /tax &lt;ncbi_taxonomy:names,nodes> /gi2taxi &lt;gi2taxi.bin> [/gi &lt;nt.gi.csv> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function AssociateTaxonomy([in] As String, tax As String, gi2taxi As String, Optional gi As String = "", Optional out As String = "") As Integer
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
''' ```
''' /Associates.Brief /in &lt;in.DIR> /ls &lt;ls.txt> [/index &lt;Name> /out &lt;out.tsv>]
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
''' ```
''' /Build_gi2taxi /in &lt;gi2taxi.dmp> [/out &lt;out.dat>]
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
''' ```
''' /Export.GI /in &lt;ncbi:nt.fasta> [/out &lt;out.csv>]
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
''' ```
''' /Filter.Exports /in &lt;nt.fasta> /tax &lt;taxonomy_DIR> /gi2taxid &lt;gi2taxid.txt> /words &lt;list.txt> [/out &lt;out.DIR>]
''' ```
''' String similarity match of the fasta title with given terms for search and export by taxonomy.
''' </summary>
'''
Public Function FilterExports([in] As String, tax As String, gi2taxid As String, words As String, Optional out As String = "") As Integer
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
''' ```
''' /gbff.union /in &lt;data.directory> [/out &lt;output.union.gb>]
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
''' ```
''' /gi.Match /in &lt;nt.parts.fasta/list.txt> /gi2taxid &lt;gi2taxid.dmp> [/out &lt;gi_match.txt>]
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
''' ```
''' /gi.Matchs /in &lt;nt.parts.fasta.DIR> /gi2taxid &lt;gi2taxid.dmp> [/out &lt;gi_match.txt.DIR> /num_threads &lt;-1>]
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
''' ```
''' /gpff.fasta /in &lt;gpff.txt> [/out &lt;out.fasta>]
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
''' ```
''' /MapHits.list /in &lt;in.csv> [/out &lt;out.txt>]
''' ```
''' </summary>
'''
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
''' ```
''' /nt.matches.accession /in &lt;nt.fasta> /list &lt;accession.list> [/accid &lt;default="tokens '.' first"> /out &lt;subset.fasta>]
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
''' ```
''' /nt.matches.key /in &lt;nt.fasta> /list &lt;words.txt> [/out &lt;out.fasta>]
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
''' ```
''' /nt.matches.name /in &lt;nt.fasta> /list &lt;names.csv> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
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
''' ```
''' /Nt.Taxonomy /in &lt;nt.fasta> /gi2taxi &lt;gi2taxi.bin> /tax &lt;ncbi_taxonomy:names,nodes> [/out &lt;out.fasta>]
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
''' ```
''' /OTU.associated /in &lt;OTU.Data> /maps &lt;mapsHit.csv> [/RawMap &lt;data_mapping.csv> /OTU_Field &lt;"#OTU_NUM"> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function OTUAssociated([in] As String, maps As String, Optional rawmap As String = "", Optional otu_field As String = "", Optional out As String = "") As Integer
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
''' ```
''' /OTU.diff /ref &lt;OTU.Data1.csv> /parts &lt;OTU.Data2.csv> [/out &lt;out.csv>]
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
''' ```
''' /OTU.Taxonomy /in &lt;OTU.Data> /maps &lt;mapsHit.csv> /tax &lt;taxonomy:nodes/names> [/out &lt;out.csv>]
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
''' ```
''' /OTU.Taxonomy.Replace /in &lt;otu.table.csv> /maps &lt;maphits.csv> [/out &lt;out.csv>]
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
''' ```
''' /Search.Taxonomy /in &lt;list.txt/expression.csv> /ncbi_taxonomy &lt;taxnonmy:name/nodes.dmp> [/top 10 /expression /cut 0.65 /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function SearchTaxonomy([in] As String, ncbi_taxonomy As String, Optional top As String = "", Optional cut As String = "", Optional out As String = "", Optional expression As Boolean = False) As Integer
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
''' ```
''' /Split.By.Taxid /in &lt;nt.fasta> [/gi2taxid &lt;gi2taxid.txt> /out &lt;outDIR>]
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
''' ```
''' /Split.By.Taxid.Batch /in &lt;nt.fasta.DIR> [/num_threads &lt;-1> /out &lt;outDIR>]
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
''' ```
''' /Taxonomy.Data /data &lt;data.csv> /field.gi &lt;GI> /gi2taxid &lt;gi2taxid.list.txt> /tax &lt;ncbi_taxonomy:nodes/names> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function TaxonomyTreeData(data As String, field_gi As String, gi2taxid As String, tax As String, Optional out As String = "") As Integer
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
''' ```
''' /Taxonomy.Maphits.Overview /in &lt;in.DIR> [/out &lt;out.csv>]
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
''' ```
''' /Taxonomy.Tree /taxid &lt;taxid.list.txt> /tax &lt;ncbi_taxonomy:nodes/names> [/out &lt;out.csv>]
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
''' ```
''' /word.tokens /in &lt;list.txt> [/out &lt;out.csv>]
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
