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
'  // VERSION:   3.3277.7609.23259
'  // ASSEMBLY:  Settings, Version=3.3277.7609.23259, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/31/2020 12:55:18 PM
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
'  /association:     Append part of data of table ``b`` to table ``a``
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
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Excel
          Return New Excel(App:=directory & "/" & Excel.App)
     End Function

''' <summary>
''' ```bash
''' /association /a &lt;a.csv&gt; /b &lt;dataset.csv&gt; [/column.A &lt;scan0&gt; /column.B &lt;scan0&gt; /ignore.blank.index /out &lt;out.csv&gt;]
''' ```
''' Append part of data of table ``b`` to table ``a``
''' </summary>
'''

Public Function Association(a As String, 
                               b As String, 
                               Optional column_a As String = "", 
                               Optional column_b As String = "", 
                               Optional out As String = "", 
                               Optional ignore_blank_index As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/association")
    Call CLI.Append(" ")
    Call CLI.Append("/a " & """" & a & """ ")
    Call CLI.Append("/b " & """" & b & """ ")
    If Not column_a.StringEmpty Then
            Call CLI.Append("/column.a " & """" & column_a & """ ")
    End If
    If Not column_b.StringEmpty Then
            Call CLI.Append("/column.b " & """" & column_b & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If ignore_blank_index Then
        Call CLI.Append("/ignore.blank.index ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /cbind /in &lt;a.csv&gt; /append &lt;b.csv&gt; [/ID.a &lt;default=ID&gt; /ID.b &lt;default=ID&gt; /grep.ID &lt;grep_script, default=&quot;token &lt;SPACE&gt; first&quot;&gt; /unique /nothing.as.empty /out &lt;ALL.csv&gt;]
''' ```
''' Join of two table by a unique ID.
''' </summary>
'''
''' <param name="[in]"> The table for append by column, its row ID can be duplicated.
''' </param>
''' <param name="append"> The target table that will be append into the table ``a``, the row ID must be unique!
''' </param>
''' <param name="grep_ID"> This argument parameter describ how to parse the ID in file ``a.csv``
''' </param>
''' <param name="unique"> Make the id of file ``append`` be unique?
''' </param>
Public Function cbind([in] As String, 
                         append As String, 
                         Optional id_a As String = "ID", 
                         Optional id_b As String = "ID", 
                         Optional grep_id As String = "token <SPACE", 
                         Optional out As String = "", 
                         Optional unique As Boolean = False, 
                         Optional nothing_as_empty As Boolean = False) As Integer
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
''' ```bash
''' /Create /target &lt;xlsx&gt;
''' ```
''' Create an empty Excel xlsx package file on a specific file path
''' </summary>
'''
''' <param name="Create"> The file path for save this New created Excel xlsx package.
''' </param>
Public Function newEmpty(target As String) As Integer
    Dim CLI As New StringBuilder("/Create")
    Call CLI.Append(" ")
    Call CLI.Append("/target " & """" & target & """ ")
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Extract /open &lt;xlsx&gt; [/sheetName &lt;name_string, default=*&gt; /out &lt;out.csv/directory&gt;]
''' ```
''' Open target excel file And get target table And save into a csv file.
''' </summary>
'''
''' <param name="open"> File path of the Excel ``*.xlsx`` file for open And read.
''' </param>
''' <param name="sheetName"> The worksheet table name for read data And save as csv file. 
'''               If this argument value is equals to ``*``, then all of the tables in the target xlsx excel file will be extract.
''' </param>
''' <param name="out"> The csv output file path or a directory path value when the ``/sheetName`` parameter is value ``*``.
''' </param>
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
''' ```bash
''' /fill.zero /in &lt;dataset.csv&gt; [/out &lt;out.csv&gt;]
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
''' ```bash
''' /name.values /in &lt;table.csv&gt; /name &lt;fieldName&gt; /value &lt;fieldName&gt; [/describ &lt;descriptionInfo.fieldName, default=Description&gt; /out &lt;values.csv&gt;]
''' ```
''' Subset of the input table file by columns, produce a &lt;name,value,description&gt; dataset.
''' </summary>
'''

Public Function NameValues([in] As String, 
                              name As String, 
                              value As String, 
                              Optional describ As String = "Description", 
                              Optional out As String = "") As Integer
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
''' ```bash
''' /Print /in &lt;table.csv/xlsx&gt; [/fields &lt;fieldNames&gt; /sheet &lt;sheetName&gt; /out &lt;device/txt&gt;]
''' ```
''' Print the csv/xlsx file content onto the console screen or text file in table layout.
''' </summary>
'''
''' <param name="sheet"> The sheet name of table in xlsx file for display, this option only works when target file format is a xlsx file.
''' </param>
''' <param name="fields"> A list of selected field names for display, seperated with comma symbol. By default, is display all of the fields data.
''' </param>
''' <param name="[in]"> Standard input pipeline device only works for csv/tsv file. Target table file for display on the console.
''' </param>
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
''' ```bash
''' /push /write &lt;*.xlsx&gt; /table &lt;*.csv&gt; [/sheetName &lt;name_string&gt; /saveAs &lt;*.xlsx&gt;]
''' ```
''' Write target csv table its content data as a worksheet into the target Excel package.
''' </summary>
'''
''' <param name="sheetName"> The New sheet table name, if this argument Is Not presented, then the program will 
'''               using the file basename as the sheet table name. If the sheet table name Is exists in current xlsx file, 
'''               then the exists table value will be updated, otherwise will add New table.
''' </param>
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
''' ```bash
''' /rbind /in &lt;*.csv.DIR&gt; [/order_by &lt;column_name&gt; /out &lt;EXPORT.csv&gt;]
''' ```
''' Row bind(merge tables directly) of the csv tables
''' </summary>
'''
''' <param name="[in]"> A directory path that contains csv files that will be merge into one file directly.
''' </param>
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
''' ```bash
''' /rbind.group /in &lt;*.csv.DIR&gt; [/out &lt;out.directory&gt;]
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
''' ```bash
''' /removes /in &lt;dataset.csv&gt; /pattern &lt;regexp_pattern&gt; [/by_row /out &lt;out.csv&gt;]
''' ```
''' Removes row or column data by given regular expression pattern.
''' </summary>
'''
''' <param name="by_row"> This argument specific that removes data by row or by column, by default is by column.
''' </param>
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
''' ```bash
''' /subset /in &lt;table.csv&gt; /columns &lt;column.list&gt; [/out &lt;subset.csv&gt;]
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
''' ```bash
''' /subtract /a &lt;data.csv&gt; /b &lt;data.csv&gt; [/out &lt;subtract.csv&gt;]
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
''' ```bash
''' /takes /in &lt;data.csv&gt; /id &lt;id.list&gt; [/reverse /out &lt;takes.csv&gt;]
''' ```
''' Takes specific rows by a given row id list.
''' </summary>
'''
''' <param name="reverse"> If this argument is presents in the cli inputs, then all of the rows that not in input list will be output as result.
''' </param>
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
''' ```bash
''' /transpose /in &lt;data.csv&gt; [/out &lt;data.transpose.csv&gt;]
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
''' ```bash
''' /union /in &lt;*.csv.DIR&gt; [/tag.field &lt;null&gt; /out &lt;export.csv&gt;]
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
''' ```bash
''' /unique /in &lt;dataset.csv&gt; [/out &lt;out.csv&gt;]
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
