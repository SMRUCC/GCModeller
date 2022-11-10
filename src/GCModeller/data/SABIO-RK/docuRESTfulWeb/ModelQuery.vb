#Region "Microsoft.VisualBasic::fd53e1ea4e2aeca478463442e55348f0, GCModeller\data\SABIO-RK\docuRESTfulWeb\ModelQuery.vb"

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

    '   Total Lines: 43
    '    Code Lines: 36
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.67 KB


    ' Class ModelQuery
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: cacheGuid, CreateQueryURL, parseSBML
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports sbXML = SMRUCC.genomics.Model.SBML.Level3.XmlFile(Of SMRUCC.genomics.Data.SABIORK.SBML.SBMLReaction)

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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function parseSBML(xml As String, schema As Type) As Object
        Return xml.LoadFromXml(Of sbXML)(throwEx:=False)
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
