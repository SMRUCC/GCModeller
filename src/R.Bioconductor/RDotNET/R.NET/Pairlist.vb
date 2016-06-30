Imports RDotNet.Internals
Imports System.Collections
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

''' <summary>
''' A pairlist.
''' </summary>
Public Class Pairlist
	Inherits SymbolicExpression
	Implements IEnumerable(Of Symbol)
	''' <summary>
	''' Creates a pairlist.
	''' </summary>
	''' <param name="engine">The engine</param>
	''' <param name="pointer">The pointer.</param>
	Protected Friend Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Gets the number of nodes.
	''' </summary>
	Public ReadOnly Property Count() As Integer
		Get
			Return Me.GetFunction(Of Rf_length)()(handle)
		End Get
	End Property

	#Region "IEnumerable<Symbol> Members"

	''' <summary>
	''' Gets an enumerator over this pairlist
	''' </summary>
	''' <returns>The enumerator</returns>
    Public Iterator Function GetEnumerator() As IEnumerator(Of Symbol) Implements IEnumerable(Of Symbol).GetEnumerator
        If Count <> 0 Then
            Dim sexp As SEXPREC = GetInternalStructure()
            While sexp.sxpinfo.type <> SymbolicExpressionType.Null
                Yield New Symbol(Engine, sexp.listsxp.tagval)
                sexp = CType(Marshal.PtrToStructure(sexp.listsxp.cdrval, GetType(SEXPREC)), SEXPREC)
            End While
        End If
    End Function

	Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
		Return GetEnumerator()
	End Function

	#End Region
End Class
