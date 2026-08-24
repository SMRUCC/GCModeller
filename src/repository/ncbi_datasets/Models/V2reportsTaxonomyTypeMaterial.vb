#Region "Microsoft.VisualBasic::18def0136d8d0264f628cc8716c5c311, ncbi_datasets\Models\V2reportsTaxonomyTypeMaterial.vb"

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


    '     Class V2reportsTaxonomyTypeMaterial
    ' 
    '         Properties: BioCollectionId, BioCollectionName, CollectionType, TypeClass, TypeStrainId
    '                     TypeStrainName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsTaxonomyTypeMaterial.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsTaxonomyTypeMaterial
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsTaxonomyTypeMaterial

        ''' <summary>
        ''' type_strain_name 属性
        ''' </summary>
        <Field("type_strain_name")>
        Public Property TypeStrainName As String

        ''' <summary>
        ''' type_strain_id 属性
        ''' </summary>
        <Field("type_strain_id")>
        Public Property TypeStrainId As String

        ''' <summary>
        ''' bio_collection_id 属性
        ''' </summary>
        <Field("bio_collection_id")>
        Public Property BioCollectionId As String

        ''' <summary>
        ''' bio_collection_name 属性
        ''' </summary>
        <Field("bio_collection_name")>
        Public Property BioCollectionName As String

        ''' <summary>
        ''' collection_type 属性
        ''' </summary>
        <Field("collection_type")>
        Public Property CollectionType As List(Of Object)

        ''' <summary>
        ''' type_class 属性
        ''' </summary>
        <Field("type_class")>
        Public Property TypeClass As String

    End Class

End Namespace

