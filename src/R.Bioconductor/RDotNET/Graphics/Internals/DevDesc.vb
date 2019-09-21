#Region "Microsoft.VisualBasic::42c641935912ab64ffdea8d4dd70107d, RDotNET\Graphics\Internals\DevDesc.vb"

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

    '     Structure DevDesc
    ' 
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Function
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Function
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Function
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Function
    ' 
    ' 
    '     Delegate Function
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
    Friend Delegate Sub _DevDesc_activate(pDevDesc As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_circle(x As Double, y As Double, r As Double, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_clip(x0 As Double, x1 As Double, y0 As Double, y1 As Double, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_close(dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_deactivate(pDevDesc As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function _DevDesc_locator(ByRef x As Double, ByRef y As Double, dd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_line(x1 As Double, y1 As Double, x2 As Double, y2 As Double, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_metricInfo(c As Integer, gc As IntPtr, ByRef ascent As Double, ByRef descent As Double, ByRef width As Double, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_mode(mode As Integer, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_newPage(gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_polygon(n As Integer, x As IntPtr, y As IntPtr, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_Polyline(n As Integer, x As IntPtr, y As IntPtr, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_rect(x0 As Double, y0 As Double, x1 As Double, y1 As Double, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_path(x As IntPtr, y As IntPtr, npoly As Integer, nper As IntPtr, <MarshalAs(UnmanagedType.Bool)> winding As Boolean, gc As IntPtr,
        dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_raster(raster As IntPtr, w As Integer, h As Integer, x As Double, y As Double, width As Double,
        height As Double, rot As Double, <MarshalAs(UnmanagedType.Bool)> interpolate As Boolean, gc As IntPtr, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function _DevDesc_cap(dd As IntPtr) As IntPtr

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_size(ByRef left As Double, ByRef right As Double, ByRef bottom As Double, ByRef top As Double, dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function _DevDesc_strWidth(<MarshalAs(UnmanagedType.LPStr)> str As String, gc As IntPtr, dd As IntPtr) As Double

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_text(x As Double, y As Double, <MarshalAs(UnmanagedType.LPStr)> str As String, rot As Double, hadj As Double, gc As IntPtr,
        dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_onExit(dd As IntPtr)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function _DevDesc_getEvent(sexp As IntPtr, <MarshalAs(UnmanagedType.LPStr)> e As String) As IntPtr

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function _DevDesc_newFrameConfirm(dd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean

    '[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    'internal delegate void _DevDesc_textUTF8(double x, double y, [MarshalAs(UnmanagedType.LPStr)] string str, double rot, double hadj, IntPtr gc, IntPtr dd);
    '[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    'internal delegate double _DevDesc_strWidthUTF8([MarshalAs(UnmanagedType.LPStr)] string str, IntPtr gc, IntPtr dd);
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub _DevDesc_eventHelper(dd As IntPtr, code As Integer)
End Namespace

