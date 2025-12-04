Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' Module code for generates reads for run algorithm debug test
''' </summary>
Public Module ReadsFakeSource

    ''' <summary>
    ''' 基于FASTA序列集合，模拟生成具有指定长度分布的随机测序reads。
    ''' 此函数模拟第三代长读长测序的随机断裂过程。
    ''' </summary>
    ''' <param name="genomes">包含所有参考基因组序列的FastaSeq数组。</param>
    ''' <param name="nreads">需要生成的reads总数。</param>
    ''' <param name="len">一个元组，指定模拟reads的最小和最大长度。</param>
    ''' <returns>一个IEnumerable(Of String)，通过迭代器模式逐个返回模拟的read序列。</returns>
    Public Iterator Function FakeReads(genomes As FastaSeq(), nreads As Integer, len As DoubleRange) As IEnumerable(Of String)
        ' --- 参数验证 ---
        If genomes Is Nothing OrElse genomes.Length = 0 Then
            Throw New ArgumentException("输入的基因组数组不能为空。", NameOf(genomes))
        End If
        If nreads <= 0 Then
            ' 如果nreads为0或负数，直接返回，不产生任何输出
            Return
        End If
        If len.Min <= 0 OrElse len.Max < len.Min Then
            Throw New ArgumentException("长度范围无效。min必须大于0，且max必须大于等于min。", NameOf(len))
        End If

        ' 过滤掉长度为0的无效基因组，防止后续出错
        Dim validGenomes = genomes.Where(Function(g) Not String.IsNullOrEmpty(g.SequenceData)).ToArray()
        If validGenomes.Length = 0 Then
            Throw New ArgumentException("所有提供的基因组序列都为空，无法生成reads。", NameOf(genomes))
        End If

        Dim lenMin As Integer = len.Min
        Dim lenMax As Integer = len.Max

        ' --- 循环生成指定数量的reads ---
        For i As Integer = 1 To nreads
            ' 1. 随机选择一个基因组
            Dim selectedGenome As FastaSeq = validGenomes(randf.Next(validGenomes.Length))
            Dim sequence As String = selectedGenome.SequenceData

            ' 2. 随机确定read的长度
            ' rand.Next(min, maxExclusive) -> 为了包含max，需要+1
            Dim readLength As Integer = randf.NextInteger(lenMin, lenMax + 1)

            ' 3. 处理边界情况：如果选中的基因组比期望的read长度还短
            If sequence.Length < readLength Then
                ' 策略：将read长度调整为整个基因组的长度，模拟一个完整的短序列被测出
                readLength = sequence.Length
            End If

            ' 如果调整后长度为0，则跳过此次生成（例如，一个空序列被错误地包含进来）
            If readLength = 0 Then
                Continue For
            End If

            ' 4. 随机选择起始位置
            ' 起始位置的最大值是 序列长度 - read长度
            Dim maxStartIndex As Integer = sequence.Length - readLength
            Dim startIndex As Integer = randf.NextInteger(0, maxStartIndex + 1)

            ' 5. 截取子序列作为模拟的read
            Dim simulatedRead As String = sequence.Substring(startIndex, readLength)

            ' 6. 使用Yield返回当前生成的read
            Yield simulatedRead
        Next
    End Function
End Module
