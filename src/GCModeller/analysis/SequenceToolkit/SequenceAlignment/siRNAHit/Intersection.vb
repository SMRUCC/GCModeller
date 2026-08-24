Imports System.Runtime.CompilerServices

Namespace SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit

    ''' <summary>
    ''' psRNATarget 与 TargetFinder 两算法结果的交集合并器。
    ''' 
    ''' 高置信靶标定义为：同一 (miRNA, 靶标 mRNA, 靶位点) 同时被两种算法命中。
    ''' 由于两算法报告的坐标可能存在小幅偏移，采用 ±<see cref="SiteTolerance"/> nt
    ''' 的容差进行对齐。
    ''' </summary>
    Public Class Intersection

        ''' <summary>mRNA 靶位点坐标对齐容差（nt），默认 ±3。</summary>
        Public Property SiteTolerance As Integer = 3

        ''' <summary>
        ''' 取两算法命中集合的交集。
        ''' </summary>
        ''' <param name="psrna">psRNATarget 命中结果</param>
        ''' <param name="targetFinder">TargetFinder 命中结果</param>
        Public Function Merge(psrna As IEnumerable(Of siRNAHit), targetFinder As IEnumerable(Of siRNAHit)) As List(Of siRNAHit)
            Dim a As List(Of siRNAHit) = psrna.ToList()
            Dim b As List(Of siRNAHit) = targetFinder.ToList()
            Dim result As New List(Of siRNAHit)()

            For Each x In a
                For Each y In b
                    If IsSameSite(x, y) Then
                        result.Add(MergeHit(x, y))
                    End If
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 判断两命中是否属于同一靶位点：
        ''' miRNA 与 靶标 mRNA 标识一致，且 mRNA 靶位点区间在容差内重叠。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function IsSameSite(x As siRNAHit, y As siRNAHit) As Boolean
            If Not String.Equals(x.miRNA, y.miRNA, StringComparison.Ordinal) Then
                Return False
            End If
            ' 忽略 secondary 后缀差异
            If Not String.Equals(NormalizeTarget(x.Target), NormalizeTarget(y.Target), StringComparison.Ordinal) Then
                Return False
            End If
            ' 区间重叠（带容差）
            Dim xA As Integer = x.StartSite - SiteTolerance
            Dim xB As Integer = x.EndSite + SiteTolerance
            Dim yA As Integer = y.StartSite
            Dim yB As Integer = y.EndSite
            Return Not (xB < yA OrElse xA > yB)
        End Function

        ''' <summary>去除 secondary 后缀后用于比较的靶标标识。</summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function NormalizeTarget(t As String) As String
            If t Is Nothing Then
                Return Nothing
            End If
            If t.EndsWith("_secondary") Then
                Return t.Substring(0, t.Length - "_secondary".Length)
            End If
            Return t
        End Function

        ''' <summary>
        ''' 合并两个同源命中：保留坐标，互补注释期望值、错配/G:U/gap 计数，
        ''' 标记是否任一算法判为翻译抑制候选。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function MergeHit(x As siRNAHit, y As siRNAHit) As siRNAHit
            Dim merged As New siRNAHit With {
                .miRNA = x.miRNA,
                .Target = NormalizeTarget(x.Target),
                .StartSite = If(x.StartSite <= y.StartSite, x.StartSite, y.StartSite),
                .EndSite = If(x.EndSite >= y.EndSite, x.EndSite, y.EndSite),
                .MismatchCount = Math.Max(x.MismatchCount, y.MismatchCount),
                .WobbleCount = Math.Max(x.WobbleCount, y.WobbleCount),
                .GapCount = Math.Max(x.GapCount, y.GapCount),
                .TranslationInhibition = x.TranslationInhibition OrElse y.TranslationInhibition,
                .Source = "Intersection(psRNATarget+TargetFinder)",
                .Alignment = $"psRNATarget E={x.Expectation:F2} | TargetFinder Penalty={y.Expectation:F2}"
            }
            merged.Length = merged.EndSite - merged.StartSite + 1
            Return merged
        End Function
    End Class
End Namespace
