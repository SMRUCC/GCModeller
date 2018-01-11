#Region "Microsoft.VisualBasic::736da5a50bbc318cd74773ad4515e19e, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\GenericParser.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.LinkDB

    ''' <summary>
    ''' 通用化的linkdb数据解析器
    ''' </summary>
    Public Module GenericParser

        Const regexpLine$ = "<a href="".+?"">.+?</a>.+?$"

        <Extension>
        Public Iterator Function LinkDbEntries(url As String) As IEnumerable(Of KeyValuePair)
            Dim html As String = Strings.Split(url.GET, Modules.SEPERATOR).Last
            Dim links$() = Regex _
                .Matches(html, regexpLine, RegexOptions.Multiline + RegexOptions.IgnoreCase) _
                .ToArray

            For Each line As String In links.Take(links.Length - 1)
                Dim entry As String = Regex.Match(line, ">.+?</a>").Value.GetValue
                Dim description As String = Strings.Split(line, "</a>").Last.Trim
                Dim out As New KeyValuePair With {
                    .Key = entry,
                    .Value = description
                }

                Yield out
            Next
        End Function
    End Module
End Namespace
