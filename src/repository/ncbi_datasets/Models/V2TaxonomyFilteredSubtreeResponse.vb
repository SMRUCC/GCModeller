#Region "Microsoft.VisualBasic::eae6c7ce25448c28bfce2d1967f583df, ncbi_datasets\Models\V2TaxonomyFilteredSubtreeResponse.vb"

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

    '   Total Lines: 41
    '    Code Lines: 15 (36.59%)
    ' Comment Lines: 17 (41.46%)
    '    - Xml Docs: 70.59%
    ' 
    '   Blank Lines: 9 (21.95%)
    '     File Size: 1.18 KB


    '     Class V2TaxonomyFilteredSubtreeResponse
    ' 
    '         Properties: Edges, Errors, RootNodes, Warnings
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2TaxonomyFilteredSubtreeResponse.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyFilteredSubtreeResponse
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyFilteredSubtreeResponse

        ''' <summary>
        ''' root_nodes 属性
        ''' </summary>
        <Field("root_nodes")>
        Public Property RootNodes As List(Of Integer)

        ''' <summary>
        ''' edges 属性
        ''' </summary>
        <Field("edges")>
        Public Property Edges As Object

        ''' <summary>
        ''' warnings 属性
        ''' </summary>
        <Field("warnings")>
        Public Property Warnings As List(Of Object)

        ''' <summary>
        ''' errors 属性
        ''' </summary>
        <Field("errors")>
        Public Property Errors As List(Of Object)

    End Class

End Namespace

