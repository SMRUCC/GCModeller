#Region "Microsoft.VisualBasic::e321831b1513749208df805f1d65ad76, ncbi_datasets\Models\V2TaxonomyImageMetadataResponse.vb"

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

    '   Total Lines: 65
    '    Code Lines: 23 (35.38%)
    ' Comment Lines: 29 (44.62%)
    '    - Xml Docs: 82.76%
    ' 
    '   Blank Lines: 13 (20.00%)
    '     File Size: 1.75 KB


    '     Class V2TaxonomyImageMetadataResponse
    ' 
    '         Properties: Attribution, Format, ImageSizes, License, LicenseUrl
    '                     Source, Src, TaxId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2TaxonomyImageMetadataResponse.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyImageMetadataResponse
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyImageMetadataResponse

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' src 属性
        ''' </summary>
        <Field("src")>
        Public Property Src As String

        ''' <summary>
        ''' license 属性
        ''' </summary>
        <Field("license")>
        Public Property License As String

        ''' <summary>
        ''' attribution 属性
        ''' </summary>
        <Field("attribution")>
        Public Property Attribution As String

        ''' <summary>
        ''' source 属性
        ''' </summary>
        <Field("source")>
        Public Property Source As String

        ''' <summary>
        ''' image_sizes 属性
        ''' </summary>
        <Field("image_sizes")>
        Public Property ImageSizes As List(Of Object)

        ''' <summary>
        ''' format 属性
        ''' </summary>
        <Field("format")>
        Public Property Format As String

        ''' <summary>
        ''' license_url 属性
        ''' </summary>
        <Field("license_url")>
        Public Property LicenseUrl As String

    End Class

End Namespace

