Imports RDotNet.Diagnostics
Imports RDotNet.Dynamic
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Dynamic
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Security.Permissions


''' <summary>
''' A data frame.
''' </summary>
<DebuggerDisplay("ColumnCount = {ColumnCount}; RowCount = {RowCount}; RObjectType = {Type}")>
<DebuggerTypeProxy(GetType(DataFrameDebugView))>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Class DataFrame
    Inherits Vector(Of DynamicVector)

    Private Const RRowNamesSymbolName As String = "R_RowNamesSymbol"

    ''' <summary>
    ''' Creates a new instance.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="coerced">The pointer to a data frame.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
        MyBase.New(engine, coerced)
    End Sub

    ''' <summary>
    ''' Gets or sets the column at the specified index as a vector.
    ''' </summary>
    ''' <param name="columnIndex">The zero-based index of the column to get or set.</param>
    ''' <returns>The column at the specified index.</returns>
    Default Public Overrides Property Item(ByVal columnIndex As Integer) As DynamicVector
        Get

            If columnIndex < 0 OrElse Length <= columnIndex Then
                Throw New ArgumentOutOfRangeException()
            End If

            Using New ProtectedPointer(Me)
                Return GetColumn(columnIndex)
            End Using
        End Get
        Set(ByVal value As DynamicVector)

            If columnIndex < 0 OrElse Length <= columnIndex Then
                Throw New ArgumentOutOfRangeException()
            End If

            Using New ProtectedPointer(Me)
                SetColumn(columnIndex, value)
            End Using
        End Set
    End Property

    ''' <summary>
    ''' Gets an array of the columns of this R data frame object
    ''' </summary>
    ''' <returns></returns>
    Protected Overrides Function GetArrayFast() As DynamicVector()
        Dim res = New DynamicVector(Length - 1) {}

        For i = 0 To res.Length - 1
            res(i) = GetColumn(i)
        Next

        Return res
    End Function

    Private Function GetColumn(ByVal columnIndex As Integer) As DynamicVector
        Dim offset = GetOffset(columnIndex)
        Dim pointer = Marshal.ReadIntPtr(DataPointer, offset)
        Return New DynamicVector(Engine, pointer)
    End Function

    Private Sub SetColumn(ByVal columnIndex As Integer, ByVal value As DynamicVector)
        Dim offset = GetOffset(columnIndex)
        Marshal.WriteIntPtr(DataPointer, offset, If(value, Engine.NilValue).DangerousGetHandle())
    End Sub

    ''' <summary>
    ''' Efficient initialisation of R vector values from an array representation in the CLR
    ''' </summary>
    Protected Overrides Sub SetVectorDirect(ByVal values As DynamicVector())
        For i = 0 To values.Length - 1
            SetColumn(i, values(i))
        Next
    End Sub

    ''' <summary>
    ''' Gets or sets the element at the specified indexes.
    ''' </summary>
    ''' <param name="rowIndex">The row index.</param>
    ''' <param name="columnIndex">The column index.</param>
    ''' <returns>The element.</returns>
    Default Public Overloads Property Item(ByVal rowIndex As Integer, ByVal columnIndex As Integer) As Object
        Get
            Dim column = Me(columnIndex)
            Return column(rowIndex)
        End Get
        Set(ByVal value As Object)
            Dim column = Me(columnIndex)
            column(rowIndex) = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the element at the specified index and name.
    ''' </summary>
    ''' <param name="rowIndex">The row index.</param>
    ''' <param name="columnName">The column name.</param>
    ''' <returns>The element.</returns>
    Default Public Overloads Property Item(ByVal rowIndex As Integer, ByVal columnName As String) As Object
        Get
            Dim column = Me(columnName)
            Return column(rowIndex)
        End Get
        Set(ByVal value As Object)
            Dim column = Me(columnName)
            column(rowIndex) = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the element at the specified names.
    ''' </summary>
    ''' <param name="rowName">The row name.</param>
    ''' <param name="columnName">The column name.</param>
    ''' <returns>The element.</returns>
    Default Public Overloads Property Item(ByVal rowName As String, ByVal columnName As String) As Object
        Get
            Dim column = Me(columnName)
            Return column(rowName)
        End Get
        Set(ByVal value As Object)
            Dim column = Me(columnName)
            column(rowName) = value
        End Set
    End Property

    ''' <summary>
    ''' Gets the number of data sets.
    ''' </summary>
    Public ReadOnly Property RowCount As Integer
        Get
            Return If(ColumnCount = 0, 0, Me(0).Length)
        End Get
    End Property

    ''' <summary>
    ''' Gets the number of kinds of data.
    ''' </summary>
    Public ReadOnly Property ColumnCount As Integer
        Get
            Return Length
        End Get
    End Property

    ''' <summary>
    ''' Gets the names of rows.
    ''' </summary>
    Public ReadOnly Property RowNames As String()
        Get
            Dim rowNamesSymbol = Engine.GetPredefinedSymbol(RRowNamesSymbolName)
            Dim lRowNames = GetAttribute(rowNamesSymbol)

            If lRowNames Is Nothing Then
                Return Nothing
            End If

            Dim rowNamesVector As CharacterVector = lRowNames.AsCharacter()

            If rowNamesVector Is Nothing Then
                Return Nothing
            End If

            Dim length = rowNamesVector.Length
            Dim result = New String(length - 1) {}
            rowNamesVector.CopyTo(result, length)
            Return result
        End Get
    End Property

    ''' <summary>
    ''' Gets the names of columns.
    ''' </summary>
    Public ReadOnly Property ColumnNames As String()
        Get
            Return Names
        End Get
    End Property

    ''' <summary>
    ''' Gets the data size of each element in this vector, i.e. the offset in memory between elements.
    ''' </summary>
    Protected Overrides ReadOnly Property DataSize As Integer
        Get
            Return Marshal.SizeOf(GetType(IntPtr))
        End Get
    End Property

    ''' <summary>
    ''' Gets the row at the specified index.
    ''' </summary>
    ''' <param name="rowIndex">The index.</param>
    ''' <returns>The row.</returns>
    Public Function GetRow(ByVal rowIndex As Integer) As DataFrameRow
        Return New DataFrameRow(Me, rowIndex)
    End Function

    ''' <summary>
    ''' Gets the row at the specified index mapping a specified class.
    ''' </summary>
    ''' <typeparam name="TRow">The row type with <see cref="DataFrameRowAttribute"/>.</typeparam>
    ''' <returns>The row.</returns>
    Public Function GetRow(Of TRow As {Class, New})(ByVal rowIndex As Integer) As TRow
        Dim rowType = GetType(TRow)
        Dim attribute = CType(rowType.GetCustomAttributes(GetType(DataFrameRowAttribute), False).[Single](), DataFrameRowAttribute)

        If attribute Is Nothing Then
            Throw New ArgumentException("DataFrameRowAttribute is required.")
        End If

        Dim row = GetRow(rowIndex)
        Return attribute.Convert(Of TRow)(row)
    End Function

    ''' <summary>
    ''' Enumerates all the rows in the data frame.
    ''' </summary>
    ''' <returns>The collection of the rows.</returns>
    Public Iterator Function GetRows() As IEnumerable(Of DataFrameRow)
        Dim rowCount = Me.RowCount

        For rowIndex = 0 To rowCount - 1
            Yield GetRow(rowIndex)
        Next
    End Function

    ''' <summary>
    ''' Enumerates all the rows in the data frame mapping a specified class.
    ''' </summary>
    ''' <typeparam name="TRow">The row type with <see cref="DataFrameRowAttribute"/>.</typeparam>
    ''' <returns>The collection of the rows.</returns>
    Public Iterator Function GetRows(Of TRow As {Class, New})() As IEnumerable(Of TRow)
        Dim rowType = GetType(TRow)
        Dim attribute = CType(rowType.GetCustomAttributes(GetType(DataFrameRowAttribute), False).[Single](), DataFrameRowAttribute)

        If attribute Is Nothing Then
            Throw New ArgumentException("DataFrameRowAttribute is required.")
        End If

        Dim rowCount = Me.RowCount

        For rowIndex = 0 To rowCount - 1
            Dim row = GetRow(rowIndex)
            Yield attribute.Convert(Of TRow)(row)
        Next
    End Function

    ''' <summary>
    ''' returns a new DataFrameDynamicMeta for this DataFrame
    ''' </summary>
    ''' <param name="parameter"></param>
    ''' <returns></returns>
    Public Overrides Function GetMetaObject(ByVal parameter As Expressions.Expression) As DynamicMetaObject
        Return New DataFrameDynamicMeta(parameter, Me)
    End Function
End Class

