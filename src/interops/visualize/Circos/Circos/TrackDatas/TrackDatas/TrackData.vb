#Region "Microsoft.VisualBasic::b96c3570fbeb706ddec4c0230f763866, ..\interops\visualize\Circos\Circos\TrackDatas\TrackDatas\TrackData.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TrackDatas

    ''' <summary>
    ''' + Finally, links are a special track type which associates two ranges together. The format For links Is analogous To other data types, 
    ''' except now two coordinates are specified.
    ''' 
    ''' ```
    ''' chr12 1000 5000 chr15 5000 7000
    ''' ```
    ''' </summary>
    Public Structure link : Implements ITrackData

        Dim a As TrackData
        Dim b As TrackData

        Public Property comment As String Implements ITrackData.comment

        Public Overrides Function ToString() As String Implements ITrackData.GetLineData
            Return a.ToString & " " & b.ToString
        End Function
    End Structure

    Public Interface ITrackData
        Property comment As String
        ''' <summary>
        ''' Usually Using <see cref="TrackData.ToString()"/> method for creates tracks data document.
        ''' </summary>
        ''' <returns></returns>
        Function GetLineData() As String
    End Interface

    Public Structure Connection
        Implements ITrackData

        Public Property comment As String Implements ITrackData.comment
        Public Property from As Integer
        Public Property [to] As Integer
        Public Property chr As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public ReadOnly Property IsEmpty As Boolean
            Get
                If Not String.IsNullOrEmpty(chr) OrElse from > 0 OrElse [to] > 0 Then
                    Return False
                Else
                    Return True
                End If
            End Get
        End Property

        Public Function GetLineData() As String Implements ITrackData.GetLineData
            Return $"{chr} {from} {[to]}"
        End Function
    End Structure

    ''' <summary>
    ''' Data for tracks is loaded from a plain-text file. Each data point is stored on a 
    ''' separate line, except for links which use two lines per link.
    ''' 
    ''' The definition Of a data point within a track Is based On the genomic range, 
    ''' which Is a combination Of chromosome And start/End position.
    ''' </summary>
    ''' <remarks>
    ''' Data for tracks is loaded from a plain-text file. Each data point is stored on a separate line, except for links which use two lines per link.
    ''' The definition Of a data point within a track Is based On the genomic range, which Is a combination Of chromosome And start/End position. 
    ''' For example,
    ''' 
    ''' ```
    ''' # the basis for a data point Is a range
    ''' chr12 1000 5000
    ''' ```
    ''' 
    ''' All data values, regardless Of track type, will be positioned Using a range rather than a Single position. To explicitly specify a Single position, 
    ''' use a range With equal start And End positions.
    ''' 
    ''' + Tracks such As scatter plot, line plot, histogram Or heat map, associate a value With Each range. The input To this kind Of track would be
    ''' 
    ''' ```
    ''' # scatter, line, histogram And heat maps require a value
    ''' chr12 1000 5000 0.25
    ''' ```
    ''' 
    ''' + The exception Is a stacked histogram, which associates a list Of values With a range.
    ''' 
    ''' ```
    ''' # stacked histograms take a list of values
    ''' chr12 1000 5000 0.25,0.35,0.60
    ''' ```
    ''' 
    ''' + The value For a text track Is interpreted As a text label (other tracks require that this field be a floating point number).
    ''' 
    ''' ```
    ''' # value for text tracks Is interpreted as text
    ''' chr12 1000 5000 geneA
    ''' ```
    ''' 
    ''' + The tile track does Not take a value—only a range.
    ''' 
    ''' ```
    ''' chr12 1000 5000
    ''' ```
    ''' 
    ''' + Finally, links are a special track type which associates two ranges together. The format For links Is analogous To other data types, 
    ''' except now two coordinates are specified.
    ''' 
    ''' ```
    ''' chr12 1000 5000 chr15 5000 7000
    ''' ```
    ''' 
    ''' + In addition to the chromosome, range And (if applicable) value, each data point can be annotated with formatting parameters that control how the point Is drawn. 
    ''' The parameters need to be compatible with the track type for which the file Is destined. Thus, a scatter plot data point might have
    ''' 
    ''' ```
    ''' chr12 1000 5000 0.25 glyph_size=10p,glyph=circle
    ''' ```
    ''' 
    ''' whereas a histogram data point might include the Option To fill the data value's bin
    ''' 
    ''' ```
    ''' chr12 1000 5000 0.25 fill_color=orange
    ''' ```
    ''' 
    ''' + Other features, such As URLs, can be associated With any data point. For URLs the parameter can contain parsable fields (e.g. [start]) which 
    ''' are populated automatically With the data point's associated property.
    ''' 
    ''' ```
    ''' # the URL for this point would be
    ''' # http://domain.com/script?start=1000&amp;end=5000&amp;chr=chr12
    ''' chr12 1000 5000 0.25 url=http//domain.com/script?start=[start]&amp;end=[end]&amp;chr=[chr]
    ''' ```
    ''' </remarks>
    Public MustInherit Class TrackData
        Implements ITrackData
        Implements IAddress(Of Integer)

        ''' <summary>
        ''' Chromosomes name
        ''' </summary>
        Public Property chr As String
        Public Property start As Integer Implements IAddress(Of Integer).Address
        Public Property [end] As Integer
        Public Property formatting As Formatting
        Public Property comment As String Implements ITrackData.comment

        ''' <summary>
        ''' Using <see cref="ToString()"/> method for creates tracks data document.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String Implements ITrackData.GetLineData
            Dim s As String = __trackData()
            Dim format As String = formatting.ToString

            If Not String.IsNullOrEmpty(format) Then
                s &= " " & format
            End If

            Return s
        End Function

        Protected MustOverride Function __trackData() As String

    End Class

    ''' <summary>
    ''' 这个只是用来表示基因组上面的一个区域位置
    ''' </summary>
    Public Class RegionTrackData : Inherits TrackData

        Protected Overrides Function __trackData() As String
            Return $"{chr} {start} {[end]}"
        End Function
    End Class

    ''' <summary>
    ''' Annotated with formatting parameters that control how the point Is drawn. 
    ''' </summary>
    Public Structure Formatting

        ''' <summary>
        ''' Only works in scatter, example is ``10p``
        ''' </summary>
        Dim glyph_size As String
        ''' <summary>
        ''' Only works in scatter, example is ``circle``
        ''' </summary>
        Dim glyph As String
        ''' <summary>
        ''' Works on histogram
        ''' </summary>
        Dim fill_color As String
        ''' <summary>
        ''' Works on any <see cref="Trackdata"/> data type.
        ''' </summary>
        Dim URL As String

        Public Overrides Function ToString() As String
            Dim s As New StringBuilder

            Call __attach(s, NameOf(glyph), glyph)
            Call __attach(s, NameOf(glyph_size), glyph_size)
            Call __attach(s, NameOf(fill_color), fill_color)
            Call __attach(s, "url", URL)

            Return s.ToString
        End Function

        Private Shared Sub __attach(ByRef s As StringBuilder, name As String, value As String)
            If String.IsNullOrEmpty(value) Then
                Return
            End If

            If s.Length = 0 Then
                Call s.Append($"{name}={value}")
            Else
                Call s.Append($",{name}={value}")
            End If
        End Sub
    End Structure

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

        Protected Overrides Function __trackData() As String
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

        Protected Overrides Function __trackData() As String
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

        Protected Overrides Function __trackData() As String
            Return $"{chr} {start} {[end]} {text}"
        End Function
    End Class
End Namespace
