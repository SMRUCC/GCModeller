Imports RDotNet.Internals
Imports System.Runtime.InteropServices

Imports RDotNET.REngineExtension
Imports RDotNET.SymbolicExpressionExtension

''' <summary>
''' A symbol object.
''' </summary>
Public Class Symbol
	Inherits SymbolicExpression
	''' <summary>
	''' Creates a symbol.
	''' </summary>
	''' <param name="engine">The engine.</param>
	''' <param name="pointer">The pointer.</param>
	Protected Friend Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Gets and sets the name.
	''' </summary>
	Public Property PrintName() As String
		Get
			Dim sexp As SEXPREC = GetInternalStructure()
			Return New InternalString(Engine, sexp.symsxp.pname).ToString()
		End Get
		Set
			Dim pointer As IntPtr = (If(value Is Nothing, Engine.NilValue, New InternalString(Engine, value))).DangerousGetHandle()
			Dim offset As Integer = GetOffsetOf("pname")
			Marshal.WriteIntPtr(handle, offset, pointer)
		End Set
	End Property

	''' <summary>
	''' Gets the internal function.
	''' </summary>
	Public ReadOnly Property Internal() As SymbolicExpression
		Get
			Dim sexp As SEXPREC = GetInternalStructure()
            If CheckNil(Engine, sexp.symsxp.value) Then
                Return Nothing
            End If
			Return New SymbolicExpression(Engine, sexp.symsxp.internal)
		End Get
	End Property

	''' <summary>
	''' Gets the symbol value.
	''' </summary>
	Public ReadOnly Property Value() As SymbolicExpression
		Get
			Dim sexp As SEXPREC = GetInternalStructure()
            If CheckNil(Engine, sexp.symsxp.value) Then
                Return Nothing
            End If
			Return New SymbolicExpression(Engine, sexp.symsxp.value)
		End Get
	End Property

	Private Shared Function GetOffsetOf(fieldName As String) As Integer
		Return Marshal.OffsetOf(GetType(SEXPREC), "u").ToInt32() + Marshal.OffsetOf(GetType(symsxp), fieldName).ToInt32()
	End Function
End Class
