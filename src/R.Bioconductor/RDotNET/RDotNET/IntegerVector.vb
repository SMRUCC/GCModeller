#Region "Microsoft.VisualBasic::a27ece0ea8ddc7be61d5a56be1801d57, RDotNET\RDotNET\IntegerVector.vb"

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

    ' Class IntegerVector
    ' 
    '     Properties: DataSize, Item, NACode
    ' 
    '     Constructor: (+4 Overloads) Sub New
    ' 
    '     Function: GetArrayFast
    ' 
    '     Sub: CopyTo, SetVectorDirect
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A collection of integers from <c>-2^31 + 1</c> to <c>2^31 - 1</c>.
''' </summary>
''' <remarks>
''' The minimum value of IntegerVector is different from that of System.Int32 in .NET Framework.
''' </remarks>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class IntegerVector
	Inherits Vector(Of Integer)
    ''' <summary>
    ''' Creates a new empty IntegerVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="length">The length.</param>
    ''' <seealso cref="REngineExtension.CreateIntegerVector"/>
    Public Sub New(engine As REngine, length As Integer)
		MyBase.New(engine, SymbolicExpressionType.IntegerVector, length)
	End Sub

    ''' <summary>
    ''' Creates a new IntegerVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateIntegerVector"/>
    Public Sub New(engine As REngine, vector As IEnumerable(Of Integer))
		MyBase.New(engine, SymbolicExpressionType.IntegerVector, vector)
	End Sub

    ''' <summary>
    ''' Creates a new IntegerVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateIntegerVector"/>
    Public Sub New(engine As REngine, vector As Integer())
		MyBase.New(engine, SymbolicExpressionType.IntegerVector, vector.Length)
		Marshal.Copy(vector, 0, DataPointer, vector.Length)
	End Sub

	''' <summary>
	''' Creates a new instance for an integer vector.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to an integer vector.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="index">The zero-based index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(index As Integer) As Integer
		Get
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Dim offset As Integer = GetOffset(index)
				Return Marshal.ReadInt32(DataPointer, offset)
			End Using
		End Get
		Set
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Dim offset As Integer = GetOffset(index)
				Marshal.WriteInt32(DataPointer, offset, value)
			End Using
		End Set
	End Property

	''' <summary>
	''' Efficient conversion from R vector representation to the array equivalent in the CLR
	''' </summary>
	''' <returns>Array equivalent</returns>
	Protected Overrides Function GetArrayFast() As Integer()
		Dim res = New Integer(Me.Length - 1) {}
		Marshal.Copy(DataPointer, res, 0, res.Length)
		Return res
	End Function

	''' <summary>
	''' Efficient initialisation of R vector values from an array representation in the CLR
	''' </summary>
	Protected Overrides Sub SetVectorDirect(values As Integer())
		Marshal.Copy(values, 0, DataPointer, values.Length)
	End Sub

	''' <summary>
	''' Gets the size of an integer in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return 4
		End Get
	End Property

    ''' <summary>
    ''' Copies the elements to the specified array.
    ''' </summary>
    ''' <param name="destination">The destination array.</param>
    ''' <param name="length">The length to copy.</param>
    ''' <param name="sourceIndex">The first index of the vector.</param>
    ''' <param name="destinationIndex">The first index of the destination array.</param>
    Public Shadows Sub CopyTo(destination As Integer(), length As Integer, Optional sourceIndex As Integer = 0, Optional destinationIndex As Integer = 0)
        If destination Is Nothing Then
            Throw New ArgumentNullException("destination")
        End If
        If length < 0 Then
            Throw New IndexOutOfRangeException("length")
        End If
        If sourceIndex < 0 OrElse MyBase.Length < sourceIndex + length Then
            Throw New IndexOutOfRangeException("sourceIndex")
        End If
        If destinationIndex < 0 OrElse destination.Length < destinationIndex + length Then
            Throw New IndexOutOfRangeException("destinationIndex")
        End If

        Dim offset As Integer = GetOffset(sourceIndex)
        Dim pointer As IntPtr = IntPtr.Add(DataPointer, offset)
        Marshal.Copy(pointer, destination, destinationIndex, length)
    End Sub

	''' <summary>
	''' Gets the code used for NA for integer vectors
	''' </summary>
	Protected ReadOnly Property NACode() As Integer
		Get
			Return Integer.MinValue
		End Get
	End Property
End Class

