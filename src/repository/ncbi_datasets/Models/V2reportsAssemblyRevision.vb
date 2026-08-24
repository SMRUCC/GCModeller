#Region "Microsoft.VisualBasic::4e7f7272d2f0f3b0506224c7895559ed, ncbi_datasets\Models\V2reportsAssemblyRevision.vb"

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

    '   Total Lines: 65
    '    Code Lines: 23 (35.38%)
    ' Comment Lines: 29 (44.62%)
    '    - Xml Docs: 82.76%
    ' 
    '   Blank Lines: 13 (20.00%)
    '     File Size: 1.89 KB


    '     Class V2reportsAssemblyRevision
    ' 
    '         Properties: AssemblyLevel, AssemblyName, GenbankAccession, Identical, RefseqAccession
    '                     ReleaseDate, SequencingTechnology, SubmissionDate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsAssemblyRevision.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAssemblyRevision
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAssemblyRevision

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
        ''' assembly_name 属性
        ''' </summary>
        <Field("assembly_name")>
        Public Property AssemblyName As String

        ''' <summary>
        ''' assembly_level 属性
        ''' </summary>
        <Field("assembly_level")>
        Public Property AssemblyLevel As Object

        ''' <summary>
        ''' release_date 属性
        ''' </summary>
        <Field("release_date")>
        Public Property ReleaseDate As String

        ''' <summary>
        ''' submission_date 属性
        ''' </summary>
        <Field("submission_date")>
        Public Property SubmissionDate As String

        ''' <summary>
        ''' sequencing_technology 属性
        ''' </summary>
        <Field("sequencing_technology")>
        Public Property SequencingTechnology As String

        ''' <summary>
        ''' identical 属性
        ''' </summary>
        <Field("identical")>
        Public Property Identical As Boolean?

    End Class

End Namespace

