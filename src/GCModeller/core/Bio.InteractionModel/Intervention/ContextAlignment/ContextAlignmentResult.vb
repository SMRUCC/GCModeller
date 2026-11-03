Namespace Intervention

    ''' <summary>
    ''' 上下文对齐的结果：原始取值 + 对齐之后的取值 + 诊断。
    ''' </summary>
    ''' <remarks>
    ''' <see cref="Values"/> 只包含<b>命中了模型基因</b>的条目；未覆盖的模型基因是否需要补齐
    ''' 由调用方决定（见 <see cref="ToVector"/>）。这个约定让“上下文注入”天然是增量的：
    ''' 没有给出的基因继续沿用模型自己的基线，而不是被悄悄写成 0。
    ''' </remarks>
    Public Class ContextAlignmentResult

        ''' <summary>本次使用的对齐模式。</summary>
        Public Property Mode As ContextAlignmentMode

        ''' <summary>对齐之后的取值（基因名 → 表达值，大小写不敏感）。</summary>
        Public Property Values As Dictionary(Of String, Double) =
            New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>对齐之前的原始取值（仅命中基因）。</summary>
        Public Property Raw As Dictionary(Of String, Double) =
            New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>对齐过程的诊断。</summary>
        Public Property Diagnostics As ContextAlignmentDiagnostics

        ''' <summary>对齐之后的结果条目数。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return If(Values Is Nothing, 0, Values.Count)
            End Get
        End Property

        ''' <summary>
        ''' 把对齐结果展开成与模型基因顺序一致的稠密向量（未覆盖的基因按
        ''' <paramref name="fill"/> 策略取值）。
        ''' </summary>
        ''' <param name="reference">提供基因名与填充所需的统计量</param>
        ''' <param name="fill">未覆盖基因的填充策略</param>
        ''' <returns>长度等于 <see cref="ContextAlignmentReference.NGene"/> 的向量</returns>
        ''' <remarks>
        ''' <see cref="ContextMissingGeneFill.Mean"/> 需要 <see cref="ContextAlignmentReference.Means"/>，
        ''' <see cref="ContextMissingGeneFill.Minimum"/> 需要 <see cref="ContextAlignmentReference.Population"/>；
        ''' 缺少所需数据时对应基因填 0（并在无法填充时保持确定性行为，不会抛异常）。
        ''' </remarks>
        Public Function ToVector(reference As ContextAlignmentReference,
                                 Optional fill As ContextMissingGeneFill = ContextMissingGeneFill.Mean) As Double()
            If reference Is Nothing Then
                Throw New ArgumentNullException(NameOf(reference))
            End If

            Dim n As Integer = reference.NGene
            Dim result(n - 1) As Double
            Dim names As String() = reference.GeneNames

            For i As Integer = 0 To n - 1
                Dim name As String = If(names Is Nothing, Nothing, names(i))

                If Not String.IsNullOrEmpty(name) AndAlso Values.ContainsKey(name) Then
                    result(i) = Values(name)
                Else
                    result(i) = fillValue(reference, i, fill)
                End If
            Next

            Return result
        End Function

        Private Shared Function fillValue(reference As ContextAlignmentReference,
                                          index As Integer,
                                          fill As ContextMissingGeneFill) As Double
            Select Case fill
                Case ContextMissingGeneFill.Mean
                    If reference.Means IsNot Nothing AndAlso index < reference.Means.Length Then
                        Return reference.Means(index)
                    End If
                Case ContextMissingGeneFill.Minimum
                    If reference.Population IsNot Nothing AndAlso index < reference.Population.Length Then
                        Dim values As Double() = reference.Population(index)

                        If values IsNot Nothing AndAlso values.Length > 0 Then
                            Dim min As Double = Double.MaxValue

                            For Each v As Double In values
                                If Double.IsNaN(v) OrElse Double.IsInfinity(v) Then
                                    Continue For
                                End If
                                If v < min Then
                                    min = v
                                End If
                            Next

                            If min < Double.MaxValue Then
                                Return min
                            End If
                        End If
                    End If
            End Select

            Return 0.0R
        End Function

        Public Overrides Function ToString() As String
            Return $"context alignment [{Mode}]: {Count} gene(s)" &
                   If(Diagnostics Is Nothing, "", $" / {Diagnostics.Matched}/{Diagnostics.NGene} covered")
        End Function

    End Class

End Namespace
