Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Slicer

Public Class PrimerCoverage

    Public Property GeneID As String
    Public Property Chromosome As String
    Public Property Start As Integer
    Public Property Ends As Integer
    Public Property Strand As Strands
    Public Property Product As String
    Public Property Sequence As String

    Public Overrides Function ToString() As String
        Return $"[{Chromosome}] {GeneID}"
    End Function

    Public Shared Iterator Function GetCoverage(targetHits As NucleotideLocation(), chr As GenomeContext(Of GFF.Feature), chrSeq As ISlicer) As IEnumerable(Of PrimerCoverage)
        ' 3. 计算共同覆盖区间
        Dim minLeft = targetHits.Min(Function(h) h.left)
        Dim maxRight = targetHits.Max(Function(h) h.right)
        Dim targetChr As String = chr.contextName

        Console.WriteLine($"目标染色体: {targetChr}, 引物覆盖原始区间: {minLeft} - {maxRight} 长度约{StringFormats.Lanudry(maxRight - minLeft)}")

        ' 4. 动态延伸逻辑 (2Mb ~ 5Mb)
        Dim extendLength As Integer = 2 * ISequenceModel.MB  ' 默认2Mb
        Dim lowDensityThreshold As Double = 0.0001 ' 设定低密度阈值，例如 1 gene / 10kb = 0.0001 gene/bp

        ' 检查上下2Mb的密度
        Dim upDensity = chr.GeneDensity(Math.Max(1, minLeft - extendLength), minLeft)
        Dim downDensity = chr.GeneDensity(maxRight, maxRight + extendLength)

        If upDensity < lowDensityThreshold OrElse downDensity < lowDensityThreshold Then
            extendLength = 5 * ISequenceModel.MB  ' 放宽至5Mb
            Console.WriteLine("基因密度较低，延伸长度放宽至 5Mb")
        Else
            Console.WriteLine("基因密度正常，延伸长度为 2Mb")
        End If

        ' 5. 提取区间内基因
        Dim finalStart = Math.Max(1, minLeft - extendLength)
        Dim finalEnd = maxRight + extendLength

        Dim extractedGenes = chr.SelectByRange(finalStart, finalEnd).ToList()

        ' 6. 提取序列并输出
        For Each gene As GFF.Feature In extractedGenes
            ' 根据坐标从基因组字符串中截取序列 (注意索引从0开始)
            Dim seqLength = gene.Location.right - gene.Location.left + 1
            Dim seq = chrSeq.SliceRegionSite(gene.Location.left, seqLength)

            ' 如果是负链，需要取反向互补
            If gene.Location.Strand = Strands.Reverse Then
                seq = NucleicAcid.GetReverseComplement(seq)
            End If

            Yield New PrimerCoverage With {
                .GeneID = gene.feature,
                .Chromosome = targetChr,
                .Start = gene.Location.left,
                .Ends = gene.Location.right,
                .Strand = gene.Location.Strand,
                .Product = gene.Product,
                .Sequence = seq
            }
        Next

        Console.WriteLine($"提取完成，共找到 {extractedGenes.Count} 个基因")
    End Function
End Class
