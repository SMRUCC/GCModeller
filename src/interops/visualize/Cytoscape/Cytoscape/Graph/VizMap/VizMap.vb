#Region "Microsoft.VisualBasic::ab34df2ca4411d4947937e0be6bed1ae, ..\interops\visualize\Cytoscape\Cytoscape\Graph\VizMap\VizMap.vb"

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

Imports System.Xml.Serialization
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Cytoscape.Visualization

Namespace Visualization

    Public Class VizMap

        <XmlAttribute("documentVersion")> Public Property documentVersion As String = "3.0"
        <XmlAttribute("id")> Public Property id As String '= "VizMap-2015_07_25-05_45"
        <XmlElement("visualStyle")> Public Property visualStyle As visualStyle()

    End Class
End Namespace
