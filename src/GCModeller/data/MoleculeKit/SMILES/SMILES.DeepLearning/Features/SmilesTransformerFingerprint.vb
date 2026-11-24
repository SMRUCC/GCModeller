Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.Transformer
Imports SMRUCC.genomics.Molecule.SMILES.DeepLearning.Corpus
Imports SMRUCC.genomics.Molecule.SMILES.SmilesSyntax

Namespace Features

    ''' <summary>
    ''' 一组分子的指纹矩阵（行 = 分子，列 = 特征）。
    ''' </summary>
    Public Class SmilesFingerprint

        ''' <summary>分子标识（原始 SMILES）。</summary>
        Public Property Titles As String()

        ''' <summary>特征矩阵，形状 <c>[分子数, 特征数]</c>。</summary>
        Public Property Matrix As Double(,)

        ''' <summary>特征列数（= 4 × hiddenSize）。</summary>
        Public ReadOnly Property Width As Integer
            Get
                Return If(Matrix Is Nothing, 0, Matrix.GetLength(1))
            End Get
        End Property

        ''' <summary>分子数。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return If(Matrix Is Nothing, 0, Matrix.GetLength(0))
            End Get
        End Property

        ''' <summary>列名（<c>mean_0.., max_0.., first_0.., penul_0..</c>）。</summary>
        Public Function ColumnNames() As String()
            Dim half As Integer = Width \ 4
            Dim names(Width - 1) As String

            For i As Integer = 0 To half - 1
                names(i) = $"mean_{i}"
                names(half + i) = $"max_{i}"
                names(half * 2 + i) = $"first_{i}"
                names(half * 3 + i) = $"penul_{i}"
            Next

            Return names
        End Function

        ''' <summary>
        ''' 导出为 CSV（首列是 SMILES，其余是特征），便于下游 RanFog / Logistic 使用。
        ''' </summary>
        ''' <param name="path">目标文件。</param>
        ''' <param name="delimiter">分隔符。</param>
        Public Sub SaveCsv(path As String, Optional delimiter As Char = ","c)
            Dim columns As String() = ColumnNames()
            Dim sb As New StringBuilder

            sb.Append("smiles").Append(delimiter).AppendLine(String.Join(delimiter.ToString(), columns))

            For r As Integer = 0 To Count - 1
                sb.Append(escape(Titles(r), delimiter)).Append(delimiter)

                For c As Integer = 0 To Width - 1
                    If c > 0 Then sb.Append(delimiter)

                    sb.Append(Matrix(r, c).ToString("G9", Globalization.CultureInfo.InvariantCulture))
                Next

                sb.AppendLine()
            Next

            Call File.WriteAllText(path, sb.ToString(), Encoding.UTF8)
        End Sub

        Private Shared Function escape(value As String, delimiter As Char) As String
            If value Is Nothing Then Return String.Empty

            If value.IndexOf(delimiter) >= 0 OrElse value.IndexOf(""""c) >= 0 OrElse
               value.IndexOf(ControlChars.Lf) >= 0 Then
                Return """" & value.Replace("""", """""") & """"
            End If

            Return value
        End Function

    End Class

    ''' <summary>
    ''' 用训练好的 <see cref="SequenceTransformer"/> 为分子生成 4H 维指纹。
    ''' </summary>
    ''' <remarks>
    ''' 对应 Python 的 <c>TrfmSeq2seq.encode()</c>：
    ''' <c>hstack([mean, max, first, 倒数第二层 first])</c>，维度为 <c>4 × hiddenSize</c>
    ''' （论文配置下是 1024 维）。
    ''' </remarks>
    Public Class SmilesTransformerFingerprint

        Private ReadOnly _model As SequenceTransformer
        Private ReadOnly _vocab As WordVocab
        Private ReadOnly _seqLen As Integer

        ''' <summary>指纹维度（= 4 × hiddenSize）。</summary>
        Public ReadOnly Property Dimension As Integer
            Get
                Return _model.HiddenSize * 4
            End Get
        End Property

        ''' <summary>
        ''' 创建指纹提取器。
        ''' </summary>
        ''' <param name="model">已加载权重（或已训练）的模型。</param>
        ''' <param name="vocab">与模型配套的词表。</param>
        ''' <param name="seqLen">定长序列长度，必须与模型的位置编码容量一致。</param>
        Public Sub New(model As SequenceTransformer, vocab As WordVocab, Optional seqLen As Integer = 220)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            If vocab Is Nothing Then Throw New ArgumentNullException(NameOf(vocab))
            If seqLen > model.SequenceLength Then
                Throw New ArgumentException($"seqLen({seqLen}) 超过模型的序列长度上限({model.SequenceLength})")
            End If

            _model = model
            _vocab = vocab
            _seqLen = seqLen
        End Sub

        ''' <summary>
        ''' 为一批 SMILES 生成指纹矩阵。
        ''' </summary>
        ''' <param name="smiles">SMILES 集合（解析失败或编码后过短的样本会得到全 0 行）。</param>
        ''' <param name="batchSize">内部批大小（控制内存峰值）。</param>
        Public Function Encode(smiles As IEnumerable(Of String), Optional batchSize As Integer = 64) As SmilesFingerprint
            If smiles Is Nothing Then Throw New ArgumentNullException(NameOf(smiles))

            Dim list As List(Of String) = smiles.ToList()
            Dim result As New SmilesFingerprint With {
                .Titles = list.ToArray()
            }

            If list.Count = 0 Then
                Dim empty(-1, -1) As Double

                result.Matrix = empty

                Return result
            End If

            Dim matrix(list.Count - 1, Dimension - 1) As Double
            Dim start As Integer = 0

            While start < list.Count
                Dim count As Integer = Math.Min(batchSize, list.Count - start)
                Dim ids(count - 1, _seqLen - 1) As Integer

                For i As Integer = 0 To count - 1
                    Dim seq As Integer() = encodeOne(list(start + i))

                    For t As Integer = 0 To _seqLen - 1
                        ids(i, t) = seq(t)
                    Next
                Next

                Dim encoded As Double(,) = _model.Encode(ids)

                For r As Integer = 0 To count - 1
                    For c As Integer = 0 To Dimension - 1
                        matrix(start + r, c) = encoded(r, c)
                    Next
                Next

                start += count
            End While

            result.Matrix = matrix

            Return result
        End Function

        ''' <summary>把单条 SMILES 编码为定长下标序列（含 <c>&lt;sos&gt;</c>/<c>&lt;eos&gt;</c> 与 padding）。</summary>
        ''' <param name="smiles">SMILES 文本。</param>
        Public Function encodeOne(smiles As String) As Integer()
            Dim tokens As String() = SmilesTokenizer.Split(smiles)
            Dim originLength As Integer = 0

            Return _vocab.ToSeq(tokens, _seqLen, withEos:=True, withSos:=True, originLength:=originLength)
        End Function

    End Class

End Namespace
