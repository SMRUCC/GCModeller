#Region "Microsoft.VisualBasic::3979f63822f5d472c02a7e38c244b165, ncbi_datasets\Models\V2reportsTaxonomyNode.vb"

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

    '   Total Lines: 107
    '    Code Lines: 37 (34.58%)
    ' Comment Lines: 50 (46.73%)
    '    - Xml Docs: 90.00%
    ' 
    '   Blank Lines: 20 (18.69%)
    '     File Size: 3.04 KB


    '     Class V2reportsTaxonomyNode
    ' 
    '         Properties: Basionym, Children, Classification, Counts, CuratorCommonName
    '                     CurrentScientificName, CurrentScientificNameIsFormal, Extinct, GenomicMoltype, GroupName
    '                     HasTypeMaterial, Parents, Rank, SecondaryTaxIds, TaxId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsTaxonomyNode.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsTaxonomyNode
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsTaxonomyNode

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' rank 属性
        ''' </summary>
        <Field("rank")>
        Public Property Rank As Object

        ''' <summary>
        ''' current_scientific_name 属性
        ''' </summary>
        <Field("current_scientific_name")>
        Public Property CurrentScientificName As Object

        ''' <summary>
        ''' basionym 属性
        ''' </summary>
        <Field("basionym")>
        Public Property Basionym As Object

        ''' <summary>
        ''' curator_common_name 属性
        ''' </summary>
        <Field("curator_common_name")>
        Public Property CuratorCommonName As String

        ''' <summary>
        ''' group_name 属性
        ''' </summary>
        <Field("group_name")>
        Public Property GroupName As String

        ''' <summary>
        ''' has_type_material 属性
        ''' </summary>
        <Field("has_type_material")>
        Public Property HasTypeMaterial As Boolean?

        ''' <summary>
        ''' classification 属性
        ''' </summary>
        <Field("classification")>
        Public Property Classification As Object

        ''' <summary>
        ''' parents 属性
        ''' </summary>
        <Field("parents")>
        Public Property Parents As List(Of Integer)

        ''' <summary>
        ''' children 属性
        ''' </summary>
        <Field("children")>
        Public Property Children As List(Of Integer)

        ''' <summary>
        ''' counts 属性
        ''' </summary>
        <Field("counts")>
        Public Property Counts As List(Of Object)

        ''' <summary>
        ''' genomic_moltype 属性
        ''' </summary>
        <Field("genomic_moltype")>
        Public Property GenomicMoltype As String

        ''' <summary>
        ''' current_scientific_name_is_formal 属性
        ''' </summary>
        <Field("current_scientific_name_is_formal")>
        Public Property CurrentScientificNameIsFormal As Boolean?

        ''' <summary>
        ''' secondary_tax_ids 属性
        ''' </summary>
        <Field("secondary_tax_ids")>
        Public Property SecondaryTaxIds As List(Of Integer)

        ''' <summary>
        ''' extinct 属性
        ''' </summary>
        <Field("extinct")>
        Public Property Extinct As Boolean?

    End Class

End Namespace

