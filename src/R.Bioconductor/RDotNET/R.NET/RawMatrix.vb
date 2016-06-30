Imports RDotNet.Internals
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A matrix of byte values.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class RawMatrix
	Inherits Matrix(Of Byte)
	''' <summary>
	''' Creates a new RawMatrix with the specified size.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="rowCount">The row size.</param>
	''' <param name="columnCount">The column size.</param>
    ''' <seealso cref="REngineExtension.CreateRawMatrix"/>
	Public Sub New(engine As REngine, rowCount As Integer, columnCount As Integer)
		MyBase.New(engine, SymbolicExpressionType.RawVector, rowCount, columnCount)
	End Sub

	''' <summary>
	''' Creates a new RawMatrix with the specified values.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="matrix">The values.</param>
    ''' <seealso cref="REngineExtension.CreateRawMatrix"/>
	Public Sub New(engine As REngine, matrix As Byte(,))
		MyBase.New(engine, SymbolicExpressionType.RawVector, matrix)
	End Sub

	''' <summary>
	''' Creates a new instance for a raw matrix.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a raw matrix.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
	''' <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(rowIndex As Integer, columnIndex As Integer) As Byte
		Get
			If rowIndex < 0 OrElse RowCount <= rowIndex Then
				Throw New ArgumentOutOfRangeException("rowIndex")
			End If
			If columnIndex < 0 OrElse ColumnCount <= columnIndex Then
				Throw New ArgumentOutOfRangeException("columnIndex")
			End If
			Using New ProtectedPointer(Me)
				Dim offset As Integer = GetOffset(rowIndex, columnIndex)
				Return Marshal.ReadByte(DataPointer, offset)
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
				Marshal.WriteByte(DataPointer, offset, value)
			End Using
		End Set
	End Property

	''' <summary>
	''' Initializes this R matrix, using the values in a rectangular array.
	''' </summary>
	''' <param name="matrix"></param>
	Protected Overrides Sub InitMatrixFastDirect(matrix As Byte(,))
		Dim values = Utility.ArrayConvertOneDim(matrix)
		Marshal.Copy(values, 0, DataPointer, values.Length)
	End Sub

	''' <summary>
	''' Efficient conversion from R vector representation to the array equivalent in the CLR
	''' </summary>
	''' <returns>Array equivalent</returns>
	Protected Overrides Function GetArrayFast() As Byte(,)
		Dim values As Byte() = New Byte(Me.ItemCount - 1) {}
		Marshal.Copy(DataPointer, values, 0, values.Length)
		Return Utility.ArrayConvertAllTwoDim(values, Me.RowCount, Me.ColumnCount)
	End Function

	''' <summary>
	''' Gets the size of an Raw in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return 1
		End Get
	End Property
End Class
