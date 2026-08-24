#Region "Microsoft.VisualBasic::c71641cd60568421fcbf1f9c285785e2, ncbi_datasets\Models\V2reportsSequenceGeneContext.vb"

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

    '   Total Lines: 53
    '    Code Lines: 19 (35.85%)
    ' Comment Lines: 23 (43.40%)
    '    - Xml Docs: 78.26%
    ' 
    '   Blank Lines: 11 (20.75%)
    '     File Size: 1.52 KB


    '     Class V2reportsSequenceGeneContext
    ' 
    '         Properties: Exons, GeneId, GeneSymbol, GenomicLocation, RefseqSelectCategory
    '                     SelectCategory
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsSequenceGeneContext.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceGeneContext
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceGeneContext

        ''' <summary>
        ''' gene_symbol 属性
        ''' </summary>
        <Field("gene_symbol")>
        Public Property GeneSymbol As String

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <Field("gene_id")>
        Public Property GeneId As String

        ''' <summary>
        ''' genomic_location 属性
        ''' </summary>
        <Field("genomic_location")>
        Public Property GenomicLocation As Object

        ''' <summary>
        ''' exons 属性
        ''' </summary>
        <Field("exons")>
        Public Property Exons As Object

        ''' <summary>
        ''' select_category 属性
        ''' </summary>
        <Field("select_category")>
        Public Property SelectCategory As String

        ''' <summary>
        ''' refseq_select_category 属性
        ''' </summary>
        <Field("refseq_select_category")>
        Public Property RefseqSelectCategory As String

    End Class

End Namespace

