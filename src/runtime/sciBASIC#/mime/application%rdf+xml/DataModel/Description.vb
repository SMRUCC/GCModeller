﻿#Region "Microsoft.VisualBasic::194870d454153b4e2c6e2490c39ae3b1, mime\application%rdf+xml\DataModel\Description.vb"

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

    ' Class Description
    ' 
    '     Properties: about
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

<XmlType("Description", [Namespace]:=RDF.Namespace)>
Public Class Description

    <XmlNamespaceDeclarations()>
    Public xmlns As XmlSerializerNamespaces

    Sub New()
        xmlns.Add("rdf", RDF.Namespace)
    End Sub

    <XmlAttribute("about", [Namespace]:=RDF.Namespace)>
    Public Property about As String
End Class
