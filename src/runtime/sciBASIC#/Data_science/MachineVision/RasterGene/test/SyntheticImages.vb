' ============================================================================
' SyntheticImages.vb - 用 GDI+ 绘制训练 / 测试用的合成图像
'
' 为什么不直接用真实图片：
'   32x32 的小图上，卷积 VAE 的可见效果必须来自「结构可辨认」的数据。这里用 5 类
'   几何图案（渐变、圆斑、条纹、棋盘、同心环）保证：
'     * 每类有明确的结构，训练后能看出重构是否真的学到了形状；
'     * 类别之间颜色 / 纹理差异明显，随机生成的样本容易用肉眼判断是否合理；
'     * 全部由固定种子生成，回归测试可完全复现。
' ============================================================================

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports std = System.Math

''' <summary>
''' 合成图案的种类。
''' </summary>
Public Enum PatternKind

    ''' <summary>双色线性渐变背景。</summary>
    Gradient

    ''' <summary>背景 + 若干实心圆斑。</summary>
    Circles

    ''' <summary>背景 + 等宽条纹。</summary>
    Stripes

    ''' <summary>双色棋盘。</summary>
    Checkerboard

    ''' <summary>同心圆环。</summary>
    Rings

End Enum

''' <summary>
''' GDI+ 合成图像生成器。
''' </summary>
Public Module SyntheticImages

    ''' <summary>全部图案种类。</summary>
    Public ReadOnly Property AllKinds As PatternKind()
        Get
            Return New PatternKind() {
                PatternKind.Gradient, PatternKind.Circles, PatternKind.Stripes,
                PatternKind.Checkerboard, PatternKind.Rings
            }
        End Get
    End Property

    ''' <summary>
    ''' 生成 <paramref name="count"/> 张 <paramref name="size"/> 见方的 RGB 图像，
    ''' 各图案种类均匀分布且顺序已打乱。
    ''' </summary>
    Public Function CreateDataset(count As Integer, size As Integer, seed As Integer) As Bitmap()
        If count <= 0 Then
            Throw New ArgumentException($"样本数必须为正，实际 {count}", NameOf(count))
        End If

        Dim rng As New Random(seed)
        Dim kinds As PatternKind() = AllKinds
        Dim images As Bitmap() = New Bitmap(count - 1) {}

        For i As Integer = 0 To count - 1
            ' 轮转分配保证每类数量均衡，再让随机数决定具体的颜色与布局
            Dim kind As PatternKind = kinds(i Mod kinds.Length)

            images(i) = Create(kind, size, rng)
        Next

        Call Shuffle(images, rng)

        Return images
    End Function

    ''' <summary>按指定图案生成一张图像。</summary>
    Public Function Create(kind As PatternKind, size As Integer, rng As Random) As Bitmap
        If rng Is Nothing Then
            Throw New ArgumentNullException(NameOf(rng))
        End If

        If size < 8 Then
            Throw New ArgumentException($"图像尺寸至少为 8，实际 {size}", NameOf(size))
        End If

        Dim bitmap As New Bitmap(size, size, PixelFormat.Format32bppArgb)

        Using g As Graphics = Graphics.FromImage(bitmap)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.Clear(Background(rng))

            Select Case kind
                Case PatternKind.Gradient
                    Call DrawGradient(g, size, rng)
                Case PatternKind.Circles
                    Call DrawCircles(g, size, rng)
                Case PatternKind.Stripes
                    Call DrawStripes(g, size, rng)
                Case PatternKind.Checkerboard
                    Call DrawCheckerboard(g, size, rng)
                Case PatternKind.Rings
                    Call DrawRings(g, size, rng)
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(kind), kind, "未知的图案种类")
            End Select
        End Using

        Return bitmap
    End Function

#Region "各类图案"

    Private Sub DrawGradient(g As Graphics, size As Integer, rng As Random)
        Dim start As Color = Vivid(rng)
        Dim [end] As Color = Vivid(rng)
        Dim horizontal As Boolean = rng.Next(2) = 0

        Using brush As New LinearGradientBrush(
            New Point(0, 0),
            If(horizontal, New Point(size, 0), New Point(0, size)),
            start, [end])

            Call g.FillRectangle(brush, 0, 0, size, size)
        End Using
    End Sub

    Private Sub DrawCircles(g As Graphics, size As Integer, rng As Random)
        Dim count As Integer = rng.Next(2, 5)

        For i As Integer = 0 To count - 1
            Dim radius As Single = CSng(size * (0.14 + rng.NextDouble() * 0.22))
            Dim centerX As Single = CSng(rng.Next(0, size))
            Dim centerY As Single = CSng(rng.Next(0, size))

            Using brush As New SolidBrush(Vivid(rng))
                Call g.FillEllipse(brush,
                                   centerX - radius, centerY - radius,
                                   radius * 2.0F, radius * 2.0F)
            End Using
        Next
    End Sub

    Private Sub DrawStripes(g As Graphics, size As Integer, rng As Random)
        Dim bands As Integer = rng.Next(3, 7)
        Dim vertical As Boolean = rng.Next(2) = 0
        Dim width As Single = CSng(size) / bands

        For i As Integer = 0 To bands - 1
            If i Mod 2 = 0 Then
                Continue For
            End If

            Using brush As New SolidBrush(Vivid(rng))
                If vertical Then
                    Call g.FillRectangle(brush, i * width, 0, width, size)
                Else
                    Call g.FillRectangle(brush, 0, i * width, size, width)
                End If
            End Using
        Next
    End Sub

    Private Sub DrawCheckerboard(g As Graphics, size As Integer, rng As Random)
        Dim cell As Integer = rng.Next(4, 9)
        Dim first As Color = Vivid(rng)
        Dim second As Color = Vivid(rng)

        Using brushA As New SolidBrush(first), brushB As New SolidBrush(second)
            For y As Integer = 0 To size - 1 Step cell
                For x As Integer = 0 To size - 1 Step cell
                    Dim brush As Brush = If(((x \ cell) + (y \ cell)) Mod 2 = 0, brushA, brushB)

                    Call g.FillRectangle(brush, x, y, cell, cell)
                Next
            Next
        End Using
    End Sub

    Private Sub DrawRings(g As Graphics, size As Integer, rng As Random)
        Dim rings As Integer = rng.Next(3, 6)
        Dim center As Single = CSng(size) / 2.0F
        Dim outer As Single = CSng(size) * 0.62F
        Dim stepWidth As Single = outer / rings

        For i As Integer = 0 To rings - 1
            Dim radius As Single = outer - i * stepWidth

            If radius <= 0 Then
                Continue For
            End If

            Using brush As New SolidBrush(Vivid(rng))
                Call g.FillEllipse(brush, center - radius, center - radius, radius * 2.0F, radius * 2.0F)
            End Using
        Next
    End Sub

#End Region

#Region "颜色"

    ''' <summary>HSV 色彩空间中随机取一个高饱和、高亮度的颜色。</summary>
    Private Function Vivid(rng As Random) As Color
        Return HsvToColor(rng.NextDouble() * 360.0, 0.55 + rng.NextDouble() * 0.45, 0.75 + rng.NextDouble() * 0.25)
    End Function

    ''' <summary>随机取一个低饱和度的背景色，保证前景图案足够突出。</summary>
    Private Function Background(rng As Random) As Color
        Return HsvToColor(rng.NextDouble() * 360.0, 0.05 + rng.NextDouble() * 0.2, 0.35 + rng.NextDouble() * 0.6)
    End Function

    Private Function HsvToColor(hue As Double, saturation As Double, value As Double) As Color
        Dim c As Double = value * saturation
        Dim h As Double = (hue Mod 360.0) / 60.0
        Dim x As Double = c * (1.0 - std.Abs((h Mod 2.0) - 1.0))
        Dim m As Double = value - c

        Dim r As Double = 0, g As Double = 0, b As Double = 0

        Select Case CInt(std.Floor(h))
            Case 0
                r = c : g = x
            Case 1
                r = x : g = c
            Case 2
                g = c : b = x
            Case 3
                g = x : b = c
            Case 4
                r = x : b = c
            Case Else
                r = c : b = x
        End Select

        Return Color.FromArgb(
            CByte(std.Round((r + m) * 255.0)),
            CByte(std.Round((g + m) * 255.0)),
            CByte(std.Round((b + m) * 255.0)))
    End Function

#End Region

    Private Sub Shuffle(items As Bitmap(), rng As Random)
        For i As Integer = items.Length - 1 To 1 Step -1
            Dim j As Integer = rng.Next(i + 1)
            Dim temp As Bitmap = items(i)

            items(i) = items(j)
            items(j) = temp
        Next
    End Sub

    ''' <summary>
    ''' 把 <see cref="PatternKind"/> 描述成中文名称（用于报告）。
    ''' </summary>
    Public Function Describe(kind As PatternKind) As String
        Select Case kind
            Case PatternKind.Gradient
                Return "渐变"
            Case PatternKind.Circles
                Return "圆斑"
            Case PatternKind.Stripes
                Return "条纹"
            Case PatternKind.Checkerboard
                Return "棋盘"
            Case PatternKind.Rings
                Return "同心环"
            Case Else
                Return kind.ToString()
        End Select
    End Function

End Module
