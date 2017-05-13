Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/NCBI_tools.exe

Namespace GCModellerApps


''' <summary>
''' Tools collection for handling NCBI data, includes: nt/nr database, NCBI taxonomy analysis, OTU taxonomy analysis, genbank database, and sequence query tools.
''' </summary>
'''
Public Class NCBI_tools : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
''' ```
''' /accid2taxid.Match /in &lt;nt.parts.fasta/list.txt> /acc2taxid &lt;acc2taxid.dmp/DIR> [/gb_priority /out &lt;acc2taxid_match.txt>]
''' ```
''' </summary>
'''
Public Function accidMatch(_in As String, _acc2taxid As String, Optional _out As String = "", Optional _gb_priority As Boolean = False) As Integer
Dim CLI As New StringBuilder("accidMatch")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/acc2taxid " & """" & _acc2taxid & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _gb_priority Then
Call CLI.Append("/gb_priority ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Assign.Taxonomy /in &lt;in.DIR> /gi &lt;regexp> /index &lt;fieldName> /tax &lt;NCBI nodes/names.dmp> /gi2taxi &lt;gi2taxi.txt/bin> [/out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function AssignTaxonomy(_in As String, _gi As String, _index As String, _tax As String, _gi2taxi As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("AssignTaxonomy")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/gi " & """" & _gi & """ ")
Call CLI.Append("/index " & """" & _index & """ ")
Call CLI.Append("/tax " & """" & _tax & """ ")
Call CLI.Append("/gi2taxi " & """" & _gi2taxi & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Assign.Taxonomy.From.Ref /in &lt;in.DIR> /ref &lt;nt.taxonomy.fasta> [/index &lt;Name> /non-BIOM /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function AssignTaxonomyFromRef(_in As String, _ref As String, Optional _index As String = "", Optional _out As String = "", Optional _non_biom As Boolean = False) As Integer
Dim CLI As New StringBuilder("AssignTaxonomyFromRef")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/ref " & """" & _ref & """ ")
If Not _index.StringEmpty Then
Call CLI.Append("/index " & """" & _index & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _non_biom Then
Call CLI.Append("/non-biom ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Assign.Taxonomy.SSU /in &lt;in.DIR> /index &lt;fieldName> /ref &lt;SSU-ref.fasta> [/out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function AssignTaxonomy2(_in As String, _index As String, _ref As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("AssignTaxonomy2")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/index " & """" & _index & """ ")
Call CLI.Append("/ref " & """" & _ref & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Associate.Taxonomy /in &lt;in.DIR> /tax &lt;ncbi_taxonomy:names,nodes> /gi2taxi &lt;gi2taxi.bin> [/gi &lt;nt.gi.csv> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function AssociateTaxonomy(_in As String, _tax As String, _gi2taxi As String, Optional _gi As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("AssociateTaxonomy")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/tax " & """" & _tax & """ ")
Call CLI.Append("/gi2taxi " & """" & _gi2taxi & """ ")
If Not _gi.StringEmpty Then
Call CLI.Append("/gi " & """" & _gi & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Associates.Brief /in &lt;in.DIR> /ls &lt;ls.txt> [/index &lt;Name> /out &lt;out.tsv>]
''' ```
''' </summary>
'''
Public Function Associates(_in As String, _ls As String, Optional _index As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("Associates")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/ls " & """" & _ls & """ ")
If Not _index.StringEmpty Then
Call CLI.Append("/index " & """" & _index & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build_gi2taxi /in &lt;gi2taxi.dmp> [/out &lt;out.dat>]
''' ```
''' </summary>
'''
Public Function Build_gi2taxi(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("Build_gi2taxi")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.GI /in &lt;ncbi:nt.fasta> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ExportGI(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("ExportGI")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


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
Public Function FilterExports(_in As String, _tax As String, _gi2taxid As String, _words As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("FilterExports")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/tax " & """" & _tax & """ ")
Call CLI.Append("/gi2taxid " & """" & _gi2taxid & """ ")
Call CLI.Append("/words " & """" & _words & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /gi.Match /in &lt;nt.parts.fasta/list.txt> /gi2taxid &lt;gi2taxid.dmp> [/out &lt;gi_match.txt>]
''' ```
''' </summary>
'''
Public Function giMatch(_in As String, _gi2taxid As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("giMatch")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/gi2taxid " & """" & _gi2taxid & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /gi.Matchs /in &lt;nt.parts.fasta.DIR> /gi2taxid &lt;gi2taxid.dmp> [/out &lt;gi_match.txt.DIR> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function giMatchs(_in As String, _gi2taxid As String, Optional _out As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("giMatchs")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/gi2taxid " & """" & _gi2taxid & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /MapHits.list /in &lt;in.csv> [/out &lt;out.txt>]
''' ```
''' </summary>
'''
Public Function GetMapHitsList(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("GetMapHitsList")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /nt.matches.key /in &lt;nt.fasta> /list &lt;words.txt> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function NtKeyMatches(_in As String, _list As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("NtKeyMatches")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/list " & """" & _list & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /nt.matches.name /in &lt;nt.fasta> /list &lt;names.csv> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function NtNameMatches(_in As String, _list As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("NtNameMatches")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/list " & """" & _list & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Nt.Taxonomy /in &lt;nt.fasta> /gi2taxi &lt;gi2taxi.bin> /tax &lt;ncbi_taxonomy:names,nodes> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function NtTaxonomy(_in As String, _gi2taxi As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("NtTaxonomy")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/gi2taxi " & """" & _gi2taxi & """ ")
Call CLI.Append("/tax " & """" & _tax & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /OTU.associated /in &lt;OTU.Data> /maps &lt;mapsHit.csv> [/RawMap &lt;data_mapping.csv> /OTU_Field &lt;"#OTU_NUM"> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function OTUAssociated(_in As String, _maps As String, Optional _rawmap As String = "", Optional _otu_field As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("OTUAssociated")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/maps " & """" & _maps & """ ")
If Not _rawmap.StringEmpty Then
Call CLI.Append("/rawmap " & """" & _rawmap & """ ")
End If
If Not _otu_field.StringEmpty Then
Call CLI.Append("/otu_field " & """" & _otu_field & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /OTU.diff /ref &lt;OTU.Data1.csv> /parts &lt;OTU.Data2.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function OTUDiff(_ref As String, _parts As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("OTUDiff")
Call CLI.Append("/ref " & """" & _ref & """ ")
Call CLI.Append("/parts " & """" & _parts & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /OTU.Taxonomy /in &lt;OTU.Data> /maps &lt;mapsHit.csv> /tax &lt;taxonomy:nodes/names> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function OTU_Taxonomy(_in As String, _maps As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("OTU_Taxonomy")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/maps " & """" & _maps & """ ")
Call CLI.Append("/tax " & """" & _tax & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


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
Public Function OTUTaxonomyReplace(_in As String, _maps As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("OTUTaxonomyReplace")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/maps " & """" & _maps & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Search.Taxonomy /in &lt;list.txt/expression.csv> /ncbi_taxonomy &lt;taxnonmy:name/nodes.dmp> [/top 10 /expression /cut 0.65 /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function SearchTaxonomy(_in As String, _ncbi_taxonomy As String, Optional _top As String = "", Optional _cut As String = "", Optional _out As String = "", Optional _expression As Boolean = False) As Integer
Dim CLI As New StringBuilder("SearchTaxonomy")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/ncbi_taxonomy " & """" & _ncbi_taxonomy & """ ")
If Not _top.StringEmpty Then
Call CLI.Append("/top " & """" & _top & """ ")
End If
If Not _cut.StringEmpty Then
Call CLI.Append("/cut " & """" & _cut & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _expression Then
Call CLI.Append("/expression ")
End If


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
Public Function SplitByTaxid(_in As String, Optional _gi2taxid As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("SplitByTaxid")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _gi2taxid.StringEmpty Then
Call CLI.Append("/gi2taxid " & """" & _gi2taxid & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Split.By.Taxid.Batch /in &lt;nt.fasta.DIR> [/num_threads &lt;-1> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function SplitByTaxidBatch(_in As String, Optional _num_threads As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("SplitByTaxidBatch")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Taxonomy.Data /data &lt;data.csv> /field.gi &lt;GI> /gi2taxid &lt;gi2taxid.list.txt> /tax &lt;ncbi_taxonomy:nodes/names> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function TaxonomyTreeData(_data As String, _field_gi As String, _gi2taxid As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("TaxonomyTreeData")
Call CLI.Append("/data " & """" & _data & """ ")
Call CLI.Append("/field.gi " & """" & _field_gi & """ ")
Call CLI.Append("/gi2taxid " & """" & _gi2taxid & """ ")
Call CLI.Append("/tax " & """" & _tax & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Taxonomy.Maphits.Overview /in &lt;in.DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function TaxidMapHitViews(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("TaxidMapHitViews")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


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
Public Function TaxonomyTree(_taxid As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("TaxonomyTree")
Call CLI.Append("/taxid " & """" & _taxid & """ ")
Call CLI.Append("/tax " & """" & _tax & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /word.tokens /in &lt;list.txt> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function GetWordTokens(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("GetWordTokens")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
