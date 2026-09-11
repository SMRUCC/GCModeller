Imports System.Runtime.CompilerServices

Namespace Configurations.Nodes.Plots

    ''' <summary>
    ''' Tile tracks are the Circos equivalent of a genome browser.
    '''
    ''' Which Is To say, the tile track show interval data And automatically place Multiple 
    ''' intervals In vertical stacks so that they Do not overlap And do not extend outside the track.
    '''
    ''' The tile track does Not take a value --- only a range.
    '''
    ''' ```
    ''' #chr start End [options]
    ''' chr12 1000 5000
    ''' ```
    ''' </summary>
    ''' <remarks>
    ''' The default values of this plot are taken from the official template
    ''' ``etc/tracks/tile.conf`` in the circos distribution.
    ''' </remarks>
    Public Class TilePlot : Inherits TracksPlot(Of RegionTrackData)

        <Circos> Public Overrides ReadOnly Property type As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return "tile"
            End Get
        End Property

        ''' <summary>
        ''' Whether the track overflows its reserved vertical space should all *layers* be full.
        ''' 
        ''' When layers_overflow = hide the default, any tile that is not 
        ''' able to fit within the number of requested layers is hidden. 
        ''' When layers_overflow = grow this restriction Is lifted And the tiles 
        ''' continue To be stacked.
        ''' </summary>
        ''' <returns>``hide``(默认值) 或者 ``grow``</returns>
        <Circos> Public Property layers_overflow As String = "hide"
        ''' <summary>
        ''' How are track dimensions interpreted?
        '''
        ''' This crop]): mismatched one Of Two options ``whratio``(default) Or ``widths``.
        '''
        ''' This parameter is used together with <see cref="layers"/>, in the default
        ''' ``whratio`` mode each layer having a width-to-height ratio given by the
        ''' remaining space.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property layout_orientation_mode As String = "whratio"
        ''' <summary>
        ''' Number Of stacked layers Of tiles. When there is "too much" data many Tiles will 
        ''' be stacked On top Of one another.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property layers As String = "10"
        ''' <summary>
        ''' Tile margin in radial direction, automatically applied from the
        ''' ideogram radius向内 Correction 2 pixels outwards If you need more space.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property margin As String = "1u"
        ''' <summary>
        ''' Tile padding in the angular direction, in units of pixels.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property padding As String = "3"
        ''' <summary>
        ''' Tile thickness (height), in pixels.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property thickness As String = "10"

        Sub New(data As data(Of RegionTrackData))
            Call MyBase.New(data)

            ' 默认值取自 circos 发行版之中的 etc/tracks/tile.conf
            fill_color = "grey"
            stroke_color = "vlgrey"
            stroke_thickness = "1"
            r1 = "0.55r"
            r0 = "0.40r"
            orientation = orientations.out
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return Me.GenerateConfigLines()
        End Function
    End Class
End Namespace
