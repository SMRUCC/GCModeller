''' <summary>
''' 一次反应两侧（反应物集合 ↔ 产物集合）的原子映射结果
''' </summary>
Public Class MappingResult

    ''' <summary>(反应物原子索引, 产物原子索引)</summary>
    Public ReadOnly Property Pairs As New List(Of (r As Integer, p As Integer))

    ''' <summary>反应物原子 → 产物原子（-1 = 未映射）</summary>
    Public Property ReactantToProduct As Integer()

    ''' <summary>产物原子 → 反应物原子（-1 = 未映射）</summary>
    Public Property ProductToReactant As Integer()

    ''' <summary>反应物侧的未映射原子（离去基团）</summary>
    Public ReadOnly Property ReactantOnly As New List(Of Integer)

    ''' <summary>产物侧的未映射原子（加入基团）</summary>
    Public ReadOnly Property ProductOnly As New List(Of Integer)

End Class