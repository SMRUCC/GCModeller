#Region "Microsoft.VisualBasic::221128bf699f955173f5036701435bfe, Shared\InternalApps_CLI\Apps\Excel.vb"

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

    ' Class Excel
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
' assembly: ..\bin\Excel.exe

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
'  < Excel_CLI.CLI >
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /Association:     
'  /fill.zero:       
'  /Print:           Print the csv/xlsx file content onto the console screen or text file in table layout.
'  /removes:         Removes row or column data by given regular expression pattern.
'  /Subtract:        
' 
' 
' API list that with functional grouping
' 
' 1. Comma-Separated Values CLI Helpers
' 
' 
'    /Cbind:           Join of two table by a unique ID.
'    /rbind:           Row bind(merge tables directly) of the csv tables
'    /rbind.group:     
'    /union:           
'    /Unique:          Helper tools for make the ID column value uniques.
' 
' 
' 2. Microsoft Xlsx File CLI Tools
' 
' 
'    /Create:          Create an empty Excel xlsx package file on a specific file path
'    /Extract:         Open target excel file And get target table And save into a csv file.
'    /push:            Write target csv table its content data as a worksheet into the target Excel package.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Excel_CLI.CLI
''' </summary>
'''
Public Class Excel : Inherits InteropService

    Public Const App$ = "Excel.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Excel
          Return New Excel(App:=directory & "/" & Excel.App)
     End Function

''' <summary>
''' ```
''' /Association /a &lt;a.csv> /b &lt;dataset.csv> [/column.A &lt;scan0> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function Association(a As String, b As String, Optional column_a As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Association")
    Call CLI.Append(" ")
    Call CLI.Append("/a " & """" & a & """ ")
    Call CLI.Append("/b " & """" & b & """ ")
    If Not column_a.StringEmpty Then
            Call CLI.Append("/column.a " & """" & column_a & """ ")
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
''' /cbind /in &lt;a.csv> /append &lt;b.csv> [/ID.a &lt;default=ID> /ID.b &lt;default=ID> /grep.ID &lt;grep_script, default="token &lt;SPACE> first"> /unique /nothing.as.empty /out &lt;ALL.csv>]
''' ```
''' Join of two table by a unique ID.
''' </summary>
'''
Public Function cbind([in] As String, append As String, Optional id_a As String = "ID", Optional id_b As String = "ID", Optional grep_id As String = "token <SPACE", Optional out As String = "", Optional unique As Boolean = False, Optional nothing_as_empty As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/cbind")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/append " & """" & append & """ ")
    If Not id_a.StringEmpty Then
            Call CLI.Append("/id.a " & """" & id_a & """ ")
    End If
    If Not id_b.StringEmpty Then
            Call CLI.Append("/id.b " & """" & id_b & """ ")
    End If
    If Not grep_id.StringEmpty Then
            Call CLI.Append("/grep.id " & """" & grep_id & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If unique Then
        Call CLI.Append("/unique ")
    End If
    If nothing_as_empty Then
        Call CLI.Append("/nothing.as.empty ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Create /target &lt;xlsx>
''' ```
''' Create an empty Excel xlsx package file on a specific file path
''' </summary>
'''
Public Function newEmpty(target As String) As Integer
    Dim CLI As New StringBuilder("/Create")
    Call CLI.Append(" ")
    Call CLI.Append("/target " & """" & target & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Extract /open &lt;xlsx> [/sheetName &lt;name_string, default=*> /out &lt;out.csv/directory>]
''' ```
''' Open target excel file And get target table And save into a csv file.
''' </summary>
'''
Public Function Extract(open As String, Optional sheetname As String = "*", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Extract")
    Call CLI.Append(" ")
    Call CLI.Append("/open " & """" & open & """ ")
    If Not sheetname.StringEmpty Then
            Call CLI.Append("/sheetname " & """" & sheetname & """ ")
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
''' /fill.zero /in &lt;dataset.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function FillZero([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/fill.zero")
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
''' ```
''' /Print /in &lt;table.csv/xlsx> [/sheet &lt;sheetName> /out &lt;device/txt>]
''' ```
''' Print the csv/xlsx file content onto the console screen or text file in table layout.
''' </summary>
'''
Public Function Print([in] As String, Optional sheet As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Print")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not sheet.StringEmpty Then
            Call CLI.Append("/sheet " & """" & sheet & """ ")
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
''' /push /write &lt;*.xlsx> /table &lt;*.csv> [/sheetName &lt;name_string> /saveAs &lt;*.xlsx>]
''' ```
''' Write target csv table its content data as a worksheet into the target Excel package.
''' </summary>
'''
Public Function PushTable(write As String, table As String, Optional sheetname As String = "", Optional saveas As String = "") As Integer
    Dim CLI As New StringBuilder("/push")
    Call CLI.Append(" ")
    Call CLI.Append("/write " & """" & write & """ ")
    Call CLI.Append("/table " & """" & table & """ ")
    If Not sheetname.StringEmpty Then
            Call CLI.Append("/sheetname " & """" & sheetname & """ ")
    End If
    If Not saveas.StringEmpty Then
            Call CLI.Append("/saveas " & """" & saveas & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /rbind /in &lt;*.csv.DIR> [/out &lt;EXPORT.csv>]
''' ```
''' Row bind(merge tables directly) of the csv tables
''' </summary>
'''
Public Function rbind([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/rbind")
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
''' ```
''' /rbind.group /in &lt;*.csv.DIR> [/out &lt;out.directory>]
''' ```
''' </summary>
'''
Public Function rbindGroup([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/rbind.group")
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
''' ```
''' /removes /in &lt;dataset.csv> /pattern &lt;regexp_pattern> [/by_row /out &lt;out.csv>]
''' ```
''' Removes row or column data by given regular expression pattern.
''' </summary>
'''
Public Function Removes([in] As String, pattern As String, Optional out As String = "", Optional by_row As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/removes")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/pattern " & """" & pattern & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If by_row Then
        Call CLI.Append("/by_row ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Subtract /a &lt;data.csv> /b &lt;data.csv> [/out &lt;subtract.csv>]
''' ```
''' </summary>
'''
Public Function Subtract(a As String, b As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Subtract")
    Call CLI.Append(" ")
    Call CLI.Append("/a " & """" & a & """ ")
    Call CLI.Append("/b " & """" & b & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /union /in &lt;*.csv.DIR> [/tag.field &lt;null> /out &lt;export.csv>]
''' ```
''' </summary>
'''
Public Function Union([in] As String, Optional tag_field As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/union")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not tag_field.StringEmpty Then
            Call CLI.Append("/tag.field " & """" & tag_field & """ ")
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
''' /Unique /in &lt;dataset.csv> [/out &lt;out.csv>]
''' ```
''' Helper tools for make the ID column value uniques.
''' </summary>
'''
Public Function Unique([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Unique")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace

