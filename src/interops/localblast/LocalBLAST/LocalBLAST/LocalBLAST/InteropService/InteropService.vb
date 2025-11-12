#Region "Microsoft.VisualBasic::68b97573f24d4c15ac6ff204ae3f60f6, localblast\LocalBLAST\LocalBLAST\LocalBLAST\InteropService\InteropService.vb"

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

    '   Total Lines: 256
    '    Code Lines: 83 (32.42%)
    ' Comment Lines: 147 (57.42%)
    '    - Xml Docs: 91.84%
    ' 
    '   Blank Lines: 26 (10.16%)
    '     File Size: 10.63 KB


    '     Class LocalBlastProgramGroup
    ' 
    '         Properties: BlastBin, NumThreads, Version
    ' 
    '         Function: ToString
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    '     Class InteropService
    ' 
    ' 
    '         Enum AlignmentViewOptions
    ' 
    '             ASNBinary, ASNText, FlatQueryAnchored, FlatQueryAnchoredNoIdentities, FlatQueryAnchoredWithIdentities
    '             Pairwise, QueryAnchored, QueryAnchoredNoIdentities, QueryAnchoredShowingIdentities, Tabular
    '             TabularWithCommentLines, XML
    ' 
    ' 
    ' 
    '         Delegate Function
    ' 
    '             Properties: LastBLASTOutputFilePath
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: TryInvoke
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Diagnostics
Imports Microsoft.VisualBasic.CommandLine
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports CLI = Microsoft.VisualBasic.CommandLine.InteropService.InteropService
Imports File = System.String

