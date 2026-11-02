Namespace SmilesSyntax

    ''' <summary>
    ''' SMILES 随机枚举器：把一个分子随机重写成另一条等价的 SMILES 字符串。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 对应 RDKit 的 <c>Chem.RenumberAtoms</c> + <c>Chem.MolToSmiles(canonical=False)</c>：
    ''' 每一次随机化都会选择不同的起点、不同的邻居展开顺序与不同的环闭合号分配顺序，
    ''' 因此同一个分子可以得到多条合法且互不相同的 SMILES（用于去噪自编码式预训练语料增强）。
    ''' </para>
    ''' <para>
    ''' 实现方式与 RDKit 不同：这里不重排分子图，而是在写出阶段随机化遍历顺序。原因是
    ''' <see cref="Graph.MoleculeGraph.MorganRanks"/> 是拓扑不变量，重排原子下标后写出结果不会改变。
    ''' </para>
    ''' <para>
    ''' 立体化学：随机展开顺序会改变 <c>@</c>/<c>@@</c> 的相对语义，因此
    ''' <see cref="Isomeric"/> 默认为 <c>False</c>；需要保真立体信息时请使用默认的
    ''' 确定性写出器（<see cref="SmilesWriter.Write(Graph.MoleculeGraph)"/>）保留原始文本。
    ''' </para>
    ''' </remarks>
    Public Class SmilesEnumerator

        Private ReadOnly rnd As Random
        Private ReadOnly sync As New Object()

        ''' <summary>
        ''' 是否输出立体记号（<c>@</c>/<c>@@</c>、<c>/</c>、<c>\</c>）。
        ''' </summary>
        ''' <remarks>
        ''' 默认 <c>False</c>：随机展开的邻居顺序会破坏手性标记的语义，详见类型备注。
        ''' </remarks>
        Public ReadOnly Property Isomeric As Boolean

        ''' <summary>
        ''' 创建一个枚举器。
        ''' </summary>
        ''' <param name="seed">
        ''' 随机数种子；<c>Nothing</c> 时使用时间相关的默认种子。设定后整条随机序列可复现。
        ''' </param>
        ''' <param name="isomeric">是否输出立体记号；默认 <c>False</c>。</param>
        Public Sub New(Optional seed As Integer? = Nothing, Optional isomeric As Boolean = False)
            Me.Isomeric = isomeric
            Me.rnd = If(seed.HasValue, New Random(seed.Value), New Random())
        End Sub

        ''' <summary>
        ''' 随机重写一条 SMILES。
        ''' </summary>
        ''' <param name="smiles">原始 SMILES。</param>
        ''' <returns>随机化后的 SMILES；解析失败时返回 <c>Nothing</c>（与 RDKit 版的 None 一致）。</returns>
        Public Function Randomize(smiles As String) As String
            Dim graph As Graph.MoleculeGraph = Nothing

            If Not MoleculeIO.TryParse(smiles, graph) Then Return Nothing

            Return Randomize(graph)
        End Function

        ''' <summary>
        ''' 随机重写一个已解析的分子图。
        ''' </summary>
        ''' <param name="graph">待随机化的分子图。</param>
        ''' <returns>随机化后的 SMILES。</returns>
        Public Function Randomize(graph As Graph.MoleculeGraph) As String
            If graph Is Nothing Then Return Nothing

            Dim options As New SmilesWriteOptions With {
                .RandomizeNeighborOrder = True,
                .EmitAromatic = True,
                .EmitStereo = Isomeric,
                .Seed = nextSeed()
            }

            Return SmilesWriter.Write(graph, options)
        End Function

        ''' <summary>
        ''' 批量随机化；解析失败的条目返回 <c>Nothing</c>，顺序与输入一致。
        ''' </summary>
        ''' <param name="smiles">原始 SMILES 集合。</param>
        Public Function RandomizeAll(smiles As IEnumerable(Of String)) As String()
            If smiles Is Nothing Then Return New String() {}

            Return smiles.Select(Function(sm) Randomize(sm)).ToArray()
        End Function

        ''' <summary>
        ''' 为单次随机化取一个互相独立的种子。
        ''' </summary>
        ''' <remarks>
        ''' 让 <see cref="SmilesWriter"/> 自己持有随机数发生器，可以把随机化写成无共享状态的形式，
        ''' 从而支持并行生成语料；这里只共享一个受锁保护的种子发生器。
        ''' </remarks>
        Private Function nextSeed() As Integer
            SyncLock sync
                Return rnd.Next()
            End SyncLock
        End Function

    End Class

End Namespace
