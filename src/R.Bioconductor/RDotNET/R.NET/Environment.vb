Imports RDotNet.Internals
Imports System.Runtime.InteropServices

Imports RDotNET.REngineExtension
Imports RDotNET.SymbolicExpressionExtension

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
    Protected Friend Sub New(engine As REngine, pointer As IntPtr)
        MyBase.New(engine, pointer)
    End Sub

    ''' <summary>
    ''' Creates a new environment object.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="parent">The parent environment.</param>
    Public Sub New(engine As REngine, parent As REnvironment)
        MyBase.New(engine, engine.GetFunction(Of Rf_NewEnvironment)()(engine.NilValue.DangerousGetHandle(), engine.NilValue.DangerousGetHandle(), parent.handle))
    End Sub

    ''' <summary>
    ''' Gets the parental environment.
    ''' </summary>
    Public ReadOnly Property Parent() As REnvironment
        Get
            Dim sexp As SEXPREC = GetInternalStructure()
            Dim parent__1 As IntPtr = sexp.envsxp.enclos
            Return If(CheckNil(Engine, parent__1), Nothing, New REnvironment(Engine, parent__1))
        End Get
    End Property

    ''' <summary>
    ''' Gets a symbol defined in this environment.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <returns>The symbol.</returns>
    Public Function GetSymbol(name As String) As SymbolicExpression
        If name Is Nothing Then
            Throw New ArgumentNullException()
        End If
        If name = String.Empty Then
            Throw New ArgumentException()
        End If

        Dim installedName As IntPtr = Me.GetFunction(Of Rf_install)()(name)
        Dim value As IntPtr = Me.GetFunction(Of Rf_findVar)()(installedName, handle)
        If CheckUnbound(Engine, value) Then
            Throw New EvaluationException(String.Format("Error: object '{0}' not found", name))
        End If

        Dim sexp = CType(Marshal.PtrToStructure(value, GetType(SEXPREC)), SEXPREC)
        If sexp.sxpinfo.type = SymbolicExpressionType.Promise Then
            value = Me.GetFunction(Of Rf_eval)()(value, handle)
        End If
        Return New SymbolicExpression(Engine, value)
    End Function

    ''' <summary>
    ''' Defines a symbol in this environment.
    ''' </summary>
    ''' <param name="name">The name.</param>
    ''' <param name="expression">The symbol.</param>
    Public Sub SetSymbol(name As String, expression As SymbolicExpression)
        If name Is Nothing Then
            Throw New ArgumentNullException("name", "'name' cannot be null")
        End If
        If name = String.Empty Then
            Throw New ArgumentException("'name' cannot be an empty string")
        End If
        If expression Is Nothing Then
            expression = Engine.NilValue
        End If
        If expression.Engine IsNot Me.Engine Then
            Throw New ArgumentException()
        End If
        Dim installedName As IntPtr = Me.GetFunction(Of Rf_install)()(name)
        Me.GetFunction(Of Rf_defineVar)()(installedName, expression.DangerousGetHandle(), handle)
    End Sub

    ''' <summary>
    ''' Gets the symbol names defined in this environment.
    ''' </summary>
    ''' <param name="all">Including special functions or not.</param>
    ''' <returns>Symbol names.</returns>
    Public Function GetSymbolNames(Optional all As Boolean = False) As String()
        Dim symbolNames = New CharacterVector(Engine, Me.GetFunction(Of R_lsInternal)()(handle, all))
        Dim length As Integer = symbolNames.Length
        Dim copy = New String(length - 1) {}
        symbolNames.CopyTo(copy, length)
        Return copy
    End Function
End Class
