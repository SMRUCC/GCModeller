Imports RDotNet.Internals
Imports System
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
    Protected Friend Sub New(ByVal engine As REngine, ByVal pointer As IntPtr)
        MyBase.New(engine, pointer)
    End Sub

    ''' <summary>
    ''' Gets the number of nodes.
    ''' </summary>
    Public ReadOnly Property Count As Integer
        Get
            Return GetFunction(Of Rf_length)()(handle)
        End Get
    End Property

#Region "IEnumerable<Symbol> Members"

    ''' <summary>
    ''' Gets an enumerator over this pairlist
    ''' </summary>
    ''' <returns>The enumerator</returns>
    Public Iterator Function GetEnumerator() As IEnumerator(Of Symbol) Implements IEnumerable(Of Symbol).GetEnumerator
        If Count <> 0 Then
            Dim sexprecType = Engine.GetSEXPRECType()
            Dim sexp As Object = GetInternalStructure()

            While sexp.sxpinfo.type <> SymbolicExpressionType.Null
                Yield New Symbol(Engine, sexp.listsxp.tagval)
                sexp = Convert.ChangeType(Marshal.PtrToStructure(sexp.listsxp.cdrval, sexprecType), sexprecType)
            End While
        End If
    End Function

    Private Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

#End Region
End Class

