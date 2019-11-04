Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace Drawing2D

    Public Module Shadow

        <Extension>
        Public Sub DrawCircleShadow(g As IGraphics, centra As PointF, radius As Single,
                                    Optional shadowColor$ = NameOf(Color.Gray),
                                    Optional alphaLevels$ = "0,120,150,200",
                                    Optional gradientLevels$ = "[0,0.125,0.5,1]")

            Dim path As New GraphicsPath()
            Dim points As PointF() = Shapes.Circle.PathIterator(centra, radius, 100).ToArray
            Dim a As PointF = points(Scan0)

            For Each vertex As PointF In points.Skip(1)
                Call path.AddLine(a, vertex)
            Next
        End Sub

        ''' <summary>
        ''' Draw shadow of a specifc <paramref name="rectangle"/>
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="rectangle"></param>
        ''' <param name="shadowColor$"></param>
        ''' <param name="alphaLevels$"></param>
        ''' <param name="gradientLevels$"></param>
        <Extension> Public Sub DropdownShadows(g As IGraphics,
                                               rectangle As RectangleF,
                                               Optional shadowColor$ = NameOf(Color.Gray),
                                               Optional alphaLevels$ = "0,120,150,200",
                                               Optional gradientLevels$ = "[0,0.125,0.5,1]")
            Dim path As New GraphicsPath

            Call path.AddRectangle(rectangle)
            Call path.CloseAllFigures()
            Call g.DropdownShadows(path, shadowColor, alphaLevels, gradientLevels)
        End Sub

        ''' <summary>
        ''' Draw shadow of a specifc <paramref name="polygon"/>
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="polygon"></param>
        ''' <param name="shadowColor$"></param>
        ''' <param name="alphaLevels$"></param>
        ''' <param name="gradientLevels$"></param>
        <Extension> Public Sub DropdownShadows(g As IGraphics,
                                               polygon As GraphicsPath,
                                               Optional shadowColor$ = NameOf(Color.Gray),
                                               Optional alphaLevels$ = "0,120,150,200",
                                               Optional gradientLevels$ = "[0,0.125,0.5,1]")

            Dim alphas As Vector = alphaLevels
            ' Create a color blend to manage our colors And positions And
            ' since we need 3 colors set the default length to 3
            Dim colorBlend As New ColorBlend(alphas.Length)
            Dim baseColor As Color = shadowColor.TranslateColor

            ' here Is the important part of the shadow making process, remember
            ' the clamp mode on the colorblend object layers the colors from
            ' the outside to the center so we want our transparent color first
            ' followed by the actual shadow color. Set the shadow color to a 
            ' slightly transparent DimGray, I find that it works best.|
            colorBlend.Colors = alphas _
                .Select(Function(a) Color.FromArgb(a, baseColor)) _
                .ToArray

            ' our color blend will control the distance of each color layer
            ' we want to set our transparent color to 0 indicating that the 
            ' transparent color should be the outer most color drawn, then
            ' our Dimgray color at about 10% of the distance from the edge
            colorBlend.Positions = CType(gradientLevels, Vector).AsSingle

            ' this Is where we create the shadow effect, so we will use a 
            ' pathgradientbursh And assign our GraphicsPath that we created of a 
            ' Rounded Rectangle
            Using pgBrush As New PathGradientBrush(polygon) With {
                .WrapMode = WrapMode.Clamp,
                .InterpolationColors = colorBlend
            }
                ' fill the shadow with our pathgradientbrush
                Call g.FillPath(pgBrush, polygon)
            End Using
        End Sub

    End Module
End Namespace