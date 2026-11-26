Imports System
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Statistics
Imports std = System.Math

Namespace Data

    ''' <summary>一次两组对比的结果（逐基因）。</summary>
    Public Class Contrast

        Public Property Name As String
        Public Property Genes As String()
        Public Property Effect As Double()
        Public Property Z As Double()
        Public Property PValue As Double()
        Public Property Adjusted As Double()
        Public Property Probability As Double()
        Public Property CaseCells As Integer
        Public Property ControlCells As Integer

    End Class

    ''' <summary>
    ''' 基因主序的稀疏表达（每个基因一份 <c>(细胞, 计数)</c> 列表，一次构建、多次对比复用）。
    ''' </summary>
    Public Class GeneMajor

        Public Property Genes As String()
        Public Property Cells As List(Of Integer)()
        Public Property Values As List(Of Double)()
        Public Property CellCount As Integer

        ''' <summary>由"细胞 × 基因"的 CSR 构建（单遍，O(nnz)）。</summary>
        Public Shared Function FromCsr(controls As Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed.CsrMatrix,
                                       genes As String()) As GeneMajor
            Dim nGenes As Integer = If(genes, New String() {}).Length
            Dim cells(nGenes - 1) As List(Of Integer)
            Dim values(nGenes - 1) As List(Of Double)

            For g As Integer = 0 To nGenes - 1
                cells(g) = New List(Of Integer)
                values(g) = New List(Of Double)
            Next

            For c As Integer = 0 To controls.RowDimension - 1
                For k As Integer = controls.Indptr(c) To controls.Indptr(c + 1) - 1
                    Dim gene As Integer = controls.Indices(k)

                    If gene < 0 OrElse gene >= nGenes Then
                        Continue For
                    End If

                    Call cells(gene).Add(c)
                    Call values(gene).Add(controls.Values(k))
                Next
            Next

            Return New GeneMajor With {
                .Genes = genes,
                .Cells = cells,
                .Values = values,
                .CellCount = controls.RowDimension
            }
        End Function

    End Class

    ''' <summary>
    ''' 两组细胞的差异表达：逐基因 log2FC、Mann–Whitney z、双侧 p、BH 校正与证据概率。
    ''' </summary>
    Public Module DifferentialPanel

        ''' <summary>计算一次两组对比。</summary>
        Public Function Contrast(name As String, major As GeneMajor, librarySizes As Double(),
                                 caseCells As Integer(), controlCells As Integer(),
                                 Optional normalizer As Double = 1000000.0) As Contrast
            Dim genes As String() = major.Genes
            Dim nGenes As Integer = genes.Length
            Dim effect(nGenes - 1) As Double
            Dim zScores(nGenes - 1) As Double
            Dim pValues(nGenes - 1) As Double
            Dim inCase As Boolean() = Mask(major.CellCount, caseCells)
            Dim positions As New Dictionary(Of Integer, Integer)

            For c As Integer = 0 To controlCells.Length - 1
                positions(controlCells(c)) = c
            Next

            For g As Integer = 0 To nGenes - 1
                Dim members As List(Of Integer) = major.Cells(g)
                Dim raw As List(Of Double) = major.Values(g)
                Dim controlAll(controlCells.Length - 1) As Double
                Dim caseCpm As New List(Of Double)
                Dim caseSum As Double = 0.0

                For k As Integer = 0 To members.Count - 1
                    Dim cell As Integer = members(k)
                    Dim librarySize As Double = std.Max(librarySizes(cell), 1.0E-12)
                    Dim cpm As Double = normalizer * raw(k) / librarySize

                    If inCase(cell) Then
                        Call caseCpm.Add(cpm)
                        caseSum += cpm
                        Continue For
                    End If

                    Dim at As Integer = -1

                    If positions.TryGetValue(cell, at) Then
                        controlAll(at) = cpm
                    End If
                Next

                Dim meanCase As Double = caseSum / std.Max(caseCpm.Count, 1)
                Dim meanControl As Double = 0.0

                For c As Integer = 0 To controlAll.Length - 1
                    meanControl += controlAll(c)
                Next

                meanControl /= std.Max(controlAll.Length, 1)

                If caseCpm.Count = 0 OrElse controlAll.Length = 0 Then
                    effect(g) = 0.0
                    pValues(g) = 1.0
                    Continue For
                End If

                Call Array.Sort(controlAll)

                Dim z As Double = RankSumZ(caseCpm.ToArray, controlAll, TieSum(controlAll))

                zScores(g) = z
                pValues(g) = NormalDistribution.TwoSidedPValue(z)
                effect(g) = std.Log((meanCase + 1.0) / (meanControl + 1.0), 2.0)
            Next

            Dim adjusted As Double() = FalseDiscoveryRate.PValueAdjust(pValues, PValueAdjustMethod.BH, nGenes)
            Dim probability(nGenes - 1) As Double

            For g As Integer = 0 To nGenes - 1
                probability(g) = std.Max(std.Min(1.0 - adjusted(g), 1.0), 0.0)
            Next

            Return New Contrast With {
                .Name = name,
                .Genes = genes,
                .Effect = effect,
                .Z = zScores,
                .PValue = pValues,
                .Adjusted = adjusted,
                .Probability = probability,
                .CaseCells = caseCells.Length,
                .ControlCells = controlCells.Length
            }
        End Function

        ''' <summary>某个基因在全部细胞上的表达（CPM）。</summary>
        Public Function GeneExpression(major As GeneMajor, gene As Integer, librarySizes As Double()) As Double()
            Dim result(major.CellCount - 1) As Double
            Dim members As List(Of Integer) = major.Cells(gene)
            Dim raw As List(Of Double) = major.Values(gene)

            For k As Integer = 0 To members.Count - 1
                Dim cell As Integer = members(k)

                result(cell) = raw(k) / std.Max(librarySizes(cell), 1.0E-12)
            Next

            Return result
        End Function

        ''' <summary>按表达量取上/下分位，返回 (high, low) 两组细胞下标。</summary>
        Public Function Stratify(values As Double(), members As Integer(),
                                 Optional quantile As Double = 0.25) As (High As Integer(), Low As Integer())
            Dim ranked As Integer() = members.
                OrderBy(Function(i) values(i)).
                ThenBy(Function(i) i).
                ToArray
            Dim take As Integer = CInt(std.Floor(ranked.Length * quantile))

            If take < 3 Then
                Return (ranked, ranked)
            End If

            Return (ranked.Skip(ranked.Length - take).ToArray, ranked.Take(take).ToArray)
        End Function

        Private Function Mask(n As Integer, members As Integer()) As Boolean()
            Dim flags(n - 1) As Boolean

            For Each i As Integer In members
                If i >= 0 AndAlso i < n Then
                    flags(i) = True
                End If
            Next

            Return flags
        End Function

        ''' <summary>Mann–Whitney U 的 z（带并列方差校正）。</summary>
        Private Function RankSumZ(sample As Double(), controlSorted As Double(), controlTieSum As Double) As Double
            Dim n As Integer = sample.Length
            Dim m As Integer = controlSorted.Length

            If n = 0 OrElse m = 0 Then
                Return 0.0
            End If

            Dim ordered As Double() = DirectCast(sample.Clone(), Double())

            Call Array.Sort(ordered)

            Dim u As Double = 0.0
            Dim adjustment As Double = 0.0
            Dim i As Integer = 0

            While i < n
                Dim j As Integer = i

                While j + 1 < n AndAlso ordered(j + 1) = ordered(i)
                    j += 1
                End While

                Dim group As Double = j - i + 1
                Dim low As Integer = LowerBound(controlSorted, ordered(i))
                Dim high As Integer = UpperBound(controlSorted, ordered(i))
                Dim overlap As Double = high - low
                Dim merged As Double = overlap + group

                u += group * (low + 0.5 * overlap)
                adjustment += (merged * merged * merged - merged) - (overlap * overlap * overlap - overlap)
                i = j + 1
            End While

            Dim total As Double = n + m
            Dim correction As Double = 1.0 - (controlTieSum + adjustment) / (total * total * total - total)
            Dim variance As Double = correction * n * m * (total + 1.0) / 12.0

            If variance <= 1.0E-24 Then
                Return 0.0
            End If

            Return (u - n * m / 2.0) / std.Sqrt(variance)
        End Function

        Private Function LowerBound(sorted As Double(), value As Double) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = sorted.Length

            While lo < hi
                Dim mid As Integer = (lo + hi) \ 2

                If sorted(mid) < value Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Return lo
        End Function

        Private Function UpperBound(sorted As Double(), value As Double) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = sorted.Length

            While lo < hi
                Dim mid As Integer = (lo + hi) \ 2

                If sorted(mid) <= value Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Return lo
        End Function

        Private Function TieSum(sorted As Double()) As Double
            Dim total As Double = 0.0
            Dim i As Integer = 0

            While i < sorted.Length
                Dim j As Integer = i

                While j + 1 < sorted.Length AndAlso sorted(j + 1) = sorted(i)
                    j += 1
                End While

                Dim group As Double = j - i + 1

                total += group * group * group - group
                i = j + 1
            End While

            Return total
        End Function

    End Module
End Namespace
