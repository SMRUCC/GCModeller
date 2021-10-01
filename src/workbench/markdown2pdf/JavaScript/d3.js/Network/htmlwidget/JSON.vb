#Region "Microsoft.VisualBasic::fea3898a643ed32d13ff097efde347fc, markdown2pdf\JavaScript\d3.js\Network\htmlwidget\JSON.vb"

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

    '     Class JSON
    ' 
    '         Properties: evals, jsHooks, x
    ' 
    '     Class NetGraph
    ' 
    '         Properties: links, nodes, options
    ' 
    '     Class Options
    ' 
    '         Properties: arrows, bounded, charge, clickAction, clickTextSize
    '                     colourScale, fontFamily, fontSize, Group, legend
    '                     linkDistance, linkWidth, NodeID, nodesize, opacity
    '                     opacityNoHover, radiusCalculation, zoom
    ' 
    '     Class Links
    ' 
    '         Properties: colour, source, target
    ' 
    '     Class Nodes
    ' 
    '         Properties: group, name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Network.htmlwidget

    Public Class JSON
        Public Property x As NetGraph
        Public Property evals As String()
        Public Property jsHooks As String()
    End Class

    Public Class NetGraph
        Public Property links As Links
        Public Property nodes As Nodes
        Public Property options As Options
    End Class

    Public Class Options
        Public Property NodeID As String
        Public Property Group As String
        Public Property colourScale As String
        Public Property fontSize As Integer
        Public Property fontFamily As String
        Public Property clickTextSize As Integer
        Public Property linkDistance As Integer
        Public Property linkWidth As String
        Public Property charge As Integer
        Public Property opacity As Double
        Public Property zoom As Boolean
        Public Property legend As Boolean
        Public Property arrows As Boolean
        Public Property nodesize As Boolean
        Public Property radiusCalculation As String
        Public Property bounded As Boolean
        Public Property opacityNoHover As Double
        Public Property clickAction As String
    End Class

    Public Class Links
        Public Property source As Integer()
        Public Property target As Integer()
        Public Property colour As String()
    End Class

    Public Class Nodes
        Public Property name As String()
        Public Property group As String()
    End Class
End Namespace
