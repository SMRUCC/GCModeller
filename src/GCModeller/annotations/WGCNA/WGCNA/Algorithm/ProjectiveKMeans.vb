#Region "Microsoft.VisualBasic::ProjectiveKMeans, annotations\WGCNA\WGCNA\Algorithm\ProjectiveKMeans.vb"

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

#End Region

Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

''' <summary>
''' R WGCNA <c>projectiveKMeans</c> 式的基因预聚类
''' </summary>
''' <remarks>
''' 分块计算的关键前提是「把高度共表达的基因放进同一个块」。如果按原始顺序
''' 机械切块，跨块的相关性会被丢掉，模块会被人为割裂。
''' 
''' <para>
''' R 的做法（<c>projectiveKMeans</c>）是：
''' <list type="number">
''' <item><description>把表达矩阵按行 z-score 标准化；</description></item>
''' <item><description>随机抽样一部分基因，在其上运行 k-means（k = ceil(nGenes / preferredSize)）；</description></item>
''' <item><description>用 k-means 的质心作为"投影方向"，把所有基因按与质心的相关性分配到最近的簇；</description></item>
''' <item><description>对仍然超过 preferredSize 的簇递归再分，直到每簇大小合适。</description></item>
''' </list>
''' </para>
''' 
''' <para>
''' 抽样 k-means 让这一步的代价只有 O(sampleSize · k · nSamples)，
''' 而全基因分配是 O(nGenes · k · nSamples)，都远低于后续 O(nGenes · B^2) 的 TOM。
''' </para>
''' </remarks>
Public Module ProjectiveKMeans

    ''' <summary>递归拆分的最大深度，防止病态数据下无限递归</summary>
    Private Const MaxDepth As Integer = 8
    ''' <summary>Lloyd 迭代次数</summary>
    Private Const MaxIter As Integer = 25

    ''' <summary>
    ''' 把基因划分成若干个块
    ''' </summary>
    ''' <param name="expr">基因 x 样本的表达矩阵（交错数组）</param>
    ''' <param name="preferredSize">每个块的期望基因数（即 maxBlockSize）</param>
    ''' <param name="seed">随机种子，保证结果可复现</param>
    ''' <param name="sampleSize">k-means 的抽样基因数（0 表示自动取 min(n, max(1000, 20*k))）</param>
    ''' <returns>块数组，每个元素是该块内基因在 <paramref name="expr"/> 中的 0 基行下标</returns>
    Public Function Partition(expr As Double()(), preferredSize As Integer,
                              Optional seed As Integer = 12345,
                              Optional sampleSize As Integer = 0) As Integer()()

        Dim n As Integer = expr.Length

        If n <= preferredSize Then
            Return New Integer()() {Enumerable.Range(0, n).ToArray()}
        End If

        Dim cols As Integer = expr(Scan0).Length
        Dim z As Double()() = ZScore(expr, n, cols)
        Dim k As Integer = CInt(std.Ceiling(n / CDbl(preferredSize)))

        If k < 2 Then k = 2

        Dim sample As Integer() = TakeSample(n, sampleSize, k, seed)
        Dim centers As Double()() = Lloyd(z, sample, k, cols, seed)
        Dim assign As Integer() = AssignAll(z, centers, n, cols)
        Dim blocks As New List(Of Integer())

        Call SplitRecursively(z, assign, k, preferredSize, cols, seed, 0, blocks)

        Return blocks _
            .Where(Function(b) b.Length > 0) _
            .ToArray()
    End Function

    ''' <summary>
    ''' 按行做 z-score 标准化（标准差为 0 的行置零）
    ''' </summary>
    Private Function ZScore(expr As Double()(), n As Integer, cols As Integer) As Double()()
        Dim z(n - 1)() As Double

        Call Parallel.For(0, n,
            Sub(i)
                Dim src As Double() = expr(i)
                Dim row(cols - 1) As Double
                Dim mean As Double = 0
                Dim count As Integer = 0

                For j As Integer = 0 To cols - 1
                    If Not Double.IsNaN(src(j)) Then
                        mean += src(j)
                        count += 1
                    End If
                Next

                If count = 0 Then
                    z(i) = row
                    Return
                End If

                mean /= count

                Dim ss As Double = 0

                For j As Integer = 0 To cols - 1
                    Dim d As Double = If(Double.IsNaN(src(j)), 0, src(j) - mean)
                    row(j) = d
                    ss += d * d
                Next

                If ss > 0 Then
                    Dim sd As Double = std.Sqrt(ss / cols)

                    For j As Integer = 0 To cols - 1
                        row(j) /= sd
                    Next
                Else
                    For j As Integer = 0 To cols - 1
                        row(j) = 0
                    Next
                End If

                z(i) = row
            End Sub)

        Return z
    End Function

    ''' <summary>
    ''' 抽取 k-means 用的基因下标
    ''' </summary>
    Private Function TakeSample(n As Integer, sampleSize As Integer, k As Integer, seed As Integer) As Integer()
        Dim size As Integer = sampleSize

        If size <= 0 Then
            size = std.Max(1000, 20 * k)
        End If

        If size >= n Then
            Return Enumerable.Range(0, n).ToArray()
        End If

        Dim rnd As New Random(seed)
        Dim idx As Integer() = Enumerable.Range(0, n).ToArray()

        ' 部分 Fisher-Yates 洗牌
        For i As Integer = 0 To size - 1
            Dim j As Integer = rnd.Next(i, n)
            Dim tmp As Integer = idx(i)
            idx(i) = idx(j)
            idx(j) = tmp
        Next

        Return idx.Take(size).ToArray()
    End Function

    ''' <summary>
    ''' 在抽样子集上运行 Lloyd k-means（欧氏距离）
    ''' </summary>
    Private Function Lloyd(z As Double()(), sample As Integer(), k As Integer, cols As Integer, seed As Integer) As Double()()
        Dim m As Integer = sample.Length
        Dim rnd As New Random(seed)
        Dim centers As Double()() = New Double(k - 1)() {}
        Dim taken As New List(Of Integer)

        For c As Integer = 0 To k - 1
            Dim p As Integer = rnd.Next(0, m)

            While taken.Contains(p)
                p = rnd.Next(0, m)
            End While

            taken.Add(p)
            centers(c) = CType(z(sample(p)).Clone(), Double())
        Next

        Dim assign(m - 1) As Integer

        For iter As Integer = 1 To MaxIter
            Dim moved As Boolean = False

            Call Parallel.For(0, m,
                Sub(i)
                    Dim row As Double() = z(sample(i))
                    Dim best As Integer = 0
                    Dim bestD As Double = Double.PositiveInfinity

                    For c As Integer = 0 To k - 1
                        Dim d As Double = SqDist(row, centers(c), cols)

                        If d < bestD Then
                            bestD = d
                            best = c
                        End If
                    Next

                    If assign(i) <> best Then
                        assign(i) = best
                        moved = True
                    End If
                End Sub)

            ' 重算质心
            Dim sums As Double()() = New Double(k - 1)() {}
            Dim counts As Integer() = New Integer(k - 1) {}

            For c As Integer = 0 To k - 1
                sums(c) = New Double(cols - 1) {}
            Next

            For i As Integer = 0 To m - 1
                Dim row As Double() = z(sample(i))
                Dim c As Integer = assign(i)

                For j As Integer = 0 To cols - 1
                    sums(c)(j) += row(j)
                Next

                counts(c) += 1
            Next

            For c As Integer = 0 To k - 1
                If counts(c) > 0 Then
                    For j As Integer = 0 To cols - 1
                        centers(c)(j) = sums(c)(j) / counts(c)
                    Next
                End If
            Next

            If Not moved AndAlso iter > 1 Then
                Exit For
            End If
        Next

        Return centers
    End Function

    ''' <summary>
    ''' 平方欧氏距离
    ''' </summary>
    Private Function SqDist(a As Double(), b As Double(), cols As Integer) As Double
        Dim s As Double = 0

        For j As Integer = 0 To cols - 1
            Dim d As Double = a(j) - b(j)
            s += d * d
        Next

        Return s
    End Function

    ''' <summary>
    ''' 把全部基因分配到与质心相关性最高的簇
    ''' </summary>
    Private Function AssignAll(z As Double()(), centers As Double()(), n As Integer, cols As Integer) As Integer()
        Dim k As Integer = centers.Length
        Dim norm As Double()() = New Double(k - 1)() {}

        For c As Integer = 0 To k - 1
            norm(c) = Normalize(centers(c), cols)
        Next

        Dim assign(n - 1) As Integer

        Call Parallel.For(0, n,
            Sub(i)
                Dim row As Double() = Normalize(z(i), cols)
                Dim best As Integer = 0
                Dim bestR As Double = Double.NegativeInfinity

                For c As Integer = 0 To k - 1
                    Dim r As Double = Dot(row, norm(c), cols)

                    If r > bestR Then
                        bestR = r
                        best = c
                    End If
                Next

                assign(i) = best
            End Sub)

        Return assign
    End Function

    ''' <summary>
    ''' 对仍然过大的簇做递归拆分
    ''' </summary>
    Private Sub SplitRecursively(z As Double()(), assign As Integer(), k As Integer,
                                 preferredSize As Integer, cols As Integer, seed As Integer,
                                 depth As Integer, out As List(Of Integer()))

        Dim groups As List(Of Integer)() = New List(Of Integer)(k - 1) {}

        For c As Integer = 0 To k - 1
            groups(c) = New List(Of Integer)()
        Next

        For i As Integer = 0 To assign.Length - 1
            Call groups(assign(i)).Add(i)
        Next

        For c As Integer = 0 To k - 1
            Dim members As Integer() = groups(c).ToArray()

            If members.Length = 0 Then
                Continue For
            End If

            If members.Length > preferredSize AndAlso depth < MaxDepth AndAlso members.Length >= 4 Then
                Dim subK As Integer = CInt(std.Ceiling(members.Length / CDbl(preferredSize)))
                Dim subCenters As Double()() = Lloyd(z, members, subK, cols, seed + depth * 31 + c)
                Dim subAssign As Integer() = AssignAll(z, subCenters, members.Length, cols)

                Call SplitRecursively(z, subAssign, subK, preferredSize, cols, seed + 17, depth + 1, out)
            Else
                Call out.Add(members)
            End If
        Next
    End Sub

    ''' <summary>
    ''' L2 归一化（零向量保持为零向量）
    ''' </summary>
    Private Function Normalize(row As Double(), cols As Integer) As Double()
        Dim out(cols - 1) As Double
        Dim ss As Double = 0

        For j As Integer = 0 To cols - 1
            ss += row(j) * row(j)
        Next

        If ss <= 0 Then
            Return out
        End If

        Dim inv As Double = 1 / std.Sqrt(ss)

        For j As Integer = 0 To cols - 1
            out(j) = row(j) * inv
        Next

        Return out
    End Function

    ''' <summary>
    ''' 向量点积
    ''' </summary>
    Private Function Dot(a As Double(), b As Double(), cols As Integer) As Double
        Dim s As Double = 0

        For j As Integer = 0 To cols - 1
            s += a(j) * b(j)
        Next

        Return s
    End Function
End Module
