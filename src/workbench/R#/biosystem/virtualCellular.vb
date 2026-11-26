Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.VirtualCellular.Data
Imports SMRUCC.genomics.Analysis.VirtualCellular.IO
Imports SMRUCC.genomics.Analysis.VirtualCellular.Model
Imports SMRUCC.genomics.Analysis.VirtualCellular.Perturbation
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' VirtualCellular: 基于 CellularAI 的虚拟细胞模型与虚拟扰动实验
''' </summary>
''' <remarks>
''' <para>
''' 这个模块把 <c>VirtualCellular</c> 工作流的四步暴露给 R# 脚本：
''' </para>
''' <list type="number">
''' <item><description><c>model.build</c>：从表达矩阵 CSV + 样本元数据装配模型并落盘（一次，约十几分钟）；</description></item>
''' <item><description><c>model.open</c> / <c>model.save</c>：复用已装配好的模型包，不再解析原始 CSV；</description></item>
''' <item><description><c>overexpress</c>：对指定基因做虚拟过表达，导出差异表达表与 Top 变化清单；</description></item>
''' <item><description><c>model.genes</c> / <c>model.perturbations</c>：检查模型的基因轴与伪扰动轴。</description></item>
''' </list>
''' <para>
''' 模型对象是 CLR 引用，可在 R# 变量里长期持有；重复调用 <c>overexpress</c> 不会重新装配。
''' </para>
''' </remarks>
<Package("VirtualCellular")>
Public Module virtualCellularTools

    ''' <summary>
    ''' 装配一个虚拟细胞模型并保存为模型包。
    ''' </summary>
    ''' <param name="expr">表达矩阵 CSV（行=基因，列=细胞）</param>
    ''' <param name="meta">细胞元数据 CSV（需含 ID / patient_id / stage / type 列）</param>
    ''' <param name="output">模型包输出路径（*.zip）</param>
    ''' <param name="stages">只保留这些期别的细胞；<c>NULL</c> 表示全量</param>
    ''' <param name="limit">每个期别最多保留的细胞数；0 表示不限制</param>
    ''' <param name="seed">随机种子（写入模型清单）</param>
    ''' <returns>装配好的 <see cref="VirtualCellModel"/> 对象</returns>
    <ExportAPI("model.build")>
    <RApiReturn(GetType(VirtualCellModel))>
    Public Function BuildModel(expr As String, meta As String, output As String,
                               Optional stages As String() = Nothing,
                               Optional limit As Integer = 0,
                               Optional seed As Integer = 0) As VirtualCellModel
        Dim stream As ExpressionStream = ExpressionStream.Load(expr, meta, stages, verbose:=True, limitPerStage:=limit)
        Dim bundle = SummaryAssembly.Build(stream)
        Dim model As New VirtualCellModel With {
            .Options = SMRUCC.genomics.Analysis.Illumina.CellularAI.Model.CellularAiModel.Default(),
            .Bundle = bundle,
            .Controls = stream.Controls,
            .Genes = stream.Genes,
            .Cells = stream.Cells,
            .Metadata = stream.Metadata,
            .Manifest = New ModelManifest With {
                .Seed = seed,
                .NormalSelector = stream.Metadata.NormalSelector,
                .StageFilter = stream.StageFilter
            }
        }

        Call Save(model, output)

        Return model
    End Function

    ''' <summary>打开一个已保存的模型包。</summary>
    ''' <param name="path">模型包路径（*.zip）</param>
    <ExportAPI("model.open")>
    <RApiReturn(GetType(VirtualCellModel))>
    Public Function OpenModel(path As String) As VirtualCellModel
        Using input As Stream = File.OpenRead(path)
            Return VirtualCellModel.Load(input)
        End Using
    End Function

    ''' <summary>把模型保存为模型包。</summary>
    <ExportAPI("model.save")>
    <RApiReturn(GetType(String))>
    Public Function Save(model As VirtualCellModel, path As String) As String
        ' 参数名 path 与 System.IO.Path 类同名（VB 不区分大小写），这里显式全限定
        Dim folder As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path))

        If Not String.IsNullOrEmpty(folder) Then
            Call Directory.CreateDirectory(folder)
        End If

        Using output As Stream = File.Create(path)
            Call model.Save(output)
        End Using

        Return path
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
    ''' 对指定基因做虚拟过表达，导出差异表达表与 Top 变化清单。
    ''' </summary>
    ''' <param name="model">由 <c>model.build</c> 或 <c>model.open</c> 得到的模型</param>
    ''' <param name="genes">目标基因；多个基因会自动做联合过表达</param>
    ''' <param name="output">输出目录（每个基因一个子目录）</param>
    ''' <param name="cells">合成细胞数</param>
    ''' <param name="seed">随机种子</param>
    ''' <returns>显著变化的基因数</returns>
    <ExportAPI("overexpress")>
    <RApiReturn(GetType(Integer))>
    Public Function Overexpress(model As VirtualCellModel, genes As String(), output As String,
                                Optional cells As Integer = 400,
                                Optional seed As Integer = 0) As Integer
        Dim engine As New CellularAIPerturbation(model, seed)
        Dim alpha As String() = If(genes Is Nothing, New String() {}, genes)
        Dim singles As New List(Of String)
        Dim significant As Integer = 0

        For Each gene As String In alpha
            Dim result = engine.OverexpressGene(gene, cells)

            If result Is Nothing OrElse result.Undefined Then
                Continue For
            End If

            Call Write(result, gene, output)
            Call singles.Add(gene)
            significant += result.NAffected
        Next

        If alpha.Length > 1 Then
            Dim joint = engine.OverexpressGenes(alpha, cells)

            If joint IsNot Nothing AndAlso Not joint.Undefined Then
                Call Write(joint, String.Join("+", alpha), output)
                significant += joint.NAffected
            End If
        End If

        Return significant
    End Function

    Private Sub Write(result As SMRUCC.genomics.BioSystem.Intervention.InterventionResult,
                      label As String, output As String)
        Dim folder As String = Path.Combine(output, $"{label}_oe")

        Call Directory.CreateDirectory(folder)
        Call ResultExporter.WriteDe(result, Path.Combine(folder, "de.csv"))
        Call ResultExporter.WriteSummary(result, Path.Combine(folder, "summary.csv"))
    End Sub

End Module
