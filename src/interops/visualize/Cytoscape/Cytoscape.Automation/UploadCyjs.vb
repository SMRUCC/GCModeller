#Region "Microsoft.VisualBasic::c853cb0d31cd390349330671cab432bd, visualize\Cytoscape\Cytoscape.Automation\UploadCyjs.vb"

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

    '     Class CyjsUpload
    ' 
    '         Properties: data, elements
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class networkElement
    ' 
    '         Properties: edges, nodes
    ' 
    '     Class cyjsNode
    ' 
    '         Properties: data
    ' 
    '     Class cyjsedge
    ' 
    '         Properties: data
    ' 
    '     Class edgeData2
    ' 
    '         Properties: interaction, source, target
    ' 
    '     Class nodeData2
    ' 
    '         Properties: common, id
    ' 
    '     Class cyjsdata
    ' 
    '         Properties: name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Cyjs

Namespace Upload

    Public Class CyjsUpload

        Public Property data As cyjsdata
        Public Property elements As networkElement

        Sub New(cyjs As Cyjs, title As String)
            data = New cyjsdata With {
                .name = If(title, App.NextTempName)
            }
            elements = New networkElement With {
                .edges = cyjs.elements.edges.Select(Function(a) New cyjsedge With {.data = New edgeData2 With {.interaction = a.data.interaction, .source = a.data.source, .target = a.data.target}}).ToArray,
                .nodes = cyjs.elements.nodes.Select(Function(a) New cyjsNode With {.data = New nodeData2 With {.common = a.data.common, .id = a.data.id}}).ToArray
            }
        End Sub

    End Class

    Public Class networkElement
        Public Property nodes As cyjsNode()
        Public Property edges As cyjsedge()
    End Class

    Public Class cyjsNode
        Public Property data As nodeData2
    End Class

    Public Class cyjsedge
        Public Property data As edgeData2
    End Class

    Public Class edgeData2
        Public Property source As String
        Public Property target As String
        Public Property interaction As String
    End Class

    Public Class nodeData2
        Public Property id As String
        Public Property common As String
    End Class

    Public Class cyjsdata
        Public Property name As String = App.NextTempName
    End Class
End Namespace

