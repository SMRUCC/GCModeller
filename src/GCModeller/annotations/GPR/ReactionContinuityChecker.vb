#Region "Microsoft.VisualBasic::f7dc9f750cd50c9d14240ce221fa9018, annotations\GPR\ReactionContinuityChecker.vb"

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

    '   Total Lines: 62
    '    Code Lines: 37 (59.68%)
    ' Comment Lines: 14 (22.58%)
    '    - Xml Docs: 71.43%
    ' 
    '   Blank Lines: 11 (17.74%)
    '     File Size: 2.62 KB


    ' Class ReactionContinuityChecker
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: LoadFromContext
    ' 
    '     Sub: CheckContinuity
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 检查反应之间的化学相容性
''' 如果一个反应的产物是下一个反应的底物，增强这些反应的分数
''' </summary>
Public Class ReactionContinuityChecker

    ' 反应ID -> 底物/产物映射
    Private reactionCompounds As Dictionary(Of String, MetabolicReaction)

    Public Sub New(reactionData As Dictionary(Of String, MetabolicReaction))
        reactionCompounds = reactionData
    End Sub

    ''' <summary>
    ''' 对通路中的每个反应对检查连续性
    ''' </summary>
    ''' <param name="pathway"></param>
    ''' <param name="geneScores"></param>
    ''' <param name="genome"></param>
    Public Sub CheckContinuity(pathway As Pathway, geneScores As Dictionary(Of String, Double), genome As Genome)
        For i As Integer = 0 To pathway.metabolicNetwork.Length - 2
            Dim currRxn = pathway.metabolicNetwork(i)
            Dim nextRxn = pathway.metabolicNetwork(i + 1)

            If Not reactionCompounds.ContainsKey(currRxn.id) Or
               Not reactionCompounds.ContainsKey(nextRxn.id) Then Continue For

            Dim currProducts As String() = reactionCompounds(currRxn.id).right.Keys
            Dim nextSubstrates As String() = reactionCompounds(nextRxn.id).left.Keys

            ' 检查化学相容性
            Dim overlap = currProducts.Intersect(nextSubstrates).Count()
            If overlap > 0 Then
                ' 增强这两个反应的关联分数
                Dim continuityScore = 0.3 + (overlap / Math.Max(currProducts.Count, nextSubstrates.Count)) * 0.3

                ' 如果基因已经被关联到这些反应，增强分数
                For Each geneId As String In genome.GetGenesForReaction(currRxn.id).Keys
                    If geneScores.ContainsKey(geneId) Then
                        geneScores(geneId) = Math.Max(geneScores(geneId), continuityScore)
                    End If
                Next
            End If
        Next
    End Sub

    Public Shared Function LoadFromContext(context As ContextIndices) As ReactionContinuityChecker
        Dim rxnIndex = context.ECtoReactions.Values _
            .SelectMany(Function(v) v) _
            .GroupBy(Function(r) r.id) _
            .ToDictionary(Function(r) r.Key,
                          Function(r)
                              Return r.First
                          End Function)

        Return New ReactionContinuityChecker(rxnIndex)
    End Function

End Class
