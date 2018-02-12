#Region "Microsoft.VisualBasic::8d300a06218ca030368c33dc347b07aa, Shared\InternalApps_CLI\Apps\Excel.vb"

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

    ' Class Excel
    ' 
    '     Sub: New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\Excel.exe

' ====================================================
' SMRUCC genomics GCModeller Programs Profiles Manager
' ====================================================
' 
' < Excel_CLI.CLI >
' 
' All of the command that available in this program has been list below:
' 
'  /Cbind:       Join of two table by a unique ID.
'  /Create:      Create an empty Excel xlsx package file on a specific file path
'  /Extract:     Open target excel file and get target table and save into a csv file.
'  /Print:       Print the csv/xlsx file content onto the console screen or text file in table layout.
'  /push:        Write target csv table its content data as a worksheet into the target Excel package.
'  /rbind:       Row bind(merge tables directly) of the csv tables
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    You can using "Settings ??<commandName>" for getting more details command help.

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
''' /Print /in &lt;table.csv/xlsx> [/sheet &lt;sheetName> /out &lt;device/txt>]
''' ```
''' Print the csv/xlsx file content onto the console screen or text file in table layout.
''' </summary>
'''
Public Function Print([in] As String, Optional sheet As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Print")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not sheet.StringEmpty Then
            Call CLI.Append("/sheet " & """" & sheet & """ ")
    End If
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
