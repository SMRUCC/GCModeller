#Region "Microsoft.VisualBasic::ba2cebf81642e923f57ec3add0ba0dfa, ncbi_datasets\Models\V2reportsGenomeAnnotationReportMatch.vb"

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
    '     File Size: 1.32 KB


    '     Class V2reportsGenomeAnnotationReportMatch
    ' 
    '         Properties: Annotation, Errors, Query, RowId, Warning
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsGenomeAnnotationReportMatch.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGenomeAnnotationReportMatch
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsGenomeAnnotationReportMatch

        ''' <summary>
        ''' annotation 属性
        ''' </summary>
        <Field("annotation")>
        Public Property Annotation As Object

        ''' <summary>
        ''' query 属性
        ''' </summary>
        <Field("query")>
        Public Property Query As List(Of String)

        ''' <summary>
        ''' warning 属性
        ''' </summary>
        <Field("warning")>
        Public Property Warning As Object

        ''' <summary>
        ''' errors 属性
        ''' </summary>
        <Field("errors")>
        Public Property Errors As List(Of Object)

        ''' <summary>
        ''' row_id 属性
        ''' </summary>
        <Field("row_id")>
        Public Property RowId As String

    End Class

End Namespace

