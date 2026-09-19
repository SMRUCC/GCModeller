Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 一个跨物种保守的基因邻接簇。
''' 
''' 注意：<see cref="geneIDs"/> 才是簇内基因的集合，<see cref="functions"/> 是该簇在其它
''' 物种中所被赋予的功能（EC 编号 / 反应编号 / 通路编号）。
''' </summary>
Public Class ConservedCluster

    Public Property ClusterID As String

    Public Property geneIDs As String()

    ''' <summary>
    ''' 该保守簇所记录的功能，允许是 EC 编号、反应编号或者通路编号
    ''' </summary>
    Public Property functions As String()

    ''' <summary>
    ''' 簇内基因数目（<see cref="geneIDs"/> 为空时返回 0）
    ''' </summary>
    Public ReadOnly Property GeneSetSize As Integer
        Get
            If geneIDs Is Nothing Then Return 0
            Return geneIDs.Length
        End Get
    End Property

    ''' <summary>
    ''' 安全地获取簇内基因集合
    ''' </summary>
    Public Function GetGeneIDs() As String()
        If geneIDs Is Nothing Then Return New String() {}
        Return geneIDs.Where(Function(id) Not String.IsNullOrEmpty(id)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray
    End Function

    ''' <summary>
    ''' 安全地获取功能集合
    ''' </summary>
    Public Function GetFunctions() As String()
        If functions Is Nothing Then Return New String() {}
        Return functions.Where(Function(f) Not String.IsNullOrEmpty(f)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray
    End Function

    Public Overrides Function ToString() As String
        Return $"{ClusterID} [{GeneSetSize}]"
    End Function

End Class
