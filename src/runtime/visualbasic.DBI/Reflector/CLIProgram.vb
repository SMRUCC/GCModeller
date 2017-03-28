#Region "Microsoft.VisualBasic::6764b110a41e488a643ac5ba98ab085c, ..\visualbasic.DBI\Reflector\CLIProgram.vb"

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

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.CodeSolution

<PackageNamespace("MySQL.Reflector", Description:="Tools for convert the mysql schema dump sql script into VisualBasic classes source code.")>
Module CLIProgram

    Public Function Main() As Integer
        Return GetType(CLIProgram).RunCLI(args:=App.CommandLine)
    End Function

    Const InputsNotFound As String = "The required input parameter ""/sql"" is not specified!"

    <ExportAPI("--reflects",
               Info:="Automatically generates visualbasic source code from the MySQL database schema dump.",
               Usage:="--reflects /sql <sql_path/std_in> [-o <output_path> /namespace <namespace> /split]",
               Example:="--reflects /sql ./test.sql /split /namespace ExampleNamespace")>
    <Argument("/sql", False,
                   AcceptTypes:={GetType(String)},
                   Description:="The file path of the MySQL database schema dump file."),
     Argument("-o", True,
                   AcceptTypes:={GetType(String)},
                   Description:="The output file path of the generated visual basic source code file from the SQL dump file ""/sql"""),
     Argument("/namespace", True,
                   AcceptTypes:={GetType(String)},
                   Description:="The namespace value will be insert into the generated source code if this parameter is not null.")>
    <Argument("/split", True,
                   AcceptTypes:={GetType(Boolean)},
                   Description:="Split the source code into sevral files and named by table name?")>
    Public Function ReflectsConvert(args As CommandLine) As Integer
        Dim split As Boolean = args.GetBoolean("/split")
        Dim SQL As String = args("/sql"), out As String = args("-o")
        Dim ns As String = args("/namespace")

        If Not SQL.FileExists Then  ' 当文件不存在的时候可能是std_in，则判断是否存在out并且是split状态
            If split AndAlso String.IsNullOrEmpty(out) Then
                Call VBDebugger.Warning(InputsNotFound)
                Return -1
            End If
        End If

        If FileIO.FileSystem.FileExists(SQL) Then
            Dim writer As StreamWriter = Nothing
            If Not split Then
                writer = args.OpenStreamOutput("-o")
            End If
            Return __EXPORT(SQL, args.OpenStreamInput("/sql"), ns, out, writer, split)
        Else
            Dim msg As String = $"The target schema sql dump file ""{SQL}"" is not exists on your file system!"
            Call VBDebugger.PrintException(msg)
            Return -2
        End If

        Return 0
    End Function

    Private Function __EXPORT(SQL As String, file As StreamReader, ns As String, out As String, output As StreamWriter, split As Boolean) As Integer
        If split Then ' 分开文档的输出形式，则不能够使用stream了
            If String.IsNullOrEmpty(out) Then
                out = FileIO.FileSystem.GetParentPath(SQL)
                out = $"{out}/{IO.Path.GetFileNameWithoutExtension(SQL)}/"
            End If

            Call FileIO.FileSystem.CreateDirectory(out)

            For Each doc As KeyValuePair(Of String, String) In VisualBasic.CodeGenerator.GenerateCodeSplit(file, ns, SQL)
                Call doc.Value.SaveTo($"{out}/{doc.Key}.vb", Encoding.Unicode)
            Next
        Else ' 整个的文档形式
            If output Is Nothing Then
                If String.IsNullOrEmpty(out) Then
                    out = FileIO.FileSystem.GetParentPath(SQL)
                    out = $"{out}/{SQL.BaseName}.vb"
                End If

                Dim doc As String = VisualBasic.CodeGenerator.GenerateCode(file, ns, SQL)  ' Convert the SQL file into a visualbasic source code
                Return doc.SaveTo(out, Encoding.Unicode).CLICode               ' Save the vb source code into a text file
            Else
                Call output.Write(VisualBasic.CodeGenerator.GenerateCode(file, ns, SQL))
                Call output.Flush()
            End If
        End If

        Return 0
    End Function

    <ExportAPI("--export.dump",
               Usage:="--export.dump [-o <out_dir> /namespace <namespace> --dir <source_dir>]")>
    Public Function ExportDumpDir(args As CommandLine) As Integer
        Dim DIR As String = args("--dir")
        Dim ns As String = args("/namespace")
        Dim outDIR As String = args("-o")

        If String.IsNullOrEmpty(DIR) Then
            DIR = App.CurrentDirectory
        End If
        If String.IsNullOrEmpty(outDIR) Then
            outDIR = App.CurrentDirectory & "/MySQL_Tables/"
        End If

        Call FileIO.FileSystem.CreateDirectory(outDIR)

        Dim SQLs As IEnumerable(Of String) = ls - l - wildcards("*.sql") <= DIR
        Dim LQuery = SQLs.ToArray(
            Function(sql) VisualBasic.CodeGenerator.GenerateClass(sql.ReadAllText, ns))

        For Each cls As NamedValue(Of String) In LQuery
            Dim vb As String = $"{outDIR}/{cls.Name}.vb"
            Call cls.Value.SaveTo(vb)
        Next

        Return LQuery.IsNullOrEmpty.CLICode
    End Function
End Module
