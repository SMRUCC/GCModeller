Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports RDotNet.Internals

''' <summary>
''' A matrix of strings.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Class CharacterMatrix
    Inherits Matrix(Of String)
    ''' <summary>
    ''' Creates a new empty CharacterMatrix with the specified size.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="rowCount">The row size.</param>
    ''' <param name="columnCount">The column size.</param>
    ''' <seealso cref="REngineExtension.CreateCharacterMatrix(REngine,Integer,Integer)"/>
    Public Sub New(ByVal engine As REngine, ByVal rowCount As Integer, ByVal columnCount As Integer)
        MyBase.New(engine, SymbolicExpressionType.CharacterVector, rowCount, columnCount)
    End Sub

    ''' <summary>
    ''' Creates a new CharacterMatrix with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="matrix">The values.</param>
    ''' <seealso cref="REngineExtension.CreateCharacterMatrix"/>
    Public Sub New(ByVal engine As REngine, ByVal matrix As String(,))
        MyBase.New(engine, SymbolicExpressionType.CharacterVector, matrix)
    End Sub

    ''' <summary>
    ''' Creates a new instance for a string matrix.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="coerced">The pointer to a string matrix.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
        MyBase.New(engine, coerced)
    End Sub

    ''' <summary>
    ''' Gets or sets the element at the specified index.
    ''' </summary>
    ''' <param name="rowIndex">The zero-based rowIndex index of the element to get or set.</param>
    ''' <param name="columnIndex">The zero-based columnIndex index of the element to get or set.</param>
    ''' <returns>The element at the specified index.</returns>
    Default Public Overrides Property Item(ByVal rowIndex As Integer, ByVal columnIndex As Integer) As String
        Get

            If rowIndex < 0 OrElse RowCount <= rowIndex Then
                Throw New ArgumentOutOfRangeException("rowIndex")
            End If

            If columnIndex < 0 OrElse ColumnCount <= columnIndex Then
                Throw New ArgumentOutOfRangeException("columnIndex")
            End If

            Using New ProtectedPointer(Me)
                Dim offset = GetOffset(rowIndex, columnIndex)
                Dim pointer = Marshal.ReadIntPtr(DataPointer, offset)
                Return New InternalString(Engine, pointer)
            End Using
        End Get
        Set(ByVal value As String)

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

    Private Sub SetValue(ByVal rowIndex As Integer, ByVal columnIndex As Integer, ByVal value As String)
        Dim offset = GetOffset(rowIndex, columnIndex)
        Dim s As SymbolicExpression = If(Equals(value, Nothing), Engine.NilValue, New InternalString(Engine, value))

        Using New ProtectedPointer(s)
            Marshal.WriteIntPtr(DataPointer, offset, s.DangerousGetHandle())
        End Using
    End Sub

    ''' <summary>
    ''' Initializes this R matrix, using the values in a rectangular array.
    ''' </summary>
    ''' <param name="matrix"></param>
    Protected Overrides Sub InitMatrixFastDirect(ByVal matrix As String(,))
        Dim rows = matrix.GetLength(0)
        Dim cols = matrix.GetLength(1)

        For i = 0 To rows - 1

            For j = 0 To cols - 1
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
    Protected Overrides ReadOnly Property DataSize As Integer
        Get
            Return Marshal.SizeOf(GetType(IntPtr))
        End Get
    End Property
End Class

