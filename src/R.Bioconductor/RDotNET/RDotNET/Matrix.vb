Imports RDotNet.Diagnostics
Imports RDotNet.Internals
Imports System
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Security.Permissions


    ''' <summary>
    ''' A matrix base.
    ''' </summary>
    ''' <typeparam name="T">The element type.</typeparam>
    <DebuggerDisplay("MatrixSize = {RowCount} x {ColumnCount}; RObjectType = {Type}")>
    <DebuggerTypeProxy(GetType(MatrixDebugView(Of)))>
    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
    Public MustInherit Class Matrix(Of T)
        Inherits SymbolicExpression
        ''' <summary>
        ''' Creates a new matrix with the specified size.
        ''' </summary>
        ''' <param name="engine">The engine.</param>
        ''' <param name="type">The element type.</param>
        ''' <param name="rowCount">The size of row.</param>
        ''' <param name="columnCount">The size of column.</param>
        Protected Sub New(ByVal engine As REngine, ByVal type As SymbolicExpressionType, ByVal rowCount As Integer, ByVal columnCount As Integer)
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
        Public Sub New(ByVal engine As REngine, ByVal type As SymbolicExpressionType, ByVal matrix As T(,))
            MyBase.New(engine, engine.GetFunction(Of Rf_allocMatrix)()(type, matrix.GetLength(0), matrix.GetLength(1)))
            Dim rowCount = Me.RowCount
            Dim columnCount = Me.ColumnCount
            'InitMatrixWithIndexers(matrix, rowCount, columnCount);
            InitMatrixFast(matrix)
        End Sub

        Private Sub InitMatrixFast(ByVal matrix As T(,))
            Using New ProtectedPointer(Me)
                InitMatrixFastDirect(matrix)
            End Using
        End Sub

        ''' <summary>
        ''' Initializes this R matrix, using the values in a rectangular array.
        ''' </summary>
        ''' <param name="matrix"></param>
        Protected MustOverride Sub InitMatrixFastDirect(ByVal matrix As T(,))

        ''' <summary>
        ''' Creates a new instance for a matrix.
        ''' </summary>
        ''' <param name="engine">The engine.</param>
        ''' <param name="coerced">The pointer to a matrix.</param>
        Protected Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
            MyBase.New(engine, coerced)
        End Sub

        ''' <summary>
        ''' Gets or sets the element at the specified index.
        ''' </summary>
        ''' <param name="rowIndex">The zero-based row index of the element to get or set.</param>
        ''' <param name="columnIndex">The zero-based column index of the element to get or set.</param>
        ''' <returns>The element at the specified index.</returns>
        Default Public MustOverride Property Item(ByVal rowIndex As Integer, ByVal columnIndex As Integer) As T

        ''' <summary>
        ''' Gets or sets the element at the specified names.
        ''' </summary>
        ''' <param name="rowName">The row name of the element to get or set.</param>
        ''' <param name="columnName">The column name of the element to get or set.</param>
        ''' <returns>The element at the specified names.</returns>
        Default Public Overridable Property Item(ByVal rowName As String, ByVal columnName As String) As T
            Get

                If Equals(rowName, Nothing) Then
                    Throw New ArgumentNullException("rowName")
                End If

                If Equals(columnName, Nothing) Then
                    Throw New ArgumentNullException("columnName")
                End If

                Dim rowNames = Me.RowNames

                If rowNames Is Nothing Then
                    Throw New InvalidOperationException()
                End If

                Dim columnNames = Me.ColumnNames

                If columnNames Is Nothing Then
                    Throw New InvalidOperationException()
                End If

                Dim rowIndex = Array.IndexOf(rowNames, rowName)
                Dim columnIndex = Array.IndexOf(columnNames, columnName)
                Return Me(rowIndex, columnIndex)
            End Get
            Set(ByVal value As T)

                If Equals(rowName, Nothing) Then
                    Throw New ArgumentNullException("rowName")
                End If

                If Equals(columnName, Nothing) Then
                    Throw New ArgumentNullException("columnName")
                End If

                Dim rowNames = Me.RowNames

                If rowNames Is Nothing Then
                    Throw New InvalidOperationException()
                End If

                Dim columnNames = Me.ColumnNames

                If columnNames Is Nothing Then
                    Throw New InvalidOperationException()
                End If

                Dim rowIndex = Array.IndexOf(rowNames, rowName)
                Dim columnIndex = Array.IndexOf(columnNames, columnName)
                Me(rowIndex, columnIndex) = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the row size of elements.
        ''' </summary>
        Public ReadOnly Property RowCount As Integer
            Get
                Return GetFunction(Of Rf_nrows)()(handle)
            End Get
        End Property

        ''' <summary>
        ''' Gets the column size of elements.
        ''' </summary>
        Public ReadOnly Property ColumnCount As Integer
            Get
                Return GetFunction(Of Rf_ncols)()(handle)
            End Get
        End Property

        ''' <summary>
        ''' Gets the total number of items (rows times columns) in this matrix
        ''' </summary>
        Public ReadOnly Property ItemCount As Integer
            Get
                Return RowCount * ColumnCount
            End Get
        End Property

        ''' <summary>
        ''' Gets the names of rows.
        ''' </summary>
        Public ReadOnly Property RowNames As String()
            Get
                Dim dimnamesSymbol = Engine.GetPredefinedSymbol("R_DimNamesSymbol")
                Dim dimnames = GetAttribute(dimnamesSymbol)

                If dimnames Is Nothing Then
                    Return Nothing
                End If

                Dim lRowNames As CharacterVector = dimnames.AsList()(0).AsCharacter()

                If lRowNames Is Nothing Then
                    Return Nothing
                End If

                Dim length = lRowNames.Length
                Dim result = New String(length - 1) {}
                lRowNames.CopyTo(result, length)
                Return result
            End Get
        End Property

        ''' <summary>
        ''' Gets the names of columns.
        ''' </summary>
        Public ReadOnly Property ColumnNames As String()
            Get
                Dim dimnamesSymbol = Engine.GetPredefinedSymbol("R_DimNamesSymbol")
                Dim dimnames = GetAttribute(dimnamesSymbol)

                If dimnames Is Nothing Then
                    Return Nothing
                End If

                Dim lColumnNames As CharacterVector = dimnames.AsList()(1).AsCharacter()

                If lColumnNames Is Nothing Then
                    Return Nothing
                End If

                Dim length = lColumnNames.Length
                Dim result = New String(length - 1) {}
                lColumnNames.CopyTo(result, length)
                Return result
            End Get
        End Property

        ''' <summary>
        ''' Gets the pointer for the first element.
        ''' </summary>
        Protected ReadOnly Property DataPointer As IntPtr
            Get

                Select Case Engine.Compatibility
                    Case REngine.CompatibilityMode.ALTREP
                        Return GetFunction(Of DATAPTR_OR_NULL)()(DangerousGetHandle())
                    Case REngine.CompatibilityMode.PreALTREP
                        Return IntPtr.Add(handle, Marshal.SizeOf(GetType(PreALTREP.VECTOR_SEXPREC)))
                    Case Else
                        Throw New MemberAccessException("Unable to translate the DataPointer for this R compatibility mode")
                End Select
            End Get
        End Property

        ''' <summary>
        ''' Gets the size of an element in byte.
        ''' </summary>
        Protected MustOverride ReadOnly Property DataSize As Integer

        ''' <summary>
        ''' Gets the offset for the specified indexes.
        ''' </summary>
        ''' <param name="rowIndex">The index of row.</param>
        ''' <param name="columnIndex">The index of column.</param>
        ''' <returns>The offset.</returns>
        Protected Function GetOffset(ByVal rowIndex As Integer, ByVal columnIndex As Integer) As Integer
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
        Public Sub CopyTo(ByVal destination As T(,), ByVal rowCount As Integer, ByVal columnCount As Integer, ByVal Optional sourceRowIndex As Integer = 0, ByVal Optional sourceColumnIndex As Integer = 0, ByVal Optional destinationRowIndex As Integer = 0, ByVal Optional destinationColumnIndex As Integer = 0)
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

            While Threading.Interlocked.Decrement(rowCount) >= 0
                Dim currentSourceRowIndex = Math.Min(Threading.Interlocked.Increment(sourceRowIndex), sourceRowIndex - 1)
                Dim currentDestinationRowIndex = Math.Min(Threading.Interlocked.Increment(destinationRowIndex), destinationRowIndex - 1)
                Dim currentColumnCount = columnCount
                Dim currentSourceColumnIndex = sourceColumnIndex
                Dim currentDestinationColumnIndex = destinationColumnIndex

                While Threading.Interlocked.Decrement(currentColumnCount) >= 0
                    destination(currentDestinationRowIndex, Math.Min(Threading.Interlocked.Increment(currentDestinationColumnIndex), currentDestinationColumnIndex - 1)) = Me(currentSourceRowIndex, Math.Min(Threading.Interlocked.Increment(currentSourceColumnIndex), currentSourceColumnIndex - 1))
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

