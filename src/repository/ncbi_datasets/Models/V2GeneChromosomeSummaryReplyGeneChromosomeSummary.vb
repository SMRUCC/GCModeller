#Region "Microsoft.VisualBasic::ed7436d176f2c7078f5f5f4c07a2f9c8, ncbi_datasets\Models\V2GeneChromosomeSummaryReplyGeneChromosomeSummary.vb"

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
    '     File Size: 1.04 KB


    '     Class V2GeneChromosomeSummaryReplyGeneChromosomeSummary
    ' 
    '         Properties: Accession, Count, Name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2GeneChromosomeSummaryReplyGeneChromosomeSummary.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneChromosomeSummaryReplyGeneChromosomeSummary
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2GeneChromosomeSummaryReplyGeneChromosomeSummary

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' count 属性
        ''' </summary>
        <Field("count")>
        Public Property Count As Integer?

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

    End Class

End Namespace

