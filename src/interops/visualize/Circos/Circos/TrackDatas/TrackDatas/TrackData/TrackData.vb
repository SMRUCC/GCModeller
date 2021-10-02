#Region "Microsoft.VisualBasic::581c9c5009e094735aae4fb76cfee51c, visualize\Circos\Circos\TrackDatas\TrackDatas\TrackData\TrackData.vb"

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

    '     Class TrackData
    ' 
    '         Properties: [end], chr, comment, formatting, start
    ' 
    '         Function: ToString
    ' 
    '         Sub: Assign
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TrackDatas

    ''' <summary>
    ''' Data for tracks is loaded from a plain-text file. Each data point is stored on a 
    ''' separate line, except for links which use two lines per link.
    ''' 
    ''' The definition Of a data point within a track Is based On the genomic range, 
    ''' which Is a combination Of chromosome And start/End position.
    ''' 
    ''' (请注意，最终的行数据都是使用<see cref="ToString"/>方法来生成的，<see cref="trackData"/>
    ''' 生成的只是绘图的原始数据，最终的style以及formats是在<see cref="ToString"/>之中合成的，所以
    ''' 请不要轻易的重写继承类的<see cref="ToString"/>方法)
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
            Dim s As String = trackData()
            Dim format As String = formatting.ToString

            If Not String.IsNullOrEmpty(format) Then
                s &= " " & format
            End If

            Return s
        End Function

        Protected MustOverride Function trackData() As String

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Me.start = address
        End Sub
    End Class
End Namespace
