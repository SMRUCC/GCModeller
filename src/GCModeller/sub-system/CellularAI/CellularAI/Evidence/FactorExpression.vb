Namespace Evidence

    ''' <summary>因子的作用轴。</summary>
    Public Enum FactorAxis
        ''' <summary>基因方向（每个训练行 × 每个基因一个权重）。</summary>
        Row = 0
        ''' <summary>扰动方向（每个训练行 × 每个扰动一个权重）。</summary>
        Col = 1
        ''' <summary>整行（每个训练行一个权重）。</summary>
        Mat = 2
    End Enum

    ''' <summary>
    ''' 因子表达式的解析：把 <c>"sef*kd"</c> 这样的算子串拆成有序的因子名，并做合法性校验。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 表达式是<b>模型定义的一部分</b>：因子名的集合决定"哪些证据被用上"，
    ''' 相乘的顺序决定浮点舍入的方向。因此解析结果保留原始顺序，不排序、不去重（重复即报错）。
    ''' </para>
    ''' <para>
    ''' 可用的因子分两类：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description><b>数据内因子</b>：<c>inv:mad:dl</c>（逆方差 + DerSimonian–Laird 随机效应权重），
    ''' 它依赖逐基因的响应方差与合并后的 τ²，因此由池化内核自己计算，不在这里求值；</description></item>
    ''' <item><description><b>细胞系因子</b>：<c>nr</c>（噪声比）、<c>sef</c>（尺度误差）、<c>kd</c>（敲低）、
    ''' <c>sim</c>（相似度）、<c>pair</c>（配对降权）、<c>auxiliary</c>（辅助行加权），
    ''' 由 <see cref="PoolingFactors"/> 预先算好。</description></item>
    ''' </list>
    ''' </remarks>
    Public Class FactorExpression

        ''' <summary>数据内因子（由池化内核计算）。</summary>
        Public Const LocalVarianceFactor As String = "inv:mad:dl"

        Private Shared ReadOnly RowFactors As String() = {LocalVarianceFactor, "nr"}
        Private Shared ReadOnly ColFactors As String() = {"kd", "sef"}
        Private Shared ReadOnly MatFactors As String() = {"auxiliary", "sim", "pair"}

        ''' <summary>某个轴上全部合法的因子名。</summary>
        Public Shared Function Known(axis As FactorAxis) As String()
            Select Case axis
                Case FactorAxis.Row
                    Return DirectCast(RowFactors.Clone(), String())
                Case FactorAxis.Col
                    Return DirectCast(ColFactors.Clone(), String())
                Case Else
                    Return DirectCast(MatFactors.Clone(), String())
            End Select
        End Function

        ''' <summary>
        ''' 解析表达式：<c>"sef*kd"</c> → <c>{"sef","kd"}</c>；<c>"1"</c> 或空串 → 空数组。
        ''' </summary>
        ''' <param name="expr">因子表达式</param>
        ''' <param name="axis">作用轴（决定合法因子集合）</param>
        ''' <exception cref="ArgumentException">出现未知因子或重复因子</exception>
        Public Shared Function Parse(expr As String, axis As FactorAxis) As String()
            If String.IsNullOrEmpty(expr) Then
                Return New String() {}
            End If

            Dim known As New HashSet(Of String)(StringComparer.Ordinal)
            Dim allowed As String() = Known(axis)

            For Each item As String In allowed
                Call known.Add(item)
            Next

            Dim names As New List(Of String)

            For Each token As String In expr.Split("*"c)
                Dim name As String = token.Trim()

                If name.Length = 0 OrElse name = "1" Then
                    Continue For
                End If
                If Not known.Contains(name) Then
                    Throw New ArgumentException(
                        $"{axis} op '{expr}': factor '{name}' is not supported; known: {String.Join(", ", Known(axis))}",
                        NameOf(expr))
                End If
                If names.Contains(name) Then
                    Throw New ArgumentException($"{axis} op '{expr}' names the factor '{name}' twice", NameOf(expr))
                End If

                Call names.Add(name)
            Next

            Return names.ToArray
        End Function

        ''' <summary>表达式里是否包含需要池化内核自己计算的数据内因子。</summary>
        Public Shared Function RequiresLocal(names As String()) As Boolean
            Return names IsNot Nothing AndAlso Array.IndexOf(names, LocalVarianceFactor) >= 0
        End Function

        ''' <summary>取出需要预先计算的细胞系因子名（去掉数据内因子）。</summary>
        Public Shared Function CellLineNames(names As String()) As String()
            If names Is Nothing Then
                Return New String() {}
            End If

            Return names.Where(Function(n) Not String.Equals(n, LocalVarianceFactor, StringComparison.Ordinal)).ToArray
        End Function

    End Class
End Namespace
