#Region "Microsoft.VisualBasic::67a2bfab8facd50e1577ee11c2a526c2, engine\IO\GCTabular\DataVisualization\Net\NetModel.vb"

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

    '     Class NetModel
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic

Namespace DataVisualization

    Public Class NetModel : Inherits Network(Of NodeAttributes, Interactions)

        Sub New(nodes As IEnumerable(Of NodeAttributes), edges As IEnumerable(Of Interactions))
            Me.Nodes = nodes.ToArray
            Me.Edges = edges.ToArray
        End Sub

        Sub New(edges As IEnumerable(Of Interactions), nodes As IEnumerable(Of NodeAttributes))
            Me.Nodes = nodes.ToArray
            Me.Edges = edges.ToArray
        End Sub
    End Class
End Namespace
