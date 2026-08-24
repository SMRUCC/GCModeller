#Region "Microsoft.VisualBasic::4f0cbbdb55be045d6fa47dac325610c2, ncbi_datasets\Models\V2DownloadSummaryAvailableFiles.vb"

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

    '   Total Lines: 71
    '    Code Lines: 25 (35.21%)
    ' Comment Lines: 32 (45.07%)
    '    - Xml Docs: 84.38%
    ' 
    '   Blank Lines: 14 (19.72%)
    '     File Size: 2.01 KB


    '     Class V2DownloadSummaryAvailableFiles
    ' 
    '         Properties: AllGenomicFasta, AnnotationReport, CdsFasta, GenomeGbff, GenomeGff
    '                     GenomeGtf, ProtFasta, RnaFasta, SequenceReport
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2DownloadSummaryAvailableFiles.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2DownloadSummaryAvailableFiles
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2DownloadSummaryAvailableFiles

        ''' <summary>
        ''' all_genomic_fasta 属性
        ''' </summary>
        <Field("all_genomic_fasta")>
        Public Property AllGenomicFasta As Object

        ''' <summary>
        ''' genome_gff 属性
        ''' </summary>
        <Field("genome_gff")>
        Public Property GenomeGff As Object

        ''' <summary>
        ''' genome_gbff 属性
        ''' </summary>
        <Field("genome_gbff")>
        Public Property GenomeGbff As Object

        ''' <summary>
        ''' rna_fasta 属性
        ''' </summary>
        <Field("rna_fasta")>
        Public Property RnaFasta As Object

        ''' <summary>
        ''' prot_fasta 属性
        ''' </summary>
        <Field("prot_fasta")>
        Public Property ProtFasta As Object

        ''' <summary>
        ''' genome_gtf 属性
        ''' </summary>
        <Field("genome_gtf")>
        Public Property GenomeGtf As Object

        ''' <summary>
        ''' cds_fasta 属性
        ''' </summary>
        <Field("cds_fasta")>
        Public Property CdsFasta As Object

        ''' <summary>
        ''' sequence_report 属性
        ''' </summary>
        <Field("sequence_report")>
        Public Property SequenceReport As Object

        ''' <summary>
        ''' annotation_report 属性
        ''' </summary>
        <Field("annotation_report")>
        Public Property AnnotationReport As Object

    End Class

End Namespace

