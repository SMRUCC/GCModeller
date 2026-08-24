#Region "Microsoft.VisualBasic::5a722580c112595738d1904eccd0f081, ncbi_datasets\Models\V2VirusAnnotationReportRequest.vb"

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
    '     File Size: 1.34 KB


    '     Class V2VirusAnnotationReportRequest
    ' 
    '         Properties: Filter, PageSize, PageToken, TableFields, TableFormat
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2VirusAnnotationReportRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2VirusAnnotationReportRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2VirusAnnotationReportRequest

        ''' <summary>
        ''' filter 属性
        ''' </summary>
        <Field("filter")>
        Public Property Filter As Object

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <Field("table_fields")>
        Public Property TableFields As List(Of String)

        ''' <summary>
        ''' table_format 属性
        ''' </summary>
        <Field("table_format")>
        Public Property TableFormat As String

        ''' <summary>
        ''' page_size 属性
        ''' </summary>
        <Field("page_size")>
        Public Property PageSize As Integer?

        ''' <summary>
        ''' page_token 属性
        ''' </summary>
        <Field("page_token")>
        Public Property PageToken As String

    End Class

End Namespace

