Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports RDotNet.Internals

''' <summary>
''' A collection of strings.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
Public Class CharacterVector
    Inherits Vector(Of String)
    ''' <summary>
    ''' Creates a new empty CharacterVector with the specified length.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="length">The length.</param>
    ''' <seealso cref="REngineExtension.CreateCharacterVector(REngine,Integer)"/>
    Public Sub New(ByVal engine As REngine, ByVal length As Integer)
        MyBase.New(engine, SymbolicExpressionType.CharacterVector, length)
    End Sub

    ''' <summary>
    ''' Creates a new CharacterVector with the specified values.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateCharacterVector"/>
    Public Sub New(ByVal engine As REngine, ByVal vector As IEnumerable(Of String))
        MyBase.New(engine, SymbolicExpressionType.CharacterVector, vector)
    End Sub

    ''' <summary>
    ''' Creates a new instance for a string vector.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="coerced">The pointer to a string vector.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
        MyBase.New(engine, coerced)
    End Sub


    ''' <summary>
    ''' Gets an array representation of this R vector. Note that the implementation is not as fast as for numeric vectors.
    ''' </summary>
    ''' <returns></returns>
    Protected Overrides Function GetArrayFast() As String()
        Dim n = Length
        Dim res = New String(n - 1) {}
        Dim useAltRep = Engine.Compatibility = REngine.CompatibilityMode.ALTREP

        For i = 0 To n - 1
            res(i) = If(useAltRep, GetValueAltRep(i), GetValue(i))
        Next

        Return res
    End Function

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overrides Function GetValueAltRep(ByVal index As Integer) As String
        ' To work with ALTREP (introduced in R 3.5.0) and non-ALTREP objects, we will get strings
        ' via STRING_ELT, instead of offseting the DataPointer.  This lets R manage the details of
        ' ALTREP conversion for us.
        Dim objPointer As IntPtr = GetFunction(Of STRING_ELT)()(DangerousGetHandle(), CType(index, IntPtr))

        If objPointer = Engine.NaStringPointer Then
            Return Nothing
        End If

        Dim stringData As IntPtr = GetFunction(Of DATAPTR_OR_NULL)()(objPointer)
        Return InternalString.StringFromNativeUtf8(stringData)
    End Function

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overrides Function GetValue(ByVal index As Integer) As String
        Dim offset = GetOffset(index)
        Dim pointerItem = Marshal.ReadIntPtr(DataPointer, offset)

        If pointerItem = Engine.NaStringPointer Then
            Return Nothing
        End If

        Dim pointer = IntPtr.Add(pointerItem, Marshal.SizeOf(GetType(PreALTREP.VECTOR_SEXPREC)))
        Return InternalString.StringFromNativeUtf8(pointer)
    End Function

    ''' <summary> Gets alternate rep array.</summary>
    '''
    ''' <exception cref="NotSupportedException"> Thrown when the requested operation is not supported.</exception>
    '''
    ''' <returns> An array of t.</returns>
    Public Overrides Function GetAltRepArray() As String()
        Return GetArrayFast()
    End Function

    Private _mkChar As Rf_mkChar = Nothing

    Private Function mkChar(ByVal value As String) As IntPtr
        If _mkChar Is Nothing Then _mkChar = GetFunction(Of Rf_mkChar)()
        Return _mkChar(value)
    End Function

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValueAltRep(ByVal index As Integer, ByVal value As String)
        SetValue(index, value)
    End Sub

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValue(ByVal index As Integer, ByVal value As String)
        Dim offset = GetOffset(index)
        Dim stringPointer = If(Equals(value, Nothing), Engine.NaStringPointer, mkChar(value))
        Marshal.WriteIntPtr(DataPointer, offset, stringPointer)
    End Sub

    ''' <summary>
    ''' Efficient initialisation of R vector values from an array representation in the CLR
    ''' </summary>
    Protected Overrides Sub SetVectorDirect(ByVal values As String())
        ' Possibly not the fastest implementation, but faster may require C code.
        ' TODO check the behavior of P/Invoke on array of strings (VT_ARRAY|VT_LPSTR?)
        Dim useAltRep = Engine.Compatibility = REngine.CompatibilityMode.ALTREP

        For i = 0 To values.Length - 1

            If useAltRep Then
                SetValueAltRep(i, values(i))
            End If

            If True Then
                SetValue(i, values(i))
            End If
        Next
    End Sub

    ''' <summary>
    ''' Gets the size of a pointer in byte.
    ''' </summary>
    Protected Overrides ReadOnly Property DataSize As Integer
        Get
            Return Marshal.SizeOf(GetType(IntPtr))
        End Get
    End Property
End Class

