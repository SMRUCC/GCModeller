#Region "Microsoft.VisualBasic::75bc7ccabd2ac3abef185d878a04432d, ncbi_datasets\Models\V2reportsTaxonomyNamesDescriptor.vb"

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

    '   Total Lines: 77
    '    Code Lines: 27 (35.06%)
    ' Comment Lines: 35 (45.45%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 15 (19.48%)
    '     File Size: 2.29 KB


    '     Class V2reportsTaxonomyNamesDescriptor
    ' 
    '         Properties: Citations, CuratorCommonName, CurrentScientificName, CurrentScientificNameIsFormal, GeneralNotes
    '                     GroupName, LinksFromType, OtherCommonNames, Rank, TaxId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsTaxonomyNamesDescriptor.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsTaxonomyNamesDescriptor
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsTaxonomyNamesDescriptor

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As String

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
        ''' group_name 属性
        ''' </summary>
        <Field("group_name")>
        Public Property GroupName As String

        ''' <summary>
        ''' curator_common_name 属性
        ''' </summary>
        <Field("curator_common_name")>
        Public Property CuratorCommonName As String

        ''' <summary>
        ''' other_common_names 属性
        ''' </summary>
        <Field("other_common_names")>
        Public Property OtherCommonNames As List(Of String)

        ''' <summary>
        ''' general_notes 属性
        ''' </summary>
        <Field("general_notes")>
        Public Property GeneralNotes As List(Of String)

        ''' <summary>
        ''' links_from_type 属性
        ''' </summary>
        <Field("links_from_type")>
        Public Property LinksFromType As String

        ''' <summary>
        ''' citations 属性
        ''' </summary>
        <Field("citations")>
        Public Property Citations As List(Of Object)

        ''' <summary>
        ''' current_scientific_name_is_formal 属性
        ''' </summary>
        <Field("current_scientific_name_is_formal")>
        Public Property CurrentScientificNameIsFormal As Boolean?

    End Class

End Namespace

