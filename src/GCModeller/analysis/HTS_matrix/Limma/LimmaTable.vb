
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Math.Statistics
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

''' <summary>
''' limma ``topTable`` output dataframe.
''' </summary>
Public Class LimmaTable : Implements IDeg, INamedValue, IReadOnlyId, IStatPvalue, IStatFDR

    ''' <summary>
    ''' row names - gene id
    ''' </summary>
    ''' <returns></returns>
    Public Property id As String Implements IDeg.label, INamedValue.Key, IReadOnlyId.Identity
    ''' <summary>
    ''' logFC（对数倍数变化）
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' logFC表示基因在两组之间的对数倍数变化（log2 fold change）。对于简单的两两比较，
    ''' logFC等于处理组平均表达量减去对照组平均表达量（在log2尺度下）。
    ''' 例如，logFC=1表示处理组表达量是对照组的2倍（因为2^1=2），logFC=-1表示处理组表达量是对照组的一半。
    ''' logFC是差异表达分析中最直观的指标，用于衡量基因表达的上调或下调程度。
    ''' 需要注意的是，logFC的符号和大小依赖于实验设计和对比的设置，
    ''' 因此在多因素实验中应结合设计矩阵解读其含义。
    ''' </remarks>
    Public Property logFC As Double Implements IDeg.log2FC
    ''' <summary>
    ''' AveExpr（平均表达量）
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' AveExpr表示该基因在所有样本中的平均表达水平（通常取log2 CPM或log2强度）。
    ''' AveExpr可用于评估基因的表达丰度，以及结合logFC判断差异变化的相对幅度。
    ''' 例如，一个logFC=2的基因，如果AveExpr很低（例如接近背景水平），则其实际上调倍数可能并不显著；
    ''' 反之，如果AveExpr很高，则logFC=2可能表示生物学上有意义的上调。
    ''' AveExpr在微阵列分析中常用于绘制MA图（M-A plot），其中M表示logFC，A表示AveExpr，
    ''' 以直观地展示差异表达与表达丰度的关系。在RNA-seq分析中，
    ''' AveExpr同样可用于质量控制和结果解释。
    ''' </remarks>
    Public Property AveExpr As Double
    ''' <summary>
    ''' t（经验贝叶斯调节的t统计量）
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' t列即为经验贝叶斯调节的t统计量。它是logFC与其标准误差的比值，
    ''' 反映了logFC偏离零的程度相对于其估计误差的大小。t统计量越大（绝对值），
    ''' 表示logFC相对于其标准误差越大，即差异表达越显著。与常规t统计量不同，
    ''' 这里的t已经经过经验贝叶斯调节，其标准误差$s_j^{\text{post}}$融合了先验信息，
    ''' 因此在小样本情况下更稳定。t统计量服从自由度更高的t分布，其对应的P值即为P.Value。
    ''' 在解读时，t的符号与logFC一致，绝对值大小与P.Value呈负相关（|t|越大，P.Value越小）。
    ''' </remarks>
    Public Property t As Double
    ''' <summary>
    ''' P.Value（未校正的P值）
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' P.Value是针对原假设“该基因无差异表达”计算得到的P值，基于经验贝叶斯调节的t统计量。
    ''' 具体地，P值通过比较调节的t统计量与其理论分布（t分布）获得，表示在原假设下观察到当前或更极端t值的概率。
    ''' 由于采用了经验贝叶斯调节，该P值在小样本情况下更加可靠。P.Value越小，表示拒绝原假设的证据越强，
    ''' 即基因差异表达的可能性越高。然而，由于成千上万个基因同时进行检验，
    ''' P.Value需要进行多重检验校正以控制错误发现率。
    ''' </remarks>
    <Column("P.Value")>
    Public Property P_Value As Double Implements IDeg.pvalue, IStatPvalue.pValue
    ''' <summary>
    ''' adj.P.Val（多重检验校正后的P值）
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' adj.P.Val是对P.Value进行多重检验校正后的P值，用于控制错误发现率（FDR）或家族误差率（FWER）。
    ''' limma支持多种校正方法，其中最常用的是Benjamini-Hochberg（BH）方法，即控制FDR。
    ''' adj.P.Val表示在所有被宣布差异表达的基因中，预期假阳性的比例。例如，设定adj.P.Val阈值为0.05，
    ''' 意味着我们期望在所有选中的差异表达基因中，最多有5%是假阳性。adj.P.Val是筛选差异表达基因时的重要依据，
    ''' 通常选择adj.P.Val &lt; 0.05作为显著性阈值。需要注意的是，adj.P.Val依赖于所选的校正方法和阈值，
    ''' 不同的校正方法（如BH、Bonferroni、Holm等）会得到不同的校正后P值。
    ''' </remarks>
    <Column("adj.P.Val")>
    Public Property adj_P_Val As Double Implements IStatFDR.adjPVal

    ''' <summary>
    ''' B（贝叶斯对数几率统计量）
    ''' 
    ''' B统计量或log-odds，表示基因是差异表达的概率的对数；计算公式：B = log10( (1 - p) / p ) 其中p是基因是差异表达的概率，基于经验贝叶斯方法估计；
    ''' B统计量的数学意义：表示"基因差异表达的可信度"（B > 0表示有差异表达证据，B > 1表示强证据）。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' B列是贝叶斯对数几率统计量，表示基因差异表达的后验对数几率。B值综合考虑了效应大小（logFC）和先验信息，
    ''' 可以转换为后验概率来解释。例如，B=1.5表示基因差异表达的后验几率约为e^1.5=4.48:1，即后验概率约82%。
    ''' B值越大，表示支持差异表达的证据越强；B值越小（负值），表示支持非差异表达的证据越强。
    ''' B统计量已经内置了多重检验校正，因此可以直接用于基因排序和筛选。在实际应用中，
    ''' B值与P.Value通常提供一致的基因排序，但B值提供了概率解释，
    ''' 有助于在决策时结合生物学意义进行权衡。
    ''' </remarks>
    Public Property B As Double

    Public Property [class] As String

    Public Overrides Function ToString() As String
        Return $"{id} - logfc:{logFC}, p-value={P_Value}"
    End Function

    Public Shared Iterator Function LoadTable(filepath As String) As IEnumerable(Of LimmaTable)
        Dim lines As String() = filepath.LineIterators(strictFile:=True).ToArray
        Dim cols As Index(Of String) = Document.ParseHeaders(lines(Scan0)).Indexing(base:=0)
        Dim logFC As Integer = cols("logFC")
        Dim AveExpr As Integer = cols("AveExpr")
        Dim t As Integer = cols("t")
        Dim P_Value As Integer = cols("P.Value")
        Dim adj_P_Val As Integer = cols("adj.P.Val")
        Dim B As Integer = cols("B")

        For Each line As String In lines.Skip(1)
            Dim vec As NamedValue(Of Double()) = Reader.ParseGeneRowTokens(line)
            Dim data As Double() = CType(vec, Double())
            Dim expr As New LimmaTable With {
                .id = vec.Name,
                .logFC = If(logFC > -1, data(logFC), 0),
                .adj_P_Val = If(adj_P_Val > -1, data(adj_P_Val), 1),
                .AveExpr = If(AveExpr > -1, data(AveExpr), 0),
                .B = If(B > -1, data(B), 0),
                .P_Value = If(P_Value > -1, data(P_Value), 1),
                .t = If(t > -1, data(t), 0)
            }

            Yield expr
        Next
    End Function

End Class
