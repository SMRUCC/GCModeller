#Region "Microsoft.VisualBasic::2d2ade33835b1ad8557149c6b779a04c, cli_tool\CLI.vb"

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
    '     Function: [Set], CLIDevelopment, SetMySQL, ViewVar
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.ManView
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports MySql = Oracle.LinuxCompatibility.MySQL.MySqli

''' <summary>
''' GCModeller config tool
''' </summary>
<Package("GCModeller.Configuration.CLI",
        Category:=APICategories.CLI_MAN,
        Url:="http://gcmodeller.org",
        Description:="GCModeller configuration console.",
        Publisher:="xie.guigang@gcmodeller.org",
        Revision:=215)>
<ExceptionHelp(Documentation:="http://docs.gcmodeller.org", Debugging:="https://github.com/SMRUCC/GCModeller/wiki", EMailLink:="xie.guigang@gcmodeller.org")>
<CLI> Public Module CLI

    Const Config_CLI$ = "GCModeller configuration CLI tool"
    Const Dev_CLI$ = "GCModeller development helper CLI"

    Sub New()
        Settings.Session.Initialize()
    End Sub

    <ExportAPI("Set", Info:="Setting up the configuration data node.",
               Usage:="Set <varName> <value>",
               Example:="Set java /usr/lib/java/java.bin")>
    <Argument("<varName>", False, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="The variable name in the GCModeller configuration file.")>
    <Group(CLI.Config_CLI)>
    Public Function [Set](args As CommandLine) As Integer
        Using Settings = Global.GCModeller.Configuration.Settings.Session.ProfileData
            Dim params As String() = args.Parameters
            Dim x As String = params(0)
            Dim Value As String = params(1)

            Call Settings.Set(x, Value)

            Return 0
        End Using
    End Function

    <ExportAPI("var", Info:="Gets the settings value.",
               Usage:="var [varName] [/value]",
               Example:="")>
    <Argument("[VarName]", True, CLITypes.String,
              Description:="If this value is null, then the program will prints all of the variables in the gcmodeller config file or when the variable is presents in the database, only the config value of the specific variable will be display.")>
    <Argument("/value", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this argument is presented, then this settings program will only output the variable value, otherwise will output data in format: key = value")>
    <Group(CLI.Config_CLI)>
    Public Function ViewVar(args As CommandLine) As Integer
        Using Settings = Global.GCModeller.Configuration.Settings.Session.ProfileData

            ' list all setting items
            If args.Parameters.Length = 0 Then
                Call Console.WriteLine(Settings.View)
                Call Console.WriteLine()
                Call Console.Write($"  Read from {Settings.FilePath.GetFullPath}")
            Else
                Dim x As String = args.Parameters.First
                Dim value = Settings.View(x)

                If Not args.Parameters _
                    .Where(Function(s) s.TextEquals("/value")) _
                    .FirstOrDefault _
                    .StringEmpty Then

                    With value.GetTagValue("=") _
                        .Value _
                        .Trim(ASCII.Quot, " "c)

                        Call Console.Write(.ByRef)
                    End With
                Else
                    Call Console.WriteLine()
                    Call Console.Write("   " & value)
                End If
            End If

            Return 0
        End Using
    End Function

    <ExportAPI("/set.mysql")>
    <Description("Setting up the mysql connection parameters")>
    <Usage("/set.mysql /test")>
    <Argument("/test", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this boolean argument is set, then the program will testing for the mysqli connection before write the configuration file. If the connection test failure, then the configuration file will not be updated!")>
    <Group(CLI.Config_CLI)>
    Public Function SetMySQL(args As CommandLine) As Integer
        Using Settings = Global.GCModeller.Configuration.Settings.Session.ProfileData
            Call MySqliHelper.RunConfig(
                Sub(uri)
                    MySQLExtensions.MySQL = uri
                End Sub)

            If args.IsTrue("/test") Then
                Dim mysqli As MySql = MySQLExtensions.GetMySQLClient(, DBName:=Nothing)
            End If

            Return 0
        End Using
    End Function

    Const GCModellerApps$ = NameOf(GCModellerApps)

    <ExportAPI("/dev")>
    <Description("Generates Apps CLI visualbasic reference source code.")>
    <Usage("/dev [/out <DIR>]")>
    <Argument("/out", True, CLITypes.File,
              AcceptTypes:={GetType(String)},
              Description:="The generated VisualBasic source file output directory location.")>
    <Group(CLI.Dev_CLI)>
    Public Function CLIDevelopment(args As CommandLine) As Integer
        Dim out$ = args("/out") Or "./Apps/"
        Dim CLI As New Value(Of Type)

        For Each file$ In ls - l - "*.exe" <= App.HOME
            If IO.Path.GetFullPath(file) = IO.Path.GetFullPath(App.ExecutablePath) Then
                Continue For
            End If

            If Not (CLI = CLIAttribute.ParseAssembly(dll:=file)) Is Nothing Then
                Call New VisualBasic(CLI, GCModellerApps) _
                    .GetSourceCode _
                    .SaveTo($"{out}/{file.BaseName}.vb")
                Call file.__INFO_ECHO
            End If
        Next

        Return 0
    End Function
End Module
