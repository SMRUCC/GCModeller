﻿#Region "Microsoft.VisualBasic::f74b5fd18bed40123585d64443dfb57d, mime\text%html\Document\HtmlDocument.vb"

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

    '   Total Lines: 40
    '    Code Lines: 21 (52.50%)
    ' Comment Lines: 12 (30.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (17.50%)
    '     File Size: 1.45 KB


    '     Class HtmlDocument
    ' 
    '         Function: LoadDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.MIME.Html.Language
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace Document

    ''' <summary>
    ''' A root document that is a kind of subclass of <see cref="HtmlElement"/>
    ''' </summary>
    Public Class HtmlDocument : Inherits HtmlElement

        ''' <summary>
        ''' 假设所加载的html文档是完好的格式的，即没有不匹配的标签的
        ''' </summary>
        ''' <param name="handle">document text or url or file path</param>
        ''' <param name="strip">
        ''' do html document text cleanup at first? includes removes javascript block, 
        ''' css block and html comments. 
        ''' </param>
        ''' <returns></returns>
        Public Shared Function LoadDocument(handle As String, Optional strip As Boolean = False) As HtmlDocument
            Dim text As String = handle.SolveStream
            Dim htmlInput As String = text

            If strip Then
                htmlInput = htmlInput _
                    .RemovesJavaScript _
                    .RemovesCSSstyles

                htmlInput = New StringBuilder(htmlInput) _
                    .RemovesHtmlComments _
                    .ToString
            End If

            Dim document As HtmlDocument = HtmlParser.ParseTree(document:=htmlInput)

            Return document
        End Function
    End Class
End Namespace
