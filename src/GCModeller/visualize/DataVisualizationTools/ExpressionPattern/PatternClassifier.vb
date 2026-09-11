#Region "Microsoft.VisualBasic::e4481b123e93189c13ec211bbe702727, visualize\DataVisualizationExtensions\ExpressionPattern\PatternClassifier.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 136
    '    Code Lines: 94 (69.12%)
    ' Comment Lines: 21 (15.44%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 21 (15.44%)
    '     File Size: 5.70 KB


    ' Module PatternClassifier
    ' 
    '     Function: ClassifyAllGenes, GetPatternName
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Module PatternClassifier

    ' 主函数：对所有的基因进行模式分类，返回基因ID和对应模式标签的字典
    Public Function ClassifyAllGenes(times As String(), genes As IEnumerable(Of NamedCollection(Of Double))) As Dictionary(Of String, String)
        Dim results As New Dictionary(Of String, String)
        For Each gene In genes
            Dim patternName As String = GetPatternName(times, gene.value)
            results.Add(gene.name, patternName)
        Next
        Return results
    End Function

    ' 核心算法：根据 Z-score 序列判断表达模式
    Private Function GetPatternName(times As String(), zexpr As Double()) As String
        If zexpr Is Nothing OrElse zexpr.Length < 2 Then Return "Unknown"
        If zexpr.Length <> times.Length Then Return "Dimension Mis-Matched"

        Dim n = zexpr.Length
        Dim maxVal = zexpr.Max
        Dim minVal = zexpr.Min
        Dim maxIdx = Array.IndexOf(zexpr, maxVal)
        Dim minIdx = Array.IndexOf(zexpr, minVal)
        Dim range = maxVal - minVal

        ' --- 1. 无显著变化 (-类型) ---
        ' 如果整体变化幅度（极差）小于1.0个标准差，认为是平缓的
        Dim flatThresh As Double = 1.0
        If range < flatThresh Then
            Return "Stable expression"
        End If

        ' 单调性容差：允许在上升/下降过程中存在轻微波动（不超过总range的20%仍算单调）
        Dim monoThresh As Double = 0.2 * range

        ' --- 2. 逐渐升高 (/类型) ---
        If minIdx = 0 AndAlso maxIdx = n - 1 Then
            Dim isMonotonicUp As Boolean = True
            For i = 1 To n - 1
                ' 如果出现显著的下降（回落超过容差），则不属于单调升高
                If zexpr(i) < zexpr(i - 1) - monoThresh Then
                    isMonotonicUp = False
                    Exit For
                End If
            Next
            If isMonotonicUp Then Return "Gradually increasing"
        End If

        ' --- 3. 逐渐降低 (\类型) ---
        If maxIdx = 0 AndAlso minIdx = n - 1 Then
            Dim isMonotonicDown As Boolean = True
            For i = 1 To n - 1
                ' 如果出现显著的上升（反弹超过容差），则不属于单调降低
                If zexpr(i) > zexpr(i - 1) + monoThresh Then
                    isMonotonicDown = False
                    Exit For
                End If
            Next
            If isMonotonicDown Then Return "Gradually decreasing"
        End If

        ' 平台/山谷容差：相邻点差距小于总range的25%视为平台或山谷的一部分
        Dim plateauThresh As Double = 0.25 * range

        ' --- 4. 某某时间点最高 或 平台型 (/\ 或 /-\) ---
        If maxIdx > 0 AndAlso maxIdx < n - 1 Then
            Dim pStart As Integer = maxIdx
            Dim pEnd As Integer = maxIdx

            ' 向左寻找高表达平台期起点
            While pStart > 0 AndAlso Math.Abs(zexpr(pStart) - zexpr(pStart - 1)) < plateauThresh
                pStart -= 1
            End While

            ' 向右寻找高表达平台期终点
            While pEnd < n - 1 AndAlso Math.Abs(zexpr(pEnd) - zexpr(pEnd + 1)) < plateauThresh
                pEnd += 1
            End While

            ' 只有当平台左右两端都有下降趋势时，才是真正的 /-\ 平台型，否则只是边缘高
            If pStart > 0 AndAlso pEnd < n - 1 Then
                If pStart <> pEnd Then
                    Return $"Plateau from {times(pStart)} to {times(pEnd)}"
                Else
                    Return $"Peak at {times(maxIdx)}"
                End If
            Else
                Return $"Peak at {times(maxIdx)}"
            End If
        End If

        ' --- 5. 某某时间点最低 或 山谷型 (\/ 或 \_/) ---
        If minIdx > 0 AndAlso minIdx < n - 1 Then
            Dim vStart As Integer = minIdx
            Dim vEnd As Integer = minIdx

            ' 向左寻找低谷起点
            While vStart > 0 AndAlso Math.Abs(zexpr(vStart) - zexpr(vStart - 1)) < plateauThresh
                vStart -= 1
            End While

            ' 向右寻找低谷终点
            While vEnd < n - 1 AndAlso Math.Abs(zexpr(vEnd) - zexpr(vEnd + 1)) < plateauThresh
                vEnd += 1
            End While

            ' 只有当山谷左右两端都有上升趋势时，才是真正的 \_/ 山谷型
            If vStart > 0 AndAlso vEnd < n - 1 Then
                If vStart <> vEnd Then
                    Return $"Valley from {times(vStart)} to {times(vEnd)}"
                Else
                    Return $"Trough at {times(minIdx)}"
                End If
            Else
                Return $"Trough at {times(minIdx)}"
            End If
        End If

        ' --- 6. 边缘极值及兜底复杂波动情况 ---
        ' 如果首尾不是同时为最小/最大值，且中间没有绝对的极值
        If maxIdx = 0 Then Return $"Decreasing after peak at {times(0)}"
        If maxIdx = n - 1 Then Return $"Increasing to peak at {times(n - 1)}"
        If minIdx = 0 Then Return $"Increasing after trough at {times(0)}"
        If minIdx = n - 1 Then Return $"Decreasing to trough at {times(n - 1)}"

        ' 兜底：波动型趋势
        If zexpr(0) < zexpr(n - 1) Then
            Return "Fluctuating increase"
        Else
            Return "Fluctuating decrease"
        End If

    End Function

End Module

