#Region "Microsoft.VisualBasic::ded8f5aeab3d4c7f9b6ebd2fab36be12, ncbi_datasets\Models\V2reportsAdditionalSubmitter.vb"

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
    '     File Size: 1.55 KB


    '     Class V2reportsAdditionalSubmitter
    ' 
    '         Properties: BioprojectAccession, ChrName, GenbankAccession, MoleculeType, RefseqAccession
    '                     Submitter
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsAdditionalSubmitter.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAdditionalSubmitter
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAdditionalSubmitter

        ''' <summary>
        ''' genbank_accession 属性
        ''' </summary>
        <Field("genbank_accession")>
        Public Property GenbankAccession As String

        ''' <summary>
        ''' refseq_accession 属性
        ''' </summary>
        <Field("refseq_accession")>
        Public Property RefseqAccession As String

        ''' <summary>
        ''' chr_name 属性
        ''' </summary>
        <Field("chr_name")>
        Public Property ChrName As String

        ''' <summary>
        ''' molecule_type 属性
        ''' </summary>
        <Field("molecule_type")>
        Public Property MoleculeType As String

        ''' <summary>
        ''' submitter 属性
        ''' </summary>
        <Field("submitter")>
        Public Property Submitter As String

        ''' <summary>
        ''' bioproject_accession 属性
        ''' </summary>
        <Field("bioproject_accession")>
        Public Property BioprojectAccession As String

    End Class

End Namespace

