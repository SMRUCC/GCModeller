#Region "Microsoft.VisualBasic::bc907dcf01c020b7d6d36f104a901c81, Shared\InternalApps_CLI\Apps\Profiler.vb"

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

    ' Class Profiler
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
' assembly: ..\bin\Profiler.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   1.0.0.0
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // 
' 
' 
'  < Profiler.CLI >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /GO.clusters:     
'  /GSEA:            
'  /id.converts:     
'  /KO.clusters:     Create KEGG pathway map background for a given genome data.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Profiler.CLI
''' </summary>
'''
Public Class Profiler : Inherits InteropService

    Public Const App$ = "Profiler.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Profiler
          Return New Profiler(App:=directory & "/" & Profiler.App)
     End Function

''' <summary>
''' ```
''' /GO.clusters /uniprot &lt;uniprot.XML> /go &lt;go.obo> [/out &lt;clusters.XML>]
''' ```
''' </summary>
'''
Public Function CreateGOClusters(uniprot As String, go As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/GO.clusters")
    Call CLI.Append(" ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/go " & """" & go & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /GSEA /background &lt;clusters.XML> /geneSet &lt;geneSet.txt> [/hide.progress /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function EnrichmentTest(background As String, geneSet As String, Optional out As String = "", Optional hide_progress As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/GSEA")
    Call CLI.Append(" ")
    Call CLI.Append("/background " & """" & background & """ ")
    Call CLI.Append("/geneSet " & """" & geneSet & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If hide_progress Then
        Call CLI.Append("/hide.progress ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /id.converts /uniprot &lt;uniprot.XML> /geneSet &lt;geneSet.txt> [/out &lt;converts.txt>]
''' ```
''' </summary>
'''
Public Function IDconverts(uniprot As String, geneSet As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/id.converts")
    Call CLI.Append(" ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/geneSet " & """" & geneSet & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /KO.clusters /uniprot &lt;uniprot.XML> /maps &lt;kegg_maps.XML/directory> [/out &lt;clusters.XML>]
''' ```
''' Create KEGG pathway map background for a given genome data.
''' </summary>
'''
Public Function CreateKOCluster(uniprot As String, maps As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KO.clusters")
    Call CLI.Append(" ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace

