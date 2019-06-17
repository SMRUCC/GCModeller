Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights
Imports CircosMain = SMRUCC.genomics.Visualize.Circos.Configurations.Circos

Module Legends

    Dim Margin As Integer = 200

    <ExportAPI("Circos.Add.Legends",
               Info:="If the NCBI alignment result plots was includes in your circos plots,
then you can using this method to adding the legends on your circos plots image when you have finish invoke drawing by the circos script program.")>
    Public Function DrawingImageAddLegend(doc As Configurations.Circos) As Image
        Dim ImagePath As String = FileIO.FileSystem.GetParentPath(doc.FilePath) & "/Circos.png"
        Dim CircosImage = Image.FromFile(ImagePath)

        Dim AlignmentData = doc.GetBlastAlignmentData
        Dim Font = New Font(FontFace.Ubuntu, 20)
        Dim sz As SizeF

        If Not AlignmentData.IsNullOrEmpty Then
            sz = AlignmentData.Keys.MaxLengthString.MeasureSize(New Size(1, 1).CreateGDIDevice, Font)
        Else
            sz = New SizeF(1, 20)
        End If

        Dim device = (New SizeF(CircosImage.Width + 3 * Margin + sz.Width * 2, CInt(CircosImage.Height + Margin * 4))).CreateGDIDevice
        Call device.Graphics.DrawImage(CircosImage, New Point(Margin, Margin))

        Dim refPt As Point = New Point(100, 100)

        If Not doc.Plots.IsNullOrEmpty Then

            For Each PlotElement As ITrackPlot In doc.Plots
                '   refPt = PlotElement.KaryotypeDocumentData.LegendsDrawing(refPt, Device)
            Next
        End If

        If Not AlignmentData.IsNullOrEmpty Then

            Font = New Font(FontFace.Ubuntu, 28)
            sz = AlignmentData.Keys.MaxLengthString.MeasureSize(device, Font)

            Dim dh = CInt(sz.Height)
            Dim Y As Integer = Margin * 3
            Dim X As Integer = CInt(device.Width - sz.Width - 2 * Margin)
            Dim ColorBlockSize As New SizeF(200, sz.Height)

            Call device.Graphics.DrawString("Localblast Alignment Order:", Font, Brushes.Black, New Point(X, Y))
            Y += 2 * dh

            For Each ID As NamedValue(Of String) In AlignmentData
                Call device.Graphics.DrawString(ID.Name, Font, Brushes.Black, New Point(X, Y))
                Call device.Graphics.FillRectangle(New SolidBrush(CircosColor.FromKnownColorName(ID.Value)), New RectangleF(New PointF(X - ColorBlockSize.Width - 10, Y), ColorBlockSize))

                Y += dh + 3
            Next
        End If

        Return device.ImageResource
    End Function

    ''' <summary>
    ''' 不可以使用并行拓展，因为有顺序之分
    ''' 
    ''' {SpeciesName, Color}
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <Extension>
    Public Function GetBlastAlignmentData(circos As CircosMain) As NamedValue(Of String)()
        Dim LQuery As NamedValue(Of String)() =
            LinqAPI.Exec(Of NamedValue(Of String)) <= From trackPlot As ITrackPlot
                                                      In circos.Plots
                                                      Where String.Equals(trackPlot.type, "highlight", StringComparison.OrdinalIgnoreCase) AndAlso
                                                          TypeOf trackPlot.tracksData Is BlastMaps
                                                      Let Alignment = DirectCast(trackPlot.tracksData, BlastMaps)
                                                      Select New NamedValue(Of String)(Alignment.SubjectSpecies, Alignment.SpeciesColor)
        Return LQuery
    End Function
End Module
