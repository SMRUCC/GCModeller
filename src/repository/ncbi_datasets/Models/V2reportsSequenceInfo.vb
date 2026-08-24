#Region "Microsoft.VisualBasic::daa2ed046aafac07db64cb1fdbe9e8c0, ncbi_datasets\Models\V2reportsSequenceInfo.vb"

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

    '   Total Lines: 107
    '    Code Lines: 37 (34.58%)
    ' Comment Lines: 50 (46.73%)
    '    - Xml Docs: 90.00%
    ' 
    '   Blank Lines: 20 (18.69%)
    '     File Size: 3.04 KB


    '     Class V2reportsSequenceInfo
    ' 
    '         Properties: AssemblyAccession, AssemblyUnit, AssemblyUnplacedCount, AssignedMoleculeLocationType, ChrName
    '                     GcCount, GcPercent, GenbankAccession, Length, RefseqAccession
    '                     Role, SequenceName, SortOrder, UcscStyleName, UnlocalizedCount
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsSequenceInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceInfo

        ''' <summary>
        ''' assembly_accession 属性
        ''' </summary>
        <Field("assembly_accession")>
        Public Property AssemblyAccession As String

        ''' <summary>
        ''' chr_name 属性
        ''' </summary>
        <Field("chr_name")>
        Public Property ChrName As String

        ''' <summary>
        ''' ucsc_style_name 属性
        ''' </summary>
        <Field("ucsc_style_name")>
        Public Property UcscStyleName As String

        ''' <summary>
        ''' sort_order 属性
        ''' </summary>
        <Field("sort_order")>
        Public Property SortOrder As Integer?

        ''' <summary>
        ''' assigned_molecule_location_type 属性
        ''' </summary>
        <Field("assigned_molecule_location_type")>
        Public Property AssignedMoleculeLocationType As String

        ''' <summary>
        ''' refseq_accession 属性
        ''' </summary>
        <Field("refseq_accession")>
        Public Property RefseqAccession As String

        ''' <summary>
        ''' assembly_unit 属性
        ''' </summary>
        <Field("assembly_unit")>
        Public Property AssemblyUnit As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <Field("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' genbank_accession 属性
        ''' </summary>
        <Field("genbank_accession")>
        Public Property GenbankAccession As String

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
        ''' unlocalized_count 属性
        ''' </summary>
        <Field("unlocalized_count")>
        Public Property UnlocalizedCount As Integer?

        ''' <summary>
        ''' assembly_unplaced_count 属性
        ''' </summary>
        <Field("assembly_unplaced_count")>
        Public Property AssemblyUnplacedCount As Integer?

        ''' <summary>
        ''' role 属性
        ''' </summary>
        <Field("role")>
        Public Property Role As String

        ''' <summary>
        ''' sequence_name 属性
        ''' </summary>
        <Field("sequence_name")>
        Public Property SequenceName As String

    End Class

End Namespace

