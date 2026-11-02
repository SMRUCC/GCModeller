Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics

Namespace Bayesian.Model

    ''' <summary>
    ''' 残基接触位点的反向诊断：给定「两个蛋白互作」，推断哪些残基对最可能处于界面上。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这是 readme 第 6 节强调的「可解释性」能力：贝叶斯网络既可以回答
    ''' <c>P(I = 1 | e)</c>，也可以反过来回答 <c>P(C_ij = 1 | I = 1, e)</c>。
    ''' 前者是蛋白对级别的互作预测，后者是残基对级别的接触预测。
    ''' </para>
    ''' <para>
    ''' 实现上直接使用似然比 <c>ln R_ij</c> 作为接触的证据强度，并对跨家族的列对做排序；
    ''' 同时给出该列对之上出现频率最高的氨基酸组合，便于与 PDB 复合物结构比对。
    ''' </para>
    ''' </remarks>
    Public Class ResidueContacts

        ''' <summary>
        ''' 一个候选的残基接触对
        ''' </summary>
        Public Class ContactPair

            ''' <summary>
            ''' 家族 A 一侧的比对列下标
            ''' </summary>
            Public Property ColumnA As Integer
            ''' <summary>
            ''' 家族 B 一侧的比对列下标
            ''' </summary>
            Public Property ColumnB As Integer
            ''' <summary>
            ''' 联合计数之中出现频率最高的氨基酸组合，家族 A 一侧
            ''' </summary>
            Public Property ResidueA As Char
            ''' <summary>
            ''' 联合计数之中出现频率最高的氨基酸组合，家族 B 一侧
            ''' </summary>
            Public Property ResidueB As Char
            ''' <summary>
            ''' 似然比 <c>R_ij</c> 的对数值
            ''' </summary>
            Public Property LogR As Double
            ''' <summary>
            ''' 该列对的联合计数总数
            ''' </summary>
            Public Property Support As Integer

            Public Overrides Function ToString() As String
                Return $"A{ColumnA}{ResidueA} -- B{ColumnB}{ResidueB}  logR={LogR:0.###} (n={Support})"
            End Function
        End Class

        Private ReadOnly _model As PpiBayesianModel

        ''' <summary>
        ''' 由贝叶斯模型构造反向诊断器
        ''' </summary>
        Public Sub New(model As PpiBayesianModel)
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If

            _model = model
        End Sub

        ''' <summary>
        ''' 被打分的模型
        ''' </summary>
        Public ReadOnly Property Model As PpiBayesianModel
            Get
                Return _model
            End Get
        End Property

        ''' <summary>
        ''' 返回得分最高的前 <paramref name="k"/> 个跨家族残基对
        ''' </summary>
        ''' <param name="k">返回的残基对个数，-1 表示全部</param>
        Public Function TopContacts(Optional k As Integer = -1) As ContactPair()
            Dim alignment As JointAlignment = _model.Alignment
            Dim counts As ColumnCounts = _model.Counts
            Dim alphabet As ResidueAlphabet = _model.Options.Alphabet
            Dim pairs As New List(Of ContactPair)

            For i As Integer = 0 To alignment.LengthA - 1
                For j As Integer = alignment.LengthA To alignment.Length - 1
                    Dim joint As Dictionary(Of Integer, Integer) = counts.PairCounts(i, j)

                    If joint.Count = 0 Then
                        Continue For
                    End If

                    Dim bestCode As Integer = -1
                    Dim bestCount As Integer = -1
                    Dim total As Integer = 0

                    For Each entry As KeyValuePair(Of Integer, Integer) In joint
                        total += entry.Value

                        If entry.Value > bestCount Then
                            bestCount = entry.Value
                            bestCode = entry.Key
                        End If
                    Next

                    Call pairs.Add(New ContactPair With {
                        .ColumnA = i,
                        .ColumnB = j,
                        .ResidueA = alphabet.Symbol(bestCode \ counts.AlphabetSize),
                        .ResidueB = alphabet.Symbol(bestCode Mod counts.AlphabetSize),
                        .LogR = _model.LogR.LogR(i, j),
                        .Support = total
                    })
                Next
            Next

            Dim ordered As ContactPair() = pairs.OrderByDescending(Function(p) p.LogR).ToArray()

            If k >= 0 AndAlso k < ordered.Length Then
                Return ordered.Take(k).ToArray()
            Else
                Return ordered
            End If
        End Function

        ''' <summary>
        ''' 把残基对级别的证据聚合为蛋白对级别的证据：取 top-K 残基对的 <c>ln R</c> 之和。
        ''' </summary>
        ''' <param name="k">参与聚合的残基对个数</param>
        ''' <remarks>
        ''' readme 第 4 节指出，把残基对分数聚合到蛋白对级别常用「top-K 之和」或者
        ''' 「top-K 均值 + logistic 转换」。本方法实现的是前者，并且在 <see cref="PpiScoring"/>
        ''' 之中已经提供了论文原本的打分公式，两者可以互相校验。
        ''' </remarks>
        Public Function AggregateScore(Optional k As Integer = 20) As Double
            Dim top As ContactPair() = TopContacts(k)
            Dim total As Double = 0.0

            For Each pair As ContactPair In top
                If pair.LogR > 0 Then
                    total += pair.LogR
                End If
            Next

            Return total
        End Function

        ''' <summary>
        ''' 把 top-K 聚合分数通过 logistic 函数转换为 <c>[0, 1]</c> 的概率
        ''' </summary>
        ''' <param name="k">参与聚合的残基对个数</param>
        ''' <param name="midpoint">logistic 的中点</param>
        ''' <param name="slope">logistic 的斜率</param>
        Public Function AggregateProbability(Optional k As Integer = 20,
                                            Optional midpoint As Double = 10.0,
                                            Optional slope As Double = 0.25) As Double
            Dim score As Double = AggregateScore(k)
            Return 1.0 / (1.0 + Math.Exp(-slope * (score - midpoint)))
        End Function
    End Class
End Namespace
