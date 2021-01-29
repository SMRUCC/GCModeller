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

    End Class
End Namespace