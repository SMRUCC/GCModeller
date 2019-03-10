﻿#Region "Microsoft.VisualBasic::fc820edbffd3bb43e3c07be7affd07b0, data\WebServices\EBI\WSDbfetchClient.vb"

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

'' $Id: WSDbfetchClient.vb 2523 2013-02-06 13:51:28Z hpm $
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
'' WSDbfetch Visual Basic .NET client.
''
'' See:
'' http://www.ebi.ac.uk/Tools/webservices/services/dbfetch
'' http://www.ebi.ac.uk/Tools/webservices/tutorials/vb.net
'' ======================================================================

'Option Explicit On
'Option Strict On

'Imports System
'Imports System.Net
'Imports System.Text
'Imports SMRUCC.genomics.DataWebServices.EbiWS.WSDbfetchWs ' "Web Reference" or wsdl.exe generated stubs.

'Namespace EbiWS
'    Public Class WSDbfetchClient
'        ' Output level
'        Public Property OutputLevel() As Integer
'            Get
'                Return _outputLevel
'            End Get
'            Set( value As Integer)
'                If value > -1 Then
'                    _outputLevel = value
'                End If
'            End Set
'        End Property
'        Private _outputLevel As Integer = 1
'        ' Debug output level.
'        Public Property DebugLevel() As Integer
'            Get
'                Return _debugLevel
'            End Get
'            Set( value As Integer)
'                If value > -1 Then
'                    _debugLevel = value
'                End If
'            End Set
'        End Property
'        Private _debugLevel As Integer = 0
'        ' SOAP service endpoint for requests.
'        Public Property ServiceEndPoint() As String
'            Get
'                Return _serviceEndPoint
'            End Get
'            Set( value As String)
'                _serviceEndPoint = value
'            End Set
'        End Property
'        Private _serviceEndPoint As String = Nothing
'        ' Web service proxy
'        Public Property SrvProxy() As WebRequest

'        ' Client object revision/version.
'        Private revision As String = "$Revision: 2523 $"

'        ' Default constructor.
'        Public Sub New()
'            MyBase.New
'            Me.OutputLevel = 1 ' Normal output.
'            Me.DebugLevel = 0  ' Debug output off.
'        End Sub

'        ' Print a debug message at the specified level.
'        Sub PrintDebugMessage( methodName As String,  message As String,  level As Integer)
'            If level <= Me.DebugLevel Then
'                Console.Error.WriteLine("[{0}()] {1}", methodName, message)
'            End If
'        End Sub

'        ' Print a progress message, at the specified output level.
'        Sub PrintProgressMessage( message As String,  level As Integer)
'            If level <= Me.OutputLevel Then
'                Console.WriteLine(message)
'            End If
'        End Sub

'        ' Get the service connection. Has to be called before attempting to use any of the service operations.
'        Protected Sub ServiceProxyConnect()
'            PrintDebugMessage("ServiceProxyConnect", "Begin", 11)
'            If Me.SrvProxy Is Nothing Then
'                Me.SrvProxy = New EbiWS.WSDbfetchWs.WSDBFetchDoclitServerService()
'                Me.SrvProxy.EnableDecompression = True  ' Support HTTP compression.
'                PrintDebugMessage("ServiceProxyConnect", "SrvProxy: " & Me.SrvProxy.ToString(), 12)
'                SetProxyEndPoint() ' Set explicit service endpoint, if defined.
'                SetProxyUserAgent() ' Set user-agent for client.
'            End If
'            PrintDebugMessage("ServiceProxyConnect", "End", 11)
'        End Sub

'        ' Set specific web service endpoint for web service proxy.
'        Private Sub SetProxyEndPoint()
'            PrintDebugMessage("SetProxyEndPoint", "Begin", 11)
'            If Me.ServiceEndPoint IsNot Nothing AndAlso ServiceEndPoint.Length > 0 Then
'                Me.SrvProxy.Url = Me.ServiceEndPoint
'            End If
'            Me.ServiceEndPoint = Me.SrvProxy.Url
'            PrintDebugMessage("SetProxyEndPoint", "ServiceEndPoint: " & Me.ServiceEndPoint, 12)
'            PrintDebugMessage("SetProxyEndPoint", "End", 11)
'        End Sub

'        ' Set User-agent for web service proxy.
'        Private Sub SetProxyUserAgent()
'            PrintDebugMessage("SetProxyUserAgent", "Begin", 11)
'            Dim userAgent As String = constuctUserAgentStr(revision, Me.GetType().Name, Me.SrvProxy.UserAgent)
'            PrintDebugMessage("SetProxyUserAgent", "userAgent: " & userAgent, 12)
'            Me.SrvProxy.UserAgent = userAgent
'            PrintDebugMessage("SetProxyUserAgent", "End", 11)
'        End Sub

