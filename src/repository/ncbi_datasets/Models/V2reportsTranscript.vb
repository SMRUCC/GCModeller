#Region "Microsoft.VisualBasic::edd52e372697e278a0860192885657db, ncbi_datasets\Models\V2reportsTranscript.vb"

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

    '   Total Lines: 71
    '    Code Lines: 25 (35.21%)
    ' Comment Lines: 32 (45.07%)
    '    - Xml Docs: 84.38%
    ' 
    '   Blank Lines: 14 (19.72%)
    '     File Size: 1.94 KB


    '     Class V2reportsTranscript
    ' 
    '         Properties: AccessionVersion, Cds, EnsemblTranscript, GenomicLocations, Length
    '                     Name, Protein, SelectCategory, Type
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsTranscript.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsTranscript
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsTranscript

        ''' <summary>
        ''' accession_version 属性
        ''' </summary>
        <Field("accession_version")>
        Public Property AccessionVersion As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <Field("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' cds 属性
        ''' </summary>
        <Field("cds")>
        Public Property Cds As Object

        ''' <summary>
        ''' genomic_locations 属性
        ''' </summary>
        <Field("genomic_locations")>
        Public Property GenomicLocations As List(Of Object)

        ''' <summary>
        ''' ensembl_transcript 属性
        ''' </summary>
        <Field("ensembl_transcript")>
        Public Property EnsemblTranscript As String

        ''' <summary>
        ''' protein 属性
        ''' </summary>
        <Field("protein")>
        Public Property Protein As Object

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <Field("type")>
        Public Property Type As Object

        ''' <summary>
        ''' select_category 属性
        ''' </summary>
        <Field("select_category")>
        Public Property SelectCategory As Object

    End Class

End Namespace

