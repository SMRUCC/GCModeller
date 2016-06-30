Imports RDotNet.Internals
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' Internal string.
''' </summary>
<DebuggerDisplay("Content = {ToString()}; RObjectType = {Type}")> _
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class InternalString
	Inherits SymbolicExpression
	''' <summary>
	''' Creates a new instance.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="pointer">The pointer to a string.</param>
	Public Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Creates a new instance.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="s">The string</param>
	Public Sub New(engine As REngine, s As String)
		MyBase.New(engine, engine.GetFunction(Of Rf_mkChar)()(s))
	End Sub

	''' <summary>
	''' Converts to the string into .NET Framework string.
	''' </summary>
	''' <param name="s">The R string.</param>
	''' <returns>The .NET Framework string.</returns>
	Public Shared Widening Operator CType(s As InternalString) As String
		Return s.ToString()
	End Operator

	''' <summary>
	''' Gets the string representation of the string object.
    ''' This returns <c>"NA"</c> if the value is <c>NA</c>, whereas <see cref="GetInternalValue"/> returns <c>null</c>.
	''' </summary>
	''' <returns>The string representation.</returns>
    ''' <seealso cref="GetInternalValue"/>
	Public Overrides Function ToString() As String
		Dim pointer As IntPtr = IntPtr.Add(handle, Marshal.SizeOf(GetType(VECTOR_SEXPREC)))
		Return Marshal.PtrToStringAnsi(pointer)
	End Function

	''' <summary>
	''' Gets the string representation of the string object.
    ''' This returns <c>null</c> if the value is <c>NA</c>, whereas <see cref="ToString"/> returns <c>"NA"</c>.
	''' </summary>
	''' <returns>The string representation.</returns>
	Public Function GetInternalValue() As String
		If handle = Engine.NaStringPointer Then
			Return Nothing
		End If
		Dim pointer As IntPtr = IntPtr.Add(handle, Marshal.SizeOf(GetType(VECTOR_SEXPREC)))
		Return Marshal.PtrToStringAnsi(pointer)
	End Function
End Class
