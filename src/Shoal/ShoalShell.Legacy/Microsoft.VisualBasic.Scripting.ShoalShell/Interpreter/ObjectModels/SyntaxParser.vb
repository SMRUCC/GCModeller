Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.ObjectModels.Statements
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.ObjectModels.Tokens

Namespace Interpreter.ObjectModels

    ''' <summary>
    ''' 语法解析器
    ''' </summary>
    Public Module SyntaxParser

        ''' <summary>
        ''' 从这里开始解析表达式
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        Public Function Parsing(Expression As String) As ShoalShell.Interpreter.ObjectModels.Statements.Statement

            Dim Parser As New ShoalShell.Interpreter.ObjectModels.Tokenliser(Expression)
            Dim Tokens As Tokens.Token() = Parser.Tokens
            Dim Expr As Statement = Nothing

            If Not TryParseVariableAssign(Expression, Tokens).ShadowCopy(Expr) Is Nothing Then
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseCollectionOpr(Expression, Tokens).ShadowCopy(Expr) Is Nothing Then
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseExtCall(Expression, Tokens).ShadowCopy(Expr) Is Nothing Then
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseVariableDeclaration(Expression, Tokens).ShadowCopy(Expr) Is Nothing
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseHybirdsScriptPush(Expression, Tokens).ShadowCopy(Expr) Is Nothing Then
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseHybridsScript(Expression, Tokens).ShadowCopy(Expr) Is Nothing Then
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseSelfCast(Expression, Tokens).ShadowCopy(Expr) Is Nothing Then
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            End If

            If Not TryParseGotoTag(Expression, Tokens).ShadowCopy(Expr) Is Nothing Then
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseOutputHandle(Expression, Tokens).ShadowCopy(Expr) Is Nothing Then
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseImports(Expression, Tokens).ShadowCopy(Expr) Is Nothing Then
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseInclude(Expression, Tokens).ShadowCopy(Expr) Is Nothing
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseLibrary(Expression, Tokens).ShadowCopy(Expr) Is Nothing
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseMethodCalling(Expression, Tokens).ShadowCopy(Expr) Is Nothing Then
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            ElseIf Not TryParseGoto(Expression, Tokens).ShadowCopy(Expr) Is Nothing
                Return Expr.InvokeSet(Of String)(NameOf(Statement.Comments), value:=Parser.Comments)

            End If

            Return New SyntaxError("The syntax is currently not support yet!   ===>  " & Expression, Expression)
        End Function

        Public Function TryParseLibrary(expression As String, Tokens As Tokens.Token()) As ShoalShell.Interpreter.ObjectModels.Statements.Library
            If Tokens.Count <> 2 Then
                Return Nothing
            End If

            If Not String.Equals(Tokens(0).GetTokenValue, "library", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Return New Library(expression) With {.Assembly = Tokens(1)}
        End Function

        Public Function TryParseInclude(expression As String, Tokens As Tokens.Token()) As ShoalShell.Interpreter.ObjectModels.Statements.Include
            If Tokens.Count <> 2 Then
                Return Nothing
            End If

            If Not String.Equals(Tokens(0).GetTokenValue, "include", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Return New Include(expression) With {.ExternalScript = Tokens(1)}
        End Function

        Public Function TryParseGoto(expression As String, Tokens As Tokens.Token()) As ShoalShell.Interpreter.ObjectModels.Statements.GOTO
            If Tokens.Count <> 4 Then
                Return Nothing
            End If

            If Not (String.Equals(Tokens(0).GetTokenValue, "goto", StringComparison.OrdinalIgnoreCase) AndAlso String.Equals(Tokens(2).GetTokenValue, "when", StringComparison.OrdinalIgnoreCase)) Then
                Return Nothing
            End If

            Return New [GOTO](expression) With {.GotoFlag = New ObjectModels.Tokens.InternalExpression(Tokens(1)), .BooleanExpression = New ObjectModels.Tokens.InternalExpression(Tokens(3))}
        End Function

        Public Function TryParseGotoTag(expression As String, Tokens As Tokens.Token()) As ShoalShell.Interpreter.ObjectModels.Statements.GotoTag
            If Tokens.Count <> 1 Then
                Return Nothing
            End If

            Dim Tag As String = Tokens(0).GetTokenValue.Trim
            If Tag.Last <> ":"c Then
                Return Nothing
            Else
                Tag = Mid(Tag, 1, Len(Tag) - 1)
            End If

            Return New GotoTag(expression) With {.TagData = Tag}
        End Function

        Public Function TryParseVariableDeclaration(expression As String, Tokens As Tokens.Token()) As ShoalShell.Interpreter.ObjectModels.Statements.VariableDeclaration
            If Not (Tokens.Count = 4 OrElse Tokens.Count = 6) Then
                Return Nothing
            End If

            If Not String.Equals(Tokens(Scan0).GetTokenValue, "Dim", StringComparison.OrdinalIgnoreCase) OrElse
                String.Equals(Tokens(Scan0).GetTokenValue, "var", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            If ShoalShell.Interpreter.ObjectModels.Tokens.Operator.GetOperator(Tokens(2).GetTokenValue) <> [Operator].Operators.ValueAssign Then
                Return Nothing
            End If

            Dim Type As String

            If Tokens.Count > 4 Then
                If Not String.Equals(Tokens(4).GetTokenValue, "As", StringComparison.OrdinalIgnoreCase) Then
                    Return Nothing
                Else
                    Type = Tokens(5).GetTokenValue
                End If
            Else
                Type = "Object"
            End If

            Return New VariableDeclaration(expression) With {.InterExpression = New InternalExpression(Tokens(3)), .Variable = Tokens(1).GetTokenValue, .Type = Type}
        End Function

        Public Function TryParseMethodCalling(expression As String, Tokens As Tokens.Token()) As ShoalShell.Interpreter.ObjectModels.Statements.MethodCalling
            Dim Parameters As Dictionary(Of Tokens.ParameterName, Tokens.InternalExpression)
            Try
                If Tokens.IsNullOrEmpty Then
                    Return Nothing
                End If

                Parameters = __createParameters(Tokens, 1)
            Catch ex As Exception
                Return Nothing
            End Try

            Return New Statements.MethodCalling(expression) With {.EntryPoint = New ObjectModels.Tokens.EntryPoint(Tokens(0).GetTokenValue), .Parameters = Parameters}
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="expression"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseOutputHandle(expression As String, Tokens As Tokens.Token()) As ShoalShell.Interpreter.ObjectModels.Statements.OutputRef
            If Not Tokens.Count = 1 Then
                Return Nothing
            End If

            Return New OutputRef(Tokens(0).GetTokenValue)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="expression"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseImports(expression As String, Tokens As Tokens.Token()) As ShoalShell.Interpreter.ObjectModels.Statements.Imports
            If Tokens.Count <> 2 Then '只有两个单词：   Imports <Namespace>
                Return Nothing
            End If

            If Not String.Equals(Tokens(0).GetTokenValue, "Imports", StringComparison.OrdinalIgnoreCase) Then
                Return Nothing
            End If

            Return New [Imports](expression) With {.Namespace = Tokens(1).GetTokenValue}
        End Function

        ''' <summary>
        ''' <see cref="ShoalShell.Interpreter.ObjectModels.Tokens.[Operator].Operators .ValueAssign"/>
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="expression"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseVariableAssign(expression As String, Tokens As Tokens.Token()) As Expression

            If GetOperator(Tokens) <> ObjectModels.Tokens.Operator.Operators.ValueAssign Then
                Return Nothing
            End If

            Dim Expr As Expression = New Expression(expression) With
                {
                    .Operator = New ObjectModels.Tokens.Operator("<-"),
                    .LeftAssignedVariable = New ObjectModels.Tokens.LeftAssignedVariable(Tokens(0).GetTokenValue)}

            '第二个元素可能是变量也可能是函数名
            '假若存在第三个元素，并且是拓展函数调用方法 -> 的话，则第二个元素为变量名

            Dim idx As Integer

            If Tokens.Count > 4 AndAlso String.Equals(Tokens(3).GetTokenValue, "->") Then
                '                  2   3   4    ....   
                '拓展方法调用: 拓展变量 -> 函数名 参数列表

                idx = 5
                Expr.ExtensionVariable = New Tokens.LeftAssignedVariable(Tokens(2).GetTokenValue)
                Expr.EntryPoint = New Tokens.EntryPoint(Tokens(4).GetTokenValue)

            Else

                '                0   1    2     3 
                '普通的方法调用: 变量 <- 函数名称 参数列表 
                idx = 3
                Expr.EntryPoint = New Tokens.EntryPoint(Tokens(2).GetTokenValue)
            End If

            Try
                Expr.Parameters = __createParameters(Tokens, Index:=idx)
            Catch ex As Exception
                '当可用的参数的数目不是偶数的时候，说明语法错了
                Return Nothing
            End Try

            Return Expr
        End Function

        Private Function __createParameters(Tokens As Tokens.Token(), Index As Integer) As Dictionary(Of Tokens.ParameterName, Tokens.InternalExpression)
            Dim hash = New Dictionary(Of Tokens.ParameterName, Tokens.InternalExpression)

            If Index = Tokens.Count - 1 Then '函数可能只有一个参数，则参数名被省略了
                Call hash.Add(New Tokens.ParameterName(ObjectModels.Tokens.ParameterName.ParameterType.SingleParameter, ""),
                              New Tokens.InternalExpression(Tokens.Last))
                Return hash
            End If

            Dim LQuery = (From obj In Tokens Where obj.GetTokenValue.Last = ","c Select 1).ToArray

            If LQuery.Count = Tokens.Count - Index - 1 Then

                ' 使用逗号来分隔，参数是按照函数的定义顺序来排列的
                Do While Index < Tokens.Count - 1

                    Dim pName = New Tokens.ParameterName(ObjectModels.Tokens.ParameterName.ParameterType.OrderReference, "")
                    Dim pValue = New Tokens.InternalExpression(Tokens(Index.MoveNext).GetTokenValue)

                    pValue.Expression = Mid(pValue.Expression, 1, Len(pValue.Expression) - 1)

                    Call hash.Add(pName, pValue)
                Loop

                Call hash.Add(New Tokens.ParameterName(ObjectModels.Tokens.ParameterName.ParameterType.OrderReference, ""), New Tokens.InternalExpression(Tokens.Last))
            Else

                Dim valueTokens As String() = (From Token In Tokens.Skip(Index) Select Token.GetTokenValue).ToArray
                Dim CommandLine = Microsoft.VisualBasic.CommandLine.CommandLine.CreateParameterValues(valueTokens, True)

                For Each obj In CommandLine
                    Dim pName As Tokens.ParameterName

                    If Microsoft.VisualBasic.CommandLine.CommandLine.IsPossibleBooleanSwitch(obj.Key) Then
                        pName = New ParameterName(ParameterName.ParameterType.BooleanSwitch, Microsoft.VisualBasic.CommandLine.CommandLine.TrimBooleanSwitchPrefix(obj.Key))
                    Else
                        pName = New Tokens.ParameterName(ObjectModels.Tokens.ParameterName.ParameterType.Normal, obj.Key)
                    End If

                    Dim pValue = New Tokens.InternalExpression(obj.Value)

                    Call hash.Add(pName, pValue)
                Next
            End If

            Return hash
        End Function

        Private Function GetOperator(Tokens As Tokens.Token()) As Tokens.Operator.Operators
            If Tokens.Count <= 2 Then
                Return ObjectModels.Tokens.Operator.Operators.NULL
            End If

            Dim Temp = Tokens(1)
            Dim [operator] = ShoalShell.Interpreter.ObjectModels.Tokens.Operator.GetOperator(Temp.GetTokenValue)
            Return [operator]
        End Function

        ''' <summary>
        ''' <see cref="ShoalShell.Interpreter.ObjectModels.Tokens.[Operator].Operators .ExtCall"/>
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="expression"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseExtCall(expression As String, Tokens As Tokens.Token()) As Expression

            If GetOperator(Tokens) <> ObjectModels.Tokens.Operator.Operators.ExtCall Then
                Return Nothing
            End If

            Dim Expr As Expression = New Expression(expression) With {.Operator = New ObjectModels.Tokens.Operator("->"),
                .EntryPoint = New ObjectModels.Tokens.EntryPoint(Tokens(2)),
                .ExtensionVariable = New ObjectModels.Tokens.LeftAssignedVariable(Tokens(0)),
                .LeftAssignedVariable = New ObjectModels.Tokens.LeftAssignedVariable("$")}

            Call TryGetParameters(Tokens.Skip(3).ToArray, Expr.Parameters, Expr.BooleanSwitches)

            Return Expr

        End Function

        Private Sub TryGetParameters(Tokens As Token(), ByRef parameters As Dictionary(Of ParameterName, InternalExpression), ByRef BooleanSwitches As String())
            Dim Temp As String() = (From obj In Tokens Select obj.GetTokenValue).ToArray
            Dim SingleParameter As String = ""
            Dim Parser = Microsoft.VisualBasic.CommandLine.CommandLine.GetPossibleSwitches(Temp, SingleParameter)
            Dim params = Microsoft.VisualBasic.CommandLine.CommandLine.CreateParameterValues(Temp, True)

            parameters = New Dictionary(Of ParameterName, InternalExpression)

            If Not String.IsNullOrEmpty(SingleParameter) Then
                Call parameters.Add(New ParameterName(ParameterName.ParameterType.SingleParameter, ""), New InternalExpression(SingleParameter))
            End If

            parameters.AddRange((From obj In params
                                 Select Name = New ParameterName(ParameterName.ParameterType.Normal, obj.Key),
                              value = New InternalExpression(obj.Value)).ToArray.ToDictionary(Function(obj) obj.Name, elementSelector:=Function(obj) obj.value))
            BooleanSwitches = (From s As String In Parser Select CommandLine.CommandLine.TrimBooleanSwitchPrefix(s)).ToArray
        End Sub

        ''' <summary>
        ''' <see cref="ShoalShell.Interpreter.ObjectModels.Tokens.[Operator].Operators .CollectionOpr"/>
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="expression"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseCollectionOpr(expression As String, Tokens As Tokens.Token()) As Expression
            If GetOperator(Tokens) <> ObjectModels.Tokens.Operator.Operators.CollectionOpr Then
                Return Nothing
            End If

            Dim Expr As Expression = New Expression(expression) With {.Operator = New ObjectModels.Tokens.Operator("<=")}
            Return Expr
        End Function

        ''' <summary>
        ''' <see cref="ShoalShell.Interpreter.ObjectModels.Tokens.[Operator].Operators .SelfCast"/>
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="expression"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseSelfCast(expression As String, Tokens As Tokens.Token()) As Expression
            If GetOperator(Tokens) <> ObjectModels.Tokens.Operator.Operators.SelfCast Then
                Return Nothing
            End If

            Dim Expr As Expression = New Expression(expression) With {.Operator = New ObjectModels.Tokens.Operator("=")}
            Return Expr
        End Function

        ''' <summary>
        ''' <see cref="ShoalShell.Interpreter.ObjectModels.Tokens.[Operator].Operators .HybridsScript"/>
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="expression"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseHybridsScript(expression As String, Tokens As Tokens.Token()) As Expression
            If GetOperator(Tokens) <> ObjectModels.Tokens.Operator.Operators.HybridsScript Then
                Return Nothing
            End If

            Dim Expr As Expression = New Expression(expression) With {.Operator = New ObjectModels.Tokens.Operator("<<")}
            Return Expr
        End Function

        ''' <summary>
        ''' <see cref="ShoalShell.Interpreter.ObjectModels.Tokens.[Operator].Operators .HybirdsScriptPush"/>
        ''' </summary>
        ''' <param name="expression">只是用于显示的原始脚本行</param>
        ''' <param name="Tokens">使用这个已经解析好的词元进行<see cref="expression"/></param>对象的生成
        ''' <returns></returns>
        Public Function TryParseHybirdsScriptPush(expression As String, Tokens As Tokens.Token()) As Expression
            If GetOperator(Tokens) <> ObjectModels.Tokens.Operator.Operators.HybirdsScriptPush Then
                Return Nothing
            End If

            Dim Expr As Expression = New Expression(expression) With {.Operator = New ObjectModels.Tokens.Operator(">>")}
            Return Expr
        End Function

    End Module
End Namespace