﻿#Region "Microsoft.VisualBasic::accfb6ab35932afcd21f673d69dd0c6d, mime\application%vnd.openxmlformats-officedocument.spreadsheetml.sheet\Excel\IO\xl\calcChain.xml.vb"

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

    '     Class calcChain
    ' 
    '         Properties: c
    ' 
    '     Class c
    ' 
    '         Properties: i, l, r
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace XML.xl

    <XmlType("calcChain", [Namespace]:="http://schemas.openxmlformats.org/spreadsheetml/2006/main")>
    Public Class calcChain
        <XmlElement("c")>
        Public Property c As c()
    End Class

    Public Class c
        <XmlAttribute> Public Property r As String
        <XmlAttribute> Public Property i As String
        <XmlAttribute> Public Property l As String
    End Class
End Namespace
