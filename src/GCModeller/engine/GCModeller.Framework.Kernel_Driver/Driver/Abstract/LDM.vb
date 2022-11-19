#Region "Microsoft.VisualBasic::158e2041d88b46b37b655e0e35630e74, GCModeller\engine\GCModeller.Framework.Kernel_Driver\Driver\Abstract\LDM.vb"

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

    '   Total Lines: 73
    '    Code Lines: 35
    ' Comment Lines: 28
    '   Blank Lines: 10
    '     File Size: 2.83 KB


    ' Class Expression
    ' 
    '     Properties: Handle, Identifier, Value
    ' 
    '     Function: get_ObjectHandle
    ' 
    '     Sub: Assign
    ' 
    ' Class Variable
    ' 
    '     Properties: Handle, UniqueId, Value
    ' 
    '     Sub: Assign
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' This object represent a dynamics expression in the calculation system.(代表一个对象的动力学模型)
''' </summary>
''' <remarks></remarks>
Public MustInherit Class Expression
    Implements IAddressOf, IDynamicsExpression(Of Double)

    Protected _value As Double

#Region "Interface"
    Public MustOverride Function Evaluate() As Double Implements IDynamicsExpression(Of Double).Evaluate

    Public Overridable Function get_ObjectHandle() As DataStorage.FileModel.ObjectHandle Implements IDynamicsExpression(Of Double).get_ObjectHandle
        Return New Framework.Kernel_Driver.DataStorage.FileModel.ObjectHandle With {
            .Handle = Handle,
            .ID = Identifier
        }
    End Function
#End Region

    Public Overridable ReadOnly Property Value As Double Implements IDynamicsExpression(Of Double).Value
        Get
            Return _value
        End Get
    End Property

    Public Property Identifier As String Implements INamedValue.Key
    Public Property Handle As Integer Implements IAddressOf.Address

    Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
        Me.Handle = address
    End Sub
End Class

''' <summary>
''' The variable represents a node instance in the network system.(变量对象代表了网络对象之中的一个实体节点)
''' </summary>
''' <remarks></remarks>
Public MustInherit Class Variable : Implements IAddressOf
    Implements INamedValue

    ''' <summary>
    ''' The location pointer of this variable node in the network system.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlAttribute> Public Property Handle As Integer Implements IAddressOf.Address
    ''' <summary>
    ''' The node states in the current network state.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlAttribute> Public Overridable Property Value As Double
    ''' <summary>
    ''' The unique id of this node entity in the network, the function of this property is as the same as the 
    ''' <see cref="Handle"></see> property to unique indicates this variable node instance in the network system.
    ''' (本属性和<see cref="Handle"></see>属性的作用是一样的，都可以唯一性的只是本节点在网络系统之中的唯一的位置)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlAttribute> Public Property UniqueId As String Implements INamedValue.Key

    Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
        Me.Handle = address
    End Sub
End Class
