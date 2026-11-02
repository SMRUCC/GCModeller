Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Mol = SMRUCC.genomics.Molecule.SMILES.Graph
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle

Namespace Draw2D

    ''' <summary>
    ''' 化学键的 2D 绘制：单/双/叁键、芳香键、实楔形与虚楔形。
    ''' </summary>
    ''' <remarks>
    ''' 所有坐标都是已经换算到屏幕空间的像素坐标。双键/叁键以键轴的法线方向偏移出平行线，
    ''' 立体键按照 SMILES 的 <c>/</c> 与 <c>\</c> 标记（映射到 <see cref="Mol.BondStereo"/>）绘制楔形。
    ''' </remarks>
    Public Module BondDrawer

        ''' <summary>
        ''' 绘制一条化学键。
        ''' </summary>
        ''' <param name="canvas">目标画布。</param>
        ''' <param name="p1">起点（屏幕坐标）。</param>
        ''' <param name="p2">终点（屏幕坐标）。</param>
        ''' <param name="bond">键对象。</param>
        ''' <param name="theme">绘制主题。</param>
        ''' <param name="stereoCenter">手性中心所在的原子下标（楔形收窄端），未知时传 <c>-1</c>。</param>
        ''' <param name="inwardX">环内方向的 X 分量（屏幕坐标），仅在 <paramref name="hasInward"/> 为真时使用。</param>
        ''' <param name="inwardY">环内方向的 Y 分量（屏幕坐标），仅在 <paramref name="hasInward"/> 为真时使用。</param>
        ''' <param name="hasInward">是否已知该键的"环内侧"方向（用于把双键的第二条线画在环内）。</param>
        Public Sub Draw(canvas As IGraphics, p1 As PointF, p2 As PointF, bond As Mol.Bond, theme As Styling.ChemTheme,
                        Optional stereoCenter As Integer = -1,
                        Optional inwardX As Double = 0,
                        Optional inwardY As Double = 0,
                        Optional hasInward As Boolean = False)

            If stereoCenter >= 0 AndAlso bond.a = stereoCenter AndAlso bond.b <> stereoCenter Then
                Dim swap As PointF = p1

                p1 = p2
                p2 = swap
            End If

            Dim color As Color = Styling.ElementColors.ParseHex(theme.BondColor, Color.Black)
            Dim dx As Double = p2.X - p1.X
            Dim dy As Double = p2.Y - p1.Y
            Dim length As Double = Math.Sqrt(dx * dx + dy * dy)

            If length < 0.001 Then Return

            Dim ux As Double = dx / length
            Dim uy As Double = dy / length
            Dim nx As Double = -uy
            Dim ny As Double = ux

            Select Case bond.Stereo
                Case Mol.BondStereo.Up
                    Call drawWedge(canvas, p1, p2, nx, ny, length, color, True)
                    Return
                Case Mol.BondStereo.Down
                    Call drawWedge(canvas, p1, p2, nx, ny, length, color, False)
                    Return
            End Select

            ' 只有在未做 Kekulé 化（键级仍为 1.5）时才用虚线表示芳香键；
            ' 完成 Kekulé 化之后按经典的单/双键画法绘制，避免两种表示混用
            Dim aromatic As Boolean = bond.IsAromatic AndAlso Math.Abs(bond.order - 1.5) < 0.01
            Dim order As Integer = If(aromatic, 1, bond.OrderInt)

            ' 环内侧方向决定附加平行线画在哪一边
            Dim sign As Double = 1

            If hasInward AndAlso (nx * inwardX + ny * inwardY) < 0 Then sign = -1

            Select Case order
                Case 1
                    Call canvas.DrawLine(New Pen(color, theme.BondWidth), p1, p2)

                    If aromatic Then
                        Dim dash As New Pen(color, Math.Max(1.0F, theme.BondWidth * 0.6F)) With {
                            .DashStyle = DashStyle.Dash
                        }

                        Call canvas.DrawLine(dash,
                            offsetPoint(p1, nx, ny, CSng(sign * theme.BondSeparation * 0.7)),
                            offsetPoint(p2, nx, ny, CSng(sign * theme.BondSeparation * 0.7)))
                    End If

                Case 2
                    If hasInward Then
                        ' 环上的双键：一条线在键轴上，另一条线向环内偏移并略微缩短
                        Dim offset As Single = CSng(sign * theme.BondSeparation)

                        Call canvas.DrawLine(New Pen(color, theme.BondWidth), p1, p2)
                        Call canvas.DrawLine(New Pen(color, theme.BondWidth),
                            offsetPoint(shrink(p1, p2, 0.12), nx, ny, offset),
                            offsetPoint(shrink(p2, p1, 0.12), nx, ny, offset))
                    Else
                        Dim offset As Single = theme.BondSeparation * 0.5F

                        Call canvas.DrawLine(New Pen(color, theme.BondWidth),
                            offsetPoint(p1, nx, ny, offset), offsetPoint(p2, nx, ny, offset))
                        Call canvas.DrawLine(New Pen(color, theme.BondWidth),
                            offsetPoint(p1, nx, ny, -offset), offsetPoint(p2, nx, ny, -offset))
                    End If

                Case Else
                    Dim offset As Single = theme.BondSeparation

                    Call canvas.DrawLine(New Pen(color, theme.BondWidth), p1, p2)
                    Call canvas.DrawLine(New Pen(color, theme.BondWidth),
                        offsetPoint(p1, nx, ny, offset), offsetPoint(p2, nx, ny, offset))
                    Call canvas.DrawLine(New Pen(color, theme.BondWidth),
                        offsetPoint(p1, nx, ny, -offset), offsetPoint(p2, nx, ny, -offset))
            End Select
        End Sub

        ''' <summary>
        ''' 把点 <paramref name="p"/> 沿 <paramref name="towards"/> 方向缩短给定的比例。
        ''' </summary>
        Private Function shrink(p As PointF, towards As PointF, ratio As Double) As PointF
            Return New PointF(
                CSng(p.X + (towards.X - p.X) * ratio),
                CSng(p.Y + (towards.Y - p.Y) * ratio))
        End Function

        Private Function offsetPoint(p As PointF, nx As Double, ny As Double, offset As Single) As PointF
            Return New PointF(CSng(p.X + nx * offset), CSng(p.Y + ny * offset))
        End Function

        ''' <summary>
        ''' 绘制实楔形（<paramref name="solid"/>）或虚楔形键。
        ''' </summary>
        Private Sub drawWedge(canvas As IGraphics, p1 As PointF, p2 As PointF,
                              nx As Double, ny As Double, length As Double, color As Color, solid As Boolean)

            Dim halfWidth As Single = CSng(Math.Min(14.0, length * 0.16))
            Dim brush As Brush = New SolidBrush(color)

            If solid Then
                Dim points As PointF() = {
                    p1,
                    New PointF(CSng(p2.X + nx * halfWidth), CSng(p2.Y + ny * halfWidth)),
                    New PointF(CSng(p2.X - nx * halfWidth), CSng(p2.Y - ny * halfWidth))
                }

                Call canvas.FillPolygon(brush, points)
            Else
                Const STEPS As Integer = 6

                For i As Integer = 1 To STEPS
                    Dim t As Double = i / STEPS
                    Dim center As New PointF(CSng(p1.X + (p2.X - p1.X) * t), CSng(p1.Y + (p2.Y - p1.Y) * t))
                    Dim width As Single = CSng(halfWidth * t)
                    Dim pen As New Pen(color, 1.2F)

                    Call canvas.DrawLine(pen,
                        New PointF(CSng(center.X + nx * width), CSng(center.Y + ny * width)),
                        New PointF(CSng(center.X - nx * width), CSng(center.Y - ny * width)))
                Next
            End If
        End Sub

    End Module

End Namespace
