Imports Microsoft.VisualBasic.DataVisualization.Network.Abstract

Namespace LDM

    ''' <summary>
    ''' 相当于网络之中的一个节点
    ''' </summary>
    Public Class Protein : Implements INode

        Public Property LocusId As String Implements INode.Identifier
        ''' <summary>
        ''' 家族分类
        ''' </summary>
        ''' <returns></returns>
        Public Property Family As String Implements INode.NodeType
        ''' <summary>
        ''' 这个蛋白质所处的基因组的名称
        ''' </summary>
        ''' <returns></returns>
        Public Property Genome As String
    End Class
End Namespace