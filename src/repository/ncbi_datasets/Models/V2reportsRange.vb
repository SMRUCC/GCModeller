#Region "Microsoft.VisualBasic::55c6cfaf7346844e74a0567653d8b915, ncbi_datasets\Models\V2reportsRange.vb"

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
    '     File Size: 1.26 KB


    '     Class V2reportsRange
    ' 
    '         Properties: Begin, End, Order, Orientation, RibosomalSlippage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsRange.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsRange
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsRange

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

        ''' <summary>
        ''' orientation 属性
        ''' </summary>
        <Field("orientation")>
        Public Property Orientation As Object

        ''' <summary>
        ''' order 属性
        ''' </summary>
        <Field("order")>
        Public Property Order As Integer?

        ''' <summary>
        ''' ribosomal_slippage 属性
        ''' </summary>
        <Field("ribosomal_slippage")>
        Public Property RibosomalSlippage As Integer?

    End Class

End Namespace

