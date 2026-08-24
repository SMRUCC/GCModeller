#Region "Microsoft.VisualBasic::bfa141d4b23da334677ea611325e83fd, annotations\GPR\EnzymeComplexDetector.vb"

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

    '   Total Lines: 90
    '    Code Lines: 62 (68.89%)
    ' Comment Lines: 12 (13.33%)
    '    - Xml Docs: 33.33%
    ' 
    '   Blank Lines: 16 (17.78%)
    '     File Size: 3.58 KB


    ' Class EnzymeComplexDetector
    ' 
    '     Function: AreGenesFunctionallyRelated, DetectComplexes
    ' 
    '     Sub: EnhanceComplexScores
    ' 
    ' /********************************************************************************/

#End Region

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
        If gene1.strand <> gene2.strand Then
            Return False
        End If
        If gene1.EC_Number.IsNullOrEmpty OrElse gene2.EC_Number.IsNullOrEmpty Then
            Return False
        End If

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
