Namespace Script

    ''' <summary>
    ''' 向量化发射时的一个运算对象: 已经渲染好的 VB 表达式文本 + 它的浅层类型。
    ''' </summary>
    Public Structure VectorOperand

        ''' <summary>表达式的 VB 源码文本(其中的向量子表达式已完成向量化)</summary>
        Public ReadOnly Text As String

        ''' <summary>该表达式的浅层类型</summary>
        Public ReadOnly Type As ValueTypeInfo

        Public Sub New(text As String, type As ValueTypeInfo)
            Me.Text = text
            Me.Type = type
        End Sub
    End Structure

    ''' <summary>
    ''' 「向量表达式 → 运行时 <c>Microsoft.VisualBasic.Math.SIMD</c> 调用」的映射与发射。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 本模块只负责**纯文本发射**, 不依赖 Roslyn: 输入是运算符/函数名与已经渲染好的操作数,
    ''' 输出是可以直接写进生成代码的 VB 调用表达式。语法树的解析与类型推断由
    ''' <see cref="VectorExpressionRewriter"/> 负责, 两者职责分离。
    ''' </para>
    ''' <para>
    ''' <b>发射形态的约定</b>:
    ''' <list type="bullet">
    ''' <item>
    ''' 目标方法是 <b>具体</b> 重载时(例如 <c>SimdAdd(Double(), Double())</c>、<c>SimdModulo(Integer(), Integer())</c>)
    ''' 不写类型实参;</item>
    ''' <item>
    ''' 目标方法是 <b>泛型</b> 入口时(例如 <c>SimdAddScalar(Of T)</c>、<c>SimdAbs(Of T)</c>、<c>SimdConvert(Of TIn, TOut)</c>)
    ''' 一律显式写出类型实参 —— 显式类型实参会让重载解析只保留泛型候选,
    ''' 从根本上规避「新增泛型重载与既有具体重载产生二义性」的风险;</item>
    ''' <item>
    ''' 唯一的例外是 <c>SimdMultiplyScalar</c> + <see cref="NumericKind.Double"/>:
    ''' 运行时的具体重载走 <c>SimdParallel</c> 分块并行, 优于泛型入口的单线程内核,
    ''' 因此这一种情形刻意不写类型实参, 让具体重载胜出。</item>
    ''' </list>
    ''' </para>
    ''' <para>
    ''' <b>类型对齐</b>: VB 对数组不存在逐元素转换, 因此当某个向量操作数的元素类型
    ''' 与运算的目标类型不一致时, 必须在它外面包一层 <c>SimdConvert(Of TIn, TOut)</c>。
    ''' </para>
    ''' </remarks>
    Public Module SimdVocabulary

        ''' <summary>需要显式写出类型实参的泛型目标方法</summary>
        Private ReadOnly GenericTargets As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            "SimdAddScalar",
            "SimdSubtractScalar",
            "SimdMultiplyScalar",
            "SimdScalarSubtract",
            "SimdNegate",
            "SimdAbs",
            "SimdSquare",
            "SimdCount"
        }

        ''' <summary>
        ''' 逐元素数学函数名 → 支持改写。
        ''' </summary>
        ''' <remarks>
        ''' 只登记「语义与 <c>System.Math</c> 同名函数一致、且可以被逐元素化」的一元函数;
        ''' <c>Pow</c> 是二元的, 会被当作 <c>^</c> 运算符处理。
        ''' </remarks>
        Private ReadOnly MathFunctions As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            "Abs", "Sqrt", "Exp", "Log", "Log10", "Sign",
            "Floor", "Ceiling", "Truncate",
            "Sin", "Cos", "Tan", "Asin", "Acos", "Atan", "Round",
            "Pow"
        }

        ''' <summary>聚合归约成员名</summary>
        Private ReadOnly ReduceNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            "Sum", "Mean", "Average", "Min", "Max", "Product", "Count"
        }

