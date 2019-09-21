#Region "Microsoft.VisualBasic::7d3ec5e7c3da82bdedfab931ea94016f, WebServices\EBI\wsdbfetch.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'' ======================================================================
'' WSDbfetch Visual Basic .NET client.
''
'' See
'' http://www.ebi.ac.uk/Tools/webservices/services/dbfetch
'' http://www.ebi.ac.uk/Tools/webservices/clients/dbfetch
'' http://www.ebi.ac.uk/Tools/webservices/tutorials/vb.net
'' ======================================================================

'Imports System
'Imports System.Text
'Imports SMRUCC.genomics.DataWebServices.EbiWS.WsDbfetchSOAP ' "Web Reference" or wsdl.exe generated stubs.

'Namespace ebiws
'	Class WsDbfetch
'		' Web service proxy
'		Private Dbfetch As WSDBFetchServerService = Nothing
'		' Output level
'		Private outputLevel As Integer = 1

'		' Get the webservice proxy (if not initalised)
'		Private Sub WsConnect()
'			If Me.Dbfetch Is Nothing Then
'				Me.Dbfetch = New WSDBFetchServerService()
'			End If
'		End Sub

'		' Print a list of strings
'		Private Sub PrintStrList( strList As String())
'			For Each item As String In strList
'				If item IsNot Nothing AndAlso item <> "" Then
'					Console.WriteLine(item)
'				End If
'			Next
'		End Sub

'		' Get the list of supported databases
'		Public Function GetSupportedDBs() As String()
'			Dim retVal As String()
'			retVal = Me.Dbfetch.getSupportedDBs()
'			Return retVal
'		End Function

'		' Get the list of supported formats for each database
'		Public Function GetSupportedFormats() As String()
'			Dim retVal As String()
'			retVal = Me.Dbfetch.getSupportedFormats()
'			Return retVal
'		End Function

'		' Get the list of supported styles for each database
'		Public Function GetSupportedStyles() As String()
'			Dim retVal As String()
'			retVal = Me.Dbfetch.getSupportedStyles()
'			Return retVal
'		End Function

'		' Get a list of the available formats for a specific database
'		Public Function GetDbFormats( dbName As String) As String()
'			Dim retVal As String()
'			retVal = Me.Dbfetch.getDbFormats(dbName)
'			Return retVal
'		End Function

'		' Get a list of supported styles for a database and format
'		Public Function GetFormatStyles( dbName As String,  dataFormat As String) As String()
'			Dim retVal As String()
'			retVal = Me.Dbfetch.getFormatStyles(dbName, dataFormat)
'			Return retVal
'		End Function

'		' Fetch a database entry
'		Public Function FetchData( args As String()) As String()
'			Dim retVal As String() = New String(200) {}
'			If args.Length > 1 Then
'				Dim dataFormat As String
'				If args.Length > 2 Then
'					dataFormat = args(2)
'				Else
'					dataFormat = "default"
'				End If
'				Dim dataStyle As String
'				If args.Length > 3 Then
'					dataStyle = args(3)
'				Else
'					dataStyle = "default"
'				End If
'				' Increased output
'				If outputLevel > 1 Then
'					Console.WriteLine("entryId: " + args(1))
'					Console.WriteLine("dataFormat: " + dataFormat)
'					Console.WriteLine("dataStyle: " + dataStyle)
'				End If
'				If args(1) = "-" Then
'					' IDs from STDIN
'					Dim line As String = Nothing
'					Dim entryN As Integer = 0
'					While line = Console.ReadLine() AndAlso line IsNot Nothing AndAlso entryN < 201
'						' Strip line ending
'						line = line.Trim()
'						retVal(entryN) = Me.Dbfetch.fetchData(line, dataFormat, dataStyle)
'						entryN += 1
'							' Wait before next entry
'						System.Threading.Thread.Sleep(1000)
'					End While
'				Else
'					retVal(0) = Me.Dbfetch.fetchData(args(1), dataFormat, dataStyle)
'				End If
'			End If
'			Return retVal
'		End Function

'		' Fetch a set of database entries
'		Public Function FetchBatch( args As String()) As String()
'			Dim retVal As String() = New String(0) {}
'			' ID list
'			Dim idList As String = Nothing
'			If args(2) = "-" Then
'				' Get ID list from STDIN
'				Dim tmpIdList As New StringBuilder()
'				Dim line As String = Nothing
'				While line = Console.ReadLine() AndAlso line IsNot Nothing
'					' Strip line ending
'					line = line.Trim()
'					' Add to list
'					If tmpIdList.Length > 0 Then
'						tmpIdList.Append("," + line)
'					Else
'						tmpIdList.Append(line)
'					End If
'				End While
'				idList = tmpIdList.ToString()
'			Else
'				idList = args(2)
'			End If
'			If args.Length > 2 Then
'				Dim dataFormat As String
'				If args.Length > 3 Then
'					dataFormat = args(3)
'				Else
'					dataFormat = "default"
'				End If
'				Dim dataStyle As String
'				If args.Length > 4 Then
'					dataStyle = args(4)
'				Else
'					dataStyle = "default"
'				End If
'				' Increased output
'				If outputLevel > 1 Then
'					Console.WriteLine(args(1) + "\t" + idList + "\t" + dataFormat + "\t" + dataStyle)
'				End If
'				retVal(0) = Me.Dbfetch.fetchBatch(args(1), idList, dataFormat, dataStyle)
'			End If
'			Return retVal
'		End Function

