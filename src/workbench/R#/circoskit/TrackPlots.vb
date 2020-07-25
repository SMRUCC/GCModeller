Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

<Package("track.plots")>
Module TrackPlots

    ''' <summary>
    ''' Tracks such As scatter plot, line plot, histogram Or heat map, 
    ''' associate a value With Each range. The input To this kind Of 
    ''' track would be
    ''' 
    ''' ```
    ''' # scatter, line, histogram And heat maps require a value
    ''' chr12 1000 5000 0.25
    ''' ```
    ''' </summary>
    ''' <param name="start"></param>
    ''' <param name="end"></param>
    ''' <param name="value"></param>
    ''' <param name="chr"></param>
    ''' <returns></returns>
    <ExportAPI("value")>
    Public Function ValueTrackData(start%, end%, value#, Optional chr As String = "chr1") As ValueTrackData
        Return New ValueTrackData With {
            .chr = chr,
            .[end] = [end],
            .start = start,
            .value = value
        }
    End Function

    ''' <summary>
    ''' The value For a text track Is interpreted As a text label 
    ''' (other tracks require that this field be a floating point 
    ''' number).
    ''' 
    ''' ```
    ''' # value for text tracks Is interpreted as text
    ''' chr12 1000 5000 geneA
    ''' ```
    ''' </summary>
    ''' <param name="start"></param>
    ''' <param name="end"></param>
    ''' <param name="text"></param>
    ''' <param name="chr"></param>
    ''' <returns></returns>
    <ExportAPI("text")>
    Public Function TextTrackData(start%, end%, text$, Optional chr As String = "chr1") As TextTrackData
        Return New TextTrackData With {
            .start = start,
            .chr = chr,
            .[end] = [end],
            .text = text
        }
    End Function
End Module
