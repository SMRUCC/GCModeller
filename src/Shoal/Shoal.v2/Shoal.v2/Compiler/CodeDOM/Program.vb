Imports System.CodeDom
Imports Microsoft.VisualBasic.Emit.CodeDOM_VBC
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Configuration

Namespace Compiler.CodeDOM

    Public Class Program

        Public ReadOnly Property Assembly As CodeNamespace
        Protected ReadOnly Program As CodeMemberMethod = New CodeMemberMethod With {
            .Name = ScriptApp
        }

        Const ScriptApp As String = "ScriptApp"
        Const __innerScriptEngine As String = "__innerScriptEngine"

        Sub New()
            Assembly = New CodeNamespace("Shoal.CodeDOM.App")
            Dim ProgramClass As New CodeTypeDeclaration(NameOf(Program))
            Dim EntryPoint = CodeDOMExpressions.EntryPoint
            Call Assembly.Types.Add(ProgramClass)
            Call ProgramClass.Members.Add(Program)
            Call ProgramClass.Members.Add(EntryPoint)
            Call ProgramClass.Members.Add(CodeDOMExpressions.Field(__innerScriptEngine, GetType(Runtime.ScriptEngine)))

            Program.ReturnType = Type(Of Integer)()
            Program.Parameters.Add(Argument(Of CommandLine.CommandLine)("args"))
            EntryPoint.Statements.Add(LocalsInit("__args", GetType(String), initExpression:=Reference(GetType(Microsoft.VisualBasic.App), NameOf(App.Command))))
            EntryPoint.Statements.Add([Call](GetType(Extensions), NameOf(__DEBUG_ECHO), Parameters:={LocalVariable("__args")}))
            EntryPoint.Statements.Add(LocalsInit("args", GetType(CommandLine.CommandLine), initExpression:=[Call](GetType(CommandLine.CommandLine), NameOf(CommandLine.TryParse), {LocalVariable("__args")})))
            EntryPoint.Statements.Add(LocalsInit(ScriptApp, NameOf(Program), [New](NameOf(Program), {})))
            EntryPoint.Statements.Add(ValueAssign(Reference(LocalVariable(ScriptApp), __innerScriptEngine), [New](GetType(Runtime.ScriptEngine), {[Call](GetType(Config), NameOf(Config.LoadDefault), {})})))
            EntryPoint.Statements.Add(LocalsInit("rtvl", GetType(Integer), [Call](LocalVariable(ScriptApp), ScriptApp, {LocalVariable("args")})))
            EntryPoint.Statements.Add([Call](GetType(Extensions), NameOf(App.Pause), {}))
            EntryPoint.Statements.Add([Return](LocalVariable("rtvl")))
        End Sub

        Public Sub __localsInit(Expr As Interpreter.LDM.Expressions.VariableDeclaration)
            Dim TypeRef = InputHandler.GetType(Expr.Type, True)
            Dim Init As System.CodeDom.CodeMethodInvokeExpression = Nothing

            If Expr.Initializer.IsConstant Then
                Init = __castType(__getConstant(Expr.Initializer.GetTokenValue), TypeRef)

            ElseIf Expr.Initializer.IsVariable Then


            ElseIf Expr.Initializer.IsPrimaryValue Then
                Init = __castType(Value(Expr.Initializer.Expression.PrimaryExpression), TypeRef)

            ElseIf Expr.Initializer.IsExpr Then

            End If

            Call Program.Statements.Add(Comments(Expr.Comments))
            Call Program.Statements.Add(LocalsInit(Expr.Name, TypeRef, Init))
        End Sub

        Private Function __getConstant(Name As String) As System.CodeDom.CodeMethodInvokeExpression
            Return [Call](FieldRef(__innerScriptEngine), NameOf(Runtime.ScriptEngine.GetValue), Parameters:={Value(Name)})
        End Function

        ''' <summary>
        ''' 与公共框架里面的方法所不同的是，这个类型的转换方法会实现Shoal脚本语言之中的更加动态的类型转换
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="typeRef"></param>
        ''' <returns></returns>
        Private Function __castType(value As CodeExpression, typeRef As System.Type) As CodeMethodInvokeExpression
            Return [Call](GetType(InputHandler), NameOf(Scripting.CTypeDynamic), Parameters:={value, CodeDOMExpressions.[GetType](typeRef)})
        End Function

        ''' <summary>
        ''' 请注意，由于函数的返回值是<see cref="System.Int32"/>类型，所以这里的数据类型转换总是转换至整形数的
        ''' </summary>
        ''' <param name="Expr"></param>
        Public Sub __return(Expr As Interpreter.LDM.Expressions.Keywords.Return)
            If Expr.ValueExpression.IsConstant Then  '从内置的脚本引擎之中取常数值
                Call Program.Statements.Add([Return](__castType(__getConstant(Expr.ValueExpression.GetTokenValue), GetType(Integer))))
            ElseIf Expr.ValueExpression.IsVariable Then
                Call Program.Statements.Add([Return](__castType(LocalVariable(Expr.ValueExpression.GetTrimExpr), GetType(Integer))))
            ElseIf Expr.ValueExpression.IsPrimaryValue Then
                Call Program.Statements.Add([Return](__castType(Value(Expr.ValueExpression.Expression.PrimaryExpression), GetType(Integer))))
            End If
        End Sub
    End Class
End Namespace