Imports RDotNet.Internals
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A collection of real numbers in double precision.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class NumericVector
	Inherits Vector(Of Double)
	''' <summary>
	''' Creates a new empty NumericVector with the specified length.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="length">The length.</param>
    ''' <seealso cref="REngineExtension.CreateNumericVector"/>
	Public Sub New(engine As REngine, length As Integer)
		MyBase.New(engine, SymbolicExpressionType.NumericVector, length)
	End Sub

	''' <summary>
	''' Creates a new NumericVector with the specified values.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateNumericVector"/>
	Public Sub New(engine As REngine, vector As IEnumerable(Of Double))
		MyBase.New(engine, SymbolicExpressionType.NumericVector, vector)
	End Sub

	''' <summary>
	''' Creates a new NumericVector with the specified values.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateNumericVector"/>
	Public Sub New(engine As REngine, vector As Double())
		MyBase.New(engine, SymbolicExpressionType.NumericVector, vector.Length)
		Marshal.Copy(vector, 0, DataPointer, vector.Length)
	End Sub

	''' <summary>
	''' Creates a new instance for a numeric vector.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a numeric vector.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="index">The zero-based index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(index As Integer) As Double
		Get
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Dim data = New Double(0) {}
				Dim offset As Integer = GetOffset(index)
				Dim pointer As IntPtr = IntPtr.Add(DataPointer, offset)
				Marshal.Copy(pointer, data, 0, data.Length)
				Return data(0)
			End Using
		End Get
		Set
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
                Dim data = New Double() {Value}
				Dim offset As Integer = GetOffset(index)
				Dim pointer As IntPtr = IntPtr.Add(DataPointer, offset)
				Marshal.Copy(data, 0, pointer, data.Length)
			End Using
		End Set
	End Property

	''' <summary>
	''' Efficient conversion from R vector representation to the array equivalent in the CLR
	''' </summary>
	''' <returns>Array equivalent</returns>
	Protected Overrides Function GetArrayFast() As Double()
		Dim res = New Double(Me.Length - 1) {}
		Marshal.Copy(DataPointer, res, 0, res.Length)
		Return res
	End Function

	''' <summary>
	''' Efficient initialisation of R vector values from an array representation in the CLR
	''' </summary>
	Protected Overrides Sub SetVectorDirect(values As Double())
		Dim pointer As IntPtr = IntPtr.Add(DataPointer, 0)
		Marshal.Copy(values, 0, pointer, values.Length)
	End Sub

	''' <summary>
	''' Gets the size of a real number in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return 8
		End Get
	End Property

	''' <summary>
	''' Copies the elements to the specified array.
	''' </summary>
	''' <param name="destination">The destination array.</param>
    ''' <param name="length__1">The length to copy.</param>
	''' <param name="sourceIndex">The first index of the vector.</param>
	''' <param name="destinationIndex">The first index of the destination array.</param>
	Public Shadows Sub CopyTo(destination As Double(), length__1 As Integer, Optional sourceIndex As Integer = 0, Optional destinationIndex As Integer = 0)
		If destination Is Nothing Then
			Throw New ArgumentNullException("destination")
		End If
		If length__1 < 0 Then
			Throw New IndexOutOfRangeException("length")
		End If
		If sourceIndex < 0 OrElse Length < sourceIndex + length__1 Then
			Throw New IndexOutOfRangeException("sourceIndex")
		End If
		If destinationIndex < 0 OrElse destination.Length < destinationIndex + length__1 Then
			Throw New IndexOutOfRangeException("destinationIndex")
		End If

		Dim offset As Integer = GetOffset(sourceIndex)
		Dim pointer As IntPtr = IntPtr.Add(DataPointer, offset)
		Marshal.Copy(pointer, destination, destinationIndex, length__1)
	End Sub
End Class
