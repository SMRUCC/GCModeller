#Region "Microsoft.VisualBasic::95a4d3f8fe531aaeacb067b03e520678, ncbi_datasets\Models\V2AssemblyDatasetRequest.vb"

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
    '     File Size: 1.38 KB


    '     Class V2AssemblyDatasetRequest
    ' 
    '         Properties: Accessions, Chromosomes, Hydrated, IncludeAnnotationType, IncludeTsv
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2AssemblyDatasetRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2AssemblyDatasetRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2AssemblyDatasetRequest

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <Field("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' chromosomes 属性
        ''' </summary>
        <Field("chromosomes")>
        Public Property Chromosomes As List(Of String)

        ''' <summary>
        ''' include_annotation_type 属性
        ''' </summary>
        <Field("include_annotation_type")>
        Public Property IncludeAnnotationType As List(Of Object)

        ''' <summary>
        ''' hydrated 属性
        ''' </summary>
        <Field("hydrated")>
        Public Property Hydrated As Object

        ''' <summary>
        ''' include_tsv 属性
        ''' </summary>
        <Field("include_tsv")>
        Public Property IncludeTsv As Boolean?

    End Class

End Namespace

