Namespace Search

    Public Class SearchOptions

        ''' <summary>
        ''' beam | dfs
        ''' </summary>
        Public Strategy As String = "beam"
        Public BeamWidth As Int32 = 50
        Public MaxDepth As Int32 = 6
        Public MaxPaths As Int32 = 20
        ''' <summary>
        ''' 每规则每分子最大匹配数
        ''' </summary>
        Public MatchLimit As Int32 = 50

    End Class
End Namespace