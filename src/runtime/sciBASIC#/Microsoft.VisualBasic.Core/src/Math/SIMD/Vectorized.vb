Imports System.Linq

Namespace Math.SIMD

    ''' <summary>
    ''' 向量化运算的统一词汇表。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <see cref="SimdExtensions"/> 面向 <see cref="Double"/> 提供了一整套 SIMD 门面，
    ''' 但 <see cref="Single"/> / <see cref="Integer"/> / <see cref="Long"/> / <see cref="Short"/>
    ''' 只覆盖了 <c>+ - *</c> 等少数算子，缺少标量广播、整除、取余、幂、一元函数与整数归约。
    ''' 本模块把这些缺口一次补齐，使得「脚本向量化改写器」可以用一套统一的 <c>Simd*</c>
    ''' 命名生成代码，而不必按元素类型分支选择不同的调用形态。
    ''' </para>
    ''' <para>
    ''' <b>关键约定</b>：
    ''' <list type="bullet">
    ''' <item>
    ''' <see cref="SimdExtensions"/> 已经提供的**具体**重载（尤其是 <see cref="Double"/> 的全部算子）
    ''' 一律不在这里重复声明 —— 既避免跨模块的重复签名造成二义性，
    ''' 也保证 <see cref="Double"/> 路径继续走 <see cref="SimdParallel"/> 的分块并行优化；
    ''' </item>
    ''' <item>
    ''' 这里新增的泛型重载与 <see cref="SimdExtensions"/> 的具体重载可以安全共存：
    ''' VB 的重载解析规则规定「非泛型候选优先于泛型候选」，
    ''' 因此 <c>SimdAddScalar(dblArray, 1.0)</c> 仍然会绑定到既有的具体重载；
    ''' </item>
    ''' <item>
    ''' 底层内核是泛型的运算（<see cref="SimdEngine"/> 的 <c>Add/Subtract/Multiply/*Scalar</c>、
    ''' <see cref="SimdMath"/> 的 <c>Abs/Negate/Square</c>）一律声明为泛型方法，直接转调内核；</item>
    ''' <item>
    ''' 内核不可用或硬件不支持的运算（VB 的 <c>\</c>、<c>Mod</c>、<c>^</c>、乘积归约、逐元素映射）
    ''' 退化为标量循环，与运行时既有的 <c>Modulo</c> / <c>Exponent</c> 门面保持同一实现水平，
    ''' 但语义与 VB 运算符逐元素展开严格一致。</item>
    ''' </list>
    ''' </para>
    ''' <para>
    ''' <b>长度契约</b>：全部向量⊕向量运算沿用 <see cref="SimdEngine"/> 的约定 ——
    ''' 假设两个输入长度一致，不做额外校验（长度不足时由运行时自身的边界检查抛出）。
    ''' </para>
    ''' </remarks>
    Public Module Vectorized

#Region "helpers"

        ''' <summary>
        ''' 逐元素二元运算的标量循环实现（没有 SIMD 内核可用时使用）。
        ''' </summary>
        ''' <remarks>
        ''' 之所以不能做成泛型的 <c>a + b</c> 形式：VB 不支持泛型运算符约束，
        ''' 所以运算本身必须由调用方以 lambda 注入。
        ''' </remarks>
        Private Function ZipMap(Of T)(v1 As T(), v2 As T(), f As Func(Of T, T, T)) As T()
            If v1 Is Nothing Then
                Throw New ArgumentNullException(NameOf(v1), "the input vector can not be NULL!")
            End If
            If v2 Is Nothing Then
                Throw New ArgumentNullException(NameOf(v2), "the input vector can not be NULL!")
            End If

            Dim len As Integer = v1.Length
            If len = 0 Then Return Array.Empty(Of T)()

            Dim out As T() = New T(len - 1) {}
            For i As Integer = 0 To len - 1
                out(i) = f(v1(i), v2(i))
            Next

            Return out
        End Function

        ''' <summary>向量 ⊕ 标量 的标量循环实现</summary>
        Private Function ZipScalarMap(Of T)(v As T(), scalar As T, f As Func(Of T, T, T)) As T()
            If v Is Nothing Then
                Throw New ArgumentNullException(NameOf(v), "the input vector can not be NULL!")
            End If

            Dim len As Integer = v.Length
            If len = 0 Then Return Array.Empty(Of T)()

            Dim out As T() = New T(len - 1) {}
            For i As Integer = 0 To len - 1
                out(i) = f(v(i), scalar)
            Next

            Return out
        End Function

        ''' <summary>标量 ⊕ 向量 的标量循环实现（标量在左操作数一侧）</summary>
        Private Function ScalarZipMap(Of T)(scalar As T, v As T(), f As Func(Of T, T, T)) As T()
            If v Is Nothing Then
                Throw New ArgumentNullException(NameOf(v), "the input vector can not be NULL!")
            End If

            Dim len As Integer = v.Length
            If len = 0 Then Return Array.Empty(Of T)()

            Dim out As T() = New T(len - 1) {}
            For i As Integer = 0 To len - 1
                out(i) = f(scalar, v(i))
            Next

            Return out
        End Function

#End Region

#Region "arithmetic: vector op vector"

        ''' <summary>
        ''' 逐元素相减（<see cref="Single"/>）：<c>out(i) = v1(i) - v2(i)</c>
        ''' </summary>
        ''' <remarks>
        ''' <see cref="SimdExtensions.SimdSubtract(Double(), Double())"/> 等已覆盖
        ''' <see cref="Double"/> / <see cref="Integer"/> / <see cref="Long"/> / <see cref="Short"/>，
        ''' 唯独缺少 <see cref="Single"/>，这里补齐以保持「一种元素类型一套算子」的完整性。
        ''' </remarks>
        Public Function SimdSubtract(v1 As Single(), v2 As Single()) As Single()
            Return SimdEngine.Subtract(Of Single)(v1, v2)
        End Function

        ''' <summary>
        ''' 逐元素整除（VB 的 <c>\</c> 运算符，<see cref="Integer"/>）。
        ''' </summary>
        ''' <remarks>
        ''' <c>System.Numerics.Vector</c> 没有整数除法通道（x86 的 <c>divpd</c>/<c>divps</c>
        ''' 只面向浮点），因此这里退化为标量循环；结果类型与 <c>Integer \ Integer</c> 一致。
        ''' </remarks>
        Public Function SimdIntegerDivide(v1 As Integer(), v2 As Integer()) As Integer()
            Return ZipMap(Of Integer)(v1, v2, Function(a, b) a \ b)
        End Function

        ''' <summary>逐元素整除（VB 的 <c>\</c> 运算符，<see cref="Long"/>）</summary>
        Public Function SimdIntegerDivide(v1 As Long(), v2 As Long()) As Long()
            Return ZipMap(Of Long)(v1, v2, Function(a, b) a \ b)
        End Function

        ''' <summary>
        ''' 逐元素取余（VB 的 <c>Mod</c> 运算符）。
        ''' </summary>
        ''' <remarks>
        ''' 没有可用的 SIMD 内核，退化为标量循环；<see cref="Short"/> 不单独提供，
        ''' 因为 VB 的数值提升会把 <c>Short Mod Short</c> 提升为 <see cref="Integer"/>。
        ''' </remarks>
        Public Function SimdModulo(v1 As Integer(), v2 As Integer()) As Integer()
            Return ZipMap(Of Integer)(v1, v2, Function(a, b) a Mod b)
        End Function

        ''' <summary>逐元素取余（VB 的 <c>Mod</c> 运算符，<see cref="Long"/>）</summary>
        Public Function SimdModulo(v1 As Long(), v2 As Long()) As Long()
            Return ZipMap(Of Long)(v1, v2, Function(a, b) a Mod b)
        End Function

        ''' <summary>逐元素取余（VB 的 <c>Mod</c> 运算符，<see cref="Single"/>）</summary>
        Public Function SimdModulo(v1 As Single(), v2 As Single()) As Single()
            Return ZipMap(Of Single)(v1, v2, Function(a, b) a Mod b)
        End Function

        ''' <summary>逐元素取余（VB 的 <c>Mod</c> 运算符，<see cref="Double"/>）</summary>
        Public Function SimdModulo(v1 As Double(), v2 As Double()) As Double()
            Return ZipMap(Of Double)(v1, v2, Function(a, b) a Mod b)
        End Function

        ''' <summary>
        ''' 逐元素幂（VB 的 <c>^</c> 运算符）：<c>out(i) = v1(i) ^ v2(i)</c>
        ''' </summary>
        ''' <remarks>
        ''' VB 的 <c>^</c> 无论操作数为何种数值类型，结果恒为 <see cref="Double"/>，
        ''' 因此这里只提供 <see cref="Double"/> 形态；转调内核 <see cref="SimdMath.Pow(Double(), Double())"/>。
        ''' </remarks>
        Public Function SimdPower(v1 As Double(), v2 As Double()) As Double()
            Return SimdMath.Pow(v1, v2)
        End Function

#End Region

#Region "arithmetic: vector op scalar"

        ''' <summary>向量加标量：<c>out(i) = v(i) + scalar</c></summary>
        Public Function SimdAddScalar(Of T As Structure)(v As T(), scalar As T) As T()
            Return SimdEngine.AddScalar(Of T)(v, scalar)
        End Function

        ''' <summary>向量减标量：<c>out(i) = v(i) - scalar</c></summary>
        Public Function SimdSubtractScalar(Of T As Structure)(v As T(), scalar As T) As T()
            Return SimdEngine.SubtractScalar(Of T)(v, scalar)
        End Function

        ''' <summary>标量减向量：<c>out(i) = scalar - v(i)</c></summary>
        Public Function SimdScalarSubtract(Of T As Structure)(scalar As T, v As T()) As T()
            Return SimdEngine.ScalarSubtract(Of T)(scalar, v)
        End Function

        ''' <summary>向量乘标量：<c>out(i) = v(i) * scalar</c></summary>
        Public Function SimdMultiplyScalar(Of T As Structure)(v As T(), scalar As T) As T()
            Return SimdEngine.MultiplyScalar(Of T)(scalar, v)
        End Function

        ''' <summary>
        ''' 向量除以标量（<see cref="Single"/>）：<c>out(i) = v(i) / scalar</c>
        ''' </summary>
        ''' <remarks><see cref="SimdExtensions.SimdDivideScalar(Double(), Double)"/> 只覆盖了 <see cref="Double"/>，这里补齐 Single。</remarks>
        Public Function SimdDivideScalar(v As Single(), scalar As Single) As Single()
            Return SimdEngine.DivideScalar(v, scalar)
        End Function

        ''' <summary>
        ''' 标量除以向量（<see cref="Double"/>）：<c>out(i) = scalar / v(i)</c>
        ''' </summary>
        Public Function SimdScalarDivide(scalar As Double, v As Double()) As Double()
            Return SimdEngine.ScalarDivide(scalar, v)
        End Function

        ''' <summary>标量除以向量（<see cref="Single"/>）</summary>
        Public Function SimdScalarDivide(scalar As Single, v As Single()) As Single()
            Return SimdEngine.ScalarDivide(scalar, v)
        End Function

        ''' <summary>向量整除标量（VB 的 <c>\</c>，<see cref="Integer"/>）</summary>
        Public Function SimdIntegerDivideScalar(v As Integer(), scalar As Integer) As Integer()
            Return ZipScalarMap(Of Integer)(v, scalar, Function(a, b) a \ b)
        End Function

        ''' <summary>向量整除标量（VB 的 <c>\</c>，<see cref="Long"/>）</summary>
        Public Function SimdIntegerDivideScalar(v As Long(), scalar As Long) As Long()
            Return ZipScalarMap(Of Long)(v, scalar, Function(a, b) a \ b)
        End Function

        ''' <summary>标量整除向量（VB 的 <c>\</c>，<see cref="Integer"/>）</summary>
        Public Function SimdScalarIntegerDivide(scalar As Integer, v As Integer()) As Integer()
            Return ScalarZipMap(Of Integer)(scalar, v, Function(a, b) a \ b)
        End Function

        ''' <summary>标量整除向量（VB 的 <c>\</c>，<see cref="Long"/>）</summary>
        Public Function SimdScalarIntegerDivide(scalar As Long, v As Long()) As Long()
            Return ScalarZipMap(Of Long)(scalar, v, Function(a, b) a \ b)
        End Function

        ''' <summary>向量对达量取余（VB 的 <c>Mod</c>，<see cref="Integer"/>）</summary>
        Public Function SimdModuloScalar(v As Integer(), scalar As Integer) As Integer()
            Return ZipScalarMap(Of Integer)(v, scalar, Function(a, b) a Mod b)
        End Function

        ''' <summary>向量对标量取余（VB 的 <c>Mod</c>，<see cref="Long"/>）</summary>
        Public Function SimdModuloScalar(v As Long(), scalar As Long) As Long()
            Return ZipScalarMap(Of Long)(v, scalar, Function(a, b) a Mod b)
        End Function

        ''' <summary>向量对标量取余（VB 的 <c>Mod</c>，<see cref="Single"/>）</summary>
        Public Function SimdModuloScalar(v As Single(), scalar As Single) As Single()
            Return ZipScalarMap(Of Single)(v, scalar, Function(a, b) a Mod b)
        End Function

        ''' <summary>向量对标量取余（VB 的 <c>Mod</c>，<see cref="Double"/>）</summary>
        Public Function SimdModuloScalar(v As Double(), scalar As Double) As Double()
            Return ZipScalarMap(Of Double)(v, scalar, Function(a, b) a Mod b)
        End Function

        ''' <summary>标量对向量取余（VB 的 <c>Mod</c>，<see cref="Integer"/>）</summary>
        Public Function SimdScalarModulo(scalar As Integer, v As Integer()) As Integer()
            Return ScalarZipMap(Of Integer)(scalar, v, Function(a, b) a Mod b)
        End Function

        ''' <summary>标量对向量取余（VB 的 <c>Mod</c>，<see cref="Long"/>）</summary>
        Public Function SimdScalarModulo(scalar As Long, v As Long()) As Long()
            Return ScalarZipMap(Of Long)(scalar, v, Function(a, b) a Mod b)
        End Function

        ''' <summary>标量对向量取余（VB 的 <c>Mod</c>，<see cref="Single"/>）</summary>
        Public Function SimdScalarModulo(scalar As Single, v As Single()) As Single()
            Return ScalarZipMap(Of Single)(scalar, v, Function(a, b) a Mod b)
        End Function

        ''' <summary>标量对向量取余（VB 的 <c>Mod</c>，<see cref="Double"/>）</summary>
        Public Function SimdScalarModulo(scalar As Double, v As Double()) As Double()
            Return ScalarZipMap(Of Double)(scalar, v, Function(a, b) a Mod b)
        End Function

        ''' <summary>
        ''' 向量的标量次幂（VB 的 <c>v ^ n</c>）：<c>out(i) = v(i) ^ exponent</c>
        ''' </summary>
        Public Function SimdPowerScalar(v As Double(), exponent As Double) As Double()
            Return SimdMath.PowScalar(v, exponent)
        End Function

        ''' <summary>
        ''' 标量的向量次幂（VB 的 <c>b ^ v</c>）：<c>out(i) = base ^ v(i)</c>
        ''' </summary>
        Public Function SimdScalarPower(base As Double, v As Double()) As Double()
            Return ZipScalarMap(Of Double)(v, base, Function(a, b) a ^ b)
        End Function

#End Region

#Region "unary"

        ''' <summary>
        ''' 逐元素取负：<c>out(i) = -v(i)</c>
        ''' </summary>
        ''' <remarks><see cref="SimdExtensions.SimdNegate(Double())"/> / <c>(Long())</c> 已存在，这里以泛型补齐其余元素类型。</remarks>
        Public Function SimdNegate(Of T As Structure)(v As T()) As T()
            Return SimdMath.Negate(Of T)(v)
        End Function

        ''' <summary>
        ''' 逐元素绝对值：<c>out(i) = |v(i)|</c>
        ''' </summary>
        ''' <remarks><see cref="SimdExtensions.SimdAbs(Double())"/> / <c>(Single())</c> / <c>(Integer())</c> 已存在，这里以泛型补齐其余元素类型。</remarks>
        Public Function SimdAbs(Of T As Structure)(v As T()) As T()
            Return SimdMath.Abs(Of T)(v)
        End Function

        ''' <summary>
        ''' 逐元素平方：<c>out(i) = v(i) ^ 2</c>
        ''' </summary>
        ''' <remarks>
        ''' 注意 VB 的 <c>^</c> 对数值类型恒返回 <see cref="Double"/>；
        ''' 这里保留元素类型是为了让改写器在 <c>v * v</c> 这种「平方」语义上避免多余的类型提升。
        ''' </remarks>
        Public Function SimdSquare(Of T As Structure)(v As T()) As T()
            Return SimdMath.Square(Of T)(v)
        End Function

        ' 逐元素平方根 SimdSqrt(Double()/Single()) 已由 SimdExtensions 提供，这里不再重复声明
        ' （两个模块中若出现完全相同的签名，调用点会产生二义性编译错误）。

        ''' <summary>逐元素自然指数：<c>out(i) = Exp(v(i))</c></summary>
        Public Function SimdExp(v As Double()) As Double()
            Return SimdMath.Exp(v)
        End Function

        ''' <summary>逐元素自然指数（<see cref="Single"/>）</summary>
        Public Function SimdExp(v As Single()) As Single()
            Return SimdMath.Exp(v)
        End Function

        ''' <summary>逐元素自然对数：<c>out(i) = Log(v(i))</c></summary>
        Public Function SimdLog(v As Double()) As Double()
            Return SimdMath.Log(v)
        End Function

        ''' <summary>逐元素自然对数（<see cref="Single"/>）</summary>
        Public Function SimdLog(v As Single()) As Single()
            Return SimdMath.Log(v)
        End Function

        ''' <summary>逐元素任意底对数：<c>out(i) = Log(v(i), base)</c></summary>
        Public Function SimdLog(v As Double(), base As Double) As Double()
            Return SimdMath.Log(v, base)
        End Function

        ''' <summary>逐元素符号函数</summary>
        Public Function SimdSign(v As Double()) As Double()
            Return SimdMath.Sign(v)
        End Function

        ''' <summary>逐元素符号函数（<see cref="Single"/>）</summary>
        Public Function SimdSign(v As Single()) As Single()
            Return SimdMath.Sign(v)
        End Function

        ''' <summary>逐元素向下取整</summary>
        Public Function SimdFloor(v As Double()) As Double()
            Return SimdMath.Floor(v)
        End Function

        ''' <summary>逐元素向下取整（<see cref="Single"/>）</summary>
        Public Function SimdFloor(v As Single()) As Single()
            Return SimdMath.Floor(v)
        End Function

        ''' <summary>逐元素向上取整</summary>
        Public Function SimdCeiling(v As Double()) As Double()
            Return SimdMath.Ceiling(v)
        End Function

        ''' <summary>逐元素向上取整（<see cref="Single"/>）</summary>
        Public Function SimdCeiling(v As Single()) As Single()
            Return SimdMath.Ceiling(v)
        End Function

        ''' <summary>逐元素截断取整</summary>
        Public Function SimdTruncate(v As Double()) As Double()
            Return SimdMath.Truncate(v)
        End Function

        ''' <summary>逐元素截断取整（<see cref="Single"/>）</summary>
        Public Function SimdTruncate(v As Single()) As Single()
            Return SimdMath.Truncate(v)
        End Function

        ''' <summary>逐元素倒数：<c>out(i) = 1 / v(i)</c></summary>
        Public Function SimdReciprocal(v As Single()) As Single()
            Return SimdMath.Reciprocal(v)
        End Function

#End Region

#Region "map / convert"

        ''' <summary>
        ''' 任意一元函数的逐元素映射：<c>out(i) = f(v(i))</c>
        ''' </summary>
        ''' <remarks>
        ''' 这是「逐元素数学函数」的通用兜底入口：运行时没有专门 SIMD 内核的函数
        ''' （三角函数、四舍五入、用户自定义函数等）都可以通过本方法被向量化。
        ''' 实现为标量循环，因为委托调用本身无法被 SIMD 内核吸收。
        ''' </remarks>
        Public Function SimdMap(Of TIn As Structure, TOut As Structure)(v As TIn(),
                                                                         f As Func(Of TIn, TOut)) As TOut()
            If v Is Nothing Then
                Throw New ArgumentNullException(NameOf(v), "the input vector can not be NULL!")
            End If

            Dim len As Integer = v.Length
            If len = 0 Then Return Array.Empty(Of TOut)()

            Dim out As TOut() = New TOut(len - 1) {}
            For i As Integer = 0 To len - 1
                out(i) = f(v(i))
            Next

            Return out
        End Function

        ''' <summary>
        ''' 逐元素元素类型转换。
        ''' </summary>
        ''' <remarks>
        ''' VB 对数组**不存在**逐元素转换（<c>Short()</c> 无法赋给 <c>Integer()</c> 形参），
        ''' 所以当向量化运算的两侧元素类型不同（或结果类型需要提升）时，
        ''' 改写器必须显式插入一次元素类型转换，本方法即为该用途。
        ''' 实现为标量循环（<see cref="Convert.ChangeType(Object, Type)"/> 无法向量化），
        ''' 仅在类型不一致时产生一次额外遍历。
        ''' </remarks>
        Public Function SimdConvert(Of TIn As Structure, TOut As Structure)(v As TIn()) As TOut()
            Dim target As Type = GetType(TOut)

            Return SimdMap(Of TIn, TOut)(
                v,
                Function(x) DirectCast(Convert.ChangeType(x, target), TOut))
        End Function

#End Region

#Region "reduce"

        ''' <summary>
        ''' 向量求和：<c>SUM(v)</c>
        ''' </summary>
        ''' <remarks>
        ''' <see cref="SimdExtensions.SimdSum(Double())"/> / <c>(Single())</c> 走 SIMD 归约内核，
        ''' 这里只为整数类型补齐；语义与 <see cref="Enumerable.Sum(IEnumerable(Of Integer))"/> 一致。
        ''' </remarks>
        Public Function SimdSum(v As Integer()) As Integer
            Return v.Sum()
        End Function

        ''' <summary>向量求和（<see cref="Long"/>）</summary>
        Public Function SimdSum(v As Long()) As Long
            Return v.Sum()
        End Function

        ''' <summary>
        ''' 向量求和（<see cref="Short"/>）
        ''' </summary>
        ''' <remarks>
        ''' <see cref="Enumerable.Sum(IEnumerable(Of Integer))"/> 这类 LINQ 归约把
        ''' <see cref="Byte"/>/<see cref="Short"/> 一律拓宽到 <see cref="Integer"/>，
        ''' 没有 <c>Short</c> 重载，因此这里手写累加，保持「按元素类型累加」的语义。
        ''' </remarks>
        Public Function SimdSum(v As Short()) As Short
            Dim total As Short = 0

            For Each x As Short In v
                total = CShort(total + x)
            Next

            Return total
        End Function

        ''' <summary>
        ''' 向量均值：<c>SUM(v) / N</c>
        ''' </summary>
        ''' <remarks>
        ''' 整数向量的均值恒为 <see cref="Double"/>，语义与
        ''' <see cref="Enumerable.Average(IEnumerable(Of Integer))"/> 一致。
        ''' </remarks>
        Public Function SimdMean(v As Integer()) As Double
            Return v.Average()
        End Function

        ''' <summary>向量均值（<see cref="Long"/>）</summary>
        Public Function SimdMean(v As Long()) As Double
            Return v.Average()
        End Function

        ''' <summary>
        ''' 向量最小值：<c>MIN(v)</c>
        ''' </summary>
        ''' <remarks><see cref="SimdReduce.Min(Double())"/> 已覆盖 <see cref="Double"/>，这里补齐其余类型。</remarks>
        Public Function SimdMin(v As Single()) As Single
            Return v.Min()
        End Function

        ''' <summary>向量最小值（<see cref="Integer"/>）</summary>
        Public Function SimdMin(v As Integer()) As Integer
            Return v.Min()
        End Function

        ''' <summary>向量最小值（<see cref="Long"/>）</summary>
        Public Function SimdMin(v As Long()) As Long
            Return v.Min()
        End Function

        ''' <summary>向量最小值（<see cref="Short"/>）</summary>
        Public Function SimdMin(v As Short()) As Short
            Return v.Min()
        End Function

        ''' <summary>
        ''' 向量最大值：<c>MAX(v)</c>
        ''' </summary>
        ''' <remarks><see cref="SimdExtensions.SimdMax(Double())"/> / <c>(Single())</c> 已存在，这里补齐整数类型。</remarks>
        Public Function SimdMax(v As Integer()) As Integer
            Return v.Max()
        End Function

        ''' <summary>向量最大值（<see cref="Long"/>）</summary>
        Public Function SimdMax(v As Long()) As Long
            Return v.Max()
        End Function

        ''' <summary>向量最大值（<see cref="Short"/>）</summary>
        Public Function SimdMax(v As Short()) As Short
            Return v.Max()
        End Function

        ''' <summary>
        ''' 向量乘积：<c>PRODUCT(v)</c>
        ''' </summary>
        ''' <remarks>空向量返回乘法单位元 <c>1</c>（与 <see cref="Enumerable.Aggregate"/> 的种子语义一致）。</remarks>
        Public Function SimdProduct(v As Double()) As Double
            Return v.Aggregate(1.0, Function(a, b) a * b)
        End Function

        ''' <summary>向量乘积（<see cref="Single"/>）</summary>
        Public Function SimdProduct(v As Single()) As Single
            Return v.Aggregate(1.0F, Function(a, b) a * b)
        End Function

        ''' <summary>向量乘积（<see cref="Integer"/>）</summary>
        Public Function SimdProduct(v As Integer()) As Integer
            Return v.Aggregate(1, Function(a, b) a * b)
        End Function

        ''' <summary>向量乘积（<see cref="Long"/>）</summary>
        Public Function SimdProduct(v As Long()) As Long
            Return v.Aggregate(1L, Function(a, b) a * b)
        End Function

        ''' <summary>向量乘积（<see cref="Short"/>）</summary>
        Public Function SimdProduct(v As Short()) As Short
            Return v.Aggregate(1S, Function(a, b) a * b)
        End Function

        ''' <summary>
        ''' 向量元素个数：<c>COUNT(v)</c>
        ''' </summary>
        ''' <remarks>与 <see cref="Enumerable.Count(Of TSource)(IEnumerable(Of TSource))"/> 的数组快速路径一致，为 <c>O(1)</c>。</remarks>
        Public Function SimdCount(Of T)(v As T()) As Integer
            If v Is Nothing Then
                Return 0
            Else
                Return v.Length
            End If
        End Function

#End Region
    End Module
End Namespace
