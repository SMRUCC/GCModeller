Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports RDotNet.Internals
Imports RDotNet.Utilities

''' <summary>
''' A matrix of real numbers in double precision.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Class NumericMatrix
    Inherits Matrix(Of Double)
    ''' <summary>
    ''' Creates a new empty NumericMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <seealso cref="REngineExtension.CreateNumericMatrix(REngine,Integer,Integer)"/>
    Public Sub New(ByVal engine As REngine, ByVal rowCount As Integer, ByVal columnCount As Integer)
        MyBase.New(engine, SymbolicExpressionType.NumericVector, rowCount, columnCount)
    End Sub

    ''' <summary>
    ''' Creates a new NumericMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="matrix">The values.</param>
    ''' <seealso cref="REngineExtension.CreateNumericMatrix"/>
    Public Sub New(ByVal engine As REngine, ByVal matrix As Double(,))
        MyBase.New(engine, SymbolicExpressionType.NumericVector, matrix)
    End Sub

    ''' <summary>
    ''' Creates a new instance for a numeric matrix.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="coerced">The pointer to a numeric matrix.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
        MyBase.New(engine, coerced)
    End Sub

    ''' <summary>
    ''' Gets or sets the element at the specified index.
    ''' </summary>
    ''' <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
    ''' <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
    ''' <returns>The element at the specified index.</returns>
    Default Public Overrides Property Item(ByVal rowIndex As Integer, ByVal columnIndex As Integer) As Double
        Get

            If rowIndex < 0 OrElse RowCount <= rowIndex Then
                Throw New ArgumentOutOfRangeException("rowIndex")
            End If

            If columnIndex < 0 OrElse ColumnCount <= columnIndex Then
                Throw New ArgumentOutOfRangeException("columnIndex")
            End If

            Using New ProtectedPointer(Me)
                Dim data = New Byte(DataSize - 1) {}
                Dim pointer = DataPointer
                Dim offset = GetOffset(rowIndex, columnIndex)

                For byteIndex = 0 To data.Length - 1
                    data(byteIndex) = Marshal.ReadByte(pointer, offset + byteIndex)
                Next

                Return BitConverter.ToDouble(data, 0)
            End Using
        End Get
        Set(ByVal value As Double)

            If rowIndex < 0 OrElse RowCount <= rowIndex Then
                Throw New ArgumentOutOfRangeException("rowIndex")
            End If

            If columnIndex < 0 OrElse ColumnCount <= columnIndex Then
                Throw New ArgumentOutOfRangeException("columnIndex")
            End If

            Using New ProtectedPointer(Me)
                Dim data = BitConverter.GetBytes(value)
                Dim pointer = DataPointer
                Dim offset = GetOffset(rowIndex, columnIndex)

                For byteIndex = 0 To data.Length - 1
                    Marshal.WriteByte(pointer, offset + byteIndex, data(byteIndex))
                Next
            End Using
        End Set
    End Property

    ''' <summary>
    ''' Initializes this R matrix, using the values in a rectangular array.
    ''' </summary>
    ''' <param name="matrix"></param>
    Protected Overrides Sub InitMatrixFastDirect(ByVal matrix As Double(,))
        Dim values = ArrayConvertOneDim(matrix)
        Marshal.Copy(values, 0, DataPointer, values.Length)
    End Sub

    ''' <summary>
    ''' Gets a rectangular array representation in the CLR, equivalent of a matrix in R.
    ''' </summary>
    ''' <returns>Rectangular array with values representing the content of the R matrix. Beware NA codes</returns>
    Protected Overrides Function GetArrayFast() As Double(,)
        Dim values = New Double(ItemCount - 1) {}
        Marshal.Copy(DataPointer, values, 0, values.Length)
        Return ArrayConvertAllTwoDim(values, RowCount, ColumnCount)
    End Function

    ''' <summary>
    ''' Gets the size of a real number in byte.
    ''' </summary>
    Protected Overrides ReadOnly Property DataSize As Integer
        Get
            Return Marshal.SizeOf(GetType(Double))
        End Get
    End Property
End Class

