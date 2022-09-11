#Region "Microsoft.VisualBasic::35d55cb8c03f6262b9a43b94aa1110d7, GCModeller\analysis\SequenceToolkit\ClustalOmega\Clustal.vb"

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


    ' Code Statistics:

    '   Total Lines: 159
    '    Code Lines: 103
    ' Comment Lines: 35
    '   Blank Lines: 21
    '     File Size: 7.62 KB


    ' Class Clustal
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+2 Overloads) Align, AlignmentTask, CreateSession, Help, MultipleAlignment
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.FileIO.Path
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports CLI = Microsoft.VisualBasic.CommandLine.InteropService.InteropService

''' <summary>
''' Clustal Omega(多序列比对工具)
''' Clustal Omega - 1.2.0 (AndreaGiacomo)
'''
''' If you Like Clustal - Omega please cite:
''' 
''' > Sievers F, Wilm A, Dineen D, Gibson TJ, Karplus K, Li W, Lopez R, McWilliam H, Remmert M, Sding J, Thompson JD, Higgins DG.
''' > Fast, scalable generation Of high-quality protein multiple sequence alignments Using Clustal Omega.
''' > Mol Syst Biol. 2011 Oct 11;7:539. doi: 10.1038/msb.2011.75. PMID: 21988835.
''' 
''' If you don't like Clustal-Omega, please let us know why (and cite us anyway).
'''
''' Check http://www.clustal.org for more information And updates.
''' </summary>
''' <remarks></remarks>
''' 
<Package("Clustal",
                    Cites:="Sievers, F., et al. (2011). ""Fast, scalable generation Of high-quality protein multiple sequence alignments Using Clustal Omega."" Mol Syst Biol 7: 539.
<p>Multiple sequence alignments are fundamental to many sequence analysis methods. Most alignments are computed using the progressive alignment heuristic. These methods are starting to become a bottleneck in some analysis pipelines when faced with data sets of the size of many thousands of sequences. Some methods allow computation of larger data sets while sacrificing quality, and others produce high-quality alignments, but scale badly with the number of sequences. In this paper, we describe a new program called Clustal Omega, which can align virtually any number of protein sequences quickly and that delivers accurate alignments. The accuracy of the package on smaller test cases is similar to that of the high-quality aligners. On larger data sets, Clustal Omega outperforms other packages in terms of execution time and quality. Clustal Omega also has powerful features for adding sequences to and exploiting information in existing alignments, making use of the vast amount of precomputed information in public databases like Pfam.

", Publisher:="Sievers, F., et al.")>
<Cite(Title:="Fast, scalable generation of high-quality protein multiple sequence alignments using Clustal Omega",
      Volume:=7, Pages:="539",
      Authors:="Sievers, F.
Wilm, A.
Dineen, D.
Gibson, T. J.
Karplus, K.
Li, W.
Lopez, R.
McWilliam, H.
Remmert, M.
Soding, J.
Thompson, J. D.
Higgins, D. G.", Journal:="Molecular systems biology", ISSN:="1744-4292 (Electronic);
1744-4292 (Linking)", DOI:="10.1038/msb.2011.75", Year:=2011, Keywords:="Algorithms
Amino Acid Sequence
Base Sequence
Data Mining/*methods
Databases, Factual
Molecular Sequence Data
Proteins/*analysis/chemistry
Sequence Alignment/*methods
Sequence Analysis, Protein/*methods
Software
*Systems Biology/instrumentation/methods",
      Abstract:="Multiple sequence alignments are fundamental to many sequence analysis methods. Most alignments are computed using the progressive alignment heuristic. 
These methods are starting to become a bottleneck in some analysis pipelines when faced with data sets of the size of many thousands of sequences. 
Some methods allow computation of larger data sets while sacrificing quality, and others produce high-quality alignments, but scale badly with the number of sequences. 
In this paper, we describe a new program called Clustal Omega, which can align virtually any number of protein sequences quickly and that delivers accurate alignments. 
The accuracy of the package on smaller test cases is similar to that of the high-quality aligners. On larger data sets, Clustal Omega outperforms other packages in terms of execution time and quality. 
Clustal Omega also has powerful features for adding sequences to and exploiting information in existing alignments, making use of the vast amount of precomputed information in public databases like Pfam.",
      AuthorAddress:="School of Medicine and Medical Science, UCD Conway Institute of Biomolecular and Biomedical Research, University College Dublin, Dublin, Ireland.",
      PubMed:=21988835)>
Public Class Clustal : Inherits CLI

    ''' <summary>
    ''' 
    ''' </summary>
    Public Const CLUSTAL_ARGUMENTS As String = "--in ""{0}"" --out ""{1}"""

#Region "CLI"

#End Region

    Const SourceNotExists As String = "Source data file ""{0}"" is not exists on your file system!"

    ''' <summary>
    ''' 目标多序列比对文件的文件路径，出错会返回空值
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MultipleAlignment(source As String) As FASTA.FastaFile
        If Not source.FileExists Then
            Dim msg$ = String.Format(SourceNotExists, source.ToFileURL)
            Dim ex As New DataException(msg)

            Throw New Exception(source.ToFileURL, ex)
        End If

        Dim out As String = TempFileSystem.GetAppSysTempFile(".fasta", sessionID:=App.PID)
        Dim args As String = String.Format(CLUSTAL_ARGUMENTS, source, out)

        Call MyBase.RunProgram(args).Run()

        Dim result As FASTA.FastaFile = FASTA.FastaFile.Read(out, False)
        Return result
    End Function

    ''' <summary>
    ''' 这个是通过标准输入来传递序列数据的
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    Public Function Align(source As IEnumerable(Of FASTA.FastaSeq)) As FASTA.FastaFile
        Dim fa As New FASTA.FastaFile(source)
        Dim input As String = fa.Generate
        Dim out As String = MyBase._executableAssembly.Call("", input)
        Return FASTA.FastaFile.DocParser(out.LineTokens)
    End Function

    Public Function AlignmentTask(source As String) As AsyncHandle(Of FASTA.FastaFile)
        Dim sourceInvoke = Function() MultipleAlignment(source)
        Dim hwnd As New AsyncHandle(Of FASTA.FastaFile)(sourceInvoke)
        Return hwnd.Run
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Exe">Clustal可执行文件的文件路径</param>
    ''' <remarks></remarks>
    Sub New(Exe As String)
        MyBase._executableAssembly = Exe
    End Sub

    <ExportAPI("Session.New")>
    Public Shared Function CreateSession() As Clustal
        Dim directories$() = ProgramPathSearchTool.SearchDirectory("clustal-omega")

        If directories.IsNullOrEmpty Then
            GoTo RELEASE_PACKAGE
        End If

        For Each dir As String In directories
            Dim program = ProgramPathSearchTool _
                .SearchProgram(dir, "clustalo") _
                .ToArray

            If Not program.IsNullOrEmpty Then
                Return New Clustal(program.First)
            End If
        Next

RELEASE_PACKAGE:
        Return New Clustal(ReleasePackage(App.AppSystemTemp))
    End Function

    <ExportAPI("clustal.align")>
    Public Shared Function Align(session As Clustal, source As String, argvs As String) As FASTA.FastaFile
        Return session.MultipleAlignment(source)
    End Function

    <ExportAPI("clustal.help")>
    Public Shared Function Help() As String
        Call Console.WriteLine(My.Resources.AUTHORS)
        Return My.Resources.AUTHORS
    End Function
End Class