'        ' Construct a User-agent string for the client.
'        Protected Function constuctUserAgentStr( revision As String,  clientClassName As String,  userAgent As String) As String
'            PrintDebugMessage("constuctUserAgentStr", "Begin", 31)
'            Dim retUserAgent As String = "EBI-Sample-Client"
'            Dim clientVersion As String = "0"
'            ' Client version.
'            If revision IsNot Nothing AndAlso revision.Length > 0 Then
'                ' CVS/Subversion revision tag.
'                If revision.StartsWith("$") Then
'                    ' Populated tag, extract revision number.
'                    If revision.Length > 13 Then
'                        clientVersion = revision.Substring(11, (revision.Length - 13))
'                    End If
'                    ' Alternative revision/version string.
'                Else
'                    clientVersion = revision
'                End If
'            End If
'            Dim strBuilder As StringBuilder = New StringBuilder()
'            strBuilder.Append(retUserAgent & "/" & clientVersion)
'            strBuilder.Append(" (" & clientClassName & "; VB.NET; " & Environment.OSVersion.ToString)
'            If userAgent Is Nothing OrElse userAgent.Length < 1 Then ' No agent
'                strBuilder.Append(")")
'            ElseIf userAgent.Contains("(") Then ' MS .NET
'                strBuilder.Append(") " & userAgent)
'            Else ' Mono
'                strBuilder.Append("; " & userAgent & ")")
'            End If
'            retUserAgent = strBuilder.ToString
'            PrintDebugMessage("constuctUserAgentStr", "retUserAgent: " & retUserAgent, 32)
'            PrintDebugMessage("constuctUserAgentStr", "End", 31)
'            Return retUserAgent
'        End Function

'        ' Get list of database names from sevice.
'        Public Function GetSupportedDBs() As String()
'            PrintDebugMessage("GetSupportedDBs", "Begin", 1)
'            ServiceProxyConnect()
'            Dim dbNameList() As String
'            dbNameList = Me.SrvProxy.getSupportedDBs()
'            If dbNameList Is Nothing Then
'                Console.Error.WriteLine("Error: Null returned by web service")
'                dbNameList = New String() {} ' Replace with an empty array.
'            End If
'            PrintDebugMessage("GetSupportedDBs", "got " & dbNameList.Length & " db names", 2)
'            PrintDebugMessage("GetSupportedDBs", "End", 1)
'            Return dbNameList
'        End Function

'        ' Print list of available search databases.
'        Public Sub PrintGetSupportedDBs()
'            PrintDebugMessage("PrintGetSupportedDBs", "Begin", 1)
'            Dim result As String()
'            result = Me.GetSupportedDBs()
'            PrintStrList(result)
'            PrintDebugMessage("PrintGetSupportedDBs", "End", 1)
'        End Sub

'        ' Get list of database and format names from sevice.
'        Public Function GetSupportedFormats() As String()
'            PrintDebugMessage("GetSupportedFormats", "Begin", 1)
'            ServiceProxyConnect()
'            Dim nameList As String()
'            nameList = Me.SrvProxy.getSupportedFormats()
'            If nameList Is Nothing Then
'                Console.Error.WriteLine("Error: Null returned by web service.")
'                nameList = New String() {} ' Replace with an empty array.
'            End If
'            PrintDebugMessage("GetSupportedFormats", "got " & nameList.Length & " names", 2)
'            PrintDebugMessage("GetSupportedFormats", "End", 1)
'            Return nameList
'        End Function

'        ' Print list of available search databases and formats.
'        Public Sub PrintGetSupportedFormats()
'            PrintDebugMessage("PrintGetSupportedFormats", "Begin", 1)
'            Dim result As String()
'            result = Me.GetSupportedFormats()
'            PrintStrList(result)
'            PrintDebugMessage("PrintGetSupportedFormats", "End", 1)
'        End Sub

'        ' Get list of database and style names from sevice.
'        Public Function GetSupportedStyles() As String()
'            PrintDebugMessage("GetSupportedStyles", "Begin", 1)
'            ServiceProxyConnect()
'            Dim nameList As String()
'            nameList = Me.SrvProxy.getSupportedStyles()
'            If nameList Is Nothing Then
'                Console.Error.WriteLine("Error: Null returned by web service.")
'                nameList = New String() {} ' Replace with an empty array.
'            End If
'            PrintDebugMessage("GetSupportedStyles", "got " & nameList.Length & " names", 2)
'            Return nameList
'        End Function

'        ' Print list of available search databases and styles.
'        Public Sub PrintGetSupportedStyles()
'            PrintDebugMessage("PrintGetSupportedStyles", "Begin", 1)
'            Dim result As String()
'            result = Me.GetSupportedStyles()
'            PrintStrList(result)
'            PrintDebugMessage("PrintGetSupportedStyles", "End", 1)
'        End Sub

