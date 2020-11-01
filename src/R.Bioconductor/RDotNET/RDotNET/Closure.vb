Imports System
Imports System.Collections.Generic
Imports System.Linq


''' <summary>
''' A closure.
''' </summary>
Public Class Closure
    Inherits [Function]
    ''' <summary>
    ''' Creates a closure object.
    ''' </summary>
    ''' <param name="engine">The engine.</param>
    ''' <param name="pointer">The pointer.</param>
    Protected Friend Sub New(ByVal engine As REngine, ByVal pointer As IntPtr)
        MyBase.New(engine, pointer)
    End Sub

    ''' <summary>
    ''' Gets the arguments list.
    ''' </summary>
    Public ReadOnly Property Arguments As Pairlist
        Get
            Dim sexp As Object = GetInternalStructure()
            Return New Pairlist(Engine, sexp.closxp.formals)
        End Get
    End Property

    ''' <summary>
    ''' Gets the body.
    ''' </summary>
    Public ReadOnly Property Body As Language
        Get
            Dim sexp As Object = GetInternalStructure()
            Return New Language(Engine, sexp.closxp.body)
        End Get
    End Property

    ''' <summary>
    ''' Gets the environment.
    ''' </summary>
    Public ReadOnly Property Environment As REnvironment
        Get
            Dim sexp As Object = GetInternalStructure()
            Return New REnvironment(Engine, DirectCast(sexp.closxp.env, REnvironment))
        End Get
    End Property

    ''' <summary>
    ''' Invoke this function, using an ordered list of unnamed arguments.
    ''' </summary>
    ''' <param name="args">The arguments of the function</param>
    ''' <returns>The result of the evaluation</returns>
    Public Overloads Overrides Function Invoke(ParamArray args As SymbolicExpression()) As SymbolicExpression
        'int count = Arguments.Count;
        'if (args.Length > count)
        '   throw new ArgumentException("Too many arguments provided for this function", "args");
        Return InvokeOrderedArguments(args)
    End Function

    ''' <summary>
    ''' Invoke this function, using named arguments provided as key-value pairs
    ''' </summary>
    ''' <param name="args">the representation of named arguments, as a dictionary</param>
    ''' <returns>The result of the evaluation</returns>
    Public Overloads Overrides Function Invoke(ByVal args As IDictionary(Of String, SymbolicExpression)) As SymbolicExpression
        Dim a = args.ToArray()
        Return InvokeViaPairlist(Array.ConvertAll(a, Function(x) x.Key), Array.ConvertAll(a, Function(x) x.Value))
    End Function

    Private Function GetArgumentNames() As String()
        Return Arguments.[Select](Function(arg) arg.PrintName).ToArray()
    End Function
End Class

