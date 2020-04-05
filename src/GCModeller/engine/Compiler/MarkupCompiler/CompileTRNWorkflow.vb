Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace MarkupCompiler

    Public Class CompileTRNWorkflow : Inherits CompilerWorkflow

        Public Sub New(compiler As v2MarkupCompiler)
            MyBase.New(compiler)
        End Sub

        Private Function getIdMapper() As Func(Of String, String)
            Dim allCompounds = compiler.KEGG.GetCompounds.Compounds
            ' lower name -> cid mapping
            Dim mapperIndex As New Dictionary(Of String, String)

            Return Function(name)
                       name = LCase(name).Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF, "-"c, ";"c)

                       If mapperIndex.ContainsKey(name) Then
                           Return mapperIndex(name)
                       Else
                           Return Nothing
                       End If
                   End Function
        End Function

        Friend Iterator Function getTFregulations() As IEnumerable(Of transcription)
            Dim centralDogmas = compiler.model.Genotype.centralDogmas.ToDictionary(Function(d) d.geneID)
            Dim getId As Func(Of String, String) = getIdMapper()

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