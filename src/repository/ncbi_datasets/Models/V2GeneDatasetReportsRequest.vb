#Region "Microsoft.VisualBasic::38bb68c271f928ea23856573026b29e8, ncbi_datasets\Models\V2GeneDatasetReportsRequest.vb"

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

    '   Total Lines: 113
    '    Code Lines: 39 (34.51%)
    ' Comment Lines: 53 (46.90%)
    '    - Xml Docs: 90.57%
    ' 
    '   Blank Lines: 21 (18.58%)
    '     File Size: 3.16 KB


    '     Class V2GeneDatasetReportsRequest
    ' 
    '         Properties: AccessionFilter, Accessions, GeneIds, IncludeTabularHeader, LocusTags
    '                     PageSize, PageToken, Query, ReturnedContent, Sort
    '                     SymbolsForTaxon, TableFields, TableFormat, Taxon, TaxSearchSubtree
    '                     Types
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2GeneDatasetReportsRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneDatasetReportsRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2GeneDatasetReportsRequest

        ''' <summary>
        ''' returned_content 属性
        ''' </summary>
        <Field("returned_content")>
        Public Property ReturnedContent As Object

        ''' <summary>
        ''' gene_ids 属性
        ''' </summary>
        <Field("gene_ids")>
        Public Property GeneIds As List(Of Integer)

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <Field("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' symbols_for_taxon 属性
        ''' </summary>
        <Field("symbols_for_taxon")>
        Public Property SymbolsForTaxon As Object

        ''' <summary>
        ''' taxon 属性
        ''' </summary>
        <Field("taxon")>
        Public Property Taxon As String

        ''' <summary>
        ''' locus_tags 属性
        ''' </summary>
        <Field("locus_tags")>
        Public Property LocusTags As List(Of String)

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <Field("table_fields")>
        Public Property TableFields As List(Of String)

        ''' <summary>
        ''' table_format 属性
        ''' </summary>
        <Field("table_format")>
        Public Property TableFormat As String

        ''' <summary>
        ''' include_tabular_header 属性
        ''' </summary>
        <Field("include_tabular_header")>
        Public Property IncludeTabularHeader As Object

        ''' <summary>
        ''' page_size 属性
        ''' </summary>
        <Field("page_size")>
        Public Property PageSize As Integer?

        ''' <summary>
        ''' page_token 属性
        ''' </summary>
        <Field("page_token")>
        Public Property PageToken As String

        ''' <summary>
        ''' query 属性
        ''' </summary>
        <Field("query")>
        Public Property Query As String

        ''' <summary>
        ''' types 属性
        ''' </summary>
        <Field("types")>
        Public Property Types As List(Of Object)

        ''' <summary>
        ''' accession_filter 属性
        ''' </summary>
        <Field("accession_filter")>
        Public Property AccessionFilter As List(Of String)

        ''' <summary>
        ''' tax_search_subtree 属性
        ''' </summary>
        <Field("tax_search_subtree")>
        Public Property TaxSearchSubtree As Boolean?

        ''' <summary>
        ''' sort 属性
        ''' </summary>
        <Field("sort")>
        Public Property Sort As List(Of Object)

    End Class

End Namespace

