Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace API.ImportantNodes

    ''' <summary>
    ''' 从footprint之中导出来的Cytoscape的网络数据文件
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Regulations
        Public Property Regulator As String
        Public Property ORF As String
    End Class

    Public Class RankRegulations

        ''' <summary>
        ''' 得分越高越重要
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RankScore As Integer
        Public Property Regulators As String()
        Public Property GeneCluster As String()

        Public Overrides Function ToString() As String
            Return String.Format("Rank.Score={0}; GeneCluster={1}; Regulators={2}", RankScore, String.Join(", ", GeneCluster), String.Join(", ", Regulators))
        End Function
    End Class

    Public Class NodeRank

        Public Property Rank As Integer

        <Collection("ImportantNodes")>
        Public Property Nodes As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace