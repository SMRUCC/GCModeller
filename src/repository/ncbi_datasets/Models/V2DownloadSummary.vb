#Region "Microsoft.VisualBasic::965fe0b48114f5af2750f74999626a7a, ncbi_datasets\Models\V2DownloadSummary.vb"

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
    '     File Size: 1.82 KB


    '     Class V2DownloadSummary
    ' 
    '         Properties: AssemblyCount, AvailableFiles, Dehydrated, Errors, Hydrated
    '                     Messages, RecordCount, ResourceUpdatedOn
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2DownloadSummary.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2DownloadSummary
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2DownloadSummary

        ''' <summary>
        ''' record_count 属性
        ''' </summary>
        <Field("record_count")>
        Public Property RecordCount As Integer?

        ''' <summary>
        ''' assembly_count 属性
        ''' </summary>
        <Field("assembly_count")>
        Public Property AssemblyCount As Integer?

        ''' <summary>
        ''' resource_updated_on 属性
        ''' </summary>
        <Field("resource_updated_on")>
        Public Property ResourceUpdatedOn As DateTime?

        ''' <summary>
        ''' hydrated 属性
        ''' </summary>
        <Field("hydrated")>
        Public Property Hydrated As Object

        ''' <summary>
        ''' dehydrated 属性
        ''' </summary>
        <Field("dehydrated")>
        Public Property Dehydrated As Object

        ''' <summary>
        ''' errors 属性
        ''' </summary>
        <Field("errors")>
        Public Property Errors As List(Of Object)

        ''' <summary>
        ''' messages 属性
        ''' </summary>
        <Field("messages")>
        Public Property Messages As List(Of Object)

        ''' <summary>
        ''' available_files 属性
        ''' </summary>
        <Field("available_files")>
        Public Property AvailableFiles As Object

    End Class

End Namespace

