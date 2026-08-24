#Region "Microsoft.VisualBasic::acb1e5ba5b26512a6a411a0e422fa05f, ncbi_datasets\Models\V2GeneDatasetRequest.vb"

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

    '   Total Lines: 71
    '    Code Lines: 25 (35.21%)
    ' Comment Lines: 32 (45.07%)
    '    - Xml Docs: 84.38%
    ' 
    '   Blank Lines: 14 (19.72%)
    '     File Size: 2.10 KB


    '     Class V2GeneDatasetRequest
    ' 
    '         Properties: AccessionFilter, AuxReport, FastaFilter, GeneIds, IncludeAnnotationType
    '                     ReturnedContent, TableFields, TableReportType, TabularReports
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2GeneDatasetRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneDatasetRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2GeneDatasetRequest

        ''' <summary>
        ''' gene_ids 属性
        ''' </summary>
        <Field("gene_ids")>
        Public Property GeneIds As List(Of Integer)

        ''' <summary>
        ''' include_annotation_type 属性
        ''' </summary>
        <Field("include_annotation_type")>
        Public Property IncludeAnnotationType As List(Of Object)

        ''' <summary>
        ''' returned_content 属性
        ''' </summary>
        <Field("returned_content")>
        Public Property ReturnedContent As Object

        ''' <summary>
        ''' fasta_filter 属性
        ''' </summary>
        <Field("fasta_filter")>
        Public Property FastaFilter As List(Of String)

        ''' <summary>
        ''' accession_filter 属性
        ''' </summary>
        <Field("accession_filter")>
        Public Property AccessionFilter As List(Of String)

        ''' <summary>
        ''' aux_report 属性
        ''' </summary>
        <Field("aux_report")>
        Public Property AuxReport As List(Of Object)

        ''' <summary>
        ''' tabular_reports 属性
        ''' </summary>
        <Field("tabular_reports")>
        Public Property TabularReports As List(Of Object)

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <Field("table_fields")>
        Public Property TableFields As List(Of String)

        ''' <summary>
        ''' table_report_type 属性
        ''' </summary>
        <Field("table_report_type")>
        Public Property TableReportType As Object

    End Class

End Namespace

