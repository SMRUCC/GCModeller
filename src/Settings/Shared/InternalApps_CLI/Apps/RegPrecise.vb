Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: D:/GCModeller/GCModeller/bin/RegPrecise.exe

Namespace GCModellerApps


''' <summary>
'''
''' </summary>
'''
Public Class RegPrecise : Inherits InteropService

Public Const App$ = "RegPrecise.exe"

Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
''' ```
''' /Build.Operons /bbh &lt;bbh.csv> /PTT &lt;genome.PTT> /TF-bbh &lt;bbh.csv> [/tfHit_hash /out &lt;out.csv> /regprecise &lt;regprecise.Xml>]
''' ```
''' If the /regprecise parameter is not presented, then you should install the regprecise in the GCModeller database repostiory first.
''' </summary>
'''
Public Function OperonBuilder(bbh As String, PTT As String, TF_bbh As String, Optional out As String = "", Optional regprecise As String = "", Optional tfhit_hash As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Build.Operons")
Call CLI.Append(" ")
Call CLI.Append("/bbh " & """" & bbh & """ ")
Call CLI.Append("/PTT " & """" & PTT & """ ")
Call CLI.Append("/TF-bbh " & """" & TF_bbh & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If
If Not regprecise.StringEmpty Then
Call CLI.Append("/regprecise " & """" & regprecise & """ ")
End If
If tfhit_hash Then
Call CLI.Append("/tfhit_hash ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.Regulons.Batch /bbh &lt;bbh.DIR> /PTT &lt;PTT.DIR> /tf-bbh &lt;tf-bbh.DIR> /regprecise &lt;regprecise.Xml> [/num_threads &lt;-1> /hits_hash /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function RegulonBatchBuilder(bbh As String, PTT As String, tf_bbh As String, regprecise As String, Optional num_threads As String = "", Optional out As String = "", Optional hits_hash As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Build.Regulons.Batch")
Call CLI.Append(" ")
Call CLI.Append("/bbh " & """" & bbh & """ ")
Call CLI.Append("/PTT " & """" & PTT & """ ")
Call CLI.Append("/tf-bbh " & """" & tf_bbh & """ ")
Call CLI.Append("/regprecise " & """" & regprecise & """ ")
If Not num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & num_threads & """ ")
End If
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If
If hits_hash Then
Call CLI.Append("/hits_hash ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /CORN /in &lt;regulons.DIR> /motif-sites &lt;motiflogs.csv.DIR> /sites &lt;motiflogs.csv> /ref &lt;regulons.Csv> [/out &lt;out.csv>]
''' ```
''' Join two vertices by edge if the correspondent operons:
''' i) are orthologous;
''' ii) have cantiodate transcription factor binding sites.
''' Collect all linked components. Two operons from two different genomes are called orthologous if they share at least one orthologous gene.
''' </summary>
'''
Public Function CORN([in] As String, motif_sites As String, sites As String, ref As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/CORN")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
Call CLI.Append("/motif-sites " & """" & motif_sites & """ ")
Call CLI.Append("/sites " & """" & sites & """ ")
Call CLI.Append("/ref " & """" & ref & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /CORN.Batch /sites &lt;motiflogs.gff.sites.Csv.DIR> /regulons &lt;regprecise.regulons.csv.DIR> [/name &lt;name> /out &lt;outDIR> /num_threads &lt;-1> /null-regprecise]
''' ```
''' </summary>
'''
Public Function CORNBatch(sites As String, regulons As String, Optional name As String = "", Optional out As String = "", Optional num_threads As String = "", Optional null_regprecise As Boolean = False) As Integer
Dim CLI As New StringBuilder("/CORN.Batch")
Call CLI.Append(" ")
Call CLI.Append("/sites " & """" & sites & """ ")
Call CLI.Append("/regulons " & """" & regulons & """ ")
If Not name.StringEmpty Then
Call CLI.Append("/name " & """" & name & """ ")
End If
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If
If Not num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & num_threads & """ ")
End If
If null_regprecise Then
Call CLI.Append("/null-regprecise ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /CORN.thread /hit &lt;regulons.Csv> /hit-sites &lt;motiflogs.csv> /sites &lt;query.motiflogs.csv> /ref &lt;query.regulons.Csv> [/null-regprecise /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function CORNSingleThread(hit As String, hit_sites As String, sites As String, ref As String, Optional out As String = "", Optional null_regprecise As Boolean = False) As Integer
Dim CLI As New StringBuilder("/CORN.thread")
Call CLI.Append(" ")
Call CLI.Append("/hit " & """" & hit & """ ")
Call CLI.Append("/hit-sites " & """" & hit_sites & """ ")
Call CLI.Append("/sites " & """" & sites & """ ")
Call CLI.Append("/ref " & """" & ref & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If
If null_regprecise Then
Call CLI.Append("/null-regprecise ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /DOOR.Merge /in &lt;operon.csv> /DOOR &lt;genome.opr> [/out &lt;out.opr>]
''' ```
''' </summary>
'''
Public Function MergeDOOR([in] As String, DOOR As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/DOOR.Merge")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
Call CLI.Append("/DOOR " & """" & DOOR & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Download.Motifs /imports &lt;RegPrecise.DIR> [/export &lt;EXPORT_DIR>]
''' ```
''' </summary>
'''
Public Function DownloadMotifSites([imports] As String, Optional export As String = "") As Integer
Dim CLI As New StringBuilder("/Download.Motifs")
Call CLI.Append(" ")
Call CLI.Append("/imports " & """" & [imports] & """ ")
If Not export.StringEmpty Then
Call CLI.Append("/export " & """" & export & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' Download.Regprecise [/work ./ /save &lt;saveXml>]
''' ```
''' Download Regprecise database from Web API
''' </summary>
'''
Public Function DownloadRegprecise2(Optional work As String = "", Optional save As String = "") As Integer
Dim CLI As New StringBuilder("Download.Regprecise")
Call CLI.Append(" ")
If Not work.StringEmpty Then
Call CLI.Append("/work " & """" & work & """ ")
End If
If Not save.StringEmpty Then
Call CLI.Append("/save " & """" & save & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Effector.FillNames /in &lt;effectors.csv> /compounds &lt;metacyc.compounds> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function EffectorFillNames([in] As String, compounds As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/Effector.FillNames")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
Call CLI.Append("/compounds " & """" & compounds & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.Regulators /imports &lt;regprecise.downloads.DIR> /Fasta &lt;regprecise.fasta> [/locus-out /out &lt;out.fasta>]
''' ```
''' Exports all of the fasta sequence of the TF regulator from the download RegPrecsie FASTA database.
''' </summary>
'''
Public Function ExportRegulators([imports] As String, Fasta As String, Optional out As String = "", Optional locus_out As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.Regulators")
Call CLI.Append(" ")
Call CLI.Append("/imports " & """" & [imports] & """ ")
Call CLI.Append("/Fasta " & """" & Fasta & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If
If locus_out Then
Call CLI.Append("/locus-out ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Family.Hits /bbh &lt;bbh.csv> [/regprecise &lt;RegPrecise.Xml> /pfamKey &lt;query.pfam-string> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function FamilyHits(bbh As String, Optional regprecise As String = "", Optional pfamkey As String = "", Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/Family.Hits")
Call CLI.Append(" ")
Call CLI.Append("/bbh " & """" & bbh & """ ")
If Not regprecise.StringEmpty Then
Call CLI.Append("/regprecise " & """" & regprecise & """ ")
End If
If Not pfamkey.StringEmpty Then
Call CLI.Append("/pfamkey " & """" & pfamkey & """ ")
End If
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' Fasta.Downloads /source &lt;sourceDIR> [/out &lt;outDIR> /keggTools &lt;kegg.exe>]
''' ```
''' Download protein fasta sequence from KEGG database.
''' </summary>
'''
Public Function DownloadFasta(source As String, Optional out As String = "", Optional keggtools As String = "") As Integer
Dim CLI As New StringBuilder("Fasta.Downloads")
Call CLI.Append(" ")
Call CLI.Append("/source " & """" & source & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If
If Not keggtools.StringEmpty Then
Call CLI.Append("/keggtools " & """" & keggtools & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Fetches /ncbi &lt;all_gbk.DIR> /imports &lt;inDIR> /out &lt;outDIR>
''' ```
''' </summary>
'''
Public Function Fetch(ncbi As String, [imports] As String, out As String) As Integer
Dim CLI As New StringBuilder("/Fetches")
Call CLI.Append(" ")
Call CLI.Append("/ncbi " & """" & ncbi & """ ")
Call CLI.Append("/imports " & """" & [imports] & """ ")
Call CLI.Append("/out " & """" & out & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Fetches.Thread /gbk &lt;gbkDIR> /query &lt;query.txt> /out &lt;outDIR>
''' ```
''' </summary>
'''
Public Function FetchThread(gbk As String, query As String, out As String) As Integer
Dim CLI As New StringBuilder("/Fetches.Thread")
Call CLI.Append(" ")
Call CLI.Append("/gbk " & """" & gbk & """ ")
Call CLI.Append("/query " & """" & query & """ ")
Call CLI.Append("/out " & """" & out & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Gets.Sites.Genes /in &lt;tf.bbh.csv> /sites &lt;motiflogs.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function GetSites([in] As String, sites As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/Gets.Sites.Genes")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
Call CLI.Append("/sites " & """" & sites & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /heap.supports /in &lt;inDIR> [/out &lt;out.Csv> /T /l]
''' ```
''' </summary>
'''
Public Function Supports([in] As String, Optional out As String = "", Optional t As Boolean = False, Optional l As Boolean = False) As Integer
Dim CLI As New StringBuilder("/heap.supports")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If
If t Then
Call CLI.Append("/t ")
End If
If l Then
Call CLI.Append("/l ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /install.motifs /imports &lt;motifs.DIR>
''' ```
''' </summary>
'''
Public Function InstallRegPreciseMotifs([imports] As String) As Integer
Dim CLI As New StringBuilder("/install.motifs")
Call CLI.Append(" ")
Call CLI.Append("/imports " & """" & [imports] & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Maps.Effector /imports &lt;RegPrecise.DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function Effectors([imports] As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/Maps.Effector")
Call CLI.Append(" ")
Call CLI.Append("/imports " & """" & [imports] & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Merge.CORN /in &lt;inDIR> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function MergeCORN([in] As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/Merge.CORN")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Merge.RegPrecise.Fasta [/in &lt;inDIR> /out outDIR /offline]
''' ```
''' </summary>
'''
Public Function MergeDownload(Optional [in] As String = "", Optional out As String = "", Optional offline As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Merge.RegPrecise.Fasta")
Call CLI.Append(" ")
If Not [in].StringEmpty Then
Call CLI.Append("/in " & """" & [in] & """ ")
End If
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If
If offline Then
Call CLI.Append("/offline ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Prot_Motifs.EXPORT.pfamString /in &lt;motifs.json> /PTT &lt;genome.ptt> [/out &lt;pfam-string.csv>]
''' ```
''' </summary>
'''
Public Function ProteinMotifsEXPORT([in] As String, PTT As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/Prot_Motifs.EXPORT.pfamString")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
Call CLI.Append("/PTT " & """" & PTT & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Prot_Motifs.PfamString /in &lt;RegPrecise.Download_DIR> [/fasta &lt;RegPrecise.fasta> /out &lt;pfam-string.csv>]
''' ```
''' </summary>
'''
Public Function ProtMotifToPfamString([in] As String, Optional fasta As String = "", Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/Prot_Motifs.PfamString")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
If Not fasta.StringEmpty Then
Call CLI.Append("/fasta " & """" & fasta & """ ")
End If
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /ProtMotifs.Downloads /source &lt;source.DIR> [/kegg.Tools &lt;./kegg.exe>]
''' ```
''' Download protein domain motifs structures from KEGG ssdb.
''' </summary>
'''
Public Function DownloadProteinMotifs(source As String, Optional kegg_tools As String = "") As Integer
Dim CLI As New StringBuilder("/ProtMotifs.Downloads")
Call CLI.Append(" ")
Call CLI.Append("/source " & """" & source & """ ")
If Not kegg_tools.StringEmpty Then
Call CLI.Append("/kegg.tools " & """" & kegg_tools & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Repository.Fetch /imports &lt;RegPrecise.Xml> /genbank &lt;NCBI_Genbank_DIR> [/full /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function FetchRepostiory([imports] As String, genbank As String, Optional out As String = "", Optional full As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Repository.Fetch")
Call CLI.Append(" ")
Call CLI.Append("/imports " & """" & [imports] & """ ")
Call CLI.Append("/genbank " & """" & genbank & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If
If full Then
Call CLI.Append("/full ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Rfam.Regulates /in &lt;RegPrecise.regulons.csv> /rfam &lt;rfam_search.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function RfamRegulates([in] As String, rfam As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/Rfam.Regulates")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
Call CLI.Append("/rfam " & """" & rfam & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Select.TF.BBH /bbh &lt;bbh.csv> /imports &lt;RegPrecise.downloads.DIR> [/out &lt;out.bbh.csv>]
''' ```
''' </summary>
'''
Public Function SelectTFBBH(bbh As String, [imports] As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/Select.TF.BBH")
Call CLI.Append(" ")
Call CLI.Append("/bbh " & """" & bbh & """ ")
Call CLI.Append("/imports " & """" & [imports] & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Select.TF.Pfam-String /pfam-string &lt;RegPrecise.pfam-string.csv> /imports &lt;regprecise.downloads.DIR> [/out &lt;TF.pfam-string.csv>]
''' ```
''' </summary>
'''
Public Function SelectTFPfams(pfam_string As String, [imports] As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/Select.TF.Pfam-String")
Call CLI.Append(" ")
Call CLI.Append("/pfam-string " & """" & pfam_string & """ ")
Call CLI.Append("/imports " & """" & [imports] & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /siRNA.Maps /in &lt;siRNA.csv> /hits &lt;blastn.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function siRNAMaps([in] As String, hits As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/siRNA.Maps")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
Call CLI.Append("/hits " & """" & hits & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
