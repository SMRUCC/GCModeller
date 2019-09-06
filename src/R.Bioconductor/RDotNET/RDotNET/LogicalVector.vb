#Region "Microsoft.VisualBasic::4c4c99c7f17acaaeb24b335f9bbfe71e, RDotNET\RDotNET\LogicalVector.vb"

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

    ' Class LogicalVector
    ' 
    '     Properties: DataSize, Item
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: GetArrayFast
    ' 
    '     Sub: SetVectorDirect
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A collection of Boolean values.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class LogicalVector
	Inherits Vector(Of Boolean)
    ''' <summary>
    ''' Creates a new empty LogicalVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="length">The length.</param>
    ''' <seealso cref="REngineExtension.CreateLogicalVector"/>
    Public Sub New(engine As REngine, length As Integer)
		MyBase.New(engine, SymbolicExpressionType.LogicalVector, length)
	End Sub

    ''' <summary>
    ''' Creates a new LogicalVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateLogicalVector"/>
    Public Sub New(engine As REngine, vector As IEnumerable(Of Boolean))
		MyBase.New(engine, SymbolicExpressionType.LogicalVector, vector)
	End Sub

	''' <summary>
	''' Creates a new instance for a Boolean vector.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a Boolean vector.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="index">The zero-based index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(index As Integer) As Boolean
		Get
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Dim offset As Integer = GetOffset(index)
				Dim data As Integer = Marshal.ReadInt32(DataPointer, offset)
				Return Convert.ToBoolean(data)
			End Using
		End Get
		Set
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Dim offset As Integer = GetOffset(index)
				Dim data As Integer = Convert.ToInt32(value)
				Marshal.WriteInt32(DataPointer, offset, data)
			End Using
		End Set
	End Property

	''' <summary>
	''' Efficient conversion from R vector representation to the array equivalent in the CLR
	''' </summary>
	''' <returns>Array equivalent</returns>
	Protected Overrides Function GetArrayFast() As Boolean()
		Dim intValues As Integer() = New Integer(Me.Length - 1) {}
		Marshal.Copy(DataPointer, intValues, 0, intValues.Length)
		Return Array.ConvertAll(intValues, AddressOf Convert.ToBoolean)
	End Function

	''' <summary>
	''' Efficient initialisation of R vector values from an array representation in the CLR
	''' </summary>
	Protected Overrides Sub SetVectorDirect(values As Boolean())
		Dim intValues = Array.ConvertAll(values, AddressOf Convert.ToInt32)
		Marshal.Copy(intValues, 0, DataPointer, values.Length)
	End Sub

	''' <summary>
	''' Gets the size of a Boolean value in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			' Boolean is int internally.
			Return 4
		End Get
	End Property
End Class

