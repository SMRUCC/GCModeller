Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.ComponentModel.Annotation

''' <summary>
''' 共表达的基因伙伴及其相关系数
''' </summary>
Public Class CoexpressionPartner

    Public Property GeneId As String
    Public Property Correlation As Double

    Public Overrides Function ToString() As String
        Return $"{GeneId} (r={Correlation.ToString("F3")})"
    End Function

End Class

''' <summary>
''' 基于表达数据的共表达分析。
''' 
''' 共表达的基因很可能参与同一通路，因此可以把共表达伙伴的"直接证据反应"
''' 作为当前基因的一条弱证据。
''' 
''' 修复要点：
''' 
''' + 原实现在 <see cref="Genome.MetabolicNetwork"/> 尚未物化时被调用，
'''   导致共表达分析恒为空、完全失效；现在由 <see cref="MetabolicAssociator"/>
'''   在阶段 1 物化种子反应之后调用；
''' + 原实现未排除自身（相关矩阵对角线恒为 1.0）；
''' + 原实现硬编码 0.7 / 0.4 两个魔法数字，现在全部来自 <see cref="GPRParameters"/>；
''' + 原实现要求外部传入一个与算法内部不同的 <see cref="Genome"/> 实例，状态无法共享。
''' </summary>
Public Class CoexpressionAnalyzer

    ReadOnly correlation As CorrelationMatrix

    Public Sub New(expressionData As Matrix)
        If expressionData Is Nothing Then
            Throw New ArgumentNullException(NameOf(expressionData))
        End If

        Me.correlation = expressionData.Correlation(Function(row) row.experiments)
    End Sub

    ''' <summary>
    ''' 表达矩阵中参与共表达分析的基因数目
    ''' </summary>
    Public ReadOnly Property GeneCount As Integer
        Get
            If correlation Is Nothing Then Return 0
            Return correlation.size
        End Get
    End Property

    ''' <summary>
    ''' 与目标基因共表达（相关系数不低于阈值）的其它基因，按相关系数降序
    ''' </summary>
    Public Function FindCoexpressedGenes(geneId As String,
                                         threshold As Double,
                                         Optional maxItems As Integer = Integer.MaxValue) As IEnumerable(Of CoexpressionPartner)

        Dim results As New List(Of CoexpressionPartner)

        If String.IsNullOrEmpty(geneId) Then Return results
        If correlation Is Nothing OrElse Not correlation.HasObject(geneId) Then Return results

        For Each label As String In correlation.GetLabels()
            ' 修复：必须排除基因自身，相关矩阵的对角线恒为 1.0
            If String.Equals(label, geneId, StringComparison.OrdinalIgnoreCase) Then Continue For

            Dim r As Double = correlation.dist(geneId, label)
            If Double.IsNaN(r) Then Continue For
            If r < threshold Then Continue For

            results.Add(New CoexpressionPartner With {
                .GeneId = label,
                .Correlation = r
            })
        Next

        Return results _
            .OrderByDescending(Function(p) p.Correlation) _
            .Take(Math.Max(0, maxItems)) _
            .ToArray
    End Function

    ''' <summary>
    ''' 收集某个基因的共表达证据
    ''' </summary>
    Public Function CollectEvidence(gene As GeneTable, genome As Genome, opt As GPRParameters) As IEnumerable(Of ReactionEvidence)
        Dim results As New List(Of ReactionEvidence)

        If gene Is Nothing OrElse genome Is Nothing Then Return results
        If opt Is Nothing Then opt = New GPRParameters

        For Each partner As CoexpressionPartner In FindCoexpressedGenes(gene.locus_id, opt.CoexpressionThreshold, opt.MaxCoexpressionPartners)
            For Each reactionId As String In genome.GetSeedReactions(partner.GeneId)
                results.Add(New ReactionEvidence(reactionId, New AssociationEvidence With {
                    .Kind = EvidenceKind.Coexpression,
                    .Weight = opt.BaseCoexpressionScore,
                    .RawScore = partner.Correlation,
                    .Source = partner.ToString
                }))
            Next
        Next

        Return results
    End Function

End Class
