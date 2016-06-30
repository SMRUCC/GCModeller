Imports RDotNet.Internals
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A collection of Boolean values.
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class LogicalVector
	Inherits Vector(Of Boolean)
	''' <summary>
	''' Creates a new empty LogicalVector with the specified length.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="length">The length.</param>
    ''' <seealso cref="REngineExtension.CreateLogicalVector"/>
	Public Sub New(engine As REngine, length As Integer)
		MyBase.New(engine, SymbolicExpressionType.LogicalVector, length)
	End Sub

	''' <summary>
	''' Creates a new LogicalVector with the specified values.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="vector">The values.</param>
    ''' <seealso cref="REngineExtension.CreateLogicalVector"/>
	Public Sub New(engine As REngine, vector As IEnumerable(Of Boolean))
		MyBase.New(engine, SymbolicExpressionType.LogicalVector, vector)
	End Sub

	''' <summary>
	''' Creates a new instance for a Boolean vector.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a Boolean vector.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified index.
	''' </summary>
	''' <param name="index">The zero-based index of the element to get or set.</param>
	''' <returns>The element at the specified index.</returns>
	Public Overrides Default Property Item(index As Integer) As Boolean
		Get
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Dim offset As Integer = GetOffset(index)
				Dim data As Integer = Marshal.ReadInt32(DataPointer, offset)
				Return Convert.ToBoolean(data)
			End Using
		End Get
		Set
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Dim offset As Integer = GetOffset(index)
				Dim data As Integer = Convert.ToInt32(value)
				Marshal.WriteInt32(DataPointer, offset, data)
			End Using
		End Set
	End Property

	''' <summary>
	''' Efficient conversion from R vector representation to the array equivalent in the CLR
	''' </summary>
	''' <returns>Array equivalent</returns>
	Protected Overrides Function GetArrayFast() As Boolean()
		Dim intValues As Integer() = New Integer(Me.Length - 1) {}
		Marshal.Copy(DataPointer, intValues, 0, intValues.Length)
		Return Array.ConvertAll(intValues, AddressOf Convert.ToBoolean)
	End Function

	''' <summary>
	''' Efficient initialisation of R vector values from an array representation in the CLR
	''' </summary>
	Protected Overrides Sub SetVectorDirect(values As Boolean())
		Dim intValues = Array.ConvertAll(values, AddressOf Convert.ToInt32)
		Marshal.Copy(intValues, 0, DataPointer, values.Length)
	End Sub

	''' <summary>
	''' Gets the size of a Boolean value in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			' Boolean is int internally.
			Return 4
		End Get
	End Property
End Class
