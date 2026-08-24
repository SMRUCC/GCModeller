#Region "Microsoft.VisualBasic::94d408d78ce76433f2e3e1c875dc499f, ncbi_datasets\Models\V2reportsGeneSummary.vb"

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
    '     File Size: 961 B


    '     Class V2reportsGeneSummary
    ' 
    '         Properties: Date, Description, Source
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsGeneSummary.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGeneSummary
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsGeneSummary

        ''' <summary>
        ''' source 属性
        ''' </summary>
        <Field("source")>
        Public Property Source As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' date 属性
        ''' </summary>
        <Field("date")>
        Public Property Date As String

    End Class

End Namespace

