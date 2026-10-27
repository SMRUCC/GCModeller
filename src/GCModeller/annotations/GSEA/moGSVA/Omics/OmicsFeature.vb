Namespace Omics

    ''' <summary>
    ''' 拼接矩阵中的单个特征条目，记录 (feature_id, omics_source, gene) 三元索引。
    ''' </summary>
    Public Class OmicsFeature
        ''' <summary>拼接矩阵中的唯一特征 id，格式为 <c>{omics}:{gene}</c></summary>
        Public Property feature_id As String
        ''' <summary>原始的基因符号（用于与基因集对齐）</summary>
        Public Property gene As String
        ''' <summary>该特征所属的组学来源</summary>
        Public Property omics As String

        Public Overrides Function ToString() As String
            Return feature_id
        End Function
    End Class
End Namespace