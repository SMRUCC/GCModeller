Imports System
Imports System.Runtime.InteropServices

Namespace Graphics.Internals
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
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
        <MarshalAs(UnmanagedType.Bool)>
        Friend canClip As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Friend canChangeGamma As Boolean
        Friend canHAdj As Adjustment
        Friend startps As Double
        Friend startcol As Color
        Friend startfill As Color
        Friend startlty As LineType
        Friend startfont As Integer
        Friend startgamma As Double
        Friend deviceSpecific As IntPtr
        <MarshalAs(UnmanagedType.Bool)>
        Friend displayListOn As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Friend canGenMouseDown As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Friend canGenMouseMove As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Friend canGenMouseUp As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Friend canGenKeybd As Boolean
        <MarshalAs(UnmanagedType.Bool)>
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
        <MarshalAs(UnmanagedType.Bool)>
        Friend hasTextUTF8 As Boolean

        'internal _DevDesc_textUTF8 textUTF8;
        'internal _DevDesc_strWidthUTF8 strWidthUTF8;
        Friend textUTF8 As _DevDesc_text
        Friend strWidthUTF8 As _DevDesc_strWidth
        <MarshalAs(UnmanagedType.Bool)>
        Friend wantSymbolUTF8 As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Friend useRotatedTextInContour As Boolean
        Friend eventEnv As IntPtr
        Friend eventHelper As _DevDesc_eventHelper
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
        Friend reserved As String
    End Structure

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_activate(ByVal pDevDesc As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_circle(ByVal x As Double, ByVal y As Double, ByVal r As Double, ByVal gc As IntPtr, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_clip(ByVal x0 As Double, ByVal x1 As Double, ByVal y0 As Double, ByVal y1 As Double, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_close(ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_deactivate(ByVal pDevDesc As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function _DevDesc_locator(<Out> ByRef x As Double, <Out> ByRef y As Double, ByVal dd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_line(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double, ByVal gc As IntPtr, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_metricInfo(ByVal c As Integer, ByVal gc As IntPtr, <Out> ByRef ascent As Double, <Out> ByRef descent As Double, <Out> ByRef width As Double, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_mode(ByVal mode As Integer, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_newPage(ByVal gc As IntPtr, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_polygon(ByVal n As Integer, ByVal x As IntPtr, ByVal y As IntPtr, ByVal gc As IntPtr, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_Polyline(ByVal n As Integer, ByVal x As IntPtr, ByVal y As IntPtr, ByVal gc As IntPtr, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_rect(ByVal x0 As Double, ByVal y0 As Double, ByVal x1 As Double, ByVal y1 As Double, ByVal gc As IntPtr, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_path(ByVal x As IntPtr, ByVal y As IntPtr, ByVal npoly As Integer, ByVal nper As IntPtr,
    <MarshalAs(UnmanagedType.Bool)> ByVal winding As Boolean, ByVal gc As IntPtr, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_raster(ByVal raster As IntPtr, ByVal w As Integer, ByVal h As Integer, ByVal x As Double, ByVal y As Double, ByVal width As Double, ByVal height As Double, ByVal rot As Double,
    <MarshalAs(UnmanagedType.Bool)> ByVal interpolate As Boolean, ByVal gc As IntPtr, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function _DevDesc_cap(ByVal dd As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_size(<Out> ByRef left As Double, <Out> ByRef right As Double, <Out> ByRef bottom As Double, <Out> ByRef top As Double, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function _DevDesc_strWidth(
    <MarshalAs(UnmanagedType.LPStr)> ByVal str As String, ByVal gc As IntPtr, ByVal dd As IntPtr) As Double
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_text(ByVal x As Double, ByVal y As Double,
    <MarshalAs(UnmanagedType.LPStr)> ByVal str As String, ByVal rot As Double, ByVal hadj As Double, ByVal gc As IntPtr, ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_onExit(ByVal dd As IntPtr)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function _DevDesc_getEvent(ByVal sexp As IntPtr,
    <MarshalAs(UnmanagedType.LPStr)> ByVal e As String) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function _DevDesc_newFrameConfirm(ByVal dd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean

    '[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    'internal delegate void _DevDesc_textUTF8(double x, double y, [MarshalAs(UnmanagedType.LPStr)] string str, double rot, double hadj, IntPtr gc, IntPtr dd);
    '[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    'internal delegate double _DevDesc_strWidthUTF8([MarshalAs(UnmanagedType.LPStr)] string str, IntPtr gc, IntPtr dd);
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_eventHelper(ByVal dd As IntPtr, ByVal code As Integer)
End Namespace
