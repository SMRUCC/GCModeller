﻿#Region "Microsoft.VisualBasic::7233f25c97db46da85daab23f74c454a, data\SABIO-RK\docuRESTfulWeb\ModelQuery.vb"

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


' Code Statistics:

'   Total Lines: 65
'    Code Lines: 53 (81.54%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 12 (18.46%)
'     File Size: 2.57 KB


' Class ModelQuery
' 
'     Constructor: (+2 Overloads) Sub New
'     Function: contextPrefix, CreateQueryURL, doParseGuid, doParseObject, doParseUrl
'               parseSBML
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.Model.SBML.Level3

Public Class ModelQuery : Inherits WebQueryModule(Of Dictionary(Of QueryFields, String))

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub New(<CallerMemberName>
                   Optional cache As String = Nothing,
                   Optional interval As Integer = -1,
                   Optional offline As Boolean = False)

        Call MyBase.New(cache, interval, offline)
    End Sub

    Public Sub New(cache As IFileSystemEnvironment,
                   Optional interval As Integer = -1,
                   Optional offline As Boolean = False)

        Call MyBase.New(cache, interval, offline)
    End Sub

    Public Shared Function CreateQueryURL(q As Dictionary(Of QueryFields, String)) As String
        Dim searches As String() = q _
            .Select(Function(t)
                        Return $"{t.Key.Description}:""{t.Value}"""
                    End Function) _
            .ToArray
        Dim query As String = searches.JoinBy(" AND ").UrlEncode
        Dim url As String = $"http://sabiork.h-its.org/sabioRestWebServices/searchKineticLaws/sbml?q={query}"

        Return url
    End Function

    Protected Overrides Function isEmptyContent(cache_path As String) As Boolean
        Dim str = Strings.Trim(cache.ReadAllText(cache_path))

        If str.StringEmpty(, True) Then
            Return True
        ElseIf str = "No results found for query" Then
            Return True
        Else
            Return False
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function parseSBML(xml As String, Optional schema As Type = Nothing) As Object
        If schema Is GetType(XmlFile(Of SBMLReaction)) Then
            Return SbmlDocument.LoadXml(xml)
        Else
            Return SbmlDocument.LoadDocument(xml)
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Protected Overrides Function doParseUrl(context As Dictionary(Of QueryFields, String)) As String
        Return CreateQueryURL(q:=context)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Protected Overrides Function doParseObject(html As String, schema As Type) As Object
        Return parseSBML(html, schema)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Protected Overrides Function doParseGuid(context As Dictionary(Of QueryFields, String)) As String
        Return context.GetJson.MD5
    End Function

    Protected Overrides Function contextPrefix(guid As String) As String
        If TypeOf cache Is Directory Then
            Return guid.Substring(1, 2)
        Else
            Return $"/.cache/{guid.Substring(1, 2)}"
        End If
    End Function
End Class
