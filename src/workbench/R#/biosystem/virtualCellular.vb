Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.VirtualCellular.Data
Imports SMRUCC.genomics.Analysis.VirtualCellular.IO
Imports SMRUCC.genomics.Analysis.VirtualCellular.Model
Imports SMRUCC.genomics.Analysis.VirtualCellular.Perturbation
Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Intervention
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' VirtualCellular: 基于 CellularAI 的虚拟细胞模型与虚拟扰动实验
''' </summary>
''' <remarks>
''' <para>
''' 模块内的每个 api 只做**一件事**，可以按需组合；计算与文件 IO 严格分离：
''' </para>
''' <list type="table">
''' <item><description><b>数据加载</b>：<c>load_expr</c>（表达矩阵 + 元数据 → <c>ExpressionStream</c>）</description></item>
''' <item><description><b>纯计算</b>：<c>model.build</c>（装配模型，不落盘）、<c>as_perturbation</c>（包装成扰动实验对象）、
''' <c>overexpress_genes</c>（联合过表达）</description></item>
''' <item><description><b>纯 IO</b>：<c>model.save</c> / <c>model.open</c> / <c>export.de</c> / <c>export.summary</c></description></item>
''' <item><description><b>查询</b>：<c>model.genes</c> / <c>model.perturbations</c></description></item>
''' </list>
''' <para>
''' <c>as_perturbation</c> 返回的对象实现了 <see cref="InsilicoPerturbationExperiment"/>，
''' 因此可以直接交给 <c>intervention</c> 包已有的 <c>knockouts</c> / <c>overexpress</c> /
''' <c>knockdown</c> / <c>make_exports</c> 复用，不必在本模块里重复实现扰动与批量导出。
''' </para>
''' </remarks>
<Package("VirtualCellular")>
Public Module virtualCellularTools

    ''' <param name="stages">只保留这些期别的细胞；<c>NULL</c> 表示全量</param>
    ''' <param name="limit">每个期别最多保留的细胞数；0 表示不限制</param>
    ''' 
    ''' <param name="expr">表达矩阵 CSV（行=基因，列=细胞）</param>
    ''' <param name="meta">细胞元数据 CSV（需含 ID / patient_id / stage / type 列）</param>
    <ExportAPI("load_expr")>
    Public Function LoadExpression(expr As String, meta As String, Optional stages As String() = Nothing, Optional limit As Integer = 0) As ExpressionStream
        Return ExpressionStream.Load(expr, meta, stages, verbose:=True, limitPerStage:=limit)
    End Function

    ''' <summary>
    ''' 由加载好的表达数据装配虚拟细胞模型（纯计算，不写任何文件）。
    ''' </summary>
    ''' <param name="exp">由 <c>load_expr</c> 得到的表达数据（含对照矩阵与细胞元数据）</param>
    ''' <param name="opt">CellularAI 参数；<c>NULL</c> 使用默认值</param>
    ''' <param name="seed">随机种子（写入模型清单，供后续扰动复现）</param>
    ''' <returns>装配好的 <see cref="VirtualCellModel"/>；需要保存时另行调用 <c>model.save</c></returns>
    <ExportAPI("model.build")>
    <RApiReturn(GetType(VirtualCellModel))>
    Public Function BuildModel(exp As ExpressionStream, Optional opt As CellularAiModel = Nothing, Optional seed As Integer = 0) As VirtualCellModel
        Dim bundle = SummaryAssembly.Build(exp)

        Return New VirtualCellModel With {
            .Options = If(opt, CellularAiModel.Default),
            .Bundle = bundle,
            .Controls = exp.Controls,
            .Genes = exp.Genes,
            .Cells = exp.Cells,
            .Metadata = exp.Metadata,
            .Manifest = New ModelManifest With {
                .Seed = seed,
                .NormalSelector = exp.Metadata.NormalSelector,
                .StageFilter = exp.StageFilter
            }
        }
    End Function

    ''' <summary>把模型保存为模型包（纯 IO）。</summary>
    ''' <param name="output">模型包路径（*.zip）</param>
    <ExportAPI("model.save")>
    <RApiReturn(GetType(String))>
    Public Function SaveModel(model As VirtualCellModel, output As String) As String
        ' 参数名与 System.IO.Path 类同名（VB 不区分大小写），这里显式全限定
        Dim folder As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(output))

        If Not String.IsNullOrEmpty(folder) Then
            Call Directory.CreateDirectory(folder)
        End If

        Using stream As Stream = File.Create(output)
            Call model.Save(stream)
        End Using

        Return output
    End Function

    ''' <summary>打开一个已保存的模型包（纯 IO）。</summary>
    ''' <param name="path">模型包路径（*.zip）</param>
    <ExportAPI("model.open")>
    <RApiReturn(GetType(VirtualCellModel))>
    Public Function OpenModel(path As String) As VirtualCellModel
        Using input As Stream = File.OpenRead(path)
            Return VirtualCellModel.Load(input)
        End Using
    End Function

    ''' <summary>模型的基因轴。</summary>
    <ExportAPI("model.genes")>
    <RApiReturn(GetType(String))>
    Public Function ModelGenes(model As VirtualCellModel) As String()
        Return model.Genes
    End Function

    ''' <summary>模型的伪扰动轴（期别对比 / 患者对比 / 高表达签名）。</summary>
    <ExportAPI("model.perturbations")>
    <RApiReturn(GetType(String))>
    Public Function ModelPerturbations(model As VirtualCellModel) As String()
        Return model.Perturbations
    End Function

    ''' <summary>
    ''' 把模型包装成虚拟扰动实验对象（纯计算，不做任何 IO）。
    ''' </summary>
    ''' <param name="model">由 <c>model.build</c> 或 <c>model.open</c> 得到的模型</param>
    ''' <param name="cells">默认合成细胞数；<c>intervention</c> 包的 api 未显式指定时使用</param>
    ''' <param name="seed">随机种子</param>
    ''' <returns>
    ''' 实现了 <see cref="InsilicoPerturbationExperiment"/> 的 <see cref="CellularAIPerturbation"/>，
    ''' 可直接传给 <c>intervention::overexpress</c> / <c>knockouts</c> / <c>knockdown</c>。
    ''' </returns>
    <ExportAPI("as_perturbation")>
    <RApiReturn(GetType(CellularAIPerturbation))>
    Public Function AsPerturbation(model As VirtualCellModel,
                                   Optional cells As Integer = 400,
                                   Optional seed As Integer = 0) As CellularAIPerturbation
        Return New CellularAIPerturbation(model, seed) With {
            .DefaultCells = If(cells > 0, cells, 400)
        }
    End Function

    ''' <summary>
    ''' 联合过表达（接口只定义了单基因，多基因场景走这个入口）。
    ''' </summary>
    ''' <param name="experiment">由 <c>as_perturbation</c> 得到的实验对象</param>
    ''' <param name="genes">同时过表达的基因集合</param>
    ''' <param name="cells">合成细胞数；0 表示使用实验对象的默认值</param>
    <ExportAPI("overexpress_genes")>
    <RApiReturn(GetType(InterventionResult))>
    Public Function OverexpressGenes(experiment As CellularAIPerturbation, genes As String(),
                                     Optional cells As Integer = 0) As InterventionResult
        Return experiment.OverexpressGenes(If(genes, New String() {}), cells)
    End Function

    ''' <summary>显著变化的基因（纯查询，无 IO）。</summary>
    ''' <param name="result">一次扰动结果</param>
    <ExportAPI("perturbation.significant")>
    <RApiReturn(GetType(String))>
    Public Function SignificantGenes(result As InterventionResult) As String()
        If result Is Nothing OrElse result.GeneNames Is Nothing OrElse result.IsSignificant Is Nothing Then
            Return New String() {}
        End If

        Dim genes As New List(Of String)

        For i As Integer = 0 To result.GeneNames.Length - 1
            If result.IsSignificant(i) Then
                Call genes.Add(result.GeneNames(i))
            End If
        Next

        Return genes.ToArray
    End Function

    ''' <summary>导出差异表达表（纯 IO）。</summary>
    ''' <param name="result">一次扰动结果</param>
    ''' <param name="path">输出 CSV 路径</param>
    <ExportAPI("export.de")>
    <RApiReturn(GetType(String))>
    Public Function ExportDe(result As InterventionResult, path As String) As String
        Call EnsureFolder(path)
        Call ResultExporter.WriteDe(result, path)

        Return path
    End Function

    ''' <summary>导出 Top 变化清单（纯 IO）。</summary>
    ''' <param name="result">一次扰动结果</param>
    ''' <param name="path">输出 CSV 路径</param>
    ''' <param name="top_n">导出前多少个基因，默认 200</param>
    <ExportAPI("export.summary")>
    <RApiReturn(GetType(String))>
    Public Function ExportSummary(result As InterventionResult, path As String,
                                  Optional top_n As Integer = 200) As String
        Call EnsureFolder(path)
        Call ResultExporter.WriteSummary(result, path, top_n)

        Return path
    End Function

    Private Sub EnsureFolder(path As String)
        Dim folder As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path))

        If Not String.IsNullOrEmpty(folder) Then
            Call Directory.CreateDirectory(folder)
        End If
    End Sub

End Module
