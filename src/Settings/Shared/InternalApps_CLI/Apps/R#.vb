#Region "Microsoft.VisualBasic::05fc2bc265ce00267722ecd1b397245d, Shared\InternalApps_CLI\Apps\R#.vb"

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

    ' Class R_
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
' assembly: ..\bin\R#.exe

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
'  < Rterm.CLI >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /bash:                  
'  --info:                 Print R# interpreter version information and R# terminal version information.
'  --man.1:                Exports unix man page data for current installed packages.
'  --syntax:               Show syntax parser result of the input script.
'  --version:              Print R# interpreter version
' 
' 
' API list that with functional grouping
' 
' 1. R# System Utils
' 
'    R# language system and environment configuration util tools.
' 
' 
'    --install.packages:     Install new packages.
'    --setup:                Initialize the R# runtime environment.
'    --startups:             
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
''' /bash --script &lt;run.R&gt;
''' ```
''' </summary>
'''

Public Function BashRun(script As String) As Integer
    Dim CLI As New StringBuilder("/bash")
    Call CLI.Append(" ")
    Call CLI.Append("--script " & """" & script & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' 
''' ```
''' Print R# interpreter version information and R# terminal version information.
''' </summary>
'''

Public Function Info() As Integer
    Dim CLI As New StringBuilder("--info")
    Call CLI.Append(" ")
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
''' <param name="[module]"> .NET Framework 4.8 assembly module file.
''' </param>
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

''' <summary>
''' ```bash
''' --man.1 [--module &lt;module.dll&gt; --debug --out &lt;directory, default=./&gt;]
''' ```
''' Exports unix man page data for current installed packages.
''' </summary>
'''

Public Function unixman(Optional [module] As String = "", Optional out As String = "./", Optional debug As Boolean = False) As Integer
    Dim CLI As New StringBuilder("--man.1")
    Call CLI.Append(" ")
    If Not [module].StringEmpty Then
            Call CLI.Append("--module " & """" & [module] & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("--out " & """" & out & """ ")
    End If
    If debug Then
        Call CLI.Append("--debug ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' 
''' ```
''' Initialize the R# runtime environment.
''' </summary>
'''

Public Function InitializeEnvironment() As Integer
    Dim CLI As New StringBuilder("--setup")
    Call CLI.Append(" ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' --startups [--add &lt;namespaceList&gt; --remove &lt;namespaceList&gt;]
''' ```
''' </summary>
'''

Public Function ConfigStartups(Optional add As String = "", Optional remove As String = "") As Integer
    Dim CLI As New StringBuilder("--startups")
    Call CLI.Append(" ")
    If Not add.StringEmpty Then
            Call CLI.Append("--add " & """" & add & """ ")
    End If
    If Not remove.StringEmpty Then
            Call CLI.Append("--remove " & """" & remove & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' --syntax /script &lt;script.R&gt;
''' ```
''' Show syntax parser result of the input script.
''' </summary>
'''

Public Function SyntaxText(script As String) As Integer
    Dim CLI As New StringBuilder("--syntax")
    Call CLI.Append(" ")
    Call CLI.Append("/script " & """" & script & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' 
''' ```
''' Print R# interpreter version
''' </summary>
'''

Public Function Version() As Integer
    Dim CLI As New StringBuilder("--version")
    Call CLI.Append(" ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace

