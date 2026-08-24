#Region "Microsoft.VisualBasic::9c49359c7399fa566e168a3d33d77123, ncbi_datasets\Models\V2TaxonomyFilteredSubtreeResponseEdge.vb"

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
    '     File Size: 1.58 KB


    '     Class V2TaxonomyFilteredSubtreeResponseEdge
    ' 
    '         Properties: AssemblyCount, ChildrenStatus, CuratorCommonName, Rank, ScientificName
    '                     VisibleChildren
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2TaxonomyFilteredSubtreeResponseEdge.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyFilteredSubtreeResponseEdge
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyFilteredSubtreeResponseEdge

        ''' <summary>
        ''' visible_children 属性
        ''' </summary>
        <Field("visible_children")>
        Public Property VisibleChildren As List(Of Integer)

        ''' <summary>
        ''' children_status 属性
        ''' </summary>
        <Field("children_status")>
        Public Property ChildrenStatus As Object

        ''' <summary>
        ''' rank 属性
        ''' </summary>
        <Field("rank")>
        Public Property Rank As Object

        ''' <summary>
        ''' scientific_name 属性
        ''' </summary>
        <Field("scientific_name")>
        Public Property ScientificName As String

        ''' <summary>
        ''' curator_common_name 属性
        ''' </summary>
        <Field("curator_common_name")>
        Public Property CuratorCommonName As String

        ''' <summary>
        ''' assembly_count 属性
        ''' </summary>
        <Field("assembly_count")>
        Public Property AssemblyCount As Integer?

    End Class

End Namespace

