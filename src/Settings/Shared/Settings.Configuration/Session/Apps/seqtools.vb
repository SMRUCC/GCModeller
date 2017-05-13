Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/seqtools.exe

Namespace GCModellerApps


''' <summary>
''' Sequence operation utilities
''' </summary>
'''
Public Class seqtools : Inherits InteropService


Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
''' ```
''' /align.SmithWaterman /query &lt;query.fasta> /subject &lt;subject.fasta> [/blosum &lt;matrix.txt> /out &lt;out.xml>]
''' ```
''' </summary>
'''
Public Function Align2(_query As String, _subject As String, Optional _blosum As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("Align2")
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
''' ```
''' /CAI /ORF &lt;orf_nt.fasta> [/out &lt;out.XML>]
''' ```
''' </summary>
'''
Public Function CAI(_ORF As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("CAI")
Call CLI.Append("/ORF " & """" & _ORF & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /check.attrs /in &lt;in.fasta> /n &lt;attrs.count> [/all]
''' ```
''' </summary>
'''
Public Function CheckHeaders(_in As String, _n As String, Optional _all As Boolean = False) As Integer
Dim CLI As New StringBuilder("CheckHeaders")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/n " & """" & _n & """ ")
If _all Then
Call CLI.Append("/all ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Clustal.Cut /in &lt;in.fasta> [/left 0.1 /right 0.1 /out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function CutMlAlignment(_in As String, Optional _left As String = "", Optional _right As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("CutMlAlignment")
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
''' ```
''' /Compare.By.Locis /file1 &lt;file1.fasta> /file2 &lt;/file2.fasta>
''' ```
''' </summary>
'''
Public Function CompareFile(_file1 As String, _file2 As String) As Integer
Dim CLI As New StringBuilder("CompareFile")
Call CLI.Append("/file1 " & """" & _file1 & """ ")
Call CLI.Append("/file2 " & """" & _file2 & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Count /in &lt;data.fasta>
''' ```
''' </summary>
'''
Public Function Count(_in As String) As Integer
Dim CLI As New StringBuilder("Count")
Call CLI.Append("/in " & """" & _in & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Distinct /in &lt;in.fasta> [/out &lt;out.fasta> /by_Uid &lt;uid_regexp>]
''' ```
''' Distinct fasta sequence by sequence content.
''' </summary>
'''
Public Function Distinct(_in As String, Optional _out As String = "", Optional _by_uid As String = "") As Integer
Dim CLI As New StringBuilder("Distinct")
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
''' ```
''' /Excel.2Fasta /in &lt;anno.csv> [/out &lt;out.fasta> /attrs &lt;gene;locus_tag;gi;location,...> /seq &lt;Sequence>]
''' ```
''' Convert the sequence data in a excel annotation file into a fasta sequence file.
''' </summary>
'''
Public Function ToFasta(_in As String, Optional _out As String = "", Optional _attrs As String = "", Optional _seq As String = "") As Integer
Dim CLI As New StringBuilder("ToFasta")
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
''' ```
''' /Get.Locis /in &lt;locis.csv> /nt &lt;genome.nt.fasta> [/out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function GetSimpleSegments(_in As String, _nt As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("GetSimpleSegments")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/nt " & """" & _nt & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Gff.Sites /fna &lt;genomic.fna> /gff &lt;genome.gff> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function GffSites(_fna As String, _gff As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("GffSites")
Call CLI.Append("/fna " & """" & _fna & """ ")
Call CLI.Append("/gff " & """" & _gff & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /gwANI /in &lt;in.fasta> [/fast /out &lt;out.Csv>]
''' ```
''' </summary>
'''
Public Function gwANI(_in As String, Optional _out As String = "", Optional _fast As Boolean = False) As Integer
Dim CLI As New StringBuilder("gwANI")
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
''' ```
''' /Loci.describ /ptt &lt;genome-context.ptt> [/test &lt;loci:randomize> /complement /unstrand]
''' ```
''' Testing
''' </summary>
'''
Public Function LociDescript(_ptt As String, Optional _test As String = "", Optional _complement As Boolean = False, Optional _unstrand As Boolean = False) As Integer
Dim CLI As New StringBuilder("LociDescript")
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
''' ```
''' /logo /in &lt;clustal.fasta> [/out &lt;out.png> /title ""]
''' ```
''' * Drawing the sequence logo from the clustal alignment result.
''' </summary>
'''
Public Function SequenceLogo(_in As String, Optional _out As String = "", Optional _title As String = "") As Integer
Dim CLI As New StringBuilder("SequenceLogo")
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
''' ```
''' /Merge /in &lt;fasta.DIR> [/out &lt;out.fasta> /trim /unique /ext &lt;*.fasta> /brief]
''' ```
''' Only search for 1 level folder, dit not search receve.
''' </summary>
'''
Public Function Merge(_in As String, Optional _out As String = "", Optional _ext As String = "", Optional _trim As Boolean = False, Optional _unique As Boolean = False, Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("Merge")
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
''' ```
''' /Merge.Simple /in &lt;DIR> [/exts &lt;default:*.fasta,*.fa> /line.break 120 /out &lt;out.fasta>]
''' ```
''' This tools just merge the fasta sequence into one larger file.
''' </summary>
'''
Public Function SimpleMerge(_in As String, Optional _exts As String = "", Optional _line_break As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("SimpleMerge")
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
''' ```
''' /Mirror.Batch /nt &lt;nt.fasta> [/out &lt;out.csv> /mp /min &lt;3> /max &lt;20> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function MirrorBatch(_nt As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _num_threads As String = "", Optional _mp As Boolean = False) As Integer
Dim CLI As New StringBuilder("MirrorBatch")
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
''' ```
''' /Mirror.Fuzzy /in &lt;in.fasta> [/out &lt;out.csv> /cut 0.6 /max-dist 6 /min 3 /max 20]
''' ```
''' </summary>
'''
Public Function FuzzyMirrors(_in As String, Optional _out As String = "", Optional _cut As String = "", Optional _max_dist As String = "", Optional _min As String = "", Optional _max As String = "") As Integer
Dim CLI As New StringBuilder("FuzzyMirrors")
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
''' ```
''' /Mirror.Fuzzy.Batch /in &lt;in.fasta/DIR> [/out &lt;out.DIR> /cut 0.6 /max-dist 6 /min 3 /max 20 /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function FuzzyMirrorsBatch(_in As String, Optional _out As String = "", Optional _cut As String = "", Optional _max_dist As String = "", Optional _min As String = "", Optional _max As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("FuzzyMirrorsBatch")
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
''' ```
''' /Mirror.Vector /in &lt;inDIR> /size &lt;genome.size> [/out out.txt]
''' ```
''' </summary>
'''
Public Function MirrorsVector(_in As String, _size As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("MirrorsVector")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/size " & """" & _size & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Mirrors.Context /in &lt;mirrors.csv> /PTT &lt;genome.ptt> [/trans /strand &lt;+/-> /out &lt;out.csv> /stranded /dist &lt;500bp>]
''' ```
''' This function will convert the mirror data to the simple segment object data
''' </summary>
'''
Public Function MirrorContext(_in As String, _PTT As String, Optional _strand As String = "", Optional _out As String = "", Optional _dist As String = "", Optional _trans As Boolean = False, Optional _stranded As Boolean = False) As Integer
Dim CLI As New StringBuilder("MirrorContext")
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
''' ```
''' /Mirrors.Context.Batch /in &lt;mirrors.csv.DIR> /PTT &lt;genome.ptt.DIR> [/trans /strand &lt;+/-> /out &lt;out.csv> /stranded /dist &lt;500bp> /num_threads -1]
''' ```
''' This function will convert the mirror data to the simple segment object data
''' </summary>
'''
Public Function MirrorContextBatch(_in As String, _PTT As String, Optional _strand As String = "", Optional _out As String = "", Optional _dist As String = "", Optional _num_threads As String = "", Optional _trans As Boolean = False, Optional _stranded As Boolean = False) As Integer
Dim CLI As New StringBuilder("MirrorContextBatch")
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
''' ```
''' /Mirrors.Group /in &lt;mirrors.Csv> [/batch /fuzzy &lt;-1> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function MirrorGroups(_in As String, Optional _fuzzy As String = "", Optional _out As String = "", Optional _batch As Boolean = False) As Integer
Dim CLI As New StringBuilder("MirrorGroups")
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
''' ```
''' /Mirrors.Group.Batch /in &lt;mirrors.DIR> [/fuzzy &lt;-1> /out &lt;out.DIR> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function MirrorGroupsBatch(_in As String, Optional _fuzzy As String = "", Optional _out As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("MirrorGroupsBatch")
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
''' ```
''' /Mirrors.Nt.Trim /in &lt;mirrors.Csv> [/out &lt;out.Csv>]
''' ```
''' </summary>
'''
Public Function TrimNtMirrors(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("TrimNtMirrors")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /NeedlemanWunsch.NT /query &lt;nt> /subject &lt;nt>
''' ```
''' </summary>
'''
Public Function NWNT(_query As String, _subject As String) As Integer
Dim CLI As New StringBuilder("NWNT")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/subject " & """" & _subject & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /nw /query &lt;query.fasta> /subject &lt;subject.fasta> [/out &lt;out.txt>]
''' ```
''' RunNeedlemanWunsch
''' </summary>
'''
Public Function NW(_query As String, _subject As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("NW")
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
''' /Palindrome.BatchTask /in &lt;in.DIR> [/num_threads 4 /min 3 /max 20 /min-appears 2 /cutoff &lt;0.6> /Palindrome /max-dist &lt;1000 (bp)> /partitions &lt;-1> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function PalindromeBatchTask(_in As String, Optional _num_threads As String = "", Optional _min As String = "", Optional _max As String = "", Optional _min_appears As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _partitions As String = "", Optional _out As String = "", Optional _palindrome As Boolean = False) As Integer
Dim CLI As New StringBuilder("PalindromeBatchTask")
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
''' ```
''' /Palindrome.Screen.MaxMatches /in &lt;in.csv> /min &lt;min.max-matches> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function FilteringMatches(_in As String, _min As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("FilteringMatches")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/min " & """" & _min & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Palindrome.Screen.MaxMatches.Batch /in &lt;inDIR> /min &lt;min.max-matches> [/out &lt;out.DIR> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function FilteringMatchesBatch(_in As String, _min As String, Optional _out As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("FilteringMatchesBatch")
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
''' ```
''' /Palindrome.Workflow /in &lt;in.fasta> [/batch /min-appears 2 /min 3 /max 20 /cutoff &lt;0.6> /max-dist &lt;1000 (bp)> /Palindrome /partitions &lt;-1> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function PalindromeWorkflow(_in As String, Optional _min_appears As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _partitions As String = "", Optional _out As String = "", Optional _batch As Boolean = False, Optional _palindrome As Boolean = False) As Integer
Dim CLI As New StringBuilder("PalindromeWorkflow")
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
''' ```
''' /Promoter.Palindrome.Fasta /in &lt;palindrome.csv> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function PromoterPalindrome2Fasta(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("PromoterPalindrome2Fasta")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Promoter.Regions.Palindrome /in &lt;genbank.gb> [/min &lt;3> /max &lt;20> /len &lt;100,150,200,250,300,400,500, default:=250> /mirror /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function PromoterRegionPalindrome(_in As String, Optional _min As String = "", Optional _max As String = "", Optional _len As String = "", Optional _out As String = "", Optional _mirror As Boolean = False) As Integer
Dim CLI As New StringBuilder("PromoterRegionPalindrome")
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
''' ```
''' /Promoter.Regions.Parser.gb /gb &lt;genbank.gb> [/out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function PromoterRegionParser_gb(_gb As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("PromoterRegionParser_gb")
Call CLI.Append("/gb " & """" & _gb & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Rule.dnaA_gyrB /genome &lt;genbank.gb> [/out &lt;out.fasta>]
''' ```
''' </summary>
'''
Public Function dnaA_gyrB_rule(_genome As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("dnaA_gyrB_rule")
Call CLI.Append("/genome " & """" & _genome & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Rule.dnaA_gyrB.Matrix /genomes &lt;genomes.gb.DIR> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function RuleMatrix(_genomes As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("RuleMatrix")
Call CLI.Append("/genomes " & """" & _genomes & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Screen.sites /in &lt;DIR/sites.csv> /range &lt;min_bp>,&lt;max_bp> [/type &lt;type,default:=RepeatsView,alt:RepeatsView,RevRepeatsView,PalindromeLoci,ImperfectPalindrome> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ScreenRepeats(_in As String, _range As String, Optional _type As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("ScreenRepeats")
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
''' ```
''' /Select.By_Locus /in &lt;locus.txt/csv> /fa &lt;fasta/.inDIR> [/field &lt;columnName> /reverse /out &lt;out.fasta>]
''' ```
''' Select fasta sequence by local_tag.
''' </summary>
'''
Public Function SelectByLocus(_in As String, _fa As String, Optional _field As String = "", Optional _out As String = "", Optional _reverse As Boolean = False) As Integer
Dim CLI As New StringBuilder("SelectByLocus")
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
''' ```
''' /Sigma /in &lt;in.fasta> [/out &lt;out.Csv> /simple /round &lt;-1>]
''' ```
''' </summary>
'''
Public Function Sigma(_in As String, Optional _out As String = "", Optional _round As String = "", Optional _simple As Boolean = False) As Integer
Dim CLI As New StringBuilder("Sigma")
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
''' ```
''' /SimpleSegment.AutoBuild /in &lt;locis.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ConvertsAuto(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("ConvertsAuto")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /SimpleSegment.Mirrors /in &lt;in.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function ConvertMirrors(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("ConvertMirrors")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /SimpleSegment.Mirrors.Batch /in &lt;in.DIR> [/out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function ConvertMirrorsBatch(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("ConvertMirrorsBatch")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Sites2Fasta /in &lt;segments.csv> [/assemble /out &lt;out.fasta>]
''' ```
''' Converts the simple segment object collection as fasta file.
''' </summary>
'''
Public Function Sites2Fasta(_in As String, Optional _out As String = "", Optional _assemble As Boolean = False) As Integer
Dim CLI As New StringBuilder("Sites2Fasta")
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
''' ```
''' /SNP /in &lt;nt.fasta> [/ref &lt;int_index/title, default:0> /pure /monomorphic /high &lt;0.65>]
''' ```
''' </summary>
'''
Public Function SNP(_in As String, Optional _ref As String = "", Optional _high As String = "", Optional _pure As Boolean = False, Optional _monomorphic As Boolean = False) As Integer
Dim CLI As New StringBuilder("SNP")
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
''' ```
''' /Split /in &lt;in.fasta> [/n &lt;4096> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function Split(_in As String, Optional _n As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("Split")
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
''' ```
''' /subset /lstID &lt;lstID.txt> /fa &lt;source.fasta>
''' ```
''' </summary>
'''
Public Function SubSet(_lstID As String, _fa As String) As Integer
Dim CLI As New StringBuilder("SubSet")
Call CLI.Append("/lstID " & """" & _lstID & """ ")
Call CLI.Append("/fa " & """" & _fa & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Time.Mutation /in &lt;aln.fasta> [/ref &lt;default:first,other:title/index> /cumulative /out &lt;out.csv>]
''' ```
''' The ongoing time mutation of the genome sequence.
''' </summary>
'''
Public Function TimeDiffs(_in As String, Optional _ref As String = "", Optional _out As String = "", Optional _cumulative As Boolean = False) As Integer
Dim CLI As New StringBuilder("TimeDiffs")
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
''' ```
''' /Write.Seeds /out &lt;out.dat> [/prot /max &lt;20>]
''' ```
''' </summary>
'''
Public Function WriteSeeds(_out As String, Optional _max As String = "", Optional _prot As Boolean = False) As Integer
Dim CLI As New StringBuilder("WriteSeeds")
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
''' ```
''' -321 /in &lt;sequence.txt> [/out &lt;out.fasta>]
''' ```
''' Polypeptide sequence 3 letters to 1 lettes sequence.
''' </summary>
'''
Public Function PolypeptideBriefs(_in As String, Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("PolypeptideBriefs")
Call CLI.Append("/in " & """" & _in & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --align /query &lt;query.fasta> /subject &lt;subject.fasta> [/out &lt;out.DIR> /cost &lt;0.7>]
''' ```
''' </summary>
'''
Public Function Align(_query As String, _subject As String, Optional _out As String = "", Optional _cost As String = "") As Integer
Dim CLI As New StringBuilder("Align")
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
''' ```
''' --align.Self /query &lt;query.fasta> /out &lt;out.DIR> [/cost 0.75]
''' ```
''' </summary>
'''
Public Function AlignSelf(_query As String, _out As String, Optional _cost As String = "") As Integer
Dim CLI As New StringBuilder("AlignSelf")
Call CLI.Append("/query " & """" & _query & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
If Not _cost.StringEmpty Then
Call CLI.Append("/cost " & """" & _cost & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' -complement -i &lt;input_fasta> [-o &lt;output_fasta>]
''' ```
''' </summary>
'''
Public Function Complement(_i As String, Optional _o As String = "") As Integer
Dim CLI As New StringBuilder("Complement")
Call CLI.Append("-i " & """" & _i & """ ")
If Not _o.StringEmpty Then
Call CLI.Append("-o " & """" & _o & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --Drawing.ClustalW /in &lt;align.fasta> [/out &lt;out.png> /dot.Size 10]
''' ```
''' </summary>
'''
Public Function DrawClustalW(_in As String, Optional _out As String = "", Optional _dot_size As String = "") As Integer
Dim CLI As New StringBuilder("DrawClustalW")
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
''' ```
''' --Hairpinks /in &lt;in.fasta> [/out &lt;out.csv> /min &lt;6> /max &lt;7> /cutoff 3 /max-dist &lt;35 (bp)>]
''' ```
''' </summary>
'''
Public Function Hairpinks(_in As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "") As Integer
Dim CLI As New StringBuilder("Hairpinks")
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
''' ```
''' --Hairpinks.batch.task /in &lt;in.fasta> [/out &lt;outDIR> /min &lt;6> /max &lt;7> /cutoff &lt;0.6> /max-dist &lt;35 (bp)> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function HairpinksBatch(_in As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("HairpinksBatch")
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
''' ```
''' --ImperfectsPalindrome.batch.Task /in &lt;in.fasta> /out &lt;outDir> [/min &lt;3> /max &lt;20> /cutoff &lt;0.6> /max-dist &lt;1000 (bp)> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function BatchSearchImperfectsPalindrome(_in As String, _out As String, Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("BatchSearchImperfectsPalindrome")
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
''' ```
''' --Mirror.From.Fasta /nt &lt;nt-sequence.fasta> [/out &lt;out.csv> /min &lt;3> /max &lt;20>]
''' ```
''' Mirror Palindrome, search from a fasta file.
''' </summary>
'''
Public Function SearchMirrotFasta(_nt As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "") As Integer
Dim CLI As New StringBuilder("SearchMirrotFasta")
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
''' ```
''' --Mirror.From.NT /nt &lt;nt-sequence> /out &lt;out.csv> [/min &lt;3> /max &lt;20>]
''' ```
''' Mirror Palindrome, and this function is for the debugging test
''' </summary>
'''
Public Function SearchMirrotNT(_nt As String, _out As String, Optional _min As String = "", Optional _max As String = "") As Integer
Dim CLI As New StringBuilder("SearchMirrotNT")
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
''' ```
''' --Palindrome.batch.Task /in &lt;in.fasta> /out &lt;outDir> [/min &lt;3> /max &lt;20> /num_threads &lt;-1>]
''' ```
''' </summary>
'''
Public Function BatchSearchPalindrome(_in As String, _out As String, Optional _min As String = "", Optional _max As String = "", Optional _num_threads As String = "") As Integer
Dim CLI As New StringBuilder("BatchSearchPalindrome")
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
''' ```
''' --Palindrome.From.Fasta /nt &lt;nt-sequence.fasta> [/out &lt;out.csv> /min &lt;3> /max &lt;20>]
''' ```
''' </summary>
'''
Public Function SearchPalindromeFasta(_nt As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "") As Integer
Dim CLI As New StringBuilder("SearchPalindromeFasta")
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
''' ```
''' --Palindrome.From.NT /nt &lt;nt-sequence> /out &lt;out.csv> [/min &lt;3> /max &lt;20>]
''' ```
''' This function is just for debugger test, /nt parameter is the nucleotide sequence data as ATGCCCC
''' </summary>
'''
Public Function SearchPalindromeNT(_nt As String, _out As String, Optional _min As String = "", Optional _max As String = "") As Integer
Dim CLI As New StringBuilder("SearchPalindromeNT")
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
''' ```
''' --Palindrome.Imperfects /in &lt;in.fasta> [/out &lt;out.csv> /min &lt;3> /max &lt;20> /cutoff &lt;0.6> /max-dist &lt;1000 (bp)> /partitions &lt;-1>]
''' ```
''' </summary>
'''
Public Function ImperfectPalindrome(_in As String, Optional _out As String = "", Optional _min As String = "", Optional _max As String = "", Optional _cutoff As String = "", Optional _max_dist As String = "", Optional _partitions As String = "") As Integer
Dim CLI As New StringBuilder("ImperfectPalindrome")
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
''' ```
''' -pattern_search -i &lt;file_name> -p &lt;regex_pattern>[ -o &lt;output_directory> -f &lt;format:fsa/gbk>]
''' ```
''' Parsing the sequence segment from the sequence source using regular expression.
''' </summary>
'''
Public Function PatternSearchA(_i As String, _p As String, Optional _o As String = "", Optional _f As String = "") As Integer
Dim CLI As New StringBuilder("PatternSearchA")
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
''' ```
''' --PerfectPalindrome.Filtering /in &lt;inDIR> [/min &lt;8> /out &lt;outDIR>]
''' ```
''' </summary>
'''
Public Function FilterPerfectPalindrome(_in As String, Optional _min As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("FilterPerfectPalindrome")
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
''' ```
''' Repeats.Density /dir &lt;dir> /size &lt;size> /ref &lt;refName> [/out &lt;out.csv> /cutoff &lt;default:=0>]
''' ```
''' </summary>
'''
Public Function RepeatsDensity(_dir As String, _size As String, _ref As String, Optional _out As String = "", Optional _cutoff As String = "") As Integer
Dim CLI As New StringBuilder("RepeatsDensity")
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
''' ```
''' -reverse -i &lt;input_fasta> [-o &lt;output_fasta>]
''' ```
''' </summary>
'''
Public Function Reverse(_i As String, Optional _o As String = "") As Integer
Dim CLI As New StringBuilder("Reverse")
Call CLI.Append("-i " & """" & _i & """ ")
If Not _o.StringEmpty Then
Call CLI.Append("-o " & """" & _o & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' rev-Repeats.Density /dir &lt;dir> /size &lt;size> /ref &lt;refName> [/out &lt;out.csv> /cutoff &lt;default:=0>]
''' ```
''' </summary>
'''
Public Function revRepeatsDensity(_dir As String, _size As String, _ref As String, Optional _out As String = "", Optional _cutoff As String = "") As Integer
Dim CLI As New StringBuilder("revRepeatsDensity")
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
''' ```
''' Search.Batch /aln &lt;alignment.fasta> [/min 3 /max 20 /min-rep 2 /out &lt;./>]
''' ```
''' Batch search for repeats.
''' </summary>
'''
Public Function BatchSearch(_aln As String, Optional _min As String = "", Optional _max As String = "", Optional _min_rep As String = "", Optional _out As String = "") As Integer
Dim CLI As New StringBuilder("BatchSearch")
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
''' ```
''' -segment /fasta &lt;Fasta_Token> [-loci &lt;loci>] [/left &lt;left> /length &lt;length> /right &lt;right> [/reverse]] [/ptt &lt;ptt> /geneID &lt;gene_id> /dist &lt;distance> /downstream] -o &lt;saved> [-line.break 100]
''' ```
''' </summary>
'''
Public Function GetSegment(_fasta As String, Optional _loci As String = "", Optional _length As String = "", Optional _right As String = "", Optional __reverse__ As String = "", Optional _geneid As String = "", Optional _dist As String = "", Optional _o As String = "", Optional __line_break As String = "", Optional _downstream_ As Boolean = False) As Integer
Dim CLI As New StringBuilder("GetSegment")
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
''' ```
''' --segments /regions &lt;regions.csv> /fasta &lt;nt.fasta> [/complement /reversed /brief-dump]
''' ```
''' </summary>
'''
Public Function GetSegments(_regions As String, _fasta As String, Optional _complement As Boolean = False, Optional _reversed As Boolean = False, Optional _brief_dump As Boolean = False) As Integer
Dim CLI As New StringBuilder("GetSegments")
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
''' ```
''' --ToVector /in &lt;in.DIR> /min &lt;4> /max &lt;8> /out &lt;out.txt> /size &lt;genome.size>
''' ```
''' </summary>
'''
Public Function ToVector(_in As String, _min As String, _max As String, _out As String, _size As String) As Integer
Dim CLI As New StringBuilder("ToVector")
Call CLI.Append("/in " & """" & _in & """ ")
Call CLI.Append("/min " & """" & _min & """ ")
Call CLI.Append("/max " & """" & _max & """ ")
Call CLI.Append("/out " & """" & _out & """ ")
Call CLI.Append("/size " & """" & _size & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' --translates /orf &lt;orf.fasta> [/transl_table 1 /force]
''' ```
''' Translates the ORF gene as protein sequence. If any error was output from the console, please using > operator dump the output to a log file for the analysis.
''' </summary>
'''
Public Function Translates(_orf As String, Optional _transl_table As String = "", Optional _force As Boolean = False) As Integer
Dim CLI As New StringBuilder("Translates")
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
''' ```
''' --Trim /in &lt;in.fasta> [/case &lt;u/l> /break &lt;-1/int> /out &lt;out.fasta> /brief]
''' ```
''' </summary>
'''
Public Function Trim(_in As String, Optional _case As String = "", Optional _break As String = "", Optional _out As String = "", Optional _brief As Boolean = False) As Integer
Dim CLI As New StringBuilder("Trim")
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
