Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports F = Microsoft.VisualBasic.Math.Statistics.FisherTest

''' <summary>
''' 基于Fisher Extract test算法的富集分析
''' </summary>
Public Module Enrichment

    <Extension>
    Public Iterator Function Enrichment(genome As Genome,
                                        list As IEnumerable(Of String),
                                        Optional outputAll As Boolean = False,
                                        Optional showProgress As Boolean = True) As IEnumerable(Of EnrichmentResult)

        Dim genes% = genome.clusters _
                           .Select(Function(c) c.Members) _
                           .IteratesALL _
                           .Distinct _
                           .Count
        Dim doProgress As Action(Of String)
        Dim progress As ProgressBar = Nothing
        Dim tick As New ProgressProvider(genome.clusters.Length)
        Dim ETA$

        If showProgress Then
            progress = New ProgressBar("Do enrichment...")
            doProgress = Sub(id)
                             ETA = $"{id}.... ETA: {tick.ETA(progress.ElapsedMilliseconds)}"
                             progress.SetProgress(tick.StepProgress, ETA)
                         End Sub
        Else
            doProgress = Sub()
                             ' Do Nothing
                         End Sub
        End If

        With list.ToArray
            For Each cluster In genome.clusters
                Dim enriched$() = cluster.Intersect(.ByRef).ToArray
                Dim a% = enriched.Length
                Dim b% = cluster.Members.Length
                Dim c% = .Length
                Dim d% = genes
                Dim pvalue# = F.FisherPvalue(a, b, c, d)
                Dim score# = a / b

                Call doProgress(cluster.ID)

                If (pvalue.IsNaNImaginary OrElse enriched.Length = 0) AndAlso Not outputAll Then
                    Continue For
                End If

                Yield New EnrichmentResult With {
                    .term = cluster.ID,
                    .geneIDs = enriched,
                    .pvalue = pvalue,
                    .score = score,
                    .cluster = b,
                    .enriched = $"{a}/{c}"
                }
            Next
        End With

        If Not progress Is Nothing Then
            progress.Dispose()
        End If
    End Function

    <Extension>
    Public Function FDRCorrection(enrichments As IEnumerable(Of EnrichmentResult)) As EnrichmentResult()
        With enrichments.Shadows
            !FDR = !Pvalue.FDR
            Return .ByRef.ToArray
        End With
    End Function
End Module
