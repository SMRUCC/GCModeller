#Region "Microsoft.VisualBasic::dc3fd76029a7a9ab49c4d0692bed1a5a, Shared\InternalApps_CLI\Apps\venn.vb"

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

    ' Class venn
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
' assembly: ..\bin\venn.exe

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
'  Tools for creating venn diagram model for the R program and venn diagram visualize drawing.
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
' 1. R plot API
' 
'    The R language API tools for invoke the venn diagram plot.
' 
' 
'    .Draw:     Draw the venn diagram from a csv data file, you can specific the diagram drawing options
'               from this command switch value. The generated venn dragram will be saved as tiff file
'               format.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Tools for creating venn diagram model for the R program and venn diagram visualize drawing.
''' </summary>
'''
Public Class venn : Inherits InteropService

    Public Const App$ = "venn.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As venn
          Return New venn(App:=directory & "/" & venn.App)
     End Function

''' <summary>
''' ```
''' .Draw -i &lt;csv_file> [-t &lt;diagram_title> -o &lt;_diagram_saved_path> -s &lt;partitions_option_pairs/*.csv> /First.ID.Skip -rbin &lt;r_bin_directory>]
''' ```
''' Draw the venn diagram from a csv data file, you can specific the diagram drawing options from this command switch value. The generated venn dragram will be saved as tiff file format.
''' </summary>
'''
Public Function VennDiagramA(i As String, Optional t As String = "", Optional o As String = "", Optional s As String = "", Optional rbin As String = "", Optional first_id_skip As Boolean = False) As Integer
    Dim CLI As New StringBuilder(".Draw")
    Call CLI.Append(" ")
    Call CLI.Append("-i " & """" & i & """ ")
    If Not t.StringEmpty Then
            Call CLI.Append("-t " & """" & t & """ ")
    End If
    If Not o.StringEmpty Then
            Call CLI.Append("-o " & """" & o & """ ")
    End If
    If Not s.StringEmpty Then
            Call CLI.Append("-s " & """" & s & """ ")
    End If
    If Not rbin.StringEmpty Then
            Call CLI.Append("-rbin " & """" & rbin & """ ")
    End If
    If first_id_skip Then
        Call CLI.Append("/first.id.skip ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace

