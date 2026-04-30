Namespace ContextModel

    ''' <summary>
    ''' 存储计算得出的操纵子预测特征得分
    ''' </summary>
    Public Class FeatureScores

        Public Property upstreamID As String
        Public Property downstreamID As String

        ''' <summary>
        ''' 获取或设置基因间距离
        ''' 论文公式: DI = downstream_gene_start - (upstream_gene_end + 1)，并应用 [-50, 250] 截断
        ''' </summary>
        Public Property IntergenicDistance As Double
        ''' <summary>
        ''' 获取或设置基因邻域保守性得分
        ''' 论文公式: S = -∑ L(gi, gj, Gk)
        ''' </summary>
        Public Property NeighborhoodConservation As Double
        ''' <summary>
        ''' 获取或设置系统发育距离 (此处存储汉明距离)
        ''' 论文公式: DH = ∑di, 当仅单基因存在时 di=1，否则 di=0
        ''' </summary>
        Public Property PhylogeneticDistance As Double

        Public Property PhylogeneticDistanceShannon As Double
        ''' <summary>
        ''' 获取或设置相邻基因对长度比的自然对数
        ''' 论文公式: L = ln(li/lj), j = i + 1
        ''' </summary>
        Public Property LengthRatio As Double
        ''' <summary>
        ''' 获取或设置GO功能相似性得分
        ''' 论文定义: SGO(gi,gj) = max s(Vi,Vj)，即诱导路径中共同术语数的最大值
        ''' </summary>
        Public Property GOSimilarity As Double

        Public Property MetabolicScore As Double

        ''' <summary>
        ''' 获取或设置特定DNA基序在基因间区域的归一化频率字典
        ''' 键为基序名称 (如 "Motif_TTTTT")，值为观测次数与期望次数的比值
        ''' </summary>
        Public Property Motifs As Dictionary(Of String, Double)

        Public Property DistanceGroup As IntergenicDistanceGroup

    End Class
End Namespace