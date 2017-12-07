Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: D:/GCModeller/GCModeller/bin/Excel.exe

Namespace GCModellerApps


''' <summary>
''' Excel_CLI.CLI
''' </summary>
'''
Public Class Excel : Inherits InteropService

Public Const App$ = "Excel.exe"

Sub New(App$)
MyBase._executableAssembly = App$
End Sub

''' <summary>
''' ```
''' /cbind /in &lt;a.csv> /append &lt;b.csv> [/token0.ID &lt;deli, default=&lt;SPACE> /out &lt;ALL.csv>]
''' ```
''' Join of two table by a unique ID.
''' </summary>
'''
Public Function cbind([in] As String, append As String, Optional token0_id As String = "<SPACE", Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/cbind")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
Call CLI.Append("/append " & """" & append & """ ")
If Not token0_id.StringEmpty Then
Call CLI.Append("/token0.id " & """" & token0_id & """ ")
End If
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Create /target &lt;xlsx>
''' ```
''' Create an empty Excel xlsx package file on a specific file path
''' </summary>
'''
Public Function newEmpty(target As String) As Integer
Dim CLI As New StringBuilder("/Create")
Call CLI.Append(" ")
Call CLI.Append("/target " & """" & target & """ ")


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /Extract /open &lt;xlsx> /sheetName &lt;name_string> [/out &lt;out.csv>]
''' ```
''' Open target excel file and get target table and save into a csv file.
''' </summary>
'''
Public Function Extract(open As String, sheetName As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/Extract")
Call CLI.Append(" ")
Call CLI.Append("/open " & """" & open & """ ")
Call CLI.Append("/sheetName " & """" & sheetName & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /push /write &lt;*.xlsx> /table &lt;*.csv> [/sheetName &lt;name_string> /saveAs &lt;*.xlsx>]
''' ```
''' Write target csv table its content data as a worksheet into the target Excel package.
''' </summary>
'''
Public Function PushTable(write As String, table As String, Optional sheetname As String = "", Optional saveas As String = "") As Integer
Dim CLI As New StringBuilder("/push")
Call CLI.Append(" ")
Call CLI.Append("/write " & """" & write & """ ")
Call CLI.Append("/table " & """" & table & """ ")
If Not sheetname.StringEmpty Then
Call CLI.Append("/sheetname " & """" & sheetname & """ ")
End If
If Not saveas.StringEmpty Then
Call CLI.Append("/saveas " & """" & saveas & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function

''' <summary>
''' ```
''' /rbind /in &lt;*.csv.DIR> [/out &lt;EXPORT.csv>]
''' ```
''' Row bind(merge tables directly) of the csv tables
''' </summary>
'''
Public Function rbind([in] As String, Optional out As String = "") As Integer
Dim CLI As New StringBuilder("/rbind")
Call CLI.Append(" ")
Call CLI.Append("/in " & """" & [in] & """ ")
If Not out.StringEmpty Then
Call CLI.Append("/out " & """" & out & """ ")
End If


Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
Return proc.Run()
End Function
End Class
End Namespace
