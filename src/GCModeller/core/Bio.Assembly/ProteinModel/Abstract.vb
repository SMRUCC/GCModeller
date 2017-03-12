Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ProteinModel

    ''' <summary>
    ''' 一个蛋白质结构域对象的抽象模型
    ''' </summary>
    Public Interface IMotifDomain

        ''' <summary>
        ''' 蛋白质结构域在数据库之中的编号或者名称
        ''' </summary>
        ''' <returns></returns>
        Property ID As String
        ''' <summary>
        ''' 在蛋白质分子序列上面的位置区域
        ''' </summary>
        ''' <returns></returns>
        Property Location As Location

    End Interface
End Namespace