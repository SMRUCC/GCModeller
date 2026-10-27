Imports std = System.Math

Namespace MultipleOmics

    ''' <summary>
    ''' <see cref="OmicsNormalization"/> 的执行器。
    ''' </summary>
    Public Module OmicsNormalizer

        ''' <summary>
        ''' 对给定组学层施加层内标准化，返回新的层对象（不影响输入层）。
        ''' </summary>
        ''' <param name="layer">待标准化的组学层</param>
        ''' <param name="method">标准化方法</param>
        Public Function Apply(layer As OmicsLayer, method As OmicsNormalization) As OmicsLayer
            If layer Is Nothing Then
                Throw New ArgumentNullException(NameOf(layer))
            End If
            If method = OmicsNormalization.None Then
                Return New OmicsLayer With {
                    .Name = layer.Name,
                    .Expr = layer.Expr,
                    .Normalization = NameOf(OmicsNormalization.None)
                }
            End If

            Dim rows As Double()() = layer.Expr.expression _
                .Select(Function(r) DirectCast(r.experiments.Clone(), Double())) _
                .ToArray

            Select Case method
                Case OmicsNormalization.ZScore
                    ZScoreRows(rows)
                Case OmicsNormalization.RobustZScore
                    RobustRows(rows)
                Case OmicsNormalization.RankPerFeature
                    RankRows(rows)
                Case OmicsNormalization.QuantilePerSample
                    QuantileColumns(rows)
                Case OmicsNormalization.Log2
                    LogRows(rows)
                Case Else
                    Throw New NotSupportedException($"unsupported omics normalization method: {method}")
            End Select

            For Each row As Double() In rows
                For j As Integer = 0 To row.Length - 1
                    If Double.IsNaN(row(j)) OrElse Double.IsInfinity(row(j)) Then
                        row(j) = 0
                    End If
                Next
            Next

            Dim newRows As DataFrameRow() = layer.Expr.expression _
                .Select(Function(r, i) New DataFrameRow With {
                    .geneID = r.geneID,
                    .experiments = rows(i)
                }) _
                .ToArray

            Return New OmicsLayer With {
                .Name = layer.Name,
                .Expr = New Matrix With {
                    .sampleID = layer.Expr.sampleID,
                    .tag = $"{layer.Expr.tag}:{method}",
                    .expression = newRows
                },
                .Normalization = method.ToString
            }
        End Function

        Private Sub ZScoreRows(rows As Double()())
            For Each row As Double() In rows
                Dim n As Integer = row.Length
                Dim mean As Double = 0

                For Each x As Double In row
                    mean += x
                Next

                mean /= n

                Dim ss As Double = 0

                For Each x As Double In row
                    ss += (x - mean) ^ 2
                Next

                Dim sd As Double = If(n > 1, std.Sqrt(ss / (n - 1)), 0)

                If sd <= 0 Then
                    For j As Integer = 0 To n - 1
                        row(j) = 0
                    Next
                Else
                    For j As Integer = 0 To n - 1
                        row(j) = (row(j) - mean) / sd
                    Next
                End If
            Next
        End Sub

        Private Sub RobustRows(rows As Double()())
            For Each row As Double() In rows
                Dim n As Integer = row.Length
                Dim center As Double = Median(row)
                Dim dev As Double() = New Double(n - 1) {}

                For j As Integer = 0 To n - 1
                    dev(j) = std.Abs(row(j) - center)
                Next

                Dim scale As Double = 1.4826 * Median(dev)

                If scale <= 0 Then
                    For j As Integer = 0 To n - 1
                        row(j) = 0
                    Next
                Else
                    For j As Integer = 0 To n - 1
                        row(j) = (row(j) - center) / scale
                    Next
                End If
            Next
        End Sub

        Private Sub RankRows(rows As Double()())
            For Each row As Double() In rows
                Dim ranks As Double() = RanksAverage(row)
                Dim scale As Double = If(row.Length > 1, row.Length - 1, 1)

                For j As Integer = 0 To row.Length - 1
                    row(j) = (ranks(j) - 1) / scale
                Next
            Next
        End Sub

        Private Sub QuantileColumns(rows As Double()())
            Dim nFeature As Integer = rows.Length
            Dim nSample As Integer = rows(0).Length
            Dim sorted As Double()() = New Double(nSample - 1)() {}

            For j As Integer = 0 To nSample - 1
                Dim col As Double() = New Double(nFeature - 1) {}

                For i As Integer = 0 To nFeature - 1
                    col(i) = rows(i)(j)
                Next

                Call Array.Sort(col)
                sorted(j) = col
            Next

            Dim reference As Double() = New Double(nFeature - 1) {}

            For r As Integer = 0 To nFeature - 1
                Dim sum As Double = 0

                For j As Integer = 0 To nSample - 1
                    sum += sorted(j)(r)
                Next

                reference(r) = sum / nSample
            Next

            For j As Integer = 0 To nSample - 1
                Dim col As Double() = New Double(nFeature - 1) {}

                For i As Integer = 0 To nFeature - 1
                    col(i) = rows(i)(j)
                Next

                Dim ranks As Double() = RanksAverage(col)

                For i As Integer = 0 To nFeature - 1
                    Dim r As Integer = CInt(std.Round(ranks(i))) - 1

                    If r < 0 Then
                        r = 0
                    ElseIf r > nFeature - 1 Then
                        r = nFeature - 1
                    End If

                    rows(i)(j) = reference(r)
                Next
            Next
        End Sub

        Private Sub LogRows(rows As Double()())
            Dim shift As Double = 0

            For Each row As Double() In rows
                For Each x As Double In row
                    If x + shift <= 0 Then
                        shift = 1 - x
                    End If
                Next
            Next

            For Each row As Double() In rows
                For j As Integer = 0 To row.Length - 1
                    row(j) = std.Log(row(j) + shift, 2)
                Next
            Next
        End Sub

        ''' <summary>
        ''' 中位数（对输入做排序副本，不修改原数组）。
        ''' </summary>
        Public Function Median(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then
                Return 0
            End If

            Dim copy As Double() = DirectCast(values.Clone(), Double())

            Call Array.Sort(copy)

            Dim mid As Integer = copy.Length \ 2

            If copy.Length Mod 2 = 1 Then
                Return copy(mid)
            Else
                Return (copy(mid - 1) + copy(mid)) / 2.0
            End If
        End Function

        ''' <summary>
        ''' 平均秩（并列取值共享其占据秩的均值），返回 1..n 的秩向量。
        ''' </summary>
        Public Function RanksAverage(values As Double()) As Double()
            Dim n As Integer = values.Length
            Dim order As Integer() = Enumerable.Range(0, n).ToArray

            Call Array.Sort(order,
                Function(a As Integer, b As Integer) As Integer
                    Dim c As Integer = values(a).CompareTo(values(b))

                    If c = 0 Then
                        Return a.CompareTo(b)
                    Else
                        Return c
                    End If
                End Function)

            Dim ranks As Double() = New Double(n - 1) {}
            Dim cursor As Integer = 0

            While cursor < n
                Dim next_ As Integer = cursor + 1

                While next_ < n AndAlso values(order(next_)) = values(order(cursor))
                    next_ += 1
                End While

                Dim average As Double = (cursor + 1 + next_) / 2.0

                For t As Integer = cursor To next_ - 1
                    ranks(order(t)) = average
                Next

                cursor = next_
            End While

            Return ranks
        End Function

    End Module

End Namespace
