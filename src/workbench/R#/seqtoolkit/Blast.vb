
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative
Imports SMRUCC.genomics.SequenceModel.FASTA

<Package("bioseq.blast")>
Module Blast

    <ExportAPI("align.smith_waterman")>
    Public Function doAlign(query As FastaSeq, ref As FastaSeq, Optional blosum As Blosum = Nothing) As SmithWaterman
        Return SmithWaterman.Align(query, ref, blosum)
    End Function

    <ExportAPI("align.needleman_wunsch")>
    Public Function RunGlobalNeedlemanWunsch(query As FastaSeq, ref As FastaSeq) As FactorValue(Of Double, GlobalAlign(Of Char)())
        Dim score As Double = 0
        Dim alignments = RunNeedlemanWunsch.RunAlign(query, ref, score).ToArray

        Return (score, alignments)
    End Function

    <ExportAPI("align.gwANI")>
    Public Function gwANIMultipleAlignment(multipleSeq As FastaFile) As DataSet()
        Return gwANI.calculate_and_output_gwani(multipleSeq)
    End Function
End Module