#Region "运算符"

        ''' <summary>
        ''' 生成一个二元算术/比较运算的向量化调用文本; 不支持改写时返回 <c>Nothing</c>。
        ''' </summary>
        ''' <param name="op">VB 运算符文本(<c>+ - * / \ Mod ^</c>)</param>
        Public Function EmitBinary(op As String, left As VectorOperand, right As VectorOperand) As String
            If Not (left.Type.IsVector OrElse right.Type.IsVector) Then
                ' 两侧都是标量: 本就不需要向量化
                Return Nothing
            End If

            If Not left.Type.IsKnown OrElse Not right.Type.IsKnown Then
                ' 只要有一侧的数值类型不确定, 就无法确定提升结果, 放弃改写
                Return Nothing
            End If

            Dim target As NumericKind = ResultKind(op, left.Type.Kind, right.Type.Kind)

            If target = NumericKind.Unknown Then
                Return Nothing
            End If

            Dim method As String = MethodName(op, left.Type.IsVector, right.Type.IsVector)

            If method Is Nothing Then
                Return Nothing
            End If

            Dim vl As String = Coerce(left, target)
            Dim vr As String = Coerce(right, target)

            ' 交换律运算在「标量在左」时把向量换到第一个实参位置
            ' (SimdAddScalar / SimdMultiplyScalar 的向量参数在第一位)
            If Not left.Type.IsVector AndAlso (op = "+" OrElse op = "*") Then
                Dim swap As String = vl
                vl = vr
                vr = swap
            End If

            Return Invoke(method, target, {vl, vr})
        End Function

        ''' <summary>生成一元取负的向量化调用文本; 不支持改写时返回 <c>Nothing</c></summary>
        Public Function EmitNegate(operand As VectorOperand) As String
            If Not operand.Type.IsNumericVector Then
                Return Nothing
            End If

            Return Invoke("SimdNegate", operand.Type.Kind, {operand.Text})
        End Function

        ''' <summary>运算符 → 逐元素结果元素类型</summary>
        Private Function ResultKind(op As String, a As NumericKind, b As NumericKind) As NumericKind
            Select Case op
                Case "+", "-", "*" : Return VectorType.Promote(a, b)
                Case "Mod" : Return VectorType.ModuloKind(a, b)
                Case "/" : Return VectorType.DivideKind(a, b)
                Case "\" : Return VectorType.IntegerDivideKind(a, b)
                Case "^" : Return VectorType.PowerKind(a, b)
                Case Else : Return NumericKind.Unknown
            End Select
        End Function

        ''' <summary>运算符 + 操作数形态 → 运行时方法名</summary>
        Private Function MethodName(op As String, leftVector As Boolean, rightVector As Boolean) As String
            Dim both As Boolean = leftVector AndAlso rightVector

            Select Case op
                Case "+"
                    Return If(both, "SimdAdd", "SimdAddScalar")
                Case "*"
                    Return If(both, "SimdMultiply", "SimdMultiplyScalar")
                Case "-"
                    If both Then Return "SimdSubtract"
                    Return If(leftVector, "SimdSubtractScalar", "SimdScalarSubtract")
                Case "/"
                    If both Then Return "SimdDivide"
                    Return If(leftVector, "SimdDivideScalar", "SimdScalarDivide")
                Case "\"
                    If both Then Return "SimdIntegerDivide"
                    Return If(leftVector, "SimdIntegerDivideScalar", "SimdScalarIntegerDivide")
                Case "Mod"
                    If both Then Return "SimdModulo"
                    Return If(leftVector, "SimdModuloScalar", "SimdScalarModulo")
                Case "^"
                    If both Then Return "SimdPower"
                    Return If(leftVector, "SimdPowerScalar", "SimdScalarPower")
                Case Else
                    Return Nothing
            End Select
        End Function

        ''' <summary>把操作数的元素类型对齐到目标类型(必要时插入 SimdConvert)</summary>
        Private Function Coerce(operand As VectorOperand, target As NumericKind) As String
            If operand.Type.IsVector AndAlso operand.Type.Kind <> target Then
                Return $"SimdConvert(Of {VectorType.DisplayName(operand.Type.Kind)}, {VectorType.DisplayName(target)})({operand.Text})"
            End If

            ' 标量一律交给生成代码的隐式数值转换(生成代码固定为 Option Strict Off)
            Return operand.Text
        End Function

        ''' <summary>按目标方法是否泛型入口, 决定是否写出显式类型实参</summary>
        Private Function Invoke(method As String, target As NumericKind, args As String()) As String
            If NeedsTypeArgument(method, target) Then
                Return $"{method}(Of {VectorType.DisplayName(target)})({String.Join(", ", args)})"
            Else
                Return $"{method}({String.Join(", ", args)})"
            End If
        End Function

        Private Function NeedsTypeArgument(method As String, target As NumericKind) As Boolean
            ' 例外: Double 的向量乘标量保留具体重载, 以继续走 SimdParallel 的分块并行
            If String.Equals(method, "SimdMultiplyScalar", StringComparison.OrdinalIgnoreCase) AndAlso
               target = NumericKind.Double Then

                Return False
            End If

            Return GenericTargets.Contains(method)
        End Function

#End Region

#Region "聚合归约"

        ''' <summary>成员名是否为可向量化的聚合归约</summary>
        Public Function IsReduceName(name As String) As Boolean
            Return Not String.IsNullOrEmpty(name) AndAlso ReduceNames.Contains(name)
        End Function

        ''' <summary>
        ''' 聚合归约的结果类型(用于类型传播); 不支持该(归约, 元素类型)组合时返回 Unknown。
        ''' </summary>
        ''' <remarks>
        ''' 与运行时实际提供的 <c>Simd</c> 归约重载严格对应:
        ''' <c>Mean</c> 在运行时没有 <c>Short</c> 形态(而 VB/LINQ 也没有),
        ''' 因此对 <c>Short</c> 返回 Unknown 以避免发射出无法编译的调用。
        ''' </remarks>
        Public Function ReduceResult(name As String, kind As NumericKind) As ValueTypeInfo
            If kind = NumericKind.Unknown Then
                Return New ValueTypeInfo()
            End If

            Select Case name.ToLower()
                Case "sum", "product", "min", "max"
                    Return New ValueTypeInfo(kind, False)

                Case "mean", "average"
                    If kind = NumericKind.Short Then
                        Return New ValueTypeInfo()
                    End If

                    Return New ValueTypeInfo(If(kind = NumericKind.Single, NumericKind.Single, NumericKind.Double), False)

                Case "count"
                    Return New ValueTypeInfo(NumericKind.Integer, False)

                Case Else
                    Return New ValueTypeInfo()
            End Select
        End Function

        ''' <summary>生成聚合归约的向量化调用文本; 不支持时返回 <c>Nothing</c></summary>
        Public Function EmitReduce(name As String, operand As VectorOperand) As String
            Dim result As ValueTypeInfo = ReduceResult(name, operand.Type.Kind)

            If Not operand.Type.IsNumericVector OrElse Not result.IsKnown Then
                Return Nothing
            End If

            Select Case name.ToLower()
                Case "sum" : Return $"SimdSum({operand.Text})"
                Case "mean", "average" : Return $"SimdMean({operand.Text})"
                Case "min" : Return $"SimdMin({operand.Text})"
                Case "max" : Return $"SimdMax({operand.Text})"
                Case "product" : Return $"SimdProduct({operand.Text})"
                Case "count" : Return $"SimdCount(Of {VectorType.DisplayName(operand.Type.Kind)})({operand.Text})"
                Case Else : Return Nothing
            End Select
        End Function

#End Region

#Region "逐元素数学函数"

        ''' <summary>函数名是否为可被逐元素化的数学函数</summary>
        Public Function IsMathFunctionName(name As String) As Boolean
            Return Not String.IsNullOrEmpty(name) AndAlso MathFunctions.Contains(name)
        End Function

        ''' <summary>
        ''' 逐元素数学函数的结果类型(用于类型传播); 不支持该(函数, 参数类型)组合时返回 Unknown。
        ''' </summary>
        ''' <remarks>
        ''' 与运行时实际提供的重载严格对应:
        ''' <c>Sqrt</c>/<c>Exp</c>/<c>Log</c>/<c>Floor</c>/<c>Ceiling</c>/<c>Truncate</c>
        ''' 在运行时只有 <see cref="NumericKind.Double"/>/<see cref="NumericKind.Single"/> 形态,
        ''' 整数向量会先被 <c>SimdConvert</c> 提升为 <c>Double</c>;
        ''' 三角函数与 <c>Round</c> 没有专用内核, 统一通过 <c>SimdMap</c> 输出 <c>Double</c>。
        ''' </remarks>
        Public Function MathResult(name As String, argType As ValueTypeInfo, argCount As Integer) As ValueTypeInfo
            If Not argType.IsKnown Then
                Return New ValueTypeInfo()
            End If

            Dim argKind As NumericKind = argType.Kind
            Dim isSingle As Boolean = argKind = NumericKind.Single
            Dim result As NumericKind

            Select Case name.ToLower()
                Case "abs"
                    result = argKind

                Case "sqrt", "exp"
                    result = If(isSingle, NumericKind.Single, NumericKind.Double)

                Case "log"
                    result = If(argCount = 2, NumericKind.Double, If(isSingle, NumericKind.Single, NumericKind.Double))

                Case "log10", "sin", "cos", "tan", "asin", "acos", "atan", "round"
                    result = NumericKind.Double

                Case "sign"
                    ' Math.Sign 对 Single/Double 返回同类型; 对 Byte/SByte/Short 返回 Integer
                    result = If(argKind = NumericKind.Short, NumericKind.Integer, argKind)

                Case "floor", "ceiling", "truncate"
                    If argKind = NumericKind.Double OrElse isSingle Then
                        result = argKind
                    Else
                        Return New ValueTypeInfo()
                    End If

                Case Else
                    Return New ValueTypeInfo()
            End Select

            Return New ValueTypeInfo(result, argType.IsVector)
        End Function

        ''' <summary>
        ''' 生成逐元素数学函数的向量化调用文本; 不支持时返回 <c>Nothing</c>。
        ''' </summary>
        ''' <param name="name">函数名(不含 <c>Math.</c> 限定)</param>
        ''' <param name="args">已经渲染好的实参文本</param>
        ''' <param name="argTypes">实参的浅层类型(与 <paramref name="args"/> 等长)</param>
        Public Function EmitMathFunction(name As String, args As String(), argTypes As ValueTypeInfo()) As String
            If args.Length = 0 Then
                Return Nothing
            End If
            If Not argTypes(0).IsNumericVector Then
                ' 只有标量参与: 不属于向量化范畴
                Return Nothing
            End If

            Dim kind As NumericKind = argTypes(0).Kind
            Dim result As ValueTypeInfo = MathResult(name, argTypes(0), args.Length)

            If Not result.IsNumericVector Then
                Return Nothing
            End If

            ' 有专用 SIMD 内核的浮点函数在元素类型为整数时, 先把向量提升为 Double
            Dim effKind As NumericKind = kind
            Dim first As String = args(0)

            If kind <> NumericKind.Double AndAlso kind <> NumericKind.Single Then
                first = ConvertElement(args(0), kind, NumericKind.Double)
                effKind = NumericKind.Double
            End If

            Select Case name.ToLower()
                Case "abs"
                    Return $"SimdAbs(Of {VectorType.DisplayName(kind)})({args(0)})"

                Case "sqrt"
                    Return $"SimdSqrt({first})"

                Case "exp"
                    Return $"SimdExp({first})"

                Case "log"
                    If args.Length = 2 Then
                        ' SimdLog(v As Double(), base As Double) 只提供 Double 形态
                        Return $"SimdLog({ConvertElement(args(0), kind, NumericKind.Double)}, {args(1)})"
                    End If

                    Return $"SimdLog({first})"

                Case "sign"
                    If kind = NumericKind.Single OrElse kind = NumericKind.Double Then
                        Return $"SimdSign({args(0)})"
                    End If

                    ' Math.Sign 对整型返回同宽整型, 通过 SimdMap 逐元素调用
                    Return MapCall(kind, result.Kind, args(0), "Sign")

                Case "floor"
                    Return $"SimdFloor({first})"

                Case "ceiling"
                    Return $"SimdCeiling({first})"

                Case "truncate"
                    Return $"SimdTruncate({first})"

                Case Else
                    ' 没有专用 SIMD 内核的函数(三角函数、Round、Log10)统一走 SimdMap
                    Return MapCall(effKind, NumericKind.Double, first, name)
            End Select
        End Function

        ''' <summary>
        ''' 生成 <c>SimdMap</c> 形式的逐元素函数调用:
        ''' <c>SimdMap(Of K, R)(v, Function(x As K) Math.F(x))</c>。
        ''' </summary>
        ''' <remarks>
        ''' lambda 显式声明参数类型, 使两个泛型实参都能被确定地绑定;
        ''' <c>Math.F</c> 的形参通常是 <see cref="NumericKind.Double"/>, VB 的隐式拓宽保证
        ''' <c>Function(__v As Integer) Math.Sin(__v)</c> 这类写法可以编译。
        ''' 参数名固定为 <c>__v</c>, 避免与脚本之中的同名变量产生遮蔽冲突。
        ''' </remarks>
        Private Function MapCall(kind As NumericKind, result As NumericKind, arg As String, functionName As String) As String
            Dim k As String = VectorType.DisplayName(kind)
            Dim r As String = VectorType.DisplayName(result)

            Return $"SimdMap(Of {k}, {r})({arg}, Function(__v As {k}) Math.{functionName}(__v))"
        End Function

        ''' <summary>在元素类型不一致的一侧插入显式的逐元素类型转换</summary>
        Private Function ConvertElement(text As String, sourceKind As NumericKind, target As NumericKind) As String
            If sourceKind = target Then
                Return text
            End If

            Return $"SimdConvert(Of {VectorType.DisplayName(sourceKind)}, {VectorType.DisplayName(target)})({text})"
        End Function

#End Region
    End Module
End Namespace
