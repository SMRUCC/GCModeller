Imports System.Numerics
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports RDotNet.Internals
Imports RDotNet.Utilities

''' <summary>
''' A collection of complex numbers.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Class ComplexVector
    Inherits Vector(Of Complex)
    ''' <summary>
    ''' Creates a new empty ComplexVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="length">The length.</param>
    ''' <seealso cref="REngineExtension.CreateComplexVector(REngine,Integer)"/>
    Public Sub New(ByVal engine As REngine, ByVal length As Integer)
        MyBase.New(engine, SymbolicExpressionType.ComplexVector, length)
    End Sub

    ''' <summary>
    ''' Creates a new ComplexVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateComplexVector"/>
    Public Sub New(ByVal engine As REngine, ByVal vector As IEnumerable(Of Complex))
        MyBase.New(engine, SymbolicExpressionType.ComplexVector, vector)
    End Sub

    ''' <summary>
    ''' Creates a new instance for a complex number vector.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="coerced">The pointer to a complex number vector.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
        MyBase.New(engine, coerced)
    End Sub

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overrides Function GetValue(ByVal index As Integer) As Complex
        Dim data = New Double(1) {}
        Dim offset = GetOffset(index)
        Dim pointer = IntPtr.Add(DataPointer, offset)
        Marshal.Copy(pointer, data, 0, data.Length)
        Return New Complex(data(0), data(1))
    End Function

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overrides Function GetValueAltRep(ByVal index As Integer) As Complex
        Dim data = GetFunction(Of COMPLEX_ELT)()(DangerousGetHandle(), CType(index, IntPtr))
        Return New Complex(data.r, data.i)
    End Function

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValue(ByVal index As Integer, ByVal value As Complex)
        Dim data = {value.Real, value.Imaginary}
        Dim offset = GetOffset(index)
        Dim pointer = IntPtr.Add(DataPointer, offset)
        Marshal.Copy(data, 0, pointer, data.Length)
    End Sub

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValueAltRep(ByVal index As Integer, ByVal value As Complex)
        GetFunction(Of SET_COMPLEX_ELT)()(DangerousGetHandle(), CType(index, IntPtr), SerializeComplexToRComplex(value))
    End Sub

    ''' <summary>
    ''' Gets an array representation in the CLR of a vector in R.
    ''' </summary>
    ''' <returns></returns>
    Protected Overrides Function GetArrayFast() As Complex()
        Dim n = Length
        Dim data = New Double(2 * n - 1) {}
        Marshal.Copy(DataPointer, data, 0, 2 * n)
        Return DeserializeComplexFromDouble(data)
    End Function

    ''' <summary>
    ''' Efficient initialisation of R vector values from an array representation in the CLR
    ''' </summary>
    Protected Overrides Sub SetVectorDirect(ByVal values As Complex())
        Dim data = SerializeComplexToDouble(values)
        Dim pointer = IntPtr.Add(DataPointer, 0)
        Marshal.Copy(data, 0, pointer, data.Length)
    End Sub

    ''' <summary>
    ''' Gets the size of a complex number in byte.
    ''' </summary>
    Protected Overrides ReadOnly Property DataSize As Integer
        Get
            Return Marshal.SizeOf(GetType(Complex))
        End Get
    End Property
End Class

