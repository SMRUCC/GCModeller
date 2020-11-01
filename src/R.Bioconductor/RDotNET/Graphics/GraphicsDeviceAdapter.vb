Imports System.Runtime.InteropServices
Imports RDotNet.Graphics.Internals

Namespace Graphics
    Friend Class GraphicsDeviceAdapter
        Implements IDisposable

        Private ReadOnly device As IGraphicsDevice
        Private ReadOnly delegateHandles As List(Of GCHandle)
        Private description As DeviceDescription
        Private engineField As REngine
        Private gdd As IntPtr

        Public Sub New(ByVal device As IGraphicsDevice)
            If device Is Nothing Then
                Throw New ArgumentNullException("device")
            End If

            Me.device = device
            delegateHandles = New List(Of GCHandle)()
            gdd = IntPtr.Zero
        End Sub

        Public ReadOnly Property Engine As REngine
            Get
                Return engineField
            End Get
        End Property

        Protected Function GetFunction(Of TDelegate As Class)() As TDelegate
            Return Engine.GetFunction(Of TDelegate)()
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            description.Dispose()

            If disposing AndAlso gdd <> IntPtr.Zero Then
                Kill()
            End If
        End Sub

        Public Sub Kill()
            GetFunction(Of GEkillDevice)()(description.DangerousGetHandle())
            gdd = IntPtr.Zero
        End Sub

        Public Sub SetEngine(ByVal engine As REngine)
            If gdd <> IntPtr.Zero Then
                Throw New InvalidOperationException("engine is already set")
            End If

            If engine Is Nothing Then
                Throw New ArgumentNullException("engine")
            End If

            If engineField IsNot Nothing Then
                Throw New InvalidOperationException()
            End If

            If Not engine.IsRunning Then
                Throw New ArgumentException(Nothing, "engine")
            End If

            engineField = engine

            'this.GetFunction<R_GE_checkVersionOrDie>()(this.device.Version);
            GetFunction(Of R_CheckDeviceAvailable)()()
            Dim oldSuspended = GraphicsDeviceAdapter.GetInterruptsSuspended(engine)
            GraphicsDeviceAdapter.SetInterruptsSuspended(engine, True)
            description = New DeviceDescription()
            SetMethod()
            gdd = GetFunction(Of GEcreateDevDesc)()(description.DangerousGetHandle())
            GetFunction(Of GEaddDevice2)()(gdd, device.Name)
            GraphicsDeviceAdapter.SetInterruptsSuspended(engine, oldSuspended)

            If GraphicsDeviceAdapter.GetInterruptsPending(engine) AndAlso Not GraphicsDeviceAdapter.GetInterruptsSuspended(engine) Then
                GetFunction(Of Rf_onintr)()()
            End If
        End Sub

        Private Shared Sub SetInterruptsSuspended(ByVal engine As REngine, ByVal value As Boolean)
            Dim pointer = engine.DangerousGetHandle("R_interrupts_suspended")
            Marshal.WriteInt32(pointer, Convert.ToInt32(value))
        End Sub

        Private Shared Function GetInterruptsSuspended(ByVal engine As REngine) As Boolean
            Dim pointer = engine.DangerousGetHandle("R_interrupts_suspended")
            Return Convert.ToBoolean(Marshal.ReadInt32(pointer))
        End Function

        Private Shared Function GetInterruptsPending(ByVal engine As REngine) As Boolean
            Dim pointer = engine.DangerousGetHandle("R_interrupts_pending")
            Return Convert.ToBoolean(Marshal.ReadInt32(pointer))
        End Function

        Private Sub SetMethod()
            Dim activate = CType(AddressOf Me.Activate, _DevDesc_activate)
            Alloc(activate)
            description.SetMethod("activate", activate)
            Dim cap = CType(AddressOf Capture, _DevDesc_cap)
            Alloc(cap)
            description.SetMethod("cap", cap)
            Dim circle = CType(AddressOf DrawCircle, _DevDesc_circle)
            Alloc(circle)
            description.SetMethod("circle", circle)
            Dim clip = CType(AddressOf Me.Clip, _DevDesc_clip)
            Alloc(clip)
            description.SetMethod("clip", clip)
            Dim close = CType(AddressOf Me.Close, _DevDesc_close)
            Alloc(close)
            description.SetMethod("close", close)
            Dim deactivate = CType(AddressOf Me.Deactivate, _DevDesc_deactivate)
            Alloc(deactivate)
            description.SetMethod("deactivate", deactivate)
            Dim line = CType(AddressOf DrawLine, _DevDesc_line)
            Alloc(line)
            description.SetMethod("line", line)
            Dim locator = CType(AddressOf GetLocation, _DevDesc_locator)
            Alloc(locator)
            description.SetMethod("locator", locator)
            Dim metricInfo = CType(AddressOf GetMetricInfo, _DevDesc_metricInfo)
            Alloc(metricInfo)
            description.SetMethod("metricInfo", metricInfo)
            Dim mode = CType(AddressOf ChangeMode, _DevDesc_mode)
            Alloc(mode)
            description.SetMethod("mode", mode)
            Dim newPage = CType(AddressOf Me.NewPage, _DevDesc_newPage)
            Alloc(newPage)
            description.SetMethod("newPage", newPage)
            Dim path = CType(AddressOf DrawPath, _DevDesc_path)
            Alloc(path)
            description.SetMethod("path", path)
            Dim polygon = CType(AddressOf DrawPolygon, _DevDesc_polygon)
            Alloc(polygon)
            description.SetMethod("polygon", polygon)
            Dim polyline = CType(AddressOf DrawPolyline, _DevDesc_Polyline)
            Alloc(polyline)
            description.SetMethod("polyline", polyline)
            Dim raster = CType(AddressOf DrawRaster, _DevDesc_raster)
            Alloc(raster)
            description.SetMethod("raster", raster)
            Dim rect = CType(AddressOf DrawRectangle, _DevDesc_rect)
            Alloc(rect)
            description.SetMethod("rect", rect)
            Dim size = CType(AddressOf Resize, _DevDesc_size)
            Alloc(size)
            description.SetMethod("size", size)
            Dim strWidth = CType(AddressOf MeasureWidth, _DevDesc_strWidth)
            Alloc(strWidth)
            description.SetMethod("strWidth", strWidth)
            Dim text = CType(AddressOf DrawText, _DevDesc_text)
            Alloc(text)
            description.SetMethod("text", text)
            Dim strWidthUTF8 = CType(AddressOf MeasureWidth, _DevDesc_strWidth)
            Alloc(strWidthUTF8)
            description.SetMethod("strWidthUTF8", strWidthUTF8)
            Dim textUTF8 = CType(AddressOf DrawText, _DevDesc_text)
            Alloc(textUTF8)
            description.SetMethod("textUTF8", textUTF8)
            Dim newFrameConfirm = CType(AddressOf ConfirmNewFrame, _DevDesc_newFrameConfirm)
            Alloc(newFrameConfirm)
            description.SetMethod("newFrameConfirm", newFrameConfirm)
            Dim getEvent = CType(AddressOf Me.GetEvent, _DevDesc_getEvent)
            Alloc(getEvent)
            description.SetMethod("getEvent", getEvent)
            Dim eventHelper = CType(AddressOf Me.EventHelper, _DevDesc_eventHelper)
            Alloc(eventHelper)
            description.SetMethod("eventHelper", eventHelper)
        End Sub

        Private Sub Alloc(ByVal d As [Delegate])
            Dim handle = GCHandle.Alloc(d)
            delegateHandles.Add(handle)
        End Sub

        Private Sub FreeAll()
            delegateHandles.ForEach(Sub(handle) handle.Free())
            delegateHandles.Clear()
        End Sub

        Private Sub Activate(ByVal pDevDesc As IntPtr)
            device.OnActivated(description)
        End Sub

        Private Sub Deactivate(ByVal pDevDesc As IntPtr)
            device.OnDeactivated(description)
        End Sub

        Private Sub NewPage(ByVal gc As IntPtr, ByVal dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            device.OnNewPageRequested(context, description)
        End Sub

        Private Sub Resize(<Out> ByRef left As Double, <Out> ByRef right As Double, <Out> ByRef bottom As Double, <Out> ByRef top As Double, ByVal dd As IntPtr)
            Dim rectangle = device.OnResized(description)
            left = rectangle.Left
            right = rectangle.Right
            bottom = rectangle.Bottom
            top = rectangle.Top
        End Sub

        Private Sub Close(ByVal dd As IntPtr)
            device.OnClosed(description)
            ClearDevDesc()
        End Sub

        Private Sub ClearDevDesc()
            Dim geDevDesc = CType(Marshal.PtrToStructure(gdd, GetType(GEDevDesc)), GEDevDesc)
            geDevDesc.dev = IntPtr.Zero
            Marshal.StructureToPtr(geDevDesc, gdd, False)
        End Sub

        Private Function ConfirmNewFrame(ByVal dd As IntPtr) As Boolean
            Return device.ConfirmNewFrame(description)
        End Function

        Private Sub ChangeMode(ByVal mode As Integer, ByVal dd As IntPtr)
            If mode = 0 Then
                device.OnDrawStarted(description)
            ElseIf mode = 1 Then
                device.OnDrawStopped(description)
            End If
        End Sub

        Private Sub DrawCircle(ByVal x As Double, ByVal y As Double, ByVal r As Double, ByVal gc As IntPtr, ByVal dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim center = New Point(x, y)
            device.DrawCircle(center, r, context, description)
        End Sub

        Private Sub Clip(ByVal x0 As Double, ByVal x1 As Double, ByVal y0 As Double, ByVal y1 As Double, ByVal dd As IntPtr)
            Dim rectangle = New Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x0 - x1), Math.Abs(y0 - y1))
            device.Clip(rectangle, description)
        End Sub

        Private Function GetLocation(<Out> ByRef x As Double, <Out> ByRef y As Double, ByVal dd As IntPtr) As Boolean
            Dim location = device.GetLocation(description)

            If Not location.HasValue Then
                x = Nothing
                y = Nothing
                Return False
            End If

            Dim p = location.Value
            x = p.X
            y = p.Y
            Return True
        End Function

        Private Sub DrawLine(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double, ByVal gc As IntPtr, ByVal dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim source = New Point(x1, y1)
            Dim destination = New Point(x2, y2)
            device.DrawLine(source, destination, context, description)
        End Sub

        Private Sub GetMetricInfo(ByVal c As Integer, ByVal gc As IntPtr, <Out> ByRef ascent As Double, <Out> ByRef descent As Double, <Out> ByRef width As Double, ByVal dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim metric = device.GetMetricInfo(c, context, description)
            ascent = metric.Ascent
            descent = metric.Descent
            width = metric.Width
        End Sub

        Private Sub DrawPolygon(ByVal n As Integer, ByVal x As IntPtr, ByVal y As IntPtr, ByVal gc As IntPtr, ByVal dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim points = GetPoints(n, x, y)
            device.DrawPolygon(points, context, description)
        End Sub

        Private Sub DrawPolyline(ByVal n As Integer, ByVal x As IntPtr, ByVal y As IntPtr, ByVal gc As IntPtr, ByVal dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim points = GetPoints(n, x, y)
            device.DrawPolyline(points, context, description)
        End Sub

        Private Sub DrawRectangle(ByVal x0 As Double, ByVal y0 As Double, ByVal x1 As Double, ByVal y1 As Double, ByVal gc As IntPtr, ByVal dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim rectangle = New Rectangle(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs(x0 - x1), Math.Abs(y0 - y1))
            device.DrawRectangle(rectangle, context, description)
        End Sub

        Private Sub DrawPath(ByVal x As IntPtr, ByVal y As IntPtr, ByVal npoly As Integer, ByVal nper As IntPtr, ByVal winding As Boolean, ByVal gc As IntPtr, ByVal dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim points = GetPoints(x, y, npoly, nper)
            device.DrawPath(points, winding, context, description)
        End Sub

        Private Sub DrawRaster(ByVal raster As IntPtr, ByVal w As Integer, ByVal h As Integer, ByVal x As Double, ByVal y As Double, ByVal width As Double, ByVal height As Double, ByVal rot As Double, ByVal interpolate As Boolean, ByVal gc As IntPtr, ByVal dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            Dim output = New Raster(w, h)
            ' BEGIN TODO : Visual Basic does not support checked statements!
            For i = 0 To w - 1

                For j = 0 To h - 1
                    output(i, j) = Color.FromUInt32(Marshal.ReadInt32(raster))
                    raster = System.IntPtr.Add(raster, Marshal.SizeOf(GetType(Integer)))
                Next
            Next
            ' END TODO : Visual Basic does not support checked statements!
            device.DrawRaster(output, New Rectangle(x, y, width, height), rot, interpolate, context, description)
        End Sub

        Private Function Capture(ByVal dd As IntPtr) As IntPtr
            Dim raster = device.Capture(description)
            Return RasterExtension.CreateIntegerMatrix(Engine, CType(raster, Raster)).DangerousGetHandle()
        End Function

        Private Function MeasureWidth(ByVal str As String, ByVal gc As IntPtr, ByVal dd As IntPtr) As Double
            Dim context = New GraphicsContext(gc)
            Return device.MeasureWidth(str, context, description)
        End Function

        Private Sub DrawText(ByVal x As Double, ByVal y As Double, ByVal str As String, ByVal rot As Double, ByVal hadj As Double, ByVal gc As IntPtr, ByVal dd As IntPtr)
            Dim context = New GraphicsContext(gc)
            device.DrawText(str, New Point(x, y), rot, hadj, context, description)
        End Sub

        Private Function GetEvent(ByVal sexp As IntPtr, ByVal s As String) As IntPtr
            Return IntPtr.Zero
        End Function

        Private Sub EventHelper(ByVal dd As IntPtr, ByVal code As Integer)
        End Sub

        Private Function GetPoints(ByVal n As Integer, ByVal x As IntPtr, ByVal y As IntPtr) As IEnumerable(Of Point)
            Return Enumerable.Range(0, n).[Select](Function(index)
                                                       Dim offset = Marshal.SizeOf(GetType(Double)) * index
                                                       Dim px = Utility.ReadDouble(x, offset)
                                                       Dim py = Utility.ReadDouble(y, offset)
                                                       Return New Point(px, py)
                                                   End Function)
        End Function

        Private Iterator Function GetPoints(ByVal x As IntPtr, ByVal y As IntPtr, ByVal npoly As Integer, ByVal nper As IntPtr) As IEnumerable(Of IEnumerable(Of Point))
            If Not Engine.IsRunning Then
                Throw New InvalidOperationException()
            End If

            For index = 0 To npoly - 1
                Dim offset = Marshal.SizeOf(GetType(Integer)) * index
                Dim n = Marshal.ReadInt32(nper, offset)
                Yield GetPoints(n, x, y)

                Dim pointOffset = Marshal.SizeOf(GetType(Double)) * n

                x = IntPtr.Add(x, pointOffset)
                y = IntPtr.Add(y, pointOffset)
            Next
        End Function
    End Class
End Namespace
