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

Imports Microsoft.VisualBasic.Linq

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    ''' <summary>
    ''' 按照位点类型名称(feature key)分组的索引。
    ''' </summary>
    ''' <remarks>
    ''' 对于人基因组的一条染色体记录而言，位点的数量可以达到4万个以上，
    ''' 按照类型分组之后，<see cref="FEATURES.Query"/>以及<see cref="FEATURES.ListFeatures"/>
    ''' 只需要扫描目标类型的位点，而不是整个列表。
    ''' </remarks>
    Public NotInheritable Class FeatureKeyIndex

        Private Shared ReadOnly EMPTY As Feature() = New Feature() {}

        Private ReadOnly index As Dictionary(Of String, Feature())

        ''' <summary>
        ''' 创建一个空的索引
        ''' </summary>
        Sub New()
            index = New Dictionary(Of String, Feature())(StringComparer.OrdinalIgnoreCase)
        End Sub

        ''' <summary>
        ''' 根据给定的位点列表建立索引
        ''' </summary>
        Sub New(features As IEnumerable(Of Feature))
            index = New Dictionary(Of String, Feature())(StringComparer.OrdinalIgnoreCase)
            Call Rebuild(features)
        End Sub

        ''' <summary>
        ''' 清空并且重建索引
        ''' </summary>
        Public Sub Rebuild(features As IEnumerable(Of Feature))
            Call index.Clear()

            If features Is Nothing Then
                Return
            End If

            Dim buckets As New Dictionary(Of String, List(Of Feature))(StringComparer.OrdinalIgnoreCase)

            For Each feature As Feature In features
                If feature Is Nothing Then
                    Continue For
                End If

                Dim key As String = If(feature.KeyName, "")

                If key Is Nothing Then
                    key = ""
                End If

                Dim bucket As List(Of Feature) = Nothing

                If Not buckets.TryGetValue(key, bucket) Then
                    bucket = New List(Of Feature)
                    buckets(key) = bucket
                End If

                Call bucket.Add(feature)
            Next

            For Each part As KeyValuePair(Of String, List(Of Feature)) In buckets
                index(part.Key) = part.Value.ToArray
            Next
        End Sub

        ''' <summary>
        ''' 索引之中所包含的位点类型名称
        ''' </summary>
        Public ReadOnly Property Keys As IEnumerable(Of String)
            Get
                Return index.Keys
            End Get
        End Property

        ''' <summary>
        ''' 位点类型的数量
        ''' </summary>
        Public ReadOnly Property Count As Integer
            Get
                Return index.Count
            End Get
        End Property

        ''' <summary>
        ''' 按照类型名称获取位点列表，类型不存在的时候返回空数组
        ''' </summary>
        Default Public ReadOnly Property Item(key As String) As Feature()
            Get
                Return GetByKey(key)
            End Get
        End Property

        ''' <summary>
        ''' 按照类型名称获取位点列表，类型不存在的时候返回空数组
        ''' </summary>
        ''' <param name="key">位点类型名称，例如``CDS``/``gene``/``mRNA``</param>
        Public Function GetByKey(key As String) As Feature()
            If String.IsNullOrEmpty(key) Then
                Return EMPTY
            End If

            Dim found As Feature() = Nothing

            If index.TryGetValue(key, found) Then
                Return found
            Else
                Return EMPTY
            End If
        End Function
    End Class

    ''' <summary>
    ''' 位点的位置区间索引。
    ''' </summary>
    ''' <remarks>
    ''' 旧版本的``GetByLocation``是对整个位点列表进行线性扫描(<c>O(n)</c>)，
    ''' 对于人基因组这种具有数万个位点的染色体记录而言，每一次区间查询都是极其昂贵的。
    ''' 
    ''' 这里的实现是: 按照位点的起始位置排序，同时维护一个"前缀最大终止位置"的数组，
    ''' 于是区间查询可以通过两次二分查找定位到候选的位点范围，复杂度为<c>O(log n + k)</c>，
    ''' 其中<c>k</c>为实际重叠的位点数量。额外的内存开销约为<c>n*(8+4)</c>字节。
    ''' </remarks>
    Public NotInheritable Class FeatureIntervalIndex

        Private Shared ReadOnly EMPTY As Feature() = New Feature() {}

        Private features As Feature() = New Feature() {}
        Private lefts As Long() = New Long() {}
        Private maxRight As Long() = New Long() {}

        ''' <summary>
        ''' 索引之中的位点数量
        ''' </summary>
        Public ReadOnly Property Count As Integer
            Get
                Return features.Length
            End Get
        End Property

        ''' <summary>
        ''' 根据给定的位点列表重建索引
        ''' </summary>
        Public Sub Rebuild(source As IEnumerable(Of Feature))
            If source Is Nothing Then
                features = New Feature() {}
                lefts = New Long() {}
                maxRight = New Long() {}
                Return
            End If

            Dim list As New List(Of Feature)

            For Each feature As Feature In source
                If feature Is Nothing OrElse feature.Location Is Nothing Then
                    Continue For
                End If
                If feature.Location.Right <= 0 Then
                    Continue For
                End If

                Call list.Add(feature)
            Next

            Dim n As Integer = list.Count

            If n = 0 Then
                features = New Feature() {}
                lefts = New Long() {}
                maxRight = New Long() {}
                Return
            End If

            Dim items As Feature() = list.ToArray
            Dim keys As Long() = New Long(n - 1) {}

            For i As Integer = 0 To n - 1
                keys(i) = items(i).Location.Left
            Next

            Call Array.Sort(keys, items)

            Dim maxR As Long() = New Long(n - 1) {}
            Dim current As Long = Long.MinValue

            For i As Integer = 0 To n - 1
                Dim right As Long = items(i).Location.Right

                If right > current Then
                    current = right
                End If

                maxR(i) = current
            Next

            features = items
            lefts = keys
            maxRight = maxR
        End Sub

        ''' <summary>
        ''' 返回与目标区间存在重叠的全部位点
        ''' </summary>
        ''' <param name="left">区间的起始位置(1-based，包含)</param>
        ''' <param name="right">区间的结束位置(1-based，包含)</param>
        Public Function Overlap(left As Long, right As Long) As Feature()
            If features.Length = 0 Then
                Return EMPTY
            End If

            Dim l As Long = left
            Dim r As Long = right

            If l > r Then
                l = right
                r = left
            End If

            ' 候选范围的右边界: 起始位置小于等于目标区间结束位置的最后一个位点
            Dim hi As Integer = __lastLessOrEqual(r)

            If hi < 0 Then
                Return EMPTY
            End If

            ' 候选范围的左边界: 前缀最大终止位置大于等于目标区间起始位置的第一个位点
            Dim lo As Integer = __firstMaxRightAtLeast(l, hi)

            If lo < 0 OrElse lo > hi Then
                Return EMPTY
            End If

            Dim out As New List(Of Feature)(hi - lo + 1)

            For i As Integer = lo To hi
                If features(i).Location.Right >= l Then
                    Call out.Add(features(i))
                End If
            Next

            Return out.ToArray
        End Function

        ''' <summary>
        ''' 二分查找: 起始位置小于等于目标值的最后一个下标
        ''' </summary>
        Private Function __lastLessOrEqual(value As Long) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = lefts.Length - 1
            Dim ans As Integer = -1

            Do While lo <= hi
                Dim mid As Integer = (lo + hi) \ 2

                If lefts(mid) <= value Then
                    ans = mid
                    lo = mid + 1
                Else
                    hi = mid - 1
                End If
            Loop

            Return ans
        End Function

        ''' <summary>
        ''' 二分查找: 前缀最大终止位置大于等于目标值的最小下标(在<c>[0, limit]</c>的范围之内)
        ''' </summary>
        Private Function __firstMaxRightAtLeast(value As Long, limit As Integer) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = limit
            Dim ans As Integer = -1

            Do While lo <= hi
                Dim mid As Integer = (lo + hi) \ 2

                If maxRight(mid) >= value Then
                    ans = mid
                    hi = mid - 1
                Else
                    lo = mid + 1
                End If
            Loop

            Return ans
        End Function
    End Class
End Namespace
