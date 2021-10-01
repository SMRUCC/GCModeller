#Region "Microsoft.VisualBasic::8568009530ea4d1204a4c30009a93c4f, Shared\InternalApps_CLI\Apps\GCC.vb"

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

    ' Class gcc
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
' assembly: ..\bin\gcc.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   3.3277.7290.24332
'  // ASSEMBLY:  Settings, Version=3.3277.7290.24332, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     12/17/2019 1:31:04 PM
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
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As gcc
          Return New gcc(App:=directory & "/" & gcc.App)
     End Function

''' <summary>
''' ```bash
''' /compile.KEGG /in &lt;genome.gb&gt; /KO &lt;ko.assign.csv&gt; /maps &lt;kegg.pathways.repository&gt; /compounds &lt;kegg.compounds.repository&gt; /reactions &lt;kegg.reaction.repository&gt; [/location.as.locus_tag /glycan.cpd &lt;id.maps.json&gt; /regulations &lt;transcription.regulates.csv&gt; /out &lt;out.model.Xml/xlsx&gt;]
''' ```
''' Create GCModeller virtual cell data model file from KEGG reference data. Which the model genome have no reference genome data in KEGG database.
''' </summary>
'''
''' <param name="regulations">
''' </param>
''' <param name="[in]"> The genome annotation data in genbank format, apply for the genome data modelling which target genome is not yet published to public.
''' </param>
''' <param name="maps"> The KEGG reference pathway data repository, not the data repository for Map render data.
''' </param>
''' <param name="location_as_locus_tag"> If the target genome for create the VirtualCell model is not yet publish on NCBI, 
'''               then it have no formal locus_tag id assigned for the genes yet, so you can enable this option 
'''               for telling the model compiler use the genes&apos; genome coordinate value as its unique locus_tag 
'''               id value.
''' </param>
Public Function CompileKEGG([in] As String, 
                               KO As String, 
                               maps As String, 
                               compounds As String, 
                               reactions As String, 
                               Optional glycan_cpd As String = "", 
                               Optional regulations As String = "", 
                               Optional out As String = "", 
                               Optional location_as_locus_tag As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/compile.KEGG")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/KO " & """" & KO & """ ")
    Call CLI.Append("/maps " & """" & maps & """ ")
    Call CLI.Append("/compounds " & """" & compounds & """ ")
    Call CLI.Append("/reactions " & """" & reactions & """ ")
    If Not glycan_cpd.StringEmpty Then
            Call CLI.Append("/glycan.cpd " & """" & glycan_cpd & """ ")
    End If
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
''' <param name="kegg"> A directory path that contains pathway data from command ``kegg_tools /Download.Pathway.Maps``.
''' </param>
''' <param name="[in]"> A NCBI genbank file that contains the genomics data. If the genome contains multiple replicon like plasmids, 
'''               you can union all of the replicon data into one genbankfile and then using this union file as this input argument.
''' </param>
Public Function CompileKEGGOrganism([in] As String, 
                                       kegg As String, 
                                       Optional regulations As String = "", 
                                       Optional out As String = "", 
                                       Optional location_as_locus_tag As Boolean = False) As Integer
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
''' <param name="pathway"> Apply a pathway module filter on the network model, only the gene contains in the given pathway list then will be output to user. 
'''               By default is export all. Pathway id should be a KO pathway id list, like ``ko04146,ko02010``, and id was seperated by comma symbol.
''' </param>
Public Function ExportModelGraph(model As String, 
                                    Optional pathway As String = "none", 
                                    Optional degree As String = "1", 
                                    Optional out As String = "", 
                                    Optional disable_trim As Boolean = False) As Integer
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
''' <param name="rulefile"> a file contains some protein interaction rules
''' </param>
''' <param name="db"> original database for the target compiled model
''' </param>
''' <param name="model"> Target model file for adding some new rules
''' </param>
''' <param name="grep"> If null then the system will using the MeatCyc database unique-id parsing method as default.
''' </param>
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
''' <param name="i"> 
''' </param>
''' <param name="o"> 
''' </param>
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

