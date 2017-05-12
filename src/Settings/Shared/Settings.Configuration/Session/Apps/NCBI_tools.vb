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
Dim CLI$ = $"/accid2taxid.Match /in ""{_in}"" /acc2taxid ""{_acc2taxid}"" /out ""{_out}"" {If(_gb_priority, "/gb_priority", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Assign_Taxonomy(_in As String, _gi As String, _index As String, _tax As String, _gi2taxi As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Assign.Taxonomy /in ""{_in}"" /gi ""{_gi}"" /index ""{_index}"" /tax ""{_tax}"" /gi2taxi ""{_gi2taxi}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Assign_Taxonomy_From_Ref(_in As String, _ref As String, Optional _index As String = "", Optional _out As String = "", Optional _non_biom As Boolean = False) As Integer
Dim CLI$ = $"/Assign.Taxonomy.From.Ref /in ""{_in}"" /ref ""{_ref}"" /index ""{_index}"" /out ""{_out}"" {If(_non_biom, "/non-biom", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Assign_Taxonomy_SSU(_in As String, _index As String, _ref As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Assign.Taxonomy.SSU /in ""{_in}"" /index ""{_index}"" /ref ""{_ref}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Associate_Taxonomy(_in As String, _tax As String, _gi2taxi As String, Optional _gi As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Associate.Taxonomy /in ""{_in}"" /tax ""{_tax}"" /gi2taxi ""{_gi2taxi}"" /gi ""{_gi}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Associates_Brief(_in As String, _ls As String, Optional _index As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Associates.Brief /in ""{_in}"" /ls ""{_ls}"" /index ""{_index}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_gi2taxi(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Build_gi2taxi /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Export_GI(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Export.GI /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''String similarity match of the fasta title with given terms for search and export by taxonomy.
''' </summary>
'''
Public Function Filter_Exports(_in As String, _tax As String, _gi2taxid As String, _words As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Filter.Exports /in ""{_in}"" /tax ""{_tax}"" /gi2taxid ""{_gi2taxid}"" /words ""{_words}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function gi_Match(_in As String, _gi2taxid As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/gi.Match /in ""{_in}"" /gi2taxid ""{_gi2taxid}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function gi_Matchs(_in As String, _gi2taxid As String, Optional _out As String = "", Optional _num_threads As String = "") As Integer
Dim CLI$ = $"/gi.Matchs /in ""{_in}"" /gi2taxid ""{_gi2taxid}"" /out ""{_out}"" /num_threads ""{_num_threads}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function MapHits_list(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/MapHits.list /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function nt_matches_key(_in As String, _list As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/nt.matches.key /in ""{_in}"" /list ""{_list}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function nt_matches_name(_in As String, _list As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/nt.matches.name /in ""{_in}"" /list ""{_list}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Nt_Taxonomy(_in As String, _gi2taxi As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Nt.Taxonomy /in ""{_in}"" /gi2taxi ""{_gi2taxi}"" /tax ""{_tax}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function OTU_associated(_in As String, _maps As String, Optional _rawmap As String = "", Optional _otu_field As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/OTU.associated /in ""{_in}"" /maps ""{_maps}"" /rawmap ""{_rawmap}"" /otu_field ""{_otu_field}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function OTU_diff(_ref As String, _parts As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/OTU.diff /ref ""{_ref}"" /parts ""{_parts}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function OTU_Taxonomy(_in As String, _maps As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/OTU.Taxonomy /in ""{_in}"" /maps ""{_maps}"" /tax ""{_tax}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Using ``MapHits`` property
''' </summary>
'''
Public Function OTU_Taxonomy_Replace(_in As String, _maps As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/OTU.Taxonomy.Replace /in ""{_in}"" /maps ""{_maps}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Search_Taxonomy(_in As String, _ncbi_taxonomy As String, Optional _top As String = "", Optional _cut As String = "", Optional _out As String = "", Optional _expression As Boolean = False) As Integer
Dim CLI$ = $"/Search.Taxonomy /in ""{_in}"" /ncbi_taxonomy ""{_ncbi_taxonomy}"" /top ""{_top}"" /cut ""{_cut}"" /out ""{_out}"" {If(_expression, "/expression", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Split the input fasta file by taxid grouping.
''' </summary>
'''
Public Function Split_By_Taxid(_in As String, Optional _gi2taxid As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Split.By.Taxid /in ""{_in}"" /gi2taxid ""{_gi2taxid}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Split_By_Taxid_Batch(_in As String, Optional _num_threads As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Split.By.Taxid.Batch /in ""{_in}"" /num_threads ""{_num_threads}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Taxonomy_Data(_data As String, _field_gi As String, _gi2taxid As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Taxonomy.Data /data ""{_data}"" /field.gi ""{_field_gi}"" /gi2taxid ""{_gi2taxid}"" /tax ""{_tax}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Taxonomy_Maphits_Overview(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Taxonomy.Maphits.Overview /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Output taxonomy query info by a given NCBI taxid list.
''' </summary>
'''
Public Function Taxonomy_Tree(_taxid As String, _tax As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Taxonomy.Tree /taxid ""{_taxid}"" /tax ""{_tax}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function word_tokens(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/word.tokens /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function
End Class
End Namespace
