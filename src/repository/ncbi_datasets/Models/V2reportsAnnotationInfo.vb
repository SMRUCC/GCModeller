#Region "Microsoft.VisualBasic::31e899d1c65d74d92e57866426166969, ncbi_datasets\Models\V2reportsAnnotationInfo.vb"

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

    '   Total Lines: 83
    '    Code Lines: 29 (34.94%)
    ' Comment Lines: 38 (45.78%)
    '    - Xml Docs: 86.84%
    ' 
    '   Blank Lines: 16 (19.28%)
    '     File Size: 2.20 KB


    '     Class V2reportsAnnotationInfo
    ' 
    '         Properties: Busco, Method, Name, Pipeline, Provider
    '                     ReleaseDate, ReleaseVersion, ReportUrl, SoftwareVersion, Stats
    '                     Status
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsAnnotationInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAnnotationInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAnnotationInfo

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' provider 属性
        ''' </summary>
        <Field("provider")>
        Public Property Provider As String

        ''' <summary>
        ''' release_date 属性
        ''' </summary>
        <Field("release_date")>
        Public Property ReleaseDate As String

        ''' <summary>
        ''' report_url 属性
        ''' </summary>
        <Field("report_url")>
        Public Property ReportUrl As String

        ''' <summary>
        ''' stats 属性
        ''' </summary>
        <Field("stats")>
        Public Property Stats As Object

        ''' <summary>
        ''' busco 属性
        ''' </summary>
        <Field("busco")>
        Public Property Busco As Object

        ''' <summary>
        ''' method 属性
        ''' </summary>
        <Field("method")>
        Public Property Method As String

        ''' <summary>
        ''' pipeline 属性
        ''' </summary>
        <Field("pipeline")>
        Public Property Pipeline As String

        ''' <summary>
        ''' software_version 属性
        ''' </summary>
        <Field("software_version")>
        Public Property SoftwareVersion As String

        ''' <summary>
        ''' status 属性
        ''' </summary>
        <Field("status")>
        Public Property Status As String

        ''' <summary>
        ''' release_version 属性
        ''' </summary>
        <Field("release_version")>
        Public Property ReleaseVersion As String

    End Class

End Namespace

