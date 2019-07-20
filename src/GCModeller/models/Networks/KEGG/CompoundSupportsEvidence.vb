
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
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
    ''' <param name="reactions">所有的代谢反应过程的标准参考数据的集合</param>
    ''' <param name="background">目标基因组的KO注释结果的编号集合</param>
    ''' <param name="rxnId">
    ''' 需要进行计算判断的目标代谢反应过程
    ''' </param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function EvidenceScore(reactions As ReactionRepository, rxnId$, background As Index(Of String)) As Double
        If Not reactions.Exists(rxnId) Then
            ' 在参考数据库之中没有找到对应的代谢反应过程注释信息
            Return 0
        End If

        Dim model As Reaction = reactions.GetByKey(rxnId)
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
        Dim metabolites = model.ReactionModel _
            .GetMetabolites _
            .Select(Function(cpd) cpd.ID) _
            .Distinct _
            .ToArray
        Dim scores# = 0

        For Each metabolite As String In metabolites

        Next

        Return scores
    End Function
End Module
