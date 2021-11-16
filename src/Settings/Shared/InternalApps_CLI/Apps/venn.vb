#Region "Microsoft.VisualBasic::4941e74ce5564bbe7f96b7c80e1a8c27, Shared\InternalApps_CLI\Apps\venn.vb"

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
'  // VERSION:   3.3277.7609.23646
'  // ASSEMBLY:  Settings, Version=3.3277.7609.23646, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/31/2020 1:08:12 PM
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
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As venn
          Return New venn(App:=directory & "/" & venn.App)
     End Function

''' <summary>
''' ```bash
''' .Draw -i &lt;csv_file&gt; [-t &lt;diagram_title&gt; -o &lt;_diagram_saved_path&gt; -s &lt;partitions_option_pairs/*.csv&gt; /First.ID.Skip -rbin &lt;r_bin_directory&gt;]
''' ```
''' Draw the venn diagram from a csv data file, you can specific the diagram drawing options from this command switch value. The generated venn dragram will be saved as tiff file format.
''' </summary>
'''
''' <param name="i"> The csv data source file for drawing the venn diagram graph.
''' </param>
''' <param name="t"> Optional, the venn diagram title text
''' </param>
''' <param name="o"> Optional, the saved file location for the venn diagram, if this switch value is not specific by the user then 
'''               the program will save the generated venn diagram to user desktop folder and using the file name of the input csv file as default.
''' </param>
''' <param name="s"> Optional, the profile settings for the partitions in the venn diagram, each partition profile data is
'''                in a key value paired like: name,color, and each partition profile pair is seperated by a &apos;;&apos; character.
'''               If this switch value is not specific by the user then the program will trying to parse the partition name
'''               from the column values and apply for each partition a randomize color.
''' </param>
''' <param name="rbin"> Optional, Set up the r bin path for drawing the venn diagram, if this switch value is not specific by the user then 
'''               the program just output the venn diagram drawing R script file in a specific location, or if this switch 
'''               value is specific by the user and is valid for call the R program then will output both venn diagram tiff image file and R script for drawing the output venn diagram.
'''               This switch value is just for the windows user, when this program was running on a LINUX/UNIX/MAC platform operating 
'''               system, you can ignore this switch value, but you should install the R program in your linux/MAC first if you wish to
'''                get the venn diagram directly from this program.
''' </param>
Public Function VennDiagramA(i As String, 
                                Optional t As String = "", 
                                Optional o As String = "", 
                                Optional s As String = "", 
                                Optional rbin As String = "", 
                                Optional first_id_skip As Boolean = False) As Integer
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

