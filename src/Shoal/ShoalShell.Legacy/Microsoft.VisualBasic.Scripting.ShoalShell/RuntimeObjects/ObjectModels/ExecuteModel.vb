Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.ObjectModels.Statements

Namespace Runtime.Objects.ObjectModels

    Public Class ExecuteModel : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent

        ''' <summary>
        ''' Value是指向<see cref="Expressions"/>列表之中的元素的位置下表
        ''' </summary>
        Dim GotoFlags As Dictionary(Of String, Integer)
        Dim _InternalScriptCodeLineHash As IReadOnlyDictionary(Of Integer, ShoalShell.Interpreter.ObjectModels.Statements.Statement)
        Protected _InternalEXEC_Pointer As Integer = 0

        Sub New(ScriptModel As ShoalShell.Interpreter.ObjectModels.ScriptModel, ScriptEngine As ShoalShell.Runtime.Objects.ShellScript)
            Call MyBase.New(ScriptEngine)
            _InternalScriptCodeLineHash = (From i As Integer In ScriptModel.Expressions.Sequence Select i, Expr = ScriptModel.Expressions(i)).ToArray.ToDictionary(Function(obj) obj.i, elementSelector:=Function(obj) obj.Expr)
            GotoFlags = ScriptModel.GotoFlags
        End Sub

        Public ReadOnly Property ImportsNewNamespace As Boolean
            Get
                Return ScriptEngine._ImportsNewNamespace
            End Get
        End Property

        Private ReadOnly Property HasImportsNamespaceStatements As Boolean
            Get
                Dim LQuery = (From item In Me._InternalScriptCodeLineHash Where item.Value.TypeID = Interpreter.ObjectModels.Statements.Statement.Types.Imports Select item).ToArray
                Return Not LQuery.IsNullOrEmpty
            End Get
        End Property

        Protected Const EXCEPTION_GOTO_FLAG_MISSING As String = "GOTO_FLAG_MISSING:: Could not found the goto flag ""{0}"":  {1};  Line: {2}"
        Protected Const FLAG_STATEMENT_RETURN_STACK As String = "Return.Stack"

        Protected Function InternalExecuteGoto(CodeLine As ShoalShell.Interpreter.ObjectModels.Statements.Statement) As Boolean
            If Not CodeLine.TypeID = Statement.Types.GoTo Then Return False

            Dim GotoCodeLine = DirectCast(CodeLine, ShoalShell.Interpreter.ObjectModels.Statements.GOTO)

            If Not CBool(GotoCodeLine.BooleanExpression.GetValue(ScriptEngine)) = True Then Return False

            Dim Flag As String = CStr(GotoCodeLine.GotoFlag.GetValue(ScriptEngine))

            If Me.GotoFlags.ContainsKey(Flag) Then
                _InternalEXEC_Pointer = GotoFlags(Flag) - 1
            Else '没有找到相应的跳转标签，则抛出错误
                Dim ex As String = String.Format(EXCEPTION_GOTO_FLAG_MISSING,
                                                 Flag,
                                                 GotoCodeLine.Expression,
                                                 GotoCodeLine.OriginalLineNumber)
                Throw New ArgumentException(ex)
            End If

            Return True
        End Function

        Protected Overridable Sub InternalExecuteScript(CodeLine As ShoalShell.Interpreter.ObjectModels.Statements.Statement)
            'If String.Equals(CodeLine.OrignialScriptLine, FLAG_STATEMENT_RETURN_STACK, StringComparison.OrdinalIgnoreCase) Then
            '    Call _InternalEXEC_Pointer.MoveNext()
            '    Return
            'Else
            If InternalExecuteGoto(CodeLine) Then
                Return
            End If

            Dim var As String = ""
            Dim Value As Object = InternalExecute(CodeLine, var, ScriptEngine)
            Dim sys As Objects.I_MemoryManagementDevice = ScriptEngine._EngineMemoryDevice
            Call sys.InsertOrUpdate(var, Value)
        End Sub

        Public Shared Function InternalExecute(CodeLine As ShoalShell.Interpreter.ObjectModels.Statements.Statement, ByRef LeftAssignedVariable As String, ScriptEngine As Runtime.Objects.ShellScript) As Object

            Select Case CodeLine.TypeID

                Case Statement.Types.Expression
                    Dim Expr = DirectCast(CodeLine, Expression)  '赋值的变量地址是最后才被计算的
                    Dim value As Object = Expr.Execute(ScriptEngine)
                    LeftAssignedVariable = Expr.LeftAssignedVariable.GetAddress(ScriptEngine.ScriptEngineMemoryDevice)
                    Return value

                Case Statement.Types.MethodCalling
                    LeftAssignedVariable = "$"
                    Return DirectCast(CodeLine, Interpreter.ObjectModels.Statements.MethodCalling).Execute(ScriptEngine)

                Case Statement.Types.OutputRef
                    LeftAssignedVariable = "$"
                    Return DirectCast(CodeLine, OutputRef).InnerExpression.GetValue(ScriptEngine)

                Case Statement.Types.Imports
                    Dim ExprObj = DirectCast(CodeLine, [Imports])
                    Return ExprObj.Execute(ScriptEngine)

                Case Statement.Types.SyntaxError
                    Dim ExprObj = DirectCast(CodeLine, SyntaxError)
                    If ExprObj.IsSyntaxError Then
                        Throw New SyntaxErrorException(ExprObj.Expression)
                    Else
                        Return ""
                    End If

                Case Statement.Types.Library
                    Dim ExprObj = DirectCast(CodeLine, Library)



                Case Else
                    Throw New NotImplementedException

            End Select
        End Function

        Private Sub InternalExecuteScript(CodeLines As ShoalShell.Interpreter.ObjectModels.Statements.Statement())
            Do While _InternalEXEC_Pointer < _InternalScriptCodeLineHash.Count

                Dim CodeLine As Statement = CodeLines(_InternalEXEC_Pointer)
                Dim ConsoleTitle As String = String.Format("ShoalShell: @""{0}""", CodeLine.Expression)

                Call Console.Title.InvokeSet(ConsoleTitle)
                Call Debug.Print(CodeLine.ToString)

#If RELEASE Then
                Try
#End If
                Call InternalExecuteScript(CodeLine)
                If ExitScript() Then
                    Return
                End If
#If RELEASE Then
                Catch ex As Exception
                    If Not _Flag_OnErrorResumeNext Then
                        Throw New Exceptions.ScriptRunTimeException("", ScriptEngine._EngineMemoryDevice) With
                              {
                                  .LineNumber = CodeLine.LineNumber,
                                  .ScriptLine = CodeLine.OrignialScriptLine,
                                  .InnerExceptionValue = ex.ToString
                              }
                    End If
                End Try
#End If
                Call _InternalEXEC_Pointer.MoveNext()
            Loop
        End Sub

        Protected Overridable Function ExitScript() As Boolean
            Return False
        End Function

        ''' <summary>
        ''' 执行脚本对象然后返回脚本的运行结果
        ''' </summary>
        ''' <returns></returns>
        Public Function Execute() As Object
            Call InternalExecuteScript(CodeLines:=_InternalScriptCodeLineHash.Values.ToArray)
            Call Console.Title.InvokeSet("Shoal Shell")
            Return ScriptEngine._EngineMemoryDevice("$")
        End Function
    End Class
End Namespace