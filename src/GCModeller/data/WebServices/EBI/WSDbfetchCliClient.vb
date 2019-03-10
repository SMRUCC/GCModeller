﻿#Region "Microsoft.VisualBasic::a70b7af47b0ae60853734c019d1b2da4, data\WebServices\EBI\WSDbfetchCliClient.vb"

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

'' $Id: WSDbfetchCliClient.vb 2523 2013-02-06 13:51:28Z hpm $
'' ======================================================================
'' 
'' Copyright 2011-2013 EMBL - European Bioinformatics Institute
''
'' Licensed under the Apache License, Version 2.0 (the "License");
'' you may not use this file except in compliance with the License.
'' You may obtain a copy of the License at
''
''     http://www.apache.org/licenses/LICENSE-2.0
''
'' Unless required by applicable law or agreed to in writing, software
'' distributed under the License is distributed on an "AS IS" BASIS,
'' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'' See the License for the specific language governing permissions and
'' limitations under the License.
'' 
'' ======================================================================
'' WSDbfetch Visual Basic .NET command-line client.
''
'' See:
'' http://www.ebi.ac.uk/Tools/webservices/services/dbfetch
'' http://www.ebi.ac.uk/Tools/webservices/tutorials/vb.net
'' ======================================================================

'Option Explicit On
'Option Strict On

'Imports System
'Imports SMRUCC.genomics.DataWebServices.EbiWS ' Service wrapper classes.

'Namespace EbiWS
'	Public Class WSDbfetchCliClient 
'		Inherits WSDbfetchClient
'		' Tool specific usage.
'		Private usageMsg As String = "WSDbfetch" & Environment.NewLine & _
'"=========" & Environment.NewLine & _
'Environment.NewLine & _
'"WSDbfetchCliClient.exe <method> [arguments...]" & Environment.NewLine & _
'Environment.NewLine & _
'"A number of methods are available:" & Environment.NewLine & _
'Environment.NewLine & _
'"getSupportedDBs - list available databases" & Environment.NewLine & _
'"getSupportedFormats - list available databases with formats" & Environment.NewLine & _
'"getSupportedStyles - list available databases with styles" & Environment.NewLine & _
'"getDbFormats - list formats for a specifed database" & Environment.NewLine & _
'"getFormatStyles - list styles for a specified database and format" & Environment.NewLine & _
'"fetchData - retrive an database entry. See below for details of arguments." & Environment.NewLine & _
'"fetchBatch - retrive database entries. See below for details of arguments." & Environment.NewLine & _
'Environment.NewLine & _
'"Fetching an entry: fetchData" & Environment.NewLine & _
'Environment.NewLine & _
'"WSDbfetchCliClient.exe fetchData <dbName:id> [format [style]]" & Environment.NewLine & _
'Environment.NewLine & _
'"dbName:id  database name and entry ID or accession (e.g. UNIPROT:WAP_RAT)" & Environment.NewLine & _
'"format     format to retrive (e.g. uniprot)" & Environment.NewLine & _
'"style      style to retrive (e.g. raw)" & Environment.NewLine & _
'Environment.NewLine & _
'"Fetching entries: fetchBatch" & Environment.NewLine & _
'Environment.NewLine & _
'"WSDbfetchCliClient.exe fetchBatch <dbName> <idList> [format [style]]" & Environment.NewLine & _
'Environment.NewLine & _
'"dbName     database name (e.g. UNIPROT)" & Environment.NewLine & _
'"idList     list of entry IDs or accessions (e.g. 1433T_RAT,WAP_RAT)." & Environment.NewLine & _
'"           Maximum of 200 IDs or accessions." & Environment.NewLine & _
'"format     format to retrive (e.g. uniprot)" & Environment.NewLine & _
'"style      style to retrive (e.g. raw)" & Environment.NewLine & _
'Environment.NewLine & _
'"Further information:" & Environment.NewLine & _
'Environment.NewLine & _
'"  http://www.ebi.ac.uk/Tools/webservices/services/dbfetch" & Environment.NewLine & _
'"  http://www.ebi.ac.uk/Tools/webservices/tutorials/vb.net" & Environment.NewLine & _
'Environment.NewLine & _
'"Support/Feedback:" & Environment.NewLine & _
'Environment.NewLine & _
'"  http://www.ebi.ac.uk/support/" & Environment.NewLine

'		' Execution entry point.
'		Public Shared Function Main( args() As String) As Integer
'			Dim retVal As Integer = 0 ' Return value
'			' Create an instance of the wrapper object
'			Dim wsApp As EbiWS.WSDbfetchCliClient
'			wsApp = New EbiWS.WSDbfetchCliClient()
'			' If no arguments print usage and return
'			If args.Length < 1 Then
'				wsApp.PrintUsageMessage()
'				Return retVal
'			End If
'			Try
'				' Parse the command line
'				retVal = wsApp.ParseCommand(args)
'			Catch ex As System.Exception ' Catch all exceptions
'				Console.Error.WriteLine("Error: " + ex.Message)
'				'Console.Error.WriteLine(ex.StackTrace)
'				retVal = 2
'			End Try
'			Return retVal
'		End Function

