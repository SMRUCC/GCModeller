#Region "Microsoft.VisualBasic::08c092ab3f4046de2c03391ce2b771b6, ncbi_datasets\Models\V2TaxonomyFilteredSubtreeRequest.vb"

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
    '     File Size: 1.56 KB


    '     Class V2TaxonomyFilteredSubtreeRequest
    ' 
    '         Properties: ExcludeExtinct, IncludeIncertaeSedis, Levels, RankLimits, SpecifiedLimit
    '                     Taxons
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2TaxonomyFilteredSubtreeRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyFilteredSubtreeRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyFilteredSubtreeRequest

        ''' <summary>
        ''' taxons 属性
        ''' </summary>
        <Field("taxons")>
        Public Property Taxons As List(Of String)

        ''' <summary>
        ''' specified_limit 属性
        ''' </summary>
        <Field("specified_limit")>
        Public Property SpecifiedLimit As Boolean?

        ''' <summary>
        ''' exclude_extinct 属性
        ''' </summary>
        <Field("exclude_extinct")>
        Public Property ExcludeExtinct As Boolean?

        ''' <summary>
        ''' levels 属性
        ''' </summary>
        <Field("levels")>
        Public Property Levels As Integer?

        ''' <summary>
        ''' rank_limits 属性
        ''' </summary>
        <Field("rank_limits")>
        Public Property RankLimits As List(Of Object)

        ''' <summary>
        ''' include_incertae_sedis 属性
        ''' </summary>
        <Field("include_incertae_sedis")>
        Public Property IncludeIncertaeSedis As Boolean?

    End Class

End Namespace

