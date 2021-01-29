Namespace Regprecise

    Public Class RegulatorTable

        Public Property locus_tag As String
        Public Property geneName As String
        Public Property family As String
        Public Property effector As String
        Public Property pathway As String
        Public Property biological_process As String()
        Public Property regulationMode As String
        Public Property regulog As String
        Public Property description As String
        Public Property genomeName As String

        Public Shared Function FromRegulator(tf As Regulator, Optional description$ = Nothing) As RegulatorTable
            Return New RegulatorTable With {
                .biological_process = tf.biological_process,
                .effector = tf.effector,
                .family = tf.family,
                .geneName = tf.regulator.name,
                .locus_tag = tf.locus_tag.name,
                .pathway = tf.pathway,
                .regulationMode = tf.regulationMode,
                .regulog = tf.regulog.name,
                .description = description
            }
        End Function

        Public Shared Iterator Function FromGenome(regulome As BacteriaRegulome, info As Func(Of String, String)) As IEnumerable(Of RegulatorTable)
            For Each tf As Regulator In regulome.regulome.regulators
                If tf.type <> Types.TF Then
                    Continue For
                End If

                Dim reg As RegulatorTable = FromRegulator(tf)

                reg.genomeName = regulome.genome.name
                reg.description = info(reg.locus_tag)

                Yield reg
            Next
        End Function
    End Class
End Namespace