Imports SMRUCC.genomics.ComponentModel.Annotation

''' <summary>
''' 检测潜在的多亚基酶复合体
''' 基于基因距离、方向、以及功能相似性
''' </summary>
Public Class EnzymeComplexDetector

    Private Const ComplexMaxDistance As Integer = 1000
    Private Const MinComplexGenes As Integer = 2

    Public Function DetectComplexes(genes As GeneTable()) As List(Of List(Of GeneTable))
        Dim complexes = New List(Of List(Of GeneTable))()
        Dim visited = New HashSet(Of String)()

        For i = 0 To genes.Length - 2
            If visited.Contains(genes(i).locus_id) Then Continue For

            Dim currentComplex = New List(Of GeneTable) From {genes(i)}

            ' 寻找相邻的、同链的基因
            For j = i + 1 To genes.Length - 1
                Dim distance = genes(j).left - genes(i).right
                If distance > ComplexMaxDistance Then Exit For

                ' 检查功能相关性
                If AreGenesFunctionallyRelated(genes(i), genes(j)) Then
                    currentComplex.Add(genes(j))
                    visited.Add(genes(j).locus_id)
                End If
            Next

            If currentComplex.Count >= MinComplexGenes Then
                complexes.Add(currentComplex)
            End If
        Next

        Return complexes
    End Function

    Private Function AreGenesFunctionallyRelated(gene1 As GeneTable, gene2 As GeneTable) As Boolean
        ' 1. 同链检测
        If gene1.strand <> gene2.strand Then Return False

        ' 2. EC号相似性（同一大类）
        If gene1.EC_Number.Any() AndAlso gene2.EC_Number.Any() Then
            Dim ec1Classes = gene1.EC_Number.Select(Function(ec) ec.Split("."c)(0))
            Dim ec2Classes = gene2.EC_Number.Select(Function(ec) ec.Split("."c)(0))
            Return ec1Classes.Intersect(ec2Classes).Any()
        End If

        ' 3. 可添加基因名称相似性等其他指标
        Return True
    End Function

    Public Sub EnhanceComplexScores(complexes As List(Of List(Of GeneTable)),
                                    context As ContextIndices,
                                    ByRef geneScores As Dictionary(Of String, Dictionary(Of String, Double)))

        For Each complexGenes In complexes
            ' 收集复合体中所有EC号
            Dim allECs = New List(Of String)()
            For Each gene In complexGenes
                allECs.AddRange(gene.EC_Number)
            Next

            ' 找到这些EC号共同参与的通路
            Dim commonPathways As IEnumerable(Of Pathway) = context.FindCommonPathways(allECs)

            ' 增强这些通路的分数
            For Each pathway In commonPathways
                Dim complexScore = 0.4
                For Each gene In complexGenes
                    For Each reaction In pathway.metabolicNetwork
                        If Not geneScores(gene.locus_id).ContainsKey(reaction.id) Then
                            geneScores(gene.locus_id)(reaction.id) = complexScore
                        ElseIf geneScores(gene.locus_id)(reaction.id) < complexScore Then
                            geneScores(gene.locus_id)(reaction.id) = complexScore
                        End If
                    Next
                Next
            Next
        Next
    End Sub
End Class