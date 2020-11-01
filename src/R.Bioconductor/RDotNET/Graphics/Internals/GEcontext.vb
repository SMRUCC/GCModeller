#Region "Microsoft.VisualBasic::d626a38d872d64fe629c4ae20fff9ca0, RDotNET\Graphics\Internals\GEcontext.vb"

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

    '     Structure GEcontext
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices

Namespace Graphics.Internals
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
    Friend Structure GEcontext
        Friend col As Color
        Friend fill As Color
        Friend gamma As Double
        Friend lwd As Double
        Friend lty As LineType
        Friend lend As LineEnd
        Friend ljoin As LineJoin
        Friend lmitre As Double
        Friend cex As Double
        Friend ps As Double
        Friend lineheight As Double
        Friend fontface As FontFace

        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=201)>
        Friend fontfamily As String
    End Structure
End Namespace

