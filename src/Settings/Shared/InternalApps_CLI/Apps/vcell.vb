Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\vcell.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   3.3277.7268.38152
'  // ASSEMBLY:  Settings, Version=3.3277.7268.38152, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     11/24/2019 8:47:12 AM
'  // 
' 
' 
'  < vcell.CLI >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /run:       Run GCModeller VirtualCell.
'  /union:     
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' vcell.CLI
''' </summary>
'''
Public Class vcell : Inherits InteropService

    Public Const App$ = "vcell.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As vcell
          Return New vcell(App:=directory & "/" & vcell.App)
     End Function

''' <summary>
''' ```bash
''' /run /model &lt;model.gcmarkup&gt; [/deletes &lt;genelist&gt; /iterations &lt;default=5000&gt; /csv /out &lt;raw/result_directory&gt;]
''' ```
''' Run GCModeller VirtualCell.
''' </summary>
'''
Public Function Run(model As String, Optional deletes As String = "", Optional iterations As String = "5000", Optional out As String = "", Optional csv As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/run")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    If Not deletes.StringEmpty Then
            Call CLI.Append("/deletes " & """" & deletes & """ ")
    End If
    If Not iterations.StringEmpty Then
            Call CLI.Append("/iterations " & """" & iterations & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If csv Then
        Call CLI.Append("/csv ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /union /raw &lt;*.csv data_directory&gt; /time &lt;timepoint&gt; [/out &lt;union_matrix.csv&gt;]
''' ```
''' </summary>
'''
Public Function UnionCompareMatrix(raw As String, time As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/union")
    Call CLI.Append(" ")
    Call CLI.Append("/raw " & """" & raw & """ ")
    Call CLI.Append("/time " & """" & time & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
