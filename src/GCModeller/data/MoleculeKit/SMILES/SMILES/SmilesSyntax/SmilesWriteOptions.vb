Namespace SmilesSyntax

    ''' <summary>
    ''' SMILES 写出选项：控制起点、邻居展开顺序、环闭合号顺序以及芳香/立体记号的输出。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 默认实例（<c>Default</c>）等价于历史行为：起点与邻居顺序由
    ''' <see cref="Graph.MoleculeGraph.MorganRanks"/> 决定，输出 Kekulé 形式、不写立体记号。
    ''' </para>
    ''' <para>
    ''' <see cref="RandomizeNeighborOrder"/> 打开后，同一个分子每次写出可以得到不同的
    ''' SMILES（SMILES 枚举/随机化）。随机化必须发生在<b>遍历层</b>：分子图的
    ''' Morgan 秩是拓扑不变量，仅仅重排原子下标再写出并不会改变结果。
    ''' </para>
    ''' <para>
    ''' 注意：SMILES 的四面体手性标记（<c>@</c>/<c>@@</c>）是相对于"邻居的书写顺序"解释的。
    ''' 随机展开邻居顺序会改变这一顺序，因此 <see cref="RandomizeNeighborOrder"/> 与
    ''' <see cref="EmitStereo"/> 不应同时开启；需要保留手性时请使用默认（确定性）遍历。
    ''' </para>
    ''' </remarks>
    Public Class SmilesWriteOptions

        ''' <summary>
        ''' 起点原子下标；<c>Nothing</c> 时按 <see cref="RandomizeNeighborOrder"/> 决定
        ''' （随机取分量内原子）或者退回 Morgan 秩最小的原子。
        ''' </summary>
        ''' <remarks>多组分分子（<c>A.B</c>）中该下标只对包含它的分量生效。</remarks>
        Public Property StartAtom As Integer? = Nothing

        ''' <summary>
        ''' 随机数种子；<c>Nothing</c> 时使用时间相关的默认种子。设定后写出结果可复现。
        ''' </summary>
        Public Property Seed As Integer? = Nothing

        ''' <summary>
        ''' 随机化邻居展开顺序、起点与环闭合号分配顺序。
        ''' </summary>
        Public Property RandomizeNeighborOrder As Boolean = False

        ''' <summary>
        ''' 芳香原子输出小写符号（<c>c</c>、<c>n</c>、<c>se</c> 等），芳香键省略键符号。
        ''' </summary>
        Public Property EmitAromatic As Boolean = False

        ''' <summary>
        ''' 输出四面体手性（<c>@</c>/<c>@@</c>）与双键立体记号（<c>/</c>、<c>\</c>）。
        ''' </summary>
        ''' <remarks>
        ''' 立体标记只有在"邻居书写顺序与解析时记录的顺序一致"时才保持原语义；
        ''' 因此该选项适合在确定性遍历下使用，见类型备注。
        ''' </remarks>
        Public Property EmitStereo As Boolean = False

        ''' <summary>
        ''' 默认选项：确定性遍历、Kekulé 形式、无立体记号（与历史行为一致）。
        ''' </summary>
        Public Shared ReadOnly Property [Default] As SmilesWriteOptions
            Get
                Return New SmilesWriteOptions()
            End Get
        End Property

        ''' <summary>
        ''' 随机枚举选项：随机起点/邻居顺序/环闭合号，保留芳香性，不输出立体记号。
        ''' </summary>
        ''' <param name="seed">随机数种子。</param>
        Public Shared Function Randomize(Optional seed As Integer? = Nothing) As SmilesWriteOptions
            Return New SmilesWriteOptions With {
                .RandomizeNeighborOrder = True,
                .EmitAromatic = True,
                .EmitStereo = False,
                .Seed = seed
            }
        End Function

    End Class

End Namespace
