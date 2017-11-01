#Region "Microsoft.VisualBasic::c278d04fdfc0a322ed97f133ad9d5f77, ..\interops\visualize\Cytoscape\Cytoscape\Graph\Xgmml\Node.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq

Namespace CytoscapeGraphView.XGMML

    <XmlType("node")>
    Public Class Node : Inherits AttributeDictionary
        Implements IAddressOf

        ''' <summary>
        ''' 当前的这个节点在整个网络的节点列表之中的位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property id As Integer Implements IAddressOf.Address
        <XmlAttribute> Public Property label As String
        <XmlElement("graphics")> Public Property Graphics As NodeGraphics

        Public ReadOnly Property Location As Point
            Get
                Return New Point(Graphics.x, Graphics.y)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim array As String() = Attributes.Select(AddressOf Scripting.ToString)
            Return String.Format("{0} ""{1}""  ==> {2}", id, label, String.Join("; ", array))
        End Function
    End Class
End Namespace
