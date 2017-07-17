Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.SCOM

Namespace Runtime

    ''' <summary>
    ''' 执行整个脚本的模块
    ''' </summary>
    Public Class FSMMachine : Inherits RuntimeComponent

        Dim Script As SyntaxModel
        Dim p As Integer
        Dim OnErrorResumeNext As Boolean

        ''' <summary>
        ''' Initialize a runtime state machine for running a script file.
        ''' </summary>
        ''' <param name="ScriptEngine"></param>
        ''' <param name="Script"></param>
        Sub New(ScriptEngine As ScriptEngine, Script As SyntaxModel)
            Call MyBase.New(ScriptEngine)
            Me.Script = Script
            Me.ExecuteModel = ScriptEngine.ExecuteModel
        End Sub

        ReadOnly ExecuteModel As ExecuteModel

        ''' <summary>
        ''' 已经包含有错误处理的代码了，由于这个是执行的是脚本文件，故而出错的时候会直接退出运行
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Dirty works here in this function.</remarks>
        Public Function Execute() As Object
            Dim __executeReturn As Boolean = False
            Dim consoleTitleEnable As Boolean = ScriptEngine.Config.EnableConsoleTitle

            Call ExecuteModel.Stack.Push(New Stack(Script.FilePath))

            Do While p <= Script.Expressions.Length - 1
#If Not DEBUG Then  '调试的模式会不处理错误，让错误自动定位到原始的位置方便调试程序
                 Try
#End If
                If consoleTitleEnable Then
                    Call Console.Title.InvokeSet(Script.Expressions(p).PrimaryExpression)
                End If

                Call __execute(__executeReturn)
#If Not DEBUG Then
                Catch ex As Exception
                    Call App.LogException(ex, $"{NameOf(FSMMachine)}::{NameOf(Execute)}")

                    If OnErrorResumeNext Then '忽略掉错误继续执行
                    
                    Else
                        Return -1  '执行错误，则返回错误代码
                    End If
                End Try
#End If
                If __executeReturn Then
                    Exit Do
                Else
                    p += 1  '移动到下一行代码执行
                End If
            Loop

            Call ExecuteModel.Stack.Pop()

            If consoleTitleEnable Then
                Call Console.Title.InvokeSet(App.ExecutablePath.ToFileURL)
            End If

            Return ScriptEngine.MMUDevice.SystemReserved.Value  '执行了Return代码之后，Return的表达式会被写入到这个变量之中
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="__return">是否执行了<see cref="Interpreter.LDM.Expressions.Keywords.Return"/>代码</param>
        Private Sub __execute(ByRef __return As Boolean)
            Dim Line As PrimaryExpression = Script.Expressions(p)

            If Line.IsNonExecuteCode Then Return

            If Line.ExprTypeID = ExpressionTypes.OnErrorResumeNext Then
                OnErrorResumeNext = True

            ElseIf Line.ExprTypeID = ExpressionTypes.Return Then  '退出脚本的运行并返回指定的代码
                Dim rtvlExpr = Line.As(Of Keywords.Return)
                Dim value = ExecuteModel.Exec(rtvlExpr.ValueExpression.Expression)
                __return = True
                ScriptEngine.MMUDevice.SystemReserved.Value = value

            Else

                Dim value = ExecuteModel.Exec(Line)
                ScriptEngine.MMUDevice.SystemReserved.Value = value

            End If
        End Sub
    End Class
End Namespace