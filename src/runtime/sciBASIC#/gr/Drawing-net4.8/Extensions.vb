﻿#Region "Microsoft.VisualBasic::5d354d77c45168bacbe89869a17fd4f9, gr\Drawing-net4.8\Extensions.vb"

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


    ' Code Statistics:

    '   Total Lines: 243
    '    Code Lines: 162 (66.67%)
    ' Comment Lines: 49 (20.16%)
    '    - Xml Docs: 89.80%
    ' 
    '   Blank Lines: 32 (13.17%)
    '     File Size: 9.17 KB


    ' Module Extensions
    ' 
    '     Function: CanvasCreateFromImageFile, CreateCanvas2D, (+4 Overloads) CreateGDIDevice, CreateObject, (+2 Overloads) GetIcon
    '               GetStringPath, SaveIcon
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Bitmap = System.Drawing.Bitmap
Imports Font = System.Drawing.Font
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports Image = System.Drawing.Image
Imports Pens = System.Drawing.Pens

Public Module Extensions

    <Extension>
    Public Function SaveIcon(ico As Icon, path$) As Boolean
        Call path.ParentPath.MakeDir

        Try
            Using file As New FileStream(path, FileMode.OpenOrCreate)
                Call ico.Save(file)
                Call file.Flush()
            End Using

            Return True
        Catch ex As Exception
            Call App.LogException(New Exception(path, ex))
        End Try

        Return False
    End Function

    ''' <summary>
    ''' 无需处理图像数据，这个函数默认已经自动克隆了该对象，不会影响到原来的对象，
    ''' 除非你将<paramref name="directAccess"/>参数设置为真，函数才不会自动克隆图像对象
    ''' </summary>
    ''' <param name="res"></param>
    ''' <param name="bg">
    ''' the color string literal value of the default background color
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateCanvas2D(res As Image,
                                       Optional directAccess As Boolean = False,
                                       Optional bg As String = Nothing,
                                       <CallerMemberName>
                                       Optional caller$ = "") As Graphics2D

        If directAccess Then
            Return Graphics2D.CreateObject(Graphics.FromImage(res), res)
        Else
            With res.Size.CreateGDIDevice(filled:=bg.TranslateColor)
                Call .DrawImage(res, 0, 0, .Width, .Height)
                Return .ByRef
            End With
        End If
    End Function

    ''' <summary>
    ''' 从指定的文件之中加载GDI+设备的句柄
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    '''
    <Extension>
    Public Function CanvasCreateFromImageFile(path As String) As Graphics2D
#If NET8_0_OR_GREATER Then
        Throw New NotImplementedException
#Else
                Dim image As Image = LoadImage(path)
        Dim g As Graphics = Graphics.FromImage(image)

        With g
            .CompositingQuality = CompositingQuality.HighQuality
            .TextRenderingHint = TextRenderingHint.ClearTypeGridFit
        End With

        Return Graphics2D.CreateObject(g, image)
#End If
    End Function

    ''' <summary>
    ''' 创建一个GDI+的绘图设备
    ''' </summary>
    ''' <param name="r"></param>
    ''' <param name="filled">默认的背景填充颜色为白色</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("GDI+.Create")>
    <Extension>
    Public Function CreateGDIDevice(r As SizeF, Optional filled As Color = Nothing) As Graphics2D
        Return (New Size(CInt(r.Width), CInt(r.Height))).CreateGDIDevice(filled)
    End Function

    ''' <summary>
    ''' Convert image to icon
    ''' </summary>
    ''' <param name="res"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("To.Icon")>
    <Extension> Public Function GetIcon(res As Image) As Icon
        Return Icon.FromHandle(New Bitmap(res).GetHicon)
    End Function

    ''' <summary>
    ''' Convert image to icon
    ''' </summary>
    ''' <param name="res"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("To.Icon")>
    <Extension> Public Function GetIcon(res As Bitmap) As Icon
        Return Icon.FromHandle(res.GetHicon)
    End Function

    ''' <summary>
    ''' Internal create gdi device helper.(这个函数不会克隆原来的图像对象<paramref name="res"/>)
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="res">绘图的基础图像对象</param>
    ''' <returns></returns>
    Friend Function CreateObject(g As Graphics, res As System.Drawing.Image) As Graphics2D
        Dim img As Image

