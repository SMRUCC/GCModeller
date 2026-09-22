Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 多结构域融合酶检测。
''' 
''' 如果一个蛋白同时携带多个 EC 编号，并且这些 EC 所对应的反应在同一条通路中构成
''' 连续（间隔不超过 <see cref="GPRParameters.MaxGapInPathway"/> 步）的酶促步骤，
''' 则认为这是一个融合酶，并对其在通路中真正参与的反应给出证据。
''' </summary>
Public Class FusionGeneAnalyzer

    ReadOnly index As ContextIndices

    Sub New(index As ContextIndices)
        Me.index = index
    End Sub

    ''' <summary>
    ''' 收集某个基因的融合酶证据
    ''' </summary>
    Public Function CollectEvidence(gene As GeneTable, opt As GPRParameters) As IEnumerable(Of ReactionEvidence)
        Dim results As New List(Of ReactionEvidence)

        If gene Is Nothing OrElse index Is Nothing Then Return results
        If opt Is Nothing Then opt = New GPRParameters
        If gene.EC_Number Is Nothing Then Return results

        Dim ecNumbers As String() = gene.EC_Number _
            .Where(Function(ec) Not String.IsNullOrEmpty(ec)) _
            .Select(Function(ec) ec.Trim) _
            .Distinct(StringComparer.OrdinalIgnoreCase) _
            .ToArray

        If ecNumbers.Length < Math.Max(2, opt.FusionMinECNumbers) Then Return results

        ' 该基因各个 EC 编号所指向的反应
        Dim reactions As New List(Of MetabolicReaction)

        For Each ec As String In ecNumbers
            reactions.AddRange(index.GetReactionsByEC(ec))
        Next

        ' 修复：先按反应编号去重，并在后续按通路逐个筛选隶属关系。
        ' 原实现直接使用全局 EC 索引取出的反应与通路图做匹配，
        ' 大多数反应其实并不属于当前通路，导致证据几乎全部被跳过。
        Dim distinct As MetabolicReaction() = reactions _
            .Where(Function(r) r IsNot Nothing AndAlso Not String.IsNullOrEmpty(r.id)) _
            .GroupBy(Function(r) r.id, StringComparer.OrdinalIgnoreCase) _
            .Select(Function(g) g.First) _
            .ToArray

        If distinct.Length < 2 Then Return results

        For Each pathway As Pathway In index.Pathways
            Dim key As String = If(String.IsNullOrEmpty(pathway.ID), pathway.name, pathway.ID)
            Dim members As MetabolicReaction() = distinct _
                .Where(Function(r) index.IsReactionInPathway(key, r.id)) _
                .ToArray

            If members.Length < 2 Then Continue For

            Dim continuity As Double = EvaluateContinuity(key, members, opt)
            If continuity <= 0 Then Continue For

            Dim weight As Double = AssociationEvidence.Normalize(opt.FusionBaseScore + opt.FusionContinuityGain * continuity)

            For Each reaction As MetabolicReaction In members
                results.Add(New ReactionEvidence(reaction.id, New AssociationEvidence With {
                    .Kind = EvidenceKind.FusionGene,
                    .Weight = weight,
                    .RawScore = 1.0,
                    .Source = $"{gene.locus_id} fusion [{String.Join("/", ecNumbers)}] @ {key}"
                }))
            Next
        Next

        Return results
    End Function

    ''' <summary>
    ''' 计算若干反应在同一条通路内部的连续程度，取值 [0, 1]。
    ''' 
    ''' 通过有界广度优先搜索统计"存在有向相邻关系"的反应对比例，
    ''' 从而把同一通路内彼此无关的反应排除出去。
    ''' </summary>
    Private Function EvaluateContinuity(pathwayId As String, reactions As MetabolicReaction(), opt As GPRParameters) As Double
        ' 修复：原实现直接做 joint / reactions.Count，当 reactions 为空时会除零。
        If reactions Is Nothing OrElse reactions.Length < 2 Then Return 0

        Dim ordered As Integer = reactions.Length * (reactions.Length - 1)
        If ordered <= 0 Then Return 0

        Dim joint As Integer = 0
        Dim maxGap As Integer = Math.Max(1, opt.MaxGapInPathway)

        For i As Integer = 0 To reactions.Length - 1
            For j As Integer = 0 To reactions.Length - 1
                If i = j Then Continue For

                Dim gap As Integer = index.GetReactionGap(pathwayId, reactions(i).id, reactions(j).id, maxGap)
                If gap > 0 Then joint += 1
            Next
        Next

        If joint = 0 Then Return 0

        Return Math.Min(1.0, CDbl(joint) / ordered)
    End Function

End Class
