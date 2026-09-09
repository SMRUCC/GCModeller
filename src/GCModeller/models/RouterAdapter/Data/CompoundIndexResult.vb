Namespace Data

    ''' <summary>
    ''' 结构索引的构建结果。
    ''' </summary>
    Public Class CompoundIndexResult

        ''' <summary>id → 已净化结构（可直接供规则挖掘与汇构建使用）。</summary>
        Public ReadOnly Property Structures As New Dictionary(Of String, CompoundStructure)()

        ''' <summary>数据源未提供 SMILES 的化合物 id。</summary>
        Public ReadOnly Property MissingSmiles As New List(Of String)()

        ''' <summary>SMILES 存在但不可用（解析失败 / 含不支持元素 / 超重原子上限）的原因计数。</summary>
        Public ReadOnly Property RejectReasons As New Dictionary(Of String, Integer)()

        ''' <summary>每种拒绝原因的第一个样例（"id = SMILES"），便于定位问题条目。</summary>
        Public ReadOnly Property RejectSamples As New Dictionary(Of String, String)()

        ''' <summary>输入的化合物总数。</summary>
        Public Property Total As Integer

        ''' <summary>成功建立结构的化合物数。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return Structures.Count
            End Get
        End Property

        ''' <summary>取某个化合物的结构；不存在时返回 Nothing。</summary>
        Public Function TryGet(id As String) As CompoundStructure
            Dim st As CompoundStructure = Nothing
            Structures.TryGetValue(id, st)
            Return st
        End Function

    End Class
End Namespace