'		' Print the usage message.
'		Private Sub PrintUsageMessage()
'			PrintDebugMessage("PrintUsageMessage", "Begin", 1)
'			Console.WriteLine(usageMsg)
'			PrintDebugMessage("PrintUsageMessage", "End", 1)
'		End Sub

'		' Parse command-line options.
'		Private Function ParseCommand( args() As String) As Integer
'			PrintDebugMessage("ParseCommand", "Begin", 1)
'			' Return value
'			Dim retVal As Integer = 0
'			' Parameter values.
'			Dim dbName As String = "default"
'			Dim formatName As String = "default"
'			Dim styleName As String = "default"
'			' Loop over command-line options
'			For i As Integer = 0 To (args.Length - 1) Step 1
'				If retVal <> 0 Then
'					Exit For
'				End If
'				PrintDebugMessage("parseCommand", "arg(" & i & "): " & args(i), 2)
'				Select Case args(i)
'					' Generic options.
'					Case "--help" ' Usage info
'						Me.PrintUsageMessage()
'						Exit Select
'					Case "-h"
'						Me.PrintUsageMessage()
'						Exit Select
'					Case "/help"
'						Me.PrintUsageMessage()
'						Exit Select
'					Case "/h"
'						Me.PrintUsageMessage()
'						Exit Select
'					Case "--verbose" ' Increase output level
'						Me.OutputLevel += 1
'						Exit Select
'					Case "/verbose"
'						Me.OutputLevel += 1
'						Exit Select
'					Case "--quiet" ' Reduce output level
'						Me.OutputLevel -= 1
'						Exit Select
'					Case "/quiet"
'						Me.OutputLevel -= 1
'						Exit Select
'					Case "--debugLevel" ' Debug output level.
'						i += 1 ' Shift to option value
'						Me.DebugLevel = Convert.ToInt32(args(i))
'						Exit Select
'					Case "/debugLevel"
'						i += 1 ' Shift to option value
'						Me.DebugLevel = Convert.ToInt32(args(i))
'						Exit Select
'					Case "--endpoint" ' Service endpoint
'						i += 1 ' Shift to option value
'						Me.ServiceEndPoint = args(i)
'						Exit Select
'					Case "/endpoint"
'						i += 1 ' Shift to option value
'						Me.ServiceEndPoint = args(i)
'						Exit Select

'					Case "getSupportedDBs" ' Databases available to search.
'						Me.PrintGetSupportedDBs()
'						Exit Select
'					Case "getSupportedFormats" ' Databases and formats available.
'						Me.PrintGetSupportedFormats()
'						Exit Select
'					Case "getSupportedStyles" ' Databases and styles available.
'						Me.PrintGetSupportedStyles()
'						Exit Select
'					Case "getDbFormats" ' Formats for a database.
'						i += 1 ' Shift to option value
'						Me.PrintGetDbFormats(args(i))
'						Exit Select
'					Case "getFormatStyles" ' Styles for a format of a database.
'						i += 1 ' Shift to option value for dbName
'						dbName = args(i)
'						i += 1 ' Shift to option value for formatName
'						formatName = args(i)
'						Me.PrintGetFormatStyles(dbName, formatName)
'						Exit Select
'					Case "fetchData" ' Fetch an entry.
'						i += 1 ' Shift to option value for "query".
'						Dim query As String = args(i)
'						If args.Length > (i + 1) Then
'							i += 1 ' Shift to option value for formatName.
'							formatName = args(i)
'						End If
'						If args.Length > (i + 1) Then
'							i += 1 ' Shift to option value for styleName.
'							styleName = args(i)
'						End If
'						Me.PrintFetchData(query, formatName, styleName)
'						Exit Select
'					Case "fetchBatch" ' Fetch a set of entries.
'						i += 1 ' Shift to option value for dbName
'						dbName = args(i)
'						i += 1 ' Shift to option value for idList
'						Dim idListStr As String = args(i)
'						If args.Length > (i + 1) Then
'							i += 1 ' Shift to option value for formatName.
'							formatName = args(i)
'						End If
'						If args.Length > (i + 1) Then
'							i += 1 ' Shift to option value for styleName.
'							styleName = args(i)
'						End If
'						Me.PrintFetchBatch(dbName, idListStr, formatName, styleName)
'						Exit Select
'					Case Else ' Don't know what to do, so print error message
'						Console.Error.WriteLine("Error: unknown option: " & args(i) & Environment.Newline)
'						retVal = 1
'						Exit Select
'				End Select
'				PrintDebugMessage("parseCommand", "arg(" & i & ")", 2)
'			Next i
'			PrintDebugMessage("ParseCommand", "End", 1)
'			Return retVal
'		End Function

'	End Class
'End Namespace
