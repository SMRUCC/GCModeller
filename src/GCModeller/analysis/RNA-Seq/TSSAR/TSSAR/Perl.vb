#Region "Microsoft.VisualBasic::8f01d168a7dc04d4b142eadb43ca0a0e, analysis\RNA-Seq\TSSAR\TSSAR\Perl.vb"

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

    ' Module Perl
    ' 
    '     Function: Invoke, LoadFastaq, LoadSAM, Located, SaveAlignmentReadsMapping
    '     Class LocatedAlignment
    ' 
    '         Properties: BitwiseFLAG, CIGAR, MappingPosition, MapQuality, PosNext
    '                     QueryTemplateName, RefName, RefNext, SegmentDirection
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.genomics.SequenceModel.SAM

''' <summary>
''' dRNA-seq
''' 
''' TSSAR Perl invoke module
''' 
''' DESCRIPTION
''' *T*ranscription *S*tart *S*ites *A*nnotation *R*egime for dRNA-seq data,
''' based on a Skellam distribution with parameter estimation by
''' zero-inflated-poisson model regression analysis. The input are two
''' mapped sequencing files in SAM file formate (library[+] and library[-]),
''' the output is a *.BED file with an entry for each position which is
''' annotated as a TSS, writen to STDOUT. Addtionally, a file named Dump.bed
''' is created. It specifies regions where the applied regression model does
''' not converge. Hence, those regions are omitted from analysis.
''' </summary>
''' <remarks>
''' 
''' CONSIDERATIONS
'''  This is only a beta-version which was not thoroughly tested.
'''
''' VERSION
'''  Version 0.9.6 beta -- Distribution is modeled locally, by assuming a
'''  mixed model between
'''
'''  Poisson-Part -> Transcribed Region (sampling zeros)
'''
'''  Zero-Part -> Not Transcribed Region (structural zeros)
'''
'''  The Poisson-Part is seperated from the Zero-Part by
'''  Zero-Inflated-Poisson-Model Regression Analysis. The Parameters for
'''  Skellam is the winzorized mean over the Poisson-Part.
'''
''' AUTHOR
'''  Fabian Amman, afabian@bioinf.uni-leipzig.de
'''
''' LICENCE
'''  TSSAR itself comes under GNU General Public License v2.0
'''
'''  Please note that TSSAR uses the R libraries Skellam and VGAM. Both
'''  libraries are not our property and might have altering licencing. Please
'''  cite independantly.
''' 
''' </remarks>
''' 
<[Namespace]("TSSAR", Description:="TSSAR Perl invoke module")>
Public Module Perl

    ''' <summary>
    ''' R程序的路径
    ''' </summary>
    ''' <remarks></remarks>
    <DataFrameColumn("R_PATH")> Dim R As String

    ''' <summary>
    ''' *T*ranscription *S*tart *S*ites *A*nnotation *R*egime for dRNA-seq data,
    ''' based on a Skellam distribution with parameter estimation by
    ''' zero-inflated-poisson model regression analysis. The input are two
    ''' mapped sequencing files in SAM file formate (library[+] and library[-]),
    ''' the output is a *.BED file with an entry for each position which is
    ''' annotated as a TSS, writen to STDOUT. Addtionally, a file named Dump.bed
    ''' is created. It specifies regions where the applied regression model does
    ''' not converge. Hence, those regions are omitted from analysis.
    ''' </summary>
    ''' <param name="prorata">
    ''' If set, the information from the SAM file how many times a read was
    ''' mapped to the genome is used, if present. If the read maps *n* times
    ''' to the genome, each position is counted only *1/n* times. Usefull in
    ''' combination with e.g. segemehl mapper, which can report suboptimal
    ''' mapping positions and/or reports all location where a read maps
    ''' optimally. Default is off.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 这个Perl和R脚本的执行效率太低了！
    ''' </remarks>
    ''' <param name="libP">
    ''' Input library (P .. Plus; M .. Minus) in SAM format. The plus library is the one with enriched TSS 
    ''' (for dRNA-seq this means that the plus library is the treated library, while the minus library is
    ''' the untreated library)
    ''' </param>
    ''' <param name="libM">
    ''' Input library (P .. Plus; M .. Minus) in SAM format. The plus library is the one with enriched TSS 
    ''' (for dRNA-seq this means that the plus library is the treated library, while the minus library is
    ''' the untreated library)
    ''' </param>
    ''' <param name="fasta">
    ''' Either the location of reference genome sequence in fasta file
    ''' format OR the genome size in *INT*. The fasta file is only used to
    ''' parse the genome size so just one of the two must be specified.
    ''' </param>
    ''' <param name="g_size">
    ''' Either the location of reference genome sequence in fasta file
    ''' format OR the genome size in *INT*. The fasta file is only used to
    ''' parse the genome size so just one of the two must be specified.
    ''' </param>
    ''' <param name="minPeak"> 
    ''' Minimal Peak size in *INT*. Only positions where read start count in
    ''' the (+)library is greater or equal then *INT* are evaluated to be a
    ''' TSS. Positions with less reads are seen as backgroound noise and not
    ''' considered. Default is *3*.
    ''' </param>
    ''' <param name="pval">Maximal P-value for each position to be annotated as a TSS. Default is *1e-04*.</param>
    ''' <param name="winSize">Size of the window which slides over the genome and defines the statistical properties of the local model. Default is *1,000*.</param>
    ''' <param name="verbose">If set, some progress reports are printed to STDERR during computation.</param>
    ''' <param name="score">
    ''' If score mode is *p* the p-value is used as score in the TSS BED
    ''' file. If score mode is *d* the peak difference is used as score in
    ''' the TSS BED file. Default is *d*. Also used for clustering, which
    ''' advices to use 'd', since the p-value often becomes zero for
    ''' consecutive positions, thus disabling a proper merging of
    ''' consecutive positions to the best one.
    ''' </param>
    ''' <param name="nocluster">
    ''' If --nocluster is set all positions annotated as TSS are reported.
    ''' If --cluster is set consecutive TSS positions are clustered and only
    ''' the 'best' position is reported. 'Best' position depends on the
    ''' setting of --score (see above). Either the position with the lowest
    ''' p-Value or the position with the highest peak difference between
    ''' plus and minus library is reported. Default is --cluster. The option
    ''' --range defines the maximal distance for two significant positions
    ''' to be called 'consecutive'.
    ''' </param>
    ''' <param name="range">
    ''' The maximal distance for two significant positions to be be
    ''' clustered together if option --cluster is set. Default is *3* nt. If
    ''' --cluster is set to --nocluster, --range is ignored.</param>
    ''' <param name="clean">
    ''' If --clean is set, all temporary files which are created during the
    ''' computation are deleted afterwards. With --noclean they are stored.
    ''' Mainly for debugging purpose. Default setting is --clean.</param>
    ''' 
    <ExportAPI("TSSAR", Info:="*T*ranscription *S*tart *S*ites *A*nnotation *R*egime for dRNA-seq data, based on a Skellam distribution with parameter estimation by " &
        "zero-inflated-poisson model regression analysis. The input are two mapped sequencing files in SAM file formate (library[+] and library[-]), " &
        "the output is a *.BED file with an entry for each position which is annotated as a TSS, writen to STDOUT. Addtionally, a file named Dump.bed " &
        "is created. It specifies regions where the applied regression model does not converge. Hence, those regions are omitted from analysis.")>
    Public Function Invoke(<Parameter("libP", "Input library (P .. Plus; M .. Minus) in SAM format. The plus library is the one with enriched TSS " &
                           "(for dRNA-seq this means that the plus library is the treated library, while the minus library is the untreated library)")> libP As String,
                           <Parameter("libM", "Input library (P .. Plus; M .. Minus) in SAM format. The plus library is the one with enriched TSS " &
                           "(for dRNA-seq this means that the plus library is the treated library, while the minus library is the untreated library)")> libM As String,
                           <Parameter("score", "If score mode is *p* the p-value is used as score in the TSS BED file. If score mode is *d* the peak difference is used as score in " &
                               "the TSS BED file. Default is *d*. Also used for clustering, which advices to use 'd', since the p-value often becomes zero for " &
                               "consecutive positions, thus disabling a proper merging of consecutive positions to the best one.")> Optional score As String = "d",
                           <Parameter("fasta", "Either the location of reference genome sequence in fasta file format OR the genome size in *INT*. The fasta file is only used to " &
                               "parse the genome size so just one of the two must be specified.")> Optional fasta As String = "",
                           <Parameter("g_size", "Either the location of reference genome sequence in fasta file format OR the genome size in *INT*. The fasta file is only used to " &
                               "parse the genome size so just one of the two must be specified.")> Optional g_size As Integer = 100,
                           <Parameter("minPeak", "Minimal Peak size in *INT*. Only positions where read start count in the (+)library is greater or equal then *INT* are evaluated to be a " &
                               "TSS. Positions with less reads are seen as backgroound noise and not considered. Default is *3*.")> Optional minPeak As Integer = 3,
                           <Parameter("pval", "Maximal P-value for each position to be annotated as a TSS. Default is *1e-04*.")> Optional pval As String = "1e-04",
                           <Parameter("winSize", "Size of the window which slides over the genome and defines the statistical properties of the local model. Default is *1,000*.")> Optional winSize As Integer = 100,
                           <Parameter("verbose", "If set, some progress reports are printed to STDERR during computation.")> Optional verbose As Boolean = True,
                           <Parameter("clean", "If --clean is set, all temporary files which are created during the computation are deleted afterwards. " &
                               "With --noclean they are stored. Mainly for debugging purpose. Default setting is --clean.")> Optional clean As Boolean = False,
                           <Parameter("nocluster", "If --nocluster is set all positions annotated as TSS are reported. If --cluster is set consecutive TSS positions are clustered and only " &
                               "the 'best' position is reported. 'Best' position depends on the setting of --score (see above). Either the position with the lowest " &
                               "p-Value or the position with the highest peak difference between plus and minus library is reported. Default is --cluster. The option " &
                               "--range defines the maximal distance for two significant positions to be called 'consecutive'.")> Optional nocluster As Boolean = True,
                           <Parameter("range", "The maximal distance for two significant positions to be be clustered together if option --cluster is set. Default is *3* nt. " &
                               "If --cluster is set to --nocluster, --range is ignored.")> Optional range As Integer = 3,
                           <Parameter("prorata", "If set, the information from the SAM file how many times a read was mapped to the genome is used, if present. If the read maps *n* times " &
                               "to the genome, each position is counted only *1/n* times. Usefull in combination with e.g. segemehl mapper, which can report suboptimal " &
                               "mapping positions and/or reports all location where a read maps optimally. Default is off.")> Optional prorata As Boolean = False) _
 _
          As <FunctionReturns("")> Integer

        If String.IsNullOrEmpty(R) OrElse Not R.FileExists Then
            Call Console.WriteLine("TSSAR Perl script could not found the R system, threading exit!")
            Return -1
        End If

        ' --tmpdir *DIR*
        ' Specifies where the temporary files should be stored. Default is
        ' */tmp*.

        'SYNOPSIS
        ' ./TSSAR --libP *libraryP.sam* --libM *libraryM.sam* [--score *p|d*]
        '  [--fasta *genome.fa* --g_size *INT*] [--minPeak *INT*] [--pval *FLOAT*]
        '  [--winSize *INT*] [--verbose] [--noclean] [--nocluster] [-range *INT*]]
        '  [<--tmpdir> *DIR*] [--help|?] [--man]

        Dim argBuilder As New Dictionary(Of String, String) From {
            {"--libP", libP.Replace("\", "/")},
            {"--libM", libM.Replace("\", "/")},
            {"--tmpdir", Settings.DataCache.Replace("\", "/") & "/" & Rnd()}
        }

        Call FileIO.FileSystem.CreateDirectory(argBuilder("--tmpdir"))

        If Not String.IsNullOrEmpty(fasta) AndAlso fasta.FileExists Then
            Call argBuilder.Add("--fasta", fasta.Replace("\", "/"))
        Else
            Call argBuilder.Add("--g_size", g_size)
        End If
        If nocluster Then
            Call argBuilder.Add("--nocluster", "")
        Else
            Call argBuilder.Add("--cluster", "")
        End If
        If clean Then
            Call argBuilder.Add("--clean", "")
        Else
            Call argBuilder.Add("--noclean", "")
        End If
        If verbose Then
            Call argBuilder.Add("--verbose", "")
        End If

        Call argBuilder.Add("--minPeak", minPeak)
        Call argBuilder.Add("--pval", pval)
        Call argBuilder.Add("--winSize", winSize)
        Call argBuilder.Add("--score", score)
        Call argBuilder.Add("--range", range)

        Dim sBuilder As StringBuilder = New StringBuilder(1024)
        Dim TSSAR As String = Settings.DataCache & "/TSSAR.pl"

        Call My.Resources.dskellam.SaveTo(Settings.DataCache & "/dskellam.R")
        Call My.Resources.dskellam_sp.SaveTo(Settings.DataCache & "/dskellam_sp.R")
        Call My.Resources.pskellam.SaveTo(Settings.DataCache & "/pskellam.R")
        Call My.Resources.pskellam_sp.SaveTo(Settings.DataCache & "/pskellam_sp.R")
        Call My.Resources.qskellam.SaveTo(Settings.DataCache & "/qskellam.R")
        Call My.Resources.rskellam.SaveTo(Settings.DataCache & "/rskellam.R")

        Dim TSSARPerl As StringBuilder = New StringBuilder(My.Resources.TSSAR)

        Call TSSARPerl.Replace("{skellam-0}", (Settings.DataCache & "/dskellam.R").Replace("\", "/"))
        Call TSSARPerl.Replace("{skellam-1}", (Settings.DataCache & "/dskellam_sp.R").Replace("\", "/"))
        Call TSSARPerl.Replace("{skellam-2}", (Settings.DataCache & "/pskellam.R").Replace("\", "/"))
        Call TSSARPerl.Replace("{skellam-3}", (Settings.DataCache & "/pskellam_sp.R").Replace("\", "/"))
        Call TSSARPerl.Replace("{skellam-4}", (Settings.DataCache & "/qskellam.R").Replace("\", "/"))
        Call TSSARPerl.Replace("{skellam-5}", (Settings.DataCache & "/rskellam.R").Replace("\", "/"))

        Dim R_PATH As String = Perl.R
        'If R_PATH.Contains(" "c) Then
        '    R_PATH = """" & R_PATH & """"
        'End If
        R_PATH = R_PATH.Replace("\", "/")

        Call TSSARPerl.Replace("{R_PATH}", R_PATH)
        Call TSSARPerl.ToString.SaveTo(TSSAR)

        If TSSAR.Contains(" "c) Then
            TSSAR = """" & TSSAR & """"
        End If

        Call sBuilder.Append(TSSAR)

        For Each para In argBuilder
            Call sBuilder.Append(" ")

            If String.IsNullOrEmpty(para.Value) Then 'boolean
                Call sBuilder.Append(para.Key)
                Continue For
            End If

            Dim pValue As String = para.Value
            If pValue.Contains(" "c) Then
                pValue = """" & pValue & """"
            End If
            Call sBuilder.Append(para.Key & " " & pValue)
        Next

        Dim argvs As String = sBuilder.ToString
        Dim InvokeCli As New IORedirect("perl", argvs)
        Call ("Perl " & argvs).SaveTo(Settings.DataCache & "/Perl_Invoke.bat")
        Return InvokeCli.Start(waitForExit:=True, displaDebug:=True)
    End Function

    <ExportAPI("Read.Fastaq")>
    Public Function LoadFastaq(Path As String) As FastQFile
        Return FastQFile.Load(Path)
    End Function

    <ExportAPI("Read.SAM")>
    Public Function LoadSAM(Path As String) As SAM
        Return SAM.Load(Path)
    End Function

    '''' <summary>
    '''' 使用一个SAM库进行TSS的注释
    '''' </summary>
    '''' <param name="SAM"></param>
    '''' <returns></returns>
    '<Command("SAM.TSSs", Info:="Invoke the TSS annotation using the Sam mapping result file.")>
    'Public Function TSSAnno(SAM As SAM) As AlignmentReads()
    '    Dim Forwards As AlignmentReads() = Nothing,
    '        Reversed As AlignmentReads() = Nothing

    '    Call SAM.Assembling(Forwards, Reversed)        '进行装配

    '    Dim PredictedTss = InternalTSSAnno(Forwards, Reversed)
    '    Return PredictedTss
    'End Function

    'Private Function InternalTSSAnno(Forwards As Generic.IEnumerable(Of AlignmentReads), Reversed As Generic.IEnumerable(Of AlignmentReads)) As AlignmentReads()
    '    Dim FwAyHandle = Function() As AlignmentReads()
    '                         Dim ForwardsAssembleContigs = (From Contig In Forwards.AsParallel Select Contig Group By Contig.POS Into Group).ToArray '按照左端位置进行分组
    '                         Return (From Contig In ForwardsAssembleContigs.AsParallel
    '                                 Where Contig.Group.Count >= 30
    '                                 Select ReadMapping = (From Segment In Contig.Group Select Segment Order By Segment.Length Descending).First
    '                                 Order By ReadMapping.POS Ascending).ToArray '超过30条共享同一个左端的Reads被作为一个候选的TSSs
    '                     End Function
    '    Dim FwStart = FwAyHandle.BeginInvoke(Nothing, Nothing)

    '    Dim ReversedAssembleContigs = (From Contig In Reversed.AsParallel Select Contig Group By Contig.POS Into Group).ToArray
    '    Dim ReversedPossibleTSSs = (From Contig In ReversedAssembleContigs.AsParallel
    '                                Where Contig.Group.Count >= 30
    '                                Select ReadMapping = (From Segment In Contig.Group Select Segment Order By Segment.Length Descending).First
    '                                Order By ReadMapping.POS Ascending).ToArray '超过30条共享同一个左端的Reads被作为一个候选的TSSs

    '    Dim ChunkBuffer = FwAyHandle.EndInvoke(FwStart).AsList
    '    ChunkBuffer.AddRange(ReversedPossibleTSSs)
    '    Return ChunkBuffer.ToArray
    'End Function

    '''' <summary>
    '''' 将多个SAM库进行合并来进行TSS的预测分析，但是我在这里有一个疑问：不同条件之下的SAM库能否应用于同一个富集操作？？？
    '''' 先进行合并，再进行预测
    '''' </summary>
    '''' <returns></returns>
    '''' 
    '<Command("SAM.TSSs")>
    'Public Function TSSAnno(<Parameter("Dir.SAM.Source", "The directory which contains the *.sam library file for the TSS annotation.")> SAM As String) As AlignmentReads()

    '    '做序列装配
    '    Dim Forwards As New LinkedList(Of DocumentFormat.SAM.DocumentElements.AlignmentReads)
    '    Dim Reversed As New LinkedList(Of DocumentFormat.SAM.DocumentElements.AlignmentReads)

    '    For Each SamFilePath In SAM.LoadSourceEntryList("*.sam")
    '        Dim ForwardsArr As AlignmentReads() = Nothing,
    '            ReversedArr As AlignmentReads() = Nothing
    '        Dim SAMFile = DocumentFormat.SAM.SAM.Load(SamFilePath.Value)

    '        Call Console.WriteLine($"[{Now.ToString}] {SamFilePath.Value.ToFileURL} data load done!")
    '        Call SAMFile.Assembling(ForwardsArr, ReversedArr)        '进行装配
    '        Call Console.WriteLine("Data assembling job done!")

    '        Call Forwards.AddRange(ForwardsArr)
    '        Call Reversed.AddRange(ReversedArr)
    '        Call Console.WriteLine("Start to load data next....")
    '    Next

    '    Call Console.WriteLine("Start to annotation of TSSs")

    '    Dim PredictedTss = InternalTSSAnno(Forwards, Reversed)
    '    Return PredictedTss
    'End Function

    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="pTss"></param>
    '''' <param name="Contigs">经过左端从小到大排序的</param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Private Function Assembly(pTss As AlignmentReads, Contigs As DocumentFormat.SAM.DocumentElements.AlignmentReads()) As AlignmentReads
    '    Dim Right As Integer = pTss.POS + pTss.Length
    '    Dim i As Integer = 0

    '    For Each cc In Contigs
    '        If cc.POS > pTss.POS AndAlso cc.POS < Right Then
    '            Exit For '先找打最左端的下一个Reads
    '        Else
    '            i += 1
    '        End If
    '    Next

    '    '开始进行装配
    '    Do While True

    '        Dim cc = Contigs(i)

    '        If Not (cc.POS > pTss.POS AndAlso cc.POS < Right) Then  '当前的这个片段已经和前一个片段断裂了；额
    '            Exit Do
    '        End If

    '        Dim cc_left As String = cc.POS
    '        Dim l As Integer = cc_left - pTss.POS
    '        pTss.SequenceData = Mid(pTss.SequenceData, 1, l) & cc.SequenceData

    '        Right = pTss.POS + pTss.Length

    '        i += 1

    '    Loop

    '    Return pTss
    'End Function

    <ExportAPI("Write.Csv.MappingReads")>
    Public Function SaveAlignmentReadsMapping(data As IEnumerable(Of AlignmentReads), SaveTo As String) As Boolean
        Return data.SaveTo(SaveTo, False)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="PTT">参考基因组之中的基因的摘要信息</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Assembly.Located")>
    Public Function Located(data As IEnumerable(Of AlignmentReads), PTT As PTT) As LocatedAlignment()

        Call Settings.Initialize()

        ' 通过blastn方法进行搜索定位的旧方法

        'Dim Fasta = (From Tss In data Let ID = Tss.Length & Tss.MappingPosition Select fsa = New SMRUCC.genomics.SequenceModel.FASTA.FastaObject With {.Attributes = {ID}, .SequenceData = Tss.SequenceData}, ID, Tss).ToArray
        'Dim Temp As String = Program.Settings.DataCache & "/" & basename(FileIO.FileSystem.GetTempFileName) & ".fasta"
        'Call CType((From obj In Fasta Select obj.fsa).ToArray, SMRUCC.genomics.SequenceModel.FASTA.FastaFile).Save(Temp)
        'Dim Log = Program.Settings.DataCache & "/" & basename(FileIO.FileSystem.GetTempFileName) & ".txt"
        'Call SMRUCC.genomics.NCBI.Extensions.Blastn(SMRUCC.genomics.NCBI.Extensions.CreateSession, Temp, refGenes, Log, "1000")

        '   Dim LQWuery = (From site In data Select site, genes = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GetRelatedGenes(Ptt, site.MappingPosition, site.MappingPosition + 50)).ToArray
        Throw New NotImplementedException
    End Function

    Public Class LocatedAlignment : Inherits ISequenceModel
        Public Property QueryTemplateName As String
        Public Property BitwiseFLAG As String
        Public Property RefName As String
        Public Property MappingPosition As Integer
        Public Property MapQuality As Integer
        Public Property CIGAR As String
        Public Property RefNext As String
        Public Property PosNext As Integer
        Public Property SegmentDirection As Strands
    End Class

End Module
