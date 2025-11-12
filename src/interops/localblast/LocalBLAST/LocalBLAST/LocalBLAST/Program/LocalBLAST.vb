#Region "Microsoft.VisualBasic::9834f95683e3e3e191a6cc413cfe2c99, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Program\LocalBLAST.vb"

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

    '   Total Lines: 505
    '    Code Lines: 98 (19.41%)
    ' Comment Lines: 353 (69.90%)
    '    - Xml Docs: 98.58%
    ' 
    '   Blank Lines: 54 (10.69%)
    '     File Size: 19.87 KB


    '     Class LocalBLAST
    ' 
    '         Properties: AlignmentView, BestHitsKeepsNumber, CheckpointFile, CompositionBasedScoreAdjustments, ConcatenatedQueries
    '                     ConcatenatedQueriesNumber, DBEffectiveLength, DBGeneticCode, DropoffValue, EValue
    '                     Filter, ForceLegacy, ForceLegacyNumber, FrameShiftPenalty, GapExtend
    '                     GapOpen, GappedAlignment, GI, Hits, HitsWindowSize
    '                     HTML, IntronLength, LowerCaseFiltering, MatchReward, Matrix
    '                     MegaBlast, MismatchPenalty, MolTypeNucleotide, MolTypeProtein, Processors
    '                     QueryDeflineBelieve, QueryGeneticCode, QueryLocation, RestrictList, SearchSpace
    '                     SeqAlign, SmithWatermanAlignments, Strand, Threshold, UngappedExtensionsXDropOff
    '                     WordSize, XDropoffValue
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Blastn, Blastp, FormatDb, GetLastLogFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports File = System.String

Namespace LocalBLAST.Programs

    Public Class LocalBLAST : Inherits InteropService.InteropService

        ''' <summary>
        ''' The file path of the blastall program the in the BLAST+ program groups.
        ''' (在BLAST+程序组之中的BLASTALL程序的文件路径)
        ''' </summary>
        ''' <remarks></remarks>
        Dim BLASTALLAssembly As String
        ''' <summary>
        ''' formatdb程序的文件名
        ''' </summary>
        ''' <remarks></remarks>
        Dim FormatDbAssembly As String

        ''' <summary>
        ''' Expectation value (E) [Real]  
        ''' Default = 10.0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-e", CLITypes.Double)> Public Property EValue As String

        ''' <summary>
        ''' Alignment view options, Range from 0 to 11:
        ''' 0 = pairwise,
        ''' 1 = query-anchored showing identities,
        ''' 2 = query-anchored no identities,
        ''' 3 = flat query-anchored, show identities,
        ''' 4 = flat query-anchored, no identities,
        ''' 5 = query-anchored no identities and blunt ends,
        ''' 6 = flat query-anchored, no identities and blunt ends,
        ''' 7 = XML Blast output,
        ''' 8 = tabular, 
        ''' 9 tabular with comment lines
        ''' 10 ASN, text
        ''' 11 ASN, binary [Integer]
        ''' Default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-m", CLITypes.Integer)> Public Property AlignmentView As String

        ''' <summary>
        ''' Filter query sequence (DUST with blastn, SEG with others) [String]
        ''' Default = T
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-F", CLITypes.String)> Public Property Filter As String

        ''' <summary>
        ''' Cost to open a gap (-1 invokes default behavior) [Integer]
        ''' Default = -1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-G", CLITypes.Integer)> Public Property GapOpen As String

        ''' <summary>
        ''' Cost to extend a gap (-1 invokes default behavior) [Integer]
        ''' default = -1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-E", CLITypes.Integer)> Public Property GapExtend As String

        ''' <summary>
        ''' X dropoff value for gapped alignment (in bits) (zero invokes default behavior)
        ''' blastn 30, megablast 20, tblastx 0, all others 15 [Integer]
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-X", CLITypes.Integer)> Public Property DropoffValue As String

        ''' <summary>
        ''' Show GI's in deflines [T/F]
        ''' default = F
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-I", CLITypes.String)> Public Property GI As String

        ''' <summary>
        ''' Penalty for a nucleotide mismatch (blastn only) [Integer]
        ''' default = -3
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-q", CLITypes.Integer)> Public Property MismatchPenalty As String

        ''' <summary>
        ''' Reward for a nucleotide match (blastn only) [Integer]
        ''' default = 1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-r", CLITypes.Integer)> Public Property MatchReward As String

        ''' <summary>
        ''' Number of database sequences to show one-line descriptions for (V) [Integer]
        ''' default = 500
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-v", CLITypes.Integer)> Public Property ForceLegacyNumber As String

        ''' <summary>
        ''' Number of database sequence to show alignments for (B) [Integer]
        ''' default = 250
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-b", CLITypes.Integer)> Public Property ConcatenatedQueriesNumber As String

        ''' <summary>
        ''' Threshold for extending hits, default if zero
        ''' blastp 11, blastn 0, blastx 12, tblastn 13
        ''' tblastx 13, megablast 0 [Real]
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-f", CLITypes.Double)> Public Property Threshold As String

        ''' <summary>
        ''' Perform gapped alignment (not available with tblastx) [T/F]
        ''' default = T
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-g", CLITypes.String)> Public Property GappedAlignment As String

        ''' <summary>
        ''' Query Genetic code to use [Integer]
        ''' default = 1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-Q", CLITypes.Integer)> Public Property QueryGeneticCode As String

        ''' <summary>
        ''' DB Genetic code (for tblast[nx] only) [Integer]
        ''' default = 1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-D", CLITypes.Integer)> Public Property DBGeneticCode As String

        ''' <summary>
        ''' Number of processors to use [Integer]
        ''' default = 1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-a", CLITypes.Integer)> Public Property Processors As String

        ''' <summary>
        ''' SeqAlign file [File Out]  Optional
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-O", CLITypes.String)> Public Property SeqAlign As String

        ''' <summary>
        ''' Believe the query defline [T/F]
        ''' default = F
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-J", CLITypes.String)> Public Property QueryDeflineBelieve As String

        ''' <summary>
        ''' Matrix [String]
        ''' default = BLOSUM62
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-M", CLITypes.String)> Public Property Matrix As String

        ''' <summary>
        ''' Word size, default if zero (blastn 11, megablast 28, all others 3) [Integer]
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-W", CLITypes.Integer)> Public Property WordSize As String

        ''' <summary>
        ''' Effective length of the database (use zero for the real size) [Real]
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-z", CLITypes.Double)> Public Property DBEffectiveLength As String

        ''' <summary>
        ''' Number of best hits from a region to keep. Off by default.
        ''' If used a value of 100 is recommended.  Very high values of -v or -b is also suggested [Integer]
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-K", CLITypes.Integer)> Public Property BestHitsKeepsNumber As String

        ''' <summary>
        ''' 0 for multiple hit, 1 for single hit (does not apply to blastn) [Integer]
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-P", CLITypes.Integer)> Public Property Hits As String

        ''' <summary>
        ''' Effective length of the search space (use zero for the real size) [Real]
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-Y", CLITypes.Double)> Public Property SearchSpace As String

        ''' <summary>
        ''' Query strands to search against database (for blast[nx], and tblastx)
        ''' 3 is both, 1 is top, 2 is bottom [Integer]
        ''' default = 3
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-S", CLITypes.Integer)> Public Property Strand As String

        ''' <summary>
        ''' Produce HTML output [T/F]
        ''' default = F
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-T", CLITypes.String)> Public Property HTML As String

        ''' <summary>
        ''' Restrict search of database to list of GI's [String]  Optional
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-l", CLITypes.String)> Public Property RestrictList As String

        ''' <summary>
        ''' Use lower case filtering of FASTA sequence [T/F]  Optional
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-U", CLITypes.String)> Public Property LowerCaseFiltering As String

        ''' <summary>
        ''' X dropoff value for ungapped extensions in bits (0.0 invokes default behavior)
        ''' blastn 20, megablast 10, all others 7 [Real]
        ''' default = 0.0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-y", CLITypes.Double)> Public Property UngappedExtensionsXDropOff As String

        ''' <summary>
        ''' X dropoff value for final gapped alignment in bits (0.0 invokes default behavior)
        ''' blastn/megablast 100, tblastx 0, all others 25 [Integer]
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-Z", CLITypes.Integer)> Public Property XDropoffValue As String

        ''' <summary>
        ''' PSI-TBLASTN checkpoint file [File In]  Optional
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-R", CLITypes.String)> Public Property CheckpointFile As String

        ''' <summary>
        ''' MegaBlast search [T/F]
        ''' default = F
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-n", CLITypes.String)> Public Property MegaBlast As String

        ''' <summary>
        ''' Location on query sequence [String]  Optional
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-L", CLITypes.String)> Public Property QueryLocation As String

        ''' <summary>
        ''' Multiple Hits window size, default if zero (blastn/megablast 0, all others 40 [Integer]
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-A", CLITypes.Integer)> Public Property HitsWindowSize As String

        ''' <summary>
        ''' Frame shift penalty (OOF algorithm for blastx) [Integer]
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-w", CLITypes.Integer)> Public Property FrameShiftPenalty As String

        ''' <summary>
        ''' Length of the largest intron allowed in a translated nucleotide sequence when linking multiple distinct alignments. (0 invokes default behavior; a negative value disables linking.) [Integer]
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-t", CLITypes.Integer)> Public Property IntronLength As String

        ''' <summary>
        ''' Number of concatenated queries, for blastn and tblastn [Integer]  Optional
        ''' default = 0
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-B", CLITypes.Integer)> Public Property ConcatenatedQueries As String

        ''' <summary>
        ''' Force use of the legacy BLAST engine [T/F]  Optional
        ''' default = F
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-V", CLITypes.String)> Public Property ForceLegacy As String

        ''' <summary>
        ''' Use composition-based score adjustments for blastp or tblastn:
        ''' 
        ''' As first character:
        ''' D or d: default (equivalent to T)
        ''' 0 or F or f: no composition-based statistics
        ''' 2 or T or t: Composition-based score adjustments as in Bioinformatics 21:902-911,
        ''' 1: Composition-based statistics as in NAR 29:2994-3005, 2001
        ''' 2005, conditioned on sequence properties
        ''' 3: Composition-based score adjustment as in Bioinformatics 21:902-911,
        ''' 2005, unconditionally
        ''' For programs other than tblastn, must either be absent or be D, F or 0.
        ''' 
        ''' As second character, if first character is equivalent to 1, 2, or 3:
        ''' U or u: unified p-value combining alignment p-value and compositional p-value in round 1 only
        ''' 
        ''' default = D
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-C", CLITypes.String)> Public Property CompositionBasedScoreAdjustments As String

        ''' <summary>
        ''' Compute locally optimal Smith-Waterman alignments (This option is only available for gapped tblastn.) [T/F]
        ''' default = F
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-s", CLITypes.String)> Public Property SmithWatermanAlignments As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="BlastBin">Blast+ bin dir</param>
        ''' <remarks></remarks>
        Sub New(BlastBin As String)
            MyBase.New(BlastBin)
            BLASTALLAssembly = (BlastBin & "/blastall.exe").CLIPath
            FormatDbAssembly = (BlastBin & "/formatdb.exe").CLIPath
        End Sub

        Public Shared Widening Operator CType(BLASTBin As String) As LocalBLAST
            Return New LocalBLAST(BLASTBin)
        End Operator

#Region "LocalBLAST"

        ''' <summary>
        ''' The command line arguments of the blastp program.
        ''' (blastp程序的命令行参数)
        ''' </summary>
        ''' <remarks></remarks>
        Const ARGUMS_BLASTP As String = "-p blastp -i ""{0}"" -d ""{1}"" -o ""{2}"" -e {3}"
        ''' <summary>
        ''' The command line arguments of the blastn program.
        ''' </summary>
        ''' <remarks></remarks>
        Const ARGUMS_BLASTN As String = "-p blastn -i ""{0}"" -d ""{1}"" -o ""{2}"" -e {3}"

        Const ARGUMS_FORMATDB As String = "-i ""{0}"" -p {1} -o T"

        ''' <summary>
        ''' Generate the command line arguments of the program blastp.
        ''' (生成blastp程序的命令行参数)
        ''' </summary>
        ''' <param name="Input">The target sequence FASTA file.(包含有目标待比对序列的FASTA文件)</param>
        ''' <param name="TargetDb">The selected database that to aligned.(将要进行比对的目标数据库)</param>
        ''' <param name="Output"></param>
        ''' <param name="e"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Blastp(Input As File, TargetDb As String, Output As File, Optional e As String = "10") As Microsoft.VisualBasic.CommandLine.IORedirect
            Dim Argums As String = String.Format(ARGUMS_BLASTP, Input, TargetDb, Output, e)
            MyBase._InternalLastBLASTOutputFile = Output
            Dim Cmdl As String = String.Format("{0} {1}", BLASTALLAssembly, Argums)
            Call Console.WriteLine("BLASTP::" & vbCrLf & "  ---> {0}", Cmdl)
            Return New CommandLine.IORedirect(BLASTALLAssembly, Argums)
        End Function

        ''' <summary>
        ''' 返回BLAST程序的日志文件，本函数必须在执行完BLASTP或者BLASTN操作之后才可以调用，否则返回空对象
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function GetLastLogFile() As BLASTOutput.IBlastOutput
            If FileIO.FileSystem.FileExists(MyBase._InternalLastBLASTOutputFile) Then
                Return NCBI.Extensions.LocalBLAST.BLASTOutput.XmlFile.BlastOutput.LoadFromFile(MyBase._InternalLastBLASTOutputFile)
            Else
                Return Nothing
            End If
        End Function
#End Region

        Public Overloads Overrides Function Blastn(Input As String, TargetDb As String, Output As String, Optional e As String = "10") As CommandLine.IORedirect
            Dim Argums As String = String.Format(ARGUMS_BLASTN, Input, TargetDb, Output, e, Strand, Output & ".aln")
            MyBase._InternalLastBLASTOutputFile = Output
            Dim Cmdl As String = String.Format("{0} {1}", BLASTALLAssembly, Argums)
            Call Console.WriteLine("BLASTN::" & vbCrLf & "  ---> {0}", Cmdl)
            Return New CommandLine.IORedirect(BLASTALLAssembly, Argums)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Db"></param>
        ''' <param name="dbType">"T" for protein, "F" for nucleotide.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Overrides Function FormatDb(Db As String, dbType As String) As CommandLine.IORedirect
            Dim CommandLine As String = String.Format("{0} {1}", FormatDbAssembly, String.Format(ARGUMS_FORMATDB, Db, dbType))
            Call Console.WriteLine("FORMAT_DB::" & vbCrLf & "  ---> {0}", CommandLine)
            Return New CommandLine.IORedirect(FormatDbAssembly, String.Format(ARGUMS_FORMATDB, Db, dbType))
        End Function

        Public Overrides ReadOnly Property MolTypeNucleotide As String
            Get
                Return "F"
            End Get
        End Property

        Public Overrides ReadOnly Property MolTypeProtein As String
            Get
                Return "T"
            End Get
        End Property
    End Class
End Namespace
