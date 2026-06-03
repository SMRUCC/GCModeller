
' ========================================================================
' MODULE 11: OUTPUT WRITERS
' ========================================================================

Public Class OutputWriter

    ''' <summary>Write protein sequences in FASTA format</summary>
    Public Shared Sub WriteProteinFasta(predictions As List(Of GenePrediction), filePath As String)
        Using writer As New System.IO.StreamWriter(filePath)
            For Each pred In predictions
                If String.IsNullOrEmpty(pred.ProteinSequence) Then Continue For
                writer.WriteLine($">{pred.GeneID} target={pred.TargetID} contig={pred.ContigID} strand={CStr(pred.Strand)} score={pred.TotalScore:F2} evalue={pred.BestEvalue:E2e} exons={pred.ExonCount}")
                ' Write sequence in 80-char lines
                Dim seq = pred.ProteinSequence
                Dim pos As Integer = 0
                While pos < seq.Length
                    Dim len = Math.Min(80, seq.Length - pos)
                    writer.WriteLine(seq.Substring(pos, len))
                    pos += len
                End While
            Next
        End Using
        Console.WriteLine($"[INFO] Wrote protein FASTA: {filePath}")
    End Sub

    ''' <summary>Write gene models in GFF3 format</summary>
    Public Shared Sub WriteGFF3(predictions As List(Of GenePrediction), filePath As String)
        Using writer As New System.IO.StreamWriter(filePath)
            ' GFF3 header
            writer.WriteLine("##gff-version 3")
            writer.WriteLine("##feature-ontology https://github.com/The-Sequence-Ontology/SO-Ontologies/blob/v3.1/Features.md")
            writer.WriteLine("##source metaeuk-vb")

            For Each pred In predictions
                Dim strandChar = If(pred.Strand = StrandOrientation.Plus, "+"c, "-"c)

                ' Gene feature
                writer.WriteLine($"{pred.ContigID}{vbTab}metaeuk-vb{vbTab}gene{vbTab}{pred.DnaStart}{vbTab}{pred.DnaEnd}{vbTab}{pred.TotalScore:F2}{vbTab}{strandChar}{vbTab}.{vbTab}ID={pred.GeneID};target={pred.TargetID};evalue={pred.BestEvalue:E2e}")

                ' mRNA feature
                Dim mrnaID = $"{pred.GeneID}.mRNA"
                writer.WriteLine($"{pred.ContigID}{vbTab}metaeuk-vb{vbTab}mRNA{vbTab}{pred.DnaStart}{vbTab}{pred.DnaEnd}{vbTab}{pred.TotalScore:F2}{vbTab}{strandChar}{vbTab}.{vbTab}ID={mrnaID};Parent={pred.GeneID}")

                ' Exon features
                For exonIdx = 0 To pred.Exons.Count - 1
                    Dim exon = pred.Exons(exonIdx)
                    Dim exonID = $"{pred.GeneID}.exon{exonIdx + 1}"
                    writer.WriteLine($"{pred.ContigID}{vbTab}metaeuk-vb{vbTab}exon{vbTab}{exon.DnaStart}{vbTab}{exon.DnaEnd}{vbTab}{exon.Score:F2}{vbTab}{strandChar}{vbTab}.{vbTab}ID={exonID};Parent={mrnaID};target_align={exon.Hit.AlignStartTarget}-{exon.Hit.AlignEndTarget}")
                Next

                ' CDS features
                For exonIdx = 0 To pred.Exons.Count - 1
                    Dim exon = pred.Exons(exonIdx)
                    Dim cdsID = $"{pred.GeneID}.cds{exonIdx + 1}"
                    writer.WriteLine($"{pred.ContigID}{vbTab}metaeuk-vb{vbTab}CDS{vbTab}{exon.DnaStart}{vbTab}{exon.DnaEnd}{vbTab}{exon.Score:F2}{vbTab}{strandChar}{vbTab}0{vbTab}ID={cdsID};Parent={mrnaID}")
                Next
            Next
        End Using
        Console.WriteLine($"[INFO] Wrote GFF3: {filePath}")
    End Sub

    ''' <summary>Write summary in TSV format</summary>
    Public Shared Sub WriteTSV(predictions As List(Of GenePrediction), filePath As String)
        Using writer As New System.IO.StreamWriter(filePath)
            ' Header
            writer.WriteLine("gene_id" & vbTab & "contig" & vbTab & "strand" & vbTab &
                             "start" & vbTab & "end" & vbTab & "length_bp" & vbTab &
                             "exon_count" & vbTab & "target_id" & vbTab &
                             "total_score" & vbTab & "best_evalue" & vbTab &
                             "protein_length")

            For Each pred In predictions
                Dim protLen = If(pred.ProteinSequence IsNot Nothing, pred.ProteinSequence.Length, 0)
                writer.WriteLine($"{pred.GeneID}{vbTab}{pred.ContigID}{vbTab}{CStr(pred.Strand)}{vbTab}" &
                                 $"{pred.DnaStart}{vbTab}{pred.DnaEnd}{vbTab}{pred.DnaEnd - pred.DnaStart + 1}{vbTab}" &
                                 $"{pred.ExonCount}{vbTab}{pred.TargetID}{vbTab}" &
                                 $"{pred.TotalScore:F2}{vbTab}{pred.BestEvalue:E2e}{vbTab}{protLen}")
            Next
        End Using
        Console.WriteLine($"[INFO] Wrote TSV summary: {filePath}")
    End Sub

End Class
