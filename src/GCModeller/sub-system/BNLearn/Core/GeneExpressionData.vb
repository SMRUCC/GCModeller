' ============================================================
' GeneExpressionData.vb - 基因表达矩阵数据结构
' ============================================================
' 存储时间序列基因表达矩阵，支持：
'   - 多时间点样本
'   - 基因名映射
'   - 数据标准化
'   - 时间切片提取
' ============================================================

Namespace Core

    ''' <summary>
    ''' 基因表达矩阵
    ''' 行 = 基因，列 = 样本（可能包含多个时间点）
    ''' </summary>
    Public Class GeneExpressionData

        ''' <summary>基因名称列表</summary>
        Public Property GeneNames As String()

        ''' <summary>样本名称列表</summary>
        Public Property SampleNames As String()

        ''' <summary>表达矩阵 [gene, sample]，基因×样本</summary>
        Public Property Matrix As Double(,)

        ''' <summary>时间点标签（每个样本对应的时间点）</summary>
        Public Property TimePoints As Double()

        ''' <summary>唯一时间点列表（排序后）</summary>
        Public ReadOnly Property UniqueTimePoints As Double()
            Get
                If _uniqueTimes Is Nothing Then
                    Dim set_ As New HashSet(Of Double)()
                    For Each t In TimePoints
                        set_.Add(t)
                    Next
                    Dim arr As Double() = New Double(set_.Count - 1) {}
                    set_.CopyTo(arr)
                    Array.Sort(arr)
                    _uniqueTimes = arr
                End If
                Return _uniqueTimes
            End Get
        End Property
        Private _uniqueTimes As Double() = Nothing

        ''' <summary>基因数量</summary>
        Public ReadOnly Property NGene As Integer
            Get
                Return GeneNames.Length
            End Get
        End Property

        ''' <summary>样本数量</summary>
        Public ReadOnly Property NSample As Integer
            Get
                Return SampleNames.Length
            End Get
        End Property

        ' ==================== 数据访问 ====================

        ''' <summary>获取指定基因的表达向量</summary>
        Public Function GetGeneExpression(geneIndex As Integer) As Double()
            Dim result As Double() = New Double(NSample - 1) {}
            For j = 0 To NSample - 1
                result(j) = Matrix(geneIndex, j)
            Next
            Return result
        End Function

        ''' <summary>获取指定样本的所有基因表达值</summary>
        Public Function GetSample(sampleIndex As Integer) As Double()
            Dim result As Double() = New Double(NGene - 1) {}
            For i = 0 To NGene - 1
                result(i) = Matrix(i, sampleIndex)
            Next
            Return result
        End Function

        ''' <summary>获取指定时间点的所有样本索引</summary>
        Public Function GetSampleIndicesAtTime(time As Double) As Integer()
            Dim indices As New List(Of Integer)()
            For j = 0 To NSample - 1
                If Math.Abs(TimePoints(j) - time) < 1e-10 Then
                    indices.Add(j)
                End If
            Next
            Return indices.ToArray()
        End Function

        ''' <summary>获取指定时间点的子矩阵</summary>
        Public Function GetSubMatrixAtTime(time As Double) As Double(,)
            Dim indices As Integer() = GetSampleIndicesAtTime(time)
            If indices.Length = 0 Then Return Nothing
            Dim sub_ As Double(,) = New Double(NGene - 1, indices.Length - 1) {}
            For i = 0 To NGene - 1
                For k = 0 To indices.Length - 1
                    sub_(i, k) = Matrix(i, indices(k))
                Next
            Next
            Return sub_
        End Function

        ' ==================== 数据预处理 ====================

        ''' <summary>Z-score 标准化（每基因减均值除标准差）</summary>
        Public Function Standardize() As GeneExpressionData
            Dim result As GeneExpressionData = CType(Me.MemberwiseClone(), GeneExpressionData)
            result.Matrix = CType(Matrix.Clone(), Double(,))
            result._uniqueTimes = Nothing

            For i = 0 To NGene - 1
                ' 计算均值
                Dim sum As Double = 0
                For j = 0 To NSample - 1
                    sum += result.Matrix(i, j)
                Next
                Dim mean As Double = sum / NSample

                ' 计算标准差
                Dim ss As Double = 0
                For j = 0 To NSample - 1
                    ss += (result.Matrix(i, j) - mean) ^ 2
                Next
                Dim sd As Double = Math.Sqrt(ss / (NSample - 1))
                If sd < 1e-15 Then sd = 1.0

                ' 标准化
                For j = 0 To NSample - 1
                    result.Matrix(i, j) = (result.Matrix(i, j) - mean) / sd
                Next
            Next

            Return result
        End Function

        ''' <summary>分位数归一化</summary>
        Public Function QuantileNormalize() As GeneExpressionData
            Dim result As GeneExpressionData = CType(Me.MemberwiseClone(), GeneExpressionData)
            result.Matrix = CType(Matrix.Clone(), Double(,))
            result._uniqueTimes = Nothing

            Dim nS As Integer = NSample
            Dim nG As Integer = NGene

            ' 对每列排序，计算排名均值，再替换回原位
            For j = 0 To nS - 1
                ' 获取列值和原始索引
                Dim colData As New List(Of (Value As Double, OrigIdx As Integer))()
                For i = 0 To nG - 1
                    colData.Add((result.Matrix(i, j), i))
                Next

                ' 按值排序
                colData.Sort(Function(a, b) a.Value.CompareTo(b.Value))

                ' 计算每个排名在所有样本中的均值
                Dim rankMeans As Double() = New Double(nG - 1) {}
                For rank = 0 To nG - 1
                    Dim rankSum As Double = 0
                    For jj = 0 To nS - 1
                        Dim sortedCol As New List(Of Double)()
                        For ii = 0 To nG - 1
                            sortedCol.Add(Matrix(ii, jj))
                        Next
                        sortedCol.Sort()
                        rankSum += sortedCol(rank)
                    Next
                    rankMeans(rank) = rankSum / nS
                Next

                ' 用排名均值替换
                For rank = 0 To nG - 1
                    result.Matrix(colData(rank).OrigIdx, j) = rankMeans(rank)
                Next
            Next

            Return result
        End Function

        ''' <summary>获取基因索引</summary>
        Public Function GetGeneIndex(geneName As String) As Integer
            For i = 0 To GeneNames.Length - 1
                If String.Equals(GeneNames(i), geneName, StringComparison.OrdinalIgnoreCase) Then
                    Return i
                End If
            Next
            Return -1
        End Function

    End Class

End Namespace
