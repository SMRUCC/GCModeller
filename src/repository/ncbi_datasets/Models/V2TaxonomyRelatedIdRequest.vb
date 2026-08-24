#Region "Microsoft.VisualBasic::bd9f18a3553fce5b12ab8d2c379d51f2, ncbi_datasets\Models\V2TaxonomyRelatedIdRequest.vb"

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

    '   Total Lines: 53
    '    Code Lines: 19 (35.85%)
    ' Comment Lines: 23 (43.40%)
    '    - Xml Docs: 78.26%
    ' 
    '   Blank Lines: 11 (20.75%)
    '     File Size: 1.49 KB


    '     Class V2TaxonomyRelatedIdRequest
    ' 
    '         Properties: IncludeLineage, IncludeSubtree, PageSize, PageToken, Ranks
    '                     TaxId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2TaxonomyRelatedIdRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyRelatedIdRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyRelatedIdRequest

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' include_lineage 属性
        ''' </summary>
        <Field("include_lineage")>
        Public Property IncludeLineage As Boolean?

        ''' <summary>
        ''' include_subtree 属性
        ''' </summary>
        <Field("include_subtree")>
        Public Property IncludeSubtree As Boolean?

        ''' <summary>
        ''' ranks 属性
        ''' </summary>
        <Field("ranks")>
        Public Property Ranks As List(Of Object)

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

    End Class

End Namespace

