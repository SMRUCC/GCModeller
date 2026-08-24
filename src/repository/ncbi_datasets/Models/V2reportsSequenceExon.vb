#Region "Microsoft.VisualBasic::c12dc01a7335a3fea57ba90b30205136, ncbi_datasets\Models\V2reportsSequenceExon.vb"

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
    '     File Size: 959 B


    '     Class V2reportsSequenceExon
    ' 
    '         Properties: Begin, End, ExonNumber
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsSequenceExon.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceExon
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceExon

        ''' <summary>
        ''' exon_number 属性
        ''' </summary>
        <Field("exon_number")>
        Public Property ExonNumber As Integer?

        ''' <summary>
        ''' begin 属性
        ''' </summary>
        <Field("begin")>
        Public Property Begin As String

        ''' <summary>
        ''' end 属性
        ''' </summary>
        <Field("end")>
        Public Property End As String

    End Class

End Namespace

