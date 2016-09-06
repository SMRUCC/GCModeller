Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TAB

    ''' <summary>
    ''' PSI-MITAB 2.7 tab-limited text format
    ''' </summary>
    Public Class v27

        <Field("#unique_identifier_A")>
        Public Property unique_identifier_A As String
        Public Property unique_identifier_B As String
        Public Property alt_identifier_A As String
        Public Property alt_identifier_B As String
        Public Property alias_A As String
        Public Property alias_B As String
        Public Property interaction_detection_method As String
        Public Property author As String
        Public Property pmid As String
        Public Property ncbi_taxid_A As String
        Public Property ncbi_taxid_B As String
        Public Property interaction_type As String
        Public Property source_database As String
        Public Property idinteraction_in_source_db As String
        Public Property confidence_score As String
        Public Property expansion_method As String
        Public Property biological_role_A As String
        Public Property biological_role_B As String
        Public Property exp_role_A As String
        Public Property exp_role_B As String
        Public Property interactor_type_A As String
        Public Property interactor_type_B As String
        Public Property xrefs_A As String
        Public Property xrefs_B As String
        Public Property xrefs_interaction As String
        Public Property annotations_A As String
        Public Property annotations_B As String
        Public Property annotations_interaction As String
        Public Property ncbi_taxid_host_organism As String
        Public Property parameters_interaction As String
        Public Property creation_date As String
        Public Property update_date As String
        Public Property checksum_A As String
        Public Property checksum_B As String
        Public Property checksum_interaction As String
        Public Property negative As String
        Public Property features_A As String
        Public Property features_B As String
        Public Property stoichiometry_A As String
        Public Property stoichiometry_B As String
        Public Property participant_identification_method_A As String
        Public Property participant_identification_method_B As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace