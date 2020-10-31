Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\kb.exe

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
'  < kb.CLI >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /field.translate:          
'  /kb.abstract:              
'  /kb.build.query:           
'  /KEGG.compound.rda:        Create a kegg organism-compound maps dataset and save in rda file.
'  /KEGG.maps.background:     
'  /pubmed.kb:                
'  /summary:                  
'  /word.translation:         
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' kb.CLI
''' </summary>
'''
Public Class kb : Inherits InteropService

    Public Const App$ = "kb.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As kb
          Return New kb(App:=directory & "/" & kb.App)
     End Function

''' <summary>
''' ```bash
''' /field.translate /in &lt;data.csv&gt; /field &lt;fieldName&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function TranslateField([in] As String, field As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/field.translate")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/field " & """" & field & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /kb.abstract /in &lt;kb.directory&gt; [/min.weight &lt;default=0.05&gt; /out &lt;out.json&gt;]
''' ```
''' </summary>
'''

Public Function GetKBAbstractInformation([in] As String, Optional min_weight As String = "0.05", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/kb.abstract")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not min_weight.StringEmpty Then
            Call CLI.Append("/min.weight " & """" & min_weight & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /kb.build.query /term &lt;term&gt; [/pages &lt;default=20&gt; /out &lt;out.directory&gt;]
''' ```
''' </summary>
'''

Public Function BingAcademicQuery(term As String, Optional pages As String = "20", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/kb.build.query")
    Call CLI.Append(" ")
    Call CLI.Append("/term " & """" & term & """ ")
    If Not pages.StringEmpty Then
            Call CLI.Append("/pages " & """" & pages & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KEGG.compound.rda /repo &lt;directory&gt; [/out &lt;save.rda&gt;]
''' ```
''' Create a kegg organism-compound maps dataset and save in rda file.
''' </summary>
'''

Public Function KEGGCompoundDataSet(repo As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KEGG.compound.rda")
    Call CLI.Append(" ")
    Call CLI.Append("/repo " & """" & repo & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KEGG.maps.background /in &lt;reference_maps.directory&gt; [/out &lt;gsea_background.rda&gt;]
''' ```
''' </summary>
'''

Public Function KEGGMapsBackground([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KEGG.maps.background")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /pubmed.kb /term &lt;term_string&gt; [/out &lt;out_directory&gt;]
''' ```
''' </summary>
'''

Public Function BuildPubMedDatabase(term As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/pubmed.kb")
    Call CLI.Append(" ")
    Call CLI.Append("/term " & """" & term & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /summary /in &lt;directory&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function Summary([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/summary")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /word.translation /in &lt;list_words.txt&gt; [/out &lt;translation.csv&gt; /@set sleep=2000]
''' ```
''' </summary>
'''

Public Function WordTranslation([in] As String, Optional out As String = "", Optional _set As String = "") As Integer
    Dim CLI As New StringBuilder("/word.translation")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not _set.StringEmpty Then
     Call CLI.Append($"/@set """"--internal_pipeline=TRUE;'{_set}'"""" ")
Else
     Call CLI.Append("/@set --internal_pipeline=TRUE ")
    End If


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
