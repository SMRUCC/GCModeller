#Region "Microsoft.VisualBasic::PanGenome.Report, PanGenomeReportData.vb"

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

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ReportJSON

    ''' <summary>
    ''' 泛基因组报告页面所使用的 JSON 数据模型。
    ''' </summary>
    ''' <remarks>
    ''' 这些类型会通过 <see cref="Microsoft.VisualBasic.Serialization.JSON.JsonContract"/> 被序列化为
    ''' 内嵌到 HTML 页面之中的 JSON 数据。属性名称即为前端 JavaScript 读取时使用的键名，
    ''' 因此这里刻意采用 camelCase 命名，以保持与模板脚本中的读取代码一致。
    ''' 
    ''' 注意：<see cref="System.Runtime.Serialization.Json.DataContractJsonSerializer"/> 不支持匿名类型，
    ''' 所以这里必须使用具名的 Public 类型；同时只有 Public 属性(不含字段)会被序列化。
    ''' </remarks>
    Public Class GenomeStatRow : Implements INamedValue

        Public Property name As String Implements INamedValue.Key
        Public Property geneCount As Integer
        Public Property specificCount As Integer
        Public Property coreRatio As Double

    End Class

    ''' <summary>
    ''' 通用的分类数据项（饼图、条形图以及结构变异类型的统计）
    ''' </summary>
    Public Class CategoryItem

        Public Property name As String
        Public Property value As Integer
        Public Property color As String

    End Class

    ''' <summary>
    ''' 泛基因组曲线的绘图数据
    ''' </summary>
    Public Class PangenomeCurveDataset

        Public Property genomeCounts As Integer()
        Public Property panGenes As Integer()
        Public Property coreGenes As Integer()
        ''' <summary>
        ''' 软核心基因家族的数量曲线（出现比例不低于软核心阈值）
        ''' </summary>
        Public Property softCoreGenes As Integer()

    End Class

    ''' <summary>
    ''' PAV 矩阵的绘图数据
    ''' </summary>
    Public Class PAVMatrixDataset

        Public Property genomes As String()
        Public Property families As String()
        Public Property matrix As Integer()()
        ''' <summary>
        ''' 是否因为规模上限而对数据做了抽样截断
        ''' </summary>
        Public Property truncated As Boolean

    End Class

    ''' <summary>
    ''' 遗传距离矩阵的绘图数据
    ''' </summary>
    Public Class GeneticDistanceDataset

        Public Property genomes As String()
        Public Property data As Double()()
        ''' <summary>
        ''' 是否因为规模上限而对数据做了抽样截断
        ''' </summary>
        Public Property truncated As Boolean

    End Class

    ''' <summary>
    ''' 共线性基因组对的展示数据项
    ''' </summary>
    Public Class CollinearPairItem

        Public Property genome1 As String
        Public Property genome2 As String
        ''' <summary>
        ''' 基因组对之间的共线性统计值，指标含义由 <see cref="CollinearityDataset.metricLabel"/> 说明
        ''' </summary>
        Public Property value As Double
        ''' <summary>
        ''' 基因组对之间的共线性区块个数
        ''' </summary>
        Public Property blocks As Integer
        ''' <summary>
        ''' 参与的染色体对摘要
        ''' </summary>
        Public Property chromosomes As String

    End Class

    ''' <summary>
    ''' 共线性结果的可视化数据：基因组×基因组共线性矩阵 + Top 基因组对排行
    ''' </summary>
    Public Class CollinearityDataset

        Public Property genomes As String()
        ''' <summary>
        ''' 对称的共线性统计矩阵，单元格含义由 <see cref="metricLabel"/> 说明
        ''' </summary>
        Public Property matrix As Double()()
        ''' <summary>
        ''' 矩阵与排行所使用的统计指标名称（共线基因对 / 共线性区块数）
        ''' </summary>
        Public Property metricLabel As String
        Public Property pairs As CollinearPairItem()
        ''' <summary>
        ''' 是否因为规模上限而只展示了部分基因组
        ''' </summary>
        Public Property truncated As Boolean

    End Class

    ''' <summary>
    ''' PAV矩阵PCA三维散点图中的单个样本点（一个基因组）
    ''' </summary>
    Public Class PCAPoint : Implements INamedValue

        Public Property name As String Implements INamedValue.Key
        ''' <summary>
        ''' 第一主成分得分
        ''' </summary>
        Public Property pc1 As Double
        ''' <summary>
        ''' 第二主成分得分
        ''' </summary>
        Public Property pc2 As Double
        ''' <summary>
        ''' 第三主成分得分
        ''' </summary>
        Public Property pc3 As Double
        ''' <summary>
        ''' 该基因组的核心基因占比（散点图的着色维度）
        ''' </summary>
        Public Property coreRatio As Double
        Public Property geneCount As Integer

    End Class

    ''' <summary>
    ''' PAV矩阵的PCA分析结果数据（用于三维散点图）
    ''' </summary>
    Public Class PCAScatterDataset

        Public Property points As PCAPoint()
        ''' <summary>
        ''' 坐标轴标题，例如 <c>PC1 (42.51%)</c>
        ''' </summary>
        Public Property pc1Label As String
        Public Property pc2Label As String
        Public Property pc3Label As String
        ''' <summary>
        ''' 着色维度的标题
        ''' </summary>
        Public Property colorLabel As String
        ''' <summary>
        ''' 实际参与PCA分析的基因家族数量
        ''' </summary>
        Public Property familyCount As Integer
        ''' <summary>
        ''' 各主成分的方差贡献率（百分比）
        ''' </summary>
        Public Property explained As Double()

    End Class

    ''' <summary>
    ''' 基因组级别的三维散点图数据点（一个基因组）
    ''' </summary>
    Public Class GenomeEntropyPoint : Implements INamedValue

        Public Property name As String Implements INamedValue.Key
        ''' <summary>
        ''' 基因存在/缺失均衡度的香农信息熵 H = -(p*log(p) + (1-p)*log(1-p))
        ''' </summary>
        Public Property entropy As Double
        ''' <summary>
        ''' 该基因组之中存在的基因家族数量 K
        ''' </summary>
        Public Property presentFamilies As Integer
        ''' <summary>
        ''' 该基因组之中缺失的基因家族数量 N - K
        ''' </summary>
        Public Property absentFamilies As Integer
        ''' <summary>
        ''' 特有基因数 / 基因总数（百分比）
        ''' </summary>
        Public Property specificRatio As Double
        ''' <summary>
        ''' 核心基因占比（百分比）
        ''' </summary>
        Public Property coreRatio As Double
        Public Property geneCount As Integer

    End Class

    ''' <summary>
    ''' 基因组级别的三维散点图数据（均衡度熵 / 特有基因占比 / 核心基因占比）
    ''' </summary>
    Public Class GenomeEntropyDataset

        Public Property points As GenomeEntropyPoint()
        Public Property entropyLabel As String
        Public Property specificRatioLabel As String
        Public Property coreRatioLabel As String
        ''' <summary>
        ''' 泛基因组的基因家族总数 N
        ''' </summary>
        Public Property familyCount As Integer

    End Class

    ''' <summary>
    ''' 数值为浮点数的分类数据项（饼图与条形图）
    ''' </summary>
    ''' <remarks>
    ''' 与 <see cref="CategoryItem"/> 的区别在于这里的 <see cref="value"/> 是浮点数的百分比，
    ''' 而 <see cref="CategoryItem.value"/> 是整数的家族绝对数量，两者不可以混用。
    ''' </remarks>
    Public Class PercentageItem

        Public Property name As String
        ''' <summary>
        ''' 百分比数值（0-100）
        ''' </summary>
        Public Property value As Double
        Public Property color As String

    End Class

    ''' <summary>
    ''' 基因家族分布比例的统计数据（每个基因组的四类家族占比的平均值 + 逐基因组明细）
    ''' </summary>
    ''' <remarks>
    ''' 当基因组的数量非常多的时候，壳基因与云基因会因为基因组数量的累加而在
    ''' 绝对数量上面远远超过核心基因，但是它们在单个基因组内部的占比其实并不高。
    ''' 这里通过"单个基因组内部的占比"这个口径来消除基因组数量的影响。
    ''' </remarks>
    Public Class CategoryPercentDataset

        ''' <summary>
        ''' 四个家族类别的名称，顺序与 <see cref="byGeneCount"/> / <see cref="byFamilyCount"/> 一致
        ''' </summary>
        Public Property categories As String()
        ''' <summary>
        ''' 基因组名称（按名称排序）
        ''' </summary>
        Public Property genomes As String()

        ''' <summary>
        ''' 口径一：某个基因组之内属于该类别的基因(拷贝数)数量占该基因组基因总数的百分比，
        ''' 再对全部基因组取平均。与 <see cref="categories"/> 同序。
        ''' </summary>
        Public Property byGeneCount As Double()
        ''' <summary>
        ''' 口径二：某个基因组之内属于该类别的家族个数占该基因组内出现的家族总数的百分比，
        ''' 再对全部基因组取平均。与 <see cref="categories"/> 同序。
        ''' </summary>
        Public Property byFamilyCount As Double()

        ''' <summary>
        ''' [类别][基因组] 口径一的逐基因组百分比明细
        ''' </summary>
        Public Property genomeByGeneCount As Double()()
        ''' <summary>
        ''' [类别][基因组] 口径二的逐基因组百分比明细
        ''' </summary>
        Public Property genomeByFamilyCount As Double()()

        ''' <summary>
        ''' 饼图与条形图所使用的配色，与 <see cref="categories"/> 同序
        ''' </summary>
        Public Property colors As String()

    End Class

    ''' <summary>
    ''' 结构变异矩阵的热图绘图数据（行=基因家族，列=基因组）
    ''' </summary>
    Public Class SVMatrixDataset

        ''' <summary>
        ''' 基因家族ID（矩阵的行）
        ''' </summary>
        Public Property families As String()
        ''' <summary>
        ''' 基因组名称（矩阵的列）
        ''' </summary>
        Public Property genomes As String()
        ''' <summary>
        ''' 矩阵数值 [家族][基因组]，没有SV事件的单元格为0
        ''' </summary>
        Public Property matrix As Double()()
        ''' <summary>
        ''' 单元格数值的含义说明（SV CopyNumber / SV Median）
        ''' </summary>
        Public Property metricLabel As String
        ''' <summary>
        ''' 是否因为规模上限而对家族行做了抽样
        ''' </summary>
        Public Property truncated As Boolean

    End Class

    ''' <summary>
    ''' SV 信息熵散点图当中的一个基因家族样本点
    ''' </summary>
    Public Class SVDomainEntropyPoint : Implements INamedValue

        ''' <summary>
        ''' 基因家族ID
        ''' </summary>
        Public Property name As String Implements INamedValue.Key
        ''' <summary>
        ''' 该家族的 SV CopyNumber 矩阵行（跨基因组的拷贝数分布）的香农信息熵
        ''' </summary>
        Public Property hCopyNumber As Double
        ''' <summary>
        ''' 该家族的 SV Median 矩阵行的香农信息熵（按Median的精确取值统计频率）
        ''' </summary>
        Public Property hMedian As Double
        ''' <summary>
        ''' Z-score 标准化之后的 CopyNumber 熵（散点图的X轴）
        ''' </summary>
        Public Property zCopyNumber As Double
        ''' <summary>
        ''' Z-score 标准化之后的 Median 熵（散点图的Y轴）
        ''' </summary>
        Public Property zMedian As Double
        ''' <summary>
        ''' 该家族存在SV事件的基因组数量
        ''' </summary>
        Public Property presentGenomes As Integer
        ''' <summary>
        ''' KMeans 聚类结果（按照簇中心确定性重排之后的簇编号）
        ''' </summary>
        Public Property cluster As Integer

    End Class

    ''' <summary>
    ''' SV 信息熵聚类的簇摘要
    ''' </summary>
    Public Class SVEntropyCluster

        ''' <summary>
        ''' 簇编号（与 <see cref="SVDomainEntropyPoint.cluster"/> 对应）
        ''' </summary>
        Public Property cluster As Integer
        ''' <summary>
        ''' 簇内的基因家族数量
        ''' </summary>
        Public Property size As Integer
        ''' <summary>
        ''' 簇内家族的平均 CopyNumber 熵（原始值）
        ''' </summary>
        Public Property meanCopyNumberEntropy As Double
        ''' <summary>
        ''' 簇内家族的平均 Median 熵（原始值）
        ''' </summary>
        Public Property meanMedianEntropy As Double
        ''' <summary>
        ''' 该簇在四象限进化模型之中的解读名称
        ''' </summary>
        Public Property label As String

    End Class

    ''' <summary>
    ''' SV 结构变异的信息熵散点图与 KMeans 聚类结果
    ''' </summary>
    Public Class SVDomainEntropyDataset

        Public Property points As SVDomainEntropyPoint()
        Public Property clusters As SVEntropyCluster()
        ''' <summary>
        ''' 参与聚类分析的基因家族数量
        ''' </summary>
        Public Property familyCount As Integer
        ''' <summary>
        ''' 因为出现频率过低而被过滤掉的基因家族数量
        ''' </summary>
        Public Property filteredCount As Integer
        ''' <summary>
        ''' KMeans 的簇数
        ''' </summary>
        Public Property clusterCount As Integer
        ''' <summary>
        ''' X轴标题（CopyNumber 熵）
        ''' </summary>
        Public Property copyNumberEntropyLabel As String
        ''' <summary>
        ''' Y轴标题（Median 熵）
        ''' </summary>
        Public Property medianEntropyLabel As String
        ''' <summary>
        ''' 参与统计的基因组总数（用于计算家族的出现频率）
        ''' </summary>
        Public Property genomeCount As Integer

    End Class
End Namespace