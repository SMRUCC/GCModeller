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
'  // VERSION:   3.3277.7238.20186
'  // ASSEMBLY:  Settings, Version=3.3277.7238.20186, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/26/2019 11:12:52 AM
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
'  /association:     
'  /fill.zero:       
'  /name.values:     Subset of the input table file by columns, produce a <name,value,description> dataset.
'  /Print:           Print the csv/xlsx file content onto the console screen or text file in table layout.
'  /removes:         Removes row or column data by given regular expression pattern.
'  /subset:          Subset of the table file by a given specific column labels
'  /subtract:        Performing ``a - b`` subtract by row unique id.
'  /takes:           Takes specific rows by a given row id list.
'  /transpose:       
' 
' 
' API list that with functional grouping
' 
' 1. Comma-Separated Values CLI Helpers
' 
' 
'    /cbind:           Join of two table by a unique ID.
'    /rbind:           Row bind(merge tables directly) of the csv tables
'    /rbind.group:     
'    /union:           
'    /unique:          Helper tools for make the ID column value uniques.
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
''' /association /a &lt;a.csv> /b &lt;dataset.csv> [/column.A &lt;scan0> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function Association(a As String, b As String, Optional column_a As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/association")
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
''' /name.values /in &lt;table.csv> /name &lt;fieldName> /value &lt;fieldName> [/describ &lt;descriptionInfo.fieldName, default=Description> /out &lt;values.csv>]
''' ```
''' Subset of the input table file by columns, produce a <name,value,description> dataset.
''' </summary>
'''
Public Function NameValues([in] As String, name As String, value As String, Optional describ As String = "Description", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/name.values")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/name " & """" & name & """ ")
    Call CLI.Append("/value " & """" & value & """ ")
    If Not describ.StringEmpty Then
            Call CLI.Append("/describ " & """" & describ & """ ")
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
''' /Print /in &lt;table.csv/xlsx> [/fields &lt;fieldNames> /sheet &lt;sheetName> /out &lt;device/txt>]
''' ```
''' Print the csv/xlsx file content onto the console screen or text file in table layout.
''' </summary>
'''
Public Function Print([in] As String, Optional fields As String = "", Optional sheet As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Print")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not fields.StringEmpty Then
            Call CLI.Append("/fields " & """" & fields & """ ")
    End If
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
''' /rbind /in &lt;*.csv.DIR> [/order_by &lt;column_name> /out &lt;EXPORT.csv>]
''' ```
''' Row bind(merge tables directly) of the csv tables
''' </summary>
'''
Public Function rbind([in] As String, Optional order_by As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/rbind")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not order_by.StringEmpty Then
            Call CLI.Append("/order_by " & """" & order_by & """ ")
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
''' /subset /in &lt;table.csv> /columns &lt;column.list> [/out &lt;subset.csv>]
''' ```
''' Subset of the table file by a given specific column labels
''' </summary>
'''
Public Function SubsetByColumns([in] As String, columns As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/subset")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/columns " & """" & columns & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /subtract /a &lt;data.csv> /b &lt;data.csv> [/out &lt;subtract.csv>]
''' ```
''' Performing ``a - b`` subtract by row unique id.
''' </summary>
'''
Public Function Subtract(a As String, b As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/subtract")
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
''' /takes /in &lt;data.csv> /id &lt;id.list> [/reverse /out &lt;takes.csv>]
''' ```
''' Takes specific rows by a given row id list.
''' </summary>
'''
Public Function Takes([in] As String, id As String, Optional out As String = "", Optional reverse As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/takes")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/id " & """" & id & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If reverse Then
        Call CLI.Append("/reverse ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /transpose /in &lt;data.csv> [/out &lt;data.transpose.csv>]
''' ```
''' </summary>
'''
Public Function Transpose([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/transpose")
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
''' /union /in &lt;*.csv.DIR> [/tag.field &lt;null> /out &lt;export.csv>]
''' ```
''' </summary>
'''
Public Function [Union]([in] As String, Optional tag_field As String = "", Optional out As String = "") As Integer
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
''' /unique /in &lt;dataset.csv> [/out &lt;out.csv>]
''' ```
''' Helper tools for make the ID column value uniques.
''' </summary>
'''
Public Function Unique([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/unique")
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
