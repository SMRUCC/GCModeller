Imports System.Text.RegularExpressions
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Script

    ''' <summary>
    ''' 向量表达式改写器: 逐行把「数值向量参与的标量写法」改写为运行时的 SIMD 调用。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>为什么要用 Roslyn</b>: 表达式改写必须正确处理运算符优先级、括号嵌套、
    ''' 实参列表、lambda 等语法结构, 纯正则无法可靠完成。工程已经依赖
    ''' <c>Microsoft.CodeAnalysis.VisualBasic</c>, 因此这里直接用
    ''' 解析器(见 <see cref="ParseStatementsOfLine"/>)拿到语句的完整语法树。
    ''' </para>
    ''' <para>
    ''' <b>工作流程</b>(每一行):
    ''' <list type="number">
    ''' <item>登记函数签名中的向量参数、函数名(供后续"同名函数不改写"判断);</item>
    ''' <item>解析语句, 遇到语法错误/被跳过的词元直接放弃该行(保守策略);</item>
    ''' <item>登记本行声明的变量类型, 并撤销"被重新赋值为标量"的向量登记;</item>
    ''' <item><see cref="Render"/> 自底向上渲染整条语句, 遇到可向量化的表达式就发射 SIMD 调用,
    ''' 其余节点递归替换子表达式后原样拼回。</item>
    ''' </list>
    ''' </para>
    ''' <para>
    ''' <b>保守性保证</b>: 只要出现任何一种不确定(未知标识符、多行表达式续行、括号不配平、
    ''' 用户自定义的同名函数、方括号下标访问等), 就放弃该表达式的改写。
    ''' 放弃改写等价于保持原有行为 —— 而这些写法在引入本功能之前本来也无法编译,
    ''' 因此不存在行为回归。
    ''' </para>
    ''' </remarks>
    Public Class VectorExpressionRewriter

        ''' <summary>
        ''' 语句解析桩的前缀: 被解析的一行源码会被嵌入
        ''' <c>LineStub + line + LF + "End Sub"</c> 之中作为一个方法体。
        ''' </summary>
        Private Shared ReadOnly LineStub As String = "Sub __vbs_vectorize_probe()" & vbLf

        ''' <summary>函数/子程序签名之中的参数: <c>[ByVal] name [()] As Type [()] [= default]</c></summary>
        Private ReadOnly ParameterPattern As New Regex(
            "^(?:(?:byval|byref|optional|paramarray)\s+)*(?<name>[A-Za-z_]\w*)\s*(?:\(\s*\))?\s+" &
            "As\s+(?<type>[A-Za-z_]\w*(?:\.\w+)*)\s*(?<rank>\(\s*,?\s*\))?\s*(?:=[\s\S]*)?$",
            RegexOptions.IgnoreCase)

        ''' <summary>顶层 Function/Sub 定义(用于"用户自定义同名函数"判断)</summary>
        Private ReadOnly FunctionPattern As New Regex(
            "^\s*(?:(?:public|private|friend|protected|shared|static|overloads|overrides)\s+)*(?:function|sub)\s+(?<name>[A-Za-z_]\w*)",
            RegexOptions.IgnoreCase)

        ''' <summary>变量名 → 浅层类型(只登记类型确定的名称)</summary>
        Private ReadOnly _types As New Dictionary(Of String, ValueTypeInfo)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>在不同作用域被声明为不同类型的名称: 一律不参与改写</summary>
        Private ReadOnly _conflicts As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>脚本自身(或 lambda 赋值)定义过的函数名: 不做逐元素数学函数改写</summary>
        Private ReadOnly _functions As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>当前已知的向量变量名(供跳过报告与内部使用)</summary>
        Public ReadOnly Property VectorNames As String()
            Get
                Return _types _
                    .Where(Function(kv) kv.Value.IsVector) _
                    .Select(Function(kv) kv.Key) _
                    .ToArray()
            End Get
        End Property

        ' ==================================================================
        ' 行级入口
        ' ==================================================================

        ''' <summary>
        ''' 对一行脚本源码做向量化改写; 无法处理时原样返回。
        ''' </summary>
        Public Function RewriteLine(line As String, report As VectorizationReport) As String
            If String.IsNullOrWhiteSpace(line) Then
                Return line
            End If

            Dim trimmed As String = line.TrimStart()

            ' 注释行与预处理指令行不参与改写(#no-vectorize 等指令已在上游剔除)
            If trimmed.StartsWith("'"c) OrElse trimmed.StartsWith("#"c) Then
                Return line
            End If

            ' 记录本行处理**之前**就已经登记的向量名: 跳过报告只依据它们,
            ' 否则"本行自己声明的向量"会让声明行自己也被报成"未能改写"。
            Dim knownBefore As String() = VectorNames

            ' 1. 函数签名: 登记向量参数与函数名(签名行本身不是语句, 解析会失败)
            Call RegisterParameters(line)
            Call RegisterFunctionName(line)

            ' 2. 解析语句; 语法不完整(块首行/块尾行/跨行表达式)时放弃
            Dim statements As List(Of StatementSyntax) = ParseStatementsOfLine(line)

            If statements.Count = 0 Then
                Call ReportSkipped(line, report, knownBefore)
                Return line
            End If

            ' 3. 先左到右登记全部声明(靠后的语句要能看到靠前的声明),
            '    并跳过语法不完整的语句
            Dim accepted As New List(Of StatementSyntax)

            For Each stmt As StatementSyntax In statements
                If stmt.ContainsDiagnostics OrElse HasSkippedTokens(stmt) Then
                    Continue For
                End If

                Call RegisterDeclarations(stmt)
                Call accepted.Add(stmt)
            Next

            ' 4. 再右到左渲染并回写, 保证左侧语句的字符偏移在替换后依然有效
            Dim text As String = line
            Dim modified As Boolean = False

            For i As Integer = accepted.Count - 1 To 0 Step -1
                Dim stmt As StatementSyntax = accepted(i)
                Dim rendered As String = Render(stmt)

                If String.Equals(rendered, stmt.ToString(), StringComparison.Ordinal) Then
                    Continue For
                End If

                Dim start As Integer = stmt.SpanStart - LineStub.Length

                text = text.Substring(0, start) & rendered & text.Substring(start + stmt.Span.Length)
                modified = True
            Next

            If modified Then
                Return text
            End If

            Call ReportSkipped(line, report)
            Return line
        End Function

        ''' <summary>
        ''' 解析一行源码之中的全部语句。
        ''' </summary>
        ''' <remarks>
        ''' VB 的 <c>SyntaxFactory</c> 没有 <c>ParseStatement</c>(那是 C# 侧的 API),
        ''' 因此这里把整行**塞进一个空方法体**之后再用
        ''' <c>VisualBasicSyntaxTree.ParseText</c> 解析:
        ''' <code>
        ''' Sub __vbs_vectorize_probe()
        ''' &lt;line&gt;
        ''' End Sub
        ''' </code>
        ''' 这样做的好处是可以完整复用 VB 的语句/表达式文法(优先级、实参表、lambda 等),
        ''' 并且 <c>a = 1 : b = 2</c> 这类冒号分隔的多语句行也能一次全部拿到。
        ''' 块首行(例如 <c>If x Then</c>)会让桩方法无法闭合, 解析结果带诊断信息, 从而被安全跳过。
        ''' </remarks>
        Private Shared Function ParseStatementsOfLine(line As String) As List(Of StatementSyntax)
            Dim result As New List(Of StatementSyntax)()

            ' 行内含有 CR 时偏移计算不再可靠(桩之中统一使用 LF), 直接放弃
            If line.IndexOf(vbCr) >= 0 Then
                Return result
            End If

            Dim stub As String = LineStub & line & vbLf & "End Sub"
            Dim tree As SyntaxTree = VisualBasicSyntaxTree.ParseText(stub)
            Dim probe As MethodBlockSyntax = tree.GetRoot() _
                .DescendantNodes() _
                .OfType(Of MethodBlockSyntax)() _
                .FirstOrDefault()

            If probe Is Nothing Then
                Return result
            End If

            For Each stmt As StatementSyntax In probe.Statements
                ' 语句必须完整落在被探测的那一行之内
                Dim start As Integer = stmt.SpanStart - LineStub.Length

                If start < 0 OrElse (start + stmt.Span.Length) > line.Length Then
                    Continue For
                End If

                Call result.Add(stmt)
            Next

            Return result
        End Function

        ' ==================================================================
        ' 渲染(自底向上)
        ' ==================================================================

        ''' <summary>
        ''' 渲染一个语法节点: 若该节点本身可以被向量化则直接发射 SIMD 调用,
        ''' 否则递归渲染其中的子表达式并把结果拼回原文本。
        ''' </summary>
        Private Function Render(node As SyntaxNode) As String
            If node Is Nothing Then
                Return Nothing
            End If

            Dim replacement As String = TryEmit(node)

            If replacement IsNot Nothing Then
                Return replacement
            End If

            Dim units As List(Of SyntaxNode) = CollectUnits(node)
            Dim text As String = node.ToString()

            If units.Count = 0 Then
                Return text
            End If

            ' 从右向左替换, 保证左侧单元的偏移在替换之后依然有效
            For i As Integer = units.Count - 1 To 0 Step -1
                Dim unit As SyntaxNode = units(i)
                Dim rendered As String = Render(unit)

                If String.Equals(rendered, unit.ToString(), StringComparison.Ordinal) Then
                    Continue For
                End If

                Dim offset As Integer = unit.SpanStart - node.SpanStart

                text = text.Substring(0, offset) & rendered & text.Substring(offset + unit.Span.Length)
            Next

            Return text
        End Function

        ''' <summary>
        ''' 收集一个节点之中"需要独立渲染"的直接子节点。
        ''' </summary>
        ''' <remarks>
        ''' 除了表达式本身, 还需要下钻这些"语法包装节点", 否则其内部的表达式到不了:
        ''' <list type="bullet">
        ''' <item><c>ArgumentList</c>/<c>Argument</c>: 调用语句的实参列表(实参里面才是表达式);</item>
        ''' <item><c>EqualsValue</c>: <c>Dim x = expr</c> 之中的 <c>= expr</c> 部分;</item>
        ''' <item><c>VariableDeclarator</c>: <c>Dim</c> 语句的声明项;</item>
        ''' <item><c>Statement</c>: 单行 <c>If cond Then stmt</c> 之中的内嵌语句。</item>
        ''' </list>
        ''' </remarks>
        Private Shared Function CollectUnits(node As SyntaxNode) As List(Of SyntaxNode)
            Dim units As New List(Of SyntaxNode)

            For Each child As SyntaxNode In node.ChildNodes()
                If TypeOf child Is ExpressionSyntax OrElse
                   TypeOf child Is ArgumentListSyntax OrElse
                   TypeOf child Is ArgumentSyntax OrElse
                   TypeOf child Is EqualsValueSyntax OrElse
                   TypeOf child Is VariableDeclaratorSyntax OrElse
                   TypeOf child Is StatementSyntax Then

                    Call units.Add(child)
                End If
            Next

            Return units
        End Function

        ''' <summary>尝试把整个节点发射为一个 SIMD 调用; 不可向量化时返回 Nothing</summary>
        Private Function TryEmit(node As SyntaxNode) As String
            Dim invocation As InvocationExpressionSyntax = TryCast(node, InvocationExpressionSyntax)

            If invocation IsNot Nothing Then
                Return TryEmitInvocation(invocation)
            End If

            ' 一元取负: 操作数是向量时整体向量化
            Dim unary As UnaryExpressionSyntax = TryCast(node, UnaryExpressionSyntax)

            If unary IsNot Nothing AndAlso unary.Kind = SyntaxKind.UnaryMinusExpression Then
                Dim operand As VectorOperand = MakeOperand(unary.Operand)

                If operand.Type.IsNumericVector Then
                    Return SimdVocabulary.EmitNegate(operand)
                End If

                Return Nothing
            End If

            ' 二元算术: 只要有一侧是向量就整体向量化(整棵子树一次性发射)
            Dim binary As BinaryExpressionSyntax = TryCast(node, BinaryExpressionSyntax)

            If binary IsNot Nothing Then
                Dim op As String = OperatorText(binary)

                If op Is Nothing Then
                    Return Nothing
                End If

                Return SimdVocabulary.EmitBinary(op, MakeOperand(binary.Left), MakeOperand(binary.Right))
            End If

            Return Nothing
        End Function

        ''' <summary>把一个表达式包装成"已渲染文本 + 浅层类型"的运算对象</summary>
        Private Function MakeOperand(expr As ExpressionSyntax) As VectorOperand
            Return New VectorOperand(Render(expr), InferType(expr))
        End Function

        ' ==================================================================
        ' 调用表达式: 聚合归约 / 逐元素数学函数
        ' ==================================================================

        Private Function TryEmitInvocation(invocation As InvocationExpressionSyntax) As String
            Dim reduce As String = TryEmitReduce(invocation)

            If reduce IsNot Nothing Then
                Return reduce
            End If

            Return TryEmitMath(invocation)
        End Function

        ''' <summary>发射 <c>x.Sum()</c> 形式的聚合归约</summary>
        Private Function TryEmitReduce(invocation As InvocationExpressionSyntax) As String
            Dim receiver As ExpressionSyntax = Nothing
            Dim name As String = Nothing

            If Not TryGetReduceCall(invocation, receiver, name) Then
                Return Nothing
            End If

            Return SimdVocabulary.EmitReduce(name, MakeOperand(receiver))
        End Function

        ''' <summary>识别 <c>x.Sum()</c> / <c>x.Average()</c> / <c>x.Count()</c> 这类零实参聚合调用</summary>
        Private Shared Function TryGetReduceCall(invocation As InvocationExpressionSyntax,
                                                 ByRef receiver As ExpressionSyntax,
                                                 ByRef name As String) As Boolean

            receiver = Nothing
            name = Nothing

            If invocation.ArgumentList Is Nothing OrElse invocation.ArgumentList.Arguments.Count <> 0 Then
                Return False
            End If

            Dim member As MemberAccessExpressionSyntax = TryCast(invocation.Expression, MemberAccessExpressionSyntax)

            If member Is Nothing Then
                Return False
            End If

            name = member.Name.Identifier.ValueText

            If Not SimdVocabulary.IsReduceName(name) Then
                Return False
            End If

            receiver = member.Expression
            Return True
        End Function

        ''' <summary>发射逐元素数学函数调用</summary>
        Private Function TryEmitMath(invocation As InvocationExpressionSyntax) As String
            Dim name As String = Nothing

            If Not TryGetMathFunctionName(invocation, name) Then
                Return Nothing
            End If

            Dim args As New List(Of String)
            Dim types As New List(Of ValueTypeInfo)

            For Each arg As ArgumentSyntax In invocation.ArgumentList.Arguments
                Dim expr As ExpressionSyntax = arg.GetExpression()

                Call args.Add(Render(expr))
                Call types.Add(InferType(expr))
            Next

            ' Math.Pow(x, y) 与 x ^ y 等价, 复用运算符发射路径
            If String.Equals(name, "Pow", StringComparison.OrdinalIgnoreCase) AndAlso args.Count = 2 Then
                Return SimdVocabulary.EmitBinary("^",
                    New VectorOperand(args(0), types(0)),
                    New VectorOperand(args(1), types(1)))
            End If

            Return SimdVocabulary.EmitMathFunction(name, args.ToArray(), types.ToArray())
        End Function

        ''' <summary>
        ''' 判断调用是否为可向量化的数学函数: 接受 <c>Math.Sqrt(x)</c> 与裸名 <c>Sqrt(x)</c> 两种写法。
        ''' </summary>
        ''' <remarks>
        ''' 裸名写法只在「脚本自身没有定义同名函数」时才接受, 避免静默改变用户自定义函数的行为。
        ''' </remarks>
        Private Function TryGetMathFunctionName(invocation As InvocationExpressionSyntax, ByRef name As String) As Boolean
            name = Nothing

            Dim member As MemberAccessExpressionSyntax = TryCast(invocation.Expression, MemberAccessExpressionSyntax)

            If member IsNot Nothing Then
                Dim receiver As String = member.Expression.ToString().Trim()

                If String.Equals(receiver, "Math", StringComparison.OrdinalIgnoreCase) OrElse
                   String.Equals(receiver, "System.Math", StringComparison.OrdinalIgnoreCase) Then

                    name = member.Name.Identifier.ValueText
                    Return SimdVocabulary.IsMathFunctionName(name)
                End If

                Return False
            End If

            Dim identifier As IdentifierNameSyntax = TryCast(invocation.Expression, IdentifierNameSyntax)

            If identifier Is Nothing Then
                Return False
            End If

            Dim bare As String = identifier.Identifier.ValueText

            If _functions.Contains(bare) OrElse Not SimdVocabulary.IsMathFunctionName(bare) Then
                Return False
            End If

            name = bare
            Return True
        End Function

        ' ==================================================================
        ' 类型推断
        ' ==================================================================

        ''' <summary>
        ''' 推断一个表达式的浅层类型(元素类型 + 是否向量)。
        ''' 无法确定时返回 <see cref="NumericKind.Unknown"/>, 调用方据此放弃改写。
        ''' </summary>
        Friend Function InferType(expr As ExpressionSyntax) As ValueTypeInfo
            If expr Is Nothing Then
                Return New ValueTypeInfo()
            End If

            Select Case expr.Kind
                Case SyntaxKind.IdentifierName
                    Dim info As ValueTypeInfo

                    If _types.TryGetValue(DirectCast(expr, IdentifierNameSyntax).Identifier.ValueText, info) Then
                        Return info
                    Else
                        Return New ValueTypeInfo()
                    End If

                Case SyntaxKind.ParenthesizedExpression
                    Return InferType(DirectCast(expr, ParenthesizedExpressionSyntax).Expression)

                Case SyntaxKind.UnaryMinusExpression, SyntaxKind.UnaryPlusExpression
                    ' 一元运算不改变类型
                    Return InferType(DirectCast(expr, UnaryExpressionSyntax).Operand)

                Case SyntaxKind.NumericLiteralExpression
                    Dim kind As NumericKind = VectorType.LiteralKind(DirectCast(expr, LiteralExpressionSyntax).Token.Text)

                    Return New ValueTypeInfo(kind, False)

                Case SyntaxKind.AddExpression, SyntaxKind.SubtractExpression, SyntaxKind.MultiplyExpression,
                     SyntaxKind.DivideExpression, SyntaxKind.IntegerDivideExpression, SyntaxKind.ModuloExpression,
                     SyntaxKind.ExponentiateExpression

                    Return InferBinary(DirectCast(expr, BinaryExpressionSyntax))

                Case SyntaxKind.InvocationExpression
                    Return InferInvocation(DirectCast(expr, InvocationExpressionSyntax))

                Case SyntaxKind.CollectionInitializer
                    ' 数组字面量: Dim x = {1, 2, 3, 4, 5}
                    Return InferArrayLiteral(DirectCast(expr, CollectionInitializerSyntax))

                Case Else
                    Return InferCreation(expr)
            End Select
        End Function

        ''' <summary>
        ''' 推断数组字面量 <c>{...}</c> 的类型: 全部元素都必须是数值标量,
        ''' 结果元素类型取它们的公共数值类型(<c>{1, 2.5}</c> 得到 <c>Double()</c>)。
        ''' </summary>
        Private Function InferArrayLiteral(literal As CollectionInitializerSyntax) As ValueTypeInfo
            Dim kind As NumericKind = NumericKind.Unknown
            Dim first As Boolean = True

            For Each element As ExpressionSyntax In literal.Initializers
                Dim item As ValueTypeInfo = InferType(element)

                If Not item.IsKnown OrElse item.IsVector Then
                    ' 空字面量 {} / 含非数值元素 / 含嵌套数组 => 不是数值向量
                    Return New ValueTypeInfo()
                End If

                ' 注意: 不能用 Promote 直接累加 —— 它对 Unknown 的语义是"传染",
                ' 而这里 Unknown 是"尚未有第一个元素"的初始状态。
                If first Then
                    kind = item.Kind
                    first = False
                Else
                    kind = VectorType.Promote(kind, item.Kind)
                End If
            Next

            If kind = NumericKind.Unknown Then
                Return New ValueTypeInfo()
            End If

            Return New ValueTypeInfo(kind, True)
        End Function

        ''' <summary>
        ''' 推断 <c>New T(n) {}</c> / <c>New T(n)</c> 形式的数组创建表达式。
        ''' </summary>
        ''' <remarks>
        ''' <c>New Double()</c>(无实参)是标量构造调用; 只有在出现数组上界实参
        ''' (<c>New Double(4)</c>, 长度为 5)或者花括号初始化器时才视为数组。
        ''' </remarks>
        Private Function InferCreation(expr As ExpressionSyntax) As ValueTypeInfo
            Dim creation As ArrayCreationExpressionSyntax = TryCast(expr, ArrayCreationExpressionSyntax)

            If creation IsNot Nothing Then
                Return InferCreatedArray(creation.Type, hasBounds:=True)
            End If

            Dim [new] As ObjectCreationExpressionSyntax = TryCast(expr, ObjectCreationExpressionSyntax)

            If [new] Is Nothing Then
                Return New ValueTypeInfo()
            End If

            Dim hasBounds As Boolean = [new].Initializer IsNot Nothing OrElse
                                       ([new].ArgumentList IsNot Nothing AndAlso
                                        [new].ArgumentList.Arguments.Count > 0)

            Return InferCreatedArray([new].Type, hasBounds)
        End Function

        Private Function InferCreatedArray(type As TypeSyntax, hasBounds As Boolean) As ValueTypeInfo
            ' New Integer() 这类写法: 类型本身已经是数组类型
            Dim array As ArrayTypeSyntax = TryCast(type, ArrayTypeSyntax)

            If array IsNot Nothing Then
                Return ResolveTypeSyntax(array)
            End If

            If Not hasBounds Then
                Return New ValueTypeInfo()
            End If

            Dim kind As NumericKind

            If VectorType.TryParseKind(type.ToString(), kind) Then
                Return New ValueTypeInfo(kind, True)
            End If

            Return New ValueTypeInfo()
        End Function

        Private Function InferBinary(binary As BinaryExpressionSyntax) As ValueTypeInfo
            Dim left As ValueTypeInfo = InferType(binary.Left)
            Dim right As ValueTypeInfo = InferType(binary.Right)

            If Not left.IsKnown OrElse Not right.IsKnown Then
                Return New ValueTypeInfo()
            End If

            Dim op As String = OperatorText(binary)
            Dim kind As NumericKind

            Select Case op
                Case "+", "-", "*" : kind = VectorType.Promote(left.Kind, right.Kind)
                Case "Mod" : kind = VectorType.ModuloKind(left.Kind, right.Kind)
                Case "/" : kind = VectorType.DivideKind(left.Kind, right.Kind)
                Case "\" : kind = VectorType.IntegerDivideKind(left.Kind, right.Kind)
                Case "^" : kind = VectorType.PowerKind(left.Kind, right.Kind)
                Case Else : Return New ValueTypeInfo()
            End Select

            If kind = NumericKind.Unknown Then
                Return New ValueTypeInfo()
            End If

            Return New ValueTypeInfo(kind, left.IsVector OrElse right.IsVector)
        End Function

        Private Function InferInvocation(invocation As InvocationExpressionSyntax) As ValueTypeInfo
            ' 聚合归约
            Dim receiver As ExpressionSyntax = Nothing
            Dim reduceName As String = Nothing

            If TryGetReduceCall(invocation, receiver, reduceName) Then
                Dim recv As ValueTypeInfo = InferType(receiver)

                If recv.IsNumericVector Then
                    Return SimdVocabulary.ReduceResult(reduceName, recv.Kind)
                End If
            End If

            ' 逐元素数学函数
            Dim functionName As String = Nothing

            If TryGetMathFunctionName(invocation, functionName) Then
                Dim types As New List(Of ValueTypeInfo)

                For Each arg As ArgumentSyntax In invocation.ArgumentList.Arguments
                    Call types.Add(InferType(arg.GetExpression()))
                Next

                If types.Count > 0 Then
                    If String.Equals(functionName, "Pow", StringComparison.OrdinalIgnoreCase) AndAlso types.Count = 2 Then
                        If Not types(0).IsKnown OrElse Not types(1).IsKnown Then
                            Return New ValueTypeInfo()
                        End If

                        Return New ValueTypeInfo(
                            VectorType.PowerKind(types(0).Kind, types(1).Kind),
                            types(0).IsVector OrElse types(1).IsVector)
                    End If

                    Return SimdVocabulary.MathResult(functionName, types(0), types.Count)
                End If
            End If

            Return New ValueTypeInfo()
        End Function

        ''' <summary>二元运算符语法种类 → VB 运算符文本</summary>
        Private Shared Function OperatorText(binary As BinaryExpressionSyntax) As String
            Select Case binary.Kind
                Case SyntaxKind.AddExpression : Return "+"
                Case SyntaxKind.SubtractExpression : Return "-"
                Case SyntaxKind.MultiplyExpression : Return "*"
                Case SyntaxKind.DivideExpression : Return "/"
                Case SyntaxKind.IntegerDivideExpression : Return "\"
                Case SyntaxKind.ModuloExpression : Return "Mod"
                Case SyntaxKind.ExponentiateExpression : Return "^"
                Case Else : Return Nothing
            End Select
        End Function

        ' ==================================================================
        ' 声明登记
        ' ==================================================================

        ''' <summary>登记语句中声明的变量类型, 并撤销"被重新赋值为标量"的向量登记</summary>
        Private Sub RegisterDeclarations(stmt As StatementSyntax)
            For Each declarator As VariableDeclaratorSyntax In stmt.DescendantNodes().OfType(Of VariableDeclaratorSyntax)()
                Dim info As ValueTypeInfo = ResolveDeclaredType(declarator)

                For Each name As ModifiedIdentifierSyntax In declarator.Names
                    Call SetType(name.Identifier.ValueText, info)
                Next

                ' lambda 赋值(Dim f = Function(...))视同定义了同名函数
                If declarator.Initializer IsNot Nothing AndAlso
                   TypeOf declarator.Initializer.Value Is LambdaExpressionSyntax Then

                    For Each name As ModifiedIdentifierSyntax In declarator.Names
                        Call _functions.Add(name.Identifier.ValueText)
                    Next
                End If
            Next

            Dim assign As AssignmentStatementSyntax = TryCast(stmt, AssignmentStatementSyntax)

            If assign Is Nothing Then
                Return
            End If

            Dim target As IdentifierNameSyntax = TryCast(assign.Left, IdentifierNameSyntax)

            If target Is Nothing Then
                Return
            End If

            Dim rhs As ValueTypeInfo = InferType(assign.Right)

            If rhs.IsNumericVector Then
                Call SetType(target.Identifier.ValueText, rhs)
                Return
            End If

            ' 原本登记为向量, 现在被整体赋值为标量 => 撤销登记, 避免后续误改写
            Dim existing As ValueTypeInfo

            If _types.TryGetValue(target.Identifier.ValueText, existing) AndAlso existing.IsVector Then
                Call _types.Remove(target.Identifier.ValueText)
            End If
        End Sub

        ''' <summary>解析一个 <c>Dim</c> 声明项的类型</summary>
        Private Function ResolveDeclaredType(declarator As VariableDeclaratorSyntax) As ValueTypeInfo
            ' Dim x(5) As Integer 这类写法: 数组边界出现在变量名上, 元素类型在 As 子句上
            Dim hasBounds As Boolean = False

            For Each name As ModifiedIdentifierSyntax In declarator.Names
                If name.ArrayBounds IsNot Nothing Then
                    hasBounds = True
                    Exit For
                End If
            Next

            If declarator.AsClause IsNot Nothing Then
                Dim declared As ValueTypeInfo = ResolveAsClause(declarator.AsClause)

                If declared.IsKnown Then
                    If hasBounds AndAlso Not declared.IsVector Then
                        Return New ValueTypeInfo(declared.Kind, True)
                    End If

                    Return declared
                End If
            End If

            If hasBounds Then
                ' Dim x(5): 没有 As 子句 => 元素类型为 Object, 不参与向量化
                Return New ValueTypeInfo()
            End If

            If declarator.Initializer IsNot Nothing Then
                Return InferType(declarator.Initializer.Value)
            End If

            Return New ValueTypeInfo()
        End Function

        Private Function ResolveAsClause(asClause As AsClauseSyntax) As ValueTypeInfo
            Dim simple As SimpleAsClauseSyntax = TryCast(asClause, SimpleAsClauseSyntax)

            If simple IsNot Nothing Then
                Return ResolveTypeSyntax(simple.Type)
            End If

            Dim asNew As AsNewClauseSyntax = TryCast(asClause, AsNewClauseSyntax)

            If asNew IsNot Nothing AndAlso asNew.NewExpression IsNot Nothing Then
                Return ResolveTypeSyntax(asNew.NewExpression.Type)
            End If

            Return New ValueTypeInfo()
        End Function

        ''' <summary>解析一个类型语法: 只接受一维数值数组与数值标量</summary>
        Private Function ResolveTypeSyntax(type As TypeSyntax) As ValueTypeInfo
            Dim array As ArrayTypeSyntax = TryCast(type, ArrayTypeSyntax)

            If array IsNot Nothing Then
                ' 只支持一维数组; 交错数组/多维数组一律放弃
                If array.RankSpecifiers.Count <> 1 OrElse array.RankSpecifiers(0).Rank <> 1 Then
                    Return New ValueTypeInfo()
                End If

                Dim kind As NumericKind

                If VectorType.TryParseKind(array.ElementType.ToString(), kind) Then
                    Return New ValueTypeInfo(kind, True)
                End If

                Return New ValueTypeInfo()
            End If

            Dim scalar As NumericKind

            If VectorType.TryParseKind(type.ToString(), scalar) Then
                Return New ValueTypeInfo(scalar, False)
            End If

            Return New ValueTypeInfo()
        End Function

        ''' <summary>登记名称的类型; 同名不同类型时记入冲突名单并永久排除</summary>
        Private Sub SetType(name As String, info As ValueTypeInfo)
            If String.IsNullOrEmpty(name) OrElse _conflicts.Contains(name) Then
                Return
            End If

            Dim existing As ValueTypeInfo

            If _types.TryGetValue(name, existing) Then
                If existing.Kind <> info.Kind OrElse existing.IsVector <> info.IsVector Then
                    ' 不同作用域下同名变量类型不一致 => 之后一律不改写该名称
                    Call _types.Remove(name)
                    Call _conflicts.Add(name)
                End If

                Return
            End If

            If info.IsKnown Then
                _types(name) = info
            End If
        End Sub

        ' ==================================================================
        ' 函数签名与预检查
        ' ==================================================================

        Private Sub RegisterFunctionName(line As String)
            Dim m As Match = FunctionPattern.Match(line)

            If m.Success Then
                Call _functions.Add(m.Groups("name").Value)
            End If
        End Sub

        ''' <summary>登记函数/子程序/lambda 签名之中的向量参数</summary>
        Private Sub RegisterParameters(line As String)
            If Not Regex.IsMatch(line, "\b(function|sub)\b", RegexOptions.IgnoreCase) Then
                Return
            End If

            Dim parameters As String = ExtractParameterList(line)

            If parameters Is Nothing Then
                Return
            End If

            For Each part As String In ScriptStructure.SplitTopLevel(parameters)
                Dim m As Match = ParameterPattern.Match(part.Trim())

                If Not m.Success Then
                    Continue For
                End If

                Dim kind As NumericKind

                If Not VectorType.TryParseKind(m.Groups("type").Value, kind) Then
                    Continue For
                End If

                Call SetType(m.Groups("name").Value,
                             New ValueTypeInfo(kind, isVector:=m.Groups("rank").Success))
            Next
        End Sub

        ''' <summary>
        ''' 截取 <c>Function</c>/<c>Sub</c> 关键字之后的参数表(自动跳过泛型参数表 <c>(Of T)</c>);
        ''' 无参数表时返回 Nothing。
        ''' </summary>
        Private Shared Function ExtractParameterList(line As String) As String
            Dim keyword As Match = Regex.Match(line, "\b(function|sub)\b", RegexOptions.IgnoreCase)

            If Not keyword.Success Then
                Return Nothing
            End If

            Dim search As Integer = keyword.Index + keyword.Length

            Do While search < line.Length
                Dim openAt As Integer = line.IndexOf("("c, search)

                If openAt < 0 Then
                    Return Nothing
                End If

                Dim closeAt As Integer = MatchingClose(line, openAt)

                If closeAt < 0 Then
                    Return Nothing
                End If

                ' 泛型参数表 (Of T) 不是真正的参数表, 跳过它继续找
                Dim inner As String = line.Substring(openAt + 1, closeAt - openAt - 1)

                If Regex.IsMatch(inner, "^\s*Of\s", RegexOptions.IgnoreCase) Then
                    search = closeAt + 1
                    Continue Do
                End If

                Return inner
            Loop

            Return Nothing
        End Function

        ''' <summary>定位与 <paramref name="open"/> 处左括号配平的右括号下标; 不配平返回 -1</summary>
        Private Shared Function MatchingClose(text As String, open As Integer) As Integer
            Dim depth As Integer = 0

            For i As Integer = open To text.Length - 1
                Select Case text(i)
                    Case "("c
                        depth += 1
                    Case ")"c
                        depth -= 1

                        If depth = 0 Then
                            Return i
                        End If
                End Select
            Next

            Return -1
        End Function

        ''' <summary>语法树之中是否存在被跳过的词元(例如 <c>Dim a = 1 : Dim b = 2</c> 的后半段)</summary>
        Private Shared Function HasSkippedTokens(node As SyntaxNode) As Boolean
            For Each trivia As SyntaxTrivia In node.DescendantTrivia()
                If TypeOf trivia.GetStructure() Is SkippedTokensTriviaSyntax Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' 记录一行"疑似应该被改写但未能改写"的代码: 只有该行确实引用了已知的向量变量时才记录,
        ''' 避免把与向量化无关的语法(块首行、块尾行等)全部塞进报告。
        ''' </summary>
        Private Sub ReportSkipped(line As String, report As VectorizationReport)
            If report Is Nothing Then
                Return
            End If

            For Each name As String In _types.Keys
                If Regex.IsMatch(line, "\b" & Regex.Escape(name) & "\b", RegexOptions.IgnoreCase) Then
                    Call report.Skipped.Add(line.Trim())
                    Return
                End If
            Next
        End Sub
    End Class
End Namespace
