Namespace RBase.Vectors

    Public Class Vector : Inherits GenericVector(Of Double)
        Implements IEnumerable(Of Double)

        Default Public Overloads Property ElementWhere(Conditions As BooleanVector) As Vector
            Get
                Return New Vector(Me.ElementWhere(Conditions))
            End Get
            Set(value As Vector)
                Me.ElementWhere(Conditions) = value.Elements
            End Set
        End Property

        Public Shared ReadOnly Property NAN As Vector
            Get
                Return New Vector({Double.NaN})
            End Get
        End Property

        Public Shared ReadOnly Property Inf As Vector
            Get
                Return New Vector({Double.PositiveInfinity})
            End Get
        End Property

        Public Shared ReadOnly Property Zero As Vector
            Get
                Return New Vector({0})
            End Get
        End Property

        Public Sub New(m As Integer)
            Elements = New Double(m - 1) {}
        End Sub

        Protected Sub New()
        End Sub

        Public Sub New(data As Generic.IEnumerable(Of Double))
            Elements = data.ToArray
        End Sub

        Public Shared Widening Operator CType(data As Double()) As Vector
            Return New Vector With {.Elements = data}
        End Operator

        Public Shared Narrowing Operator CType(vector As Vector) As Integer
            Return vector.Elements(0)
        End Operator

        ''' <summary>
        ''' 两个向量加法算符重载，分量分别相加
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator +(v1 As Vector, v2 As Vector) As Vector
            Dim N0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            Dim v3 As New Vector(N0)

            Dim j As Integer
            For j = 0 To N0 - 1
                v3.Elements(j) = v1.Elements(j) + v2.Elements(j)
            Next
            Return v3
        End Operator

        Public Shared Operator >(x As Vector, n As Double) As BooleanVector
            Return New BooleanVector((From d As Double In x.Elements Select d > n).ToArray)
        End Operator

        Public Shared Operator <(x As Vector, n As Double) As BooleanVector
            Return New BooleanVector((From d As Double In x.Elements Select d < n).ToArray)
        End Operator

        Public Shared Operator >=(x As Vector, n As Double) As BooleanVector
            Return New BooleanVector((From d As Double In x Select d >= n).ToArray)
        End Operator

        Public Shared Operator <=(x As Vector, n As Double) As BooleanVector
            Return New BooleanVector((From d As Double In x Select d <= n).ToArray)
        End Operator

        ''' <summary>
        ''' 向量减法算符重载，分量分别想减
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator -(v1 As Vector, v2 As Vector) As Vector
            Dim N0 As Integer
            '获取变量维数
            N0 = v1.[Dim]

            Dim v3 As New Vector(N0)

            Dim j As Integer
            For j = 0 To N0 - 1
                v3.Elements(j) = v1.Elements(j) - v2.Elements(j)
            Next
            Return v3
        End Operator

        ''' <summary>
        ''' 向量乘法算符重载，分量分别相乘，相当于MATLAB中的  .*算符
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator *(v1 As Vector, v2 As Vector) As Vector

            Dim N0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            Dim v3 As New Vector(N0)

            Dim j As Integer
            For j = 0 To N0 - 1
                v3.Elements(j) = v1.Elements(j) * v2.Elements(j)
            Next
            Return v3
        End Operator

        ''' <summary>
        ''' 向量除法算符重载，分量分别相除，相当于MATLAB中的   ./算符
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator /(v1 As Vector, v2 As Vector) As Vector
            Dim N0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            Dim v3 As New Vector(N0)

            Dim j As Integer
            For j = 0 To N0 - 1
                v3.Elements(j) = v1.Elements(j) / v2.Elements(j)
            Next
            Return v3
        End Operator

        ''' <summary>
        ''' 向量减加实数，各分量分别加实数
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="a"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator +(v1 As Vector, a As Double) As Vector
            '向量数加算符重载
            Dim N0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            Dim v2 As New Vector(N0)

            Dim j As Integer
            For j = 0 To N0 - 1
                v2.Elements(j) = v1.Elements(j) + a
            Next
            Return v2
        End Operator

        ''' <summary>
        ''' 向量减实数，各分量分别减实数
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="a"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator -(v1 As Vector, a As Double) As Vector
            '向量数加算符重载
            Dim N0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            Dim v2 As New Vector(N0)

            Dim j As Integer
            For j = 0 To N0 - 1
                v2.Elements(j) = v1.Elements(j) - a
            Next
            Return v2
        End Operator

        ''' <summary>
        ''' 向量 数乘，各分量分别乘以实数
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="a"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator *(v1 As Vector, a As Double) As Vector
            Dim N0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            Dim v2 As New Vector(N0)

            Dim j As Integer
            For j = 0 To N0 - 1
                v2.Elements(j) = v1.Elements(j) * a
            Next
            Return v2
        End Operator

        ''' <summary>
        ''' 向量 数除，各分量分别除以实数
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="a"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator /(v1 As Vector, a As Double) As Vector
            Dim N0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            Dim v2 As New Vector(N0)

            Dim j As Integer
            For j = 0 To N0 - 1
                v2.Elements(j) = v1.Elements(j) / a
            Next
            Return v2
        End Operator

        Public Shared Operator /(n As Double, x As Vector) As Vector
            Return New Vector((From d As Double In x.Elements Select n / d).ToArray)
        End Operator

        ''' <summary>
        ''' 实数加向量
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="v1"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator +(a As Double, v1 As Vector) As Vector
            '向量数加算符重载
            Dim N0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            Dim v2 As New Vector(N0)

            Dim j As Integer
            For j = 0 To N0 - 1
                v2.Elements(j) = v1.Elements(j) + a
            Next
            Return v2
        End Operator

        ''' <summary>
        ''' 实数减向量
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="v1"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator -(a As Double, v1 As Vector) As Vector
            '向量数加算符重载
            Dim N0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            Dim v2 As New Vector(N0)

            Dim j As Integer
            For j = 0 To N0 - 1
                v2.Elements(j) = v1.Elements(j) - a
            Next
            Return v2
        End Operator

        ''' <summary>
        ''' 向量 数乘
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="v1"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator *(a As Double, v1 As Vector) As Vector
            Dim N0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            Dim v2 As New Vector(N0)

            Dim j As Integer
            For j = 0 To N0 - 1
                v2.Elements(j) = v1.Elements(j) * a
            Next
            Return v2
        End Operator

        Public Shared Operator Mod(x As Vector, y As Vector) As Vector
            Return New Vector((From i As Integer In x Select x.Elements(i) Mod y.Elements(i)).ToArray)
        End Operator

        Public Shared Operator Mod(n As Double, x As Vector) As Vector
            Return New Vector((From d As Double In x.Elements Select n Mod d).ToArray)
        End Operator

        ''' <summary>
        ''' 向量内积
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <param name="v2"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator Or(v1 As Vector, v2 As Vector) As Double
            Dim N0 As Integer, M0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            M0 = v2.[Dim]

            If N0 <> M0 Then
                System.Console.WriteLine("Inner vector dimensions must agree！")
            End If
            '如果向量维数不匹配，给出告警信息

            Dim sum As Double
            sum = 0.0

            Dim j As Integer
            For j = 0 To N0 - 1
                sum = sum + v1.Elements(j) * v2.Elements(j)
            Next
            Return sum
        End Operator

        ' ''' <summary>
        ' ''' 向量外积（相当于列向量，乘以横向量）
        ' ''' </summary>
        ' ''' <param name="v1"></param>
        ' ''' <param name="v2"></param>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public Shared Operator Xor(v1 As Vector, v2 As Vector) As MATRIX
        '    Dim N0 As Integer, M0 As Integer

        '    '获取变量维数
        '    N0 = v1.[dim]

        '    M0 = v2.[dim]

        '    If N0 <> M0 Then
        '        System.Console.WriteLine("Inner vector dimensions must agree！")
        '    End If
        '    '如果向量维数不匹配，给出告警信息

        '    Dim vvmat As New MATRIX(N0, N0)

        '    For i As Integer = 0 To N0 - 1
        '        For j As Integer = 0 To N0 - 1
        '            vvmat(i, j) = v1(i) * v2(j)
        '        Next
        '    Next

        '    '返回外积矩阵

        '    Return vvmat
        'End Operator

        ''' <summary>
        ''' 向量模的平方
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator Not(v1 As Vector) As Double
            Dim N0 As Integer

            '获取变量维数
            N0 = v1.[Dim]

            Dim sum As Double
            sum = 0.0

            Dim j As Integer
            For j = 0 To N0 - 1
                sum = sum + v1.Elements(j) * v1.Elements(j)
            Next
            Return sum
        End Operator

        ''' <summary>
        ''' 负向量 
        ''' </summary>
        ''' <param name="v1"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator -(v1 As Vector) As Vector
            Dim N0 As Integer = v1.[Dim]

            Dim v2 As New Vector(N0)

            For i As Integer = 0 To N0 - 1
                v2.Elements(i) = -v1.Elements(i)
            Next

            Return v2
        End Operator

        Public Shared Operator <>(x As Vector, y As Vector) As BooleanVector
            Dim LQuery = (From i As Integer In x.Elements.Sequence Select x.Elements(i) <> y.Elements(i)).ToArray
            Return New BooleanVector(LQuery)
        End Operator

        Public Shared Operator =(x As Vector, y As Vector) As BooleanVector
            Dim LQuery = (From i As Integer In x.Elements.Sequence Select x.Elements(i) = y.Elements(i)).ToArray
            Return New BooleanVector(LQuery)
        End Operator

        Public Shared Operator =(x As Vector, n As Integer) As BooleanVector
            Return New BooleanVector((From d As Double In x.Elements Select d = n).ToArray)
        End Operator

        Public Shared Operator <>(x As Vector, n As Integer) As BooleanVector
            Return New BooleanVector((From d As Double In x.Elements Select d <> n).ToArray)
        End Operator

        Public Shared Operator ^(v As Vector, n As Integer) As Vector
            Return New Vector With {.Elements = (From d As Double In v.Elements Select d ^ n).ToArray}
        End Operator
    End Class
End Namespace