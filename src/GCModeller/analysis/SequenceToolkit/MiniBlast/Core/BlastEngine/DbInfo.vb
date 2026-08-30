Imports Microsoft.VisualBasic.Linq
Imports MiniBlast.Options
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Core

    ''' <summary>数据库条目（编码 + 掩码一次生成）</summary>
    Public Class DbEntry

        Public Id As String
        Public Description As String
        Public Codes() As Int32
        Public Mask() As Boolean
        Public Length As Integer

    End Class

    Public Class DbStatistics

        Public Sequences As Long
        Public Residues As Long

    End Class

    Public Module BlastDb

        ''' <summary>数据库预处理：编码 + 低复杂度掩码 [README §一.1]</summary>
        Public Function BuildDatabase(sequences As IEnumerable(Of FastaSeq), opts As BlastOptions) As Tuple(Of List(Of DbEntry), DbStatistics)
            Dim result As New List(Of DbEntry)()
            Dim stats As New DbStatistics()

            For Each seq As FastaSeq In sequences.SafeQuery
                Dim entry As New DbEntry With {
                    .Id = seq.locus_tag,
                    .Description = seq.Title,
                    .Length = seq.SequenceData.Length
                }
                If opts.Program = "blastn" Then
                    entry.Codes = NtAlphabet.Encode(seq.SequenceData)
                    entry.Mask = If(opts.Dust,
                                    Dust.Mask(entry.Codes, opts.DustLevel, 64),
                                    New Boolean(entry.Codes.Length - 1) {})
                Else
                    entry.Codes = AaAlphabet.Encode(seq.SequenceData)
                    entry.Mask = If(opts.Seg,
                                    SegFilter.Mask(entry.Codes, 12, 2.2, 2.5),
                                    New Boolean(entry.Codes.Length - 1) {})
                End If
                result.Add(entry)
                stats.Sequences += 1
                stats.Residues += entry.Length
            Next

            Return Tuple.Create(result, stats)
        End Function
    End Module
End Namespace