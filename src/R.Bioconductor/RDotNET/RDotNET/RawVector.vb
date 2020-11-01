Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports RDotNet.Internals

''' <summary>
''' A sequence of byte values.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Class RawVector
    Inherits Vector(Of Byte)
    ''' <summary>
    ''' Creates a new RawVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="length">The length.</param>
    Public Sub New(ByVal engine As REngine, ByVal length As Integer)
        MyBase.New(engine, SymbolicExpressionType.RawVector, length)
    End Sub

    ''' <summary>
    ''' Creates a new RawVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateRawVector"/>
    Public Sub New(ByVal engine As REngine, ByVal vector As IEnumerable(Of Byte))
        MyBase.New(engine, SymbolicExpressionType.RawVector, vector)
    End Sub

    ''' <summary>
    ''' Creates a new RawVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateRawVector(REngine,Integer)"/>
    Public Sub New(ByVal engine As REngine, ByVal vector As Byte())
        MyBase.New(engine, SymbolicExpressionType.RawVector, vector.Length)
        Marshal.Copy(vector, 0, DataPointer, vector.Length)
    End Sub

    ''' <summary>
    ''' Creates a new instance for a raw vector.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="coerced">The pointer to a raw vector.</param>
    ''' <seealso cref="REngineExtension.CreateRawVector"/>
    Protected Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
        MyBase.New(engine, coerced)
    End Sub

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overrides Function GetValue(ByVal index As Integer) As Byte
        Dim offset = GetOffset(index)
        Return Marshal.ReadByte(DataPointer, offset)
    End Function

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overrides Function GetValueAltRep(ByVal index As Integer) As Byte
        Return GetFunction(Of RAW_ELT)()(DangerousGetHandle(), CType(index, IntPtr))
    End Function

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValue(ByVal index As Integer, ByVal value As Byte)
        Dim offset = GetOffset(index)
        Marshal.WriteByte(DataPointer, offset, value)
    End Sub

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValueAltRep(ByVal index As Integer, ByVal value As Byte)
        GetFunction(Of SET_RAW_ELT)()(DangerousGetHandle(), CType(index, IntPtr), value)
    End Sub

    ''' <summary>
    ''' Efficient conversion from R vector representation to the array equivalent in the CLR
    ''' </summary>
    ''' <returns>Array equivalent</returns>
    Protected Overrides Function GetArrayFast() As Byte()
        Dim res = New Byte(Length - 1) {}
        Marshal.Copy(DataPointer, res, 0, res.Length)
        Return res
    End Function

    ''' <summary>
    ''' Sets the values of this RawVector
    ''' </summary>
    ''' <param name="values">Managed values, to be converted to unmanaged equivalent</param>
    Protected Overrides Sub SetVectorDirect(ByVal values As Byte())
        Marshal.Copy(values, 0, DataPointer, values.Length)
    End Sub

    ''' <summary>
    ''' Gets the size of a byte value in byte.
    ''' </summary>
    Protected Overrides ReadOnly Property DataSize As Integer
        Get
            Return Marshal.SizeOf(GetType(Byte))
        End Get
    End Property

    ''' <summary>
    ''' Copies the elements to the specified array.
    ''' </summary>
    ''' <param name="destination">The destination array.</param>
    ''' <param name="length">The length to copy.</param>
    ''' <param name="sourceIndex">The first index of the vector.</param>
    ''' <param name="destinationIndex">The first index of the destination array.</param>
    Public Overloads Sub CopyTo(ByVal destination As Byte(), ByVal length As Integer, ByVal Optional sourceIndex As Integer = 0, ByVal Optional destinationIndex As Integer = 0)
        If destination Is Nothing Then
            Throw New ArgumentNullException("destination")
        End If

        If length < 0 Then
            Throw New IndexOutOfRangeException("length")
        End If

        If sourceIndex < 0 OrElse MyBase.Length < sourceIndex + length Then
            Throw New IndexOutOfRangeException("sourceIndex")
        End If

        If destinationIndex < 0 OrElse destination.Length < destinationIndex + length Then
            Throw New IndexOutOfRangeException("destinationIndex")
        End If

        Dim offset = GetOffset(sourceIndex)
        Dim pointer = IntPtr.Add(DataPointer, offset)
        Marshal.Copy(pointer, destination, destinationIndex, length)
    End Sub
End Class

