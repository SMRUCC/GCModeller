#Region "Microsoft.VisualBasic::f2234e33a3cedf05b88945e612896027, ncbi_datasets\Models\Ncbiprotddv2SimilarStructureRequest.vb"

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


    '     Class Ncbiprotddv2SimilarStructureRequest
    ' 
    '         Properties: HitsPerPage, PageToken, RedundancyLevel, Sdid, SortBy
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' Ncbiprotddv2SimilarStructureRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2SimilarStructureRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2SimilarStructureRequest

        ''' <summary>
        ''' sdid 属性
        ''' </summary>
        <Field("sdid")>
        Public Property Sdid As String

        ''' <summary>
        ''' page_token 属性
        ''' </summary>
        <Field("page_token")>
        Public Property PageToken As String

        ''' <summary>
        ''' redundancy_level 属性
        ''' </summary>
        <Field("redundancy_level")>
        Public Property RedundancyLevel As Object

        ''' <summary>
        ''' sort_by 属性
        ''' </summary>
        <Field("sort_by")>
        Public Property SortBy As Object

        ''' <summary>
        ''' hits_per_page 属性
        ''' </summary>
        <Field("hits_per_page")>
        Public Property HitsPerPage As Integer?

    End Class

End Namespace

