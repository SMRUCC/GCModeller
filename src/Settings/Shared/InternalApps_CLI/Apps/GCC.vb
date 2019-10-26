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
'  // VERSION:   3.3277.7238.20186
'  // ASSEMBLY:  Settings, Version=3.3277.7238.20186, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/26/2019 11:12:52 AM
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
'  /compile.KEGG:                   Create GCModeller virtual cell data model file from KEGG reference
'                                   data.
'  /compile.organism:               Create GCModeller virtual cell data model from KEGG organism pathway
'                                   data
'  /export.model.graph:             Export cellular module network from virtual cell model file for
'                                   cytoscape visualization.
'  /export.model.pathway_graph:     
'  /summary:                        
'  -add_replacement:                
'  -add_rule:                       
'  compile_metacyc:                 compile a metacyc database into a gcml(genetic clock markup language)
'                                   model file.
' 
' 
' API list that with functional grouping
' 
' 1. iGEM cli tools
' 
' 
'    /iGEM.query.parts:               Query parts data from iGEM server by given id list.
'    /iGEM.select.parts:              Select iGEM part sequence by given id list.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

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
''' Create GCModeller virtual cell data model file from KEGG reference data.
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
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /compile.organism /in &lt;genome.gb> /kegg &lt;kegg.organism_pathways.repository/model.xml> [/regulations &lt;transcription.regulates.csv> /out &lt;out.model.Xml>]
''' ```
''' Create GCModeller virtual cell data model from KEGG organism pathway data
''' </summary>
'''
Public Function CompileKEGGOrganism([in] As String, kegg As String, Optional regulations As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/compile.organism")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/kegg " & """" & kegg & """ ")
    If Not regulations.StringEmpty Then
            Call CLI.Append("/regulations " & """" & regulations & """ ")
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
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /export.model.pathway_graph /model &lt;GCMarkup.xml/table.xlsx> [/disable.trim /degree &lt;default=1> /out &lt;out.dir>]
''' ```
''' </summary>
'''
Public Function ExportPathwaysNetwork(model As String, Optional degree As String = "1", Optional out As String = "", Optional disable_trim As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/export.model.pathway_graph")
    Call CLI.Append(" ")
    Call CLI.Append("/model " & """" & model & """ ")
    If Not degree.StringEmpty Then
            Call CLI.Append("/degree " & """" & degree & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If disable_trim Then
        Call CLI.Append("/disable.trim ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /iGEM.query.parts /list &lt;id.list.txt> [/out &lt;table.xls>]
''' ```
''' Query parts data from iGEM server by given id list.
''' </summary>
'''
Public Function QueryParts(list As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/iGEM.query.parts")
    Call CLI.Append(" ")
    Call CLI.Append("/list " & """" & list & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /iGEM.select.parts /list &lt;id.list.txt> /allparts &lt;ALL_parts.fasta> [/out &lt;table.xls>]
''' ```
''' Select iGEM part sequence by given id list.
''' </summary>
'''
Public Function SelectParts(list As String, allparts As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/iGEM.select.parts")
    Call CLI.Append(" ")
    Call CLI.Append("/list " & """" & list & """ ")
    Call CLI.Append("/allparts " & """" & allparts & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /summary &lt;model.GCMarkup>
''' ```
''' </summary>
'''
Public Function Summary(term As String) As Integer
    Dim CLI As New StringBuilder("/summary")
    Call CLI.Append(" ")
    Call CLI.Append($"{term}")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


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
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


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
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


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
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
