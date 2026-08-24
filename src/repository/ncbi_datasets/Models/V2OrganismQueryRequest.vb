#Region "Microsoft.VisualBasic::ecc49a7ba373c6904c5be3446d377276, ncbi_datasets\Models\V2OrganismQueryRequest.vb"

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
    '     File Size: 1.37 KB


    '     Class V2OrganismQueryRequest
    ' 
    '         Properties: ExactMatch, OrganismQuery, TaxonQuery, TaxonResourceFilter, TaxRankFilter
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2OrganismQueryRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2OrganismQueryRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2OrganismQueryRequest

        ''' <summary>
        ''' organism_query 属性
        ''' </summary>
        <Field("organism_query")>
        Public Property OrganismQuery As String

        ''' <summary>
        ''' taxon_query 属性
        ''' </summary>
        <Field("taxon_query")>
        Public Property TaxonQuery As String

        ''' <summary>
        ''' tax_rank_filter 属性
        ''' </summary>
        <Field("tax_rank_filter")>
        Public Property TaxRankFilter As Object

        ''' <summary>
        ''' taxon_resource_filter 属性
        ''' </summary>
        <Field("taxon_resource_filter")>
        Public Property TaxonResourceFilter As Object

        ''' <summary>
        ''' exact_match 属性
        ''' </summary>
        <Field("exact_match")>
        Public Property ExactMatch As Boolean?

    End Class

End Namespace

