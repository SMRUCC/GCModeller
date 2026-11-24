Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Tree

Namespace Bayesian.Model

    ''' <summary>
    ''' 论文第 6 节给出的静态打分 <c>X(S_k, S_r)</c>。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 打分公式为
    ''' </para>
    ''' <code>
    ''' X(S_k, S_r) = ln [ P(S_k, S_r | T*) / ( P(S_k | T_k) · P(S_r | T_r) ) ]
    ''' </code>
    ''' <para>
    ''' 展开成逐边求和的形式：
    ''' </para>
    ''' <code>
    ''' X(S_k, S_r) = Σ_(i,j)∈T* ln [ p^ij_(αβ) / ( p^i_α · p^j_β ) ]
    ''' </code>
    ''' <para>
    ''' 其中 <c>p^ij</c> 与 <c>p^i</c> 是由计数加伪计数平滑之后的相对频率：
    ''' <c>p^ij_(αβ) = (n_ij(αβ) + λ') / (n + K²λ')</c>、<c>p^i_α = (n_i(α) + λ) / (n + Kλ)</c>。
    ''' </para>
    ''' <para>
    ''' 判定规则：<b><c>X ≥ 1</c> 判为互作</b>。论文指出这个阈值相当保守 ——
    ''' 在 cognate–cognate 预测之中大约一半是真正的 cognate 对。
    ''' </para>
    ''' </remarks>
    Public Class PpiScoring

        ''' <summary>
        ''' 单条边对总打分的贡献
        ''' </summary>
        Public Class EdgeContribution

            ''' <summary>
            ''' 家族 A 的一侧列下标
            ''' </summary>
            Public Property ColumnA As Integer
            ''' <summary>
            ''' 家族 B 的一侧列下标
            ''' </summary>
            Public Property ColumnB As Integer
            ''' <summary>
            ''' 家族 A 一侧的残基
            ''' </summary>
            Public Property ResidueA As Char
            ''' <summary>
            ''' 家族 B 一侧的残基
            ''' </summary>
            Public Property ResidueB As Char
            ''' <summary>
            ''' 逐边对数比值 <c>ln[p^ij / (p^i p^j)]</c>
            ''' </summary>
            Public Property LogRatio As Double

            Public Overrides Function ToString() As String
                Return $"{ColumnA}:{ResidueA} -- {ColumnB}:{ResidueB} => {LogRatio:0.###}"
            End Function
        End Class

        ''' <summary>
        ''' 一次打分的详细结果，用于可解释性输出
        ''' </summary>
        Public Class ScoreDetail

            ''' <summary>
            ''' 总分 <c>X</c>
            ''' </summary>
            Public Property Score As Double
            ''' <summary>
            ''' 逐边贡献，按贡献从大到小排列
            ''' </summary>
            Public Property Contributions As EdgeContribution()
            ''' <summary>
            ''' 按照当前阈值得到的判定结果
            ''' </summary>
            Public Property IsInteraction As Boolean

            Public Overrides Function ToString() As String
                Return $"X={Score:0.###} (interaction={IsInteraction})"
            End Function
        End Class

        Private ReadOnly _model As PpiBayesianModel
        Private ReadOnly _prior As DirichletMultinomial

        ''' <summary>
        ''' 由贝叶斯模型构造打分器
        ''' </summary>
        Public Sub New(model As PpiBayesianModel)
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If

            _model = model
            _prior = model.LogR.Prior
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
        ''' 互作判定阈值
        ''' </summary>
        Public ReadOnly Property Threshold As Double
            Get
                Return _model.Options.InteractionThreshold
            End Get
        End Property

        ''' <summary>
        ''' 按照残基编码打分
        ''' </summary>
        ''' <param name="codesA">家族 A 一侧的残基编码，长度等于 <c>L_A</c></param>
        ''' <param name="codesB">家族 B 一侧的残基编码，长度等于 <c>L_B</c></param>
        Public Function Score(codesA As Integer(), codesB As Integer()) As Double
            Return ScoreDetailed(codesA, codesB).Score
        End Function

        ''' <summary>
        ''' 按照序列文本打分
        ''' </summary>
        Public Function Score(sequenceA As String, sequenceB As String) As Double
            Dim alphabet As ResidueAlphabet = _model.Options.Alphabet
            Return Score(alphabet.Encode(sequenceA), alphabet.Encode(sequenceB))
        End Function

        ''' <summary>
        ''' 打分并返回逐边贡献
        ''' </summary>
        Public Function ScoreDetailed(codesA As Integer(), codesB As Integer()) As ScoreDetail
            Dim alignment As Alignment.JointAlignment = _model.Alignment
            Dim tree As DependencyTree = _model.FullTree
            Dim alphabet As ResidueAlphabet = _model.Options.Alphabet
            Dim contributions As New List(Of EdgeContribution)
            Dim total As Double = 0.0

            DebugAudit.Require(codesA IsNot Nothing AndAlso codesA.Length = alignment.LengthA,
                               $"the family A sequence must contain {alignment.LengthA} residues")
            DebugAudit.Require(codesB IsNot Nothing AndAlso codesB.Length = alignment.LengthB,
                               $"the family B sequence must contain {alignment.LengthB} residues")

            For Each edge As DependencyTree.Edge In tree.Edges
                Dim columnA As Integer
                Dim columnB As Integer
                Dim residueA As Integer
                Dim residueB As Integer

                If alignment.IsBlockA(edge.I) AndAlso Not alignment.IsBlockA(edge.J) Then
                    columnA = edge.I
                    columnB = edge.J
                    residueA = codesA(edge.I)
                    residueB = codesB(edge.J - alignment.LengthA)
                ElseIf alignment.IsBlockA(edge.J) AndAlso Not alignment.IsBlockA(edge.I) Then
                    columnA = edge.J
                    columnB = edge.I
                    residueA = codesA(edge.J)
                    residueB = codesB(edge.I - alignment.LengthA)
                ElseIf alignment.IsBlockA(edge.I) Then
                    columnA = edge.I
                    columnB = edge.J
                    residueA = codesA(edge.I)
                    residueB = codesA(edge.J)
                Else
                    columnA = edge.I
                    columnB = edge.J
                    residueA = codesB(edge.I - alignment.LengthA)
                    residueB = codesB(edge.J - alignment.LengthA)
                End If

                Dim ratio As Double = LogRatio(columnA, columnB, residueA, residueB)

                Call contributions.Add(New EdgeContribution With {
                    .ColumnA = columnA,
                    .ColumnB = columnB,
                    .ResidueA = alphabet.Symbol(residueA),
                    .ResidueB = alphabet.Symbol(residueB),
                    .LogRatio = ratio
                })

                total += ratio
            Next

            Dim ordered As EdgeContribution() = contributions.OrderByDescending(Function(c) c.LogRatio).ToArray()

            Return New ScoreDetail With {
                .Score = total,
                .Contributions = ordered,
                .IsInteraction = total >= Threshold
            }
        End Function

        ''' <summary>
        ''' 判定打分是否达到互作阈值
        ''' </summary>
        Public Function IsInteraction(score As Double) As Boolean
            Return score >= Threshold
        End Function

        ''' <summary>
        ''' 逐边对数比值 <c>ln[p^ij_(αβ) / (p^i_α · p^j_β)]</c>
        ''' </summary>
        Public Function LogRatio(columnA As Integer, columnB As Integer, residueA As Integer, residueB As Integer) As Double
            Return LogPij(columnA, columnB, residueA, residueB) -
                LogPi(columnA, residueA) -
                LogPj(columnB, residueB)
        End Function

        ''' <summary>
        ''' 平滑之后的联合相对频率 <c>ln p^ij_(αβ)</c>
        ''' </summary>
        Public Function LogPij(columnA As Integer, columnB As Integer, residueA As Integer, residueB As Integer) As Double
            Dim counts As Alignment.ColumnCounts = _model.Counts
            Dim k As Integer = counts.AlphabetSize
            Dim joint As Dictionary(Of Integer, Integer) = counts.PairCounts(columnA, columnB)
            Dim total As Double = counts.PairTotal(columnA, columnB)
            Dim lambda As Double = _prior.Lambda
            Dim n As Integer = 0

            Call joint.TryGetValue(residueA * k + residueB, n)

            Return Math.Log((n + lambda) / (total + lambda * k * k))
        End Function

        ''' <summary>
        ''' 平滑之后的边缘相对频率 <c>ln p^i_α</c>
        ''' </summary>
        Public Function LogPi(column As Integer, residue As Integer) As Double
            Dim counts As Alignment.ColumnCounts = _model.Counts
            Dim k As Integer = counts.AlphabetSize
            Dim lambda As Double = _prior.LambdaSingle
            Dim total As Double = counts.SingleTotal(column)
            Dim n As Integer = counts.SingleCounts(column)(residue)

            Return Math.Log((n + lambda) / (total + lambda * k))
        End Function

        Private Function LogPj(column As Integer, residue As Integer) As Double
            Return LogPi(column, residue)
        End Function
    End Class
End Namespace
