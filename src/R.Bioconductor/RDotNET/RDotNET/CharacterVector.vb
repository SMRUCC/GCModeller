#Region "Microsoft.VisualBasic::4423231801fdab472f0da1e6d2c241b2, RDotNET\RDotNET\CharacterVector.vb"

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

    ' Class CharacterVector
    ' 
    '     Properties: DataSize, Item
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: GetArrayFast, GetValue, mkChar
    ' 
    '     Sub: SetValue, SetVectorDirect
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A collection of strings.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class CharacterVector
	Inherits Vector(Of String)
    ''' <summary>
    ''' Creates a new empty CharacterVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="length">The length.</param>
    ''' <seealso cref="REngineExtension.CreateCharacterVector"/>
    Public Sub New(engine As REngine, length As Integer)
		MyBase.New(engine, SymbolicExpressionType.CharacterVector, length)
	End Sub

    ''' <summary>
    ''' Creates a new CharacterVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateCharacterVector"/>
    Public Sub New(engine As REngine, vector As IEnumerable(Of String))
		MyBase.New(engine, SymbolicExpressionType.CharacterVector, vector)
	End Sub

	''' <summary>
	''' Creates a new instance for a string vector.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a string vector.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="index">The zero-based index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(index As Integer) As String
		Get
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Return GetValue(index)
			End Using
		End Get
		Set
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				SetValue(index, value)
			End Using
		End Set
	End Property

	''' <summary>
	''' Gets an array representation of this R vector. Note that the implementation is not as fast as for numeric vectors.
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function GetArrayFast() As String()
		Dim n As Integer = Me.Length
		Dim res As String() = New String(n - 1) {}
		For i As Integer = 0 To n - 1
			res(i) = GetValue(i)
		Next
		Return res
	End Function

	Private Function GetValue(index As Integer) As String
		Dim offset As Integer = GetOffset(index)
		Dim pointerItem As IntPtr = Marshal.ReadIntPtr(DataPointer, offset)
		If pointerItem = Engine.NaStringPointer Then
			Return Nothing
		End If
		Dim pointer As IntPtr = IntPtr.Add(pointerItem, Marshal.SizeOf(GetType(VECTOR_SEXPREC)))
		Return InternalString.StringFromNativeUtf8(pointer)
	End Function

	Private _mkChar As Rf_mkChar = Nothing

	Private Function mkChar(value As String) As IntPtr
		If _mkChar Is Nothing Then
			_mkChar = Me.GetFunction(Of Rf_mkChar)()
		End If
		Return _mkChar(value)
	End Function

	Private Sub SetValue(index As Integer, value As String)
		Dim offset As Integer = GetOffset(index)
		Dim stringPointer As IntPtr = If(value Is Nothing, Engine.NaStringPointer, mkChar(value))
		Marshal.WriteIntPtr(DataPointer, offset, stringPointer)
	End Sub

	''' <summary>
	''' Efficient initialisation of R vector values from an array representation in the CLR
	''' </summary>
	Protected Overrides Sub SetVectorDirect(values As String())
		' Possibly not the fastest implementation, but faster may require C code.
		' TODO check the behavior of P/Invoke on array of strings (VT_ARRAY|VT_LPSTR?)
		For i As Integer = 0 To values.Length - 1
			SetValue(i, values(i))
		Next
	End Sub

	''' <summary>
	''' Gets the size of a pointer in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return Marshal.SizeOf(GetType(IntPtr))
		End Get
	End Property
End Class

