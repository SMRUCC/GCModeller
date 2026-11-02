Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics

Namespace Bayesian.Model

    ''' <summary>
    ''' 蛋白质互作贝叶斯模型的统一配置。
    ''' </summary>
    ''' <remarks>
    ''' 这里集中了所有会影响到推断结果或者计算代价的开关，避免它们散落在各个类之中。
    ''' 默认值是论文的推荐设置：Jeffreys 先验(<c>λ' = 1/2</c>)、空位列阈值 75%、
    ''' 使用最大生成树近似(而不是行列式)、判定阈值 <c>X ≥ 1</c>。
    ''' </remarks>
    Public Class PpiModelOptions

        Private _alphabet As ResidueAlphabet = Nothing
        Private _priorLambda As Double = 0.5
        Private _maxGapFraction As Double = 0.75
        Private _useMatrixTree As Boolean = False
        Private _includeWithinBlockPairs As Boolean = True
        Private _randomSeed As Integer = 0
        Private _interactionThreshold As Double = 1.0
        Private _topResiduePairs As Integer = 20

        ''' <summary>
        ''' 残基字母表，为 <c>Nothing</c> 的时候使用论文的 21 符号字母表
        ''' </summary>
        Public Property Alphabet As ResidueAlphabet
            Get
                Return If(_alphabet, ResidueAlphabet.Standard21)
            End Get
            Set(value As ResidueAlphabet)
                _alphabet = value
            End Set
        End Property

        ''' <summary>
        ''' 成对联合分布的 Dirichlet 先验参数 <c>λ'</c>，默认 0.5(Jeffreys 先验)
        ''' </summary>
        Public Property PriorLambda As Double
            Get
                Return _priorLambda
            End Get
            Set(value As Double)
                If value <= 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), "the Dirichlet prior must be positive")
                End If

                _priorLambda = value
            End Set
        End Property

        ''' <summary>
        ''' 列过滤的空位比例上限，超过该比例的列会在预处理阶段被删除
        ''' </summary>
        Public Property MaxGapFraction As Double
            Get
                Return _maxGapFraction
            End Get
            Set(value As Double)
                _maxGapFraction = value
            End Set
        End Property

        ''' <summary>
        ''' 是否使用 Matrix-Tree 行列式对全部生成树求和。
        ''' </summary>
        ''' <remarks>
        ''' 默认关闭：行列式的复杂度是 <c>O(L³)</c>，在数百列的规模上代价很高，
        ''' 而且论文本身也指出并行多类时行列式计算存在数值不稳定的问题。
        ''' 打开之后 <c>P(D|a)</c> 会使用精确的边缘化结果，适合小规模数据与数值对照测试。
        ''' </remarks>
        Public Property UseMatrixTree As Boolean
            Get
                Return _useMatrixTree
            End Get
            Set(value As Boolean)
                _useMatrixTree = value
            End Set
        End Property

        ''' <summary>
        ''' 是否统计同家族内部的列对。
        ''' </summary>
        ''' <remarks>
        ''' 只关心「分配」的相对优劣时可以关闭，这样可以只统计 <c>L_A × L_B</c> 个跨家族列对；
        ''' 但要得到论文意义上的三棵依赖树(<c>T*</c>、<c>T_k</c>、<c>T_r</c>)以及正确的打分，
        ''' 必须打开。
        ''' </remarks>
        Public Property IncludeWithinBlockPairs As Boolean
            Get
                Return _includeWithinBlockPairs
            End Get
            Set(value As Boolean)
                _includeWithinBlockPairs = value
            End Set
        End Property

        ''' <summary>
        ''' 随机数种子，固定之后采样结果完全可复现
        ''' </summary>
        Public Property RandomSeed As Integer
            Get
                Return _randomSeed
            End Get
            Set(value As Integer)
                _randomSeed = value
            End Set
        End Property

        ''' <summary>
        ''' 判定「存在互作」的打分阈值，论文使用 <c>X ≥ 1</c>
        ''' </summary>
        Public Property InteractionThreshold As Double
            Get
                Return _interactionThreshold
            End Get
            Set(value As Double)
                _interactionThreshold = value
            End Set
        End Property

        ''' <summary>
        ''' 反向诊断之中输出的最高分残基对个数
        ''' </summary>
        Public Property TopResiduePairs As Integer
            Get
                Return _topResiduePairs
            End Get
            Set(value As Integer)
                _topResiduePairs = value
            End Set
        End Property

        ''' <summary>
        ''' 按当前配置创建一个 Dirichlet-多项模型
        ''' </summary>
        Public Function CreatePrior(alphabetSize As Integer) As DirichletMultinomial
            Return New DirichletMultinomial(alphabetSize, _priorLambda)
        End Function

        ''' <summary>
        ''' 复制一份配置
        ''' </summary>
        Public Function Clone() As PpiModelOptions
            Return New PpiModelOptions With {
                .Alphabet = _alphabet,
                .PriorLambda = _priorLambda,
                .MaxGapFraction = _maxGapFraction,
                .UseMatrixTree = _useMatrixTree,
                .IncludeWithinBlockPairs = _includeWithinBlockPairs,
                .RandomSeed = _randomSeed,
                .InteractionThreshold = _interactionThreshold,
                .TopResiduePairs = _topResiduePairs
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"lambda'={_priorLambda}, useMatrixTree={_useMatrixTree}, threshold={_interactionThreshold}"
        End Function
    End Class
End Namespace
