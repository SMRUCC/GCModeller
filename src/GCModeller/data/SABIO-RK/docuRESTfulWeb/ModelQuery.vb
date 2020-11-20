﻿#Region "Microsoft.VisualBasic::fd53e1ea4e2aeca478463442e55348f0, data\SABIO-RK\docuRESTfulWeb\ModelQuery.vb"

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

    ' Class ModelQuery
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: cacheGuid, CreateQueryURL, parseSBML
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports sbXML = SMRUCC.genomics.Model.SBML.Level3.XmlFile(Of SMRUCC.genomics.Data.SABIORK.SBML.SBMLReaction)

Public Class ModelQuery : Inherits WebQuery(Of Dictionary(Of QueryFields, String))

    Public Sub New(<CallerMemberName>
                   Optional cache As String = Nothing,
                   Optional interval As Integer = -1,
                   Optional offline As Boolean = False)

        MyBase.New(
            url:=AddressOf CreateQueryURL,
            contextGuid:=AddressOf cacheGuid,
            parser:=AddressOf parseSBML,
            prefix:=Function(guid) guid.First,
            cache:=cache,
            interval:=interval,
            offline:=offline
        )
    End Sub

    Private Shared Function cacheGuid(q As Dictionary(Of QueryFields, String)) As String
        Return q.GetJson.MD5
    End Function

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

    Public Shared Function parseSBML(xml As String, schema As Type) As Object
        Return xml.LoadFromXml(Of sbXML)(throwEx:=False)
    End Function
End Class
