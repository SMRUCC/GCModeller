#Region "Microsoft.VisualBasic::51fe6038a6f0f567014ccf14564d967b, ncbi_datasets\Models\V2reportsGeneReportMatch.vb"

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
    '     File Size: 1.43 KB


    '     Class V2reportsGeneReportMatch
    ' 
    '         Properties: Errors, Gene, Product, Query, Warning
    '                     Warnings
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsGeneReportMatch.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGeneReportMatch
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsGeneReportMatch

        ''' <summary>
        ''' gene 属性
        ''' </summary>
        <Field("gene")>
        Public Property Gene As Object

        ''' <summary>
        ''' product 属性
        ''' </summary>
        <Field("product")>
        Public Property Product As Object

        ''' <summary>
        ''' query 属性
        ''' </summary>
        <Field("query")>
        Public Property Query As List(Of String)

        ''' <summary>
        ''' warnings 属性
        ''' </summary>
        <Field("warnings")>
        Public Property Warnings As List(Of Object)

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

    End Class

End Namespace

