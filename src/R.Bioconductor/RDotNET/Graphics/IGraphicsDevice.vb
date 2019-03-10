Imports System.Collections.Generic

Namespace Graphics

    Public Interface IGraphicsDevice
        ' for R_GE_checkVersionOrDie
        'int Version { get; }
        ReadOnly Property Name() As String

        Sub OnActivated(description As DeviceDescription)

        Sub OnDeactivated(description As DeviceDescription)

        Sub OnNewPageRequested(context As GraphicsContext, description As DeviceDescription)

        Function OnResized(description As DeviceDescription) As Rectangle

        Sub OnClosed(description As DeviceDescription)

        Sub OnDrawStarted(description As DeviceDescription)

        Sub OnDrawStopped(description As DeviceDescription)

        Function GetSize(context As GraphicsContext, description As DeviceDescription) As Rectangle

        Function ConfirmNewFrame(description As DeviceDescription) As Boolean

        Sub DrawCircle(center As Point, radius As Double, context As GraphicsContext, description As DeviceDescription)

        Sub Clip(rectangle As Rectangle, description As DeviceDescription)

        Function GetLocation(description As DeviceDescription) As System.Nullable(Of Point)

        Sub DrawLine(source As Point, destination As Point, context As GraphicsContext, description As DeviceDescription)

        Function GetMetricInfo(character As Integer, context As GraphicsContext, description As DeviceDescription) As MetricsInfo

        Sub DrawPolygon(points As IEnumerable(Of Point), context As GraphicsContext, description As DeviceDescription)

        Sub DrawPolyline(points As IEnumerable(Of Point), context As GraphicsContext, description As DeviceDescription)

        Sub DrawRectangle(rectangle As Rectangle, context As GraphicsContext, description As DeviceDescription)

        Sub DrawPath(points As IEnumerable(Of IEnumerable(Of Point)), winding As Boolean, context As GraphicsContext, description As DeviceDescription)

        Sub DrawRaster(raster As Raster, destination As Rectangle, rotation As Double, interpolated As Boolean, context As GraphicsContext, description As DeviceDescription)

        Function Capture(description As DeviceDescription) As Raster

        Function MeasureWidth(s As String, context As GraphicsContext, description As DeviceDescription) As Double

        Sub DrawText(s As String, location As Point, rotation As Double, adjustment As Double, context As GraphicsContext, description As DeviceDescription)
    End Interface
End Namespace