#If NET48 Then
        img = res 
#Else
        Throw New NotImplementedException
#End If

        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.CompositingQuality = CompositingQuality.HighQuality
        g.SmoothingMode = SmoothingMode.HighQuality
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit

        With New Graphics2D(img) With {.g = g}
#If NET8_0_OR_GREATER Then
            .Font = New Microsoft.VisualBasic.Imaging.Font(FontFace.MicrosoftYaHei, 12)
            .Stroke = Microsoft.VisualBasic.Imaging.Pens.Black
#Else
            .Font = New Font(FontFace.MicrosoftYaHei, 12)
            .Stroke = Pens.Black
#End If

            ' .Clear(Color.Transparent)
            Return .ByRef
        End With
    End Function

    Const InvalidSize As String = "One of the size parameter for the gdi+ device is not valid!"

    <Extension>
    Public Function GetStringPath(s$, dpi!, rect As RectangleF, font As Font, format As StringFormat) As GraphicsPath
        Dim path As New GraphicsPath()
        ' Convert font size into appropriate coordinates
        Dim emSize! = dpi * font.SizeInPoints / 72
        path.AddString(s, font.FontFamily, font.Style, emSize, rect, format)
        Return path
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function CreateGDIDevice(t As (width%, height%),
                                    Optional filled As Color = Nothing,
                                    <CallerMemberName>
                                    Optional trace$ = "",
                                    Optional dpi$ = "100,100") As Graphics2D

        Return CreateGDIDevice(t.width, t.height, filled:=filled, dpi:=dpi, trace:=trace)
    End Function

    ''' <summary>
    ''' 创建一个GDI+的绘图设备，默认的背景填充色为白色
    ''' </summary>
    ''' <param name="r"></param>
    ''' <param name="filled">默认的背景填充颜色为白色</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("GDI+.Create")>
    <Extension> Public Function CreateGDIDevice(r As Size,
                                                Optional filled As Color = Nothing,
                                                <CallerMemberName>
                                                Optional trace$ = "",
                                                Optional dpi$ = "100,100") As Graphics2D
        Return CreateGDIDevice(r.Width, r.Height, filled:=filled, dpi:=dpi, trace:=trace)
    End Function

    Public Function CreateGDIDevice(width%, height%,
                                    Optional filled As Color = Nothing,
                                    <CallerMemberName>
                                    Optional trace$ = "",
                                    Optional dpi$ = "100,100") As Graphics2D
        Dim bitmap As Bitmap
        Dim dpi_sz As Size = dpi.SizeParser

        If width <= 0 OrElse height <= 0 Then
            Throw New Exception(InvalidSize)
        ElseIf dpi_sz.Width <= 0 OrElse dpi_sz.Height <= 0 Then
            Throw New Exception("dpi size should be a tuple of the positive integers!")
        End If

        Try
            bitmap = New Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)

            With dpi_sz
                Call bitmap.SetResolution(.Width, .Height)
            End With
        Catch ex As Exception
            ex = New Exception(New Size(width, height).ToString, ex)
            ex = New Exception(trace, ex)
            Call App.LogException(ex, MethodBase.GetCurrentMethod.GetFullName)
            Throw ex
        End Try

        Dim g As Graphics = Graphics.FromImage(bitmap)
        Dim rect As New Rectangle(New Point, bitmap.Size)

        If filled.IsNullOrEmpty Then
            filled = Color.White
        End If

        Call g.Clear(filled)

        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.CompositingQuality = CompositingQuality.HighQuality
        g.SmoothingMode = SmoothingMode.HighQuality
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit
        g.CompositingMode = CompositingMode.SourceOver

        Return Graphics2D.CreateObject(g, bitmap)
    End Function
End Module