Namespace LocalBLAST.InteropService

    Public MustInherit Class LocalBlastProgramGroup : Inherits CLI
        Implements System.IDisposable

        ''' <summary>
        ''' Blast程序组所在的安装文件夹
        ''' </summary>
        ''' <remarks></remarks>
        Protected _innerBLASTBinDIR As String
        ''' <summary>
        ''' The blast output file path of the last time blast operation.(上一次执行BLAST操作时所输出的日志文件)
        ''' </summary>
        ''' <remarks></remarks>
        Protected _InternalLastBLASTOutputFile As String

        ''' <summary>
        ''' The cpu core number usage of the blast operation.(进行blast操作所需要的线程数)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property NumThreads As Integer = 8

        ''' <summary>
        ''' Gets the directory which contains the local blast program group.(本地blast程序组所在的文件夹)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property BlastBin As String
            Get
                Return _innerBLASTBinDIR
            End Get
        End Property

        Public Overridable ReadOnly Property Version As String
            Get
                Return ""
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return _innerBLASTBinDIR
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class

    ''' <summary>
    ''' InteropService to the local blast program.(对本地BLAST程序的中间服务)
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class InteropService : Inherits LocalBlastProgramGroup

        ''' <summary>
        ''' Gets the command line parameter of the protein sequence molecular type.(获取蛋白质分子序列的命令行参数的值)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride ReadOnly Property MolTypeProtein As String
        ''' <summary>
        ''' Gets the command line parameter of the nucleotide sequence molecular type.(获取核酸序列类型的分子序列的命令行参数的值)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride ReadOnly Property MolTypeNucleotide As String

        ''' <summary>
        ''' Alignment view options.(比对输出视图的选项)
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum AlignmentViewOptions
            ''' <summary>
            ''' Pairwise
            ''' </summary>
            ''' <remarks></remarks>
            Pairwise
            ''' <summary>
            ''' Query-anchored showing identities
            ''' </summary>
            ''' <remarks></remarks>
            QueryAnchoredShowingIdentities
            ''' <summary>
            ''' Query-anchored no identities
            ''' </summary>
            ''' <remarks></remarks>
            QueryAnchoredNoIdentities
            ''' <summary>
            ''' Flat query-anchored, show identities
            ''' </summary>
            ''' <remarks></remarks>
            FlatQueryAnchoredWithIdentities
            ''' <summary>
            ''' Flat query-anchored, no identities
            ''' </summary>
            ''' <remarks></remarks>
            FlatQueryAnchoredNoIdentities
            ''' <summary>
            ''' Query-anchored no identities and blunt ends
            ''' </summary>
            ''' <remarks></remarks>
            QueryAnchored
            ''' <summary>
            ''' Flat query-anchored, no identities and blunt ends
            ''' </summary>
            ''' <remarks></remarks>
            FlatQueryAnchored
            ''' <summary>
            ''' XML Blast output
            ''' </summary>
            ''' <remarks></remarks>
            XML
            ''' <summary>
            ''' Tabular
            ''' </summary>
            ''' <remarks></remarks>
            Tabular
            ''' <summary>
            ''' Tabular with comment lines
            ''' </summary>
            ''' <remarks></remarks>
            TabularWithCommentLines
            ''' <summary>
            ''' ASN, text
            ''' </summary>
            ''' <remarks></remarks>
            ASNText
            ''' <summary>
            ''' ASN, binary [Integer]
            ''' </summary>
            ''' <remarks></remarks>
            ASNBinary
        End Enum

        Sub New(bin As String)
            _innerBLASTBinDIR = bin
            NumThreads = 2

            If Not bin.DirectoryExists Then
                Dim msg$ = $"localblast bin can not be found on the file system location: {bin$}!"
                Throw New ObjectNotFoundException(msg)
            End If
        End Sub

        ''' <summary>
        ''' Get the blast program output result file from the last BLAST operation. Filepath: <see cref="InteropService.LastBLASTOutputFilePath"></see>.
        ''' (获取上一个BLAST操作所输出的结果日志文件)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function GetLastLogFile() As IBlastOutput

        ''' <summary>
        ''' Get the last blast operation output log file path.(获取上一次BLAST操作的输出文件的文件名)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property LastBLASTOutputFilePath As String
            Get
                Return _InternalLastBLASTOutputFile
            End Get
        End Property

#Region "LocalBLAST"

        Public Delegate Function ILocalBLAST(InputQuery As File, TargetSubjectDb As String, Output As File, e As String) As IORedirect

        ''' <summary>
        ''' Generate the command line arguments of the program blastp.(生成blastp程序的命令行参数)
        ''' </summary>
        ''' <param name="InputQuery">The target sequence FASTA file.(包含有目标待比对序列的FASTA文件)</param>
        ''' <param name="TargetSubjectDb">The selected database that to aligned.(将要进行比对的目标数据库)</param>
        ''' <param name="Output">结果文件</param>
        ''' <param name="e">The E-value</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function Blastp(InputQuery As File, TargetSubjectDb As String, Output As File, Optional e As String = "10") As IORedirect

        ''' <summary>
        ''' Generate the command line arguments of the program blastn.(生成blastn程序的命令行参数)
        ''' </summary>
        ''' <param name="Input">The target sequence FASTA file.(包含有目标待比对序列的FASTA文件)</param>
        ''' <param name="TargetDb">The selected database that to aligned.(将要进行比对的目标FASTA数据库的文件名)</param>
        ''' <param name="Output">结果文件</param>
        ''' <param name="e">The E-value</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function Blastn(Input As File, TargetDb As String, Output As File, Optional e As String = "10") As IORedirect

        ''' <summary>
        ''' Format theta target fasta sequence database for the blast search.
        ''' </summary>
        ''' <param name="Db">The name of the target formated db.(目标格式化数据库的名称)</param>
        ''' <param name="dbType">Database type for the target sequence database(目标序列数据库的分子类型)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function FormatDb(Db As String, dbType As String) As IORedirect
#End Region

        ''' <summary>
        ''' Try invoke the blast program based on its program name as the executable file name.
        ''' </summary>
        ''' <param name="Program">The program file name</param>
        ''' <param name="Query"></param>
        ''' <param name="Subject"></param>
        ''' <param name="Evalue"></param>
        ''' <param name="Output"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TryInvoke(Program As String, Query As String, Subject As String, Evalue As String, Output As String) As IORedirect
            Dim argvs As String = String.Format("-query ""{0}"" -subject ""{1}"" -evalue {2} -out ""{3}""", Query, Subject, Evalue, Output)
            Program = _innerBLASTBinDIR & "/" & Program

            Return New IORedirect(Program, argvs, IOredirect:=False, hide:=False)
        End Function
    End Class
End Namespace
