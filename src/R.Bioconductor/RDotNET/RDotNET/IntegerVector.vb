Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports RDotNet.Internals

''' <summary>
''' A collection of integers from <c>-2^31 + 1</c> to <c>2^31 - 1</c>.
''' </summary>
''' <remarks>
''' The minimum value of IntegerVector is different from that of System.Int32 in .NET Framework.
''' </remarks>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Class IntegerVector
    Inherits Vector(Of Integer)
    ''' <summary>
    ''' Creates a new empty IntegerVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="length">The length.</param>
    ''' <seealso cref="REngineExtension.CreateIntegerVector(REngine,Integer)"/>
    Public Sub New(ByVal engine As REngine, ByVal length As Integer)
        MyBase.New(engine, SymbolicExpressionType.IntegerVector, length)
    End Sub

    ''' <summary>
    ''' Creates a new IntegerVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateIntegerVector"/>
    Public Sub New(ByVal engine As REngine, ByVal vector As IEnumerable(Of Integer))
        MyBase.New(engine, SymbolicExpressionType.IntegerVector, vector)
    End Sub

    ''' <summary>
    ''' Creates a new IntegerVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateIntegerVector"/>
    Public Sub New(ByVal engine As REngine, ByVal vector As Integer())
        MyBase.New(engine, SymbolicExpressionType.IntegerVector, vector.Length)
        Marshal.Copy(vector, 0, DataPointer, vector.Length)
    End Sub

    ''' <summary>
    ''' Creates a new instance for an integer vector.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="coerced">The pointer to an integer vector.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
        MyBase.New(engine, coerced)
    End Sub

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overrides Function GetValue(ByVal index As Integer) As Integer
        Dim offset = GetOffset(index)
        Return Marshal.ReadInt32(DataPointer, offset)
    End Function

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overrides Function GetValueAltRep(ByVal index As Integer) As Integer
        Return GetFunction(Of INTEGER_ELT)()(DangerousGetHandle(), CType(index, IntPtr))
    End Function

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValue(ByVal index As Integer, ByVal value As Integer)
        Dim offset = GetOffset(index)
        Marshal.WriteInt32(DataPointer, offset, value)
    End Sub

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValueAltRep(ByVal index As Integer, ByVal value As Integer)
        GetFunction(Of SET_INTEGER_ELT)()(DangerousGetHandle(), CType(index, IntPtr), value)
    End Sub

    ''' <summary>
    ''' Efficient conversion from R vector representation to the array equivalent in the CLR
    ''' </summary>
    ''' <returns>Array equivalent</returns>
    Protected Overrides Function GetArrayFast() As Integer()
        Dim res = New Integer(Length - 1) {}
        Marshal.Copy(DataPointer, res, 0, res.Length)
        Return res
    End Function

    ''' <summary> Gets alternate rep array.</summary>
    '''
    ''' <exception cref="NotSupportedException"> Thrown when the requested operation is not supported.</exception>
    '''
    ''' <returns> An array of t.</returns>
    Public Overrides Function GetAltRepArray() As Integer()
        ' by inference from `static SEXP compact_intseq_Duplicate(SEXP x, Rboolean deep)`  in altrep.c
        Dim res = New Integer(Length - 1) {}
        GetFunction(Of INTEGER_GET_REGION)()(DangerousGetHandle(), CType(0, IntPtr), CType(Length, IntPtr), res)
        Return res
    End Function


    ''' <summary>
    ''' Efficient initialisation of R vector values from an array representation in the CLR
    ''' </summary>
    Protected Overrides Sub SetVectorDirect(ByVal values As Integer())
        Marshal.Copy(values, 0, DataPointer, values.Length)
    End Sub

    ''' <summary>
    ''' Gets the size of an integer in byte.
    ''' </summary>
    Protected Overrides ReadOnly Property DataSize As Integer
        Get
            Return Marshal.SizeOf(GetType(Integer))
        End Get
    End Property

    ''' <summary>
    ''' Copies the elements to the specified array.
    ''' </summary>
    ''' <param name="destination">The destination array.</param>
    ''' <param name="length">The length to copy.</param>
    ''' <param name="sourceIndex">The first index of the vector.</param>
    ''' <param name="destinationIndex">The first index of the destination array.</param>
    Public Overloads Sub CopyTo(ByVal destination As Integer(), ByVal length As Integer, ByVal Optional sourceIndex As Integer = 0, ByVal Optional destinationIndex As Integer = 0)
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

    ''' <summary>
    ''' Gets the code used for NA for integer vectors
    ''' </summary>
    Protected ReadOnly Property NACode As Integer
        Get
            Return Integer.MinValue
        End Get
    End Property
End Class

