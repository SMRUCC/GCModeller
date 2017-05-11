Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/localblast.exe

Namespace GCModellerApps


''' <summary>
'''Wrapper tools for the ncbi blast+ program and the blast output data analysis program. 
'''                  For running a large scale parallel alignment task, using ``/venn.BlastAll`` command for ``blastp`` and ``/blastn.Query.All`` command for ``blastn``.
''' </summary>
'''
Public Class localblast : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
'''Add locus_tag qualifier into the feature slot.
''' </summary>
'''
Public Function add_locus_tag(_gb As String, _prefix As String, Optional _out As String = "out.gb", Optional _add_gene As Boolean = False) As Integer
Dim CLI$ = $"/add.locus_tag /gb ""{_gb}"" /prefix ""{_prefix}"" /out ""{_out}"" {If(_add_gene, "/add.gene", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function add_names(_anno As String, _gb As String, Optional _out As String = "out.gbk", Optional _tag As String = "overrides_name") As Integer
Dim CLI$ = $"/add.names /anno ""{_anno}"" /gb ""{_gb}"" /out ""{_out}"" /tag ""{_tag}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function AlignmentTable_TopBest(_in As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/AlignmentTable.TopBest /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Bash_Venn(_blast As String, _inDIR As String, _inRef As String, Optional _out As String = "outDIR", Optional _evalue As String = "evalue:10") As Integer
Dim CLI$ = $"/Bash.Venn /blast ""{_blast}"" /inDIR ""{_inDIR}"" /inRef ""{_inRef}"" /out ""{_out}"" /evalue ""{_evalue}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Export bbh mapping result from the blastp raw output.
''' </summary>
'''
Public Function bbh_EXPORT(_query As String, _subject As String, Optional _out As String = "bbh.csv", Optional _evalue As String = "1e-3", Optional _coverage As String = "0.85", Optional _identities As String = "0.3", Optional _trim As Boolean = False) As Integer
Dim CLI$ = $"/bbh.EXPORT /query ""{_query}"" /subject ""{_subject}"" /out ""{_out}"" /evalue ""{_evalue}"" /coverage ""{_coverage}"" /identities ""{_identities}"" {If(_trim, "/trim", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function BBH_Merge(_in As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/BBH.Merge /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function BestHits_Filtering(_in As String, _sp As String, Optional _out As String = "out.Xml") As Integer
Dim CLI$ = $"/BestHits.Filtering /in ""{_in}"" /sp ""{_sp}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Blastn_Maps_Taxid(_in As String, _2taxid As String, Optional _tax As String = "NCBI_taxonomy:nodes/names", Optional _out As String = "out.csv", Optional _gi2taxid As Boolean = False, Optional _trim As Boolean = False) As Integer
Dim CLI$ = $"/Blastn.Maps.Taxid /in ""{_in}"" /2taxid ""{_2taxid}"" /tax ""{_tax}"" /out ""{_out}"" {If(_gi2taxid, "/gi2taxid", "")} {If(_trim, "/trim", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Using target fasta sequence query against all of the fasta sequence in target direcotry. This function is single thread.
''' </summary>
'''
Public Function blastn_Query(_query As String, _db As String, Optional _evalue As String = "1e-5", Optional _word_size As String = "-1", Optional _out As String = "out.DIR", Optional _thread As Boolean = False) As Integer
Dim CLI$ = $"/blastn.Query /query ""{_query}"" /db ""{_db}"" /evalue ""{_evalue}"" /word_size ""{_word_size}"" /out ""{_out}"" {If(_thread, "/thread", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Using the fasta sequence in a directory query against all of the sequence in another directory.
''' </summary>
'''
Public Function blastn_Query_All(_query As String, _db As String, Optional _evalue As String = "10", Optional _word_size As String = "-1", Optional _out As String = "out.DIR", Optional _penalty As String = "penalty", Optional _reward As String = "reward", Optional _skip_format As Boolean = False, Optional _parallel As Boolean = False) As Integer
Dim CLI$ = $"/blastn.Query.All /query ""{_query}"" /db ""{_db}"" /evalue ""{_evalue}"" /word_size ""{_word_size}"" /out ""{_out}"" /penalty ""{_penalty}"" /reward ""{_reward}"" {If(_skip_format, "/skip-format", "")} {If(_parallel, "/parallel", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function BlastnMaps_Match_Taxid(_in As String, _acc2taxid As String, Optional _out As String = "out.tsv") As Integer
Dim CLI$ = $"/BlastnMaps.Match.Taxid /in ""{_in}"" /acc2taxid ""{_acc2taxid}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function BlastnMaps_Select(_in As String, _data As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/BlastnMaps.Select /in ""{_in}"" /data ""{_data}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function BlastnMaps_Select_Top(_in As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/BlastnMaps.Select.Top /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function BlastnMaps_Summery(_in As String, Optional _split As String = ""-"", Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/BlastnMaps.Summery /in ""{_in}"" /split ""{_split}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Using query fasta invoke blastp against the fasta files in a directory.
'''               * This command tools required of NCBI blast+ suite, you must config the blast bin path by using ``settings.exe`` before running this command.
''' </summary>
'''
Public Function Blastp_BBH_Query(_query As String, _hit As String, Optional _out As String = "outDIR", Optional _num_threads As String = "-1", Optional _overrides As Boolean = False) As Integer
Dim CLI$ = $"/Blastp.BBH.Query /query ""{_query}"" /hit ""{_hit}"" /out ""{_out}"" /num_threads ""{_num_threads}"" {If(_overrides, "/overrides", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Chromosomes_Export(_reads As String, _maps As String, Optional _out As String = "outDIR") As Integer
Dim CLI$ = $"/Chromosomes.Export /reads ""{_reads}"" /maps ""{_maps}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function COG_myva(_blastp As String, _whog As String, Optional _out As String = "out.csv/txt", Optional _simple As Boolean = False) As Integer
Dim CLI$ = $"/COG.myva /blastp ""{_blastp}"" /whog ""{_whog}"" /out ""{_out}"" {If(_simple, "/simple", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function COG_Statics(_in As String, Optional _locus As String = "locus.txt/csv", Optional _locumap As String = "Gene", Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/COG.Statics /in ""{_in}"" /locus ""{_locus}"" /locumap ""{_locumap}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function COG2014_result(_sbh As String, _cog As String, Optional _cog_names As String = "cognames2003-2014.tab", Optional _out As String = "out.myva_cog.csv") As Integer
Dim CLI$ = $"/COG2014.result /sbh ""{_sbh}"" /cog ""{_cog}"" /cog.names ""{_cog_names}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Copy target type files from different sub directory into a directory.
''' </summary>
'''
Public Function Copy_Fasta(_imports As String, Optional _type As String = "faa,fna,ffn,fasta,...., default:=faa", Optional _out As String = "DIR") As Integer
Dim CLI$ = $"/Copy.Fasta /imports ""{_imports}"" /type ""{_type}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Copy_PTT(_in As String, Optional _out As String = "outDIR") As Integer
Dim CLI$ = $"/Copy.PTT /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Copys(_imports As String, Optional _out As String = "outDIR") As Integer
Dim CLI$ = $"/Copys /imports ""{_imports}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_AlignmentTable(_in As String, Optional _out As String = "outDIR/file", Optional _split As Boolean = False, Optional _header_split As Boolean = False) As Integer
Dim CLI$ = $"/Export.AlignmentTable /in ""{_in}"" /out ""{_out}"" {If(_split, "/split", "")} {If(_header_split, "/header.split", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_AlignmentTable_giList(_in As String, Optional _out As String = "gi.txt") As Integer
Dim CLI$ = $"/Export.AlignmentTable.giList /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_Blastn(_in As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/Export.Blastn /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_blastnMaps(_in As String, Optional _out As String = "out.csv", Optional _best As Boolean = False) As Integer
Dim CLI$ = $"/Export.blastnMaps /in ""{_in}"" /out ""{_out}"" {If(_best, "/best", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Multiple processor task.
''' </summary>
'''
Public Function Export_blastnMaps_Batch(_in As String, Optional _out As String = "out.DIR", Optional _num_threads As String = "-1", Optional _best As Boolean = False) As Integer
Dim CLI$ = $"/Export.blastnMaps.Batch /in ""{_in}"" /out ""{_out}"" /num_threads ""{_num_threads}"" {If(_best, "/best", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_blastnMaps_littles(_in As String, Optional _out As String = "out.csv.DIR") As Integer
Dim CLI$ = $"/Export.blastnMaps.littles /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Exports large amount of blastn output files and write all data into a specific csv file.
''' </summary>
'''
Public Function Export_blastnMaps_Write(_in As String, Optional _out As String = "write.csv", Optional _best As Boolean = False) As Integer
Dim CLI$ = $"/Export.blastnMaps.Write /in ""{_in}"" /out ""{_out}"" {If(_best, "/best", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_BlastX(_in As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/Export.BlastX /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function EXPORT_COGs_from_DOOR(_in As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/EXPORT.COGs.from.DOOR /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Export the *.fna, *.faa, *.ptt file from the gbk file.
''' </summary>
'''
Public Function Export_gb(_gb As String, Optional _out As String = "outDIR", Optional _simple As Boolean = False, Optional _batch As Boolean = False) As Integer
Dim CLI$ = $"/Export.gb /gb ""{_gb}"" /out ""{_out}"" {If(_simple, "/simple", "")} {If(_batch, "/batch", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_gb_genes(_gb As String, Optional _out As String = "out.fasta", Optional _genename As Boolean = False) As Integer
Dim CLI$ = $"/Export.gb.genes /gb ""{_gb}"" /out ""{_out}"" {If(_genename, "/genename", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_gpff(_in As String, _gff As String, Optional _out As String = "out.PTT") As Integer
Dim CLI$ = $"/Export.gpff /in ""{_in}"" /gff ""{_gff}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_gpffs(Optional _in As String = "inDIR") As Integer
Dim CLI$ = $"/Export.gpffs /in ""{_in}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_Locus(_in As String, Optional _out As String = "out.txt", Optional _hit As Boolean = False) As Integer
Dim CLI$ = $"/Export.Locus /in ""{_in}"" /out ""{_out}"" {If(_hit, "/hit", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Export all of the protein sequence from the genbank database file.
''' </summary>
'''
Public Function Export_Protein(_gb As String, Optional _out As String = "out.fasta") As Integer
Dim CLI$ = $"/Export.Protein /gb ""{_gb}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Filter the fasta sequence subset from a larger fasta database by using the regexp for match on the fasta title.
''' </summary>
'''
Public Function Fasta_Filters(_in As String, _key As String, Optional _out As String = "out.fasta", Optional _tokens As Boolean = False, Optional _p As Boolean = False) As Integer
Dim CLI$ = $"/Fasta.Filters /in ""{_in}"" /key ""{_key}"" /out ""{_out}"" {If(_tokens, "/tokens", "")} {If(_p, "/p", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Identities_Matrix(_hit As String, Optional _out As String = "out.csv", Optional _cut As String = "0.65") As Integer
Dim CLI$ = $"/Identities.Matrix /hit ""{_hit}"" /out ""{_out}"" /cut ""{_cut}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Config the ``prot2003-2014.fasta`` database for GCModeller localblast tools. This database will be using for the COG annotation. 
'''               This command required of the blast+ install first.
''' </summary>
'''
Public Function install_cog2003_2014(_db As String) As Integer
Dim CLI$ = $"/install.cog2003-2014 /db ""{_db}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function locus_Selects(_locus As String, _bh As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/locus.Selects /locus ""{_locus}"" /bh ""{_bh}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function MAT_evalue(_in As String, Optional _out As String = "mat.csv", Optional _flip As Boolean = False) As Integer
Dim CLI$ = $"/MAT.evalue /in ""{_in}"" /out ""{_out}"" {If(_flip, "/flip", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Merge_faa(_in As String, _out As String) As Integer
Dim CLI$ = $"/Merge.faa /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Paralog(_blastp As String, Optional _coverage As String = "0.5", Optional _identities As String = "0.3", Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/Paralog /blastp ""{_blastp}"" /coverage ""{_coverage}"" /identities ""{_identities}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Print(_in As String, Optional _ext As String = "ext", Optional _out As String = "out.Csv") As Integer
Dim CLI$ = $"/Print /in ""{_in}"" /ext ""{_ext}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Protein COG annotation by using NCBI cog2003-2014.fasta database.
''' </summary>
'''
Public Function query_cog2003_2014(_query As String, Optional _evalue As String = "1e-5", Optional _coverage As String = "0.65", Optional _identities As String = "0.85", Optional _out As String = "out.DIR", Optional _db As String = "cog2003-2014.fasta", Optional _blast_ As String = "blast+/bin", Optional _all As Boolean = False) As Integer
Dim CLI$ = $"/query.cog2003-2014 /query ""{_query}"" /evalue ""{_evalue}"" /coverage ""{_coverage}"" /identities ""{_identities}"" /out ""{_out}"" /db ""{_db}"" /blast+ ""{_blast_}"" {If(_all, "/all", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''If the blastnmapping data have the duplicated OTU tags, then this function will makes a copy of the duplicated OTU tag data. top-best data will not.
''' </summary>
'''
Public Function Reads_OTU_Taxonomy(_in As String, _OTU As String, _tax As String, Optional _out As String = "out.csv", Optional _fill_empty As Boolean = False) As Integer
Dim CLI$ = $"/Reads.OTU.Taxonomy /in ""{_in}"" /OTU ""{_OTU}"" /tax ""{_tax}"" /out ""{_out}"" {If(_fill_empty, "/fill.empty", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function ref_acc_list(_in As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/ref.acc.list /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function ref_gi_list(_in As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/ref.gi.list /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function SBH_BBH_Batch(_in As String, Optional _identities As String = "-1", Optional _coverage As String = "-1", Optional _out As String = "bbh.DIR", Optional _num_threads As String = "-1", Optional _all As Boolean = False) As Integer
Dim CLI$ = $"/SBH.BBH.Batch /in ""{_in}"" /identities ""{_identities}"" /coverage ""{_coverage}"" /out ""{_out}"" /num_threads ""{_num_threads}"" {If(_all, "/all", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Using this command for export the sbh result of your blastp raw data.
''' </summary>
'''
Public Function SBH_Export_Large(_in As String, Optional _out As String = "sbh.csv", Optional _identities As String = "0.15", Optional _coverage As String = "0.5", Optional _trim_kegg As Boolean = False) As Integer
Dim CLI$ = $"/SBH.Export.Large /in ""{_in}"" /out ""{_out}"" /identities ""{_identities}"" /coverage ""{_coverage}"" {If(_trim_kegg, "/trim-kegg", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function SBH_Trim(_in As String, _evalue As String, Optional _identities As String = "0.15", Optional _coverage As String = "0.5", Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/SBH.Trim /in ""{_in}"" /evalue ""{_evalue}"" /identities ""{_identities}"" /coverage ""{_coverage}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Export bbh result from the sbh pairs.
''' </summary>
'''
Public Function sbh2bbh(_qvs As String, _svq As String, Optional _identities As String = "-1", Optional _coverage As String = "-1", Optional _out As String = "bbh.csv", Optional _trim As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI$ = $"/sbh2bbh /qvs ""{_qvs}"" /svq ""{_svq}"" /identities ""{_identities}"" /coverage ""{_coverage}"" /out ""{_out}"" {If(_trim, "/trim", "")} {If(_all, "/all", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Select_Meta(_in As String, _bbh As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/Select.Meta /in ""{_in}"" /bbh ""{_bbh}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function SSBH2BH_LDM(_in As String, Optional _coverage As String = "0.8", Optional _identities As String = "0.3", Optional _out As String = "out.xml", Optional _xml As Boolean = False) As Integer
Dim CLI$ = $"/SSBH2BH_LDM /in ""{_in}"" /coverage ""{_coverage}"" /identities ""{_identities}"" /out ""{_out}"" {If(_xml, "/xml", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function SSDB_Export(_in As String, Optional _coverage As String = "0.8", Optional _identities As String = "0.3", Optional _out As String = "out.Xml") As Integer
Dim CLI$ = $"/SSDB.Export /in ""{_in}"" /coverage ""{_coverage}"" /identities ""{_identities}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Fetch the taxonomy information of the fasta sequence from NCBI web server.
''' </summary>
'''
Public Function Taxonomy_efetch(_in As String, Optional _out As String = "out.DIR") As Integer
Dim CLI$ = $"/Taxonomy.efetch /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Taxonomy_efetch_Merge(_in As String, Optional _out As String = "out.Csv") As Integer
Dim CLI$ = $"/Taxonomy.efetch.Merge /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''2. Build venn table And bbh data from the blastp result out Or sbh data cache.
''' </summary>
'''
Public Function venn_BBH(_imports As String, Optional _query As String = "queryName", Optional _coverage As String = "0.6", Optional _identities As String = "0.3", Optional _out As String = "outDIR", Optional _skip_load As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI$ = $"/venn.BBH /imports ""{_imports}"" /query ""{_query}"" /coverage ""{_coverage}"" /identities ""{_identities}"" /out ""{_out}"" {If(_skip_load, "/skip-load", "")} {If(_all, "/all", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Completely paired combos blastp bbh operations for the venn diagram Or network builder.
''' </summary>
'''
Public Function venn_BlastAll(_query As String, Optional _out As String = "outDIR", Optional _num_threads As String = "-1", Optional _evalue As String = "10", Optional _coverage As String = "0.8", Optional _identities As String = "0.3", Optional _overrides As Boolean = False, Optional _all As Boolean = False) As Integer
Dim CLI$ = $"/venn.BlastAll /query ""{_query}"" /out ""{_out}"" /num_threads ""{_num_threads}"" /evalue ""{_evalue}"" /coverage ""{_coverage}"" /identities ""{_identities}"" {If(_overrides, "/overrides", "")} {If(_all, "/all", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''1. [SBH_Batch] Creates the sbh cache data for the downstream bbh analysis. 
'''               And this batch function is suitable with any scale of the blastp sbh data output.
''' </summary>
'''
Public Function venn_cache(_imports As String, Optional _out As String = "sbh.out.DIR", Optional _coverage As String = "0.6", Optional _identities As String = "0.3", Optional _num_threads As String = "-1", Optional _overrides As Boolean = False) As Integer
Dim CLI$ = $"/venn.cache /imports ""{_imports}"" /out ""{_out}"" /coverage ""{_coverage}"" /identities ""{_identities}"" /num_threads ""{_num_threads}"" {If(_overrides, "/overrides", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function venn_sbh_thread(_in As String, Optional _out As String = "out.sbh.csv", Optional _coverage As String = "0.6", Optional _identities As String = "0.3", Optional _overrides As Boolean = False) As Integer
Dim CLI$ = $"/venn.sbh.thread /in ""{_in}"" /out ""{_out}"" /coverage ""{_coverage}"" /identities ""{_identities}"" {If(_overrides, "/overrides", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Venn_Single(_in As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"/Venn.Single /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Whog_XML(_in As String, Optional _out As String = "whog.XML") As Integer
Dim CLI$ = $"/Whog.XML /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Batch export bbh result data from a directory.
''' </summary>
'''
Public Function bbh_export(_in As String, Optional _out As String = "out.DIR", Optional _single_query As String = "queryName", Optional _coverage As String = "0.5", Optional _identities As String = "0.15", Optional _all As Boolean = False) As Integer
Dim CLI$ = $"--bbh.export /in ""{_in}"" /out ""{_out}"" /single-query ""{_single_query}"" /coverage ""{_coverage}"" /identities ""{_identities}"" {If(_all, "/all", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''In order to draw as venn diagram for a specific set of genome and study the diferrence and consists between these genomes, you should do the blast operation from the protein amino aciad sequence first. The blastp operation can be performenced by the blast+ program which you can download from the NCBI website, this command is a interop service for the NCBI blast program, you should install the blast+ program at first.
''' </summary>
'''
Public Function blast(_i As String, _blast_bin As String, _program As String, Optional _ld As String = "log_dir", Optional _xld As String = "xml_log_dir") As Integer
Dim CLI$ = $"blast -i ""{_i}"" -blast_bin ""{_blast_bin}"" -program ""{_program}"" -ld ""{_ld}"" -xld ""{_xld}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Query fasta query against itself for paralogs.
''' </summary>
'''
Public Function blast_self(_query As String, Optional _blast As String = "blast_HOME", Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"--blast.self /query ""{_query}"" /blast ""{_blast}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function copy(_i As String, _os As String, Optional _osidx As String = "id_column_index", Optional _os_skip_first As String = "T/F") As Integer
Dim CLI$ = $"-copy -i ""{_i}"" -os ""{_os}"" -osidx ""{_osidx}"" -os_skip_first ""{_os_skip_first}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_Fasta(_hits As String, _query As String, _subject As String) As Integer
Dim CLI$ = $"--Export.Fasta /hits ""{_hits}"" /query ""{_query}"" /subject ""{_subject}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_Overviews(_blast As String, Optional _out As String = "overview.csv") As Integer
Dim CLI$ = $"--Export.Overviews /blast ""{_blast}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Export_SBH(_in As String, _prefix As String, _out As String, Optional _txt As Boolean = False) As Integer
Dim CLI$ = $"--Export.SBH /in ""{_in}"" /prefix ""{_prefix}"" /out ""{_out}"" {If(_txt, "/txt", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function export_besthit(_i As String, _o As String) As Integer
Dim CLI$ = $"-export_besthit -i ""{_i}"" -o ""{_o}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''The gene id in the blast output log file are not well format for reading and program processing, so before you generate the venn diagram you should call this command to parse the gene id from the log file. You can also done this id parsing job using other tools.
''' </summary>
'''
Public Function grep(_i As String, _q As String, _h As String) As Integer
Dim CLI$ = $"grep -i ""{_i}"" -q ""{_q}"" -h ""{_h}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''Parsing the xml format blast log into a csv data file that use for venn diagram drawing.
''' </summary>
'''
Public Function logs_analysis(_d As String, _export As String) As Integer
Dim CLI$ = $"logs_analysis -d ""{_d}"" -export ""{_export}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>
'''This program can not use the blast parsing result for the venn diagram drawing operation, and this command is using for generate the drawing data for the venn diagram drawing command, this command merge the blast log parsing result and then using the parsing result for drawing a venn diagram.
''' </summary>
'''
Public Function merge(_d As String, _o As String) As Integer
Dim CLI$ = $"merge -d ""{_d}"" -o ""{_o}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function merge_besthit(_i As String, _o As String, _os As String, Optional _osidx As String = "id_column_index", Optional _os_skip_first As String = "T/F") As Integer
Dim CLI$ = $"-merge_besthit -i ""{_i}"" -o ""{_o}"" -os ""{_os}"" -osidx ""{_osidx}"" -os_skip_first ""{_os_skip_first}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Xml2Excel(_in As String, Optional _out As String = "out.csv") As Integer
Dim CLI$ = $"--Xml2Excel /in ""{_in}"" /out ""{_out}"""
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function

''' <summary>

''' </summary>
'''
Public Function Xml2Excel_Batch(_in As String, Optional _out As String = "outDIR", Optional _merge As Boolean = False) As Integer
Dim CLI$ = $"--Xml2Excel.Batch /in ""{_in}"" /out ""{_out}"" {If(_merge, "/merge", "")}"
Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
Return proc.Run()
End Function
End Class
End Namespace
