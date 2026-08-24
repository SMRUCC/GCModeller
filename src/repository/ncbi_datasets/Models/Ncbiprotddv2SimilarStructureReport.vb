#Region "Microsoft.VisualBasic::086747cc03f63c9940fe75d9a87e7aa6, ncbi_datasets\Models\Ncbiprotddv2SimilarStructureReport.vb"

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
    '     File Size: 2.45 KB


    '     Class Ncbiprotddv2SimilarStructureReport
    ' 
    '         Properties: AlignId, ChainId, DomainNumber, Footprints, MmdbId
    '                     PdbId, ProteinChainName, Sdid, StructureTitle, SuperkingdomId
    '                     TaxId, VastScore
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' Ncbiprotddv2SimilarStructureReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2SimilarStructureReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2SimilarStructureReport

        ''' <summary>
        ''' sdid 属性
        ''' </summary>
        <Field("sdid")>
        Public Property Sdid As Integer?

        ''' <summary>
        ''' structure_title 属性
        ''' </summary>
        <Field("structure_title")>
        Public Property StructureTitle As String

        ''' <summary>
        ''' protein_chain_name 属性
        ''' </summary>
        <Field("protein_chain_name")>
        Public Property ProteinChainName As String

        ''' <summary>
        ''' chain_id 属性
        ''' </summary>
        <Field("chain_id")>
        Public Property ChainId As String

        ''' <summary>
        ''' domain_number 属性
        ''' </summary>
        <Field("domain_number")>
        Public Property DomainNumber As Integer?

        ''' <summary>
        ''' mmdb_id 属性
        ''' </summary>
        <Field("mmdb_id")>
        Public Property MmdbId As Integer?

        ''' <summary>
        ''' pdb_id 属性
        ''' </summary>
        <Field("pdb_id")>
        Public Property PdbId As String

        ''' <summary>
        ''' vast_score 属性
        ''' </summary>
        <Field("vast_score")>
        Public Property VastScore As Object

        ''' <summary>
        ''' align_id 属性
        ''' </summary>
        <Field("align_id")>
        Public Property AlignId As Integer?

        ''' <summary>
        ''' superkingdom_id 属性
        ''' </summary>
        <Field("superkingdom_id")>
        Public Property SuperkingdomId As Integer?

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' footprints 属性
        ''' </summary>
        <Field("footprints")>
        Public Property Footprints As List(Of Object)

    End Class

End Namespace

