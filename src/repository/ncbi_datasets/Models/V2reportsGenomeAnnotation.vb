#Region "Microsoft.VisualBasic::118e499250dada203c724aa3bdc01d39, ncbi_datasets\Models\V2reportsGenomeAnnotation.vb"

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

    '   Total Lines: 149
    '    Code Lines: 51 (34.23%)
    ' Comment Lines: 71 (47.65%)
    '    - Xml Docs: 92.96%
    ' 
    '   Blank Lines: 27 (18.12%)
    '     File Size: 4.07 KB


    '     Class V2reportsGenomeAnnotation
    ' 
    '         Properties: Annotations, Chromosomes, CommonName, Description, EnsemblGeneIds
    '                     GeneId, GeneType, GenomicRegions, LocusTag, Name
    '                     OmimIds, Orientation, Proteins, ReferenceStandards, RnaType
    '                     SwissProtAccessions, Symbol, Synonyms, TaxId, Taxname
    '                     Transcripts, Type
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsGenomeAnnotation.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGenomeAnnotation
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsGenomeAnnotation

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
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

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
        ''' gene_type 属性
        ''' </summary>
        <Field("gene_type")>
        Public Property GeneType As String

        ''' <summary>
        ''' rna_type 属性
        ''' </summary>
        <Field("rna_type")>
        Public Property RnaType As Object

        ''' <summary>
        ''' orientation 属性
        ''' </summary>
        <Field("orientation")>
        Public Property Orientation As Object

        ''' <summary>
        ''' locus_tag 属性
        ''' </summary>
        <Field("locus_tag")>
        Public Property LocusTag As String

        ''' <summary>
        ''' reference_standards 属性
        ''' </summary>
        <Field("reference_standards")>
        Public Property ReferenceStandards As List(Of Object)

        ''' <summary>
        ''' genomic_regions 属性
        ''' </summary>
        <Field("genomic_regions")>
        Public Property GenomicRegions As List(Of Object)

        ''' <summary>
        ''' transcripts 属性
        ''' </summary>
        <Field("transcripts")>
        Public Property Transcripts As List(Of Object)

        ''' <summary>
        ''' proteins 属性
        ''' </summary>
        <Field("proteins")>
        Public Property Proteins As List(Of Object)

        ''' <summary>
        ''' chromosomes 属性
        ''' </summary>
        <Field("chromosomes")>
        Public Property Chromosomes As List(Of String)

        ''' <summary>
        ''' swiss_prot_accessions 属性
        ''' </summary>
        <Field("swiss_prot_accessions")>
        Public Property SwissProtAccessions As List(Of String)

        ''' <summary>
        ''' ensembl_gene_ids 属性
        ''' </summary>
        <Field("ensembl_gene_ids")>
        Public Property EnsemblGeneIds As List(Of String)

        ''' <summary>
        ''' omim_ids 属性
        ''' </summary>
        <Field("omim_ids")>
        Public Property OmimIds As List(Of String)

        ''' <summary>
        ''' synonyms 属性
        ''' </summary>
        <Field("synonyms")>
        Public Property Synonyms As List(Of String)

        ''' <summary>
        ''' annotations 属性
        ''' </summary>
        <Field("annotations")>
        Public Property Annotations As List(Of Object)

    End Class

End Namespace

