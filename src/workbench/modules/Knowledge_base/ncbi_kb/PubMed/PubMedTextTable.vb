#Region "Microsoft.VisualBasic::d524f1c491919d2b4686fb60e6101b2c, modules\Knowledge_base\ncbi_kb\PubMed\PubMedTextTable.vb"

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

    '   Total Lines: 111
    '    Code Lines: 66 (59.46%)
    ' Comment Lines: 29 (26.13%)
    '    - Xml Docs: 96.55%
    ' 
    '   Blank Lines: 16 (14.41%)
    '     File Size: 3.99 KB


    '     Class PubMedTextTable
    ' 
    '         Properties: annotation, articleabstract, articleaffil, articleauth, articlejourname
    '                     articlepubdate, articletitle, articletype, cids, citation
    '                     doi, meshcodes, meshheadings, meshsubheadings, pmid
    '                     pmidsrcs, sids
    ' 
    '         Function: GetPublishDate, LoadTable, ParseJSON, ToString, TryParseInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework.IO.Linq
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Namespace PubMed

    ''' <summary>
    ''' A publication record in pubmed database, export from pubchem database export in csv/json list format
    ''' </summary>
    ''' <remarks>
    ''' dump data from pubchem data that make associates from pubchem and pubmed
    ''' </remarks>
    Public Class PubMedTextTable

        ''' <summary>
        ''' the article pubmed id
        ''' </summary>
        ''' <returns></returns>
        Public Property pmid As Long
        Public Property articlepubdate As Integer
        Public Property articletype As String
        <Collection("pmidsrcs", "|")>
        Public Property pmidsrcs As String()

        ''' <summary>
        ''' keywords about this article
        ''' </summary>
        ''' <returns></returns>
        <Collection("meshheadings", "|")>
        Public Property meshheadings As String()
        <Collection("meshsubheadings", "|")>
        Public Property meshsubheadings As String()
        <Collection("meshcodes", "|")>
        Public Property meshcodes As String()

        ''' <summary>
        ''' the related compound list about this article
        ''' </summary>
        ''' <returns></returns>
        <Collection("cids", "|")>
        Public Property cids As String()
        Public Property sids As String
        Public Property articletitle As String
        Public Property articleabstract As String
        Public Property articlejourname As String

        ''' <summary>
        ''' authors names
        ''' </summary>
        ''' <returns></returns>
        Public Property articleauth As String
        Public Property articleaffil As String
        Public Property citation As String
        Public Property doi As String
        Public Property annotation As String

        Public Function GetPublishDate() As Date
            Try
                Return TryParseInternal(articlepubdate.ToString)
            Catch ex As Exception
                Return New Date(2010, 1, 1)
            End Try
        End Function

        Private Shared Function TryParseInternal(str As String) As Date
            If str.Length <> 8 Then
                str = str.PadEnd(8, "0"c)
            End If

            Dim yyyy = str.Substring(0, 4)
            Dim mm = str.Substring(4, 2)
            Dim dd = str.Substring(6, 2)

            If mm = "00" Then mm = "01"
            If dd = "00" Then dd = "01"

            Return New Date(Integer.Parse(yyyy), Integer.Parse(mm), Integer.Parse(dd))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadTable(file As String) As IEnumerable(Of PubMedTextTable)
            Return DataStream.OpenHandle(file, trim:=True).AsLinq(Of PubMedTextTable)(silent:=True)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="json">
        ''' could be a json file its file path or the json text content data.
        ''' </param>
        ''' <returns></returns>
        Public Shared Function ParseJSON(json As String) As PubMedTextTable()
            Dim list As JsonArray = JSONTextParser.ParseJson(json.SolveStream, False)
            Dim articles As PubMedTextTable() = list _
                .AsObjects _
                .Select(Function(j)
                            Return j.CreateObject(Of PubMedTextTable)(True)
                        End Function) _
                .ToArray

            Return articles
        End Function

        Public Overrides Function ToString() As String
            Return articletitle
        End Function

    End Class
End Namespace
