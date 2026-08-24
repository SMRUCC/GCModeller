#Region "Microsoft.VisualBasic::11edea611856eb57e47105dceddde2dd, ncbi_datasets\Models\V2TaxonomyLinksResponse.vb"

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

    '   Total Lines: 59
    '    Code Lines: 21 (35.59%)
    ' Comment Lines: 26 (44.07%)
    '    - Xml Docs: 80.77%
    ' 
    '   Blank Lines: 12 (20.34%)
    '     File Size: 1.74 KB


    '     Class V2TaxonomyLinksResponse
    ' 
    '         Properties: EncyclopediaOfLife, GenericLinks, GlobalBiodiversityInformationFacility, Inaturalist, TaxId
    '                     Viralzone, Wikipedia
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2TaxonomyLinksResponse.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyLinksResponse
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyLinksResponse

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' encyclopedia_of_life 属性
        ''' </summary>
        <Field("encyclopedia_of_life")>
        Public Property EncyclopediaOfLife As String

        ''' <summary>
        ''' global_biodiversity_information_facility 属性
        ''' </summary>
        <Field("global_biodiversity_information_facility")>
        Public Property GlobalBiodiversityInformationFacility As String

        ''' <summary>
        ''' inaturalist 属性
        ''' </summary>
        <Field("inaturalist")>
        Public Property Inaturalist As String

        ''' <summary>
        ''' viralzone 属性
        ''' </summary>
        <Field("viralzone")>
        Public Property Viralzone As String

        ''' <summary>
        ''' wikipedia 属性
        ''' </summary>
        <Field("wikipedia")>
        Public Property Wikipedia As String

        ''' <summary>
        ''' generic_links 属性
        ''' </summary>
        <Field("generic_links")>
        Public Property GenericLinks As List(Of Object)

    End Class

End Namespace

