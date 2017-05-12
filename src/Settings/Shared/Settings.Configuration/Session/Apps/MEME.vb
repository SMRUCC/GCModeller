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
Dim CLI$ = $"/BBH.Select.Regulators /in ""{_in}"" /db ""{_db}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Build_FamilyDb(_prot As String, _pfam As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Build.FamilyDb /prot ""{_prot}"" /pfam ""{_pfam}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Copys(_in As String, Optional _out As String = "", Optional _file As String = "") As Integer
Dim CLI$ = $"/Copys /in ""{_in}"" /out ""{_out}"" /file ""{_file}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Copys_DIR(_in As String, _out As String) As Integer
Dim CLI$ = $"/Copys.DIR /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function CORN(_in As String, _mast As String, _PTT As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/CORN /in ""{_in}"" /mast ""{_mast}"" /PTT ""{_PTT}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function EXPORT_MotifDraws(_in As String, _MEME As String, _KEGG As String, Optional _out As String = "", Optional _pathway As Boolean = False) As Integer
Dim CLI$ = $"/EXPORT.MotifDraws /in ""{_in}"" /MEME ""{_MEME}"" /KEGG ""{_KEGG}"" /out ""{_out}"" {If(_pathway, "/pathway", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Motif iteration step 1
''' </summary>
'''
Public Function Export_MotifSites(_in As String, Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI$ = $"/Export.MotifSites /in ""{_in}"" /out ""{_out}"" {If(_batch, "/batch", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''This commandline tool have no argument parameters.
''' </summary>
'''
Public Function Export_Regprecise_Motifs() As Integer
Dim CLI$ = $"/Export.Regprecise.Motifs "
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Motif iteration step 2
''' </summary>
'''
Public Function Export_Similarity_Hits(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Export.Similarity.Hits /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''3 - Generates the regulation footprints.
''' </summary>
'''
Public Function Footprints(_footprints As String, _coor As String, _DOOR As String, _maps As String, Optional _out As String = "", Optional _cuts As String = "", Optional _extract As Boolean = False) As Integer
Dim CLI$ = $"/Footprints /footprints ""{_footprints}"" /coor ""{_coor}"" /DOOR ""{_DOOR}"" /maps ""{_maps}"" /out ""{_out}"" /cuts ""{_cuts}"" {If(_extract, "/extract", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''2
''' </summary>
'''
Public Function Hits_Context(_footprints As String, _PTT As String, Optional _out As String = "", Optional _regprecise As String = "") As Integer
Dim CLI$ = $"/Hits.Context /footprints ""{_footprints}"" /PTT ""{_PTT}"" /out ""{_out}"" /regprecise ""{_regprecise}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function LDM_Compares(_query As String, _sub As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/LDM.Compares /query ""{_query}"" /sub ""{_sub}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function LDM_MaxW(Optional _in As String = "") As Integer
Dim CLI$ = $"/LDM.MaxW /in ""{_in}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function LDM_Selects(_trace As String, _meme As String, Optional _out As String = "", Optional _named As Boolean = False) As Integer
Dim CLI$ = $"/LDM.Selects /trace ""{_trace}"" /meme ""{_meme}"" /out ""{_out}"" {If(_named, "/named", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function MAST_MotifMatches(_meme As String, _mast As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/MAST.MotifMatches /meme ""{_meme}"" /mast ""{_mast}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''1
''' </summary>
'''
Public Function MAST_MotifMatchs_Family(_meme As String, _mast As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/MAST.MotifMatchs.Family /meme ""{_meme}"" /mast ""{_mast}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function mast_Regulations(_in As String, _correlation As String, _DOOR As String, Optional _out As String = "", Optional _cut As String = "") As Integer
Dim CLI$ = $"/mast.Regulations /in ""{_in}"" /correlation ""{_correlation}"" /DOOR ""{_DOOR}"" /out ""{_out}"" /cut ""{_cut}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function MAST_LDM_Build(_source As String, Optional _out As String = "", Optional _evalue As String = "") As Integer
Dim CLI$ = $"/MAST_LDM.Build /source ""{_source}"" /out ""{_out}"" /evalue ""{_evalue}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Batch meme task by using tmod toolbox.
''' </summary>
'''
Public Function MEME_Batch(_in As String, Optional _out As String = "", Optional _evalue As String = "", Optional _nmotifs As String = "", Optional _mod As String = "", Optional _maxw As String = "") As Integer
Dim CLI$ = $"/MEME.Batch /in ""{_in}"" /out ""{_out}"" /evalue ""{_evalue}"" /nmotifs ""{_nmotifs}"" /mod ""{_mod}"" /maxw ""{_maxw}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function MEME_LDMs(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/MEME.LDMs /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Motif_BuildRegulons(_meme As String, _model As String, _DOOR As String, _maps As String, _corrs As String, Optional _cut As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Motif.BuildRegulons /meme ""{_meme}"" /model ""{_model}"" /DOOR ""{_DOOR}"" /maps ""{_maps}"" /corrs ""{_corrs}"" /cut ""{_cut}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Assign the phenotype information And genomic context Info for the motif sites. [SimpleSegment] -> [MotifLog]
''' </summary>
'''
Public Function Motif_Info(_loci As String, Optional _motifs As String = "", Optional _gff As String = "", Optional _atg_dist As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Motif.Info /loci ""{_loci}"" /motifs ""{_motifs}"" /gff ""{_gff}"" /atg-dist ""{_atg_dist}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''[SimpleSegment] -> [MotifLog]
''' </summary>
'''
Public Function Motif_Info_Batch(_in As String, _gffs As String, Optional _motifs As String = "", Optional _num_threads As String = "", Optional _atg_dist As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Motif.Info.Batch /in ""{_in}"" /gffs ""{_gffs}"" /motifs ""{_motifs}"" /num_threads ""{_num_threads}"" /atg-dist ""{_atg_dist}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Export of the calculation result from the tomtom program.
''' </summary>
'''
Public Function Motif_Similarity(_in As String, _motifs As String, Optional _out As String = "", Optional _bp_var As Boolean = False) As Integer
Dim CLI$ = $"/Motif.Similarity /in ""{_in}"" /motifs ""{_motifs}"" /out ""{_out}"" {If(_bp_var, "/bp.var", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function MotifHits_Regulation(_hits As String, _source As String, _PTT As String, _correlates As String, _bbh As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/MotifHits.Regulation /hits ""{_hits}"" /source ""{_source}"" /PTT ""{_PTT}"" /correlates ""{_correlates}"" /bbh ""{_bbh}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function MotifSites_Fasta(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/MotifSites.Fasta /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_DEGs(_degs As String, _PTT As String, _door As String, _out As String, Optional _log_fold As String = "") As Integer
Dim CLI$ = $"/Parser.DEGs /degs ""{_degs}"" /PTT ""{_PTT}"" /door ""{_door}"" /out ""{_out}"" /log-fold ""{_log_fold}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Locus(_locus As String, _PTT As String, _DOOR As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Parser.Locus /locus ""{_locus}"" /PTT ""{_PTT}"" /DOOR ""{_DOOR}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Log2(_in As String, _PTT As String, _DOOR As String, Optional _factor As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Parser.Log2 /in ""{_in}"" /PTT ""{_PTT}"" /DOOR ""{_DOOR}"" /factor ""{_factor}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_MAST(_sites As String, _ptt As String, _door As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Parser.MAST /sites ""{_sites}"" /ptt ""{_ptt}"" /door ""{_door}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Modules(_KEGG_Modules As String, _PTT As String, _DOOR As String, Optional _locus As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Parser.Modules /KEGG.Modules ""{_KEGG_Modules}"" /PTT ""{_PTT}"" /DOOR ""{_DOOR}"" /locus ""{_locus}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Operon(_in As String, _PTT As String, Optional _out As String = "", Optional _offset As String = "", Optional _family As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI$ = $"/Parser.Operon /in ""{_in}"" /PTT ""{_PTT}"" /out ""{_out}"" /offset ""{_offset}"" {If(_family, "/family", "")} {If(_all, "/all", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Pathway(_KEGG_Pathways As String, _PTT As String, _DOOR As String, Optional _locus As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Parser.Pathway /KEGG.Pathways ""{_KEGG_Pathways}"" /PTT ""{_PTT}"" /DOOR ""{_DOOR}"" /locus ""{_locus}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_RegPrecise_Operons(_operon As String, _PTT As String, Optional _door As String = "", Optional _id As String = "", Optional _locus As String = "", Optional _out As String = "", Optional _corn As Boolean = False) As Integer
Dim CLI$ = $"/Parser.RegPrecise.Operons /operon ""{_operon}"" /PTT ""{_PTT}"" /door ""{_door}"" /id ""{_id}"" /locus ""{_locus}"" /out ""{_out}"" {If(_corn, "/corn", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Regulon(_inDIR As String, _out As String, _PTT As String, Optional _door As String = "") As Integer
Dim CLI$ = $"/Parser.Regulon /inDIR ""{_inDIR}"" /out ""{_out}"" /PTT ""{_PTT}"" /door ""{_door}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Regulon_gb(_inDIR As String, _out As String, _gb As String, Optional _door As String = "") As Integer
Dim CLI$ = $"/Parser.Regulon.gb /inDIR ""{_inDIR}"" /out ""{_out}"" /gb ""{_gb}"" /door ""{_door}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Parser_Regulon_Merged(_in As String, _out As String, _PTT As String, Optional _door As String = "") As Integer
Dim CLI$ = $"/Parser.Regulon.Merged /in ""{_in}"" /out ""{_out}"" /PTT ""{_PTT}"" /door ""{_door}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Regulator_Motifs(_bbh As String, _regprecise As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Regulator.Motifs /bbh ""{_bbh}"" /regprecise ""{_regprecise}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Regulator_Motifs_Test(_hits As String, _motifs As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Regulator.Motifs.Test /hits ""{_hits}"" /motifs ""{_motifs}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Regprecise regulators data source compiler.
''' </summary>
'''
Public Function regulators_compile() As Integer
Dim CLI$ = $"/regulators.compile "
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function regulon_export(_in As String, _ref As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/regulon.export /in ""{_in}"" /ref ""{_ref}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Regulon_Reconstruct(_bbh As String, _genome As String, _door As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Regulon.Reconstruct /bbh ""{_bbh}"" /genome ""{_genome}"" /door ""{_door}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Regulon_Reconstruct2(_bbh As String, _genome As String, _door As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Regulon.Reconstruct2 /bbh ""{_bbh}"" /genome ""{_genome}"" /door ""{_door}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Doing the regulon reconstruction job in batch mode.
''' </summary>
'''
Public Function Regulon_Reconstructs(_bbh As String, _genome As String, Optional _door As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/Regulon.Reconstructs /bbh ""{_bbh}"" /genome ""{_genome}"" /door ""{_door}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Regulon_Test(_in As String, _reg As String, _bbh As String) As Integer
Dim CLI$ = $"/Regulon.Test /in ""{_in}"" /reg ""{_reg}"" /bbh ""{_bbh}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function RfamSites(_source As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/RfamSites /source ""{_source}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function seq_logo(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/seq.logo /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Motif iteration step 3
''' </summary>
'''
Public Function Similarity_Union(_in As String, _meme As String, _hits As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/Similarity.Union /in ""{_in}"" /meme ""{_meme}"" /hits ""{_hits}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''[MAST.Xml] -> [SimpleSegment]
''' </summary>
'''
Public Function Site_MAST_Scan(_mast As String, Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI$ = $"/Site.MAST_Scan /mast ""{_mast}"" /out ""{_out}"" {If(_batch, "/batch", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''[MAST.Xml] -> [SimpleSegment]
''' </summary>
'''
Public Function Site_MAST_Scan_Batch(_mast As String, Optional _out As String = "", Optional _num_threads As String = "") As Integer
Dim CLI$ = $"/Site.MAST_Scan /mast ""{_mast}"" /out ""{_out}"" /num_threads ""{_num_threads}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Site_RegexScan(_meme As String, _nt As String, Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI$ = $"/Site.RegexScan /meme ""{_meme}"" /nt ""{_nt}"" /out ""{_out}"" {If(_batch, "/batch", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function site_scan(_query As String, _subject As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/site.scan /query ""{_query}"" /subject ""{_subject}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Generates the regulation information.
''' </summary>
'''
Public Function SiteHits_Footprints(_sites As String, _bbh As String, _meme As String, _PTT As String, _DOOR As String, Optional _out As String = "", Optional _queryhash As Boolean = False) As Integer
Dim CLI$ = $"/SiteHits.Footprints /sites ""{_sites}"" /bbh ""{_bbh}"" /meme ""{_meme}"" /PTT ""{_PTT}"" /DOOR ""{_DOOR}"" /out ""{_out}"" {If(_queryhash, "/queryhash", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SWTOM_Compares(_query As String, _subject As String, Optional _out As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI$ = $"/SWTOM.Compares /query ""{_query}"" /subject ""{_subject}"" /out ""{_out}"" {If(_no_html, "/no-html", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SWTOM_Compares_Batch(_query As String, _subject As String, Optional _out As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI$ = $"/SWTOM.Compares.Batch /query ""{_query}"" /subject ""{_subject}"" /out ""{_out}"" {If(_no_html, "/no-html", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SWTOM_LDM(_query As String, _subject As String, Optional _out As String = "", Optional _method As String = "") As Integer
Dim CLI$ = $"/SWTOM.LDM /query ""{_query}"" /subject ""{_subject}"" /out ""{_out}"" /method ""{_method}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SWTOM_Query(_query As String, Optional _out As String = "", Optional _method As String = "", Optional _bits_level As String = "", Optional _minw As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI$ = $"/SWTOM.Query /query ""{_query}"" /out ""{_out}"" /method ""{_method}"" /bits.level ""{_bits_level}"" /minw ""{_minw}"" {If(_no_html, "/no-html", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function SWTOM_Query_Batch(_query As String, Optional _out As String = "", Optional _sw_offset As String = "", Optional _method As String = "", Optional _bits_level As String = "", Optional _minw As String = "", Optional _sw_threshold As String = "", Optional _tom_threshold As String = "", Optional _no_html As Boolean = False) As Integer
Dim CLI$ = $"/SWTOM.Query.Batch /query ""{_query}"" /out ""{_out}"" /sw-offset ""{_sw_offset}"" /method ""{_method}"" /bits.level ""{_bits_level}"" /minw ""{_minw}"" /sw-threshold ""{_sw_threshold}"" /tom-threshold ""{_tom_threshold}"" {If(_no_html, "/no-html", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Tom_Query(_query As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "", Optional _meme As Boolean = False) As Integer
Dim CLI$ = $"/Tom.Query /query ""{_query}"" /out ""{_out}"" /method ""{_method}"" /cost ""{_cost}"" /threshold ""{_threshold}"" {If(_meme, "/meme", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Tom_Query_Batch(_query As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "") As Integer
Dim CLI$ = $"/Tom.Query.Batch /query ""{_query}"" /out ""{_out}"" /method ""{_method}"" /cost ""{_cost}"" /threshold ""{_threshold}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TomTOM(_query As String, _subject As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "") As Integer
Dim CLI$ = $"/TomTOM /query ""{_query}"" /subject ""{_subject}"" /out ""{_out}"" /method ""{_method}"" /cost ""{_cost}"" /threshold ""{_threshold}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TomTom_LDM(_query As String, _subject As String, Optional _out As String = "", Optional _method As String = "", Optional _cost As String = "", Optional _threshold As String = "") As Integer
Dim CLI$ = $"/TomTom.LDM /query ""{_query}"" /subject ""{_subject}"" /out ""{_out}"" /method ""{_method}"" /cost ""{_cost}"" /threshold ""{_threshold}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TomTOM_Similarity(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/TomTOM.Similarity /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TOMTOM_Similarity_Batch(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"/TOMTOM.Similarity.Batch /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TomTom_Sites_Groups(_in As String, _meme As String, Optional _grep As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"/TomTom.Sites.Groups /in ""{_in}"" /meme ""{_meme}"" /grep ""{_grep}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Trim_MastSite(_in As String, _locus As String, _correlations As String, Optional _out As String = "", Optional _cut As String = "") As Integer
Dim CLI$ = $"/Trim.MastSite /in ""{_in}"" /locus ""{_locus}"" /correlations ""{_correlations}"" /out ""{_out}"" /cut ""{_cut}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Trim meme input data set for duplicated sequence and short seqeucne which its min length is smaller than the required min length.
''' </summary>
'''
Public Function Trim_MEME_Dataset(_in As String, Optional _out As String = "", Optional _minl As String = "", Optional _distinct As Boolean = False) As Integer
Dim CLI$ = $"/Trim.MEME.Dataset /in ""{_in}"" /out ""{_out}"" /minl ""{_minl}"" {If(_distinct, "/distinct", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Genome wide step 2
''' </summary>
'''
Public Function build_Regulations(_bbh As String, _mast As String, Optional _cutoff As String = "", Optional _out As String = "", Optional _sp As String = "", Optional _door As String = "", Optional _door_extract As Boolean = False) As Integer
Dim CLI$ = $"--build.Regulations /bbh ""{_bbh}"" /mast ""{_mast}"" /cutoff ""{_cutoff}"" /out ""{_out}"" /sp ""{_sp}"" /door ""{_door}"" {If(_door_extract, "/door.extract", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function build_Regulations_From_Motifs(_bbh As String, _motifs As String, Optional _cutoff As String = "", Optional _sp As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"--build.Regulations.From.Motifs /bbh ""{_bbh}"" /motifs ""{_motifs}"" /cutoff ""{_cutoff}"" /sp ""{_sp}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function CExpr_WGCNA(_mods As String, _genome As String, _out As String) As Integer
Dim CLI$ = $"--CExpr.WGCNA /mods ""{_mods}"" /genome ""{_genome}"" /out ""{_out}"""
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
Public Function Dump_KEGG_Family(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"--Dump.KEGG.Family /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function family_statics(_sites As String, _mods As String) As Integer
Dim CLI$ = $"--family.statics /sites ""{_sites}"" /mods ""{_mods}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Get_Intergenic(_PTT As String, _nt As String, Optional _o As String = "", Optional _len As String = "", Optional _strict As Boolean = False) As Integer
Dim CLI$ = $"--Get.Intergenic /PTT ""{_PTT}"" /nt ""{_nt}"" /o ""{_o}"" /len ""{_len}"" {If(_strict, "/strict", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function GetFasta(_bbh As String, _id As String, _regprecise As String) As Integer
Dim CLI$ = $"--GetFasta /bbh ""{_bbh}"" /id ""{_id}"" /regprecise ""{_regprecise}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function hits_diff(_query As String, _subject As String, Optional _reverse As Boolean = False) As Integer
Dim CLI$ = $"--hits.diff /query ""{_query}"" /subject ""{_subject}"" {If(_reverse, "/reverse", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Intersect_Max(_query As String, _subject As String) As Integer
Dim CLI$ = $"--Intersect.Max /query ""{_query}"" /subject ""{_subject}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function logo_Batch(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"--logo.Batch -in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function mapped_Back(_meme As String, _mast As String, _ptt As String, Optional _out As String = "", Optional _offset As String = "", Optional _atg_dist As String = "") As Integer
Dim CLI$ = $"--mapped-Back /meme ""{_meme}"" /mast ""{_mast}"" /ptt ""{_ptt}"" /out ""{_out}"" /offset ""{_offset}"" /atg-dist ""{_atg_dist}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function mast_compile(_mast As String, _ptt As String, Optional _p_value As String = "", Optional _mast_ldm As String = "", Optional _atg_dist As String = "", Optional _no_meme As Boolean = False, Optional _no_reginfo As Boolean = False) As Integer
Dim CLI$ = $"mast.compile /mast ""{_mast}"" /ptt ""{_ptt}"" /p-value ""{_p_value}"" /mast-ldm ""{_mast_ldm}"" /atg-dist ""{_atg_dist}"" {If(_no_meme, "/no-meme", "")} {If(_no_reginfo, "/no-reginfo", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Genome wide step 1
''' </summary>
'''
Public Function mast_compile_bulk(_source As String, Optional _ptt As String = "", Optional _atg_dist As String = "", Optional _p_value As String = "", Optional _mast_ldm As String = "", Optional _no_meme As Boolean = False, Optional _no_reginfo As Boolean = False, Optional _related_all As Boolean = False) As Integer
Dim CLI$ = $"mast.compile.bulk /source ""{_source}"" /ptt ""{_ptt}"" /atg-dist ""{_atg_dist}"" /p-value ""{_p_value}"" /mast-ldm ""{_mast_ldm}"" {If(_no_meme, "/no-meme", "")} {If(_no_reginfo, "/no-reginfo", "")} {If(_related_all, "/related.all", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Exports the Venn diagram model for the module regulations.
''' </summary>
'''
Public Function modules_regulates(_in As String, Optional _out As String = "", Optional _mods As String = "") As Integer
Dim CLI$ = $"--modules.regulates /in ""{_in}"" /out ""{_out}"" /mods ""{_mods}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function Motif_Locates(_ptt As String, _meme As String, Optional _out As String = "") As Integer
Dim CLI$ = $"Motif.Locates -ptt ""{_ptt}"" -meme ""{_meme}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Scan for the motif site by using fragment similarity.
''' </summary>
'''
Public Function MotifScan(_nt As String, _motif As String, Optional _delta As String = "", Optional _delta2 As String = "", Optional _offset As String = "", Optional _out As String = "") As Integer
Dim CLI$ = $"MotifScan -nt ""{_nt}"" /motif ""{_motif}"" /delta ""{_delta}"" /delta2 ""{_delta2}"" /offset ""{_offset}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Associates of the pathway regulation information for the predicted virtual footprint information.
''' </summary>
'''
Public Function pathway_regulates(_footprints As String, _pathway As String, Optional _out As String = "") As Integer
Dim CLI$ = $"--pathway.regulates -footprints ""{_footprints}"" /pathway ""{_pathway}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''The repository parameter is a directory path which is the regprecise database root directory in the GCModeller directory, if you didn't know how to set this value, please leave it blank.
''' </summary>
'''
Public Function Regprecise_Compile(Optional _src As String = "") As Integer
Dim CLI$ = $"Regprecise.Compile /src ""{_src}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.
''' </summary>
'''
Public Function regulators_bbh(_bbh As String, Optional _save As String = "", Optional _maps As String = "", Optional _direct As Boolean = False, Optional _regulons As Boolean = False) As Integer
Dim CLI$ = $"regulators.bbh /bbh ""{_bbh}"" /save ""{_save}"" /maps ""{_maps}"" {If(_direct, "/direct", "")} {If(_regulons, "/regulons", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function site_Match(_meme As String, _mast As String, _out As String, Optional _ptt As String = "", Optional _len As String = "") As Integer
Dim CLI$ = $"--site.Match /meme ""{_meme}"" /mast ""{_mast}"" /out ""{_out}"" /ptt ""{_ptt}"" /len ""{_len}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function site_Matches(_meme As String, _mast As String, _out As String, Optional _ptt As String = "") As Integer
Dim CLI$ = $"--site.Matches /meme ""{_meme}"" /mast ""{_mast}"" /out ""{_out}"" /ptt ""{_ptt}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Using this function for processing the meme text output from the tmod toolbox.
''' </summary>
'''
Public Function site_Matches_text(_meme As String, _mast As String, _out As String, Optional _ptt As String = "", Optional _fasta As String = "") As Integer
Dim CLI$ = $"--site.Matches.text /meme ""{_meme}"" /mast ""{_mast}"" /out ""{_out}"" /ptt ""{_ptt}"" /fasta ""{_fasta}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Statics of the PCC correlation distribution of the regulation
''' </summary>
'''
Public Function site_stat(_in As String, Optional _out As String = "") As Integer
Dim CLI$ = $"--site.stat /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TCS_Module_Regulations(_MiST2 As String, _footprint As String, _Pathways As String) As Integer
Dim CLI$ = $"--TCS.Module.Regulations /MiST2 ""{_MiST2}"" /footprint ""{_footprint}"" /Pathways ""{_Pathways}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''
''' </summary>
'''
Public Function TCS_Regulations(_TCS As String, _modules As String, _regulations As String) As Integer
Dim CLI$ = $"--TCS.Regulations /TCS ""{_TCS}"" /modules ""{_modules}"" /regulations ""{_regulations}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Associate the dip information with the Sigma 70 virtual footprints.
''' </summary>
'''
Public Function VirtualFootprint_DIP(vf_csv As String, dip_csv As String) As Integer
Dim CLI$ = $"VirtualFootprint.DIP vf.csv ""{vf_csv}"" dip.csv ""{dip_csv}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Download Regprecise database from REST API
''' </summary>
'''
Public Function wGet_Regprecise(Optional _repository_export As String = "", Optional _updates As Boolean = False) As Integer
Dim CLI$ = $"wGet.Regprecise /repository-export ""{_repository_export}"" {If(_updates, "/updates", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function
End Class
End Namespace
