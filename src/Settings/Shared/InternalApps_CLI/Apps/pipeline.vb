Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\pipeline.exe

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
'  < pipeline.Program >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
' API list that with functional grouping
' 
' 1. Pipeline Resource Controller
' 
' 
'    /dispose:      Delete an exists memory mapping file resource.
'    /register:     Allocate a new memory mapping file resource for save the temp data for cli pipeline
'                   scripting
' 
' 
' 2. Pipeline Services Controller
' 
' 
'    /start:        Start the IPC pipeline host services
'    /stop:         Send a stop signal to the IPC host to shutdown the running services instance.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' pipeline.Program
''' </summary>
'''
Public Class pipeline : Inherits InteropService

    Public Const App$ = "pipeline.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As pipeline
          Return New pipeline(App:=directory & "/" & pipeline.App)
     End Function

''' <summary>
''' ```bash
''' /dispose /resource &lt;resource_name&gt;
''' ```
''' Delete an exists memory mapping file resource.
''' </summary>
'''

Public Function Dispose(resource As String) As Integer
    Dim CLI As New StringBuilder("/dispose")
    Call CLI.Append(" ")
    Call CLI.Append("/resource " & """" & resource & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /register /resource &lt;resource_name&gt; /size &lt;size_in_bytes&gt; /type &lt;meta_base64&gt;
''' ```
''' Allocate a new memory mapping file resource for save the temp data for cli pipeline scripting
''' </summary>
'''

Public Function Register(resource As String, size As String, type As String) As Integer
    Dim CLI As New StringBuilder("/register")
    Call CLI.Append(" ")
    Call CLI.Append("/resource " & """" & resource & """ ")
    Call CLI.Append("/size " & """" & size & """ ")
    Call CLI.Append("/type " & """" & type & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /start [/port &lt;default=8833&gt;]
''' ```
''' Start the IPC pipeline host services
''' </summary>
'''

Public Function Start(Optional port As String = "8833") As Integer
    Dim CLI As New StringBuilder("/start")
    Call CLI.Append(" ")
    If Not port.StringEmpty Then
            Call CLI.Append("/port " & """" & port & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /stop [/port &lt;default=8833&gt;]
''' ```
''' Send a stop signal to the IPC host to shutdown the running services instance.
''' </summary>
'''

Public Function [Stop](Optional port As String = "8833") As Integer
    Dim CLI As New StringBuilder("/stop")
    Call CLI.Append(" ")
    If Not port.StringEmpty Then
            Call CLI.Append("/port " & """" & port & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
