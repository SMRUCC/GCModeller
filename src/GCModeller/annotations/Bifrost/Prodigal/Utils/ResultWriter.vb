Imports System.Text
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 结果输出工具
''' </summary>
Public Class ResultWriter

    ''' <summary>
    ''' 输出GFF3格式基因预测结果
    ''' </summary>
    Public Shared Sub WriteGff3(results As IReadOnlyCollection(Of PredictionResult), filePath As String)
        Dim geneList As New List(Of Feature)

        For Each result In results
            For Each gene As PredictedGene In result.Genes
                Dim partialTag = If(String.IsNullOrEmpty(gene.PartialType), ".", gene.PartialType)

                Call geneList.Add(New Feature With {
                    .ID = "gene_" & gene.GeneIndex,
                    .strand = gene.Strand.GetStrands,
                    .left = gene.Start,
                    .right = gene.End,
                    .score = gene.TotalScore,
                    .frame = gene.Frame + 1,
                    .seqname = result.SeqId,
                    .feature = "CDS",
                    .source = "Prodigal",
                    .Product = "-",
                    .comments = "-",
                    .COG = "-",
                    .attributes = New Dictionary(Of String, String) From {
                        {"start_codon", gene.StartCodon},
                        {"rbs_motif", gene.RbsMotif},
                        {"cscore", gene.CodingScore},
                        {"sscore", gene.StartScore},
                        {"rscore", gene.RbsScore},
                        {"tscore", gene.TypeScore},
                        {"uscore", gene.UpstreamScore},
                        {"partial", partialTag}
                    }
                })
            Next
        Next

        Dim gff As New GFFTable With {.features = geneList.ToArray, .[date] = Now.ToString, .GffVersion = 3, .processor = "Prodigal"}

        Call gff.Save(filePath)
    End Sub

    ''' <summary>
    ''' 输出蛋白质FASTA文件
    ''' </summary>
    Public Shared Sub WriteProteinFasta(results As IReadOnlyCollection(Of PredictionResult), filePath As String)
        Dim seqs As New List(Of FastaSeq)
        For Each result In results
            For Each gene As PredictedGene In result.Genes
                Call seqs.Add(gene.CreateProteinFasta(result.SeqId))
            Next
        Next

        Call New FastaFile(seqs).Save(filePath)
    End Sub

    ''' <summary>
    ''' 输出核苷酸FASTA文件
    ''' </summary>
    Public Shared Sub WriteNucleotideFasta(results As IReadOnlyCollection(Of PredictionResult), filePath As String)
        Dim seqs As New List(Of FastaSeq)
        For Each result In results
            For Each gene As PredictedGene In result.Genes
                Call seqs.Add(gene.CreateGeneFasta(result.SeqId))
            Next
        Next

        Call New FastaFile(seqs).Save(filePath)
    End Sub

    ''' <summary>
    ''' 输出详细得分表（制表符分隔）
    ''' </summary>
    Public Shared Sub WriteScoreTable(results As IReadOnlyCollection(Of PredictionResult), filePath As String)
        Using writer As New System.IO.StreamWriter(filePath, False, Encoding.UTF8)
            writer.WriteLine($"SeqID{vbTab}GeneIndex{vbTab}Start{vbTab}End{vbTab}Strand{vbTab}Length{vbTab}" &
                $"StartCodon{vbTab}StopCodon{vbTab}TotalScore{vbTab}CodingScore{vbTab}StartScore{vbTab}" &
                $"RbsScore{vbTab}TypeScore{vbTab}UpstreamScore{vbTab}RbsMotif{vbTab}RbsSpacing{vbTab}Partial")
            For Each result In results
                For Each gene As PredictedGene In result.Genes
                    writer.WriteLine($"{result.SeqId}{vbTab}{gene.GeneIndex}{vbTab}{gene.Start}{vbTab}{gene.End}{vbTab}" &
                        $"{gene.Strand}{vbTab}{gene.Length}{vbTab}{gene.StartCodon}{vbTab}{gene.StopCodon}{vbTab}" &
                        $"{gene.TotalScore:F4}{vbTab}{gene.CodingScore:F4}{vbTab}{gene.StartScore:F4}{vbTab}" &
                        $"{gene.RbsScore:F4}{vbTab}{gene.TypeScore:F4}{vbTab}{gene.UpstreamScore:F4}{vbTab}" &
                        $"{gene.RbsMotif}{vbTab}{gene.RbsSpacing}{vbTab}{gene.PartialType}")
                Next
            Next
        End Using
    End Sub

    ''' <summary>
    ''' 控制台输出预测结果摘要
    ''' </summary>
    Public Shared Sub PrintSummary(results As List(Of PredictionResult))
        Console.WriteLine()
        Console.WriteLine("="c, 70)
        Console.WriteLine("  Prodigal VB.NET 基因预测结果摘要")
        Console.WriteLine("="c, 70)

        Dim totalGenes As Integer = 0
        For Each result In results
            Console.WriteLine($"  序列: {result.SeqId}  (长度: {result.SeqLength:N0} bp)")
            Console.WriteLine($"    预测基因数: {result.Genes.Count}")
            If result.Genes.Count > 0 Then
                Dim avgLen = result.Genes.Average(Function(g) g.Length)
                Dim avgScore = result.Genes.Average(Function(g) g.TotalScore)
                Console.WriteLine($"    平均基因长度: {avgLen:F0} bp")
                Console.WriteLine($"    平均得分: {avgScore:F2}")
            End If
            totalGenes += result.Genes.Count
            Console.WriteLine()
        Next

        Console.WriteLine($"  总计预测基因数: {totalGenes}")
        Console.WriteLine("="c, 70)
        Console.WriteLine()
    End Sub

End Class

