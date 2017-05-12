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

        '''' <summary>
        ''''
        '''' </summary>
        ''''
        'Public Function align(_query As String, _subject As String, Optional _blosum As String = "", Optional _out As String = "") As Integer
        '    Dim CLI$ = $"/align /query ""{_query}"" /subject ""{_subject}"" /blosum ""{_blosum}"" /out ""{_out}"""
        '    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
        '    Return proc.Run()
        'End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function CAI(_ORF As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/CAI /ORF ""{_ORF}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function check_attrs(_in As String, _n As String, Optional _all As Boolean = False) As Integer
            Dim CLI$ = $"/check.attrs /in ""{_in}"" /n ""{_n}"" {If(_all, "/all", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Clustal_Cut(_in As String, Optional _left As String = "", Optional _right As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/Clustal.Cut /in ""{_in}"" /left ""{_left}"" /right ""{_right}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Compare_By_Locis(_file1 As String, _file2 As String) As Integer
            Dim CLI$ = $"/Compare.By.Locis /file1 ""{_file1}"" /file2 ""{_file2}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Count(_in As String) As Integer
            Dim CLI$ = $"/Count /in ""{_in}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Distinct fasta sequence by sequence content.
        ''' </summary>
        '''
        Public Function Distinct(_in As String, Optional _out As String = "", Optional _by_uid As String = "") As Integer
            Dim CLI$ = $"/Distinct /in ""{_in}"" /out ""{_out}"" /by_uid ""{_by_uid}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Convert the sequence data in a excel annotation file into a fasta sequence file.
        ''' </summary>
        '''
        Public Function Excel_2Fasta(_in As String, Optional _out As String = "", Optional _attrs As String = "", Optional _seq As String = "") As Integer
            Dim CLI$ = $"/Excel.2Fasta /in ""{_in}"" /out ""{_out}"" /attrs ""{_attrs}"" /seq ""{_seq}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Get_Locis(_in As String, _nt As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Get.Locis /in ""{_in}"" /nt ""{_nt}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Gff_Sites(_fna As String, _gff As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Gff.Sites /fna ""{_fna}"" /gff ""{_gff}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function gwANI(_in As String, Optional _out As String = "", Optional _fast As Boolean = False) As Integer
            Dim CLI$ = $"/gwANI /in ""{_in}"" /out ""{_out}"" {If(_fast, "/fast", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Testing
        ''' </summary>
        '''
        Public Function Loci_describ(_ptt As String, Optional _test As String = "", Optional _complement As Boolean = False, Optional _unstrand As Boolean = False) As Integer
            Dim CLI$ = $"/Loci.describ /ptt ""{_ptt}"" /test ""{_test}"" {If(_complement, "/complement", "")} {If(_unstrand, "/unstrand", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''* Drawing the sequence logo from the clustal alignment result.
        ''' </summary>
        '''
        Public Function logo(_in As String, Optional _out As String = "", Optional _title As String = "") As Integer
            Dim CLI$ = $"/logo /in ""{_in}"" /out ""{_out}"" /title ""{_title}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Only search for 1 level folder, dit not search receve.
        ''' </summary>
        '''
        Public Function Merge(_in As String, Optional _out As String = "", Optional _ext As String = "", Optional _trim As Boolean = False, Optional _unique As Boolean = False, Optional _brief As Boolean = False) As Integer
            Dim CLI$ = $"/Merge /in ""{_in}"" /out ""{_out}"" /ext ""{_ext}"" {If(_trim, "/trim", "")} {If(_unique, "/unique", "")} {If(_brief, "/brief", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''This tools just merge the fasta sequence into one larger file.
        ''' </summary>
        '''
        Public Function Merge_Simple(_in As String, Optional _exts As String = "", Optional _line_break As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/Merge.Simple /in ""{_in}"" /exts ""{_exts}"" /line.break ""{_line_break}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Mirror_Batch(_nt As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _num_threads As String = "", Optional _mp As Boolean = False) As Integer
            Dim CLI$ = $"/Mirror.Batch /nt ""{_nt}"" /out ""{_out}"" /min ""{_min}"" /max ""{_max}"" /num_threads ""{_num_threads}"" {If(_mp, "/mp", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Mirror_Fuzzy(_in As String, Optional _out As String = "", Optional _cut As String = "", Optional _max_dist As String = "", Optional _min As String = "", Optional _max As String = "") As Integer
            Dim CLI$ = $"/Mirror.Fuzzy /in ""{_in}"" /out ""{_out}"" /cut ""{_cut}"" /max-dist ""{_max_dist}"" /min ""{_min}"" /max ""{_max}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Mirror_Fuzzy_Batch(_in As String, Optional _out As String = "", Optional _cut As String = "", Optional _max_dist As String = "", Optional _min As String = "", Optional _max As String = "", Optional _num_threads As String = "") As Integer
            Dim CLI$ = $"/Mirror.Fuzzy.Batch /in ""{_in}"" /out ""{_out}"" /cut ""{_cut}"" /max-dist ""{_max_dist}"" /min ""{_min}"" /max ""{_max}"" /num_threads ""{_num_threads}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Mirror_Vector(_in As String, _size As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Mirror.Vector /in ""{_in}"" /size ""{_size}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''This function will convert the mirror data to the simple segment object data
        ''' </summary>
        '''
        Public Function Mirrors_Context(_in As String, _PTT As String, Optional _strand As String = "", Optional _out As String = "", Optional _dist As String = "", Optional _trans As Boolean = False, Optional _stranded As Boolean = False) As Integer
            Dim CLI$ = $"/Mirrors.Context /in ""{_in}"" /PTT ""{_PTT}"" /strand ""{_strand}"" /out ""{_out}"" /dist ""{_dist}"" {If(_trans, "/trans", "")} {If(_stranded, "/stranded", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''This function will convert the mirror data to the simple segment object data
        ''' </summary>
        '''
        Public Function Mirrors_Context_Batch(_in As String, _PTT As String, Optional _strand As String = "", Optional _out As String = "", Optional _dist As String = "", Optional _num_threads As String = "", Optional _trans As Boolean = False, Optional _stranded As Boolean = False) As Integer
            Dim CLI$ = $"/Mirrors.Context.Batch /in ""{_in}"" /PTT ""{_PTT}"" /strand ""{_strand}"" /out ""{_out}"" /dist ""{_dist}"" /num_threads ""{_num_threads}"" {If(_trans, "/trans", "")} {If(_stranded, "/stranded", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Mirrors_Group(_in As String, Optional _fuzzy As String = "", Optional _out As String = "", Optional _batch As Boolean = False) As Integer
            Dim CLI$ = $"/Mirrors.Group /in ""{_in}"" /fuzzy ""{_fuzzy}"" /out ""{_out}"" {If(_batch, "/batch", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Mirrors_Group_Batch(_in As String, Optional _fuzzy As String = "", Optional _out As String = "", Optional _num_threads As String = "") As Integer
            Dim CLI$ = $"/Mirrors.Group.Batch /in ""{_in}"" /fuzzy ""{_fuzzy}"" /out ""{_out}"" /num_threads ""{_num_threads}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Mirrors_Nt_Trim(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Mirrors.Nt.Trim /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function NeedlemanWunsch_NT(_query As String, _subject As String) As Integer
            Dim CLI$ = $"/NeedlemanWunsch.NT /query ""{_query}"" /subject ""{_subject}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''RunNeedlemanWunsch
        ''' </summary>
        '''
        Public Function nw(_query As String, _subject As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/nw /query ""{_query}"" /subject ""{_subject}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Palindrome_BatchTask(_in As String, Optional _num_threads As String = "", Optional _min As String = "", Optional _max As String = "", Optional _min_appears As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _partitions As String = "", Optional _out As String = "", Optional _palindrome As Boolean = False) As Integer
            Dim CLI$ = $"/Palindrome.BatchTask /in ""{_in}"" /num_threads ""{_num_threads}"" /min ""{_min}"" /max ""{_max}"" /min-appears ""{_min_appears}"" /cutoff ""{_cutoff}"" /max-dist ""{_max_dist}"" /partitions ""{_partitions}"" /out ""{_out}"" {If(_palindrome, "/palindrome", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Palindrome_Screen_MaxMatches(_in As String, _min As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Palindrome.Screen.MaxMatches /in ""{_in}"" /min ""{_min}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Palindrome_Screen_MaxMatches_Batch(_in As String, _min As String, Optional _out As String = "", Optional _num_threads As String = "") As Integer
            Dim CLI$ = $"/Palindrome.Screen.MaxMatches.Batch /in ""{_in}"" /min ""{_min}"" /out ""{_out}"" /num_threads ""{_num_threads}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Palindrome_Workflow(_in As String, Optional _min_appears As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _partitions As String = "", Optional _out As String = "", Optional _batch As Boolean = False, Optional _palindrome As Boolean = False) As Integer
            Dim CLI$ = $"/Palindrome.Workflow /in ""{_in}"" /min-appears ""{_min_appears}"" /min ""{_min}"" /max ""{_max}"" /cutoff ""{_cutoff}"" /max-dist ""{_max_dist}"" /partitions ""{_partitions}"" /out ""{_out}"" {If(_batch, "/batch", "")} {If(_palindrome, "/palindrome", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Promoter_Palindrome_Fasta(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Promoter.Palindrome.Fasta /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Promoter_Regions_Palindrome(_in As String, Optional _min As String = "", Optional _max As String = "", Optional _len As String = "", Optional _out As String = "", Optional _mirror As Boolean = False) As Integer
            Dim CLI$ = $"/Promoter.Regions.Palindrome /in ""{_in}"" /min ""{_min}"" /max ""{_max}"" /len ""{_len}"" /out ""{_out}"" {If(_mirror, "/mirror", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Promoter_Regions_Parser_gb(_gb As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Promoter.Regions.Parser.gb /gb ""{_gb}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Rule_dnaA_gyrB(_genome As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Rule.dnaA_gyrB /genome ""{_genome}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Rule_dnaA_gyrB_Matrix(_genomes As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Rule.dnaA_gyrB.Matrix /genomes ""{_genomes}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Screen_sites(_in As String, _range As String, Optional _type As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/Screen.sites /in ""{_in}"" /range ""{_range}"" /type ""{_type}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Select fasta sequence by local_tag.
        ''' </summary>
        '''
        Public Function Select_By_Locus(_in As String, _fa As String, Optional _field As String = "", Optional _out As String = "", Optional _reverse As Boolean = False) As Integer
            Dim CLI$ = $"/Select.By_Locus /in ""{_in}"" /fa ""{_fa}"" /field ""{_field}"" /out ""{_out}"" {If(_reverse, "/reverse", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Sigma(_in As String, Optional _out As String = "", Optional _round As String = "", Optional _simple As Boolean = False) As Integer
            Dim CLI$ = $"/Sigma /in ""{_in}"" /out ""{_out}"" /round ""{_round}"" {If(_simple, "/simple", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function SimpleSegment_AutoBuild(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/SimpleSegment.AutoBuild /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function SimpleSegment_Mirrors(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/SimpleSegment.Mirrors /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function SimpleSegment_Mirrors_Batch(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/SimpleSegment.Mirrors.Batch /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Converts the simple segment object collection as fasta file.
        ''' </summary>
        '''
        Public Function Sites2Fasta(_in As String, Optional _out As String = "", Optional _assemble As Boolean = False) As Integer
            Dim CLI$ = $"/Sites2Fasta /in ""{_in}"" /out ""{_out}"" {If(_assemble, "/assemble", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function SNP(_in As String, Optional _ref As String = "", Optional _high As String = "", Optional _pure As Boolean = False, Optional _monomorphic As Boolean = False) As Integer
            Dim CLI$ = $"/SNP /in ""{_in}"" /ref ""{_ref}"" /high ""{_high}"" {If(_pure, "/pure", "")} {If(_monomorphic, "/monomorphic", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Split(_in As String, Optional _n As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/Split /in ""{_in}"" /n ""{_n}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function subset(_lstID As String, _fa As String) As Integer
            Dim CLI$ = $"/subset /lstID ""{_lstID}"" /fa ""{_fa}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''The ongoing time mutation of the genome sequence.
        ''' </summary>
        '''
        Public Function Time_Mutation(_in As String, Optional _ref As String = "", Optional _out As String = "", Optional _cumulative As Boolean = False) As Integer
            Dim CLI$ = $"/Time.Mutation /in ""{_in}"" /ref ""{_ref}"" /out ""{_out}"" {If(_cumulative, "/cumulative", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Write_Seeds(_out As String, Optional _max As String = "", Optional _prot As Boolean = False) As Integer
            Dim CLI$ = $"/Write.Seeds /out ""{_out}"" /max ""{_max}"" {If(_prot, "/prot", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Polypeptide sequence 3 letters to 1 lettes sequence.
        ''' </summary>
        '''
        Public Function _321(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"-321 /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function align(_query As String, _subject As String, Optional _out As String = "", Optional _cost As String = "") As Integer
            Dim CLI$ = $"--align /query ""{_query}"" /subject ""{_subject}"" /out ""{_out}"" /cost ""{_cost}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function align_Self(_query As String, _out As String, Optional _cost As String = "") As Integer
            Dim CLI$ = $"--align.Self /query ""{_query}"" /out ""{_out}"" /cost ""{_cost}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function complement(_i As String, Optional _o As String = "") As Integer
            Dim CLI$ = $"-complement -i ""{_i}"" -o ""{_o}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Drawing_ClustalW(_in As String, Optional _out As String = "", Optional _dot_size As String = "") As Integer
            Dim CLI$ = $"--Drawing.ClustalW /in ""{_in}"" /out ""{_out}"" /dot.size ""{_dot_size}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Hairpinks(_in As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "") As Integer
            Dim CLI$ = $"--Hairpinks /in ""{_in}"" /out ""{_out}"" /min ""{_min}"" /max ""{_max}"" /cutoff ""{_cutoff}"" /max-dist ""{_max_dist}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Hairpinks_batch_task(_in As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _num_threads As String = "") As Integer
            Dim CLI$ = $"--Hairpinks.batch.task /in ""{_in}"" /out ""{_out}"" /min ""{_min}"" /max ""{_max}"" /cutoff ""{_cutoff}"" /max-dist ""{_max_dist}"" /num_threads ""{_num_threads}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function ImperfectsPalindrome_batch_Task(_in As String, _out As String, Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _num_threads As String = "") As Integer
            Dim CLI$ = $"--ImperfectsPalindrome.batch.Task /in ""{_in}"" /out ""{_out}"" /min ""{_min}"" /max ""{_max}"" /cutoff ""{_cutoff}"" /max-dist ""{_max_dist}"" /num_threads ""{_num_threads}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Mirror Palindrome, search from a fasta file.
        ''' </summary>
        '''
        Public Function Mirror_From_Fasta(_nt As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "") As Integer
            Dim CLI$ = $"--Mirror.From.Fasta /nt ""{_nt}"" /out ""{_out}"" /min ""{_min}"" /max ""{_max}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Mirror Palindrome, and this function is for the debugging test
        ''' </summary>
        '''
        Public Function Mirror_From_NT(_nt As String, _out As String, Optional _min As String = "", Optional _max As String = "") As Integer
            Dim CLI$ = $"--Mirror.From.NT /nt ""{_nt}"" /out ""{_out}"" /min ""{_min}"" /max ""{_max}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Palindrome_batch_Task(_in As String, _out As String, Optional _min As String = "", Optional _max As String = "", Optional _num_threads As String = "") As Integer
            Dim CLI$ = $"--Palindrome.batch.Task /in ""{_in}"" /out ""{_out}"" /min ""{_min}"" /max ""{_max}"" /num_threads ""{_num_threads}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Palindrome_From_FASTA(_nt As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "") As Integer
            Dim CLI$ = $"--Palindrome.From.Fasta /nt ""{_nt}"" /out ""{_out}"" /min ""{_min}"" /max ""{_max}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''This function is just for debugger test, /nt parameter is the nucleotide sequence data as ATGCCCC
        ''' </summary>
        '''
        Public Function Palindrome_From_NT(_nt As String, _out As String, Optional _min As String = "", Optional _max As String = "") As Integer
            Dim CLI$ = $"--Palindrome.From.NT /nt ""{_nt}"" /out ""{_out}"" /min ""{_min}"" /max ""{_max}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Palindrome_Imperfects(_in As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _partitions As String = "") As Integer
            Dim CLI$ = $"--Palindrome.Imperfects /in ""{_in}"" /out ""{_out}"" /min ""{_min}"" /max ""{_max}"" /cutoff ""{_cutoff}"" /max-dist ""{_max_dist}"" /partitions ""{_partitions}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Parsing the sequence segment from the sequence source using regular expression.
        ''' </summary>
        '''
        Public Function pattern_search(_i As String, _p As String, Optional _o As String = "", Optional _f As String = "") As Integer
            Dim CLI$ = $"-pattern_search -i ""{_i}"" -p ""{_p}"" -o ""{_o}"" -f ""{_f}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function PerfectPalindrome_Filtering(_in As String, Optional _min As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"--PerfectPalindrome.Filtering /in ""{_in}"" /min ""{_min}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Repeats_Density(_dir As String, _size As String, _ref As String, Optional _out As String = "", Optional _cutoff As String = "") As Integer
            Dim CLI$ = $"Repeats.Density /dir ""{_dir}"" /size ""{_size}"" /ref ""{_ref}"" /out ""{_out}"" /cutoff ""{_cutoff}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function reverse(_i As String, Optional _o As String = "") As Integer
            Dim CLI$ = $"-reverse -i ""{_i}"" -o ""{_o}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function rev_Repeats_Density(_dir As String, _size As String, _ref As String, Optional _out As String = "", Optional _cutoff As String = "") As Integer
            Dim CLI$ = $"rev-Repeats.Density /dir ""{_dir}"" /size ""{_size}"" /ref ""{_ref}"" /out ""{_out}"" /cutoff ""{_cutoff}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Batch search for repeats.
        ''' </summary>
        '''
        Public Function Search_Batch(_aln As String, Optional _min As String = "", Optional _max As String = "", Optional _min_rep As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"Search.Batch /aln ""{_aln}"" /min ""{_min}"" /max ""{_max}"" /min-rep ""{_min_rep}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function segment(_fasta As String, Optional _loci As String = "", Optional _length As String = "", Optional _right As String = "", Optional __reverse__ As String = "", Optional _geneid As String = "", Optional _dist As String = "", Optional _o As String = "", Optional __line_break As String = "", Optional _downstream_ As Boolean = False) As Integer
            Dim CLI$ = $"-segment /fasta ""{_fasta}"" -loci ""{_loci}"" /length ""{_length}"" /right ""{_right}"" [/reverse]] ""{__reverse__}"" /geneid ""{_geneid}"" /dist ""{_dist}"" -o ""{_o}"" [-line.break ""{__line_break}"" {If(_downstream_, "/downstream]", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function segments(_regions As String, _fasta As String, Optional _complement As Boolean = False, Optional _reversed As Boolean = False, Optional _brief_dump As Boolean = False) As Integer
            Dim CLI$ = $"--segments /regions ""{_regions}"" /fasta ""{_fasta}"" {If(_complement, "/complement", "")} {If(_reversed, "/reversed", "")} {If(_brief_dump, "/brief-dump", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function ToVector(_in As String, _min As String, _max As String, _out As String, _size As String) As Integer
            Dim CLI$ = $"--ToVector /in ""{_in}"" /min ""{_min}"" /max ""{_max}"" /out ""{_out}"" /size ""{_size}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.
        ''' </summary>
        '''
        Public Function translates(_orf As String, Optional _transl_table As String = "", Optional _force As Boolean = False) As Integer
            Dim CLI$ = $"--translates /orf ""{_orf}"" /transl_table ""{_transl_table}"" {If(_force, "/force", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Trim(_in As String, Optional _case As String = "", Optional _break As String = "", Optional _out As String = "", Optional _brief As Boolean = False) As Integer
            Dim CLI$ = $"--Trim /in ""{_in}"" /case ""{_case}"" /break ""{_break}"" /out ""{_out}"" {If(_brief, "/brief", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function
    End Class
End Namespace
