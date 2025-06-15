﻿#Region "Microsoft.VisualBasic::3bce7ac0f2a44427fe6d5493f7f91d7b, metadb\Massbank\Public\NCBI\PubMed.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 77
'    Code Lines: 47 (61.04%)
' Comment Lines: 18 (23.38%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 12 (15.58%)
'     File Size: 2.57 KB


'     Class PubMed
' 
'         Properties: annotation, articleabstract, articleaffil, articleauth, articlejourname
'                     articlepubdate, articletitle, articletype, cids, citation
'                     doi, meshcodes, meshheadings, meshsubheadings, pmid
'                     pmidsrcs, sids
' 
'         Function: GetPublishDate, ToString, TryParseInternal
' 
' 
' /********************************************************************************/

#End Region

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
