#Region "Microsoft.VisualBasic::619129d54776b6befa7c41e05e2ed0c4, analysis\Motifs\CRISPR\sgRNAcas\API.vb"

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

    ' Module API
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: check_sgRNA_seq, extract_targetSeq, Initialize, LoadSingleStrandResult, sgRNAcas
    '               sgRPrimer
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' sgRNAcas9 --- a tool for fast designing CRISPR sgRNA with high specificity.  
''' 
''' If you use this program in your research, please cite:
''' Xie S, Shen B, Zhang C, Huang X * and Zhang Y *. sgRNAcas9: a software package for designing CRISPR sgRNA and evaluating potential off-target cleavage sites. PloS one, 2014
''' Please send bug reports to: ssxieinfo@gmail.com
''' </summary>
''' <remarks></remarks>
<[Namespace]("sgRNAcas", Description:="sgRNAcas9 --- a tool for fast designing CRISPR sgRNA with high specificity")>
Public Module API

    <ExportAPI("Read.Csv.sgRNA.Single_Strand")>
    Public Function LoadSingleStrandResult(Path As String) As SingleStrandResult()
        Return SingleStrandResult.LoadDocument(Path)
    End Function

    ''' <summary>
    ''' Perl脚本程序的存放的文件夹
    ''' </summary>
    ''' <remarks></remarks>
    Dim PerlScriptBin As String

    Sub New()
        Call Settings.Initialize()
    End Sub

    <ExportAPI("_Init()")>
    Public Function Initialize() As Boolean
        API.PerlScriptBin = Settings.DataCache & "/sgRNAcas/"

        Call My.Resources.check_sgRNA_seq.SaveTo(API.PerlScriptBin & "check_sgRNA_seq.pl")
        Call My.Resources.combine_genome.SaveTo(API.PerlScriptBin & "combine_genome.pl")
        Call My.Resources.combine_result.SaveTo(API.PerlScriptBin & "combine_result.pl")
        Call My.Resources.extract_targetSeq.SaveTo(API.PerlScriptBin & "extract_targetSeq.pl")
        Call My.Resources.format_genome.SaveTo(API.PerlScriptBin & "format_genome.pl")
        Call My.Resources.ot2gtf_v2.SaveTo(API.PerlScriptBin & "ot2gtf_v2.pl")
        Call My.Resources.pot2gtf_v2.SaveTo(API.PerlScriptBin & "pot2gtf_v2.pl")
        Call My.Resources.sgRNAcas9_2_0_10.SaveTo(API.PerlScriptBin & "sgRNAcas.pl")
        Call My.Resources.sgRPrimer.SaveTo(API.PerlScriptBin & "sgRPrimer.pl")

        Call My.Resources.seqmap_1_04.FlushStream(API.PerlScriptBin & "seqmap-1.0.12-windows.exe")

        Return True
    End Function

    ''' <summary>
    ''' sgRNAcas9 --- a tool for fast designing CRISPR sgRNA with high specificity
    ''' </summary>
    ''' <param name="InputFile">Input file</param>
    ''' <param name="Length">Lenght of sgRNA</param>
    ''' <param name="MinGC"></param>
    ''' <param name="MaxGC"></param>
    ''' <param name="RefGenome"></param>
    ''' <param name="Option"></param>
    ''' <param name="SearchModel"></param>
    ''' <param name="OSVersion"></param>
    ''' <param name="Mismatches"></param>
    ''' <param name="MinOffSet"></param>
    ''' <param name="MaxOffSet"></param>
    ''' <param name="Output"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("sgRNAcas", Info:="sgRNAcas9 --- a tool for fast designing CRISPR sgRNA with high specificity")>
    Public Function sgRNAcas(<Parameter("-i", "Input file")> InputFile As String,
                             <Parameter("-g", "The reference genome sequence.")> RefGenome As String,
                             <Parameter("-p", "Output path")> Output As String,
                             <Parameter("-x", "Length of sgRNA, default value is 20.")> Optional Length As Integer = 20,
                             <Parameter("-l", "The minimum value of GC content.")> Optional MinGC As Integer = 20,
                             <Parameter("-m", "The maximum value of GC content.")> Optional MaxGC As Integer = 80,
                             <Parameter("-o", "Searching CRISPR target sites using DNA strands based option (s/a/b); " &
                                 "s, sense strand searching mode; " &
                                 "a, anti-sense strand searching mode; " &
                                 "b, both strand searching mode.")> Optional [Option] As String = "b",
                             <Parameter("-t", "Type of gRNA searching mode(s/p); s, single-gRNA searching mode; p, paired-gRNA searching mode.")> Optional SearchModel As String = "s",
                             <Parameter("-v", "Operation system(w/l/u/m/a); w, for windows-32, 64; l, for linux-64; u, for linux-32; m, for MacOSX-64; a, for MacOSX-32")> Optional OSVersion As String = "w",
                             <Parameter("-n", "Maximum number of mismatches")> Optional Mismatches As Integer = 5,
                             <Parameter("-s", "The minimum value of sgRNA offset")> Optional MinOffSet As Integer = -2,
                             <Parameter("-e", "The maximum value of the sgRNA offset")> Optional MaxOffSet As Integer = 32) As Boolean
        Dim argvs As String = String.Format("{0}/sgRNAcas.pl -i ""{1}"" -g ""{2}"" -x {4} -l {5} -m {6} -o {7} -t {8} -v {9} -n {10} -s {11} -e {12} -p ""{3}""",
                                            API.PerlScriptBin,
                                            FileIO.FileSystem.GetFileInfo(InputFile).FullName,
                                            FileIO.FileSystem.GetFileInfo(RefGenome).FullName,
                                            FileIO.FileSystem.GetDirectoryInfo(Output).FullName, Length, MinGC, MaxGC, [Option], SearchModel, OSVersion, Mismatches, MinOffSet, MaxOffSet)
        Using process As New IORedirect("perl", argvs)
            Using temp As New FileIO.TemporaryEnvironment(Output)
                Call process.Start(waitForExit:=True, displaDebug:=True)
                Call Thread.Sleep(100)
            End Using
        End Using

        Return True
    End Function

    <ExportAPI("check_sgRNA_seq")>
    Public Function check_sgRNA_seq(<Parameter("-i", "CRISPR target sequences")> Input As String,
                                    <Parameter("-r", "restriction enzyme cutting sites")> RestrictedSites As String) As Boolean
        Dim argvs As String = String.Format("{0}/check_sgRNA_seq.pl -i ""{1}"" -r ""{2}""", API.PerlScriptBin, FileIO.FileSystem.GetFileInfo(Input).FullName, RestrictedSites)
        Dim Process As New IORedirect("perl", argvs)
        Call Process.Start(waitForExit:=True, displaDebug:=True)

        Return True
    End Function

    <ExportAPI("extract_targetSeq")>
    Public Function extract_targetSeq(<Parameter("-i", "Input file")> Input As String,
                                      <Parameter("-g", "The reference genome sequence")> RefGenome As String,
                                      <Parameter("-l", "Lenght of flank sequences")> Optional Length As Integer = 1000) As Boolean
        Dim argvs As String = String.Format("{0}/extract_targetSeq.pl -i ""{1}"" -g ""{2}"" -l {3}", API.PerlScriptBin, FileIO.FileSystem.GetFileInfo(Input).FullName, RefGenome, Length)
        Dim Process As New IORedirect("perl", argvs)
        Call Process.Start(waitForExit:=True, displaDebug:=True)

        Return True
    End Function

    <ExportAPI("sgRPrimer")>
    Public Function sgRPrimer(<Parameter("-i", "Input file")> Input As String,
                              <Parameter("-s", "A file of IDs")> IDListFile As String,
                              <Parameter("-l", "Length of sgRNA")> Optional Length As Integer = 20,
                              <Parameter("-f", "Restriction enzyme cutting site for forward primer")> Optional ForwardRs As String = "acgg",
                              <Parameter("-r", "Restriction enzyme cutting site for reverse primer")> Optional ReversedRs As String = "aaac") As Boolean
        Dim argvs As String = String.Format("{0}/sgRPrimer.pl -i ""{1}"" -s ""{2}"" -l {3} -f ""{4}"" -r ""{5}""", API.PerlScriptBin, FileIO.FileSystem.GetFileInfo(Input).FullName, IDListFile, Length, ForwardRs, ReversedRs)
        Dim Process As New IORedirect("perl", argvs)
        Call Process.Start(waitForExit:=True, displaDebug:=True)

        Return True
    End Function
End Module
