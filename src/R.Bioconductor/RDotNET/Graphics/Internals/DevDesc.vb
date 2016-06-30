Imports System.Runtime.InteropServices

Namespace Graphics.Internals
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)> _
    Friend Structure DevDesc
        Friend left As Double
        Friend right As Double
        Friend bottom As Double
        Friend top As Double
        Friend clipLeft As Double
        Friend clipRight As Double
        Friend clipBottom As Double
        Friend clipTop As Double
        Friend xCharOffset As Double
        Friend yCharOffset As Double
        Friend yLineBias As Double

        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2, ArraySubType:=UnmanagedType.R8)>
        Friend ipr As Double()

        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2, ArraySubType:=UnmanagedType.R8)>
        Friend cra As Double()

        Friend gamma As Double

        <MarshalAs(UnmanagedType.Bool)> _
        Friend canClip As Boolean

        <MarshalAs(UnmanagedType.Bool)> _
        Friend canChangeGamma As Boolean

        Friend canHAdj As Adjustment
        Friend startps As Double
        Friend startcol As Color
        Friend startfill As Color
        Friend startlty As LineType
        Friend startfont As Integer
        Friend startgamma As Double
        Friend deviceSpecific As IntPtr

        <MarshalAs(UnmanagedType.Bool)> _
        Friend displayListOn As Boolean

        <MarshalAs(UnmanagedType.Bool)> _
        Friend canGenMouseDown As Boolean

        <MarshalAs(UnmanagedType.Bool)> _
        Friend canGenMouseMove As Boolean

        <MarshalAs(UnmanagedType.Bool)> _
        Friend canGenMouseUp As Boolean

        <MarshalAs(UnmanagedType.Bool)> _
        Friend canGenKeybd As Boolean

        <MarshalAs(UnmanagedType.Bool)> _
        Friend gettingEvent As Boolean

        Friend activate As _DevDesc_activate
        Friend circle As _DevDesc_circle
        Friend clip As _DevDesc_clip
        Friend close As _DevDesc_close
        Friend deactivate As _DevDesc_deactivate
        Friend locator As _DevDesc_locator
        Friend line As _DevDesc_line
        Friend metricInfo As _DevDesc_metricInfo
        Friend mode As _DevDesc_mode
        Friend newPage As _DevDesc_newPage
        Friend polygon As _DevDesc_polygon
        Friend polyline As _DevDesc_Polyline
        Friend rect As _DevDesc_rect
        Friend path As _DevDesc_path
        Friend raster As _DevDesc_raster
        Friend cap As _DevDesc_cap
        Friend size As _DevDesc_size
        Friend strWidth As _DevDesc_strWidth
        Friend text As _DevDesc_text
        Friend onExit As _DevDesc_onExit
        Friend getEvent As _DevDesc_getEvent
        Friend newFrameConfirm As _DevDesc_newFrameConfirm

        <MarshalAs(UnmanagedType.Bool)> _
        Friend hasTextUTF8 As Boolean

        'internal _DevDesc_textUTF8 textUTF8;
        'internal _DevDesc_strWidthUTF8 strWidthUTF8;
        Friend textUTF8 As _DevDesc_text

        Friend strWidthUTF8 As _DevDesc_strWidth

        <MarshalAs(UnmanagedType.Bool)> _
        Friend wantSymbolUTF8 As Boolean

        <MarshalAs(UnmanagedType.Bool)> _
        Friend useRotatedTextInContour As Boolean

        Friend eventEnv As IntPtr
        Friend eventHelper As _DevDesc_eventHelper

        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
        Friend reserved As String
    End Structure

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_activate(pDevDesc As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_circle(x As Double, y As Double, r As Double, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_clip(x0 As Double, x1 As Double, y0 As Double, y1 As Double, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_close(dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_deactivate(pDevDesc As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Function _DevDesc_locator(ByRef x As Double, ByRef y As Double, dd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_line(x1 As Double, y1 As Double, x2 As Double, y2 As Double, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_metricInfo(c As Integer, gc As IntPtr, ByRef ascent As Double, ByRef descent As Double, ByRef width As Double, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_mode(mode As Integer, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_newPage(gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_polygon(n As Integer, x As IntPtr, y As IntPtr, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_Polyline(n As Integer, x As IntPtr, y As IntPtr, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_rect(x0 As Double, y0 As Double, x1 As Double, y1 As Double, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_path(x As IntPtr, y As IntPtr, npoly As Integer, nper As IntPtr, <MarshalAs(UnmanagedType.Bool)> winding As Boolean, gc As IntPtr, _
        dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_raster(raster As IntPtr, w As Integer, h As Integer, x As Double, y As Double, width As Double, _
        height As Double, rot As Double, <MarshalAs(UnmanagedType.Bool)> interpolate As Boolean, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Function _DevDesc_cap(dd As IntPtr) As IntPtr

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_size(ByRef left As Double, ByRef right As Double, ByRef bottom As Double, ByRef top As Double, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Function _DevDesc_strWidth(<MarshalAs(UnmanagedType.LPStr)> str As String, gc As IntPtr, dd As IntPtr) As Double

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_text(x As Double, y As Double, <MarshalAs(UnmanagedType.LPStr)> str As String, rot As Double, hadj As Double, gc As IntPtr, _
        dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_onExit(dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Function _DevDesc_getEvent(sexp As IntPtr, <MarshalAs(UnmanagedType.LPStr)> e As String) As IntPtr

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Function _DevDesc_newFrameConfirm(dd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean

    '[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    'internal delegate void _DevDesc_textUTF8(double x, double y, [MarshalAs(UnmanagedType.LPStr)] string str, double rot, double hadj, IntPtr gc, IntPtr dd);
    '[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    'internal delegate double _DevDesc_strWidthUTF8([MarshalAs(UnmanagedType.LPStr)] string str, IntPtr gc, IntPtr dd);
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub _DevDesc_eventHelper(dd As IntPtr, code As Integer)
End Namespace
