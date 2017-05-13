Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/RegPrecise.exe

Namespace GCModellerApps


''' <summary>
'''
''' </summary>
'''
Public Class RegPrecise : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
'''If the /regprecise parameter is not presented, then you should install the regprecise in the GCModeller database repostiory first.
''' </summary>
'''
Public Function Build_Operons(_bbh As String, _PTT As String, _TF_bbh As String, Optional _out As String = "", Optional _regprecise As String = "", Optional _tfhit_hash As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Build.Operons")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
Call CLI.Append("/TF-bbh " & """" & _TF_bbh & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _regprecise.StringEmpty Then
Call CLI.Append("/regprecise " & """" & _regprecise & """ ")
End If
If _tfhit_hash Then
Call CLI.Append("/tfhit_hash ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Regulons_Batch(_bbh As String, _PTT As String, _tf_bbh As String, _regprecise As String, Optional _num_threads As String = "", Optional _out As String = "", Optional _hits_hash As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Build.Regulons.Batch")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
Call CLI.Append("/tf-bbh " & """" & _tf_bbh & """ ")
Call CLI.Append("/regprecise " & """" & _regprecise & """ ")
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _hits_hash Then
Call CLI.Append("/hits_hash ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Join two vertices by edge if the correspondent operons: 
'''               i) are orthologous; 
'''               ii) have cantiodate transcription factor binding sites. 
'''               Collect all linked components. Two operons from two different genomes are called orthologous if they share at least one orthologous gene.
''' </summary>
'''
Public Function CORN(_in As String, _motif_sites As String, _sites As String, _ref As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/CORN")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/motif-sites " & """" & _motif_sites & """ ")
Call CLI.Append("/sites " & """" & _sites & """ ")
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
Public Function CORN_Batch(_sites As String, _regulons As String, Optional _name As String = "", Optional _out As String = "", Optional _num_threads As String = "", Optional _null_regprecise As Boolean = False) As Integer
Dim CLI As New StringBuilder("/CORN.Batch")
Call CLI.Append("/sites " & """" & _sites & """ ")
Call CLI.Append("/regulons " & """" & _regulons & """ ")
If Not _name.StringEmpty Then
Call CLI.Append("/name " & """" & _name & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If _null_regprecise Then
Call CLI.Append("/null-regprecise ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function CORN_thread(_hit As String, _hit_sites As String, _sites As String, _ref As String, Optional _out As String = "", Optional _null_regprecise As Boolean = False) As Integer
Dim CLI As New StringBuilder("/CORN.thread")
Call CLI.Append("/hit " & """" & _hit & """ ")
Call CLI.Append("/hit-sites " & """" & _hit_sites & """ ")
Call CLI.Append("/sites " & """" & _sites & """ ")
Call CLI.Append("/ref " & """" & _ref & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _null_regprecise Then
Call CLI.Append("/null-regprecise ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function DOOR_Merge(_in As String, _DOOR As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/DOOR.Merge")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/DOOR " & """" & _DOOR & """ ")
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
Public Function Download_Motifs(_imports As String, Optional _export As String = "") As Integer
Dim CLI As New StringBuilder("/Download.Motifs")
Call CLI.Append("/imports " & """" & _imports & """ ")
If Not _export.StringEmpty Then
Call CLI.Append("/export " & """" & _export & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Download Regprecise database from Web API
''' </summary>
'''
Public Function Download_Regprecise(Optional _work As String = "", Optional _save As String = "") As Integer
Dim CLI As New StringBuilder("/Download.Regprecise")
If Not _work.StringEmpty Then
Call CLI.Append("/work " & """" & _work & """ ")
End If
If Not _save.StringEmpty Then
Call CLI.Append("/save " & """" & _save & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Effector_FillNames(_in As String, _compounds As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Effector.FillNames")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/compounds " & """" & _compounds & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Exports all of the fasta sequence of the TF regulator from the download RegPrecsie FASTA database.
''' </summary>
'''
Public Function Export_Regulators(_imports As String, _Fasta As String, Optional _out As String = "", Optional _locus_out As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.Regulators")
Call CLI.Append("/imports " & """" & _imports & """ ")
Call CLI.Append("/Fasta " & """" & _Fasta & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _locus_out Then
Call CLI.Append("/locus-out ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Family_Hits(_bbh As String, Optional _regprecise As String = "", Optional _pfamkey As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Family.Hits")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
If Not _regprecise.StringEmpty Then
Call CLI.Append("/regprecise " & """" & _regprecise & """ ")
End If
If Not _pfamkey.StringEmpty Then
Call CLI.Append("/pfamkey " & """" & _pfamkey & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Download protein fasta sequence from KEGG database.
''' </summary>
'''
Public Function Fasta_Downloads(_source As String, Optional _out As String = "", Optional _keggtools As String = "") As Integer
Dim CLI As New StringBuilder("/Fasta.Downloads")
Call CLI.Append("/source " & """" & _source & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _keggtools.StringEmpty Then
Call CLI.Append("/keggtools " & """" & _keggtools & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Fetches(_ncbi As String, _imports As String, _out As String) As Integer
Dim CLI As New StringBuilder("/Fetches")
Call CLI.Append("/ncbi " & """" & _ncbi & """ ")
Call CLI.Append("/imports " & """" & _imports & """ ")
Call CLI.Append("/out " & """" & _out & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Fetches_Thread(_gbk As String, _query As String, _out As String) As Integer
Dim CLI As New StringBuilder("/Fetches.Thread")
Call CLI.Append("/gbk " & """" & _gbk & """ ")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/out " & """" & _out & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Gets_Sites_Genes(_in As String, _sites As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Gets.Sites.Genes")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/sites " & """" & _sites & """ ")
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
Public Function heap_Supports(_in As String, Optional _out As String = "", Optional _t As Boolean = False, Optional _l As Boolean = False) As Integer
Dim CLI As New StringBuilder("/heap.Supports")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _t Then
Call CLI.Append("/t ")
End If
If _l Then
Call CLI.Append("/l ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function install_motifs(_imports As String) As Integer
Dim CLI As New StringBuilder("/install.motifs")
Call CLI.Append("/imports " & """" & _imports & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Maps_Effector(_imports As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Maps.Effector")
Call CLI.Append("/imports " & """" & _imports & """ ")
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
Public Function Merge_CORN(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Merge.CORN")
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
Public Function Merge_RegPrecise_Fasta(Optional _in As String = "", Optional _out As String = "", Optional _offline As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Merge.RegPrecise.Fasta")
If Not _in.StringEmpty Then
Call CLI.Append("/in " & """" & _in & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _offline Then
Call CLI.Append("/offline ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Prot_Motifs_EXPORT_pfamString(_in As String, _PTT As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Prot_Motifs.EXPORT.pfamString")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
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
Public Function Prot_Motifs_PfamString(_in As String, Optional _fasta As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Prot_Motifs.PfamString")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _fasta.StringEmpty Then
Call CLI.Append("/fasta " & """" & _fasta & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Download protein domain motifs structures from KEGG ssdb.
''' </summary>
'''
Public Function ProtMotifs_Downloads(_source As String, Optional _kegg_tools As String = "") As Integer
Dim CLI As New StringBuilder("/ProtMotifs.Downloads")
Call CLI.Append("/source " & """" & _source & """ ")
If Not _kegg_tools.StringEmpty Then
Call CLI.Append("/kegg.tools " & """" & _kegg_tools & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Repository_Fetch(_imports As String, _genbank As String, Optional _out As String = "", Optional _full As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Repository.Fetch")
Call CLI.Append("/imports " & """" & _imports & """ ")
Call CLI.Append("/genbank " & """" & _genbank & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _full Then
Call CLI.Append("/full ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Rfam_Regulates(_in As String, _rfam As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Rfam.Regulates")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/rfam " & """" & _rfam & """ ")
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
Public Function Select_TF_BBH(_bbh As String, _imports As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Select.TF.BBH")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/imports " & """" & _imports & """ ")
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
Public Function Select_TF_Pfam_String(_pfam_string As String, _imports As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Select.TF.Pfam-String")
Call CLI.Append("/pfam-string " & """" & _pfam_string & """ ")
Call CLI.Append("/imports " & """" & _imports & """ ")
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
Public Function siRNA_Maps(_in As String, _hits As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/siRNA.Maps")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/hits " & """" & _hits & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
