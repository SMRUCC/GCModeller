Imports System.Text

Namespace Runtime.Exceptions

    Public Class FunCallFailured : Inherits RuntimeException

        Sub New(Expr As Interpreter.LDM.Expressions.FunctionCalls, ScriptEngine As Runtime.ScriptEngine)
            Call MyBase.New($"Unable call function:  {Expr.ToString}", ScriptEngine)
        End Sub
    End Class

    ''' <summary>
    ''' The exception that is thrown when there is an attempt to dynamically access a
    ''' method that does not exist.
    ''' </summary>
    Public Class MethodNotFoundException : Inherits RuntimeException

        Dim API As Interpreter.Linker.APIHandler.APIEntryPoint,
                args As KeyValuePair(Of String, Object)(),
                Expr As Interpreter.LDM.Expressions.FunctionCalls

        ''' <summary>
        ''' Initializes a new instance of the <see cref="MethodNotFoundException"/> class with the
        ''' specified class name and method name.
        ''' </summary>
        ''' <param name="API">The name of the class in which access to a nonexistent method was attempted.</param>
        ''' <param name="args"></param>
        ''' <param name="Expr">The name of the method that cannot be accessed.</param>
        ''' <param name="ScriptEngine"></param>
        Sub New(API As Interpreter.Linker.APIHandler.APIEntryPoint,
                args As KeyValuePair(Of String, Object)(),
                Expr As Interpreter.LDM.Expressions.FunctionCalls,
                ScriptEngine As Runtime.ScriptEngine)

            Call MyBase.New(MethodNotFoundException.__innerMessage(API, args, Expr, ScriptEngine),
                            New MissingMethodException(className:=NameOf(ExecuteModel),
                                                       methodName:=Expr.EntryPoint.Name.Expression.PrimaryExpression),
                            ScriptEngine)

            Me.Script = Expr
            Me.API = API
            Me.args = args
            Me.Expr = Expr
        End Sub

        Public Sub New(innerMessage As String, ScriptEngine As Runtime.ScriptEngine)
            Call MyBase.New(innerMessage, ScriptEngine)
        End Sub

        Private Shared Function __innerMessage(API As Interpreter.Linker.APIHandler.APIEntryPoint,
                                               args As KeyValuePair(Of String, Object)(),
                                               Expr As Interpreter.LDM.Expressions.FunctionCalls,
                                               ScriptEngine As Runtime.ScriptEngine) As String

            Dim sbr As StringBuilder = New StringBuilder(Expr.PrimaryExpression)
            Call sbr.AppendLine()
            Call sbr.AppendLine()

            If Not Expr Is Nothing Then
                Call sbr.AppendLine("Source Details:")
                Call sbr.AppendLine($"{NameOf(Expr.ExtensionVariable)}     = $[{If(Expr.ExtensionVariable Is Nothing, "", Expr.ExtensionVariable.Expression.ExceptionExpr)}]")
                Call sbr.AppendLine($"{NameOf(Expr.Comments)}              = {Expr.Comments}")
                Call sbr.AppendLine($"{NameOf(Expr.LeftAssignedVariable)} = {Expr.LeftAssignedVariable.ExceptionExpr}")
                Call sbr.AppendLine($"{NameOf(Expr.EntryPoint)}              = {Expr.EntryPoint.ExceptionExpr}")
                Call sbr.AppendLine($"{NameOf(Expr.Parameters)}             =    ")
                Call sbr.AppendLine(String.Join(vbCrLf & "               ", (From param In Expr.Parameters Select $"[{param.Key.ExceptionExpr},  {param.Value.ExceptionExpr}]").ToArray))
                Call sbr.AppendLine()
            End If

            Call sbr.AppendLine($"{NameOf(args)}                             =")
            Call sbr.AppendLine(String.Join(vbCrLf & "               ", (From param In args Select $"{param.Key}  ==> {InputHandler.ToString(param.Value)}")))
            Call sbr.AppendLine()
            Call sbr.AppendLine()
            Call sbr.AppendLine($"{NameOf(API)} Details:")
            Call sbr.AppendLine($"{NameOf(API.Name)}                      = {API.Name}  {If(API.IsOverloaded, $"(+{API.OverloadsNumber} Overloads)", "")}")
            Call sbr.AppendLine("[")
            For Each func In API
                Call sbr.AppendLine($"   {func.EntryPoint.EntryPoint.ToString}")
            Next
            Call sbr.AppendLine("]")

            Return sbr.ToString
        End Function
    End Class
End Namespace