Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace Configurations.Nodes.Plots

    ''' <summary>
    ''' Scatter plots are useful for showing the position of a probe, such as
    ''' SNP data, on the genome.
    '''
    ''' Scatter plots use the same data format as line plots, histograms and heat maps.
    '''
    ''' ```
    ''' #chr start end value [options]
    ''' hs5 50 75 0.75
    ''' ```
    ''' </summary>
    ''' <remarks>
    ''' The size and shape of the points are controlled by the ``glyph`` and
    ''' ``glyph_size`` parameters.
    '''
    ''' The available glyph shapes are: 
    ''' 
    ''' + ``circle``
    ''' + ``rectangle``
    ''' + ``triangle``
    ''' + ``cross``
    ''' + ``plus``
    ''' + ``square``
    '''
    ''' (see the glyph reference in the circos tutorials for the complete list)
    '''
    ''' The default values of this plot are taken from the official template
    ''' ``etc/tracks/scatter.conf`` in the circos distribution.
    ''' </remarks>
    Public Class ScatterPlot : Inherits TracksPlot(Of ValueTrackData)

        <Circos> Public Overrides ReadOnly Property type As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return "scatter"
            End Get
        End Property

        ''' <summary>
        ''' Shape of the glyph: circle / rectangle / triangle / cross / plus / square
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property glyph As String = "circle"
        ''' <summary>
        ''' Size of the glyph, in pixels.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property glyph_size As String = "10"
        ''' <summary>
        ''' The primary color parameter of the glyph.
        ''' (与继承得到的<see cref="TracksPlot(Of T).fill_color"/>是同义参数，二者使用任意一个即可)
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property color As String = null
        ''' <summary>
        ''' 当相邻的数据点之间的间隔大于所给定的阈值的时候，不再将这些数据点连接起来
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' For example ``max_gap = 1u`` is useful to automatically skip gaps larger than
        ''' a given size, in this way it isn't necessary to split data into blocks that
        ''' reflect absence of data over a large region (e.g. centromere, heterochromatin).
        ''' </remarks>
        <Circos> Public Property max_gap As String = null

        Sub New(data As data(Of ValueTrackData))
            Call MyBase.New(data)

            ' 默认值取自 circos 发行版之中的 etc/tracks/scatter.conf
            fill_color = "grey"
            stroke_color = "black"
            stroke_thickness = "0"
            r1 = "0.79r"
            r0 = "0.70r"
            orientation = orientations.out
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return Me.GenerateConfigLines()
        End Function
    End Class
End Namespace
