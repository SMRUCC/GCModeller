#Region "Microsoft.VisualBasic::61046dc3189af3d36cbf8d174cb3d3cb, visualize\Cytoscape\Cytoscape\Graph\VizMap\visualStyle\network.vb"

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

    '     Class network
    ' 
    ' 
    ' 
    '     Class visualNode
    ' 
    '         Properties: dependency, visualPropertys
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Visualize.Cytoscape.Visualization.visualProperty

Namespace Visualization

    Public Class network : Inherits visualNode
    End Class

    Public MustInherit Class visualNode
        <XmlElement("visualProperty")> Public Property visualPropertys As visualProperty.visualProperty()
        <XmlElement("dependency")> Public Property dependency As dependency()
    End Class
End Namespace
