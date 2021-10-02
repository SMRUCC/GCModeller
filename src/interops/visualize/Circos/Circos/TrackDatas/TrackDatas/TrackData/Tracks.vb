#Region "Microsoft.VisualBasic::5eef30d94e072d67ec7aa9a5a04677e4, visualize\Circos\Circos\TrackDatas\TrackDatas\TrackData\Tracks.vb"

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

    '     Class RegionTrackData
    ' 
    '         Function: trackData
    ' 
    '     Class ValueTrackData
    ' 
    '         Properties: value
    ' 
    '         Function: trackData
    ' 
    '     Class StackedTrackData
    ' 
    '         Properties: values
    ' 
    '         Function: trackData
    ' 
    '     Class TextTrackData
    ' 
    '         Properties: text
    ' 
    '         Function: trackData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace TrackDatas

    ''' <summary>
    ''' 这个只是用来表示基因组上面的一个区域位置
    ''' </summary>
    Public Class RegionTrackData : Inherits TrackData

        Protected Overrides Function trackData() As String
            Return $"{chr} {start} {[end]}"
        End Function
    End Class

    ''' <summary>
    ''' Tracks such As scatter plot, line plot, histogram Or heat map, associate a value With Each range. The input To this kind Of track would be
    ''' 
    ''' ```
    ''' # scatter, line, histogram And heat maps require a value
    ''' chr12 1000 5000 0.25
    ''' ```
    ''' </summary>
    ''' <remarks>
    ''' In addition to the chromosome, range And (if applicable) value, each data point can be annotated with formatting parameters that control how the point Is drawn. 
    ''' The parameters need to be compatible with the track type for which the file Is destined. Thus, a scatter plot data point might have
    ''' 
    ''' ```
    ''' chr12 1000 5000 0.25 glyph_size=10p,glyph=circle
    ''' ```
    ''' </remarks>
    Public Class ValueTrackData : Inherits TrackData
        Implements ITrackData

        Public Overridable Property value As Double

        Protected Overrides Function trackData() As String
            Return $"{chr} {start} {[end]} {value}"
        End Function
    End Class

    ''' <summary>
    ''' The exception Is a stacked histogram, which associates a list Of values With a range.
    ''' 
    ''' ```
    ''' # stacked histograms take a list of values
    ''' chr12 1000 5000 0.25,0.35,0.60
    ''' ```
    ''' </summary>
    Public Class StackedTrackData : Inherits TrackData

        Public Property values As Double()

        Protected Overrides Function trackData() As String
            Dim values As String = Me.values.Select(Function(d) d.ToString).JoinBy(",")
            Return $"{chr} {start} {[end]} {values}"
        End Function
    End Class

    ''' <summary>
    ''' The value For a text track Is interpreted As a text label (other tracks require that this field be a floating point number).
    ''' 
    ''' ```
    ''' # value for text tracks Is interpreted as text
    ''' chr12 1000 5000 geneA
    ''' ```
    ''' </summary>
    Public Class TextTrackData : Inherits TrackData

        Public Property text As String

        Protected Overrides Function trackData() As String
            Return $"{chr} {start} {[end]} {text}"
        End Function
    End Class
End Namespace
