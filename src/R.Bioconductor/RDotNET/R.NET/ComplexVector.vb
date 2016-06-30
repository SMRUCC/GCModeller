Imports RDotNet.Internals
Imports System.Collections.Generic
Imports System.Numerics
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A collection of complex numbers.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class ComplexVector
	Inherits Vector(Of Complex)
	''' <summary>
	''' Creates a new empty ComplexVector with the specified length.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="length">The length.</param>
    ''' <seealso cref="REngineExtension.CreateComplexVector"/>
	Public Sub New(engine As REngine, length As Integer)
		MyBase.New(engine, SymbolicExpressionType.ComplexVector, length)
	End Sub

	''' <summary>
	''' Creates a new ComplexVector with the specified values.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateComplexVector"/>
	Public Sub New(engine As REngine, vector As IEnumerable(Of Complex))
		MyBase.New(engine, SymbolicExpressionType.ComplexVector, vector)
	End Sub

	''' <summary>
	''' Creates a new instance for a complex number vector.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a complex number vector.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="index">The zero-based index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(index As Integer) As Complex
		Get
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Dim data = New Double(1) {}
				Dim offset As Integer = GetOffset(index)
				Dim pointer As IntPtr = IntPtr.Add(DataPointer, offset)
				Marshal.Copy(pointer, data, 0, data.Length)
				Return New Complex(data(0), data(1))
			End Using
		End Get
		Set
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
                Dim data As Double() = New Double() {Value.Real, Value.Imaginary}
				Dim offset As Integer = GetOffset(index)
				Dim pointer As IntPtr = IntPtr.Add(DataPointer, offset)
				Marshal.Copy(data, 0, pointer, data.Length)
			End Using
		End Set
	End Property

	''' <summary>
	''' Gets an array representation in the CLR of a vector in R. 
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function GetArrayFast() As Complex()
		Dim n As Integer = Me.Length
		Dim data = New Double(2 * n - 1) {}
		Marshal.Copy(DataPointer, data, 0, 2 * n)
		Return Utility.DeserializeComplexFromDouble(data)
	End Function

	''' <summary>
	''' Efficient initialisation of R vector values from an array representation in the CLR
	''' </summary>
	Protected Overrides Sub SetVectorDirect(values As Complex())
		Dim data As Double() = Utility.SerializeComplexToDouble(values)
		Dim pointer As IntPtr = IntPtr.Add(DataPointer, 0)
		Marshal.Copy(data, 0, pointer, data.Length)
	End Sub

	''' <summary>
	''' Gets the size of a complex number in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return Marshal.SizeOf(GetType(Complex))
		End Get
	End Property
End Class
