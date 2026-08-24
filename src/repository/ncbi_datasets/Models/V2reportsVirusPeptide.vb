#Region "Microsoft.VisualBasic::076435cfa4ac3c19f17f280bc0ee1a70, ncbi_datasets\Models\V2reportsVirusPeptide.vb"

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

    '   Total Lines: 77
    '    Code Lines: 27 (35.06%)
    ' Comment Lines: 35 (45.45%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 15 (19.48%)
    '     File Size: 2.11 KB


    '     Class V2reportsVirusPeptide
    ' 
    '         Properties: Accession, Cdd, MaturePeptide, Name, Nucleotide
    '                     OtherNames, PdbIds, Protein, ProteinCompleteness, UniProtKb
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsVirusPeptide.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsVirusPeptide
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsVirusPeptide

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' other_names 属性
        ''' </summary>
        <Field("other_names")>
        Public Property OtherNames As List(Of String)

        ''' <summary>
        ''' nucleotide 属性
        ''' </summary>
        <Field("nucleotide")>
        Public Property Nucleotide As Object

        ''' <summary>
        ''' protein 属性
        ''' </summary>
        <Field("protein")>
        Public Property Protein As Object

        ''' <summary>
        ''' pdb_ids 属性
        ''' </summary>
        <Field("pdb_ids")>
        Public Property PdbIds As List(Of String)

        ''' <summary>
        ''' cdd 属性
        ''' </summary>
        <Field("cdd")>
        Public Property Cdd As List(Of Object)

        ''' <summary>
        ''' uni_prot_kb 属性
        ''' </summary>
        <Field("uni_prot_kb")>
        Public Property UniProtKb As Object

        ''' <summary>
        ''' mature_peptide 属性
        ''' </summary>
        <Field("mature_peptide")>
        Public Property MaturePeptide As List(Of Object)

        ''' <summary>
        ''' protein_completeness 属性
        ''' </summary>
        <Field("protein_completeness")>
        Public Property ProteinCompleteness As Object

    End Class

End Namespace

