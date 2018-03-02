#Region "Microsoft.VisualBasic::bf5220682ff9105891881b6540bd0321, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\Graphics\grDevices\grDevice.vb"

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

    '     Class grDevice
    ' 
    '         Properties: bg, family, height, width
    ' 
    '         Function: (+2 Overloads) Plot
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

Namespace SymbolBuilder.packages.grDevices

    Public MustInherit Class grDevice : Inherits IRToken

        ''' <summary>
        ''' the width of the device.
        ''' </summary>
        ''' <returns></returns>
        Public Property width As Integer = 480
        ''' <summary>
        ''' the height of the device.
        ''' </summary>
        ''' <returns></returns>
        Public Property height As Integer = 480
        ''' <summary>
        ''' A length-one character vector specifying the default font family. The default means to use the font numbers on the Windows GDI versions and "sans" on the cairographics versions.
        ''' </summary>
        ''' <returns></returns>
        Public Property family As String = ""
        ''' <summary>
        ''' the initial background colour: can be overridden by setting par("bg").
        ''' </summary>
        ''' <returns></returns>
        Public Property bg As String = "white"

        ''' <summary>
        ''' 生成创建图像文件的脚本代码
        ''' </summary>
        ''' <param name="plots">绘图的脚本表达式</param>
        ''' <returns></returns>
        Public Function Plot(plots As String) As String
            Dim script As New StringBuilder(RScript)
            Call script.AppendLine()
            Call script.AppendLine()
            Call script.AppendLine(plots)
            Call script.AppendLine("dev.off()")

            Return script.ToString
        End Function

        ''' <summary>
        ''' 生成创建图像文件的脚本代码
        ''' </summary>
        ''' <param name="plots">绘图的脚本表达式</param>
        ''' <returns></returns>
        Public Function Plot(plots As Func(Of String)) As String
            Return Plot(plots())
        End Function
    End Class
End Namespace
