#Region "Metabopolis: treemap partition"

' ============================================================================
' TreeMap 空间填充划分（论文「块内布局 / 层级正交布局」的第一步）
' ----------------------------------------------------------------------------
' 论文用 TreeMap[36] 把一个街区继续划分成若干「建筑块」(building block)，
' 从而在街区内部形成网格状的道路网络，为后续的块内正交布局与车道生成
' 提供几何骨架。基础库中没有现成的 TreeMap 实现，因此这里自行实现：
'
'   * 按权重把条目递归二分（每次找到累计权重最接近一半的切分点）；
'   * 在较长的那条边上按权重比例切分矩形；
'   * 递归下去得到一组互不重叠、正好铺满原矩形的子矩形。
'
' 这种「交替方向 + 权重比例」的切法能同时得到接近正方形的单元格与
' 横平竖直的网格道路，与论文追求的 grid-like road network 一致。
' ============================================================================

Imports System.Drawing
Imports Microsoft.VisualBasic.Linq
Imports Metabopolis.Model

Namespace Routing

    ''' <summary>
    ''' TreeMap 的一个待划分条目。
    ''' </summary>
    Public Class TreeMapItem

        ''' <summary>条目编号。</summary>
        Public Property Id As String

        ''' <summary>划分权重（面积正比于权重）。</summary>
        Public Property Weight As Double

        Public Overrides Function ToString() As String
            Return $"{Id} w={Weight:0.##}"
        End Function

    End Class

    ''' <summary>
    ''' TreeMap 划分后的一个单元格。
    ''' </summary>
    Public Class TreeMapCell

        ''' <summary>对应条目编号。</summary>
        Public Property Id As String

        ''' <summary>单元格矩形。</summary>
        Public Property Rect As Rect

        ''' <summary>单元格面积占原区域的比例。</summary>
        Public Property Weight As Double

        Public Overrides Function ToString() As String
            Return $"{Id} @ {Rect}"
        End Function

    End Class

    ''' <summary>
    ''' 空间填充划分。
    ''' </summary>
    Public Module TreeMapPartition

        ''' <summary>
        ''' 把 <paramref name="area"/> 按权重划分给各个条目。
        ''' </summary>
        ''' <remarks>
        ''' 权重小于等于 0 的条目会被赋予一个极小权重，保证每个条目都能分到面积。
        ''' </remarks>
        Public Function Partition(area As Rect, items As IEnumerable(Of TreeMapItem)) As TreeMapCell()
            Dim list As New List(Of TreeMapItem)()

            For Each item As TreeMapItem In items.SafeQuery
                If item Is Nothing OrElse String.IsNullOrEmpty(item.Id) Then
                    Continue For
                End If

                list.Add(New TreeMapItem With {
                    .Id = item.Id,
                    .Weight = Math.Max(1E-06, item.Weight)
                })
            Next

            Dim result As New List(Of TreeMapCell)()

            If list.Count = 0 OrElse area Is Nothing OrElse area.Width <= 0 OrElse area.Height <= 0 Then
                Return result.ToArray
            End If

            Dim total As Double = list.Sum(Function(i) i.Weight)

            PartitionRecursive(area, list, total, result)

            Return result.ToArray
        End Function

        Private Sub PartitionRecursive(area As Rect,
                                       items As List(Of TreeMapItem),
                                       total As Double,
                                       output As List(Of TreeMapCell))

            If items.Count = 0 Then
                Return
            End If

            If items.Count = 1 Then
                output.Add(New TreeMapCell With {
                    .Id = items(0).Id,
                    .Rect = area,
                    .Weight = items(0).Weight / Math.Max(1E-09, total)
                })

                Return
            End If

            ' 找累计权重最接近一半的切分点
            Dim prefix As Double = 0
            Dim split As Integer = 1
            Dim bestDiff As Double = Double.MaxValue

            For k As Integer = 1 To items.Count - 1
                prefix += items(k - 1).Weight
                Dim diff As Double = Math.Abs(prefix - total / 2.0)

                If diff < bestDiff Then
                    bestDiff = diff
                    split = k
                End If
            Next

            Dim leftWeight As Double = 0

            For k As Integer = 0 To split - 1
                leftWeight += items(k).Weight
            Next

            Dim rightWeight As Double = Math.Max(1E-09, total - leftWeight)
            Dim ratio As Double = Math.Max(1E-06, Math.Min(1 - 1E-06, leftWeight / Math.Max(1E-09, total)))

            Dim leftRect As Rect
            Dim rightRect As Rect

            If area.Width >= area.Height Then
                leftRect = Rect.FromSize(area.X, area.Y, area.Width * ratio, area.Height)
                rightRect = Rect.FromSize(leftRect.P, area.Y, area.Width - leftRect.Width, area.Height)
            Else
                leftRect = Rect.FromSize(area.X, area.Y, area.Width, area.Height * ratio)
                rightRect = Rect.FromSize(area.X, leftRect.Q, area.Width, area.Height - leftRect.Height)
            End If

            PartitionRecursive(leftRect, items.GetRange(0, split), leftWeight, output)
            PartitionRecursive(rightRect, items.GetRange(split, items.Count - split), rightWeight, output)
        End Sub

        ''' <summary>
        ''' 把划分子区域向内收缩，得到「道路」占用的间隙。
        ''' </summary>
        ''' <remarks>
        ''' 论文中街区之间与建筑块之间的空隙即道路；渲染时用收缩后的矩形画建筑块，
        ''' 空隙自然形成网格道路。
        ''' </remarks>
        Public Function Shrink(cells As IEnumerable(Of TreeMapCell), gap As Double) As TreeMapCell()
            Dim result As New List(Of TreeMapCell)()

            For Each cell As TreeMapCell In cells.SafeQuery
                result.Add(New TreeMapCell With {
                    .Id = cell.Id,
                    .Weight = cell.Weight,
                    .Rect = cell.Rect.Inflate(-gap / 2.0)
                })
            Next

            Return result.ToArray
        End Function

    End Module

End Namespace

#End Region
