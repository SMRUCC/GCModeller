#Region "Microsoft.VisualBasic::4456074ca8e0bac371689aea7edf64c0, ncbi_datasets\Models\Ncbiprotddv2ChainFootprint.vb"

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

    '   Total Lines: 41
    '    Code Lines: 15 (36.59%)
    ' Comment Lines: 17 (41.46%)
    '    - Xml Docs: 70.59%
    ' 
    '   Blank Lines: 9 (21.95%)
    '     File Size: 1.18 KB


    '     Class Ncbiprotddv2ChainFootprint
    ' 
    '         Properties: DependentFrom, DependentTo, QueryFrom, QueryTo
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' Ncbiprotddv2ChainFootprint.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2ChainFootprint
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2ChainFootprint

        ''' <summary>
        ''' query_from 属性
        ''' </summary>
        <Field("query_from")>
        Public Property QueryFrom As Integer?

        ''' <summary>
        ''' query_to 属性
        ''' </summary>
        <Field("query_to")>
        Public Property QueryTo As Integer?

        ''' <summary>
        ''' dependent_from 属性
        ''' </summary>
        <Field("dependent_from")>
        Public Property DependentFrom As Integer?

        ''' <summary>
        ''' dependent_to 属性
        ''' </summary>
        <Field("dependent_to")>
        Public Property DependentTo As Integer?

    End Class

End Namespace

