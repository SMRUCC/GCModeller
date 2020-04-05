Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace MarkupCompiler

    Public Class CompileTRNWorkflow : Inherits CompilerWorkflow

        Public Sub New(compiler As v2MarkupCompiler)
            MyBase.New(compiler)
        End Sub

        Friend Iterator Function getTFregulations(getId As Func(Of String, String)) As IEnumerable(Of transcription)
            Dim centralDogmas = compiler.model.Genotype.centralDogmas.ToDictionary(Function(d) d.geneID)

            For Each reg As RegulationFootprint In compiler.regulations
                Dim process As CentralDogma = centralDogmas.TryGetValue(reg.regulated)

                If process.geneID.StringEmpty Then
                    Call $"{reg.regulated} process not found!".Warning
                End If

                If reg.motif Is Nothing Then
                    reg.motif = New NucleotideLocation
                End If

                Yield New transcription With {
                    .biological_process = reg.biological_process,
                    .effector = reg.effector _
                        .StringSplit("\s*;\s*") _
                        .Select(getId) _
                        .ToArray,
                    .mode = reg.mode,
                    .regulator = reg.regulator,
                    .motif = New Motif With {
                        .family = reg.family,
                        .left = reg.motif.left,
                        .right = reg.motif.right,
                        .strand = reg.motif.Strand.GetBriefCode,
                        .sequence = reg.sequenceData,
                        .distance = reg.distance
                    },
                    .centralDogma = process.ToString
                }
            Next
        End Function
    End Class
End Namespace