#Region "Microsoft.VisualBasic::f8ed8df7a1a10bc35fad2f9338edb11d, RssChannel\RSSReader.vb"

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

    ' Class RSSReader
    ' 
    '     Properties: channel, version
    ' 
    ' Class Channel
    ' 
    '     Properties: description, image, items, language, lastBuildDate
    '                 link, title
    ' 
    ' Class Image
    ' 
    '     Properties: height, link, title, url, width
    ' 
    ' Class Guid
    ' 
    '     Properties: isPermaLink, link
    ' 
    ' Class Item
    ' 
    '     Properties: category, comments, content, dc_creator, description
    '                 guid, link, pubDate, slash_comments, title
    '                 wfw_commentRss
    ' 
    ' Class Category
    ' 
    '     Properties: value
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

<XmlType("rss")>
Public Class RSSReader

    Public Property version As String
    Public Property channel As Channel

End Class

Public Class Channel
    Public Property title As String
    Public Property link As String
    Public Property description As String
    Public Property lastBuildDate As String
    Public Property language As String
    Public Property image As Image
    <XmlElement("item")> Public Property items As Item()
End Class

Public Class Image
    Public Property url As String
    Public Property title As String
    Public Property link As String
    Public Property width As Integer
    Public Property height As Integer
End Class

Public Class Guid
    <XmlAttribute> Public Property isPermaLink As Boolean
    <XmlText> Public Property link As String
End Class

Public Class Item
    Public Property title As String
    Public Property link As String
    Public Property comments As String
    Public Property pubDate As String
    Public Property dc_creator As String
    <XmlElement> Public Property category As Category()
    Public Property guid As Guid
    Public Property description As String
    Public Property content As String
    Public Property wfw_commentRss As String
    Public Property slash_comments As Integer
End Class

Public Class Category
    <XmlText> Public Property value As String
End Class
