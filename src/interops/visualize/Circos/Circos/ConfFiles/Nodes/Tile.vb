Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace Configurations.Nodes.Plots

    ''' <summary>
    ''' Tile tracks are the Circos equivalent of a genome browser.
    '''
    ''' Which is to say, the tile track shows interval data and automatically places
    ''' multiple intervals in vertical stacks so that they do not overlap and do not
    ''' extend outside of the track.
    '''
    ''' The tile track does not take a value --- only a range.
    '''
    ''' ```
    ''' #chr start end [options]
    ''' chr12 1000 5000
    ''' ```
    ''' </summary>
    ''' <remarks>
    ''' The default values of this plot are taken from the official template
    ''' ``etc/tracks/tile.conf`` in the circos distribution.
    ''' </remarks>
    Public Class TilePlot : Inherits TrackPlot(Of RegionTrackData)

        <Circos> Public Overrides ReadOnly Property type As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return "tile"
            End Get
        End Property

        ''' <summary>
        ''' Number of stacked layers of tiles. When there is "too much" data,
        ''' many tiles will be stacked on top of one another.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property layers As String = "10"
        ''' <summary>
        ''' Whether the track overflows its reserved vertical space should all 
        ''' <see cref="layers"/> be full.
        '''
        ''' + ``hide``(默认值): any tile that is not able to fit within the requested 
        '''   number of layers is hidden.
        ''' + ``grow``: this restriction is lifted and the tiles continue to be stacked.
        ''' </summary>
        ''' <returns>``hide`` 或者 ``grow``</returns>
        <Circos> Public Property layers_overflow As String = "hide"
        ''' <summary>
        ''' How are the track dimensions interpreted?
        '''
        ''' One of two options: ``whratio``(default) or ``widths``. This parameter is
        ''' used together with <see cref="layers"/>: in the default ``whratio`` mode
        ''' each layer has a width-to-height ratio derived from the remaining radial space.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property layout_orientation_mode As String = "whratio"
        ''' <summary>
        ''' Tile margin in the radial direction.(设置为``0``关闭)
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property margin As String = "1u"
        ''' <summary>
        ''' Tile padding in the angular direction, in units of pixels.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property padding As String = "3"

        Sub New(data As TrackDataDocument(Of RegionTrackData))
            Call MyBase.New(data)

            ' 默认值取自 circos 发行版之中的 etc/tracks/tile.conf
            fill_color = "grey"
            stroke_color = "vlgrey"
            stroke_thickness = "1"
            thickness = "10"
            r1 = "0.55r"
            r0 = "0.40r"
            orientation = Orientation.Out
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return Me.GenerateConfigLines()
        End Function
    End Class
End Namespace
