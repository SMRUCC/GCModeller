''' <summary>
''' 一个可被 RetroPath 使用的化合物结构（净化后的 SMILES + 分子图）
''' </summary>
Public Class CompoundStructure

    ''' <summary>BioCyc 的 compound frame id（UNIQUE-ID）</summary>
    Public Property Id As String
    ''' <summary>净化后的 SMILES（剔除立体标记之后的字符串）</summary>
    Public Property Smiles As String
    ''' <summary>该 SMILES 对应的分子图</summary>
    Public Property Mol As Molecule

    Public Overrides Function ToString() As String
        Return $"{Id} ({Smiles})"
    End Function

End Class