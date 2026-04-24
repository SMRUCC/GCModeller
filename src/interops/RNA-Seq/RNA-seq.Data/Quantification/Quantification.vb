Imports SMRUCC.genomics.SequenceModel.SAM.featureCount

Namespace GeneQuantification

    Public Module Quantification

        Public Iterator Function MakeGeneExpressions(featureCounts As IEnumerable(Of featureCounts)) As IEnumerable(Of GeneSampleSet)

        End Function

        ''' <summary>
        ''' 将 Raw Count 转化为 TPM 和 FPKM
        ''' </summary>
        ''' <param name="stats">the sam index stats table file</param>
        ''' <param name="totalMappedFragments">
        ''' Optional parameter specifying the total number of mapped fragments. 
        ''' If not provided, it will be approximated using the sum of raw counts.
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 使用了 `totalRawCount += hit.RawCount`，也就是**把传入的所有基因的 Raw Count 加起来**作为 Total Mapped Fragments。
        ''' 这种做法的好处是：如果用户没有提供 totalMappedFragments 参数，我们仍然可以计算出一个合理的 TPM 和 FPKM 值。
        ''' 
        ''' 这在大多数简单情况下是可行的，但在严格的生信分析中，**基因 Count 的总和并不等于实际的 Total Mapped Fragments**。原因如下：
        ''' 
        ''' 1. **未比对上的 Reads**：有些 Fragment 比对到了基因间区或内含子，不会被 `featureCounts` 计入任何基因的 Count 中，但它们依然是 Mapped Fragments。
        ''' 2. **多基因比对**：有些 Fragment 比对到了多个基因，`featureCounts` 默认会丢弃它们（不计入 RawCount），但它们客观存在于比对结果中。
        ''' 3. **线粒体基因/rRNA**：有时在计算 mRNA 表达量时，会故意剔除线粒体基因或 rRNA，但这会导致 Count 总和远小于 Total Mapped Fragments。
        ''' 
        ''' **更严谨的做法：**
        ''' 
        ''' 如果你手头有这个样本的**实际比对总数**（通常可以从 `featureCounts` 的运行日志 `.summary` 文件中读取到 `Assigned + Unassigned` 的总数，或者从 `samtools flagstat` 中获取），
        ''' 你应该将其作为参数传入函数，而不是在函数内部累加。
        ''' </remarks>
        Public Iterator Function ConvertCountsToTPM(stats As IEnumerable(Of IndexStats), Optional totalMappedFragments As Long? = Nothing) As IEnumerable(Of GeneData)
            Dim genes As New List(Of GeneData)()
            Dim totalRPK As Double = 0.0
            ' 用于累计总 Count 数，作为 Total Mapped Fragments 的近似
            Dim totalRawCount As Long = 0L

            ' --- 第一步：计算 RPK 并累加 totalRPK 和 totalRawCount ---
            For Each hit As IndexStats In stats
                Dim gene As New GeneData() With {
                    .GeneID = hit.GeneID,
                    .Length = hit.Length,
                    .RawCount = hit.RawCount,
                    .RPK = (.RawCount * 1000.0) / .Length
                }

                genes.Add(gene)
                totalRPK += gene.RPK
                totalRawCount += hit.RawCount ' 累加原始计数
            Next

            ' --- 第二步：根据 totalRPK 计算 TPM ---
            If totalRPK = 0 Then
                Call "Warning: Total RPK is 0. All TPM values will be 0.".warning
            End If
            If totalRawCount = 0 Then
                Call "Warning: Total Raw Count is 0. All FPKM values will be 0.".warning
            End If

            If totalMappedFragments IsNot Nothing Then
                totalRawCount = totalMappedFragments
            End If

            For Each gene As GeneData In genes
                ' 计算 TPM
                If totalRPK > 0 Then
                    gene.TPM = (gene.RPK / totalRPK) * 1000000.0
                Else
                    gene.TPM = 0.0
                End If

                ' 计算 FPKM
                ' 利用已有的 RPK：FPKM = (RPK * 1,000,000) / totalRawCount
                ' 等效于原公式：(RawCount * 10^9) / (totalRawCount * Length)
                If totalRawCount > 0 Then
                    gene.FPKM = (gene.RPK * 1000000.0) / totalRawCount
                Else
                    gene.FPKM = 0.0
                End If

                Yield gene
            Next
        End Function
    End Module
End Namespace