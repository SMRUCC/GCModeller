#Region "Microsoft.VisualBasic::90879a389eb47287088980a718474e6e, RDotNET\RDotNET\NumericMatrix.vb"

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

    ' Class NumericMatrix
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
''' A matrix of real numbers in double precision.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class NumericMatrix
	Inherits Matrix(Of Double)
    ''' <summary>
    ''' Creates a new empty NumericMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <seealso cref="REngineExtension.CreateNumericMatrix"/>
    Public Sub New(engine As REngine, rowCount As Integer, columnCount As Integer)
		MyBase.New(engine, SymbolicExpressionType.NumericVector, rowCount, columnCount)
	End Sub

    ''' <summary>
    ''' Creates a new NumericMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="matrix">The values.</param>
    ''' <seealso cref="REngineExtension.CreateNumericMatrix"/>
    Public Sub New(engine As REngine, matrix As Double(,))
		MyBase.New(engine, SymbolicExpressionType.NumericVector, matrix)
	End Sub

	''' <summary>
	''' Creates a new instance for a numeric matrix.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a numeric matrix.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
	''' <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(rowIndex As Integer, columnIndex As Integer) As Double
		Get
			If rowIndex < 0 OrElse RowCount <= rowIndex Then
				Throw New ArgumentOutOfRangeException("rowIndex")
			End If
			If columnIndex < 0 OrElse ColumnCount <= columnIndex Then
				Throw New ArgumentOutOfRangeException("columnIndex")
			End If
			Using New ProtectedPointer(Me)
				Dim data = New Byte(DataSize - 1) {}
				Dim pointer As IntPtr = DataPointer
				Dim offset As Integer = GetOffset(rowIndex, columnIndex)
				For byteIndex As Integer = 0 To data.Length - 1
					data(byteIndex) = Marshal.ReadByte(pointer, offset + byteIndex)
				Next
				Return BitConverter.ToDouble(data, 0)
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
				Dim data As Byte() = BitConverter.GetBytes(value)
				Dim pointer As IntPtr = DataPointer
				Dim offset As Integer = GetOffset(rowIndex, columnIndex)
				For byteIndex As Integer = 0 To data.Length - 1
					Marshal.WriteByte(pointer, offset + byteIndex, data(byteIndex))
				Next
			End Using
		End Set
	End Property

	''' <summary>
	''' Initializes this R matrix, using the values in a rectangular array.
	''' </summary>
	''' <param name="matrix"></param>
	Protected Overrides Sub InitMatrixFastDirect(matrix As Double(,))
		Dim values = ArrayConverter.ArrayConvertOneDim(matrix)
		Marshal.Copy(values, 0, DataPointer, values.Length)
	End Sub

	''' <summary>
	''' Gets a rectangular array representation in the CLR, equivalent of a matrix in R.
	''' </summary>
	''' <returns>Rectangular array with values representing the content of the R matrix. Beware NA codes</returns>
	Protected Overrides Function GetArrayFast() As Double(,)
		Dim values = New Double(Me.ItemCount - 1) {}
		Marshal.Copy(DataPointer, values, 0, values.Length)
		Return ArrayConverter.ArrayConvertAllTwoDim(values, Me.RowCount, Me.ColumnCount)
	End Function

	''' <summary>
	''' Gets the size of a real number in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return 8
		End Get
	End Property
End Class

