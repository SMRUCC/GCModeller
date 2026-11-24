Namespace Bayesian.Statistics

    ''' <summary>
    ''' 连续特征到离散取值的等频分箱(equi-frequency binning)。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' readme 第 5.2 节指出：把连续特征(MI、DCA 分数、界面倾向性打分等)分箱之后，
    ''' 条件概率表可以直接用带 Dirichlet 平滑的最大似然估计得到。本类提供这一分箱步骤，
    ''' 并且保证箱的边界取自实际观测值，从而避免外插。
    ''' </para>
    ''' <para>
    ''' 当观测值之中存在大量并列(ties)的时候，等频分箱无法精确保证每个箱的元素个数相同，
    ''' 此时退化为「按照分位数切分」的近似分箱 —— 这一点与 <c>bnlearn</c> / <c>pgmpy</c>
    ''' 之中的行为一致，因此结果仍然是可复现的。
    ''' </para>
    ''' </remarks>
    Public Class Discretization

        Private ReadOnly _bounds As Double()
        Private ReadOnly _bins As Integer

        ''' <summary>
        ''' 由观测值构造一个等频分箱器
        ''' </summary>
        ''' <param name="values">需要离散化的连续特征</param>
        ''' <param name="bins">箱的个数(即离散化之后的取值个数)，通常取 5 ~ 10</param>
        Public Sub New(values As IEnumerable(Of Double), Optional bins As Integer = 5)
            If values Is Nothing Then
                Throw New ArgumentNullException(NameOf(values))
            End If
            If bins < 2 Then
                Throw New ArgumentOutOfRangeException(NameOf(bins), "the number of bins must be at least 2")
            End If

            Dim sorted As Double() = values.Where(Function(v) Not Double.IsNaN(v)) _
                .OrderBy(Function(v) v) _
                .ToArray()

            If sorted.Length = 0 Then
                Throw New ArgumentException("the values can not be empty", NameOf(values))
            End If

            _bins = bins
            _bounds = New Double(bins - 2) {}

            For k As Integer = 1 To bins - 1
                Dim position As Integer = CInt(Math.Floor(k * sorted.Length / CDbl(bins)))

                If position >= sorted.Length Then
                    position = sorted.Length - 1
                End If

                _bounds(k - 1) = sorted(position)
            Next

            ' 保证边界严格递增，否则并列值会导致空箱
            For k As Integer = 1 To _bounds.Length - 1
                If _bounds(k) <= _bounds(k - 1) Then
                    _bounds(k) = _bounds(k - 1) + Double.Epsilon
                End If
            Next
        End Sub

        ''' <summary>
        ''' 箱的个数
        ''' </summary>
        Public ReadOnly Property BinCount As Integer
            Get
                Return _bins
            End Get
        End Property

        ''' <summary>
        ''' 箱的划分边界(升序，长度为 <c>BinCount - 1</c>)
        ''' </summary>
        Public ReadOnly Property Bounds As Double()
            Get
                Return _bounds
            End Get
        End Property

        ''' <summary>
        ''' 把一个连续值映射到箱的下标
        ''' </summary>
        Public Function Bin(value As Double) As Integer
            Dim index As Integer = 0

            For k As Integer = 0 To _bounds.Length - 1
                If value >= _bounds(k) Then
                    index = k + 1
                Else
                    Exit For
                End If
            Next

            Return index
        End Function

        ''' <summary>
        ''' 批量离散化
        ''' </summary>
        Public Function Apply(values As IEnumerable(Of Double)) As Integer()
            Return values.Select(Function(v) Bin(v)).ToArray()
        End Function

        ''' <summary>
        ''' 把一个二维特征矩阵离散化为数据表，用于离散贝叶斯网络的参数学习
        ''' </summary>
        Public Shared Function BuildDataSet(features As Double()(), bins As Integer) As Integer()()
            If features Is Nothing OrElse features.Length = 0 Then
                Throw New ArgumentException("the feature matrix can not be empty", NameOf(features))
            End If

            Dim columns As Integer = features(0).Length
            Dim discretizers As Discretization() = New Discretization(columns - 1) {}

            For c As Integer = 0 To columns - 1
                discretizers(c) = New Discretization(features.Select(Function(row) row(c)), bins)
            Next

            Dim result As Integer()() = New Integer(features.Length - 1)() {}

            For r As Integer = 0 To features.Length - 1
                Dim row As Integer() = New Integer(columns - 1) {}

                For c As Integer = 0 To columns - 1
                    row(c) = discretizers(c).Bin(features(r)(c))
                Next

                result(r) = row
            Next

            Return result
        End Function
    End Class
End Namespace
