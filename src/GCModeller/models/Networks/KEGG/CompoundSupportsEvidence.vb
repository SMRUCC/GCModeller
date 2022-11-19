#Region "Microsoft.VisualBasic::7864267ce3a974b87b22a0ff227a6e64, GCModeller\models\Networks\KEGG\CompoundSupportsEvidence.vb"

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

    '   Total Lines: 97
    '    Code Lines: 59
    ' Comment Lines: 28
    '   Blank Lines: 10
    '     File Size: 3.99 KB


    ' Module CompoundSupportsEvidence
    ' 
    '     Function: EvidenceScore
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Data

''' <summary>
''' 假若直接根据KO对应的Reaction进行网络的装配的话,可能会造成生成的网络非常的密集的现象出现
''' 因为一个reaction发生的基础是存在必须要有相应的代谢物和酶的存在
''' 所以在这个模块之中会尝试依据代谢物是否缺失来判断某一个反应网络之中的边是否存在
''' </summary>
Public Module CompoundSupportsEvidence

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="repo">所有的代谢反应过程的标准参考数据的集合</param>
    ''' <param name="background">目标基因组的KO注释结果的编号集合</param>
    ''' <param name="rxnId">
    ''' 需要进行计算判断的目标代谢反应过程
    ''' </param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function EvidenceScore(repo As ReactionRepository, rxnId$, background As Index(Of String), Optional depth% = 2) As Double
        If Not repo.Exists(rxnId) Then
            ' 在参考数据库之中没有找到对应的代谢反应过程注释信息
            Return 0
        ElseIf depth = 0 Then
            ' 已经到达推断的深度了
            Return 0
        End If

        Dim model As Reaction = repo.GetByKey(rxnId)
        ' 获取所需要的酶的列表
        Dim enzymes = model.Orthology.EntityList
        ' 获取在当前的background之下所能够得到的酶的列表
        Dim availableEnzymes As String() = enzymes.Where(Function(KOid) KOid Like background).ToArray

        ' 如果这个反应是酶促反应,但是没有在background中找到对应的KO注释
        ' 则说明反应不可能发生
        If enzymes.Length > 0 AndAlso availableEnzymes.Length = 0 Then
            Return 0
        End If

        ' 然后查找是否存在生成或者消耗代谢物的反应过程
        Dim objModel As DefaultTypes.Equation = model.ReactionModel
        Dim metabolites = objModel _
            .GetMetabolites _
            .Select(Function(cpd) cpd.ID) _
            .Distinct _
            .ToArray
        Dim scores# = 0
        Dim reactionIndex = repo.GetCompoundIndex

        For Each metabolite As String In metabolites
            ' 因为肯定是会包含当前的目标代谢反应的,所以下面的表达式肯定存在值
            Dim reactionIds = reactionIndex.TryGetValue(metabolite) _
                .Where(Function(rnId) rnId <> rxnId) _
                .ToArray

            If reactionIds.Length = 0 Then
                ' 除了自己以外,没有其他的代谢反应涉及到这个代谢物了
                ' 如果是右边,则可能存在
                If Not objModel.Produce(metabolite) Then
                    Return 0
                Else
                    scores += 1
                End If
            Else
                Dim supports = 0

                ' 每一个reaction增加一个supports
                For Each reactionId As String In reactionIds
                    If depth = 1 Then
                        supports += repo.GetByKey(reactionId) _
                            .Orthology _
                            .EntityList _
                            .Count(Function(id) id Like background)
                    Else
                        supports += repo.EvidenceScore(reactionId, background, depth - 1)
                    End If
                Next

                If supports = 0 Then
                    ' 当前的这个代谢物没有可以能够产生的代谢过程
                    ' 则当前的这个反应无法发生
                    Return 0
                Else
                    scores += supports
                End If
            End If
        Next

        Return scores
    End Function
End Module
