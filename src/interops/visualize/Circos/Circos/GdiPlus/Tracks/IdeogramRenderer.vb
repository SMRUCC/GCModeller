Imports System.Drawing
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes
Imports CircosDoc = SMRUCC.genomics.Visualize.Circos.Configurations.Circos

Namespace GdiPlus.Tracks

    ''' <summary>
    ''' 基因组骨架圈（ideogram / cytogenetic bands / 标签）与刻度（ticks）的绘制
    ''' </summary>
    Public Module IdeogramRenderer

        ''' <summary>
        ''' ideogram 的径向度量信息
        ''' </summary>
        Public Structure IdeogramMetrics
            Public Inner As Double
            Public Outer As Double
            Public ShowLabel As Boolean
            Public LabelRadius As Double
        End Structure

        Public Function Render(ctx As GdiRenderContext) As IdeogramMetrics
            Dim metrics As New IdeogramMetrics With {.Inner = 0, .Outer = 0}

            Dim include = ctx.Circos.Ideogram

            If include Is Nothing OrElse include.Ideogram Is Nothing Then
                Return metrics
            End If

            Dim idg As IdeogramBlock = include.Ideogram
            Dim centerR As Double = ctx.Radius(idg.radius, ctx.Canvas.ImageRadius * 0.85)
            Dim thickness As Double = ctx.Radius(idg.thickness, 30)

            If thickness <= 0 Then
                thickness = 1
            End If

            metrics.Inner = centerR - thickness / 2
            metrics.Outer = centerR + thickness / 2
            metrics.ShowLabel = CircosUnits.IsYes(idg.show_label)
            metrics.LabelRadius = ctx.Radius(idg.label_radius, metrics.Outer + 12)

            Dim fill As Boolean = CircosUnits.IsYes(idg.fill)
            Dim fillDefault As Color = ctx.ColorOf(idg.fill_color, Color.Black)
            Dim strokeThickness As Double = ctx.Radius(idg.stroke_thickness, 0)
            Dim strokeColor As Color = ctx.ColorOf(idg.stroke_color, Color.Black)

            For Each band As ChromosomeBand In ctx.Layout.Bands
                Dim color As Color = fillDefault

                If Not String.IsNullOrWhiteSpace(band.Entry.color) Then
                    color = ctx.ColorOf(band.Entry.color, fillDefault)
                ElseIf Not fill Then
                    color = Color.Transparent
                End If

                If fill Then
                    ctx.Canvas.FillAnnularSector(metrics.Inner, metrics.Outer, band.StartAngle, band.SweepAngle, color)
                End If

                If strokeThickness > 0 Then
                    ctx.Canvas.DrawArc(metrics.Outer, band.StartAngle, band.SweepAngle, strokeColor, CSng(strokeThickness))
                    ctx.Canvas.DrawArc(metrics.Inner, band.StartAngle, band.SweepAngle, strokeColor, CSng(strokeThickness))
                    ctx.Canvas.DrawRadialLine(band.StartAngle, metrics.Inner, metrics.Outer, strokeColor, CSng(strokeThickness))
                    ctx.Canvas.DrawRadialLine(band.EndAngle, metrics.Inner, metrics.Outer, strokeColor, CSng(strokeThickness))
                End If
            Next

            If CircosUnits.IsYes(idg.show_bands) AndAlso ctx.Circos.skeletonKaryotype IsNot Nothing Then
                RenderBands(ctx, metrics, idg)
            End If

            If metrics.ShowLabel Then
                RenderLabels(ctx, metrics, idg)
            End If

            Return metrics
        End Function

        Private Sub RenderBands(ctx As GdiRenderContext, metrics As IdeogramMetrics, idg As IdeogramBlock)
            Dim bandStroke As Double = ctx.Radius(idg.band_stroke_thickness, 0)

            For Each band In ctx.Circos.skeletonKaryotype.BandData
                Dim chromosome As ChromosomeBand = Nothing

                If Not ctx.Layout.TryGetBand(band.chrName, chromosome) Then
                    Continue For
                End If

                Dim startAngle As Double
                Dim sweepAngle As Double

                If Not ctx.Layout.AngleRangeOf(chromosome, band.start, band.end, startAngle, sweepAngle) Then
                    Continue For
                End If

                Dim color As Color = ctx.ColorOf(band.color, Color.Gray)

                Call ctx.Canvas.FillAnnularSector(metrics.Inner, metrics.Outer, startAngle, sweepAngle, color)

                If bandStroke > 0 Then
                    Call ctx.Canvas.DrawArc(metrics.Outer, startAngle, sweepAngle, Color.Black, CSng(bandStroke))
                End If
            Next
        End Sub

        Private Sub RenderLabels(ctx As GdiRenderContext, metrics As IdeogramMetrics, idg As IdeogramBlock)
            Dim size As Double = CircosUnits.ParseNumber(idg.label_size, 36)

            If size <= 0 Then
                size = 36
            End If

            ' circos 的 label_size 使用 pt 单位，画布使用像素，这里做一个简单的换算
            Dim font = ctx.CreateFont(size * 0.75)
            Dim upper As Boolean = String.Equals(idg.label_case, "upper", StringComparison.OrdinalIgnoreCase)

            For Each band As ChromosomeBand In ctx.Layout.Bands
                Dim text$ = band.Entry.chrLabel

                If String.IsNullOrEmpty(text) Then
                    text = band.Name
                End If
                If upper Then
                    text = text.ToUpperInvariant()
                End If

                Dim angle As Double = band.StartAngle + band.SweepAngle / 2

                Call ctx.Canvas.DrawRadialText(text, angle, metrics.LabelRadius, font, Color.Black)
            Next
        End Sub

