Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace GdiPlus.Tracks

    ''' <summary>
    ''' 轨道（track）的径向几何换算：把数据值映射为画布半径。
    ''' 
    ''' + ``orientation = out``：数据从内圈（r0）向外圈（r1）生长
    ''' + ``orientation = in``：数据从外圈（r1）向内圈（r0）生长
    ''' </summary>
    Public Class TrackGeometry

        Public ReadOnly Property InnerRadius As Double
        Public ReadOnly Property OuterRadius As Double
        Public ReadOnly Property Orientation As Orientation
        Public ReadOnly Property MinValue As Double
        Public ReadOnly Property MaxValue As Double

        Public Sub New(track As ITrackPlot, ctx As GdiRenderContext)
            Dim r0 As Double = ctx.Radius(track.r0, ctx.Canvas.ImageRadius * 0.6)
            Dim r1 As Double = ctx.Radius(track.r1, ctx.Canvas.ImageRadius * 0.75)

            InnerRadius = Math.Min(r0, r1)
            OuterRadius = Math.Max(r0, r1)
            Orientation = track.orientation

            Dim range = dataRange(track)

            MinValue = range.min
            MaxValue = range.max
        End Sub

        ''' <summary>
        ''' 数据值为最小值时的半径（数据的基线）
        ''' </summary>
        Public ReadOnly Property Baseline As Double
            Get
                If Orientation = Orientation.out Then
                    Return InnerRadius
                Else
                    Return OuterRadius
                End If
            End Get
        End Property

        ''' <summary>
        ''' 数据值为最大值时的半径
        ''' </summary>
        Public ReadOnly Property Full As Double
            Get
                If Orientation = Orientation.out Then
                    Return OuterRadius
                Else
                    Return InnerRadius
                End If
            End Get
        End Property

        ''' <summary>
        ''' 数据值 -&gt; 半径（像素）
        ''' </summary>
        Public Function RadiusOf(value As Double) As Double
            Dim f As Double = FractionOf(value)

            If Orientation = Orientation.out Then
                Return InnerRadius + f * (OuterRadius - InnerRadius)
            Else
                Return OuterRadius - f * (OuterRadius - InnerRadius)
            End If
        End Function

        ''' <summary>
        ''' 数据值 -&gt; [0,1] 的比例值
        ''' </summary>
        Public Function FractionOf(value As Double) As Double
            Dim span As Double = MaxValue - MinValue
            Dim f As Double

            If span <= 0 Then
                f = 0
            Else
                f = (value - MinValue) / span
            End If

            If f < 0 Then f = 0
            If f > 1 Then f = 1

            Return f
        End Function

        ''' <summary>
        ''' 比例值 -&gt; 半径（像素）
        ''' </summary>
        Public Function RadiusOfFraction(f As Double) As Double
            If f < 0 Then f = 0
            If f > 1 Then f = 1

            If Orientation = Orientation.out Then
                Return InnerRadius + f * (OuterRadius - InnerRadius)
            Else
                Return OuterRadius - f * (OuterRadius - InnerRadius)
            End If
        End Function

        Private Shared Function dataRange(track As ITrackPlot) As (min As Double, max As Double)
            Dim values As New List(Of Double)

            If track.tracksData IsNot Nothing Then
                For Each d As ITrackData In track.tracksData.GetEnumerator()
                    Dim v = TryCast(d, ValueTrackData)

                    If v IsNot Nothing AndAlso Not Double.IsNaN(v.value) AndAlso Not Double.IsInfinity(v.value) Then
                        values.Add(v.value)
                    End If
                Next
            End If

            If values.Count = 0 Then
                Return (0, 1)
            End If

            Return (values.Min(), values.Max())
        End Function
    End Class
End Namespace
