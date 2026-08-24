#Region "Microsoft.VisualBasic::f0dbc103a10b41a468c5e9828f1cfa88, ncbi_datasets\Models\V2OrthologRequest.vb"

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

    '   Total Lines: 47
    '    Code Lines: 17 (36.17%)
    ' Comment Lines: 20 (42.55%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 10 (21.28%)
    '     File Size: 1.31 KB


    '     Class V2OrthologRequest
    ' 
    '         Properties: GeneId, PageSize, PageToken, ReturnedContent, TaxonFilter
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2OrthologRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2OrthologRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2OrthologRequest

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <Field("gene_id")>
        Public Property GeneId As Integer?

        ''' <summary>
        ''' returned_content 属性
        ''' </summary>
        <Field("returned_content")>
        Public Property ReturnedContent As Object

        ''' <summary>
        ''' taxon_filter 属性
        ''' </summary>
        <Field("taxon_filter")>
        Public Property TaxonFilter As List(Of String)

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

