#Region "Microsoft.VisualBasic::76c2b7161e7dba8ae342cfee3d0f89be, ncbi_datasets\Models\V2VirusDatasetRequest.vb"

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
    '     File Size: 3.28 KB


    '     Class V2VirusDatasetRequest
    ' 
    '         Properties: Accessions, AnnotatedOnly, AuxReport, CompleteOnly, Format
    '                     GeoLocation, Host, IncludeSequence, PangolinClassification, RefseqOnly
    '                     ReleasedSince, TableFields, Taxon, Taxons, UpdatedSince
    '                     UsaState, UsePsg
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2VirusDatasetRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2VirusDatasetRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2VirusDatasetRequest

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <Field("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' taxon 属性
        ''' </summary>
        <Field("taxon")>
        Public Property Taxon As String

        ''' <summary>
        ''' taxons 属性
        ''' </summary>
        <Field("taxons")>
        Public Property Taxons As List(Of String)

        ''' <summary>
        ''' refseq_only 属性
        ''' </summary>
        <Field("refseq_only")>
        Public Property RefseqOnly As Boolean?

        ''' <summary>
        ''' annotated_only 属性
        ''' </summary>
        <Field("annotated_only")>
        Public Property AnnotatedOnly As Boolean?

        ''' <summary>
        ''' released_since 属性
        ''' </summary>
        <Field("released_since")>
        Public Property ReleasedSince As DateTime?

        ''' <summary>
        ''' updated_since 属性
        ''' </summary>
        <Field("updated_since")>
        Public Property UpdatedSince As DateTime?

        ''' <summary>
        ''' host 属性
        ''' </summary>
        <Field("host")>
        Public Property Host As String

        ''' <summary>
        ''' pangolin_classification 属性
        ''' </summary>
        <Field("pangolin_classification")>
        Public Property PangolinClassification As String

        ''' <summary>
        ''' geo_location 属性
        ''' </summary>
        <Field("geo_location")>
        Public Property GeoLocation As String

        ''' <summary>
        ''' usa_state 属性
        ''' </summary>
        <Field("usa_state")>
        Public Property UsaState As String

        ''' <summary>
        ''' complete_only 属性
        ''' </summary>
        <Field("complete_only")>
        Public Property CompleteOnly As Boolean?

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <Field("table_fields")>
        Public Property TableFields As List(Of Object)

        ''' <summary>
        ''' include_sequence 属性
        ''' </summary>
        <Field("include_sequence")>
        Public Property IncludeSequence As List(Of Object)

        ''' <summary>
        ''' aux_report 属性
        ''' </summary>
        <Field("aux_report")>
        Public Property AuxReport As List(Of Object)

        ''' <summary>
        ''' format 属性
        ''' </summary>
        <Field("format")>
        Public Property Format As Object

        ''' <summary>
        ''' use_psg 属性
        ''' </summary>
        <Field("use_psg")>
        Public Property UsePsg As Boolean?

    End Class

End Namespace

