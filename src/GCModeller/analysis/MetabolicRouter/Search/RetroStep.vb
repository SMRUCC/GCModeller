Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Namespace Search

    ''' <summary>
    ''' 一步逆合成：把某个化合物按某条规则分解成若干前体。
    ''' </summary>
    Public Class RetroStep

        ''' <summary>所使用的规则 ID。</summary>
        Public RuleId As String
        ''' <summary>所使用的规则名称。</summary>
        Public RuleName As String
        ''' <summary>
        ''' 规则定义方向的适用态：forward / reverse（规则定义方向）。
        ''' </summary>
        Public Orientation As String
        ''' <summary>
        ''' 被分解化合物的分子指纹。
        ''' </summary>
        Public SubstrateKey As String
        ''' <summary>
        ''' 被分解化合物的分子对象（用于正向组装时还原结构）。
        ''' </summary>
        Public SubstrateMol As Molecule
        ''' <summary>
        ''' 分解得到的前体列表，每项为 (分子指纹, 分子)。
        ''' </summary>
        Public Precursors As New List(Of (key As String, mol As Molecule))()
        ''' <summary>
        ''' 本次分解中属于货币分子或自身的共产物个数（它们不计入前体）。
        ''' </summary>
        Public CoproductCount As Int32
        ''' <summary>
        ''' 该步反应的 ΔG（取自规则的 ΔG）。
        ''' </summary>
        Public DeltaG As Double
        ''' <summary>
        ''' 该步反应的酶可得性层级（1/2/3）。
        ''' </summary>
        Public EnzymeTier As EnzymeTiers
        ''' <summary>
        ''' 本次变换的原子映射，每项为 (模式类号, 分子原子索引)。
        ''' </summary>
        Public AtomMap As New List(Of Tuple(Of Int32, Int32))()

    End Class
End Namespace