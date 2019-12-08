Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\R#.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   3.3277.7281.33964
'  // ASSEMBLY:  Settings, Version=3.3277.7281.33964, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     12/7/2019 6:27:36 AM
'  // 
' 
' 
'  < Rterm.CLI >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /compile:               
'  --install.packages:     Install new packages.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Rterm.CLI
''' </summary>
'''
Public Class R_ : Inherits InteropService

    Public Const App$ = "R#.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As R_
          Return New R_(App:=directory & "/" & R_.App)
     End Function

''' <summary>
''' ```bash
''' /compile --script &lt;script.R&gt; [--out &lt;app.exec&gt;]
''' ```
''' </summary>
'''

Public Function Compile(script As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/compile")
    Call CLI.Append(" ")
    Call CLI.Append("--script " & """" & script & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("--out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' --install.packages /module &lt;*.dll&gt; [--verbose]
''' ```
''' Install new packages.
''' </summary>
'''

Public Function Install([module] As String, Optional verbose As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--install.packages")
    Call CLI.Append(" ")
    Call CLI.Append("/module " & """" & [module] & """ ")
    If verbose Then
        Call CLI.Append("--verbose ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
