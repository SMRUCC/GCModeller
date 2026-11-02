Imports SMRUCC.genomics.Molecule.SMILES.SmilesSyntax

Namespace Corpus

    ''' <summary>
    ''' 一个批次的训练数据。
    ''' </summary>
    Public Class SmilesBatch

        ''' <summary>token 下标矩阵，形状 <c>[batch, time]</c>。</summary>
        Public Property Ids As Integer(,)

        ''' <summary>每个样本的原始长度（含 <c>&lt;sos&gt;</c>/<c>&lt;eos&gt;</c>）。</summary>
        Public Property Lengths As Integer()

        ''' <summary>各样本在语料中的下标（便于回溯）。</summary>
        Public Property Indexes As Integer()

        ''' <summary>批次大小。</summary>
        Public ReadOnly Property Size As Integer
            Get
                Return If(Ids Is Nothing, 0, Ids.GetLength(0))
            End Get
        End Property

    End Class

    ''' <summary>
    ''' SMILES 序列数据集：按需随机化 / 分词 / 编码 / 填充，并按批次产出。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 对应 Python 的 <c>Seq2seqDataset</c> + <c>DataLoader(batch_size, shuffle)</c>：
    ''' 每个 epoch 重新洗牌，每条样本在取用时可以选择做一次 SMILES 随机枚举
    ''' （把"同一分子的不同写法"当作去噪自编码的噪声源）。
    ''' </para>
    ''' <para>
    ''' 这里不做并行预取（Python 的 <c>num_workers</c>）：随机化与分词都是纯 CPU 短任务，
    ''' 顺序执行可以让训练完全可复现；如果确实需要，调用方可以并行构造多个 loader。
    ''' </para>
    ''' </remarks>
    Public Class SequenceDataLoader

        Private ReadOnly _smiles As List(Of String)
        Private ReadOnly _vocab As WordVocab
        Private ReadOnly _seqLen As Integer
        Private ReadOnly _enumerate As Boolean
        Private ReadOnly _enumerator As SmilesEnumerator
        Private ReadOnly _rnd As Random

        ''' <summary>语料条数。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return _smiles.Count
            End Get
        End Property

        ''' <summary>定长序列长度（padding / 截断目标）。</summary>
        Public ReadOnly Property SequenceLength As Integer
            Get
                Return _seqLen
            End Get
        End Property

        ''' <summary>是否在取样本时做 SMILES 随机枚举。</summary>
        Public ReadOnly Property EnumerateSmiles As Boolean
            Get
                Return _enumerate
            End Get
        End Property

        ''' <summary>
        ''' 创建数据加载器。
        ''' </summary>
        ''' <param name="smiles">SMILES 语料。</param>
        ''' <param name="vocab">词表。</param>
        ''' <param name="seqLen">定长序列长度，默认 220（论文取值）。</param>
        ''' <param name="enumerate">是否在取样本时做一次随机枚举。</param>
        ''' <param name="seed">洗牌与随机枚举的种子；<c>Nothing</c> 表示不固定。</param>
        Public Sub New(smiles As IEnumerable(Of String),
                       vocab As WordVocab,
                       Optional seqLen As Integer = 220,
                       Optional enumerate As Boolean = False,
                       Optional seed As Integer? = Nothing)
            If smiles Is Nothing Then Throw New ArgumentNullException(NameOf(smiles))
            If vocab Is Nothing Then Throw New ArgumentNullException(NameOf(vocab))
            If seqLen <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(seqLen))

            _smiles = smiles.Where(Function(sm) Not String.IsNullOrWhiteSpace(sm)).ToList()
            _vocab = vocab
            _seqLen = seqLen
            _enumerate = enumerate
            _rnd = If(seed.HasValue, New Random(seed.Value), New Random())
            _enumerator = If(enumerate, New SmilesEnumerator(seed, False), Nothing)
        End Sub

        ''' <summary>
        ''' 按批次枚举数据。
        ''' </summary>
        ''' <param name="batchSize">批大小。</param>
        ''' <param name="shuffle">是否打乱顺序（每个 epoch 调用一次即可）。</param>
        ''' <param name="dropLast">是否丢弃最后一个不完整批次。</param>
        Public Function Batches(batchSize As Integer,
                                Optional shuffle As Boolean = True,
                                Optional dropLast As Boolean = False) As IEnumerable(Of SmilesBatch)
            If batchSize <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(batchSize))

            Return iterate(batchSize, shuffle, dropLast)
        End Function

        Private Iterator Function iterate(batchSize As Integer, shuffle As Boolean,
                                          dropLast As Boolean) As IEnumerable(Of SmilesBatch)
            Dim order As Integer() = Enumerable.Range(0, _smiles.Count).ToArray()

            If shuffle Then shuffleInPlace(order)

            Dim start As Integer = 0

            While start < order.Length
                Dim count As Integer = Math.Min(batchSize, order.Length - start)

                If count < batchSize AndAlso dropLast Then Exit While

                Yield buildBatch(order, start, count)
                start += count
            End While
        End Function

        ''' <summary>把一个批次的 SMILES 编码为 <c>[batch, seqLen]</c> 的下标矩阵。</summary>
        Private Function buildBatch(order As Integer(), start As Integer, count As Integer) As SmilesBatch
            Dim ids(count - 1, _seqLen - 1) As Integer
            Dim lengths(count - 1) As Integer
            Dim indexes(count - 1) As Integer

            For i As Integer = 0 To count - 1
                Dim index As Integer = order(start + i)
                Dim smiles As String = _smiles(index)
                Dim text As String = If(_enumerate, enumerateOnce(smiles), smiles)
                Dim tokens As String() = SmilesTokenizer.Split(text)
                Dim originLength As Integer = 0
                Dim seq As Integer() = _vocab.ToSeq(tokens, _seqLen, withEos:=True, withSos:=True,
                                                    originLength:=originLength)

                For t As Integer = 0 To _seqLen - 1
                    ids(i, t) = seq(t)
                Next

                lengths(i) = originLength
                indexes(i) = index
            Next

            Return New SmilesBatch With {
                .Ids = ids,
                .Lengths = lengths,
                .Indexes = indexes
            }
        End Function

        ''' <summary>
        ''' 做一次随机枚举；解析失败时退回原始文本（与 Python 的 <c>Randomizer.__call__</c> 一致）。
        ''' </summary>
        Private Function enumerateOnce(smiles As String) As String
            Dim randomized As String = _enumerator.Randomize(smiles)

            If String.IsNullOrEmpty(randomized) Then Return smiles

            ' Python 侧在随机化后长度超过 MAX_LEN-2 时会退回未随机化的写法
            If SmilesTokenizer.Split(randomized).Length > _seqLen - 3 Then Return smiles

            Return randomized
        End Function

        Private Sub shuffleInPlace(items As Integer())
            For i As Integer = items.Length - 1 To 1 Step -1
                Dim j As Integer = _rnd.Next(i + 1)
                Dim tmp As Integer = items(i)

                items(i) = items(j)
                items(j) = tmp
            Next
        End Sub

    End Class

End Namespace
