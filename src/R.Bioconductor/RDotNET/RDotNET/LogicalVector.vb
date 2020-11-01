Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports RDotNet.Internals

''' <summary>
''' A collection of Boolean values.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Class LogicalVector
    Inherits Vector(Of Boolean)
    ''' <summary>
    ''' Creates a new empty LogicalVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="length">The length.</param>
    ''' <seealso cref="REngineExtension.CreateLogicalVector(REngine,Integer)"/>
    Public Sub New(ByVal engine As REngine, ByVal length As Integer)
        MyBase.New(engine, SymbolicExpressionType.LogicalVector, length)
    End Sub

    ''' <summary>
    ''' Creates a new LogicalVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateLogicalVector"/>
    Public Sub New(ByVal engine As REngine, ByVal vector As IEnumerable(Of Boolean))
        MyBase.New(engine, SymbolicExpressionType.LogicalVector, vector)
    End Sub

    ''' <summary>
    ''' Creates a new instance for a Boolean vector.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="coerced">The pointer to a Boolean vector.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
        MyBase.New(engine, coerced)
    End Sub

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overrides Function GetValue(ByVal index As Integer) As Boolean
        Dim offset = GetOffset(index)
        Dim data = Marshal.ReadInt32(DataPointer, offset)
        Return Convert.ToBoolean(data)
    End Function

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overrides Function GetValueAltRep(ByVal index As Integer) As Boolean
        Dim data = GetFunction(Of LOGICAL_ELT)()(DangerousGetHandle(), CType(index, IntPtr))
        Return Convert.ToBoolean(data)
    End Function

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValue(ByVal index As Integer, ByVal value As Boolean)
        Dim offset = GetOffset(index)
        Dim data = Convert.ToInt32(value)
        Marshal.WriteInt32(DataPointer, offset, data)
    End Sub

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValueAltRep(ByVal index As Integer, ByVal value As Boolean)
        Dim data = Convert.ToInt32(value)
        GetFunction(Of SET_LOGICAL_ELT)()(DangerousGetHandle(), CType(index, IntPtr), data)
    End Sub

    ''' <summary>
    ''' Efficient conversion from R vector representation to the array equivalent in the CLR
    ''' </summary>
    ''' <returns>Array equivalent</returns>
    Protected Overrides Function GetArrayFast() As Boolean()
        Dim intValues = New Integer(Length - 1) {}
        Marshal.Copy(DataPointer, intValues, 0, intValues.Length)
        Return Array.ConvertAll(intValues, New Converter(Of Integer, Boolean)(AddressOf Convert.ToBoolean))
    End Function

    ''' <summary>
    ''' Efficient initialisation of R vector values from an array representation in the CLR
    ''' </summary>
    Protected Overrides Sub SetVectorDirect(ByVal values As Boolean())
        Dim intValues = Array.ConvertAll(values, New Converter(Of Boolean, Integer)(AddressOf Convert.ToInt32))
        Marshal.Copy(intValues, 0, DataPointer, values.Length)
    End Sub

    ''' <summary>
    ''' Gets the size of a Boolean value in byte.
    ''' </summary>
    Protected Overrides ReadOnly Property DataSize As Integer
        Get
            ' Boolean is int internally.
            Return Marshal.SizeOf(GetType(Integer))
        End Get
    End Property
End Class

