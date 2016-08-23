#Region "Microsoft.VisualBasic::9e15c4a136cbb5ef8ed7e58c46d5b580, ..\R.Bioconductor\RDotNET\Graphics\Internals\Delegates.vb"

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

Imports System.Runtime.InteropServices

Namespace Graphics.Internals
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub R_GE_checkVersionOrDie(version As Integer)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub R_CheckDeviceAvailable()

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub Rf_onintr()

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Function GEcreateDevDesc(dev As IntPtr) As IntPtr

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub GEaddDevice2(dev As IntPtr, name As String)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
    Friend Delegate Sub GEkillDevice(dev As IntPtr)
End Namespace

