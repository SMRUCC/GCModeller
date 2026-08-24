#Region "Microsoft.VisualBasic::fca6e7eb9e4d99739023bbd1db02fcfa, ncbi_datasets\Models\V2VirusAnnotationFilter.vb"

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
    '     File Size: 2.47 KB


    '     Class V2VirusAnnotationFilter
    ' 
    '         Properties: Accessions, AnnotatedOnly, CompleteOnly, GeoLocation, Host
    '                     PangolinClassification, RefseqOnly, ReleasedSince, Taxon, Taxons
    '                     UpdatedSince, UsaState
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2VirusAnnotationFilter.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2VirusAnnotationFilter
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2VirusAnnotationFilter

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

    End Class

End Namespace

