#Region "Microsoft.VisualBasic::bfcfacd25e67de8b4373ab78228da970, ncbi_datasets\Models\V2reportsAssemblyDataReportPage.vb"

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
    '     File Size: 1.36 KB


    '     Class V2reportsAssemblyDataReportPage
    ' 
    '         Properties: ContentType, Messages, NextPageToken, Reports, TotalCount
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsAssemblyDataReportPage.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAssemblyDataReportPage
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAssemblyDataReportPage

        ''' <summary>
        ''' reports 属性
        ''' </summary>
        <Field("reports")>
        Public Property Reports As List(Of Object)

        ''' <summary>
        ''' content_type 属性
        ''' </summary>
        <Field("content_type")>
        Public Property ContentType As Object

        ''' <summary>
        ''' total_count 属性
        ''' </summary>
        <Field("total_count")>
        Public Property TotalCount As Integer?

        ''' <summary>
        ''' next_page_token 属性
        ''' </summary>
        <Field("next_page_token")>
        Public Property NextPageToken As String

        ''' <summary>
        ''' messages 属性
        ''' </summary>
        <Field("messages")>
        Public Property Messages As List(Of Object)

    End Class

End Namespace

