Imports System.Collections.Generic

Namespace Graphics
    Public Interface IGraphicsDevice
        ' for R_GE_checkVersionOrDie
        'int Version { get; }
        ReadOnly Property Name As String
        Sub OnActivated(ByVal description As DeviceDescription)
        Sub OnDeactivated(ByVal description As DeviceDescription)
        Sub OnNewPageRequested(ByVal context As GraphicsContext, ByVal description As DeviceDescription)
        Function OnResized(ByVal description As DeviceDescription) As Rectangle
        Sub OnClosed(ByVal description As DeviceDescription)
        Sub OnDrawStarted(ByVal description As DeviceDescription)
        Sub OnDrawStopped(ByVal description As DeviceDescription)
        Function GetSize(ByVal context As GraphicsContext, ByVal description As DeviceDescription) As Rectangle
        Function ConfirmNewFrame(ByVal description As DeviceDescription) As Boolean
        Sub DrawCircle(ByVal center As Point, ByVal radius As Double, ByVal context As GraphicsContext, ByVal description As DeviceDescription)
        Sub Clip(ByVal rectangle As Rectangle, ByVal description As DeviceDescription)
        Function GetLocation(ByVal description As DeviceDescription) As Point?
        Sub DrawLine(ByVal source As Point, ByVal destination As Point, ByVal context As GraphicsContext, ByVal description As DeviceDescription)
        Function GetMetricInfo(ByVal character As Integer, ByVal context As GraphicsContext, ByVal description As DeviceDescription) As MetricsInfo
        Sub DrawPolygon(ByVal points As IEnumerable(Of Point), ByVal context As GraphicsContext, ByVal description As DeviceDescription)
        Sub DrawPolyline(ByVal points As IEnumerable(Of Point), ByVal context As GraphicsContext, ByVal description As DeviceDescription)
        Sub DrawRectangle(ByVal rectangle As Rectangle, ByVal context As GraphicsContext, ByVal description As DeviceDescription)
        Sub DrawPath(ByVal points As IEnumerable(Of IEnumerable(Of Point)), ByVal winding As Boolean, ByVal context As GraphicsContext, ByVal description As DeviceDescription)
        Sub DrawRaster(ByVal raster As Raster, ByVal destination As Rectangle, ByVal rotation As Double, ByVal interpolated As Boolean, ByVal context As GraphicsContext, ByVal description As DeviceDescription)
        Function Capture(ByVal description As DeviceDescription) As Raster
        Function MeasureWidth(ByVal s As String, ByVal context As GraphicsContext, ByVal description As DeviceDescription) As Double
        Sub DrawText(ByVal s As String, ByVal location As Point, ByVal rotation As Double, ByVal adjustment As Double, ByVal context As GraphicsContext, ByVal description As DeviceDescription)
    End Interface
End Namespace
