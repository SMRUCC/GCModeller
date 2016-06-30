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
