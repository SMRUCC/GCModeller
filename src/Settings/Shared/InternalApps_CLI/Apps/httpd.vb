#Region "Microsoft.VisualBasic::392a6f85a3b7b04b9a85951c723544cc, Shared\InternalApps_CLI\Apps\httpd.vb"

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

    ' Class httpd
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
' assembly: ..\bin\httpd.exe

' ====================================================
' SMRUCC genomics GCModeller Programs Profiles Manager
' ====================================================
' 
' < SMRUCC.WebCloud.httpd.CLI >
' 
' All of the command that available in this program has been list below:
' 
'  /GET:                Tools for http get request the content of a specific url.
'  /run.dll:            
'  /Stress.Testing:     Using Ctrl + C to stop the stress testing.
' 
' 
' API list that with functional grouping
' 
' 1. httpdServerCLI
' 
'    Server CLI for running this httpd web server.
' 
' 
'    /run:                Run start the web server with specific Web App.
'    /start:              Run start the httpd web server.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    You can using "Settings ??<commandName>" for getting more details command help.

Namespace GCModellerApps


''' <summary>
''' SMRUCC.WebCloud.httpd.CLI
''' </summary>
'''
Public Class httpd : Inherits InteropService

    Public Const App$ = "httpd.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

''' <summary>
''' ```
''' /GET /url &lt;url, /std_in> [/out &lt;file/std_out>]
''' ```
''' Tools for http get request the content of a specific url.
''' </summary>
'''
Public Function [GET](url As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/GET")
    Call CLI.Append(" ")
    Call CLI.Append("/url " & """" & url & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /run /dll &lt;app.dll> [/port &lt;default=80> /wwwroot &lt;wwwroot_DIR>]
''' ```
''' Run start the web server with specific Web App.
''' </summary>
'''
Public Function RunApp(dll As String, Optional port As String = "80", Optional wwwroot As String = "") As Integer
    Dim CLI As New StringBuilder("/run")
    Call CLI.Append(" ")
    Call CLI.Append("/dll " & """" & dll & """ ")
    If Not port.StringEmpty Then
            Call CLI.Append("/port " & """" & port & """ ")
    End If
    If Not wwwroot.StringEmpty Then
            Call CLI.Append("/wwwroot " & """" & wwwroot & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /run.dll /api &lt;namespace::apiName> [....]
''' ```
''' </summary>
'''
Public Function RunDll(api As String, Optional ____ As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/run.dll")
    Call CLI.Append(" ")
    Call CLI.Append("/api " & """" & api & """ ")
    If ____ Then
        Call CLI.Append(".... ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /start [/port 80 /wwwroot &lt;wwwroot_DIR> /threads &lt;default=-1> /cache]
''' ```
''' Run start the httpd web server.
''' </summary>
'''
Public Function Start(Optional port As String = "", Optional wwwroot As String = "", Optional threads As String = "-1", Optional cache As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/start")
    Call CLI.Append(" ")
    If Not port.StringEmpty Then
            Call CLI.Append("/port " & """" & port & """ ")
    End If
    If Not wwwroot.StringEmpty Then
            Call CLI.Append("/wwwroot " & """" & wwwroot & """ ")
    End If
    If Not threads.StringEmpty Then
            Call CLI.Append("/threads " & """" & threads & """ ")
    End If
    If cache Then
        Call CLI.Append("/cache ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Stress.Testing /url &lt;target_url> [/out &lt;out.txt>]
''' ```
''' Using Ctrl + C to stop the stress testing.
''' </summary>
'''
Public Function StressTest(url As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Stress.Testing")
    Call CLI.Append(" ")
    Call CLI.Append("/url " & """" & url & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
