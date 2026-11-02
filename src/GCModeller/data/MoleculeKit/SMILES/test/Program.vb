Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Molecule.SMILES
Imports SMRUCC.genomics.Molecule.SMILES.Coordinates
Imports SMRUCC.genomics.Molecule.SMILES.Graph
Imports SMILESView

''' <summary>
''' SMILES / InChI 解析、坐标重建与 2D/3D 结构绘图的命令行示例与验证程序。
''' </summary>
''' <remarks>
''' 运行后会在 <c>test/output/</c> 下生成：
''' <list type="bullet">
''' <item>每个用例的 <c>*.2d.png</c> / <c>*.2d.svg</c> 平面结构图；</item>
''' <item>每个用例的 <c>*.3d.ballstick.png</c> / <c>*.3d.spacefill.png</c> 立体结构图；</item>
''' <item><c>audit_report.txt</c> 与 <c>audit_report.json</c> 测试报告。</item>
''' </list>
''' 进程退出码由 <see cref="DebugAudit.FailureCount"/> 决定（0 表示全部通过）。
''' </remarks>
Module Program

    Const IMAGE_WIDTH As Integer = 800
    Const IMAGE_HEIGHT As Integer = 600

    Sub New()
        Call ImageDriver.Register()
    End Sub

    Function Main(args As String()) As Integer
        Dim folder As String = Report.ResolveOutputDirectory()

        Call DebugAudit.Reset()
        Call DebugAudit.Mark("output directory", folder)

        Dim cases As List(Of MoleculeCase) = TestCases.All()

        Call Report.Section($"parsing / coordinate / rendering checks ({cases.Count} cases)")

        For Each item As MoleculeCase In cases
            Call runCase(item, folder)
        Next

        Call Report.Section("audit report")

        Dim outputs = Report.Save(folder)

        Console.WriteLine(DebugAudit.Dump(includeRecords:=False))
        Console.WriteLine($"full log    : {outputs.text}")
        Console.WriteLine($"json report : {outputs.json}")
        Console.WriteLine($"images      : {folder}")
        Console.WriteLine($"failures    : {DebugAudit.FailureCount}")

        Return If(DebugAudit.FailureCount = 0, 0, 1)
    End Function

    Private Sub runCase(item As MoleculeCase, folder As String)
        Console.WriteLine()
        Console.WriteLine($"> {item.Name}  ({item.Text})")

        Dim graph As MoleculeGraph = Nothing
        Dim [error] As String = Nothing

        If Not MoleculeIO.TryParse(item.Text, graph, item.Format, [error]) Then
            Call DebugAudit.Test(False, $"{item.Name}: parse failed -> {[error]}")
            Return
        End If

        Call DebugAudit.Test(graph IsNot Nothing, $"{item.Name}: molecule graph created")
        Call DebugAudit.AreEqual(item.Atoms, graph.AtomCount, $"{item.Name}: heavy atom count")
        Call DebugAudit.AreEqual(item.Bonds, graph.BondCount, $"{item.Name}: bond count")
        Call DebugAudit.AreEqual(item.Formula, graph.MolecularFormula(), $"{item.Name}: molecular formula")
        Call DebugAudit.AreEqual(item.Rings, graph.Rings().Count, $"{item.Name}: ring count")

        Dim violations As List(Of Integer) = graph.ValenceViolations()

        Call DebugAudit.Test(violations.Count = 0,
            $"{item.Name}: no valence violation (found {violations.Count})")

        For Each message As String In graph.Warnings
            Console.WriteLine($"    warning: {message}")
        Next

        Console.WriteLine($"    {MoleculeIO.Describe(graph)}")

        ' ---- 2D 坐标 ----
        Dim planar As MoleculeGraph = graph.Clone()

        Call planar.Layout2D()

        Dim quality2D As CoordinateQuality = CoordinateQuality.Measure2D(planar)

        Console.WriteLine($"    {quality2D}")

        Call DebugAudit.Test(quality2D.IsValid, $"{item.Name}: 2D coordinates are sane")
        Call DebugAudit.Test(quality2D.OverlapCount = 0, $"{item.Name}: no 2D atom overlap")

        If graph.BondCount > 0 Then
            Call DebugAudit.Test(Math.Abs(quality2D.AverageBondLength - 1.0) < 0.2,
                $"{item.Name}: 2D bond length normalised ({quality2D.AverageBondLength:F3})")
        End If

        ' ---- 3D 坐标（含显式氢） ----
        Dim stereo As MoleculeGraph = graph.Clone()

        Call stereo.Embed3D()

        Dim quality3D As CoordinateQuality = CoordinateQuality.Measure3D(stereo)

        Console.WriteLine($"    {quality3D}  ({stereo.AtomCount} atoms with H)")

        Call DebugAudit.Test(quality3D.IsValid, $"{item.Name}: 3D coordinates are sane")

        If graph.BondCount > 0 Then
            Call DebugAudit.Test(quality3D.AverageBondLength > 0.9 AndAlso quality3D.AverageBondLength < 2.0,
                $"{item.Name}: 3D bond length in physical range ({quality3D.AverageBondLength:F3})")
        End If

        Call DebugAudit.Test(quality3D.MinNonBondedDistance > 0.5,
            $"{item.Name}: no 3D atom clash ({quality3D.MinNonBondedDistance:F3}, {quality3D.MinNonBondedPair})")

        ' ---- SMILES round trip ----
        If graph.Format = MoleculeFormat.Smiles Then
            Dim written As String = MoleculeIO.WriteSmiles(graph)
            Dim reparsed As MoleculeGraph = Nothing
            Dim reason As String = Nothing

            If MoleculeIO.TryParse(written, reparsed, MoleculeFormat.Smiles, reason) Then
                Call DebugAudit.AreEqual(graph.MolKey(), reparsed.MolKey(), $"{item.Name}: SMILES round trip")
                Console.WriteLine($"    round trip: {written}")
            Else
                Call DebugAudit.Test(False, $"{item.Name}: written SMILES cannot be reparsed -> {reason}")
            End If
        End If

        If Not item.Render Then Return

        Try
            Call renderImages(item, planar, stereo, folder)
        Catch ex As Exception
            Call DebugAudit.Test(False, $"{item.Name}: image rendering failed -> {ex.GetType().Name}: {ex.Message}")
        End Try
    End Sub

    Private Sub renderImages(item As MoleculeCase, planar As MoleculeGraph, stereo As MoleculeGraph, folder As String)
        Dim size As New Size(IMAGE_WIDTH, IMAGE_HEIGHT)
        Dim viewer2D As New StructureViewer(planar)
        Dim viewer3D As New StructureViewer(stereo)
        Dim png2D As String = Path.Combine(folder, $"{item.Name}.2d.png")
        Dim svg2D As String = Path.Combine(folder, $"{item.Name}.2d.svg")
        Dim ballStick As String = Path.Combine(folder, $"{item.Name}.3d.ballstick.png")
        Dim spaceFill As String = Path.Combine(folder, $"{item.Name}.3d.spacefill.png")
        Dim svg3D As String = Path.Combine(folder, $"{item.Name}.3d.svg")

        Call DebugAudit.Test(viewer2D.SavePng2D(png2D, size), $"{item.Name}: 2D PNG written")
        Call DebugAudit.Test(viewer2D.SaveSvg2D(svg2D, size), $"{item.Name}: 2D SVG written")
        Call DebugAudit.Test(viewer3D.SavePng3D(ballStick, size, RenderMode.BallAndStick),
            $"{item.Name}: 3D ball-and-stick PNG written")
        Call DebugAudit.Test(viewer3D.SavePng3D(spaceFill, size, RenderMode.SpaceFilling),
            $"{item.Name}: 3D space-filling PNG written")
        Call DebugAudit.Test(viewer3D.SaveSvg3D(svg3D, size, RenderMode.BallAndStick),
            $"{item.Name}: 3D SVG written")

        Console.WriteLine($"    images: {Path.GetFileName(png2D)}, {Path.GetFileName(ballStick)}, {Path.GetFileName(spaceFill)}")
    End Sub

End Module
