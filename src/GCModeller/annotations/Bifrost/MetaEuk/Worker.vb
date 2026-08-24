#Region "Microsoft.VisualBasic::dfcb703e57b8081cc44d35c64139a5dc, annotations\Bifrost\MetaEuk\Worker.vb"

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

    '   Total Lines: 134
    '    Code Lines: 71 (52.99%)
    ' Comment Lines: 44 (32.84%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 19 (14.18%)
    '     File Size: 7.25 KB


    ' Module MetaEukWorker
    ' 
    '     Function: Predict
    ' 
    '     Sub: ExportResult
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' MetaEuk - Eukaryotic Gene Prediction Tool (VB.NET Implementation)
' Based on MetaEuk algorithm: homology-based exon chain optimization
'
' Algorithm Pipeline:
'   1. Six-frame translation of contigs -> candidate coding fragments
'   2. Homology search (Smith-Waterman) against reference protein database
'   3. Group by (Target, Contig, Strand) -> TCS groups
'   4. Dynamic programming: optimal exon set per TCS
'   5. Redundancy removal: cluster overlapping predictions, pick representative
'   6. Same-strand conflict resolution: keep best E-value
'   7. Output: Protein FASTA, GFF3, TSV summary
' ============================================================================

Imports System.IO
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module MetaEukWorker

    Public Function Predict(config As MetaEukConfig) As IEnumerable(Of GenePrediction)
        ' Validate required arguments
        If String.IsNullOrEmpty(config.ContigsFile) OrElse String.IsNullOrEmpty(config.ReferenceFile) Then
            Throw New InvalidDataException("[ERROR] Both --contigs and --reference are required.")
        End If

        Dim startTime = DateTime.Now

        ' ----------------------------------------------------------
        ' STEP 1: Read input FASTA files
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 1] Reading input files...")
        Dim contigs = FastaFile.Read(config.ContigsFile)
        Dim references = FastaFile.Read(config.ReferenceFile)
        Console.WriteLine($"  Contigs: {contigs.Count} sequences, total {contigs.Sum(Function(c) c.SequenceData.Length):N0} bp")
        Console.WriteLine($"  References: {references.Count} protein sequences")

        If contigs.Count = 0 Then
            Throw New InvalidDataException("[ERROR] No contigs found. Check input file.")
        End If
        If references.Count = 0 Then
            Throw New InvalidDataException("[ERROR] No reference proteins found. Check reference file.")
        End If

        ' ----------------------------------------------------------
        ' STEP 2: Six-frame translation & candidate fragment generation
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 2] Six-frame translation and candidate fragment generation...")
        Dim allFragments As New List(Of CandidateFragment)
        For Each contig In contigs
            Dim frags = SixFrameTranslator.GenerateFragments(contig, config)
            allFragments.AddRange(frags)
            If config.Verbose Then
                Console.WriteLine($"  {contig.locus_tag}: {frags.Count} candidate fragments")
            End If
        Next
        Console.WriteLine($"  Total candidate fragments: {allFragments.Count}")

        ' ----------------------------------------------------------
        ' STEP 3: Homology search (Smith-Waterman alignment)
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 3] Homology search against reference proteins...")
        Dim hits = HomologySearchEngine.SearchAll(allFragments, references.ToList, config)
        Console.WriteLine($"  Significant hits: {hits.Count}")

        If hits.Count = 0 Then
            Console.WriteLine("[WARN] No significant homology hits found. Try relaxing E-value threshold.")
            Console.WriteLine("       Writing empty output files...")
            Return Nothing
        End If

        ' ----------------------------------------------------------
        ' STEP 4: TCS grouping (Target-Contig-Strand)
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 4] Grouping hits by Target-Contig-Strand...")
        Dim tcsGroups = TCSGrouper.GroupHits(hits, config)
        Console.WriteLine($"  TCS groups: {tcsGroups.Count}")

        ' ----------------------------------------------------------
        ' STEP 5: Dynamic programming - optimal exon chain
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 5] Dynamic programming: optimal exon chain selection...")
        ExonChainOptimizer.OptimizeAll(tcsGroups, config)

        ' ----------------------------------------------------------
        ' STEP 6: Redundancy removal
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 6] Redundancy removal...")
        Dim predictions = RedundancyReducer.Reduce(tcsGroups, config)

        ' ----------------------------------------------------------
        ' STEP 7: Same-strand conflict resolution
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 7] Same-strand conflict resolution...")
        predictions = ConflictResolver.Resolve(predictions, config)

        ' ----------------------------------------------------------
        ' STEP 8: Reconstruct protein sequences
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 8] Reconstructing protein sequences...")
        ProteinReconstructor.ReconstructAll(predictions, contigs)

        Return predictions
    End Function

    Public Sub ExportResult(predictions As IReadOnlyCollection(Of GenePrediction), config As MetaEukConfig)
        ' ----------------------------------------------------------
        ' STEP 9: Write output files
        ' ----------------------------------------------------------
        Console.WriteLine("[STEP 9] Writing output files...")
        OutputWriter.WriteProteinFasta(predictions, $"{config.OutputPrefix}.faa")
        OutputWriter.WriteGFF3(predictions, $"{config.OutputPrefix}.gff3")
        OutputWriter.WriteTSV(predictions, $"{config.OutputPrefix}.tsv")

        ' ----------------------------------------------------------
        ' Summary
        ' ----------------------------------------------------------

        Console.WriteLine()
        Console.WriteLine("============================================================")
        Console.WriteLine("  PREDICTION SUMMARY")
        Console.WriteLine("============================================================")
        Console.WriteLine($"  Total gene predictions:  {predictions.Count}")
        Console.WriteLine($"  Total exons:             {predictions.Sum(Function(p) p.ExonCount)}")
        Console.WriteLine($"  Avg exons per gene:      {If(predictions.Count > 0, predictions.Average(Function(p) p.ExonCount).ToString("F1"), "N/A")}")
        Console.WriteLine($"  Avg protein length:      {If(predictions.Count > 0, predictions.Average(Function(p) If(p.ProteinSequence?.Length, 0)).ToString("F0"), "N/A")} AA")

        Console.WriteLine()
        Console.WriteLine($"  Output files:")
        Console.WriteLine($"    Protein FASTA:  {config.OutputPrefix}.faa")
        Console.WriteLine($"    Gene models:    {config.OutputPrefix}.gff3")
        Console.WriteLine($"    Summary table:  {config.OutputPrefix}.tsv")
        Console.WriteLine("============================================================")
    End Sub
End Module

