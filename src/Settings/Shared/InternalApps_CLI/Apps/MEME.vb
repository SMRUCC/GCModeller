Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/MEME.exe

Namespace GCModellerApps


''' <summary>
''' A wrapper tools for the NCBR meme tools, this is a powerfull tools for reconstruct the regulation in the bacterial genome.
''' </summary>
'''
Public Class MEME : Inherits InteropService

Public Const App$ = "MEME.exe"

Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
''' ```
''' /BBH.Select.Regulators /in &lt;bbh.csv> /db &lt;regprecise_downloads.DIR> [/out &lt;out.csv>]
''' ```
''' Select bbh result for the regulators in RegPrecise database from the regulon bbh data.
''' </summary>
'''
Public Function SelectRegulatorsBBH(_in As String, _db As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/BBH.Select.Regulators")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/db " & """" & _db & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Build.FamilyDb /prot &lt;RegPrecise.prot.fasta> /pfam &lt;pfam-string.csv> [/out &lt;out.Xml>]
''' ```
''' </summary>
'''
Public Function BuildFamilyDb(_prot As String, _pfam As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Build.FamilyDb")
Call CLI.Append(" ")
Call CLI.Append("/prot " & """" & _prot & """ ")
Call CLI.Append("/pfam " & """" & _pfam & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Copys /in &lt;inDIR> [/out &lt;outDIR> /file &lt;meme.txt>]
''' ```
''' </summary>
'''
Public Function BatchCopy(_in As String, Optional _out As String = "", Optional _file As String = "") As Integer
Dim CLI As New StringBuilder("/Copys")
Call CLI.Append(" ")
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
''' ```
''' /Copys.DIR /in &lt;inDIR> /out &lt;outDIR>
''' ```
''' </summary>
'''
Public Function BatchCopyDIR(_in As String, _out As String) As Integer
Dim CLI As New StringBuilder("/Copys.DIR")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/out " & """" & _out & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /CORN /in &lt;operons.csv> /mast &lt;mastDIR> /PTT &lt;genome.ptt> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function CORN(_in As String, _mast As String, _PTT As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/CORN")
Call CLI.Append(" ")
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
''' ```
''' /EXPORT.MotifDraws /in &lt;virtualFootprints.csv> /MEME &lt;meme.txt.DIR> /KEGG &lt;KEGG_Modules/Pathways.DIR> [/pathway /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ExportMotifDraw(_in As String, _MEME As String, _KEGG As String, Optional _out As String = "", Optional _pathway As Boolean = False) As Integer
Dim CLI As New StringBuilder("/EXPORT.MotifDraws")
Call CLI.Append(" ")
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
''' ```
''' /Export.MotifSites /in &lt;meme.txt> [/out &lt;outDIR> /batch]
''' ```
''' Motif iteration step 1
''' </summary>
'''
Public Function ExportTestMotifs(_in As String, Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Export.MotifSites")
Call CLI.Append(" ")
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
''' ```
''' /Export.Regprecise.Motifs
''' ```
''' This commandline tool have no argument parameters.
''' </summary>
'''
Public Function ExportRegpreciseMotifs() As Integer
Dim CLI As New StringBuilder("/Export.Regprecise.Motifs")
Call CLI.Append(" ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Export.Similarity.Hits /in &lt;inDIR> [/out &lt;out.Csv>]
''' ```
''' Motif iteration step 2
''' </summary>
'''
Public Function LoadSimilarityHits(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Export.Similarity.Hits")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Footprints /footprints &lt;footprints.xml> /coor &lt;name/DIR> /DOOR &lt;genome.opr> /maps &lt;bbhMappings.Csv> [/out &lt;out.csv> /cuts &lt;0.65> /extract]
''' ```
''' 3 - Generates the regulation footprints.
''' </summary>
'''
Public Function ToFootprints(_footprints As String, _coor As String, _DOOR As String, _maps As String, Optional _out As String = "", Optional _cuts As String = "", Optional _extract As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Footprints")
Call CLI.Append(" ")
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
''' ```
''' /Hits.Context /footprints &lt;footprints.Xml> /PTT &lt;genome.PTT> [/out &lt;out.Xml> /RegPrecise &lt;RegPrecise.Regulations.Xml>]
''' ```
''' 2
''' </summary>
'''
Public Function HitContext(_footprints As String, _PTT As String, Optional _out As String = "", Optional _regprecise As String = "") As Integer
Dim CLI As New StringBuilder("/Hits.Context")
Call CLI.Append(" ")
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
''' ```
''' /LDM.Compares /query &lt;query.LDM.Xml> /sub &lt;subject.LDM.Xml> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function CompareMotif(_query As String, _sub As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/LDM.Compares")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/sub " & """" & _sub & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /LDM.MaxW [/in &lt;sourceDIR>]
''' ```
''' </summary>
'''
Public Function LDMMaxLen(Optional _in As String = "") As Integer
Dim CLI As New StringBuilder("/LDM.MaxW")
Call CLI.Append(" ")
If Not _in.StringEmpty Then
Call CLI.Append("/in " & """" & _in & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /LDM.Selects /trace &lt;footprints.xml> /meme &lt;memeDIR> [/out &lt;outDIR> /named]
''' ```
''' </summary>
'''
Public Function Selectes(_trace As String, _meme As String, Optional _out As String = "", Optional _named As Boolean = False) As Integer
Dim CLI As New StringBuilder("/LDM.Selects")
Call CLI.Append(" ")
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
''' ```
''' /MAST.MotifMatches /meme &lt;meme.txt.DIR> /mast &lt;MAST_OUT.DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function MotifMatch2(_meme As String, _mast As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/MAST.MotifMatches")
Call CLI.Append(" ")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/mast " & """" & _mast & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /MAST.MotifMatchs.Family /meme &lt;meme.txt.DIR> /mast &lt;MAST_OUT.DIR> [/out &lt;out.Xml>]
''' ```
''' 1
''' </summary>
'''
Public Function MotifMatch(_meme As String, _mast As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/MAST.MotifMatchs.Family")
Call CLI.Append(" ")
Call CLI.Append("/meme " & """" & _meme & """ ")
Call CLI.Append("/mast " & """" & _mast & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /mast.Regulations /in &lt;mastSites.Csv> /correlation &lt;sp_name/DIR> /DOOR &lt;DOOR.opr> [/out &lt;footprint.csv> /cut &lt;0.65>]
''' ```
''' </summary>
'''
Public Function MastRegulations(_in As String, _correlation As String, _DOOR As String, Optional _out As String = "", Optional _cut As String = "") As Integer
Dim CLI As New StringBuilder("/mast.Regulations")
Call CLI.Append(" ")
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
''' ```
''' /MAST_LDM.Build /source &lt;sourceDIR> [/out &lt;exportDIR:=./> /evalue &lt;1e-3>]
''' ```
''' </summary>
'''
Public Function BuildPWMDb(_source As String, Optional _out As String = "", Optional _evalue As String = "") As Integer
Dim CLI As New StringBuilder("/MAST_LDM.Build")
Call CLI.Append(" ")
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
''' ```
''' /MEME.Batch /in &lt;inDIR> [/out &lt;outDIR> /evalue &lt;1> /nmotifs &lt;30> /mod &lt;zoops> /maxw &lt;100>]
''' ```
''' Batch meme task by using tmod toolbox.
''' </summary>
'''
Public Function MEMEBatch(_in As String, Optional _out As String = "", Optional _evalue As String = "", Optional _nmotifs As String = "", Optional _mod As String = "", Optional _maxw As String = "") As Integer
Dim CLI As New StringBuilder("/MEME.Batch")
Call CLI.Append(" ")
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
''' ```
''' /MEME.LDMs /in &lt;meme.txt> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function MEME2LDM(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/MEME.LDMs")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Motif.BuildRegulons /meme &lt;meme.txt.DIR> /model &lt;FootprintTrace.xml> /DOOR &lt;DOOR.opr> /maps &lt;bbhmappings.csv> /corrs &lt;name/DIR> [/cut &lt;0.65> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function BuildRegulons(_meme As String, _model As String, _DOOR As String, _maps As String, _corrs As String, Optional _cut As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Motif.BuildRegulons")
Call CLI.Append(" ")
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
''' ```
''' /Motif.Info /loci &lt;loci.csv> [/motifs &lt;motifs.DIR> /gff &lt;genome.gff> /atg-dist 250 /out &lt;out.csv>]
''' ```
''' Assign the phenotype information And genomic context Info for the motif sites. [SimpleSegment] -> [MotifLog]
''' </summary>
'''
Public Function MotifInfo(_loci As String, Optional _motifs As String = "", Optional _gff As String = "", Optional _atg_dist As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Motif.Info")
Call CLI.Append(" ")
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
''' ```
''' /Motif.Info.Batch /in &lt;sites.csv.inDIR> /gffs &lt;gff.DIR> [/motifs &lt;regulogs.motiflogs.MEME.DIR> /num_threads -1 /atg-dist 350 /out &lt;out.DIR>]
''' ```
''' [SimpleSegment] -> [MotifLog]
''' </summary>
'''
Public Function MotifInfoBatch(_in As String, _gffs As String, Optional _motifs As String = "", Optional _num_threads As String = "", Optional _atg_dist As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Motif.Info.Batch")
Call CLI.Append(" ")
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
''' ```
''' /Motif.Similarity /in &lt;tomtom.DIR> /motifs &lt;MEME_OUT.DIR> [/out &lt;out.csv> /bp.var]
''' ```
''' Export of the calculation result from the tomtom program.
''' </summary>
'''
Public Function MEMETOM_MotifSimilarity(_in As String, _motifs As String, Optional _out As String = "", Optional _bp_var As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Motif.Similarity")
Call CLI.Append(" ")
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
''' ```
''' /MotifHits.Regulation /hits &lt;motifHits.Csv> /source &lt;meme.txt.DIR> /PTT &lt;genome.PTT> /correlates &lt;sp/DIR> /bbh &lt;bbhh.csv> [/out &lt;out.footprints.Csv>]
''' ```
''' </summary>
'''
Public Function HitsRegulation(_hits As String, _source As String, _PTT As String, _correlates As String, _bbh As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/MotifHits.Regulation")
Call CLI.Append(" ")
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
''' ```
''' /MotifSites.Fasta /in &lt;mast_motifsites.csv> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function MotifSites2Fasta(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/MotifSites.Fasta")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Parser.DEGs /degs &lt;deseq2.csv> /PTT &lt;genomePTT.DIR> /door &lt;genome.opr> /out &lt;out.DIR> [/log-fold 2]
''' ```
''' </summary>
'''
Public Function ParserDEGs(_degs As String, _PTT As String, _door As String, _out As String, Optional _log_fold As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.DEGs")
Call CLI.Append(" ")
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
''' ```
''' /Parser.Locus /locus &lt;locus.txt> /PTT &lt;genomePTT.DIR> /DOOR &lt;genome.opr> [/out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function ParserLocus(_locus As String, _PTT As String, _DOOR As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Locus")
Call CLI.Append(" ")
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
''' ```
''' /Parser.Log2 /in &lt;log2.csv> /PTT &lt;genomePTT.DIR> /DOOR &lt;genome.opr> [/factor 1 /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ParserLog2(_in As String, _PTT As String, _DOOR As String, Optional _factor As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Log2")
Call CLI.Append(" ")
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
''' ```
''' /Parser.MAST /sites &lt;mastsites.csv> /ptt &lt;genome-context.pttDIR> /door &lt;genome.opr> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ParserMAST(_sites As String, _ptt As String, _door As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.MAST")
Call CLI.Append(" ")
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
''' ```
''' /Parser.Modules /KEGG.Modules &lt;KEGG.modules.DIR> /PTT &lt;genomePTT.DIR> /DOOR &lt;genome.opr> [/locus &lt;union/initx/locus, default:=union> /out &lt;fasta.outDIR>]
''' ```
''' </summary>
'''
Public Function ModuleParser(_KEGG_Modules As String, _PTT As String, _DOOR As String, Optional _locus As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Modules")
Call CLI.Append(" ")
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
''' ```
''' /Parser.Operon /in &lt;footprint.csv> /PTT &lt;PTTDIR> [/out &lt;outDIR> /family /offset &lt;50> /all]
''' ```
''' </summary>
'''
Public Function ParserNextIterator(_in As String, _PTT As String, Optional _out As String = "", Optional _offset As String = "", Optional _family As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Parser.Operon")
Call CLI.Append(" ")
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
''' ```
''' /Parser.Pathway /KEGG.Pathways &lt;KEGG.pathways.DIR> /PTT &lt;genomePTT.DIR> /DOOR &lt;genome.opr> [/locus &lt;union/initx/locus, default:=union> /out &lt;fasta.outDIR>]
''' ```
''' </summary>
'''
Public Function PathwayParser(_KEGG_Pathways As String, _PTT As String, _DOOR As String, Optional _locus As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Pathway")
Call CLI.Append(" ")
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
''' ```
''' /Parser.RegPrecise.Operons /operon &lt;operons.Csv> /PTT &lt;PTT_DIR> [/corn /DOOR &lt;genome.opr> /id &lt;null> /locus &lt;union/initx/locus, default:=union> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function ParserRegPreciseOperon(_operon As String, _PTT As String, Optional _door As String = "", Optional _id As String = "", Optional _locus As String = "", Optional _out As String = "", Optional _corn As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Parser.RegPrecise.Operons")
Call CLI.Append(" ")
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
''' ```
''' /Parser.Regulon /inDIR &lt;regulons.inDIR> /out &lt;fasta.outDIR> /PTT &lt;genomePTT.DIR> [/door &lt;genome.opr>]
''' ```
''' </summary>
'''
Public Function RegulonParser(_inDIR As String, _out As String, _PTT As String, Optional _door As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Regulon")
Call CLI.Append(" ")
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
''' ```
''' /Parser.Regulon.gb /inDIR &lt;regulons.inDIR> /out &lt;fasta.outDIR> /gb &lt;genbank.gbk> [/door &lt;genome.opr>]
''' ```
''' </summary>
'''
Public Function RegulonParser2(_inDIR As String, _out As String, _gb As String, Optional _door As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Regulon.gb")
Call CLI.Append(" ")
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
''' ```
''' /Parser.Regulon.Merged /in &lt;merged.Csv> /out &lt;fasta.outDIR> /PTT &lt;genomePTT.DIR> [/DOOR &lt;genome.opr>]
''' ```
''' </summary>
'''
Public Function RegulonParser3(_in As String, _out As String, _PTT As String, Optional _door As String = "") As Integer
Dim CLI As New StringBuilder("/Parser.Regulon.Merged")
Call CLI.Append(" ")
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
''' ```
''' /Regulator.Motifs /bbh &lt;bbh.csv> /regprecise &lt;genome.DIR> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function RegulatorMotifs(_bbh As String, _regprecise As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Regulator.Motifs")
Call CLI.Append(" ")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/regprecise " & """" & _regprecise & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Regulator.Motifs.Test /hits &lt;familyHits.Csv> /motifs &lt;motifHits.Csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function TestRegulatorMotifs(_hits As String, _motifs As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Regulator.Motifs.Test")
Call CLI.Append(" ")
Call CLI.Append("/hits " & """" & _hits & """ ")
Call CLI.Append("/motifs " & """" & _motifs & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /regulators.compile
''' ```
''' Regprecise regulators data source compiler.
''' </summary>
'''
Public Function RegulatorsCompile() As Integer
Dim CLI As New StringBuilder("/regulators.compile")
Call CLI.Append(" ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /regulon.export /in &lt;sw-tom_out.DIR> /ref &lt;regulon.bbh.xml.DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ExportRegulon(_in As String, _ref As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/regulon.export")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/ref " & """" & _ref & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Regulon.Reconstruct /bbh &lt;bbh.csv> /genome &lt;RegPrecise.genome.xml> /door &lt;operon.door> [/out &lt;outfile.csv>]
''' ```
''' </summary>
'''
Public Function RegulonReconstruct(_bbh As String, _genome As String, _door As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Regulon.Reconstruct")
Call CLI.Append(" ")
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
''' ```
''' /Regulon.Reconstruct2 /bbh &lt;bbh.csv> /genome &lt;RegPrecise.genome.DIR> /door &lt;operons.opr> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function RegulonReconstructs2(_bbh As String, _genome As String, _door As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Regulon.Reconstruct2")
Call CLI.Append(" ")
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
''' ```
''' /Regulon.Reconstructs /bbh &lt;bbh_EXPORT_csv.DIR> /genome &lt;RegPrecise.genome.DIR> [/door &lt;operon.door> /out &lt;outDIR>]
''' ```
''' Doing the regulon reconstruction job in batch mode.
''' </summary>
'''
Public Function RegulonReconstructs(_bbh As String, _genome As String, Optional _door As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Regulon.Reconstructs")
Call CLI.Append(" ")
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
''' ```
''' /Regulon.Test /in &lt;meme.txt> /reg &lt;genome.bbh.regulon.xml> /bbh &lt;maps.bbh.Csv>
''' ```
''' </summary>
'''
Public Function RegulonTest(_in As String, _reg As String, _bbh As String) As Integer
Dim CLI As New StringBuilder("/Regulon.Test")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/reg " & """" & _reg & """ ")
Call CLI.Append("/bbh " & """" & _bbh & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /RfamSites /source &lt;sourceDIR> [/out &lt;out.fastaDIR>]
''' ```
''' </summary>
'''
Public Function RfamSites(_source As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/RfamSites")
Call CLI.Append(" ")
Call CLI.Append("/source " & """" & _source & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /seq.logo /in &lt;meme.txt> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function SequenceLogoTask(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/seq.logo")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Similarity.Union /in &lt;preSource.fasta.DIR> /meme &lt;meme.txt.DIR> /hits &lt;similarity_hist.Csv> [/out &lt;out.DIR>]
''' ```
''' Motif iteration step 3
''' </summary>
'''
Public Function UnionSimilarity(_in As String, _meme As String, _hits As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/Similarity.Union")
Call CLI.Append(" ")
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
''' ```
''' /Site.MAST_Scan /mast &lt;mast.xml/DIR> [/batch /out &lt;out.csv>]
''' ```
''' [MAST.Xml] -> [SimpleSegment]
''' </summary>
'''
Public Function SiteMASTScan(_mast As String, Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Site.MAST_Scan")
Call CLI.Append(" ")
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
''' ```
''' /Site.MAST_Scan /mast &lt;mast.xml.DIR> [/out &lt;out.csv.DIR> /num_threads &lt;-1>]
''' ```
''' [MAST.Xml] -> [SimpleSegment]
''' </summary>
'''
Public Function SiteMASTScanBatch(_mast As String, Optional _out As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("/Site.MAST_Scan")
Call CLI.Append(" ")
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
''' ```
''' /Site.RegexScan /meme &lt;meme.txt> /nt &lt;nt.fasta> [/batch /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function SiteRegexScan(_meme As String, _nt As String, Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Site.RegexScan")
Call CLI.Append(" ")
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
''' ```
''' /site.scan /query &lt;LDM.xml> /subject &lt;subject.fasta> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function SiteScan(_query As String, _subject As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/site.scan")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /SiteHits.Footprints /sites &lt;MotifSiteHits.Csv> /bbh &lt;bbh.Csv> /meme &lt;meme.txt_DIR> /PTT &lt;genome.PTT> /DOOR &lt;DOOR.opr> [/queryHash /out &lt;out.csv>]
''' ```
''' Generates the regulation information.
''' </summary>
'''
Public Function SiteHitsToFootprints(_sites As String, _bbh As String, _meme As String, _PTT As String, _DOOR As String, Optional _out As String = "", Optional _queryhash As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SiteHits.Footprints")
Call CLI.Append(" ")
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
''' ```
''' /SWTOM.Compares /query &lt;query.meme.txt> /subject &lt;subject.meme.txt> [/out &lt;outDIR> /no-HTML]
''' ```
''' </summary>
'''
Public Function SWTomCompares(_query As String, _subject As String, Optional _out As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SWTOM.Compares")
Call CLI.Append(" ")
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
''' ```
''' /SWTOM.Compares.Batch /query &lt;query.meme.DIR> /subject &lt;subject.meme.DIR> [/out &lt;outDIR> /no-HTML]
''' ```
''' </summary>
'''
Public Function SWTomComparesBatch(_query As String, _subject As String, Optional _out As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SWTOM.Compares.Batch")
Call CLI.Append(" ")
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
''' ```
''' /SWTOM.LDM /query &lt;ldm.xml> /subject &lt;ldm.xml> [/out &lt;outDIR> /method &lt;pcc/ed/sw; default:=pcc>]
''' ```
''' </summary>
'''
Public Function SWTomLDM(_query As String, _subject As String, Optional _out As String = "", Optional _method As String = "") As Integer
Dim CLI As New StringBuilder("/SWTOM.LDM")
Call CLI.Append(" ")
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
''' ```
''' /SWTOM.Query /query &lt;meme.txt> [/out &lt;outDIR> /method &lt;pcc> /bits.level 1.6 /minW 6 /no-HTML]
''' ```
''' </summary>
'''
Public Function SWTomQuery(_query As String, Optional _out As String = "", Optional _method As String = "", Optional _bits_level As String = "", Optional _minw As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SWTOM.Query")
Call CLI.Append(" ")
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
''' ```
''' /SWTOM.Query.Batch /query &lt;meme.txt.DIR> [/out &lt;outDIR> /SW-offset 0.6 /method &lt;pcc> /bits.level 1.5 /minW 4 /SW-threshold 0.75 /tom-threshold 0.75 /no-HTML]
''' ```
''' </summary>
'''
Public Function SWTomQueryBatch(_query As String, Optional _out As String = "", Optional _sw_offset As String = "", Optional _method As String = "", Optional _bits_level As String = "", Optional _minw As String = "", Optional _sw_threshold As String = "", Optional _tom_threshold As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI As New StringBuilder("/SWTOM.Query.Batch")
Call CLI.Append(" ")
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
''' ```
''' /Tom.Query /query &lt;ldm.xml/meme.txt> [/out &lt;outDIR> /method &lt;pcc/ed; default:=pcc> /cost &lt;0.7> /threshold &lt;0.65> /meme]
''' ```
''' </summary>
'''
Public Function TomQuery(_query As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "", Optional _meme As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Tom.Query")
Call CLI.Append(" ")
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
''' ```
''' /Tom.Query.Batch /query &lt;inDIR> [/out &lt;outDIR> /method &lt;pcc/ed; default:=pcc> /cost 0.7 /threshold &lt;0.65>]
''' ```
''' </summary>
'''
Public Function TomQueryBatch(_query As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "") As Integer
Dim CLI As New StringBuilder("/Tom.Query.Batch")
Call CLI.Append(" ")
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
''' ```
''' /TomTOM /query &lt;meme.txt> /subject &lt;LDM.xml> [/out &lt;outDIR> /method &lt;pcc/ed; default:=pcc> /cost &lt;0.7> /threshold &lt;0.3>]
''' ```
''' </summary>
'''
Public Function TomTOMMethod(_query As String, _subject As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "") As Integer
Dim CLI As New StringBuilder("/TomTOM")
Call CLI.Append(" ")
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
''' ```
''' /TomTom.LDM /query &lt;ldm.xml> /subject &lt;ldm.xml> [/out &lt;outDIR> /method &lt;pcc/ed/sw; default:=sw> /cost &lt;0.7> /threshold &lt;0.65>]
''' ```
''' </summary>
'''
Public Function LDMTomTom(_query As String, _subject As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "") As Integer
Dim CLI As New StringBuilder("/TomTom.LDM")
Call CLI.Append(" ")
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
''' ```
''' /TomTOM.Similarity /in &lt;TOM_OUT.DIR> [/out &lt;out.Csv>]
''' ```
''' </summary>
'''
Public Function MEMEPlantSimilarity(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/TomTOM.Similarity")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /TOMTOM.Similarity.Batch /in &lt;inDIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function MEMEPlantSimilarityBatch(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/TOMTOM.Similarity.Batch")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /TomTom.Sites.Groups /in &lt;similarity.csv> /meme &lt;meme.DIR> [/grep &lt;regex> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function ExportTOMSites(_in As String, _meme As String, Optional _grep As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("/TomTom.Sites.Groups")
Call CLI.Append(" ")
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
''' ```
''' /Trim.MastSite /in &lt;mastSite.Csv> /locus &lt;locus_tag> /correlations &lt;DIR/name> [/out &lt;out.csv> /cut &lt;0.65>]
''' ```
''' </summary>
'''
Public Function Trim(_in As String, _locus As String, _correlations As String, Optional _out As String = "", Optional _cut As String = "") As Integer
Dim CLI As New StringBuilder("/Trim.MastSite")
Call CLI.Append(" ")
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
''' ```
''' /Trim.MEME.Dataset /in &lt;seq.fasta> [/out &lt;out.fasta> /minl 8 /distinct]
''' ```
''' Trim meme input data set for duplicated sequence and short seqeucne which its min length is smaller than the required min length.
''' </summary>
'''
Public Function TrimInputs(_in As String, Optional _out As String = "", Optional _minl As String = "", Optional _distinct As Boolean = False) As Integer
Dim CLI As New StringBuilder("/Trim.MEME.Dataset")
Call CLI.Append(" ")
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
''' ```
''' --build.Regulations /bbh &lt;regprecise.bbhMapped.csv> /mast &lt;mastSites.csv> [/cutoff &lt;0.6> /out &lt;out.csv> /sp &lt;spName> /DOOR &lt;genome.opr> /DOOR.extract]
''' ```
''' Genome wide step 2
''' </summary>
'''
Public Function Build(_bbh As String, _mast As String, Optional _cutoff As String = "", Optional _out As String = "", Optional _sp As String = "", Optional _door As String = "", Optional _door_extract As Boolean = False) As Integer
Dim CLI As New StringBuilder("--build.Regulations")
Call CLI.Append(" ")
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
''' ```
''' --build.Regulations.From.Motifs /bbh &lt;regprecise.bbhMapped.csv> /motifs &lt;motifSites.csv> [/cutoff &lt;0.6> /sp &lt;spName> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function BuildFromMotifSites(_bbh As String, _motifs As String, Optional _cutoff As String = "", Optional _sp As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--build.Regulations.From.Motifs")
Call CLI.Append(" ")
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
''' ```
''' --CExpr.WGCNA /mods &lt;CytoscapeNodes.txt> /genome &lt;genome.DIR|*.PTT;*.fna> /out &lt;DIR.out>
''' ```
''' </summary>
'''
Public Function WGCNAModsCExpr(_mods As String, _genome As String, _out As String) As Integer
Dim CLI As New StringBuilder("--CExpr.WGCNA")
Call CLI.Append(" ")
Call CLI.Append("/mods " & """" & _mods & """ ")
Call CLI.Append("/genome " & """" & _genome & """ ")
Call CLI.Append("/out " & """" & _out & """ ")


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
Public Function DownloadRegprecise2(Optional _work As String = "", Optional _save As String = "") As Integer
Dim CLI As New StringBuilder("Download.Regprecise")
Call CLI.Append(" ")
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
''' ```
''' --Dump.KEGG.Family /in &lt;in.fasta> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function KEGGFamilyDump(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--Dump.KEGG.Family")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --family.statics /sites &lt;motifSites.csv> /mods &lt;directory.kegg_modules>
''' ```
''' </summary>
'''
Public Function FamilyStatics(_sites As String, _mods As String) As Integer
Dim CLI As New StringBuilder("--family.statics")
Call CLI.Append(" ")
Call CLI.Append("/sites " & """" & _sites & """ ")
Call CLI.Append("/mods " & """" & _mods & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --Get.Intergenic /PTT &lt;genome.ptt> /nt &lt;genome.fasta> [/o &lt;out.fasta> /len 100 /strict]
''' ```
''' </summary>
'''
Public Function GetIntergenic(_PTT As String, _nt As String, Optional _o As String = "", Optional _len As String = "", Optional _strict As Boolean = False) As Integer
Dim CLI As New StringBuilder("--Get.Intergenic")
Call CLI.Append(" ")
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
''' ```
''' --GetFasta /bbh &lt;bbhh.csv> /id &lt;subject_id> /regprecise &lt;regprecise.fasta>
''' ```
''' </summary>
'''
Public Function GetFasta(_bbh As String, _id As String, _regprecise As String) As Integer
Dim CLI As New StringBuilder("--GetFasta")
Call CLI.Append(" ")
Call CLI.Append("/bbh " & """" & _bbh & """ ")
Call CLI.Append("/id " & """" & _id & """ ")
Call CLI.Append("/regprecise " & """" & _regprecise & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --hits.diff /query &lt;bbhh.csv> /subject &lt;bbhh.csv> [/reverse]
''' ```
''' </summary>
'''
Public Function DiffHits(_query As String, _subject As String, Optional _reverse As Boolean = False) As Integer
Dim CLI As New StringBuilder("--hits.diff")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")
If _reverse Then
Call CLI.Append("/reverse ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --Intersect.Max /query &lt;bbhh.csv> /subject &lt;bbhh.csv>
''' ```
''' </summary>
'''
Public Function MaxIntersection(_query As String, _subject As String) As Integer
Dim CLI As New StringBuilder("--Intersect.Max")
Call CLI.Append(" ")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --logo.Batch -in &lt;inDIR> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function LogoBatch(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--logo.Batch")
Call CLI.Append(" ")
Call CLI.Append("-in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --mapped-Back /meme &lt;meme.text> /mast &lt;mast.xml> /ptt &lt;genome.ptt> [/out &lt;out.csv> /offset &lt;10> /atg-dist &lt;250>]
''' ```
''' </summary>
'''
Public Function SiteMappedBack(_meme As String, _mast As String, _ptt As String, Optional _out As String = "", Optional _offset As String = "", Optional _atg_dist As String = "") As Integer
Dim CLI As New StringBuilder("--mapped-Back")
Call CLI.Append(" ")
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
''' ```
''' mast.compile /mast &lt;mast.xml> /ptt &lt;genome.ptt> [/no-meme /no-regInfo /p-value 1e-3 /mast-ldm &lt;DIR default:=GCModeller/Regprecise/MEME/MAST_LDM> /atg-dist 250]
''' ```
''' </summary>
'''
Public Function CompileMast(_mast As String, _ptt As String, Optional _p_value As String = "", Optional _mast_ldm As String = "", Optional _atg_dist As String = "", Optional _no_meme As Boolean = False, Optional _no_reginfo As Boolean = False) As Integer
Dim CLI As New StringBuilder("mast.compile")
Call CLI.Append(" ")
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
''' ```
''' mast.compile.bulk /source &lt;source_dir> [/ptt &lt;genome.ptt> /atg-dist &lt;500> /no-meme /no-regInfo /p-value 1e-3 /mast-ldm &lt;DIR default:=GCModeller/Regprecise/MEME/MAST_LDM> /related.all]
''' ```
''' Genome wide step 1
''' </summary>
'''
Public Function CompileMastBuck(_source As String, Optional _ptt As String = "", Optional _atg_dist As String = "", Optional _p_value As String = "", Optional _mast_ldm As String = "", Optional _no_meme As Boolean = False, Optional _no_reginfo As Boolean = False, Optional _related_all As Boolean = False) As Integer
Dim CLI As New StringBuilder("mast.compile.bulk")
Call CLI.Append(" ")
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
''' ```
''' --modules.regulates /in &lt;virtualfootprints.csv> [/out &lt;out.DIR> /mods &lt;KEGG_modules.DIR>]
''' ```
''' Exports the Venn diagram model for the module regulations.
''' </summary>
'''
Public Function ModuleRegulates(_in As String, Optional _out As String = "", Optional _mods As String = "") As Integer
Dim CLI As New StringBuilder("--modules.regulates")
Call CLI.Append(" ")
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
''' ```
''' Motif.Locates -ptt &lt;bacterial_genome.ptt> -meme &lt;meme.txt> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function MotifLocites(_ptt As String, _meme As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("Motif.Locates")
Call CLI.Append(" ")
Call CLI.Append("-ptt " & """" & _ptt & """ ")
Call CLI.Append("-meme " & """" & _meme & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' MotifScan -nt &lt;nt.fasta> /motif &lt;motifLDM.xml/LDM_Name/FamilyName> [/delta &lt;default:80> /delta2 &lt;default:70> /offSet &lt;default:5> /out &lt;saved.csv>]
''' ```
''' Scan for the motif site by using fragment similarity.
''' </summary>
'''
Public Function MotifScan(_nt As String, _motif As String, Optional _delta As String = "", Optional _delta2 As String = "", Optional _offset As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("MotifScan")
Call CLI.Append(" ")
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
''' ```
''' --pathway.regulates -footprints &lt;virtualfootprint.csv> /pathway &lt;DIR.KEGG.Pathways> [/out &lt;./PathwayRegulations/>]
''' ```
''' Associates of the pathway regulation information for the predicted virtual footprint information.
''' </summary>
'''
Public Function PathwayRegulations(_footprints As String, _pathway As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--pathway.regulates")
Call CLI.Append(" ")
Call CLI.Append("-footprints " & """" & _footprints & """ ")
Call CLI.Append("/pathway " & """" & _pathway & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' Regprecise.Compile [/src &lt;repository>]
''' ```
''' The repository parameter is a directory path which is the regprecise database root directory in the GCModeller directory, if you didn't know how to set this value, please leave it blank.
''' </summary>
'''
Public Function CompileRegprecise(Optional _src As String = "") As Integer
Dim CLI As New StringBuilder("Regprecise.Compile")
Call CLI.Append(" ")
If Not _src.StringEmpty Then
Call CLI.Append("/src " & """" & _src & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' regulators.bbh /bbh &lt;bbhDIR/bbh.index.Csv> [/save &lt;save.csv> /direct /regulons /maps &lt;genome.gb>]
''' ```
''' Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.
''' </summary>
'''
Public Function RegulatorsBBh(_bbh As String, Optional _save As String = "", Optional _maps As String = "", Optional _direct As Boolean = False, Optional _regulons As Boolean = False) As Integer
Dim CLI As New StringBuilder("regulators.bbh")
Call CLI.Append(" ")
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
''' ```
''' --site.Match /meme &lt;meme.text> /mast &lt;mast.xml> /out &lt;out.csv> [/ptt &lt;genome.ptt> /len &lt;150,200,250,300,350,400,450,500>]
''' ```
''' </summary>
'''
Public Function SiteMatch(_meme As String, _mast As String, _out As String, Optional _ptt As String = "", Optional _len As String = "") As Integer
Dim CLI As New StringBuilder("--site.Match")
Call CLI.Append(" ")
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
''' ```
''' --site.Matches /meme &lt;DIR.meme.text> /mast &lt;DIR.mast.xml> /out &lt;out.csv> [/ptt &lt;genome.ptt>]
''' ```
''' </summary>
'''
Public Function SiteMatches(_meme As String, _mast As String, _out As String, Optional _ptt As String = "") As Integer
Dim CLI As New StringBuilder("--site.Matches")
Call CLI.Append(" ")
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
''' ```
''' --site.Matches.text /meme &lt;DIR.meme.text> /mast &lt;DIR.mast.xml> /out &lt;out.csv> [/ptt &lt;genome.ptt> /fasta &lt;original.fasta.DIR>]
''' ```
''' Using this function for processing the meme text output from the tmod toolbox.
''' </summary>
'''
Public Function SiteMatchesText(_meme As String, _mast As String, _out As String, Optional _ptt As String = "", Optional _fasta As String = "") As Integer
Dim CLI As New StringBuilder("--site.Matches.text")
Call CLI.Append(" ")
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
''' ```
''' --site.stat /in &lt;footprints.csv> [/out &lt;out.csv>]
''' ```
''' Statics of the PCC correlation distribution of the regulation
''' </summary>
'''
Public Function SiteStat(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("--site.stat")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --TCS.Module.Regulations /MiST2 &lt;MiST2.xml> /footprint &lt;footprints.csv> /Pathways &lt;KEGG_Pathways.DIR>
''' ```
''' </summary>
'''
Public Function TCSRegulateModule(_MiST2 As String, _footprint As String, _Pathways As String) As Integer
Dim CLI As New StringBuilder("--TCS.Module.Regulations")
Call CLI.Append(" ")
Call CLI.Append("/MiST2 " & """" & _MiST2 & """ ")
Call CLI.Append("/footprint " & """" & _footprint & """ ")
Call CLI.Append("/Pathways " & """" & _Pathways & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --TCS.Regulations /TCS &lt;DIR.TCS.csv> /modules &lt;DIR.mod.xml> /regulations &lt;virtualfootprint.csv>
''' ```
''' </summary>
'''
Public Function TCSRegulations(_TCS As String, _modules As String, _regulations As String) As Integer
Dim CLI As New StringBuilder("--TCS.Regulations")
Call CLI.Append(" ")
Call CLI.Append("/TCS " & """" & _TCS & """ ")
Call CLI.Append("/modules " & """" & _modules & """ ")
Call CLI.Append("/regulations " & """" & _regulations & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' VirtualFootprint.DIP vf.csv &lt;csv> dip.csv &lt;csv>
''' ```
''' Associate the dip information with the Sigma 70 virtual footprints.
''' </summary>
'''
Public Function VirtualFootprintDIP(vf_csv As String, dip_csv As String) As Integer
Dim CLI As New StringBuilder("VirtualFootprint.DIP")
Call CLI.Append(" ")
Call CLI.Append("vf.csv " & """" & vf_csv & """ ")
Call CLI.Append("dip.csv " & """" & dip_csv & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' wGet.Regprecise [/repository-export &lt;dir.export, default: ./> /updates]
''' ```
''' Download Regprecise database from REST API
''' </summary>
'''
Public Function DownloadRegprecise(Optional _repository_export As String = "", Optional _updates As Boolean = False) As Integer
Dim CLI As New StringBuilder("wGet.Regprecise")
Call CLI.Append(" ")
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
