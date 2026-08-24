#Region "Microsoft.VisualBasic::4255c2385ae2612c128714779d18316c, ncbi_datasets\Models\V2reportsAssemblyDataReport.vb"

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

    '   Total Lines: 101
    '    Code Lines: 35 (34.65%)
    ' Comment Lines: 47 (46.53%)
    '    - Xml Docs: 89.36%
    ' 
    '   Blank Lines: 19 (18.81%)
    '     File Size: 2.92 KB


    '     Class V2reportsAssemblyDataReport
    ' 
    '         Properties: Accession, AdditionalSubmitters, AnnotationInfo, AssemblyInfo, AssemblyStats
    '                     AverageNucleotideIdentity, CheckmInfo, CurrentAccession, OrganelleInfo, Organism
    '                     PairedAccession, SourceDatabase, TypeMaterial, WgsInfo
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsAssemblyDataReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAssemblyDataReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAssemblyDataReport

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' current_accession 属性
        ''' </summary>
        <Field("current_accession")>
        Public Property CurrentAccession As String

        ''' <summary>
        ''' paired_accession 属性
        ''' </summary>
        <Field("paired_accession")>
        Public Property PairedAccession As String

        ''' <summary>
        ''' source_database 属性
        ''' </summary>
        <Field("source_database")>
        Public Property SourceDatabase As Object

        ''' <summary>
        ''' organism 属性
        ''' </summary>
        <Field("organism")>
        Public Property Organism As Object

        ''' <summary>
        ''' assembly_info 属性
        ''' </summary>
        <Field("assembly_info")>
        Public Property AssemblyInfo As Object

        ''' <summary>
        ''' assembly_stats 属性
        ''' </summary>
        <Field("assembly_stats")>
        Public Property AssemblyStats As Object

        ''' <summary>
        ''' organelle_info 属性
        ''' </summary>
        <Field("organelle_info")>
        Public Property OrganelleInfo As List(Of Object)

        ''' <summary>
        ''' additional_submitters 属性
        ''' </summary>
        <Field("additional_submitters")>
        Public Property AdditionalSubmitters As List(Of Object)

        ''' <summary>
        ''' annotation_info 属性
        ''' </summary>
        <Field("annotation_info")>
        Public Property AnnotationInfo As Object

        ''' <summary>
        ''' wgs_info 属性
        ''' </summary>
        <Field("wgs_info")>
        Public Property WgsInfo As Object

        ''' <summary>
        ''' type_material 属性
        ''' </summary>
        <Field("type_material")>
        Public Property TypeMaterial As Object

        ''' <summary>
        ''' checkm_info 属性
        ''' </summary>
        <Field("checkm_info")>
        Public Property CheckmInfo As Object

        ''' <summary>
        ''' average_nucleotide_identity 属性
        ''' </summary>
        <Field("average_nucleotide_identity")>
        Public Property AverageNucleotideIdentity As Object

    End Class

End Namespace

