#Region "Microsoft.VisualBasic::9acc928938c29be832c89fa2b38a1431, ..\CLI_tools\c2\CommandLines\CommandLines.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ConsoleDevice.STDIO

Imports File = System.String
Imports System.Text
Imports Microsoft.VisualBasic.Scripting.MetaData

<[PackageNamespace]("C2.CLI", Category:=APICategories.CLI_MAN, Description:="C2 program CLI Entry.")>
Public Module CommandLines

    <ExportAPI("set", Info:="setup the profile data", Usage:="set <item> <value>", Example:="set blastbin ""/home/xieguigang/blast/bin/""")>
    Public Function [Set](CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim Variable As String = CommandLine.Parameters.First.ToLower
        Dim value As String = CommandLine.Parameters(1)

        Dim Settings = c2.Settings.ProfileData

        If Settings.ExistsNode(Variable) Then
            Call Settings.Set(Variable, value)
            Call Settings.Save()

            Return 0
        Else
            Call Printf("Could not find a profile item named ""%s""!", Variable)
            Return -1
        End If
    End Function

    <ExportAPI("var", Info:="Get the information about the setting variables in the c2 program",
        Usage:="var [<var_name>]",
        Example:="var blastbin")>
    Public Function Variables(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        If CommandLine.Parameters.Count = 0 Then
            Return ListAllVariables()
        Else
            Return Var(CommandLine.Parameters.First)
        End If
    End Function

    Private Function Var(name As String) As Integer
        Dim Vars As Dictionary(Of String, String) = New Dictionary(Of String, String) From {
            {"blastbin", "setup the file path of the blast program 'blastall', example value as '/home/xieguigang/blast/bin/blastall.bin'"},
            {"formatdb", "setup the file path of the blast db formatter program 'formatdb', example value as '/home/xieguigang/blast/bin/formatdb.bin'"},
            {"blastdb", "setup the directory path of the default blast program database, example value as '/home/xieguigang/blast/db'"},
            {"blast-e", "setup the default e-value for the parameter of the blast operation, default value is '1e-2'"}}

        name = name.ToLower

        If Vars.ContainsKey(name) Then
            Printf("Information about the setting variable '%s':\n> %s", name, Vars(name))
        Else
            Printf("No such a variable named '%s'", name)
            Return -1
        End If

        Return 0
    End Function

    Private Function ListAllVariables() As Integer
        Dim sBuilder As StringBuilder = New StringBuilder(512)

        sBuilder.AppendLine("All of the setting variables were list below:" & vbCrLf)

        sBuilder.AppendLine(String.Format("   'blastbin' = '{0}'", Settings.GetSettings("blastbin")))
        '   sBuilder.AppendLine(String.Format("   'formatdb' = '{0}'", Profile.LocalBlast.Bin & "/formatdb [Executable Assembly]"))
        sBuilder.AppendLine(String.Format("   'blastdb'  = '{0}'", Settings.GetSettings("blastdb")))
        sBuilder.AppendLine()
        sBuilder.AppendLine("While you can use command 'var <var_name>' to get more detail information about the setting variable.")

        Console.WriteLine(sBuilder.ToString)
        Return 0
    End Function

    ''' <summary>
    ''' Build the fasta database from a genbank flat file.(使用一个NCBI GenBank数据库文件创建一个FASTA蛋白质序列数据库)
    ''' </summary>
    ''' <param name="CommandLine"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("build", Info:="build the fasta database from a genbank flat file.",
        Usage:="build -i <inputfile> [-o <outputfile> -f <gbk/fsa>]",
        Example:="build -i /home/xieguigang/ncbi/xcc8004.gbk -f gbk")>
    <CommandLine.Reflection.ParameterInfo("-i",
        Description:="The input data source file that will be use for build up a fasta sequence database.",
        Example:="/home/xieguigang/ncbi/xcc8004.gbk")>
    <CommandLine.Reflection.ParameterInfo("-f", True,
        Description:="Optional, The input data source file format, it can be a fasta sequence file or a ncbi genbank flat file,default format is FASTA format" & vbCrLf &
                     " gbk - The input file format is ncbi genbank gbff format;" & vbCrLf &
                     " fsa - The input file format is a fasta sequence data file.",
        Example:="gbk")>
    Public Function BuildDb(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Printf("[c2.exe, version %s]\n  c2 compiler, the genetic clock model alternative compiler\n", My.Application.Info.Version.ToString)

        Dim Input As String = CommandLine("-i")

        If String.IsNullOrEmpty(Input) Then
            Dim Msg As String = "Argument error: can not found any data source file to compile!"
            Printf(Msg)
            Using LogFile As Microsoft.VisualBasic.Logging.LogFile =
                         New Microsoft.VisualBasic.Logging.LogFile(Program.Logs)
                LogFile.WriteLine(Msg, "LocalBLAST", Microsoft.VisualBasic.Logging.MSG_TYPES.ERR)
            End Using
            Return -1
        ElseIf Not FileIO.FileSystem.FileExists(Input) Then
            Printf("FileIO Exception: could not found the specific file!\nFile \'%s\' is not exists!", Input)
            Return -2
        Else
            Dim f As String = CommandLine("-f")

            Printf("  Variable 'blastbin' = \'%s\'", Settings.GetSettings("blastbin"))
            '   Printf("  Variable 'formatdb' = \'%s\'", Profile.LocalBlast.Bin & "/formatdb [Executable Assembly]")
            Printf("  Variable 'blastdb'  = \'%s\'", Settings.GetSettings("blastdb"))

            If String.Equals(f, "gbk") Then
                '    Call LocalBLAST2.LocalBlast.FormatGBK(Input, CommandLine("-o"), LocalBLAST2.LocalBlast.FASTATypes.Protein)
            Else 'FASTA File
                '   Call LocalBLAST2.LocalBlast.BuildFASTA(Input, False)
            End If

            Return 0
        End If
    End Function
End Module
