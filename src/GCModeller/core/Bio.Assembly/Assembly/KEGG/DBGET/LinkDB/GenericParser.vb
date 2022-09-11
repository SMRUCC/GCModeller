#Region "Microsoft.VisualBasic::d8cf0a08c26978f73bbc513f1be06835, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\GenericParser.vb"

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

    '   Total Lines: 78
    '    Code Lines: 60
    ' Comment Lines: 3
    '   Blank Lines: 15
    '     File Size: 3.04 KB


    '     Class GenericParser
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: argumentPrefix, (+2 Overloads) LinkDbEntries, ParsePage, queryArguments
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.LinkDB

    ''' <summary>
    ''' 通用化的linkdb数据解析器
    ''' </summary>
    Public Class GenericParser : Inherits WebQuery(Of String)

        Const regexpLine$ = "<a href="".+?"">.+?</a>.+?$"

        Public Const LinkDbCache$ = "./.kegg/linkdb/"

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            MyBase.New(url:=Function(strUrl) strUrl,
                       contextGuid:=AddressOf queryArguments,
                       parser:=AddressOf ParsePage,
                       prefix:=AddressOf argumentPrefix,
                       cache:=cache,
                       interval:=interval,
                       offline:=offline
                   )
        End Sub

        Public Shared Function LinkDbEntries(url$, Optional cache$ = LinkDbCache, Optional offline As Boolean = False) As NamedValue()
            Static handlers As New Dictionary(Of String, GenericParser)

            Dim query As GenericParser = handlers.ComputeIfAbsent(
                key:=cache,
                lazyValue:=Function()
                               Return New GenericParser(cache,, offline)
                           End Function)

            Return query.Query(Of NamedValue())(url, ".html")
        End Function

        Private Shared Function queryArguments(url As String) As String
            Return url.GetTagValue("?", trim:=True) _
                .Value _
                .NormalizePathString(alphabetOnly:=False) _
                .Replace("+", "/")
        End Function

        Private Shared Function argumentPrefix(arg As String) As String
            Return Nothing
        End Function

        Private Shared Function ParsePage(html$, null As Type) As Object
            Return LinkDbEntries(Strings.Split(html, Modules.SEPERATOR).Last).ToArray
        End Function

        Private Shared Iterator Function LinkDbEntries(html As String) As IEnumerable(Of NamedValue)
            Dim links$() = Regex _
                .Matches(html, regexpLine, RegexICMul) _
                .ToArray

            For Each line As String In links.Take(links.Length - 1)
                Dim entry As String = Regex.Match(line, ">.+?</a>").Value.GetValue
                Dim description As String = Strings.Split(line, "</a>").Last.Trim
                Dim out As New NamedValue With {
                    .name = entry,
                    .text = description
                }

                Yield out
            Next
        End Function
    End Class
End Namespace
