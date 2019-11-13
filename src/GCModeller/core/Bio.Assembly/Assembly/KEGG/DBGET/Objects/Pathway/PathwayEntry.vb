#Region "Microsoft.VisualBasic::b6f0edcef7d64647197ca1acc4906874, core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayEntry.vb"

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

    '     Class PathwayEntry
    ' 
    '         Properties: [Object], Description, Entry, Legend, Name
    '                     Url
    ' 
    '         Function: __parserEntry, ToString, TryParseWebPage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class PathwayEntry

        Public Property Entry As String
        Public Property Name As String
        Public Property Description As String
        Public Property [Object] As String
        Public Property Legend As String
        Public Property Url As String

        Public Const ENTRY_ITEM As String = "<a target="".+?"" href=.+?</tr>"

        Public Overrides Function ToString() As String
            Return String.Format("{0}:  {1}", Entry, Description)
        End Function

        Public Shared Function TryParseWebPage(url As String) As PathwayEntry()
            Dim html As String = url.GET
            Dim sbuf As String() = Regex.Matches(html, ENTRY_ITEM, RegexICSng).ToArray
            Dim result As PathwayEntry() = sbuf.Select(AddressOf __parserEntry).ToArray

            Return result
        End Function

        Private Shared Function __parserEntry(s As String) As PathwayEntry
            Dim entry As New PathwayEntry
            Dim sbuf As String() = Strings.Split(s, vbLf)
            entry.Entry = sbuf.First.GetValue
            entry.Url = sbuf.First.href
            sbuf = sbuf.Skip(3).ToArray

            Dim i As i32 = Scan0
            entry.Name = sbuf(++i).GetValue
            entry.Description = sbuf(++i).GetValue
            entry.Object = sbuf(++i).GetValue
            entry.Legend = sbuf(++i).GetValue

            Return entry
        End Function
    End Class
End Namespace
