Namespace Coordinates

    ''' <summary>
    ''' 坐标重建的质量度量，供单元测试与 demo 的断言使用。
    ''' </summary>
    Public Class CoordinateQuality

        ''' <summary>坐标维度描述（<c>2D</c> / <c>3D</c>）。</summary>
        Public Property Dimension As String
        ''' <summary>参与度量的原子个数。</summary>
        Public Property AtomCount As Integer
        ''' <summary>平均键长。</summary>
        Public Property AverageBondLength As Double
        ''' <summary>键长标准差，用于衡量"等键长"约束是否收敛。</summary>
        Public Property BondLengthDeviation As Double
        ''' <summary>非键原子之间的最小距离；不存在非键原子对时为 <c>99</c>。</summary>
        Public Property MinNonBondedDistance As Double = 99
        ''' <summary>最小非键距离对应的原子对描述（如 <c>#3 C - #9 H</c>）。</summary>
        Public Property MinNonBondedPair As String
        ''' <summary>距离过近（重叠）的原子对个数。</summary>
        Public Property OverlapCount As Integer
        ''' <summary>平均绝对坐标值，用于检测退化的原点堆积。</summary>
        Public Property MeanExtent As Double
        ''' <summary>是否存在 NaN/Infinity 坐标。</summary>
        Public Property HasInvalidCoordinate As Boolean

        ''' <summary>
        ''' 坐标是否可用：无非法值、无重叠，且（原子数大于 1 时）具有非零的延展。
        ''' </summary>
        Public ReadOnly Property IsValid As Boolean
            Get
                Return Not HasInvalidCoordinate AndAlso
                    OverlapCount = 0 AndAlso
                    (AtomCount <= 1 OrElse MeanExtent > 1.0E-6)
            End Get
        End Property

        ''' <summary>
        ''' 测量 2D 坐标质量。
        ''' </summary>
        ''' <param name="graph">已重建 2D 坐标的分子图。</param>
        Public Shared Function Measure2D(graph As Graph.MoleculeGraph) As CoordinateQuality
            Return measure(graph, False)
        End Function

        ''' <summary>
        ''' 测量 3D 坐标质量。
        ''' </summary>
        ''' <param name="graph">已重建 3D 坐标的分子图。</param>
        Public Shared Function Measure3D(graph As Graph.MoleculeGraph) As CoordinateQuality
            Return measure(graph, True)
        End Function

        Private Shared Function measure(graph As Graph.MoleculeGraph, threeD As Boolean) As CoordinateQuality
            Dim n As Integer = graph.AtomCount
            Dim result As New CoordinateQuality With {
                .Dimension = If(threeD, "3D", "2D"),
                .AtomCount = n
            }

            If n = 0 Then Return result

            Dim points(n - 1) As Vec3

            For i = 0 To n - 1
                Dim atom As Graph.Atom = graph.Atoms(i)

                points(i) = New Vec3(atom.X, atom.Y, If(threeD, atom.Z, 0))

                If Double.IsNaN(atom.X) OrElse Double.IsNaN(atom.Y) OrElse Double.IsNaN(atom.Z) OrElse
                   Double.IsInfinity(atom.X) OrElse Double.IsInfinity(atom.Y) OrElse Double.IsInfinity(atom.Z) Then
                    result.HasInvalidCoordinate = True
                End If
            Next

            Dim lengths As New List(Of Double)

            For Each bond As Graph.Bond In graph.Bonds
                lengths.Add(Vec3.Distance(points(bond.a), points(bond.b)))
            Next

            If lengths.Count > 0 Then
                result.AverageBondLength = lengths.Average()

                Dim variance As Double = 0

                For Each length As Double In lengths
                    variance += (length - result.AverageBondLength) ^ 2
                Next

                result.BondLengthDeviation = Math.Sqrt(variance / lengths.Count)
            End If

            Dim extent As Double = 0

            For Each point As Vec3 In points
                extent += Math.Abs(point.X) + Math.Abs(point.Y) + Math.Abs(point.Z)
            Next

            result.MeanExtent = extent / n

            Dim threshold As Double = If(result.AverageBondLength > 0, result.AverageBondLength * 0.35, 0.35)

            For a = 0 To n - 1
                For b = a + 1 To n - 1
                    If graph.BondOrder(a, b) > 0 Then Continue For

                    Dim distance As Double = Vec3.Distance(points(a), points(b))

                    If distance < result.MinNonBondedDistance Then
                        result.MinNonBondedDistance = distance
                        result.MinNonBondedPair = $"#{a} {graph.Atoms(a).Element} - #{b} {graph.Atoms(b).Element}"
                    End If

                    If distance < threshold Then result.OverlapCount += 1
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 生成单行摘要，便于写入测试报告。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return $"{Dimension}: avg bond {AverageBondLength:F4} ± {BondLengthDeviation:F4}, " &
                   $"min non-bonded {MinNonBondedDistance:F4} ({MinNonBondedPair}), overlaps {OverlapCount}"
        End Function

    End Class

End Namespace
