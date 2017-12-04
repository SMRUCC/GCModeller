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
Public Function MySQLMarkdown(_sql As String, Optional _out As String = "", Optional _toc As Boolean = False) As Integer
Dim CLI As New StringBuilder("/MySQL.Markdown")
Call CLI.Append(" ")
Call CLI.Append("/sql " & """" & _sql & """ ")
If Not _out.StringEmpty Then
Call CLI.Append("/out " & """" & _out & """ ")
End If
If _toc Then
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
Public Function ExportDumpDir(Optional _o As String = "", Optional _namespace As String = "", Optional __dir As String = "") As Integer
Dim CLI As New StringBuilder("--export.dump")
Call CLI.Append(" ")
If Not _o.StringEmpty Then
Call CLI.Append("-o " & """" & _o & """ ")
End If
If Not _namespace.StringEmpty Then
Call CLI.Append("/namespace " & """" & _namespace & """ ")
End If
If Not __dir.StringEmpty Then
Call CLI.Append("--dir " & """" & __dir & """ ")
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
Public Function ReflectsConvert(_sql As String, Optional _o As String = "", Optional _namespace As String = "", Optional __language As String = "visualbasic", Optional _split As Boolean = False, Optional _auto_increment_disable As Boolean = False) As Integer
Dim CLI As New StringBuilder("--reflects")
Call CLI.Append(" ")
Call CLI.Append("/sql " & """" & _sql & """ ")
If Not _o.StringEmpty Then
Call CLI.Append("-o " & """" & _o & """ ")
End If
If Not _namespace.StringEmpty Then
Call CLI.Append("/namespace " & """" & _namespace & """ ")
End If
If Not __language.StringEmpty Then
Call CLI.Append("--language " & """" & __language & """ ")
End If
If _split Then
Call CLI.Append("/split ")
End If
If _auto_increment_disable Then
Call CLI.Append("/auto_increment.disable ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
