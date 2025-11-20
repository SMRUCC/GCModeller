Imports System.IO
Imports System.Runtime.CompilerServices
Imports Darwinism.HPC.Parallel
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module ScannerTask

    <Extension>
    Public Function ScanSites(regions As IEnumerable(Of FastaSeq), motif_db As String,
                              Optional n_threads As Integer = 8,
                              Optional identities_cutoff As Double = 0.8,
                              Optional minW As Double = 0.85,
                              Optional top As Integer = 3,
                              Optional permutation As Integer = 2500) As IEnumerable(Of MotifMatch)

    End Function

    <EmitStream(GetType(MotifSiteFile), Target:=GetType(MotifMatch()))>
    Public Function ScanTask(region As FastaSeq, motif_db As String,
                             n_threads As Integer,
                             identities_cutoff As Double,
                             minW As Double,
                             top As Integer,
                             permutation As Integer) As MotifMatch()

        Dim pwm = MotifDatabase.OpenReadOnly(motif_db.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
    End Function

End Module
