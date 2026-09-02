' ============================================================================
' Program.vb — MiniDock 命令行入口
' ----------------------------------------------------------------------------
' 用法：
'   MiniDock dock --receptor protein.pdb --ligand lig.sdf [--out r.json] [选项]
'   MiniDock dock --receptor a.pdb --ligand b.pdb          （蛋白-蛋白，刚体）
'   MiniDock mmgbsa --complex c.pdb --ligand-chain L [--nwat 10] [--out g.json]
'   MiniDock selftest
'
' 选项（dock）：
'   --box-center x,y,z   口袋中心（默认受体形心）    --box-half N  盒半宽（默认 12Å）
'   --exhaustiveness N   独立搜索次数（默认 8）      --steps-per-run N  每次扰动轮数（默认 30）
'   --num-modes N        输出姿态数（默认 9）        --min-rmsd X   姿态去重阈值（默认 1.5Å）
'   --seed N             随机种子（0=随机）          --mmgbsa       对最优姿态做 MM-GBSA 重打分
'   --nwat N             Nwat-MMGBSA 保留水数（默认 0）
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports MiniDock.Core
Imports MiniDock.Model
Imports SMRUCC.genomics.Data.RCSB.PDB.Structures


Public Module Program

    Private Const VersionString As String = "1.0.0"

    Public Function Main(args As String()) As Integer
        If args.Length = 0 OrElse args(0) = "--help" OrElse args(0) = "-h" Then
            PrintUsage()
            Return 0
        End If
        Dim cmd = args(0).ToLowerInvariant()

        Try
            If cmd = "dock" Then
                Return RunDock(args)
            ElseIf cmd = "mmgbsa" Then
                Return RunMmGbsa(args)
            Else
                Console.Error.WriteLine($"未知子命令: {cmd}")
                Return 2
            End If
        Catch ex As Exception
            Console.Error.WriteLine($"错误: {ex.Message}")
            Return 1
        End Try
    End Function

    Private Function FlagValue(args As String(), name As String) As String
        For i = 0 To args.Length - 2
            If args(i).ToLowerInvariant() = name Then Return args(i + 1)
        Next
        Return Nothing
    End Function

    Private Function HasFlag(args As String(), name As String) As Boolean
        For i = 0 To args.Length - 1
            If args(i).ToLowerInvariant() = name Then Return True
        Next
        Return False
    End Function

    Private Function IntArg(args As String(), name As String, defVal As Int32) As Int32
        Dim v = FlagValue(args, name)
        If v Is Nothing Then Return defVal
        Return Integer.Parse(v, CultureInfo.InvariantCulture)
    End Function

    Private Function DblArg(args As String(), name As String, defVal As Double) As Double
        Dim v = FlagValue(args, name)
        If v Is Nothing Then Return defVal
        Return Double.Parse(v, CultureInfo.InvariantCulture)
    End Function

    ' ---------------- dock ----------------

    Private Function RunDock(args As String()) As Integer
        Dim recPath = FlagValue(args, "--receptor")
        Dim ligPath = FlagValue(args, "--ligand")
        Dim outPath = FlagValue(args, "--out")
        If recPath Is Nothing OrElse ligPath Is Nothing Then
            Console.Error.WriteLine("必须提供 --receptor 与 --ligand")
            Return 2
        End If

        Dim opts As New DockOptions With {
            .Exhaustiveness = IntArg(args, "--exhaustiveness", 8),
            .StepsPerRun = IntArg(args, "--steps-per-run", 30),
            .NumModes = IntArg(args, "--num-modes", 9),
            .MinRmsd = DblArg(args, "--min-rmsd", 1.5),
            .BoxHalfSize = DblArg(args, "--box-half", 12.0),
            .Seed = IntArg(args, "--seed", 0),
            .Mmgbsa = HasFlag(args, "--mmgbsa"),
            .Nwat = IntArg(args, "--nwat", 0),
            .MmgbsaTop = IntArg(args, "--mmgbsa-top", 3)}

        Dim bcStr = FlagValue(args, "--box-center")
        If bcStr IsNot Nothing Then
            Dim parts = bcStr.Split(","c)
            If parts.Length = 3 Then
                opts.BoxCenter = {
                    Double.Parse(parts(0), CultureInfo.InvariantCulture),
                    Double.Parse(parts(1), CultureInfo.InvariantCulture),
                    Double.Parse(parts(2), CultureInfo.InvariantCulture)}
            End If
        End If

        Dim receptor = StructureIO.ReadPdb(Of VinaAtom, VinaMolecule)(recPath)
        Dim ligand As VinaMolecule
        Dim mode As String
        Dim ligExt = IO.Path.GetExtension(ligPath).ToLowerInvariant()
        If ligExt = ".sdf" OrElse ligExt = ".mol" Then
            ligand = SdfIO.ReadSdf(ligPath)
            mode = "ligand"
            MolBuilder.AssignTypesSdf(ligand)
            Charges.AssignPoeCharges(ligand, 0.0)
        Else
            ligand = StructureIO.ReadPdb(Of VinaAtom, VinaMolecule)(ligPath)
            mode = "protein-protein"
            MolBuilder.AssignTypesPdb(ligand)
            Charges.AssignProteinCharges(ligand)
        End If
        MolBuilder.AssignTypesPdb(receptor)
        Charges.AssignProteinCharges(receptor)

        Console.Error.WriteLine($"MiniDock {VersionString} ({mode})")
        Console.Error.WriteLine($"受体 {receptor.Atoms.Count} 原子；配体 {ligand.Atoms.Count} 原子；exhaustiveness={opts.Exhaustiveness}")

        Dim sw = System.Diagnostics.Stopwatch.StartNew()
        Dim lr = DockEngine.Dock(receptor, ligand, opts)
        sw.Stop()
        Console.Error.WriteLine($"搜索完成 {sw.Elapsed.TotalSeconds:F1}s，{lr.Poses.Count} 个姿态")

        ' 可选 MM-GBSA 重打分（最优 N 个姿态）
        If opts.Mmgbsa Then
            For k = 0 To Math.Min(opts.MmgbsaTop, lr.Poses.Count) - 1
                Dim pose = lr.Poses(k)
                Dim poseAtoms As New List(Of VinaAtom)()
                For Each pa In pose.Atoms
                    poseAtoms.Add(New VinaAtom With {.X = pa.X, .Y = pa.Y, .Z = pa.Z,
                                                     .Element = pa.Element, .ChainID = "L",
                                                     .ResName = If(mode = "ligand", "LIG", pa.ResName),
                                                     .ResSeq = 1, .AtomName = pa.AtomName,
                                                     .FromReceptor = False})
                Next
                ' 配体电荷：ligand.Mol 的 PEOE 电荷按原子序映射到姿态原子
                ' （SDF 与 PDB 配体均在对接前完成电荷分配）
                Dim chargeByIndex As New List(Of Double)()
                For Each a In ligand.Atoms
                    chargeByIndex.Add(a.Charge)
                Next
                For i = 0 To poseAtoms.Count - 1
                    If i < chargeByIndex.Count Then
                        poseAtoms(i).Charge = chargeByIndex(i)
                    Else
                        poseAtoms(i).Charge = 0
                    End If
                Next
                Dim r = DockEngine.MmGbsaRescore(receptor.Atoms, poseAtoms, opts.Nwat)
                pose.Mmgbsa = New MmGbsaResultDto With {
                    .DeltaG = Math.Round(r.DeltaG, 3),
                    .Vdw = Math.Round(r.Vdw, 3),
                    .Elec = Math.Round(r.Elec, 3),
                    .GbPolar = Math.Round(r.GbPolar, 3),
                    .SasNonpolar = Math.Round(r.SasNonpolar, 3),
                    .Nwat = r.NwatSelected}
            Next
        End If

        Dim report As New DockReport With {
            .Program = "MiniDock",
            .Version = VersionString,
            .mode = mode,
            .Parameters = New DockParameters With {
                .Exhaustiveness = opts.Exhaustiveness,
                .StepsPerRun = opts.StepsPerRun,
                .NumModes = opts.NumModes,
                .MinRmsd = opts.MinRmsd,
                .BoxCenter = opts.BoxCenter,
                .BoxHalfSize = opts.BoxHalfSize,
                .TemperatureMetropolis = Math.Round(293.15 * 0.001987, 4),
                .Weights = {-0.0356, -0.00516, 0.84, -0.0351, -0.587},
                .Mmgbsa = opts.Mmgbsa,
                .Nwat = opts.Nwat,
                .ReceptorAtoms = receptor.Atoms.Count,
                .Seed = opts.Seed},
            .Results = New List(Of LigandResult) From {lr}}

        Dim jsonOpts As New JsonSerializerOptions With {
            .WriteIndented = HasFlag(args, "--pretty"),
            .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull}
        Dim json = JsonSerializer.Serialize(report, jsonOpts)
        If outPath IsNot Nothing Then
            File.WriteAllText(outPath, json)
            Console.Error.WriteLine($"结果已写入 {outPath}")
        Else
            Console.Out.WriteLine(json)
        End If
        Return 0
    End Function

    ' ---------------- mmgbsa ----------------

    Private Function RunMmGbsa(args As String()) As Integer
        Dim complexPath = FlagValue(args, "--complex")
        Dim outPath = FlagValue(args, "--out")
        If complexPath Is Nothing Then
            Console.Error.WriteLine("必须提供 --complex")
            Return 2
        End If
        Dim ligChain = FlagValue(args, "--ligand-chain")
        Dim ligResname = FlagValue(args, "--ligand-resname")
        Dim nwat = IntArg(args, "--nwat", 0)

        Dim frames = StructureIO.ReadPdbFrames(Of VinaAtom, VinaMolecule)(complexPath)
        Dim frameResults As New List(Of MmGbsaFrame)()
        Dim modelNo = 0

        For Each mol In frames
            modelNo += 1
            Dim rec As New List(Of VinaAtom)()
            Dim lig As New List(Of VinaAtom)()
            For Each a In mol.Atoms
                If a.IsWater Then Continue For
                Dim isLig As Boolean = False
                If ligChain IsNot Nothing AndAlso a.ChainId = ligChain Then isLig = True
                If ligResname IsNot Nothing AndAlso a.ResName = ligResname.ToUpperInvariant() Then isLig = True
                If isLig Then lig.Add(a) Else rec.Add(a)
            Next
            If lig.Count = 0 Then
                Console.Error.WriteLine($"帧 {modelNo}：未找到配体原子")
                Continue For
            End If

            Dim r = DockEngine.MmGbsaRescore(rec, lig, nwat)
            frameResults.Add(New MmGbsaFrame With {
                .Model = modelNo,
                .DeltaG = Math.Round(r.DeltaG, 3),
                .Vdw = Math.Round(r.Vdw, 3),
                .Elec = Math.Round(r.Elec, 3),
                .GbPolar = Math.Round(r.GbPolar, 3),
                .SasNonpolar = Math.Round(r.SasNonpolar, 3),
                .NwatSelected = r.NwatSelected,
                .ReceptorAtoms = rec.Count,
                .LigandAtoms = lig.Count})
        Next

        Dim report As New MmGbsaReport With {
            .Program = "MiniDock",
            .Version = VersionString,
            .Mode = "mmgbsa",
            .nwat = nwat,
            .frames = frameResults}

        Dim jsonOpts As New JsonSerializerOptions With {
            .WriteIndented = HasFlag(args, "--pretty"),
            .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull}
        Dim json = JsonSerializer.Serialize(report, jsonOpts)
        If outPath IsNot Nothing Then
            File.WriteAllText(outPath, json)
            Console.Error.WriteLine($"结果已写入 {outPath}")
        Else
            Console.Out.WriteLine(json)
        End If
        Return 0
    End Function

    Private Sub PrintUsage()
        Console.WriteLine("MiniDock — 从头实现的 Vina 分子对接 + MM-GBSA/Nwat-MMGBSA 重打分（纯 BCL）")
        Console.WriteLine()
        Console.WriteLine("用法:")
        Console.WriteLine("  MiniDock dock --receptor protein.pdb --ligand lig.sdf [--out r.json] [--pretty] [选项]")
        Console.WriteLine("  MiniDock dock --receptor a.pdb --ligand b.pdb    （蛋白-蛋白，刚体 6 DOF）")
        Console.WriteLine("  MiniDock mmgbsa --complex c.pdb --ligand-chain L [--nwat 10]")
        Console.WriteLine("  MiniDock selftest")
        Console.WriteLine()
        Console.WriteLine("dock 选项: --box-center x,y,z --box-half N --exhaustiveness N --steps-per-run N")
        Console.WriteLine("           --num-modes N --min-rmsd X --seed N --mmgbsa --nwat N")
    End Sub

End Module


