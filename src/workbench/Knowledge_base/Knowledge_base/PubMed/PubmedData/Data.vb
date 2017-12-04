#Region "Microsoft.VisualBasic::c94affdca7909ad3c51749ca74c9230e, ..\workbench\Knowledge_base\Knowledge_base\PubMed\PubmedData\Data.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Xml.Serialization

Public Class History

    <XmlElement("PubMedPubDate")> Public Property PubMedPubDate As PubDate()
End Class

Public Class ArticleId
    <XmlAttribute>
    Public Property IdType As String
    <XmlText>
    Public Property ID As String

    Public Overrides Function ToString() As String
        Return $"{IdType}: {ID}"
    End Function
End Class
