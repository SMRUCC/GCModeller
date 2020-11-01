Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports RDotNet.Internals
Imports RDotNet.Utilities

''' <summary>
''' A matrix of Boolean values.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Class LogicalMatrix
    Inherits Matrix(Of Boolean)
    ''' <summary>
    ''' Creates a new empty LogicalMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <seealso cref="REngineExtension.CreateLogicalMatrix(REngine,Integer,Integer)"/>
    Public Sub New(ByVal engine As REngine, ByVal rowCount As Integer, ByVal columnCount As Integer)
        MyBase.New(engine, SymbolicExpressionType.LogicalVector, rowCount, columnCount)
    End Sub

    ''' <summary>
    ''' Creates a new LogicalMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="matrix">The values.</param>
    ''' <seealso cref="REngineExtension.CreateLogicalMatrix"/>
    Public Sub New(ByVal engine As REngine, ByVal matrix As Boolean(,))
        MyBase.New(engine, SymbolicExpressionType.LogicalVector, matrix)
    End Sub

    ''' <summary>
    ''' Creates a new instance for a Boolean matrix.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="coerced">The pointer to a Boolean matrix.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
        MyBase.New(engine, coerced)
    End Sub

    ''' <summary>
    ''' Gets or sets the element at the specified index.
    ''' </summary>
    ''' <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
    ''' <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
    ''' <returns>The element at the specified index.</returns>
    Default Public Overrides Property Item(ByVal rowIndex As Integer, ByVal columnIndex As Integer) As Boolean
        Get

            If rowIndex < 0 OrElse RowCount <= rowIndex Then
                Throw New ArgumentOutOfRangeException("rowIndex")
            End If

            If columnIndex < 0 OrElse ColumnCount <= columnIndex Then
                Throw New ArgumentOutOfRangeException("columnIndex")
            End If

            Using New ProtectedPointer(Me)
                Dim offset = GetOffset(rowIndex, columnIndex)
                Dim data = Marshal.ReadInt32(DataPointer, offset)
                Return Convert.ToBoolean(data)
            End Using
        End Get
        Set(ByVal value As Boolean)

            If rowIndex < 0 OrElse RowCount <= rowIndex Then
                Throw New ArgumentOutOfRangeException("rowIndex")
            End If

            If columnIndex < 0 OrElse ColumnCount <= columnIndex Then
                Throw New ArgumentOutOfRangeException("columnIndex")
            End If

            Using New ProtectedPointer(Me)
                Dim offset = GetOffset(rowIndex, columnIndex)
                Dim data = Convert.ToInt32(value)
                Marshal.WriteInt32(DataPointer, offset, data)
            End Using
        End Set
    End Property

    ''' <summary>
    ''' Initializes this R matrix, using the values in a rectangular array.
    ''' </summary>
    ''' <param name="matrix"></param>
    Protected Overrides Sub InitMatrixFastDirect(ByVal matrix As Boolean(,))
        Dim intValues = ArrayConvertAllOneDim(matrix, New Func(Of Boolean, Integer)(AddressOf Convert.ToInt32))
        Marshal.Copy(intValues, 0, DataPointer, intValues.Length)
    End Sub

    ''' <summary>
    ''' Gets a rectangular array representation in the CLR, equivalent of a matrix in R.
    ''' </summary>
    ''' <returns>Rectangular array with values representing the content of the R matrix. Beware NA codes</returns>
    Protected Overrides Function GetArrayFast() As Boolean(,)
        Dim intValues = New Integer(ItemCount - 1) {}
        Marshal.Copy(DataPointer, intValues, 0, intValues.Length)
        Return ArrayConvertAllTwoDim(intValues, New Func(Of Integer, Boolean)(AddressOf Convert.ToBoolean), RowCount, ColumnCount)
    End Function

    ''' <summary>
    ''' Gets the size of an integer in byte.
    ''' </summary>
    Protected Overrides ReadOnly Property DataSize As Integer
        Get
            Return Marshal.SizeOf(GetType(Integer))
        End Get
    End Property
End Class

