' ============================================================================
' ExpressionIO.vb — 表达矩阵与伪时间文件的读取
'
' 读取职责刻意"薄"——真正的解析全部复用既有基础库，避免重复实现数据格式：
'   表达矩阵   HTS_matrix 的 Matrix.LoadData(file)（CSV/TSV，首列基因名、首行样本名）
'              → BNLearn 的 BnIO.ReadGeneExpressionMatrix 转置为 [gene, sample]
'   伪时间文件 复用 BNLearn 的 BnIO.ReadExpressionVector（两列：样本名 → 数值）
' 本模块只补两件这里独有的工作：
'   1. 统一异常信息（文件不存在 / 交集为空时给出可操作的提示）；
'   2. 把"样本名 → 伪时间"字典按表达矩阵的样本顺序对齐成数组。
' ============================================================================

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Namespace IO

    ''' <summary>基因表达矩阵与伪时间的数据读取</summary>
    Public Module ExpressionIO

        ''' <summary>
        ''' 读取基因表达矩阵（行 = 基因，列 = 样本）。
        ''' </summary>
        ''' <param name="path">CSV/TSV 文件路径：首列为基因名，首行为样本名</param>
        ''' <param name="sampleinfo">
        ''' 可选的样本设计信息：提供后会把 <c>time</c> 列解析为
        ''' <see cref="GeneExpressionData.TimePoints"/>，从而让伪时间离散化
        ''' 直接使用真实时间点；不提供时 TimePoints 全为 0（由样本名回退解析）。
        ''' </param>
        ''' <param name="time_label">SampleInfo 中承载时间标签的列名</param>
        <Extension>
        Public Function LoadExpressionMatrix(path As String,
                                             Optional sampleinfo As SampleInfo() = Nothing,
                                             Optional time_label As String = "time") As GeneExpressionData
            If String.IsNullOrWhiteSpace(path) Then
                Throw New ArgumentException("表达矩阵路径不能为空", NameOf(path))
            End If
            If Not File.Exists(path) Then
                Throw New FileNotFoundException($"表达矩阵文件不存在: {path}", path)
            End If

            Dim matrix As Matrix = Matrix.LoadData(path)
            Dim expr = BnIO.ReadGeneExpressionMatrix(matrix, sampleinfo, time_label)

            If expr.GeneNames Is Nothing OrElse expr.GeneNames.Length = 0 Then
                Throw New InvalidOperationException(
                    $"表达矩阵解析失败（未读到任何基因行）: {path}；" &
                    "请确认首列为基因名且文件为 CSV/TSV 文本格式")
            End If

            Return expr
        End Function

        ''' <summary>
        ''' 读取外部伪时间文件（两列：样本名/细胞名 → 伪时间值），
        ''' 常见来源为 Monocle3 的 <c>pseudotime</c> 导出表。
        ''' </summary>
        ''' <remarks>
        ''' 复用 <see cref="BnIO.ReadExpressionVector"/>：它按 <c>,</c> / Tab / <c>;</c> 切分，
        ''' 并会跳过无法解析为数值的表头行，因此形如
        ''' <code>cell,pseudotime</code> 的表头不影响解析。
        ''' </remarks>
        <Extension>
        Public Function LoadPseudotime(path As String) As Dictionary(Of String, Double)
            If String.IsNullOrWhiteSpace(path) Then
                Throw New ArgumentException("伪时间文件路径不能为空", NameOf(path))
            End If
            If Not File.Exists(path) Then
                Throw New FileNotFoundException($"伪时间文件不存在: {path}", path)
            End If

            Return BnIO.ReadExpressionVector(path)
        End Function

        ''' <summary>
        ''' 把"样本名 → 伪时间"字典按样本顺序对齐为数组。
        ''' </summary>
        ''' <param name="sampleNames">表达矩阵的样本名（决定输出顺序）</param>
        ''' <param name="pseudotime">伪时间字典（键为样本名，大小写不敏感）</param>
        ''' <returns>与 <paramref name="sampleNames"/> 一一对应的伪时间数组</returns>
        ''' <remarks>
        ''' 未在字典中命中的样本会抛出异常——静默填 0 会让这些样本被错误地排到时间轴最前端，
        ''' 从而污染整条训练轨迹，属于"必须尽早失败"的情形。
        ''' </remarks>
        Public Function AlignPseudotime(sampleNames As String(),
                                        pseudotime As Dictionary(Of String, Double)) As Double()
            If sampleNames Is Nothing Then
                Throw New ArgumentNullException(NameOf(sampleNames))
            End If
            If pseudotime Is Nothing Then
                Throw New ArgumentNullException(NameOf(pseudotime))
            End If

            Dim values(sampleNames.Length - 1) As Double
            Dim missing As New List(Of String)()

            For i = 0 To sampleNames.Length - 1
                Dim v As Double
                If pseudotime.TryGetValue(sampleNames(i), v) Then
                    values(i) = v
                Else
                    values(i) = Double.NaN
                    missing.Add(sampleNames(i))
                End If
            Next

            If missing.Count > 0 Then
                Throw New InvalidOperationException(
                    $"伪时间文件中有 {missing.Count}/{sampleNames.Length} 个样本未命中：" &
                    $"{String.Join(", ", missing.Take(5))}{(If(missing.Count > 5, " ...", ""))}；" &
                    "请确认样本命名与表达矩阵完全一致（大小写不敏感）")
            End If

            Return values
        End Function

    End Module

End Namespace
