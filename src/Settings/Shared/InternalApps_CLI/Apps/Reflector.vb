Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\Reflector.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   1.0.0.*
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // 
' 
' 
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /union:              Union all of the sql file in the target directory into a one big sql text file.
' 
' 
' API list that with functional grouping
' 
' 1. MySQL documentation tool
' 
' 
'    /MySQL.Markdown:     Generates the SDK document of your mysql database.
' 
' 
' 2. MySQL ORM code solution tool
' 
' 
'    --export.dump:       Scans for the table schema sql files in a directory and converts these sql file
'                         as visualbasic source code.
'    --reflects:          Automatically generates visualbasic source code from the MySQL database schema
'                         dump.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    You can using "Settings ??<commandName>" for getting more details command help.

Namespace GCModellerApps


''' <summary>
'''
''' </summary>
'''
Public Class Reflector : Inherits InteropService

    Public Const App$ = "Reflector.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

''' <summary>
''' ```
''' /MySQL.Markdown /sql &lt;database.sql/std_in> [/toc /out &lt;out.md/std_out>]
''' ```
''' Generates the SDK document of your mysql database.
''' </summary>
'''
Public Function MySQLMarkdown(sql As String, Optional out As String = "", Optional toc As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/MySQL.Markdown")
    Call CLI.Append(" ")
    Call CLI.Append("/sql " & """" & sql & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If toc Then
        Call CLI.Append("/toc ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /union /in &lt;directory> [/out &lt;out.sql>]
''' ```
''' Union all of the sql file in the target directory into a one big sql text file.
''' </summary>
'''
Public Function Union([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/union")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --export.dump [-o &lt;out_dir> /namespace &lt;namespace> --dir &lt;source_dir>]
''' ```
''' Scans for the table schema sql files in a directory and converts these sql file as visualbasic source code.
''' </summary>
'''
Public Function ExportDumpDir(Optional o As String = "", Optional [namespace] As String = "", Optional dir As String = "") As Integer
    Dim CLI As New StringBuilder("--export.dump")
    Call CLI.Append(" ")
    If Not o.StringEmpty Then
            Call CLI.Append("-o " & """" & o & """ ")
    End If
    If Not [namespace].StringEmpty Then
            Call CLI.Append("/namespace " & """" & [namespace] & """ ")
    End If
    If Not dir.StringEmpty Then
            Call CLI.Append("--dir " & """" & dir & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' --reflects /sql &lt;sql_path/std_in> [-o &lt;output_path> /namespace &lt;namespace> --language &lt;php/visualbasic, default=visualbasic> /split]
''' ```
''' Automatically generates visualbasic source code from the MySQL database schema dump.
''' </summary>
'''
Public Function ReflectsConvert(sql As String, Optional o As String = "", Optional [namespace] As String = "", Optional language As String = "visualbasic", Optional split As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--reflects")
    Call CLI.Append(" ")
    Call CLI.Append("/sql " & """" & sql & """ ")
    If Not o.StringEmpty Then
            Call CLI.Append("-o " & """" & o & """ ")
    End If
    If Not [namespace].StringEmpty Then
            Call CLI.Append("/namespace " & """" & [namespace] & """ ")
    End If
    If Not language.StringEmpty Then
            Call CLI.Append("--language " & """" & language & """ ")
    End If
    If split Then
        Call CLI.Append("/split ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
