#Region "Microsoft.VisualBasic::2e32437ae392fce83a90177fa5ddc419, Shared\InternalApps_CLI\Apps\Synteny.vb"

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

    ' Class Synteny
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
' assembly: ..\bin\Synteny.exe

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
'  < Synteny.CLI >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /cluster.tree:     
'  /mapping.plot:     
'  /test:             
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Synteny.CLI
''' </summary>
'''
Public Class Synteny : Inherits InteropService

    Public Const App$ = "Synteny.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Synteny
          Return New Synteny(App:=directory & "/" & Synteny.App)
     End Function

''' <summary>
''' ```
''' /cluster.tree /in &lt;besthit.csv> /genomes &lt;fasta.directory> [/out &lt;clusters.csv>]
''' ```
''' </summary>
'''
Public Function ClusterTree([in] As String, genomes As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/cluster.tree")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/genomes " & """" & genomes & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /mapping.plot /mapping &lt;blastn_mapping.csv> /query &lt;query.gff3> /ref &lt;subject.gff3> [/Ribbon &lt;default=Spectral:c6> /size &lt;default=6000,4000> /auto.reverse &lt;default=0.9> /grep &lt;default="-"> /out &lt;Synteny.png>]
''' ```
''' </summary>
'''
Public Function PlotMapping(mapping As String, query As String, ref As String, Optional ribbon As String = "Spectral:c6", Optional size As String = "6000,4000", Optional auto_reverse As String = "0.9", Optional grep As String = "-", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/mapping.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/mapping " & """" & mapping & """ ")
    Call CLI.Append("/query " & """" & query & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    If Not ribbon.StringEmpty Then
            Call CLI.Append("/ribbon " & """" & ribbon & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not auto_reverse.StringEmpty Then
            Call CLI.Append("/auto.reverse " & """" & auto_reverse & """ ")
    End If
    If Not grep.StringEmpty Then
            Call CLI.Append("/grep " & """" & grep & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' 
''' ```
''' </summary>
'''
Public Function Test() As Integer
    Dim CLI As New StringBuilder("/test")
    Call CLI.Append(" ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace

