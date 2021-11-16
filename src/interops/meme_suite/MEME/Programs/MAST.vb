#Region "Microsoft.VisualBasic::bfbd9eca5de828aff82c42e6684584a2, meme_suite\MEME\Programs\MAST.vb"

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

    '     Class MAST
    ' 
    '         Properties: BackgroundFile, Best, Composition, Count, DbList
    '                     df, Diag, dl, DNA, EValue
    '                     HitList, Mev, mf, MotifNumber, ms
    '                     mt, NoHtml, NoReverse, NoStatus, NoText
    '                     OutputDir, OutputOverwriting, RemoveMotifs, Separate, SeqP
    '                     WeakMatches
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Invoke
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports CLIApp = Microsoft.VisualBasic.CommandLine.InteropService.InteropService

Namespace Programs

    ''' <summary>
    ''' MAST: Motif Alignment and Search Tool
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MAST : Inherits CLIApp
        ' ## inputs
        ''' <summary>
        ''' read background frequencies from file
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-bfile")> Public Property BackgroundFile As String
        ''' <summary>
        ''' read the sequence file as a list of FASTA-formatted databases
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-dblist")> Public Property DbList As Boolean
        ' ## Outputs
        ''' <summary>
        ''' directory to output mast results; directory must not exist
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-o")> Public Property OutputDir As String
        ''' <summary>
        ''' directory to output mast results with overwriting allowed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-oc")> Public Property OutputOverwriting As Boolean
        ''' <summary>
        ''' print a machine-readable list of all hits only; outputs to standard out and overrides -seqp
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-hit_list")> Public Property HitList As Boolean
        ' ## Which Motifs To Use
        ''' <summary>
        ''' remove highly correlated motifs from query
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-remcorr")> Public Property RemoveMotifs As Boolean
        ''' <summary>
        ''' use only motif number m (overrides -mev); this can be repeated to select multiple motifs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-m")> Public Property MotifNumber As Integer
        ''' <summary>
        ''' only use the first count motifs or all motifs when count is zero (default: 0)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-c")> Public Property Count As Integer
        ''' <summary>
        ''' use only motifs with E-values less than mev
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-mev")> Public Property Mev As Integer
        ''' <summary>
        ''' nominal order and spacing of motifs is specified by diag which is a block diagram
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-diag")> Public Property Diag As Integer
        ' ## DNA-Only Options
        ''' <summary>
        ''' do not score reverse complement DNA strand
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-norc")> Public Property NoReverse As Boolean
        ''' <summary>
        ''' score reverse complement DNA strand as a separate sequence
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-sep")> Public Property Separate As Boolean
        ''' <summary>
        ''' translate DNA sequences to protein; motifs must be protein; sequences must be DNA
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-dna")> Public Property DNA As Boolean
        ''' <summary>
        ''' adjust p-values and E-values for sequence composition
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-comp")> Public Property Composition As Boolean
        ' ## Which Results To Print
        ''' <summary>
        ''' print results for sequences with E-value small than ev (default: 10)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-ev")> Public Property EValue As Double
        ' ## Appearance Of Block Diagrams
        ''' <summary>
        ''' show motif matches with p-value smaller than mt (default: 0.0001)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-mt")> Public Property mt As Double
        ''' <summary>
        ''' show weak matches (mt small than p-value and small than mt*10) in angle brackets in the hit list or when the xml is converted to text
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-w")> Public Property WeakMatches As Boolean
        ''' <summary>
        ''' include only the best motif hits in -hit_list diagrams
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-best")> Public Property Best As Boolean
        ''' <summary>
        ''' use SEQUENCE p-values for motif thresholds (default: use POSITION p-values)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-seqp")> Public Property SeqP As Boolean
        ' ## Miscellaneous
        ''' <summary>
        ''' in results use mf as motif file name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-mf")> Public Property mf As String
        ''' <summary>
        ''' in results use df as database name (ignored when -dblist)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-df")> Public Property df As String
        ''' <summary>
        ''' in results use dl as link to search sequence names; token SEQUENCEID is replaced with the FASTA sequence ID; ignored when -dblist;
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-dl")> Public Property dl As String
        ''' <summary>
        ''' lower bound on number of sequences in db
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-minseqs")> Public Property ms As String
        ''' <summary>
        ''' do not print progress report
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-nostatus")> Public Property NoStatus As Boolean
        ''' <summary>
        ''' do not create text output
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-notext")> Public Property NoText As Boolean
        ''' <summary>
        ''' do not create html output
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Argv("-nohtml")> Public Property NoHtml As Boolean

        Const MAST_COMMANDLINES As String = "{0} ""{1}"" ""{2}"" {3}"

        ''' <summary>
        ''' mast motif_file sequence_file [options]
        ''' </summary>
        ''' <param name="motifFile">file containing motifs to use; normally a MEME output file</param>
        ''' <param name="SequenceFile">search sequences in FASTA-formatted database with motifs</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Invoke(motifFile As String, SequenceFile As String) As IORedirect
            Dim OptionalArguments As String = CLIBuildMethod.GetCLI(Me)
            Return String.Format(MAST_COMMANDLINES, MyBase._executableAssembly, motifFile, SequenceFile, OptionalArguments)
        End Function

        Sub New(MAST As String)
            MyBase._executableAssembly = MAST
        End Sub
    End Class
End Namespace
