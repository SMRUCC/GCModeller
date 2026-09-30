#Region "Microsoft.VisualBasic::96717b41f0e05c091df96c8af82c709f, annotations\GSEA\GSVA\test\ArabidopsisThaliana.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    ' Module ArabidopsisThalianaTest
    ' 
    '     Function: (+2 Overloads) LoadKEGG
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSVA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports std = System.Math

''' <summary>
''' GSVA 算法准确度自动化测试（以拟南芥转录组数据 + KEGG 通路为载体）
''' </summary>
''' <remarks>
''' 本测试包含四层校验：
''' 
''' 1. <see cref="runRealData"/>：加载真实数据并运行全部四种方法，
'''    产出分数矩阵到数据目录，供人工检查与 R 端比对使用
''' 2. <see cref="checkInvariants"/>：不变量检查——GSVA 分数必须落在 [-1, 1]、
'''    不得出现 NaN / 无穷大、矩阵形状与样本标签必须一致
''' 3. <see cref="compareWithReference"/>：与 R 4.5.0 + Bioconductor GSVA 2.2.1
'''    计算出的金标准逐元素比对（需要 ``reference/gsva_reference.R`` 生成的参考分数）
''' 4. <see cref="plantedSignalTest"/>：植入信号的模拟数据正答验证，
'''    被激活的通路必须被正确检出
''' 
''' 全部断言通过时以退出码 0 结束，任一失败则以退出码 1 结束。
''' </remarks>
Public Module ArabidopsisThalianaTest

    ''' <summary>测试数据目录，可用命令行第一个参数覆盖</summary>
    Dim dataDir As String = "G:\GCModeller\test\demo\HTS\GSVA"

    ''' <summary>R 的可执行文件路径，用于自动生成金标准参考分数</summary>
    Const RSCRIPT As String = "C:\Program Files\R\R-4.5.0\bin\Rscript.exe"

    ''' <summary>与 R 金标准比对的容差</summary>
    Const REF_TOLERANCE As Double = 0.0000000001

    Const REF_SCRIPT As String = "gsva_reference.R"

    Dim m_failures As New List(Of String)
    Dim m_pass As Integer = 0

    Sub Main(args As String())
        If args.Length > 0 Then
            dataDir = args(0)
        End If

        Call "==========================================================".println
        Call " GSVA 算法准确度测试 (Arabidopsis thaliana + KEGG)".println
        Call "==========================================================".println
        Call $"data dir : {dataDir}".println

        Try
            Call runRealData()
        Catch ex As Exception
            Call check("真实数据流程", False, ex.GetType.Name & ": " & ex.Message)
        End Try

        Try
            Call plantedSignalTest()
        Catch ex As Exception
            Call check("植入信号模拟数据", False, ex.GetType.Name & ": " & ex.Message)
        End Try

        Call "==========================================================".println
        Call $" 通过 {m_pass} 项断言，失败 {m_failures.Count} 项".println

        If m_failures.Count > 0 Then
            For Each f As String In m_failures
                Call $"   - {f}".println
            Next

            Call "==========================================================".println
            Call Environment.Exit(1)
        Else
            Call " 全部通过!".println
            Call "==========================================================".println
        End If
    End Sub

    ''' <summary>
    ''' 加载拟南芥表达矩阵与 KEGG 通路背景，运行四种方法并做校验
    ''' </summary>
    Private Sub runRealData()
        Dim expr As Matrix = Matrix.LoadData($"{dataDir}/ath_norm.csv")
        Dim kegg As Background = loadKEGG()

        Call $"loaded {expr.size} genes x {expr.sampleID.Length} samples".println
        Call $"KEGG background: {kegg.clusters.Length} pathways".println
        Call "".println

        ' 先导出「映射到全基因集上的通路成员长表」。
        ' R 端无法读取 GCModeller 自有的 HDSPack 二进制 ath.db，
        ' 必须由本测试把基因集导出为纯文本，再交给 R 生成金标准。
        ' 两端因此使用完全相同的基因全集与通路成员。
        Dim geneSetFile As String = $"{dataDir}/gsva_genesets.tsv"

        Call exportGeneSets(expr.rownames, kegg, geneSetFile)
        Call $"exported gene sets -> {geneSetFile}".println
        Call "".println

        ' -----------------------------------------------------------
        ' 运行四种方法
        ' -----------------------------------------------------------
        Dim gsvaScores As Matrix = GSVA.gsva(expr, kegg, verbose:=True)
        Dim ssgseaScores As Matrix = GSVA.gsva(expr, kegg, method:=Methods.ssgsea)
        Dim zscoreScores As Matrix = GSVA.gsva(expr, kegg, method:=Methods.zscore)
        Dim plageScores As Matrix = GSVA.gsva(expr, kegg, method:=Methods.plage)

        Call gsvaScores.SaveMatrix($"{dataDir}/gsva_dotnet.csv", "kegg_pathways")
        Call ssgseaScores.SaveMatrix($"{dataDir}/ssgsea_dotnet.csv", "kegg_pathways")
        Call zscoreScores.SaveMatrix($"{dataDir}/zscore_dotnet.csv", "kegg_pathways")
        Call plageScores.SaveMatrix($"{dataDir}/plage_dotnet.csv", "kegg_pathways")

        Call "".println
        Call "--- 1. 矩阵形状与样本标签 ---".println
        Call checkShape("gsva", gsvaScores, expr.sampleID)
        Call checkShape("ssgsea", ssgseaScores, expr.sampleID)
        Call checkShape("zscore", zscoreScores, expr.sampleID)
        Call checkShape("plage", plageScores, expr.sampleID)

        Call "".println
        Call "--- 2. 不变量检查 ---".println
        Call checkInvariants("gsva", gsvaScores, bounded:=True)
        Call checkInvariants("ssgsea", ssgseaScores, bounded:=False)
        Call checkInvariants("zscore", zscoreScores, bounded:=False)
        Call checkInvariants("plage", plageScores, bounded:=False)

        Call "".println
        Call "--- 3. 与 R GSVA 2.2.1 金标准比对 ---".println
        Call ensureReference()

        ' PLAGE 的奇异向量符号不确定，比对前需要统一符号
        Call compareWithReference("gsva", gsvaScores, alignSign:=False)
        Call compareWithReference("ssgsea", ssgseaScores, alignSign:=False)
        Call compareWithReference("zscore", zscoreScores, alignSign:=False)
        Call compareWithReference("plage", plageScores, alignSign:=True)

        ' kcdf = "none"（不使用核函数，直接经验 CDF）路径单独校验
        Call "".println
        Call "--- 4. kcdf=none 经验 CDF 路径比对 ---".println
        Dim ecdfScores As Matrix = GSVA.gsva(expr, kegg, kcdf:=KCDFs.none)

        Call ecdfScores.SaveMatrix($"{dataDir}/gsva_ecdf_dotnet.csv", "kegg_pathways")
        Call compareOneReference("gsva-ecdf", ecdfScores, $"{dataDir}/gsva_ecdf_reference.csv", alignSign:=False)
    End Sub

    ''' <summary>
    ''' 植入信号的模拟数据正答验证
    ''' </summary>
    ''' <remarks>
    ''' 构造 300 个基因 x 20 个样本的模拟矩阵，分为两个各 10 个样本的组；
    ''' 基因集 setA 的 30 个基因在第二组中被整体上调 2.0，setB、setC 不做任何处理。
    ''' 正确的 GSVA 实现应当给出：setA 在两组之间的分数差为正，
    ''' 且明显大于作为阴性对照的 setB、setC。
    ''' </remarks>
    Private Sub plantedSignalTest()
        Const nGenes As Integer = 300
        Const nSamples As Integer = 20
        Const groupSize As Integer = 10
        Const setSize As Integer = 30
        Const effect As Double = 2.0

        Dim rng As New Random(20260930)
        Dim genes As String() = Enumerable.Range(1, nGenes).Select(Function(i) $"g{i}").ToArray
        Dim sampleId As String() = Enumerable.Range(1, nSamples).Select(Function(i) $"s{i}").ToArray
        Dim rows As New List(Of DataFrameRow)

        For i As Integer = 0 To nGenes - 1
            Dim v As Double() = New Double(nSamples - 1) {}

            For j As Integer = 0 To nSamples - 1
                ' 两组都来自同一标准正态分布
                v(j) = gauss(rng)
            Next

            ' 植入信号：setA（前 setSize 个基因）在第二组中整体上调
            If i < setSize Then
                For j As Integer = groupSize To nSamples - 1
                    v(j) += effect
                Next
            End If

            rows.Add(New DataFrameRow With {.geneID = genes(i), .experiments = v})
        Next

        Dim expr As New Matrix With {
            .sampleID = sampleId,
            .tag = "planted",
            .expression = rows.ToArray
        }

        Dim sets As String()() = {
            Enumerable.Range(0, setSize).Select(Function(i) genes(i)).ToArray,
            Enumerable.Range(setSize, setSize).Select(Function(i) genes(i)).ToArray,
            Enumerable.Range(setSize * 2, setSize).Select(Function(i) genes(i)).ToArray
        }
        Dim setNames As String() = {"setA", "setB", "setC"}

        Dim kegg As New Background With {
            .id = "simulated",
            .name = "planted signal simulation",
            .clusters = sets _
                .Select(Function(members, i)
                            Return New Cluster With {
                                .ID = setNames(i),
                                .names = setNames(i),
                                .description = setNames(i),
                                .members = members _
                                    .Select(Function(g)
                                                Return New BackgroundGene With {
                                                    .accessionID = g,
                                                    .name = g,
                                                    .[alias] = {g}
                                                }
                                            End Function) _
                                    .ToArray
                            }
                        End Function) _
                .ToArray
        }

        Dim scores As Matrix = GSVA.gsva(expr, kegg)

        ' 第二组是否比第一组高
        Dim shift As New Dictionary(Of String, Double)

        For Each row As DataFrameRow In scores.expression
            Dim g1 As Double = row.experiments.Take(groupSize).Average
            Dim g2 As Double = row.experiments.Skip(groupSize).Average
            Dim d As Double = g2 - g1

            shift(row.geneID) = d
            Call $"  {row.geneID}: group1 mean = {g1:F6}, group2 mean = {g2:F6}, shift = {d:F6}".println
        Next

        Call "".println
        Call check("植入信号: setA 组间分数差为正", shift("setA") > 0.05, $"shift = {shift("setA"):F6}")
        Call check("植入信号: setA 的分数差显著大于阴性对照 setB",
                   shift("setA") > shift("setB") + 0.2,
                   $"setA = {shift("setA"):F6}, setB = {shift("setB"):F6}")
        Call check("植入信号: setA 的分数差显著大于阴性对照 setC",
                   shift("setA") > shift("setC") + 0.2,
                   $"setA = {shift("setA"):F6}, setC = {shift("setC"):F6}")

        ' 稳健性：所有分数都必须落在 [-1, 1] 内
        Dim flat As Double() = scores.expression.SelectMany(Function(r) r.experiments).ToArray

        Call check("植入信号: 分数有界", flat.All(Function(x) x >= -1 - 0.000000001 AndAlso x <= 1 + 0.000000001),
                   $"range = [{flat.Min:F6}, {flat.Max:F6}]")
    End Sub

    ''' <summary>Box-Muller 变换产生标准正态随机数</summary>
    Private Function gauss(rng As Random) As Double
        Dim u1 As Double = 1 - rng.NextDouble()
        Dim u2 As Double = rng.NextDouble()

        Return std.Sqrt(-2 * std.Log(u1)) * std.Cos(2 * std.PI * u2)
    End Function

    ''' <summary>
    ''' 把「映射到给定基因全集上的通路成员」导出为长表 TSV，供 R 端读取
    ''' </summary>
    Private Sub exportGeneSets(features As String(), kegg As Background, path As String)
        Using writer As New StreamWriter(path, False, Encoding.UTF8)
            Call writer.WriteLine("pathway" & vbTab & "gene")

            For Each cl As Cluster In kegg.clusters
                For Each gene As String In cl.Intersect(features)
                    Call writer.WriteLine(cl.ID & vbTab & gene)
                Next
            Next
        End Using
    End Sub

    Private Sub checkShape(name As String, scores As Matrix, sampleId As String())
        Dim okSamples As Boolean = scores.sampleID.SequenceEqual(sampleId)
        Dim okRows As Boolean = scores.size > 0 AndAlso scores.expression.All(Function(r) r.experiments.Length = sampleId.Length)

        Call check($"{name}: 行列数与样本标签一致", okSamples AndAlso okRows,
                   $"{scores.size} pathways x {scores.sampleID.Length} samples")
    End Sub

    ''' <param name="bounded">GSVA 分数在数学上必须落在 [-1, 1]，其余方法没有这个边界</param>
    Private Sub checkInvariants(name As String, scores As Matrix, bounded As Boolean)
        Dim flat As Double() = scores.expression.SelectMany(Function(r) r.experiments).ToArray
        Dim nan As Integer = flat.Count(Function(x) Double.IsNaN(x) OrElse Double.IsInfinity(x))

        Call check($"{name}: 无 NaN / 无穷大", nan = 0, $"{nan} 个异常值 / {flat.Length} 个分数")

        If bounded Then
            Dim outOfRange As Integer = flat _
                .Count(Function(x) x < -1 - 0.000000001 OrElse x > 1 + 0.000000001)

            Call check($"{name}: 分数落在 [-1, 1]",
                       outOfRange = 0,
                       $"{outOfRange} 个越界值, range = [{flat.Min:F6}, {flat.Max:F6}]")
        End If
    End Sub

    ''' <summary>
    ''' 若参考分数文件缺失且本机存在 R，则调用 R 脚本生成金标准
    ''' </summary>
    Private Sub ensureReference()
        Dim methods As String() = {"gsva", "ssgsea", "zscore", "plage", "gsva_ecdf"}

        If methods.All(Function(m) File.Exists($"{dataDir}/{m}_reference.csv")) Then
            Call "参考分数文件已存在，跳过 R 计算".println
            Return
        End If

        Dim script As String = locateScript()

        If script Is Nothing Then
            Call $"[WARN] 找不到 {REF_SCRIPT}，无法生成金标准，相关比对将失败".println
            Return
        End If

        If Not File.Exists(RSCRIPT) Then
            Call $"[WARN] 未找到 R ({RSCRIPT})，无法生成金标准，相关比对将失败".println
            Return
        End If

        Call $"invoking R to generate reference scores...".println
        Call $"  script: {script}".println

        Dim info As New ProcessStartInfo With {
            .FileName = RSCRIPT,
            .Arguments = $"""{script}"" ""{dataDir}""",
            .UseShellExecute = False,
            .RedirectStandardOutput = True,
            .RedirectStandardError = True,
            .CreateNoWindow = True
        }

        Using proc As Process = Process.Start(info)
            Dim stdout As String = proc.StandardOutput.ReadToEnd()
            Dim stderr As String = proc.StandardError.ReadToEnd()

            proc.WaitForExit()

            Call stdout.println

            If proc.ExitCode <> 0 Then
                Throw New Exception($"R 参考分数生成失败(exit={proc.ExitCode}): {stderr}")
            ElseIf stderr.Length > 0 Then
                Call $"[R stderr] {stderr}".println
            End If
        End Using
    End Sub

    ''' <summary>
    ''' 从可执行文件所在目录逐级向上查找 R 参考脚本
    ''' </summary>
    Private Function locateScript() As String
        Dim dir As DirectoryInfo = New DirectoryInfo(AppContext.BaseDirectory)
        Dim i As Integer = 0

        While Not dir Is Nothing AndAlso i < 10
            Dim candidate As String = Path.Combine(dir.FullName, "reference", REF_SCRIPT)

            If File.Exists(candidate) Then
                Return candidate
            End If

            candidate = Path.Combine(dir.FullName, REF_SCRIPT)

            If File.Exists(candidate) Then
                Return candidate
            End If

            dir = dir.Parent
            i += 1
        End While

        Return Nothing
    End Function

    Private Sub compareWithReference(name As String, scores As Matrix, alignSign As Boolean)
        Call compareOneReference(name, scores, $"{dataDir}/{name}_reference.csv", alignSign)
    End Sub

    ''' <summary>
    ''' 与 R 金标准逐元素比对
    ''' </summary>
    Private Sub compareOneReference(name As String,
                                    scores As Matrix,
                                    refFile As String,
                                    alignSign As Boolean)

        If Not File.Exists(refFile) Then
            Call check($"{name} 与 R 金标准比对", False, $"缺少参考文件 {refFile}")
            Return
        End If

        Dim reference As Dictionary(Of String, Double()) = readReferenceMatrix(refFile)
        Dim dotnet As New Dictionary(Of String, Double())

        For Each row As DataFrameRow In scores.expression
            dotnet(row.geneID) = row.experiments
        Next

        Dim onlyInVB As String() = dotnet.Keys.Except(reference.Keys).ToArray
        Dim onlyInR As String() = reference.Keys.Except(dotnet.Keys).ToArray

        Call check($"{name}: 通路集合与 R 一致",
                   onlyInVB.Length = 0 AndAlso onlyInR.Length = 0,
                   $"VB 独有 {onlyInVB.Length} 条, R 独有 {onlyInR.Length} 条")

        Dim maxDiff As Double = 0
        Dim sumDiff As Double = 0
        Dim n As Integer = 0
        Dim maxPath As String = ""
        Dim sampleCount As Integer = -1

        For Each pathway As String In reference.Keys
            If Not dotnet.ContainsKey(pathway) Then
                Continue For
            End If

            Dim rv As Double() = reference(pathway)
            Dim vv As Double() = dotnet(pathway)

            If sampleCount = -1 Then
                sampleCount = rv.Length
            ElseIf rv.Length <> sampleCount Then
                Call check($"{name} 与 R 金标准比对", False, $"通路 {pathway} 样本数不一致")
                Return
            End If

            If alignSign Then
                Dim sr As Double = canonicalSign(rv)
                Dim sv As Double = canonicalSign(vv)

                rv = rv.Select(Function(x) x * sr).ToArray
                vv = vv.Select(Function(x) x * sv).ToArray
            End If

            For j As Integer = 0 To rv.Length - 1
                Dim d As Double = std.Abs(rv(j) - vv(j))

                sumDiff += d
                n += 1

                If d > maxDiff Then
                    maxDiff = d
                    maxPath = pathway
                End If
            Next
        Next

        If n = 0 Then
            Call check($"{name} 与 R 金标准比对", False, "没有可比对的通路")
            Return
        End If

        Dim meanDiff As Double = sumDiff / n

        Call check($"{name} 与 R 金标准比对",
                   maxDiff <= REF_TOLERANCE,
                   $"compared {n} values, max|diff| = {maxDiff:E3} ({maxPath}), mean|diff| = {meanDiff:E3}")
    End Sub

    ''' <summary>
    ''' 让向量的符号确定化：令绝对值最大的分量为正，返回应乘的符号因子
    ''' </summary>
    Private Function canonicalSign(v As Double()) As Double
        Dim imax As Integer = 0

        For i As Integer = 1 To v.Length - 1
            If std.Abs(v(i)) > std.Abs(v(imax)) Then
                imax = i
            End If
        Next

        Return If(v(imax) >= 0, 1.0, -1.0)
    End Function

    ''' <summary>
    ''' 读取 R 的 ``write.csv`` 产出的 通路 x 样本 分数矩阵
    ''' </summary>
    Private Function readReferenceMatrix(path As String) As Dictionary(Of String, Double())
        Dim result As New Dictionary(Of String, Double())

        Using parser As New TextFieldParser(path) With {
            .TextFieldType = FieldType.Delimited,
            .Delimiters = {","},
            .HasFieldsEnclosedInQuotes = True
        }
            Call parser.ReadFields() ' 表头行

            While Not parser.EndOfData
                Dim fields As String() = parser.ReadFields()

                If fields Is Nothing OrElse fields.Length < 2 Then
                    Continue While
                End If

                Dim vals As Double() = New Double(fields.Length - 2) {}

                For i As Integer = 0 To vals.Length - 1
                    vals(i) = Double.Parse(fields(i + 1), Globalization.CultureInfo.InvariantCulture)
                Next

                result(fields(0)) = vals
            End While
        End Using

        Return result
    End Function

    Private Sub check(name As String, ok As Boolean, detail As String)
        If ok Then
            m_pass += 1
            Call $"  [PASS] {name}  ({detail})".println
        Else
            m_failures.Add($"{name}: {detail}")
            Call $"  [FAIL] {name}  ({detail})".println
        End If
    End Sub

    ''' <summary>测试内部的终端输出助手</summary>
    <Extension>
    Private Sub println(msg As String)
        Call Console.WriteLine(msg)
    End Sub

    Private Function loadKEGG() As Background
        Return New Background With {
            .clusters = loadKEGGClusters.ToArray,
            .name = "Arabidopsis Thaliana",
            .id = "ath"
        }
    End Function

    Private Iterator Function loadKEGGClusters() As IEnumerable(Of Cluster)
        Using file = $"{dataDir}/ath.db".Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Using pack As New StreamPack(file, [readonly]:=True)
                Dim pathways As StreamGroup = pack.GetObject("/pathways/")

                For Each cl In loadKEGGClusters(pack, pathways)
                    Yield cl
                Next
            End Using
        End Using
    End Function

    Private Iterator Function loadKEGGClusters(pack As StreamPack, dir As StreamGroup) As IEnumerable(Of Cluster)
        For Each file As StreamObject In dir.files
            If TypeOf file Is StreamGroup Then
                For Each cl In loadKEGGClusters(pack, DirectCast(file, StreamGroup))
                    Yield cl
                Next
            Else
                Dim xml As String = pack.ReadText(file.referencePath.ToString)
                Dim pathway As Pathway = xml.LoadFromXml(Of Pathway)()

                Yield New Cluster With {
                    .ID = pathway.name,
                    .description = pathway.description,
                    .names = pathway.name,
                    .members = pathway.genes _
                        .SafeQuery _
                        .Select(Function(g)
                                    Return New BackgroundGene With {
                                        .accessionID = g.geneId,
                                        .[alias] = {},
                                        .locus_tag = New NamedValue With {.name = g.geneId, .text = g.geneName},
                                        .name = g.geneName,
                                        .term_id = BackgroundGene.UnknownTerms(g.KO).ToArray
                                    }
                                End Function) _
                        .ToArray
                }
            End If
        Next
    End Function
End Module
