#Region "Microsoft.VisualBasic::bf57b8ec918e0fa4d68d7c3cbc096e6d, ncbi_datasets\Models\V2reportsOrganelleDataReports.vb"

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
    '     File Size: 1.19 KB


    '     Class V2reportsOrganelleDataReports
    ' 
    '         Properties: Messages, NextPageToken, Reports, TotalCount
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsOrganelleDataReports.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsOrganelleDataReports
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsOrganelleDataReports

        ''' <summary>
        ''' messages 属性
        ''' </summary>
        <Field("messages")>
        Public Property Messages As List(Of Object)

        ''' <summary>
        ''' reports 属性
        ''' </summary>
        <Field("reports")>
        Public Property Reports As List(Of Object)

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

    End Class

End Namespace

