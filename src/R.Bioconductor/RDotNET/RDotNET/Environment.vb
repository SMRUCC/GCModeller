Imports RDotNet.Internals
Imports RDotNet.Utilities
Imports System
Imports System.Runtime.InteropServices


''' <summary>
''' An environment object.
''' </summary>
Public Class REnvironment
    Inherits SymbolicExpression
    ''' <summary>
    ''' Creates an environment object.
    ''' </summary>
    ''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
    ''' <param name="pointer">The pointer to an environment.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal pointer As IntPtr)
        MyBase.New(engine, pointer)
    End Sub

    ''' <summary>
    ''' Creates a new environment object.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="parent">The parent environment.</param>
    Public Sub New(ByVal engine As REngine, ByVal parent As REnvironment)
        MyBase.New(engine, engine.GetFunction(Of Rf_NewEnvironment)()(engine.NilValue.DangerousGetHandle(), engine.NilValue.DangerousGetHandle(), parent.handle))
    End Sub

    ''' <summary>
    ''' Gets the parental environment.
    ''' </summary>
    Public ReadOnly Property Parent As REnvironment
        Get
            Dim sexp As Object = GetInternalStructure()
            Dim lParent As IntPtr = sexp.envsxp.enclos
            Return If(Engine.EqualsRNilValue(lParent), Nothing, New REnvironment(Engine, lParent))
        End Get
    End Property

    ''' <summary>
    ''' Gets a symbol defined in this environment.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <returns>The symbol.</returns>
    Public Function GetSymbol(ByVal name As String) As SymbolicExpression
        If Equals(name, Nothing) Then
            Throw New ArgumentNullException()
        End If

        If Equals(name, String.Empty) Then
            Throw New ArgumentException()
        End If

        Dim installedName As IntPtr = GetFunction(Of Rf_install)()(name)
        Dim value As IntPtr = GetFunction(Of Rf_findVar)()(installedName, handle)

        If Engine.CheckUnbound(value) Then
            Throw New EvaluationException(String.Format("Error: object '{0}' not found", name))
        End If

        Dim sexprecType = Engine.GetSEXPRECType()
        Dim sexp As Object = Convert.ChangeType(Marshal.PtrToStructure(value, sexprecType), sexprecType)

        If sexp.sxpinfo.type = SymbolicExpressionType.Promise Then
            value = GetFunction(Of Rf_eval)()(value, handle)
        End If

        Return New SymbolicExpression(Engine, value)
    End Function

    ''' <summary>
    ''' Defines a symbol in this environment.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <param name="expression">The symbol.</param>
    Public Sub SetSymbol(ByVal name As String, ByVal expression As SymbolicExpression)
        If Equals(name, Nothing) Then
            Throw New ArgumentNullException("name", "'name' cannot be null")
        End If

        If Equals(name, String.Empty) Then
            Throw New ArgumentException("'name' cannot be an empty string")
        End If

        If expression Is Nothing Then
            expression = Engine.NilValue
        End If

        If expression.Engine IsNot Engine Then
            Throw New ArgumentException()
        End If

        Dim installedName As IntPtr = GetFunction(Of Rf_install)()(name)
        GetFunction(Of Rf_defineVar)()(installedName, expression.DangerousGetHandle(), handle)
    End Sub

    ''' <summary>
    ''' Gets the symbol names defined in this environment.
    ''' </summary>
    ''' <param name="all">Including special functions or not.</param>
    ''' <returns>Symbol names.</returns>
    Public Function GetSymbolNames(ByVal Optional all As Boolean = False) As String()
        Dim symbolNames = New CharacterVector(Engine, GetFunction(Of R_lsInternal)()(handle, all))
        Dim length = symbolNames.Length
        Dim copy = New String(length - 1) {}
        symbolNames.CopyTo(copy, length)
        Return copy
    End Function
End Class

