Imports RDotNet.Internals
Imports System
Imports System.Security.Permissions


    <SecurityPermission(SecurityAction.Demand, Flags:=SecurityPermissionFlag.UnmanagedCode)>
    Friend Class ProtectedPointer
        Implements IDisposable

        Private ReadOnly engine As REngine

        Protected Function GetFunction(Of TDelegate As Class)() As TDelegate
            Return engine.GetFunction(Of TDelegate)()
        End Function

        Private ReadOnly sexp As IntPtr

        Public Sub New(ByVal engine As REngine, ByVal sexp As IntPtr)
            Me.sexp = sexp
            Me.engine = engine
            GetFunction(Of Rf_protect)()(Me.sexp)
        End Sub

        Public Sub New(ByVal sexp As SymbolicExpression)
            Me.sexp = sexp.DangerousGetHandle()
            engine = sexp.Engine
            GetFunction(Of Rf_protect)()(Me.sexp)
        End Sub

#Region "IDisposable Members"

        Public Sub Dispose() Implements IDisposable.Dispose
            GetFunction(Of Rf_unprotect_ptr)()(sexp)
        End Sub

#End Region

        Public Shared Widening Operator CType(ByVal p As ProtectedPointer) As IntPtr
            Return p.sexp
        End Operator
    End Class

