Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/seqtools.exe

Namespace GCModellerApps


''' <summary>
'''Sequence operation utilities
''' </summary>
'''
Public Class seqtools : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
'''
''' </summary>
'''
Public Function align_SmithWaterman(_query As String, _subject As String, Optional _blosum As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/align.SmithWaterman")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
If Not _blosum.StringEmpty Then
Call CLI.Append("/blosum " & """" & _blosum & """ ")
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
Public Function CAI(_ORF As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/CAI")
Call CLI.Append("/ORF " & """" & _ORF & """ ")
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
Public Function check_attrs(_in As String, _n As String, Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/check.attrs")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/n " & """" & _n & """ ")
If _all Then
Call CLI.Append("/all ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Clustal_Cut(_in As String, Optional _left As String = "", Optional _right As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Clustal.Cut")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _left.StringEmpty Then
Call CLI.Append("/left " & """" & _left & """ ")
End If
If Not _right.StringEmpty Then
Call CLI.Append("/right " & """" & _right & """ ")
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
Public Function Compare_By_Locis(_file1 As String, _file2 As String) As Integer
Dim CLI As New StringBuilder("/Compare.By.Locis")
Call CLI.Append("/file1 " & """" & _file1 & """ ")
Call CLI.Append("/file2 " & """" & _file2 & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Count(_in As String) As Integer
Dim CLI As New StringBuilder("/Count")
Call CLI.Append("/in " & """" & _in & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Distinct fasta sequence by sequence content.
''' </summary>
'''
Public Function Distinct(_in As String, Optional _out As String = "", Optional _by_uid As String = "") As Integer
Dim CLI As New StringBuilder("/Distinct")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _by_uid.StringEmpty Then
Call CLI.Append("/by_uid " & """" & _by_uid & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Convert the sequence data in a excel annotation file into a fasta sequence file.
''' </summary>
'''
Public Function Excel_2Fasta(_in As String, Optional _out As String = "", Optional _attrs As String = "", Optional _seq As String = "") As Integer
Dim CLI As New StringBuilder("/Excel.2Fasta")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _attrs.StringEmpty Then
Call CLI.Append("/attrs " & """" & _attrs & """ ")
End If
If Not _seq.StringEmpty Then
Call CLI.Append("/seq " & """" & _seq & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Get_Locis(_in As String, _nt As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Get.Locis")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/nt " & """" & _nt & """ ")
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
Public Function Gff_Sites(_fna As String, _gff As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Gff.Sites")
Call CLI.Append("/fna " & """" & _fna & """ ")
Call CLI.Append("/gff " & """" & _gff & """ ")
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
Public Function gwANI(_in As String, Optional _out As String = "", Optional _fast As Boolean = False) As Integer
Dim CLI As New StringBuilder("/gwANI")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _fast Then
Call CLI.Append("/fast ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Testing
''' </summary>
'''
Public Function Loci_describ(_ptt As String, Optional _test As String = "", Optional _complement As Boolean = False, Optional _unstrand As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Loci.describ")
Call CLI.Append("/ptt " & """" & _ptt & """ ")
If Not _test.StringEmpty Then
Call CLI.Append("/test " & """" & _test & """ ")
End If
If _complement Then
Call CLI.Append("/complement ")
End If
If _unstrand Then
Call CLI.Append("/unstrand ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''* Drawing the sequence logo from the clustal alignment result.
''' </summary>
'''
Public Function logo(_in As String, Optional _out As String = "", Optional _title As String = "") As Integer
Dim CLI As New StringBuilder("/logo")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _title.StringEmpty Then
Call CLI.Append("/title " & """" & _title & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Only search for 1 level folder, dit not search receve.
''' </summary>
'''
Public Function Merge(_in As String, Optional _out As String = "", Optional _ext As String = "", Optional _trim As Boolean = False, Optional _unique As Boolean = False, Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Merge")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _ext.StringEmpty Then
Call CLI.Append("/ext " & """" & _ext & """ ")
End If
If _trim Then
Call CLI.Append("/trim ")
End If
If _unique Then
Call CLI.Append("/unique ")
End If
If _brief Then
Call CLI.Append("/brief ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''This tools just merge the fasta sequence into one larger file.
''' </summary>
'''
Public Function Merge_Simple(_in As String, Optional _exts As String = "", Optional _line_break As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Merge.Simple")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _exts.StringEmpty Then
Call CLI.Append("/exts " & """" & _exts & """ ")
End If
If Not _line_break.StringEmpty Then
Call CLI.Append("/line.break " & """" & _line_break & """ ")
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
Public Function Mirror_Batch(_nt As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _num_threads As String = "", Optional _mp As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Mirror.Batch")
Call CLI.Append("/nt " & """" & _nt & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If _mp Then
Call CLI.Append("/mp ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Mirror_Fuzzy(_in As String, Optional _out As String = "", Optional _cut As String = "", Optional _max_dist As String = "", Optional _min As String = "", Optional _max As String = "") As Integer
Dim CLI As New StringBuilder("/Mirror.Fuzzy")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cut.StringEmpty Then
Call CLI.Append("/cut " & """" & _cut & """ ")
End If
If Not _max_dist.StringEmpty Then
Call CLI.Append("/max-dist " & """" & _max_dist & """ ")
End If
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Mirror_Fuzzy_Batch(_in As String, Optional _out As String = "", Optional _cut As String = "", Optional _max_dist As String = "", Optional _min As String = "", Optional _max As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("/Mirror.Fuzzy.Batch")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cut.StringEmpty Then
Call CLI.Append("/cut " & """" & _cut & """ ")
End If
If Not _max_dist.StringEmpty Then
Call CLI.Append("/max-dist " & """" & _max_dist & """ ")
End If
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
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
Public Function Mirror_Vector(_in As String, _size As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Mirror.Vector")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/size " & """" & _size & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''This function will convert the mirror data to the simple segment object data
''' </summary>
'''
Public Function Mirrors_Context(_in As String, _PTT As String, Optional _strand As String = "", Optional _out As String = "", Optional _dist As String = "", Optional _trans As Boolean = False, Optional _stranded As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Mirrors.Context")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
If Not _strand.StringEmpty Then
Call CLI.Append("/strand " & """" & _strand & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _dist.StringEmpty Then
Call CLI.Append("/dist " & """" & _dist & """ ")
End If
If _trans Then
Call CLI.Append("/trans ")
End If
If _stranded Then
Call CLI.Append("/stranded ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''This function will convert the mirror data to the simple segment object data
''' </summary>
'''
Public Function Mirrors_Context_Batch(_in As String, _PTT As String, Optional _strand As String = "", Optional _out As String = "", Optional _dist As String = "", Optional _num_threads As String = "", Optional _trans As Boolean = False, Optional _stranded As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Mirrors.Context.Batch")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
If Not _strand.StringEmpty Then
Call CLI.Append("/strand " & """" & _strand & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _dist.StringEmpty Then
Call CLI.Append("/dist " & """" & _dist & """ ")
End If
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If _trans Then
Call CLI.Append("/trans ")
End If
If _stranded Then
Call CLI.Append("/stranded ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Mirrors_Group(_in As String, Optional _fuzzy As String = "", Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Mirrors.Group")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _fuzzy.StringEmpty Then
Call CLI.Append("/fuzzy " & """" & _fuzzy & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _batch Then
Call CLI.Append("/batch ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Mirrors_Group_Batch(_in As String, Optional _fuzzy As String = "", Optional _out As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("/Mirrors.Group.Batch")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _fuzzy.StringEmpty Then
Call CLI.Append("/fuzzy " & """" & _fuzzy & """ ")
End If
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
Public Function Mirrors_Nt_Trim(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Mirrors.Nt.Trim")
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
Public Function NeedlemanWunsch_NT(_query As String, _subject As String) As Integer
Dim CLI As New StringBuilder("/NeedlemanWunsch.NT")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''RunNeedlemanWunsch
''' </summary>
'''
Public Function nw(_query As String, _subject As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/nw")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
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
Public Function Palindrome_BatchTask(_in As String, Optional _num_threads As String = "", Optional _min As String = "", Optional _max As String = "", Optional _min_appears As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _partitions As String = "", Optional _out As String = "", Optional _palindrome As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Palindrome.BatchTask")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If
If Not _min_appears.StringEmpty Then
Call CLI.Append("/min-appears " & """" & _min_appears & """ ")
End If
If Not _cutoff.StringEmpty Then
Call CLI.Append("/cutoff " & """" & _cutoff & """ ")
End If
If Not _max_dist.StringEmpty Then
Call CLI.Append("/max-dist " & """" & _max_dist & """ ")
End If
If Not _partitions.StringEmpty Then
Call CLI.Append("/partitions " & """" & _partitions & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _palindrome Then
Call CLI.Append("/palindrome ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Palindrome_Screen_MaxMatches(_in As String, _min As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Palindrome.Screen.MaxMatches")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/min " & """" & _min & """ ")
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
Public Function Palindrome_Screen_MaxMatches_Batch(_in As String, _min As String, Optional _out As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("/Palindrome.Screen.MaxMatches.Batch")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/min " & """" & _min & """ ")
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
Public Function Palindrome_Workflow(_in As String, Optional _min_appears As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _partitions As String = "", Optional _out As String = "", Optional _batch As Boolean = False, Optional _palindrome As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Palindrome.Workflow")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _min_appears.StringEmpty Then
Call CLI.Append("/min-appears " & """" & _min_appears & """ ")
End If
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If
If Not _cutoff.StringEmpty Then
Call CLI.Append("/cutoff " & """" & _cutoff & """ ")
End If
If Not _max_dist.StringEmpty Then
Call CLI.Append("/max-dist " & """" & _max_dist & """ ")
End If
If Not _partitions.StringEmpty Then
Call CLI.Append("/partitions " & """" & _partitions & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _batch Then
Call CLI.Append("/batch ")
End If
If _palindrome Then
Call CLI.Append("/palindrome ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Promoter_Palindrome_Fasta(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Promoter.Palindrome.Fasta")
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
Public Function Promoter_Regions_Palindrome(_in As String, Optional _min As String = "", Optional _max As String = "", Optional _len As String = "", Optional _out As String = "", Optional _mirror As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Promoter.Regions.Palindrome")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If
If Not _len.StringEmpty Then
Call CLI.Append("/len " & """" & _len & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _mirror Then
Call CLI.Append("/mirror ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Promoter_Regions_Parser_gb(_gb As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Promoter.Regions.Parser.gb")
Call CLI.Append("/gb " & """" & _gb & """ ")
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
Public Function Rule_dnaA_gyrB(_genome As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Rule.dnaA_gyrB")
Call CLI.Append("/genome " & """" & _genome & """ ")
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
Public Function Rule_dnaA_gyrB_Matrix(_genomes As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Rule.dnaA_gyrB.Matrix")
Call CLI.Append("/genomes " & """" & _genomes & """ ")
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
Public Function Screen_sites(_in As String, _range As String, Optional _type As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Screen.sites")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/range " & """" & _range & """ ")
If Not _type.StringEmpty Then
Call CLI.Append("/type " & """" & _type & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Select fasta sequence by local_tag.
''' </summary>
'''
Public Function Select_By_Locus(_in As String, _fa As String, Optional _field As String = "", Optional _out As String = "", Optional _reverse As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Select.By_Locus")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/fa " & """" & _fa & """ ")
If Not _field.StringEmpty Then
Call CLI.Append("/field " & """" & _field & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _reverse Then
Call CLI.Append("/reverse ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Sigma(_in As String, Optional _out As String = "", Optional _round As String = "", Optional _simple As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Sigma")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _round.StringEmpty Then
Call CLI.Append("/round " & """" & _round & """ ")
End If
If _simple Then
Call CLI.Append("/simple ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SimpleSegment_AutoBuild(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/SimpleSegment.AutoBuild")
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
Public Function SimpleSegment_Mirrors(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/SimpleSegment.Mirrors")
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
Public Function SimpleSegment_Mirrors_Batch(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/SimpleSegment.Mirrors.Batch")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Converts the simple segment object collection as fasta file.
''' </summary>
'''
Public Function Sites2Fasta(_in As String, Optional _out As String = "", Optional _assemble As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Sites2Fasta")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _assemble Then
Call CLI.Append("/assemble ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SNP(_in As String, Optional _ref As String = "", Optional _high As String = "", Optional _pure As Boolean = False, Optional _monomorphic As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SNP")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _ref.StringEmpty Then
Call CLI.Append("/ref " & """" & _ref & """ ")
End If
If Not _high.StringEmpty Then
Call CLI.Append("/high " & """" & _high & """ ")
End If
If _pure Then
Call CLI.Append("/pure ")
End If
If _monomorphic Then
Call CLI.Append("/monomorphic ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Split(_in As String, Optional _n As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Split")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _n.StringEmpty Then
Call CLI.Append("/n " & """" & _n & """ ")
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
Public Function subset(_lstID As String, _fa As String) As Integer
Dim CLI As New StringBuilder("/subset")
Call CLI.Append("/lstID " & """" & _lstID & """ ")
Call CLI.Append("/fa " & """" & _fa & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''The ongoing time mutation of the genome sequence.
''' </summary>
'''
Public Function Time_Mutation(_in As String, Optional _ref As String = "", Optional _out As String = "", Optional _cumulative As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Time.Mutation")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _ref.StringEmpty Then
Call CLI.Append("/ref " & """" & _ref & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _cumulative Then
Call CLI.Append("/cumulative ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Write_Seeds(_out As String, Optional _max As String = "", Optional _prot As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Write.Seeds")
Call CLI.Append("/out " & """" & _out & """ ")
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If
If _prot Then
Call CLI.Append("/prot ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Polypeptide sequence 3 letters to 1 lettes sequence.
''' </summary>
'''
Public Function _321(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("-321")
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
Public Function align(_query As String, _subject As String, Optional _out As String = "", Optional _cost As String = "") As Integer
Dim CLI As New StringBuilder("--align")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cost.StringEmpty Then
Call CLI.Append("/cost " & """" & _cost & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function align_Self(_query As String, _out As String, Optional _cost As String = "") As Integer
Dim CLI As New StringBuilder("--align.Self")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If Not _cost.StringEmpty Then
Call CLI.Append("/cost " & """" & _cost & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function complement(_i As String, Optional _o As String = "") As Integer
Dim CLI As New StringBuilder("-complement")
Call CLI.Append("-i " & """" & _i & """ ")
If Not _o.StringEmpty Then
Call CLI.Append("-o " & """" & _o & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Drawing_ClustalW(_in As String, Optional _out As String = "", Optional _dot_size As String = "") As Integer
Dim CLI As New StringBuilder("--Drawing.ClustalW")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _dot_size.StringEmpty Then
Call CLI.Append("/dot.size " & """" & _dot_size & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Hairpinks(_in As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "") As Integer
Dim CLI As New StringBuilder("--Hairpinks")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If
If Not _cutoff.StringEmpty Then
Call CLI.Append("/cutoff " & """" & _cutoff & """ ")
End If
If Not _max_dist.StringEmpty Then
Call CLI.Append("/max-dist " & """" & _max_dist & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Hairpinks_batch_task(_in As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("--Hairpinks.batch.task")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If
If Not _cutoff.StringEmpty Then
Call CLI.Append("/cutoff " & """" & _cutoff & """ ")
End If
If Not _max_dist.StringEmpty Then
Call CLI.Append("/max-dist " & """" & _max_dist & """ ")
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
Public Function ImperfectsPalindrome_batch_Task(_in As String, _out As String, Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("--ImperfectsPalindrome.batch.Task")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If
If Not _cutoff.StringEmpty Then
Call CLI.Append("/cutoff " & """" & _cutoff & """ ")
End If
If Not _max_dist.StringEmpty Then
Call CLI.Append("/max-dist " & """" & _max_dist & """ ")
End If
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Mirror Palindrome, search from a fasta file.
''' </summary>
'''
Public Function Mirror_From_Fasta(_nt As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "") As Integer
Dim CLI As New StringBuilder("--Mirror.From.Fasta")
Call CLI.Append("/nt " & """" & _nt & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Mirror Palindrome, and this function is for the debugging test
''' </summary>
'''
Public Function Mirror_From_NT(_nt As String, _out As String, Optional _min As String = "", Optional _max As String = "") As Integer
Dim CLI As New StringBuilder("--Mirror.From.NT")
Call CLI.Append("/nt " & """" & _nt & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Palindrome_batch_Task(_in As String, _out As String, Optional _min As String = "", Optional _max As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("--Palindrome.batch.Task")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
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
Public Function Palindrome_From_FASTA(_nt As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "") As Integer
Dim CLI As New StringBuilder("--Palindrome.From.FASTA")
Call CLI.Append("/nt " & """" & _nt & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''This function is just for debugger test, /nt parameter is the nucleotide sequence data as ATGCCCC
''' </summary>
'''
Public Function Palindrome_From_NT(_nt As String, _out As String, Optional _min As String = "", Optional _max As String = "") As Integer
Dim CLI As New StringBuilder("--Palindrome.From.NT")
Call CLI.Append("/nt " & """" & _nt & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Palindrome_Imperfects(_in As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _partitions As String = "") As Integer
Dim CLI As New StringBuilder("--Palindrome.Imperfects")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If
If Not _cutoff.StringEmpty Then
Call CLI.Append("/cutoff " & """" & _cutoff & """ ")
End If
If Not _max_dist.StringEmpty Then
Call CLI.Append("/max-dist " & """" & _max_dist & """ ")
End If
If Not _partitions.StringEmpty Then
Call CLI.Append("/partitions " & """" & _partitions & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Parsing the sequence segment from the sequence source using regular expression.
''' </summary>
'''
Public Function pattern_search(_i As String, _p As String, Optional _o As String = "", Optional _f As String = "") As Integer
Dim CLI As New StringBuilder("-pattern_search")
Call CLI.Append("-i " & """" & _i & """ ")
Call CLI.Append("-p " & """" & _p & """ ")
If Not _o.StringEmpty Then
Call CLI.Append("-o " & """" & _o & """ ")
End If
If Not _f.StringEmpty Then
Call CLI.Append("-f " & """" & _f & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function PerfectPalindrome_Filtering(_in As String, Optional _min As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--PerfectPalindrome.Filtering")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
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
Public Function Repeats_Density(_dir As String, _size As String, _ref As String, Optional _out As String = "", Optional _cutoff As String = "") As Integer
Dim CLI As New StringBuilder("Repeats.Density")
Call CLI.Append("/dir " & """" & _dir & """ ")
Call CLI.Append("/size " & """" & _size & """ ")
Call CLI.Append("/ref " & """" & _ref & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cutoff.StringEmpty Then
Call CLI.Append("/cutoff " & """" & _cutoff & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function reverse(_i As String, Optional _o As String = "") As Integer
Dim CLI As New StringBuilder("-reverse")
Call CLI.Append("-i " & """" & _i & """ ")
If Not _o.StringEmpty Then
Call CLI.Append("-o " & """" & _o & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function rev_Repeats_Density(_dir As String, _size As String, _ref As String, Optional _out As String = "", Optional _cutoff As String = "") As Integer
Dim CLI As New StringBuilder("rev-Repeats.Density")
Call CLI.Append("/dir " & """" & _dir & """ ")
Call CLI.Append("/size " & """" & _size & """ ")
Call CLI.Append("/ref " & """" & _ref & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cutoff.StringEmpty Then
Call CLI.Append("/cutoff " & """" & _cutoff & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Batch search for repeats.
''' </summary>
'''
Public Function Search_Batch(_aln As String, Optional _min As String = "", Optional _max As String = "", Optional _min_rep As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("Search.Batch")
Call CLI.Append("/aln " & """" & _aln & """ ")
If Not _min.StringEmpty Then
Call CLI.Append("/min " & """" & _min & """ ")
End If
If Not _max.StringEmpty Then
Call CLI.Append("/max " & """" & _max & """ ")
End If
If Not _min_rep.StringEmpty Then
Call CLI.Append("/min-rep " & """" & _min_rep & """ ")
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
Public Function segment(_fasta As String, Optional _loci As String = "", Optional _length As String = "", Optional _right As String = "", Optional __reverse__ As String = "", Optional _geneid As String = "", Optional _dist As String = "", Optional _o As String = "", Optional __line_break As String = "", Optional _downstream_ As Boolean = False) As Integer
Dim CLI As New StringBuilder("-segment")
Call CLI.Append("/fasta " & """" & _fasta & """ ")
If Not _loci.StringEmpty Then
Call CLI.Append("-loci " & """" & _loci & """ ")
End If
If Not _length.StringEmpty Then
Call CLI.Append("/length " & """" & _length & """ ")
End If
If Not _right.StringEmpty Then
Call CLI.Append("/right " & """" & _right & """ ")
End If
If Not __reverse__.StringEmpty Then
Call CLI.Append("[/reverse]] " & """" & __reverse__ & """ ")
End If
If Not _geneid.StringEmpty Then
Call CLI.Append("/geneid " & """" & _geneid & """ ")
End If
If Not _dist.StringEmpty Then
Call CLI.Append("/dist " & """" & _dist & """ ")
End If
If Not _o.StringEmpty Then
Call CLI.Append("-o " & """" & _o & """ ")
End If
If Not __line_break.StringEmpty Then
Call CLI.Append("[-line.break " & """" & __line_break & """ ")
End If
If _downstream_ Then
Call CLI.Append("/downstream] ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function segments(_regions As String, _fasta As String, Optional _complement As Boolean = False, Optional _reversed As Boolean = False, Optional _brief_dump As Boolean = False) As Integer
Dim CLI As New StringBuilder("--segments")
Call CLI.Append("/regions " & """" & _regions & """ ")
Call CLI.Append("/fasta " & """" & _fasta & """ ")
If _complement Then
Call CLI.Append("/complement ")
End If
If _reversed Then
Call CLI.Append("/reversed ")
End If
If _brief_dump Then
Call CLI.Append("/brief-dump ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function ToVector(_in As String, _min As String, _max As String, _out As String, _size As String) As Integer
Dim CLI As New StringBuilder("--ToVector")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/min " & """" & _min & """ ")
Call CLI.Append("/max " & """" & _max & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
Call CLI.Append("/size " & """" & _size & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.
''' </summary>
'''
Public Function translates(_orf As String, Optional _transl_table As String = "", Optional _force As Boolean = False) As Integer
Dim CLI As New StringBuilder("--translates")
Call CLI.Append("/orf " & """" & _orf & """ ")
If Not _transl_table.StringEmpty Then
Call CLI.Append("/transl_table " & """" & _transl_table & """ ")
End If
If _force Then
Call CLI.Append("/force ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Trim(_in As String, Optional _case As String = "", Optional _break As String = "", Optional _out As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("--Trim")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _case.StringEmpty Then
Call CLI.Append("/case " & """" & _case & """ ")
End If
If Not _break.StringEmpty Then
Call CLI.Append("/break " & """" & _break & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _brief Then
Call CLI.Append("/brief ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
