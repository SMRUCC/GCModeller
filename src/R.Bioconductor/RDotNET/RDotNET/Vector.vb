#Region "Microsoft.VisualBasic::2d192813a5bbbf41a63c2d7f92f06cf5, RDotNET\RDotNET\Vector.vb"

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

    ' Class Vector
    ' 
    '     Properties: DataPointer, Item, Length, Names
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: GetEnumerator, getIndex, GetOffset, IEnumerable_GetEnumerator, ToArray
    ' 
    '     Sub: CopyTo, SetVector
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Diagnostics
Imports RDotNet.Internals
Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A vector base.
''' </summary>
''' <typeparam name="T">The element type.</typeparam>
<DebuggerDisplay("Length = {Length}; RObjectType = {Type}")> _
<DebuggerTypeProxy(GetType(VectorDebugView(Of )))> _
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public MustInherit Class Vector(Of T)
	Inherits SymbolicExpression
	Implements IEnumerable(Of T)
	''' <summary>
	''' Creates a new vector with the specified size.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="type">The element type.</param>
	''' <param name="length">The length of vector.</param>
	Protected Sub New(engine As REngine, type As SymbolicExpressionType, length As Integer)
		MyBase.New(engine, engine.GetFunction(Of Rf_allocVector)()(type, length))
		If length <= 0 Then
			Throw New ArgumentOutOfRangeException("length")
		End If
		Dim empty = New Byte(length * DataSize - 1) {}
		Marshal.Copy(empty, 0, DataPointer, empty.Length)
	End Sub

    ''' <summary>
    ''' Creates a new vector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="type">The element type.</param>
    ''' <param name="vector">The elements of vector.</param>
    Protected Sub New(engine As REngine, type As SymbolicExpressionType, vector As IEnumerable(Of T))
        MyBase.New(engine, engine.GetFunction(Of Rf_allocVector)()(type, vector.Count()))
        SetVector(vector.ToArray())
    End Sub

	''' <summary>
	''' Creates a new instance for a vector.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a vector.</param>
	Protected Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="index">The zero-based index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public MustOverride Default Property Item(index As Integer) As T

	''' <summary>
	''' Initializes the content of a vector with runtime speed in mind. This method protects the R vector, then call SetVectorDirect.
	''' </summary>
	''' <param name="values">The values to put in the vector. Length must match exactly the vector size</param>
	Public Sub SetVector(values As T())
		If values.Length <> Me.Length Then
			Throw New ArgumentException("The length of the array provided differs from the vector length")
		End If
		Using New ProtectedPointer(Me)
			SetVectorDirect(values)
		End Using
	End Sub

	''' <summary>
	''' A method to transfer data from native to .NET managed array equivalents fast.
	''' </summary>
	''' <returns>Array of values in the vector</returns>
	Public Function ToArray() As T()
		Using New ProtectedPointer(Me)
			Return GetArrayFast()
		End Using
	End Function

	''' <summary>
	''' Gets a representation as a one dimensional array of an R vector, with efficiency in mind for the unmanaged to managed transition, if possible.
	''' </summary>
	''' <returns></returns>
	Protected MustOverride Function GetArrayFast() As T()

	''' <summary>
	''' Initializes the content of a vector with runtime speed in mind. The vector must already be protected before calling this method.
	''' </summary>
	''' <param name="values">The values to put in the vector. Length must match exactly the vector size</param>
	Protected MustOverride Sub SetVectorDirect(values As T())

	''' <summary>
	''' Gets or sets the element at the specified name.
	''' </summary>
	''' <param name="name">The name of the element to get or set.</param>
	''' <returns>The element at the specified name.</returns>
	Public Overridable Default Property Item(name As String) As T
		Get
			Dim index As Integer = getIndex(name)
			Return Me(index)
		End Get
		Set
			Dim index As Integer = getIndex(name)
			Me(index) = value
		End Set
	End Property

	Private Function getIndex(name As String) As Integer
		If name Is Nothing Then
			Throw New ArgumentNullException("name", "indexing a vector by name requires a non-null name argument")
		End If
        Dim names As String() = Me.Names
        If names Is Nothing Then
            Throw New NotSupportedException("The vector has no names defined - indexing it by name cannot be supported")
        End If
        Dim index As Integer = Array.IndexOf(names, name)
        Return index
	End Function

	''' <summary>
	''' Gets the number of elements.
	''' </summary>
	Public ReadOnly Property Length() As Integer
		Get
			Return Me.GetFunction(Of Rf_length)()(handle)
		End Get
	End Property

	''' <summary>
	''' Gets the names of elements.
	''' </summary>
	Public ReadOnly Property Names() As String()
		Get
			Dim namesSymbol As SymbolicExpression = Engine.GetPredefinedSymbol("R_NamesSymbol")
            Dim nameList As SymbolicExpression = GetAttribute(namesSymbol)
            If nameList Is Nothing Then
                Return Nothing
            End If
            Dim namesVector As CharacterVector = nameList.AsCharacter()
            If namesVector Is Nothing Then
				Return Nothing
			End If

			Dim length As Integer = namesVector.Length
			Dim result = New String(length - 1) {}
			namesVector.CopyTo(result, length)
			Return result
		End Get
	End Property

	''' <summary>
	''' Gets the pointer for the first element.
	''' </summary>
	Protected ReadOnly Property DataPointer() As IntPtr
		Get
			Return IntPtr.Add(handle, Marshal.SizeOf(GetType(VECTOR_SEXPREC)))
		End Get
	End Property

	''' <summary>
	''' Gets the size of an element in byte.
	''' </summary>
	Protected MustOverride ReadOnly Property DataSize() As Integer

#Region "IEnumerable<T> Members"

    ''' <summary>
    ''' Gets enumerator
    ''' </summary>
    Public Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
        For index As Integer = 0 To Length - 1
            Yield Me(index)
        Next
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
		Return GetEnumerator()
	End Function

#End Region

    ''' <summary>
    ''' Copies the elements to the specified array.
    ''' </summary>
    ''' <param name="destination">The destination array.</param>
    ''' <param name="length">The length to copy.</param>
    ''' <param name="sourceIndex">The first index of the vector.</param>
    ''' <param name="destinationIndex">The first index of the destination array.</param>
    Public Sub CopyTo(destination As T(), length As Integer, Optional sourceIndex As Integer = 0, Optional destinationIndex As Integer = 0)
        If destination Is Nothing Then
            Throw New ArgumentNullException("destination")
        End If
        If length < 0 Then
            Throw New IndexOutOfRangeException("length")
        End If
        If sourceIndex < 0 OrElse Me.Length < sourceIndex + length Then
            Throw New IndexOutOfRangeException("sourceIndex")
        End If
        If destinationIndex < 0 OrElse destination.Length < destinationIndex + length Then
            Throw New IndexOutOfRangeException("destinationIndex")
        End If

        While System.Threading.Interlocked.Decrement(length) >= 0
            destination(System.Math.Max(System.destinationIndex),destinationIndex - 1)) = Me(System.Math.Max(System.sourceIndex),sourceIndex - 1))
		End While
	End Sub

	''' <summary>
	''' Gets the offset for the specified index.
	''' </summary>
	''' <param name="index">The index.</param>
	''' <returns>The offset.</returns>
	Protected Function GetOffset(index As Integer) As Integer
		Return DataSize * index
	End Function
End Class

