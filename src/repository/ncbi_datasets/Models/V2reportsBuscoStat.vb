#Region "Microsoft.VisualBasic::df93d9c0ca656770fb212c7ef6dacb3a, ncbi_datasets\Models\V2reportsBuscoStat.vb"

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

    '   Total Lines: 65
    '    Code Lines: 23 (35.38%)
    ' Comment Lines: 29 (44.62%)
    '    - Xml Docs: 82.76%
    ' 
    '   Blank Lines: 13 (20.00%)
    '     File Size: 1.76 KB


    '     Class V2reportsBuscoStat
    ' 
    '         Properties: BuscoLineage, BuscoVer, Complete, Duplicated, Fragmented
    '                     Missing, SingleCopy, TotalCount
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsBuscoStat.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBuscoStat
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsBuscoStat

        ''' <summary>
        ''' busco_lineage 属性
        ''' </summary>
        <Field("busco_lineage")>
        Public Property BuscoLineage As String

        ''' <summary>
        ''' busco_ver 属性
        ''' </summary>
        <Field("busco_ver")>
        Public Property BuscoVer As String

        ''' <summary>
        ''' complete 属性
        ''' </summary>
        <Field("complete")>
        Public Property Complete As Single?

        ''' <summary>
        ''' single_copy 属性
        ''' </summary>
        <Field("single_copy")>
        Public Property SingleCopy As Single?

        ''' <summary>
        ''' duplicated 属性
        ''' </summary>
        <Field("duplicated")>
        Public Property Duplicated As Single?

        ''' <summary>
        ''' fragmented 属性
        ''' </summary>
        <Field("fragmented")>
        Public Property Fragmented As Single?

        ''' <summary>
        ''' missing 属性
        ''' </summary>
        <Field("missing")>
        Public Property Missing As Single?

        ''' <summary>
        ''' total_count 属性
        ''' </summary>
        <Field("total_count")>
        Public Property TotalCount As String

    End Class

End Namespace