#Region "Ticks"

        ''' <summary>
        ''' 绘制刻度与刻度标签（沿用文档之中的 ``ticks.conf`` 配置）
        ''' </summary>
        Public Sub RenderTicks(ctx As GdiRenderContext, ideogramOuter As Double)
            Dim include = findTicksInclude(ctx.Circos)

            If include Is Nothing OrElse include.Ticks Is Nothing Then
                Return
            End If
            If Not CircosUnits.IsYes(include.show_ticks) Then
                Return
            End If

            Dim block As TicksBlock = include.Ticks
            Dim showLabels As Boolean = CircosUnits.IsYes(include.show_tick_labels)
            Dim radiusBase As Double = ctx.Radius(block.radius, ideogramOuter)

            If radiusBase <= 0 Then
                radiusBase = ideogramOuter
            End If

            Dim color As Color = ctx.ColorOf(block.color, Color.Black)
            Dim multiplier As Double = CircosUnits.ParseNumber(block.multiplier, 1)
            Dim units As Double = CircosUnits.ParseNumber(ctx.Circos.chromosomes_units, 1)

            If units <= 0 Then
                units = 1
            End If

            For Each tick As TickBlock In block.ticks
                Dim spacingNt As Double = spacingInNt(tick.spacing, units)

                If spacingNt <= 0 Then
                    Continue For
                End If

                Dim tickSize As Double = CircosUnits.ParseNumber(tick.size, CircosUnits.ParseNumber(block.size, 20))
                Dim tickThickness As Double = CircosUnits.ParseNumber(block.thickness, 3)
                Dim tickColor As Color = ctx.ColorOf(tick.color, color)
                Dim showTickLabel As Boolean = showLabels AndAlso CircosUnits.IsYes(tick.show_label)
                Dim labelOffset As Double = CircosUnits.ParseNumber(
                    If(tick.label_offset, block.label_offset), 5)
                Dim labelSize As Double = CircosUnits.ParseNumber(tick.label_size, CircosUnits.ParseNumber(block.label_size, 36))
                Dim suffix$ = stripQuotes(tick.suffix)

                Dim labelFont = ctx.CreateFont(labelSize * 0.75)

                For Each band As ChromosomeBand In ctx.Layout.Bands
                    Dim first As Integer = 0
                    Dim count As Integer = CInt(Math.Floor(band.Length / spacingNt))

                    For i As Integer = first To count
                        Dim position As Double = band.Entry.start + i * spacingNt

                        If position > band.Entry.end Then
                            Exit For
                        End If

                        Dim angle As Double = ctx.Layout.AngleOf(band, CInt(position))

                        Call ctx.Canvas.DrawRadialLine(
                            angle,
                            radiusBase,
                            radiusBase + Math.Max(1, tickSize),
                            tickColor,
                            CSng(Math.Max(1, tickThickness)))

                        If showTickLabel Then
                            Dim label$ = CircosUnits.FormatTick(position * multiplier, tick.format) & suffix
                            Dim labelRadius As Double = radiusBase + Math.Max(1, tickSize) + labelOffset + labelSize * 0.5

                            Call ctx.Canvas.DrawRadialText(label, angle, labelRadius, labelFont, color)
                        End If
                    Next
                Next
            Next
        End Sub

        Private Function findTicksInclude(circos As CircosDoc) As TicksInclude
            If circos.includes Is Nothing Then
                Return Nothing
            End If

            For Each include As CircosConfig In circos.includes
                If TypeOf include Is TicksInclude Then
                    Return DirectCast(include, TicksInclude)
                End If
            Next

            Return Nothing
        End Function

        Private Function spacingInNt(expr As String, units As Double) As Double
            Dim s$ = If(expr, "").Trim()

            If s.Length = 0 Then
                Return 0
            End If

            If s.EndsWith("u", StringComparison.OrdinalIgnoreCase) Then
                Return CircosUnits.ParseNumber(s.Substring(0, s.Length - 1), 0) * units
            End If

            Return CircosUnits.ParseNumber(s, 0)
        End Function

        Private Function stripQuotes(text As String) As String
            Dim s$ = If(text, "").Trim()

            Do While s.Length >= 2 AndAlso s.StartsWith("""") AndAlso s.EndsWith("""")
                s = s.Substring(1, s.Length - 2).Trim()
            Loop

            Return s
        End Function

#End Region

    End Module
End Namespace
