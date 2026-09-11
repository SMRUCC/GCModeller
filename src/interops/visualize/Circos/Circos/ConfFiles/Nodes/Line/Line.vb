Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace Configurations.Nodes.Plots.Lines

    ''' <summary>
    ''' Line plots are useful for showing values associated with a position on the
    ''' ideogram, such as sequence conservation scores, or values derived from a
    ''' sliding window scan of the genome.
    '''
    ''' Line plots use the same data format as scatter plots, histograms and heat maps.
    '''
    ''' ```
    ''' #chr start end value [options]
    ''' hs5 50 75 0.75
    ''' ```
    ''' </summary>
    ''' <remarks>
    ''' The default values of this plot are taken from the official template
    ''' ``etc/tracks/line.conf`` in the circos distribution.
    ''' </remarks>
    Public Class LinePlot : Inherits TrackPlot(Of ValueTrackData)

        <Circos> Public Overrides ReadOnly Property type As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return "line"
            End Get
        End Property

        ''' <summary>
        ''' The line color parameter
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property color As String = "black"
        ''' <summary>
        ''' 当相邻的数据点之间的间隔大于所给定的阈值的时候，不再将这些数据点连接起来
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' For example ``max_gap = 1u`` splits the line into separate segments whenever
        ''' two neighbouring data points are separated by a gap larger than one chrom
        ''' osome unit.
        ''' </remarks>
        <Circos> Public Property max_gap As String = null

        Sub New(data As TrackDataDocument(Of ValueTrackData))
            Call MyBase.New(data)

            ' 默认值取自 circos 发行版之中的 etc/tracks/line.conf
            thickness = "1"
            r1 = "0.69r"
            r0 = "0.60r"
            orientation = Orientation.Out
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return Me.GenerateConfigLines()
        End Function
    End Class
End Namespace
