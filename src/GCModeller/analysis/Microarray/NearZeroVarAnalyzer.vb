Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Module NearZeroVarAnalyzer

    ''' <summary>
    ''' 识别数据集中的近零方差变量（列）。
    ''' 逻辑等同于 R 语言 mixOmics::nearZeroVar 或 caret::nearZeroVar
    ''' </summary>
    ''' <param name="data">列代表样本，行代表变量</param>
    ''' <param name="freqCut">频数比阈值，默认 19.0</param>
    ''' <param name="uniqueCut">唯一值百分比阈值，默认 10.0</param>
    ''' <returns>包含近零方差的基因ID</returns>
    ''' 
    <Extension>
    Public Iterator Function FindNearZeroVarColumns(data As Matrix,
                                           Optional freqCut As Double = 19.0,
                                           Optional uniqueCut As Double = 10.0) As IEnumerable(Of String)

    End Function

    ''' <summary>
    ''' 识别数据集中的近零方差变量（列）。
    ''' 逻辑等同于 R 语言 mixOmics::nearZeroVar 或 caret::nearZeroVar
    ''' </summary>
    ''' <param name="data">二维双精度浮点数组，行代表样本，列代表变量</param>
    ''' <param name="freqCut">频数比阈值，默认 19.0</param>
    ''' <param name="uniqueCut">唯一值百分比阈值，默认 10.0</param>
    ''' <returns>包含近零方差列的索引列表（从0开始）</returns>
    Public Function FindNearZeroVarColumns(data As Double(,),
                                           Optional freqCut As Double = 19.0,
                                           Optional uniqueCut As Double = 10.0) As List(Of Integer)

        Dim nzvColumns As New List(Of Integer)

        If data Is Nothing OrElse data.Length = 0 Then
            Return nzvColumns
        End If

        Dim rowCount As Integer = data.GetLength(0)
        Dim colCount As Integer = data.GetLength(1)

        ' 逐列计算
        For col As Integer = 0 To colCount - 1

            ' 1. 统计该列每个值出现的频数
            Dim freqDict As New Dictionary(Of Double, Integer)()
            Dim validRowCount As Integer = 0

            For row As Integer = 0 To rowCount - 1
                Dim val As Double = data(row, col)

                ' 忽略 NaN 值 (可选，根据数据清洁度决定是否保留此逻辑)
                If Double.IsNaN(val) Then Continue For

                validRowCount += 1
                If freqDict.ContainsKey(val) Then
                    freqDict(val) += 1
                Else
                    freqDict.Add(val, 1)
                End If
            Next

            ' 如果该列全为 NaN 或无有效数据，则跳过
            If validRowCount = 0 Then Continue For

            ' 2. 计算唯一值百分比
            Dim uniqueCount As Integer = freqDict.Count
            Dim uniquePercent As Double = (uniqueCount / validRowCount) * 100.0

            ' 3. 计算频数比
            Dim freqRatio As Double = Double.PositiveInfinity ' 默认无穷大（应对只有一个唯一值的情况）

            If uniqueCount > 1 Then
                ' 找出最高频次和第二高频次 (不使用 LINQ，使用基础遍历以提高兼容性和速度)
                Dim max1 As Integer = 0
                Dim max2 As Integer = 0

                For Each freq As Integer In freqDict.Values
                    If freq > max1 Then
                        max2 = max1
                        max1 = freq
                    ElseIf freq > max2 Then
                        max2 = freq
                    End If
                Next

                ' 防御性编程：如果第二高频次为0（理论上不会发生，因为 uniqueCount > 1）
                If max2 > 0 Then
                    freqRatio = max1 / max2
                End If
            End If

            ' 4. 判断是否满足近零方差条件
            ' 条件：频数比 > freqCut 且 唯一值百分比 < uniqueCut
            If freqRatio > freqCut AndAlso uniquePercent < uniqueCut Then
                nzvColumns.Add(col)
            End If

        Next

        Return nzvColumns
    End Function

End Module