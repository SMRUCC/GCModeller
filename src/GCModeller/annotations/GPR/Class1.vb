''' <summary>
''' 检测潜在的多亚基酶复合体
''' 基于基因距离、方向、以及功能相似性
''' </summary>
Public Class EnzymeComplexDetector
    Private Const ComplexMaxDistance As Integer = 1000
    Private Const MinComplexGenes As Integer = 2

    Public Function DetectComplexes(genes As Gene()) As List(Of List(Of Gene))
        Dim complexes = New List(Of List(Of Gene))()
        Dim visited = New HashSet(Of String)()

        For i = 0 To genes.Length - 2
            If visited.Contains(genes(i).LocusTag) Then Continue For

            Dim currentComplex = New List(Of Gene) From {genes(i)}

            ' 寻找相邻的、同链的基因
            For j = i + 1 To genes.Length - 1
                Dim distance = genes(j).Left - genes(i).Right
                If distance > ComplexMaxDistance Then Exit For

                ' 检查功能相关性
                If AreGenesFunctionallyRelated(genes(i), genes(j)) Then
                    currentComplex.Add(genes(j))
                    visited.Add(genes(j).LocusTag)
                End If
            Next

            If currentComplex.Count >= MinComplexGenes Then
                complexes.Add(currentComplex)
            End If
        Next

        Return complexes
    End Function

    Private Function AreGenesFunctionallyRelated(gene1 As Gene, gene2 As Gene) As Boolean
        ' 1. 同链检测
        If gene1.Strand <> gene2.Strand Then Return False

        ' 2. EC号相似性（同一大类）
        If gene1.EcNumbers.Any() AndAlso gene2.EcNumbers.Any() Then
            Dim ec1Classes = gene1.EcNumbers.Select(Function(ec) ec.Split("."c)(0))
            Dim ec2Classes = gene2.EcNumbers.Select(Function(ec) ec.Split("."c)(0))
            Return ec1Classes.Intersect(ec2Classes).Any()
        End If

        ' 3. 可添加基因名称相似性等其他指标
        Return True
    End Function

    Public Sub EnhanceComplexScores(complexes As List(Of List(Of Gene)),
                                    pathways As List(Of Pathway),
                                    ByRef geneScores As Dictionary(Of String, Dictionary(Of String, Double)))

        For Each complexGenes In complexes
            ' 收集复合体中所有EC号
            Dim allECs = New List(Of String)()
            For Each gene In complexGenes
                allECs.AddRange(gene.EcNumbers)
            Next

            ' 找到这些EC号共同参与的通路
            Dim commonPathways = FindCommonPathways(allECs, pathways)

            ' 增强这些通路的分数
            For Each pathway In commonPathways
                Dim complexScore = 0.4
                For Each gene In complexGenes
                    For Each reaction In pathway.Reactions
                        If Not geneScores(gene.LocusTag).ContainsKey(reaction.Id) Then
                            geneScores(gene.LocusTag)(reaction.Id) = complexScore
                        ElseIf geneScores(gene.LocusTag)(reaction.Id) < complexScore Then
                            geneScores(gene.LocusTag)(reaction.Id) = complexScore
                        End If
                    Next
                Next
            Next
        Next
    End Sub
End Class