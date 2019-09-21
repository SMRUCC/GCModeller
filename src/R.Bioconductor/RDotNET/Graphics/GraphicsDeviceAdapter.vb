#Region "Microsoft.VisualBasic::78dd087f0e42d495cbcafc3efe052e4c, RDotNET\Graphics\GraphicsDeviceAdapter.vb"

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

    '     Class GraphicsDeviceAdapter
    ' 
    '         Properties: Engine
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Capture, ConfirmNewFrame, GetEvent, GetFunction, GetInterruptsPending
    '                   GetInterruptsSuspended, GetLocation, (+2 Overloads) GetPoints, MeasureWidth
    ' 
    '         Sub: Activate, Alloc, ChangeMode, ClearDevDesc, Clip
    '              Close, Deactivate, (+2 Overloads) Dispose, DrawCircle, DrawLine
    '              DrawPath, DrawPolygon, DrawPolyline, DrawRaster, DrawRectangle
    '              DrawText, EventHelper, FreeAll, GetMetricInfo, Kill
    '              NewPage, Resize, SetEngine, SetInterruptsSuspended, SetMethod
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Linq
Imports System.Runtime.InteropServices
Imports RDotNet.Graphics.Internals

Namespace Graphics

    Friend Class GraphicsDeviceAdapter
        Implements IDisposable
        Private ReadOnly device As IGraphicsDevice
        Private ReadOnly delegateHandles As List(Of GCHandle)
        Private description As DeviceDescription
        Private m_engine As REngine
        Private gdd As IntPtr

        Public Sub New(device As IGraphicsDevice)
            If device Is Nothing Then
                Throw New ArgumentNullException("device")
            End If
            Me.device = device
            Me.delegateHandles = New List(Of GCHandle)()
            Me.gdd = IntPtr.Zero
        End Sub

        Public ReadOnly Property Engine() As REngine
            Get
                Return Me.m_engine
            End Get
        End Property

        Protected Function GetFunction(Of TDelegate As Class)() As TDelegate
            Return Engine.GetFunction(Of TDelegate)()
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            Me.description.Dispose()
            If disposing AndAlso Me.gdd <> IntPtr.Zero Then
                Kill()
            End If
        End Sub

        Public Sub Kill()
            Me.GetFunction(Of GEkillDevice)()(Me.description.DangerousGetHandle())
            Me.gdd = IntPtr.Zero
        End Sub

        Public Sub SetEngine(engine As REngine)
            If gdd <> IntPtr.Zero Then
                Throw New InvalidOperationException("engine is already set")
            End If
            If engine Is Nothing Then
                Throw New ArgumentNullException("engine")
            End If
            If Me.m_engine IsNot Nothing Then
                Throw New InvalidOperationException()
            End If
            If Not engine.IsRunning Then
                Throw New ArgumentException(Nothing, "engine")
            End If
            Me.m_engine = engine

            'this.GetFunction<R_GE_checkVersionOrDie>()(this.device.Version);
            Me.GetFunction(Of R_CheckDeviceAvailable)()()
            Dim oldSuspended = GetInterruptsSuspended(engine)
            SetInterruptsSuspended(engine, True)

            Me.description = New DeviceDescription()
            SetMethod()
            gdd = Me.GetFunction(Of GEcreateDevDesc)()(Me.description.DangerousGetHandle())
            Me.GetFunction(Of GEaddDevice2)()(gdd, Me.device.Name)

            SetInterruptsSuspended(engine, oldSuspended)
            If GetInterruptsPending(engine) AndAlso Not GetInterruptsSuspended(engine) Then
                Me.GetFunction(Of Rf_onintr)()()
            End If
        End Sub

        Private Shared Sub SetInterruptsSuspended(engine As REngine, value As Boolean)
            Dim pointer = engine.DangerousGetHandle("R_interrupts_suspended")
            Marshal.WriteInt32(pointer, Convert.ToInt32(value))
        End Sub

        Private Shared Function GetInterruptsSuspended(engine As REngine) As Boolean
            Dim pointer = engine.DangerousGetHandle("R_interrupts_suspended")
            Return Convert.ToBoolean(Marshal.ReadInt32(pointer))
        End Function

        Private Shared Function GetInterruptsPending(engine As REngine) As Boolean
            Dim pointer = engine.DangerousGetHandle("R_interrupts_pending")
            Return Convert.ToBoolean(Marshal.ReadInt32(pointer))
        End Function

        Private Sub SetMethod()
            Dim activate = DirectCast(AddressOf Me.Activate, _DevDesc_activate)
            Alloc(activate)
            Me.description.SetMethod("activate", activate)
            Dim cap = DirectCast(AddressOf Capture, _DevDesc_cap)
            Alloc(cap)
            Me.description.SetMethod("cap", cap)
            Dim circle = DirectCast(AddressOf DrawCircle, _DevDesc_circle)
            Alloc(circle)
            Me.description.SetMethod("circle", circle)
            Dim clip = DirectCast(AddressOf Me.Clip, _DevDesc_clip)
            Alloc(clip)
            Me.description.SetMethod("clip", clip)
            Dim close = DirectCast(AddressOf Me.Close, _DevDesc_close)
            Alloc(close)
            Me.description.SetMethod("close", close)
            Dim deactivate = DirectCast(AddressOf Me.Deactivate, _DevDesc_deactivate)
            Alloc(deactivate)
            Me.description.SetMethod("deactivate", deactivate)
            Dim line = DirectCast(AddressOf DrawLine, _DevDesc_line)
            Alloc(line)
            Me.description.SetMethod("line", line)
            Dim locator = DirectCast(AddressOf GetLocation, _DevDesc_locator)
            Alloc(locator)
            Me.description.SetMethod("locator", locator)
            Dim metricInfo = DirectCast(AddressOf GetMetricInfo, _DevDesc_metricInfo)
            Alloc(metricInfo)
            Me.description.SetMethod("metricInfo", metricInfo)
            Dim mode = DirectCast(AddressOf ChangeMode, _DevDesc_mode)
            Alloc(mode)
            Me.description.SetMethod("mode", mode)
            Dim newPage = DirectCast(AddressOf Me.NewPage, _DevDesc_newPage)
            Alloc(newPage)
            Me.description.SetMethod("newPage", newPage)
            Dim path = DirectCast(AddressOf DrawPath, _DevDesc_path)
            Alloc(path)
            Me.description.SetMethod("path", path)
            Dim polygon = DirectCast(AddressOf DrawPolygon, _DevDesc_polygon)
            Alloc(polygon)
            Me.description.SetMethod("polygon", polygon)
            Dim polyline = DirectCast(AddressOf DrawPolyline, _DevDesc_Polyline)
            Alloc(polyline)
            Me.description.SetMethod("polyline", polyline)
            Dim raster = DirectCast(AddressOf DrawRaster, _DevDesc_raster)
            Alloc(raster)
            Me.description.SetMethod("raster", raster)
            Dim rect = DirectCast(AddressOf DrawRectangle, _DevDesc_rect)
            Alloc(rect)
            Me.description.SetMethod("rect", rect)
            Dim size = DirectCast(AddressOf Resize, _DevDesc_size)
            Alloc(size)
            Me.description.SetMethod("size", size)
            Dim strWidth = DirectCast(AddressOf MeasureWidth, _DevDesc_strWidth)
            Alloc(strWidth)
            Me.description.SetMethod("strWidth", strWidth)
            Dim text = DirectCast(AddressOf DrawText, _DevDesc_text)
            Alloc(text)
            Me.description.SetMethod("text", text)
            Dim strWidthUTF8 = DirectCast(AddressOf MeasureWidth, _DevDesc_strWidth)
            Alloc(strWidthUTF8)
            Me.description.SetMethod("strWidthUTF8", strWidthUTF8)
            Dim textUTF8 = DirectCast(AddressOf DrawText, _DevDesc_text)
            Alloc(textUTF8)
            Me.description.SetMethod("textUTF8", textUTF8)
            Dim newFrameConfirm = DirectCast(AddressOf ConfirmNewFrame, _DevDesc_newFrameConfirm)
            Alloc(newFrameConfirm)
            Me.description.SetMethod("newFrameConfirm", newFrameConfirm)
            Dim getEvent = DirectCast(AddressOf Me.GetEvent, _DevDesc_getEvent)
            Alloc(getEvent)
            Me.description.SetMethod("getEvent", getEvent)
            Dim eventHelper = DirectCast(AddressOf Me.EventHelper, _DevDesc_eventHelper)
            Alloc(eventHelper)
            Me.description.SetMethod("eventHelper", eventHelper)
        End Sub

        Private Sub Alloc(d As [Delegate])
            Dim handle = GCHandle.Alloc(d)
            Me.delegateHandles.Add(handle)
        End Sub

        Private Sub FreeAll()
            Me.delegateHandles.ForEach(Sub(handle) Call handle.Free())
            Me.delegateHandles.Clear()
        End Sub

        Private Sub Activate(pDevDesc As IntPtr)
            Me.device.OnActivated(Me.description)
        End Sub

        Private Sub Deactivate(pDevDesc As IntPtr)
            Me.device.OnDeactivated(Me.description)
        End Sub

        Private Sub NewPage(gc As IntPtr, dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Me.device.OnNewPageRequested(context, Me.description)
        End Sub

        Private Sub Resize(ByRef left As Double, ByRef right As Double, ByRef bottom As Double, ByRef top As Double, dd As IntPtr)
            Dim rectangle = Me.device.OnResized(Me.description)
            left = rectangle.Left
            right = rectangle.Right
            bottom = rectangle.Bottom
            top = rectangle.Top
        End Sub

        Private Sub Close(dd As IntPtr)
            device.OnClosed(description)
            ClearDevDesc()
        End Sub

        Private Sub ClearDevDesc()
            Dim geDevDesc = CType(Marshal.PtrToStructure(gdd, GetType(GEDevDesc)), GEDevDesc)
            geDevDesc.dev = IntPtr.Zero
            Marshal.StructureToPtr(geDevDesc, gdd, False)
        End Sub

        Private Function ConfirmNewFrame(dd As IntPtr) As Boolean
            Return Me.device.ConfirmNewFrame(Me.description)
        End Function

        Private Sub ChangeMode(mode As Integer, dd As IntPtr)
            If mode = 0 Then
                Me.device.OnDrawStarted(Me.description)
            ElseIf mode = 1 Then
                Me.device.OnDrawStopped(Me.description)
            End If
        End Sub

        Private Sub DrawCircle(x As Double, y As Double, r As Double, gc As IntPtr, dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim center = New Point(x, y)
            Me.device.DrawCircle(center, r, context, Me.description)
        End Sub

        Private Sub Clip(x0 As Double, x1 As Double, y0 As Double, y1 As Double, dd As IntPtr)
            Dim rectangle = New Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x0 - x1), Math.Abs(y0 - y1))
            Me.device.Clip(rectangle, Me.description)
        End Sub

        Private Function GetLocation(ByRef x As Double, ByRef y As Double, dd As IntPtr) As Boolean
            Dim location = Me.device.GetLocation(Me.description)
            If Not location.HasValue Then
                x = 0
                y = 0
                Return False
            End If

            Dim p = location.Value
            x = p.X
            y = p.Y
            Return True
        End Function

        Private Sub DrawLine(x1 As Double, y1 As Double, x2 As Double, y2 As Double, gc As IntPtr, dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim source = New Point(x1, y1)
            Dim destination = New Point(x2, y2)
            Me.device.DrawLine(source, destination, context, Me.description)
        End Sub

        Private Sub GetMetricInfo(c As Integer, gc As IntPtr, ByRef ascent As Double, ByRef descent As Double, ByRef width As Double, dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim metric = Me.device.GetMetricInfo(c, context, Me.description)
            ascent = metric.Ascent
            descent = metric.Descent
            width = metric.Width
        End Sub

        Private Sub DrawPolygon(n As Integer, x As IntPtr, y As IntPtr, gc As IntPtr, dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim points = GetPoints(n, x, y)
            Me.device.DrawPolygon(points, context, Me.description)
        End Sub

        Private Sub DrawPolyline(n As Integer, x As IntPtr, y As IntPtr, gc As IntPtr, dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim points = GetPoints(n, x, y)
            Me.device.DrawPolyline(points, context, Me.description)
        End Sub

        Private Sub DrawRectangle(x0 As Double, y0 As Double, x1 As Double, y1 As Double, gc As IntPtr, dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim rectangle = New Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x0 - x1), Math.Abs(y0 - y1))
            Me.device.DrawRectangle(rectangle, context, Me.description)
        End Sub

        Private Sub DrawPath(x As IntPtr, y As IntPtr, npoly As Integer, nper As IntPtr, winding As Boolean, gc As IntPtr,
        dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim points = GetPoints(x, y, npoly, nper)
            Me.device.DrawPath(points, winding, context, Me.description)
        End Sub

        Private Sub DrawRaster(raster As IntPtr, w As Integer, h As Integer, x As Double, y As Double, width As Double,
        height As Double, rot As Double, interpolate As Boolean, gc As IntPtr, dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim output = New Raster(w, h)
            For i = 0 To w - 1
                For j = 0 To h - 1
                    output(i, j) = Color.FromUInt32(CUInt(Marshal.ReadInt32(raster)))
                    raster = IntPtr.Add(raster, 4)
                Next
            Next

            Me.device.DrawRaster(output, New Rectangle(x, y, width, height), rot, interpolate, context, Me.description)
        End Sub

        Private Function Capture(dd As IntPtr) As IntPtr
            Dim raster As Raster = Me.device.Capture(Me.description)
            Return Engine.CreateIntegerMatrix(raster).DangerousGetHandle()
        End Function

        Private Function MeasureWidth(str As String, gc As IntPtr, dd As IntPtr) As Double
            Dim context = New GraphicsContext(gc)
            Return Me.device.MeasureWidth(str, context, Me.description)
        End Function

        Private Sub DrawText(x As Double, y As Double, str As String, rot As Double, hadj As Double, gc As IntPtr, dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Me.device.DrawText(str, New Point(x, y), rot, hadj, context, Me.description)
        End Sub

        Private Function GetEvent(sexp As IntPtr, s As String) As IntPtr
            Return IntPtr.Zero
        End Function

        Private Sub EventHelper(dd As IntPtr, code As Integer)
        End Sub

        Private Function GetPoints(n As Integer, x As IntPtr, y As IntPtr) As IEnumerable(Of Point)
            Return Enumerable.Range(0, n).[Select](Function(index)
                                                       Dim offset = 8 * index
                                                       Dim px = Utility.ReadDouble(x, offset)
                                                       Dim py = Utility.ReadDouble(y, offset)
                                                       Return New Point(px, py)
                                                   End Function)
        End Function

        Private Iterator Function GetPoints(x As IntPtr, y As IntPtr, npoly As Integer, nper As IntPtr) As IEnumerable(Of IEnumerable(Of Point))
            If Not Engine.IsRunning Then
                Throw New InvalidOperationException()
            End If

            For index = 0 To npoly - 1
                Dim offset = 4 * index
                Dim n = Marshal.ReadInt32(nper, offset)

                Yield GetPoints(n, x, y)

                Dim pointOffset = 8 * n
                x = IntPtr.Add(x, pointOffset)
                y = IntPtr.Add(y, pointOffset)
            Next
        End Function
    End Class
End Namespace
