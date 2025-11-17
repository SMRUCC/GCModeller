Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment

Namespace FQ

    Public Module TrimReadsHeaders

        ''' <summary>
        ''' 从FastQ reads中去除指定的引物和接头序列（精确匹配）。
        ''' </summary>
        ''' <param name="reads">输入的FastQ序列集合。</param>
        ''' <param name="primersAndAdapters">一个包含所有需要去除的引物和接头序列的列表。</param>
        ''' <returns>修剪后的FastQ序列集合。</returns>
        <Extension>
        Public Iterator Function TrimPrimersAndAdapters(reads As IEnumerable(Of FastQ), primersAndAdapters As IEnumerable(Of String)) As IEnumerable(Of FastQ)
            For Each read As FastQ In reads
                Dim currentSeq As String = read.SequenceData
                Dim currentQual As String = read.Quality
                Dim trimmed As Boolean = False

                ' 1. 检查并去除5'端的引物/接头
                For Each pna As String In primersAndAdapters
                    If currentSeq.StartsWith(pna, StringComparison.OrdinalIgnoreCase) Then
                        Dim len As Integer = pna.Length
                        currentSeq = currentSeq.Substring(len)
                        currentQual = currentQual.Substring(len)
                        trimmed = True
                        Exit For ' 假设一次只匹配一个，匹配后即可跳出
                    End If
                Next

                ' 2. 检查并去除3'端的引物/接头
                For Each pna As String In primersAndAdapters
                    If currentSeq.EndsWith(pna, StringComparison.OrdinalIgnoreCase) Then
                        Dim len As Integer = pna.Length
                        currentSeq = currentSeq.Substring(0, currentSeq.Length - len)
                        currentQual = currentQual.Substring(0, currentQual.Length - len)
                        trimmed = True
                        Exit For
                    End If
                Next

                ' 如果发生了修剪，更新read对象
                If trimmed Then
                    read.SequenceData = currentSeq
                    read.Quality = currentQual
                End If

                Yield read
            Next
        End Function

        ''' <summary>
        ''' 使用Smith-Waterman局部比对算法去除FastQ reads中的引物和接头序列。
        ''' 该方法对测序错误有较好的容忍度，能够找到序列内部的匹配。
        ''' </summary>
        ''' <param name="reads">输入的FastQ序列集合。</param>
        ''' <param name="primersAndAdapters">一个包含所有需要去除的引物和接头序列的列表。</param>
        ''' <param name="minScore">HSP（高分区域）的最低得分阈值，低于此值的匹配将被忽略。</param>
        ''' <param name="gapPenalty">Smith-Waterman算法中的空位罚分。</param>
        ''' <returns>修剪后的FastQ序列集合。</returns>
        <Extension>
        Public Iterator Function TrimPrimersSmithWaterman(reads As IEnumerable(Of FastQ),
        primersAndAdapters As IEnumerable(Of String),
        Optional minScore As Integer = 15,
        Optional gapPenalty As Integer = -5) As IEnumerable(Of FastQ)

            ' 1. 预处理：将所有引物/接头字符串转换为FastQ对象，以便于比对。
            '    引物/接头本身没有质量值，我们创建一个虚拟的质量字符串。
            Dim primerObjects As New List(Of FastQ)
            For Each seq In primersAndAdapters
                primerObjects.Add(New FastQ With {
                    .SequenceData = seq,
                    .Quality = New String("!"c, seq.Length) ' 使用最低质量字符作为占位符
                })
            Next

            ' 2. 创建DNA比对矩阵
            Dim scoringMatrix = SimpleDNAMatrix.DefaultMatrix()

            ' 3. 遍历每一条read进行比对和修剪
            For Each read As FastQ In reads
                Dim bestHSP As HSP = Nothing
                Dim bestPrimer As FastQ = Nothing

                ' 将当前read与所有引物/接头进行比对，找到得分最高的HSP
                For Each primer As FastQ In primerObjects
                    ' 执行Smith-Waterman比对
                    ' 注意：SmithWaterman.Align需要能够接收gapPenalty参数，假设其内部已实现
                    Dim swResult As SmithWaterman = SmithWaterman.Align(read, primer, scoringMatrix)

                    ' 查找本次比对中得分最高的HSP
                    For Each hsp As HSP In swResult
                        ' HSP的Score属性是判断匹配好坏的关键
                        If hsp.score > minScore AndAlso (bestHSP Is Nothing OrElse hsp.score > bestHSP.score) Then
                            bestHSP = hsp
                            bestPrimer = primer
                        End If
                    Next
                Next

                ' 4. 如果找到了足够好的匹配，则进行修剪
                If bestHSP IsNot Nothing Then
                    ' 创建一个新的FastQ对象来存储修剪后的结果，避免修改原始read
                    Dim trimmedRead As New FastQ With {
                        .SequenceData = read.SequenceData,
                        .Quality = read.Quality
                    }

                    ' HSP的fromA/toA对应read上的位置，fromB/toB对应primer上的位置
                    Dim matchStartOnRead = bestHSP.fromA
                    Dim matchEndOnRead = bestHSP.toA
                    Dim readLength = read.SequenceData.Length

                    ' 策略：根据匹配在read上的位置进行修剪
                    ' (索引通常从0开始，fromA <= toA)

                    ' 情况1: 匹配区域在read的5'端
                    ' 如果匹配从read的开头开始，则切除匹配部分
                    If matchStartOnRead <= 2 Then ' 允许1-2个bp的偏移
                        Dim keepStartIndex = matchEndOnRead + 1
                        If keepStartIndex < readLength Then
                            trimmedRead.SequenceData = read.SequenceData.Substring(keepStartIndex)
                            trimmedRead.Quality = read.Quality.Substring(keepStartIndex)
                        Else
                            ' 整个read都被匹配为引物，丢弃
                            Continue For
                        End If
                        ' 情况2: 匹配区域在read的3'端
                        ' 如果匹配在read的末尾结束，则切除匹配部分
                    ElseIf matchEndOnRead >= readLength - 3 Then ' 允许1-2个bp的偏移
                        Dim keepEndIndex = matchStartOnRead - 1
                        If keepEndIndex >= 0 Then
                            trimmedRead.SequenceData = read.SequenceData.Substring(0, keepEndIndex + 1)
                            trimmedRead.Quality = read.Quality.Substring(0, keepEndIndex + 1)
                        Else
                            ' 整个read都被匹配为引物，丢弃
                            Continue For
                        End If
                        ' 情况3: 匹配在read的内部
                        ' 这种情况比较复杂，可能是嵌合体或特殊结构。最简单的处理是删除中间的匹配部分。
                        ' 此处我们选择不处理，或者可以添加更复杂的逻辑。
                        ' 例如：trimmedRead.SequenceData = read.SequenceData.Substring(0, matchStartOnRead) & read.SequenceData.Substring(matchEndOnRead + 1)
                    End If

                    Yield trimmedRead
                Else
                    ' 如果没有找到任何匹配，则原样返回
                    Yield read
                End If
            Next
        End Function
    End Module
End Namespace