#Region "Microsoft.VisualBasic::a8b03b6a819d152380836121e25ff268, ncbi_datasets\Models\V2reportsSequenceDataReport.vb"

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

    '   Total Lines: 119
    '    Code Lines: 41 (34.45%)
    ' Comment Lines: 56 (47.06%)
    '    - Xml Docs: 91.07%
    ' 
    '   Blank Lines: 22 (18.49%)
    '     File Size: 3.32 KB


    '     Class V2reportsSequenceDataReport
    ' 
    '         Properties: Accession, Description, EncodedProteins, ExternalIds, Features
    '                     GeneContext, LatestUpdateDate, Length, MoleculeType, OrganismName
    '                     PublicationDate, Publications, SourceDatabase, SourceMrna, Submissions
    '                     TaxId, Units
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsSequenceDataReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceDataReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceDataReport

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' organism_name 属性
        ''' </summary>
        <Field("organism_name")>
        Public Property OrganismName As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <Field("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' units 属性
        ''' </summary>
        <Field("units")>
        Public Property Units As String

        ''' <summary>
        ''' molecule_type 属性
        ''' </summary>
        <Field("molecule_type")>
        Public Property MoleculeType As String

        ''' <summary>
        ''' source_database 属性
        ''' </summary>
        <Field("source_database")>
        Public Property SourceDatabase As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' source_mrna 属性
        ''' </summary>
        <Field("source_mrna")>
        Public Property SourceMrna As String

        ''' <summary>
        ''' encoded_proteins 属性
        ''' </summary>
        <Field("encoded_proteins")>
        Public Property EncodedProteins As List(Of Object)

        ''' <summary>
        ''' publication_date 属性
        ''' </summary>
        <Field("publication_date")>
        Public Property PublicationDate As String

        ''' <summary>
        ''' latest_update_date 属性
        ''' </summary>
        <Field("latest_update_date")>
        Public Property LatestUpdateDate As String

        ''' <summary>
        ''' gene_context 属性
        ''' </summary>
        <Field("gene_context")>
        Public Property GeneContext As Object

        ''' <summary>
        ''' features 属性
        ''' </summary>
        <Field("features")>
        Public Property Features As List(Of Object)

        ''' <summary>
        ''' external_ids 属性
        ''' </summary>
        <Field("external_ids")>
        Public Property ExternalIds As List(Of Object)

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' submissions 属性
        ''' </summary>
        <Field("submissions")>
        Public Property Submissions As List(Of Object)

        ''' <summary>
        ''' publications 属性
        ''' </summary>
        <Field("publications")>
        Public Property Publications As List(Of Object)

    End Class

End Namespace

