Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/MEME.exe

Namespace GCModellerApps


''' <summary>
'''A wrapper tools for the NCBR meme tools, this is a powerfull tools for reconstruct the regulation in the bacterial genome.
''' </summary>
'''
Public Class MEME : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
'''Select bbh result for the regulators in RegPrecise database from the regulon bbh data.
''' </summary>
'''
Public Function BBH_Select_Regulators(_in As String, _db As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BBH.Select.Regulators")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/db " & """" & _db & """ ")
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
Public Function Build_FamilyDb(_prot As String, _pfam As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Build.FamilyDb")
Call CLI.Append("/prot " & """" & _prot & """ ")
Call CLI.Append("/pfam " & """" & _pfam & """ ")
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
Public Function Copys(_in As String, Optional _out As String = "", Optional _file As String = "") As Integer
Dim CLI As New StringBuilder("/Copys")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _file.StringEmpty Then
Call CLI.Append("/file " & """" & _file & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Copys_DIR(_in As String, _out As String) As Integer
Dim CLI As New StringBuilder("/Copys.DIR")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/out " & """" & _out & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function CORN(_in As String, _mast As String, _PTT As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/CORN")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/mast " & """" & _mast & """ ")
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
Public Function EXPORT_MotifDraws(_in As String, _MEME As String, _KEGG As String, Optional _out As String = "", Optional _pathway As Boolean = False) As Integer
Dim CLI As New StringBuilder("/EXPORT.MotifDraws")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/MEME " & """" & _MEME & """ ")
Call CLI.Append("/KEGG " & """" & _KEGG & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _pathway Then
Call CLI.Append("/pathway ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Motif iteration step 1
''' </summary>
'''
Public Function Export_MotifSites(_in As String, Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.MotifSites")
Call CLI.Append("/in " & """" & _in & """ ")
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
'''This commandline tool have no argument parameters.
''' </summary>
'''
Public Function Export_Regprecise_Motifs() As Integer
Dim CLI As New StringBuilder("/Export.Regprecise.Motifs")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Motif iteration step 2
''' </summary>
'''
Public Function Export_Similarity_Hits(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.Similarity.Hits")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''3 - Generates the regulation footprints.
''' </summary>
'''
Public Function Footprints(_footprints As String, _coor As String, _DOOR As String, _maps As String, Optional _out As String = "", Optional _cuts As String = "", Optional _extract As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Footprints")
Call CLI.Append("/footprints " & """" & _footprints & """ ")
Call CLI.Append("/coor " & """" & _coor & """ ")
Call CLI.Append("/DOOR " & """" & _DOOR & """ ")
Call CLI.Append("/maps " & """" & _maps & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cuts.StringEmpty Then
Call CLI.Append("/cuts " & """" & _cuts & """ ")
End If
If _extract Then
Call CLI.Append("/extract ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''2
''' </summary>
'''
Public Function Hits_Context(_footprints As String, _PTT As String, Optional _out As String = "", Optional _regprecise As String = "") As Integer
Dim CLI As New StringBuilder("/Hits.Context")
Call CLI.Append("/footprints " & """" & _footprints & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _regprecise.StringEmpty Then
Call CLI.Append("/regprecise " & """" & _regprecise & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function LDM_Compares(_query As String, _sub As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/LDM.Compares")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/sub " & """" & _sub & """ ")
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
Public Function LDM_MaxW(Optional _in As String = "") As Integer
Dim CLI As New StringBuilder("/LDM.MaxW")
If Not _in.StringEmpty Then
Call CLI.Append("/in " & """" & _in & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function LDM_Selects(_trace As String, _meme As String, Optional _out As String = "", Optional _named As Boolean = False) As Integer
Dim CLI As New StringBuilder("/LDM.Selects")
Call CLI.Append("/trace " & """" & _trace & """ ")
Call CLI.Append("/meme " & """" & _meme & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _named Then
Call CLI.Append("/named ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function MAST_MotifMatches(_meme As String, _mast As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/MAST.MotifMatches")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/mast " & """" & _mast & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''1
''' </summary>
'''
Public Function MAST_MotifMatchs_Family(_meme As String, _mast As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/MAST.MotifMatchs.Family")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/mast " & """" & _mast & """ ")
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
Public Function mast_Regulations(_in As String, _correlation As String, _DOOR As String, Optional _out As String = "", Optional _cut As String = "") As Integer
Dim CLI As New StringBuilder("/mast.Regulations")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/correlation " & """" & _correlation & """ ")
Call CLI.Append("/DOOR " & """" & _DOOR & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cut.StringEmpty Then
Call CLI.Append("/cut " & """" & _cut & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function MAST_LDM_Build(_source As String, Optional _out As String = "", Optional _evalue As String = "") As Integer
Dim CLI As New StringBuilder("/MAST_LDM.Build")
Call CLI.Append("/source " & """" & _source & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _evalue.StringEmpty Then
Call CLI.Append("/evalue " & """" & _evalue & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Batch meme task by using tmod toolbox.
''' </summary>
'''
Public Function MEME_Batch(_in As String, Optional _out As String = "", Optional _evalue As String = "", Optional _nmotifs As String = "", Optional _mod As String = "", Optional _maxw As String = "") As Integer
Dim CLI As New StringBuilder("/MEME.Batch")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _evalue.StringEmpty Then
Call CLI.Append("/evalue " & """" & _evalue & """ ")
End If
If Not _nmotifs.StringEmpty Then
Call CLI.Append("/nmotifs " & """" & _nmotifs & """ ")
End If
If Not _mod.StringEmpty Then
Call CLI.Append("/mod " & """" & _mod & """ ")
End If
If Not _maxw.StringEmpty Then
Call CLI.Append("/maxw " & """" & _maxw & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function MEME_LDMs(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/MEME.LDMs")
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
Public Function Motif_BuildRegulons(_meme As String, _model As String, _DOOR As String, _maps As String, _corrs As String, Optional _cut As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Motif.BuildRegulons")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/model " & """" & _model & """ ")
Call CLI.Append("/DOOR " & """" & _DOOR & """ ")
Call CLI.Append("/maps " & """" & _maps & """ ")
Call CLI.Append("/corrs " & """" & _corrs & """ ")
If Not _cut.StringEmpty Then
Call CLI.Append("/cut " & """" & _cut & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Assign the phenotype information And genomic context Info for the motif sites. [SimpleSegment] -> [MotifLog]
''' </summary>
'''
Public Function Motif_Info(_loci As String, Optional _motifs As String = "", Optional _gff As String = "", Optional _atg_dist As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Motif.Info")
Call CLI.Append("/loci " & """" & _loci & """ ")
If Not _motifs.StringEmpty Then
Call CLI.Append("/motifs " & """" & _motifs & """ ")
End If
If Not _gff.StringEmpty Then
Call CLI.Append("/gff " & """" & _gff & """ ")
End If
If Not _atg_dist.StringEmpty Then
Call CLI.Append("/atg-dist " & """" & _atg_dist & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''[SimpleSegment] -> [MotifLog]
''' </summary>
'''
Public Function Motif_Info_Batch(_in As String, _gffs As String, Optional _motifs As String = "", Optional _num_threads As String = "", Optional _atg_dist As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Motif.Info.Batch")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/gffs " & """" & _gffs & """ ")
If Not _motifs.StringEmpty Then
Call CLI.Append("/motifs " & """" & _motifs & """ ")
End If
If Not _num_threads.StringEmpty Then
Call CLI.Append("/num_threads " & """" & _num_threads & """ ")
End If
If Not _atg_dist.StringEmpty Then
Call CLI.Append("/atg-dist " & """" & _atg_dist & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Export of the calculation result from the tomtom program.
''' </summary>
'''
Public Function Motif_Similarity(_in As String, _motifs As String, Optional _out As String = "", Optional _bp_var As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Motif.Similarity")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/motifs " & """" & _motifs & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _bp_var Then
Call CLI.Append("/bp.var ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function MotifHits_Regulation(_hits As String, _source As String, _PTT As String, _correlates As String, _bbh As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/MotifHits.Regulation")
Call CLI.Append("/hits " & """" & _hits & """ ")
Call CLI.Append("/source " & """" & _source & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
Call CLI.Append("/correlates " & """" & _correlates & """ ")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
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
Public Function MotifSites_Fasta(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/MotifSites.Fasta")
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
Public Function Parser_DEGs(_degs As String, _PTT As String, _door As String, _out As String, Optional _log_fold As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.DEGs")
Call CLI.Append("/degs " & """" & _degs & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
Call CLI.Append("/door " & """" & _door & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If Not _log_fold.StringEmpty Then
Call CLI.Append("/log-fold " & """" & _log_fold & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Locus(_locus As String, _PTT As String, _DOOR As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Locus")
Call CLI.Append("/locus " & """" & _locus & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
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
Public Function Parser_Log2(_in As String, _PTT As String, _DOOR As String, Optional _factor As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Log2")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
Call CLI.Append("/DOOR " & """" & _DOOR & """ ")
If Not _factor.StringEmpty Then
Call CLI.Append("/factor " & """" & _factor & """ ")
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
Public Function Parser_MAST(_sites As String, _ptt As String, _door As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.MAST")
Call CLI.Append("/sites " & """" & _sites & """ ")
Call CLI.Append("/ptt " & """" & _ptt & """ ")
Call CLI.Append("/door " & """" & _door & """ ")
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
Public Function Parser_Modules(_KEGG_Modules As String, _PTT As String, _DOOR As String, Optional _locus As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Modules")
Call CLI.Append("/KEGG.Modules " & """" & _KEGG_Modules & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
Call CLI.Append("/DOOR " & """" & _DOOR & """ ")
If Not _locus.StringEmpty Then
Call CLI.Append("/locus " & """" & _locus & """ ")
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
Public Function Parser_Operon(_in As String, _PTT As String, Optional _out As String = "", Optional _offset As String = "", Optional _family As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Parser.Operon")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _offset.StringEmpty Then
Call CLI.Append("/offset " & """" & _offset & """ ")
End If
If _family Then
Call CLI.Append("/family ")
End If
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
Public Function Parser_Pathway(_KEGG_Pathways As String, _PTT As String, _DOOR As String, Optional _locus As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Pathway")
Call CLI.Append("/KEGG.Pathways " & """" & _KEGG_Pathways & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
Call CLI.Append("/DOOR " & """" & _DOOR & """ ")
If Not _locus.StringEmpty Then
Call CLI.Append("/locus " & """" & _locus & """ ")
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
Public Function Parser_RegPrecise_Operons(_operon As String, _PTT As String, Optional _door As String = "", Optional _id As String = "", Optional _locus As String = "", Optional _out As String = "", Optional _corn As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Parser.RegPrecise.Operons")
Call CLI.Append("/operon " & """" & _operon & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
If Not _door.StringEmpty Then
Call CLI.Append("/door " & """" & _door & """ ")
End If
If Not _id.StringEmpty Then
Call CLI.Append("/id " & """" & _id & """ ")
End If
If Not _locus.StringEmpty Then
Call CLI.Append("/locus " & """" & _locus & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _corn Then
Call CLI.Append("/corn ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Regulon(_inDIR As String, _out As String, _PTT As String, Optional _door As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Regulon")
Call CLI.Append("/inDIR " & """" & _inDIR & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
If Not _door.StringEmpty Then
Call CLI.Append("/door " & """" & _door & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Regulon_gb(_inDIR As String, _out As String, _gb As String, Optional _door As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Regulon.gb")
Call CLI.Append("/inDIR " & """" & _inDIR & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
Call CLI.Append("/gb " & """" & _gb & """ ")
If Not _door.StringEmpty Then
Call CLI.Append("/door " & """" & _door & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Regulon_Merged(_in As String, _out As String, _PTT As String, Optional _door As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Regulon.Merged")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
If Not _door.StringEmpty Then
Call CLI.Append("/door " & """" & _door & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Regulator_Motifs(_bbh As String, _regprecise As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Regulator.Motifs")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/regprecise " & """" & _regprecise & """ ")
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
Public Function Regulator_Motifs_Test(_hits As String, _motifs As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Regulator.Motifs.Test")
Call CLI.Append("/hits " & """" & _hits & """ ")
Call CLI.Append("/motifs " & """" & _motifs & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Regprecise regulators data source compiler.
''' </summary>
'''
Public Function regulators_compile() As Integer
Dim CLI As New StringBuilder("/regulators.compile")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function regulon_export(_in As String, _ref As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/regulon.export")
Call CLI.Append("/in " & """" & _in & """ ")
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
Public Function Regulon_Reconstruct(_bbh As String, _genome As String, _door As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Regulon.Reconstruct")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/genome " & """" & _genome & """ ")
Call CLI.Append("/door " & """" & _door & """ ")
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
Public Function Regulon_Reconstruct2(_bbh As String, _genome As String, _door As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Regulon.Reconstruct2")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/genome " & """" & _genome & """ ")
Call CLI.Append("/door " & """" & _door & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Doing the regulon reconstruction job in batch mode.
''' </summary>
'''
Public Function Regulon_Reconstructs(_bbh As String, _genome As String, Optional _door As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Regulon.Reconstructs")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/genome " & """" & _genome & """ ")
If Not _door.StringEmpty Then
Call CLI.Append("/door " & """" & _door & """ ")
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
Public Function Regulon_Test(_in As String, _reg As String, _bbh As String) As Integer
Dim CLI As New StringBuilder("/Regulon.Test")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/reg " & """" & _reg & """ ")
Call CLI.Append("/bbh " & """" & _bbh & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function RfamSites(_source As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/RfamSites")
Call CLI.Append("/source " & """" & _source & """ ")
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
Public Function seq_logo(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/seq.logo")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Motif iteration step 3
''' </summary>
'''
Public Function Similarity_Union(_in As String, _meme As String, _hits As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Similarity.Union")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/hits " & """" & _hits & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''[MAST.Xml] -> [SimpleSegment]
''' </summary>
'''
Public Function Site_MAST_Scan(_mast As String, Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Site.MAST_Scan")
Call CLI.Append("/mast " & """" & _mast & """ ")
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
'''[MAST.Xml] -> [SimpleSegment]
''' </summary>
'''
Public Function Site_MAST_Scan_Batch(_mast As String, Optional _out As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("/Site.MAST_Scan.Batch")
Call CLI.Append("/mast " & """" & _mast & """ ")
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
Public Function Site_RegexScan(_meme As String, _nt As String, Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Site.RegexScan")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/nt " & """" & _nt & """ ")
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
Public Function site_scan(_query As String, _subject As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/site.scan")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Generates the regulation information.
''' </summary>
'''
Public Function SiteHits_Footprints(_sites As String, _bbh As String, _meme As String, _PTT As String, _DOOR As String, Optional _out As String = "", Optional _queryhash As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SiteHits.Footprints")
Call CLI.Append("/sites " & """" & _sites & """ ")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
Call CLI.Append("/DOOR " & """" & _DOOR & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _queryhash Then
Call CLI.Append("/queryhash ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SWTOM_Compares(_query As String, _subject As String, Optional _out As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SWTOM.Compares")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _no_html Then
Call CLI.Append("/no-html ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SWTOM_Compares_Batch(_query As String, _subject As String, Optional _out As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SWTOM.Compares.Batch")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _no_html Then
Call CLI.Append("/no-html ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SWTOM_LDM(_query As String, _subject As String, Optional _out As String = "", Optional _method As String = "") As Integer
Dim CLI As New StringBuilder("/SWTOM.LDM")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _method.StringEmpty Then
Call CLI.Append("/method " & """" & _method & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SWTOM_Query(_query As String, Optional _out As String = "", Optional _method As String = "", Optional _bits_level As String = "", Optional _minw As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SWTOM.Query")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _method.StringEmpty Then
Call CLI.Append("/method " & """" & _method & """ ")
End If
If Not _bits_level.StringEmpty Then
Call CLI.Append("/bits.level " & """" & _bits_level & """ ")
End If
If Not _minw.StringEmpty Then
Call CLI.Append("/minw " & """" & _minw & """ ")
End If
If _no_html Then
Call CLI.Append("/no-html ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SWTOM_Query_Batch(_query As String, Optional _out As String = "", Optional _sw_offset As String = "", Optional _method As String = "", Optional _bits_level As String = "", Optional _minw As String = "", Optional _sw_threshold As String = "", Optional _tom_threshold As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SWTOM.Query.Batch")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _sw_offset.StringEmpty Then
Call CLI.Append("/sw-offset " & """" & _sw_offset & """ ")
End If
If Not _method.StringEmpty Then
Call CLI.Append("/method " & """" & _method & """ ")
End If
If Not _bits_level.StringEmpty Then
Call CLI.Append("/bits.level " & """" & _bits_level & """ ")
End If
If Not _minw.StringEmpty Then
Call CLI.Append("/minw " & """" & _minw & """ ")
End If
If Not _sw_threshold.StringEmpty Then
Call CLI.Append("/sw-threshold " & """" & _sw_threshold & """ ")
End If
If Not _tom_threshold.StringEmpty Then
Call CLI.Append("/tom-threshold " & """" & _tom_threshold & """ ")
End If
If _no_html Then
Call CLI.Append("/no-html ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Tom_Query(_query As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "", Optional _meme As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Tom.Query")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _method.StringEmpty Then
Call CLI.Append("/method " & """" & _method & """ ")
End If
If Not _cost.StringEmpty Then
Call CLI.Append("/cost " & """" & _cost & """ ")
End If
If Not _threshold.StringEmpty Then
Call CLI.Append("/threshold " & """" & _threshold & """ ")
End If
If _meme Then
Call CLI.Append("/meme ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Tom_Query_Batch(_query As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "") As Integer
Dim CLI As New StringBuilder("/Tom.Query.Batch")
Call CLI.Append("/query " & """" & _query & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _method.StringEmpty Then
Call CLI.Append("/method " & """" & _method & """ ")
End If
If Not _cost.StringEmpty Then
Call CLI.Append("/cost " & """" & _cost & """ ")
End If
If Not _threshold.StringEmpty Then
Call CLI.Append("/threshold " & """" & _threshold & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TomTOM(_query As String, _subject As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "") As Integer
Dim CLI As New StringBuilder("/TomTOM")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _method.StringEmpty Then
Call CLI.Append("/method " & """" & _method & """ ")
End If
If Not _cost.StringEmpty Then
Call CLI.Append("/cost " & """" & _cost & """ ")
End If
If Not _threshold.StringEmpty Then
Call CLI.Append("/threshold " & """" & _threshold & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TomTom_LDM(_query As String, _subject As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "") As Integer
Dim CLI As New StringBuilder("/TomTom.LDM")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _method.StringEmpty Then
Call CLI.Append("/method " & """" & _method & """ ")
End If
If Not _cost.StringEmpty Then
Call CLI.Append("/cost " & """" & _cost & """ ")
End If
If Not _threshold.StringEmpty Then
Call CLI.Append("/threshold " & """" & _threshold & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TomTOM_Similarity(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/TomTOM.Similarity")
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
Public Function TOMTOM_Similarity_Batch(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/TOMTOM.Similarity.Batch")
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
Public Function TomTom_Sites_Groups(_in As String, _meme As String, Optional _grep As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/TomTom.Sites.Groups")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/meme " & """" & _meme & """ ")
If Not _grep.StringEmpty Then
Call CLI.Append("/grep " & """" & _grep & """ ")
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
Public Function Trim_MastSite(_in As String, _locus As String, _correlations As String, Optional _out As String = "", Optional _cut As String = "") As Integer
Dim CLI As New StringBuilder("/Trim.MastSite")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/locus " & """" & _locus & """ ")
Call CLI.Append("/correlations " & """" & _correlations & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _cut.StringEmpty Then
Call CLI.Append("/cut " & """" & _cut & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Trim meme input data set for duplicated sequence and short seqeucne which its min length is smaller than the required min length.
''' </summary>
'''
Public Function Trim_MEME_Dataset(_in As String, Optional _out As String = "", Optional _minl As String = "", Optional _distinct As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Trim.MEME.Dataset")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _minl.StringEmpty Then
Call CLI.Append("/minl " & """" & _minl & """ ")
End If
If _distinct Then
Call CLI.Append("/distinct ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Genome wide step 2
''' </summary>
'''
Public Function build_Regulations(_bbh As String, _mast As String, Optional _cutoff As String = "", Optional _out As String = "", Optional _sp As String = "", Optional _door As String = "", Optional _door_extract As Boolean = False) As Integer
Dim CLI As New StringBuilder("--build.Regulations")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/mast " & """" & _mast & """ ")
If Not _cutoff.StringEmpty Then
Call CLI.Append("/cutoff " & """" & _cutoff & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _sp.StringEmpty Then
Call CLI.Append("/sp " & """" & _sp & """ ")
End If
If Not _door.StringEmpty Then
Call CLI.Append("/door " & """" & _door & """ ")
End If
If _door_extract Then
Call CLI.Append("/door.extract ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function build_Regulations_From_Motifs(_bbh As String, _motifs As String, Optional _cutoff As String = "", Optional _sp As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--build.Regulations.From.Motifs")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/motifs " & """" & _motifs & """ ")
If Not _cutoff.StringEmpty Then
Call CLI.Append("/cutoff " & """" & _cutoff & """ ")
End If
If Not _sp.StringEmpty Then
Call CLI.Append("/sp " & """" & _sp & """ ")
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
Public Function CExpr_WGCNA(_mods As String, _genome As String, _out As String) As Integer
Dim CLI As New StringBuilder("--CExpr.WGCNA")
Call CLI.Append("/mods " & """" & _mods & """ ")
Call CLI.Append("/genome " & """" & _genome & """ ")
Call CLI.Append("/out " & """" & _out & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Download Regprecise database from Web API
''' </summary>
'''
Public Function Download_Regprecise(Optional _work As String = "", Optional _save As String = "") As Integer
Dim CLI As New StringBuilder("Download.Regprecise")
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
Public Function Dump_KEGG_Family(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--Dump.KEGG.Family")
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
Public Function family_statics(_sites As String, _mods As String) As Integer
Dim CLI As New StringBuilder("--family.statics")
Call CLI.Append("/sites " & """" & _sites & """ ")
Call CLI.Append("/mods " & """" & _mods & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Get_Intergenic(_PTT As String, _nt As String, Optional _o As String = "", Optional _len As String = "", Optional _strict As Boolean = False) As Integer
Dim CLI As New StringBuilder("--Get.Intergenic")
Call CLI.Append("/PTT " & """" & _PTT & """ ")
Call CLI.Append("/nt " & """" & _nt & """ ")
If Not _o.StringEmpty Then
Call CLI.Append("/o " & """" & _o & """ ")
End If
If Not _len.StringEmpty Then
Call CLI.Append("/len " & """" & _len & """ ")
End If
If _strict Then
Call CLI.Append("/strict ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function GetFasta(_bbh As String, _id As String, _regprecise As String) As Integer
Dim CLI As New StringBuilder("--GetFasta")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/id " & """" & _id & """ ")
Call CLI.Append("/regprecise " & """" & _regprecise & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function hits_diff(_query As String, _subject As String, Optional _reverse As Boolean = False) As Integer
Dim CLI As New StringBuilder("--hits.diff")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
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
Public Function Intersect_Max(_query As String, _subject As String) As Integer
Dim CLI As New StringBuilder("--Intersect.Max")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function logo_Batch(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--logo.Batch")
Call CLI.Append("-in " & """" & _in & """ ")
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
Public Function mapped_Back(_meme As String, _mast As String, _ptt As String, Optional _out As String = "", Optional _offset As String = "", Optional _atg_dist As String = "") As Integer
Dim CLI As New StringBuilder("--mapped-Back")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/mast " & """" & _mast & """ ")
Call CLI.Append("/ptt " & """" & _ptt & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _offset.StringEmpty Then
Call CLI.Append("/offset " & """" & _offset & """ ")
End If
If Not _atg_dist.StringEmpty Then
Call CLI.Append("/atg-dist " & """" & _atg_dist & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function mast_compile(_mast As String, _ptt As String, Optional _p_value As String = "", Optional _mast_ldm As String = "", Optional _atg_dist As String = "", Optional _no_meme As Boolean = False, Optional _no_reginfo As Boolean = False) As Integer
Dim CLI As New StringBuilder("mast.compile")
Call CLI.Append("/mast " & """" & _mast & """ ")
Call CLI.Append("/ptt " & """" & _ptt & """ ")
If Not _p_value.StringEmpty Then
Call CLI.Append("/p-value " & """" & _p_value & """ ")
End If
If Not _mast_ldm.StringEmpty Then
Call CLI.Append("/mast-ldm " & """" & _mast_ldm & """ ")
End If
If Not _atg_dist.StringEmpty Then
Call CLI.Append("/atg-dist " & """" & _atg_dist & """ ")
End If
If _no_meme Then
Call CLI.Append("/no-meme ")
End If
If _no_reginfo Then
Call CLI.Append("/no-reginfo ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Genome wide step 1
''' </summary>
'''
Public Function mast_compile_bulk(_source As String, Optional _ptt As String = "", Optional _atg_dist As String = "", Optional _p_value As String = "", Optional _mast_ldm As String = "", Optional _no_meme As Boolean = False, Optional _no_reginfo As Boolean = False, Optional _related_all As Boolean = False) As Integer
Dim CLI As New StringBuilder("mast.compile.bulk")
Call CLI.Append("/source " & """" & _source & """ ")
If Not _ptt.StringEmpty Then
Call CLI.Append("/ptt " & """" & _ptt & """ ")
End If
If Not _atg_dist.StringEmpty Then
Call CLI.Append("/atg-dist " & """" & _atg_dist & """ ")
End If
If Not _p_value.StringEmpty Then
Call CLI.Append("/p-value " & """" & _p_value & """ ")
End If
If Not _mast_ldm.StringEmpty Then
Call CLI.Append("/mast-ldm " & """" & _mast_ldm & """ ")
End If
If _no_meme Then
Call CLI.Append("/no-meme ")
End If
If _no_reginfo Then
Call CLI.Append("/no-reginfo ")
End If
If _related_all Then
Call CLI.Append("/related.all ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Exports the Venn diagram model for the module regulations.
''' </summary>
'''
Public Function modules_regulates(_in As String, Optional _out As String = "", Optional _mods As String = "") As Integer
Dim CLI As New StringBuilder("--modules.regulates")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If Not _mods.StringEmpty Then
Call CLI.Append("/mods " & """" & _mods & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Motif_Locates(_ptt As String, _meme As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("Motif.Locates")
Call CLI.Append("-ptt " & """" & _ptt & """ ")
Call CLI.Append("-meme " & """" & _meme & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Scan for the motif site by using fragment similarity.
''' </summary>
'''
Public Function MotifScan(_nt As String, _motif As String, Optional _delta As String = "", Optional _delta2 As String = "", Optional _offset As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("MotifScan")
Call CLI.Append("-nt " & """" & _nt & """ ")
Call CLI.Append("/motif " & """" & _motif & """ ")
If Not _delta.StringEmpty Then
Call CLI.Append("/delta " & """" & _delta & """ ")
End If
If Not _delta2.StringEmpty Then
Call CLI.Append("/delta2 " & """" & _delta2 & """ ")
End If
If Not _offset.StringEmpty Then
Call CLI.Append("/offset " & """" & _offset & """ ")
End If
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Associates of the pathway regulation information for the predicted virtual footprint information.
''' </summary>
'''
Public Function pathway_regulates(_footprints As String, _pathway As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--pathway.regulates")
Call CLI.Append("-footprints " & """" & _footprints & """ ")
Call CLI.Append("/pathway " & """" & _pathway & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''The repository parameter is a directory path which is the regprecise database root directory in the GCModeller directory, if you didn't know how to set this value, please leave it blank.
''' </summary>
'''
Public Function Regprecise_Compile(Optional _src As String = "") As Integer
Dim CLI As New StringBuilder("Regprecise.Compile")
If Not _src.StringEmpty Then
Call CLI.Append("/src " & """" & _src & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.
''' </summary>
'''
Public Function regulators_bbh(_bbh As String, Optional _save As String = "", Optional _maps As String = "", Optional _direct As Boolean = False, Optional _regulons As Boolean = False) As Integer
Dim CLI As New StringBuilder("regulators.bbh")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
If Not _save.StringEmpty Then
Call CLI.Append("/save " & """" & _save & """ ")
End If
If Not _maps.StringEmpty Then
Call CLI.Append("/maps " & """" & _maps & """ ")
End If
If _direct Then
Call CLI.Append("/direct ")
End If
If _regulons Then
Call CLI.Append("/regulons ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function site_Match(_meme As String, _mast As String, _out As String, Optional _ptt As String = "", Optional _len As String = "") As Integer
Dim CLI As New StringBuilder("--site.Match")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/mast " & """" & _mast & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If Not _ptt.StringEmpty Then
Call CLI.Append("/ptt " & """" & _ptt & """ ")
End If
If Not _len.StringEmpty Then
Call CLI.Append("/len " & """" & _len & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function site_Matches(_meme As String, _mast As String, _out As String, Optional _ptt As String = "") As Integer
Dim CLI As New StringBuilder("--site.Matches")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/mast " & """" & _mast & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If Not _ptt.StringEmpty Then
Call CLI.Append("/ptt " & """" & _ptt & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Using this function for processing the meme text output from the tmod toolbox.
''' </summary>
'''
Public Function site_Matches_text(_meme As String, _mast As String, _out As String, Optional _ptt As String = "", Optional _fasta As String = "") As Integer
Dim CLI As New StringBuilder("--site.Matches.text")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/mast " & """" & _mast & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If Not _ptt.StringEmpty Then
Call CLI.Append("/ptt " & """" & _ptt & """ ")
End If
If Not _fasta.StringEmpty Then
Call CLI.Append("/fasta " & """" & _fasta & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Statics of the PCC correlation distribution of the regulation
''' </summary>
'''
Public Function site_stat(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--site.stat")
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
Public Function TCS_Module_Regulations(_MiST2 As String, _footprint As String, _Pathways As String) As Integer
Dim CLI As New StringBuilder("--TCS.Module.Regulations")
Call CLI.Append("/MiST2 " & """" & _MiST2 & """ ")
Call CLI.Append("/footprint " & """" & _footprint & """ ")
Call CLI.Append("/Pathways " & """" & _Pathways & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TCS_Regulations(_TCS As String, _modules As String, _regulations As String) As Integer
Dim CLI As New StringBuilder("--TCS.Regulations")
Call CLI.Append("/TCS " & """" & _TCS & """ ")
Call CLI.Append("/modules " & """" & _modules & """ ")
Call CLI.Append("/regulations " & """" & _regulations & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Associate the dip information with the Sigma 70 virtual footprints.
''' </summary>
'''
Public Function VirtualFootprint_DIP(vf_csv As String, dip_csv As String) As Integer
Dim CLI As New StringBuilder("VirtualFootprint.DIP")
Call CLI.Append("vf.csv " & """" & vf_csv & """ ")
Call CLI.Append("dip.csv " & """" & dip_csv & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
'''Download Regprecise database from REST API
''' </summary>
'''
Public Function wGet_Regprecise(Optional _repository_export As String = "", Optional _updates As Boolean = False) As Integer
Dim CLI As New StringBuilder("wGet.Regprecise")
If Not _repository_export.StringEmpty Then
Call CLI.Append("/repository-export " & """" & _repository_export & """ ")
End If
If _updates Then
Call CLI.Append("/updates ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
