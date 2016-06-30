Imports RDotNet.Internals
Imports System.Security.Permissions

<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Friend Class ProtectedPointer
	Implements IDisposable
	Private ReadOnly engine As REngine
	Protected Function GetFunction(Of TDelegate As Class)() As TDelegate
		Return engine.GetFunction(Of TDelegate)()
	End Function

	Private ReadOnly sexp As IntPtr

	Public Sub New(engine As REngine, sexp As IntPtr)
		Me.sexp = sexp
		Me.engine = engine

		Me.GetFunction(Of Rf_protect)()(Me.sexp)
	End Sub

	Public Sub New(sexp As SymbolicExpression)
		Me.sexp = sexp.DangerousGetHandle()
		Me.engine = sexp.Engine

		Me.GetFunction(Of Rf_protect)()(Me.sexp)
	End Sub

	#Region "IDisposable Members"

	Public Sub Dispose() Implements IDisposable.Dispose
		Me.GetFunction(Of Rf_unprotect_ptr)()(Me.sexp)
	End Sub

	#End Region

	Public Shared Widening Operator CType(p As ProtectedPointer) As IntPtr
		Return p.sexp
	End Operator
End Class
