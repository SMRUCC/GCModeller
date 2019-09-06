#Region "Microsoft.VisualBasic::a8733dedb3dbbddb805376f404ca0178, RDotNET\RDotNET\IntegerMatrix.vb"

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

    ' Class IntegerMatrix
    ' 
    '     Properties: DataSize, Item
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: GetArrayFast
    ' 
    '     Sub: InitMatrixFastDirect
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports RDotNet.Utilities
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A matrix of integers from <c>-2^31 + 1</c> to <c>2^31 - 1</c>.
''' </summary>
''' <remarks>
''' The minimum value of IntegerVector is different from that of System.Int32 in .NET Framework.
''' </remarks>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class IntegerMatrix
	Inherits Matrix(Of Integer)
    ''' <summary>
    ''' Creates a new empty IntegerMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <seealso cref="REngineExtension.CreateIntegerMatrix"/>
    Public Sub New(engine As REngine, rowCount As Integer, columnCount As Integer)
		MyBase.New(engine, SymbolicExpressionType.IntegerVector, rowCount, columnCount)
	End Sub

    ''' <summary>
    ''' Creates a new IntegerMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="matrix">The values.</param>
    ''' <seealso cref="REngineExtension.CreateIntegerMatrix"/>
    Public Sub New(engine As REngine, matrix As Integer(,))
		MyBase.New(engine, SymbolicExpressionType.IntegerVector, matrix)
	End Sub

	''' <summary>
	''' Creates a new instance for an integer matrix.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to an integer matrix.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
	''' <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(rowIndex As Integer, columnIndex As Integer) As Integer
		Get
			If rowIndex < 0 OrElse RowCount <= rowIndex Then
				Throw New ArgumentOutOfRangeException("rowIndex")
			End If
			If columnIndex < 0 OrElse ColumnCount <= columnIndex Then
				Throw New ArgumentOutOfRangeException("columnIndex")
			End If
			Using New ProtectedPointer(Me)
				Dim offset As Integer = GetOffset(rowIndex, columnIndex)
				Return Marshal.ReadInt32(DataPointer, offset)
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
				Dim offset As Integer = GetOffset(rowIndex, columnIndex)
				Marshal.WriteInt32(DataPointer, offset, value)
			End Using
		End Set
	End Property

	''' <summary>
	''' Initializes this R matrix, using the values in a rectangular array.
	''' </summary>
	''' <param name="matrix"></param>
	Protected Overrides Sub InitMatrixFastDirect(matrix As Integer(,))
		Dim values = ArrayConverter.ArrayConvertOneDim(matrix)
		Marshal.Copy(values, 0, DataPointer, values.Length)
	End Sub

	''' <summary>
	''' Gets a rectangular array representation in the CLR, equivalent of a matrix in R.
	''' </summary>
	''' <returns>Rectangular array with values representing the content of the R matrix. Beware NA codes</returns>
	Protected Overrides Function GetArrayFast() As Integer(,)
		Dim values = New Integer(Me.ItemCount - 1) {}
		Marshal.Copy(DataPointer, values, 0, values.Length)
		Return ArrayConverter.ArrayConvertAllTwoDim(values, Me.RowCount, Me.ColumnCount)
	End Function

	''' <summary>
	''' Gets the size of an integer in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return 4
		End Get
	End Property
End Class

