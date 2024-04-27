#Region "Microsoft.VisualBasic::07439946f8ed5fa41dba2277efc9ca20, G:/GCModeller/src/GCModeller/foundation/PSICQUIC/psidev//TAB/v27.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 59
    '    Code Lines: 52
    ' Comment Lines: 3
    '   Blank Lines: 4
    '     File Size: 2.58 KB


    '     Class v27
    ' 
    '         Properties: alias_A, alias_B, alt_identifier_A, alt_identifier_B, annotations_A
    '                     annotations_B, annotations_interaction, author, biological_role_A, biological_role_B
    '                     checksum_A, checksum_B, checksum_interaction, confidence_score, creation_date
    '                     exp_role_A, exp_role_B, expansion_method, features_A, features_B
    '                     idinteraction_in_source_db, interaction_detection_method, interaction_type, interactor_type_A, interactor_type_B
    '                     ncbi_taxid_A, ncbi_taxid_B, ncbi_taxid_host_organism, negative, parameters_interaction
    '                     participant_identification_method_A, participant_identification_method_B, pmid, source_database, stoichiometry_A
    '                     stoichiometry_B, unique_identifier_A, unique_identifier_B, update_date, xrefs_A
    '                     xrefs_B, xrefs_interaction
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

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
