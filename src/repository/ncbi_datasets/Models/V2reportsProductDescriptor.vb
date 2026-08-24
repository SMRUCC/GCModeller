#Region "Microsoft.VisualBasic::0fec052d5dda57789bf095108a74afd2, ncbi_datasets\Models\V2reportsProductDescriptor.vb"

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

    '   Total Lines: 89
    '    Code Lines: 31 (34.83%)
    ' Comment Lines: 41 (46.07%)
    '    - Xml Docs: 87.80%
    ' 
    '   Blank Lines: 17 (19.10%)
    '     File Size: 2.44 KB


    '     Class V2reportsProductDescriptor
    ' 
    '         Properties: CommonName, Description, GeneId, ProteinCount, RnaType
    '                     Symbol, TaxId, Taxname, TranscriptCount, Transcripts
    '                     TranscriptTypeCounts, Type
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsProductDescriptor.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProductDescriptor
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProductDescriptor

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <Field("gene_id")>
        Public Property GeneId As String

        ''' <summary>
        ''' symbol 属性
        ''' </summary>
        <Field("symbol")>
        Public Property Symbol As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' taxname 属性
        ''' </summary>
        <Field("taxname")>
        Public Property Taxname As String

        ''' <summary>
        ''' common_name 属性
        ''' </summary>
        <Field("common_name")>
        Public Property CommonName As String

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <Field("type")>
        Public Property Type As Object

        ''' <summary>
        ''' rna_type 属性
        ''' </summary>
        <Field("rna_type")>
        Public Property RnaType As Object

        ''' <summary>
        ''' transcripts 属性
        ''' </summary>
        <Field("transcripts")>
        Public Property Transcripts As List(Of Object)

        ''' <summary>
        ''' transcript_count 属性
        ''' </summary>
        <Field("transcript_count")>
        Public Property TranscriptCount As Integer?

        ''' <summary>
        ''' protein_count 属性
        ''' </summary>
        <Field("protein_count")>
        Public Property ProteinCount As Integer?

        ''' <summary>
        ''' transcript_type_counts 属性
        ''' </summary>
        <Field("transcript_type_counts")>
        Public Property TranscriptTypeCounts As List(Of Object)

    End Class

End Namespace

