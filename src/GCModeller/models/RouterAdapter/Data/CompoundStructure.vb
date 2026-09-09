Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Namespace Data

    ''' <summary>
    ''' 一个可被 RetroPath 使用的化合物结构（净化后的 SMILES + 分子图 + 可选名称信息）。
    ''' </summary>
    Public Class CompoundStructure

        ''' <summary>化合物在数据源中的唯一标识（BioCyc 为 frame id，内部模型为 <c>id</c>）。</summary>
        Public Property Id As String

        ''' <summary>净化后的 SMILES（已剔除立体标记）。</summary>
        Public Property Smiles As String

        ''' <summary>该 SMILES 对应的分子图。</summary>
        Public Property Mol As Molecule

        ''' <summary>
        ''' 化合物的显示名称（可为空）。供"按名称匹配"的可移植汇白名单使用。
        ''' </summary>
        Public Property Name As String

        ''' <summary>
        ''' 同义名集合（可为空）。供"按名称匹配"的可移植汇白名单使用。
        ''' </summary>
        Public Property Synonyms As String()

        Public Overrides Function ToString() As String
            Return $"{Id} ({Smiles})"
        End Function

    End Class
End Namespace