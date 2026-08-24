#Region "Microsoft.VisualBasic::f6a6e6cdfd2d886f3ea88b5cba64e7e2, ncbi_datasets\Models\V2VirusAvailability.vb"

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


    '     Class V2VirusAvailability
    ' 
    '         Properties: InvalidAccessions, Message, ValidAccessions
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2VirusAvailability.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2VirusAvailability
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2VirusAvailability

        ''' <summary>
        ''' valid_accessions 属性
        ''' </summary>
        <Field("valid_accessions")>
        Public Property ValidAccessions As List(Of String)

        ''' <summary>
        ''' invalid_accessions 属性
        ''' </summary>
        <Field("invalid_accessions")>
        Public Property InvalidAccessions As List(Of String)

        ''' <summary>
        ''' message 属性
        ''' </summary>
        <Field("message")>
        Public Property Message As String

    End Class

End Namespace

