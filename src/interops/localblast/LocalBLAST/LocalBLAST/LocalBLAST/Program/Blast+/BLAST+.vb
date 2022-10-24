#Region "Microsoft.VisualBasic::151392de126b7c63e48e19590cf5a9a7, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Program\Blast+\BLAST+.vb"

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

    '     Class BLASTPlus
    ' 
    '         Properties: BlastnOptionalArguments, BlastpOptionalArguments, MolTypeNucleotide, MolTypeProtein, Version
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Blastn, Blastp, FormatDb, GetHelp, GetLastLogFile
    '                   GetManual
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine

Namespace LocalBLAST.Programs

    ''' <summary>
    ''' The ``&lt;space>`` char can not exists in the input fasta file path, or blast+ program will run into an error.
    ''' (请注意：当目标FASTA序列文件的文件路径中的空格字符过多的时候，BLAST+程序组将不能够很好的工作，故而当程序出错的时候，
    ''' 请检查文件路径是否存在此问题)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BLASTPlus : Inherits InteropService.InteropService

        ''' <summary>
        ''' USAGE and DESCRIPTION
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetHelp() As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Get USAGE, DESCRIPTION and ARGUMENTS
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetManual() As String
            Throw New NotImplementedException
        End Function

        Public Property BlastpOptionalArguments As CLIArgumentsBuilder.BlastpOptionalArguments
        Public Property BlastnOptionalArguments As CLIArgumentsBuilder.BlastnOptionalArguments

        Dim _makeBlastDbAsm As String
        Dim _blastpAssembly, _blastnAssembly As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="bin">The ``bin`` directory for the NCBI blast+ suite.</param>
        Sub New(bin As String)
            Call MyBase.New(bin)

            _makeBlastDbAsm = String.Format("{0}\makeblastdb.exe", bin).CLIPath
            _blastpAssembly = String.Format("{0}\blastp.exe", bin).CLIPath
            _blastnAssembly = String.Format("{0}\blastn.exe", bin).CLIPath
        End Sub

        Const MAKE_BLAST_DB_PROT As String = "-dbtype prot -in ""{0}"""
        Const MAKE_BLAST_DB_NUCL As String = "-dbtype nucl -in ""{0}"""

        Const BLAST_PLUS_ARGUMS As String = "-query ""{0}"" -db ""{1}"" -evalue {2} -out ""{3}"" -num_threads {4}"

        ''' <summary>
        ''' 这个函数只是生成了一个blastp的操作任务，但是并没有被启动
        ''' </summary>
        ''' <param name="Input"></param>
        ''' <param name="TargetDb"></param>
        ''' <param name="Output"></param>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Public Overrides Function Blastp(Input As String, TargetDb As String, Output As String, Optional e As String = "10") As IORedirectFile
            If String.IsNullOrEmpty(e) Then
                e = "1e-3"
            End If

            Dim DIR As String = FileIO.FileSystem.GetFileInfo(Output).Directory.FullName

            Call FileIO.FileSystem.CreateDirectory(DIR)

            Dim argv As String = String.Format(BLAST_PLUS_ARGUMS, Input, TargetDb, e, Output, NumThreads)
            Dim Cmdl As String = String.Format("{0} {1}", _blastpAssembly, argv)
            Console.WriteLine("LOCALBLAST+::BLASTP" & vbCrLf & "  ---> {0}", Cmdl)
            MyBase._InternalLastBLASTOutputFile = Output
            Return New IORedirectFile(_blastpAssembly, argv)
        End Function

        Public Overrides Function GetLastLogFile() As BLASTOutput.IBlastOutput
            Return BLASTOutput.BlastPlus.Parser.TryParse(MyBase._InternalLastBLASTOutputFile)
        End Function

        Public Overloads Overrides Function Blastn(Input As String, TargetDb As String, Output As String, Optional e As String = "10") As IORedirectFile
            If String.IsNullOrEmpty(e) Then
                e = "1e-3"
            End If

            Dim DIR As String = FileIO.FileSystem.GetFileInfo(Output).Directory.FullName
            Call FileIO.FileSystem.CreateDirectory(DIR)

            Dim Argums As String = String.Format(BLAST_PLUS_ARGUMS, Input, TargetDb, e, Output, NumThreads)

            If BlastnOptionalArguments?.WordSize > 0 Then
                Argums &= " -word_size " & BlastnOptionalArguments.WordSize
            End If
            If BlastnOptionalArguments?.penalty > 0 Then
                Argums &= $" -penalty {BlastnOptionalArguments.penalty} "
            End If
            If BlastnOptionalArguments?.reward > 0 Then
                Argums &= $" -reward {BlastnOptionalArguments.reward}"
            End If

            Dim Cmdl As String = String.Format("{0} {1}", _blastnAssembly, Argums)
            Console.WriteLine("LOCALBLAST+::BLASTN" & vbCrLf & "  ---> {0}", Cmdl)
            MyBase._InternalLastBLASTOutputFile = Output
            Return New IORedirectFile(_blastnAssembly, argv:=Argums)
        End Function

        Public Overloads Overrides Function FormatDb(Db As String, dbType As String) As IORedirectFile
            Dim Argums As String
            If String.Equals(dbType, "prot") Then
                Argums = String.Format(MAKE_BLAST_DB_PROT, Db)
            Else
                Argums = String.Format(MAKE_BLAST_DB_NUCL, Db)
            End If

            Dim Cmdl As String = String.Format("{0} {1}", _makeBlastDbAsm, Argums)
            Console.WriteLine("LOCALBLAST+::MAKE_BLAST_DB" & vbCrLf & "  ---> {0}", Cmdl)
            Return New IORedirectFile(_makeBlastDbAsm, argv:=Argums, win_os:=True)
        End Function

        ''' <summary>
        ''' The nucleotide molecular type value for the makeblastdb operation.(用于makeblastdb操作所需要的核酸链分子的命令行参数值)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property MolTypeNucleotide As String
            Get
                Return "nucl"
            End Get
        End Property

        ''' <summary>
        ''' The protein molecular type value for the makeblastdb operation.(用于makeblastdb操作所需要的蛋白质分子的命令行参数值)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property MolTypeProtein As String
            Get
                Return "prot"
            End Get
        End Property

        ''' <summary>
        ''' Gets the blast program group version.(获取blast程序组的版本信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Version As String
            Get
                Dim Process As IORedirect = New IORedirect(Me._blastnAssembly, "-version", hide:=False)
                Call Process.Start(True)
                Dim str As String = Regex.Split(Process.StandardOutput, "Package:\s+", RegexOptions.IgnoreCase).Last.Trim
                Return str
            End Get
        End Property
    End Class
End Namespace
