#Region "Microsoft.VisualBasic::09e630747cd6e8a5512ae3260439a5cd, ncbi_datasets\Models\V2BioSampleDatasetReportsRequest.vb"

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

    '   Total Lines: 23
    '    Code Lines: 9 (39.13%)
    ' Comment Lines: 8 (34.78%)
    '    - Xml Docs: 37.50%
    ' 
    '   Blank Lines: 6 (26.09%)
    '     File Size: 719 B


    '     Class V2BioSampleDatasetReportsRequest
    ' 
    '         Properties: Accessions
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2BioSampleDatasetReportsRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2BioSampleDatasetReportsRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2BioSampleDatasetReportsRequest

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <Field("accessions")>
        Public Property Accessions As List(Of String)

    End Class

End Namespace

