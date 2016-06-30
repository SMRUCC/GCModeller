Imports RDotNet.Internals
Imports System.Numerics
Imports System.Runtime.InteropServices
Imports RDotNET.REngineExtension
Imports RDotNET.SymbolicExpressionExtension

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
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <remarks>
	''' The value is converted into specific type.
	''' </remarks>
	''' <param name="index">The zero-based index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(index As Integer) As Object
		Get
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Return GetValue(index)
			End Using
		End Get
		Set
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				SetValue(index, value)
			End Using
		End Set
	End Property

	''' <summary>
	''' Gets an array representation of a vector in R. Note that the implementation cannot be particularly "fast" in spite of the name.
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function GetArrayFast() As Object()
		Dim res = New Object(Me.Length - 1) {}
		For i As Integer = 0 To res.Length - 1
			res(i) = GetValue(i)
		Next
		Return res
	End Function

	Private Function GetValue(index As Integer) As Object
		Dim pointer As IntPtr = DataPointer
		Dim offset As Integer = GetOffset(index)
		Select Case Type
			Case SymbolicExpressionType.NumericVector
				Return ReadDouble(pointer, offset)

			Case SymbolicExpressionType.IntegerVector
				If Me.IsFactor() Then
					Return Me.AsFactor().GetFactor(index)
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
	Protected Overrides Sub SetVectorDirect(values As Object())
		For i As Integer = 0 To values.Length - 1
			SetValue(i, values(i))
		Next
	End Sub

	Private Sub SetValue(index As Integer, value As Object)
		Dim pointer As IntPtr = DataPointer
		Dim offset As Integer = GetOffset(index)
		Select Case Type
			Case SymbolicExpressionType.NumericVector
				WriteDouble(CDbl(value), pointer, offset)
				Return

			Case SymbolicExpressionType.IntegerVector
				If Me.IsFactor() Then
					Me.AsFactor().SetFactor(index, TryCast(value, String))
				Else
					WriteInt32(CInt(value), pointer, offset)
				End If
				Return

			Case SymbolicExpressionType.CharacterVector
				WriteString(DirectCast(value, String), pointer, offset)
				Return

			Case SymbolicExpressionType.LogicalVector
				WriteBoolean(CBool(value), pointer, offset)
				Return

			Case SymbolicExpressionType.RawVector
				WriteByte(CByte(value), pointer, offset)
				Return

			Case SymbolicExpressionType.ComplexVector
				WriteComplex(CType(value, Complex), pointer, offset)
				Return
			Case Else

				WriteSymbolicExpression(DirectCast(value, SymbolicExpression), pointer, offset)
				Return
		End Select
	End Sub

	''' <summary>
	''' Gets the data size of each element in this vector, i.e. the offset in memory between elements.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Select Case Type
				Case SymbolicExpressionType.NumericVector
					Return 8

				Case SymbolicExpressionType.IntegerVector
					Return 4

				Case SymbolicExpressionType.CharacterVector
					Return Marshal.SizeOf(GetType(IntPtr))

				Case SymbolicExpressionType.LogicalVector
					Return 4

				Case SymbolicExpressionType.RawVector
					Return 1

				Case SymbolicExpressionType.ComplexVector
					Return Marshal.SizeOf(GetType(Complex))
				Case Else

					Return Marshal.SizeOf(GetType(IntPtr))
			End Select
		End Get
	End Property

	Private Function ReadDouble(pointer As IntPtr, offset As Integer) As Double
		Dim data = New Byte(8 - 1) {}
		For byteIndex As Integer = 0 To data.Length - 1
			data(byteIndex) = Marshal.ReadByte(pointer, offset + byteIndex)
		Next
		Return BitConverter.ToDouble(data, 0)
	End Function

	Private Sub WriteDouble(value As Double, pointer As IntPtr, offset As Integer)
		Dim data As Byte() = BitConverter.GetBytes(value)
		For byteIndex As Integer = 0 To data.Length - 1
			Marshal.WriteByte(pointer, offset + byteIndex, data(byteIndex))
		Next
	End Sub

	Private Function ReadInt32(pointer As IntPtr, offset As Integer) As Integer
		Return Marshal.ReadInt32(pointer, offset)
	End Function

	Private Sub WriteInt32(value As Integer, pointer As IntPtr, offset As Integer)
		Marshal.WriteInt32(pointer, offset, value)
	End Sub

	Private Function ReadString(pointer As IntPtr, offset As Integer) As String
		pointer = Marshal.ReadIntPtr(pointer, offset)
		pointer = IntPtr.Add(pointer, Marshal.SizeOf(GetType(VECTOR_SEXPREC)))
		Return Marshal.PtrToStringAnsi(pointer)
	End Function

	Private Sub WriteString(value As String, pointer As IntPtr, offset As Integer)
		Dim stringPointer As IntPtr = Me.GetFunction(Of Rf_mkChar)()(value)
		Marshal.WriteIntPtr(pointer, offset, stringPointer)
	End Sub

	Private Function ReadBoolean(pointer As IntPtr, offset As Integer) As Boolean
		Dim data As Integer = Marshal.ReadInt32(pointer, offset)
		Return Convert.ToBoolean(data)
	End Function

	Private Sub WriteBoolean(value As Boolean, pointer As IntPtr, offset As Integer)
		Dim data As Integer = Convert.ToInt32(value)
		Marshal.WriteInt32(pointer, offset, data)
	End Sub

	Private Function ReadByte(pointer As IntPtr, offset As Integer) As Byte
		Return Marshal.ReadByte(pointer, offset)
	End Function

	Private Sub WriteByte(value As Byte, pointer As IntPtr, offset As Integer)
		Marshal.WriteByte(pointer, offset, value)
	End Sub

	Private Function ReadComplex(pointer As IntPtr, offset As Integer) As Complex
		Dim data = New Byte(Marshal.SizeOf(GetType(Complex)) - 1) {}
		Marshal.Copy(pointer, data, 0, data.Length)
		Dim real As Double = BitConverter.ToDouble(data, 0)
		Dim imaginary As Double = BitConverter.ToDouble(data, 8)
		Return New Complex(real, imaginary)
	End Function

	Private Sub WriteComplex(value As Complex, pointer As IntPtr, offset As Integer)
		Dim real As Byte() = BitConverter.GetBytes(value.Real)
		Dim imaginary As Byte() = BitConverter.GetBytes(value.Imaginary)
		Marshal.Copy(real, 0, pointer, real.Length)
		pointer = IntPtr.Add(pointer, real.Length)
		Marshal.Copy(imaginary, 0, pointer, imaginary.Length)
	End Sub

	Private Function ReadSymbolicExpression(pointer As IntPtr, offset As Integer) As SymbolicExpression
		Dim sexp As IntPtr = IntPtr.Add(pointer, offset)
		Return New SymbolicExpression(Engine, sexp)
	End Function

	Private Sub WriteSymbolicExpression(sexp As SymbolicExpression, pointer As IntPtr, offset As Integer)
		Marshal.WriteIntPtr(pointer, offset, sexp.DangerousGetHandle())
	End Sub
End Class
