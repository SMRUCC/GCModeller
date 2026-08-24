#Region "Microsoft.VisualBasic::82456ba7b85c036caedad0129b051ed2, ncbi_datasets\Models\V2reportsPublication.vb"

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
    '     File Size: 1.10 KB


    '     Class V2reportsPublication
    ' 
    '         Properties: Authors, Date, Pmid, Title
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsPublication.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsPublication
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsPublication

        ''' <summary>
        ''' pmid 属性
        ''' </summary>
        <Field("pmid")>
        Public Property Pmid As Integer?

        ''' <summary>
        ''' title 属性
        ''' </summary>
        <Field("title")>
        Public Property Title As String

        ''' <summary>
        ''' date 属性
        ''' </summary>
        <Field("date")>
        Public Property Date As String

        ''' <summary>
        ''' authors 属性
        ''' </summary>
        <Field("authors")>
        Public Property Authors As List(Of Object)

    End Class

End Namespace

