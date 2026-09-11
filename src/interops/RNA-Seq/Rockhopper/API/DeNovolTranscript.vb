' /********************************************************************************/
'
'  Rockhopper —— de novo 结果转 FASTA
'
'  由原始 `API/DeNovolTranscript.vb` 迁移而来：
'  读取 de novo 组装输出的 transcripts.txt（Sequence/Length/Expression/QValue），
'  转换为 <see cref="FastaFile"/>，便于与基因组序列做 blastn 等下游分析。
'
' /********************************************************************************/

Imports System.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace AnalysisAPI

    ''' <summary>
    ''' de novo 转录本结果（transcripts.txt）的读取与转换。
    ''' </summary>
    Public Module DeNovolTranscript

        ''' <summary>
        ''' 读取 transcripts.txt 并转换为 FASTA。
        ''' </summary>
        ''' <param name="Path">Rockhopper de novo 模式的 transcripts.txt。</param>
        Public Function LoadDocument(Path As String) As FastaFile
            Call Console.WriteLine("[Load] " & Path)

            Dim transcripts As Assembly.DeNovoTranscripts = Assembly.DeNovoTranscripts.Load(Path)
            Dim fasta As FastaSeq() =
                transcripts.Select(Function(t, i) New FastaSeq With {
                    .SequenceData = t.Sequence,
                    .Headers = New String() {$"hash={i}", $"Expression={t.Expression:F4}", $"Length={t.Length}"}
                }).ToArray()

            Return New FastaFile(fasta)
        End Function

    End Module

End Namespace
