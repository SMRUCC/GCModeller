Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Mol = SMRUCC.genomics.Molecule.SMILES.Graph

Namespace Draw2D

    ''' <summary>
    ''' 原子标签的绘制：元素符号 + 氢数下标 + 电荷上标。
    ''' </summary>
    Public Module AtomLabelDrawer

        ''' <summary>
        ''' 在指定位置居中绘制一段带白色底衬的文本。
        ''' </summary>
        ''' <param name="canvas">目标画布。</param>
        ''' <param name="text">标签文本。</param>
        ''' <param name="center">文本中心（屏幕坐标）。</param>
        ''' <param name="color">文本颜色。</param>
        ''' <param name="theme">绘制主题。</param>
        Public Sub Draw(canvas As IGraphics, text As String, center As PointF, color As Color, theme As Styling.ChemTheme)
            If String.IsNullOrEmpty(text) Then Return

            Dim font As New Font(theme.FontName, theme.AtomFontSize, FontStyle.Regular)
            Dim size As SizeF = canvas.MeasureString(text, font)
            Dim rect As New RectangleF(
                CSng(center.X - size.Width / 2),
                CSng(center.Y - size.Height / 2),
                size.Width,
                size.Height)

            Dim halo As Brush = New SolidBrush(Color.FromArgb(theme.LabelHaloAlpha, 255, 255, 255))

            Call canvas.FillRectangle(halo, rect)
            Call canvas.DrawString(text, font, New SolidBrush(color), rect)
        End Sub

        ''' <summary>
        ''' 生成某个原子的显示文本；不需要标注时返回空字符串。
        ''' </summary>
        ''' <param name="graph">分子图。</param>
        ''' <param name="index">原子下标。</param>
        ''' <param name="theme">绘制主题。</param>
        Public Function LabelOf(graph As Mol.MoleculeGraph, index As Integer, theme As Styling.ChemTheme) As String
            Dim atom As Mol.Atom = graph.Atoms(index)

            If atom.IsH Then Return "H"

            If theme.HideCarbon AndAlso atom.Element = "C" AndAlso atom.Charge = 0 AndAlso atom.Isotope = 0 Then
                Return ""
            End If

            Dim text As New System.Text.StringBuilder(atom.Element)

            If atom.Isotope <> 0 Then
                text.Insert(0, atom.Isotope.ToString)
            End If

            Dim hydrogens As Integer = graph.TotalH(index)

            If hydrogens > 0 Then
                text.Append("H")

                If hydrogens > 1 Then text.Append(hydrogens.ToString)
            End If

            If atom.Charge <> 0 Then
                If Math.Abs(atom.Charge) = 1 Then
                    text.Append(If(atom.Charge > 0, "+", "-"))
                Else
                    text.Append(atom.Charge.ToString).Append(If(atom.Charge > 0, "+", "-"))
                End If
            End If

            Return text.ToString()
        End Function

    End Module

End Namespace
