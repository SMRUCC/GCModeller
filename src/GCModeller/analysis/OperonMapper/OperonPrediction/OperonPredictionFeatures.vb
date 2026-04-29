#Region "Microsoft.VisualBasic::d17f01ddafe1b58f291fed5707766f53, analysis\OperonMapper\FeatureScores.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 252
'    Code Lines: 135 (53.57%)
' Comment Lines: 84 (33.33%)
'    - Xml Docs: 73.81%
' 
'   Blank Lines: 33 (13.10%)
'     File Size: 12.76 KB


'     Class OperonPredictionFeatures
' 
'         Function: CalculateAllFeatures, CalculateGOSimilarity, CalculateIntergenicDistance, CalculateLengthRatio, CalculateMotifFrequency
'                   CalculateNeighborhoodConservation, CalculatePhylogeneticDistanceHamming
' 
'         Sub: AddAncestorTerms
'         Structure GeneInfo
' 
' 
' 
'         Structure GenomeInfo
' 
' 
' 
' 
' 
' 
' /********************************************************************************/

#End Region


Namespace ContextModel

    ''' <summary>
    ''' 提取并计算操纵子预测所需的各种特征得分
    ''' 
    ''' To evaluate the contribution of selected features in operon prediction, we have calculated 
    ''' the numerical values of the features, And then used these values individually And in combination 
    ''' to train a classifier. The features used in our study are
    ''' 
    ''' + (i)   the intergenic distance, 
    ''' + (ii)  the conserved gene neighborhood, 
    ''' + (iii) distances between adjacent genes' phylogenetic profiles, 
    ''' + (iv)  the ratio between the lengths of two adjacent genes And 
    ''' + (v)   frequencies Of specific DNA motifs in the intergenic regions.
    ''' </summary>
    ''' <remarks>
    ''' Operon prediction using both genome-specific and general genomic information 
    ''' 
    ''' https://academic.oup.com/nar/article/35/1/288/2401876
    ''' </remarks>
    ''' 
    Public Class OperonPredictionFeatures

        ''' <summary>
        ''' 参考基因组列表
        ''' </summary>
        ReadOnly referenceGenomes As GenomeInfo()
        ''' <summary>
        ''' GO层次结构字典：GO术语 -> 父术语列表
        ''' </summary>
        ReadOnly goHierarchy As Dictionary(Of String, List(Of String))
        ReadOnly motifs As String() = {"TTT", "ATA", "TTTT", "TATA", "TTTTT", "TTTTC"}
        ''' <summary>
        ''' 基因存在概率字典
        ''' </summary>
        ''' <remarks>
        ''' 基因存在概率字典：基因ID -> (门 -> 概率pik)
        ''' </remarks>
        ReadOnly phylumProbabilities As Dictionary(Of String, Dictionary(Of String, Double))
        ''' <summary>
        ''' 核苷酸频率字典
        ''' </summary>
        ''' <remarks>
        ''' 核苷酸频率字典 (A, T, C, G -> 频率)
        ''' </remarks>
        ReadOnly nucleotideFrequencies As Dictionary(Of Char, Double)

        Sub New(referenceGenomes As IEnumerable(Of GenomeInfo), goHierarchy As Dictionary(Of String, List(Of String)))
            Me.referenceGenomes = referenceGenomes.ToArray
            Me.goHierarchy = goHierarchy
        End Sub

        ''' <summary>
        ''' 计算所有特征的综合函数
        ''' 包含前置校验(同链验证)及论文中所列全部6类特征的提取
        ''' </summary>
        ''' <param name="upstreamGene">上游基因信息</param>
        ''' <param name="downstreamGene">下游基因信息</param>
        ''' <param name="intergenicSequence">基因间序列 (下游基因上游100nt)</param>
        ''' <returns>包含所有得分的 FeatureScores 对象；若基因不在同一链上则返回 Nothing</returns>
        Public Function CalculateAllFeatures(upstreamGene As GeneInfo, downstreamGene As GeneInfo, intergenicSequence As String) As FeatureScores
            ' 验证链方向
            If upstreamGene.Strand <> downstreamGene.Strand Then
                Return Nothing ' 或者抛出异常
            End If

            Dim features As New FeatureScores With {
                .Motifs = New Dictionary(Of String, Double),
                .IntergenicDistance = CalculateIntergenicDistance(upstreamGene, downstreamGene),' 1. 基因间距离
                .NeighborhoodConservation = CalculateNeighborhoodConservation(upstreamGene, downstreamGene), ' 2. 基因邻域保守性
                .PhylogeneticDistance = CalculatePhylogeneticDistanceHamming(upstreamGene, downstreamGene), ' 3. 系统发育距离 (Hamming)
                .LengthRatio = CalculateLengthRatio(upstreamGene, downstreamGene),' 4. 基因长度比
                .GOSimilarity = CalculateGOSimilarity(upstreamGene, downstreamGene),' 6. GO功能相似性
                .PhylogeneticDistanceShannon = CalculatePhylogeneticDistanceShannon(upstreamGene, downstreamGene, .PhylogeneticDistance),
                .upstreamID = upstreamGene.GeneID,
                .downstreamID = downstreamGene.GeneID,
                .DistanceGroup = GetDistanceGroup(upstreamGene, downstreamGene)
            }

            ' 5. DNA基序频率 (使用论文中提到的关键基序)
            For Each motif As String In motifs
                features.Motifs($"Motif_{motif}") = CalculateMotifFrequency(intergenicSequence, motif)
            Next

            Return features
        End Function

        ''' <summary>
        ''' 1. 计算基因间距离
        ''' 论文公式: DI = downstream_gene_start - (upstream_gene_end + 1)
        ''' 并根据论文 Materials and Methods 应用 [-50, 250] 的截断值
        ''' </summary>
        ''' <param name="upstreamGene">上游基因信息</param>
        ''' <param name="downstreamGene">下游基因信息</param>
        ''' <returns>经截断处理后的基因间距离</returns>
        Public Shared Function CalculateIntergenicDistance(upstreamGene As GeneInfo, downstreamGene As GeneInfo) As Integer
            Dim rawDistance As Integer = downstreamGene.Start - (upstreamGene.[End] + 1)
            ' 根据论文，应用 -50 和 250 的截断值
            Return Math.Max(-50, Math.Min(250, rawDistance))
        End Function

        ''' <summary>
        ''' where ``L(gi, gj, Gk)`` is the loglikelihood of a gene pair to be neighbors in the kth genome Gk. 
        ''' The log-likelihood score Is computed as the probability that gi And gj are neighbors within a distance dk(i,j) In Gk, 
        ''' Or L(gi, gj, Gk) = log Pij; Pij Is defined as follows:
        ''' 
        ''' (i)   Pij = (1-pik)(1-pjk), if both genes are absent from genome Gk,
        ''' (ii)  Pij = (1-pik) pjk, if only gene j Is present In genome Gk,
        ''' (iii) Pij = pik (1-pjk), if only gene i Is present In genome Gk,
        ''' (iv)  Pij = (pikpjkdk(i, j) (2Nk-dk(i,j)-1))/(Nk(Nk-1)), If genes i And j are present In genome Gk.
        ''' 
        ''' dk(ij) Is the number of genes between gi And gj; Nk Is the number Of genes In genome Gk; And pik Is the probability 
        ''' that gene gi Is present In genome Gk.
        ''' </summary>
        ''' <param name="gene1">基因1信息</param>
        ''' <param name="gene2">基因2信息</param>
        ''' <returns>邻域保守性总得分 S</returns>
        ''' <remarks>
        ''' 2. 计算基因邻域保守性 (Neighborhood Conservation)
        ''' </remarks>
        Public Function CalculateNeighborhoodConservation(gene1 As GeneInfo, gene2 As GeneInfo) As Double
            Dim totalScore As Double = 0.0

            For Each genome As GenomeInfo In referenceGenomes
                Dim p1 As Double = If(phylumProbabilities.ContainsKey(gene1.GeneID) AndAlso
                                 phylumProbabilities(gene1.GeneID).ContainsKey(genome.Phylum),
                                 phylumProbabilities(gene1.GeneID)(genome.Phylum), 0.0)
                Dim p2 As Double = If(phylumProbabilities.ContainsKey(gene2.GeneID) AndAlso
                                 phylumProbabilities(gene2.GeneID).ContainsKey(genome.Phylum),
                                 phylumProbabilities(gene2.GeneID)(genome.Phylum), 0.0)

                Dim gene1Present As Boolean = gene1.PhylogeneticProfile.ContainsKey(genome.GenomeID) AndAlso
                                         gene1.PhylogeneticProfile(genome.GenomeID)
                Dim gene2Present As Boolean = gene2.PhylogeneticProfile.ContainsKey(genome.GenomeID) AndAlso
                                         gene2.PhylogeneticProfile(genome.GenomeID)

                Dim Pij As Double
                If Not gene1Present AndAlso Not gene2Present Then
                    Pij = (1 - p1) * (1 - p2)
                ElseIf Not gene1Present AndAlso gene2Present Then
                    Pij = (1 - p1) * p2
                ElseIf gene1Present AndAlso Not gene2Present Then
                    Pij = p1 * (1 - p2)
                Else
                    ' 基因均存在的情况，需确保位置索引可用
                    If genome.GenePositions.ContainsKey(gene1.GeneID) AndAlso genome.GenePositions.ContainsKey(gene2.GeneID) Then
                        Dim dk As Integer = Math.Abs(genome.GenePositions(gene1.GeneID) - genome.GenePositions(gene2.GeneID)) - 1
                        Pij = (p1 * p2 * dk * (2 * genome.GeneCount - dk - 1)) / (genome.GeneCount * (genome.GeneCount - 1))
                    Else
                        ' 如果位置数据缺失，退化为仅考虑存在概率的乘积
                        Pij = p1 * p2
                    End If
                End If

                totalScore += Math.Log(If(Pij > 0, Pij, 0.0000000001)) ' 避免log(0)
            Next

            Return -totalScore
        End Function

        ''' <summary>
        ''' For the Hamming distance between two genes A And B, we sum the number Of times that only A Or B Is found in the genome, 
        ''' DH=Sum([1,n], di)‚ where n Is the number of genomes, di=0 if the orthologs of A And B are both present Or both absent in genome i, 
        ''' And di = 1 otherwise.
        ''' </summary>
        ''' <param name="gene1">基因1信息</param>
        ''' <param name="gene2">基因2信息</param>
        ''' <returns>汉明距离得分</returns>
        ''' <remarks>
        ''' 3. 计算系统发育距离 (Hamming Distance)
        ''' </remarks>
        Public Function CalculatePhylogeneticDistanceHamming(gene1 As GeneInfo, gene2 As GeneInfo) As Integer
            Dim distance As Integer = 0
            For Each genome As GenomeInfo In referenceGenomes
                Dim g1Present As Boolean = gene1.PhylogeneticProfile.ContainsKey(genome.GenomeID) AndAlso
                                      gene1.PhylogeneticProfile(genome.GenomeID)
                Dim g2Present As Boolean = gene2.PhylogeneticProfile.ContainsKey(genome.GenomeID) AndAlso
                                      gene2.PhylogeneticProfile(genome.GenomeID)

                If g1Present <> g2Present Then
                    distance += 1
                End If
            Next
            Return distance
        End Function

        ''' <summary>
        ''' 3. 计算系统发育距离 - 香农熵距离 (补充实现)
        ''' 论文公式: DE = n - (n - DH) * Sqrt(E(p) / p)
        ''' 其中 p 是0身份(两基因均不存在)的比例，E(p) = -p*log(p) - (1-p)*log(1-p)
        ''' </summary>
        ''' <param name="gene1">基因1信息</param>
        ''' <param name="gene2">基因2信息</param>
        ''' <param name="DH">distance result of <see cref="CalculatePhylogeneticDistanceHamming"/></param>
        ''' <returns>香农熵距离得分</returns>
        '''
        Public Function CalculatePhylogeneticDistanceShannon(gene1 As GeneInfo, gene2 As GeneInfo, DH As Integer) As Double
            ' 计算 p: 0 identities 的比例
            Dim zeroCount As Integer = 0
            For Each genome As GenomeInfo In referenceGenomes
                Dim g1Present As Boolean = gene1.PhylogeneticProfile.ContainsKey(genome.GenomeID) AndAlso gene1.PhylogeneticProfile(genome.GenomeID)
                Dim g2Present As Boolean = gene2.PhylogeneticProfile.ContainsKey(genome.GenomeID) AndAlso gene2.PhylogeneticProfile(genome.GenomeID)
                ' 两者均不存在则为 0 identity
                If Not g1Present AndAlso Not g2Present Then
                    zeroCount += 1
                End If
            Next

            Dim n As Integer = referenceGenomes.Length
            Dim p As Double = zeroCount / n
            If p = 0 OrElse p = 1 Then Return n - (n - DH) ' 边界情况处理

            ' E(p) = -p*log(p) - (1-p)*log(1-p)
            Dim Ep As Double = -p * Math.Log(p) - (1 - p) * Math.Log(1 - p)

            ' DE = n - (n - DH) * Sqrt(E(p) / p)
            Dim DE As Double = n - (n - DH) * Math.Sqrt(Ep / p)

            Return DE
        End Function

        ''' <summary>
        ''' The score is calculated as the natural log of the length ratio of upstream gene And downstream gene, 
        ''' Or L = ln(li/lj), j = i + 1, whereas li And lj are the length of the genes.
        ''' </summary>
        ''' <param name="upstreamGene"></param>
        ''' <param name="downstreamGene"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 4. 计算基因长度比 (Length Ratio)
        ''' </remarks>
        Public Shared Function CalculateLengthRatio(upstreamGene As GeneInfo, downstreamGene As GeneInfo) As Double
            Return Math.Log(upstreamGene.Length / downstreamGene.Length)
        End Function

        ''' <summary>
        ''' 5. 计算DNA基序频率
        ''' 论文公式: Fm = X / ((L-d+1) * p)
        ''' X为观测次数，L为序列长度，d为基序长度，p为基于核苷酸频率计算的期望频率
        ''' </summary>
        ''' <param name="intergenicSequence">下游基因上游100nt的基因间序列</param>
        ''' <param name="motif">待检测的DNA基序字符串</param>
        ''' <returns>基序的归一化频率得分</returns>
        Public Function CalculateMotifFrequency(intergenicSequence As String, motif As String) As Double
            Dim observedCount As Integer = 0
            Dim motifLength As Integer = motif.Length

            ' 计算基序在序列中的出现次数
            For i As Integer = 0 To intergenicSequence.Length - motifLength
                Dim substring As String = intergenicSequence.Substring(i, motifLength)
                If substring = motif Then
                    observedCount += 1
                End If
            Next

            ' 计算期望频率
            Dim expectedProbability As Double = 1.0
            For Each c As String In motif
                expectedProbability *= nucleotideFrequencies(c)
            Next

            Dim expectedCount As Double = (intergenicSequence.Length - motifLength + 1) * expectedProbability
            Return If(expectedCount > 0, observedCount / expectedCount, 0)
        End Function

        ''' <summary>
        ''' 6. 计算GO功能相似性 (修正为严格符合论文的路径最大匹配)
        ''' 论文定义: SGO(gi,gj) = max s(Vi,Vj)
        ''' s(Vi,Vj) 是由两个基因的GO术语分别诱导出的两条路径之间的共同术语数量
        ''' 取所有路径对中的最大值
        ''' </summary>
        ''' <param name="gene1">基因1信息</param>
        ''' <param name="gene2">基因2信息</param>
        ''' <returns>GO功能相似性最大得分</returns>
        Public Function CalculateGOSimilarity(gene1 As GeneInfo, gene2 As GeneInfo) As Integer
            If gene1.GO_Terms Is Nothing OrElse gene2.GO_Terms Is Nothing OrElse gene1.GO_Terms.Count = 0 OrElse gene2.GO_Terms.Count = 0 Then
                Return 0
            End If

            Dim maxSimilarity As Integer = 0

            ' 遍历两个基因的GO术语对，寻找最大路径交集
            For Each term1 In gene1.GO_Terms
                Dim ancestors1 As New HashSet(Of String) From {term1}
                AddAncestorTerms(term1, ancestors1)

                For Each term2 In gene2.GO_Terms
                    Dim ancestors2 As New HashSet(Of String) From {term2}
                    AddAncestorTerms(term2, ancestors2)

                    ' 计算当前两条路径的共同祖先数
                    Dim commonTerms As Integer = ancestors1.Intersect(ancestors2).Count()
                    If commonTerms > maxSimilarity Then
                        maxSimilarity = commonTerms
                    End If
                Next
            Next

            Return maxSimilarity
        End Function

        ''' <summary>
        ''' 递归添加祖先GO术语到集合中
        ''' </summary>
        ''' <param name="term">当前GO术语</param>
        ''' <param name="termSet">用于收集祖先术语的HashSet</param>
        Private Sub AddAncestorTerms(term As String, termSet As HashSet(Of String))
            If goHierarchy.ContainsKey(term) Then
                For Each parentTerm In goHierarchy(term)
                    If termSet.Add(parentTerm) Then
                        AddAncestorTerms(parentTerm, termSet)
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' 判断相邻基因对属于哪个距离分组，用于后续选择特定的分类器模型
        ''' 论文 Results: 划分为 U40, U200, O200 进行子组训练
        ''' </summary>
        ''' <param name="upstreamGene">上游基因信息</param>
        ''' <param name="downstreamGene">下游基因信息</param>
        ''' <returns>距离分组枚举值；如果两基因不在同一链上，则返回 Nothing</returns>
        Public Shared Function GetDistanceGroup(upstreamGene As GeneInfo, downstreamGene As GeneInfo) As IntergenicDistanceGroup
            ' 1. 必须在同一条链上才可能是Operon
            If upstreamGene.Strand <> downstreamGene.Strand Then
                Return IntergenicDistanceGroup.NA
            End If

            Dim dist As Integer = CalculateIntergenicDistance(upstreamGene, downstreamGene)

            If dist < 40 Then Return IntergenicDistanceGroup.U40
            If dist > 200 Then Return IntergenicDistanceGroup.O200
            Return IntergenicDistanceGroup.U200
        End Function
    End Class
End Namespace
