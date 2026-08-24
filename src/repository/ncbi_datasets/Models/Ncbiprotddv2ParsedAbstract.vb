#Region "Microsoft.VisualBasic::21f08d887c022cccab983acaf817d5de, ncbi_datasets\Models\Ncbiprotddv2ParsedAbstract.vb"

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
    '     File Size: 1.28 KB


    '     Class Ncbiprotddv2ParsedAbstract
    ' 
    '         Properties: AbstractText, Authors, Epub, Pmid, Title
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' Ncbiprotddv2ParsedAbstract.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2ParsedAbstract
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2ParsedAbstract

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
        ''' authors 属性
        ''' </summary>
        <Field("authors")>
        Public Property Authors As List(Of Object)

        ''' <summary>
        ''' epub 属性
        ''' </summary>
        <Field("epub")>
        Public Property Epub As Object

        ''' <summary>
        ''' abstract_text 属性
        ''' </summary>
        <Field("abstract_text")>
        Public Property AbstractText As String

    End Class

End Namespace

