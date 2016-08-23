#Region "Microsoft.VisualBasic::899e3efb1ad05e8e5a9b633430fe4c81, ..\R.Bioconductor\RDotNET\Graphics\IGraphicsDevice.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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
