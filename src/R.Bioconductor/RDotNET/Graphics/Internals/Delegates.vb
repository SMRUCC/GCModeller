#Region "Microsoft.VisualBasic::302de02b6c4d2922028056199c5244ad, RDotNET\Graphics\Internals\Delegates.vb"

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
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_GE_checkVersionOrDie(version As Integer)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_CheckDeviceAvailable()

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub Rf_onintr()

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function GEcreateDevDesc(dev As IntPtr) As IntPtr

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub GEaddDevice2(dev As IntPtr, name As String)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub GEkillDevice(dev As IntPtr)
End Namespace

