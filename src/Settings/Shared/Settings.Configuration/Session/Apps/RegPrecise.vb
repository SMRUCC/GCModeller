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
Dim CLI$ = $"/Build.Operons /bbh ""{_bbh}"" /PTT ""{_PTT}"" /TF-bbh ""{_TF_bbh}"" /out ""{_out}"" /regprecise ""{_regprecise}"" {If(_tfhit_hash, "/tfhit_hash", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_Regulons_Batch(_bbh As String, _PTT As String, _tf_bbh As String, _regprecise As String, Optional _num_threads As String = "", Optional _out As String = "", Optional _hits_hash As Boolean = False) As Integer
Dim CLI$ = $"/Build.Regulons.Batch /bbh ""{_bbh}"" /PTT ""{_PTT}"" /tf-bbh ""{_tf_bbh}"" /regprecise ""{_regprecise}"" /num_threads ""{_num_threads}"" /out ""{_out}"" {If(_hits_hash, "/hits_hash", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
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
Dim CLI$ = $"/CORN /in ""{_in}"" /motif-sites ""{_motif_sites}"" /sites ""{_sites}"" /ref ""{_ref}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function CORN_Batch(_sites As String, _regulons As String, Optional _name As String = "", Optional _out As String = "", Optional _num_threads As String = "", Optional _null_regprecise As Boolean = False) As Integer
Dim CLI$ = $"/CORN.Batch /sites ""{_sites}"" /regulons ""{_regulons}"" /name ""{_name}"" /out ""{_out}"" /num_threads ""{_num_threads}"" {If(_null_regprecise, "/null-regprecise", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function CORN_thread(_hit As String, _hit_sites As String, _sites As String, _ref As String, Optional _out As String = "", Optional _null_regprecise As Boolean = False) As Integer
Dim CLI$ = $"/CORN.thread /hit ""{_hit}"" /hit-sites ""{_hit_sites}"" /sites ""{_sites}"" /ref ""{_ref}"" /out ""{_out}"" {If(_null_regprecise, "/null-regprecise", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function DOOR_Merge(_in As String, _DOOR As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/DOOR.Merge /in ""{_in}"" /DOOR ""{_DOOR}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Download_Motifs(_imports As String, Optional _export As String = "") As Integer
Dim CLI$ = $"/Download.Motifs /imports ""{_imports}"" /export ""{_export}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Download Regprecise database from Web API
''' </summary>
'''
Public Function Download_Regprecise(Optional _work As String = "", Optional _save As String = "") As Integer
Dim CLI$ = $"Download.Regprecise /work ""{_work}"" /save ""{_save}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Effector_FillNames(_in As String, _compounds As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Effector.FillNames /in ""{_in}"" /compounds ""{_compounds}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Exports all of the fasta sequence of the TF regulator from the download RegPrecsie FASTA database.
''' </summary>
'''
Public Function Export_Regulators(_imports As String, _Fasta As String, Optional _out As String = "", Optional _locus_out As Boolean = False) As Integer
Dim CLI$ = $"/Export.Regulators /imports ""{_imports}"" /Fasta ""{_Fasta}"" /out ""{_out}"" {If(_locus_out, "/locus-out", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Family_Hits(_bbh As String, Optional _regprecise As String = "", Optional _pfamkey As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Family.Hits /bbh ""{_bbh}"" /regprecise ""{_regprecise}"" /pfamkey ""{_pfamkey}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Download protein fasta sequence from KEGG database.
''' </summary>
'''
Public Function Fasta_Downloads(_source As String, Optional _out As String = "", Optional _keggtools As String = "") As Integer
Dim CLI$ = $"Fasta.Downloads /source ""{_source}"" /out ""{_out}"" /keggtools ""{_keggtools}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Fetches(_ncbi As String, _imports As String, _out As String) As Integer
Dim CLI$ = $"/Fetches /ncbi ""{_ncbi}"" /imports ""{_imports}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Fetches_Thread(_gbk As String, _query As String, _out As String) As Integer
Dim CLI$ = $"/Fetches.Thread /gbk ""{_gbk}"" /query ""{_query}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Gets_Sites_Genes(_in As String, _sites As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Gets.Sites.Genes /in ""{_in}"" /sites ""{_sites}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function heap_Supports(_in As String, Optional _out As String = "", Optional _t As Boolean = False, Optional _l As Boolean = False) As Integer
Dim CLI$ = $"/heap.supports /in ""{_in}"" /out ""{_out}"" {If(_t, "/t", "")} {If(_l, "/l", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function install_motifs(_imports As String) As Integer
Dim CLI$ = $"/install.motifs /imports ""{_imports}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Maps_Effector(_imports As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Maps.Effector /imports ""{_imports}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Merge_CORN(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Merge.CORN /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Merge_RegPrecise_Fasta(Optional _in As String = "", Optional _out As String = "", Optional _offline As Boolean = False) As Integer
Dim CLI$ = $"/Merge.RegPrecise.Fasta /in ""{_in}"" /out ""{_out}"" {If(_offline, "/offline", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Prot_Motifs_EXPORT_pfamString(_in As String, _PTT As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Prot_Motifs.EXPORT.pfamString /in ""{_in}"" /PTT ""{_PTT}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Prot_Motifs_PfamString(_in As String, Optional _fasta As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Prot_Motifs.PfamString /in ""{_in}"" /fasta ""{_fasta}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Download protein domain motifs structures from KEGG ssdb.
''' </summary>
'''
Public Function ProtMotifs_Downloads(_source As String, Optional _kegg_tools As String = "") As Integer
Dim CLI$ = $"/ProtMotifs.Downloads /source ""{_source}"" /kegg.tools ""{_kegg_tools}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Repository_Fetch(_imports As String, _genbank As String, Optional _out As String = "", Optional _full As Boolean = False) As Integer
Dim CLI$ = $"/Repository.Fetch /imports ""{_imports}"" /genbank ""{_genbank}"" /out ""{_out}"" {If(_full, "/full", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Rfam_Regulates(_in As String, _rfam As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Rfam.Regulates /in ""{_in}"" /rfam ""{_rfam}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Select_TF_BBH(_bbh As String, _imports As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Select.TF.BBH /bbh ""{_bbh}"" /imports ""{_imports}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Select_TF_Pfam_String(_pfam_string As String, _imports As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Select.TF.Pfam-String /pfam-string ""{_pfam_string}"" /imports ""{_imports}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function siRNA_Maps(_in As String, _hits As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/siRNA.Maps /in ""{_in}"" /hits ""{_hits}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function
End Class
End Namespace
