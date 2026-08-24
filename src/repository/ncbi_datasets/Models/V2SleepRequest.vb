#Region "Microsoft.VisualBasic::c2155ba9775d906ee3200b67e604144e, ncbi_datasets\Models\V2SleepRequest.vb"

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

    '   Total Lines: 29
    '    Code Lines: 11 (37.93%)
    ' Comment Lines: 11 (37.93%)
    '    - Xml Docs: 54.55%
    ' 
    '   Blank Lines: 7 (24.14%)
    '     File Size: 814 B


    '     Class V2SleepRequest
    ' 
    '         Properties: ErrorRate, SleepMsec
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2SleepRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2SleepRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2SleepRequest

        ''' <summary>
        ''' sleep_msec 属性
        ''' </summary>
        <Field("sleep_msec")>
        Public Property SleepMsec As Integer?

        ''' <summary>
        ''' error_rate 属性
        ''' </summary>
        <Field("error_rate")>
        Public Property ErrorRate As Single?

    End Class

End Namespace

