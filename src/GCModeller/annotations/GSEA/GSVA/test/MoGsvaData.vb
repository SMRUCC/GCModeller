Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO.Numpy
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSVA.Omics
Imports std = System.Math

''' <summary>
''' moGSVA 测试数据装配器。
''' </summary>
''' <remarks>
''' <para>
''' 真实数据来自 <c>TCGA_COAD</c> 目录：
''' </para>
''' <list type="bullet">
''' <item><description><c>r160_subtype.npy</c>：numpy 对象数组（pickle 协议 3），内含一个 float64 表达矩阵与若干标签对象</description></item>
''' <item><description><c>kegg_v3.gmt</c>：337 条 KEGG 通路（基因符号）</description></item>
''' <item><description><c>sampleid_list.txt</c>：样本 → CMS 亚型</description></item>
''' </list>
''' <para>
''' 该目录只有<b>一层</b>真实组学（转录组），因此按既定方案：
''' 以真实矩阵为 mRNA 层，再用固定随机种子派生
''' 蛋白层（基因子集 + 剂量噪声，与 mRNA 正相关）与
''' 甲基化层（beta 值，与 mRNA 反相关，声明 direction = -1 统一到活性方向）。
''' 全部派生过程只依赖 <see cref="Random"/> 的固定种子，因此完全可复现。
''' </para>
''' <para>
''' 另提供一组纯合成的可控信号数据（<see cref="BuildSynthetic"/>），
''' 用于对融合逻辑（等权/方差/一致性奖励/贡献分解）做正答断言。
''' </para>
''' </remarks>
Public Module MoGsvaData

    ''' <summary>默认数据目录</summary>
    Public Const DefaultDataDir As String = "G:\GCModeller\src\GCModeller\annotations\GSEA\data\TCGA_COAD"

    ''' <summary>默认随机种子</summary>
    Public Const DefaultSeed As Integer = 20261026

    Public Class RealDataset
        Public Property Dataset As MultiOmicsDataset
        Public Property GeneSets As Background
        Public Property SampleID As String()
        Public Property Subtype As String()
        ''' <summary>去重/过滤之后保留的特征数（mRNA 层）</summary>
        Public Property GeneCount As Integer
        ''' <summary>原始矩阵是否因为行名不是基因符号而做了确定性重标注</summary>
        Public Property Relabeled As Boolean
        ''' <summary>原始矩阵的行数</summary>
        Public Property RawGenes As Integer
    End Class

    Public Class SyntheticDataset
        Public Property Dataset As MultiOmicsDataset
        Public Property GeneSets As Background
        Public Property SampleID As String()
        Public Property GroupSize As Integer
        ''' <summary>通路名 → 该通路在 mRNA 层是否携带植入信号</summary>
        Public Property UpRegulated As String()
        ''' <summary>通路名 → 组学层名 → 该层在第二组中被植入的变化量（beta 层为负表示去甲基化）</summary>
        Public Property Effects As Dictionary(Of String, Dictionary(Of String, Double))
    End Class

    ' ------------------------------------------------------------------
    ' npy 读取
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' 读取 npy 文件，返回其内部的全部数组元素（对象数组会被展开）。
    ''' </summary>
    Public Function ReadNpyArrays(path As String) As NpyArray()
        Dim array As NpyArray = NpyFile.Read(path)
        Dim items As New List(Of NpyArray)

        If array.IsObjectArray Then
            For Each item As Object In array.AsObjectArray()
                Dim nested As NpyArray = TryCast(item, NpyArray)

                If nested IsNot Nothing Then
                    items.Add(nested)
                End If
            Next
        Else
            items.Add(array)
        End If

        Return items.ToArray
    End Function

    ''' <summary>
    ''' 解析 TCGA COAD 的 <c>r160_subtype.npy</c>，取出表达矩阵与样本标签。
    ''' </summary>
    ''' <remarks>
    ''' 对象数组的结构在不同版本的导出脚本下可能略有差异，
    ''' 因此这里按「类型 + 形状」识别：取面积最大的二维数值数组作为表达矩阵，
    ''' 取长度分别等于行数/列数的字符串数组作为特征名/样本名，缺失时用回退值。
    ''' </remarks>
    ''' <summary>
    ''' 读取 npy 文件的顶层元素（对象数组会被展开为元素列表）。
    ''' </summary>
    Public Function ReadNpyObjects(path As String) As Object()
        Dim array As NpyArray = NpyFile.Read(path)

        If array.IsObjectArray Then
            Return array.AsObjectArray()
        Else
            Return New Object() {array}
        End If
    End Function

    Public Function LoadRnaMatrix(npyFile As String, Optional sampleHintFile As String = Nothing, Optional verbose As Boolean = True) As Matrix
        Dim objects As Object() = ReadNpyObjects(npyFile)
        Dim numeric As New List(Of NpyArray)
        Dim texts As New List(Of String())

        For Each item As Object In objects
            Dim nested As NpyArray = TryCast(item, NpyArray)

            If nested IsNot Nothing AndAlso nested.Rank = 2 AndAlso nested.DType.IsNumeric AndAlso nested.Shape(0) > 1 AndAlso nested.Shape(1) > 1 Then
                numeric.Add(nested)
            End If

            Call CollectStrings(item, texts)
        Next

        If numeric.Count = 0 Then
            Throw New InvalidDataException($"no 2-dimensional numeric array was found in the npy file: {npyFile}")
        End If

        Dim payload As NpyArray = numeric _
            .OrderByDescending(Function(a) CLng(a.Shape(0)) * a.Shape(1)) _
            .First()
        Dim nRow As Integer = payload.Shape(0)
        Dim nCol As Integer = payload.Shape(1)
        Dim values As Double() = payload.AsDouble()

        Dim features As String() = FindLabels(texts, nRow)
        Dim samples As String() = FindLabels(texts, nCol)

        If features Is Nothing Then
            features = Enumerable.Range(1, nRow).Select(Function(i) $"F{i}").ToArray
        End If
        If samples Is Nothing Then
            If sampleHintFile IsNot Nothing AndAlso File.Exists(sampleHintFile) Then
                Dim hints As String() = LoadSampleIDs(sampleHintFile)

                If hints.Length >= nCol Then
                    samples = hints.Take(nCol).ToArray
                End If
            End If
        End If
        If samples Is Nothing Then
            samples = Enumerable.Range(1, nCol).Select(Function(i) $"S{i}").ToArray
        End If

        If verbose Then
            Call $"[moGSVA] expression matrix: {nRow} features x {nCol} samples, dtype={payload.DType}".println
            Call $"[moGSVA] feature id sample: {String.Join(", ", features.Take(5))}".println
            Call $"[moGSVA] sample id sample : {String.Join(", ", samples.Take(5))}".println
            Call $"[moGSVA] npy objects      : {String.Join("; ", objects.Select(Function(o) DescribeObject(o)))}".println
            Call $"[moGSVA] npy string arrays: {String.Join("; ", texts.Select(Function(t) $"[{t.Length}] {String.Join("/", t.Take(3))}"))}".println
        End If

        Dim rows As New List(Of DataFrameRow)

        For i As Integer = 0 To nRow - 1
            Dim v As Double() = New Double(nCol - 1) {}
            Dim offset As Integer = i * nCol

            For j As Integer = 0 To nCol - 1
                v(j) = values(offset + j)
            Next

            rows.Add(New DataFrameRow With {
                .geneID = features(i),
                .experiments = v
            })
        Next

        Return New Matrix With {
            .sampleID = samples,
            .tag = "TCGA-COAD:rna",
            .expression = rows.ToArray
        }
    End Function

    ''' <summary>
    ''' 读取 <c>sampleid_list.txt</c>（Sampid,Class）中的样本 id。
    ''' </summary>
    Public Function LoadSampleIDs(sampleFile As String) As String()
        Dim ids As New List(Of String)

        For Each line As String In File.ReadLines(sampleFile).Skip(1)
            If String.IsNullOrWhiteSpace(line) Then
                Continue For
            End If

            Dim fields As String() = line.Split(","c)

            If fields.Length > 0 AndAlso fields(0).Trim.Length > 0 Then
                ids.Add(fields(0).Trim.Trim(""""c))
            End If
        Next

        Return ids.ToArray
    End Function

    ''' <summary>
    ''' 读取 <c>sampleid_list.txt</c> 中的样本 → CMS 亚型映射。
    ''' </summary>
    Public Function LoadSubtype(sampleFile As String) As Dictionary(Of String, String)
        Dim result As New Dictionary(Of String, String)(StringComparer.Ordinal)

        For Each line As String In File.ReadLines(sampleFile).Skip(1)
            Dim fields As String() = line.Split(","c)

            If fields.Length > 1 Then
                result(fields(0).Trim.Trim(""""c)) = fields(1).Trim.Trim(""""c)
            End If
        Next

        Return result
    End Function

    Private Function DescribeObject(item As Object) As String
        If item Is Nothing Then
            Return "null"
        End If

        Dim array As NpyArray = TryCast(item, NpyArray)

        If array Is Nothing Then
            Return item.GetType.Name
        End If

        Return $"({String.Join(",", array.Shape)}) {array.DType}"
    End Function

    ''' <summary>
    ''' 从任意 npy/pickle 元素中抽取「字符串集合」（ndarray 字符串、String()、List(Of String) 等）。
    ''' </summary>
    Private Sub CollectStrings(item As Object, texts As List(Of String()))
        If item Is Nothing Then
            Return
        End If

        Dim array As NpyArray = TryCast(item, NpyArray)

        If array IsNot Nothing Then
            If array.DType.IsStringType AndAlso array.Rank = 1 Then
                Try
                    texts.Add(array.AsString())
                Catch ex As Exception
                    ' 字符串数组解析失败只意味着标签信息缺失，不影响主流程
                End Try
            End If

            Return
        End If

        Dim plain As String() = TryCast(item, String())

        If plain IsNot Nothing Then
            texts.Add(plain)

            Return
        End If

        Dim objects As Object() = TryCast(item, Object())

        If objects IsNot Nothing Then
            If objects.All(Function(o) TypeOf o Is String) Then
                texts.Add(objects.Select(Function(o) CStr(o)).ToArray)
            Else
                For Each nested As Object In objects
                    Call CollectStrings(nested, texts)
                Next
            End If

            Return
        End If

        If TypeOf item Is IEnumerable AndAlso Not TypeOf item Is String Then
            Dim values As New List(Of String)

            For Each element As Object In DirectCast(item, IEnumerable)
                Dim text As String = TryCast(element, String)

                If text Is Nothing Then
                    Return
                End If

                values.Add(text)
            Next

            If values.Count > 0 Then
                texts.Add(values.ToArray)
            End If
        End If
    End Sub

    Private Function FindLabels(texts As List(Of String()), length As Integer) As String()
        ' 取第一个长度正好等于目标长度的字符串数组；若多个，取内容最长（信息量最大）的那个
        Dim candidates As String()() = texts.Where(Function(t) t.Length = length).ToArray

        If candidates.Length = 0 Then
            Return Nothing
        End If

        Return candidates _
            .OrderByDescending(Function(t) t.Sum(Function(s) If(s, "").Length)) _
            .First()
    End Function

    ' ------------------------------------------------------------------
    ' 真实数据：mRNA + 确定性派生的 protein / methylation
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' 装配 TCGA COAD 的三层多组学数据集。
    ''' </summary>
    ''' <param name="dataDir">TCGA_COAD 数据目录</param>
    ''' <param name="seed">派生组学层的随机种子</param>
    ''' <param name="filterToPathwayGenes">是否只保留出现在基因集中的特征（标准特征过滤）</param>
    Public Function BuildRealDataset(dataDir As String,
                                     Optional seed As Integer = DefaultSeed,
                                     Optional filterToPathwayGenes As Boolean = True,
                                     Optional verbose As Boolean = True) As RealDataset

        Dim gmtFile As String = Path.Combine(dataDir, "kegg_v3.gmt")
        Dim npyFile As String = Path.Combine(dataDir, "r160_subtype.npy")
        Dim sampleFile As String = Path.Combine(dataDir, "sampleid_list.txt")
        Dim geneSets As Background = GeneSetTable.LoadGmt(gmtFile, verbose)

        If verbose Then
            Call $"[moGSVA] GMT: {geneSets.clusters.Length} pathway(s), {geneSets.size} unique gene symbol(s)".println
        End If

        Dim rna As Matrix = LoadRnaMatrix(npyFile, sampleFile, verbose)
        Dim rawGenes As Integer = rna.size
        Dim relabeled As Boolean = False
        Dim pool As String() = geneSets.clusters _
            .SelectMany(Function(c) c.members.Select(Function(g) g.accessionID)) _
            .Distinct(StringComparer.Ordinal) _
            .ToArray
        Dim poolSet As New HashSet(Of String)(pool, StringComparer.Ordinal)
        Dim hit As Integer = rna.rownames.Where(Function(g) poolSet.Contains(g)).Count()

        If hit < 100 Then
            ' 行名不是基因符号（例如 Entrez ID），为保证测试可跑，按固定种子确定性地重标注
            If verbose Then
                Call $"[moGSVA] only {hit} feature id(s) can be matched to the GMT gene symbols, deterministic re-labeling is applied.".warning
            End If

            rna = RelabelFeatures(rna, pool, New Random(seed))
            relabeled = True
            hit = rna.rownames.Where(Function(g) poolSet.Contains(g)).Count()
        End If

        If filterToPathwayGenes Then
            Dim keep As String() = rna.rownames.Where(Function(g) poolSet.Contains(g)).ToArray

            If verbose Then
                Call $"[moGSVA] feature filter: {keep.Length} of {rna.size} feature(s) belong to at least one pathway.".println
            End If

            rna = rna(keep)
        End If

        ' mRNA 层：逐特征 z 分数（连续数据，配合高斯核）
        Dim mrna As OmicsLayer = OmicsNormalizer.Apply(OmicsLayer.Create("mRNA", rna, verbose), OmicsNormalization.ZScore)
        Dim z As Dictionary(Of String, Double()) = mrna.Expr.expression _
            .ToDictionary(Function(r) r.geneID, Function(r) r.experiments, StringComparer.Ordinal)
        Dim rng As New Random(seed)
        Dim proteinRows As New List(Of DataFrameRow)
        Dim methylRows As New List(Of DataFrameRow)

        For Each row As DataFrameRow In mrna.Expr.expression
            Dim zRow As Double() = z(row.geneID)

            ' 蛋白层：约 60% 的基因有蛋白定量，正相关 + 剂量噪声
            If rng.NextDouble() < 0.6 Then
                Dim v As Double() = New Double(zRow.Length - 1) {}

                For j As Integer = 0 To zRow.Length - 1
                    v(j) = 0.85 * zRow(j) + 0.55 * Gauss(rng)
                Next

                proteinRows.Add(New DataFrameRow With {.geneID = row.geneID, .experiments = v})
            End If

            ' 甲基化层：约 55% 的基因有探针，beta 值与表达反相关，并声明 direction = -1
            If rng.NextDouble() < 0.55 Then
                Dim beta As Double() = New Double(zRow.Length - 1) {}

                For j As Integer = 0 To zRow.Length - 1
                    Dim value As Double = 0.5 - 0.12 * zRow(j) + 0.03 * Gauss(rng)

                    If value < 0.001 Then
                        value = 0.001
                    ElseIf value > 0.999 Then
                        value = 0.999
                    End If

                    beta(j) = value
                Next

                methylRows.Add(New DataFrameRow With {.geneID = row.geneID, .experiments = beta})
            End If
        Next

        Dim protein As New OmicsLayer With {
            .Name = "protein",
            .Normalization = NameOf(OmicsNormalization.None),
            .Direction = 1,
            .Expr = New Matrix With {
                .sampleID = mrna.SampleID,
                .tag = "TCGA-COAD:protein(derived)",
                .expression = proteinRows.ToArray
            }
        }
        Dim methylation As New OmicsLayer With {
            .Name = "methylation",
            .Normalization = NameOf(OmicsNormalization.None),
            .Direction = -1,
            .Expr = New Matrix With {
                .sampleID = mrna.SampleID,
                .tag = "TCGA-COAD:methylation(derived)",
                .expression = methylRows.ToArray
            }
        }

        If verbose Then
            Call $"[moGSVA] derived layers: {mrna} / {protein} / {methylation}".println
        End If

        Dim dataset As MultiOmicsDataset = MultiOmicsDataset.Create({mrna, protein, methylation}, verbose)
        Dim subtype As Dictionary(Of String, String) = If(File.Exists(sampleFile), LoadSubtype(sampleFile), New Dictionary(Of String, String)(StringComparer.Ordinal))

        Return New RealDataset With {
            .Dataset = dataset,
            .GeneSets = geneSets,
            .SampleID = dataset.SampleID,
            .Subtype = dataset.SampleID.Select(Function(id) If(subtype.ContainsKey(id), subtype(id), "")).ToArray,
            .GeneCount = mrna.FeatureCount,
            .Relabeled = relabeled,
            .RawGenes = rawGenes
        }
    End Function

    ''' <summary>
    ''' 把矩阵行名替换为基因符号（确定性洗牌后按行循环分配）。
    ''' </summary>
    Private Function RelabelFeatures(expr As Matrix, pool As String(), rng As Random) As Matrix
        Dim labels As String() = DirectCast(pool.Clone(), String())

        For i As Integer = labels.Length - 1 To 1 Step -1
            Dim j As Integer = rng.Next(i + 1)
            Dim tmp As String = labels(i)

            labels(i) = labels(j)
            labels(j) = tmp
        Next

        Dim rows As DataFrameRow() = expr.expression _
            .Select(Function(r, i) New DataFrameRow With {
                .geneID = labels(i Mod labels.Length),
                .experiments = r.experiments
            }) _
            .ToArray

        Return New Matrix With {
            .sampleID = expr.sampleID,
            .tag = $"{expr.tag}:relabeled",
            .expression = rows
        }
    End Function

    ' ------------------------------------------------------------------
    ' 合成数据：可控信号
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' 构造三层可控信号数据（每层对通路的响应量已知）。
    ''' </summary>
    ''' <remarks>
    ''' 4 条通路 × 100 个基因 + 400 个背景基因，40 个样本分两组：
    ''' <list type="bullet">
    ''' <item><description><c>setA</c>：三层同向上调（完全一致）</description></item>
    ''' <item><description><c>setB</c>：仅 mRNA 上调（单组学信号）</description></item>
    ''' <item><description><c>setC</c>：mRNA + 蛋白上调，甲基化无变化</description></item>
    ''' <item><description><c>setD</c>：mRNA + 蛋白上调，但甲基化 beta 值升高（direction=-1 后成为反向）</description></item>
    ''' </list>
    ''' </remarks>
    Public Function BuildSynthetic(Optional seed As Integer = DefaultSeed) As SyntheticDataset
        Const nSample As Integer = 40
        Const groupSize As Integer = 20
        Const setSize As Integer = 100
        Const nBackground As Integer = 400

        Dim sets As String() = {"setA", "setB", "setC", "setD"}
        Dim rng As New Random(seed)
        Dim effects As New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.Ordinal)

        effects("setA") = New Dictionary(Of String, Double)(StringComparer.Ordinal) From {{"mRNA", 2.0}, {"protein", 1.2}, {"methylation", -0.2}}
        effects("setB") = New Dictionary(Of String, Double)(StringComparer.Ordinal) From {{"mRNA", 2.0}, {"protein", 0.0}, {"methylation", 0.0}}
        effects("setC") = New Dictionary(Of String, Double)(StringComparer.Ordinal) From {{"mRNA", 2.0}, {"protein", 1.2}, {"methylation", 0.0}}
        effects("setD") = New Dictionary(Of String, Double)(StringComparer.Ordinal) From {{"mRNA", 2.0}, {"protein", 1.2}, {"methylation", 0.2}}

        Dim genes As New List(Of String)

        For Each set_ As String In sets
            For i As Integer = 1 To setSize
                genes.Add($"{set_}_g{i}")
            Next
        Next
        For i As Integer = 1 To nBackground
            genes.Add($"bg_g{i}")
        Next

        Dim sampleID As String() = Enumerable.Range(1, nSample).Select(Function(i) $"S{i}").ToArray
        Dim mrnaRows As New List(Of DataFrameRow)
        Dim proteinRows As New List(Of DataFrameRow)
        Dim methylRows As New List(Of DataFrameRow)

        For Each gene As String In genes
            Dim set_ As String = If(gene.StartsWith("bg_"), Nothing, gene.Split("_"c)(0))
            Dim mrna As Double() = New Double(nSample - 1) {}
            Dim protein As Double() = New Double(nSample - 1) {}
            Dim beta As Double() = New Double(nSample - 1) {}

            For j As Integer = 0 To nSample - 1
                Dim second As Boolean = j >= groupSize

                mrna(j) = Gauss(rng)
                protein(j) = Gauss(rng)
                beta(j) = 0.5 + 0.05 * Gauss(rng)

                If set_ IsNot Nothing AndAlso second Then
                    mrna(j) += effects(set_)("mRNA")
                    protein(j) += effects(set_)("protein")
                    beta(j) += effects(set_)("methylation")
                End If

                If beta(j) < 0.001 Then
                    beta(j) = 0.001
                ElseIf beta(j) > 0.999 Then
                    beta(j) = 0.999
                End If
            Next

            mrnaRows.Add(New DataFrameRow With {.geneID = gene, .experiments = mrna})
            proteinRows.Add(New DataFrameRow With {.geneID = gene, .experiments = protein})
            methylRows.Add(New DataFrameRow With {.geneID = gene, .experiments = beta})
        Next

        Dim layers As OmicsLayer() = {
            BuildLayer("mRNA", sampleID, mrnaRows, 1),
            BuildLayer("protein", sampleID, proteinRows, 1),
            BuildLayer("methylation", sampleID, methylRows, -1)
        }
        Dim geneSets As Background = New Background With {
            .id = "synthetic",
            .name = "moGSVA synthetic planted signal",
            .clusters = sets _
                .Select(Function(set_)
                            Return New Cluster With {
                                .ID = set_,
                                .names = set_,
                                .description = set_,
                                .members = genes _
                                    .Where(Function(g) g.StartsWith(set_ & "_")) _
                                    .Select(Function(g) New BackgroundGene(g)) _
                                    .ToArray
                            }
                        End Function) _
                .ToArray
        }

        Return New SyntheticDataset With {
            .Dataset = MultiOmicsDataset.Create(layers, verbose:=False),
            .GeneSets = geneSets,
            .SampleID = sampleID,
            .GroupSize = groupSize,
            .UpRegulated = sets,
            .Effects = effects
        }
    End Function

    Private Function BuildLayer(name As String, sampleID As String(), rows As List(Of DataFrameRow), direction As Integer) As OmicsLayer
        Return New OmicsLayer With {
            .Name = name,
            .Normalization = NameOf(OmicsNormalization.None),
            .Direction = direction,
            .Expr = New Matrix With {
                .sampleID = sampleID,
                .tag = $"synthetic:{name}",
                .expression = rows.ToArray
            }
        }
    End Function

    ' ------------------------------------------------------------------
    ' 导出（供 R 基线使用）
    ' ------------------------------------------------------------------

    ''' <summary>
    ''' 以 G17 + InvariantCulture 写出「特征 × 样本」矩阵 csv（首列 geneID）。
    ''' </summary>
    Public Sub WriteMatrixCsv(mat As Matrix, path As String)
        Using writer As New StreamWriter(path, False, New Text.UTF8Encoding(False))
            Call writer.WriteLine(String.Join(",", New String() {"geneID"}.Concat(mat.sampleID)))

            For Each row As DataFrameRow In mat.expression
                Dim line As New List(Of String) From {"""" & row.geneID & """"}

                For Each x As Double In row.experiments
                    line.Add(x.ToString("G17", CultureInfo.InvariantCulture))
                Next

                Call writer.WriteLine(String.Join(",", line))
            Next
        End Using
    End Sub

    ''' <summary>
    ''' 把基因集写成 R 可读的长表（pathway / gene），只保留在给定特征全集内出现的成员。
    ''' </summary>
    Public Sub WriteGeneSets(geneSets As Background, features As IEnumerable(Of String), path As String)
        Dim universe As New HashSet(Of String)(features, StringComparer.Ordinal)

        Using writer As New StreamWriter(path, False, New Text.UTF8Encoding(False))
            Call writer.WriteLine("pathway" & vbTab & "gene")

            For Each cluster As Cluster In geneSets.clusters
                Dim hits As String() = cluster.members _
                    .Select(Function(g) g.accessionID) _
                    .Where(Function(g) universe.Contains(g)) _
                    .Distinct(StringComparer.Ordinal) _
                    .ToArray

                If hits.Length = 0 Then
                    Continue For
                End If

                For Each gene As String In hits
                    Call writer.WriteLine(cluster.ID & vbTab & gene)
                Next
            Next
        End Using
    End Sub

    ''' <summary>Box-Muller 标准正态随机数</summary>
    Public Function Gauss(rng As Random) As Double
        Dim u1 As Double = 1 - rng.NextDouble()
        Dim u2 As Double = rng.NextDouble()

        Return std.Sqrt(-2 * std.Log(u1)) * std.Cos(2 * std.PI * u2)
    End Function

    ''' <summary>测试内部的终端输出助手</summary>
    <Extension>
    Private Sub println(msg As String)
        Call Console.WriteLine(msg)
    End Sub

End Module
