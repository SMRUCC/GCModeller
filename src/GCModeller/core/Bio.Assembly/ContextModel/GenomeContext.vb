#Region "Microsoft.VisualBasic::37baa66c45b7dcc49a61a8de199a8876, core\Bio.Assembly\ContextModel\GenomeContext.vb"

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

'   Total Lines: 187
'    Code Lines: 129 (68.98%)
' Comment Lines: 37 (19.79%)
'    - Xml Docs: 78.38%
' 
'   Blank Lines: 21 (11.23%)
'     File Size: 6.91 KB


'     Class GenomeContext
' 
'         Properties: AllFeatureKeys, N, size
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: Absent, Delta, GetByFeature, SelectByRange, selectByStrand
'                   ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ContextModel

    ''' <summary>
    ''' 表示一个基因组上下文模型，用于在内存中高效地存储、索引和查询基因组特征（如基因）。
    ''' </summary>
    ''' <typeparam name="T">基因组特征的类型，必须实现 <see cref="IGeneBrief"/> 接口。</typeparam>
    ''' <remarks>
    ''' 此对象在初始化时会对基因按照物理位置和链方向进行排序和索引，
    ''' 从而将区间查询的时间复杂度优化至 O(log N) 或通过提前跳出优化至近似 O(N)。
    ''' <para>
    ''' 注意：此类假设输入的基因组特征集合是基于同一条染色体或重叠群 的，
    ''' 跨越不同染色体的特征不应放入同一个上下文实例中。
    ''' </para>
    ''' </remarks>
    Public Class GenomeContext(Of T As IGeneBrief) : Implements Enumeration(Of T)

        ''' <summary>
        ''' 正链基因集合。按照 <see cref="NucleotideLocation.Left"/> 升序排列。
        ''' </summary>
        Dim plus As T()
        ''' <summary>
        ''' 负链基因集合。按照 <see cref="NucleotideLocation.Right"/> 降序排列（符合负链转录方向）。
        ''' </summary>
        Dim minus As T()
        ''' <summary>
        ''' 所有基因的统一物理位置序列。严格按照 <see cref="NucleotideLocation.Left"/> 从小到大排序。
        ''' 此数组是执行区间范围查询的基础，确保了空间局部性和提前跳出循环的正确性。
        ''' </summary>
        Dim sequence As T()
        ''' <summary>
        ''' 按照 <see cref="IGeneBrief.Feature"/> 关键字分组的基因字典，用于快速按名称检索。
        ''' </summary>
        Dim featureTags As Dictionary(Of String, T())

        ''' <summary>
        ''' 当前基因组上下文的名称（如染色体名称或菌株名称）。
        ''' </summary>
        Public ReadOnly Property contextName As String

        ''' <summary>
        ''' 通过物理位置索引获取基因组特征
        ''' </summary>
        ''' <param name="i">在 <see cref="sequence"/> 中的零基索引。</param>
        ''' <returns>对应位置的特征对象。</returns>
        Default Public ReadOnly Property Feature(i As Integer) As T
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return sequence(i)
            End Get
        End Property

        ''' <summary>
        ''' 通过特征名称获取基因组特征。如果存在同名特征，返回第一个匹配项。
        ''' </summary>
        ''' <param name="name">特征的唯一标识符或功能名称。</param>
        ''' <returns>匹配的特征对象，如果未找到则返回类型的默认值 (Nothing)。</returns>
        Default Public ReadOnly Property Feature(name As String) As T
            Get
                Return featureTags.TryGetValue(name).DefaultFirst
            End Get
        End Property

        ''' <summary>
        ''' The number of genes in this genome
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property N As Integer
            Get
                Return sequence.Length
            End Get
        End Property

        ''' <summary>
        ''' 得到根据所输入的位点信息估算出目标基因组可能的大小
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Integer

        ''' <summary>
        ''' 获取当前基因组中所有不重复的特征标签名称。
        ''' </summary>
        ''' <returns>特征标签字符串数组。</returns>
        Public ReadOnly Property AllFeatureKeys As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return featureTags.Keys.ToArray
            End Get
        End Property

        ''' <summary>
        ''' 使用基因组特征集合初始化 <see cref="GenomeContext(Of T)"/> 类的新实例。
        ''' 此构造函数会执行数据预处理：按特征名分组、按链方向和物理坐标排序、计算基因组大小。
        ''' </summary>
        ''' <param name="genome">基因组特征的集合。</param>
        ''' <param name="name">上下文名称，默认为 "unnamed"。</param>
        Sub New(genome As IEnumerable(Of T), Optional name$ = "unnamed")
            Dim source = genome.ToArray()

            featureTags = genome _
                .GroupBy(Function(g)
                             If g.Feature.StringEmpty Then
                                 Return "-"
                             Else
                                 Return g.Feature
                             End If
                         End Function) _
                .ToDictionary(Function(g) g.Key,
                              Function(genes)
                                  Return genes.ToArray
                              End Function)

            ' plus的时候，左边是序列的起始方向
            ' minus的时候，右边是序列的起始方向
            plus = selectByStrand(Strands.Forward) _
                .OrderBy(Function(gene) gene.Location.left) _
                .ToArray
            minus = selectByStrand(Strands.Reverse) _
                .OrderByDescending(Function(gene) gene.Location.right) _
                .ToArray
            ' 20260421
            ' 修复：统一按物理位置排序，不要将 plus 和 minus 混合排序，因为 minus 的 OrderByDescending 会导致 sequence 整体不是严格按 Left 升序
            sequence = source _
                .OrderBy(Function(gene) gene.Location.left) _
                .ToArray

            contextName = name
            ' 修复：预先计算 size，避免每次调用都进行 Linq 遍历
            size = sequence _
                .Select(Function(g) g.Location) _
                .Select(Function(loci) {loci.left, loci.right}) _
                .IteratesALL _
                .Max
        End Sub

        ''' <summary>
        ''' A helper function for the constructor to select genes by strand and store them in the corresponding arrays.
        ''' </summary>
        ''' <param name="strand"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function selectByStrand(strand As Strands) As T()
            Return featureTags.Values _
                .IteratesALL _
                .Where(Function(g) g.Location.Strand = strand) _
                .ToArray
        End Function

        ''' <summary>
        ''' 计算全基因组的平均基因长度
        ''' </summary>
        Public Function AverageGeneLength() As Double
            If sequence.Length = 0 Then Return 0
            Return sequence.Average(Function(g) g.Length)
        End Function

        ''' <summary>
        ''' 计算全基因组的平均基因间距
        ''' </summary>
        Public Function AverageIntergenicDistance() As Double
            If sequence.Length < 2 Then Return 0
            Dim distances As New List(Of Integer)

            For i As Integer = 0 To sequence.Length - 2
                Dim curr = sequence(i)
                Dim nxt = sequence(i + 1)
                Dim dist = GetPhysicalDistance(curr.Location, nxt.Location)
                distances.Add(dist)
            Next

            Return distances.Average()
        End Function

        ''' <summary>
        ''' 获取指定滑动窗口内的基因密度 (基因数量 / 窗口大小)
        ''' </summary>
        ''' <param name="start">窗口起始</param>
        ''' <param name="end">窗口结束</param>
        ''' <returns></returns>
        Public Function GeneDensity(start%, end%) As Double
            Dim count = SelectByRange(start, [end]).Count()
            Dim windowSize = [end] - start
            If windowSize <= 0 Then Return 0
            Return count / windowSize
        End Function

        ''' <summary>
        ''' 查找距离指定坐标位点一定半径范围内的所有feature
        ''' </summary>
        ''' <param name="position">基因组上的具体坐标点</param>
        ''' <param name="radius">上下游的查找半径</param>
        ''' <param name="strand">限制查找的链方向，默认为Unknown，即双向都查找</param>
        ''' <returns></returns>
        Public Function SelectByPosition(position As Integer, radius As Integer, Optional strand As Strands = Strands.Unknown) As IEnumerable(Of T)
            ' 坐标边界修正，防止出现负数
            Dim left = Math.Max(1, position - radius)
            Dim right = position + radius
            Return SelectByRange(left, right, strand)
        End Function

        ''' <summary>
        ''' 获取指定基因在基因组物理位置上的左侧和右侧紧邻基因
        ''' </summary>
        ''' <param name="gene">目标基因</param>
        ''' <param name="distanceLimit">最大允许的物理间距，超过此间距则认为不相邻，默认无限制</param>
        ''' <returns>包含左侧邻居和右侧邻居的元组</returns>
        Public Function GetPhysicalNeighbours(gene As T, Optional distanceLimit As Integer = Integer.MaxValue) As (LeftNeighbour As T, RightNeighbour As T)
            ' 使用二分查找在 sequence 中快速定位当前基因的索引
            Dim idx = Array.BinarySearch(sequence, gene, Comparer(Of T).Create(Function(a, b) a.Location.left.CompareTo(b.Location.left)))

            If idx < 0 Then Return (Nothing, Nothing)

            Dim leftNeighbour As T = Nothing
            Dim rightNeighbour As T = Nothing

            ' 寻找左侧邻居
            If idx > 0 Then
                Dim leftGene = sequence(idx - 1)
                ' 检查间距是否在限制内
                If gene.Location.left - leftGene.Location.right <= distanceLimit Then
                    leftNeighbour = leftGene
                End If
            End If

            ' 寻找右侧邻居
            If idx < sequence.Length - 1 Then
                Dim rightGene = sequence(idx + 1)
                If rightGene.Location.left - gene.Location.right <= distanceLimit Then
                    rightNeighbour = rightGene
                End If
            End If

            Return (leftNeighbour, rightNeighbour)
        End Function

        ''' <summary>
        ''' 获取指定基因在转录方向上的上游和下游紧邻基因
        ''' </summary>
        Public Function GetDirectionalNeighbours(gene As T, Optional distanceLimit As Integer = Integer.MaxValue) As (Upstream As T, Downstream As T)
            Dim physical = GetPhysicalNeighbours(gene, distanceLimit)

            If gene.Location.Strand = Strands.Forward Then
                ' 正链：左侧是上游，右侧是下游
                Return (physical.LeftNeighbour, physical.RightNeighbour)
            ElseIf gene.Location.Strand = Strands.Reverse Then
                ' 负链：右侧是上游，左侧是下游
                Return (physical.RightNeighbour, physical.LeftNeighbour)
            Else
                ' 未知链：退化为物理左右
                Return (physical.LeftNeighbour, physical.RightNeighbour)
            End If
        End Function

        ''' <summary>
        ''' 查找与指定位点存在重叠的所有基因
        ''' </summary>
        ''' <param name="loci">目标位点</param>
        ''' <param name="overlapStrand">是否要求必须在同一条链上重叠</param>
        ''' <returns></returns>
        Public Iterator Function FindOverlaps(loci As NucleotideLocation, Optional overlapStrand As Boolean = False) As IEnumerable(Of T)
            ' 因为 sequence 是按 Left 排序的，所以可以快速跳过不可能重叠的基因
            For Each gene As T In sequence
                ' 如果当前基因的左端点已经超过了目标位点的右端点，后续绝不可能重叠
                If gene.Location.left > loci.right Then Exit For

                ' 判断物理重叠
                If gene.Location.right >= loci.left AndAlso gene.Location.left <= loci.right Then
                    ' 判断是否需要链特异性
                    If Not overlapStrand OrElse gene.Location.Strand = loci.Strand Then
                        Yield gene
                    End If
                End If
            Next
        End Function

        ''' <summary>
        ''' 判断两个特征区间是否存在物理重叠
        ''' </summary>
        Public Function IsOverlapping(feature1 As String, feature2 As String) As Boolean
            Dim f1 = GetByFeature(feature1)
            Dim f2 = GetByFeature(feature2)

            For Each g1 In f1
                For Each g2 In f2
                    If g1.Location.right >= g2.Location.left AndAlso g1.Location.left <= g2.Location.right Then
                        Return True
                    End If
                Next
            Next
            Return False
        End Function

        ''' <summary>
        ''' 计算两个位点的物理距离。如果重叠，返回0
        ''' </summary>
        Public Function GetPhysicalDistance(loci1 As NucleotideLocation, loci2 As NucleotideLocation) As Integer
            ' 如果 loci1 在 loci2 左侧
            If loci1.right <= loci2.left Then
                Return loci2.left - loci1.right
                ' 如果 loci1 在 loci2 右侧
            ElseIf loci1.left >= loci2.right Then
                Return loci1.left - loci2.right
                ' 如果重叠
            Else
                Return 0
            End If
        End Function

        ''' <summary>
        ''' 计算两个基因特征之间的最短物理距离
        ''' </summary>
        Public Function GetFeatureDistance(feature1 As String, feature2 As String) As Integer
            Dim f1 = GetByFeature(feature1)
            Dim f2 = GetByFeature(feature2)

            If f1.Length = 0 OrElse f2.Length = 0 Then Return Integer.MaxValue

            ' 寻找两两组合中的最小距离
            Return f1.SelectMany(Function(g1) f2.Select(Function(g2) GetPhysicalDistance(g1.Location, g2.Location))).Min()
        End Function

        ''' <summary>
        ''' 根据基因产物的注释关键词模糊搜索基因
        ''' </summary>
        ''' <param name="keyword">搜索关键词</param>
        ''' <param name="ignoreCase">是否忽略大小写，默认忽略</param>
        ''' <returns></returns>
        Public Iterator Function SearchByProduct(keyword As String, Optional ignoreCase As Boolean = True) As IEnumerable(Of T)
            Dim comparison = If(ignoreCase, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal)

            For Each gene As T In sequence
                If Not gene.Product.StringEmpty AndAlso gene.Product.Contains(keyword, comparison) Then
                    Yield gene
                End If
            Next
        End Function

        ''' <summary>
        ''' 获取所有具有相同功能注释的基因集合
        ''' </summary>
        Public Function GetGenesByProduct(product As String, Optional ignoreCase As Boolean = True) As T()
            Return SearchByProduct(product, ignoreCase).ToArray()
        End Function

        ''' <summary>
        ''' 查找指定位点附近上下游范围内的feature。此函数会考虑链的方向性：
        ''' 对于正链，上游为坐标减小方向，下游为坐标增大方向；
        ''' 对于负链，上游为坐标增大方向，下游为坐标减小方向。
        ''' </summary>
        ''' <param name="loci">参考位点</param>
        ''' <param name="upstream">上游延伸长度</param>
        ''' <param name="downstream">下游延伸长度</param>
        ''' <param name="includeSelf">是否包含与参考位点完全重叠的feature</param>
        ''' <param name="strand">限制查找的链方向，默认为Unknown，即双向都查找</param>
        ''' <returns></returns>
        Public Function GetNearbyFeatures(loci As NucleotideLocation, upstream As Integer, downstream As Integer,
                                          Optional includeSelf As Boolean = True,
                                          Optional strand As Strands = Strands.Unknown) As IEnumerable(Of T)
            Dim rangeLeft As Integer
            Dim rangeRight As Integer

            If loci.Strand = Strands.Reverse Then
                ' 负链：Start是Right，Ends是Left
                ' 上游是坐标增大方向 (Right + upstream)
                ' 下游是坐标减小方向 (Left - downstream)
                rangeLeft = loci.left - downstream
                rangeRight = loci.right + upstream
            Else
                ' 正链(或Unknown)：Start是Left，Ends是Right
                ' 上游是坐标减小方向 (Left - upstream)
                ' 下游是坐标增大方向 (Right + downstream)
                rangeLeft = loci.left - upstream
                rangeRight = loci.right + downstream
            End If

            ' 坐标边界修正
            rangeLeft = Math.Max(1, rangeLeft)
            rangeRight = Math.Max(rangeLeft, rangeRight)

            Dim results = SelectByRange(rangeLeft, rangeRight, strand)

            If Not includeSelf Then
                ' 排除掉与参考位点自身完全一致的feature（按Left, Right, Strand三要素判断）
                results = results.Where(Function(f) Not (f.Location.left = loci.left AndAlso
                                                f.Location.right = loci.right AndAlso
                                                f.Location.Strand = loci.Strand))
            End If

            Return results
        End Function

        ''' <summary>
        ''' 查找指定名称的feature附近上下游范围内的feature
        ''' </summary>
        ''' <param name="featureName">目标feature的名称</param>
        ''' <param name="upstream">上游延伸长度</param>
        ''' <param name="downstream">下游延伸长度</param>
        ''' <param name="includeSelf">是否包含目标feature自身</param>
        ''' <param name="strand">限制查找的链方向</param>
        ''' <returns></returns>
        Public Function GetNearbyFeaturesByName(featureName As String, upstream As Integer, downstream As Integer,
                                                Optional includeSelf As Boolean = True,
                                                Optional strand As Strands = Strands.Unknown) As IEnumerable(Of T)

            Dim targets = GetByFeature(featureName)
            Dim result As New List(Of T)

            For Each target As T In targets
                Dim nearby = GetNearbyFeatures(target.Location, upstream, downstream, includeSelf, strand)
                result.AddRange(nearby)
            Next

            ' 去重处理（因为不同的target可能会查找到相同的附近feature）
            Return result.Distinct()
        End Function

        ''' <summary>
        ''' count the number of genes between feature 1 and feature 2.
        ''' </summary>
        ''' <param name="feature1"></param>
        ''' <param name="feature2"></param>
        ''' <returns></returns>
        Public Function Delta(feature1$, feature2$) As Double
            Dim l1 = GetByFeature(feature1)
            Dim l2 = GetByFeature(feature2).AsList
            Dim d As New List(Of Integer)

            For Each i As T In l1
                If l2.Count = 0 Then Exit For ' 修复：防止 l2 被减空后调用 .First 报错

                Dim j = l2.OrderBy(Function(lj) lj.Location.GetATGDistance(i)).First
                l2 -= j

                ' 修复：处理重叠和计算区间内部基因数量
                Dim minRight = Math.Min(i.Location.right, j.Location.right)
                Dim maxLeft = Math.Max(i.Location.left, j.Location.left)

                If maxLeft <= minRight Then
                    ' 基因存在重叠区域，中间不可能有其他基因
                    d += 0
                Else
                    ' 计算严格处于两者之间的基因数量
                    Dim betweenGenes = SelectByRange(maxLeft, minRight, Strands.Unknown)
                    ' 排除掉 i 和 j 自身（如果恰好边界被包含进来）
                    Dim n = Aggregate g As T
                            In betweenGenes
                            Where g.Feature <> i.Feature AndAlso g.Feature <> j.Feature
                            Into Count
                    d += n
                End If
            Next

            If d.Count = 0 Then
                Return 0
            ElseIf d.Count = 1 Then
                Return d.First
            Else
                Return d.Average
            End If
        End Function

        ''' <summary>
        ''' 将基因组上面的某一区域内的基因对象都查找出来
        ''' </summary>
        ''' <param name="i"></param>
        ''' <param name="j"></param>
        ''' <param name="strand">
        ''' 默认不限制链的方向
        ''' </param>
        ''' <returns></returns>
        Public Iterator Function SelectByRange(i%, j%, Optional strand As Strands = Strands.Unknown) As IEnumerable(Of T)
            Dim range As New IntRange({i, j})
            ' 修复：统一使用 sequence，保证数组始终是按 Left 严格升序排列的
            Dim source As T() = sequence

            For Each gene As T In source
                ' 优化：因为 sequence 是按 Left 升序排序的
                ' 如果当前基因的 Left 已经大于查询范围的最大值，后面肯定都不在范围内，直接退出
                If gene.Location.left > range.Max Then
                    Exit For
                End If

                ' 判断是否重叠
                If range.IsOverlapping(gene.Location) OrElse range.IsInside(gene.Location) Then
                    ' 判断链方向是否符合要求
                    If strand = Strands.Unknown OrElse gene.Location.Strand = strand Then
                        Yield gene
                    End If
                End If
            Next
        End Function

        ''' <summary>
        ''' Get features data via a given key id
        ''' </summary>
        ''' <param name="feature"></param>
        ''' <returns></returns>
        Public Function GetByFeature(feature As String) As T()
            If featureTags.ContainsKey(feature) Then
                Return featureTags(feature)
            Else
                Return {}
            End If
        End Function

        ''' <summary>
        ''' Check of the target feature is missing inside this genomics context model?
        ''' </summary>
        ''' <param name="feature"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Absent(feature As String) As Boolean
            Return Not featureTags.ContainsKey(feature)
        End Function

        Public Overrides Function ToString() As String
            Return $"{contextName}: {plus.Length} (+), {minus.Length} (-) with {featureTags.Count} features."
        End Function

        ''' <summary>
        ''' populate out the genomics feature <see cref="sequence"/> data
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GenericEnumerator() As IEnumerator(Of T) Implements Enumeration(Of T).GenericEnumerator
            If sequence Is Nothing Then
                Return
            End If

            For Each feature As T In sequence
                Yield feature
            Next
        End Function
    End Class
End Namespace
