Imports RDotNet.Internals
Imports System


''' <summary>
''' A language object.
''' </summary>
Public Class Language
    Inherits SymbolicExpression
    ''' <summary>
    ''' Creates a language object.
    ''' </summary>
    ''' <param name="engine">The engine</param>
    ''' <param name="pointer">The pointer.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal pointer As IntPtr)
        MyBase.New(engine, pointer)
    End Sub

    ''' <summary>
    ''' Gets function calls.
    ''' </summary>
    Public ReadOnly Property FunctionCall As Pairlist
        Get
            Dim count As Integer = GetFunction(Of Rf_length)()(handle)
            ' count == 1 for empty call.
            If count < 2 Then
                Return Nothing
            End If

            Dim sexp As Object = GetInternalStructure()
            Return New Pairlist(Engine, sexp.listsxp.cdrval)
        End Get
    End Property
End Class

