#Region "Microsoft.VisualBasic::173f3f3e041731ac6c0a6f531632ed65, ..\Bio.Assembly\Assembly\KEGG\AllLinksWidget.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
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

Imports System.Text.RegularExpressions
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.WebServices.InternalWebFormParsers

    Public Class AllLinksWidget

        Public Property Links As KeyValuePair()

        ' http://www.genome.jp/dbget-bin/get_linkdb?-t+8+path:map00010

        Default Public ReadOnly Property Url(ItemKey As String) As String
            Get
                Dim LQuery As String =
                    LinqAPI.DefaultFirst(Of String) <= From lnkValue As KeyValuePair
                                                       In Links
                                                       Where String.Equals(lnkValue.Key, ItemKey)
                                                       Select lnkValue.Value
                Return LQuery
            End Get
        End Property

        Public Shared Function InternalParser(html As String) As AllLinksWidget
            Dim Links As AllLinksWidget = New AllLinksWidget
            html = Regex.Match(html, "All links.+</pre>", RegexOptions.Singleline).Value
            Dim sbuf As String() = (From m As Match In Regex.Matches(html, "<a href="".+?"">.+?</a>") Select m.Value).ToArray

            Links.Links =
                LinqAPI.Exec(Of KeyValuePair) <= From s As String
                                                 In sbuf
                                                 Let url As String = "http://www.genome.jp" & s.Get_href
                                                 Let Key As String = s.GetValue
                                                 Select New KeyValuePair With {
                                                     .Key = Regex.Replace(Key, "\(.+?\)", "").Trim,
                                                     .Value = url
                                                 }
            Return Links
        End Function

        Public Overrides Function ToString() As String
            Return String.Join("; ", (From m As KeyValuePair
                                      In Links
                                      Select ss = m.ToString).ToArray)
        End Function
    End Class
End Namespace
