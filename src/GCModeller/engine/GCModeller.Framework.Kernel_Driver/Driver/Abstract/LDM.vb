#Region "Microsoft.VisualBasic::a04df6d33e8e3a2299ad83f430b05f30, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\Driver\Abstract\LDM.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' This object represent a dynamics expression in the calculation system.(代表一个对象的动力学模型)
''' </summary>
''' <remarks></remarks>
Public MustInherit Class Expression
    Implements IAddressHandle, IDynamicsExpression(Of Double)

    Protected _value As Double

#Region "Interface"
    Public MustOverride Function Evaluate() As Double Implements IDynamicsExpression(Of Double).Evaluate

    Public Overridable Function get_ObjectHandle() As DataStorage.FileModel.ObjectHandle Implements IDynamicsExpression(Of Double).get_ObjectHandle
        Return New Framework.Kernel_Driver.DataStorage.FileModel.ObjectHandle With {
            .Handle = Handle,
            .Identifier = Identifier
        }
    End Function
#End Region

    Public Overridable ReadOnly Property Value As Double Implements IDynamicsExpression(Of Double).Value
        Get
            Return _value
        End Get
    End Property

    Public Property Identifier As String Implements INamedValue.Key
    Public Property Handle As Integer Implements IAddressHandle.Address

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class

''' <summary>
''' The variable represents a node instance in the network system.(变量对象代表了网络对象之中的一个实体节点)
''' </summary>
''' <remarks></remarks>
Public MustInherit Class Variable : Implements IAddressHandle
    Implements INamedValue

    ''' <summary>
    ''' The location pointer of this variable node in the network system.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlAttribute> Public Property Handle As Integer Implements IAddressHandle.Address
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

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
