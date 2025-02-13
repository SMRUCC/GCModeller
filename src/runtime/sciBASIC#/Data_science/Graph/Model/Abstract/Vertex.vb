﻿#Region "Microsoft.VisualBasic::81dd8752711e1d55962cafb025dfe637, Data_science\Graph\Model\Abstract\Vertex.vb"

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


    ' Code Statistics:

    '   Total Lines: 65
    '    Code Lines: 34 (52.31%)
    ' Comment Lines: 22 (33.85%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (13.85%)
    '     File Size: 1.84 KB


    ' Class Vertex
    ' 
    '     Properties: ID, label
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: Equals, ToString
    ' 
    '     Sub: Assign
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

''' <summary>
''' Vertex in a graph
''' </summary>
''' <remarks>
''' 图之中的节点
''' </remarks>
<DataContract>
Public Class Vertex : Implements INamedValue
    Implements IAddressOf

    ''' <summary>
    ''' The unique id of this node
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property label As String Implements IKeyedEntity(Of String).Key
    ''' <summary>
    ''' Array index.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' (使用数字表示的唯一标识符)
    ''' </remarks>
    <XmlAttribute> Public Property ID As Integer Implements IAddress(Of Integer).Address

    Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(id As Integer)
        Me.ID = id
    End Sub

    Sub New(id As Integer, label As String)
        Me.ID = id
        Me.label = label
    End Sub

    Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
        ID = address
    End Sub

    ''' <summary>
    ''' just test equals by the <see cref="ID"/> is the same or not.
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    Public Overrides Function Equals(obj As Object) As Boolean
        If TypeOf obj IsNot Vertex Then
            Return False
        End If

        Return DirectCast(obj, Vertex).ID = ID
    End Function

    Public Overrides Function ToString() As String
        Return $"({ID}) {label}"
    End Function
End Class
