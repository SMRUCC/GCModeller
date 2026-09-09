Imports SMRUCC.genomics.Analysis.RetroPath.Model

''' <summary>
''' 代谢网络通路搜索器的统一契约：目标导向的逆向搜索 + 起点导向的定向合成搜索。
''' </summary>
Public Interface IRouter

    ''' <summary>以目标分子 B 做逆向合成通路搜索（结果中不限定起点）。</summary>
    Function FindPathway(targetSmiles As String) As PathReport

    ''' <summary>从起点代谢物 A 出发，搜索合成目标代谢物 B 的最经济通路。</summary>
    Function SynthesisRoute(sourceSmiles As String, targetSmiles As String,
                            Optional role As SourceRoles = SourceRoles.Source,
                            Optional strict As Boolean = False,
                            Optional allowedExtra As IEnumerable(Of (String, String)) = Nothing,
                            Optional maxRoutes As Integer = 0,
                            Optional escalate As Boolean = True) As RouteReport

End Interface
