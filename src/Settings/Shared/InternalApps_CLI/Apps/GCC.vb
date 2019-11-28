Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\gcc.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   3.3277.7271.30051
'  // ASSEMBLY:  Settings, Version=3.3277.7271.30051, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     11/28/2019 4:41:42 PM
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
'                                   data. Which the model genome have no reference genome data in KEGG
'                                   database.
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
Public Class gcc : Inherits InteropService

    Public Const App$ = "gcc.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As gcc
          Return New gcc(App:=directory & "/" & gcc.App)
     End Function

''' <summary>
''' ```bash
''' /compile.KEGG /in &lt;genome.gb&gt; /KO &lt;ko.assign.csv&gt; /maps &lt;kegg.pathways.repository&gt; /compounds &lt;kegg.compounds.repository&gt; /reactions &lt;kegg.reaction.repository&gt; [/location.as.locus_tag /regulations &lt;transcription.regulates.csv&gt; /out &lt;out.model.Xml/xlsx&gt;]
''' ```
''' Create GCModeller virtual cell data model file from KEGG reference data. Which the model genome have no reference genome data in KEGG database.
''' </summary>
'''
Public Function CompileKEGG([in] As String, KO As String, maps As String, compounds As String, reactions As String, Optional regulations As String = "", Optional out As String = "", Optional location_as_locus_tag As Boolean = False) As Integer
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
    If location_as_locus_tag Then
        Call CLI.Append("/location.as.locus_tag ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /compile.organism /in &lt;genome.gb&gt; /kegg &lt;kegg.organism_pathways.repository/model.xml&gt; [/location.as.locus_tag /regulations &lt;transcription.regulates.csv&gt; /out &lt;out.model.Xml&gt;]
''' ```
''' Create GCModeller virtual cell data model from KEGG organism pathway data
''' </summary>
'''
Public Function CompileKEGGOrganism([in] As String, kegg As String, Optional regulations As String = "", Optional out As String = "", Optional location_as_locus_tag As Boolean = False) As Integer
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
    If location_as_locus_tag Then
        Call CLI.Append("/location.as.locus_tag ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /export.model.graph /model &lt;GCMarkup.xml/table.xlsx&gt; [/pathway &lt;default=none&gt; /disable.trim /degree &lt;default=1&gt; /out &lt;out.dir&gt;]
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
''' ```bash
''' /export.model.pathway_graph /model &lt;GCMarkup.xml/table.xlsx&gt; [/disable.trim /degree &lt;default=1&gt; /out &lt;out.dir&gt;]
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
''' ```bash
''' /iGEM.query.parts /list &lt;id.list.txt&gt; [/out &lt;table.xls&gt;]
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
''' ```bash
''' /iGEM.select.parts /list &lt;id.list.txt&gt; /allparts &lt;ALL_parts.fasta&gt; [/out &lt;table.xls&gt;]
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
''' ```bash
''' /summary &lt;model.GCMarkup&gt;
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
''' ```bash
''' -add_replacement -old &lt;old_value&gt; -new &lt;new_value&gt;
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
''' ```bash
''' -add_rule -rulefile &lt;path&gt; -db &lt;datadir&gt; -model &lt;path&gt; [-grep &lt;scriptText&gt;]
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
''' ```bash
''' compile_metacyc -i &lt;data_dir&gt; -o &lt;output_file&gt;
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