'		' Entry point for execution
'		Public Shared Function Main( args As String()) As Integer
'			Dim retVal As Integer = 0
'			' Usage message
'			Dim usageMsg As String = "Usage:"  & Environment.NewLine & _
'"  wsdbfetch.exe <method> [arguments...]"  & Environment.NewLine & _
'""  & Environment.NewLine & _
'"A number of methods are available:"  & Environment.NewLine & _
'""  & Environment.NewLine & _
'"  getSupportedDBs - list available databases"  & Environment.NewLine & _
'"  getSupportedFormats - list available databases with formats"  & Environment.NewLine & _
'"  getSupportedStyles - list available databases with styles"  & Environment.NewLine & _
'"  getDbFormats - list formats for a specifed database"  & Environment.NewLine & _
'"  getFormatStyles - list styles for a specified database and format"  & Environment.NewLine & _
'"  fetchData - retrive an database entry. See below for details of arguments."  & Environment.NewLine & _
'"  fetchBatch - retrive database entries. See below for details of arguments."  & Environment.NewLine & _
'""  & Environment.NewLine & _
'"Fetching an entry: fetchData"  & Environment.NewLine & _
'""  & Environment.NewLine & _
'"  wsdbfetch.exe fetchData <dbName:id> [format [style]]"  & Environment.NewLine & _
'""  & Environment.NewLine & _
'"  dbName:id  database name and entry ID or accession (e.g. UNIPROT:WAP_RAT)"  & Environment.NewLine & _
'"  format     format to retrive (e.g. uniprot)"  & Environment.NewLine & _
'"  style      style to retrive (e.g. raw)"  & Environment.NewLine & _
'""  & Environment.NewLine & _
'"Fetching entries: fetchBatch"  & Environment.NewLine & _
'""  & Environment.NewLine & _
'"  wsdbfetch.exe fetchBatch <dbName> <idList> [format [style]]"  & Environment.NewLine & _
'""  & Environment.NewLine & _
'"  dbName     database name (e.g. UNIPROT)"  & Environment.NewLine & _
'"  idList     list of entry IDs or accessions (e.g. 1433T_RAT,WAP_RAT)."  & Environment.NewLine & _
'"             Maximum of 200 IDs or accessions."  & Environment.NewLine & _
'"  format     format to retrive (e.g. uniprot)"  & Environment.NewLine & _
'"  style      style to retrive (e.g. raw)"

'			' If aruments specified
'			If args.Length > 0 Then
'				Try
'					' Get an intance of the wrapper object
'					Dim wsDbFetch As New WsDbfetch()
'					' Initialise the webservice proxy
'					wsDbFetch.WsConnect()
'					Dim result As String()
'					' for results.
'					' Perform selected action
'					Select Case args(0)
'						Case "getSupportedDBs"
'							' Get list of available databases
'							result = wsDbFetch.GetSupportedDBs()
'							wsDbFetch.PrintStrList(result)
'							Exit Select
'						Case "getSupportedFormats"
'							' Get list of available formats
'							result = wsDbFetch.GetSupportedFormats()
'							wsDbFetch.PrintStrList(result)
'							Exit Select
'						Case "getSupportedStyles"
'							' Get list of available sytles
'							result = wsDbFetch.GetSupportedStyles()
'							wsDbFetch.PrintStrList(result)
'							Exit Select
'						Case "getDbFormats"
'							' Get list of formats for a database
'							result = wsDbFetch.GetDbFormats(args(1))
'							wsDbFetch.PrintStrList(result)
'							Exit Select
'						Case "getFormatStyles"
'							' Get list of styles for a database and format
'							result = wsDbFetch.GetFormatStyles(args(1), args(2))
'							wsDbFetch.PrintStrList(result)
'							Exit Select
'						Case "fetchData"
'							' Fetch an entry
'							result = wsDbFetch.FetchData(args)
'							wsDbFetch.PrintStrList(result)
'							Exit Select
'						Case "fetchBatch"
'							' Fetch a set of entries
'							result = wsDbFetch.FetchBatch(args)
'							wsDbFetch.PrintStrList(result)
'							Exit Select
'						Case Else
'							' Don't know what to do, so print usage message.
'							Console.WriteLine(usageMsg)
'							retVal = 1
'							Exit Select
'					End Select
'				Catch ex As System.Exception
'					Console.WriteLine("Exception: " + ex.Message)
'					Console.WriteLine(ex.StackTrace)
'					retVal = 2
'				End Try
'			Else
'				' No arguments specified so print usage
'				Console.WriteLine(usageMsg)
'			End If
'			Return retVal
'		End Function
'	End Class
'End Namespace
