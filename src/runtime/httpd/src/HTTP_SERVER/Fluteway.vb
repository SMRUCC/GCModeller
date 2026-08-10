#Region "Microsoft.VisualBasic::ab7c22dcc6268006b3ee699f572b637a, src\HTTP_SERVER\Fluteway.vb"

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


    ' Code Statistics:

    '   Total Lines: 100
    '    Code Lines: 40 (40.00%)
    ' Comment Lines: 47 (47.00%)
    '    - Xml Docs: 29.79%
    ' 
    '   Blank Lines: 13 (13.00%)
    '     File Size: 3.60 KB


    ' Class Fluteway
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
' assembly: ..\net10.0\Fluteway.dll

' 
'  // 
'  // 
'  // 
'  // VERSION:   1.0.0.0
'  // ASSEMBLY:  Fluteway, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: 
'  // GUID:      
'  // BUILT:     1/1/2000 12:00:00 AM
'  // 
' 
' 
'  < Fluteway.Program >
' 
' 
' SYNOPSIS
' Fluteway command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  --listen:     Start a local static web server for hosting statics web page files
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Fluteway ??<commandName>" for getting more details command help.
'    2. Using command "Fluteway /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Fluteway /i" for enter interactive console mode.
'    4. Using command "Fluteway /STACK:xxMB" for adjust the application stack size, example as '/STACK:64MB'.

Namespace CLI


''' <summary>
''' Fluteway.Program
''' </summary>
'''
Public Class Fluteway : Inherits InteropService

    Public Const App$ = "Fluteway.exe"

    Sub New(App$)
        Call MyBase.New(app:=App$)
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Fluteway
          Return New Fluteway(App:=directory & "/" & Fluteway.App)
     End Function

''' <summary>
''' ```bash
''' --listen [/wwwroot &lt;directory_path&gt; --attach &lt;other_directory_path/streampack&gt; --parent &lt;parent_process_id&gt; /port &lt;http_port, default=80&gt;]
''' ```
''' Start a local static web server for hosting statics web page files
''' </summary>
'''

Public Function listen(Optional wwwroot As String = "", Optional attach As String = "", Optional parent As String = "", Optional port As String = "80") As Integer
Dim cli = GetlistenCommandLine(wwwroot:=wwwroot, attach:=attach, parent:=parent, port:=port, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetlistenCommandLine(Optional wwwroot As String = "", Optional attach As String = "", Optional parent As String = "", Optional port As String = "80", Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("--listen")
    Call CLI.Append(" ")
    If Not wwwroot.StringEmpty Then
            Call CLI.Append("/wwwroot " & """" & wwwroot & """ ")
    End If
    If Not attach.StringEmpty Then
            Call CLI.Append("--attach " & """" & attach & """ ")
    End If
    If Not parent.StringEmpty Then
            Call CLI.Append("--parent " & """" & parent & """ ")
    End If
    If Not port.StringEmpty Then
            Call CLI.Append("/port " & """" & port & """ ")
    End If
     Call CLI.Append($"/@set internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function
End Class
End Namespace


