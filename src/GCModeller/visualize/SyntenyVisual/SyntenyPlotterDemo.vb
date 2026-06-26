' ============================================================================
'  SyntenyPlotterDemo.vb
'  演示如何使用 SyntenyPlotter 模块绘制泛基因组共线性图
'
'  本示例构造一组模拟的泛基因组共线性数据，并分别使用不同主题输出 PNG 图像。
' ============================================================================

Imports System.Collections.Generic
Imports System.Drawing
Imports PanGenomeSynteny

Module SyntenyPlotterDemo

    Sub Main()
        ' 1. 构造模拟的泛基因组共线性数据
        '    数据结构：Dictionary(Of String, Gene())
        '    键 = 基因组 ID，值 = 该基因组的基因数组
        Dim orthologyData = BuildSampleData()

        ' 2. 输出目录
        Dim outDir = "C:\SyntenyOutput\"
        System.IO.Directory.CreateDirectory(outDir)

        ' -------------------------------------------------------
        '  示例 1：默认主题（贝塞尔曲线 + Nature 配色）
        ' -------------------------------------------------------
        Dim theme1 = SyntenyTheme.CreateDefault()
        theme1.Title = "Pan-Genome Synteny (Bezier + Nature)"
        theme1.Subtitle = "Default theme with Bezier curve connections"
        Dim plotter1 = New SyntenyPlotter(orthologyData, theme1)
        plotter1.Plot(outDir & "synteny_bezier_nature.png")
        Console.WriteLine("[OK] synteny_bezier_nature.png")

        ' -------------------------------------------------------
        '  示例 2：直线主题 + Nature 配色
        ' -------------------------------------------------------
        Dim theme2 = SyntenyTheme.CreateStraightLineTheme()
        theme2.Title = "Pan-Genome Synteny (Straight + Nature)"
        theme2.Subtitle = "Straight line connections"
        Dim plotter2 = New SyntenyPlotter(orthologyData, theme2)
        plotter2.Plot(outDir & "synteny_straight_nature.png")
        Console.WriteLine("[OK] synteny_straight_nature.png")

        ' -------------------------------------------------------
        '  示例 3：贝塞尔曲线 + Vibrant 配色
        ' -------------------------------------------------------
        Dim theme3 = SyntenyTheme.CreateBezierTheme()
        theme3.Palette = ColorPalette.Vibrant()
        theme3.Title = "Pan-Genome Synteny (Bezier + Vibrant)"
        theme3.ConnectionAlpha = 110
        Dim plotter3 = New SyntenyPlotter(orthologyData, theme3)
        plotter3.Plot(outDir & "synteny_bezier_vibrant.png")
        Console.WriteLine("[OK] synteny_bezier_vibrant.png")

        ' -------------------------------------------------------
        '  示例 4：贝塞尔曲线 + Pastel 配色（适合论文）
        ' -------------------------------------------------------
        Dim theme4 = SyntenyTheme.CreateBezierTheme()
        theme4.Palette = ColorPalette.Pastel()
        theme4.Title = "Pan-Genome Synteny (Bezier + Pastel)"
        theme4.ConnectionAlpha = 80
        theme4.ConnectionWidth = 1.5F
        Dim plotter4 = New SyntenyPlotter(orthologyData, theme4)
        plotter4.Plot(outDir & "synteny_bezier_pastel.png")
        Console.WriteLine("[OK] synteny_bezier_pastel.png")

        ' -------------------------------------------------------
        '  示例 5：直线 + Grayscale 配色（黑白印刷）
        ' -------------------------------------------------------
        Dim theme5 = SyntenyTheme.CreateStraightLineTheme()
        theme5.Palette = ColorPalette.Grayscale()
        theme5.Title = "Pan-Genome Synteny (Straight + Grayscale)"
        theme5.ConnectionAlpha = 100
        Dim plotter5 = New SyntenyPlotter(orthologyData, theme5)
        plotter5.Plot(outDir & "synteny_straight_grayscale.png")
        Console.WriteLine("[OK] synteny_straight_grayscale.png")

        ' -------------------------------------------------------
        '  示例 6：论文发表主题（高分辨率 600 DPI）
        ' -------------------------------------------------------
        Dim theme6 = SyntenyTheme.CreatePublicationTheme()
        theme6.Palette = ColorPalette.Nature()
        theme6.ConnectionStyle = ConnectionStyle.BezierCurve
        theme6.Title = "Pan-Genome Synteny Analysis"
        theme6.Subtitle = "Publication quality (600 DPI)"
        Dim plotter6 = New SyntenyPlotter(orthologyData, theme6)
        plotter6.Plot(outDir & "synteny_publication.png")
        Console.WriteLine("[OK] synteny_publication.png")

        Console.WriteLine()
        Console.WriteLine("All images saved to: " & outDir)
        Console.WriteLine("Press any key to exit...")
        Console.ReadKey()
    End Sub


    ''' <summary>
    ''' 构造模拟的泛基因组共线性数据。
    ''' 包含 3 个基因组，每个基因组有 2 条染色体，共 10 个直系同源家族。
    ''' </summary>
    Private Function BuildSampleData() As Dictionary(Of String, Gene())
        Dim data As New Dictionary(Of String, Gene())()
        Dim rnd As New Random(42)

        ' 定义 10 个直系同源家族
        Dim families = {"OG0001", "OG0002", "OG0003", "OG0004", "OG0005",
                        "OG0006", "OG0007", "OG0008", "OG0009", "OG0010"}

        ' ---- 基因组 A：chr1 + chr2 ----
        Dim genomeA As New List(Of Gene)()
        Dim posA1 = 1000
        For i = 0 To 6
            Dim len = rnd.Next(800, 1500)
            genomeA.Add(New Gene("A_gene" & i, families(i Mod families.Length),
                                  posA1, posA1 + len, "chr1", If(i Mod 3 = 0, "+"c, "-"c)))
            posA1 += len + rnd.Next(300, 800)
        Next
        Dim posA2 = 500
        For i = 7 To 9
            Dim len = rnd.Next(800, 1500)
            genomeA.Add(New Gene("A_gene" & i, families(i Mod families.Length),
                                  posA2, posA2 + len, "chr2", If(i Mod 2 = 0, "+"c, "-"c)))
            posA2 += len + rnd.Next(300, 800)
        Next
        data("Genome_A") = genomeA.ToArray()

        ' ---- 基因组 B：chr1 + chr2（与 A 存在共线性，但顺序略有重排） ----
        Dim genomeB As New List(Of Gene)()
        Dim posB1 = 1200
        ' 家族顺序与 A 略有不同，模拟重排
        Dim bOrder1 = {0, 1, 2, 4, 3, 5, 6}
        For idx = 0 To bOrder1.Length - 1
            Dim i = bOrder1(idx)
            Dim len = rnd.Next(800, 1500)
            genomeB.Add(New Gene("B_gene" & idx, families(i),
                                  posB1, posB1 + len, "chr1", If(idx Mod 2 = 0, "+"c, "-"c)))
            posB1 += len + rnd.Next(300, 800)
        Next
        Dim posB2 = 800
        For i = 7 To 9
            Dim len = rnd.Next(800, 1500)
            genomeB.Add(New Gene("B_gene" & (i + 1), families(i),
                                  posB2, posB2 + len, "chr2", If(i Mod 2 = 0, "+"c, "-"c)))
            posB2 += len + rnd.Next(300, 800)
        Next
        data("Genome_B") = genomeB.ToArray()

        ' ---- 基因组 C：chr1 + chr2（与 B 共线性，存在部分缺失） ----
        Dim genomeC As New List(Of Gene)()
        Dim posC1 = 900
        ' 缺失 OG0004，模拟基因丢失
        Dim cOrder1 = {0, 1, 2, 5, 6}
        For idx = 0 To cOrder1.Length - 1
            Dim i = cOrder1(idx)
            Dim len = rnd.Next(800, 1500)
            genomeC.Add(New Gene("C_gene" & idx, families(i),
                                  posC1, posC1 + len, "chr1", If(idx Mod 2 = 0, "+"c, "-"c)))
            posC1 += len + rnd.Next(300, 800)
        Next
        Dim posC2 = 600
        For i = 7 To 9
            Dim len = rnd.Next(800, 1500)
            genomeC.Add(New Gene("C_gene" & (i + 1), families(i),
                                  posC2, posC2 + len, "chr2", If(i Mod 2 = 0, "+"c, "-"c)))
            posC2 += len + rnd.Next(300, 800)
        Next
        data("Genome_C") = genomeC.ToArray()

        Return data
    End Function

End Module
