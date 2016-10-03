#Region "Microsoft.VisualBasic::898d2018cd7dfb89ca65f7a72019580e, ..\interops\meme_suite\MEME\Programs\MEME.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection

Namespace Programs

    Public Class MEME : Inherits Microsoft.VisualBasic.CommandLine.InteropService

#Region "Private Fileds"
        Dim _IsDNA, _IsProtein, _Palindromes As Boolean
#End Region

        ''' <summary>
        ''' Print this message
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-h")> Public Property h As String
        ''' <summary>
        ''' name of directory for output files will not replace existing directory
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-o")> Public Property OutputDir As String
        ''' <summary>
        ''' name of directory for output files will replace existing directory
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-oc")> Public Property OutputDir2 As String
        ''' <summary>
        ''' output in text format (default is HTML)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-text")> Public Property IsTextOutput As Boolean
        ''' <summary>
        ''' sequences use DNA alphabet
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-dna")> Public Property IsDNA As Boolean
            Get
                Return _IsDNA
            End Get
            Set(value As Boolean)
                If value Then
                    _IsProtein = False
                End If
            End Set
        End Property
        ''' <summary>
        ''' sequences use protein alphabet
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-protein")> Public Property IsProtein As Boolean
            Get
                Return _IsProtein
            End Get
            Set(value As Boolean)
                If value Then
                    _IsDNA = False
                End If
            End Set
        End Property

        ''' <summary>
        ''' distribution types enum of motifs
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum Modes
            oops
            zoops
            anr
        End Enum
        ''' <summary>
        ''' oops|zoops|anr, distribution of motifs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-mod")> Public Property Mode As Modes
        ''' <summary>
        ''' maximum number of motifs to find
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-nmotifs")> Public Property NumMotifs As Integer
        ''' <summary>
        ''' stop if motif E-value greater than [evt]
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-evt")> Public Property Ev As Double
        ''' <summary>
        ''' number of sites for each motif
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-nsites")> Public Property Sites As Integer
        ''' <summary>
        ''' minimum number of sites for each motif
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-minsites")> Public Property MinSites As Integer
        ''' <summary>
        ''' maximum number of sites for each motif
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-maxsites")> Public Property MaxSites As Integer
        ''' <summary>
        ''' weight on expected number of sites
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-wnsites")> Public Property WnSites As Double
        <[Optional]("-w")> Public Property MotifWidth As Integer
        ''' <summary>
        ''' minumum motif width
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-minw")> Public Property MinWidth As Integer
        ''' <summary>
        ''' maximum motif width
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-maxw")> Public Property MaxWidth As Integer
        ''' <summary>
        ''' do not adjust motif width using multiple alignments
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-nomatrim")> Public Property NomaTrim As Boolean
        ''' <summary>
        ''' gap opening cost for multiple alignments
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-wg")> Public Property Wg As Double
        ''' <summary>
        ''' gap extension cost for multiple alignments
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-ws")> Public Property Ws As Double
        ''' <summary>
        ''' do not count end gaps in multiple alignments
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-noendgaps")> Public Property NoEndGaps As Boolean
        ''' <summary>
        ''' name of background Markov model file
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-bfile")> Public Property BackgroundFile As String
        ''' <summary>
        ''' allow sites on + or - DNA strands
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-revcomp")> Public Property RevComp As Boolean
        ''' <summary>
        ''' force palindromes (requires -dna)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-pal")> Public Property Palindromes As Boolean
            Get
                If _IsDNA Then
                    Return _Palindromes
                Else
                    Return False
                End If
            End Get
            Set(value As Boolean)
                _Palindromes = value
            End Set
        End Property
        ''' <summary>
        ''' maximum EM iterations to run
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-maxiter")> Public Property MaxIter As Integer
        ''' <summary>
        ''' EM convergence criterion
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-distance")> Public Property Distance As String
        ''' <summary>
        ''' name of positional priors file
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-psp")> Public Property PspFile As String

        Public Enum PriorTypes
            dirichlet
            dmix
            mega
            megap
            addone
        End Enum

        ''' <summary>
        ''' dirichlet|dmix|mega|megap|addone, type of prior to use
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-prior")> Public Property PriorType As PriorTypes
        ''' <summary>
        ''' strength of the prior
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-b")> Public Property PriorStrength As Integer
        ''' <summary>
        ''' name of Dirichlet prior file
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-plib")> Public Property PLib As String
        ''' <summary>
        ''' fuzziness of sequence to theta mapping
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-spfuzz")> Public Property SpFuzz As String
        Public Enum SpMaps
            uni
            pam
        End Enum
        ''' <summary>
        ''' uni|pam, starting point seq to theta mapping type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-spmap")> Public Property SpMap As SpMaps
        ''' <summary>
        ''' consensus sequence to start EM from
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-cons")> Public Property Cons As String
        ''' <summary>
        ''' size of heaps for widths where substring search occurs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-heapsize")> Public Property Hs As Integer
        ''' <summary>
        ''' perform x-branching
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-x_branch")> Public Property XBranch As Boolean
        ''' <summary>
        ''' perform width branching
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-w_branch")> Public Property WBranch As Boolean
        ''' <summary>
        ''' branching factor for branching search
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-bfactor")> Public Property BranchingFactor As Double
        ''' <summary>
        ''' maximum dataset size in characters
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-maxsize")> Public Property MaxSize As Integer
        ''' <summary>
        ''' do not print progress reports to terminal
        ''' </summary>
        ''' <remarks></remarks>
        <[Optional]("-nostatus")> Public Property NoStatus As Boolean
        ''' <summary>
        ''' use parallel version with [np] processors
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-p")> Public Property Np As Boolean
        ''' <summary>
        ''' quit before number of CPU seconds consumed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-time")> Public Property Time As Integer
        ''' <summary>
        ''' print [sf] as name of sequence file
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-sf")> Public Property sf As String
        ''' <summary>
        ''' verbose mode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <[Optional]("-V")> Public Property Verbose As Boolean

        Public Const REQUIRED_ARGUMENTS As String = """{0}"""

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="DataSet">file containing sequences in FASTA format</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Invoke(DataSet As String) As IORedirect
            Dim Optionals As String = CLIBuildMethod.GetCLI(Of MEME)(app:=Me)
            Dim arguments As String = String.Format(MEME.REQUIRED_ARGUMENTS, DataSet) & " " & Optionals
            Return New IORedirect(Me._executableAssembly, arguments)
        End Function

        Sub New(AssemblyPath As String)
            If Not FileIO.FileSystem.FileExists(AssemblyPath) Then
                Call SMRUCC.genomics.Interops.NBCR.MEME_Suite.TMoD.InitializeSession()
                AssemblyPath = SMRUCC.genomics.Interops.NBCR.MEME_Suite.TMoD.TMod.MEME
            End If

            MyBase._executableAssembly = AssemblyPath
        End Sub
    End Class
End Namespace
