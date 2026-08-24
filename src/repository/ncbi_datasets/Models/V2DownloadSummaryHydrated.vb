#Region "Microsoft.VisualBasic::c86823566031beeec3a64a4b374b031e, ncbi_datasets\Models\V2DownloadSummaryHydrated.vb"

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
    '     File Size: 1.06 KB


    '     Class V2DownloadSummaryHydrated
    ' 
    '         Properties: CliDownloadCommandLine, EstimatedFileSizeMb, Url
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2DownloadSummaryHydrated.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2DownloadSummaryHydrated
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2DownloadSummaryHydrated

        ''' <summary>
        ''' estimated_file_size_mb 属性
        ''' </summary>
        <Field("estimated_file_size_mb")>
        Public Property EstimatedFileSizeMb As Integer?

        ''' <summary>
        ''' url 属性
        ''' </summary>
        <Field("url")>
        Public Property Url As String

        ''' <summary>
        ''' cli_download_command_line 属性
        ''' </summary>
        <Field("cli_download_command_line")>
        Public Property CliDownloadCommandLine As String

    End Class

End Namespace

