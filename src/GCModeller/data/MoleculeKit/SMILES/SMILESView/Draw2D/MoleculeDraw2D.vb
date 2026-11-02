Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Mol = SMRUCC.genomics.Molecule.SMILES.Graph

Namespace Draw2D

    ''' <summary>
    ''' 2D 分子结构图渲染器：把分子图的平面坐标画成键线式结构图。
    ''' </summary>
    ''' <remarks>
    ''' 渲染流程：读取 <see cref="Mol.Atom.X"/>/<see cref="Mol.Atom.Y"/>（若尚未重建则自动调用
    ''' <see cref="Mol.MoleculeGraph.Layout2D"/>）→ 等比缩放到画布 → 翻转 Y 轴（化学坐标系 Y 向上，
    ''' 屏幕坐标 Y 向下）→ 先画键、再画原子标签与标题。
    ''' 输出的设备是 <see cref="IGraphics"/>，因此同一套代码既能出 PNG 也能出 SVG。
    ''' </remarks>
    Public Class MoleculeDraw2D

        Private ReadOnly graph As Mol.MoleculeGraph
        Private ReadOnly theme As Styling.ChemTheme

        ''' <summary>
        ''' 以一个分子图创建渲染器。
        ''' </summary>
        ''' <param name="graph">目标分子图。</param>
        ''' <param name="theme">绘制主题，为空时使用默认主题。</param>
        Public Sub New(graph As Mol.MoleculeGraph, Optional theme As Styling.ChemTheme = Nothing)
            If graph Is Nothing Then Throw New ArgumentNullException(NameOf(graph))

            Me.graph = graph
            Me.theme = If(theme, Styling.ChemTheme.Default)
        End Sub

        ''' <summary>
        ''' 绘制到指定画布。
        ''' </summary>
        ''' <param name="canvas">目标画布（GDI+ 光栅或 SVG 矢量）。</param>
        Public Sub Draw(canvas As IGraphics)
            If graph.AtomCount = 0 Then Return

            If Not hasCoordinates() Then Call graph.Layout2D()

            Dim size As Size = canvas.Size
            Dim padding As Integer = theme.Padding
            Dim titleHeight As Integer = If(theme.ShowTitle, CInt(theme.TitleFontSize) + padding, 0)
            ' 原子标签以原子坐标为中心绘制，因此四周需要额外留出半个字号的高度
            Dim labelMargin As Integer = CInt(Math.Ceiling(theme.AtomFontSize * 0.75))

            Dim minX As Double = Double.MaxValue
            Dim maxX As Double = Double.MinValue
            Dim minY As Double = Double.MaxValue
            Dim maxY As Double = Double.MinValue

            For Each atom As Mol.Atom In graph.Atoms
                minX = Math.Min(minX, atom.X)
                maxX = Math.Max(maxX, atom.X)
                minY = Math.Min(minY, atom.Y)
                maxY = Math.Max(maxY, atom.Y)
            Next

            Dim spanX As Double = Math.Max(maxX - minX, 1.0E-6)
            Dim spanY As Double = Math.Max(maxY - minY, 1.0E-6)
            Dim availableWidth As Double = Math.Max(size.Width - 2 * (padding + labelMargin), 1)
            Dim availableHeight As Double = Math.Max(size.Height - 2 * (padding + labelMargin) - titleHeight, 1)
            Dim scale As Double = Math.Min(availableWidth / spanX, availableHeight / spanY)

            ' 保持纵横比并居中
            Dim contentWidth As Double = spanX * scale
            Dim contentHeight As Double = spanY * scale
            Dim left As Double = (size.Width - contentWidth) / 2
            Dim top As Double = titleHeight + labelMargin + (availableHeight - contentHeight) / 2

            Dim project As Func(Of Mol.Atom, PointF) =
                Function(atom As Mol.Atom)
                    Return New PointF(
                        CSng(left + (atom.X - minX) * scale),
                        CSng(top + (maxY - atom.Y) * scale))
                End Function

            Dim ringCenters As Dictionary(Of Integer, PointF) = ringCentersOf(project)

            For bondIndex = 0 To graph.Bonds.Count - 1
                Dim bond As Mol.Bond = graph.Bonds(bondIndex)
                Dim center As Integer = If(bond.Stereo = Mol.BondStereo.None, -1, stereoCenterOf(bond))
                Dim inward As PointF = Nothing
                Dim hasInward As Boolean = ringCenters.TryGetValue(bondIndex, inward)
                Dim middle As New PointF(
                    CSng((project(graph.Atoms(bond.a)).X + project(graph.Atoms(bond.b)).X) / 2),
                    CSng((project(graph.Atoms(bond.a)).Y + project(graph.Atoms(bond.b)).Y) / 2))

                Call BondDrawer.Draw(canvas,
                    project(graph.Atoms(bond.a)), project(graph.Atoms(bond.b)), bond, theme, center,
                    If(hasInward, inward.X - middle.X, 0),
                    If(hasInward, inward.Y - middle.Y, 0),
                    hasInward)
            Next

            For i = 0 To graph.AtomCount - 1
                Dim text As String = AtomLabelDrawer.LabelOf(graph, i, theme)

                If String.IsNullOrEmpty(text) Then Continue For

                Dim atom As Mol.Atom = graph.Atoms(i)
                Dim color As Color = Styling.ElementColors.ColorOf(atom.Element)

                Call AtomLabelDrawer.Draw(canvas, text, project(atom), color, theme)
            Next

            If theme.ShowTitle Then
                Call drawTitle(canvas, size)
            End If
        End Sub

        ''' <summary>
        ''' 计算每个环键所属环在屏幕坐标下的质心，供双键/芳香键选择"环内侧"方向。
        ''' </summary>
        Private Function ringCentersOf(project As Func(Of Mol.Atom, PointF)) As Dictionary(Of Integer, PointF)
            Dim centers As New Dictionary(Of Integer, PointF)

            For Each ring As Mol.Ring In graph.Rings()
                Dim cx As Double = 0
                Dim cy As Double = 0

                For Each atom As Integer In ring.Atoms
                    Dim point As PointF = project(graph.Atoms(atom))

                    cx += point.X
                    cy += point.Y
                Next

                If ring.Atoms.Length = 0 Then Continue For

                Dim center As New PointF(CSng(cx / ring.Atoms.Length), CSng(cy / ring.Atoms.Length))

                For Each bondIndex As Integer In ring.BondIndices
                    If Not centers.ContainsKey(bondIndex) Then centers(bondIndex) = center
                Next
            Next

            Return centers
        End Function

        ''' <summary>
        ''' 找出楔形键上作为"收窄端"的手性中心原子。
        ''' </summary>
        Private Function stereoCenterOf(bond As Mol.Bond) As Integer
            If graph.Atoms(bond.a).Stereo <> Mol.AtomStereo.None Then Return bond.a
            If graph.Atoms(bond.b).Stereo <> Mol.AtomStereo.None Then Return bond.b

            Return -1
        End Function

        Private Sub drawTitle(canvas As IGraphics, size As Size)
            Dim title As String = graph.MolecularFormula()

            If Not String.IsNullOrEmpty(graph.SourceText) Then
                Dim source As String = graph.SourceText.Trim()

                If source.Length > 96 Then source = source.Substring(0, 93) & "..."

                title = title & "    " & source
            End If

            Dim font As New Font(theme.FontName, theme.TitleFontSize, FontStyle.Regular)
            Dim color As Color = Styling.ElementColors.ParseHex(theme.TitleColor, Color.Gray)

            Call canvas.DrawString(title, font, New SolidBrush(color), CSng(theme.Padding), CSng(theme.Padding / 2))
        End Sub

        Private Function hasCoordinates() As Boolean
            Dim extent As Double = 0

            For Each atom As Mol.Atom In graph.Atoms
                extent += Math.Abs(atom.X) + Math.Abs(atom.Y)
            Next

            Return extent > 1.0E-6
        End Function

    End Class

End Namespace
