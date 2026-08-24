#Region "Microsoft.VisualBasic::739480e317d6460c4c8924bcd5acab47, ncbi_datasets\Models\Ncbiprotddv2StructureDataReport.vb"

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
    '     File Size: 2.52 KB


    '     Class Ncbiprotddv2StructureDataReport
    ' 
    '         Properties: AsymmetricChains, AsymmetricLigands, Chains, DepositionDate, Experiment
    '                     IsObsolete, LigandChains, MmdbId, PdbId, PublicationPmid
    '                     Title, UpdateDate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' Ncbiprotddv2StructureDataReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2StructureDataReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2StructureDataReport

        ''' <summary>
        ''' pdb_id 属性
        ''' </summary>
        <Field("pdb_id")>
        Public Property PdbId As String

        ''' <summary>
        ''' mmdb_id 属性
        ''' </summary>
        <Field("mmdb_id")>
        Public Property MmdbId As Integer?

        ''' <summary>
        ''' is_obsolete 属性
        ''' </summary>
        <Field("is_obsolete")>
        Public Property IsObsolete As Boolean?

        ''' <summary>
        ''' publication_pmid 属性
        ''' </summary>
        <Field("publication_pmid")>
        Public Property PublicationPmid As List(Of Integer)

        ''' <summary>
        ''' deposition_date 属性
        ''' </summary>
        <Field("deposition_date")>
        Public Property DepositionDate As String

        ''' <summary>
        ''' update_date 属性
        ''' </summary>
        <Field("update_date")>
        Public Property UpdateDate As String

        ''' <summary>
        ''' experiment 属性
        ''' </summary>
        <Field("experiment")>
        Public Property Experiment As Object

        ''' <summary>
        ''' chains 属性
        ''' </summary>
        <Field("chains")>
        Public Property Chains As List(Of Object)

        ''' <summary>
        ''' ligand_chains 属性
        ''' </summary>
        <Field("ligand_chains")>
        Public Property LigandChains As List(Of Object)

        ''' <summary>
        ''' asymmetric_chains 属性
        ''' </summary>
        <Field("asymmetric_chains")>
        Public Property AsymmetricChains As List(Of Object)

        ''' <summary>
        ''' asymmetric_ligands 属性
        ''' </summary>
        <Field("asymmetric_ligands")>
        Public Property AsymmetricLigands As List(Of Object)

        ''' <summary>
        ''' title 属性
        ''' </summary>
        <Field("title")>
        Public Property Title As String

    End Class

End Namespace

