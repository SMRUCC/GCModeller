#Region "Microsoft.VisualBasic::f535eace158b55a51e2b2ce3e9ba6daf, RDotNET\Graphics\Internals\GEDevDesc.vb"

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

    '     Structure GEDevDesc
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices

Namespace Graphics.Internals
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
    Friend Structure GEDevDesc
        Friend dev As IntPtr

        <MarshalAs(UnmanagedType.Bool)>
        Friend displayListOn As Boolean

        Friend displayList As IntPtr
        Friend DLlastElt As IntPtr
        Friend savedSnapshot As IntPtr

        <MarshalAs(UnmanagedType.Bool)>
        Friend dirty As Boolean

        <MarshalAs(UnmanagedType.Bool)>
        Friend recordGraphics As Boolean

        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=24)>
        Friend gesd As IntPtr()

        <MarshalAs(UnmanagedType.Bool)>
        Friend ask As Boolean
    End Structure
End Namespace

