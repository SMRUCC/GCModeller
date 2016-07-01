Imports System.Drawing
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Imaging

Public MustInherit Class CurvesModel

    <DataFrameColumn("slide_Window.size")>
    Public Property WindowSize As Integer = 500
    <DataFrameColumn("slide_Window.steps")>
    Public Property Steps As Integer = 20
    <DataFrameColumn("plot.height")>
    Public Property PlotsHeight As Integer = 100
    <DataFrameColumn("offset.aix_y")>
    Public Property YValueOffset As Integer = 40
    <DataFrameColumn("average.shows")>
    Public Property ShowAverageLine As Boolean = True

    Protected PlotBrush As SolidBrush = Brushes.DarkGreen

    Public Function Draw(source As Image, buf As Double(), location As Point, size As Size) As Image
        Dim sample As DataSample(Of Double) = DataSampleAPI.DoubleSample(buf)

        Using g As IGraphics = source.GdiFromImage
            Call Draw(g, sample, location, size)
            Return DirectCast(g, GDIPlusDeviceHandle).ImageResource
        End Using
    End Function

    Protected MustOverride Sub Draw(ByRef source As IGraphics, data As DataSample(Of Double), location As Point, size As Size)

    Public Shared Function GraphicsDevice(type As GraphicTypes) As CurvesModel
        Select Case type
            Case GraphicTypes.Curves
                Return New Line
            Case GraphicTypes.Histogram
                Return New Histogram
            Case Else
                Return New Line
        End Select
    End Function
End Class

Public Enum GraphicTypes
    Curves
    Histogram
End Enum