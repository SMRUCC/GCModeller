Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH
Imports Microsoft.VisualBasic.DataVisualization.Network.Abstract

Namespace LDM

    ''' <summary>
    ''' blast之中的一个hit相当于网络之中的一条边
    ''' </summary>
    Public Class Hit
        Implements INetworkEdge, IBlastHit

        ''' <summary>
        ''' 相当于identities值
        ''' </summary>
        ''' <returns></returns>
        Public Property weight As Double Implements INetworkEdge.Confidence
        Public Property query As String Implements IInteraction.source, IBlastHit.locusId
        ''' <summary>
        ''' 基因组或者蛋白质家族的配对字符串，在进行字符串连接之前先按照字母顺序排序
        ''' </summary>
        ''' <returns></returns>
        Public Property genomePairId As String Implements INetworkEdge.InteractionType
        Public Property subject As String Implements IInteraction.target, IBlastHit.Address

        Public Overrides Function ToString() As String
            Return $"{query} => {subject}"
        End Function
    End Class
End Namespace