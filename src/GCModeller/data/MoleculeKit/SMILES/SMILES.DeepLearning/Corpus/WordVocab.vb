Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports SMRUCC.genomics.Molecule.SMILES.SmilesSyntax

Namespace Corpus

    ''' <summary>
    ''' 词表：token ↔ 整数下标，以及语料频次统计。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 与 smiles-transformer 的 <c>WordVocab</c> 语义一致：
    ''' 前 5 个位置固定为 <c>&lt;pad&gt; &lt;unk&gt; &lt;eos&gt; &lt;sos&gt; &lt;mask&gt;</c>（下标 0-4），
    ''' 其余 token 先按字典序升序、再稳定地按出现频次降序排列。
    ''' </para>
    ''' <para>
    ''' 排序顺序很关键：它决定了 embedding 的行号。加载 Python 侧的 <c>vocab.pkl</c>
    ''' 时必须<b>保持</b> pickle 中的 <c>itos</c> 顺序，而不是重新按频次排序，
    ''' 因此另提供 <see cref="VocabPickleReader"/> 做只读导入。
    ''' </para>
    ''' </remarks>
    Public Class WordVocab

        ''' <summary>填充 token 的下标。</summary>
        Public Const PadIndex As Integer = 0

        ''' <summary>未登录 token 的下标。</summary>
        Public Const UnkIndex As Integer = 1

        ''' <summary>序列结束 token 的下标。</summary>
        Public Const EosIndex As Integer = 2

        ''' <summary>序列起始 token 的下标。</summary>
        Public Const SosIndex As Integer = 3

        ''' <summary>掩码 token 的下标。</summary>
        Public Const MaskIndex As Integer = 4

        ''' <summary>固定注册的特殊 token（顺序即下标）。</summary>
        Public Shared ReadOnly Specials As String() = {"<pad>", "<unk>", "<eos>", "<sos>", "<mask>"}

        Private ReadOnly _itos As New List(Of String)
        Private ReadOnly _stoi As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
        Private ReadOnly _freqs As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

        ''' <summary>下标 → token。</summary>
        Public ReadOnly Property IToS As IReadOnlyList(Of String)
            Get
                Return _itos
            End Get
        End Property

        ''' <summary>token → 下标。</summary>
        Public ReadOnly Property SToI As IReadOnlyDictionary(Of String, Integer)
            Get
                Return _stoi
            End Get
        End Property

        ''' <summary>token → 语料出现频次（只读快照，用于诊断）。</summary>
        Public ReadOnly Property Freqs As IReadOnlyDictionary(Of String, Integer)
            Get
                Return _freqs
            End Get
        End Property

        ''' <summary>词表大小。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return _itos.Count
            End Get
        End Property

        ''' <summary>
        ''' 依据频次统计构建词表。
        ''' </summary>
        ''' <param name="counter">token → 出现次数。</param>
        ''' <param name="maxSize">
        ''' 词表容量上限（不含 5 个特殊 token，与 Python 版一致）；
        ''' <c>Nothing</c> 表示不限制。
        ''' </param>
        ''' <param name="minFreq">最低出现次数；低于该值的 token 不会进入词表。</param>
        Public Sub New(counter As IDictionary(Of String, Integer),
                       Optional maxSize As Integer? = Nothing,
                       Optional minFreq As Integer = 1)
            If counter Is Nothing Then Throw New ArgumentNullException(NameOf(counter))

            For Each token As String In Specials
                _itos.Add(token)
                _stoi(token) = _itos.Count - 1
                _freqs(token) = 0
            Next

            For Each kv As KeyValuePair(Of String, Integer) In counter
                _freqs(kv.Key) = kv.Value
            Next

            Dim limit As Integer = If(maxSize.HasValue, maxSize.Value + Specials.Length, Integer.MaxValue)
            Dim minimum As Integer = Math.Max(minFreq, 1)

            ' 与 Python 一致：先按 token 字典序排序，再对整个列表做一次"稳定的"频次降序排序。
            ' 注意不能用 ThenByDescending —— 那只会在"字典序相同"的分组内生效（而 token 是唯一的），
            ' 结果就变成了纯字典序。
            Dim ordered As IEnumerable(Of KeyValuePair(Of String, Integer)) = counter.
                Where(Function(kv) Not _stoi.ContainsKey(kv.Key)).
                OrderBy(Function(kv) kv.Key, StringComparer.Ordinal).
                OrderByDescending(Function(kv) kv.Value)

            For Each kv As KeyValuePair(Of String, Integer) In ordered
                If kv.Value < minimum OrElse _itos.Count >= limit Then Exit For

                _stoi(kv.Key) = _itos.Count
                _itos.Add(kv.Key)
            Next
        End Sub

        ''' <summary>
        ''' 直接按既有 token 顺序构建词表（用于从 pickle/JSON 还原，不重新排序）。
        ''' </summary>
        ''' <param name="itos">已排好序的 token 列表。</param>
        ''' <param name="freqs">可选的频次统计。</param>
        Friend Sub New(itos As IEnumerable(Of String), freqs As IDictionary(Of String, Integer))
            For Each token As String In itos
                If _stoi.ContainsKey(token) Then Continue For

                _stoi(token) = _itos.Count
                _itos.Add(token)
            Next

            If freqs IsNot Nothing Then
                For Each kv As KeyValuePair(Of String, Integer) In freqs
                    _freqs(kv.Key) = kv.Value
                Next
            End If
        End Sub

        ''' <summary>
        ''' 从语料行构建词表。每一行既可以是用空格分隔好的 token 序列
        ''' （<c>build_corpus.py</c> 的产物），也可以是原始 SMILES 文本。
        ''' </summary>
        ''' <param name="lines">语料行集合。</param>
        ''' <param name="maxSize">词表容量上限（不含特殊 token）。</param>
        ''' <param name="minFreq">最低出现次数。</param>
        Public Shared Function FromCorpus(lines As IEnumerable(Of String),
                                           Optional maxSize As Integer? = Nothing,
                                           Optional minFreq As Integer = 1) As WordVocab
            If lines Is Nothing Then Throw New ArgumentNullException(NameOf(lines))

            Dim counter As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For Each line As String In lines
                For Each token As String In TokenizeLine(line)
                    Dim count As Integer = 0

                    counter.TryGetValue(token, count)
                    counter(token) = count + 1
                Next
            Next

            Return New WordVocab(counter, maxSize, minFreq)
        End Function

        ''' <summary>
        ''' 把一行语料拆成 token：含空白的行按空白切分，否则按 SMILES 语法分词。
        ''' </summary>
        ''' <param name="line">语料行。</param>
        Public Shared Function TokenizeLine(line As String) As String()
            If String.IsNullOrWhiteSpace(line) Then Return New String() {}

            Dim trimmed As String = line.Trim()
            Dim parts As String() = trimmed.Split(New Char() {" "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)

            If parts.Length > 1 Then Return parts

            Return SmilesTokenizer.Split(trimmed)
        End Function

        ''' <summary>词表是否包含指定 token。</summary>
        ''' <param name="token">待查询的 token。</param>
        Public Function ContainsKey(token As String) As Boolean
            Return token IsNot Nothing AndAlso _stoi.ContainsKey(token)
        End Function

        ''' <summary>取 token 的下标；未登录时返回 <see cref="UnkIndex"/>。</summary>
        ''' <param name="token">待查询的 token。</param>
        Public Function IndexOf(token As String) As Integer
            Dim index As Integer = 0

            If token IsNot Nothing AndAlso _stoi.TryGetValue(token, index) Then Return index

            Return UnkIndex
        End Function

        ''' <summary>
        ''' 把 token 序列编码为整数序列（可选的 <c>&lt;sos&gt;</c>/<c>&lt;eos&gt;</c> 包裹与定长填充）。
        ''' </summary>
        ''' <param name="tokens">token 序列。</param>
        ''' <param name="seqLen">定长；<c>Nothing</c> 表示不填充/不截断。</param>
        ''' <param name="withEos">是否在尾部追加 <c>&lt;eos&gt;</c>。</param>
        ''' <param name="withSos">是否在头部插入 <c>&lt;sos&gt;</c>。</param>
        Public Function ToSeq(tokens As IEnumerable(Of String),
                              Optional seqLen As Integer? = Nothing,
                              Optional withEos As Boolean = False,
                              Optional withSos As Boolean = False) As Integer()
            Dim originLength As Integer = 0

            Return ToSeq(tokens, seqLen, withEos, withSos, originLength)
        End Function

        ''' <summary>
        ''' 把 token 序列编码为整数序列，并返回包裹特殊 token 之后的原始长度。
        ''' </summary>
        ''' <param name="tokens">token 序列。</param>
        ''' <param name="seqLen">定长；<c>Nothing</c> 表示不填充/不截断。</param>
        ''' <param name="withEos">是否在尾部追加 <c>&lt;eos&gt;</c>。</param>
        ''' <param name="withSos">是否在头部插入 <c>&lt;sos&gt;</c>。</param>
        ''' <param name="originLength">填充/截断之前的长度（含 <c>&lt;sos&gt;</c>/<c>&lt;eos&gt;</c>）。</param>
        Public Function ToSeq(tokens As IEnumerable(Of String),
                              seqLen As Integer?,
                              withEos As Boolean,
                              withSos As Boolean,
                              ByRef originLength As Integer) As Integer()
            If tokens Is Nothing Then Throw New ArgumentNullException(NameOf(tokens))

            Dim seq As New List(Of Integer)

            If withSos Then seq.Add(SosIndex)

            For Each token As String In tokens
                seq.Add(IndexOf(token))
            Next

            If withEos Then seq.Add(EosIndex)

            originLength = seq.Count

            If seqLen.HasValue Then
                If seq.Count <= seqLen.Value Then
                    While seq.Count < seqLen.Value
                        seq.Add(PadIndex)
                    End While
                Else
                    seq.RemoveRange(seqLen.Value, seq.Count - seqLen.Value)
                End If
            End If

            Return seq.ToArray()
        End Function

        ''' <summary>
        ''' 把整数序列解码为 token 序列。
        ''' </summary>
        ''' <param name="ids">整数序列。</param>
        ''' <param name="withPad">是否保留 <c>&lt;pad&gt;</c>。</param>
        Public Function FromSeq(ids As Integer(), Optional withPad As Boolean = False) As String()
            If ids Is Nothing Then Return New String() {}

            Dim tokens As New List(Of String)

            For Each id As Integer In ids
                If Not withPad AndAlso id = PadIndex Then Continue For

                tokens.Add(If(id >= 0 AndAlso id < _itos.Count, _itos(id), $"<{id}>"))
            Next

            Return tokens.ToArray()
        End Function

        ''' <summary>把词表保存为 JSON（token 顺序 + 频次）。</summary>
        ''' <param name="path">目标文件。</param>
        Public Sub SaveJson(path As String)
            Dim model As New VocabFileModel With {
                .Version = 1,
                .Tokens = _itos.ToArray(),
                .Freqs = New Dictionary(Of String, Integer)(_freqs, StringComparer.Ordinal)
            }

            Dim json As String = JsonSerializer.Serialize(model, newJsonOptions())

            Call File.WriteAllText(path, json, Encoding.UTF8)
        End Sub

        ''' <summary>从 JSON 还原词表（保持 token 顺序）。</summary>
        ''' <param name="path">词表文件。</param>
        Public Shared Function LoadJson(path As String) As WordVocab
            If Not File.Exists(path) Then Throw New FileNotFoundException($"vocabulary file not found: {path}", path)

            Dim model As VocabFileModel = JsonSerializer.Deserialize(Of VocabFileModel)(File.ReadAllText(path), newJsonOptions())

            If model Is Nothing OrElse model.Tokens Is Nothing OrElse model.Tokens.Length = 0 Then
                Throw New InvalidDataException($"invalid vocabulary file: {path}")
            End If

            If model.Tokens.Length < Specials.Length Then
                Throw New InvalidDataException($"vocabulary file '{path}' does not start with the 5 special tokens")
            End If

            For i As Integer = 0 To Specials.Length - 1
                If Not String.Equals(model.Tokens(i), Specials(i), StringComparison.Ordinal) Then
                    Throw New InvalidDataException(
                        $"vocabulary file '{path}' is incompatible: token #{i} is '{model.Tokens(i)}', expected '{Specials(i)}'")
                End If
            Next

            Return New WordVocab(model.Tokens, model.Freqs)
        End Function

        Private Shared Function newJsonOptions() As JsonSerializerOptions
            Return New JsonSerializerOptions With {
                .WriteIndented = True,
                .PropertyNameCaseInsensitive = True
            }
        End Function

        ''' <summary>词表摘要（大小与最高频 token）。</summary>
        Public Overrides Function ToString() As String
            Dim top As String = String.Join(", ",
                _freqs.
                    Where(Function(kv) Not _stoi.ContainsKey(kv.Key) OrElse _stoi(kv.Key) >= Specials.Length).
                    OrderByDescending(Function(kv) kv.Value).
                    ThenBy(Function(kv) kv.Key, StringComparer.Ordinal).
                    Take(5).
                    Select(Function(kv) $"{kv.Key}:{kv.Value}"))

            Return $"vocab[size={Count}] {top}"
        End Function

    End Class

    ''' <summary>词表 JSON 的落盘模型。</summary>
    Friend Class VocabFileModel

        ''' <summary>文件格式版本。</summary>
        Public Property Version As Integer = 1

        ''' <summary>按下标排列的 token 列表。</summary>
        Public Property Tokens As String()

        ''' <summary>token → 频次。</summary>
        Public Property Freqs As Dictionary(Of String, Integer)

    End Class

End Namespace
