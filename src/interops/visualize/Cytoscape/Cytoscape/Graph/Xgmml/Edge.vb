#Region "Microsoft.VisualBasic::11f88f3ff0d00896e957995291056ccb, ..\interops\visualize\Cytoscape\Cytoscape\Graph\Xgmml\Edge.vb"

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

Namespace CytoscapeGraphView.XGMML

    <XmlType("edge")>
    Public Class Edge : Inherits AttributeDictionary
        Implements IAddressOf

        <XmlAttribute("id")> Public Property Id As Integer Implements IAddressOf.Address
        <XmlAttribute("label")> Public Property Label As String
        <XmlElement("graphics")> Public Property Graphics As EdgeGraphics
        <XmlAttribute("source")> Public Property source As Long
        <XmlAttribute("target")> Public Property target As Long

        Public Function ContainsNode(id As Long) As Boolean
            Return source = id OrElse target = id
        End Function

        Public Function ContainsOneOfNode(Id As Integer()) As Boolean
            For Each handle In Id
                If source = handle OrElse target = handle Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} ""{1}""", Id, Label)
        End Function

        ''' <summary>
        ''' 应用于节点的去重
        ''' </summary>
        ''' <returns></returns>
        Protected Friend ReadOnly Property __internalUID As Long
            Get
                Dim dt = {source, target}
                Return dt.Max * 1000000 + dt.Min
            End Get
        End Property
    End Class
End Namespace
