#Region "Microsoft.VisualBasic::e59916eed92d430fd299e711c0314349, annotations\Bifrost\Prodigal\Utils\ResultWriter.vb"

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

    '   Total Lines: 140
    '    Code Lines: 104 (74.29%)
    ' Comment Lines: 19 (13.57%)
    '    - Xml Docs: 94.74%
    ' 
    '   Blank Lines: 17 (12.14%)
    '     File Size: 6.15 KB


    ' Class ResultWriter
    ' 
    '     Function: CastToGff, GetGeneSequences, GetProteinSequences
    ' 
    '     Sub: PrintSummary, WriteGff3, WriteNucleotideFasta, WriteProteinFasta, WriteScoreTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 结果输出工具
''' </summary>
Public Class ResultWriter

    Public Shared Function CastToGff(results As IReadOnlyCollection(Of PredictionResult)) As GFFTable
        Dim geneList As New List(Of Feature)

        For Each result As PredictionResult In results
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

        Return New GFFTable With {
            .features = geneList.ToArray,
            .[date] = Now.ToString,
            .GffVersion = 3,
            .processor = "Prodigal"
        }
    End Function

    ''' <summary>
    ''' 输出GFF3格式基因预测结果
    ''' </summary>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub WriteGff3(results As IReadOnlyCollection(Of PredictionResult), filePath As String)
        Call CastToGff(results).Save(filePath)
    End Sub

    Public Shared Iterator Function GetProteinSequences(results As IReadOnlyCollection(Of PredictionResult)) As IEnumerable(Of FastaSeq)
        For Each result As PredictionResult In results
            For Each gene As PredictedGene In result.Genes
                Yield gene.CreateProteinFasta(result.SeqId)
            Next
        Next
    End Function

    ''' <summary>
    ''' 输出蛋白质FASTA文件
    ''' </summary>
    Public Shared Sub WriteProteinFasta(results As IReadOnlyCollection(Of PredictionResult), filePath As String)
        Call New FastaFile(GetProteinSequences(results)).Save(filePath)
    End Sub

    Public Shared Iterator Function GetGeneSequences(results As IReadOnlyCollection(Of PredictionResult)) As IEnumerable(Of FastaSeq)
        For Each result As PredictionResult In results
            For Each gene As PredictedGene In result.Genes
                Yield gene.CreateGeneFasta(result.SeqId)
            Next
        Next
    End Function

    ''' <summary>
    ''' 输出核苷酸FASTA文件
    ''' </summary>
    Public Shared Sub WriteNucleotideFasta(results As IReadOnlyCollection(Of PredictionResult), filePath As String)
        Call New FastaFile(GetGeneSequences(results)).Save(filePath)
    End Sub

    ''' <summary>
    ''' 输出详细得分表（制表符分隔）
    ''' </summary>
    Public Shared Sub WriteScoreTable(results As IReadOnlyCollection(Of PredictionResult), filePath As String)
        Using writer As New System.IO.StreamWriter(filePath, False, Encoding.UTF8)
            writer.WriteLine($"SeqID{vbTab}GeneIndex{vbTab}Start{vbTab}End{vbTab}Strand{vbTab}Length{vbTab}" &
                $"StartCodon{vbTab}StopCodon{vbTab}TotalScore{vbTab}CodingScore{vbTab}StartScore{vbTab}" &
                $"RbsScore{vbTab}TypeScore{vbTab}UpstreamScore{vbTab}RbsMotif{vbTab}RbsSpacing{vbTab}Partial")

            For Each gene As GeneScore In GeneScore.ScoreTable(results)
                Call writer.WriteLine($"{gene.seq_id}{vbTab}{gene.gene_index}{vbTab}{gene.start}{vbTab}{gene.end}{vbTab}" &
                    $"{gene.strand.Description}{vbTab}{gene.length}{vbTab}{gene.start_codon}{vbTab}{gene.stop_codon}{vbTab}" &
                    $"{gene.total_score:F4}{vbTab}{gene.coding_score:F4}{vbTab}{gene.start_score:F4}{vbTab}" &
                    $"{gene.rbs_score:F4}{vbTab}{gene.type_score:F4}{vbTab}{gene.upstream_score:F4}{vbTab}" &
                    $"{gene.rbs_motif}{vbTab}{gene.rbs_spacing}{vbTab}{gene.partial_type}")
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
        For Each result As PredictionResult In results
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


