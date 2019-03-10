#Region "Microsoft.VisualBasic::605e2afba41d12453ab72ebb1bfb7aa6, visualize\Cytoscape\Cytoscape\Graph\VizMap\VizMap.vb"

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

    '     Class VizMap
    ' 
    '         Properties: documentVersion, id, visualStyle
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Visualize.Cytoscape.Visualization

Namespace Visualization

    ''' <summary>
    ''' visual styles xml
    ''' </summary>
    Public Class VizMap

        <XmlAttribute("documentVersion")> Public Property documentVersion As String = "3.0"
        <XmlAttribute("id")> Public Property id As String '= "VizMap-2015_07_25-05_45"
        <XmlElement("visualStyle")> Public Property visualStyle As visualStyle()

    End Class
End Namespace
