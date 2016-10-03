#Region "Microsoft.VisualBasic::2bb0cd0ef3dabfe3f368954350496ef6, ..\GCModeller\visualize\SVG.Extensions\Extensions.vb"

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

Imports System.Drawing
Imports System.Runtime.CompilerServices

Module Extensions

    <Extension> Public Function SVGUnit(px As Integer) As SVG.SvgUnit
        Return New SvgUnit(px)
    End Function

    <Extension> Public Function SVGUnit(px As Single) As SVG.SvgUnit
        Return New SvgUnit(px)
    End Function

    <Extension> Public Function SVGRectangle(rect As Rectangle) As SvgRectangle
        Return New SvgRectangle With {
            .X = rect.X.SVGUnit,
            .Y = rect.Y.SVGUnit,
            .Width = rect.Width.SVGUnit,
            .Height = rect.Height.SVGUnit
        }
    End Function

    <Extension> Public Function SVGRectangle(rect As RectangleF) As SvgRectangle
        Return New SvgRectangle With {
            .X = rect.X.SVGUnit,
            .Y = rect.Y.SVGUnit,
            .Width = rect.Width.SVGUnit,
            .Height = rect.Height.SVGUnit
        }
    End Function

    Public Function SVGRectangle(x As Single, y As Single, width As Single, height As Single) As SvgRectangle
        Return New SvgRectangle With {
            .X = x.SVGUnit,
            .Y = y.SVGUnit,
            .Width = width.SVGUnit,
            .Height = height.SVGUnit
        }
    End Function

    <Extension> Public Function SVGFontStyle(fontstyle As FontStyle) As SvgFontStyle
        Select Case fontstyle
            Case FontStyle.Bold : Return SVG.SvgFontStyle.Oblique
            Case FontStyle.Italic : Return SVG.SvgFontStyle.Italic
            Case FontStyle.Regular : Return SVG.SvgFontStyle.Normal
            Case FontStyle.Strikeout : Return SVG.SvgFontStyle.Oblique

            Case Else
                Return SVG.SvgFontStyle.Normal
        End Select
    End Function
End Module
