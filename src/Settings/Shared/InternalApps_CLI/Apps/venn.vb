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
'  // VERSION:   3.3277.7242.27856
'  // ASSEMBLY:  Settings, Version=3.3277.7242.27856, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     2019/10/30 15:28:32
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
''' ```bash
''' .Draw -i &lt;csv_file&gt; [-t &lt;diagram_title&gt; -o &lt;_diagram_saved_path&gt; -s &lt;partitions_option_pairs/*.csv&gt; /First.ID.Skip -rbin &lt;r_bin_directory&gt;]
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
