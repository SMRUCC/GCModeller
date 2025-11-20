Imports System.IO
Imports System.Runtime.CompilerServices
Imports batch
Imports Darwinism.HPC.Parallel
Imports Microsoft.VisualBasic.Linq
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

        Dim task As New Func(Of FastaSeq, String, Double, Double, Integer, Integer, MotifMatch())(AddressOf ScanTask)
        Dim env As Argument = DarwinismEnvironment.GetEnvironmentArguments
        Dim source As FastaSeq() = regions.ToArray
        Dim result = Host.ParallelFor(Of FastaSeq, MotifMatch())(env, task, source, motif_db,
                                                                 identities_cutoff,
                                                                 minW,
                                                                 top,
                                                                 permutation).ToArray
        Return result.IteratesALL
    End Function

    <EmitStream(GetType(MotifSiteFile), Target:=GetType(MotifMatch()))>
    <EmitStream(GetType(FastaSocketFile), Target:=GetType(FastaSeq))>
    Public Function ScanTask(region As FastaSeq, motif_db As String,
                             identities_cutoff As Double,
                             minW As Double,
                             top As Integer,
                             permutation As Integer) As MotifMatch()

        Dim database = MotifDatabase.OpenReadOnly(motif_db.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
        Dim pwm As Dictionary(Of String, Probability()) = database.LoadMotifs()
        Dim result As New List(Of MotifMatch)

        For Each family_pwm As KeyValuePair(Of String, Probability()) In pwm
            Call result.AddRange(region.ScanRegion(family_pwm.Key,
                                                   family_pwm.Value,
                                                   identities_cutoff:=identities_cutoff,
                                                   minW:=minW,
                                                   top:=top,
                                                   permutation:=permutation))
        Next

        Return result.ToArray
    End Function

End Module
