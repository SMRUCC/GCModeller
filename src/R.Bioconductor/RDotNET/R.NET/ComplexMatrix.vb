Imports RDotNet.Internals
Imports System.Numerics
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A matrix of complex numbers.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class ComplexMatrix
    Inherits Matrix(Of Complex)

	''' <summary>
	''' Creates a new empty ComplexMatrix with the specified size.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="rowCount">The row size.</param>
	''' <param name="columnCount">The column size.</param>
    ''' <seealso cref="REngineExtension.CreateComplexMatrix"/>
	Public Sub New(engine As REngine, rowCount As Integer, columnCount As Integer)
		MyBase.New(engine, SymbolicExpressionType.ComplexVector, rowCount, columnCount)
	End Sub

	''' <summary>
	''' Creates a new ComplexMatrix with the specified values.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="matrix">The values.</param>
    ''' <seealso cref="REngineExtension.CreateComplexMatrix"/>
	Public Sub New(engine As REngine, matrix As Complex(,))
		MyBase.New(engine, SymbolicExpressionType.CharacterVector, matrix)
	End Sub

	''' <summary>
	''' Creates a new instance for a complex number matrix.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a complex number matrix.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
	''' <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(rowIndex As Integer, columnIndex As Integer) As Complex
		Get
			If rowIndex < 0 OrElse RowCount <= rowIndex Then
				Throw New ArgumentOutOfRangeException("rowIndex")
			End If
			If columnIndex < 0 OrElse ColumnCount <= columnIndex Then
				Throw New ArgumentOutOfRangeException("columnIndex")
			End If
			Using New ProtectedPointer(Me)
				Dim data = New Double(1) {}
				Dim offset As Integer = GetOffset(rowIndex, columnIndex)
				Dim pointer As IntPtr = IntPtr.Add(DataPointer, offset)
				Marshal.Copy(pointer, data, 0, data.Length)
				Return New Complex(data(0), data(1))
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
                Dim data As Double() = New Double() {Value.Real, Value.Imaginary}
				Dim offset As Integer = GetOffset(rowIndex, columnIndex)
				Dim pointer As IntPtr = IntPtr.Add(DataPointer, offset)
                Marshal.Copy(data, 0, pointer, data.Length)
			End Using
		End Set
	End Property

	''' <summary>
	''' Initializes this R matrix, using the values in a rectangular array.
	''' </summary>
	''' <param name="matrix"></param>
	Protected Overrides Sub InitMatrixFastDirect(matrix As Complex(,))
		Dim vectorCplx = Utility.ArrayConvertOneDim(matrix)
		Dim data = Utility.SerializeComplexToDouble(vectorCplx)
		Marshal.Copy(data, 0, DataPointer, data.Length)
	End Sub

	''' <summary>
	''' Gets a rectangular array representation in the CLR, equivalent of a matrix in R. 
	''' </summary>
	''' <returns>Rectangular array with values representing the content of the R matrix. Beware NA codes</returns>
	Protected Overrides Function GetArrayFast() As Complex(,)
		Dim n As Integer = Me.ItemCount
		Dim data = New Double(2 * n - 1) {}
		Marshal.Copy(DataPointer, data, 0, 2 * n)
		Dim oneDim = Utility.DeserializeComplexFromDouble(data)
		Return Utility.ArrayConvertAllTwoDim(oneDim, Me.RowCount, Me.ColumnCount)
	End Function

	''' <summary>
	''' Gets the size of a complex number in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return Marshal.SizeOf(GetType(Complex))
		End Get
	End Property
End Class
