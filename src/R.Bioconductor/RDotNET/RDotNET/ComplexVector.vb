#Region "Microsoft.VisualBasic::cd90419b3d73be7bf8471088587e3399, RDotNET\RDotNET\ComplexVector.vb"

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

    ' Class ComplexVector
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
Imports RDotNet.Utilities
Imports System.Collections.Generic
Imports System.Numerics
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A collection of complex numbers.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class ComplexVector
	Inherits Vector(Of Complex)
    ''' <summary>
    ''' Creates a new empty ComplexVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="length">The length.</param>
    ''' <seealso cref="REngineExtension.CreateComplexVector"/>
    Public Sub New(engine As REngine, length As Integer)
		MyBase.New(engine, SymbolicExpressionType.ComplexVector, length)
	End Sub

    ''' <summary>
    ''' Creates a new ComplexVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateComplexVector"/>
    Public Sub New(engine As REngine, vector As IEnumerable(Of Complex))
		MyBase.New(engine, SymbolicExpressionType.ComplexVector, vector)
	End Sub

	''' <summary>
	''' Creates a new instance for a complex number vector.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a complex number vector.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="index">The zero-based index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(index As Integer) As Complex
		Get
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Dim data = New Double(1) {}
				Dim offset As Integer = GetOffset(index)
				Dim pointer As IntPtr = IntPtr.Add(DataPointer, offset)
				Marshal.Copy(pointer, data, 0, data.Length)
				Return New Complex(data(0), data(1))
			End Using
		End Get
		Set
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Dim data =  {value.Real, value.Imaginary}
				Dim offset As Integer = GetOffset(index)
				Dim pointer As IntPtr = IntPtr.Add(DataPointer, offset)
				Marshal.Copy(data, 0, pointer, data.Length)
			End Using
		End Set
	End Property

	''' <summary>
	''' Gets an array representation in the CLR of a vector in R.
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function GetArrayFast() As Complex()
		Dim n As Integer = Me.Length
		Dim data = New Double(2 * n - 1) {}
		Marshal.Copy(DataPointer, data, 0, 2 * n)
		Return RTypesUtil.DeserializeComplexFromDouble(data)
	End Function

	''' <summary>
	''' Efficient initialisation of R vector values from an array representation in the CLR
	''' </summary>
	Protected Overrides Sub SetVectorDirect(values As Complex())
		Dim data As Double() = RTypesUtil.SerializeComplexToDouble(values)
		Dim pointer As IntPtr = IntPtr.Add(DataPointer, 0)
		Marshal.Copy(data, 0, pointer, data.Length)
	End Sub

	''' <summary>
	''' Gets the size of a complex number in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return Marshal.SizeOf(GetType(Complex))
		End Get
	End Property
End Class

