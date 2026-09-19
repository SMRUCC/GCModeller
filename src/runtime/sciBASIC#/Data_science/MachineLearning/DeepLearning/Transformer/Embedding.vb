' ---------------------------------------------------------------------------
' Embedding —— 词嵌入层（迁移到 TensorFlow\Tensor.vb + 手写反向传播）
'
' 前向：按 one-hot 索引把 embeddingLayer 的对应行拷贝到 [batch, seq, emb]，
'       再叠加正弦位置编码，训练时按最后一维做 dropout。
' 反向：穿过 dropout 之后，把每个 (sentence, position) 的梯度散射累加回
'       embeddingLayer 对应行（同一词出现多次会自然累加）。
' ---------------------------------------------------------------------------

Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports std = System.Math

Namespace Transformer

    ''' <summary>
    ''' Use a learned embedding layer to reduce the size of the word embedding space.
    ''' </summary>
    Public Class Embedding

        Private _EmbeddingSize As Integer, _SequenceLength As Integer
        Private allWords As List(Of String) = New List(Of String)()
        Private one_hot As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)()
        Private dropoutMask As Boolean()
        Private dropoutRate As Double = 0

        ''' <summary>
        ''' Learned linear embedding layer
        ''' </summary>
        Private embeddingLayer As Tensor

        Private embeddingLayerOptimizer As Optimizer

        Public ReadOnly Property DictionarySize As Integer
            Get
                Return one_hot.Count
            End Get
        End Property

        Public Property EmbeddingSize As Integer
            Get
                Return _EmbeddingSize
            End Get
            Private Set(value As Integer)
                _EmbeddingSize = value
            End Set
        End Property

        Public Property SequenceLength As Integer
            Get
                Return _SequenceLength
            End Get
            Private Set(value As Integer)
                _SequenceLength = value
            End Set
        End Property

        ''' <summary>
        ''' 与 <see cref="embeddingLayer"/> 同形的梯度累加器。
        ''' </summary>
        Friend ReadOnly Property Parameters As Tensor
            Get
                Return embeddingLayer
            End Get
        End Property

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="embeddingSize"></param>
        ''' <param name="sequenceLength"></param>
        ''' <param name="sentences"></param>
        Public Sub New(embeddingSize As Integer, sequenceLength As Integer, sentences As List(Of List(Of String)))
            Me.EmbeddingSize = embeddingSize
            Me.SequenceLength = sequenceLength

            Call OneHotEmbedding(sentences)

            embeddingLayer = TensorOps.HeNormalInit(New Integer() {DictionarySize, Me.EmbeddingSize})
            embeddingLayerOptimizer = New Optimizer(embeddingLayer)

            dropoutMask = New Boolean(embeddingSize - 1) {}
        End Sub

        ''' <summary>
        ''' Multiply the one-hot embeddings with the embedding layer to project onto a smaller space
        ''' </summary>
        ''' <param name="sentences"></param>
        ''' <param name="isTraining"></param>
        ''' <returns></returns>
        Public Function Embed(sentences As List(Of List(Of String)), isTraining As Boolean) As Tensor
            Dim batchSize = sentences.Count
            Dim wordEmbeddings As Tensor = New Tensor(batchSize, SequenceLength, EmbeddingSize)
            Dim emb = wordEmbeddings.Data
            Dim layer = embeddingLayer.Data
            Dim n = EmbeddingSize
            Dim s = 0

            For Each sentence In sentences
                Dim word_count = 0

                For Each word In sentence
                    ' No need for matrix multiplication since only one element of vector is nonzero
                    Dim pos As Integer = one_hot(word.ToLower())
                    Array.Copy(layer, pos * n, emb, (s * SequenceLength + word_count) * n, n)
                    word_count += 1
                Next

                Call AddPositionalEncoding(wordEmbeddings, s, sentence.Count())
                s += 1
            Next

            Call wordEmbeddings.MarkHostModified()

            If isTraining AndAlso dropoutRate > 0 Then wordEmbeddings = TensorOps.DropoutMask(wordEmbeddings, dropoutMask, dropoutRate)

            Return wordEmbeddings
        End Function

        ''' <summary>
        ''' 嵌入层的反向传播：把每个 (句, 位置) 的梯度散射累加回 embeddingLayer 的对应行。
        ''' </summary>
        ''' <param name="dWordEmbeddings">对 <see cref="Embed"/> 输出的梯度，形状 [batch, seq, emb]</param>
        ''' <param name="sentences">前向阶段使用的句子（用于还原词索引）</param>
        ''' <param name="applyDropout">前向阶段是否应用过 dropout</param>
        Public Sub Backward(dWordEmbeddings As Tensor, sentences As List(Of List(Of String)), applyDropout As Boolean)
            Dim dOut = dWordEmbeddings

            If applyDropout AndAlso dropoutRate > 0 Then
                dOut = TensorOps.DropoutMaskBackward(dOut, dropoutMask, dropoutRate)
            End If

            Dim grad = embeddingLayerOptimizer.Gradient.Data
            Dim dIn = dOut.Data
            Dim n = EmbeddingSize
            Dim s = 0

            For Each sentence In sentences
                Dim word_count = 0

                For Each word In sentence
                    Dim pos As Integer = one_hot(word.ToLower())
                    Dim src = (s * SequenceLength + word_count) * n
                    Dim dst = pos * n

                    For i As Integer = 0 To n - 1
                        grad(dst + i) += dIn(src + i)
                    Next

                    word_count += 1
                Next

                s += 1
            Next

            Call embeddingLayerOptimizer.Gradient.MarkHostModified()
        End Sub

        ''' <summary>
        ''' 交叉熵损失及其对输出层 logits 的梯度。
        ''' </summary>
        ''' <remarks>
        ''' 对 softmax + 交叉熵，直接给出 <c>d(logits) = softmax − onehot</c>，
        ''' 避免对 log / softmax 做链式求导，与仓库既有 GNN 训练器写法一致。
        ''' 返回的是本步「未缩放」的损失贡献，最终由调用方统一除以 <c>sequenceLength * batchSize</c>。
        ''' </remarks>
        ''' <param name="filteredOutput">输出层 softmax 概率，形状 [batch, 1, dictSize]</param>
        ''' <param name="correctSentences">正确译文（目标语言）</param>
        ''' <param name="w">当前预测的词位置</param>
        ''' <param name="dLogits">输出参数：对 logits 的梯度，形状同 <paramref name="filteredOutput"/></param>
        Public Function CalculateLossAndGradient(filteredOutput As Tensor,
                                                 correctSentences As List(Of List(Of String)),
                                                 w As Integer,
                                                 ByRef dLogits As Tensor) As Double
            Dim dictSize = filteredOutput.Shape(filteredOutput.Rank - 1)
            Dim batchSize = filteredOutput.Shape(0)

            dLogits = New Tensor(filteredOutput.Shape)

            Dim p = filteredOutput.Data
            Dim g = dLogits.Data
            Dim total As Double = 0.0

            For s = 0 To correctSentences.Count() - 1
                If w >= correctSentences(s).Count() Then Continue For

                Dim ind = GetWordIndex(correctSentences(s)(w))
                Dim baseIdx = s * dictSize

                For j = 0 To dictSize - 1
                    g(baseIdx + j) = p(baseIdx + j)
                Next

                g(baseIdx + ind) -= 1.0
                total -= std.Log(std.Max(p(baseIdx + ind), 1.0E-12))
            Next

            Call dLogits.MarkHostModified()

            Return total
        End Function

        ''' <summary>
        ''' Get a word based on its index in the dictionary
        ''' </summary>
        ''' <param name="indexes"></param>
        ''' <returns></returns>
        Public Function GetWords(indexes As Integer()) As String()
            Dim words = New String(indexes.Length - 1) {}
            For s = 0 To indexes.Length - 1
                words(s) = allWords(indexes(s))
            Next

            Return words
        End Function

        ''' <summary>
        ''' Get the index of a specific word in a dictionary
        ''' </summary>
        ''' <param name="word"></param>
        ''' <returns></returns>
        Public Function GetWordIndex(word As String) As Integer
            Return one_hot(word)
        End Function

        Public Function AllWordsInDictionary(sentences As List(Of List(Of String)), <Out> ByRef wordNotInDictionary As String) As Boolean
            wordNotInDictionary = ""

            For Each sentence In sentences
                For Each w In sentence
                    If Not one_hot.ContainsKey(w) Then
                        wordNotInDictionary = w
                        Return False
                    End If
                Next
            Next

            Return True
        End Function

        ''' <summary>
        ''' Encode all words in a dictionary with one-hot embedding
        ''' </summary>
        ''' <param name="sentences"></param>
        Private Sub OneHotEmbedding(sentences As List(Of List(Of String)))
            Dim word_index = 0
            For Each sentence In sentences
                For Each word In sentence
                    If Not one_hot.ContainsKey(word.ToLower()) Then
                        allWords.Add(word.ToLower())
                        one_hot.Add(word.ToLower(), word_index)
                        word_index += 1
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' Add positional encoding to embedded words according to "Attention is all you need"
        ''' </summary>
        ''' <param name="wordEmbeddings"></param>
        ''' <param name="s"></param>
        ''' <param name="sentenceLength"></param>
        Private Sub AddPositionalEncoding(wordEmbeddings As Tensor, s As Integer, sentenceLength As Integer)
            Dim data = wordEmbeddings.Data
            Dim n = EmbeddingSize
            Dim baseIdx = s * SequenceLength * n

            For pos = 0 To sentenceLength - 1
                Dim posBase = baseIdx + pos * n

                For i = 0 To EmbeddingSize - 1
                    Dim pe As Double
                    If i Mod 2 = 0 Then
                        pe = std.Sin(pos / std.Pow(10000, i / EmbeddingSize))
                    Else
                        pe = std.Cos(pos / std.Pow(10000, (i - 1) / EmbeddingSize))
                    End If
                    data(posBase + i) += pe
                Next
            Next
        End Sub

        Public Sub SetDropoutNodes(dropoutRate As Double)
            If dropoutRate < 0 OrElse dropoutRate >= 1 Then Throw New ArgumentException("Error: dropout rate must be >= 0 and < 1")

            Me.dropoutRate = dropoutRate

            For i = 0 To EmbeddingSize - 1
                dropoutMask(i) = False
                If randf.NextDouble < dropoutRate Then dropoutMask(i) = True
            Next
        End Sub

        ''' <summary>清零嵌入层的梯度累加器。</summary>
        Public Sub ZeroGradients()
            embeddingLayerOptimizer.ZeroGrad()
        End Sub

        Public Sub MakeTrainingStep(learningRate As Double, [step] As Integer)
            embeddingLayerOptimizer.MakeTrainingStep(learningRate, [step], embeddingLayer)
        End Sub

    End Class
End Namespace
