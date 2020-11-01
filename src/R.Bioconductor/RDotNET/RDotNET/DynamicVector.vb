Imports RDotNet.Internals
Imports System
Imports System.Numerics
Imports System.Runtime.InteropServices


''' <summary>
''' A collection of values.
''' </summary>
''' <remarks>
''' This vector cannot contain more than one types of values.
''' Consider to use another vector class instead.
''' </remarks>
Public Class DynamicVector
    Inherits Vector(Of Object)
    ''' <summary>
    ''' Creates a container for a collection of values
    ''' </summary>
    ''' <param name="engine">The R engine</param>
    ''' <param name="coerced">Pointer to the native R object, coerced to the appropriate type</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal coerced As IntPtr)
        MyBase.New(engine, coerced)
    End Sub

    ''' <summary>
    ''' Gets an array representation of a vector in R. Note that the implementation cannot be particularly "fast" in spite of the name.
    ''' </summary>
    ''' <returns></returns>
    Protected Overrides Function GetArrayFast() As Object()
        Dim res = New Object(Length - 1) {}
        Dim useAltRep = Engine.Compatibility = REngine.CompatibilityMode.ALTREP

        For i = 0 To res.Length - 1
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
    Protected Overrides Function GetValueAltRep(ByVal index As Integer) As Object
        Return GetValue(index)
    End Function

    ''' <summary>
    ''' Gets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to get.</param>
    ''' <returns>The element at the specified index.</returns>
    Protected Overrides Function GetValue(ByVal index As Integer) As Object
        Dim pointer = DataPointer
        Dim offset = GetOffset(index)

        Select Case Type
            Case SymbolicExpressionType.NumericVector
                Return ReadDouble(pointer, offset)
            Case SymbolicExpressionType.IntegerVector

                If IsFactor() Then
                    Return AsFactor().GetFactor(index)
                Else
                    Return ReadInt32(pointer, offset)
                End If

            Case SymbolicExpressionType.CharacterVector
                Return ReadString(pointer, offset)
            Case SymbolicExpressionType.LogicalVector
                Return ReadBoolean(pointer, offset)
            Case SymbolicExpressionType.RawVector
                Return ReadByte(pointer, offset)
            Case SymbolicExpressionType.ComplexVector
                Return ReadComplex(pointer, offset)
            Case Else
                Return ReadSymbolicExpression(pointer, offset)
        End Select
    End Function

    ''' <summary>
    ''' Efficient initialisation of R vector values from an array representation in the CLR
    ''' </summary>
    Protected Overrides Sub SetVectorDirect(ByVal values As Object())
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
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for R 3.5 and higher, to account for ALTREP objects</remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValueAltRep(ByVal index As Integer, ByVal value As Object)
        SetValue(index, value)
    End Sub

    ''' <summary>
    ''' Sets the element at the specified index.
    ''' </summary>
    ''' <remarks>Used for pre-R 3.5 </remarks>
    ''' <param name="index">The zero-based index of the element to set.</param>
    ''' <param name="value">The value to set</param>
    Protected Overrides Sub SetValue(ByVal index As Integer, ByVal value As Object)
        Dim pointer = DataPointer
        Dim offset = GetOffset(index)

        Select Case Type
            Case SymbolicExpressionType.NumericVector
                WriteDouble(value, pointer, offset)
                Return
            Case SymbolicExpressionType.IntegerVector

                If IsFactor() Then
                    AsFactor().SetFactor(index, TryCast(value, String))
                Else
                    WriteInt32(value, pointer, offset)
                End If

                Return
            Case SymbolicExpressionType.CharacterVector
                WriteString(CStr(value), pointer, offset)
                Return
            Case SymbolicExpressionType.LogicalVector
                WriteBoolean(value, pointer, offset)
                Return
            Case SymbolicExpressionType.RawVector
                WriteByte(value, pointer, offset)
                Return
            Case SymbolicExpressionType.ComplexVector
                WriteComplex(value, pointer, offset)
                Return
            Case Else
                WriteSymbolicExpression(CType(value, SymbolicExpression), pointer, offset)
                Return
        End Select
    End Sub

    ''' <summary>
    ''' Gets the data size of each element in this vector, i.e. the offset in memory between elements.
    ''' </summary>
    Protected Overrides ReadOnly Property DataSize As Integer
        Get

            Select Case Type
                Case SymbolicExpressionType.NumericVector
                    Return Marshal.SizeOf(GetType(Double))
                Case SymbolicExpressionType.IntegerVector
                    Return Marshal.SizeOf(GetType(Integer))
                Case SymbolicExpressionType.CharacterVector
                    Return Marshal.SizeOf(GetType(IntPtr))
                Case SymbolicExpressionType.LogicalVector
                    Return Marshal.SizeOf(GetType(Integer))
                Case SymbolicExpressionType.RawVector
                    Return Marshal.SizeOf(GetType(Byte))
                Case SymbolicExpressionType.ComplexVector
                    Return Marshal.SizeOf(GetType(Complex))
                Case Else
                    Return Marshal.SizeOf(GetType(IntPtr))
            End Select
        End Get
    End Property

    Private Function ReadDouble(ByVal pointer As IntPtr, ByVal offset As Integer) As Double
        Dim data = New Byte(7) {}

        For byteIndex = 0 To data.Length - 1
            data(byteIndex) = Marshal.ReadByte(pointer, offset + byteIndex)
        Next

        Return BitConverter.ToDouble(data, 0)
    End Function

    Private Sub WriteDouble(ByVal value As Double, ByVal pointer As IntPtr, ByVal offset As Integer)
        Dim data = BitConverter.GetBytes(value)

        For byteIndex = 0 To data.Length - 1
            Marshal.WriteByte(pointer, offset + byteIndex, data(byteIndex))
        Next
    End Sub

    Private Function ReadInt32(ByVal pointer As IntPtr, ByVal offset As Integer) As Integer
        Return Marshal.ReadInt32(pointer, offset)
    End Function

    Private Sub WriteInt32(ByVal value As Integer, ByVal pointer As IntPtr, ByVal offset As Integer)
        Marshal.WriteInt32(pointer, offset, value)
    End Sub

    Private Function ReadString(ByVal pointer As IntPtr, ByVal offset As Integer) As String
        pointer = Marshal.ReadIntPtr(pointer, offset)

        If Engine.Compatibility = REngine.CompatibilityMode.ALTREP Then
            pointer = GetFunction(Of DATAPTR_OR_NULL)()(pointer)
        Else
            pointer = IntPtr.Add(pointer, Marshal.SizeOf(Engine.GetVectorSexprecType()))
        End If

        Return InternalString.StringFromNativeUtf8(pointer)
    End Function

    Private Sub WriteString(ByVal value As String, ByVal pointer As IntPtr, ByVal offset As Integer)
        Dim stringPointer As IntPtr = GetFunction(Of Rf_mkChar)()(value)
        Marshal.WriteIntPtr(pointer, offset, stringPointer)
    End Sub

    Private Function ReadBoolean(ByVal pointer As IntPtr, ByVal offset As Integer) As Boolean
        Dim data = Marshal.ReadInt32(pointer, offset)
        Return Convert.ToBoolean(data)
    End Function

    Private Sub WriteBoolean(ByVal value As Boolean, ByVal pointer As IntPtr, ByVal offset As Integer)
        Dim data = Convert.ToInt32(value)
        Marshal.WriteInt32(pointer, offset, data)
    End Sub

    Private Function ReadByte(ByVal pointer As IntPtr, ByVal offset As Integer) As Byte
        Return Marshal.ReadByte(pointer, offset)
    End Function

    Private Sub WriteByte(ByVal value As Byte, ByVal pointer As IntPtr, ByVal offset As Integer)
        Marshal.WriteByte(pointer, offset, value)
    End Sub

    Private Function ReadComplex(ByVal pointer As IntPtr, ByVal offset As Integer) As Complex
        Dim data = New Byte(Marshal.SizeOf(GetType(Complex)) - 1) {}
        Marshal.Copy(pointer, data, 0, data.Length)
        Dim real = BitConverter.ToDouble(data, 0)

        Dim imaginary As Double = System.BitConverter.ToDouble(data, Marshal.SizeOf(GetType(Double)))

        Return New Complex(real, imaginary)
    End Function

    Private Sub WriteComplex(ByVal value As Complex, ByVal pointer As IntPtr, ByVal offset As Integer)
        Dim real = BitConverter.GetBytes(value.Real)
        Dim imaginary = BitConverter.GetBytes(value.Imaginary)
        Marshal.Copy(real, 0, pointer, real.Length)
        pointer = IntPtr.Add(pointer, real.Length)
        Marshal.Copy(imaginary, 0, pointer, imaginary.Length)
    End Sub

    Private Function ReadSymbolicExpression(ByVal pointer As IntPtr, ByVal offset As Integer) As SymbolicExpression
        Dim sexp = IntPtr.Add(pointer, offset)
        Return New SymbolicExpression(Engine, sexp)
    End Function

    Private Sub WriteSymbolicExpression(ByVal sexp As SymbolicExpression, ByVal pointer As IntPtr, ByVal offset As Integer)
        Marshal.WriteIntPtr(pointer, offset, sexp.DangerousGetHandle())
    End Sub
End Class

