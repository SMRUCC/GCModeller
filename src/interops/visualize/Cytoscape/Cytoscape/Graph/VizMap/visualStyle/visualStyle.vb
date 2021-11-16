#Region "Microsoft.VisualBasic::5ede5846a3baf7016c61168af8e5a9db, visualize\Cytoscape\Cytoscape\Graph\VizMap\visualStyle\visualStyle.vb"

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

    '     Class visualStyle
    ' 
    '         Properties: Edge, Name, Network, Node
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Visualization

    Public Class visualStyle

        <XmlAttribute("name")> Public Property Name As String
        <XmlElement("network")> Public Property Network As network
        <XmlElement("node")> Public Property Node As node
        <XmlElement("edge")> Public Property Edge As edge

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
End Namespace
