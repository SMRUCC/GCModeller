#Region "Microsoft.VisualBasic::85dd12989ffe36bc46de1a69de28f947, ncbi_datasets\Models\Ncbiprotddv2SimilarStructureReportPage.vb"

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

    '   Total Lines: 35
    '    Code Lines: 13 (37.14%)
    ' Comment Lines: 14 (40.00%)
    '    - Xml Docs: 64.29%
    ' 
    '   Blank Lines: 8 (22.86%)
    '     File Size: 1.09 KB


    '     Class Ncbiprotddv2SimilarStructureReportPage
    ' 
    '         Properties: NextPageToken, SimilarStructures, TotalCount
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' Ncbiprotddv2SimilarStructureReportPage.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2SimilarStructureReportPage
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2SimilarStructureReportPage

        ''' <summary>
        ''' similar_structures 属性
        ''' </summary>
        <Field("similar_structures")>
        Public Property SimilarStructures As List(Of Object)

        ''' <summary>
        ''' next_page_token 属性
        ''' </summary>
        <Field("next_page_token")>
        Public Property NextPageToken As String

        ''' <summary>
        ''' total_count 属性
        ''' </summary>
        <Field("total_count")>
        Public Property TotalCount As Integer?

    End Class

End Namespace

