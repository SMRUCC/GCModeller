#Region "Microsoft.VisualBasic::cf954749ed0cf247b2ea32991461f929, visualize\Circos\Circos\ConfFiles\Nodes\Base\Plots.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class HeatMap
    ' 
    '         Properties: color, scale_log_base, type
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetProperties
    ' 
    '     Class Histogram
    ' 
    '         Properties: extend_bin, fill_color, type
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetProperties
    ' 
    '     Class TextLabel
    ' 
    '         Properties: color, label_font, label_size, label_snuggle, link_color
    '                     link_dims, link_thickness, max_snuggle_distance, padding, rpadding
    '                     show_links, snuggle_link_overlap_test, snuggle_link_overlap_tolerance, snuggle_refine, snuggle_sampling
    '                     snuggle_tolerance, type
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetProperties
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.NtProps

Namespace Configurations.Nodes.Plots

    ''' <summary>
    ''' Heat maps are used for data types which associate a value with a genomic position, Or region. 
    ''' As such, this track uses the same data format As histograms.
    '''
    ''' The track linearly maps a range Of values [min,max] onto a list Of colors c[n], i=0..N.
    '''
    ''' f = (value - min) / ( max - min )
    ''' n = N * f
    ''' </summary>
    Public Class HeatMap : Inherits TracksPlot(Of ValueTrackData)

        ''' <summary>
        ''' Colors are defined by a combination of lists or CSV. Color lists
        ''' exist For all Brewer palletes (see etc/colors.brewer.lists.conf) As
        ''' well As For N-Step hue (hue-sN, e.g. hue-s5 =
        ''' hue000,hue005,hue010,...) And N-color hue (hue-sN, e.g. hue-3 =
        ''' hue000,hue120,hue140).
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property color As String = "hs1_a5,hs1_a4,hs1_a3,hs1_a2,hs1_a1,hs1"
        ''' <summary>
        ''' If scale_log_base is used, the mapping is not linear, but a power law 
        '''
        ''' n = N * f**(1/scale_log_base)
        '''
        ''' When scale_log_base > 1 the dynamic range For values close To min Is expanded. 
        ''' When scale_log_base &lt; 1 the dynamic range For values close To max Is expanded. 
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property scale_log_base As String = "5"

        Public Sub New(data As data(Of ValueTrackData))
            Call MyBase.New(data)
        End Sub

        <Circos> Public Overrides ReadOnly Property type As String
            Get
                Return "heatmap"
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return SimpleConfig _
                .GenerateConfigurations(Of HeatMap)(Me) _
                .ToArray
        End Function
    End Class

    ''' <summary>
    ''' Histograms are a type Of track that displays 2D data, which
    ''' associates a value With a genomic position. Line plots, scatter
    ''' plots And heat maps are examples Of other 2D tracks.
    '''
    ''' The data format For 2D data Is 
    '''
    ''' #chr start End value [options]
    ''' ...
    ''' hs3 196000000 197999999 71.0000
    ''' hs3 198000000 199999999 57.0000
    ''' hs4 0 1999999 28.0000
    ''' hs4 2000000 3999999 40.0000
    ''' hs4 4000000 5999999 59.0000
    ''' ...
    '''
    ''' Each histogram Is defined In a ``&lt;plot>`` block within an enclosing ``&lt;plots`` block.
    ''' </summary>
    ''' <remarks>
    ''' Like For links, rules are used To dynamically alter formatting Of
    ''' Each data point (i.e. histogram bin). Here, I include the ```&lt;rule>```
    ''' block from a file, which contains the following
    ''' </remarks>
    Public Class Histogram : Inherits TracksPlot(Of ValueTrackData)

        <Circos> Public Overrides ReadOnly Property type As String
            Get
                Return "histogram"
            End Get
        End Property

        ''' <summary>
        ''' Histograms can have both a fill And outline. The Default outline Is 1px thick black. 
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Overrides Property fill_color As String = "vdgrey"

        ''' <summary>
        ''' Do Not join histogram bins that Do Not abut.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property extend_bin As String = no

        Public Sub New(data As data(Of ValueTrackData))
            Call MyBase.New(data)
        End Sub

        Sub New(data As GenomeGCContent)
            Call MyBase.New(data)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return SimpleConfig _
                .GenerateConfigurations(Of Histogram)(Me) _
                .ToArray
        End Function
    End Class

    ''' <summary>
    ''' Like with other tracks, text is limited to a radial range by setting
    ''' ``r0`` And ``r1``.
    '''
    ''' Individual labels can be repositioned automatically With In a
    ''' position window To fit more labels, without overlap. This Is an
    ''' advanced feature - see the 2D Track text tutorials.
    ''' </summary>
    Public Class TextLabel : Inherits TracksPlot(Of TextTrackData)

        <Circos> Public Property color As String = "black"
        <Circos> Public Property label_size As String = "16"
        ''' <summary>
        ''' For a list of fonts, see ``etc/fonts.conf`` in the Circos distribution.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property label_font As String = "light"
        ''' <summary>
        ''' text margin in angular direction
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property padding As String = "5p"
        ''' <summary>
        ''' text margin in radial direction
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property rpadding As String = "5p"
        ''' <summary>
        ''' Short lines can be placed before the label to connect them to the
        ''' label's position. This is most useful when the labels are
        ''' rearranged.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property show_links As String = yes
        <Circos> Public Property link_dims As String = "5p,4p,8p,4p,0p"
        <Circos> Public Property link_thickness As String = "1p"
        <Circos> Public Property link_color As String = "dgrey"
        <Circos> Public Property label_snuggle As String = yes
        <Circos> Public Property max_snuggle_distance As String = "2.0r"
        <Circos> Public Property snuggle_sampling As String = "1"
        <Circos> Public Property snuggle_tolerance As String = "0.25r"
        <Circos> Public Property snuggle_link_overlap_test As String = yes
        <Circos> Public Property snuggle_link_overlap_tolerance As String = "2p"
        <Circos> Public Property snuggle_refine As String = yes

        <Circos> Public Overrides ReadOnly Property type As String
            Get
                Return "text"
            End Get
        End Property

        ''' <summary>
        ''' 创建一个圈用来显示位点的标签文本信息
        ''' </summary>
        ''' <param name="data"></param>
        Sub New(data As data(Of TextTrackData))
            Call MyBase.New(data)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function GetProperties() As String()
            Return SimpleConfig _
                .GenerateConfigurations(Of TextLabel)(Me) _
                .ToArray
        End Function
    End Class
End Namespace
