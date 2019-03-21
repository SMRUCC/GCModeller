Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace v2

    Public Module OperationExtensions

        ''' <summary>
        ''' 基因组之中的基因发生了缺失突变
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="geneList$"></param>
        ''' <returns></returns>
        <Extension> Public Function DeleteMutation(model As VirtualCell, geneList$()) As VirtualCell
            Dim deleted As Index(Of String) = geneList

            ' 删除目标基因组之中所有发生缺失突变的基因
            For Each replicon As replicon In model.genome.replicons
                replicon.genes = replicon.genes _
                    .Where(Function(g)
                               Return Not g.locus_tag Like deleted
                           End Function) _
                    .ToArray
            Next

            ' 将对应的调控关系也删除掉
            model.genome.regulations = model.genome _
                .regulations _
                .Where(Function(reg)
                           Return Not reg.regulator Like deleted AndAlso Not reg.target Like deleted
                       End Function) _
                .ToArray
            ' 将对应的酶促过程也删除掉
            model.MetabolismStructure.Enzymes = model.MetabolismStructure _
                .Enzymes _
                .Where(Function(enz) Not enz.geneID Like deleted) _
                .ToArray
            ' 讲代谢途径之中的酶分子的定义也删除掉
            For Each [module] As FunctionalCategory In model.MetabolismStructure.maps
                For Each pathway As Pathway In [module].pathways
                    pathway.enzymes = pathway _
                        .enzymes _
                        .Where(Function(enz) Not enz.Comment Like deleted) _
                        .ToArray
                Next
            Next

            Return model
        End Function

        ''' <summary>
        ''' 删除所有没有在当前的模型之中找到对应的酶的酶促反应过程
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Trim(model As VirtualCell) As VirtualCell
            ' 得到在当前的虚拟细胞模型之中所有的可以被酶催化的酶促反应过程
            ' 的编号列表
            Dim allEnzymatics = model.MetabolismStructure _
                .Enzymes _
                .Select(Function(enz)
                            Return enz.catalysis.Select(Function(c) c.reaction)
                        End Function) _
                .IteratesALL _
                .Distinct _
                .Indexing

            model.MetabolismStructure.Reactions = model _
                .MetabolismStructure _
                .Reactions _
                .Where(Function(r) Not r.isDisconnectedNode(allEnzymatics)) _
                .ToArray

            Return model
        End Function

        <Extension>
        Private Function isDisconnectedNode(r As Reaction, allEnzymatics As Index(Of String)) As Boolean
            If Not r.is_enzymatic Then
                ' 如果不需要酶，则可以自行发生
                ' 则不是断开的节点
                Return False
            Else
                ' 是酶促反应过程，但是在模型之中找不到对应的酶
                If r.ID Like allEnzymatics Then
                    Return False
                Else
                    Return True
                End If
            End If
        End Function
    End Module
End Namespace