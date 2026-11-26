#Region "Microsoft.VisualBasic::31c1497e01ba828acd74ed927aaff47c, analysis\HTS_matrix\Matrix\MatrixBuilder.vb"

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


    ' Code Statistics:

    '   Total Lines: 111
    '    Code Lines: 73 (65.77%)
    ' Comment Lines: 27 (24.32%)
    '    - Xml Docs: 88.89%
    ' 
    '   Blank Lines: 11 (9.91%)
    '     File Size: 4.51 KB


    ' Module MatrixBuilder
    ' 
    '     Function: BuildAndNormalizeAbundanceMatrix, BuildMatrix, MatrixInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' Expression abundance matrix builder
''' </summary>
Public Module MatrixBuilder

    ''' <summary>
    ''' 将所有样本的物种丰度结果构建并归一化为一个丰度矩阵。
    ''' </summary>
    ''' <param name="allSampleAbundances">聚合后的数据，Key为样本ID，Value为该样本的物种丰度字典。</param>
    ''' <param name="performNormalization">是否执行按列（样本）的总和归一化，使每个样本的相对丰度之和为1。</param>
    ''' <returns>一个包含物种和样本丰度信息的DataTable。</returns>
    Public Function BuildAndNormalizeAbundanceMatrix(allSampleAbundances As NamedValue(Of Dictionary(Of String, Double))(),
                                                     Optional performNormalization As Boolean = True,
                                                     Optional tag As String = "expression_matrix") As Matrix
        If allSampleAbundances.IsNullOrEmpty Then
            Call "no sample data to build expression matrix!".warning

            Return New Matrix With {
                .tag = "empty_samples",
                .expression = {},
                .sampleID = {}
            }
        End If

        Dim allSampleIds As String() = allSampleAbundances.Keys.ToArray
        Dim table As DataFrameRow() = allSampleAbundances _
            .ToDictionary(Function(a) a.Name,
                          Function(a)
                              Return a.Value
                          End Function) _
            .MatrixInternal(allSampleIds) _
            .ToArray
        Dim matrix As New Matrix With {
            .sampleID = allSampleIds,
            .expression = table,
            .tag = tag
        }

        If performNormalization Then
            Return TPM.Normalize(matrix)
        Else
            Return matrix
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="data">
    ''' collection of the gene expression data: [gene1, [samle1, sample2, sample3, ...]]
    ''' </param>
    ''' <param name="tag"></param>
    ''' <returns></returns>
    Public Function BuildMatrix(Of T As {INamedValue, DynamicPropertyBase(Of Double)})(data As IEnumerable(Of T), Optional tag As String = "expression_matrix") As Matrix
        Dim pool As T() = data.ToArray
        Dim sample_id As String() = pool _
            .SelectMany(Function(gene) gene.Properties.Keys) _
            .Distinct _
            .OrderBy(Function(id) id) _
            .ToArray
        Dim genes As DataFrameRow() = pool _
            .Select(Function(gene) New DataFrameRow(gene.Key, gene(sample_id))) _
            .ToArray

        Return New Matrix With {
            .expression = genes,
            .sampleID = sample_id,
            .tag = tag
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="allSamples">
    ''' collection of samples data: [sample1, [gene1, gene2,...]]
    ''' </param>
    ''' <param name="allSampleIds"></param>
    ''' <returns></returns>
    <Extension>
    Private Iterator Function MatrixInternal(allSamples As Dictionary(Of String, Dictionary(Of String, Double)), allSampleIds As String()) As IEnumerable(Of DataFrameRow)
        ' --- 步骤 1: 识别所有唯一的物种和样本 ---
        Dim featuresIds As List(Of String) = allSamples.Values _
            .SelectMany(Function(dict) dict.Keys) _
            .Distinct() _
            .OrderBy(Function(id) id) _
            .ToList()

        Call VBDebugger.EchoLine($"found {featuresIds.Count} unique features and {allSampleIds.Length} unique samples.")

        For Each id As String In featuresIds
            Dim v As IEnumerable(Of Double) =
                From sample_id As String
                In allSampleIds
                Let sample As Dictionary(Of String, Double) = allSamples(sample_id)
                Select sample.TryGetValue(id)

            Yield New DataFrameRow With {
                .experiments = v.ToArray,
                .geneID = id
            }
        Next
    End Function

    ''' <summary>
    ''' 把"基因 × 样本"的流式行序列转换成 <b>基因 × 样本</b> 的 CSC 稀疏矩阵。
    ''' </summary>
    ''' <param name="rows">
    ''' 流式基因行（来自 <see cref="Document.StreamRows"/>）。每行的 <c>experiments</c> 必须与
    ''' <paramref name="sample_ids"/> 同序、同长；零值与非有限值会被跳过。
    ''' </param>
    ''' <param name="sample_ids">保留下来的样本（细胞）ID，顺序与行内 <c>experiments</c> 一致</param>
    ''' <param name="verbose">是否打印 nnz / 稀疏率 / 耗时诊断</param>
    ''' <returns>
    ''' 行=基因、列=样本的 CSC；<paramref name="rows"/> 为空时返回 <c>CscMatrix.Empty(0,0)</c>
    ''' </returns>
    ''' <remarks>
    ''' <para>
    ''' <b>为什么返回"基因 × 样本"</b>：流式输入本身就是按基因给出的，这与 CSC 的"按列压缩"并不冲突——
    ''' 无论 CSR 还是 CSC，真正的成本都在"如何把按基因到达的数据按样本归拢"。
    ''' 需要"样本 × 基因"时请用 <see cref="AsCsrMatrix"/>，两者的内部数组完全相同（互为转置）。
    ''' </para>
    ''' <para>
    ''' <b>为什么不能二次遍历</b>：<c>StreamRows</c> 是 <c>Iterator</c>，只能枚举一次，
    ''' 因此实现采用"单遍建基因主序 CSR + O(nnz) 计数排序转置"，而不是"先数一遍再填一遍"。
    ''' </para>
    ''' </remarks>
    <Extension>
    Public Function AsCscMatrix(rows As IEnumerable(Of DataFrameRow), sample_ids As String(),
                                Optional verbose As Boolean = True) As CscMatrix
        Dim sampleCount As Integer = If(sample_ids Is Nothing, 0, sample_ids.Length)
        Dim transposed As GeneMajorStream = TransposeStream(rows, sample_ids, verbose)

        If transposed Is Nothing OrElse transposed.GeneCount = 0 OrElse sampleCount = 0 Then
            Return CscMatrix.Empty(0, 0)
        End If

        ' CSC(基因 × 样本) 的 (indptr, indices) 恰好等于 CSR(Aᵀ) 的数组：
        ' indptr 按样本（列）压缩，indices 是基因下标（同一列内递增）
        Return New CscMatrix(transposed.GeneCount, sampleCount,
                             transposed.Indptr, transposed.Indices, transposed.Values)
    End Function

    ''' <summary>
    ''' 把"基因 × 样本"的流式行序列转换成 <b>样本 × 基因</b> 的 CSR 稀疏矩阵。
    ''' </summary>
    ''' <param name="rows">流式基因行；零值与非有限值会被跳过</param>
    ''' <param name="sample_ids">保留下来的样本（细胞）ID，顺序与行内 <c>experiments</c> 一致</param>
    ''' <param name="verbose">是否打印 nnz / 稀疏率 / 耗时诊断</param>
    ''' <returns>
    ''' 行=样本、列=基因的 CSR；空输入时返回 0×0 的空矩阵
    ''' </returns>
    ''' <remarks>
    ''' <para>
    ''' <b>为什么是转置</b>：单细胞算法（如 CellularAI 的对照矩阵）需要"细胞 × 基因"的行主序布局，
    ''' 而 CSV 是按基因给出的，所以必须做一次 <c>O(nnz)</c> 的计数排序转置；
    ''' 转置后每行（细胞）内的基因下标天然递增，满足 CSR 的有序性要求，无需再排序。
    ''' </para>
    ''' <para>
    ''' <b>内存</b>：峰值约为最终矩阵的 2 倍（基因主序中间态 + 转置结果），
    ''' 对 102,525 细胞 × 23,450 基因的数据量级需要预留数 GB；调用方若只需要其中一个方向，
    ''' 不要同时调用本函数与 <see cref="AsCscMatrix"/>（各自独立建一份中间态）。
    ''' </para>
    ''' </remarks>
    <Extension>
    Public Function AsCsrMatrix(rows As IEnumerable(Of DataFrameRow), sample_ids As String(),
                                Optional verbose As Boolean = True) As CsrMatrix
        Dim sampleCount As Integer = If(sample_ids Is Nothing, 0, sample_ids.Length)
        Dim transposed As GeneMajorStream = TransposeStream(rows, sample_ids, verbose)

        If transposed Is Nothing OrElse transposed.GeneCount = 0 OrElse sampleCount = 0 Then
            Return New CsrMatrix(0, 0, {0}, {}, {})
        End If

        Return New CsrMatrix(sampleCount, transposed.GeneCount,
                             transposed.Indptr, transposed.Indices, transposed.Values)
    End Function

#Region "stream kernels"

    ''' <summary>
    ''' 转置后的基因主序数据：<c>Indptr</c> 按样本压缩，<c>Indices</c> 为基因下标。
    ''' </summary>
    Private Class GeneMajorStream
        Public Property GeneCount As Integer
        Public Property Indptr As Integer()
        Public Property Indices As Integer()
        Public Property Values As Double()
    End Class

    ''' <summary>
    ''' 单遍读取流式行 → 基因主序 CSR → 按样本计数排序转置。
    ''' </summary>
    ''' <param name="rows">流式基因行</param>
    ''' <param name="sample_ids">样本 ID（用于长度校验与诊断）</param>
    ''' <param name="verbose">是否输出诊断</param>
    Private Function TransposeStream(rows As IEnumerable(Of DataFrameRow), sample_ids As String(),
                                     verbose As Boolean) As GeneMajorStream
        If rows Is Nothing Then
            Return Nothing
        End If
        If sample_ids Is Nothing Then
            Throw New ArgumentNullException(NameOf(sample_ids))
        End If

        Dim sampleCount As Integer = sample_ids.Length
        Dim clock As Stopwatch = Stopwatch.StartNew()

        ' ---- 1) 单遍：按基因累积非零元（行内样本下标天然递增）----
        Dim geneIndptr As New List(Of Integer) From {0}
        Dim geneIndices As New List(Of Integer)
        Dim geneValues As New List(Of Double)

        Dim geneCount As Integer = 0
        Dim skippedNonFinite As Long = 0
        Dim negativeCount As Long = 0
        Dim ordinal As Integer = 0

        For Each row As DataFrameRow In rows
            If row Is Nothing Then
                Call geneIndptr.Add(geneIndices.Count)
                geneCount += 1
                Continue For
            End If

            Dim values As Double() = row.experiments

            If values Is Nothing Then
                values = New Double(-1) {}
            End If
            If values.Length <> sampleCount Then
                Throw New ArgumentException(
                    $"row '{If(row.geneID, "#" & ordinal)}' carries {values.Length} value(s) " &
                    $"but sample_ids has {sampleCount}; the row/value axes are not aligned",
                    NameOf(rows))
            End If

            For j As Integer = 0 To values.Length - 1
                Dim v As Double = values(j)

                If v = 0.0 Then
                    Continue For
                End If
                If Double.IsNaN(v) OrElse Double.IsInfinity(v) Then
                    skippedNonFinite += 1
                    Continue For
                End If
                If v < 0.0 Then
                    negativeCount += 1
                End If

                Call geneIndices.Add(j)
                Call geneValues.Add(v)
            Next

            Call geneIndptr.Add(geneIndices.Count)
            geneCount += 1
            ordinal += 1
        Next

        If geneCount = 0 OrElse sampleCount = 0 Then
            Return New GeneMajorStream With {.GeneCount = geneCount, .Indptr = {0}, .Indices = {}, .Values = {}}
        End If

        Dim buildElapsed As Long = clock.ElapsedMilliseconds

        ' ---- 2) 按样本计数排序转置（O(nnz)）----
        Dim nnz As Integer = geneIndices.Count
        Dim counts(sampleCount - 1) As Integer

        For k As Integer = 0 To nnz - 1
            counts(geneIndices(k)) += 1
        Next

        Dim indptr(sampleCount) As Integer
        Dim accumulate As Integer = 0

        For c As Integer = 0 To sampleCount - 1
            indptr(c) = accumulate
            accumulate += counts(c)
        Next

        indptr(sampleCount) = accumulate

        Dim cursor As Integer() = DirectCast(indptr.Clone(), Integer())
        Dim indices(If(nnz = 0, -1, nnz - 1)) As Integer
        Dim transposedValues(If(nnz = 0, -1, nnz - 1)) As Double

        For g As Integer = 0 To geneCount - 1
            For k As Integer = geneIndptr(g) To geneIndptr(g + 1) - 1
                Dim column As Integer = geneIndices(k)
                Dim position As Integer = cursor(column)

                indices(position) = g
                transposedValues(position) = geneValues(k)
                cursor(column) = position + 1
            Next
        Next

        If verbose Then
            clock.Stop()

            Dim total As Long = CLng(geneCount) * CLng(sampleCount)
            Dim density As Double = If(total > 0, nnz / total, 0.0)

            Call Console.WriteLine(
                $"[HTS] stream->sparse: {geneCount} gene x {sampleCount} sample, nnz={nnz} " &
                $"({density:P3} dense), build {buildElapsed} ms, transpose {clock.ElapsedMilliseconds - buildElapsed} ms" &
                If(skippedNonFinite > 0, $", skipped {skippedNonFinite} non-finite", "") &
                If(negativeCount > 0, $", {negativeCount} negative value(s)", ""))
        End If

        Return New GeneMajorStream With {
            .GeneCount = geneCount,
            .Indptr = indptr,
            .Indices = indices,
            .Values = transposedValues
        }
    End Function

#End Region

End Module
