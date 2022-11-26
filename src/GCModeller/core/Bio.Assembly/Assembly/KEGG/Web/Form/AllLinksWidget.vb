#Region "Microsoft.VisualBasic::0fab4d9fba7f2c85f53ee2c91b9fe16d, GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Form\AllLinksWidget.vb"

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

    '   Total Lines: 54
    '    Code Lines: 43
    ' Comment Lines: 1
    '   Blank Lines: 10
    '     File Size: 2.18 KB


    '     Class AllLinksWidget
    ' 
    '         Properties: Links
    ' 
    '         Function: InternalParser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.WebServices.InternalWebFormParsers

    Public Class AllLinksWidget

        Public Property Links As KeyValuePair()

        ' http://www.genome.jp/dbget-bin/get_linkdb?-t+8+path:map00010

        Default Public ReadOnly Property Url(ItemKey As String) As String
            Get
                Dim LQuery As String = LinqAPI.DefaultFirst(Of String) <=
 _
                    From lnkValue As KeyValuePair
                    In Links
                    Where String.Equals(lnkValue.Key, ItemKey)
                    Select lnkValue.Value

                Return LQuery
            End Get
        End Property

        Public Shared Function InternalParser(html As String) As AllLinksWidget
            Dim Links As AllLinksWidget = New AllLinksWidget
            html = Regex.Match(html, "All links.+</pre>", RegexOptions.Singleline).Value
            Dim sbuf As String() = Regex.Matches(html, "<a href="".+?"">.+?</a>").ToArray

            Links.Links =
                LinqAPI.Exec(Of KeyValuePair) <= From s As String
                                                 In sbuf
                                                 Let url As String = "http://www.genome.jp" & s.href
                                                 Let Key As String = s.GetValue
                                                 Select New KeyValuePair With {
                                                     .Key = Regex.Replace(Key, "\(.+?\)", "").Trim,
                                                     .Value = url
                                                 }
            Return Links
        End Function

        Public Overrides Function ToString() As String
            Dim links As String() = LinqAPI.Exec(Of String) <=
 _
                From m As KeyValuePair
                In Me.Links
                Select ss = m.ToString

            Return String.Join("; ", links)
        End Function
    End Class
End Namespace
