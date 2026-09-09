
''' <summary>
''' 结构索引的输入种子：一个待索引的化合物。
''' </summary>
Public Class CompoundSeed

    ''' <summary>化合物唯一标识。</summary>
    Public Property Id As String

    ''' <summary>原始 SMILES（未净化；为空即表示数据源未提供结构）。</summary>
    Public Property Smiles As String

    ''' <summary>显示名称（可选，供名称白名单与结果展示使用）。</summary>
    Public Property Name As String

    ''' <summary>同义名（可选）。</summary>
    Public Property Synonyms As String()

    ''' <summary>便捷构造。</summary>
    Public Shared Function Create(id As String, smiles As String,
                                  Optional name As String = Nothing,
                                  Optional synonyms As String() = Nothing) As CompoundSeed
        Return New CompoundSeed With {
            .Id = id, .Smiles = smiles, .Name = name, .Synonyms = synonyms
        }
    End Function

End Class