Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions.Keywords
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions.ControlFlows
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions.Driver
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Compiler.CodeDOM
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions.HybridScript
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens.Operator
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Interpreter

    ''' <summary>
    ''' 语法解析器
    ''' </summary>
    Public Module SyntaxParser

        Public Function MSLParser(ByRef Expressions As Queue(Of String)) As PrimaryExpression
            Dim Parser = New Parser.TextTokenliser.MSLTokens().Parsing(Expressions.Dequeue)

            Do While Not Parser.FinishYet
                Call Parser.Parsing(Expressions.Dequeue)
            Loop

            Dim Tokens As Token() = Parser.Tokens

            If Parser.IsCommentLine Then
                Return New Comments(Parser.Comments)
            ElseIf Parser.IsBlank Then
                Return New SyntaxError(NameOf(Parser.IsBlank), "")
            End If

            Return Parsing(Parser.MSLExpression, Tokens, Parser.Comments)
        End Function

        ''' <summary>
        ''' Create the Shoal script language data model from the <see cref="Parser.TextTokenliser.MSLTokens.Parsing(String)"/> result.
        ''' (为了提高解析的性能，这里不会使用反射操作来进行解析操作的自动注册的，而是通过在下面的手工排序来得到最好的解析性能)
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <param name="Tokens"></param>
        ''' <param name="Comments"></param>
        ''' <returns></returns>
        Public Function Parsing(Expression As String, Tokens As Token(), Comments As String) As PrimaryExpression
            Dim Expr As New Value(Of PrimaryExpression)
            Dim invokeSet = New SetValue(Of PrimaryExpression)().GetSet(NameOf(PrimaryExpression.Comments))

            ' 为了提高解析的性能，这里不会使用反射操作来进行解析操作的自动注册的，而是通过在下面的手工排序来得到最好的解析性能

            If Not (Expr = TryParseVariableDeclaration(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseHybirdsScriptPush(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseHybridsScript(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseDelegate(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseCollectionOpr(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseFileIO(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseDynamicsCast(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseHashTable(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseDieException(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseMemoryOpr(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseReturn(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseCd(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseWiki(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseSourceSyntax(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            End If

            If Not (Expr = TryParseDoUntil(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseDoWhile(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseElse(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseElseIf(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseForLoop(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseIf(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            End If

            If Not (Expr = TryParseGotoJumpsLabel(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseOutputHandle(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseImports(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseInclude(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseLibrary(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseGoto(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseOnErrorResumeNext(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseComments(Expression, Tokens)) Is Nothing Then
                Return Expr

            ElseIf Not (Expr = TryParseRedirectStream(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            ElseIf Not (Expr = TryParseFunctionCalls(Expression, Tokens)) Is Nothing Then
                Return invokeSet(Expr, value:=Comments)

            End If

            Return New SyntaxError("The syntax is currently not support yet!   ===>  " & Expression, Expression)
        End Function

#Region "SyntaxParser"

        Public Function TryPasrseCollectionAppends(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.CollectionAppends
            If Tokens.Length < 3 OrElse Tokens(0).GetTokenValue.First <> "["c Then
                Return Nothing
            End If

            If GetOperator(Tokens) <> [Operator].Operators.CollectionOprOrDelegate Then
                Return Nothing
            End If


        End Function

        Public Function TryParseElse(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.ControlFlows.Else
            If Tokens.Length < 2 OrElse Not String.Equals(Tokens(Scan0).GetTokenValue, "else", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Return New [Else](Expression) With {.Invoke = __delegate(Tokens(1))}
        End Function

        Public Function TryParseElseIf(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.ControlFlows.ElseIf
            If Tokens.Length < 3 OrElse Not String.Equals(Tokens(Scan0).GetTokenValue, "elseif", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Return New [ElseIf](Expression) With {
                .BooleanIf = New InternalExpression(Tokens(1)),
                .Invoke = __delegate(Tokens(2))
            }
        End Function

        ''' <summary>
        ''' If &lt;boolean> {delegate}
        ''' ElseIf &lt;boolean> {delegate}
        ''' Else {Delegate}
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns></returns>
        Public Function TryParseIf(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.ControlFlows.If
            If Tokens.Length < 3 OrElse Not String.Equals(Tokens(Scan0).GetTokenValue, "if", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Return New [If](Expression) With {
                .BooleanIf = New InternalExpression(Tokens(1)),
                .Invoke = __delegate(Tokens(2))
            }
        End Function

        ''' <summary>
        ''' DoUntil &lt;boolean> {Delegate}
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns></returns>
        Public Function TryParseDoUntil(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.ControlFlows.DoUntil
            If Tokens.Length < 3 OrElse Not String.Equals(Tokens(Scan0).GetTokenValue, "dountil", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Return New DoUntil(Expression) With {
                .BooleanIf = New InternalExpression(Tokens(1)),
                .Invoke = __delegate(Tokens(2))
            }
        End Function

        ''' <summary>
        ''' DoWhile &lt;boolean> {delegate}
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns></returns>
        Public Function TryParseDoWhile(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.ControlFlows.DoWhile
            If Tokens.Length < 3 OrElse Not String.Equals(Tokens(Scan0).GetTokenValue, "dowhile", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Return New DoWhile(Expression) With {
                .BooleanIf = New InternalExpression(Tokens(1)),
                .Invoke = __delegate(Tokens(2))
            }
        End Function

        ''' <summary>
        ''' For var in {status} {Delegate}
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns></returns>
        Public Function TryParseForLoop(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.ControlFlows.ForLoop
            If Tokens.Length < 5 OrElse
                Not String.Equals(Tokens(0).GetTokenValue, "for", StringComparison.OrdinalIgnoreCase) OrElse
                Not String.Equals(Tokens(2).GetTokenValue, "in", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Return New ForLoop(Expression) With {
                .Collection = ForLoopStatus.StatusParser(Tokens(3).GetTrimExpr),
                .LoopVariable = Tokens(1).GetTokenValue,
                .Invoke = __delegate(Tokens(4))
            }
        End Function

        ''' <summary>
        ''' @name
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns></returns>
        Public Function TryParseRedirectStream(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.HybridScript.RedirectStream
            If Tokens(Scan0).GetTokenValue.First <> "@"c Then
                Return Nothing
            End If

            Dim Name As String = Tokens(Scan0).GetTokenValue
            Name = Mid(Name, 2).Trim

            Return New RedirectStream(Expression) With {.EntryPoint = Name}
        End Function

        Public Function TryParseSourceSyntax(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.Source
            If Tokens.Length < 4 Then
                Return Nothing
            End If

            If Not GetOperator(Tokens) = [Operator].Operators.CollectionOprOrDelegate Then
                Return Nothing
            End If

            If Not String.Equals(Tokens(2).GetTokenValue, "$") Then
                Return Nothing
            End If

            Dim param As String = Tokens(3).GetTokenValue

            If Not (Not param.First = "{"c AndAlso param.Last = "}"c) Then
                Return Nothing
            Else
                param = Mid(param, 1, Len(param) - 1)
            End If

            Dim args As CommandLine.CommandLine = CommandLine.TryParse(param)
            Dim Expr = New Source(Expression) With {
                .LeftAssigned = New LeftAssignedVariable(Tokens(Scan0).GetTokenValue),
                .Path = New InternalExpression(args.Name),
                .args = args.GetValueArray.ToArray(AddressOf __arg)
            }

            Return Expr
        End Function

        Private Function __arg(arg As NamedValue(Of String)) As KeyValuePair(Of String, InternalExpression)
            Return New KeyValuePair(Of String, InternalExpression)(arg.Name, New InternalExpression(arg.Value))
        End Function

        Public Function TryParseWiki(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.Keywords.Wiki
            If Not String.Equals(Tokens(0).GetTokenValue, "?") Then
                Return Nothing
            End If

            Dim obj As String = ""

            If Tokens.Length > 1 Then
                obj = Tokens(1).GetTokenValue
            End If

            Return New Wiki(Expression) With {.Object = obj}
        End Function

        Public Function TryParseCd(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.Keywords.Cd
            If Not String.Equals(Tokens(Scan0).GetTokenValue, "cd", StringComparison.OrdinalIgnoreCase) OrElse
                Tokens.Length < 2 Then
                Return Nothing
            End If

            Return New Cd(Expression) With {
                .Path = New InternalExpression(Tokens(1))
            }
        End Function

        ''' <summary>
        ''' *T &lt;= {
        '''    Imports test
        '''    Return rand
        ''' }
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns></returns>
        Public Function TryParseDelegate(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.Delegate
            If Tokens(Scan0).GetTokenValue.First <> "*"c OrElse Tokens.Length < 3 Then
                Return Nothing
            End If

            If GetOperator(Tokens) <> [Operator].Operators.CollectionOprOrDelegate Then
                Return Nothing
            End If

            Dim innerScript As LDM.SyntaxModel = __delegate(Tokens(2))
            Return New [Delegate](Expression) With {
                .FuncPointer = Tokens(0).GetTrimExpr,
                .FuncExpr = innerScript
            }
        End Function

        Private Function __delegate(Token As Parser.Tokens.Token) As LDM.SyntaxModel
            Dim ScriptParser = Interpreter.MSLParser(Token.GetTrimExpr).ToList
            Dim innerScript = LDM.SyntaxModel.CreateObject(ScriptParser)
            innerScript.FilePath = $"VB$Anonymous`LDM.Expressions.Delegate"

            Return innerScript
        End Function

        Public Function TryParseReturn(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.Keywords.Return
            If Not String.Equals(Tokens(0).GetTokenValue, "Return", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Return New [Return](Expression) With {
                .ValueExpression = New InternalExpression(Tokens(1))
            }
        End Function

        Public Function TryParseMemoryOpr(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.Keywords.Memory
            If Not String.Equals(Tokens(Scan0).GetTokenValue, "memory") Then
                Return Nothing
            End If

            Dim varName As String = If(Tokens.Length > 1, Tokens(1).GetTokenValue, "")
            Return New Memory(Expression) With {.var = varName}
        End Function

        ''' <summary>
        ''' <see cref="ShoalShell.Interpreter.Parser.Tokens.[Operator].Operators .CollectionOprOrDelegate"/>
        ''' </summary>
        ''' <param name="Expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="expression"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseCollectionOpr(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.CollectionOpr
            Dim DeclareNew As Boolean = False '是否申请新的变量

            If (String.Equals(Tokens(Scan0).GetTokenValue, "Dim", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(Tokens(Scan0).GetTokenValue, "var", StringComparison.OrdinalIgnoreCase)) Then
                DeclareNew = True

                Tokens = Tokens.Skip(1).ToArray
            End If

            If GetOperator(Tokens) <> [Operator].Operators.CollectionOprOrDelegate Then
                If Tokens.Length = 1 Then
                    Return Nothing
                End If

                If (From token In Tokens.Take(Tokens.Length - 1)
                    Where token.GetTokenValue.Last = ","c
                    Select 1).ToArray.Length = Tokens.Length - 1 Then

                    '是一个集合的创建，但是没有赋值的目标对象
                    Dim array As InternalExpression() = MakeCollection(Tokens)
                    Return New CollectionOpr(Expression) With {
                        .Array = array,
                        .InitLeft = New LeftAssignedVariable("$"),
                        .DeclareNew = False,
                        .Type = "Object"
                    }
                Else
                    Return Nothing
                End If
            Else
                If Tokens.Length < 3 Then
                    Return Nothing
                End If
            End If

            Dim Type As String

            If Tokens.Length > 3 Then
                If Not String.Equals(Tokens(3).GetTokenValue, "As", StringComparison.OrdinalIgnoreCase) Then
                    Return Nothing
                Else
                    Type = Tokens(4).GetTokenValue
                End If
            Else
                Type = "Object"
            End If

            Dim arrayExpr As InternalExpression() = MakeCollection(Tokens(2).GetTrimExpr)

            Return New CollectionOpr(Expression) With {
                .Array = arrayExpr,
                .InitLeft = New LeftAssignedVariable(Tokens(Scan0).GetTokenValue),
                .Type = Type,
                .DeclareNew = DeclareNew
            }
        End Function

        ''' <summary>
        ''' 请先去除掉大括号
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        <Extension> Public Function MakeCollection(Expression As String) As InternalExpression()
            Dim array = New TextTokenliser.MSLTokens().Parsing(Expression)
            Return MakeCollection(array.Tokens)
        End Function

        <Extension> Public Function MakeCollection(Tokens As Token()) As InternalExpression()
            Dim arrayExpr As New List(Of InternalExpression)

            For Each element In Tokens
                Dim str As String = element.GetTokenValue
                If str.Last = ","c Then
                    str = Mid(str, 1, Len(str) - 1).Trim
                End If

                arrayExpr += New InternalExpression(str)
            Next

            Return arrayExpr.ToArray
        End Function

        '''' <summary>
        '''' <see cref="ShoalShell.Interpreter.ObjectModels.Tokens.[Operator].Operators .SelfCast"/>
        '''' </summary>
        '''' <param name="expression">只是用于显示的原始脚本行</param>
        '''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="expression"/></param>对象的生成
        '''' <returns></returns>
        'Public Function TryParseSelfCast(expression As String, Tokens As Tokens.Token()) As Expression

        'End Function

        ''' <summary>
        ''' <see cref="ShoalShell.Interpreter.Parser.Tokens.[Operator].Operators .HybridsScript"/>
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="ldm.Expressions.DynamicsExpression"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseHybridsScript(expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.HybridScript.HybridsScript
            If Tokens.Length < 3 OrElse GetOperator(Tokens) <> [Operator].Operators.HybridsScript Then
                Return Nothing
            End If

            Return New HybridScript.HybridsScript(expression) With {
                .LeftAssignedVariable = New LeftAssignedVariable(Tokens(0).GetTokenValue),
                .ExternalScript = New InternalExpression(Tokens(2))
            }
        End Function

        ''' <summary>
        ''' <see cref="ShoalShell.Interpreter.Parser.Tokens.[Operator].Operators .HybirdsScriptPush"/>
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="ldm.Expressions.DynamicsExpression"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseHybirdsScriptPush(expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.HybridScript.HybirdsScriptPush
            If Tokens.Length < 3 OrElse GetOperator(Tokens) <> [Operator].Operators.HybirdsScriptPush Then
                Return Nothing
            End If

            Return New HybridScript.HybirdsScriptPush(expression) With {
               .InternalExpression = New InternalExpression(Tokens(0)),
               .ExternalVariable = New InternalExpression(Tokens(2))
            }
        End Function

        ''' <summary>
        ''' 单独的抛出错误的语句
        ''' </summary>
        ''' <param name="expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns>die &lt;message> when &lt;condition></returns>
        Public Function TryParseDieException(expression As String, Tokens As Parser.Tokens.Token()) As Die
            If Not String.Equals(Tokens(0).GetTokenValue, "die", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Dim [When] As InternalExpression
            Dim Message As String

            If Tokens.Length > 3 Then  '带有条件
                If Not String.Equals(Tokens(2).GetTokenValue, "when", StringComparison.OrdinalIgnoreCase) Then
                    Return Nothing
                Else
                    [When] = New InternalExpression(Tokens(3))
                    Message = Tokens(1).GetTokenValue
                End If

            ElseIf Tokens.Length = 2 Then  '带有消息的
                [When] = New InternalExpression("True")
                Message = Tokens(1).GetTokenValue

            Else '不带有任何消息的，就只有一个die动词的，也会被解析
                [When] = New InternalExpression("True")
                Message = "Shoal Runtime Exception"

            End If

            Return New Die(expression) With {
                .ExceptionMessage = Message,
                .When = [When]
            }
        End Function

        Public Function TryParseComments(expression As String, Tokens As Parser.Tokens.Token()) As Comments
            If LDM.Expressions.Comments.IsComments(Tokens(Scan0)) Then
                Return New Comments(expression)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' var = $args => --ssl
        ''' 或者
        ''' $args => /name
        ''' </summary>
        ''' <param name="expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns></returns>
        Public Function TryParseHashTable(expression As String, Tokens As Parser.Tokens.Token()) As HashTable
            If Tokens.Length = 5 Then
                If [Operator].GetOperator(Tokens(3).GetTokenValue) <> [Operator].Operators.HashOprOrDelegate OrElse
                    [Operator].GetOperator(Tokens(1).GetTokenValue) <> [Operator].Operators.ValueAssign Then
                    Return Nothing
                Else
                    Return New HashTable(expression) With {
                        .LeftAssignedVariable = New LeftAssignedVariable(Tokens(0).GetTokenValue),
                        .Table = New InternalExpression(Tokens(2)),
                        .Key = New InternalExpression(Tokens(4))
                    }
                End If
            End If

            If Tokens.Length = 3 Then
                If [Operator].GetOperator(Tokens(1).GetTokenValue) <> [Operator].Operators.HashOprOrDelegate Then
                    Return Nothing
                Else
                    Return New HashTable(expression) With {
                        .LeftAssignedVariable = New LeftAssignedVariable("$"),
                        .Table = New InternalExpression(Tokens(0)),
                        .Key = New InternalExpression(Tokens(2))
                    }
                End If
            End If

            Return Nothing
        End Function

        Public Function TryParseOnErrorResumeNext(expression As String, Tokens As Parser.Tokens.Token()) As OnErrorResumeNext
            If OnErrorResumeNext.IsOnErrorResumeNext(Tokens) Then
                Return New OnErrorResumeNext(expression)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 没有跟随文件名说明是列举出所有安装的模块
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns></returns>
        Public Function TryParseLibrary(Expression As String, Tokens As Parser.Tokens.Token()) As Library
            If Not (String.Equals(Tokens(0).GetTokenValue, "library", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(Tokens(0).GetTokenValue, "install", StringComparison.OrdinalIgnoreCase)) Then
                Return Nothing
            End If

            Dim assm As String = If(Tokens.Length > 1, Tokens(1).GetTokenValue, "")
            Return New Library(Expression) With {.Assembly = assm}
        End Function

        ''' <summary>
        ''' Include file1, file2, file3
        ''' </summary>
        ''' <param name="expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns></returns>
        Public Function TryParseInclude(expression As String, Tokens As Parser.Tokens.Token()) As Include
            If Tokens.Length < 2 Then
                Return Nothing
            End If

            If Not String.Equals(Tokens(0).GetTokenValue, "include", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Return New Include(expression) With {
                .ExternalScripts = LinqAPI.Exec(Of String) <=
                    From Token As Token
                    In Tokens.Skip(1)
                    Select Token.GetTokenValue
                }
        End Function

        ''' <summary>
        ''' Goto Label When BooleanExpression
        ''' </summary>
        ''' <param name="expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns></returns>
        Public Function TryParseGoto(expression As String, Tokens As Parser.Tokens.Token()) As [GOTO]
            If Not String.Equals(Tokens(0).GetTokenValue, "goto", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            If Tokens.Length = 4 Then
                If Not String.Equals(Tokens(2).GetTokenValue, "when", StringComparison.OrdinalIgnoreCase) Then
                    Return Nothing
                Else
                    Return New [GOTO](expression) With {
                        .JumpsLabel = New Parser.Tokens.InternalExpression(Tokens(1)),
                        .ExprWhen = New Parser.Tokens.InternalExpression(Tokens(3))
                    }
                End If
            End If

            If Tokens.Length = 2 Then
                Return New [GOTO](expression) With {
                    .JumpsLabel = New Parser.Tokens.InternalExpression(Tokens(1)),
                    .ExprWhen = New Parser.Tokens.InternalExpression("TRUE")
                }
            End If

            Return Nothing
        End Function

        Public Function TryParseGotoJumpsLabel(expression As String, Tokens As Parser.Tokens.Token()) As LineLabel
            If Tokens.Length <> 1 Then
                Return Nothing
            End If

            Dim Label As String = Tokens(0).GetTokenValue.Trim
            If Label.Last <> ":"c Then
                Return Nothing
            Else
                Label = Mid(Label, 1, Len(Label) - 1)
            End If

            Return New LineLabel(expression) With {.Label = Label}
        End Function

        Public Function TryParseVariableDeclaration(expression As String, Tokens As Parser.Tokens.Token()) As VariableDeclaration
            If Not (Tokens.Length = 4 OrElse Tokens.Length = 6) Then
                Return Nothing
            End If

            If Not (String.Equals(Tokens(Scan0).GetTokenValue, "Dim", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(Tokens(Scan0).GetTokenValue, "var", StringComparison.OrdinalIgnoreCase)) Then
                Return Nothing
            End If

            If [Operator].GetOperator(Tokens(2).GetTokenValue) <> [Operator].Operators.ValueAssign Then
                Return Nothing
            End If

            Dim Type As String

            If Tokens.Length > 4 Then
                If Not String.Equals(Tokens(4).GetTokenValue, "As", StringComparison.OrdinalIgnoreCase) Then
                    Return Nothing
                Else
                    Type = Tokens(5).GetTokenValue
                End If
            Else
                Type = "Object"
            End If

            Return New VariableDeclaration(expression) With {
                .Initializer = New InternalExpression(Tokens(3)),
                .Name = Tokens(1).GetTokenValue,
                .Type = Type
            }
        End Function

        ''' <summary>
        ''' var &lt; (typeID) {expression}
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <param name="Tokens"></param>
        ''' <returns></returns>
        Public Function TryParseDynamicsCast(Expression As String, Tokens As Parser.Tokens.Token()) As DynamicsCast
            If Tokens.Length < 4 Then
                Return Nothing
            End If

            If GetOperator(Tokens) <> [Operator].Operators.DynamicsCast Then
                Return Nothing
            End If

            Dim ID As String = Tokens(2).GetTrimExpr

            Return New DynamicsCast(Expression) With {
                .LeftAssigned = New LeftAssignedVariable(Tokens(0).GetTokenValue),
                .TypeID = New InternalExpression(ID),
                .SourceExpr = New InternalExpression(Tokens(3))
            }
        End Function

        Public Function TryParseFileIO(Expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.Driver.FileIO
            If Tokens.Length < 3 Then
                Return Nothing
            End If

            If Not GetOperator(Tokens) = [Operator].Operators.IODevice Then
                Return Nothing
            End If

            Return New LDM.Expressions.Driver.FileIO(Expression) With {
                .Path = New InternalExpression(Tokens(2)),
                .Value = New InternalExpression(Tokens(0))
            }
        End Function

        ''' <summary>
        ''' 开头的第一个字符必须是$或者&amp;
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="LDM.Expressions.FunctionCalls"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseOutputHandle(expression As String, Tokens As Parser.Tokens.Token()) As OutDeviceRef
            If Not Tokens.Length = 1 Then
                Return Nothing
            End If

            Dim Expr As String = Tokens(0).GetTokenValue

            If Not (Expr.First = "$"c OrElse Expr.First = "&"c) Then
                Return Nothing
            End If

            Return New OutDeviceRef(Expr)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="LDM.Expressions.FunctionCalls"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseImports(expression As String, Tokens As Parser.Tokens.Token()) As [Imports]
            If Tokens.Length < 2 Then '只有两个单词：   Imports <Namespace>
                Return Nothing
            End If

            If Not String.Equals(Tokens(0).GetTokenValue, "Imports", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Dim nsList As String() = (From Token In Tokens.Skip(1) Select Token.GetTokenValue).ToArray
            For i As Integer = 0 To nsList.Length - 1
                Dim str As String = nsList(i).Trim
                If str.Last = ","c Then
                    str = Mid(str, 1, Len(str) - 1)
                End If

                nsList(i) = str
            Next

            Return New [Imports](expression) With {.Namespaces = nsList}
        End Function

        ''' <summary>
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="LDM.Expressions.FunctionCalls"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseFunctionCalls(expression As String, Tokens As Parser.Tokens.Token()) As LDM.Expressions.FunctionCalls
            Dim IsCallPrefix As Boolean = String.Equals(Tokens(Scan0).GetTokenValue, "Call", StringComparison.OrdinalIgnoreCase)

            Tokens = (From obj In Tokens Where Not obj.IsNullOrSpace Select obj).ToArray

            If IsCallPrefix Then
                Tokens = Tokens.Skip(1).ToArray
            End If

            Dim opr As ShoalShell.Interpreter.Parser.Tokens.Operator.Operators = GetOperator(Tokens)

            If opr <> ShoalShell.Interpreter.Parser.Tokens.Operator.Operators.ValueAssign AndAlso
                opr <> [Operator].Operators.ExtCall Then                '这里是存粹的方法调用了
                Tokens = {New Token(0, "$"), New Token(0, "=")}.Join(Tokens).ToArray
            End If

            If opr = [Operator].Operators.ExtCall Then
                Tokens = {New Token(0, "$"), New Token(0, "<-")}.Join(Tokens).ToArray
            End If

            Dim Expr As LDM.Expressions.FunctionCalls =
                New FunctionCalls(expression) With {
                    .[Operator] = New ShoalShell.Interpreter.Parser.Tokens.Operator("<-"),
                    .LeftAssignedVariable = New LeftAssignedVariable(Tokens(0).GetTokenValue)
            }

            '第二个元素可能是变量也可能是函数名
            '假若存在第三个元素，并且是拓展函数调用方法 -> 的话，则第二个元素为变量名

            Dim idx As Integer

            If Tokens.Length > 4 AndAlso String.Equals(Tokens(3).GetTokenValue, "->") Then
                '                  2   3   4    ....   
                '拓展方法调用: 拓展变量 -> 函数名 参数列表

                idx = 5
                Expr.ExtensionVariable = New Parser.Tokens.InternalExpression(Tokens(2).GetTokenValue)
                Expr.EntryPoint = New Parser.Tokens.EntryPoint(Tokens(4).GetTokenValue)

            Else

                '                0   1     2     3 
                '普通的方法调用: 变量 <- 函数名称 参数列表 
                idx = 3
                Expr.EntryPoint = New Parser.Tokens.EntryPoint(Tokens(2).GetTokenValue)
            End If

            Try
                Expr.Parameters = __createParameters(Tokens, Index:=idx)
            Catch ex As Exception
                '当可用的参数的数目不是偶数的时候，说明语法错了
                Return Nothing
            End Try

            Return Expr
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Tokens"></param>
        ''' <param name="Index">
        ''' 3:  没有进行拓展方法的调用的
        ''' 5:  进行了拓展方法的调用的
        ''' </param>
        ''' <returns></returns>
        Private Function __createParameters(Tokens As Parser.Tokens.Token(), Index As Integer) As Dictionary(Of Parser.Tokens.ParameterName, Parser.Tokens.InternalExpression)
            Dim hash = New Dictionary(Of ParameterName, InternalExpression)

            If Index = Tokens.Length - 1 Then '函数可能只有一个参数，则参数名被省略了
                Dim Type = If(Index = 3,
                    ParameterName.ParameterType.SingleParameter,
                    ParameterName.ParameterType.EXtensionSingleParameter)
                Call hash.Add(New ParameterName(Type, ""), New InternalExpression(Tokens.Last))
                Return hash
            End If

            Dim LQuery = (From obj In Tokens Where obj.GetTokenValue.Last = ","c Select 1).ToArray

            If LQuery.Length = Tokens.Length - Index - 1 Then

                ' 使用逗号来分隔，参数是按照函数的定义顺序来排列的
                Do While Index < Tokens.Length - 1

                    Dim pName = New Parser.Tokens.ParameterName(ParameterName.ParameterType.OrderReference, "")
                    Dim valueExpr As String = Tokens(Index.MoveNext).GetTokenValue

                    valueExpr = Mid(valueExpr, 1, Len(valueExpr) - 1)

                    Dim pValue As Parser.Tokens.InternalExpression = New InternalExpression(valueExpr)

                    Call hash.Add(pName, pValue)
                Loop

                Call hash.Add(New Parser.Tokens.ParameterName(
                              ParameterName.ParameterType.OrderReference, ""),
                              New Parser.Tokens.InternalExpression(Tokens.Last))
            Else

                Dim valueTokens As String() = (From Token In Tokens.Skip(Index) Select Token.GetTokenValue).ToArray
                Dim CommandLine = CreateParameterValues(valueTokens, True)

                For Each obj In CommandLine
                    Dim pName As Parser.Tokens.ParameterName

                    If Not obj.Value Is Nothing AndAlso
                        obj.Value.GetType.Equals(GetType(Boolean)) AndAlso
                        IsPossibleLogicFlag(obj.Name) Then

                        pName = New ParameterName(ParameterName.ParameterType.BooleanSwitch, TrimParamPrefix(obj.Name))
                    Else
                        pName = New ParameterName(ParameterName.ParameterType.Normal, obj.Name)
                    End If

                    Dim pValue = New Parser.Tokens.InternalExpression(obj.Value)

                    Call hash.Add(pName, pValue)
                Next
            End If

            Return hash
        End Function

        Private Function GetOperator(Tokens As Parser.Tokens.Token()) As Parser.Tokens.Operator.Operators
            If Tokens.Length <= 2 Then
                Return Parser.Tokens.Operator.Operators.NULL
            End If

            Dim Temp = Tokens(1)
            Dim [operator] As Operators = Parser.Tokens.Operator.GetOperator(Temp.GetTokenValue)
            Return [operator]
        End Function

        Private Sub TryGetParameters(Tokens As Token(), ByRef parameters As Dictionary(Of ParameterName, InternalExpression), ByRef bools As String())
            Dim Temp As String() = (From obj In Tokens Select obj.GetTokenValue).ToArray
            Dim SingleParameter As String = ""
            Dim Parser As String() = GetLogicSWs(Temp, SingleParameter)
            Dim params = CreateParameterValues(Temp, True)

            parameters = New Dictionary(Of ParameterName, InternalExpression)

            If Not String.IsNullOrEmpty(SingleParameter) Then
                Call parameters.Add(New ParameterName(ParameterName.ParameterType.SingleParameter, ""), New InternalExpression(SingleParameter))
            End If

            parameters.AddRange((From obj In params
                                 Select Name = New ParameterName(ParameterName.ParameterType.Normal, obj.Name),
                                     value = New InternalExpression(obj.Value)) _
                                     .ToDictionary(Function(obj) obj.Name,
                                                   Function(obj) obj.value))
            bools = Parser.ToArray(AddressOf TrimParamPrefix)
        End Sub
#End Region
    End Module
End Namespace