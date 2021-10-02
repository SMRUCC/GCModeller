#Region "Microsoft.VisualBasic::21632fbd239661bd0f7f26101caa2343, Shared\InternalApps_CLI\Apps\vcell.vb"

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

    ' Class vcell
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
' assembly: ..\bin\vcell.exe

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
'  < vcell.CLI >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /diff:      Different expression of ``exp vs normal``.
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
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As vcell
          Return New vcell(App:=directory & "/" & vcell.App)
     End Function

''' <summary>
''' ```bash
''' /diff /normal &lt;result.json&gt; /exp &lt;experiment.json&gt; [/result &lt;output_folder&gt;]
''' ```
''' Different expression of ``exp vs normal``.
''' </summary>
'''

Public Function DiffExpression(normal As String, exp As String, Optional result As String = "") As Integer
    Dim CLI As New StringBuilder("/diff")
    Call CLI.Append(" ")
    Call CLI.Append("/normal " & """" & normal & """ ")
    Call CLI.Append("/exp " & """" & exp & """ ")
    If Not result.StringEmpty Then
            Call CLI.Append("/result " & """" & result & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /run /model &lt;model.gcmarkup&gt; [/deletes &lt;genelist&gt; /time &lt;default=100&gt; /json /out &lt;raw/result_directory&gt;]
''' ```
''' Run GCModeller VirtualCell.
''' </summary>
'''
''' <param name="deletes"> The ``locus_tag`` id list that will removes from the genome, 
'''               use the comma symbol as delimiter. Or a txt file path for the gene id list.
''' </param>
Public Function Run(model As String, 
                       Optional deletes As String = "", 
                       Optional time As String = "100", 
                       Optional out As String = "", 
                       Optional json As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/run")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    If Not deletes.StringEmpty Then
            Call CLI.Append("/deletes " & """" & deletes & """ ")
    End If
    If Not time.StringEmpty Then
            Call CLI.Append("/time " & """" & time & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If json Then
        Call CLI.Append("/json ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /union /in &lt;json.dataset_folder&gt; [/out &lt;result.folder&gt;]
''' ```
''' </summary>
'''

Public Function [Union]([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/union")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace

