#Region "Microsoft.VisualBasic::d2a0c33d96448425e6429b6a7d64880b, ncbi_datasets\Models\V2reportsWGSInfo.vb"

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
    '     File Size: 1.03 KB


    '     Class V2reportsWGSInfo
    ' 
    '         Properties: MasterWgsUrl, WgsContigsUrl, WgsProjectAccession
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsWGSInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsWGSInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsWGSInfo

        ''' <summary>
        ''' wgs_project_accession 属性
        ''' </summary>
        <Field("wgs_project_accession")>
        Public Property WgsProjectAccession As String

        ''' <summary>
        ''' master_wgs_url 属性
        ''' </summary>
        <Field("master_wgs_url")>
        Public Property MasterWgsUrl As String

        ''' <summary>
        ''' wgs_contigs_url 属性
        ''' </summary>
        <Field("wgs_contigs_url")>
        Public Property WgsContigsUrl As String

    End Class

End Namespace

