#Region "Microsoft.VisualBasic::9de3879bfc56a02d8b1a2f936d632d12, ..\LibMySQL\Reflector\CLIProgram.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Oracle.LinuxCompatibility.MySQL
Imports Microsoft.VisualBasic.ComponentModel
Imports System.Text

Module CLIProgram

    Public Function Main() As Integer
        Return GetType(CLIProgram).RunCLI(args:=App.CommandLine)
    End Function

    Const InputsNotFound As String = "The required input parameter ""/sql"" is not specified!"

    <ExportAPI("--reflects",
               Info:="Automatically generates visualbasic source code from the MySQL database schema dump.",
               Usage:="--reflects /sql <sql_path> [-o <output_path> /namespace <namespace> /split]",
               Example:="--reflects /sql ./test.sql")>
    <ParameterInfo("/sql", False,
                   Description:="The file path of the MySQL database schema dump file."),
     ParameterInfo("-o", True,
                   Description:="The output file path of the generated visual basic source code file from the SQL dump file ""/sql"""),
     ParameterInfo("/namespace", True,
                   Description:="The namespace value will be insert into the generated source code if this parameter is not null.")>
    Public Function ReflectsConvert(argvs As CommandLine) As Integer
        If Not argvs.CheckMissingRequiredParameters("/sql").IsNullOrEmpty Then
            Call VBDebugger.Warning(InputsNotFound)
            Return -1
        End If

        Dim SQL As String = argvs("/sql"), out As String = argvs("-o")
        Dim ns As String = argvs("/namespace")

        If FileIO.FileSystem.FileExists(SQL) Then
            Return __EXPORT(SQL, ns, out, argvs.GetBoolean("/split"))
        Else
            Dim msg As String = $"The target schema sql dump file ""{SQL}"" is not exists on your file system!"
            Call VBDebugger.PrintException(msg)
            Return -2
        End If

        Return 0
    End Function

    Private Function __EXPORT(SQL As String, ns As String, out As String, split As Boolean) As Integer
        If split Then
            If String.IsNullOrEmpty(out) Then
                out = FileIO.FileSystem.GetParentPath(SQL)
                out = $"{out}/{IO.Path.GetFileNameWithoutExtension(SQL)}/"
            End If

            Call FileIO.FileSystem.CreateDirectory(out)

            For Each doc As KeyValuePair(Of String, String) In CodeGenerator.GenerateCodeSplit(SQL, ns)
                Call doc.Value.SaveTo($"{out}/{doc.Key}.vb", Encoding.Unicode)
            Next
        Else
            If String.IsNullOrEmpty(out) Then
                out = FileIO.FileSystem.GetParentPath(SQL)
                out = $"{out}/{SQL.BaseName}.vb"
            End If

            Dim doc As String = CodeGenerator.GenerateCode(SQL, ns)  ' Convert the SQL file into a visualbasic source code
            Return CInt(doc.SaveTo(out, Encoding.Unicode))           ' Save the vb source code into a text file
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
            Function(sql) CodeGenerator.GenerateClass(sql.ReadAllText, ns))

        For Each cls As KeyValuePair In LQuery
            Dim vb As String = $"{outDIR}/{cls.Key}.vb"
            Call cls.Value.SaveTo(vb)
        Next

        Return LQuery.IsNullOrEmpty.CLICode
    End Function
End Module

