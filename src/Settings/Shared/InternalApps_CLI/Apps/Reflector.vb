Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: D:/GCModeller/GCModeller/bin/Reflector.exe

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
''' --reflects /sql &lt;sql_path/std_in> [-o &lt;output_path> /namespace &lt;namespace> --language &lt;php/visualbasic, default=visualbasic> /split /auto_increment.disable]
''' ```
''' Automatically generates visualbasic source code from the MySQL database schema dump.
''' </summary>
'''
Public Function ReflectsConvert(sql As String, Optional o As String = "", Optional [namespace] As String = "", Optional language As String = "visualbasic", Optional split As Boolean = False, Optional auto_increment_disable As Boolean = False) As Integer
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
    If auto_increment_disable Then
        Call CLI.Append("/auto_increment.disable ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
