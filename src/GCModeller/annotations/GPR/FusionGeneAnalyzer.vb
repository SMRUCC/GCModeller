#Region "Microsoft.VisualBasic::e278449c203f9dc9619e96d2df9c3180, annotations\GPR\FusionGeneAnalyzer.vb"

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

    '   Total Lines: 91
    '    Code Lines: 58 (63.74%)
    ' Comment Lines: 15 (16.48%)
    '    - Xml Docs: 73.33%
    ' 
    '   Blank Lines: 18 (19.78%)
    '     File Size: 3.48 KB


    ' Class FusionGeneAnalyzer
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CheckPathwayContinuity
    ' 
    '     Sub: AnalyzeFusionGenes
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 检测多域融合酶
''' 如果一个蛋白对应多个EC号，检查这些EC是否在同一个通路中形成连续的酶促步骤
''' </summary>
Public Class FusionGeneAnalyzer

    ReadOnly index As ContextIndices

    Sub New(index As ContextIndices)
        Me.index = index
    End Sub

    Public Sub AnalyzeFusionGenes(genes As IEnumerable(Of GeneTable), pathways As Pathway(), ByRef geneScores As Dictionary(Of String, Dictionary(Of String, Double)))
        For Each gene As GeneTable In genes
            If gene.EC_Number.TryCount < 2 Then
                Continue For
            End If

            ' 获取该基因所有EC号参与的反应
            Dim geneReactions = New List(Of MetabolicReaction)()

            For Each ec As String In gene.EC_Number
                If index.ECtoReactions.ContainsKey(ec) Then
                    geneReactions.AddRange(index.ECtoReactions(ec))
                End If
            Next

            ' 检查这些反应是否在同一个通路中形成连续或相邻步骤
            Dim pathwayContinuityScores = CheckPathwayContinuity(geneReactions, pathways)

            ' 增强这些通路中所有反应的分数
            For Each pathwayScore In pathwayContinuityScores
                Dim pathway = pathwayScore.Key
                Dim continuity = pathwayScore.Value

                For Each reaction In pathway.metabolicNetwork
                    Dim fusionScore = 0.3 + continuity * 0.5

                    If Not geneScores(gene.locus_id).ContainsKey(reaction.id) OrElse
                       geneScores(gene.locus_id)(reaction.id) < fusionScore Then
                        geneScores(gene.locus_id)(reaction.id) = fusionScore
                    End If
                Next
            Next
        Next
    End Sub

    ''' <summary>
    ''' 实现检测反应在通路中的连续程度
    ''' 返回通路及其连续性分数
    ''' </summary>
    ''' <param name="reactions"></param>
    ''' <param name="pathways"></param>
    ''' <returns></returns>
    Private Function CheckPathwayContinuity(reactions As List(Of MetabolicReaction), pathways As Pathway()) As Dictionary(Of Pathway, Double)
        Dim scores As New Dictionary(Of Pathway, Double)

        For Each pathway As Pathway In pathways
            Dim joint As Integer

            For Each u As MetabolicReaction In reactions
                For Each v As MetabolicReaction In reactions
                    If u Is v Then
                        Continue For
                    End If

                    ' has a direct reaction edge means no gap
                    Dim upstream = pathway.ReactionNetwork.GetElementByID(u.id)
                    Dim downstream = pathway.ReactionNetwork.GetElementByID(v.id)

                    If upstream Is Nothing OrElse downstream Is Nothing Then
                        Continue For
                    End If

                    If pathway.ReactionNetwork.GetEdge(upstream, downstream) IsNot Nothing Then
                        joint += 1
                    End If
                Next
            Next

            If joint > 0 Then
                Call scores.Add(pathway, joint / reactions.Count)
            End If
        Next

        Return scores
    End Function
End Class
