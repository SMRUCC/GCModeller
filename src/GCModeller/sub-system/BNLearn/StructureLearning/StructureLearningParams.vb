Namespace StructureLearning

    ''' <summary>
    ''' 结构学习参数
    ''' </summary>
    Public Class StructureLearningParams

        ''' <summary>算法类型</summary>
        Public Property Algorithm As StructureAlgorithm = StructureAlgorithm.MMHC

        ''' <summary>显著性水平 alpha（用于独立性检验）</summary>
        Public Property Alpha As Double = 0.05

        ''' <summary>最大父节点数</summary>
        Public Property MaxParents As Integer = 5

        ''' <summary>Tabu 搜索的禁忌表长度</summary>
        Public Property TabuLength As Integer = 20

        ''' <summary>最大迭代次数</summary>
        Public Property MaxIterations As Integer = 500

        ''' <summary>BIC 惩罚系数（>1 更稀疏）</summary>
        Public Property BICPenalty As Double = 1.0

        ''' <summary>是否使用白名单先验</summary>
        Public Property UseWhitelist As Boolean = True

        ''' <summary>是否使用黑名单</summary>
        Public Property UseBlacklist As Boolean = True

        ''' <summary>随机种子</summary>
        Public Property RandomSeed As Integer = 42

    End Class
End Namespace