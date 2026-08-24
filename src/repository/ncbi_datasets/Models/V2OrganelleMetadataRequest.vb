#Region "Microsoft.VisualBasic::1604625408c077d34849cb22e334d56c, ncbi_datasets\Models\V2OrganelleMetadataRequest.vb"

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

    '   Total Lines: 89
    '    Code Lines: 31 (34.83%)
    ' Comment Lines: 41 (46.07%)
    '    - Xml Docs: 87.80%
    ' 
    '   Blank Lines: 17 (19.10%)
    '     File Size: 2.55 KB


    '     Class V2OrganelleMetadataRequest
    ' 
    '         Properties: Accessions, FirstReleaseDate, IncludeTabularHeader, LastReleaseDate, OrganelleTypes
    '                     PageSize, PageToken, ReturnedContent, Sort, TableFormat
    '                     TaxExactMatch, Taxons
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2OrganelleMetadataRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2OrganelleMetadataRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2OrganelleMetadataRequest

        ''' <summary>
        ''' taxons 属性
        ''' </summary>
        <Field("taxons")>
        Public Property Taxons As List(Of String)

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <Field("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' organelle_types 属性
        ''' </summary>
        <Field("organelle_types")>
        Public Property OrganelleTypes As List(Of Object)

        ''' <summary>
        ''' first_release_date 属性
        ''' </summary>
        <Field("first_release_date")>
        Public Property FirstReleaseDate As DateTime?

        ''' <summary>
        ''' last_release_date 属性
        ''' </summary>
        <Field("last_release_date")>
        Public Property LastReleaseDate As DateTime?

        ''' <summary>
        ''' tax_exact_match 属性
        ''' </summary>
        <Field("tax_exact_match")>
        Public Property TaxExactMatch As Boolean?

        ''' <summary>
        ''' sort 属性
        ''' </summary>
        <Field("sort")>
        Public Property Sort As List(Of Object)

        ''' <summary>
        ''' returned_content 属性
        ''' </summary>
        <Field("returned_content")>
        Public Property ReturnedContent As Object

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
        ''' table_format 属性
        ''' </summary>
        <Field("table_format")>
        Public Property TableFormat As Object

        ''' <summary>
        ''' include_tabular_header 属性
        ''' </summary>
        <Field("include_tabular_header")>
        Public Property IncludeTabularHeader As Object

    End Class

End Namespace

