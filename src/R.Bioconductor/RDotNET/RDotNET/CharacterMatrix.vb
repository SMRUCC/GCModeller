#Region "Microsoft.VisualBasic::dca03071817d655be37de9bc18f146ba, RDotNET\RDotNET\CharacterMatrix.vb"

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

    ' Class CharacterMatrix
    ' 
    '     Properties: DataSize, Item
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: GetArrayFast
    ' 
    '     Sub: InitMatrixFastDirect, SetValue
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A matrix of strings.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class CharacterMatrix
	Inherits Matrix(Of String)
    ''' <summary>
    ''' Creates a new empty CharacterMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <seealso cref="REngineExtension.CreateCharacterMatrix"/>
    Public Sub New(engine As REngine, rowCount As Integer, columnCount As Integer)
		MyBase.New(engine, SymbolicExpressionType.CharacterVector, rowCount, columnCount)
	End Sub

    ''' <summary>
    ''' Creates a new CharacterMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="matrix">The values.</param>
    ''' <seealso cref="REngineExtension.CreateCharacterMatrix"/>
    Public Sub New(engine As REngine, matrix As String(,))
		MyBase.New(engine, SymbolicExpressionType.CharacterVector, matrix)
	End Sub

	''' <summary>
	''' Creates a new instance for a string matrix.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a string matrix.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
	''' <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(rowIndex As Integer, columnIndex As Integer) As String
		Get
			If rowIndex < 0 OrElse RowCount <= rowIndex Then
				Throw New ArgumentOutOfRangeException("rowIndex")
			End If
			If columnIndex < 0 OrElse ColumnCount <= columnIndex Then
				Throw New ArgumentOutOfRangeException("columnIndex")
			End If
			Using New ProtectedPointer(Me)
				Dim offset As Integer = GetOffset(rowIndex, columnIndex)
				Dim pointer As IntPtr = Marshal.ReadIntPtr(DataPointer, offset)
				Return New InternalString(Engine, pointer)
			End Using
		End Get
		Set
			If rowIndex < 0 OrElse RowCount <= rowIndex Then
				Throw New ArgumentOutOfRangeException("rowIndex")
			End If
			If columnIndex < 0 OrElse ColumnCount <= columnIndex Then
				Throw New ArgumentOutOfRangeException("columnIndex")
			End If
			Using New ProtectedPointer(Me)
				SetValue(rowIndex, columnIndex, value)
			End Using
		End Set
	End Property

	Private Sub SetValue(rowIndex As Integer, columnIndex As Integer, value As String)
		Dim offset As Integer = GetOffset(rowIndex, columnIndex)
		Dim s As SymbolicExpression = If(value Is Nothing, Engine.NilValue, New InternalString(Engine, value))
		Using New ProtectedPointer(s)
			Marshal.WriteIntPtr(DataPointer, offset, s.DangerousGetHandle())
		End Using
	End Sub

	''' <summary>
	''' Initializes this R matrix, using the values in a rectangular array.
	''' </summary>
	''' <param name="matrix"></param>
	Protected Overrides Sub InitMatrixFastDirect(matrix As String(,))
		Dim rows As Integer = matrix.GetLength(0)
		Dim cols As Integer = matrix.GetLength(1)
		For i As Integer = 0 To rows - 1
			For j As Integer = 0 To cols - 1
				SetValue(i, j, matrix(i, j))
			Next
		Next
	End Sub

	''' <summary>
	''' NotSupportedException();
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function GetArrayFast() As String(,)
		Throw New NotSupportedException()
	End Function

	''' <summary>
	''' Gets the size of a pointer in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return Marshal.SizeOf(GetType(IntPtr))
		End Get
	End Property
End Class

