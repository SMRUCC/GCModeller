Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/NCBI_tools.exe

Namespace GCModellerApps


''' <summary>
'''Tools collection for handling NCBI data, includes: nt/nr database, NCBI taxonomy analysis, OTU taxonomy analysis, genbank database, and sequence query tools.
''' </summary>
'''
Public Class NCBI_tools : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
'''
''' </summary>
'''
Public Function accid2taxid_Match(_in As String, _acc2taxid As String, Optional _out As String = "", Optional _gb_priority As Boolean = False) As Integer
Dim CLI As New StringBuilder("/accid2taxid.Match")
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
'''
''' </summary>
'''
Public Function Assign_Taxonomy(_in As String, _gi As String, _index As String, _tax As String, _gi2taxi As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Assign.Taxonomy")
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
'''
''' </summary>
'''
Public Function Assign_Taxonomy_From_Ref(_in As String, _ref As String, Optional _index As String = "", Optional _out As String = "", Optional _non_biom As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Assign.Taxonomy.From.Ref")
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
'''
''' </summary>
'''
Public Function Assign_Taxonomy_SSU(_in As String, _index As String, _ref As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Assign.Taxonomy.SSU")
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
'''
''' </summary>
'''
Public Function Associate_Taxonomy(_in As String, _tax As String, _gi2taxi As String, Optional _gi As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Associate.Taxonomy")
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
'''
''' </summary>
'''
Public Function Associates_Brief(_in As String, _ls As String, Optional _index As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Associates.Brief")
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
'''
''' </summary>
'''
Public Function Build_gi2taxi(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Build_gi2taxi")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Export_GI(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.GI")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''String similarity match of the fasta title with given terms for search and export by taxonomy.
''' </summary>
'''
Public Function Filter_Exports(_in As String, _tax As String, _gi2taxid As String, _words As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Filter.Exports")
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
'''
''' </summary>
'''
Public Function gi_Match(_in As String, _gi2taxid As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/gi.Match")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/gi2taxid " & """" & _gi2taxid & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function gi_Matchs(_in As String, _gi2taxid As String, Optional _out As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("/gi.Matchs")
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
'''
''' </summary>
'''
Public Function MapHits_list(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/MapHits.list")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function nt_matches_key(_in As String, _list As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/nt.matches.key")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/list " & """" & _list & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function nt_matches_name(_in As String, _list As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/nt.matches.name")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/list " & """" & _list & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Nt_Taxonomy(_in As String, _gi2taxi As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Nt.Taxonomy")
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
'''
''' </summary>
'''
Public Function OTU_associated(_in As String, _maps As String, Optional _rawmap As String = "", Optional _otu_field As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/OTU.associated")
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
'''
''' </summary>
'''
Public Function OTU_diff(_ref As String, _parts As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/OTU.diff")
Call CLI.Append("/ref " & """" & _ref & """ ")
Call CLI.Append("/parts " & """" & _parts & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function OTU_Taxonomy(_in As String, _maps As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/OTU.Taxonomy")
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
'''Using ``MapHits`` property
''' </summary>
'''
Public Function OTU_Taxonomy_Replace(_in As String, _maps As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/OTU.Taxonomy.Replace")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/maps " & """" & _maps & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Search_Taxonomy(_in As String, _ncbi_taxonomy As String, Optional _top As String = "", Optional _cut As String = "", Optional _out As String = "", Optional _expression As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Search.Taxonomy")
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
'''Split the input fasta file by taxid grouping.
''' </summary>
'''
Public Function Split_By_Taxid(_in As String, Optional _gi2taxid As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Split.By.Taxid")
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
'''
''' </summary>
'''
Public Function Split_By_Taxid_Batch(_in As String, Optional _num_threads As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Split.By.Taxid.Batch")
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
'''
''' </summary>
'''
Public Function Taxonomy_Data(_data As String, _field_gi As String, _gi2taxid As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Taxonomy.Data")
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
'''
''' </summary>
'''
Public Function Taxonomy_Maphits_Overview(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Taxonomy.Maphits.Overview")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Output taxonomy query info by a given NCBI taxid list.
''' </summary>
'''
Public Function Taxonomy_Tree(_taxid As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Taxonomy.Tree")
Call CLI.Append("/taxid " & """" & _taxid & """ ")
Call CLI.Append("/tax " & """" & _tax & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function word_tokens(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/word.tokens")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
