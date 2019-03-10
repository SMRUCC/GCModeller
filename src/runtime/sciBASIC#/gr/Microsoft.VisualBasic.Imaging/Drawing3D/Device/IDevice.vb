﻿#Region "Microsoft.VisualBasic::8f57fa2c96544f95b99df7b9c61cd0a9, gr\Microsoft.VisualBasic.Imaging\Drawing3D\Device\IDevice.vb"

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

    '     Class IDevice
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Delegate Sub
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

Namespace Drawing3D.Device

    Public MustInherit Class IDevice(Of T As UserControl)

        Protected WithEvents device As T

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(dev As T)
            device = dev
        End Sub

        Public Overrides Function ToString() As String
            Return device.ToString
        End Function
    End Class

    ''' <summary>
    ''' 3D plot for <see cref="Gdidevice"/>，由于是需要将图像显示到WinFom控件上面，所以在这里要求的是gdi+的图形驱动程序
    ''' </summary>
    ''' <param name="canvas">gdi+ handle</param>
    ''' <param name="camera">3d camera</param>
    Public Delegate Sub DrawGraphics(ByRef canvas As Graphics, camera As Camera)

End Namespace
