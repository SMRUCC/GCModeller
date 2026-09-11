Namespace Configurations.Nodes.Plots

    ''' <summary>
    ''' Scatter plots are useful for showing the position of a probe, such As
    ''' SNP data, on the genome.
    '''
    ''' The scatter plot uses the same data format As line plots, histograms And heat maps.
    '''
    ''' ```
    ''' #chr start End value [options]
    ''' hs5 50 75 0.75
    ''' ```
    ''' </summary>
    ''' <remarks>
    ''' The size And shape Of the points are controlled by the *glyph* And
    ''' *glyph_size* parameters. Circos distributions ship With a number Of
    ''' glyphs shown In the etc/tracks/scatter.conf of the Circos distribution.
    '''
    ''' Glyph options are one Of 
    ''' 
    ''' + ``circle``, 
    ''' + ``rectangle``, 
    ''' + ``triangle``, 
    ''' + ``cross``, 
    ''' + ``plus``, 
    ''' + ``square`` 
    '''
    ''' and the glyph size parameter is given in pixels.
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
        ''' The fill color of the glyph
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property fill_color As String = "grey"
        ''' <summary>
        ''' The outline (stroke) color of the glyph
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property stroke_color As String = "black"
        ''' <summary>
        ''' The outline (stroke) thickness of the glyph, use 0 to turn off the outline.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property stroke_thickness As String = "0"
        ''' <summary>
        ''' The primary color parameter of the glyph.(和<see cref="fill_color"/>同义，二者任意一个即可)
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property color As String = null

        ''' <summary>
        ''' Do not connect plots if the gap Is larger than a limit. This limit Is set 
        ''' by max_gap And Is useful to automatically skip gaps larger than a given size.
        '''
        ''' In this way it Isn't necessary to split data into blocks that reflect absense Of data 
        ''' over a large region (e.g. centromere, heterochromatin) since the gap created by 
        ''' missing data will automatically terminate draw枝条 And continue one河市.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property max_gap As String = null

        <Circos> Public Property r1 As String = "0.79r"
        <Circos> Public Property r0 As String = "0.70r"
        <Circos> Public Property orientation As orientations = orientations.out

        Sub New(data As data(Of ValueTrackData))
            Call MyBase.New(data)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return Me.GenerateConfigLines()
        End Function
    End Class
End Namespace
