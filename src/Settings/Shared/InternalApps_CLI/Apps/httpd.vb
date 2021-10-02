#Region "Microsoft.VisualBasic::ee6dac1582fae9feb7e0ac7f906d5394, Shared\InternalApps_CLI\Apps\httpd.vb"

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
    '     Constructor: (+1 Overloads) Sub New
    '     Function: FromEnvironment
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\httpd.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   3.3277.7609.23646
'  // ASSEMBLY:  Settings, Version=3.3277.7609.23646, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/31/2020 1:08:12 PM
'  // 
' 
' 
'  < SMRUCC.WebCloud.httpd.CLI >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /GET:                Tools for http get request the content of a specific url.
'  /POST:               
'  /run.dll:            
'  /socket:             Start a new websocket server.
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
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

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
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As httpd
          Return New httpd(App:=directory & "/" & httpd.App)
     End Function

''' <summary>
''' ```bash
''' /GET /url &lt;url, /std_in&gt; [/out &lt;file/std_out&gt;]
''' ```
''' Tools for http get request the content of a specific url.
''' </summary>
'''
''' <param name="url"> The resource URL on the web.
''' </param>
''' <param name="out"> The save location of your requested data file.
''' </param>
Public Function [GET](url As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/GET")
    Call CLI.Append(" ")
    Call CLI.Append("/url " & """" & url & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /POST /url &lt;url, /std_in&gt; [[/args1 value1 /args2 value2, ...] /out &lt;file/std_out&gt;]
''' ```
''' </summary>
'''

Public Function POST(url As String, Optional __args1 As String = "", Optional args2 As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/POST")
    Call CLI.Append(" ")
    Call CLI.Append("/url " & """" & url & """ ")
    If Not __args1.StringEmpty Then
            Call CLI.Append("[/args1 " & """" & __args1 & """ ")
    End If
    If Not args2.StringEmpty Then
            Call CLI.Append("/args2 " & """" & args2 & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /run /dll &lt;app.dll&gt; [/port &lt;default=80&gt; /wwwroot &lt;wwwroot_DIR&gt;]
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
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /run.dll /api &lt;namespace::apiName&gt; [....]
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
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /socket /app &lt;appName&gt; [/hostName &lt;default=127.0.0.1&gt; /port &lt;default=81&gt;]
''' ```
''' Start a new websocket server.
''' </summary>
'''

Public Function RunSocket(app As String, Optional hostname As String = "127.0.0.1", Optional port As String = "81") As Integer
    Dim CLI As New StringBuilder("/socket")
    Call CLI.Append(" ")
    Call CLI.Append("/app " & """" & app & """ ")
    If Not hostname.StringEmpty Then
            Call CLI.Append("/hostname " & """" & hostname & """ ")
    End If
    If Not port.StringEmpty Then
            Call CLI.Append("/port " & """" & port & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /start [/port 80 /wwwroot &lt;wwwroot_DIR&gt; /threads &lt;default=-1&gt; /cache]
''' ```
''' Run start the httpd web server.
''' </summary>
'''
''' <param name="port"> The server port of this httpd web server to listen.
''' </param>
''' <param name="wwwroot"> The website html root directory path.
''' </param>
''' <param name="threads"> The number of threads of this web server its thread pool.
''' </param>
''' <param name="cache"> Is this server running in file system cache mode? Not recommended for open.
''' </param>
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
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Stress.Testing /url &lt;target_url&gt; [/out &lt;out.txt&gt;]
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
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace

