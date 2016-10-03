#Region "Microsoft.VisualBasic::07439946f8ed5fa41dba2277efc9ca20, ..\GCModeller\foundation\PSICQUIC\psidev\TAB\v27.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
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
