#Region "Microsoft.VisualBasic::00226d137ac2023844a6ebe6a81cb021, ncbi_datasets\Models\V2TaxonomyNode.vb"

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

    '   Total Lines: 119
    '    Code Lines: 41 (34.45%)
    ' Comment Lines: 56 (47.06%)
    '    - Xml Docs: 91.07%
    ' 
    '   Blank Lines: 22 (18.49%)
    '     File Size: 3.33 KB


    '     Class V2TaxonomyNode
    ' 
    '         Properties: Acronyms, BlastName, Children, CommonName, Counts
    '                     DescendentWithDescribedSpeciesNamesCount, Extinct, GenbankAcronym, GenbankCommonName, GenomicMoltype
    '                     HasDescribedSpeciesName, Lineage, MaxOrd, MinOrd, OrganismName
    '                     Rank, TaxId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2TaxonomyNode.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyNode
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyNode

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' organism_name 属性
        ''' </summary>
        <Field("organism_name")>
        Public Property OrganismName As String

        ''' <summary>
        ''' common_name 属性
        ''' </summary>
        <Field("common_name")>
        Public Property CommonName As String

        ''' <summary>
        ''' genbank_common_name 属性
        ''' </summary>
        <Field("genbank_common_name")>
        Public Property GenbankCommonName As String

        ''' <summary>
        ''' acronyms 属性
        ''' </summary>
        <Field("acronyms")>
        Public Property Acronyms As List(Of String)

        ''' <summary>
        ''' genbank_acronym 属性
        ''' </summary>
        <Field("genbank_acronym")>
        Public Property GenbankAcronym As String

        ''' <summary>
        ''' blast_name 属性
        ''' </summary>
        <Field("blast_name")>
        Public Property BlastName As String

        ''' <summary>
        ''' lineage 属性
        ''' </summary>
        <Field("lineage")>
        Public Property Lineage As List(Of Integer)

        ''' <summary>
        ''' children 属性
        ''' </summary>
        <Field("children")>
        Public Property Children As List(Of Integer)

        ''' <summary>
        ''' descendent_with_described_species_names_count 属性
        ''' </summary>
        <Field("descendent_with_described_species_names_count")>
        Public Property DescendentWithDescribedSpeciesNamesCount As Integer?

        ''' <summary>
        ''' rank 属性
        ''' </summary>
        <Field("rank")>
        Public Property Rank As Object

        ''' <summary>
        ''' has_described_species_name 属性
        ''' </summary>
        <Field("has_described_species_name")>
        Public Property HasDescribedSpeciesName As Boolean?

        ''' <summary>
        ''' counts 属性
        ''' </summary>
        <Field("counts")>
        Public Property Counts As List(Of Object)

        ''' <summary>
        ''' min_ord 属性
        ''' </summary>
        <Field("min_ord")>
        Public Property MinOrd As Integer?

        ''' <summary>
        ''' max_ord 属性
        ''' </summary>
        <Field("max_ord")>
        Public Property MaxOrd As Integer?

        ''' <summary>
        ''' extinct 属性
        ''' </summary>
        <Field("extinct")>
        Public Property Extinct As Boolean?

        ''' <summary>
        ''' genomic_moltype 属性
        ''' </summary>
        <Field("genomic_moltype")>
        Public Property GenomicMoltype As String

    End Class

End Namespace

