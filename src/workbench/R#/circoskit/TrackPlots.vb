Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' package module for create track plots or config track plots
''' </summary>
<Package("track.plots")>
Module TrackPlots

    ''' <summary>
    ''' Invoke set the color of the circle element on the circos plots.
    ''' </summary>
    ''' <param name="track"></param>
    ''' <param name="color">The name of the color in the circos program.</param>
    ''' <returns></returns>
    <ExportAPI("fill_color")>
    Public Function SetTrackFillColor(track As ITrackPlot, color As String) As ITrackPlot
        track.fill_color = color
        Return track
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="track"></param>
    ''' <param name="orientation">ori = "in" or "out"</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("orientation")>
    Public Function SetTrackOrientation(track As ITrackPlot, Optional orientation As orientations = orientations.in) As ITrackPlot
        track.orientation = orientation
        Return track
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="track"></param>
    ''' <param name="r1">The radius value of the outside for this circle element.</param>
    ''' <param name="r0">The radius value of the inner circle of this element.</param>
    ''' <returns></returns>
    <ExportAPI("radius01")>
    Public Function SetPlotElementPosition(track As ITrackPlot, r1 As Double, r0 As Double) As ITrackPlot
        track.r1 = r1 & "r"
        track.r0 = r0 & "r"
        Return track
    End Function

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

    <ExportAPI("track.text")>
    Public Function TrackText(<RRawVectorArgument> texts As Object,
                              Optional r0$ = "0.90r",
                              Optional r1$ = "0.995r",
                              Optional snuggle_refine$ = "yes",
                              Optional label_snuggle$ = "yes",
                              Optional env As Environment = Nothing) As Object

        Dim textPoints As pipeline = pipeline.TryCreatePipeline(Of TextTrackData)(texts, env)

        If textPoints.isError Then
            Return textPoints.getError
        End If

        Dim labelText As New HighlightLabel(textPoints.populates(Of TextTrackData)(env))
        Dim labels As New TextLabel(labelText) With {
            .r0 = r0,
            .r1 = r1,
            .snuggle_refine = snuggle_refine,
            .label_snuggle = label_snuggle
        }

        If labels.tracksData.GetEnumerator.Count = 0 Then
            Return Internal.debug.stop("the text track data can not be empty!", env)
        Else
            Return labels
        End If
    End Function

    <ExportAPI("track.highlight")>
    <RApiReturn(GetType(HighLight))>
    Public Function HeatMapping(highlights As Highlights, Optional colors$ = ColorMap.PatternJet, Optional env As Environment = Nothing) As Object
        Dim hTrack As New HighLight(highlights)

        If hTrack.tracksData.GetEnumerator.Count = 0 Then
            Return Internal.debug.stop("the value points in the track data can not be empty!", env)
        Else
            Return hTrack
        End If
    End Function

    <ExportAPI("track.histogram")>
    Public Function Histogram(<RRawVectorArgument> values As Object, Optional env As Environment = Nothing) As Object
        Dim valuePoints As pipeline = pipeline.TryCreatePipeline(Of ValueTrackData)(values, env)

        If valuePoints.isError Then
            Return valuePoints.getError
        End If

        Dim hist As New Histogram(New NtProps.GCSkew(valuePoints.populates(Of ValueTrackData)(env)))

        If hist.tracksData.GetEnumerator.Count = 0 Then
            Return Internal.debug.stop("the value points in the track data can not be empty!", env)
        Else
            Return hist
        End If
    End Function
End Module