'        ' Get list of format names for a database.
'        Public Function GetDbFormats( dbName As String) As String()
'            PrintDebugMessage("GetDbFormats", "Begin", 1)
'            ServiceProxyConnect()
'            Dim nameList As String()
'            nameList = Me.SrvProxy.getDbFormats(dbName)
'            If nameList Is Nothing Then
'                Console.Error.WriteLine("Error: Null returned by web service.")
'                nameList = New String() {} ' Replace with an empty array.
'            End If
'            PrintDebugMessage("GetDbFormats", "got " & nameList.Length & " names", 2)
'            PrintDebugMessage("GetDbFormats", "End", 1)
'            Return nameList
'        End Function

'        ' Print list of available format names for a database.
'        Public Sub PrintGetDbFormats( dbName As String)
'            PrintDebugMessage("PrintGetDbFormats", "Begin", 1)
'            Dim result As String()
'            result = Me.GetDbFormats(dbName)
'            PrintStrList(result)
'            PrintDebugMessage("PrintGetDbFormats", "End", 1)
'        End Sub

'        ' Get list of style names for a format of a database.
'        Public Function GetFormatStyles( dbName As String,  formatName As String) As String()
'            PrintDebugMessage("GetFormatStyles", "Begin", 1)
'            ServiceProxyConnect()
'            Dim nameList As String()
'            nameList = Me.SrvProxy.getFormatStyles(dbName, formatName)
'            If nameList Is Nothing Then
'                Console.Error.WriteLine("Error: Null returned by web service.")
'                nameList = New String() {} ' Replace with an empty array.
'            End If
'            PrintDebugMessage("GetFormatStyles", "got " & nameList.Length & " names", 2)
'            PrintDebugMessage("GetFormatStyles", "End", 1)
'            Return nameList
'        End Function

'        ' Print list of available style names for a format of a database.
'        Public Sub PrintGetFormatStyles( dbName As String,  formatName As String)
'            PrintDebugMessage("PrintGetFormatStyles", "Begin", 1)
'            Dim result As String()
'            result = Me.GetFormatStyles(dbName, formatName)
'            PrintStrList(result)
'            PrintDebugMessage("PrintGetFormatStyles", "End", 1)
'        End Sub

'        ' Fetch an entry.
'        Public Function FetchData( query As String,  formatName As String,  styleName As String) As String
'            PrintDebugMessage("FetchData", "Begin", 1)
'            ServiceProxyConnect()
'            Dim entryStr As String
'            entryStr = Me.SrvProxy.fetchData(query, formatName, styleName)
'            If entryStr Is Nothing Then
'                Console.Error.WriteLine("Error: Null returned by web service.")
'            End If
'            PrintDebugMessage("FetchData", "End", 1)
'            Return entryStr
'        End Function

'        ' Print an entry.
'        Public Sub PrintFetchData( query As String,  formatName As String,  styleName As String)
'            PrintDebugMessage("PrintFetchData", "Begin", 1)
'            Dim result As String
'            result = Me.FetchData(query, formatName, styleName)
'            Console.WriteLine(result)
'            PrintDebugMessage("PrintFetchData", "End", 1)
'        End Sub

'        ' Fetch a set of entries.
'        Public Function FetchBatch( dbName As String,  idListStr As String,  formatName As String,  styleName As String) As String
'            PrintDebugMessage("FetchBatch", "Begin", 1)
'            ServiceProxyConnect()
'            Dim entriesStr As String
'            entriesStr = Me.SrvProxy.fetchBatch(dbName, idListStr, formatName, styleName)
'            If entriesStr Is Nothing Then
'                Console.Error.WriteLine("Error: Null returned by web service.")
'            End If
'            PrintDebugMessage("FetchBatch", "End", 1)
'            Return entriesStr
'        End Function

'        ' Print a set of entries.
'        Public Sub PrintFetchBatch( dbName As String,  idListStr As String,  formatName As String,  styleName As String)
'            PrintDebugMessage("PrintFetchBatch", "Begin", 1)
'            Dim result As String
'            result = Me.FetchBatch(dbName, idListStr, formatName, styleName)
'            Console.WriteLine(result)
'            PrintDebugMessage("PrintFetchBatch", "End", 1)
'        End Sub

'        ' Print a list of strings
'        Private Sub PrintStrList( strList As String())
'            PrintDebugMessage("PrintStrList", "Begin", 21)
'            If strList IsNot Nothing AndAlso strList.Length > 0 Then
'                For Each item As String In strList
'                    If item IsNot Nothing AndAlso item <> "" Then
'                        Console.WriteLine(item)
'                    End If
'                Next
'            End If
'            PrintDebugMessage("PrintStrList", "End", 21)
'        End Sub
'    End Class
'End Namespace
