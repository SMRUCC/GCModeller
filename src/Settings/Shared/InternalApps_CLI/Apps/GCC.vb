Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\GCC.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   1.0.0.*
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // 
' 
' 
'  gcc=GCModeller Compiler; Compiler program for the GCModeller virtual cell system model
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /compile.KEGG:           Create GCModeller virtual cell data model file.
'  /export.model.graph:     Export cellular module network from virtual cell model file for cytoscape
'                           visualization.
'  -add_replacement:        
'  -add_rule:               
'  compile_metacyc:         compile a metacyc database into a gcml(genetic clock markup language) model
'                           file.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    You can using "Settings ??<commandName>" for getting more details command help.

Namespace GCModellerApps


''' <summary>
''' gcc=GCModeller Compiler; Compiler program for the GCModeller virtual cell system model
''' </summary>
'''
Public Class GCC : Inherits InteropService

    Public Const App$ = "GCC.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As GCC
          Return New GCC(App:=directory & "/" & GCC.App)
     End Function

''' <summary>
''' ```
''' /compile.KEGG /in &lt;genome.gb> /KO &lt;ko.assign.csv> /maps &lt;kegg.pathways.repository> /compounds &lt;kegg.compounds.repository> /reactions &lt;kegg.reaction.repository> [/regulations &lt;transcription.regulates.csv> /out &lt;out.model.Xml/xlsx>]
''' ```
''' Create GCModeller virtual cell data model file.
''' </summary>
'''
Public Function CompileKEGG([in] As String, KO As String, maps As String, compounds As String, reactions As String, Optional regulations As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/compile.KEGG")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/KO " & """" & KO & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    Call CLI.Append("/compounds " & """" & compounds & """ ")
    Call CLI.Append("/reactions " & """" & reactions & """ ")
    If Not regulations.StringEmpty Then
            Call CLI.Append("/regulations " & """" & regulations & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /export.model.graph /model &lt;GCMarkup.xml/table.xlsx> [/pathway &lt;default=none> /disable.trim /degree &lt;default=1> /out &lt;out.dir>]
''' ```
''' Export cellular module network from virtual cell model file for cytoscape visualization.
''' </summary>
'''
Public Function ExportModelGraph(model As String, Optional pathway As String = "none", Optional degree As String = "1", Optional out As String = "", Optional disable_trim As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/export.model.graph")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    If Not pathway.StringEmpty Then
            Call CLI.Append("/pathway " & """" & pathway & """ ")
    End If
    If Not degree.StringEmpty Then
            Call CLI.Append("/degree " & """" & degree & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If disable_trim Then
        Call CLI.Append("/disable.trim ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' -add_replacement -old &lt;old_value> -new &lt;new_value>
''' ```
''' </summary>
'''
Public Function AddNewPair(old As String, [new] As String) As Integer
    Dim CLI As New StringBuilder("-add_replacement")
    Call CLI.Append(" ")
    Call CLI.Append("-old " & """" & old & """ ")
    Call CLI.Append("-new " & """" & [new] & """ ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' -add_rule -rulefile &lt;path> -db &lt;datadir> -model &lt;path> [-grep &lt;scriptText>]
''' ```
''' </summary>
'''
Public Function AddRule(rulefile As String, db As String, model As String, Optional grep As String = "") As Integer
    Dim CLI As New StringBuilder("-add_rule")
    Call CLI.Append(" ")
    Call CLI.Append("-rulefile " & """" & rulefile & """ ")
    Call CLI.Append("-db " & """" & db & """ ")
    Call CLI.Append("-model " & """" & model & """ ")
    If Not grep.StringEmpty Then
            Call CLI.Append("-grep " & """" & grep & """ ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' compile_metacyc -i &lt;data_dir> -o &lt;output_file>
''' ```
''' compile a metacyc database into a gcml(genetic clock markup language) model file.
''' </summary>
'''
Public Function CompileMetaCyc(i As String, o As String) As Integer
    Dim CLI As New StringBuilder("compile_metacyc")
    Call CLI.Append(" ")
    Call CLI.Append("-i " & """" & i & """ ")
    Call CLI.Append("-o " & """" & o & """ ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
