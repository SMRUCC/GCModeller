#Region "Microsoft.VisualBasic::d3ce6ece9e51e8fde3c0a2eb796bfd71, meme_suite\TmoD\TMod.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' interface and
    ' 
    ' 
    ' Module TMod
    ' 
    '     Properties: MEME
    ' 
    '     Function: __subSample, BatchMEMEScanning, (+2 Overloads) FastaSubSamples, InitializeSession, MotifSelect
    ' 
    '     Sub: __checkURL
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Parallel.ThreadTask
Imports SMRUCC.genomics.SequenceModel

''' <summary>
''' Tmod: toolbox of motif discovery, program assembly resources.
''' </summary>
''' 
<Cite(Title:="Tmod: toolbox of motif discovery", Year:=2010, Authors:="Sun, H.
Yuan, Y.
Wu, Y.
Liu, H.
Liu, J. S.
Xie, H.", DOI:="10.1093/bioinformatics/btp681", Journal:="Bioinformatics", Volume:=26, Issue:="3", Pages:="405-7",
      Abstract:="SUMMARY: Motif discovery is an important topic in computational transcriptional regulation studies. 
In the past decade, many researchers have contributed to the field and many de novo motif-finding tools have been developed, each may have a different strength. However, most of these tools do not have a user-friendly interface and their results are not easily comparable. 
We present a software called Toolbox of Motif Discovery (Tmod) for Windows operating systems. The current version of Tmod integrates 12 widely used motif discovery programs: MDscan, BioProspector, AlignACE, Gibbs Motif Sampler, MEME, CONSENSUS, MotifRegressor, GLAM, MotifSampler, SeSiMCMC, Weeder and YMF. 
Tmod provides a unified interface to ease the use of these programs and help users to understand the tuning parameters. It allows plug-in motif-finding programs to run either separately or in a batch mode with predetermined parameters, and provides a summary comprising of outputs from multiple programs. 
Tmod is developed in C++ with the support of Microsoft Foundation Classes and Cygwin. Tmod can also be easily expanded to include future algorithms. 
<p><p>AVAILABILITY: Tmod is available for download at http://www.fas.harvard.edu/~junliu/Tmod/.",
      AuthorAddress:="Department of Automatic Control, College of Mechatronics and Automation, National University of Defense Technology, Changsha, Hunan 410073, China.",
      ISSN:="1367-4811 (Electronic)
1367-4803 (Linking)",
      Keywords:="Algorithms
Amino Acid Motifs
Computational Biology/*methods
Sequence Analysis, DNA
Sequence Analysis, Protein
Sequence Analysis, RNA
*Software",
      PubMed:=20007740)>
<Package("Tools.Tmod",
                    Description:="Tmod: toolbox of motif discovery
                    
<br />
Motif discovery is an important topic in computational
transcriptional regulation studies. In the past decade, many
researchers have contributed to the field and many de novo motif-
finding tools have been developed, each may have a different
strength. However, most of these tools do not have a user-friendly
interface and their results are not easily comparable. We present
a software called Toolbox of Motif Discovery (Tmod) for Windows
operating systems. The current version of Tmod integrates 12 widely
used motif discovery programs: MDscan, BioProspector, AlignACE,
Gibbs Motif Sampler, MEME, CONSENSUS, MotifRegressor, GLAM,
MotifSampler, SeSiMCMC, Weeder and YMF. Tmod provides a
unified interface to ease the use of these programs and help
users to understand the tuning parameters. It allows plug-in motif-
finding programs to run either separately or in a batch mode with
predetermined parameters, and provides a summary comprising of
outputs from multiple programs. Tmod is developed in C++ with the
support of Microsoft Foundation Classes and Cygwin. Tmod can also
be easily expanded to include future algorithms.
Availability: Tmod is available for download at http://www.fas
.harvard.edu/∼junliu/Tmod/
Contact: xhwei65@nudt.edu.cn; jliu@stat.harvard.edu",
                    Publisher:="Hanchang Sun, Yuan Yuan, Yibo Wu, Hui Liu, Jun S. Liu and Hongwei Xie",
                    Revision:=256,
                    Url:="http://www.fas.harvard.edu/∼junliu/Tmod/",
                    Cites:="Sun, H., et al. (2010). ""Tmod: toolbox of motif discovery."" Bioinformatics 26(3): 405-407.
<p>SUMMARY: Motif discovery is an important topic in computational transcriptional regulation studies. In the past decade, many researchers have contributed to the field and many de novo motif-finding tools have been developed, each may have a different strength. However, most of these tools do not have a user-friendly interface and their results are not easily comparable. We present a software called Toolbox of Motif Discovery (Tmod) for Windows operating systems. The current version of Tmod integrates 12 widely used motif discovery programs: MDscan, BioProspector, AlignACE, Gibbs Motif Sampler, MEME, CONSENSUS, MotifRegressor, GLAM, MotifSampler, SeSiMCMC, Weeder and YMF. Tmod provides a unified interface to ease the use of these programs and help users to understand the tuning parameters. It allows plug-in motif-finding programs to run either separately or in a batch mode with predetermined parameters, and provides a summary comprising of outputs from multiple programs. Tmod is developed in C++ with the support of Microsoft Foundation Classes and Cygwin. Tmod can also be easily expanded to include future algorithms. AVAILABILITY: Tmod is available for download at http://www.fas.harvard.edu/~junliu/Tmod/.")>
Public Module TMod

    ''' <summary>
    ''' Initialize the tmod tools resource, this function will 
    ''' release the tmod program files into the GCModeller 
    ''' cache directory.
    ''' (释放内部的资源文件然后返回工作会话的路径)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    Public Function InitializeSession() As String
        Dim DIR As String = Settings.DataCache & "/TmoD/"

        On Error Resume Next

        Call My.Resources.adviser_exe.FlushStream(DIR & "adviser.exe")
        Call My.Resources.AlignACE_exe.FlushStream(DIR & "AlignACE.exe")
        Call My.Resources.BioProspector_exe.FlushStream(DIR & "BioProspector.exe")
        Call My.Resources.consensus_exe.FlushStream(DIR & "consensus.exe")
        Call My.Resources.CreateBackgroundModel_exe.FlushStream(DIR & "CreateBackgroundModel.exe")
        Call My.Resources.cyggcc_s_1_dll.FlushStream(DIR & "cyggcc_s-1.dll")
        Call My.Resources.cygstdc___6_dll.FlushStream(DIR & "cygstdc++-6.dll")
        Call My.Resources.cygwin1_dll.FlushStream(DIR & "cygwin1.dll")
        Call My.Resources.genomebg_exe.FlushStream(DIR & "genomebg.exe")
        Call My.Resources.Gibbs_exe.FlushStream(DIR & "Gibbs.exe")
        Call My.Resources.glam2_exe.FlushStream(DIR & "glam2.exe")
        Call My.Resources.glam2format_exe.FlushStream(DIR & "glam2format.exe")
        Call My.Resources.glam2mask_exe.FlushStream(DIR & "glam2mask.exe")
        Call My.Resources.glam2scan_exe.FlushStream(DIR & "glam2scan.exe")
        Call My.Resources.locator_exe.FlushStream(DIR & "locator.exe")
        Call My.Resources.MatrixScanner_exe.FlushStream(DIR & "MatrixScanner.exe")
        Call My.Resources.MDscan_exe.FlushStream(DIR & "MDscan.exe")
        Call My.Resources.meme_exe.FlushStream(DIR & "meme.exe")
        Call My.Resources.MotifSampler_exe.FlushStream(DIR & "MotifSampler.exe")
        Call My.Resources.purge_exe.FlushStream(DIR & "purge.exe")
        Call My.Resources.SeSiMCMC_exe.FlushStream(DIR & "SeSiMCMC.exe")
        Call My.Resources.weederlauncher_exe.FlushStream(DIR & "weederlauncher.exe")
        Call My.Resources.weederTFBS_exe.FlushStream(DIR & "weederTFBS.exe")

        Return DIR
    End Function

    ''' <summary>
    ''' The file path of the meme program from the tmod resource collection assembly. 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property MEME As String
        Get
            Return Settings.DataCache & "/TmoD/meme.exe"
        End Get
    End Property

    ''' <summary>
    ''' Batch execute the meme program for a collection of fasta file, the source is the 
    ''' directory location of the collection of fasta sequence file.
    ''' </summary>
    ''' <param name="inDIR">Fasta序列的存放文件</param>
    ''' <param name="outDIR"></param>
    ''' <param name="evt"></param>
    ''' <param name="num_motifs"></param>
    ''' <param name="mode"></param>
    ''' <param name="type">-dna / -protein</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 要在批处理中立即生效（只是临时的，生命力最弱）加一句： 
    ''' 直接用set命令：set path=%path%; 
    ''' 退出批处理后，环境变量恢复原来模样;
    ''' </remarks>
    Public Function BatchMEMEScanning(<Parameter("DIR.in")> inDIR As String,
                                      <Parameter("DIR.out")> outDIR As String,
                                      Optional evt As Double = 10,
                                      Optional num_motifs As Integer = 100,
                                      Optional mode As String = "zoops",
                                      <Parameter("Type", "-dna for DNA sequence, and -protein for protein sequence")>
                                      Optional type As String = "-dna",
                                      <Parameter("-maxw", "The maximum width of the motif searched.")>
                                      Optional maxw As Integer = 120) As String()

        Dim Resources As Dictionary(Of String, String) = inDIR.LoadSourceEntryList({"*.txt", "*.fasta", "*.fsa", "*.fa"})

        Call GetType(TMod).Assembly.FromAssembly.AppSummary("Tmod: toolbox of motif discovery", Nothing, App.StdOut)
        Call __checkURL(inDIR, outDIR, Resources)

        Dim envir = New KeyValuePair(Of String, String)() {New KeyValuePair(Of String, String)("MEME_DIRECTORY", Settings.DataCache & "/TmoD/")}
        Dim LQuery = Function(entry As KeyValuePair(Of String, String))
                         Dim out As String = String.Format("{0}/{1}.txt", outDIR, entry.Key)
                         Dim cmdl As String = $"""{entry.Value}"" {type} -mod {mode} -evt {evt} -nmotifs {num_motifs} -maxsize 1000000000 -maxw {maxw}"
                         Dim systemShell As New IORedirectFile(MEME, cmdl, environment:=envir)
                         Dim Exec As Integer = systemShell.Run
                         Dim stdOUT As String = systemShell.StandardOutput

                         Call stdOUT.SaveTo(out)

                         Return out
                     End Function

        Return ThreadTask(Of String) _
            .CreateThreads(Resources, LQuery) _
            .WithDegreeOfParallelism(App.CPUCoreNumbers) _
            .RunParallel _
            .ToArray
    End Function

    ''' <summary>
    ''' 由于%会造成bat文件出错，所以在这里进行检查然后提醒用户
    ''' </summary>
    ''' <param name="inDIR"></param>
    ''' <param name="outDIR"></param>
    ''' <param name="res"></param>
    Private Sub __checkURL(inDIR As String, outDIR As String, res As Dictionary(Of String, String))
        If inDIR.IndexOf("%"c) > -1 Then
            Throw New Exception(inDIR & " contains illegal ""%"" character in path!")
        End If
        If outDIR.IndexOf("%"c) > -1 Then
            Throw New Exception(outDIR & " contains illegal ""%"" character in path!")
        End If

        For Each path As String In res.Values
            If path.IndexOf("%"c) > -1 Then
                Throw New Exception(path & " contains illegal ""%"" character in path!")
            End If
        Next
    End Sub

    <Extension> Private Function __subSample(source As IEnumerable(Of FASTA.FastaSeq), n As Integer) As FASTA.FastaFile
        Dim data As FASTA.FastaSeq() = source.Shuffles
        Return New FASTA.FastaFile(data.Take(n))
    End Function

    ''' <summary>
    ''' The parameter n is the sequence object counts in each fasta file 
    ''' and the counts parameter is the total split file output counts.
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="n"></param>
    ''' <param name="Counts"></param>
    ''' <param name="EXPORT"></param>
    ''' <returns></returns>
    Public Function FastaSubSamples(source As FASTA.FastaFile,
                                    <Parameter("n", "The Numbers of fasta sequence in a sub-sampled fasta file.")>
                                    Optional n As Integer = 500,
                                    Optional Counts As Integer = 500,
                                    <Parameter("DIR.Export")> Optional EXPORT As String = "") As Integer
        Dim LQuery = (From i As Integer In Counts.Sequence.AsParallel
                      Select i,
                          fasta = source.__subSample(n)).ToArray
        Dim ID As String = BaseName(source.FilePath)
        Dim ASCII As System.Text.Encoding = System.Text.Encoding.ASCII

        If String.IsNullOrEmpty(EXPORT) Then
            EXPORT = source.FilePath & "_/"
        End If

        For Each ChunkBuffer In LQuery
            Dim path As String = EXPORT & ID & "_"
            If ChunkBuffer.i < 10 Then
                path = path & "000" & ChunkBuffer.i & ".fasta"
            ElseIf ChunkBuffer.i < 100 Then
                path = path & "00" & ChunkBuffer.i & ".fasta"
            ElseIf ChunkBuffer.i < 1000 Then
                path = path & "0" & ChunkBuffer.i & ".fasta"
            Else
                path = path & ChunkBuffer.i & ".fasta"
            End If

            Call ChunkBuffer.fasta.Save(path, ASCII)
        Next

        '可能还有一些没有被覆盖掉，则在这里讲这些对象取出来进行subsample
        Dim Sampled = (From fa As FASTA.FastaSeq
                       In (From item In LQuery Select item.fasta.ToArray).IteratesALL
                       Select fa.Title Distinct).ToArray
        Dim UnSampled = (From fa As FASTA.FastaSeq
                         In source.AsParallel
                         Where Array.IndexOf(Sampled, fa.Title) = -1
                         Select fa).ToArray

        Call $"There are {UnSampled.Count} object still unsampled, sub sample again...".__DEBUG_ECHO

        If UnSampled.Count <= n Then
            Call New FASTA.FastaFile(UnSampled).Save($"{EXPORT}{ID}_{CStr(Counts + 1)}.fasta", ASCII)
        Else
            LQuery = (From i As Integer In CInt((Counts / 10)).Sequence.AsParallel
                      Select i = i + Counts + 1,
                          fasta = UnSampled.__subSample(n)).ToArray

            For Each ChunkBuffer In LQuery
                Dim path As String = EXPORT & ID & "_"
                If ChunkBuffer.i < 10 Then
                    path = path & "000" & ChunkBuffer.i & ".fasta"
                ElseIf ChunkBuffer.i < 100 Then
                    path = path & "00" & ChunkBuffer.i & ".fasta"
                ElseIf ChunkBuffer.i < 1000 Then
                    path = path & "0" & ChunkBuffer.i & ".fasta"
                Else
                    path = path & ChunkBuffer.i & ".fasta"
                End If

                Call ChunkBuffer.fasta.Save(path, ASCII)
            Next
        End If

        Call Console.WriteLine("Job Done!")

        Return LQuery.Count
    End Function

    ''' <summary>
    ''' Select the fasta sequence from a fasta collection file which 
    ''' the fasta sequence its title contains a keyword in the 
    ''' <paramref name="lstLocus"></paramref>
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="lstLocus"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MotifSelect(source As FASTA.FastaFile,
                                <Parameter("id.list", "Using these keywords to search in the fasta collection.")>
                                lstLocus As IEnumerable(Of String)) As FASTA.FastaFile
        Dim LQuery = (From fa As FASTA.FastaSeq In source
                      Where (From s As String In lstLocus
                             Where InStr(fa.Headers.First, s, CompareMethod.Text) > 0
                             Select 1).Count > 0
                      Select fa).ToArray
        Return New FASTA.FastaFile(LQuery)
    End Function

    ''' <summary>
    ''' The parameter n is the sequence object counts in each fasta file 
    ''' and the counts parameter is the total split file output counts.
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="n"></param>
    ''' <param name="Counts"></param>
    ''' <param name="Export"></param>
    ''' <returns></returns>
    Public Function FastaSubSamples(
                                   <Parameter("Path.Fasta", "The fasta sequence file path.")> source As String,
                                   Optional n As Integer = 500,
                                   Optional Counts As Integer = 500,
                                   <Parameter("Dir.Export", "")> Optional Export As String = "") As Integer
        Return FastaSubSamples(FASTA.FastaFile.Read(source), n, Counts, Export)
    End Function
End Module
