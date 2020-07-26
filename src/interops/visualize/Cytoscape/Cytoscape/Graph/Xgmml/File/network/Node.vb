#Region "Microsoft.VisualBasic::c1e193b93cd74b6da503cbbb9f00189a, visualize\Cytoscape\Cytoscape\Graph\Xgmml\File\network\Node.vb"

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

    '     Class XGMMLnode
    ' 
    '         Properties: graphics, id, label, location
    ' 
    '         Function: ToString
    ' 
    '         Sub: Assign
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel

Namespace CytoscapeGraphView.XGMML.File

    <XmlType("node")>
    Public Class XGMMLnode : Inherits AttributeDictionary
        Implements IAddressOf

        ''' <summary>
        ''' 当前的这个节点在整个网络的节点列表之中的位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property id As Integer Implements IAddressOf.Address
        <XmlAttribute> Public Property label As String
        <XmlElement("graphics")> Public Property graphics As NodeGraphics

        Public ReadOnly Property location As Point
            Get
                Return New Point(graphics.x, graphics.y)
            End Get
        End Property

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Me.id = address
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{id}] {label}: [{graphics.x}, {graphics.y}]"
        End Function
    End Class
End Namespace
