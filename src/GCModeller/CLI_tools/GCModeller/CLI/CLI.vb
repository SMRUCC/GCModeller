#Region "Microsoft.VisualBasic::9655b08dbaeb6f66b965c6aabd613c37, CLI_tools\GCModeller\CLI\CLI.vb"

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

    ' Module CLI
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: About, FileMerges, InstallMySQL, Intersect, Kmeans
    '               MultipleAlignment, Register
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri

<Package("GCModeller.CLI", Publisher:="xie.guigang@gcmodeller.org", Category:=APICategories.CLI_MAN, Url:="http://gcmodeller.org")>
Public Module CLI

    <ExportAPI("/Intersect")>
    <Usage("/Intersect /a <list.txt> /b <list.txt> [/out <list.txt>]")>
    Public Function Intersect(args As CommandLine) As Integer
        Dim a$() = (args <= "/a").ReadAllLines
        Dim b$() = (args <= "/b").ReadAllLines
        Dim out$ = args.GetValue("/out", (args <= "/a").TrimSuffix & "-" & (args <= "/b").BaseName & ".txt")
        Return a.Intersect(b).FlushAllLines(out).CLICode
    End Function

    <ExportAPI("/kmeans")>
    <Usage("/kmeans /in <matrix.csv> [/cluster.n <default=6> /out <out.csv>]")>
    Public Function Kmeans(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim n% = args.GetValue("/cluster.n", 6)
        Dim out$ = (args <= "/out") Or ([in].TrimSuffix & $"-clusters={n}.csv").AsDefault
        Dim matrix As DataSet() = DataSet.LoadDataSet([in]).ToArray
        Dim clusters = matrix _
            .ToKMeansModels _
            .Kmeans(expected:=n) _
            .ToArray

        Return clusters.SaveTo(out).CLICode
    End Function

    <ExportAPI("help", Example:="gc help", Usage:="gc help", Info:="Show help information about this program.")>
    Public Function About() As Integer
        printf("gc database administration")

        Return 0
    End Function

    ''' <summary>
    ''' 创建一个用户
    ''' </summary>
    ''' <param name="CommandLine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("--user.create")>
    Public Function Register(CommandLine As CommandLine) As Integer
        Dim userName As String = CommandLine("-user")
        Dim password As String = CommandLine("-pwd")
        Dim Database As String = CommandLine("-db_name")
        Dim uri As ConnectionUri = MySQLExtensions.MySQL

        Try
            Dim MYSQL = New MySqli

            Call MYSQL.Connect(uri)  '连接数据库服务器
            Call MYSQL.Execute(String.Format("CREATE DATABASE {0};", Database)) '创建数据库
            Call MYSQL.Execute(String.Format("", userName, password))   '创建用户
            Call MYSQL.Execute(String.Format("", userName, Database))  '修改用户权限

            Return 0
        Catch ex As Exception
            ex = New Exception(CommandLine.ToString, ex)
            Call ex.PrintException
            Call App.LogException(ex)

            Return -1
        End Try
    End Function

    '''' <summary>
    '''' 从MYSQL数据库服务器之中导出计算数据
    '''' </summary>
    '''' <param name="args"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    '<ExportAPI("export", Info:="Export the calculation data from a specific data table in the mysql database server.",
    '    Usage:="export -mysql <mysql_connection_string> [-o <output_save_file/dir> -t <table_name>]",
    '    Example:="export -t all -o ~/Desktop/ -mysql ""http://localhost:8080/client?user=username%password=password%database=database""")>
    '<Argument("-t", True,
    '    Description:="Optional, The target table name for export the data set, there is a option value for this switch: all." & vbCrLf &
    '                 " <name> - export the data in the specific name of the table;" & vbCrLf &
    '                 " all - Default, export all of the table in the database, and at the mean time the -o switch value will be stand for the output directory of the exported csv files.")>
    '<Argument("-o", True,
    '    Description:="Optional, The path of the export csv file save, it can be a directory or a file path, depend on the value of the -t switch value." & vbCrLf &
    '                "Default is desktop directory and table name combination")>
    '<Argument("-mysql",
    '    Description:="The mysql connection string for gc program connect to a specific mysql database server.")>
    'Public Function ExportData(args As CommandLine) As Integer
    '    Dim Cnn As String = args("-mysql")  '获取与MySQL服务器的连接URL
    '    Dim Table As String = args("-t") '获取表名称
    '    Dim Output As String = args("-o") '获取数据输出位置
    '    Dim ExportDirectory As Boolean = False

    '    If String.IsNullOrEmpty(Output) Then
    '        Output = My.Computer.FileSystem.SpecialDirectories.Desktop
    '        ExportDirectory = True
    '    End If

    '    If String.IsNullOrEmpty(Cnn) Then '没有与数据库服务器的连接参数，程序抛出异常
    '        Return -1
    '    End If

    '    Using ExportService As DataExport = New DataExport
    '        Dim path As String
    '        Call ExportService.Connect(uri:=Cnn)

    '        If String.IsNullOrEmpty(Table) Or String.Equals(Table, "all") Then  'Default is export all of the table
    '            Call FileIO.FileSystem.CreateDirectory(Output)

    '            Dim Tables As String() = ExportService.GetStorageTables

    '            For Each Name As String In Tables
    '                path = Output & "/" & Name & ".csv"

    '                Call Console.WriteLine("Export Table ""{0}""", path)

    '                Call ExportService.FetchData(Name)
    '                Call ExportService.Export.Save(path, False)
    '            Next
    '        Else
    '            If ExportDirectory Then
    '                path = Output & "/" & Table & ".csv"
    '            Else
    '                path = Output
    '            End If

    '            Call ExportService.FetchData(Table)
    '            Call ExportService.Export.Save(path, False)
    '        End If
    '    End Using

    '    Return 0
    'End Function

    <ExportAPI("--install.MYSQL", Usage:="--install.MYSQL /user <userName> /pass <password> /repository <host_ipAddress> [/port 3306 /database <GCModeller>]")>
    Public Function InstallMySQL(args As CommandLine) As Integer
        Dim user As String = args("/user")
        Dim pass As String = args("/pass")
        Dim IPAddress As String = args("/repository")
        Dim Port As Integer = args.GetValue("/port", 3306)
        Dim DbName As String = args("/database")
        Dim uri As New ConnectionUri With {
            .Database = DbName,
            .IPAddress = IPAddress,
            .Password = pass,
            .Port = Port,
            .User = user
        }
        MySQLExtensions.MySQL = uri
        Call Settings.SettingsFile.Save()
        Return 0
    End Function

    Sub New()
        Call Settings.Initialize()
    End Sub

    Public Function MultipleAlignment(args As CommandLine) As Integer

    End Function

    ''' <summary>
    ''' 只是对文本文件进行合并处理
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' ###### 2016-11-26: 合并fasta文件的时候会出现问题？ 已解决，是输出的文件的textEncoding的问题，fasta文件应该是ASCII的
    ''' ```
    ''' BLAST options error: D:\11.24\Aedes-Anopheles-Culex.fasta does not match input format type, default input type is FASTA
    ''' ```
    ''' </remarks>
    <ExportAPI("/Merge.Files",
               Info:="Tools that works on the text files merge operation. This tool is usually used for merge of the fasta files into a larger fasta file.",
               Usage:="/Merge.Files /in <in.DIR> [/trim /ext <*.txt> /encoding <ascii> /out <out.txt>]")>
    <Group(CLIGrouping.GCModellerAppTools)>
    <Argument("/encoding", True, CLITypes.String,
              AcceptTypes:={GetType(Encodings)},
              Description:="Specific the output text file encoding value, default is ASCII encoding. Fasta file merge must be ASCII encoding output")>
    Public Function FileMerges(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim ext As String = args.GetValue("/ext", "*.txt")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ext.Replace("*", ""))
        Dim exts = ext.Split(","c).Select(AddressOf Trim).ToArray
        Dim trimNull = args.GetBoolean("/trim")
        Dim encoding As String = args.GetValue("/encoding", "ascii")
        Dim encodings As Encodings = encoding.ParseEncodingsName

        Using writer As StreamWriter = out.OpenWriter(encodings)
            For Each file$ In ls - l - r - exts <= [in]
                If trimNull Then
                    For Each line$ In file _
                        .IterateAllLines _
                        .Where(Function(s) Not String.IsNullOrEmpty(s))

                        Call writer.WriteLine(line)
                    Next
                Else
                    For Each line$ In file.IterateAllLines
                        Call writer.WriteLine(line)
                    Next
                End If
            Next

            Return 0
        End Using
    End Function
End Module
