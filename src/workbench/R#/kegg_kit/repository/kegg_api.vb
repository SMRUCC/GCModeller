#Region "Microsoft.VisualBasic::62f8f4d48674ea68ed09330f19372447, R#\kegg_kit\repository\kegg_api.vb"

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

    '   Total Lines: 41
    '    Code Lines: 32
    ' Comment Lines: 3
    '   Blank Lines: 6
    '     File Size: 1.57 KB


    ' Module kegg_api
    ' 
    '     Function: [get], convertToPathway, listing, parseWebForm
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.Data

''' <summary>
''' KEGG API is a REST-style Application Programming Interface to the KEGG database resource.
''' </summary>
<Package("kegg_api")>
Public Module kegg_api

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("listing")>
    Public Function listing(database As String,
                            Optional [option] As String = Nothing,
                            Optional cache As IHttpGet = Nothing) As Object

        Return New KEGGApi(proxy:=cache).List(database, [option])
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("get")>
    Public Function [get](id As String, Optional cache As IHttpGet = Nothing) As String
        Return New KEGGApi(proxy:=cache).GetObject(id)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("parseForm")>
    Public Function parseWebForm(text As String) As WebForm
        Return New WebForm(text)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("as.pathway")>
    Public Function convertToPathway(form As WebForm) As Pathway
        Return PathwayTextParser.ParsePathway(form)
    End Function
End Module

