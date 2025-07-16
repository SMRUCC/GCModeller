Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

Namespace MarkupCompiler.BioCyc

    Public Class RegulationCompiler

        ReadOnly biocyc As Workspace

        Sub New(biocyc As Workspace)
            Me.biocyc = biocyc
        End Sub

        Public Iterator Function CreateRegulations(operons As IEnumerable(Of TranscriptUnit)) As IEnumerable(Of transcription)
            Dim site_index As Dictionary(Of String, TranscriptUnit()) = operons _
                .Where(Function(o) Not o.sites.IsNullOrEmpty) _
                .Select(Function(o)
                            Return o.sites.Select(Function(id) (id, o))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(t) t.id) _
                .ToDictionary(Function(t) t.Key,
                              Function(t)
                                  Return t.Select(Function(o) o.Item2).ToArray
                              End Function)

            For Each reg As regulation In biocyc.regulation.features
                If reg.types(0) <> "Transcription-Factor-Binding" Then
                    Continue For
                End If

                Yield New transcription With {
                    .regulator = reg.regulator,
                    .mode = reg.mode,
                    .centralDogma = site_index _
                        .TryGetValue(reg.regulatedEntity) _
                        .SafeQuery _
                        .Select(Function(t) t.id) _
                        .ToArray,
                    .note = reg.comment
                }
            Next
        End Function
    End Class
End Namespace