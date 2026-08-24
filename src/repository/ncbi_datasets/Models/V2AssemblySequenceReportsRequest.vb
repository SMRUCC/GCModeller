#Region "Microsoft.VisualBasic::6a388172d2b0ac8cdfeea8d32f677690, ncbi_datasets\Models\V2AssemblySequenceReportsRequest.vb"

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

    '   Total Lines: 71
    '    Code Lines: 25 (35.21%)
    ' Comment Lines: 32 (45.07%)
    '    - Xml Docs: 84.38%
    ' 
    '   Blank Lines: 14 (19.72%)
    '     File Size: 2.08 KB


    '     Class V2AssemblySequenceReportsRequest
    ' 
    '         Properties: Accession, Chromosomes, CountAssemblyUnplaced, IncludeTabularHeader, PageSize
    '                     PageToken, RoleFilters, TableFields, TableFormat
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2AssemblySequenceReportsRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2AssemblySequenceReportsRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2AssemblySequenceReportsRequest

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' chromosomes 属性
        ''' </summary>
        <Field("chromosomes")>
        Public Property Chromosomes As List(Of String)

        ''' <summary>
        ''' role_filters 属性
        ''' </summary>
        <Field("role_filters")>
        Public Property RoleFilters As List(Of String)

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <Field("table_fields")>
        Public Property TableFields As List(Of String)

        ''' <summary>
        ''' count_assembly_unplaced 属性
        ''' </summary>
        <Field("count_assembly_unplaced")>
        Public Property CountAssemblyUnplaced As Boolean?

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

        ''' <summary>
        ''' include_tabular_header 属性
        ''' </summary>
        <Field("include_tabular_header")>
        Public Property IncludeTabularHeader As Object

        ''' <summary>
        ''' table_format 属性
        ''' </summary>
        <Field("table_format")>
        Public Property TableFormat As String

    End Class

End Namespace

