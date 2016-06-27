
Public Class SegmentRenderData : Inherits SiteSigma
    ''' <summary>
    ''' 当前位点<see cref="Site"></see>上面的Query的基因号
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property QueryId As String()
    ''' <summary>
    ''' 与当前位点上面的<see cref="QueryId"></see>比对上的蛋白质的编号，假若没有比对上的记录，则为空字符串
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SubjectId As String()
    Public Property Identities As Double
    Public Property Positive As Double
End Class