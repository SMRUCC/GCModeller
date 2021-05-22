﻿#Region "Microsoft.VisualBasic::ea5384aa0f61077d650fedac55a501b2, Data\Trinity\Test\graphQueryTest.vb"

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

    ' Module graphQueryTest
    ' 
    '     Sub: BookTest, complextest, Main, simpleArrayTest, SimpleTest
    ' 
    ' Class QueryData
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphQuery
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.MIME.Html.Document

Module graphQueryTest

    Sub Main()
        Call simpleParserTest()
        Call complextest()

        Call simpleArrayTest()

        Call SimpleTest()

        Call BookTest()

        Pause()

    End Sub

    Sub simpleParserTest()

        Call QueryParser.GetQuery("reaction css('a', '*') [text()]")

    End Sub

    Sub complextest()
        Dim queryText As String = "E:\GCModeller\src\repository\graphquery\kegg\kegg_table.graphquery".ReadAllText
        Dim query As Query = QueryParser.GetQuery(queryText)
        Dim engine As New Engine
        Dim doc As HtmlDocument = HtmlDocument.LoadDocument("E:\GCModeller\src\repository\graphquery\kegg\gene.html")
        Dim data As JsonElement = engine.Execute(doc, query)

        Call Console.WriteLine(data.BuildJsonString(New JSONSerializerOptions With {.indent = True}))

        Pause()
    End Sub

    Sub BookTest()
        ' Dim queryTokens = New TokenIcer("E:\GCModeller\src\runtime\sciBASIC#\Data\data\query.ql".ReadAllText).GetTokens.ToArray
        Dim queryText As String = "E:\GCModeller\src\runtime\sciBASIC#\Data\data\query.ql".ReadAllText
        Dim query As Query = QueryParser.GetQuery(queryText)
        Dim engine As New Engine
        Dim doc As HtmlDocument = HtmlDocument.LoadDocument("E:\GCModeller\src\runtime\sciBASIC#\Data\data\query.html")
        Dim data As JsonElement = engine.Execute(doc, query)

        Call Console.WriteLine(data.BuildJsonString(New JSONSerializerOptions With {.indent = True}))
    End Sub

    Sub SimpleTest()

        Dim document = <div>
                           <a href="1.html">anchor 1</a>
                           <a href="2.html">anchor 2</a>
                           <a href="3.html">anchor 3</a>
                       </div>
        Dim query As Query = QueryParser.GetQuery("

            a css('a') [{
                title  text() | trim() 
                url    attr('href') 
            }]

        ")

        Dim data As JsonElement = New Engine().Execute(document, query)
        Dim json As String = data.BuildJsonString(indent:=True)

        Call Console.WriteLine(json)

        '  Pause()
    End Sub

    Sub simpleArrayTest()

        Dim document = <html>

                           <body>
                               <a href="01.html">Page 1</a>
                               <a href="02.html">Page 2</a>
                               <a href="03.html">Page 3</a>
                           </body>

                       </html>

        Dim query As Query = QueryParser.GetQuery("
            
            graphquery { 
            
                anchor css('a') [ 
                    text() 
                ] 
            }

        ")

        Dim data As JsonElement = New Engine().Execute(document, query)
        Dim json As String = data.BuildJsonString()

        Call Console.WriteLine(json)

        '  Pause()
    End Sub

End Module


Public Class QueryData


End Class
