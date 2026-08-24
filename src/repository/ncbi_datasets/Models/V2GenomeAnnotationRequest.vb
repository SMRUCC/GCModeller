#Region "Microsoft.VisualBasic::b4c65abcb9f5577cc5be5ef76139bd7d, ncbi_datasets\Models\V2GenomeAnnotationRequest.vb"

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

    '   Total Lines: 95
    '    Code Lines: 33 (34.74%)
    ' Comment Lines: 44 (46.32%)
    '    - Xml Docs: 88.64%
    ' 
    '   Blank Lines: 18 (18.95%)
    '     File Size: 2.70 KB


    '     Class V2GenomeAnnotationRequest
    ' 
    '         Properties: Accession, AnnotationIds, GeneTypes, IncludeAnnotationType, IncludeTabularHeader
    '                     Locations, PageSize, PageToken, SearchText, Sort
    '                     Symbols, TableFields, TableFormat
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2GenomeAnnotationRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GenomeAnnotationRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2GenomeAnnotationRequest

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' annotation_ids 属性
        ''' </summary>
        <Field("annotation_ids")>
        Public Property AnnotationIds As List(Of String)

        ''' <summary>
        ''' symbols 属性
        ''' </summary>
        <Field("symbols")>
        Public Property Symbols As List(Of String)

        ''' <summary>
        ''' locations 属性
        ''' </summary>
        <Field("locations")>
        Public Property Locations As List(Of String)

        ''' <summary>
        ''' gene_types 属性
        ''' </summary>
        <Field("gene_types")>
        Public Property GeneTypes As List(Of String)

        ''' <summary>
        ''' search_text 属性
        ''' </summary>
        <Field("search_text")>
        Public Property SearchText As List(Of String)

        ''' <summary>
        ''' sort 属性
        ''' </summary>
        <Field("sort")>
        Public Property Sort As List(Of Object)

        ''' <summary>
        ''' include_annotation_type 属性
        ''' </summary>
        <Field("include_annotation_type")>
        Public Property IncludeAnnotationType As List(Of Object)

        ''' <summary>
        ''' page_size 属性
        ''' </summary>
        <Field("page_size")>
        Public Property PageSize As Integer?

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <Field("table_fields")>
        Public Property TableFields As List(Of String)

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

        ''' <summary>
        ''' page_token 属性
        ''' </summary>
        <Field("page_token")>
        Public Property PageToken As String

    End Class

End Namespace

