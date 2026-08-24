#Region "Microsoft.VisualBasic::8e2259a04844baaa6c9d59c0fc8dfacc, ncbi_datasets\Models\V2reportsAssemblyStats.vb"

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

    '   Total Lines: 113
    '    Code Lines: 39 (34.51%)
    ' Comment Lines: 53 (46.90%)
    '    - Xml Docs: 90.57%
    ' 
    '   Blank Lines: 21 (18.58%)
    '     File Size: 3.36 KB


    '     Class V2reportsAssemblyStats
    ' 
    '         Properties: AtgcCount, ContigL50, ContigN50, GapsBetweenScaffoldsCount, GcCount
    '                     GcPercent, GenomeCoverage, NumberOfComponentSequences, NumberOfContigs, NumberOfOrganelles
    '                     NumberOfScaffolds, ScaffoldL50, ScaffoldN50, TotalNumberOfChromosomes, TotalSequenceLength
    '                     TotalUngappedLength
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsAssemblyStats.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAssemblyStats
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAssemblyStats

        ''' <summary>
        ''' total_number_of_chromosomes 属性
        ''' </summary>
        <Field("total_number_of_chromosomes")>
        Public Property TotalNumberOfChromosomes As Integer?

        ''' <summary>
        ''' total_sequence_length 属性
        ''' </summary>
        <Field("total_sequence_length")>
        Public Property TotalSequenceLength As String

        ''' <summary>
        ''' total_ungapped_length 属性
        ''' </summary>
        <Field("total_ungapped_length")>
        Public Property TotalUngappedLength As String

        ''' <summary>
        ''' number_of_contigs 属性
        ''' </summary>
        <Field("number_of_contigs")>
        Public Property NumberOfContigs As Integer?

        ''' <summary>
        ''' contig_n50 属性
        ''' </summary>
        <Field("contig_n50")>
        Public Property ContigN50 As Integer?

        ''' <summary>
        ''' contig_l50 属性
        ''' </summary>
        <Field("contig_l50")>
        Public Property ContigL50 As Integer?

        ''' <summary>
        ''' number_of_scaffolds 属性
        ''' </summary>
        <Field("number_of_scaffolds")>
        Public Property NumberOfScaffolds As Integer?

        ''' <summary>
        ''' scaffold_n50 属性
        ''' </summary>
        <Field("scaffold_n50")>
        Public Property ScaffoldN50 As Integer?

        ''' <summary>
        ''' scaffold_l50 属性
        ''' </summary>
        <Field("scaffold_l50")>
        Public Property ScaffoldL50 As Integer?

        ''' <summary>
        ''' gaps_between_scaffolds_count 属性
        ''' </summary>
        <Field("gaps_between_scaffolds_count")>
        Public Property GapsBetweenScaffoldsCount As Integer?

        ''' <summary>
        ''' number_of_component_sequences 属性
        ''' </summary>
        <Field("number_of_component_sequences")>
        Public Property NumberOfComponentSequences As Integer?

        ''' <summary>
        ''' atgc_count 属性
        ''' </summary>
        <Field("atgc_count")>
        Public Property AtgcCount As String

        ''' <summary>
        ''' gc_count 属性
        ''' </summary>
        <Field("gc_count")>
        Public Property GcCount As String

        ''' <summary>
        ''' gc_percent 属性
        ''' </summary>
        <Field("gc_percent")>
        Public Property GcPercent As Single?

        ''' <summary>
        ''' genome_coverage 属性
        ''' </summary>
        <Field("genome_coverage")>
        Public Property GenomeCoverage As String

        ''' <summary>
        ''' number_of_organelles 属性
        ''' </summary>
        <Field("number_of_organelles")>
        Public Property NumberOfOrganelles As Integer?

    End Class

End Namespace

