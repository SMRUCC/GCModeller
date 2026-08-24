#Region "Microsoft.VisualBasic::0101e3f06729d6c4f5a14611fdfc23ac, ncbi_datasets\Models\V2reportsGeneCounts.vb"

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
    '     File Size: 1.29 KB


    '     Class V2reportsGeneCounts
    ' 
    '         Properties: NonCoding, Other, ProteinCoding, Pseudogene, Total
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsGeneCounts.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGeneCounts
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsGeneCounts

        ''' <summary>
        ''' total 属性
        ''' </summary>
        <Field("total")>
        Public Property Total As Integer?

        ''' <summary>
        ''' protein_coding 属性
        ''' </summary>
        <Field("protein_coding")>
        Public Property ProteinCoding As Integer?

        ''' <summary>
        ''' non_coding 属性
        ''' </summary>
        <Field("non_coding")>
        Public Property NonCoding As Integer?

        ''' <summary>
        ''' pseudogene 属性
        ''' </summary>
        <Field("pseudogene")>
        Public Property Pseudogene As Integer?

        ''' <summary>
        ''' other 属性
        ''' </summary>
        <Field("other")>
        Public Property Other As Integer?

    End Class

End Namespace

