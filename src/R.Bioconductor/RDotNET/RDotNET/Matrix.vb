Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports RDotNet.Diagnostics
Imports RDotNet.Internals

''' <summary>
''' A matrix base.
''' </summary>
''' <typeparam name="T">The element type.</typeparam>
<DebuggerDisplay("MatrixSize = {RowCount} x {ColumnCount}; RObjectType = {Type}")> _
<DebuggerTypeProxy(GetType(MatrixDebugView(Of )))> _
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public MustInherit Class Matrix(Of T)
	Inherits SymbolicExpression
	''' <summary>
	''' Creates a new matrix with the specified size.
	''' </summary>
	''' <param name="engine">The engine.</param>
	''' <param name="type">The element type.</param>
	''' <param name="rowCount">The size of row.</param>
	''' <param name="columnCount">The size of column.</param>
	Protected Sub New(engine As REngine, type As SymbolicExpressionType, rowCount As Integer, columnCount As Integer)
		MyBase.New(engine, engine.GetFunction(Of Rf_allocMatrix)()(type, rowCount, columnCount))
		If rowCount <= 0 Then
			Throw New ArgumentOutOfRangeException("rowCount")
		End If
		If columnCount <= 0 Then
			Throw New ArgumentOutOfRangeException("columnCount")
		End If
		Dim empty = New Byte(rowCount * columnCount * DataSize - 1) {}
		Marshal.Copy(empty, 0, DataPointer, empty.Length)
	End Sub

    ''' <summary>
    ''' Creates a new matrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="type">The element type.</param>
    ''' <param name="matrix">The values.</param>
    Public Sub New(engine As REngine, type As SymbolicExpressionType, matrix As T(,))
        MyBase.New(engine, engine.GetFunction(Of Rf_allocMatrix)()(type, matrix.GetLength(0), matrix.GetLength(1)))
        Dim rowCount As Integer = Me.RowCount
        Dim columnCount As Integer = Me.ColumnCount
        'InitMatrixWithIndexers(matrix, rowCount, columnCount);
        InitMatrixFast(matrix)
    End Sub

	Private Sub InitMatrixFast(matrix As T(,))
		Using New ProtectedPointer(Me)
			InitMatrixFastDirect(matrix)
		End Using
	End Sub

	''' <summary>
	''' Initializes this R matrix, using the values in a rectangular array.
	''' </summary>
	''' <param name="matrix"></param>
	Protected MustOverride Sub InitMatrixFastDirect(matrix As T(,))

	''' <summary>
	''' Creates a new instance for a matrix.
	''' </summary>
	''' <param name="engine">The engine.</param>
	''' <param name="coerced">The pointer to a matrix.</param>
	Protected Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="rowIndex">The zero-based row index of the element to get or set.</param>
	''' <param name="columnIndex">The zero-based column index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public MustOverride Default Property Item(rowIndex As Integer, columnIndex As Integer) As T

	''' <summary>
	''' Gets or sets the element at the specified names.
	''' </summary>
	''' <param name="rowName">The row name of the element to get or set.</param>
	''' <param name="columnName">The column name of the element to get or set.</param>
	''' <returns>The element at the specified names.</returns>
	Public Overridable Default Property Item(rowName As String, columnName As String) As T
		Get
			If rowName Is Nothing Then
				Throw New ArgumentNullException("rowName")
			End If
			If columnName Is Nothing Then
				Throw New ArgumentNullException("columnName")
			End If
            Dim rowNames As String() = Me.RowNames
            If rowNames Is Nothing Then
                Throw New InvalidOperationException()
            End If
            Dim columnNames As String() = Me.ColumnNames
            If columnNames Is Nothing Then
                Throw New InvalidOperationException()
            End If
            Dim rowIndex As Integer = Array.IndexOf(rowNames, rowName)
            Dim columnIndex As Integer = Array.IndexOf(columnNames, columnName)
            Return Me(rowIndex, columnIndex)
		End Get
		Set
			If rowName Is Nothing Then
				Throw New ArgumentNullException("rowName")
			End If
			If columnName Is Nothing Then
				Throw New ArgumentNullException("columnName")
			End If
            Dim rowNames As String() = Me.RowNames
            If rowNames Is Nothing Then
                Throw New InvalidOperationException()
            End If
            Dim columnNames As String() = Me.ColumnNames
            If columnNames Is Nothing Then
                Throw New InvalidOperationException()
            End If
            Dim rowIndex As Integer = Array.IndexOf(rowNames, rowName)
            Dim columnIndex As Integer = Array.IndexOf(columnNames, columnName)
            Me(rowIndex, columnIndex) = value
		End Set
	End Property

	''' <summary>
	''' Gets the row size of elements.
	''' </summary>
	Public ReadOnly Property RowCount() As Integer
		Get
			Return Me.GetFunction(Of Rf_nrows)()(handle)
		End Get
	End Property

	''' <summary>
	''' Gets the column size of elements.
	''' </summary>
	Public ReadOnly Property ColumnCount() As Integer
		Get
			Return Me.GetFunction(Of Rf_ncols)()(handle)
		End Get
	End Property

	''' <summary>
	''' Gets the total number of items (rows times columns) in this matrix
	''' </summary>
	Public ReadOnly Property ItemCount() As Integer
		Get
			Return RowCount * ColumnCount
		End Get
	End Property

	''' <summary>
	''' Gets the names of rows.
	''' </summary>
	Public ReadOnly Property RowNames() As String()
		Get
			Dim dimnamesSymbol As SymbolicExpression = Engine.GetPredefinedSymbol("R_DimNamesSymbol")
			Dim dimnames As SymbolicExpression = GetAttribute(dimnamesSymbol)
			If dimnames Is Nothing Then
				Return Nothing
			End If
            Dim rowNameList As CharacterVector = dimnames.AsList()(0).AsCharacter()
            If rowNameList Is Nothing Then
                Return Nothing
            End If

            Dim length As Integer = rowNameList.Length
            Dim result = New String(length - 1) {}
            rowNameList.CopyTo(result, length)
            Return result
		End Get
	End Property

	''' <summary>
	''' Gets the names of columns.
	''' </summary>
	Public ReadOnly Property ColumnNames() As String()
		Get
			Dim dimnamesSymbol As SymbolicExpression = Engine.GetPredefinedSymbol("R_DimNamesSymbol")
			Dim dimnames As SymbolicExpression = GetAttribute(dimnamesSymbol)
			If dimnames Is Nothing Then
				Return Nothing
			End If
            Dim colNames As CharacterVector = dimnames.AsList()(1).AsCharacter()
            If colNames Is Nothing Then
                Return Nothing
            End If

            Dim length As Integer = colNames.Length
            Dim result = New String(length - 1) {}
            colNames.CopyTo(result, length)
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

	''' <summary>
	''' Gets the offset for the specified indexes.
	''' </summary>
	''' <param name="rowIndex">The index of row.</param>
	''' <param name="columnIndex">The index of column.</param>
	''' <returns>The offset.</returns>
	Protected Function GetOffset(rowIndex As Integer, columnIndex As Integer) As Integer
		Return DataSize * (columnIndex * RowCount + rowIndex)
	End Function

    ''' <summary>
    ''' Copies the elements to the specified array.
    ''' </summary>
    ''' <param name="destination">The destination array.</param>
    ''' <param name="rowCount">The row length to copy.</param>
    ''' <param name="columnCount">The column length to copy.</param>
    ''' <param name="sourceRowIndex">The first row index of the matrix.</param>
    ''' <param name="sourceColumnIndex">The first column index of the matrix.</param>
    ''' <param name="destinationRowIndex">The first row index of the destination array.</param>
    ''' <param name="destinationColumnIndex">The first column index of the destination array.</param>
    Public Sub CopyTo(destination As T(,), rowCount As Integer, columnCount As Integer, Optional sourceRowIndex As Integer = 0, Optional sourceColumnIndex As Integer = 0, Optional destinationRowIndex As Integer = 0,
        Optional destinationColumnIndex As Integer = 0)
        If destination Is Nothing Then
            Throw New ArgumentNullException("destination")
        End If
        If rowCount < 0 Then
            Throw New IndexOutOfRangeException("rowCount")
        End If
        If columnCount < 0 Then
            Throw New IndexOutOfRangeException("columnCount")
        End If
        If sourceRowIndex < 0 OrElse Me.RowCount < sourceRowIndex + rowCount Then
            Throw New IndexOutOfRangeException("sourceRowIndex")
        End If
        If sourceColumnIndex < 0 OrElse Me.ColumnCount < sourceColumnIndex + columnCount Then
            Throw New IndexOutOfRangeException("sourceColumnIndex")
        End If
        If destinationRowIndex < 0 OrElse destination.GetLength(0) < destinationRowIndex + rowCount Then
            Throw New IndexOutOfRangeException("destinationRowIndex")
        End If
        If destinationColumnIndex < 0 OrElse destination.GetLength(1) < destinationColumnIndex + columnCount Then
            Throw New IndexOutOfRangeException("destinationColumnIndex")
        End If

        While System.Threading.Interlocked.Decrement(rowCount) >= 0
            Dim currentSourceRowIndex As Integer = System.Math.Max(System.Threading.Interlocked.Increment(sourceRowIndex), sourceRowIndex - 1)
            Dim currentDestinationRowIndex As Integer = System.Math.Max(System.Threading.Interlocked.Increment(destinationRowIndex), destinationRowIndex - 1)
            Dim currentColumnCount As Integer = columnCount
            Dim currentSourceColumnIndex As Integer = sourceColumnIndex
			Dim currentDestinationColumnIndex As Integer = destinationColumnIndex
			While System.Threading.Interlocked.Decrement(currentColumnCount) >= 0
				destination(currentDestinationRowIndex, System.Math.Max(System.Threading.Interlocked.Increment(currentDestinationColumnIndex),currentDestinationColumnIndex - 1)) = Me(currentSourceRowIndex, System.Math.Max(System.Threading.Interlocked.Increment(currentSourceColumnIndex),currentSourceColumnIndex - 1))
			End While
		End While
	End Sub

	''' <summary>
	''' Gets a .NET representation as a two dimensional array of an R matrix
	''' </summary>
	''' <returns></returns>
	Public Function ToArray() As T(,)
		Using p = New ProtectedPointer(Me)
			Return GetArrayFast()
		End Using
	End Function

	''' <summary>
	''' Efficient conversion from R matrix representation to the array equivalent in the CLR
	''' </summary>
	''' <returns>Array equivalent</returns>
	Protected MustOverride Function GetArrayFast() As T(,)
End Class
